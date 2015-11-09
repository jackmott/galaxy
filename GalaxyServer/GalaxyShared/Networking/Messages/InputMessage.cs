using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    [Serializable]
    public struct InputMessage
    {
        public int Seq; 
        public float DeltaTime; //seconds
        public float XTurn;
        public float YTurn;
        public float RollTurn;
        public float Throttle;
        public bool GoToWarp;
    }
}
