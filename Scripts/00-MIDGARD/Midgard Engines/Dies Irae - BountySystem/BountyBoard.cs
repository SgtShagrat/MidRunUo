/***************************************************************************
 *                               BountyBoard.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;

namespace Midgard.Engines.BountySystem
{
    [Flipable( 0x1E5E, 0x1E5F )]
    public class BountyBoard : Item
    {
        public override int LabelNumber
        {
            get { return ( Core.Entries.Count > 0 ? 1042679 : 1042680 ); }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( Core.Entries.Count > 0 )
            {
                string text = String.Format( "A bounty board with {0} posted bounties", Core.Entries.Count );
                list.Add( text );
            }
            else
                list.Add( 1042679 );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Config.Enabled )
            {
                from.SendMessage( "This board has been temporary disabled." );
                return;
            }

            from.SendGump( new BountyBoardGump( from, this ) );
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            if( Core.Entries.Count > 0 )
                LabelTo( from, "A bounty board with {0} posted bounties", Core.Entries.Count );
            else
                LabelTo( from, 1042679 );
        }

        public static List<BountyBoard> Instances { get; private set; }

        [Constructable]
        public BountyBoard()
            : base( 0x1E5E )
        {
            Instances.Add( this );
            Name = "Bounty Board";
            Movable = false;
        }

        public override void OnDelete()
        {
            Instances.Remove( this );

            base.OnDelete();
        }

        static BountyBoard()
        {
            Instances = new List<BountyBoard>();
        }

        #region serialization
        public BountyBoard( Serial serial )
            : base( serial )
        {
            Instances.Add( this );
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
    }
}