using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GalaxyShared;
using GalaxyShared.Networking;
using GalaxyShared.Networking.Messages;
using System.Threading;

public class NetworkManager : MonoBehaviour {

    private static TcpClient socket;
    private static NetworkStream stream;

    public static Queue messageQueue;

    void Awake()
    {
        Queue q = new Queue();
        messageQueue = Queue.Synchronized(q);

        DontDestroyOnLoad(this);
        socket = new TcpClient("localhost", 8888);
        socket.NoDelay = true;
        stream = socket.GetStream();
        Thread t = new Thread(new ThreadStart(NetworkReadLoop));
        t.Start();        
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        processMessages();
	
	}

    private void processMessages()
    {
        TypeDictionary TypeDictionary = new TypeDictionary();
        while (messageQueue.Count > 0)
        {
            object o = messageQueue.Dequeue();
            int type = TypeDictionary.GetID(o);

            switch (type)
            {
                case 0:
                    //todo
                    break;
                case 1:
                    //todo
                    break;
                case 2:
                    HandleLoginResultMessage((LoginResultMessage)o);
                    break;
                case 3:
                    HandleNewUserResultMessage((NewUserResultMessage)o);
                    break;
                case 4:
                    HandleGalaxyPlayerMessage((GalaxyPlayer)o);
                    break;
                default:
                    Console.WriteLine("unknown message");
                    break;
            }
        }
    }

    public static void Send(object msg)
    {
        GalaxyMessage m = MessageSender.PrepareForServerSend(msg);
        stream.Write(m.SizeBuffer, 0, m.SizeBuffer.Length);
        stream.Write(m.Buffer, 0, m.Size);
        Debug.Log("Sent One Message");
    }

    public void NetworkReadLoop()
    {        
        IFormatter binaryFormatter = new BinaryFormatter();        
        while (true)
        {
            object o = binaryFormatter.Deserialize(stream);
            messageQueue.Enqueue(o);
            Debug.Log("Got an object! Enqued it.");            
        }

    }


    public void HandleLoginResultMessage(LoginResultMessage msg)
    {
        //show error
    }

    public void HandleNewUserResultMessage(NewUserResultMessage msg)
    {
        //show error
    }

    public void HandleGalaxyPlayerMessage(GalaxyPlayer player)
    {
        Debug.Log("handle galaxyplayer message");
        GalaxyGen gen = new GalaxyGen();
        GalaxySector s = gen.GetSector(player.SectorPos, 1);
        ClientSolarSystem.SolarSystem = s.Systems[player.SystemIndex];
        ClientSolarSystem.PlayerStartPos = new Vector3(player.PlayerPos.X, player.PlayerPos.Y, player.PlayerPos.Z);
        Application.LoadLevel("SolarSystem");

    }
}
