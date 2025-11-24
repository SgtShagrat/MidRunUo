using System;
using System.Collections.Generic;
using Server.Engines.Craft;

namespace Server.Items
{
    public class HueListGenerator : Item
    {
        [Constructable]
        public HueListGenerator()
            : base( 0xFAB )
        {
            Hue = 0x0;
            Name = "Hue List Generator";
            LootType = LootType.Blessed;
            Visible = false;
            Movable = false;
        }

        public override void OnDoubleClick( Mobile m )
        {
            try
            {
                if( m.Map != null )
                {
                    for( int x = 0; x <= 99; ++x )
                    {
                        for( int y = 0; y <= 29; ++y )
                        {
                            SpecialHueListItem item = new SpecialHueListItem( ( ( 100 * y ) + x ), ItemID );
                            item.Name = "Hue = " + Convert.ToString( item.Hue );
                            item.Movable = false;
                            item.MoveToWorld( new Point3D( ( m.X + x ), ( m.Y + y ), m.Z ), m.Map );
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        public HueListGenerator( Serial serial )
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

        private class SpecialHueListItem : Item
        {
            public SpecialHueListItem( int hue, int itemId )
                : base( itemId )
            {
                Weight = 0.0;
                ItemID = itemId;
                Hue = hue;
                Name = "Special Hue List Item " + Convert.ToString( Hue );
            }

            public SpecialHueListItem( Serial serial )
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
        }
    }

    public class CraftHueGenerator : HueListGenerator
    {
        [Constructable]
        public CraftHueGenerator()
        {
            Hue = 0x0;
            Name = "Craft Hue List Generator";
            Visible = false;
            Movable = false;
        }

        public void DoMatrix( Point3D start )
        {
            List<CraftSystem> systems = new List<CraftSystem>();
            systems.Add( DefBlacksmithy.CraftSystem );
            systems.Add( DefCarpentry.CraftSystem );
            systems.Add( DefTailoring.CraftSystem );

            int row = 0;

            foreach( CraftSystem system in systems )
            {
                List<Item> toPlace = new List<Item>();

                for( int i = 0; i < system.CraftSubRes.Count; i++ )
                {
                    Type t = system.CraftSubRes.GetAt( i ).ItemType;
                    toPlace.Add( Loot.Construct( t ) );
                }

                int column = 0;
                int index = 0;

                while( index < toPlace.Count )
                {
                    if( column == 10 )
                    {
                        row++;
                        column = 0;
                    }

                    Place( toPlace[ index ], new Point3D( ( start.X + column ), ( start.Y + row ), start.Z ) );

                    index++;
                    column++;
                }

                row = row + 2; // space between rows
            }
        }

        private static void Place( Item item, Point3D point )
        {
            item.Movable = false;
            item.Amount = 2;
            item.MoveToWorld( point, Map.Felucca );
        }

        public override void OnDoubleClick( Mobile m )
        {
            try
            {
                DoMatrix( m.Location );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        public CraftHueGenerator( Serial serial )
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
    }
}