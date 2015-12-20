using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;


namespace FastNoise
{
    public struct NoiseOutput
    {
        public float[] RawNoise;
        //min and max values to be used for normalizing, if you wish
        public float Min;
        public float Max;

        public NoiseOutput(float[] rawNoise, float min, float max)
        {
            RawNoise = rawNoise;
            Min = min;
            Max = max;
        }

    }
    public class NoiseMaker
    {

        [DllImport("FastNoise")]
        private static unsafe extern float* GetSphereSurfaceNoiseSIMD(int width, int height, int octaves, float lacunarity, float frequency, float gain, float offset,int fractalType, int noiseType, float* outMin, float* outMax);
        [DllImport("FastNoise")]
        private static unsafe extern void CleanUpNoiseSIMD(float* resultArray);

        public enum FractalType { FBM, TURBULENCE, RIDGE, PLAIN };
        public enum NoiseType { PERLIN, SIMPLEX };

        public Color[] Gradient;

        private struct Settings
        {
            public int Octaves;
            public float Lacunarity;
            public float Gain;
            public float Offset;
            public float Frequency;
            public NoiseType NoiseType;
            public FractalType FractalType;
        }

        private Settings settings;

        public NoiseMaker(int octaves, float lacunarity, float gain, float offset, float frequency,FractalType fractalType, NoiseType noiseType)
        {
            SetNoiseSettings(octaves, lacunarity, gain, offset, frequency, fractalType,noiseType);
        }

        public void SetNoiseSettings(int octaves, float lacunarity, float gain, float offset, float frequency,FractalType fractalType, NoiseType noiseType)
        {
            settings.Octaves = octaves;
            settings.Lacunarity = lacunarity;
            settings.Gain = gain;
            settings.Offset = offset;
            settings.Frequency = frequency;
            settings.NoiseType = noiseType;
            settings.FractalType = fractalType;
        }


        static float FADE(float t)
        {
            return (t * t * t * (t * (t * 6f - 15f) + 10f));
        }



        static float LERP(float t, float a, float b)
        {
            return ((a) + (t) * ((b) - (a)));
        }


        //---------------------------------------------------------------------
        // Static data

        /*
         * Permutation table. This is just a random jumble of all numbers 0-255,
         * repeated twice to avoid wrapping the index at 255 for each lookup.
         * This needs to be exactly the same for all instances on all platforms,
         * so it's easiest to just keep it as static explicit data.
         * This also removes the need for any initialisation of this class.
         *
         * Note that making this an int[] instead of a char[] might make the
         * code run faster on platforms with a high penalty for unaligned single
         * byte addressing. Intel x86 is generally single-byte-friendly, but
         * some other CPUs are faster with 4-aligned reads.
         * However, a char[] is smaller, which avoids cache trashing, and that
         * is probably the most important aspect on most architectures.
         * This array is accessed a *lot* by the noise functions.
         * A vector-valued noise over 3D accesses it 96 times, and a
         * float-valued 4D noise 64 times. We want this to fit in the cache!
         */



        static short[] perm = {151,160,137,91,90,15,
    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
    151,160,137,91,90,15,
    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
    };

        //---------------------------------------------------------------------

        /*
         * Helper functions to compute gradients-dot-residualvectors (1D to 4D)
         * Note that these generate gradients of more than unit length. To make
         * a close match with the value range of classic Perlin noise, the final
         * noise values need to be rescaled. To match the RenderMan noise in a
         * statistical sense, the approximate scaling values (empirically
         * determined from test renderings) are:
         * 1D noise needs rescaling with 0.188
         * 2D noise needs rescaling with 0.507
         * 3D noise needs rescaling with 0.936
         * 4D noise needs rescaling with 0.87
         * Note that these noise functions are the most practical and useful
         * signed version of Perlin noise. To return values according to the
         * RenderMan specification from the SL noise() and pnoise() functions,
         * the noise values need to be scaled and offset to [0,1], like this:
         * float SLnoise = (noise3(x,y,z) + 1.0) * 0.5;
         */

