/***************************************************************************
 *                               GrapplingHook.cs
 *
 *   begin                : 21 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;

namespace Midgard.Multis
{
    public class GrapplingHook : Item
    {
        [Constructable]
        public GrapplingHook()
            : base( 0x14F8 )
        {
        }

        public override string DefaultName
        {
            get { return "a grappling hook"; }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( Deleted )
                return;

            if( !IsChildOf( from.Backpack ) )
            {
                from.SendMessage( "That must be in your backpack to use it." );
                return;
            }

            BaseBoat bt = BaseBoat.FindBoatAt( from );
            if( bt == null || !bt.IsOnDeck( from ) )
            {
                from.SendMessage( "You must be on a boat to use this." );
                return;
            }

            from.SendMessage( "Target the boat you wish to grapple." );
            from.Target = new InternalTarget( this );
        }

        #region serialization
        public GrapplingHook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        private class InternalTarget : Target
        {
            private GrapplingHook m_Hook;

            public InternalTarget( GrapplingHook g )
                : base( 10, false, TargetFlags.None )
            {
                m_Hook = g;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                BaseBoat bt = null;
                Point3D loc = new Point3D();

                //Check we have a hook
                if( m_Hook == null )
                    return;

                //first do all our validation *again* to thwart sneaky players!
                //Has to be in backpack and on a boat
                if( !m_Hook.IsChildOf( from.Backpack ) )
                {
                    from.SendMessage( "That must be in your backpack to use it." );
                    return;
                }

                bt = BaseBoat.FindBoatAt( from );
                if( bt == null || !bt.IsOnDeck( from ) )
                {
                    from.SendMessage( "You must be on a boat to use this." );
                    return;
                }

                bt = null;

                //Disallow the targetting of movable Items
                if( targeted is Item )
                {
                    Item i = (Item)targeted;
                    if( i.Movable )
                    {
                        from.SendMessage( "You must grapple onto something more sturdy!" );
                        return;
                    }

                    //this item is immovable, see if there's a boat at its location
                    bt = BaseBoat.FindBoatAt( i );
                    loc = i.Location;
                }
                else if( targeted is StaticTarget )
                {
                    StaticTarget st = (StaticTarget)targeted;

                    // Boats seem to have both TileFlag.Surface and TileFlag.Impassable flags on different parts of the deck
                    if( ( st.Flags & TileFlag.Wet ) == 0 )
                    {
                        bt = BaseBoat.FindBoatAt( st.Location, from.Map );
                        loc = st.Location;
                    }
                }

                if( bt != null )
                {
                    if( bt.IsOnDeck( from ) )
                    {
                        from.SendMessage( "You can't grapple the boat you are on!" );
                        return;
                    }

                    if( bt.IsMoving )
                    {
                        Effects.PlaySound( from.Location, from.Map, 0x2B2 );
                        from.SendMessage( "You fail to get a good hook into the boat." );
                        return;
                    }

                    //If there any players who are alive then the hook gets pusehd back in ye olde water..
                    foreach( Mobile m in bt.GetMobilesOnDeck() )
                    {
                        if( m is PlayerMobile && m.Alive && m.AccessLevel <= AccessLevel.Player )
                        {
                            from.SendMessage( "You grapple the boat, but the hook is tossed back into the water." );
                            Effects.PlaySound( from.Location, from.Map, 36 + Utility.Random( 3 ) + 1 );
                            return;
                        }
                    }

                    from.SendMessage( "You grapple the boat!" );
                    Effects.PlaySound( from.Location, from.Map, 0x2B2 );
                    Effects.PlaySound( from.Location, from.Map, 0x523 );
                    m_Hook.Consume();
                    InternalTimer t = new InternalTimer( from, bt );
                    t.Start();
                }
                else
                    from.SendMessage( "That's not a boat!" );
            }

            #region Nested type: InternalTimer
            private class InternalTimer : Timer
            {
                private BaseBoat m_Boat;
                private Mobile m_Mobile;

                public InternalTimer( Mobile m, BaseBoat boat )
                    : base( TimeSpan.FromSeconds( 2.0 ) )
                {
                    m_Mobile = m;
                    m_Boat = boat;
                }

                protected override void OnTick()
                {
                    if( m_Mobile == null || m_Boat == null || m_Boat.Map == Map.Internal )
                        return;

                    Point3D loc = m_Boat.FindSpawnLocationOnDeck();

                    if( loc == new Point3D() )
                    {
                        m_Mobile.SendMessage( "You swing across to the boat, but are unable to get a good footing upon the deck!" );
                        return;
                    }

                    m_Mobile.MoveToWorld( loc, m_Boat.Map );
                }
            }
            #endregion
        }
    }
}