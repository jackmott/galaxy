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

    void Awake()
    {
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
            Debug.Log("Got an object!");
        }

    }
}
