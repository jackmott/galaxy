using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyServer
{


    class GalaxyGen
    {

        const ulong MAX_DISTANCE = 16777216;
        int inverseSystemDensity = 10; //inverse so we can use an int directly in rng
        MMHash3 hash;

        public GalaxyGen()
        {
            hash = new MMHash3(42);
            

        }

        //x/y/z coordinates by 1/10th of a light year
        public SolarSystem GetSystem(int x, int y, int z)
        {
            int result = GetRandom(x, y, z, inverseSystemDensity);
            return null;
        }


        public int GetRandom(int inx, int iny, int inz, int max)
        {
            byte[] x = BitConverter.GetBytes(inx);
            byte[] y = BitConverter.GetBytes(iny);
            byte[] z = BitConverter.GetBytes(inz);

            byte[] combined = new byte[12];

            combined[0] = x[0];
            combined[1] = x[1];
            combined[2] = x[2];
            combined[3] = x[3];
            combined[4] = y[0];
            combined[5] = y[1];
            combined[6] = y[2];
            combined[7] = y[3];
            combined[8] = x[0];
            combined[9] = x[1];
            combined[10] = x[2];
            combined[11] = x[3];

            byte[] result = hash.ComputeHash(combined);

            int a = BitConverter.ToInt32(result, 0);
            int b = BitConverter.ToInt32(result, 4);
            int c = BitConverter.ToInt32(result, 8);

            ulong r = (ulong)(a + b + c);

            return (int)(r % (ulong)max);


        }


    }
}
