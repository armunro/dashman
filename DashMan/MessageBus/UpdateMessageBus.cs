using System.Collections.Generic;

namespace DashMan.MessageBus
{
    public static class UpdateMessageBus
    {
        private static readonly List<IMessageListener> Listeners = new List<IMessageListener>();

        public static void RegisterListener(IMessageListener listener)
        {
            Listeners.Add(listener);
        }

        public static void SendMessage()
        {
            Listeners.ForEach(x => x.MessageReceived());
        }
    }
}