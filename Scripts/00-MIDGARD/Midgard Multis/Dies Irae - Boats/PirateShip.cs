/***************************************************************************
 *                               PirateShip.cs
 *
 *   begin                : 26 agosto 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Multis;

namespace Midgard.Multis
{
    public class PirateShipBoat : LargeDragonBoat
    {
        [Constructable]
        public PirateShipBoat()
        {
            Name = "A Pirate Ship";
        }

        public PirateShipBoat( Serial serial )
            : base( serial )
        {
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }
    }

    public class PirateShipBoatDeed : BaseBoatDeed
    {
        [Constructable]
        public PirateShipBoatDeed()
            : base( 0x4014, new Point3D( 0, -1, 0 ) )
        {
            Name = "A Pirate Ship";
            Hue = 1157;
        }

        public PirateShipBoatDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041210; }
        }

        public override BaseBoat Boat
        {
            get { return new PirateShipBoat(); }
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }
    }

    public class PirateShipBoatDocked : BaseDockedBoat
    {
        public PirateShipBoatDocked( BaseBoat boat )
            : base( 0x4014, new Point3D( 0, -1, 0 ), boat )
        {
        }

        public PirateShipBoatDocked( Serial serial )
            : base( serial )
        {
        }

        public override BaseBoat Boat
        {
            get { return new LargeDragonBoat(); }
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }
    }
}