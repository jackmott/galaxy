using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;

public class NetworkTest : MonoBehaviour {
    List<TcpClient> clients;
	// Use this for initialization
	void Start () {
        Debug.Log("test");
        clients = new List<TcpClient>();
        for (int i = 0; i < 5; i++)
        {
            clients.Add(new TcpClient("localhost", 8888));
        }

        foreach (TcpClient client in clients)
        {
            for (int i = 0; i < 1000; i++)
            {
                client.GetStream().WriteByte(3);
                client.GetStream().WriteByte(0);
                client.GetStream().WriteByte(42);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
