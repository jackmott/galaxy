using System;

namespace GalaxyShared
{
    [Serializable]
    public struct GalaxyColor
    {
        public int R;
        public int G;
        public int B;

        public void FromArgb(int r,int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}
