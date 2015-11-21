using System.Collections;


class ClientWrapperPool
{
    static Queue pool = new Queue(100);

    public static ClientWrapper GetWrapper()
    {
        lock (pool)
        {
            if (pool.Count != 0)
            {
                return (ClientWrapper)pool.Dequeue();
            } else
            {
                for (int i = 0; i < 10; i++)
                {
                    pool.Enqueue(new ClientWrapper());
                }
                return new ClientWrapper();
            }

        }
    }

    public static void ReturnWrapper(ClientWrapper wrapper)
    {
        lock(pool)
        {
            pool.Enqueue(wrapper);
        }
    }

    
}

