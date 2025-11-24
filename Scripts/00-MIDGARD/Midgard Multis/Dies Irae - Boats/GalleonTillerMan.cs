/***************************************************************************
 *                               GalleonTillerMan.cs
 *
 *   begin                : 21 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Multis;

namespace Midgard.Multis
{
    public class GalleonTillerMan : TillerMan
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public bool Debug { get; set; }

        public GalleonTillerMan( BaseBoat boat )
            : base( boat )
        {
            ItemID = 0x3b81;
            Boat = boat;
        }

        public override bool CanSpeech
        {
            get { return Debug; }
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Boat != null && Boat.ShipName != null )
                LabelTo( from, Boat.ShipName );
            else
                base.OnSingleClick( from );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( Boat != null && Boat.Contains( from ) )
                Boat.BeginCommand( from );
            else if( Boat != null )
                Boat.BeginDryDock( from );
        }

        #region serialization
        public GalleonTillerMan( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); //version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}