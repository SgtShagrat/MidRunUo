using System;
using Server;
using Server.Mobiles;
namespace Midgard.Engines.BossSystem
{
	internal abstract class DungeonStageSet
	{
		//note:dungeon will not be seralized. Not need it
		public Dungeon Dungeon {get;private set;}

		/// <summary>
		/// Player will be settet from Dungeon Deserialization.
		/// </summary>
		public PlayerMobile Player {get;private set;}
		
		protected DungeonStageSet (Dungeon dungeon,PlayerMobile player)
		{
			Dungeon = dungeon;
			Player = player;
		}
		
		#region serialization
		protected DungeonStageSet (Dungeon dungeon, PlayerMobile player, GenericReader reader) : this(dungeon,player)
		{
			Deserialize(reader);
		}
		
		public virtual void Serialize (GenericWriter writer)
		{
			writer.Write((ushort)0); //version
		}
		protected virtual void Deserialize (GenericReader reader)
		{
			var version = reader.ReadUShort();
			
			switch(version)
			{
			case 0:				
				break;
			}
		}
		#endregion
	}
}

