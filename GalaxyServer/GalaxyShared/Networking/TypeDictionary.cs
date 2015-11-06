using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyShared;

namespace GalaxyShared
{
    public class TypeDictionary
    {
        Dictionary<Type, int> Dictionary;

        public enum MsgType {
            LoginMessage,
            NewUserMessage,
            LoginResultMessage,
            NewUserResultMessage,
            GalaxyPlayer,
            ListOfInputMessage,
            PlayerStateMessage
        }

        public TypeDictionary()
        {
            Dictionary = new Dictionary<Type, int>();            
            Dictionary.Add(typeof(LoginMessage), 0);
            Dictionary.Add(typeof(NewUserMessage), 1);
            Dictionary.Add(typeof(LoginResultMessage), 2);
            Dictionary.Add(typeof(NewUserResultMessage), 3);
            Dictionary.Add(typeof(GalaxyPlayer), 4);
            Dictionary.Add(typeof(List<InputMessage>), 5);
            Dictionary.Add(typeof(PlayerStateMessage), 6);
        }

        public MsgType GetID(object o)
        {
            int id = -1;
            while(!Dictionary.TryGetValue(o.GetType(), out id)) { }
            return (MsgType)id;
        }
    }
}
