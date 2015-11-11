
using System;

namespace GalaxyShared
{
    //Used to wrap all messages going to the server as asynch networking can't directly deserialize objects
    public class MessageWrapper
    {
        public object Client = null;
        public const int BUFFER_SIZE = 1024*8;
        public const int SIZE_BUFFER_SIZE = 4;
        public byte[] SizeBuffer { get; set; }
        public byte[] Buffer { get; set; }
        public int size;
        public int Size {
            get
            {
                return size;
            }
            set
            {
                size = value;
                SizeBuffer = BitConverter.GetBytes(size);
            }
        }

        public MessageWrapper(bool initBuffer = true)
        {
            if (initBuffer) {
                Buffer = new byte[BUFFER_SIZE];
                SizeBuffer = new byte[SIZE_BUFFER_SIZE];
            }
          
        }

    }
}