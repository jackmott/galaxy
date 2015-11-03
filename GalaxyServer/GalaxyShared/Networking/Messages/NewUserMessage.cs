using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared.Networking.Messages
{
    [Serializable]
    public class NewUserMessage
    {
        public string UserName;
        public string Password;

        public NewUserMessage(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
