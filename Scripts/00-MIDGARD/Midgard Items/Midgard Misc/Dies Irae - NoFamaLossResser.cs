/***************************************************************************
 *                                  NoFamaLossResser.cs
 *                            		-------------------
 *  begin                	: December, 2005
 *  version					: 1.3
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 	NoFamaLossResser e' un resser che non fa perdere fama.
 * 
 * 	27 Aprile 2007: 
 * 		Aggiunta Fazione per il ress selettivo.
 * 
 ***************************************************************************/

using System;
using Server.Factions;
using Server.Mobiles;

namespace Server.Items
{
	public class NoFamaLossResser : Item
	{
		#region campi
		private Faction m_Faction;
		#endregion
		
		#region proprietà
		[CommandProperty( AccessLevel.GameMaster )]
		public Faction Faction
		{
			get{ return m_Faction; }
			set{ m_Faction = value; }
		}
		#endregion
		
		#region costruttori
		[Constructable]
		public NoFamaLossResser( ) : this( null )
		{
		}
		
		[Constructable]
		public NoFamaLossResser( Faction faction ) : base( 0x17E5 )
		{
			Movable = false;
			Hue = 1154;
			Name = "a Special Resser";
			Light = LightType.Circle300;
			Faction = faction;
		}
		
		public NoFamaLossResser( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region metodi
		public override bool OnMoveOver( Mobile m )
		{
			if ( m is PlayerMobile && !m.Alive )
			{
				Faction fact = Faction.Find( m );
				if( fact != null && this.Faction != null && fact == Faction )
				{
					m.SendMessage( "Only followers of {0} can be resurrected here!", this.Faction.ToString() );
					return false;
				}
				
				m.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				m.PlaySound( 0x202 );
				m.Resurrect();
				m.SendMessage( "You are Alive now!" );
			}
			return true;
		}
		#endregion
		
		#region serialize-deserialize
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
		#endregion
	}	
}
