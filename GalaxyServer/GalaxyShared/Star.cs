using System;
using System.Drawing;

namespace GalaxyShared
{

    public class Star
    {
        GalaxySector parentSector;

        public const int O = 0;
        public const int B = 1;
        public const int A = 2;
        public const int F = 3;
        public const int G = 4;
        public const int K = 5;
        public const int M = 6;

        public int type;
        public float size;
        


        //360/30 = 12 buckets
        //saturation value will map to StarTypeF
        public static int[] hueToType =
        {
            M,  // red   0
            K,  // orange  30
            G,  // yellow  60
            G,  // yellow  90
            G, // yellow  120
            A,  // blue/white        150
            A,  // blue/white 180
            B,  //blue 210
            B, //blue 240
            O, //ultraviolet 270
            O, // ultra 300
            M // red 330
        };


        public static int[][] typeToTypeDistribution =
        {
            new int[] {O,B,B,A,A,F,F,F,F,G},   //O
            new int[] {B,B,B,A,A,F,F,F,G,K},   //B
            new int[] {B,A,A,A,F,F,F,G,K,M},   //A
            new int[] {A,A,A,F,F,F,F,G,K,M},   //F
            new int[] {A,F,G,G,G,G,G,K,K,M},   //G
            new int[] {F,G,G,G,K,K,K,M,M,M},   //K
            new int[] {G,G,K,K,K,M,M,M,M,M}   //M            
        };

        public static int[][] typeToSizeDistribution =
        {
            new int[] {8,8,8,8,8,8,9,9,9,10}, //O
            new int[] {6,6,6,6,6,6,7,7,7,8 }, //B
            new int[] {4,4,4,4,4,5,5,5,5,6 }, //A
            new int[] {3,3,3,3,3,4,4,4,4,5 }, //F
            new int[] {2,2,2,2,2,3,3,3,3,4 }, //G
            new int[] {1,1,1,2,2,2,2,2,3,3 }, //K
            new int[] {1,1,1,1,1,1,1,1,1,2 }, //M
        };

        


        public Star(GalaxySector parentSector,  Random r)
        {
            this.parentSector = parentSector;                           
            type = typeToTypeDistribution[parentSector.dominantStarType][r.Next(0, 10)];
            size = typeToSizeDistribution[type][r.Next(0, 10)];

            
        }

    }
}
