using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GalaxyShared;
using GalaxyShared.Networking;
using GalaxyShared.Networking.Messages;
using System.Threading;

public class NetworkManager : MonoBehaviour
{

    private static TcpClient socket;
    private static NetworkStream stream;

    public static Queue messageQueue;

    private Thread NetworkThread;



    void Awake()
    {
        Queue q = new Queue();
        messageQueue = Queue.Synchronized(q);

        DontDestroyOnLoad(this);
        socket = new TcpClient("localhost", 8888);
        socket.NoDelay = true;
        stream = socket.GetStream();
        NetworkThread = new Thread(new ThreadStart(NetworkReadLoop));
        NetworkThread.Start();
    }
    // Use this for initialization
    void Start()
    {

    }

    void OnApplicationQuit()
    {
        NetworkThread.Abort();
    }

    // Update is called once per frame
    void Update()
    {

        processMessages();

    }

    private void processMessages()
    {
        TypeDictionary TypeDictionary = new TypeDictionary();
        while (messageQueue.Count > 0)
        {
            object o = messageQueue.Dequeue();
            TypeDictionary.MsgType type = TypeDictionary.GetID(o);

            switch (type)
            {

                case TypeDictionary.MsgType.LoginResultMessage:
                    HandleLoginResultMessage((LoginResultMessage)o);
                    break;
                case TypeDictionary.MsgType.NewUserResultMessage:
                    HandleNewUserResultMessage((NewUserResultMessage)o);
                    break;
                case TypeDictionary.MsgType.GalaxyPlayer:
                    HandleGalaxyPlayerMessage((GalaxyPlayer)o);
                    break;
                case TypeDictionary.MsgType.PlayerStateMessage:
                    HandlePlayerStateMessage((PlayerStateMessage)o);
                    break;
                default:
                    Console.WriteLine("unknown message");
                    break;
            }
        }
    }

    public static void Send(object msg)
    {
        GalaxyMessage m = NetworkUtils.PrepareForServerSend(msg);
        stream.Write(m.SizeBuffer, 0, m.SizeBuffer.Length);
        stream.Write(m.Buffer, 0, m.Size);
        Debug.Log("Sent One Message");
    }

    public static void SendInputs(List<InputMessage> l)
    {
        Send(l);
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
        ClientSolarSystem.PlayerStartPos = Utility.UVector(player.PlayerPos);
        Application.LoadLevel("SolarSystem");

    }

    public void HandlePlayerStateMessage(PlayerStateMessage p)
    {
        Camera.main.transform.rotation = Utility.UQuaternion(p.Rotation);
        Camera.main.transform.position = Utility.UVector(p.PlayerPos);
        Debug.Log(p.PlayerPos);
        SystemMovement.throttle = p.Throttle;
    }
}
