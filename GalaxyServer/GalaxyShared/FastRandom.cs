using System;


namespace GalaxyShared
{
    public class FastRandom
    {
        private uint _x = 123456789;
        private uint _y = 362436069;
        private uint _z = 521288629;
        private uint _w = 88675123;


        public FastRandom(int x = 123456789, int y = 362436069, int z = 521288629, int w = 88675123)
        {
            _x -= (uint)x;
            _y -= (uint)y;
            _z -= (uint)z;
            _w += (uint)w;

        }

        public void Init(int x = 123456789, int y = 362436069, int z = 521288629, int w = 88675123)
        {
            _x -= (uint)x;
            _y -= (uint)y;
            _z -= (uint)z;
            _w += (uint)w;

        }
        public float Next(float min, float max)
        {
            uint tx = _x ^ (_x << 11);
            uint ty = _y ^ (_y << 11);
            uint tz = _z ^ (_z << 11);
            uint tw = _w ^ (_w << 11);
            _x = _w ^ (_w >> 19) ^ (tx ^ (tx >> 8));
            _y = _x ^ (_x >> 19) ^ (ty ^ (ty >> 8));
            _z = _y ^ (_y >> 19) ^ (tz ^ (tz >> 8));
            _w = _z ^ (_z >> 19) ^ (tw ^ (tw >> 8));
            
            float f = (1f / 256f) * ((byte)_x+ (1f / 256f) * ((byte)_y + (1f / 256f) * ((byte)_z + (1f / 256f) * (byte)_w)));
            return f * (max - min) + min;
            
        }

        public int Next(int min, int max)
        {
            byte[] bytes = new byte[4];
            NextBytes(bytes);
            int i = BitConverter.ToInt32(bytes, 0);
            i = Math.Abs(i);
            i = i % (max - min);
            return i + min;
        }

        public float NextGaussianFloat()
        {
            float u, v, S;

            do
            {
                u = 2.0f * this.Next(0f, 1f) - 1.0f;
                v = 2.0f * this.Next(0f, 1f) - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0);

            float fac = (float)Math.Sqrt(-2.0f * (float)Math.Log(S) / S);
            return u * fac;
        }

        public unsafe void NextBytes(byte[] buf)
        {

            fixed (byte* pbytes = buf)
            {
                uint* pbuf = (uint*)(pbytes);
               // uint* pend = (uint*)(pbytes + buf.Length);
               
                    uint tx = _x ^ (_x << 11);
                    uint ty = _y ^ (_y << 11);
                    uint tz = _z ^ (_z << 11);
                    uint tw = _w ^ (_w << 11);
                    *(pbuf++) = _x = _w ^ (_w >> 19) ^ (tx ^ (tx >> 8));
                    *(pbuf++) = _y = _x ^ (_x >> 19) ^ (ty ^ (ty >> 8));
                    *(pbuf++) = _z = _y ^ (_y >> 19) ^ (tz ^ (tz >> 8));
                    *(pbuf++) = _w = _z ^ (_z >> 19) ^ (tw ^ (tw >> 8));
                
            }
        }

       

       

    }
}