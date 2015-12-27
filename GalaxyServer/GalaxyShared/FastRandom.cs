using System;

namespace GalaxyShared
{
    public class FastRandom
    {
        /* Period parameters */
        private const int N = 624;
        private const int M = 397;
        private const uint MATRIX_A = 0x9908b0df; /* constant vector a */
        private const uint UPPER_MASK = 0x80000000; /* most significant w-r bits */
        private const uint LOWER_MASK = 0x7fffffff; /* least significant r bits */

        /* Tempering parameters */
        private const uint TEMPERING_MASK_B = 0x9d2c5680;
        private const uint TEMPERING_MASK_C = 0xefc60000;

        private static uint TEMPERING_SHIFT_U(uint y) { return (y >> 11); }
        private static uint TEMPERING_SHIFT_S(uint y) { return (y << 7); }
        private static uint TEMPERING_SHIFT_T(uint y) { return (y << 15); }
        private static uint TEMPERING_SHIFT_L(uint y) { return (y >> 18); }

        private uint[] mt = new uint[N]; /* the array for the state vector  */

        private short mti;

        private static uint[] mag01 = { 0x0, MATRIX_A };


        public FastRandom(double a, double b = 43837828, double c = 8378737, double d = 838383)
        {
            Init(Convert.ToInt32(a), Convert.ToInt32(b), Convert.ToInt32(c), Convert.ToInt32(d));
        }
        public FastRandom(int a, int b = 43837828, int c = 8378737, int d = 838383)
        {
            uint seed = (uint)(a ^ b ^ c ^ d);
            mt[0] = (uint)seed & 0xffffffffU;
            for (mti = 1; mti < N; ++mti)
            {
                mt[mti] = (69069 * mt[mti - 1]) & 0xffffffffU;
            }

        }

        public void Init(int a, int b = 43837828, int c = 8378737, int d = 838383)
        {
            uint seed = (uint)(a ^ b ^ c ^ d);
            mt[0] = (uint)seed & 0xffffffffU;
            for (mti = 1; mti < N; ++mti)
            {
                mt[mti] = (69069 * mt[mti - 1]) & 0xffffffffU;
            }
        }




        protected uint GenerateUInt()
        {
            uint y;

            /* mag01[x] = x * MATRIX_A  for x=0,1 */
            if (mti >= N) /* generate N words at one time */
            {
                short kk = 0;

                for (; kk < N - M; ++kk)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                for (; kk < N - 1; ++kk)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];

                mti = 0;
            }

            y = mt[mti++];
            y ^= TEMPERING_SHIFT_U(y);
            y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
            y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;
            y ^= TEMPERING_SHIFT_L(y);

            return y;
        }

        public virtual uint NextUInt()
        {
            return this.GenerateUInt();
        }

        public virtual uint NextUInt(uint maxValue)
        {
            return (uint)(this.GenerateUInt() / ((double)uint.MaxValue / maxValue));
        }

        public virtual uint NextUInt(uint minValue, uint maxValue) /* throws ArgumentOutOfRangeException */
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            return (uint)(this.GenerateUInt() / ((double)uint.MaxValue / (maxValue - minValue)) + minValue);
        }

        public  int Next()
        {
            return this.Next(int.MaxValue);
        }

        public  int Next(int maxValue) /* throws ArgumentOutOfRangeException */
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return 0;
            }

            return (int)(this.NextDouble() * maxValue);
        }

        public  int Next(int minValue, int maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (maxValue == minValue)
            {
                return minValue;
            }
            else
            {
                return this.Next(maxValue - minValue) + minValue;
            }
        }

        public  void NextBytes(byte[] buffer) /* throws ArgumentNullException*/
        {
            int bufLen = buffer.Length;

            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            for (int idx = 0; idx < bufLen; ++idx)
            {
                buffer[idx] = (byte)this.Next(256);
            }
        }

        public  double NextDouble()
        {
            return (double)this.GenerateUInt() / ((ulong)uint.MaxValue + 1);
        }

        public double Next(double min, double max)
        {
            return NextDouble() * (max - min) + min;
        }

        public float Next(float min, float max)
        {
            return (float)NextDouble() * (max - min) + min;
        }

        public float NextGaussianFloat()
        {
            float u, v, S;

            do
            {
                u = 2.0f * this.Next(0f,1f) - 1.0f;
                v = 2.0f * this.Next(0f,1f) - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0);

            float fac =(float) Math.Sqrt(-2.0f * (float)Math.Log(S) / S);
            return u * fac;
        }


    }

}
