using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyShared.Networking.Messages;

namespace GalaxyShared.Networking
{
    public class TypeDictionary
    {
        Dictionary<Type, int> Dictionary;

        public TypeDictionary()
        {
            Dictionary = new Dictionary<Type, int>();            
            Dictionary.Add(typeof(LoginMessage), 0);
            Dictionary.Add(typeof(NewUserMessage), 1);

        }

        public int GetID(object o)
        {
            int id = -1;
            Dictionary.TryGetValue(o.GetType(), out id);
            return id;
        }
    }
}
