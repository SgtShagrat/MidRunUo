/***************************************************************************
 *                                  DoubleClickTeleporter.cs
 *                            		-------------------
 *  begin                	: Luglio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Teleperter che fa passare solo al doppioclick.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
    public class DoubleClickTeleporter : Teleporter
    {
        private int m_Range;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Range
        {
            get { return m_Range; }
            set { m_Range = value; InvalidateProperties(); }
        }

        public override bool OnMoveOver( Mobile m )
        {
            return true;
        }

        public override void GetProperties( ObjectPropertyList list )
        {
        }

        private void EndMessageLock( object state )
        {
            ( (Mobile)state ).EndAction( this );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( Active )
            {
                if( !Creatures && !from.Player )
                    return;

                if( !from.InRange( Location, m_Range ) )
                {
                    if( from.BeginAction( this ) )
                    {
                        from.SendMessage( "You are too far away from it." );

                        Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), from );
                    }

                    return;
                }

                StartTeleport( from );
            }
        }

        [Constructable]
        public DoubleClickTeleporter()
        {
            Visible = true;
        }

        #region serialization
        public DoubleClickTeleporter( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_Range );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_Range = reader.ReadInt();

                        break;
                    }
            }
        }
        #endregion
    }
}