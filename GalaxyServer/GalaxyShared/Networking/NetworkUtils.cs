using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace GalaxyShared
{ 
    public class NetworkUtils
    {

        public const int SERVER_TICK_RATE = 30; //ms
        public const int CLIENT_BUFFER_TIME = 100; //ms
        

        public static MessageWrapper PrepareForServerSend(object o)
        {
            MessageWrapper m = new MessageWrapper(false);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            binaryFormatter.Serialize(stream, o);
            m.Buffer = stream.ToArray();
            m.Size = m.Buffer.Length;
            return m;
        }
    }
}
