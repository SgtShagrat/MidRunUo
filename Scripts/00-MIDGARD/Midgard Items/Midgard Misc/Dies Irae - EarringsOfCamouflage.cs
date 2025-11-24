/***************************************************************************
 *                                  EarringsOfCamuflage.cs
 *                            		----------------------
 *  begin                	: Giugno, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Paio di orecchini vestibili solo da Vampiri.
 * 			Quando vengono vestiti cambiano la pelle
 * 			e il colore dei capelli in un colore random umano.
 * 			Alla rimozione riportano il colore della pelle e dei capelli a 
 * 			quelli standard da vampiro.
 * 
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Items
{
	public class EarringsOfCamuflage : BaseEarrings
	{
		#region costruttori
		[Constructable]
		public EarringsOfCamuflage() : base( 0x1087 )
		{
			Weight = 0.1;
		}

		public EarringsOfCamuflage( Serial serial ) : base( serial )
		{
		}
		#endregion

		#region metodi
		public override bool OnEquip( Mobile from )
		{
			return Validate( from ) && base.OnEquip( from );
		}
			
		public bool Validate( Mobile m )
		{
			if ( m == null || !m.Player )			
				return true;
			
			if( m.Race == (Race)Race.AllRaces[14] )
			{
				return true;
			}
			else
			{
				m.SendMessage( "Only a Vampire can wear this powerful artifact!" );
				return false;
			}		
		}
		
		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );
			
			if( parent is Midgard2PlayerMobile )
			{
				Midgard2PlayerMobile from = (Midgard2PlayerMobile)parent;
				if( from != null && !from.Deleted )
				{
					if( from.Race == (Race)Race.AllRaces[14] ) // Vampire
					{
						if( from.HairItemID > 0 )
							from.HairHue = Race.Human.RandomHairHue();
						from.Hue = Race.Human.RandomSkinHue();
						from.FixedParticles( 0x36CB, 1, 9, 9911, 2024, 5, EffectLayer.Head );
					}
				}
			}
		}
		
		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );
			
			if( parent is Mobile )
			{
				Mobile from = (Mobile)parent;
				if( from.HairItemID > 0 )
					from.HairHue = ((Race)Race.AllRaces[14]).RandomHairHue();
				from.Hue = ((Race)Race.AllRaces[14]).RandomSkinHue();
				from.FixedParticles( 0x36CB, 1, 9, 9911, 2025, 5, EffectLayer.Head );
			}
		}
		#endregion
		
		#region serial-deserial
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
		#endregion
	}
}
