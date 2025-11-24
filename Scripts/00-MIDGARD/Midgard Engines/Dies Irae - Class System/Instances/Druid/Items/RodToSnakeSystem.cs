/***************************************************************************
 *                               RodToSnakeSystem.cs
 *
 *   begin                : 01 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.SpellSystem;

using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.Classes
{
    public class RodToSnakeSystem
    {
        private class RodInfo
        {
            private readonly DruidTome m_Tome;
            private readonly BaseCreature m_Summon;

            public RodInfo( BaseCreature summon, DruidTome tome )
            {
                m_Summon = summon;
                m_Tome = tome;
            }

            public void Restore( Mobile from )
            {
                DruidTome tomeToRestore = m_Tome;

                if( tomeToRestore != null && !tomeToRestore.Deleted )
                {
                    Container pack = from.Backpack;

                    if( pack != null )
                        pack.DropItem( tomeToRestore );
                    else
                        Console.WriteLine( "Warning: summoned rod has no valid backpack to be restored into." );
                }
                else
                    Console.WriteLine( "Warning: summoned rod has no tome associated with." );

                if( m_Summon != null && !m_Summon.Deleted )
                    m_Summon.Delete();
            }
        }

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "BastoneInSerpente", AccessLevel.Player, new CommandEventHandler( RodToSnake_OnCommand ) );
        }

        [Usage( "BastoneInSerpente" )]
        [Description( "Da al druido il potere di cambiare il suo bastone in serpente." )]
        private static void RodToSnake_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( !ClassSystem.IsDruid( from ) )
            {
                from.SendMessage( "Only druids can use this power." );
            }
            else if( HasRodMorphed( from ) )
            {
                RestoreRod( from );
            }
            else if( ( from.Followers + 1 ) > from.FollowersMax )
            {
                from.SendMessage( "You have too many followers to summon another one." );
            }
            else if( !HasRodEquipped( from ) )
            {
                from.SendMessage( "Your wood friend is not in your hand." );
            }
            else
            {
                from.SendMessage( "Your rod begin to change..." );
                StartMorph( from );
            }
        }

        private const double MorphDurationPerLevel = 2.0;
        private const double AnimateDelay = 1.5;

        private static double GetMorphDuration( int level )
        {
            return MorphDurationPerLevel * level;
        }

        private static bool HasRodMorphed( Mobile from )
        {
            return m_Table != null && m_Table.ContainsKey( from );
        }

        private static RodInfo GetInfo( Mobile from )
        {
            return HasRodMorphed( from ) ? m_Table[ from ] : null;
        }

        private static void StartMorph( Mobile from )
        {
            DruidTome tome = GetEquippedTome( from );
            if( tome == null )
                return;

            if( from.CanBeginAction( typeof( RodToSnakeSystem ) ) )
            {
                from.BeginAction( typeof( RodToSnakeSystem ) );

                int level = Math.Max( DruidSpell.GetFocusLevel( from ), 1 );
                int count = (int)Math.Ceiling( GetMorphDuration( level ) / AnimateDelay );

                if( count > 0 )
                {
                    AnimTimer animTimer = new AnimTimer( from, count );
                    animTimer.Start();

                    double effectiveDuration = ( count * AnimateDelay ) + 1.0;
                    from.Freeze( TimeSpan.FromSeconds( effectiveDuration ) );
                    Timer.DelayCall( TimeSpan.FromSeconds( effectiveDuration ), new TimerStateCallback( Morph_Callback ), new object[] { from, level, tome } );
                }
            }
            else
            {
                from.SendMessage( "{0}, you cannot summon your friend yet.", from.Name );
            }
        }

        private static bool HasRodEquipped( Mobile from )
        {
            Item rod = from.FindItemOnLayer( Layer.TwoHanded );

            return rod != null && rod is DruidTome;
        }

        private static DruidTome GetEquippedTome( Mobile from )
        {
            Item rod = from.FindItemOnLayer( Layer.TwoHanded );

            return rod != null && rod is DruidTome ? (DruidTome)rod : null;
        }

        private static readonly int[] m_AnimIds = new int[] { 245, 266 };

        private class AnimTimer : Timer
        {
            private readonly Mobile m_From;

            public AnimTimer( Mobile from, int count )
                : base( TimeSpan.Zero, TimeSpan.FromSeconds( AnimateDelay ), count )
            {
                m_From = from;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if( !m_From.Mounted && m_From.Body.IsHuman )
                    m_From.Animate( Utility.RandomList( m_AnimIds ), 7, 1, true, false, 0 );

                m_From.PlaySound( 0x208 );
            }
        }

        private static void Morph_Callback( object state )
        {
            object[] states = (object[])state;

            Mobile from = (Mobile)states[ 0 ];
            int level = (int)states[ 1 ];
            DruidTome tome = (DruidTome)states[ 2 ];

            TimeSpan duration = TimeSpan.FromMinutes( ( from.Skills.Spellweaving.Value / 20 ) * level );
            RodSummon summon = new RodSummon();

            SpellHelper.Summon( summon, from, summon.GetIdleSound(), duration, false, false );
            from.FixedParticles( 0x3728, 1, 10, 9910, EffectLayer.Head );

            m_Table[ from ] = new RodInfo( summon, tome );
            tome.Internalize();

            Timer.DelayCall( duration, new TimerStateCallback( RestoreRodCallback ), from );
            Timer.DelayCall( duration + TimeSpan.FromMinutes( 1.0 ), new TimerStateCallback( RemoveRodLock ), from );
        }

        private static readonly Dictionary<Mobile, RodInfo> m_Table = new Dictionary<Mobile, RodInfo>();

        public static void RestoreRod( Mobile from )
        {
            RodInfo info = GetInfo( from );
            if( info != null )
                info.Restore( from );

            if( m_Table.ContainsKey( from ) )
                m_Table.Remove( from );
        }

        private static void RestoreRodCallback( object state )
        {
            RestoreRod( (Mobile)state );
        }

        private static void RemoveRodLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( RodToSnakeSystem ) );
            ( (Mobile)state ).SendMessage( "Your rod is ready again!" );
        }
    }

    public class RodSummon : GiantSerpent
    {
        public override bool Commandable
        {
            get { return true; }
        }

        public override bool InitialInnocent
        {
            get { return true; }
        }

        public override bool IsDispellable
        {
            get { return false; }
        }

        public RodSummon()
        {
        }

        private const double DamageScalar = 0.05;

        public override void OnDamage( int amount, Mobile from, bool willKill )
        {
            if( Controlled || Summoned )
            {
                Mobile master = ( ControlMaster );

                if( master == null )
                    master = SummonMaster;

                if( master != null && master.Player && master.Map == Map && master.InRange( Location, 20 ) )
                {
                    amount = (int)( amount * DamageScalar );

                    if( master.Mana >= amount )
                    {
                        master.Mana -= amount;
                    }
                    else
                    {
                        amount -= master.Mana;
                        master.Mana = 0;
                        master.Damage( amount );

                        master.SendMessage( "Your friend has been damaged." );
                    }
                }
            }

            base.OnDamage( amount, from, willKill );
        }

        public override void Kill()
        {
            DebugSay( "My master wants to make me as a rod!" );
            RodToSnakeSystem.RestoreRod( this );

            base.Kill();
        }

        #region serialization
        public RodSummon( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
        #endregion
    }
}