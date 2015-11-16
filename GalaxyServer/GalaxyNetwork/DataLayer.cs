
using StackExchange.Redis;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using GalaxyShared;
using XnaGeometry;
using ProtoBuf;

namespace GalaxyServer
{

    public class DataLayer
    {
        


        private const string LOGIN = "login:";
        private const string GALAXY_PLAYER = "galaxyplayer:";
        private const string SYSTEM = "system:";

        public const long PLAYER_STATE_PERSIST_RATE = 10000;//ms

        private static IDatabase DB;
        


        public DataLayer()
        {

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            DB = redis.GetDatabase();
            
            

        }

        //Gets object from redis
        public static T Get<T>(string key)
        {
            RedisValue bytes = DB.StringGet(key);
            if (!bytes.HasValue)
            {
                return default(T);
            }
            MemoryStream stream = new MemoryStream(bytes);
            return Serializer.Deserialize<T>(stream);
                
        }

        //puts object in redis
        public static bool Add(string key, object value)
        {
            MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream,value);
            byte[] bytes = stream.GetBuffer();
            return DB.StringSet(key, bytes);
        }

        public static LoginMessage GetLogin(string username)
        {
            return Get<LoginMessage>(LOGIN+username);
        }



        public static SolarSystem GetSystem(Vector3 pos)
        {
             return Get<SolarSystem>(SYSTEM+pos.ToString());
        }

        public static void AddSystem(SolarSystem system)
        {
            Add(SYSTEM + system.key(), system);
        }

        public static bool CreateNewLogin(string username,string password)
        {
            LoginMessage login;
            login.UserName = username;
            login.Password = password;
            return Add(LOGIN+username,login);
        }

        public static Player GetGalaxyPlayer(string username)
        {
            return Get<Player>(GALAXY_PLAYER + username);
        }

        public static bool UpdateGalaxyPlayer(Player player)
        {            
            return Add(GALAXY_PLAYER + player.UserName, player);
        }
    }
}
