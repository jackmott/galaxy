using System;
using XnaGeometry;


namespace GalaxyShared
{

    [Serializable]
    public struct LoginMessage
    {
        public string UserName;
        public string Password;

        public LoginMessage(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }

    [Serializable]
    public struct LoginResultMessage
    {
        public bool success;
    }


    [Serializable]
    public struct NewUserMessage
    {
        public string UserName;
        public string Password;

        public NewUserMessage(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }

    [Serializable]
    public struct NewUserResultMessage
    {
        public bool success;
    }

    [Serializable]
    public struct PlayerStateMessage
    {
        public long Seq;
        public Vector3 PlayerPos;
        public Quaternion Rotation;
        public float Throttle;
    }

    [Serializable]
    public struct GoToWarpMessage
    {
        public Quaternion Rotation;
        public Location Location;
    }

    [Serializable]
    public struct DropOutOfWarpMessage
    {
        public Quaternion Rotation;
        public Location Location;
        public SolarSystem System;
    }

   [Serializable]
   public struct CargoStateMessage
    {
        public bool add;
        public Item item;
    }
}
