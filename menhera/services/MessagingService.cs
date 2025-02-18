/*
This system broadcasts events to listeners in the system. 
Unlike a more traditional event system, it is the listeners who filter who tells them what happens, not the broadcaster who filters where the message lands.
*/

namespace menhera
{
    public interface IEvent
    {
    }

    public delegate void MessageHandler<E>(E payload) where E : IEvent;
    public delegate void MessageHandlerBase(IEvent payload);

    struct MessageHandlerMeta(ActorIdentifier character, MessageHandlerBase handler, long filter)
    {
        public ActorIdentifier character = character;
        public MessageHandlerBase handler = handler;
        public long filter = filter;

        public static MessageHandlerMeta Create<E>(ActorIdentifier character, MessageHandler<E> handler, long filter) where E : IEvent
        {
            return new(character, (IEvent payload) => handler((E)payload), filter);
        }
    }

    public class MessagingService : Service
    {
        readonly Dictionary<Type, List<MessageHandlerMeta>> listenerTable = [];

        public void BroadcastEvent(ActorIdentifier sender, IEvent ev)
        {
            var validListeners = GetEventTable(ev.GetType())
                .Where(elem => (elem.filter & sender.Flag) != 0);

            foreach (var listener in validListeners)
                listener.handler.Invoke(ev);
        }

        public void Listen<E>(ActorIdentifier listener, MessageHandler<E> handler, long filter) where E : IEvent
        {
            GetEventTable(typeof(E))
                .Append(MessageHandlerMeta.Create(listener, handler, filter));
        }

        public void Unlisten(Type eventType, ActorIdentifier listener)
        {
            var table = GetEventTable(eventType);
            var removeMe = table.FindIndex(meta => meta.character == listener);
            if (removeMe != -1)
                table.RemoveAt(removeMe);
        }

        private List<MessageHandlerMeta> GetEventTable(Type eventType)
        {
            return listenerTable.GetValueOrDefault(eventType, []);
        }
    }
}
