
using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public struct GalaxyColor
    {
        [ProtoMember(1)]
        public float r;
        [ProtoMember(2)]
        public float g;
        [ProtoMember(3)]
        public float b;


        public GalaxyColor(float r,float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;

        }
        
    }
}
