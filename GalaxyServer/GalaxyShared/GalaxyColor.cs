
using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public struct GalaxyColor
    {
        [ProtoMember(1)]
        public int R;
        [ProtoMember(2)]
        public int G;
        [ProtoMember(3)]
        public int B;

        public void FromArgb(int r,int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}
