// #define verify

using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Engines.Craft;

namespace Server.Items
{
	public class BaseShield : BaseArmor
	{
		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		public BaseShield( int itemID ) : base( itemID )
		{
		}

		public BaseShield( Serial serial ) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 )
			{
				if ( this is Aegis )
					return;

				// The 15 bonus points to resistances are not applied to shields on OSI.
				PhysicalBonus = 0;
				FireBonus = 0;
				ColdBonus = 0;
				PoisonBonus = 0;
				EnergyBonus = 0;
			}
#if verify
			Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Midgard.CraftHelper.VerifyShields_Callback ), this );
#endif
		}

		public override double ArmorRating
		{
			get
			{
				Mobile m = this.Parent as Mobile;
				double ar = base.ArmorRating;

				if ( m != null )
					return ( ( m.Skills[SkillName.Parry].Value * ar ) / 200.0 ) + 1.0;
				else
					return ar;
			}
		}

		public override int OnHit( BaseWeapon weapon, int damage )
		{
			if( Core.AOS )
			{
				if( ArmorAttributes.SelfRepair > Utility.Random( 10 ) )
				{
					HitPoints += 2;
				}
				else
				{
					double halfArmor = ArmorRating / 2.0;
					int absorbed = (int)(halfArmor + (halfArmor*Utility.RandomDouble()));

					if( absorbed < 2 )
						absorbed = 2;

					int wear;

					if( weapon.Type == WeaponType.Bashing )
						wear = (absorbed / 2);
					else
						wear = Utility.Random( 2 );

					if( wear > 0 && MaxHitPoints > 0 )
					{
						if( HitPoints >= wear )
						{
							HitPoints -= wear;
							wear = 0;
						}
						else
						{
							wear -= HitPoints;
							HitPoints = 0;
						}

						if( wear > 0 )
						{
							if( MaxHitPoints > wear )
							{
								MaxHitPoints -= wear;

								if( Parent is Mobile )
									((Mobile)Parent).LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061121 ); // Your equipment is severely damaged.
							}
							else
							{
								Delete();
							}
						}
					}
				}

				return 0;
			}
			else
			{
				Mobile owner = this.Parent as Mobile;
				if( owner == null )
					return damage;

				bool OldPolParry = true;

				double ar = this.ArmorRating;

				//BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
            			Mobile attacker = weapon.Parent as Mobile;
				//double atkSkill = attacker.Skills[weapon.Skill].Value;

				double chance = (owner.Skills[SkillName.Parry].Value - (ar * 2.0)) / 100.0;
				if (attacker != null && OldPolParry)
				{
					double atkSkill = attacker.Skills[weapon.Skill].Value;
					chance = (owner.Skills[SkillName.Parry].Value-(atkSkill)/2)/ 100.0;
					ar = this.ArmorRatingScaled;
					//owner.Say( "Parry %: {0}, AR {1}", (int)(chance*100), (int)ar );
				}

				if( owner.PlayerDebug )
					owner.SendMessage( "Parry %: {0}", (int)(chance*100) );
				//formula pol:
				/*	if (shield and weapon)
		//if(weaponelem.skillid="archery" and !(defender.npctemplate))
		if(!(defender.npctemplate))
			var parryper:=cdbl(NewGetSkill(defender,cint(SKILLID_PARRY)));
			parryper:=parryper-cdbl(NewGetSkill(attacker,GetSkillIDbyName(weaponelem.skillid)))/2;
			if (parryper<5)
				 parryper:=5;
			endif
			if (parryper>95)
				 parryper:=95;
			endif
			
			//Danno arbito dallo scudo
			//baseProAbs:=CDbl(ModifiedDamage/2)*CDbl(parryper/100);
			baseProAbs:=(cdbl(shield.hp))/cdbl(shield.maxhp);
			if (baseProAbs<=0)
				baseProAbs:=1;
			endif
			if (RandomInt(100)<parryper)
				SubToRaw(baseProAbs*ModifiedDamage);
				PlaySoundEffect(defender,0x43);
				//2% di perdere 1 punto
				RuinItem(shield,2,1,defender);
				SendSysMessage(defender,"You succesfully parry.");
			endif
		endif
	endif
			*/
				if( chance < 0.01 )
					chance = 0.01;
				if ( OldPolParry && chance < 0.05)
					chance = 0.05;
				else if ( OldPolParry && chance > 0.95)
					chance = 0.95;

				/*
				FORMULA: Displayed AR = ((Parrying Skill * Base AR of Shield) ÷ 200) + 1 

				FORMULA: % Chance of Blocking = parry skill - (shieldAR * 2)

				FORMULA: Melee Damage Absorbed = (AR of Shield) / 2 | Archery Damage Absorbed = AR of Shield 
				*/
				if( owner.CheckSkill( SkillName.Parry, chance ) )
				{
				    int originalDamage = damage;

					if( weapon.Skill == SkillName.Archery )
						damage -= (int)ar;
					else
						damage -= (int)(ar / 2.0);
					
					damage = Midgard.Engines.PVMAbsorbtions.Core.OnHitShield(this,weapon,damage); //mod by Magius(CHE)

					if( damage < 0 )
						damage = 0;

					if( owner.Player )
						owner.SendMessage( owner.Language == "ITA" ? "Hai bloccato l'attacco!" : "You block an attack!" );

					owner.FixedEffect( 0x37B9, 10, 16 );
                    
					Effects.PlaySound( owner.Location, owner.Map, 0x139 ); // mod by Dies Irae

					int absorbed = originalDamage - damage;

					if( owner.PlayerDebug )
						owner.SendMessage( "Debug: your shield adsorbed {0} damage", absorbed );

					if( ChanceofRuin > Utility.RandomDouble() )
					{
						int wear;

						if( weapon.Type == WeaponType.Bashing || weapon is WarAxe ) // mod by Dies Irae
							wear = absorbed / 2;
						else
							wear = Utility.Random( 2 );

						if( wear > 0 && MaxHitPoints > 0 )
						{
							if( HitPoints >= wear )
							{
								HitPoints -= wear;
								wear = 0;
							}
							else
							{
								wear -= HitPoints;
								HitPoints = 0;
							}

							if( wear > 0 )
							{
								if( MaxHitPoints > wear )
								{
									MaxHitPoints -= wear;

									if( Parent is Mobile )
										( (Mobile)Parent ).LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061121 ); // Your equipment is severely damaged.
								}
								else
								{
									if( Parent is Mobile )
									{
										Mobile parent = Parent as Mobile;
										parent.SendMessage( parent.Language == "ITA" ? "Il tuo equipaggamento è troppo vecchio e rovinato! Si è distrutto!" : "Your equipment is too old and ruined! It is broken!" );
										parent.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "*Houch!!*" );
									}

									Delete();
								}
							}
						}
					}

                    /*
				    if( 25 > Utility.Random( 100 ) ) // 25% chance to lower durability
					{
						if( Core.AOS && ArmorAttributes.SelfRepair > Utility.Random( 10 ) )
						{
							HitPoints += 2;
						}
						else
						{
							int wear = Utility.Random( 2 );

							if( wear > 0 && MaxHitPoints > 0 )
							{
								if( HitPoints >= wear )
								{
									HitPoints -= wear;
									wear = 0;
								}
								else
								{
									wear -= HitPoints;
									HitPoints = 0;
								}

								if( wear > 0 )
								{
									if( MaxHitPoints > wear )
									{
										MaxHitPoints -= wear;

										if( Parent is Mobile )
											((Mobile)Parent).LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061121 ); // Your equipment is severely damaged.
									}
									else
									{
										Delete();
									}
								}
							}
						}
					}
                    */
				}

				return damage;
			}
		}

        #region mod by Dies Irae
        public override bool CanEquip( Mobile from )
        {
            if( from.Skills[ SkillName.Magery ].Base >= 50.0 && BlockCircle != -1 )
            {
                from.SendMessage( from.Language == "ITA" ? "Non puoi indossarlo per via del tuo potere magico." : "You cannot use this item because of your magical power." );
                return false;
            }

            return base.CanEquip( from );
        }
        #endregion

	    /*
		#region Mondain's Legacy ICraftable
		public override int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ArmorQuality)quality;

			if ( makersMark )
				Crafter = from;
			
			if ( !craftItem.ForceNonExceptional )
			{
				Type resourceType = typeRes;
	
				if ( resourceType == null )
					resourceType = craftItem.Resources.GetAt( 0 ).ItemType;
	
				Resource = CraftResources.GetFromType( resourceType );
			}

			PlayerConstructed = true;

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;			
			
			if ( Core.AOS && tool is BaseRunicTool )
				((BaseRunicTool)tool).ApplyAttributesTo( this );
			
			return quality;
		}
		#endregion
		*/
	}
}
