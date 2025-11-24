using System;
using Server;
using Server.Spells.Spellweaving;
using Midgard.Engines.Classes;

namespace Server.Items
{
	public abstract class BaseMeleeWeapon : BaseWeapon
	{
		public BaseMeleeWeapon( int itemID ) : base( itemID )
		{
		}

		public BaseMeleeWeapon( Serial serial ) : base( serial )
		{
		}

		public override int AbsorbDamage( Mobile attacker, Mobile defender, int damage )
		{
			damage = base.AbsorbDamage( attacker, defender, damage );

			//AttuneWeaponSpell.TryAbsorb( defender, ref damage );

			// mod by Dies Irae
			if (ClassSystem.IsDruid( defender ))
			{
				Midgard.Engines.SpellSystem.AttuneWeaponSpell.TryAbsorb( defender, ref damage );
				return damage;
			}

			if ( Core.AOS )
				return damage;
			
			int absorb = defender.MeleeDamageAbsorb;

			if ( absorb > 0 )
			{
				if ( absorb >= damage )
				{
					int react = damage; // /5;

					if ( react <= 0 )
						react = 1;

					defender.MeleeDamageAbsorb -= damage;

					attacker.Damage( react, defender );
					damage = 0;
					//SendSysmessage(attacker,"Your enemy reflect to you all his damage!");
					//SendSysmessage(defender,"Now you can partially reflect "+ra+" blows!");
					defender.SendMessage( defender.Language == "ITA" ? "Ora puoi riflettere parzialmente {0} danni!" : "Now you can partially reflect {0} blows!", defender.MeleeDamageAbsorb );
					attacker.SendMessage( attacker.Language == "ITA" ? "Il tuo nemico riflette tutto il danno subito!" : "Your enemy reflect to you all his damage!" );
					attacker.PlaySound( 0x1F1 );
					attacker.FixedEffect( 0x374A, 10, 16 );
				}
				else
				{
					damage -= absorb;
					attacker.Damage( absorb, defender );
					attacker.SendMessage( attacker.Language == "ITA" ? "Il tuo nemico riflette parte del danno subito!" : "Your enemy reflect to you some of his damage!" );

					defender.MeleeDamageAbsorb = 0;
					defender.SendLocalizedMessage( 1005556 ); // Your reactive armor spell has been nullified.
					DefensiveSpell.Nullify( defender );
				}
			}

			return damage;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}
