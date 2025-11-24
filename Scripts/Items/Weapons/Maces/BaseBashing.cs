using System;
using Server;
using Server.Items;

using Server.Engines.XmlSpawner2;

namespace Server.Items
{
	public abstract class BaseBashing : BaseMeleeWeapon
	{
		public override int DefHitSound{ get{ return 0x233; } }
		public override int DefMissSound{ get{ return 0x239; } }

		public override SkillName DefSkill{ get{ return SkillName.Macing; } }
		public override WeaponType DefType{ get{ return WeaponType.Bashing; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash1H; } }

		public BaseBashing( int itemID ) : base( itemID )
		{
		}

		public BaseBashing( Serial serial ) : base( serial )
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

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			#region mod by Dies Irae
			if( defender.Stam < (int)( defender.StamMax * 0.1 ) )
			{
				damageBonus *= (defender.Stam < 2 ? 1.3 : 1.1);
				attacker.SendMessage( attacker.Language == "ITA" ? "Per mancanza di stamina il nemico subisce un danno aggiuntivo." : "Due to lack of stamina your opponent suffered an extra damage." );
				if( attacker.PlayerDebug )
					attacker.SendMessage( "Low stamina bonus: 10%. Damage bonus is {0:F2}", damageBonus );
			}
			#endregion

			base.OnHit( attacker, defender, damageBonus );

			// defender.Stam -= Utility.Random( 3, 5 ); // 3-5 points of stamina loss
			// defender.Stam -= Utility.Dice( 1, 3, 2 ); // 3-5 points of stamina loss
		}

		public override bool StaminaLossOnHit { get { return true; } }

		#region Modifica By Dies Irae per le mazze con danno siedge
		public override void OnDoubleClick( Mobile from )
		{
			if( Parent != null && Parent == from )
				HandSiegeAttack.SelectTarget( from, this );
		}
		#endregion
		
		public override double GetBaseDamage( Mobile attacker )
		{
			double damage = base.GetBaseDamage( attacker );

			if ( BashingSpecialMoveEnabled && !Core.AOS && (attacker.Player || attacker.Body.IsHuman) && Layer == Layer.TwoHanded && (attacker.Skills[SkillName.Anatomy].Value / 400.0) >= Utility.RandomDouble() )
			{
				damage *= 1.5;

				attacker.SendMessage( "You deliver a crushing blow!" ); // Is this not localized?
				attacker.PlaySound( 0x11C );
			}

			return damage;
		}
	}
}