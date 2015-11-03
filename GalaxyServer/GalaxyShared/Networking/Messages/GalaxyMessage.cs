
using System;

namespace GalaxyShared.Networking.Messages
{
    public class GalaxyMessage
    {
        public object Client = null;
        public const int BUFFER_SIZE = 1024;
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

        public GalaxyMessage(bool initBuffer = true)
        {
            if (initBuffer) {
                Buffer = new byte[BUFFER_SIZE];
                SizeBuffer = new byte[SIZE_BUFFER_SIZE];
            }
          
        }

    }
}