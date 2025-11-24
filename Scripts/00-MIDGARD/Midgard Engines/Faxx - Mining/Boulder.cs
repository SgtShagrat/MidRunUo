/***************************************************************************
 *                               Boulder.cs
 *                            ---------------
 *   begin                : 12 gennaio, 2009
 *   author               :	Faxx	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Faxx - Dies Irae			
 *   revisione            : Dies Irae
 ***************************************************************************/

using System.Collections.Generic;

namespace Server.Items
{
    public abstract class Boulder : MultiItem
    {
        private Dictionary<int, int> m_Composition;
        private int m_Type;

        protected Boulder()
            : this( Utility.RandomMinMax( 0, 7 ) )
        {
        }

        protected Boulder( int type )
            : base( 0 )
        {
            Stackable = true;
            Movable = false;

            //Amount = Utility.RandomMinMax(1000,2000);

            m_Type = -1;
            BoulderType = type;
            Composition = new Dictionary<int, int>();
        }

        public Dictionary<int, int> Composition
        {
            get { return m_Composition; }
            set
            {
                if( m_Composition == null )
                    m_Composition = new Dictionary<int, int>();

                m_Composition = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Broken
        {
            get { return false; }
            set
            {
                if( value )
                    Break();
            }
        }

        /// <summary>
        /// Boulder type.
        /// In standard UO muls there are 8 boulders
        /// </summary>
        [CommandProperty( AccessLevel.GameMaster )]
        public int BoulderType
        {
            get { return m_Type; }
            set
            {
                int type = value;
                if( type < 0 )
                    type = 0;
                if( type > 7 )
                    type = 7;

                RemoveItems();
                int a = 0;
                switch( type ) //create sub items according to type
                {
                    case 0:
                        {
                            ItemID = 0x1350;
                            AddSubItem( 0x134F, -1, 0 );
                            AddSubItem( 0x1351, 0, -1 );
                            a = Utility.RandomMinMax( 500, 1000 );
                            break;
                        }
                    case 1:
                        {
                            ItemID = 0x1353;
                            AddSubItem( 0x1352, -1, 0 );
                            AddSubItem( 0x1354, 0, -1 );
                            a = Utility.RandomMinMax( 450, 900 );
                            break;
                        }
                    case 2:
                        {
                            ItemID = 0x1358;
                            AddSubItem( 0x1357, -1, 0 );
                            AddSubItem( 0x1359, 0, -1 );
                            a = Utility.RandomMinMax( 400, 800 );
                            break;
                        }
                    case 3:
                        {
                            ItemID = 0x135D;
                            AddSubItem( 0x135C, -1, 0 );
                            AddSubItem( 0x135E, 0, -1 );
                            a = Utility.RandomMinMax( 350, 700 );
                            break;
                        }
                    case 4:
                        {
                            ItemID = 0x1355;
                            AddSubItem( 0x1356, 0, -1 );
                            a = Utility.RandomMinMax( 300, 600 );
                            break;
                        }
                    case 5:
                        {
                            ItemID = 0x135A;
                            AddSubItem( 0x135B, 0, -1 );
                            a = Utility.RandomMinMax( 400, 800 );
                            break;
                        }
                    case 6:
                        {
                            ItemID = 0x135F;
                            AddSubItem( 0x1360, 1, 0 );
                            a = Utility.RandomMinMax( 250, 500 );
                            break;
                        }
                    case 7:
                        {
                            ItemID = 0x1361;
                            AddSubItem( 0x1362, 1, 0 );
                            a = Utility.RandomMinMax( 200, 400 );
                            break;
                        }
                }
                //Console.WriteLine("changing amount to: " + a + "  stackable: " + Stackable + "   type: " + type);
                Amount = a;
                m_Type = value;
            }
        }

        public virtual void Break()
        {
            bool big = ( BoulderType >= 0 && BoulderType <= 3 );

            if( Composition.Count == 0 ) // no composition, spawn some rocks
            {
                for( int idx = 0; idx < Utility.RandomMinMax( 1, 4 ); idx++ )
                {
                    Rock r = new Rock();

                    // r.Amount = Utility.RandomMinMax( 10, 15 );
                    r.OreType = CraftResource.None;
                    r.Location = Location;
                    r.X = r.X + Utility.RandomMinMax( -1, 1 );
                    r.Y = r.Y + Utility.RandomMinMax( -1, 1 );
                    r.Map = Map;
                }
            }
            else
            {
                List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();

                foreach( KeyValuePair<int, int> de in Composition ) // spawn rocks according to composition
                {
                    int rocks = de.Value;
                    if( big )
                        rocks = rocks / 2;

                    for( int idx = 0; idx < rocks; idx++ )
                    {
                        Rock r = new Rock();
                        // r.Amount = Utility.RandomMinMax( 10, 15 );
                        r.OreType = (CraftResource)de.Key;
                        r.Location = Location;
                        r.X = r.X + Utility.RandomMinMax( -1, 1 );
                        r.Y = r.Y + Utility.RandomMinMax( -1, 1 );
                        r.Map = Map;
                    }

                    list.Add( de );
                }

                if( !big )
                {
                    for( int i = 0; i < list.Count; i++ )
                    {
                        KeyValuePair<int, int> de = list[ i ];
                        int rocks = de.Value;

                        Composition[ de.Key ] = rocks - rocks / 2;
                    }
                }
            }

            //big boulders spawn a small boulder, small bouders disappear
            if( big )
                BoulderType = Utility.RandomMinMax( 4, 7 );
            else
                base.Delete(); //actually delete
        }

        public override void Delete()
        {
            if( Amount <= 0 ) // this means the item is being consumed so it must break and not be deleted 
                Break();
            else
                base.Delete(); // this means we are manually deleting the item
        }

        #region serialization
        public Boulder( Serial serial )
            : base( serial )
        {
            Composition = new Dictionary<int, int>();
            Amount = 1;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_Type );

            writer.Write( Composition.Count );
            foreach( KeyValuePair<int, int> de in Composition )
            {
                writer.Write( de.Key );
                writer.Write( de.Value );
            }
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Type = reader.ReadInt();

            int n = reader.ReadInt();

            for( int i = 0; i < n; i++ )
            {
                int res = reader.ReadInt();
                int p = reader.ReadInt();

                Composition.Add( res, p );
            }
        }
        #endregion
    }
}