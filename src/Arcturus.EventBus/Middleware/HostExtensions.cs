﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Arcturus.EventBus.Middleware;

public static class HostExtensions
{
    private const string _invokeMethodName = "Invoke";
    private const string _invokeAsyncMethodName = "InvokeAsync";
    private readonly static List<Func<RequestDelegate, RequestDelegate>> _components = [];
    private readonly static MethodInfo _getServiceInfo
        = typeof(HostExtensions).GetMethod(nameof(GetService), BindingFlags.NonPublic | BindingFlags.Static)!;

    /// <summary>
    /// Injects an event middleware into the event pipeline.
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <param name="app"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IHost UseEventMiddleware<TMiddleware>(
        this IHost app, params object?[] args) where TMiddleware : IEventMiddleware
    {
        return app.UseEventMiddleware(typeof(TMiddleware), args);
    }

    private static IHost UseEventMiddleware(
        this IHost app
        , Type middleware
        , params object?[] args)
    {
        var methods = middleware.GetMethods(BindingFlags.Instance | BindingFlags.Public);
        MethodInfo? invokeMethod = null;
        foreach (var method in methods)
        {
            if (string.Equals(method.Name, _invokeMethodName, StringComparison.Ordinal) || string.Equals(method.Name, _invokeAsyncMethodName, StringComparison.Ordinal))
            {
                if (invokeMethod is not null)
                {
                    throw new InvalidOperationException();
                }

                invokeMethod = method;
            }
        }
        if (invokeMethod is null)
        {
            throw new InvalidOperationException();
        }

        if (!typeof(Task).IsAssignableFrom(invokeMethod.ReturnType))
        {
            throw new InvalidOperationException();
        }
        var parameters = invokeMethod.GetParameters();
        if (parameters.Length == 0 || parameters[0].ParameterType != typeof(EventContext))
        {
            throw new InvalidOperationException();
        }

        var reflectionBinder = new ReflectionMiddlewareBinder(app, middleware, args, invokeMethod, parameters);
        return app.Use(reflectionBinder.CreateMiddleware);
    }

    public static IHost Use(
        this IHost app
        , Func<RequestDelegate, RequestDelegate> middleware)
    {
        _components.Add(middleware);
        return app;
    }

    internal static RequestDelegate BuildByRequestDelegate(Func<Task> innerEventDelegate)
    {
        RequestDelegate app = context => {
            return innerEventDelegate();
        };

        for (var c = _components.Count - 1; c >= 0; c--)
        {
            app = _components[c](app);
        }

        return app;
    }

    private sealed class ReflectionMiddlewareBinder
    {
        private readonly IHost _app;
        private readonly Type _middleware;
        private readonly object?[] _args;
        private readonly MethodInfo _invokeMethod;
        private readonly ParameterInfo[] _parameters;

        internal ReflectionMiddlewareBinder(
            IHost app,
            Type middleware,
            object?[] args,
            MethodInfo invokeMethod,
            ParameterInfo[] parameters)
        {
            _app = app;
            _middleware = middleware;
            _args = args;
            _invokeMethod = invokeMethod;
            _parameters = parameters;
        }

        // The CreateMiddleware method name is used by ApplicationBuilder to resolve the middleware type.
        internal RequestDelegate CreateMiddleware(RequestDelegate next)
        {
            var ctorArgs = new object[_args.Length + 1];
            ctorArgs[0] = next;
            Array.Copy(_args, 0, ctorArgs, 1, _args.Length);
            var instance = ActivatorUtilities.CreateInstance(_app.Services, _middleware, ctorArgs);
            if (_parameters.Length == 1)
            {
                return (RequestDelegate)_invokeMethod.CreateDelegate(typeof(RequestDelegate), instance);
            }

            // Performance optimization: Use compiled expressions to invoke middleware with services injected in Invoke.
            // If IsDynamicCodeCompiled is false then use standard reflection to avoid overhead of interpreting expressions.
            var factory = System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeCompiled
                ? CompileExpression<object>(_invokeMethod, _parameters)
                : ReflectionFallback<object>(_invokeMethod, _parameters);

            return context => {
                var serviceProvider = context.RequestServices ?? _app.Services;
                if (serviceProvider == null)
                {
                    throw new InvalidOperationException();
                }

                return factory(instance, context, serviceProvider);
            };
        }

        public override string ToString() => _middleware.ToString();
    }

    private static Func<T, EventContext, IServiceProvider, Task> ReflectionFallback<T>(MethodInfo methodInfo, ParameterInfo[] parameters)
    {
        Debug.Assert(!RuntimeFeature.IsDynamicCodeSupported, "Use reflection fallback when dynamic code is not supported.");

        for (var i = 1; i < parameters.Length; i++)
        {
            var parameterType = parameters[i].ParameterType;
            if (parameterType.IsByRef)
            {
                throw new NotSupportedException(_invokeMethodName);
            }
        }

        return (middleware, context, serviceProvider) => {
            var methodArguments = new object[parameters.Length];
            methodArguments[0] = context;
            for (var i = 1; i < parameters.Length; i++)
            {
                methodArguments[i] = GetService(serviceProvider, parameters[i].ParameterType, methodInfo.DeclaringType!);
            }

            return (Task)methodInfo.Invoke(middleware, BindingFlags.DoNotWrapExceptions, binder: null, methodArguments, culture: null)!;
        };
    }

    private static Func<T, EventContext, IServiceProvider, Task> CompileExpression<T>(MethodInfo methodInfo, ParameterInfo[] parameters)
    {
        Debug.Assert(RuntimeFeature.IsDynamicCodeSupported, "Use compiled expression when dynamic code is supported.");

        var middleware = typeof(T);

        var httpContextArg = Expression.Parameter(typeof(EventContext), "eventContext");
        var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
        var instanceArg = Expression.Parameter(middleware, "middleware");

        var methodArguments = new Expression[parameters.Length];
        methodArguments[0] = httpContextArg;
        for (var i = 1; i < parameters.Length; i++)
        {
            var parameterType = parameters[i].ParameterType;
            if (parameterType.IsByRef)
            {
                throw new NotSupportedException(_invokeMethodName);
            }

            var parameterTypeExpression = new Expression[]
            {
                providerArg,
                Expression.Constant(parameterType, typeof(Type)),
                Expression.Constant(methodInfo.DeclaringType, typeof(Type))
            };

            var getServiceCall = Expression.Call(_getServiceInfo, parameterTypeExpression);
            methodArguments[i] = Expression.Convert(getServiceCall, parameterType);
        }

        Expression middlewareInstanceArg = instanceArg;
        if (methodInfo.DeclaringType != null && methodInfo.DeclaringType != typeof(T))
        {
            middlewareInstanceArg = Expression.Convert(middlewareInstanceArg, methodInfo.DeclaringType);
        }

        var body = Expression.Call(middlewareInstanceArg, methodInfo, methodArguments);

        var lambda = Expression.Lambda<Func<T, EventContext, IServiceProvider, Task>>(body, instanceArg, httpContextArg, providerArg);

        return lambda.Compile();
    }

    private static object GetService(IServiceProvider sp, Type type, Type middleware)
    {
        var service = sp.GetService(type);
        if (service == null)
        {
            throw new InvalidOperationException($"{type.Name}, {middleware.Name}");
        }

        return service;
    }
}