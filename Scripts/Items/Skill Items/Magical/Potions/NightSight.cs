using System;
using Server;

namespace Server.Items
{
	public class NightSightPotion : BasePotion
	{

		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public NightSightPotion( int amount ) : base( 0xF06, PotionEffect.Nightsight, amount )
		{
		}
		
		[Constructable]
		public NightSightPotion() : this(1)
		{
		}
		#endregion
		
		public NightSightPotion( Serial serial ) : base( serial )
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

		public override void Drink( Mobile from )
		{
			if ( from.BeginAction( typeof( LightCycle ) ) )
			{
                #region mod by Dies Irae
                if( !Core.AOS )
                    LockBasePotionUse( from );
                #endregion

				new LightCycle.NightSightTimer( from ).Start();

				#region mod by Dies Irae: ora nighsight fa come un omonimo spell con 120 magery
				from.LightLevel = (int)( LightCycle.DungeonLevel );
				from.LightLevel = 4 + ( LightCycle.DungeonLevel / 2 );
				#endregion

				from.FixedParticles( 0x376A, 9, 32, 5007, EffectLayer.Waist );
				from.PlaySound( 0x1E3 );

				BasePotion.PlayDrinkEffect( from );

				this.Consume();
			}
			else
			{
				from.SendMessage( "You already have nightsight." );
			}
		}
	}
}
