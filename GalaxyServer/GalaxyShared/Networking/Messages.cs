using ProtoBuf;
using System.Net.Sockets;
using XnaGeometry;
using System;

namespace GalaxyShared
{    
    public interface IMessage
    {
        void Proto(NetworkStream stream,byte[] typeBuffer);
        void AcceptHandler(IMessageHandler handler,object o);
        
        
    }
    [ProtoContract]
    public struct LoginMessage  : IMessage
    {
        [ProtoMember(1)]
        public string UserName;
        [ProtoMember(2)]
        public string Password;
                
        public void Proto(NetworkStream stream,byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.LoginMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix<LoginMessage>(stream, this,PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler,object o)
        {
            handler.HandleMessage(this,o);
        }

                        
    }

    [ProtoContract]
    public struct LoginResultMessage : IMessage
    {
        [ProtoMember(1)]
        public bool success;

        public void AcceptHandler(IMessageHandler handler, object o)
        {
            throw new NotImplementedException();
        }

        public void Proto(NetworkStream stream, byte[] typeBuffer)
        {
            throw new NotImplementedException();
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

        public void Proto(NetworkStream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.NewUserMessage;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix<NewUserMessage>(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o)
        {
            handler.HandleMessage(this, o);
        }
        /*
public void ToBytes(ref byte[] buffer)
{
   int size = Marshal.SizeOf(this);
   IntPtr ptr = Marshal.AllocHGlobal(size);
   Marshal.StructureToPtr(this, ptr, true);
   Marshal.Copy(ptr, buffer, 0, size);
   Marshal.FreeHGlobal(ptr);
}

public static NewUserMessage FromBytes(byte[] buffer)
{
   GCHandle pinnedPacket = GCHandle.Alloc(buffer, GCHandleType.Pinned);
   NewUserMessage msg = (NewUserMessage)Marshal.PtrToStructure(pinnedPacket.AddrOfPinnedObject(),typeof(NewUserMessage));
   pinnedPacket.Free();
   return msg;
}*/
    }

    [ProtoContract]
    public struct NewUserResultMessage : IMessage
    {
        [ProtoMember(1)]
        public bool success;

        public void AcceptHandler(IMessageHandler handler, object o)
        {
            throw new NotImplementedException();
        }

        public void Proto(NetworkStream stream, byte[] typeBuffer)
        {
            throw new NotImplementedException();
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

        public void Proto(NetworkStream stream, byte[] typeBuffer)
        {
            throw new NotImplementedException();
        }

        public void AcceptHandler(IMessageHandler handler, object o)
        {
            throw new NotImplementedException();
        }
    }

    [ProtoContract]
    public struct GoToWarpMessage : IMessage
    {
        [ProtoMember(1)]
        public Quaternion Rotation;
        [ProtoMember(2)]
        public Location Location;

        public void Proto(NetworkStream stream, byte[] typeBuffer)
        {
            throw new NotImplementedException();
        }

        public void AcceptHandler(IMessageHandler handler, object o)
        {
            throw new NotImplementedException();
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
        public SolarSystem System;

        public void Proto(NetworkStream stream, byte[] typeBuffer)
        {
            throw new NotImplementedException();
        }

        public void AcceptHandler(IMessageHandler handler, object o)
        {
            throw new NotImplementedException();
        }
    }

    [ProtoContract]
    public struct CargoStateMessage : IMessage
    {
        [ProtoMember(1)]
        public bool add;
        [ProtoMember(2)]
        public Item item;

        public void AcceptHandler(IMessageHandler handler, object o)
        {
            throw new NotImplementedException();
        }

        public void Proto(NetworkStream stream, byte[] typeBuffer)
        {
            throw new NotImplementedException();
        }
    }

    [ProtoContract]
    public struct InputMessage : IMessage
    {
        [ProtoMember(1)]
        public int Seq;
        [ProtoMember(2)]
        public float DeltaTime; //seconds
        [ProtoMember(3)]
        public float XTurn;
        [ProtoMember(4)]
        public float YTurn;
        [ProtoMember(5)]
        public float RollTurn;
        [ProtoMember(6)]
        public float Throttle;
        [ProtoMember(7)]
        public bool PrimaryButton;
        [ProtoMember(8)]
        public bool SecondaryButton;

        public void Proto(NetworkStream stream, byte[] typeBuffer)
        {
            throw new NotImplementedException();
        }

        public void AcceptHandler(IMessageHandler handler, object o)
        {
            throw new NotImplementedException();
        }
    }
}
