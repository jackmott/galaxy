
using System.Collections.Concurrent;
using GalaxyShared;

namespace GalaxyServer
{
    class MessagePool
    {

        private ConcurrentQueue<GalaxyMessage> messagePool;

        public MessagePool()
        {
            messagePool = new ConcurrentQueue<GalaxyMessage>();
            AddBuffers();
        }

        public GalaxyMessage GetMessage()
        {
            if (messagePool.Count == 0)
            {
                AddBuffers();
            }
            GalaxyMessage result;
            while (!messagePool.TryDequeue(out result))
            {

            }
            return result;
        }

        public void ReplaceMessage(GalaxyMessage message)
        {
            message.Size = 0;                        
            messagePool.Enqueue(message);
        }

        private void AddBuffers()
        {
            for (int i = 0; i < 10; i++)
            {
                GalaxyMessage message = new GalaxyMessage();
                messagePool.Enqueue(message);
            }
        }



    }
}