using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyServer
{
    class GalaxyMessage
    {
        public const int BUFFER_SIZE = 1024;
        public int Size { get; set; }
        public byte[] buffer { get; set; }
        public int BufferPos { get; set; }
        public GalaxyClient Client { get; set; }

        public GalaxyMessage()
        {
            buffer = new byte[BUFFER_SIZE];
            Size = -1;
        }

    }
}
