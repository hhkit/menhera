/*
This system broadcasts events to listeners in the system. 
Unlike a more traditional event system, it is the listeners who filter who tells them what happens, not the broadcaster who filters where the message lands.
*/

namespace menhera
{
    public enum EventType
    {
        Clash,
        BleedProc,
    }

    public delegate void MessageHandler(object payload);

    struct MessageHandlerMeta(ActorIdentifier character, MessageHandler handler, long filter)
    {
        public ActorIdentifier character = character;
        public MessageHandler handler = handler;
        public long filter = filter;
    }

    public class MessagingSystem
    {
        readonly Dictionary<EventType, List<MessageHandlerMeta>> listenerTable = [];

        public void BroadcastEvent(ActorIdentifier sender, EventType eventType, object payload)
        {
            var validListeners = GetEventTable(eventType)
                .Where(elem => (elem.filter & sender.Flag) != 0);

            foreach (var listener in validListeners)
                listener.handler.Invoke(payload);
        }

        public void Listen(ActorIdentifier listener, EventType eventType, MessageHandler handler, long filter)
        {
            GetEventTable(eventType)
                .Append(new(listener, handler, filter));
        }

        public void Unlisten(EventType eventType, ActorIdentifier listener)
        {
            var table = GetEventTable(eventType);
            var removeMe = table.FindIndex(meta => meta.character == listener);
            if (removeMe != -1)
                table.RemoveAt(removeMe);
        }

        private List<MessageHandlerMeta> GetEventTable(EventType eventType)
        {
            return listenerTable.GetValueOrDefault(eventType, []);
        }
    }
}
