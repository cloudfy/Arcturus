using Arcturus.Mediation.Abstracts;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(IAbstractRequest))]
[assembly: TypeForwardedTo(typeof(IRequest<>))]
[assembly: TypeForwardedTo(typeof(IRequest))]
[assembly: TypeForwardedTo(typeof(INotification))]