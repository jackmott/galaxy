using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;

public class NetworkTest : MonoBehaviour {
    List<TcpClient> clients;
	// Use this for initialization
	void Start () {
        Debug.Log("test");
        clients = new List<TcpClient>();
        for (int i = 0; i < 100; i++)
        {
            clients.Add(new TcpClient("localhost", 8888));
        }

        foreach (TcpClient client in clients)
        {
            for (int i = 0; i < 100; i++)
            {
                client.GetStream().WriteByte(42);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
