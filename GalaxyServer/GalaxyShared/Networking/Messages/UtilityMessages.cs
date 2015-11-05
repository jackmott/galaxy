using System;
using XnaGeometry;


namespace GalaxyShared.Networking.Messages
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
        public Vector3 PlayerPos;
        public Quaternion Rotation;
        public float Throttle;
    }
}
