/***************************************************************************
 *                                  BaseCraftableTrap.cs
 *                            		--------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Regions;

namespace Midgard.Items
{
    public enum AllowedPlacing
    {
        Everywhere,
        ControlledTown,
        NotInTowns
    }

    public enum TrapLevel
    {
        Light = 1,
        Medium,
        Heavy
    }

    public abstract class BaseCraftableTrap : BaseTrap
    {
        private Timer m_Concealing;

        [CommandProperty( AccessLevel.GameMaster )]
        public TownSystem System { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Placer { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime TimeOfPlacement { get; set; }

        public List<Mobile> DamagedMobiles { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsGuildItem { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Charges { get; set; }

        public virtual int EffectSound { get { return 0; } }
        public virtual int MessageHue { get { return 0; } }
        public virtual int AttackMessage { get { return 0; } }
        public virtual string AttackMessageString { get { return String.Empty; } }
        public virtual int DisarmMessage { get { return 0; } }
        public virtual string DisarmMessageString { get { return String.Empty; } }
        public virtual AllowedPlacing AllowedPlacing { get { return AllowedPlacing.NotInTowns; } }
        public virtual TimeSpan ConcealPeriod { get { return TimeSpan.FromMinutes( 1.0 ); } }

        public virtual bool TimedDecayEnabled { get { return false; } }
        public virtual TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 100.0 ); } }

        public override bool TriggeredByUnTamedMobs { get { return true; } }

        public abstract TrapLevel Level { get; }

        #region constructors
        public BaseCraftableTrap( TownSystem system, Mobile placer, int itemID )
            : base( itemID )
        {
            Visible = false;

            System = system;
            TimeOfPlacement = DateTime.Now;
            Placer = placer;
            DamagedMobiles = new List<Mobile>();
            IsGuildItem = ( Placer != null && Placer.Guild != null );
            Charges = 5;
        }

        public BaseCraftableTrap( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region overridden methods
        public override void OnDelete()
        {
            if( System != null && System.TownTraps.Contains( this ) )
                System.TownTraps.Remove( this );

            base.OnDelete();
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            base.OnMovement( m, oldLocation );

            if( !CheckDecay() && CheckRange( m.Location, oldLocation, 6 ) )
            {
                bool castMessage = ( ( m.Skills[ SkillName.DetectHidden ].Value - 80.0 ) / 20.0 ) > Utility.RandomDouble();

                if( castMessage || ( DamagedMobiles != null && DamagedMobiles.Contains( m ) ) )
                    PrivateOverheadLocalizedMessage( m, 500813, MessageHue, "", "" ); // [trapped]
            }
        }

        public override void OnTrigger( Mobile from )
        {
            if( !IsEnemy( from ) )
                return;

            UnConceal();

            DoVisibleEffect();
            Effects.PlaySound( Location, Map, EffectSound );
            DoAttackEffect( from );

            if( from is Midgard2PlayerMobile && !DamagedMobiles.Contains( from ) )
                DamagedMobiles.Add( from );

            from.LocalOverheadMessage( MessageType.Regular, MessageHue, AttackMessage );

            ConsumeCharge( from );

            BeginConceal();
        }
        #endregion

        #region abstract methods
        public abstract void DoVisibleEffect();

        public abstract void DoAttackEffect( Mobile m );
        #endregion

        #region virtual methods
        public virtual void ConsumeCharge( Mobile from )
        {
            Charges--;
        }

        public static bool IsTown( Point3D loc, Map map )
        {
            if( map == null )
                return false;

            GuardedRegion reg = (GuardedRegion)Region.Find( loc, map ).GetRegion( typeof( GuardedRegion ) );

            return ( reg != null && !reg.IsDisabled() );
        }

        public virtual string IsValidLocation()
        {
            return IsValidLocation( GetWorldLocation(), Map );
        }

        public virtual string IsValidLocation( Point3D p, Map m )
        {
            if( m == null )
                return "You cannot place a trap on that.";

            foreach( Item item in m.GetItemsInRange( p, 0 ) )
            {
                if( item is BaseCraftableTrap )
                    return "There is already a trap at this location.";
            }

            switch( AllowedPlacing )
            {
                case AllowedPlacing.ControlledTown:
                    {
                        TownSystem system = TownSystem.Find( p, m );

                        if( system != null && system == System )
                            return String.Empty;

                        return "This trap can only be placed in your town.";
                    }
                case AllowedPlacing.NotInTowns:
                    {
                        return !IsTown( p, m ) ? String.Empty : "This trap can only be placed outside towns.";
                    }
                default:
                    return string.Empty;
            }
        }

        public virtual bool CheckDecay()
        {
            if( Charges <= 1 && !Deleted )
            {
                Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Delete ) );
                return true;
            }

            if( !TimedDecayEnabled )
                return false;

            TimeSpan decayPeriod = DecayPeriod;

            if( decayPeriod == TimeSpan.MaxValue )
                return false;

            if( ( TimeOfPlacement + decayPeriod ) < DateTime.Now )
            {
                Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Delete ) );
                return true;
            }

            return false;
        }

        public virtual void BeginConceal()
        {
            if( m_Concealing != null )
                m_Concealing.Stop();

            m_Concealing = Timer.DelayCall( ConcealPeriod, new TimerCallback( Conceal ) );
        }

        public virtual void Conceal()
        {
            if( m_Concealing != null )
                m_Concealing.Stop();

            m_Concealing = null;

            if( Charges <= 1 && !Deleted )
                Delete();

            if( !Deleted )
                Visible = false;
        }

        public virtual void UnConceal()
        {
            if( !Deleted )
                Visible = true;
        }

        public virtual bool IsEnemy( Mobile mob )
        {
            if( mob.Hidden && mob.AccessLevel > AccessLevel.Player )
                return false;

            if( !mob.Alive )
                return false;

            if( IsGuildItem && mob.Guild == null )
                return false;

            return true;
        }

        public virtual void PrivateOverheadLocalizedMessage( Mobile to, int number, int hue, string name, string args )
        {
            if( to == null )
                return;

            NetState ns = to.NetState;

            if( ns != null )
                ns.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, hue, 3, number, name, args ) );
        }
        #endregion

        #region static methods
        public static double GetDifficultyToPlaceScalar( TrapLevel level )
        {
            double scalar = 1.0;
            switch( level )
            {
                case TrapLevel.Light:
                    scalar = 1.1;
                    break;
                case TrapLevel.Medium:
                    scalar = 0.8;
                    break;
                case TrapLevel.Heavy:
                    scalar = 0.5;
                    break;
                default:
                    break;
            }

            return scalar;
        }

        public static double GetDifficultyToRemoveSkill( TrapLevel level )
        {
            double skill = 1.0;
            switch( level )
            {
                case TrapLevel.Light:
                    skill = 60.0;
                    break;
                case TrapLevel.Medium:
                    skill = 70.0;
                    break;
                case TrapLevel.Heavy:
                    skill = 80.0;
                    break;
                default:
                    break;
            }

            return skill;
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version

            // version 1
            writer.Write( Charges );

            // version 0
            TownSystem.WriteReference( writer, System );
            writer.Write( Placer );
            writer.Write( TimeOfPlacement );
            writer.Write( DamagedMobiles );
            writer.Write( IsGuildItem );

            if( Visible )
                BeginConceal();
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( DamagedMobiles == null )
                DamagedMobiles = new List<Mobile>();

            switch( version )
            {
                case 1:
                    Charges = reader.ReadInt();
                    goto case 0;
                case 0:
                    {
                        System = TownSystem.ReadReference( reader );
                        Placer = reader.ReadMobile();
                        TimeOfPlacement = reader.ReadDateTime();
                        DamagedMobiles = reader.ReadStrongMobileList();
                        IsGuildItem = reader.ReadBool();
                        break;
                    }
            }

            if( Visible )
                BeginConceal();

            CheckDecay();
        }
        #endregion

        public abstract Type DeedType { get; }

        public void Redeed( Mobile from )
        {
            BaseCraftableTrapDeed deed = Loot.Construct( DeedType ) as BaseCraftableTrapDeed;
            if( deed != null )
            {
                from.AddToBackpack( deed );
                from.SendMessage( "A trap deed has been placed in your backpack!" );
            }
        }
    }
}
