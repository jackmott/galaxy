using System;
using System.Collections.Generic;

namespace GalaxyShared
{

    public enum MsgType
    {
        LoginMessage,
        NewUserMessage,
        LoginResultMessage,
        NewUserResultMessage,
        GalaxyPlayer,
        ListOfInputMessage,
        PlayerStateMessage,
        GoToWarpMessage,
        DropOutOfWarpMessage,
        Asteroid,
        Ship,
        CargoStateMessage
    }

    //singleton
    public class TypeDictionary
    {
        public static Type[] TypeLookUpArray;   //for performant deserialization
        public static Dictionary<Type,MsgType> TypeLookupDictionary = new Dictionary<Type,MsgType>();  //for human readable code
        private static TypeDictionary instance;

       

        public TypeDictionary()
        {
            if (instance == null)
            {
                Array values = Enum.GetValues(typeof(MsgType));

                int numMsgs = values.Length;
                TypeLookUpArray = new Type[numMsgs];
                int i = 0;
                foreach (MsgType msgType in values)
                {
                    string enumName = msgType.ToString();
                    Type t = Type.GetType(enumName);
                    TypeLookUpArray[i] = t;
                    TypeLookupDictionary.Add(t, msgType);
                    i++;
                }
                instance = this;
            }

        }

        public static MsgType GetID(object o)
        {
            MsgType m;
            TypeLookupDictionary.TryGetValue(o.GetType(),out m);
            return m;
        }
  
      
        public Type TypeResolver(int fieldNumber)
        {
            return TypeLookUpArray[fieldNumber];
        }
    }
}
