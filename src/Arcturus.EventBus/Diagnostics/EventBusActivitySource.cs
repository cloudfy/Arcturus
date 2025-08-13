namespace Arcturus.EventBus.Diagnostics;

public static class EventBusActivitySource
{
    private static readonly string AssemblyVersion = typeof(EventBusActivitySource).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "";
    private static readonly ActivitySource _publisherSource = new(PublisherSourceName, AssemblyVersion);
    private static readonly ActivitySource _subscriberSource = new(SubscriberSourceName, AssemblyVersion);

    internal const string MessageId = "eventbus.message.id";
    internal const string MessageConversationId = "eventbus.message.conversation_id";
    internal const string MessagingSystem = "EventBus";
    internal const string MessagingOperationTypePublish = "publish";
    internal const string MessagingOperationTypeProcess = "process";
    internal const string MessagingOperationTypeReceive = "receive";
    internal const string MessagingOperationType = "eventbus.message.operation";

    public static bool PublisherHasListeners => _publisherSource.HasListeners();

    internal static readonly IEnumerable<KeyValuePair<string, object?>> CreationTags =
    [
        new KeyValuePair<string, object?>(MessagingSystem, "rabbitmq")
    ];

    public static Activity? Publish(string eventMessageName, ActivityContext linkedContext = default)
    {
        if (!_publisherSource.HasListeners())
        {
            return null;
        }
        Activity? activity = linkedContext == default
               ? _publisherSource.StartEventBusActivity(
                   MessagingOperationTypePublish, // UseRoutingKeyAsOperationName ? $"{routingKey} {MessagingOperationTypeSend}" : MessagingOperationTypeSend,
                   ActivityKind.Producer)
               : _publisherSource.StartLinkedEventBusActivity(
                   MessagingOperationTypePublish, // UseRoutingKeyAsOperationName ? $"{routingKey} {MessagingOperationTypeSend}" : MessagingOperationTypeSend,
                   ActivityKind.Producer, linkedContext);
        if (activity != null && activity.IsAllDataRequested)
        {
            PopulateMessagingTags(MessagingOperationTypePublish, activity);
        }

        return activity;

    }

    private static Activity? StartEventBusActivity(this ActivitySource source, string name, ActivityKind kind,
            ActivityContext parentContext = default)
    {
        return source.CreateActivity(name, kind, parentContext, idFormat: ActivityIdFormat.W3C, tags: CreationTags)?.Start();
    }

    private static Activity? StartLinkedEventBusActivity(this ActivitySource source, string name, ActivityKind kind,
        ActivityContext linkedContext = default, ActivityContext parentContext = default)
    {
        return source.CreateActivity(name, kind, parentContext: parentContext,
                links: new[] { new ActivityLink(linkedContext) }, idFormat: ActivityIdFormat.W3C,
                tags: CreationTags)
            ?.Start();
    }

    public const string PublisherSourceName = "Arcturus.EventBus.Publisher";
    public const string SubscriberSourceName = "Arcturus.EventBus.Subscriber";
    public const string ActivityName = "Arcturus.EventBus.*";

    public static Action<Activity, IDictionary<string, object?>> ContextInjector { get; set; } = DefaultContextInjector;
    public static Func<IDiagnosticProperties, ActivityContext> ContextExtractor { get; set; } = DefaultContextExtractor;

    //private static void PopulateMessagingTags(string operation, Activity activity)
    //{
    //    PopulateMessagingTags(operation, activity);

    //    if (!string.IsNullOrEmpty(readOnlyBasicProperties.CorrelationId))
    //    {
    //        activity.SetTag(MessageConversationId, readOnlyBasicProperties.CorrelationId);
    //    }

    //    if (!string.IsNullOrEmpty(readOnlyBasicProperties.MessageId))
    //    {
    //        activity.SetTag(MessageId, readOnlyBasicProperties.MessageId);
    //    }
    //}
    private static void PopulateMessagingTags(string operation, Activity activity)
    {
        activity
            .SetTag(MessagingOperationType, operation);
        //.SetTag(MessagingDestination, string.IsNullOrEmpty(exchange) ? "amq.default" : exchange)
        //.SetTag(MessagingDestinationRoutingKey, routingKey)
        //.SetTag(MessagingBodySize, bodySize);

        //if (deliveryTag > 0)
        //{
        //    activity.SetTag(RabbitMQDeliveryTag, deliveryTag);
        //}
    }

    private static void DefaultContextInjector(Activity sendActivity, IDictionary<string, object?> props)
    {
        DistributedContextPropagator.Current.Inject(sendActivity, props, DefaultContextSetter);
    }
    private static ActivityContext DefaultContextExtractor(IDiagnosticProperties props)
    {
        if (props.Headers == null)
        {
            return default;
        }

        bool hasHeaders = false;
        foreach (string header in DistributedContextPropagator.Current.Fields)
        {
            if (props.Headers.ContainsKey(header))
            {
                hasHeaders = true;
                break;
            }
        }


        if (!hasHeaders)
        {
            return default;
        }

        DistributedContextPropagator.Current.ExtractTraceIdAndState(props.Headers, DefaultContextGetter, out string? traceParent, out string? traceState);
        return ActivityContext.TryParse(traceParent, traceState, out ActivityContext context) ? context : default;
    }

    private static void DefaultContextSetter(object? carrier, string name, string value)
    {
        if (!(carrier is IDictionary<string, object> carrierDictionary))
        {
            return;
        }

        // Only propagate headers if they haven't already been set
        carrierDictionary[name] = value;
    }
    private static void DefaultContextGetter(object? carrier, string name, out string? value, out IEnumerable<string>? values)
    {
        if (carrier is IDictionary<string, object> carrierDict &&
            carrierDict.TryGetValue(name, out object? propsVal) && propsVal is byte[] bytes)
        {
            value = Encoding.UTF8.GetString(bytes);
            values = default;
        }
        else
        {
            value = default;
            values = default;
        }
    }
}