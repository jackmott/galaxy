﻿using XnaGeometry;
using ProtoBuf;
using System.IO;
namespace GalaxyShared
{
    [ProtoContract]
    public class Asteroid : IMessage
    {
        
        public SolarSystem ParentSystem;      
        public object GameObject;

        public const float CLIENT_SIZE_MULTIPLIER = 1f / 2550f * Planet.EARTH_CONSTANT;
        //model dimensions are 10x base localscale
        public const float SERVER_SIZE_MULTIPLIER = CLIENT_SIZE_MULTIPLIER * 10;
        
        [ProtoMember(1)]
        public byte Size;        
        [ProtoMember(2)]
        public byte Remaining = 100;
        [ProtoMember(3)]
        public byte Type = 0;
        [ProtoMember(4)]
        public int Hash;


        //trick to shrink Asteroid down to packed float size:        
        public Vector3 Pos;
       [ProtoMember(5,IsPacked = true,OverwriteList =true)]
        private float[] PackedPos
        {
            get { return new float[] { (float)Pos.X, (float)Pos.Y, (float)Pos.Z }; }
            set { Pos.X = value[0]; Pos.Y = value[1]; Pos.Z = value[2]; }
        }
        public Asteroid(SolarSystem parentSystem, byte orbit, float orbitAngle, byte size, Vector3 posAdjust, int hash)
        {
            this.Hash = hash;
            ParentSystem = parentSystem;                   
           
            Size = size;

            Vector3 start = Vector3.Zero;                                    
            Matrix rotation = Matrix.CreateFromYawPitchRoll(orbitAngle,0,0);
            Pos = start + Vector3.Transform(Vector3.Forward *orbit*Planet.EARTH_CONSTANT*Planet.ORBIT_MULTIPLIER, rotation);
          
            Pos += posAdjust;
          

        }
        

        public Asteroid()
        {
         
        }

       


        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.Asteroid;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }

    }
}
