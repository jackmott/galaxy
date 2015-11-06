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

        messageQueue = new Queue();

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

        lock (messageQueue)
        {
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
    }

    public static void Send(object msg)
    {
        GalaxyMessage m = NetworkUtils.PrepareForServerSend(msg);
        stream.Write(m.SizeBuffer, 0, m.SizeBuffer.Length);
        stream.Write(m.Buffer, 0, m.Size);

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
            lock (messageQueue)
            {
                messageQueue.Enqueue(o);
            }

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
        SystemMovement.PlayerState = player;
        Application.LoadLevel("SolarSystem");

    }

    public void HandlePlayerStateMessage(PlayerStateMessage p)
    {
        if (SystemMovement.BufferedInputs != null)
        {
            lock (SystemMovement.BufferedInputs)
            {
                SystemMovement.BufferedInputs.RemoveAll(input => input.Seq <= p.Seq);

                SystemMovement.PlayerState.PlayerPos = p.PlayerPos;
                SystemMovement.PlayerState.Rotation = p.Rotation;
                foreach (InputMessage input in SystemMovement.BufferedInputs)
                {
                    Simulator.ProcessInput(SystemMovement.PlayerState, input);
                    Simulator.ContinuedPhysics(SystemMovement.PlayerState);
                }
            }
        }


    }
}
