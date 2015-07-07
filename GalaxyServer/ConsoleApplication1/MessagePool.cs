using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;


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
            message.Size = -1;
            message.BufferPos = 0;
            message.Client = null;
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
