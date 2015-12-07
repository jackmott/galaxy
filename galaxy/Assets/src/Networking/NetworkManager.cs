using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System;
using GalaxyShared;
using System.Threading;
using System.Diagnostics;
using ProtoBuf;
using System.IO;


public class NetworkManager : MonoBehaviour, IMessageHandler
{
    
    public InputCollector InputCollector;
   

    private static TcpClient socket;
    private static NetworkStream stream;
    public static Queue messageQueue;
    private Thread NetworkThread;

    bool shutdown = false;

    
    public static List<InputMessage> InputsToSend;
    public static List<InputMessage> BufferedInputs;

    private GameObject Ship;

    private static Stopwatch MasterClock = new Stopwatch();
    public static long Millis
    {
        get
        {
            return MasterClock.ElapsedMilliseconds;
        }
    }

    public static long LastStateSend = 0;
    public static Player PlayerState = null;
    public static int Seq = 1;
    public static bool GoingToWarp = false;

    enum Level { MainMenu, Warp, System };
    Level CurrentLevel;

    StreamWriter log;

    void Awake()
    {
        log = File.CreateText("log.txt");

        messageQueue = new Queue();
        InputsToSend = new List<InputMessage>();
        BufferedInputs = new List<InputMessage>();
        DontDestroyOnLoad(this);
        socket = new TcpClient("localhost", 8888);
        socket.NoDelay = true;
        stream = socket.GetStream();
        NetworkThread = new Thread(new ThreadStart(NetworkReadLoop));
        NetworkThread.Start();
        
        MasterClock.Start();

    }
    // Use this for initialization
    void Start()
    {
        InputCollector = GetComponent<InputCollector>();
    }