        float grad1(int h, float x)
        {
            h = h & 15;
            float grad = 1.0f + (h & 7);  // Gradient value 1.0, 2.0, ..., 8.0
            if ((h & 8) == 1)
                grad = -grad;         // and a random sign for the gradient
            return (grad * x);           // Multiply the gradient with the distance
        }

        float grad2(int h, float x, float y)
        {
            h = h & 7;      // Convert low 3 bits of hash code
            float u = h < 4 ? x : y;  // into 8 simple gradient directions,
            float v = h < 4 ? y : x;  // and compute the dot product with (x,y).

            if ((h & 1) == 1)
                u = -u;
            float n = 2.0f * v;
            if ((h & 2) == 1)
                n = -2.0f * v;

            return u + n;
        }

        float grad3(int h, float x, float y, float z)
        {
            h = h & 15;     // Convert low 4 bits of hash code into 12 simple
            float u = h < 8 ? x : y; // gradient directions, and compute dot product.
            float v = h < 4 ? y : h == 12 || h == 14 ? x : z; // Fix repeats at h = 12 to 15
            if ((h & 1) != 0)
                u = -u;

            if ((h & 2) != 0)
                v = -v;

            return u + v;
        }

        float grad4(int h, float x, float y, float z, float t)
        {
            h = h & 31;      // Convert low 5 bits of hash code into 32 simple
            float u = h < 24 ? x : y; // gradient directions, and compute dot product.
            float v = h < 16 ? y : z;
            float w = h < 8 ? z : t;

            if ((h & 1) == 1)
                u = -u;

            if ((h & 2) == 1)
                v = -v;

            if ((h & 4) == 1)
                w = -w;

            return u + v + w;
        }

        //---------------------------------------------------------------------
        /** 1D float Perlin noise, SL "noise()"
         */
        public float noise1(float x)
        {
            int ix0, ix1;
            float fx0, fx1;
            float s;
            float n0, n1;

            ix0 = x > 0 ? (int)x : (int)x - 1;  //FASTFLOOR(x); // Integer part of x
            fx0 = x - ix0;       // Fractional part of x
            fx1 = fx0 - 1.0f;
            ix1 = (ix0 + 1) & 0xff;
            ix0 = ix0 & 0xff;    // Wrap to 0..255

            s = FADE(fx0);

            n0 = grad1(perm[ix0], fx0);
            n1 = grad1(perm[ix1], fx1);
            return 0.188f * (LERP(s, n0, n1));
        }

        //---------------------------------------------------------------------
        /** 1D float Perlin periodic noise, SL "pnoise()"
         */
        public float pnoise1(float x, int px)
        {
            int ix0, ix1;
            float fx0, fx1;
            float s;
            float n0, n1;

            ix0 = x > 0 ? (int)x : (int)x - 1;  //FASTFLOOR(x); // Integer part of x
            fx0 = x - ix0;       // Fractional part of x
            fx1 = fx0 - 1.0f;
            ix1 = ((ix0 + 1) % px) & 0xff; // Wrap to 0..px-1 *and* wrap to 0..255
            ix0 = (ix0 % px) & 0xff;      // (because px might be greater than 256)

            s = FADE(fx0);

            n0 = grad1(perm[ix0], fx0);
            n1 = grad1(perm[ix1], fx1);
            return 0.188f * (LERP(s, n0, n1));
        }


        //---------------------------------------------------------------------
        /** 2D float Perlin noise.
         */
        public float noise2(float x, float y)
        {
            int ix0, iy0, ix1, iy1;
            float fx0, fy0, fx1, fy1;
            float s, t;
            float nx0, nx1, n0, n1;


            ix0 = (x > 0) ? (int)x : (int)(x - 1);
            iy0 = (y > 0) ? (int)y : (int)(y - 1);
            fx0 = x - ix0;        // Fractional part of x
            fy0 = y - iy0;        // Fractional part of y
            fx1 = fx0 - 1.0f;
            fy1 = fy0 - 1.0f;
            ix1 = (ix0 + 1) & 0xff;  // Wrap to 0..255
            iy1 = (iy0 + 1) & 0xff;
            ix0 = ix0 & 0xff;
            iy0 = iy0 & 0xff;

            t = FADE(fy0);
            s = FADE(fx0);

            nx0 = grad2(perm[ix0 + perm[iy0]], fx0, fy0);
            nx1 = grad2(perm[ix0 + perm[iy1]], fx0, fy1);
            n0 = LERP(t, nx0, nx1);

            nx0 = grad2(perm[ix1 + perm[iy0]], fx1, fy0);
            nx1 = grad2(perm[ix1 + perm[iy1]], fx1, fy1);
            n1 = LERP(t, nx0, nx1);

            return 0.507f * (LERP(s, n0, n1));
        }

