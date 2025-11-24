using System;
using Server;
using Server.Items;

using Server.Mobiles;

namespace Midgard.Items
{
	/// <summary>
	/// 0x3358 Double Bone Blade - ( craftabile solo da karma negativo ) 
	/// </summary>
	public class DoubleBoneBlade : Cutlass, IDoubleWeapon
	{
		public override string DefaultName { get { return "double bone blade"; } }

		[Constructable]
		public DoubleBoneBlade()
		{
			ItemID = 0x3358;
			Layer = Layer.TwoHanded;
		}

		public override bool CanBeCraftedBy( Mobile from )
		{
			return from.AccessLevel > AccessLevel.Counselor || from.Karma < 0;
		}

		public virtual bool Validate( Mobile from )
		{
			if( !from.Player )
				return true;

			BaseWeapon weapon = from.Weapon as BaseWeapon;

			if( weapon == null )
				return false;

			Skill skill = from.Skills[ weapon.Skill ];
			double reqSkill = 90.0;

			if( skill != null && skill.Base >= reqSkill )
				return true;

			return false;
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			if( !Validate( attacker ) )
				return;

			base.OnHit( attacker, defender, damageBonus );

			#region edit by Arlas [double weapon Mod]
			if (attacker is Midgard2PlayerMobile)
			{
				if (((Midgard2PlayerMobile)attacker).UseDoubleWeaponsTogether)
					InDoubleStrike = false;
				else
				{
					//attacco con arma primaria e utilizzo delle armi separato
					if( !InDoubleStrike )
						InDoubleStrike = true;//prossimo attacco con arma secondaria
					else
						InDoubleStrike = false;
				}
			}
			#endregion
			/*

			if( InDoubleStrike )
				return;

			double chance = GetAttackSkillValue( attacker, defender ) / 400.0; // 0.0 -> 0.25
			if( Utility.RandomDouble() > chance )
				return;

			attacker.SendLocalizedMessage( 1060084 ); // You attack with lightning speed!
			defender.SendLocalizedMessage( 1060085 ); // Your attacker strikes with lightning speed!

			defender.PlaySound( 0x3BB );
			defender.FixedEffect( 0x37B9, 244, 25 );

			if( defender.Deleted || attacker.Deleted || defender.Map != attacker.Map || !defender.Alive || !attacker.Alive || !attacker.CanSee( defender ) )
			{
				attacker.Combatant = null;
				return;
			}

			BaseWeapon weapon = attacker.Weapon as BaseWeapon;
			if( weapon == null || !attacker.InRange( defender, weapon.MaxRange ) || !attacker.InLOS( defender ) )
				return;

			InDoubleStrike = true;
			attacker.RevealingAction();
			attacker.NextCombatTime = DateTime.Now + weapon.OnSwing( attacker, defender );
			InDoubleStrike = false;
			*/
		}

		#region serialization
		public DoubleBoneBlade( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from ) 
		{ 
			Midgard2PlayerMobile m_Player = from as Midgard2PlayerMobile;
			if ( m_Player.UseDoubleWeaponsTogether )
			{
				m_Player.UseDoubleWeaponsTogether = false;
				m_Player.SendMessage("You decide to use them separately");
			}
			else
			{
				m_Player.UseDoubleWeaponsTogether = true;
				m_Player.SendMessage("You decide to use them togheter");
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		#endregion
	}
}