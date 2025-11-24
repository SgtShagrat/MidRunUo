using System;
using Server;
using Server.Items;
using Server.Mobiles;
namespace Midgard.Engines.BossSystem.WishSpectre
{
	internal enum SpectralBoneTypes : byte
	{
		Head,
		Femor,
		RibCage,
		Pelvis,
		Spine
	}
	
	internal class SpectralBone : QuestItem
	{	
		public const int SpectralHue =  2432;
		
		[Constructable]
		public SpectralBone( Dungeon dungeon, SpectralBoneTypes type ) : this( dungeon )
		{			
			BoneType = type;
			var name="";
			ItemID = GetRandomIdAndName(type,ref name);			
			Name=name;
		}
		static public int GetRandomId(SpectralBoneTypes type)
		{
			var name = "";
			return GetRandomIdAndName(type,ref name);
		}
		static public int GetRandomIdAndName (SpectralBoneTypes type,ref string Name)
		{
			int[] ids=null;

			switch (type)
			{
			case SpectralBoneTypes.Femor:
				ids = new []{ 0x1b11, 0x1b12 };
				Name = "Exurk Alh's bone";
				break;
			case SpectralBoneTypes.Head:
				ids = new []{ 0x1ae0, 0x1ae1,0x1ae2,0x1ae3,0x1ae4 };
				Name = "Exurk Alh's skull";
				break;
			case SpectralBoneTypes.Pelvis:
				ids = ids = new []{ 0x1b15, 0x1b16};
				Name = "Exurk Alh's pelvis";
				break;
			case SpectralBoneTypes.RibCage:
				ids = ids = new []{ 0x1b17, 0x1b18};
				Name = "Exurk Alh's rib cage";
				break;
			case SpectralBoneTypes.Spine:
				ids = ids = new []{ 0x1b1b, 0x1b1c};
				Name = "Exurk Alh's spine";
				break;
			}
			return ids[Utility.Random(ids.Length)];
		}
		
		[Constructable]
		public SpectralBone( Dungeon dungeon ) : base( dungeon, 0x1b11 )
		{			
			Name = "Exurk Alh's bone";
			Stackable = false;
			Amount = 1;
			Weight = 1.0;
			Hue = SpectralHue; //2292;
		}
		
		public SpectralBoneTypes BoneType{get;private set;}
		
		public override bool CanBeSeenBy (Mobile mobile)
		{
			var ret =  base.CanBeSeenBy (mobile);
			if (!ret)
				return ret;
			
			return ret;
		}
		
		#region Serialization
		public SpectralBone( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (ushort) 0 ); // version
			
			writer.Write( (byte)BoneType);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			var version = reader.ReadUShort();
			
			BoneType = (SpectralBoneTypes)reader.ReadByte();
		}
		#endregion
		
		public bool Is(SpectralBoneTypes type)
		{
			return BoneType == type;
		}
		
		public override bool OnDroppedOnto (Mobile from, Item target)
		{
			if (BoneType != SpectralBoneTypes.Head)
				return base.OnDroppedOnto (from, target);
			
			var player = from as PlayerMobile;
			if(player==null)
				return false;
			
			var stage = Dungeon.GetPlayerStageSet( player );
			if(stage == null)
			{
				( (CustomDungeon) Dungeon ).StartFirstStageForPlayers( player, OnlyVisibleTo );
				return base.OnDroppedOnto (from, target);
			}
			else
			{
				Delete();
				return false;
			}
		}
		public override void OnDoubleClick (Mobile from)
		{
			var player = from as PlayerMobile;
			if(player==null)
				return;
			
			var stage = Dungeon.GetPlayerStageSet( player );
			if(stage == null)
				( (CustomDungeon) Dungeon ).StartFirstStageForPlayers( player, OnlyVisibleTo );
			else
				new QuestGump( (CustomDungeon) Dungeon  , player );
		}
	}
}