        //---------------------------------------------------------------------
        /** 2D float Perlin periodic noise.
         */
        public float pnoise2(float x, float y, int px, int py)
        {
            int ix0, iy0, ix1, iy1;
            float fx0, fy0, fx1, fy1;
            float s, t;
            float nx0, nx1, n0, n1;

            ix0 = (x > 0) ? (int)x : (int)(x - 1);
            iy0 = (y > 0) ? (int)y : (int)(y - 1);
            fx0 = x - ix0;        // Fractional part of x
            fy0 = y - iy0;        // Fractional part of y
            fx1 = fx0 - 1.0f;
            fy1 = fy0 - 1.0f;
            ix1 = ((ix0 + 1) % px) & 0xff;  // Wrap to 0..px-1 and wrap to 0..255
            iy1 = ((iy0 + 1) % py) & 0xff;  // Wrap to 0..py-1 and wrap to 0..255
            ix0 = (ix0 % px) & 0xff;
            iy0 = (iy0 % py) & 0xff;

            t = (fy0 * fy0 * fy0 * (fy0 * (fy0 * 6 - 15) + 10));
            s = (fx0 * fx0 * fx0 * (fx0 * (fx0 * 6 - 15) + 10));

            nx0 = grad2(perm[ix0 + perm[iy0]], fx0, fy0);
            nx1 = grad2(perm[ix0 + perm[iy1]], fx0, fy1);
            n0 = LERP(t, nx0, nx1);

            nx0 = grad2(perm[ix1 + perm[iy0]], fx1, fy0);
            nx1 = grad2(perm[ix1 + perm[iy1]], fx1, fy1);
            n1 = LERP(t, nx0, nx1);

            return ((0.446f * (LERP(s, n0, n1))) + .5f);
        }


        //---------------------------------------------------------------------
        /** 3D float Perlin noise.
         */
        public float noise3(float x, float y, float z)
        {
            int ix0, iy0, iz0, ix1, iy1, iz1;
            float fx0, fy0, fz0, fx1, fy1, fz1;
            float s, t, r;
            float nxy0, nxy1, nx0, nx1, n0, n1;

            ix0 = x > 0 ? (int)x : (int)x - 1;  //FASTFLOOR(x); // Integer part of x
            iy0 = y > 0 ? (int)y : (int)y - 1; //FASTFLOOR(y); // Integer part of y
            iz0 = z > 0 ? (int)z : (int)z - 1; //FASTFLOOR(z); // Integer part of z
            fx0 = x - ix0;        // Fractional part of x
            fy0 = y - iy0;        // Fractional part of y
            fz0 = z - iz0;        // Fractional part of z
            fx1 = fx0 - 1.0f;
            fy1 = fy0 - 1.0f;
            fz1 = fz0 - 1.0f;
            ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
            iy1 = (iy0 + 1) & 0xff;
            iz1 = (iz0 + 1) & 0xff;
            ix0 = ix0 & 0xff;
            iy0 = iy0 & 0xff;
            iz0 = iz0 & 0xff;

            r = FADE(fz0);
            t = FADE(fy0);
            s = FADE(fx0);

            nxy0 = grad3(perm[ix0 + perm[iy0 + perm[iz0]]], fx0, fy0, fz0);
            nxy1 = grad3(perm[ix0 + perm[iy0 + perm[iz1]]], fx0, fy0, fz1);
            nx0 = LERP(r, nxy0, nxy1);

            nxy0 = grad3(perm[ix0 + perm[iy1 + perm[iz0]]], fx0, fy1, fz0);
            nxy1 = grad3(perm[ix0 + perm[iy1 + perm[iz1]]], fx0, fy1, fz1);
            nx1 = LERP(r, nxy0, nxy1);

            n0 = LERP(t, nx0, nx1);

            nxy0 = grad3(perm[ix1 + perm[iy0 + perm[iz0]]], fx1, fy0, fz0);
            nxy1 = grad3(perm[ix1 + perm[iy0 + perm[iz1]]], fx1, fy0, fz1);
            nx0 = LERP(r, nxy0, nxy1);

            nxy0 = grad3(perm[ix1 + perm[iy1 + perm[iz0]]], fx1, fy1, fz0);
            nxy1 = grad3(perm[ix1 + perm[iy1 + perm[iz1]]], fx1, fy1, fz1);
            nx1 = LERP(r, nxy0, nxy1);

            n1 = LERP(t, nx0, nx1);

            return LERP(s, n0, n1) * .936f;
        }

