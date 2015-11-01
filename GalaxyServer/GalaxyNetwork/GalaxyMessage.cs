

namespace GalaxyServer
{
    class GalaxyMessage
    {
        public const int BUFFER_SIZE = 1024;
        public int Size { get; set; }
        public byte[] Buffer { get; set; }
        public int BufferPos { get; set; }
        public GalaxyClient Client { get; set; }

        public GalaxyMessage()
        {
            Buffer = new byte[BUFFER_SIZE];
            Size = -1;
        }

    }
}
