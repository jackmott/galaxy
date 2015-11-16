
using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public struct GalaxyColor
    {
        [ProtoMember(0)]
        public int R;
        [ProtoMember(1)]
        public int G;
        [ProtoMember(2)]
        public int B;

        public void FromArgb(int r,int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}
