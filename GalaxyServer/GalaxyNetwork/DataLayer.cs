﻿using System;
using StackExchange.Redis;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using GalaxyShared;


namespace GalaxyServer
{

    public class DataLayer
    {
        


        private const string LOGIN = "login:";
        private const string GALAXY_PLAYER = "galaxyplayer:";

        public const long PLAYER_STATE_PERSIST_RATE = 10000;//ms

        private static IDatabase DB;
        private static IFormatter Serializer;


        public DataLayer()
        {

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            DB = redis.GetDatabase();
            Serializer = new BinaryFormatter();
            

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
            return (T)Serializer.Deserialize(stream);
                
        }

        //puts object in redis
        public static bool Add(string key, object value)
        {
            MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream,value);
            byte[] bytes = stream.GetBuffer();
            return DB.StringSet(key, bytes);
        }

        public static PlayerLoginMessage GetLogin(string username)
        {
            return Get<PlayerLoginMessage>(LOGIN+username);
        }

        public static bool CreateNewLogin(string username,string password)
        {
            PlayerLoginMessage login = new PlayerLoginMessage(username, password);
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
