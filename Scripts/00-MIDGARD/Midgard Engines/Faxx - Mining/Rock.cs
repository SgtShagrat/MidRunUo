/***************************************************************************
 *                               Rock.cs
 *                            -------------
 *   begin                : 12 gennaio, 2009
 *   author               :	Faxx	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Faxx - Dies Irae			
 *   revisione            : Dies Irae
 ***************************************************************************/

using System;

namespace Server.Items
{
    public class Rock : Item
    {
        private CraftResource m_OreType;

        [Constructable]
        public Rock()
            : this( Utility.RandomMinMax( 0, 10 ) )
        {
        }

        [Constructable]
        public Rock( int type )
            : base( 0 )
        {
            Movable = true;
            Stackable = true;

            RockType = type;
        }

        /// <summary>
        /// Rock shape:
        /// 0-10 : rock (ItemID from 0x1363)
        /// 11-21: stalagmite (ItemID from 0x8E0)
        /// The rock shape affects the amount and weight
        /// <summary>
        public int RockType
        {
            get { return ItemID - 0x1363; }
            set
            {
                if( value < 0 )
                    value = 0;

                if( value <= 10 )
                    ItemID = 0x1363 + value; // normal rocks
                else if( value <= 21 )
                    ItemID = 0x8E0 + value - 11; // stalagmites

                Amount = TypeAmount( value );
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
        /// Ore contained in this rock
        /// <summary>
        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource OreType
        {
            get { return m_OreType; }
            set
            {
                m_OreType = value;
                Hue = CraftResources.GetHue( m_OreType );
                //Weight = 20.0*CraftResources.GetWeight(OreType);
            }
        }

        /// <summary>
        /// Amount for each rock shape
        /// <summary>
        public int TypeAmount( int type )
        {
            int a;
            switch( type )
            {
                case 0:
                    {
                        a = Utility.RandomMinMax( 15, 20 );
                        break;
                    }
                case 1:
                    {
                        a = Utility.RandomMinMax( 10, 15 );
                        break;
                    }
                case 2:
                    {
                        a = Utility.RandomMinMax( 10, 15 );
                        break;
                    }
                case 3:
                    {
                        a = Utility.RandomMinMax( 10, 15 );
                        break;
                    }
                case 4:
                    {
                        a = Utility.RandomMinMax( 15, 20 );
                        break;
                    }
                case 5:
                    {
                        a = Utility.RandomMinMax( 8, 12 );
                        break;
                    }
                case 6:
                    {
                        a = Utility.RandomMinMax( 8, 12 );
                        break;
                    }
                case 7:
                    {
                        a = Utility.RandomMinMax( 15, 20 );
                        break;
                    }
                case 8:
                    {
                        a = Utility.RandomMinMax( 7, 10 );
                        break;
                    }
                case 9:
                    {
                        a = Utility.RandomMinMax( 7, 10 );
                        break;
                    }
                case 10:
                    {
                        a = Utility.RandomMinMax( 15, 20 );
                        break;
                    }
                default:
                    {
                        a = Utility.RandomMinMax( 9000, 11000 );
                        break;
                    } // stalagmites are very though to destroy so that cleaning mines is very difficult
            }

            return a;
        }

        /// <summary>
        /// Deleting triggers rock breakdown:
        /// If the rock is deleted as a result of the amount going to 0 then it breaks
        /// <summary>
        public override void Delete()
        {
            if( Amount <= 0 ) // this means the item is being consumed so it must break and not be deleted 
                Break();
            else
                base.Delete(); // this means we are manually deleting the item
        }

        /// <summary>
        /// Break down the rock.
        /// When a rock breaks it turns into ores of the corresponding type
        /// <summary>
        public virtual void Break()
        {
            if( OreType != CraftResource.None )
            {
                Type type = CraftResources.GetInfo( OreType ).ResourceTypes[ 1 ];
                Item i = Activator.CreateInstance( type ) as Item;
                if( i != null )
                {
                    i.Amount = TypeAmount( RockType );
                    i.MoveToWorld( Location, Map );
                }
            }
            base.Delete();
        }

        #region serialization
        public Rock( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}