        //---------------------------------------------------------------------
        /** 3D float Perlin periodic noise.
         */
        public float pnoise3(float x, float y, float z, int px, int py, int pz)
        {
            int ix0, iy0, iz0, ix1, iy1, iz1;
            float fx0, fy0, fz0, fx1, fy1, fz1;
            float s, t, r;
            float nxy0, nxy1, nx0, nx1, n0, n1;


            ix0 = x > 0 ? (int)x : (int)x - 1;  //FASTFLOOR(x); // Integer part of x
            iy0 = y > 0 ? (int)y : (int)y - 1; //FASTFLOOR(y); // Integer part of y
            iz0 = z > 0 ? (int)z : (int)z - 1; //FASTFLOOR(z); // Integer part of z
            fx0 = x - ix0;        // Fractional part of x
            fy0 = y - iy0;        // Fractional part of y
            fz0 = z - iz0;        // Fractional part of z
            fx1 = fx0 - 1.0f;
            fy1 = fy0 - 1.0f;
            fz1 = fz0 - 1.0f;
            ix1 = ((ix0 + 1) % px) & 0xff; // Wrap to 0..px-1 and wrap to 0..255
            iy1 = ((iy0 + 1) % py) & 0xff; // Wrap to 0..py-1 and wrap to 0..255
            iz1 = ((iz0 + 1) % pz) & 0xff; // Wrap to 0..pz-1 and wrap to 0..255
            ix0 = (ix0 % px) & 0xff;
            iy0 = (iy0 % py) & 0xff;
            iz0 = (iz0 % pz) & 0xff;

            r = FADE(fz0);
            t = FADE(fy0);
            s = FADE(fx0);

            nxy0 = grad3(perm[ix0 + perm[iy0 + perm[iz0]]], fx0, fy0, fz0);
            nxy1 = grad3(perm[ix0 + perm[iy0 + perm[iz1]]], fx0, fy0, fz1);
            nx0 = LERP(r, nxy0, nxy1);

            nxy0 = grad3(perm[ix0 + perm[iy1 + perm[iz0]]], fx0, fy1, fz0);
            nxy1 = grad3(perm[ix0 + perm[iy1 + perm[iz1]]], fx0, fy1, fz1);
            nx1 = LERP(r, nxy0, nxy1);

            n0 = LERP(t, nx0, nx1);

            nxy0 = grad3(perm[ix1 + perm[iy0 + perm[iz0]]], fx1, fy0, fz0);
            nxy1 = grad3(perm[ix1 + perm[iy0 + perm[iz1]]], fx1, fy0, fz1);
            nx0 = LERP(r, nxy0, nxy1);

            nxy0 = grad3(perm[ix1 + perm[iy1 + perm[iz0]]], fx1, fy1, fz0);
            nxy1 = grad3(perm[ix1 + perm[iy1 + perm[iz1]]], fx1, fy1, fz1);
            nx1 = LERP(r, nxy0, nxy1);

            n1 = LERP(t, nx0, nx1);

            return 0.936f * (LERP(s, n0, n1));
        }


        //---------------------------------------------------------------------
        /** 4D float Perlin noise.
         */

