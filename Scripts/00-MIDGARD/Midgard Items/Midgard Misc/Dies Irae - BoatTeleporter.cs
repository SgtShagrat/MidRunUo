/***************************************************************************
 *                               Dies Irae - BoatTeleporter.cs
 *
 *   begin                : 27 dicembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Multis;

namespace Midgard.Items
{
    public class BoatTeleporter : Teleporter
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public string Word { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Radius { get; set; }

        public override string DefaultName
        {
            get { return "Midgard Boat Teleporter"; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech( SpeechEventArgs e )
        {
            Mobile from = e.Mobile;

            if( !e.Handled && from.InRange( this, Radius ) && e.Speech.ToLower() == Word )
            {
                BaseBoat boat = BaseBoat.FindBoatAt( from, from.Map );

                if( boat == null )
                    return;

                if( !Active )
                {
                    if( boat.TillerMan != null )
                        boat.TillerMan.Say( 502507 ); // Ar, Legend has it that these pillars are inactive! No man knows how it might be undone!

                    return;
                }

                Map map = from.Map;

                for( int i = 0; i < 10; i++ ) // Try 5 times
                {
                    int x = Utility.RandomMinMax( PointDest.X - 5, PointDest.X + 5 );
                    int y = Utility.RandomMinMax( PointDest.Y - 5, PointDest.Y + 5 );
                    int z = boat.Z;

                    Point3D dest = new Point3D( x, y, z );

                    if( boat.CanFit( dest, map, boat.ItemID ) )
                    {
                        int xOffset = x - boat.X;
                        int yOffset = y - boat.Y;

                        boat.Teleport( xOffset, yOffset, 0 );

                        return;
                    }
                }

                if( boat.TillerMan != null )
                    boat.TillerMan.Say( 502508 ); // Ar, I refuse to take that matey through here!
            }
        }


        [Constructable]
        public BoatTeleporter()
        {
            Radius = 3;
            Word = "cross";
        }

        #region serialization
        public BoatTeleporter( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            writer.WriteEncodedInt( Radius );
            writer.Write( Word );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            Radius = reader.ReadEncodedInt();
            Word = reader.ReadString();
        }
        #endregion
    }
}