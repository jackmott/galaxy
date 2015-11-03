using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared.Networking.Messages
{
    [Serializable]
    public struct BooleanResultMessage
    {
        public bool success;
    }
}
