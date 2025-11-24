using System;
using Server;
using Server.Items;
using Server.Targets;
using Server.Mobiles;
using Midgard.Engines.Classes;

namespace Server.Items
{
	public abstract class BaseKnife : BaseMeleeWeapon
	{
		public override int DefHitSound{ get{ return 0x23B; } }
		public override int DefMissSound{ get{ return 0x238; } }

		public override SkillName DefSkill{ get{ return SkillName.Swords; } }
		public override WeaponType DefType{ get{ return WeaponType.Slashing; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Slash1H; } }

		public BaseKnife( int itemID ) : base( itemID )
		{
		}

		public BaseKnife( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			from.SendLocalizedMessage( 1010018 ); // What do you want to use this item on?

			from.Target = new BladedItemTarget( this );
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );

			if( !Core.AOS && Poison != null && PoisonCharges > 0 && CanPoison( attacker ) )
			{
				// mod by Dies Irae
				// The chance of poisoning with a weapon hit is now the poisoners skill / 4.
				// http://forum.uosecondage.com/viewtopic.php?f=7&t=3403
				// --PoisonCharges;
				double chance = ( PoisonerSkill + attacker.Skills[ SkillName.Poisoning ].Value ) / ( 300.0 + defender.Skills[ SkillName.TasteID ].Value ); // 0.0 -> 0.25 //edit by arlas

				if ( Poison.Level > 18 && attacker is Midgard2PlayerMobile && ((Midgard2PlayerMobile)attacker).Class == Classes.None )
					chance = ( PoisonerSkill + attacker.Skills[ SkillName.Poisoning ].Value ) / ( 1000.0 + defender.Skills[ SkillName.TasteID ].Value * 10.0);//
				//attacker.SendMessage( "Debug: chance of poison hit: {0}.", (int)( chance * 100 ) );

				if ( Utility.RandomDouble() <= /* 0.5 */ chance ) // 50% chance to poison
				{
					defender.ApplyPoison( attacker, Poison );
					--PoisonCharges;
				}
			}
		}
	}
}