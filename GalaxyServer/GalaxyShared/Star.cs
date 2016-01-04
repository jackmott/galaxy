using System;
using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public class Star
    {
        SolarSystem ParentSystem;
        public FastRandom rand;

        public const int O = 0;
        public const int B = 1;
        public const int A = 2;
        public const int F = 3;
        public const int G = 4;
        public const int K = 5;
        public const int M = 6;

        public static GalaxyColor[] StarColors =
            {   new GalaxyColor(149/255f, 71/255f, 254/255f),
                new GalaxyColor(123/255f, 109/255f, 252/255f),
                new GalaxyColor(186/255f, 179/255f, 253/255f),
                new GalaxyColor(255/255f, 255/255f, 255/255f),
                new GalaxyColor(255/255f, 247/255f, 85/255f),
                new GalaxyColor(240/255f, 96/255f, 0/255f),
                new GalaxyColor(250/255f, 18/255f, 5/255f)
            };

        [ProtoMember(1)]
        public int Type;
        [ProtoMember(2)]
        public float Size;

        [ProtoMember(3)]
        public GalaxyColor Color;

        //360/30 = 12 buckets
        //saturation value will map to StarTypeF
        public readonly static int[] hueToType =
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


        public readonly static int[][] typeToTypeDistribution =
        {
            new int[] {O,B,B,A,A,F,F,F,F,G},   //O
            new int[] {B,B,B,A,A,F,F,F,G,K},   //B
            new int[] {B,A,A,A,F,F,F,G,K,M},   //A
            new int[] {A,A,A,F,F,F,F,G,K,M},   //F
            new int[] {A,F,G,G,G,G,G,K,K,M},   //G
            new int[] {F,G,G,G,K,K,K,M,M,M},   //K
            new int[] {G,G,K,K,K,M,M,M,M,M}   //M            
        };

        public readonly static int[][] typeToSizeDistribution =
        {
            new int[] {8,8,8,8,8,8,9,9,9,10}, //O
            new int[] {6,6,6,6,6,6,7,7,7,8 }, //B
            new int[] {4,4,4,4,4,5,5,5,5,6 }, //A
            new int[] {3,3,3,3,3,4,4,4,4,5 }, //F
            new int[] {2,2,2,2,2,3,3,3,3,4 }, //G
            new int[] {1,1,1,2,2,2,2,2,3,3 }, //K
            new int[] {1,1,1,1,1,1,1,1,1,2 }, //M
        };


        public Star() { }

        public Star(SolarSystem parentSystem, FastRandom rand)
        {
            
            ParentSystem = parentSystem;
            Sector s = ParentSystem.ParentSector;
            Type = typeToTypeDistribution[s.DominantStarType][rand.Next(0, 10)];
            Size = typeToSizeDistribution[Type][rand.Next(0, 10)];
            Color = StarColors[(int)Type];
            
        }

    }
}
