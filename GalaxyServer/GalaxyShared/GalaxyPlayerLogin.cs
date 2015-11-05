﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    
    public class GalaxyPlayerLogin
    {
        public string UserName;
        public string Password;

        public GalaxyPlayerLogin(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public GalaxyPlayerLogin()
        {
            //required for MsgPack Serializer
        }
    }
}