        public float noise4(float x, float y, float z, float w)
        {
            int ix0, iy0, iz0, iw0, ix1, iy1, iz1, iw1;
            float fx0, fy0, fz0, fw0, fx1, fy1, fz1, fw1;
            float s, t, r, q;
            float nxyz0, nxyz1, nxy0, nxy1, nx0, nx1, n0, n1;

            ix0 = x > 0 ? (int)x : (int)x - 1;  //FASTFLOOR(x); // Integer part of x
            iy0 = y > 0 ? (int)y : (int)y - 1; //FASTFLOOR(y); // Integer part of y
            iz0 = z > 0 ? (int)z : (int)z - 1; //FASTFLOOR(z); // Integer part of z
            iw0 = w > 0 ? (int)w : (int)w - 1; //FASTFLOOR(w); // Integer part of w
            fx0 = x - ix0;        // Fractional part of x
            fy0 = y - iy0;        // Fractional part of y
            fz0 = z - iz0;        // Fractional part of z
            fw0 = w - iw0;        // Fractional part of w
            fx1 = fx0 - 1.0f;
            fy1 = fy0 - 1.0f;
            fz1 = fz0 - 1.0f;
            fw1 = fw0 - 1.0f;
            ix1 = (ix0 + 1) & 0xff;  // Wrap to 0..255
            iy1 = (iy0 + 1) & 0xff;
            iz1 = (iz0 + 1) & 0xff;
            iw1 = (iw0 + 1) & 0xff;
            ix0 = ix0 & 0xff;
            iy0 = iy0 & 0xff;
            iz0 = iz0 & 0xff;
            iw0 = iw0 & 0xff;

            q = FADE(fw0);
            r = FADE(fz0);
            t = FADE(fy0);
            s = FADE(fx0);

            nxyz0 = grad4(perm[ix0 + perm[iy0 + perm[iz0 + perm[iw0]]]], fx0, fy0, fz0, fw0);
            nxyz1 = grad4(perm[ix0 + perm[iy0 + perm[iz0 + perm[iw1]]]], fx0, fy0, fz0, fw1);
            nxy0 = LERP(q, nxyz0, nxyz1);

            nxyz0 = grad4(perm[ix0 + perm[iy0 + perm[iz1 + perm[iw0]]]], fx0, fy0, fz1, fw0);
            nxyz1 = grad4(perm[ix0 + perm[iy0 + perm[iz1 + perm[iw1]]]], fx0, fy0, fz1, fw1);
            nxy1 = LERP(q, nxyz0, nxyz1);

            nx0 = LERP(r, nxy0, nxy1);

            nxyz0 = grad4(perm[ix0 + perm[iy1 + perm[iz0 + perm[iw0]]]], fx0, fy1, fz0, fw0);
            nxyz1 = grad4(perm[ix0 + perm[iy1 + perm[iz0 + perm[iw1]]]], fx0, fy1, fz0, fw1);
            nxy0 = LERP(q, nxyz0, nxyz1);

            nxyz0 = grad4(perm[ix0 + perm[iy1 + perm[iz1 + perm[iw0]]]], fx0, fy1, fz1, fw0);
            nxyz1 = grad4(perm[ix0 + perm[iy1 + perm[iz1 + perm[iw1]]]], fx0, fy1, fz1, fw1);
            nxy1 = LERP(q, nxyz0, nxyz1);

            nx1 = LERP(r, nxy0, nxy1);

            n0 = LERP(t, nx0, nx1);

            nxyz0 = grad4(perm[ix1 + perm[iy0 + perm[iz0 + perm[iw0]]]], fx1, fy0, fz0, fw0);
            nxyz1 = grad4(perm[ix1 + perm[iy0 + perm[iz0 + perm[iw1]]]], fx1, fy0, fz0, fw1);
            nxy0 = LERP(q, nxyz0, nxyz1);

            nxyz0 = grad4(perm[ix1 + perm[iy0 + perm[iz1 + perm[iw0]]]], fx1, fy0, fz1, fw0);
            nxyz1 = grad4(perm[ix1 + perm[iy0 + perm[iz1 + perm[iw1]]]], fx1, fy0, fz1, fw1);
            nxy1 = LERP(q, nxyz0, nxyz1);

            nx0 = LERP(r, nxy0, nxy1);

            nxyz0 = grad4(perm[ix1 + perm[iy1 + perm[iz0 + perm[iw0]]]], fx1, fy1, fz0, fw0);
            nxyz1 = grad4(perm[ix1 + perm[iy1 + perm[iz0 + perm[iw1]]]], fx1, fy1, fz0, fw1);
            nxy0 = LERP(q, nxyz0, nxyz1);

            nxyz0 = grad4(perm[ix1 + perm[iy1 + perm[iz1 + perm[iw0]]]], fx1, fy1, fz1, fw0);
            nxyz1 = grad4(perm[ix1 + perm[iy1 + perm[iz1 + perm[iw1]]]], fx1, fy1, fz1, fw1);
            nxy1 = LERP(q, nxyz0, nxyz1);

            nx1 = LERP(r, nxy0, nxy1);

            n1 = LERP(t, nx0, nx1);

            return 0.87f * (LERP(s, n0, n1));
        }

