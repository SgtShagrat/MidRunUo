/***************************************************************************
 *                               Galleon.cs
 *
 *   begin                : 21 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Movement;
using Server.Multis;

namespace Midgard.Multis
{
    public class Galleon : BaseBoat
    {
        public override int HitsMax { get { return 100000; } }
        public override int ResistFire { get { return 30; } }
        public override int ResistPhysical { get { return 70; } }

        public override int BoatPrice { get { return 1000; } }
        public override string Description { get { return ""; } }
        public override int HoldSize { get { return 400; } }

        public override int MinSailors
        {
            get { return 3; }
        }

        public override bool CanTurn
        {
            get { return false; }
        }

        [Constructable]
        public Galleon()
        {
            TillerMan = new GalleonTillerMan( this );
            TillerMan.MoveToWorld( new Point3D( X, Y - TillerManDistance, 45 ), Map );

            if( PPlank != null )
                PPlank.Delete();
            if( SPlank != null )
                SPlank.Delete();

            PPlank = new GalleonPlank( this, PlankSide.Port, 0 );
            SPlank = new GalleonPlank( this, PlankSide.Starboard, 0 );

            PPlank.MoveToWorld( new Point3D( X + PortOffset.X, Y + PortOffset.Y, Z + 26 ), Map );
            SPlank.MoveToWorld( new Point3D( X + StarboardOffset.X, Y + StarboardOffset.Y, Z + 25 ), Map );

            AddFixture( 0x4AC, 0, 0, 5, 0 ); // floor

            Hold.Location.Offset( 0, 0, 1 );
        }

        public Galleon( Serial serial )
            : base( serial )
        {
        }

        public override int NorthID
        {
            get { return 0x5392; }
        }

        public override int EastID
        {
            get { return 0x5392; }
        }

        public override int SouthID
        {
            get { return 0x5392; }
        }

        public override int WestID
        {
            get { return 0x5392; }
        }

        public override int HoldDistance
        {
            get { return 14; }
        }

        public override int TillerManDistance
        {
            get { return 15; }
        }

        public override Point2D StarboardOffset
        {
            get { return new Point2D( 4, 0 ); }
        }

        public override Point2D PortOffset
        {
            get { return new Point2D( -6, 2 ); }
        }

        public override Point3D MarkOffset
        {
            get { return new Point3D( 0, 1, 3 ); }
        }

        public override BaseDockedBoat DockedBoat
        {
            get { return new SmallDockedBoat( this ); }
        }

        public override void UpdateComponents()
        {
            if( PPlank != null )
            {
                PPlank.MoveToWorld( new Point3D( X + PortOffset.X, Y + PortOffset.Y, Z + 25 ), Map );
            }

            if( SPlank != null )
            {
                SPlank.MoveToWorld( new Point3D( X + StarboardOffset.X, Y + StarboardOffset.Y, Z + 25 ), Map );
            }

            int xOffset = 0, yOffset = 0;
            Movement.Offset( Facing, ref xOffset, ref yOffset );

            if( TillerMan != null )
            {
                TillerMan.Location = new Point3D( X + ( xOffset * TillerManDistance ) /*+ ( Facing == Direction.North ? 1 : 0 )*/, Y + ( yOffset * TillerManDistance ), TillerMan.Z );
                TillerMan.InvalidateProperties();
            }

            if( Hold != null )
            {
                Hold.Location = new Point3D( X + ( xOffset * HoldDistance ), Y + ( yOffset * HoldDistance ), Hold.Z );
                Hold.SetFacing( Facing );
            }

            if( PPlank != null && PPlank is GalleonPlank )
            {
                if( PPlank.IsOpen )
                {
                    ( (GalleonPlank)PPlank ).OffsetPlank( xOffset, yOffset );
                }
            }

            if( SPlank != null && SPlank is GalleonPlank )
            {
                if( SPlank.IsOpen )
                {
                    ( (GalleonPlank)SPlank ).OffsetPlank( xOffset, yOffset );
                }
            }

            UpdateFixtures( xOffset, yOffset );
        }

        [CommandProperty( AccessLevel.Administrator )]
        public bool CanBeDeleted { get; set; }

        public override void Delete()
        {
            if( !CanBeDeleted )
                return;
            else
                base.Delete();
        }

        public override bool CheckDecay()
        {
            return false; // galleons does not decay
        }

        public override int Status
        {
            get
            {
                return 1043010; // This structure is like new.
            }
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

    public class GalleonDockedBoat : BaseDockedBoat
    {
        public GalleonDockedBoat( BaseBoat boat )
            : base( 5010, Point3D.Zero, boat )
        {
            ItemID = 0x14F3;
        }

        public GalleonDockedBoat( Serial serial )
            : base( serial )
        {
        }

        public override BaseBoat Boat
        {
            get { return new Galleon(); }
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

    public class GalleonDeed : BaseBoatDeed
    {
        [Constructable]
        public GalleonDeed()
            : base( 5010, Point3D.Zero )
        {
        }

        public GalleonDeed( Serial serial )
            : base( serial )
        {
        }

        public override string DefaultName
        {
            get { return "a galleon deed"; }
        }

        public override BaseBoat Boat
        {
            get { return new Galleon(); }
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