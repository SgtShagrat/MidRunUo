/***************************************************************************
 *                               BoatGump.cs
 *
 *   begin                : 21 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Midgard.Gumps
{
    public class BoatGump : Gump
    {
        private BaseBoat m_Boat;

        public BoatGump( BaseBoat boat )
            : this( boat, Buttons.Normal )
        {
        }

        public BoatGump( BaseBoat boat, Buttons state )
            : base( 50, 50 )
        {
            m_Boat = boat;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );

            AddBackground( 0, 0, 310, 310, 9300 );

            AddImage( 240, 16, 2529 );
            AddImage( 10, 9, 171 );

            AddButton( 125, 40, 4500, 4500, (int)Buttons.Foward, GumpButtonType.Reply, 0 );
            AddButton( 170, 85, 4502, 4502, (int)Buttons.Right, GumpButtonType.Reply, 0 );
            AddButton( 125, 130, 4504, 4504, (int)Buttons.Backward, GumpButtonType.Reply, 0 );
            AddButton( 80, 85, 4506, 4506, (int)Buttons.Left, GumpButtonType.Reply, 0 );

            AddLabel( 225, 100, 0, @"Move Right" );
            AddLabel( 123, 20, 0, @"Forward" );
            AddLabel( 120, 187, 0, @"Backward" );
            AddLabel( 10, 100, 0, @"Move Left" );

            AddButton( 130, 85, 2529, 2529, (int)Buttons.Stop, GumpButtonType.Reply, 0 );

            AddCheck( 25, 245, 2510, 2511, state == Buttons.Normal, (int)Buttons.Normal );
            AddCheck( 25, 220, 2510, 2511, state == Buttons.Slow, (int)Buttons.Slow );
            AddCheck( 25, 270, 2510, 2511, state == Buttons.Fast, (int)Buttons.Fast );

            AddLabel( 50, 270, 100, @"Fast" );
            AddLabel( 50, 245, 100, @"Normal" );
            AddLabel( 50, 220, 100, @"Slow" );

            AddButton( 255, 250, 4025, 4024, (int)Buttons.DropAnchor, GumpButtonType.Reply, 0 );
            AddLabel( 175, 251, 0, m_Boat.Anchored ? "Raise Anchor" : "Lower Anchor" );
        }

        public enum Buttons
        {
            Foward = 1,
            Right,
            Backward,
            Left,
            Stop,
            Fast,
            DropAnchor,
            Normal,
            Slow,
        }

        private void StartMove( Direction d, bool fast, bool oneStep )
        {
            if( oneStep )
                m_Boat.OneMove( d );
            else
                m_Boat.StartMove( d, fast );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( m_Boat.CanCommand( from ) && m_Boat.Contains( from ) )
            {
                bool fast = info.IsSwitched( (int)Buttons.Fast );
                bool oneStep = info.IsSwitched( (int)Buttons.Slow );

                Buttons switchState = Buttons.Normal;
                if( fast )
                    switchState = Buttons.Fast;
                else if( oneStep )
                    switchState = Buttons.Slow;

                switch( info.ButtonID )
                {
                    case (int)Buttons.Stop:
                        m_Boat.StopMove( true );
                        Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), delegate { sender.Mobile.SendGump( new BoatGump( m_Boat, switchState ) ); } );
                        break;
                    case (int)Buttons.Foward:
                        StartMove( Direction.South, fast, oneStep );
                        Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), delegate { sender.Mobile.SendGump( new BoatGump( m_Boat, switchState ) ); } );
                        break;
                    case (int)Buttons.Right:
                        StartMove( Direction.East, fast, oneStep );
                        Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), delegate { sender.Mobile.SendGump( new BoatGump( m_Boat, switchState ) ); } );
                        break;
                    case (int)Buttons.Backward:
                        StartMove( Direction.North, fast, oneStep );
                        Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), delegate { sender.Mobile.SendGump( new BoatGump( m_Boat, switchState ) ); } );
                        break;
                    case (int)Buttons.Left:
                        StartMove( Direction.West, fast, oneStep );
                        Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), delegate { sender.Mobile.SendGump( new BoatGump( m_Boat, switchState ) ); } );
                        break;
                    case (int)Buttons.Normal:
                        Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), delegate { sender.Mobile.SendGump( new BoatGump( m_Boat, switchState ) ); } );
                        break;
                    case (int)Buttons.Slow:
                        Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), delegate { sender.Mobile.SendGump( new BoatGump( m_Boat, switchState ) ); } );
                        break;
                    case (int)Buttons.Fast:
                        Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), delegate { sender.Mobile.SendGump( new BoatGump( m_Boat, switchState ) ); } );
                        break;
                    case (int)Buttons.DropAnchor:
                        if( m_Boat.Anchored )
                            m_Boat.RaiseAnchor( true );
                        else
                            m_Boat.LowerAnchor( true );
                        Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), delegate { sender.Mobile.SendGump( new BoatGump( m_Boat, switchState ) ); } );
                        break;
                }
            }
        }
    }
}