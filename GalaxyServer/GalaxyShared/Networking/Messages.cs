using ProtoBuf;
using XnaGeometry;


namespace GalaxyShared
{    

    [ProtoContract]
    public struct LoginMessage  
    {
        [ProtoMember(0)]
        public string UserName;
        [ProtoMember(1)]
        public string Password;                     
    }

    [ProtoContract]
    public struct LoginResultMessage  
    {
        [ProtoMember(0)]
        public bool success;
    }

    [ProtoContract]
    public struct NewUserMessage  
    {
        [ProtoMember(0)]
        public string UserName;
        [ProtoMember(1)]
        public string Password;

        public NewUserMessage(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
       
    }

    [ProtoContract]
    public struct NewUserResultMessage  
    {
        [ProtoMember(0)]
        public bool success;    
    }

    [ProtoContract]
    public struct PlayerStateMessage  
    {
        [ProtoMember(0)]
        public long Seq;
        [ProtoMember(1)]
        public Vector3 PlayerPos;
        [ProtoMember(2)]
        public Quaternion Rotation;
        [ProtoMember(3)]
        public float Throttle;
      
    }

    [ProtoContract]
    public struct GoToWarpMessage  
    {
        [ProtoMember(0)]
        public Quaternion Rotation;
        [ProtoMember(1)]
        public Location Location;
      
    }

    [ProtoContract]
    public struct DropOutOfWarpMessage  
    {
        [ProtoMember(0)]
        public Quaternion Rotation;
        [ProtoMember(1)]
        public Location Location;
        [ProtoMember(2)]
        public SolarSystem System;
      
    }

    [ProtoContract]
    public struct CargoStateMessage  
    {
        [ProtoMember(0)]
        public bool add;
        [ProtoMember(1)]
        public Item item;
    
    }

    [ProtoContract]
    public struct InputMessage  
    {
        [ProtoMember(0)]
        public int Seq;
        [ProtoMember(1)]
        public float DeltaTime; //seconds
        [ProtoMember(2)]
        public float XTurn;
        [ProtoMember(3)]
        public float YTurn;
        [ProtoMember(4)]
        public float RollTurn;
        [ProtoMember(5)]
        public float Throttle;
        [ProtoMember(6)]
        public bool PrimaryButton;
        [ProtoMember(7)]
        public bool SecondaryButton;
       
    }
}
