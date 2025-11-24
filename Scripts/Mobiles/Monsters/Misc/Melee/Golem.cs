using System;

using Server.Engines.Craft;
using Server.Items;
using Server.Network;

using Midgard.Engines.OldCraftSystem;
using Midgard.Items.MusicBox;

namespace Server.Mobiles
{
	[CorpseName( "a golem corpse" )]
	public class Golem : BaseCreature, IRepairable
	{
		private bool m_Stunning;

	    public override bool IsScaredOfScaryThings{ get{ return false; } }
		public override bool IsScaryToPets{ get{ return true; } }

		public override bool IsBondable{ get{ return false; } }

		public override FoodType FavoriteFood { get { return FoodType.None; } }

		[Constructable]
		public Golem() : this( false, 1.0 )
		{
		}

		[Constructable]
		public Golem( bool summoned, double scalar ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			Name = "a golem";
			Body = 752;

			if ( summoned )
				Hue = 2101;

			SetStr( (int)(251*scalar), (int)(350*scalar) );
			SetDex( (int)(76*scalar), (int)(100*scalar) );
			SetInt( (int)(101*scalar), (int)(150*scalar) );

			SetHits( (int)(151*scalar), (int)(210*scalar) );

			SetDamage( (int)(13*scalar), (int)(24*scalar) );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, (int)(35*scalar), (int)(55*scalar) );

			if ( summoned )
				SetResistance( ResistanceType.Fire, (int)(50*scalar), (int)(60*scalar) );
			else
				SetResistance( ResistanceType.Fire, (int)(100*scalar) );

			SetResistance( ResistanceType.Cold, (int)(10*scalar), (int)(30*scalar) );
			SetResistance( ResistanceType.Poison, (int)(10*scalar), (int)(25*scalar) );
			SetResistance( ResistanceType.Energy, (int)(30*scalar), (int)(40*scalar) );

			SetSkill( SkillName.MagicResist, (150.1*scalar), (190.0*scalar) );
			SetSkill( SkillName.Tactics, (60.1*scalar), (100.0*scalar) );
			SetSkill( SkillName.Wrestling, (60.1*scalar), (100.0*scalar) );

			if ( summoned )
			{
				Fame = 10;
				Karma = 10;
			}
			else
			{
				Fame = 3500;
				Karma = -3500;
			}

			if ( !summoned )
			{
				PackItem( new IronIngot( Utility.RandomMinMax( 13, 21 ) ) );

				if ( 0.1 > Utility.RandomDouble() )
					PackItem( new PowerCrystal() );

				if ( 0.15 > Utility.RandomDouble() )
					PackItem( new ClockworkAssembly() );

				if ( 0.2 > Utility.RandomDouble() )
					PackItem( new ArcaneGem() );

				if ( 0.25 > Utility.RandomDouble() )
					PackItem( new Gears() );
			}

			ControlSlots = 3;
		}

		#region mod by Dies Irae
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		
			
