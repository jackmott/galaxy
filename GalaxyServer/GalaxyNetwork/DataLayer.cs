using System;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.MsgPack;
using StackExchange.Redis.Extensions.Core;
using GalaxyShared.Networking.Messages;
using GalaxyShared;
using System.Collections.Concurrent;

namespace GalaxyServer
{

    public class DataLayer
    {        
        MsgPackObjectSerializer Serializer;
        StackExchangeRedisCacheClient CacheClient;
        


        public DataLayer()
        {
            
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("pub-redis-19614.us-east-1-4.5.ec2.garantiadata.com:19614,password=5kmshut");            
            Serializer = new MsgPackObjectSerializer();
            Serializer.Serialize(new NewUserMessage("sdfsd","Sfsd"));
            CacheClient = new StackExchangeRedisCacheClient(redis, Serializer);
            Console.WriteLine("Database:" + CacheClient.Database.IsConnected("Sdfsdf"));

        }

        public void HandleLoginMessage(LoginMessage msg, GalaxyClient client)
        {
            Console.WriteLine("HandleLoginMessage");
            try
            {
                GalaxyPlayerLogin login = CacheClient.Get<GalaxyPlayerLogin>(msg.UserName);
                if (login == null)
                {
                    BooleanResultMessage b;
                    b.success = false;
                    GalaxyServer.Send(client, b);
                    return;
                }
                Console.WriteLine("User " + login.UserName + " Found");
                if (login.Password == msg.Password)
                {
                    GalaxyServer.SendSuccess(client);
                }
                else
                {
                    GalaxyServer.SendFailure(client);
                }
                
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }           
        }

       public void HandleNewUserMessage(NewUserMessage msg, GalaxyClient client)
        {
            Console.WriteLine("HandleNewUserMessage");
            GalaxyPlayerLogin login = new GalaxyPlayerLogin(msg.UserName, msg.Password);
            if (CacheClient.Add(login.UserName, login))
            {
                GalaxyServer.SendSuccess(client);
            }
            else
            {
                GalaxyServer.SendFailure(client);
            }
        }


    }
}