        //---------------------------------------------------------------------
        /** 4D float Perlin periodic noise.
         */

        public float pnoise4(float x, float y, float z, float w,
                  int px, int py, int pz, int pw)
        {
            int ix0, iy0, iz0, iw0, ix1, iy1, iz1, iw1;
            float fx0, fy0, fz0, fw0, fx1, fy1, fz1, fw1;
            float s, t, r, q;
            float nxyz0, nxyz1, nxy0, nxy1, nx0, nx1, n0, n1;

            ix0 = x > 0 ? (int)x : (int)x - 1;  //FASTFLOOR(x); // Integer part of x
            iy0 = y > 0 ? (int)y : (int)y - 1; //FASTFLOOR(y); // Integer part of y
            iz0 = z > 0 ? (int)z : (int)z - 1; //FASTFLOOR(z); // Integer part of z
            iw0 = w > 0 ? (int)w : (int)w - 1; //FASTFLOOR(w); // Integer part of w
            fx0 = x - ix0;        // Fractional part of x
            fy0 = y - iy0;        // Fractional part of y
            fz0 = z - iz0;        // Fractional part of z
            fw0 = w - iw0;        // Fractional part of w
            fx1 = fx0 - 1.0f;
            fy1 = fy0 - 1.0f;
            fz1 = fz0 - 1.0f;
            fw1 = fw0 - 1.0f;
            ix1 = ((ix0 + 1) % px) & 0xff;  // Wrap to 0..px-1 and wrap to 0..255
            iy1 = ((iy0 + 1) % py) & 0xff;  // Wrap to 0..py-1 and wrap to 0..255
            iz1 = ((iz0 + 1) % pz) & 0xff;  // Wrap to 0..pz-1 and wrap to 0..255
            iw1 = ((iw0 + 1) % pw) & 0xff;  // Wrap to 0..pw-1 and wrap to 0..255
            ix0 = (ix0 % px) & 0xff;
            iy0 = (iy0 % py) & 0xff;
            iz0 = (iz0 % pz) & 0xff;
            iw0 = (iw0 % pw) & 0xff;

            q = FADE(fw0);
            r = FADE(fz0);
            t = FADE(fy0);
            s = FADE(fx0);

            nxyz0 = grad4(perm[ix0 + perm[iy0 + perm[iz0 + perm[iw0]]]], fx0, fy0, fz0, fw0);
            nxyz1 = grad4(perm[ix0 + perm[iy0 + perm[iz0 + perm[iw1]]]], fx0, fy0, fz0, fw1);
            nxy0 = LERP(q, nxyz0, nxyz1);

            nxyz0 = grad4(perm[ix0 + perm[iy0 + perm[iz1 + perm[iw0]]]], fx0, fy0, fz1, fw0);
            nxyz1 = grad4(perm[ix0 + perm[iy0 + perm[iz1 + perm[iw1]]]], fx0, fy0, fz1, fw1);
            nxy1 = LERP(q, nxyz0, nxyz1);

            nx0 = LERP(r, nxy0, nxy1);

            nxyz0 = grad4(perm[ix0 + perm[iy1 + perm[iz0 + perm[iw0]]]], fx0, fy1, fz0, fw0);
            nxyz1 = grad4(perm[ix0 + perm[iy1 + perm[iz0 + perm[iw1]]]], fx0, fy1, fz0, fw1);
            nxy0 = LERP(q, nxyz0, nxyz1);

            nxyz0 = grad4(perm[ix0 + perm[iy1 + perm[iz1 + perm[iw0]]]], fx0, fy1, fz1, fw0);
            nxyz1 = grad4(perm[ix0 + perm[iy1 + perm[iz1 + perm[iw1]]]], fx0, fy1, fz1, fw1);
            nxy1 = LERP(q, nxyz0, nxyz1);

            nx1 = LERP(r, nxy0, nxy1);

            n0 = LERP(t, nx0, nx1);

            nxyz0 = grad4(perm[ix1 + perm[iy0 + perm[iz0 + perm[iw0]]]], fx1, fy0, fz0, fw0);
            nxyz1 = grad4(perm[ix1 + perm[iy0 + perm[iz0 + perm[iw1]]]], fx1, fy0, fz0, fw1);
            nxy0 = LERP(q, nxyz0, nxyz1);

            nxyz0 = grad4(perm[ix1 + perm[iy0 + perm[iz1 + perm[iw0]]]], fx1, fy0, fz1, fw0);
            nxyz1 = grad4(perm[ix1 + perm[iy0 + perm[iz1 + perm[iw1]]]], fx1, fy0, fz1, fw1);
            nxy1 = LERP(q, nxyz0, nxyz1);

            nx0 = LERP(r, nxy0, nxy1);

            nxyz0 = grad4(perm[ix1 + perm[iy1 + perm[iz0 + perm[iw0]]]], fx1, fy1, fz0, fw0);
            nxyz1 = grad4(perm[ix1 + perm[iy1 + perm[iz0 + perm[iw1]]]], fx1, fy1, fz0, fw1);
            nxy0 = LERP(q, nxyz0, nxyz1);

            nxyz0 = grad4(perm[ix1 + perm[iy1 + perm[iz1 + perm[iw0]]]], fx1, fy1, fz1, fw0);
            nxyz1 = grad4(perm[ix1 + perm[iy1 + perm[iz1 + perm[iw1]]]], fx1, fy1, fz1, fw1);
            nxy1 = LERP(q, nxyz0, nxyz1);

            nx1 = LERP(r, nxy0, nxy1);

            n1 = LERP(t, nx0, nx1);

            return 0.87f * (LERP(s, n0, n1));
        }

