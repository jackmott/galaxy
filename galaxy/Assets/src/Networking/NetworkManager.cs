using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GalaxyShared;
using System.Threading;
using System.Diagnostics;

public class NetworkManager : MonoBehaviour
{

    private static TcpClient socket;
    private static NetworkStream stream;
    public static Queue messageQueue;
    private Thread NetworkThread;

    public static float throttle = 0;
    public static List<InputMessage> InputsToSend;
    public static List<InputMessage> BufferedInputs;

    Stopwatch SendStopwatch = new Stopwatch();
    Stopwatch InputSampleStopwatch = new Stopwatch();

    public static GalaxyPlayer PlayerState = null;
    

    public static int Seq = 0;

    bool GoingToWarp = false;

    enum Level { MainMenu, Warp, System};
    Level CurrentLevel;


    void Awake()
    {
    
        messageQueue = new Queue();

        DontDestroyOnLoad(this);
        socket = new TcpClient("localhost", 8888);
        socket.NoDelay = true;
        stream = socket.GetStream();
        NetworkThread = new Thread(new ThreadStart(NetworkReadLoop));
        NetworkThread.Start();

        InputsToSend = new List<InputMessage>();
        BufferedInputs = new List<InputMessage>();

        InputSampleStopwatch.Start();
        SendStopwatch.Start();
       
    }
    // Use this for initialization
    void Start()
    {

    }

    void OnApplicationQuit()
    {
        socket.Close();
        NetworkThread.Abort();
    }

    void OnLevelWasLoaded(int level)
    {
        CurrentLevel = (Level)level;
        GoingToWarp = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GoingToWarp)
        {
            if (InputSampleStopwatch.ElapsedMilliseconds >= NetworkUtils.SERVER_TICK_RATE)
            {
                switch (CurrentLevel)
                {
                    case Level.System:
                        SampleSystemInput();
                        break;
                    case Level.Warp:
                        SampleWarpInput();
                        break;
                    default:
                        //nothin
                        break;
                }
                InputSampleStopwatch.Stop();
                InputSampleStopwatch.Reset();
                InputSampleStopwatch.Start();
            }

            if (SendStopwatch.ElapsedMilliseconds >= NetworkUtils.CLIENT_BUFFER_TIME)
            {
                if (InputsToSend.Count > 0)
                {
                    Send(InputsToSend);
                    InputsToSend.Clear();
                }
                SendStopwatch.Stop();
                SendStopwatch.Reset();
                SendStopwatch.Start();
            }
        }

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
                    case TypeDictionary.MsgType.GoToWarpMessage:
                        HandleGotoWarpMessage((GoToWarpMessage)o);
                        break;
                    case TypeDictionary.MsgType.DropOutOfWarpMessage:
                        HandleDropOutOfWarpMessage((DropOutOfWarpMessage)o);
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

    public void HandleGotoWarpMessage(GoToWarpMessage msg)
    {
        PlayerState.Location = msg.Location;
        PlayerState.Rotation = msg.Rotation;
        Application.LoadLevel((int)Level.Warp);        

    }

    public void HandleDropOutOfWarpMessage(DropOutOfWarpMessage msg)
    {
        Warp.ClosestSector.ParticleSystem.Clear();
        ClientSolarSystem.Cubemap = new Cubemap(2048, TextureFormat.ARGB32, false);
        bool work = Camera.main.RenderToCubemap(ClientSolarSystem.Cubemap);
        PlayerState.Location = msg.Location;
        PlayerState.Rotation = msg.Rotation;
        Application.LoadLevel(2);
    }

    public void HandleGalaxyPlayerMessage(GalaxyPlayer player)
    {
        UnityEngine.Debug.Log("handle galaxyplayer message");        
        PlayerState = player;
        Application.LoadLevel("Warp");
    }

    public void HandlePlayerStateMessage(PlayerStateMessage p)
    {
        if (BufferedInputs != null)
        {
            lock (BufferedInputs)
            {
                BufferedInputs.RemoveAll(input => input.Seq <= p.Seq);

                PlayerState.Location.Pos = p.PlayerPos;
                PlayerState.Rotation = p.Rotation;
                foreach (InputMessage input in BufferedInputs)
                {
                    Simulator.ProcessInput(PlayerState, input);
                    if (PlayerState.Location.InWarp)
                    {
                        Simulator.ContinuedPhysicsWarp(PlayerState);
                    } else
                    {
                        Simulator.ContinuedPhysics(PlayerState);
                    }
                }
            }
        }


    }

    private void SampleWarpInput()
    {
        bool anyInput = false;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);


