using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared.Networking.Messages
{
    [Serializable]
    public struct InputMessage
    {
        public float DeltaTime; //seconds
        public float XTurn;
        public float YTurn;
        public float RollTurn;
        public float Throttle;
    }
}
