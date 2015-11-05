using System;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.MsgPack;
using StackExchange.Redis.Extensions.Core;
using GalaxyShared.Networking.Messages;
using GalaxyShared;


namespace GalaxyServer
{

    public class DataLayer
    {
        private static MsgPackObjectSerializer Serializer;
        private static StackExchangeRedisCacheClient CacheClient;

        private const string LOGIN = "login:";
        private const string GALAXY_PLAYER = "galaxyplayer:";


        public DataLayer()
        {

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            Serializer = new MsgPackObjectSerializer();
            CacheClient = new StackExchangeRedisCacheClient(redis, Serializer);
            Console.WriteLine("Database:" + CacheClient.Database.IsConnected("Sdfsdf"));

        }

        public static GalaxyPlayerLogin GetLogin(string username)
        {
            return CacheClient.Get<GalaxyPlayerLogin>(LOGIN+username);
        }

        public static bool CreateNewLogin(string username,string password)
        {
            return CacheClient.Add(LOGIN+username,password);
        }

        public static GalaxyPlayer GetGalaxyPlayer(string username)
        {
            return CacheClient.Get<GalaxyPlayer>(GALAXY_PLAYER + username);
        }

        public static bool UpdateGalaxyPlayer(GalaxyPlayer player)
        {            
            return CacheClient.Add(GALAXY_PLAYER + player.UserName, player);
        }
    }
}
