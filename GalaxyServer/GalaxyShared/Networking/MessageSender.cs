﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyShared.Networking.Messages;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace GalaxyShared.Networking
{
    public class MessageSender
    {
        public static GalaxyMessage PrepareForServerSend(object o)
        {
            GalaxyMessage m = new GalaxyMessage(false);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            binaryFormatter.Serialize(stream, o);
            m.Buffer = stream.ToArray();
            m.Size = m.Buffer.Length;
            return m;
        }
    }
}