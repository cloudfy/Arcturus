using Arcturus.EventBus.Abstracts;

namespace Arcturus.EventBus.Sqlite.Internals;

internal record EventItem(IEventMessage Message, string MessageId);
