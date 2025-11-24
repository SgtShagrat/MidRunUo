/***************************************************************************
 *                               BlessCommand.cs
 *                            -------------------
 *   begin                : 21 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Commands;

namespace Midgard.Engines.Races
{
    public class BlessCommand
    {
        public const double DelayBless = 5.0;

        public static void RegisterCommands()
        {
            CommandSystem.Register( "Urlo", AccessLevel.Player, new CommandEventHandler( Bless_OnCommand ) );
        }

        [Usage( "Urlo" )]
        [Description( "Da un bonus ai compagni di clan attorno" )]
        public static void Bless_OnCommand( CommandEventArgs e )
        {
            if( !Config.BlessEnabled )
            {
                e.Mobile.SendMessage( "Questo potere e' stato disabilitato." );
                return;
            }

            if( !e.Mobile.Alive )
            {
                e.Mobile.SendMessage( "Non puoi usare questo comando in questo stato." );
                return;
            }

            Mobile from = e.Mobile;

            if( from == null )
                return;

            if( e.Length == 0 )
            {
                if( from.Race is MidgardRace && ( (MidgardRace)from.Race ).SupportsBless )
                {
                    if( from.CanBeginAction( typeof( BlessCommand ) ) )
                        DoBless( from );
                    else
                        from.SendMessage( "Non puoi usare ancora questo potere." );
                }
                else
                    from.SendMessage( "Non puoi usare questo potere." );
            }
            else
                from.SendMessage( "Uso del comando: [Urlo" );
        }

        private static bool IsAggressor( Mobile from, Mobile m )
        {
            foreach( AggressorInfo info in from.Aggressors )
            {
                if( m == info.Attacker && !info.Expired )
                    return true;
            }

            return false;
        }

        private static bool IsAggressed( Mobile from, Mobile m )
        {
            foreach( AggressorInfo info in from.Aggressed )
            {
                if( m == info.Defender && !info.Expired )
                    return true;
            }

            return false;
        }

        private static void DoBless( Mobile from )
        {
            from.BeginAction( typeof( BlessCommand ) );

            from.PlaySound( 1098 );
            from.PublicOverheadMessage( Server.Network.MessageType.Regular, 0x3B2, true, "* Urla *" );

            List<Mobile> targets = new List<Mobile>();

            Map map = from.Map;

            if( map != null )
            {
                IPooledEnumerable eable = map.GetMobilesInRange( from.Location, 10 );

                foreach( Mobile m in eable )
                {
                    if( from == m || !IsInClan( from, m ) )
                        continue;

                    if( from.CanBeBeneficial( m, false ) && !IsAggressor( from, m ) && !IsAggressed( from, m ) )
                        targets.Add( m );
                }

                eable.Free();
            }

            if( targets.Count <= 0 )
                return;

            foreach( Mobile m in targets )
            {
                from.DoBeneficial( m );

                m.SendMessage( "Il Clan ti fornisce il suo vigore!" );

                from.VirtualArmorMod += 5;
                new OldInternalTimer( from, +5, TimeSpan.FromSeconds( 120.0 ) ).Start();

                m.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
                m.PlaySound( 0x1EA );
            }
        }

        private static bool IsInClan( Mobile from, Mobile target )
        {
            if( from == target )
                return true;

            return from.Race == target.Race && from.Race is MidgardRace && ( (MidgardRace)from.Race ).SupportsBless;
        }

        private class OldInternalTimer : Timer
        {
            private readonly Mobile m_Owner;
            private readonly int m_Val;

            public OldInternalTimer( Mobile owner, int val, TimeSpan duration )
                : base( duration )
            {
                Priority = TimerPriority.OneSecond;

                m_Owner = owner;
                m_Val = val;
            }

            protected override void OnTick()
            {
                m_Owner.VirtualArmorMod -= m_Val;
                if( m_Owner.VirtualArmorMod < 0 )
                    m_Owner.VirtualArmorMod = 0;

                DelayCall( TimeSpan.FromSeconds( DelayBless ), new TimerStateCallback( ReleaseSpecialCommandsLock ), m_Owner );
            }

            private static void ReleaseSpecialCommandsLock( object state )
            {
                ( (Mobile)state ).EndAction( typeof( BlessCommand ) );
                ( (Mobile)state ).SendMessage( "Puoi usare di nuovo il tuo potere razziale." );
            }
        }
    }
}