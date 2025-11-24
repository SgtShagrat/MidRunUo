/***************************************************************************
 *                               Dies Irae - ResurrectionMenu.cs
 *
 *   begin                : 08 novembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.IO;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Gumps;
using Server.Menus.Questions;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Menus
{
    public class ResurrectionMenu : QuestionMenu
    {
        private readonly Timer m_Timer;

        private static readonly string[] m_Options = new string[] { "YES - You choose to try to come back to life now.", "NO - You prefer to remain a ghost for now." };

        public ResurrectionMenu( Mobile owner )
            : this( owner, ResurrectMessage.Generic )
        {
        }

        public ResurrectionMenu( Mobile owner, ResurrectMessage msg )
            : base( "", m_Options )
        {
            m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), m_Unfreeze, owner );

            owner.Frozen = true;

            switch ( msg )
            {
                case ResurrectMessage.Healer:
                    Question = "It is possible for you to be resurrected here by this healer. Do you wish to try?";
                    break;
                case ResurrectMessage.VirtueShrine:
                    Question = "It is possible for you to be resurrected at this Shrine to the Virtues. Do you wish to try?";
                    break;
                case ResurrectMessage.ChaosShrine:
                    Question = "It is possible for you to be resurrected at the Chaos Shrine. Do you wish to try?";
                    break;
                case ResurrectMessage.Generic:
                default:
                    Question = "It is possible for you to be resurrected now. Do you wish to try?";
                    break;
            }
        }

        private static void UnfreezeCallback( object state )
        {
            if ( state is Mobile )
                ((Mobile)state).Frozen = false;
        }

        private static readonly TimerStateCallback m_Unfreeze = new TimerStateCallback( UnfreezeCallback );

        public override void OnCancel( NetState state )
        {
            if ( state.Mobile != null && m_Timer != null && m_Timer.Running )
            {
                state.Mobile.Frozen = false;
                m_Timer.Stop();
            }
        }

        public override void OnResponse( NetState state, int index )
        {
            Mobile from = state.Mobile;
            if( from == null )
                return;

            if ( m_Timer != null && m_Timer.Running )
            {
                from.Frozen = false;
                m_Timer.Stop();
            }

            if( from.Map == null || !from.Map.CanFit( from.Location, 16, false, false ) )
            {
                from.SendAsciiMessage( "Thou can not be resurrected there!" ); // Thou can not be resurrected there!
                return;
            }

            from.PlaySound( 0x214 );
            from.FixedEffect( 0x376A, 10, 16 );

            from.Resurrect();

            if( from.Fame > 0 )
            {
                int amount = from.Fame / ( (int)( ( from.Fame / 500.0 ) + 10.0 ) );
                Server.Misc.Titles.AwardFame( from, -amount, true );
            }

            bool isStatLossImmune = false;

            TownSystem t = TownSystem.Find( from );
            if( t != null && t.IsMurdererTown )
                isStatLossImmune = true;

            if( from is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)from ).PermaRed )
                isStatLossImmune = true;

            if( from.ShortTermMurders >= 5 && !isStatLossImmune )
            {
                double loss = ( 100.0 - ( 4.0 + ( from.ShortTermMurders / 5.0 ) ) ) / 100.0; // 5 to 15% loss

                if( loss < 0.85 )
                    loss = 0.85;
                else if( loss > 0.95 )
                    loss = 0.95;

                if( from.RawStr * loss > 10 )
                    from.RawStr = (int)( from.RawStr * loss );
                if( from.RawInt * loss > 10 )
                    from.RawInt = (int)( from.RawInt * loss );
                if( from.RawDex * loss > 10 )
                    from.RawDex = (int)( from.RawDex * loss );

                for( int s = 0; s < from.Skills.Length; s++ )
                {
                    if( from.Skills[ s ].Base * loss > 35 )
                        from.Skills[ s ].Base *= loss;
                }

                using( StreamWriter op = new StreamWriter( "Logs/statloss.log", true ) )
                {
                    op.WriteLine( DateTime.Now );
                    op.WriteLine( "Name {0}, (account {1} serial {2}), longs: {3}, shorts {4}.",
                                  from.Name, from.Account.Username, from.Serial, from.Kills, from.ShortTermMurders );
                    op.WriteLine();
                }
            }

            if( from.Alive )
                from.Hits = 0;
        }
    }
}