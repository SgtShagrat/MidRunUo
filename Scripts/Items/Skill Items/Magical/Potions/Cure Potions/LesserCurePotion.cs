using System;
using Server;

namespace Server.Items
{
	public class LesserCurePotion : BaseCurePotion
	{
		private static CureLevelInfo[] m_OldLevelInfo = new CureLevelInfo[]
			{
				new CureLevelInfo( Poison.Lesser,  0.75 ), // 75% chance to cure lesser poison
				new CureLevelInfo( Poison.Regular, 0.50 ), // 50% chance to cure regular poison
				new CureLevelInfo( Poison.Greater, 0.15 ),  // 15% chance to cure greater poison
				new CureLevelInfo( Poison.MagiaLesser,  0.70 ),
				new CureLevelInfo( Poison.MagiaRegular, 0.40 ),
				new CureLevelInfo( Poison.MagiaGreater, 0.10 ),
				new CureLevelInfo( Poison.StanchezzaLesser,  0.70 ),
				new CureLevelInfo( Poison.StanchezzaRegular, 0.40 ),
				new CureLevelInfo( Poison.StanchezzaGreater, 0.10 ),
				new CureLevelInfo( Poison.ParalisiLesser,  0.70 ),
				new CureLevelInfo( Poison.ParalisiRegular, 0.40 ),
				new CureLevelInfo( Poison.ParalisiGreater, 0.10 ),
				new CureLevelInfo( Poison.BloccoLesser,  0.70 ),
				new CureLevelInfo( Poison.BloccoRegular, 0.40 ),
				new CureLevelInfo( Poison.BloccoGreater, 0.10 ),
				new CureLevelInfo( Poison.LentezzaLesser,  0.70 ),
				new CureLevelInfo( Poison.LentezzaRegular, 0.40 ),
				new CureLevelInfo( Poison.LentezzaGreater, 0.10 )
			};

		private static CureLevelInfo[] m_AosLevelInfo = new CureLevelInfo[]
			{
				new CureLevelInfo( Poison.Lesser,  0.75 ),
				new CureLevelInfo( Poison.Regular, 0.50 ),
				new CureLevelInfo( Poison.Greater, 0.25 )
			};

		public override CureLevelInfo[] LevelInfo{ get{ return Core.AOS ? m_AosLevelInfo : m_OldLevelInfo; } }

		#region Modifica by Dies Irae per le pozioni Stackable	
		[Constructable]
		public LesserCurePotion( int amount ) : base( PotionEffect.CureLesser, amount )
		{
		}
		
		[Constructable]
		public LesserCurePotion() : this(1)
		{
		}
		#endregion
		
		public LesserCurePotion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
