using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	public abstract class BaseNecroFamiliar : BaseCreature
	{
		//public BaseNecroFamiliar() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		//{
		//}
		public BaseNecroFamiliar(AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed)
		: base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
		{
		}

		private double m_CusDispelDifficulty;

		[CommandProperty( AccessLevel.GameMaster )]
		public double CusDispelDifficulty
		{
			get
			{
				return m_CusDispelDifficulty;
			}
			set
			{
				m_CusDispelDifficulty = value;
			}
		}

		public override double DispelDifficulty{ get{ return m_CusDispelDifficulty; } }
		public override double DispelFocus{ get{ return 20.0; } }

		public BaseNecroFamiliar( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{

			string mess = "";

			if ( CusDispelDifficulty > 0 && Experience <= 0 )
				mess = "(enforced)";
			else if ( CusDispelDifficulty <= 0 && Experience > 0 )
				mess = "(buffed)";
			else if ( CusDispelDifficulty > 0 && Experience > 0 )
				mess = "(buffed & enforced)";

			if ( mess != "" )
			{
				int hue = Notoriety.GetHue( Notoriety.Compute( from, this ) );
				PrivateOverheadMessage( MessageType.Regular, hue, true, string.Format( mess ), from.NetState );
			}

			base.OnSingleClick( from );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}