/*
This system broadcasts events to listeners in the system. 
Unlike a more traditional event system, it is the listeners who filter who tells them what happens, not the broadcaster who filters where the message lands.
*/

namespace menhera
{
    public abstract class IEvent
    {
    }

    public delegate void MessageHandler<E>(E payload) where E : IEvent;

    public class MessagingService(TeamManager teamManager) : Service
    {
        private readonly TeamManager teamManager = teamManager;

        protected delegate void MessageHandlerBase(IEvent payload);
        protected struct MessageHandlerMeta(ActorIdentifier character, MessageHandlerBase handler, long filter)
        {
            public ActorIdentifier character = character;
            public MessageHandlerBase handler = handler;
            public long filter = filter;

            public static MessageHandlerMeta Create<E>(ActorIdentifier character, MessageHandler<E> handler, long filter) where E : IEvent
            {
                return new(character, payload => handler((E)payload), filter);
            }
        }

        protected readonly Dictionary<Type, List<MessageHandlerMeta>> listenerTable = [];

        public void BroadcastEvent(ActorIdentifier sender, IEvent ev)
        {
            var validListeners = GetEventTable(ev.GetType())
                .Where(elem => (elem.filter & sender.Flag) != 0);

            foreach (var listener in validListeners)
                listener.handler.Invoke(ev);
        }

        public void Listen<E>(ActorIdentifier listener, MessageHandler<E> handler, long filter) where E : IEvent
        {
            GetEventTable<E>()
                .Add(MessageHandlerMeta.Create(listener, handler, filter));
        }

        public void Listen<E>(ActorIdentifier listener, MessageHandler<E> handler, Scope scope) where E : IEvent
        {
            var filter = teamManager.GetFilterFor(listener, scope);
            Listen(listener, handler, filter);
        }

        public void Unlisten<E>(ActorIdentifier listener) where E : IEvent
        {
            var table = GetEventTable<E>();
            var removeMe = table.FindIndex(meta => meta.character == listener);
            if (removeMe != -1)
                table.RemoveAt(removeMe);
        }

        private List<MessageHandlerMeta> GetEventTable<E>() where E : IEvent
        {
            return GetEventTable(typeof(E));
        }

        private List<MessageHandlerMeta> GetEventTable(Type eventType)
        {
            if (!listenerTable.ContainsKey(eventType))
                listenerTable.Add(eventType, []);
            return listenerTable.GetValueOrDefault(eventType, []);
        }
    }
}
