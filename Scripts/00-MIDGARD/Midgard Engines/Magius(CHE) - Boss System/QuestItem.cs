using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
namespace Midgard.Engines.BossSystem
{
	internal abstract class QuestItem : Item
	{				
		public Dungeon Dungeon{get;internal set;}
		
		public PlayerMobile[] OnlyVisibleTo{get;set;}
		protected QuestItem( Dungeon dungeon , int id ) : base(id)
		{		
			Dungeon = dungeon;
			dungeon.RegisterQuestItem(this);
			OnlyVisibleTo = new PlayerMobile[0];
		}
		
		public override bool CanBeSeenBy (Mobile mobile)
		{
			if (OnlyVisibleTo.Length==0)
				return base.CanBeSeenBy (mobile);
			
			foreach(var mob in OnlyVisibleTo)
			{
				if (mobile == mob)
					return base.CanBeSeenBy (mobile);
			}
			
			return false;
		}
		
		#region Serialization
		public QuestItem( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (ushort) 0 ); // version
			
			writer.Write( (byte) OnlyVisibleTo.Length );
			foreach(var mob in OnlyVisibleTo)				
				writer.Write( mob );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			var version = reader.ReadUShort();
			
			switch(version)
			{
			case 0:
			    {
				    var count = reader.ReadByte();
				    var tot = new List<PlayerMobile>();
				    for(int h=0 ; h< count; h++)
				    {
					    var mob = reader.ReadMobile() as PlayerMobile;
					    if( mob!= null)
						    tot.Add(mob);
				    }
				    OnlyVisibleTo = tot.ToArray();
				}
				break;
			}
		}
		#endregion
		
	}
}

