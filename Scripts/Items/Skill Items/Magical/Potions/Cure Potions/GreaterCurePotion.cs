using System;
using Server;

namespace Server.Items
{
	public class GreaterCurePotion : BaseCurePotion
	{
		private static CureLevelInfo[] m_OldLevelInfo = new CureLevelInfo[]
			{
				new CureLevelInfo( Poison.Lesser,  1.00 ), // 100% chance to cure lesser poison
				new CureLevelInfo( Poison.Regular, 1.00 ), // 100% chance to cure regular poison
				new CureLevelInfo( Poison.Greater, 1.00 ), // 100% chance to cure greater poison
				new CureLevelInfo( Poison.Deadly,  0.75 ), //  75% chance to cure deadly poison
				new CureLevelInfo( Poison.Lethal,  0.25 ),  //  25% chance to cure lethal poison
				new CureLevelInfo( Poison.MagiaLesser,  0.90 ),
				new CureLevelInfo( Poison.MagiaRegular, 0.80 ),
				new CureLevelInfo( Poison.MagiaGreater, 0.70 ),
				new CureLevelInfo( Poison.MagiaDeadly,  0.60 ),
				new CureLevelInfo( Poison.MagiaLethal,  0.25 ),
				new CureLevelInfo( Poison.StanchezzaLesser,  0.90 ),
				new CureLevelInfo( Poison.StanchezzaRegular, 0.80 ),
				new CureLevelInfo( Poison.StanchezzaGreater, 0.70 ),
				new CureLevelInfo( Poison.StanchezzaDeadly,  0.60 ),
				new CureLevelInfo( Poison.StanchezzaLethal,  0.25 ),
				new CureLevelInfo( Poison.ParalisiLesser,  0.90 ),
				new CureLevelInfo( Poison.ParalisiRegular, 0.80 ),
				new CureLevelInfo( Poison.ParalisiGreater, 0.70 ),
				new CureLevelInfo( Poison.ParalisiDeadly,  0.60 ),
				new CureLevelInfo( Poison.ParalisiLethal,  0.25 ),
				new CureLevelInfo( Poison.BloccoLesser,  0.90 ),
				new CureLevelInfo( Poison.BloccoRegular, 0.80 ),
				new CureLevelInfo( Poison.BloccoGreater, 0.70 ),
				new CureLevelInfo( Poison.BloccoDeadly,  0.60 ),
				new CureLevelInfo( Poison.BloccoLethal,  0.25 ),
				new CureLevelInfo( Poison.LentezzaLesser,  0.90 ),
				new CureLevelInfo( Poison.LentezzaRegular, 0.80 ),
				new CureLevelInfo( Poison.LentezzaGreater, 0.70 ),
				new CureLevelInfo( Poison.LentezzaDeadly,  0.60 ),
				new CureLevelInfo( Poison.LentezzaLethal,  0.25 )
			};

		private static CureLevelInfo[] m_AosLevelInfo = new CureLevelInfo[]
			{
				new CureLevelInfo( Poison.Lesser,  1.00 ),
				new CureLevelInfo( Poison.Regular, 1.00 ),
				new CureLevelInfo( Poison.Greater, 1.00 ),
				new CureLevelInfo( Poison.Deadly,  0.95 ),
				new CureLevelInfo( Poison.Lethal,  0.75 )
			};

		public override CureLevelInfo[] LevelInfo{ get{ return Core.AOS ? m_AosLevelInfo : m_OldLevelInfo; } }

		#region Modifica by Dies Irae per le pozioni Stackable	
		[Constructable]
		public GreaterCurePotion( int amount ) : base( PotionEffect.CureGreater, amount )
		{
		}
		
		[Constructable]
		public GreaterCurePotion() : this(1)
		{
		}
		#endregion
		
		public GreaterCurePotion( Serial serial ) : base( serial )
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
