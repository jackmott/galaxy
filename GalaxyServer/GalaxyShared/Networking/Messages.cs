using ProtoBuf;
using XnaGeometry;
using System.IO;
using System;
using System.Collections.Generic;

namespace GalaxyShared
{
    public enum MsgType
    {
        LoginMessage,
        NewUserMessage,
        LoginResultMessage,
        NewUserResultMessage,
        Player,
        InputMessage,
        PlayerStateMessage,
        GoToWarpMessage,
        DropOutOfWarpMessage,
        Asteroid,
        Ship,
        MiningMessage,
        ConstructionMessage
    }

    public interface IMessage
    {
        void Proto(Stream stream, byte[] typeBuffer);
        void AcceptHandler(IMessageHandler handler, object o = null);
    }

    [ProtoContract]
    public struct LoginMessage : IMessage
    {
        [ProtoMember(1)]
        public string UserName;
        [ProtoMember(2)]
        public string Password;

        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.LoginMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }


    }

    [ProtoContract]
    public struct LoginResultMessage : IMessage
    {
        [ProtoMember(1)]
        public bool success;

        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.LoginResultMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }
    }

    [ProtoContract]
    public struct NewUserMessage : IMessage
    {
        [ProtoMember(1)]
        public string UserName;
        [ProtoMember(2)]
        public string Password;

        public NewUserMessage(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.NewUserMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o)
        {
            handler.HandleMessage(this, o);
        }

    }

    [ProtoContract]
    public struct NewUserResultMessage : IMessage
    {
        [ProtoMember(1)]
        public bool success;

        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.NewUserResultMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }
    }

    [ProtoContract]
    public struct PlayerStateMessage : IMessage
    {
        [ProtoMember(1)]
        public long Seq;
        [ProtoMember(2)]
        public Vector3 PlayerPos;
        [ProtoMember(3)]
        public Quaternion Rotation;
        [ProtoMember(4)]
        public float Throttle;



        public override string ToString()
        {
            return "pm - Seq:" + Seq + ", Pos:" + PlayerPos + ", Throttle:" + Throttle;

        }
        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.PlayerStateMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }
    }

    [ProtoContract]
    public struct GoToWarpMessage : IMessage
    {
        [ProtoMember(1)]
        public Quaternion Rotation;
        [ProtoMember(2)]
        public Location Location;

        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.GoToWarpMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }
    }

    [ProtoContract]
    public struct DropOutOfWarpMessage : IMessage
    {
        [ProtoMember(1)]
        public Quaternion Rotation;
        [ProtoMember(2)]
        public Location Location;
        [ProtoMember(3)]
        public string SystemKey;
        [ProtoMember(4)]
        public SolarSystem System;
        [ProtoMember(5)]
        public int SystemIndex;
        [ProtoMember(6)]
        public SectorCoord SectorCoord;
       

        public void Proto(Stream stream, byte[] typeBuffer)
        {

            typeBuffer[0] = (byte)MsgType.DropOutOfWarpMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }
    }

    [ProtoContract]
    public struct MiningMessage : IMessage
    {
        [ProtoMember(1)]
        public bool Add;
        [ProtoMember(2)]
        public Item Item;
        [ProtoMember(3)]
        public int AsteroidHash;
        [ProtoMember(4)]
        public byte Remaining;

        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.MiningMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }
    }

    [ProtoContract]
    public struct ConstructionMessage : IMessage
    {
        [ProtoMember(1)]
        public bool Start;
        [ProtoMember(2)]
        public bool End;
        [ProtoMember(3)]
        public byte Progress;
        [ProtoMember(4)]
        public byte Speed;
        [ProtoMember(5)]
        public string ClassName;
        [ProtoMember(6)]
        public List<BuildRequirement> ResourcesNeeded;

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }

        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.ConstructionMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }
    }

    [ProtoContract]
    public struct InputMessage : IMessage
    {
        [ProtoMember(1)]
        public long Seq;
        [ProtoMember(2)]
        public float Yaw;
        [ProtoMember(3)]
        public float Pitch;
        [ProtoMember(4)]
        public float Roll;
        [ProtoMember(5)]
        public float Throttle;
        [ProtoMember(6)]
        public bool PrimaryButton;
        [ProtoMember(7)]
        public bool SecondaryButton;

        public double DeltaTime; //ms
        public bool ClientOnly;

        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.InputMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);

        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }

        public override string ToString()
        {
            string result = "";
            result += "InputMessage: Seq:" + Seq + ",thorttle:" + Throttle + ", deltat:" + DeltaTime;
            return result;
        }
    }
}
