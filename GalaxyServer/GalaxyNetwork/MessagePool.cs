
using System.Collections.Concurrent;
using GalaxyShared;

namespace GalaxyServer
{
    class MessagePool
    {

        private ConcurrentQueue<MessageWrapper> messagePool;

        public MessagePool()
        {
            messagePool = new ConcurrentQueue<MessageWrapper>();
            AddBuffers();
        }

        public MessageWrapper GetMessage()
        {
            if (messagePool.Count == 0)
            {
                AddBuffers();
            }
            MessageWrapper result;
            while (!messagePool.TryDequeue(out result))
            {

            }
            return result;
        }

        public void ReplaceMessage(MessageWrapper message)
        {
            message.Size = 0;                        
            messagePool.Enqueue(message);
        }

        private void AddBuffers()
        {
            for (int i = 0; i < 10; i++)
            {
                MessageWrapper message = new MessageWrapper();
                messagePool.Enqueue(message);
            }
        }



    }
}