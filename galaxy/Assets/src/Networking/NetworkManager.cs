using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System;
using GalaxyShared;
using System.Threading;
using System.Diagnostics;
using ProtoBuf;


public class NetworkManager : MonoBehaviour, IMessageHandler
{

    private static TcpClient socket;
    private static NetworkStream stream;
    public static Queue messageQueue;
    private Thread NetworkThread;
    

    public static float throttle = 0;
    public static List<InputMessage> InputsToSend;
    public static List<InputMessage> BufferedInputs;

    private GameObject Ship;

    Stopwatch SendStopwatch = new Stopwatch();
    Stopwatch InputSampleStopwatch = new Stopwatch();

    public static Player PlayerState = null;


    public static int Seq = 0;

    bool GoingToWarp = false;

    enum Level { MainMenu, Warp, System };
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
        if (level == 2 || level == 1)
        {
            Ship = GameObject.FindGameObjectWithTag("Ship");

        }
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
                    foreach (InputMessage input in InputsToSend)
                    {
                        Send(input);
                    }
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
                IMessage msg = (IMessage)messageQueue.Dequeue();
                msg.AcceptHandler(this);

                //pass in handler
                //make something implement imessagehandler on the client
            }
        }
    }

    private static byte[] typeBuffer = new byte[1];
    public static void Send(IMessage msg)
    {

        msg.Proto(stream, typeBuffer);
                
    }


    public void NetworkReadLoop()
    {
        byte[] buffer = new byte[NetworkUtils.SERVER_READ_BUFFER_SIZE];
        while (true)
        {
          
            //get type
            stream.Read(buffer,0, 1);
            IMessage msg;
            switch ((MsgType)buffer[0])
            {
                case MsgType.LoginResultMessage:
                    msg = Serializer.DeserializeWithLengthPrefix<LoginResultMessage>(stream, PrefixStyle.Fixed32);
                    break;
                case MsgType.NewUserResultMessage:
                    msg = Serializer.DeserializeWithLengthPrefix<NewUserResultMessage>(stream, PrefixStyle.Fixed32);
                    break;
                case MsgType.Player:
                    msg = Serializer.DeserializeWithLengthPrefix<Player>(stream, PrefixStyle.Fixed32);
                    break;
                case MsgType.InputMessage:
                    msg = Serializer.DeserializeWithLengthPrefix<InputMessage>(stream, PrefixStyle.Fixed32);
                    break;
                case MsgType.PlayerStateMessage:
                    msg = Serializer.DeserializeWithLengthPrefix<PlayerStateMessage>(stream, PrefixStyle.Fixed32);
                    break;
                case MsgType.GoToWarpMessage:
                    msg = Serializer.DeserializeWithLengthPrefix<GoToWarpMessage>(stream, PrefixStyle.Fixed32);
                    break;
                case MsgType.DropOutOfWarpMessage:
                    msg = Serializer.DeserializeWithLengthPrefix<DropOutOfWarpMessage>(stream, PrefixStyle.Fixed32);
                    break;
                case MsgType.Asteroid:
                    msg = Serializer.DeserializeWithLengthPrefix<Asteroid>(stream, PrefixStyle.Fixed32);
                    break;
                case MsgType.Ship:
                    msg = Serializer.DeserializeWithLengthPrefix<Ship>(stream, PrefixStyle.Fixed32);
                    break;
                case MsgType.CargoStateMessage:
                    msg = Serializer.DeserializeWithLengthPrefix<CargoStateMessage>(stream, PrefixStyle.Fixed32);
                    break;
                default:
                    msg = null;
                break;
            }
            
            
            messageQueue.Enqueue(msg);

        }

    }


    public void HandleMessage(LoginResultMessage msg, object extra = null)
    {
        //show error
    }

    public void HandleNewUserResultMessage(NewUserResultMessage msg)
    {
        //show error
    }

    public void HandleMessage(GoToWarpMessage msg, object extra = null)
    {
        PlayerState.Location = msg.Location;
        PlayerState.Rotation = msg.Rotation;
        lock (BufferedInputs)
        {
            BufferedInputs.Clear();
        }
        Application.LoadLevel((int)Level.Warp);

    }

    public void HandleMessage(DropOutOfWarpMessage msg, object extra = null)
    {
        Warp.ClosestSector.ParticleSystem.Clear();
        ClientSolarSystem.Cubemap = new Cubemap(2048, TextureFormat.ARGB32, false);
        bool work = Camera.main.RenderToCubemap(ClientSolarSystem.Cubemap);
        PlayerState.Location = msg.Location;
        PlayerState.Rotation = msg.Rotation;
        PlayerState.SolarSystem = msg.System;
        lock (BufferedInputs)
        {
            BufferedInputs.Clear();
        }
        Application.LoadLevel((int)Level.System);
    }

    public void HandleMessage(Player player, object extra = null)
    {
        UnityEngine.Debug.Log("handle player message");
        PlayerState = player;
        Application.LoadLevel("Warp");
    }

    public void HandleMessage(Ship ship, object extra = null)
    {
        //todo
    }

    public void HandleMessage(CargoStateMessage msg, object extra = null)
    {
        if (msg.add)
        {
            PlayerState.Ship.AddCargo(msg.item);
        }
        GameObject shipGO = GameObject.FindGameObjectWithTag("Ship");
        shipGO.GetComponent<ClientSolarSystem>().UpdateInventory();
    }

    public void HandleMessage(Asteroid serverAsteroid, object extra = null)
    {
        Asteroid clientAsteroid = null;
        foreach (Asteroid a in PlayerState.SolarSystem.Asteroids)
        {
            if (a.Pos == serverAsteroid.Pos)
            {
                clientAsteroid = a;
                break;
            }
        }
        if (clientAsteroid != null)
        {
            clientAsteroid.Remaining = serverAsteroid.Remaining;
            if (clientAsteroid.Remaining <= 0)
            {
                PlayerState.SolarSystem.Asteroids.Remove(clientAsteroid);
                GameObject goAsteroid = (GameObject)clientAsteroid.GameObject;
                GameObject.Destroy(goAsteroid);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Asteroid messages received for asteroid that does not exist");
        }
    }

    public void HandleMessage(PlayerStateMessage p, object extra = null)
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
                    }
                    else
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
            input.RollTurn = .01f;
            anyInput = true;
        }
        else if (Input.GetKey("e"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, 50*Time.deltaTime));
            input.RollTurn = -.01f;
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
        Ship.transform.position = Utility.UVector(PlayerState.Location.Pos);
        Ship.transform.rotation = Utility.UQuaternion(PlayerState.Rotation);


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
        else if (Input.GetKey("j"))
        {
            GoingToWarp = true;
            GoToWarpMessage msg;
            msg.Location = PlayerState.Location;
            msg.Rotation = PlayerState.Rotation;
            Send(msg);
            //todo some sort of animation/sounds            
        }
        else if (Input.GetKey("space"))
        {
            if (throttle != 0)
            {
                throttle = 0;
                anyInput = true;
            }
        }


        //do a barrel roll
        if (Input.GetKey("q"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, -50*Time.deltaTime));
            input.RollTurn = .01f;
            anyInput = true;
        }
        else if (Input.GetKey("e"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, 50*Time.deltaTime));
            input.RollTurn = -.01f;
            anyInput = true;
        }

        // Camera.main.transform.Translate(Vector3.forward * throttle * 40 *  Time.deltaTime);

        if (Input.GetMouseButton(1))
        {
            UnityEngine.Debug.Log("MouseButton1");
            anyInput = true;
            input.SecondaryButton = true;
        }

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
        Ship.transform.position = Utility.UVector(PlayerState.Location.Pos);
        Ship.transform.rotation = Utility.UQuaternion(PlayerState.Rotation);




    }


    //things the client doesn't need to implement
    public void HandleMessage(LoginMessage msg, object extra = null)
    {
        throw new NotImplementedException();
    }

    public void HandleMessage(NewUserMessage msg, object extra = null)
    {
        throw new NotImplementedException();
    }

    public void HandleMessage(NewUserResultMessage msg, object extra = null)
    {
        throw new NotImplementedException();
    }

    public void HandleMessage(InputMessage msg, object extra = null)
    {
        throw new NotImplementedException();
    }
}
