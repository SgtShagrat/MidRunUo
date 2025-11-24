/***************************************************************************
 *                                  NarcoticBandage.cs
 *                            		-------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Benda per narcotizzare un bersaglio.
 * 
 ***************************************************************************/

#define DebugNarcoticBandage

using System;
using System.Collections.Generic;

using Midgard.Engines.Classes;

using Server;
using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Midgard.Items
{
    public class NarcoticBandage : Item
    {
        private static readonly double m_StealingToUse = 90.0;
        private static readonly double m_HidingToUse = 90.0;
        private static readonly double m_StealthToUse = 90.0;
        private static readonly double m_AnatomyToUse = 90.0;

        private static readonly double DelayOnFail = 60.0;
        private static readonly double DelayOnSuccess = 30.0;

        private static readonly int BandageDuration = 600;

        private Timer m_NarcoticTimer;
        private NarcoticLevel m_Level;
        private int m_Lifespan;
        private Timer m_Timer;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public NarcoticLevel Level
        {
            get { return m_Level; }
            set
            {
                m_Level = value;
                switch( m_Level )
                {
                    case NarcoticLevel.Light: Hue = 2034; break;
                    case NarcoticLevel.Regular: Hue = 2119; break;
                    case NarcoticLevel.Medium: Hue = 2124; break;
                    case NarcoticLevel.Heavy: Hue = 2437; break;
                    default: Hue = 0; break;
                }
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int Lifespan
        {
            get { return m_Lifespan; }
            set
            {
                m_Lifespan = ( value > 0 ) ? value : 0;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsDried
        {
            get { return ( m_Lifespan < 1 ); }
        }

        [Constructable]
        public NarcoticBandage()
            : this( NarcoticLevel.None )
        {
        }

        [Constructable]
        public NarcoticBandage( NarcoticLevel level )
            : base( 0xE21 )
        {
            m_Level = level;
            m_Lifespan = BandageDuration;

            Weight = 0.1;
            Stackable = false;
        }

        public NarcoticBandage( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
#if DebugNarcoticBandage
            Console.WriteLine( "Debug DebugNarcoticBandage:" );
#endif
            Midgard2PlayerMobile thief = from as Midgard2PlayerMobile;
            if( thief == null )
                return;

            if( !ClassSystem.IsThief( from ) )
            {
                thief.SendMessage( "You are not allowed to use this item." );
            }
            else if( !IsChildOf( thief.Backpack ) )
            {
                thief.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
            /*
            else if( thief.Mounted && !( thief.Mount is EtherealMount ) )
            {
                thief.SendLocalizedMessage( 1065705 ); // You cannot use a narcotic bandage while mounting a normal mount.
            }
            */
            else if( !thief.Body.IsHuman )
            {
                thief.SendLocalizedMessage( 1065706 ); // You cannot use a narcotic bandage while in a such form.
            }
            else if( !IsEmptyHanded( thief ) )
            {
                thief.SendLocalizedMessage( 1065707 ); // You must have both hands free to use a narcotic bandage.
            }
            else if( thief.Region.IsPartOf( typeof( Server.Regions.Jail ) ) )
            {
                thief.SendLocalizedMessage( 1065708 ); // Hey, this actions are prohibited in Jail.
            }
            else if( !thief.CanBeginAction( typeof( NarcoticBandage ) ) )
            {
                thief.SendLocalizedMessage( 1065709 ); // You cannot use a narcotic bandage yet.
            }
            else if( Level == NarcoticLevel.None )
            {
                if( from.CheckAlive() )
                {
                    from.SendLocalizedMessage( 1065712 ); // Target some narcotic in your pack...
                    from.Target = new RechargeTarget( this );
                }
            }
            else if( IsDried )
            {
                thief.SendMessage( "This bandage is dried. It's no more usable." );
            }
            else
            {
#if DebugNarcoticBandage
                Console.WriteLine( "\tVictimTarget sent to {0}", thief.Name );
#endif
                thief.SendLocalizedMessage( 1065711 ); // Target the guy you want to numb.
                thief.Target = new VictimTarget( thief, this );
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( Level != NarcoticLevel.None )
            {
                list.Add( 1065701 ); // imbued with narcotic
                list.Add( 1065704, Enum.GetName( typeof( NarcoticLevel ), Level ) ); // narcotic level: ~1_LEVEL~

                if( m_Lifespan > 0 )
                    list.Add( 1072517, m_Lifespan.ToString() ); // Lifespan: ~1_val~ seconds
            }
            else if( IsDried )
            {
                list.Add( "dried" );
            }
            else
            {
                list.Add( 1065702 ); // clean
            }
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries( from, list );

            if( from.Alive && ClassSystem.IsThief( from ) && Level == NarcoticLevel.None && !IsDried )
                list.Add( new RechargeEntry( this, IsChildOf( from.Backpack ) ) );
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Level != NarcoticLevel.None )
                list.Add( 1065700, "narcotic" ); // ~1_CHARGED~ bandage
            else if( IsDried )
                list.Add( "dried bandage" );
            else
                list.Add( 1065700, "strange" ); // ~1_CHARGED~ bandage
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Level != NarcoticLevel.None )
                LabelTo( from, "narcotic bandage" );
            else if( IsDried )
                LabelTo( from, "dried bandage" );
            else
                LabelTo( from, "strange bandage" );
        }

        private static bool IsEmptyHanded( Mobile from )
        {
#if DebugNarcoticBandage
            Console.WriteLine( "\tIsEmptyHanded Called correctly." );
#endif
            if( from.FindItemOnLayer( Layer.OneHanded ) != null )
                return false;

            if( from.FindItemOnLayer( Layer.TwoHanded ) != null )
                return false;

            return true;
        }

        public virtual void StartTimer()
        {
            if( m_Timer != null )
                return;

            m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 10 ), TimeSpan.FromSeconds( 10 ), new TimerCallback( Slice ) );
            m_Timer.Priority = TimerPriority.OneSecond;
        }

        public virtual void StopTimer()
        {
            if( m_Timer != null )
                m_Timer.Stop();

            m_Timer = null;
        }

        public virtual void Slice()
        {
            m_Lifespan -= 10;

            InvalidateProperties();

            if( m_Lifespan <= 0 )
                Decay();
        }

        public virtual void Decay()
        {
            if( !Deleted )
                Delete();
        }

        private class RechargeEntry : ContextMenuEntry
        {
            #region campi
            private NarcoticBandage m_Bandage;
            private bool m_Enabled;
            #endregion

            #region costruttori
            public RechargeEntry( NarcoticBandage bandage, bool enabled )
                : base( 1043 )
            {
                m_Bandage = bandage;

                if( !enabled )
                {
                    Flags |= CMEFlags.Disabled;
                    m_Enabled = false;
                }
                else
                    m_Enabled = true;
            }
            #endregion

            #region metodi
            public override void OnClick()
            {
                if( m_Bandage.Deleted )
                    return;

                if( !m_Enabled )
                    return;

                Mobile from = Owner.From;

                if( from.CheckAlive() )
                {
                    from.SendLocalizedMessage( 1065712 ); // Target some narcotic in your pack...
                    from.Target = new RechargeTarget( m_Bandage );
                }
            }
            #endregion
        }

        private class RechargeTarget : Target
        {
            #region campi
            private NarcoticBandage m_Bandage;
            #endregion

            #region costruttori
            public RechargeTarget( NarcoticBandage bandage )
                : base( -1, false, TargetFlags.None )
            {
                m_Bandage = bandage;
            }
            #endregion

            #region metodi
            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Bandage == null || m_Bandage.Deleted )
                    return;

                if( m_Bandage.Level != NarcoticLevel.None )
                {
                    from.SendLocalizedMessage( 1065713 ); // Hey, how much narc would you use on this bandage: it's full!!
                }
                else if( !( targeted is BaseNarcoticPotion ) )
                {
                    from.SendLocalizedMessage( 1065714 ); // Target a narcotic potion, fool!				
                }
                else
                {
                    BaseNarcoticPotion bnp = ( (BaseNarcoticPotion)targeted );

                    bnp.Consume();
                    m_Bandage.Level = bnp.Level;

                    m_Bandage.Lifespan = BandageDuration;
                    m_Bandage.StartTimer();

                    from.SendLocalizedMessage( 1065715 ); // Your bandage is now dropping narcotic venom...
                }
            }
            #endregion
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 1 );

            writer.Write( m_Lifespan );
            writer.Write( (int)m_Level );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        m_Lifespan = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        if( version < 1 )
                            reader.ReadDateTime();
                        m_Level = (NarcoticLevel)reader.ReadInt();
                        break;
                    }
            }

            if( m_Lifespan > 0 )
                StartTimer();
        }
        #endregion

        private class VictimTarget : Target
        {
            #region campi
            private Midgard2PlayerMobile m_Thief;
            private Midgard2PlayerMobile m_Victim;
            private NarcoticBandage m_Bandage;
            private double m_Duration;
            #endregion

            #region costruttori
            public VictimTarget( Midgard2PlayerMobile thief, NarcoticBandage bandage )
                : base( 1, false, TargetFlags.None )
            {
                m_Thief = thief;
                m_Bandage = bandage;
                m_Duration = GetNarcoticDuration( m_Bandage.Level );
#if DebugNarcoticBandage
                Console.WriteLine( "\tVictimTarget created." );
#endif
            }
            #endregion

            #region metodi
            protected override void OnTarget( Mobile from, object target )
            {
#if DebugNarcoticBandage
                Console.WriteLine( "\tOnTarget called correctly." );
#endif
                if( m_Bandage == null || m_Bandage.Deleted )
                    return;

                if( from.CanBeginAction( typeof( NarcoticBandage ) ) )
                {
                    if( !m_Bandage.IsChildOf( from.Backpack ) )
                    {
                        BadAction( from, 1065725, true ); // The bandage must be in your pack for you to use it, fool!
                    }
                    else if( m_Bandage.IsDried )
                    {
                        BadAction( from, "This bandage became dry. You cannot use it!", false );
                    }
                    else if( !from.Hidden )
                    {
                        BadAction( from, 1065726, false ); // Don't you think you should be hidden, guy?
                    }
                    else if( !CheckThiefSkills( from ) )
                    {
                        BadAction( from, null, false );
                    }
                    else
                    {
                        m_Victim = target as Midgard2PlayerMobile;

                        if( m_Victim != null )
                        {
#if DebugNarcoticBandage
                            Console.WriteLine( "\tTarget is Midgard2PlayerMobile." );
#endif
                            if( m_Victim == from )
                            {
#if DebugNarcoticBandage
                                Console.WriteLine( "\tSame target." );
#endif
                                BadAction( from, 1065727, false );	// Do you need help to sleep?
                            }
                            else if( m_Victim.Mounted && !m_Thief.Mounted )
                            {
#if DebugNarcoticBandage
                                Console.WriteLine( "\tMounted target, thief walking." );
#endif
                                BadAction( from, "You cannot use a narcotic bangage on a mounted guy without riding a mount!", false );
                            }
                            else if( !m_Victim.Mounted && m_Thief.Mounted )
                            {
#if DebugNarcoticBandage
                                Console.WriteLine( "\tMounted thief, victim walking." );
#endif
                                BadAction( from, "You cannot use a narcotic bangage on a walking guy while riding a mount!", false );
                            }
                            //							else if( !(from.Direction == m_Victim.Direction ) )
                            //							{
                            //#if DebugNarcoticBandage
                            //								Console.WriteLine( "\tBad Direction." );
                            //#endif
                            //							   	BadAction( from, 1065730, true ); // You must be behind your victim: same direction.					
                            //							}
                            else if( !IsBehind( from, m_Victim ) )
                            {
#if DebugNarcoticBandage
                                Console.WriteLine( "\tThief is not behind victim." );
#endif
                                BadAction( from, 1065730, true ); // You must be behind your victim: same direction.
                            }
                            else
                            {
#if DebugNarcoticBandage
                                Console.WriteLine( "\tAll checks passed." );
#endif
                                from.BeginAction( typeof( NarcoticBandage ) );
                                m_Bandage.Level = NarcoticLevel.None;
                                TrySmothingVictim();
                            }
                        }
                        else
                        {
                            BadAction( from, 1065731, true ); // Bad target guy...
                        }
                    }
                }
                else
                {
                    from.SendLocalizedMessage( 1065716 ); // You cannot use a narcotic bandage yet.
                }
            }

            private void TrySmothingVictim()
            {
#if DebugNarcoticBandage
                Console.WriteLine( "\tTrySmothingVictim called correctly." );
#endif
                if( !CheckSmothResisted( m_Thief, m_Victim, m_Bandage.Level ) )
                {
#if DebugNarcoticBandage
                    Console.WriteLine( "\tVictim not resisted." );
#endif
                    m_Thief.SendLocalizedMessage( 1065717 ); // You have narcotized your victim.
                    // m_Victim.SendLocalizedMessage( 1065718 ); // You fall down in a deep sleep...

                    string message = m_Victim.TrueLanguage == LanguageType.Ita ? "* sei stato narcotizzato *" : "* you have been narcotized *";
                    m_Victim.PublicOverheadMessage( MessageType.Regular, 0xB32, true, message );

                    m_Bandage.m_NarcoticTimer = new NarcoticTimer( m_Thief, m_Victim, m_Duration );
                    m_Bandage.m_NarcoticTimer.Start();

                    new AnimTimer( m_Victim ).Start();
                }
                else
                {
#if DebugNarcoticBandage
                    Console.WriteLine( "\tVictim resisted." );
#endif
                    m_Thief.SendLocalizedMessage( 1065719 ); // Your victim resisted to narcotic.
                    m_Victim.SendLocalizedMessage( 1065720 ); // Somebody tryed to narcotize you.
                    m_Thief.CriminalAction( false );

                    string message = String.Format( "You notice {0} trying to agress {1}.", m_Thief.Name, m_Victim.Name );

                    foreach( NetState ns in m_Thief.GetClientsInRange( 8 ) )
                    {
                        if( ns != m_Thief.NetState )
                            ns.Mobile.SendMessage( message );
                    }

                    Timer.DelayCall( TimeSpan.FromSeconds( DelayOnFail ), new TimerCallback( RemoveBandageLock ) );
                }
            }

            private static double GetNarcoticDuration( NarcoticLevel level )
            {
                double duration;

                switch( level )
                {
                    case NarcoticLevel.Light: duration = 5.0; break;
                    case NarcoticLevel.Medium: duration = 7.0; break;
                    case NarcoticLevel.Regular: duration = 9.0; break;
                    case NarcoticLevel.Heavy: duration = 12.0; break;
                    default: duration = 5.0; break;
                }
#if DebugNarcoticBandage
                Console.WriteLine( "\tGetNarcoticDuration - level: {0} - duration {1}", level, duration );
#endif
                return duration;
            }

            private void RemoveBandageLock()
            {
                m_Thief.EndAction( typeof( NarcoticBandage ) );
                m_Thief.SendLocalizedMessage( 1065721 ); // You can use a narcotic bandage again!
            }

            private static bool CheckSmothResisted( Mobile thiev, Mobile victim, NarcoticLevel level )
            {
                int thievPars = ( thiev.Str * 10 ) + ( thiev.Dex * 10 ) + thiev.Skills[ SkillName.Anatomy ].Fixed;
                int victimPars = ( victim.Str * 10 ) + ( victim.Dex * 10 ) + thiev.Skills[ SkillName.TasteID ].Fixed;

                double bonusNarcoticLevel;
                switch( level )
                {
                    case NarcoticLevel.Light: bonusNarcoticLevel = 1.0; break;
                    case NarcoticLevel.Medium: bonusNarcoticLevel = 1.2; break;
                    case NarcoticLevel.Regular: bonusNarcoticLevel = 1.4; break;
                    case NarcoticLevel.Heavy: bonusNarcoticLevel = 1.6; break;
                    default: bonusNarcoticLevel = 1.0; break;
                }

                int chance;
                if( victimPars > 0 )
                    chance = (int)( ( ( 50 * thievPars ) / (double)victimPars ) * bonusNarcoticLevel );
                else
                    chance = 100;

#if DebugNarcoticBandage
                Console.WriteLine( "\tCheckSmothResisted - chanceToResist: {0}", chance );
#endif
                return chance < Utility.Random( 100 );
            }

            private static void BadAction( Mobile guy, object message, bool isCriminalAction )
            {
                if( message != null )
                {
                    if( message is string )
                        guy.SendMessage( (string)message );
                    else
                        guy.SendLocalizedMessage( (int)message );
                }

                if( isCriminalAction )
                    guy.CriminalAction( false );

                double skillToCheck = ( guy.Skills[ SkillName.Hiding ].Value + guy.Skills[ SkillName.Stealth ].Value ) / 240.0;

                if( Utility.RandomDouble() > skillToCheck )
                    guy.RevealingAction();
            }

            private static bool CheckThiefSkills( Mobile thiev )
            {
                if( thiev == null || thiev.Deleted )
                    return false;

                if( thiev.Skills[ SkillName.Stealing ].Value < m_StealingToUse )
                {
                    thiev.SendLocalizedMessage( 1065722, string.Format( "{0}\t{1}",
                                                m_StealingToUse.ToString( "F1" ), SkillName.Stealing ) ); // You must have ~1_VALUE~ in ~2_SKILL~ to use this narcotic bandage.
                    return false;
                }
                else if( thiev.Skills[ SkillName.Hiding ].Value < m_HidingToUse )
                {
                    thiev.SendLocalizedMessage( 1065722, string.Format( "{0}\t{1}",
                                                m_HidingToUse.ToString( "F1" ), SkillName.Hiding ) ); // You must have ~1_VALUE~ in ~2_SKILL~ to use this narcotic bandage.
                    return false;
                }
                else if( thiev.Skills[ SkillName.Stealth ].Value < m_StealthToUse )
                {
                    thiev.SendLocalizedMessage( 1065722, string.Format( "{0}\t{1}",
                                                m_StealthToUse.ToString( "F1" ), SkillName.Stealth ) ); // You must have ~1_VALUE~ in ~2_SKILL~ to use this narcotic bandage.
                    return false;
                }
                else if( thiev.Skills[ SkillName.Anatomy ].Value < m_AnatomyToUse )
                {
                    thiev.SendLocalizedMessage( 1065722, string.Format( "{0}\t{1}",
                                                m_AnatomyToUse.ToString( "F1" ), SkillName.Anatomy ) ); // You must have ~1_VALUE~ in ~2_SKILL~ to use this narcotic bandage.
                    return false;
                }
                else
                    return true;
            }

            private static bool IsBehind( Mobile thief, Mobile victim )
            {
#if true
                Point3D thiefLoc = thief.Location;
                Direction thiefDir = thief.Direction;
                Point3D victimLoc = victim.Location;
                Direction victimDir = victim.Direction;
#if DebugNarcoticBandage
                Console.WriteLine( "\ttLoc: {0} - vLoc: {1} - tDir: {2} - vDir: {3}",
                                   thiefLoc, victimLoc, thiefDir, victimDir );
#endif
                if( thiefLoc == victimLoc ) // too close
                {
#if DebugNarcoticBandage
                    Console.WriteLine( "\tThief same location of victim." );
#endif
                    return false;
                }
                else if( !Utility.InRange( thiefLoc, victimLoc, 1 ) ) // too distant
                {
#if DebugNarcoticBandage
                    Console.WriteLine( "\tThief too distant from victim." );
#endif
                    return false;
                }
                else if( ( ( thiefDir & Direction.Mask ) != Utility.GetDirection( thiefLoc, victimLoc ) ) )
                {
#if DebugNarcoticBandage
                    Console.WriteLine( "\tNot same direction." );
#endif
                    return false;
                }
                else if( ( ( victimDir & Direction.Mask ) != Utility.GetDirection( thiefLoc, victimLoc ) ) )
                {
#if DebugNarcoticBandage
                    Console.WriteLine( "\tNot same direction." );
#endif
                    return false;
                }
                //                    else if( !( Utility.GetDirection( thiefLoc, victimLoc ) == thiefDir ) ) // not same direction
                //                    {
                //#if DebugNarcoticBandage
                //                        Console.WriteLine( "\tNot same direction." );
                //#endif
                //                        return false;
                //                    }
                else
                {
#if DebugNarcoticBandage
                    Console.WriteLine( "\tThief is behind of victim." );
#endif
                    return true;
                }
#else
                    int offX, offY;
                    switch( victim.Direction )
                    {
                        case Direction.North: offX = 0; offY = 1; break;
                        case Direction.West: offX = 1; offY = 0; break;
                        case Direction.South: offX = 0; offY = -1; break;
                        case Direction.East: offX = -1; offY = 0; break;

                        case Direction.Up: offX = 1; offY = 1; break;
                        case Direction.Left: offX = 1; offY = -1; break;
                        case Direction.Down: offX = -1; offY = -1; break;
                        case Direction.Right: offX = -1; offY = 1; break;

                        default: offX = 0; offY = 0; break;
                    }

                    if( ( thief.Location.X == victim.Location.X + offX ) && ( thief.Location.Y == victim.Location.Y + offY ) )
                        return true;
                    else
                        return false;
#endif
            }
            #endregion
        }

        private class AnimTimer : Timer
        {
            private readonly Mobile m_From;

            public AnimTimer( Mobile from )
                : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.5 ) )
            {
                m_From = from;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if( m_From == null || m_From.Deleted )
                {
                    Stop();
                    return;
                }

                if( m_From is Midgard2PlayerMobile && !( (Midgard2PlayerMobile)m_From ).IsSmothed )
                {
                    Stop();
                    return;
                }

                // Animate( EmoteList[i].Anim, 5, 1, true, false, 0 );
                m_From.Animate( 33, 5, 1, true, false, 0 );
                m_From.PlaySound( m_From.Female ? 785 : 1056 );
            }
        }

        private class NarcoticTimer : Timer
        {
            #region campi
            private Midgard2PlayerMobile m_Thief;
            private Midgard2PlayerMobile m_Victim;
            private double m_Duration;
            #endregion

            #region costruttori
            public NarcoticTimer( Midgard2PlayerMobile thief, Midgard2PlayerMobile victim, double duration )
                : base( TimeSpan.FromSeconds( duration ) )
            {
                Priority = TimerPriority.FiftyMS;

                m_Thief = thief;
                m_Victim = victim;
                m_Duration = duration;

                m_Victim.IsSmothed = true;
                // m_Victim.Kill();
                m_Victim.Freeze( TimeSpan.FromSeconds( m_Duration ) );

#if DebugNarcoticBandage
                Console.WriteLine( "\tNarcotizeTimer started correctly." );
#endif
            }
            #endregion

            #region metodi
            protected override void OnTick()
            {
#if DebugNarcoticBandage
                Console.WriteLine( "\tNarcotizeTimer.OnTick called correctly." );
#endif
                m_Victim.IsSmothed = false;
                // m_Victim.Resurrect();
                // m_Victim.Heal( m_Victim.HitsMax );

                #region Corspe found...
                /*
                if( m_Victim.Corpse is Corpse )
                {
                    Corpse c = (Corpse)m_Victim.Corpse;
#if DebugNarcoticBandage
                    Console.WriteLine( "\tCorpse found correctly." );
#endif
                    c.Open( m_Victim, true );
                    
                    // c.Delete();
                    c.Visible = false;
                    c.Movable = false; // DEBUG by Dies Irae
                }
                */
                #endregion

                #region Corpse not found
                /*
                else
                {
#if DebugNarcoticBandage
                    Console.WriteLine( "\tCorpse NOT found correctly." );
#endif
                    Container pack = m_Victim.Backpack;
                    Container corpse = m_Victim.Corpse;

                    if( pack != null && corpse != null )
                    {
                        List<Item> items = new List<Item>( corpse.Items );

                        for( int i = 0; i < items.Count; ++i )
                        {
                            Item item = items[ i ];

                            if( item.Layer != Layer.Hair && item.Layer != Layer.FacialHair && item.Movable )
                                pack.DropItem( item );
                        }

                        List<Item> itemsRemainedInCorpse = new List<Item>( corpse.Items );
                        int counter = 0;

                        for( int i = 0; i < itemsRemainedInCorpse.Count; ++i )
                        {
                            Item rem = itemsRemainedInCorpse[ i ];
                            if( rem.Layer != Layer.Hair && rem.Layer != Layer.FacialHair && rem.Movable )
                                counter++;
                        }
                        if( counter == 0 )
                        {
#if DebugNarcoticBandage
                            Console.WriteLine( "\tItems dropped to pack." );
#endif
                            corpse.Delete();
                        }
                    }
                }
                */
                #endregion

                m_Victim.SendLocalizedMessage( 1065723 ); // You woke up... what's happened??!!
                m_Thief.SendLocalizedMessage( 1065724 ); // Your victim woke up... may be a good reason to run away.

                DelayCall( TimeSpan.FromSeconds( DelayOnSuccess ), new TimerCallback( RemoveBandageLock ) );
                Stop();
            }

            private void RemoveBandageLock()
            {
                m_Thief.EndAction( typeof( NarcoticBandage ) );
                m_Thief.SendLocalizedMessage( 1065721 ); // You can use a Narcotic Bangare again!
            }
            #endregion
        }
    }
}