        public float pfbm2(float x, float y, int px, int py, int octaves, float alpha, float omega)
        {
            float sum = 0f;
            float a = 1;
            float b = 1;
            for (int i = 1; i <= octaves; i++)
            {
                sum += (1f / a) * pnoise2(x * b, y * b, px, py);
                a *= alpha;
                b *= omega;

            }
            return Mathf.Clamp(sum * (1f - (1f / alpha)) * 1.4f, 0, 1);
        }

        public float fbm2(float x, float y, int octaves, float alpha, float omega)
        {
            float sum = 0f;
            float a = 1;
            float b = 1;
            for (int i = 1; i <= octaves; i++)
            {
                sum += (1f / a) * noise2(x * b, y * b);
                a *= alpha;
                b *= omega;

            }
            return Mathf.Clamp(sum * (1f - (1f / alpha)) * 1.4f, 0, 1);
        }

        //private static float[] fmb3Offset = new float[] {0f,     0f, .0737f, .1189f, .1440f, .1530f};
        //private static float[] fmb3Scale = new float[]  {0f, 1.066f, .8584f, .8120f, .8083f, .8049f };    
        private static float[] fmb3Offset = new float[] { 0f, 0f, .0737f, .1189f, .1440f, .1530f };
        private static float[] fmb3Scale = new float[] { 0f, 1.066f, .8584f, .8120f, .8083f, .8049f };
        public Color fbm3(float x, float y, float z, int octaves, float lacunarity, float gain)
        {

            float sum = 0f;
            float frequency = 1;
            float amplitude = 1;


            for (int i = 0; i < octaves; i++)
            {
                sum += frequency * noise3(x * amplitude, y * amplitude, z * amplitude);
                frequency /= lacunarity;
                amplitude *= gain;
            }

            sum = (sum - fmb3Offset[octaves]) * fmb3Scale[octaves];
            //clamp it
            if (sum > 1) sum = 1;
            if (sum < 0) sum = 0;
            //look it up in the gradient and return the color
            return Gradient[(int)(sum * (Gradient.Length - 1))];

        }



