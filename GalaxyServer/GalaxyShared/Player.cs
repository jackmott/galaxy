using XnaGeometry;
using ProtoBuf;
using System.IO;
using System;


namespace GalaxyShared
{
	[ProtoContract]
	public class Player : IMessage
	{
		[ProtoMember(1)]
		public string UserName;
		[ProtoMember(2)]
		public bool InWarp;
		[ProtoMember(3)]
		public Quaternion Rotation;
		[ProtoMember(4)]
		public float Throttle;
		[ProtoMember(5)]
		public long Seq;
		[ProtoMember(6)]
		public Ship Ship;
		[ProtoMember(7)]
		public Vector3 Pos;
		
		public SectorCoord SectorPos
		{
			get
			{
				return new SectorCoord((int)(Pos.X / Galaxy.SECTOR_SIZE),
									   (int)(Pos.Y / Galaxy.SECTOR_SIZE),
									   (int)(Pos.Z / Galaxy.SECTOR_SIZE));
			}
			
		}

		public SolarSystem SolarSystem;
		public long LastPhysicsUpdate = -10;


		//new player
		public Player(string userName)
		{

			UserName = userName;
			Pos = new Vector3(Galaxy.GALAXY_SIZE_SECTORS / 2, Galaxy.GALAXY_SIZE_SECTORS / 2, Galaxy.GALAXY_THICKNESS_SECTORS / 2);
			Pos = Pos * Galaxy.SECTOR_SIZE;
			Console.WriteLine("Pos:" + Pos);
			Console.WriteLine("SectorPos:" + SectorPos);
			Sector s = new Sector(SectorPos);			
			Rotation = Quaternion.Identity;
			Throttle = 0;
			InWarp = true;
			Ship = new Ship(this);
		}

		public Player()
		{

		}



		public void Proto(Stream stream, byte[] typeBuffer)
		{
			typeBuffer[0] = (byte)MsgType.Player;
			stream.Write(typeBuffer, 0, 1);
			Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
		}

		public void AcceptHandler(IMessageHandler handler, object o = null)
		{
			handler.HandleMessage(this, o);
		}

	}
}
