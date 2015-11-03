using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
}