        public Color[] rescaleAndColorArray(float[] a, float min, float max, Color[] colors)
        {

            float offset = 0 - min;
            float scale = 1f / (max + offset);
            Color[] result = new Color[a.Length];

            for (int i = 0; i < a.Length; i++)
            {
                result[i] = colors[(int)(((a[i] + offset) * scale) * (colors.Length - 1))];
            }
            return result;
        }

        public Color[] rescaleAndColorArrayMenu(float[] a, float min, float max, Color[] colors)
        {

            float offset = 0 - min;
            float scale = 1f / (max + offset);
            Color[] result = new Color[a.Length];

            for (int i = 0; i < a.Length; i++)
            {
                if (result[i].a == 0)
                {
                    result[i] = new Color(colors[0].r, colors[0].g, colors[0].b, 1);
                }
                else
                {
                    result[i] = colors[(int)(((a[i] + offset) * scale) * (colors.Length - 1))];
                }

            }
            return result;
        }

        public Color[] rescaleArray(float[] a, float min, float max)
        {

            float offset = 0 - min;
            float scale = 1f / (max + offset);
            Color[] result = new Color[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                float x = (a[i] + offset) * scale;
                result[i] = new Color(1, 1, 1, x);
            }
            return result;
        }


        /*
                public NoiseOutput GenerateSurfaceSphereNoiseSIMD(int width, int height)
                {

                    float min = 0;
                    float max = 0;

                    IntPtr ptrNativeData = GetSphereSurfaceNoiseSIMD(width, height, settings.Octaves, settings.Lacunarity, settings.Frequency, settings.Gain, settings.Offset, (int)settings.NoiseType,out min, out max);
                    float[] floatColor = new float[width * height];
                    IntPtr p = ptrNativeData;



                    Marshal.Copy(p, floatColor, 0, width * height);            



                    CleanUpNoiseSIMD(ptrNativeData);
                    NoiseOutput output = new NoiseOutput(floatColor, min, max);

                    return output;
                }
                 */

        /// <summary>
        /// Creates a Texture2D that will wrap properly around a sphere
        /// </summary>
        /// <param name="width">width of texture</param>
        /// <param name="height">height of texture</param>
        /// <param name="LowColor">Starting color for the gradient</param>
        /// <param name="HighColor">Ending color for the gradient</param>
        /// <param name="GradientLookup">Optional custom gradient lookup table. High and Low color are ignored if this is supplied</param>
        /// <returns></returns>
        public unsafe Color[] GetColorForSphere(int width, int height, Color[] GradientLookup = null)
        {

            float min = 0;
            float max = 0;

           
            float* floatColors = GetSphereSurfaceNoiseSIMD(width, height, settings.Octaves, settings.Lacunarity, settings.Frequency, settings.Gain, settings.Offset,(int)settings.FractalType, (int)settings.NoiseType, &min, &max);
                                            

            //normalizing to [0..1]
            float offset = -min;
            float scale = 1.0f / (max - min);
                        
            Color[] colors = new Color[width * height];
            
            int l = GradientLookup.Length;
            scale = scale * l;
            int size = width * height;
            for (int i = 0; i < size; i++)
            {
                float n =  (floatColors[i] + offset ) * scale;
                int low = (int)n;
                float remainder = n - low;
                int lowIndex = low % l;
                //    int highIndex = (lowIndex + 1) % l;
                colors[i] = GradientLookup[lowIndex];//FastLerp(GradientLookup[lowIndex], GradientLookup[highIndex], remainder);
            
            }            
                      
            CleanUpNoiseSIMD(floatColors);


            return colors;
        }
        //ignores alpha, no bounds checking. because we live on the edge, here in perlin land
        public static Color FastLerp(Color c1, Color c2, float value)
        {

            return new Color(c1.r + (c2.r - c1.r) * value,
                                c1.g + (c2.g - c1.g) * value,
                                c1.b + (c2.b - c1.b) * value);
                                
        }

    }
}