    void OnApplicationQuit()
    {
        socket.Close();
        shutdown = true;
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
               
            

            if (Millis-LastStateSend >= NetworkUtils.CLIENT_BUFFER_TIME)
            {
                if (InputsToSend.Count > 0)
                {
                    foreach (InputMessage input in InputsToSend)
                    {
                        Send(input);
                    }
                    InputsToSend.Clear();
                }
                LastStateSend = Millis;
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
                ClientWrapper wrapper = (ClientWrapper)messageQueue.Dequeue();
                MemoryStream stream = new MemoryStream(wrapper.buffer, 0, wrapper.size);


                IMessage msg;
                switch (wrapper.type)
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
                    case MsgType.MiningMessage:
                        msg = Serializer.DeserializeWithLengthPrefix<MiningMessage>(stream, PrefixStyle.Fixed32);
                        break;
                    case MsgType.ConstructionMessage:
                        msg = Serializer.DeserializeWithLengthPrefix<ConstructionMessage>(stream, PrefixStyle.Fixed32);
                        break;
                    default:
                        msg = null;
                        break;
                }


                msg.AcceptHandler(this);
                ClientWrapperPool.ReturnWrapper(wrapper);
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
        byte[] typeBuffer = new byte[1];
        while (true)
        {
            if (shutdown) return;
            try
            {
                ClientWrapper wrapper = ClientWrapperPool.GetWrapper();
                int bytesRead = 0;
                while (bytesRead < 1)
                {
                    bytesRead += stream.Read(typeBuffer, 0, 1);
                }

                bytesRead = 0;
                while (bytesRead < 4)
                {
                    bytesRead += stream.Read(wrapper.buffer, 0, 4 - bytesRead);
                }

                int size = BitConverter.ToInt32(wrapper.buffer, 0);
                if (size > 5000)
                {
                    UnityEngine.Debug.Log("BIG Size:" + size);
                    UnityEngine.Debug.Log("type:" + typeBuffer[0]);
                }

                bytesRead = 0;
                while (bytesRead < size)
                {
                    bytesRead += stream.Read(wrapper.buffer, bytesRead + 4, size - bytesRead);
                }

                wrapper.type = (MsgType)typeBuffer[0];
                if (wrapper.type == MsgType.DropOutOfWarpMessage) UnityEngine.Debug.Log("got drop out of warp");
                wrapper.size = size + 4;

                messageQueue.Enqueue(wrapper);
            }
            catch (Exception Ex)
            {

                UnityEngine.Debug.Log(Ex);
                if (shutdown) return;
            }
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
        //Warp.ClosestSector.ParticleSystem.Clear();
        ClientSolarSystem.Cubemap = new Cubemap(4096, TextureFormat.ARGB32, false);
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

    public void HandleMessage(MiningMessage msg, object extra = null)
    {
        if (msg.Add)
        {
            PlayerState.Ship.AddCargo(msg.Item);
        }
        GameObject hudGO = GameObject.FindGameObjectWithTag("Hud");
        Hud hud = hudGO.GetComponent<Hud>();
        hud.UpdateInventory();
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        ship.GetComponent<ClientSolarSystem>().UpdateAsteroid(msg.AsteroidHash, msg.Remaining);
        hud.UpdateInfo();
        
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

    public static void GoToWarp()
    {        
        GoingToWarp = true;
        GoToWarpMessage msg;
        msg.Location = PlayerState.Location;
        msg.Rotation = PlayerState.Rotation;
        Send(msg);
    }

    public void HandleMessage(PlayerStateMessage p, object extra = null)
    {
        lock (BufferedInputs)
        {
            BufferedInputs.RemoveAll(input => input.Seq <= p.Seq);

            //  log.WriteLine("PlayerState.Pos:" + PlayerState.Location.Pos + ", throttle:" + PlayerState.Throttle);

            PlayerState.Location.Pos = p.PlayerPos;
            PlayerState.Rotation = p.Rotation;
            PlayerState.Throttle = p.Throttle;

            //  log.WriteLine(p);
            //  log.WriteLine("bufferedinput count:" + BufferedInputs.Count);

            foreach (InputMessage input in BufferedInputs)
            {

                if (!input.ClientOnly)
                    Simulator.ProcessInput(PlayerState, input);


                if (PlayerState.Location.InWarp)
                {
                    // log.WriteLine("input:" + input);
                    Simulator.ContinuedPhysicsWarp(PlayerState, input.DeltaTime);
                }
                else
                {
                    Simulator.ContinuedPhysics(PlayerState, input.DeltaTime);

                }

            }

            BufferedInputs.RemoveAll(input => input.ClientOnly);

            // log.WriteLine("PlayerState.Pos:" + PlayerState.Location.Pos + ", throttle:" + PlayerState.Throttle);

            //  log.WriteLine("-----------------------------------------------");

        }
    }

    public void HandleMessage(ConstructionMessage msg, object extra = null)
    {
        if (msg.Progress == 0)
        {
            GameObject g = Resources.Load<GameObject>("Station/Construction");
            GameObject constructionModule = (GameObject)Instantiate(g, Utility.UVector(PlayerState.Location.Pos), Utility.UQuaternion(PlayerState.Rotation));
            constructionModule.transform.Translate(Vector3.forward * 3);
        }
    }



    private void SampleWarpInput()
    {
        int deltaT = (int)(Millis - PlayerState.LastPhysicsUpdate);
        if (deltaT >= NetworkUtils.SERVER_TICK_RATE)
        {
            
            bool anyInput = false;
            InputMessage input = new InputMessage();


            //rotation
            if (InputCollector.Yaw != 0 || InputCollector.Pitch != 0 || InputCollector.Roll != 0 || InputCollector.Throttle != PlayerState.Throttle)
            {

                //  Camera.main.transform.Rotate(new Vector3(-yDelta * Time.deltaTime, xDelta  * Time.deltaTime, 0));        
                input.Yaw = InputCollector.Yaw;
                input.Pitch = InputCollector.Pitch;
                input.Roll = InputCollector.Roll;
                input.Throttle = InputCollector.Throttle;
                anyInput = true;
            }

            if (anyInput)
            {
                input.ClientOnly = false;
                input.Seq = Seq;
                input.DeltaTime = deltaT;
                Simulator.ProcessInput(PlayerState, input);
                Seq++;
            }
            else
            {
                input.ClientOnly = true;
                input.Seq = Seq;
                input.DeltaTime = deltaT;
            }

            lock (BufferedInputs)
            {
                BufferedInputs.Add(input);
            }

            if (anyInput)
            {
                InputsToSend.Add(input);
            }

            Simulator.ContinuedPhysicsWarp(PlayerState, deltaT);
            PlayerState.LastPhysicsUpdate = Millis;
            

            Ship.transform.position = Utility.UVector(PlayerState.Location.Pos);
            Ship.transform.rotation = Utility.UQuaternion(PlayerState.Rotation);
        }

    }

    private void SampleSystemInput()
    {
        int deltaT = (int)(Millis - PlayerState.LastPhysicsUpdate);
        if (deltaT >= NetworkUtils.SERVER_TICK_RATE)         
        {

            bool anyInput = false;
            InputMessage input = new InputMessage();


            //rotation
            if (InputCollector.Yaw != 0 || InputCollector.Pitch != 0 || InputCollector.Roll != 0 || InputCollector.Throttle != PlayerState.Throttle)
            {

                //  Camera.main.transform.Rotate(new Vector3(-yDelta * Time.deltaTime, xDelta  * Time.deltaTime, 0));        
                input.Yaw = InputCollector.Yaw;
                input.Pitch = InputCollector.Pitch;
                input.Roll = InputCollector.Roll;
                input.Throttle = InputCollector.Throttle;
                anyInput = true;
            }



          

            if (InputCollector.SecondaryButton || InputCollector.PrimaryButton)
            {
                UnityEngine.Debug.Log("MouseButton1");
                anyInput = true;
                input.SecondaryButton = InputCollector.SecondaryButton;
                input.PrimaryButton = InputCollector.PrimaryButton;
            }
         

            if (anyInput)
            {
                input.ClientOnly = false;
                input.Seq = Seq;
                input.DeltaTime = deltaT;
                Simulator.ProcessInput(PlayerState, input);
                Seq++;

            }
            else
            {
                input.ClientOnly = true;
                input.Seq = Seq;
                input.DeltaTime = deltaT;
            }

            lock (BufferedInputs)
            {
                BufferedInputs.Add(input);

            }
            if (anyInput)
            {
                // log.WriteLine("Input at send - " + input);
                InputsToSend.Add(input);
            }


            Simulator.ContinuedPhysics(PlayerState, deltaT);
            PlayerState.LastPhysicsUpdate = Millis;

            Ship.transform.position = Utility.UVector(PlayerState.Location.Pos);
            Ship.transform.rotation = Utility.UQuaternion(PlayerState.Rotation);

        }


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

    public void HandleMessage(StationModule sm, object extra = null)
    {
        throw new NotImplementedException();
    }
}