			if( !Summoned && Utility.RandomDouble() < 0.05 )
			{
				if( IsParagon )
					c.DropItem( MusicBoxGears.RandomMusixBoxGears( TrackRarity.Rare ) );
				else
				{
					if( Utility.RandomBool() )
						c.DropItem( MusicBoxGears.RandomMusixBoxGears( TrackRarity.Common ) );
					else
						c.DropItem( MusicBoxGears.RandomMusixBoxGears( TrackRarity.UnCommon ) );
				}
			}
        }

        #region IRepairable members
        public bool Repair( Mobile from, BaseTool tool )
        {
            CraftSystem system = tool.CraftSystem;
            int number = 0;

            if( system is DefTinkering )
            {
                int damage = HitsMax - Hits;

                if( IsDeadBondedPet )
                {
                    number = 500426; // You can't repair that.
                }
                else if( damage <= 0 )
                {
                    number = 500423; // That is already in full repair.
                }
                else
                {
                    double skillValue = from.Skills[ SkillName.Tinkering ].Value;

                    if( skillValue < 60.0 )
                    {
                        number = 1044153; // You don't have the required skills to attempt this item.
                    }
                    else if( !from.CanBeginAction( typeof( Golem ) ) )
                    {
                        number = 501789; // You must wait before trying again.
                    }
                    else
                    {
                        if( damage > (int)( skillValue * 0.3 ) )
                            damage = (int)( skillValue * 0.3 );

                        damage += 30;

                        if( !from.CheckSkill( SkillName.Tinkering, 0.0, 100.0 ) )
                            damage /= 2;

                        Container pack = from.Backpack;

                        if( pack != null )
                        {
                            int v = pack.ConsumeUpTo( typeof( IronIngot ), ( damage + 4 ) / 5 );

                            if( v > 0 )
                            {
                                Hits += v * 5;

                                number = 1044279; // You repair the item.

                                from.BeginAction( typeof( Golem ) );
                                Timer.DelayCall( TimeSpan.FromSeconds( 12.0 ), new TimerStateCallback( EndGolemRepair ), from );
                            }
                            else
                            {
                                number = 1044037; // You do not have sufficient metal to make that.
                            }
                        }
                        else
                        {
                            number = 1044037; // You do not have sufficient metal to make that.
                        }
                    }
                }

                if( number > 0 )
                    from.SendLocalizedMessage( number );
            }
            else
            {
                from.SendMessage( "That is not a proper tool to repair a golem" );
            }

            return ( number == 1044279 );
        }

		private static void EndGolemRepair( object state )
		{
			((Mobile)state).EndAction( typeof( Golem ) );
		}
        #endregion
        #endregion

        public override bool DeleteOnRelease{ get{ return true; } }

		public override int GetAngerSound()
		{
			return 541;
		}

		public override int GetIdleSound()
		{
			if ( !Controlled )
				return 542;

			return base.GetIdleSound();
		}

		public override int GetDeathSound()
		{
			if ( !Controlled )
				return 545;

			return base.GetDeathSound();
		}

		public override int GetAttackSound()
		{
			return 562;
		}

		public override int GetHurtSound()
		{
			if ( Controlled )
				return 320;

			return base.GetHurtSound();
		}

		public override bool AutoDispel{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( !m_Stunning && 0.3 > Utility.RandomDouble() )
			{
				m_Stunning = true;

				defender.Animate( 21, 6, 1, true, false, 0 );
				this.PlaySound( 0xEE );
				defender.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You have been stunned by a colossal blow!" );

				BaseWeapon weapon = this.Weapon as BaseWeapon;
				if ( weapon != null )
					weapon.OnHit( this, defender );

				if ( defender.Alive )
				{
					defender.Frozen = true;
					Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( Recover_Callback ), defender );
				}
			}
		}

		private void Recover_Callback( object state )
		{
			Mobile defender = state as Mobile;

			if ( defender != null )
			{
				defender.Frozen = false;
				defender.Combatant = null;
				defender.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You recover your senses." );
			}

			m_Stunning = false;
		}

	    private const double DamageScalar = 0.10; // mod by Dies Irae

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( Controlled || Summoned )
			{
				Mobile master = ( this.ControlMaster );

				if ( master == null )
					master = this.SummonMaster;

				if ( master != null && master.Player && master.Map == this.Map && master.InRange( Location, 20 ) )
				{
				    amount = (int) ( amount * DamageScalar ); // mod by Dies Irae

					if ( master.Mana >= amount )
					{
						master.Mana -= amount;
					}
					else
					{
						amount -= master.Mana;
						master.Mana = 0;
						master.Damage( amount );
					}
				}
			}

			base.OnDamage( amount, from, willKill );
		}

		public override bool BardImmune{ get{ return !Core.AOS || Controlled; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public Golem( Serial serial ) : base( serial )
		{
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