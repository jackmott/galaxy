using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    [Serializable]
    public class PlayerLoginMessage
    {
        public string UserName;
        public string Password;

        public PlayerLoginMessage(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

      
    }
}