        float xDelta = mousePos.x - screenCenter.x;
        float yDelta = mousePos.y - screenCenter.y;


        InputMessage input = new InputMessage();


        //rotation
        yDelta = Mathf.Clamp(yDelta, -70, 70);
        xDelta = Mathf.Clamp(xDelta, -70, 70);
        if (Math.Abs(xDelta) > 10 || Math.Abs(yDelta) > 10)
        {
            //  Camera.main.transform.Rotate(new Vector3(-yDelta * Time.deltaTime, xDelta  * Time.deltaTime, 0));        
            input.XTurn = xDelta / 4000f;
            input.YTurn = -yDelta / 4000f;

            anyInput = true;
        }
        else
        {
            input.XTurn = 0;
            input.YTurn = 0;
        }

        //throttle    
        if (Input.GetKey("w"))
        {
            if (throttle < 100)
            {
                throttle = Mathf.Clamp(throttle + 1, 0, 100);
                anyInput = true;
            }

        }
        else if (Input.GetKey("s"))
        {
            if (throttle > 0)
            {
                throttle = Mathf.Clamp(throttle - 1, 0, 100);
                anyInput = true;
            }
        }
        else if (Input.GetKey("space"))
        {
            throttle = 0;
           
        }


        //do a barrel roll
        if (Input.GetKey("q"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, -50*Time.deltaTime));
            input.RollTurn = .001f;
            anyInput = true;
        }
        else if (Input.GetKey("e"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, 50*Time.deltaTime));
            input.RollTurn = -.001f;
            anyInput = true;
        }

        // Camera.main.transform.Translate(Vector3.forward * throttle * 40 *  Time.deltaTime);

        if (anyInput)
        {
            input.Seq = Seq;
            input.Throttle = throttle;
            InputsToSend.Add(input);
            lock (BufferedInputs)
            {
                BufferedInputs.Add(input);
            }
            Simulator.ProcessInput(PlayerState, input);
            Seq++;

        }

        Simulator.ContinuedPhysicsWarp(PlayerState);
        Camera.main.transform.position = Utility.UVector(PlayerState.Location.Pos);
        Camera.main.transform.rotation = Utility.UQuaternion(PlayerState.Rotation);


    }

    private void SampleSystemInput()
    {
        bool anyInput = false;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);


        float xDelta = mousePos.x - screenCenter.x;
        float yDelta = mousePos.y - screenCenter.y;


        InputMessage input = new InputMessage();


        //rotation
        yDelta = Mathf.Clamp(yDelta, -70, 70);
        xDelta = Mathf.Clamp(xDelta, -70, 70);
        if (Math.Abs(xDelta) > 10 || Math.Abs(yDelta) > 10)
        {
            //  Camera.main.transform.Rotate(new Vector3(-yDelta * Time.deltaTime, xDelta  * Time.deltaTime, 0));        
            input.XTurn = xDelta / 4000f;
            input.YTurn = -yDelta / 4000f;

            anyInput = true;
        }
        else
        {
            input.XTurn = 0;
            input.YTurn = 0;
        }

        //throttle    
        if (Input.GetKey("w"))
        {
            if (throttle < 100)
            {
                throttle = Mathf.Clamp(throttle + 1, 0, 100);
                anyInput = true;
            }

        }
        else if (Input.GetKey("s"))
        {
            if (throttle > 0)
            {
                throttle = Mathf.Clamp(throttle - 1, 0, 100);
                anyInput = true;
            }
        }
        else if (Input.GetKey("space"))
        {
            GoingToWarp = true;
            GoToWarpMessage msg;
            msg.Location = PlayerState.Location;
            msg.Rotation = PlayerState.Rotation;
            Send(msg);            
            //todo some sort of animation/sounds            
        }


        //do a barrel roll
        if (Input.GetKey("q"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, -50*Time.deltaTime));
            input.RollTurn = .001f;
            anyInput = true;
        }
        else if (Input.GetKey("e"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, 50*Time.deltaTime));
            input.RollTurn = -.001f;
            anyInput = true;
        }

        // Camera.main.transform.Translate(Vector3.forward * throttle * 40 *  Time.deltaTime);

        if (anyInput)
        {
            input.Seq = Seq;
            input.Throttle = throttle;
            InputsToSend.Add(input);
            lock (BufferedInputs)
            {
                BufferedInputs.Add(input);
            }
            Simulator.ProcessInput(PlayerState, input);
            Seq++;

        }

        Simulator.ContinuedPhysics(PlayerState);
        Camera.main.transform.position = Utility.UVector(PlayerState.Location.Pos);
        Camera.main.transform.rotation = Utility.UQuaternion(PlayerState.Rotation);


       

    }
}
