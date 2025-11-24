using System;

using Midgard.Items;

using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.SkillHandlers
{
	public class ArmsLore
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.ArmsLore].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse(Mobile m)
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 500349 ); // What item do you wish to get information about?

			return TimeSpan.FromSeconds( 10.0 );
		}

		[PlayerVendorTarget]
		private class InternalTarget : Target
		{
			public InternalTarget() : base( 2, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				#region modifica by Dies Irae
				/*
				if( Core.AOS && targeted is Item )
				{
					if( XmlForceIdentify.IsUnidentified( (Item)targeted ) )
					{
						XmlForceIdentify.HandleIdentification( from, (Item)targeted );
						return;
					}
					else if( targeted is BaseArmor )
					{
						int level = (int)Math.Floor( XmlForceIdentify.GetItemRarity( (BaseArmor)targeted ) );

						if( level <= 5 )
							from.SendMessage( String.Format( "This armor could be classified as {0}.", Enum.GetName( typeof( JewelIntensity ), level ) ) );
						else
							from.SendMessage( "This armor could be classified as Legendary." );
					}
				}
				*/

				if( targeted == null || targeted is StaticTarget || targeted is LandTarget || ( targeted is Item && !( (Item)targeted ).Movable ) )
				{
					from.SendMessage( (from.Language == "ITA"? "Non puoi usare la tua abilità su quello." : "You can't use armslore on that.") );
					return;
				}

				bool success = Utility.Random( 101 ) < from.Skills[SkillName.ArmsLore].Value;//from.CheckTargetSkill( SkillName.ArmsLore, targeted, 0, 100 );

				if( success && targeted is IDurability )
				{
					IDurability dur = ( (IDurability)targeted );

					if( dur.MaxHitPoints != 0 )
					{
						int hp = (int)( ( dur.HitPoints / (double)dur.MaxHitPoints ) * 10 );

						if( hp < 0 )
							hp = 0;
						else if( hp > 9 )
							hp = 9;

						from.SendLocalizedMessage( 1038285 + hp );
					}
				}

				#endregion

				if ( targeted is BaseWeapon )
				{
					if ( success /* from.CheckTargetSkill( SkillName.ArmsLore, targeted, 0, 100 ) */ )
					{
						from.CheckSkill( SkillName.ArmsLore, 0.01 );
						BaseWeapon weap = (BaseWeapon)targeted;

						#region mod by Dies Irae: qality and damage for weapons
						if( weap.CustomQuality != Quality.Undefined )
							from.SendMessage( from.Language == "ITA" ? "La qualità di questo oggetto è: {0}" : "The quality of this item is: {0}", Quality.GetQualityName( weap ) );

						from.SendMessage( from.Language == "ITA" ? "Quest'arma fa {0}d{1}+{2} danni." : "This weapon deals {0}d{1}+{2} damage.", weap.NumDice, weap.NumSides, weap.DiceBonus );
						#endregion

						/*
						if ( weap.MaxHitPoints != 0 )
						{
							int hp = (int)((weap.HitPoints / (double)weap.MaxHitPoints) * 10);

							if ( hp < 0 )
								hp = 0;
							else if ( hp > 9 )
								hp = 9;

							from.SendLocalizedMessage( 1038285 + hp );
						}
						*/

						int damage = (weap.MaxDamage + weap.MinDamage) / 2;
						int hand = (weap.Layer == Layer.OneHanded ? 0 : 1);

						if ( damage < 3 )
							damage = 0;
						else
							damage = (int)Math.Ceiling( Math.Min( damage, 30 ) / 5.0 );
							/*
						else if ( damage < 6 )
							damage = 1;
						else if ( damage < 11 )
							damage = 2;
						else if ( damage < 16 )
							damage = 3;
						else if ( damage < 21 )
							damage = 4;
						else if ( damage < 26 )
							damage = 5;
						else
							damage = 6;
							 * */

						WeaponType type = weap.Type;

						if ( type == WeaponType.Ranged )
							from.SendLocalizedMessage( 1038224 + (damage * 9) );
						else if ( type == WeaponType.Piercing )
							from.SendLocalizedMessage( 1038218 + hand + (damage * 9) );
						else if ( type == WeaponType.Slashing )
							from.SendLocalizedMessage( 1038220 + hand + (damage * 9) );
						else if ( type == WeaponType.Bashing )
							from.SendLocalizedMessage( 1038222 + hand + (damage * 9) );
						else
							from.SendLocalizedMessage( 1038216 + hand + (damage * 9) );

						if ( weap.Poison != null && weap.PoisonCharges > 0 )
							from.SendLocalizedMessage( 1038284 ); // It appears to have poison smeared on it.
					}
					else
					{
						from.CheckSkill( SkillName.ArmsLore, 0.95 );
						from.SendLocalizedMessage( 500353 ); // You are not certain...
					}
				}
				else if(targeted is BaseArmor)
				{
					if( success /* from.CheckTargetSkill( SkillName.ArmsLore, targeted, 0, 100 ) */ )
					{
						from.CheckSkill( SkillName.ArmsLore, 0.01 );
						BaseArmor arm = (BaseArmor)targeted;

						#region mod by Dies Irae: qality for armors
						if( arm.CustomQuality != Quality.Undefined )
							from.SendMessage( from.Language == "ITA" ? "La qualità di questo oggetto è: {0}" : "The quality of this item is: {0}", Quality.GetQualityName( arm ) );
						#endregion

						/*
						if ( arm.MaxHitPoints != 0 )
						{
							int hp = (int)((arm.HitPoints / (double)arm.MaxHitPoints) * 10);

							if ( hp < 0 )
								hp = 0;
							else if ( hp > 9 )
								hp = 9;

							from.SendLocalizedMessage( 1038285 + hp );
						}
						*/

						from.SendLocalizedMessage( 1038295 + (int)Math.Ceiling( Math.Min( arm.ArmorRating, 35 ) / 5.0 ) );
						
						/*
						if ( arm.ArmorRating < 1 )
							from.SendLocalizedMessage( 1038295 ); // This armor offers no defense against attackers.
						else if ( arm.ArmorRating < 6 )
							from.SendLocalizedMessage( 1038296 ); // This armor provides almost no protection.
						else if ( arm.ArmorRating < 11 )
							from.SendLocalizedMessage( 1038297 ); // This armor provides very little protection.
						else if ( arm.ArmorRating < 16 )
							from.SendLocalizedMessage( 1038298 ); // This armor offers some protection against blows.
						else if ( arm.ArmorRating < 21 )
							from.SendLocalizedMessage( 1038299 ); // This armor serves as sturdy protection.
						else if ( arm.ArmorRating < 26 )
							from.SendLocalizedMessage( 1038300 ); // This armor is a superior defense against attack.
						else if ( arm.ArmorRating < 31 )
							from.SendLocalizedMessage( 1038301 ); // This armor offers excellent protection.
						else
							from.SendLocalizedMessage( 1038302 ); // This armor is superbly crafted to provide maximum protection.
						 * */
					}
					else
					{
						from.CheckSkill( SkillName.ArmsLore, 0.95 );
						from.SendLocalizedMessage( 500353 ); // You are not certain...
					}
				}
				else if ( targeted is SwampDragon && ((SwampDragon)targeted).HasBarding )
				{
					SwampDragon pet = (SwampDragon)targeted;

					if ( success /* from.CheckTargetSkill( SkillName.ArmsLore, targeted, 0, 100 ) */ )
					{
						int perc = (4 * pet.BardingHP) / pet.BardingMaxHP;

						if ( perc < 0 )
							perc = 0;
						else if ( perc > 4 )
							perc = 4;

						pet.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 1053021 - perc, from.NetState );
					}
					else
					{
						from.SendLocalizedMessage( 500353 ); // You are not certain...
					}
				}
				else if( targeted is HorseBardingDeed )
				{
					// we just used IDurability case.
				}
				else
				{
					from.SendLocalizedMessage( 500352 ); // This is neither weapon nor armor.
				}
			}
		}
	}
}