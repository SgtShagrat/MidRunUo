/***************************************************************************
 *                               IronMine.cs
 *                            -----------------
 *   begin                : 12 gennaio, 2009
 *   author               :	Faxx	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Faxx - Dies Irae			
 *   revisione            : Dies Irae
 ***************************************************************************/

using Server.Mobiles;

namespace Server.Items
{
    public class SmallIronMine : BaseIronMine
    {
        [Constructable]
        public SmallIronMine()
        {
            Level = 0;
            SpawnsToGo = 2;
            BlockSize = 2;
        }

        #region serialization
        public SmallIronMine( Serial s )
            : base( s )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            reader.ReadInt();
        }
        #endregion
    }

    public class MediumIronMine : BaseIronMine
    {
        [Constructable]
        public MediumIronMine()
        {
            Level = 1;
            SpawnsToGo = 3;
            BlockSize = 3;
        }

        #region serialization
        public MediumIronMine( Serial s )
            : base( s )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            reader.ReadInt();
        }
        #endregion
    }

    public class LargeIronMine : BaseIronMine
    {
        [Constructable]
        public LargeIronMine()
        {
            Level = 2;
            SpawnsToGo = 4;
            BlockSize = 4;
        }

        #region serialization
        public LargeIronMine( Serial s )
            : base( s )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            reader.ReadInt();
        }
        #endregion
    }

    public abstract class BaseIronMine : CaveBoulder
    {
        private static SpawnInfo[] m_SpawnInfos = new SpawnInfo[]
                                                      {
                                                      new SpawnInfo( typeof ( Rat ), 0.05 ),
                                                      new SpawnInfo( typeof ( GiantRat ), 0.03 ),
                                                      new SpawnInfo( typeof ( AntLion ), 0.02 ),
                                                      new SpawnInfo( typeof ( Snake ), 0.02 )
                                                      };

        protected BaseIronMine()
        {
            Composition[ (int)CraftResource.Iron ] = 6;
            Composition[ (int)CraftResource.ShadowIron ] = 3;
        }

        public override SpawnInfo[] SpawnInfos
        {
            get { return m_SpawnInfos; }
        }

        public override object Spawn( Point3D p )
        {
            Mobile m = base.Spawn( p ) as Mobile;

            if( m is AntLion )
            {
                AntLion ant = m as AntLion;
                ant.PackItem( new IronOre( Utility.RandomMinMax( 2, 10 ) ) );
                ant.PackItem( new ShadowIronOre( Utility.RandomMinMax( 1, 5 ) ) );

                if( Utility.Random( 5 ) == 0 )
                {
                    switch( Utility.Random( 5 ) )
                    {
                        case 0:
                            ant.PackItem( new Amber( Utility.RandomMinMax( 1, 3 ) ) );
                            break;
                        case 1:
                            ant.PackItem( new Amethyst( Utility.RandomMinMax( 1, 3 ) ) );
                            break;
                        case 2:
                            ant.PackItem( new Citrine( Utility.RandomMinMax( 1, 3 ) ) );
                            break;
                        case 3:
                            ant.PackItem( new Ruby() );
                            break;
                        case 4:
                            ant.PackItem( new Diamond() );
                            break;
                    }
                }
            }

            return m;
        }

        #region serialization
        public BaseIronMine( Serial s )
            : base( s )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            reader.ReadInt();
        }
        #endregion
    }
}