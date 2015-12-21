
using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public struct GalaxyColor
    {
        [ProtoMember(1)]
        public byte R;
        [ProtoMember(2)]
        public byte G;
        [ProtoMember(3)]
        public byte B;


        private GalaxyColor(byte r,byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;

        }
        public static GalaxyColor FromArgb(int r,int g, int b)
        {
            return new GalaxyColor((byte)r, (byte)g, (byte)b);            
        }

        public static GalaxyColor FromArgb(byte r, byte g, byte b)
        {
            return new GalaxyColor(r, g, b);
        }
    }
}
