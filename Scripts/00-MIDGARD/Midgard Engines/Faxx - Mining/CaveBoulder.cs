/***************************************************************************
 *                               CaveBoulder.cs
 *                            --------------------
 *   begin                : 12 gennaio, 2009
 *   author               :	Faxx	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Faxx - Dies Irae			
 *   revisione            : Dies Irae
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Server.Items
{
    public abstract class CaveBoulder : Boulder
    {
        public static readonly int LevelDepth = 30;

        /// <summary>
        /// Number of spawns left
        /// <summary>
        private int m_BlockSize;

        /// <summary>
        /// Number of spawns left
        /// <summary>
        private int m_Level;

        /// <summary>
        /// Number of spawns left
        /// <summary>
        private int m_SpawnsToGo;

        protected CaveBoulder( int mineSize )
        {
            m_SpawnsToGo = mineSize;
            m_BlockSize = 2;
        }

        protected CaveBoulder()
            : this( 1 )
        {
        }

        /// <summary>
        /// List of possible spawns
        /// <summary>
        public virtual SpawnInfo[] SpawnInfos
        {
            get { return null; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int SpawnsToGo
        {
            get { return m_SpawnsToGo; }
            set { m_SpawnsToGo = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int BlockSize
        {
            get { return m_BlockSize; }
            set { m_BlockSize = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        /// <summary>
        /// Helper property: [set expand true will break the boulder
        /// <summary>
        [CommandProperty( AccessLevel.GameMaster )]
        public bool Expand
        {
            get { return false; }
            set
            {
                if( value )
                    Break();
            }
        }

        /// <summary>
        /// Custom break method.
        /// Boulder pieces are rocks containing ores
        /// The boulder expands itself into a cave region 5x5
        /// <summary>
        public override void Break()
        {
            //seek empty tiles 2 tiles around the boulder
            List<Point3D> locations = new List<Point3D>();
            for( int x = X - m_BlockSize; x <= X + m_BlockSize; x++ )
            {
                for( int y = Y - m_BlockSize; y <= Y + m_BlockSize; y++ )
                {
                    bool isempty = true;
                    //check if we are traversing a mountain wall or if we are out of range
                    if( Map.Tiles.GetLandTile( x, y ).Z < Z + 20 || Math.Sqrt( ( x - X ) * ( x - X ) + ( y - Y ) * ( y - Y ) ) > m_BlockSize + 0.5 )
                        isempty = false;
                    else
                    {
                        IPooledEnumerable e = Map.GetItemsInRange( new Point3D( x, y, Z ), 0 );
                        foreach( Item i in e )
                        {
                            if( !( i is InternalItem || i is CaveBoulder ) && i.Z == Z )
                            {
                                isempty = false;
                                break;
                            }
                        }
                    }

                    if( isempty )
                        locations.Add( new Point3D( x, y, Z ) );
                }
            }
            //add cave floor
            foreach( Point3D p in locations )
            {
                Item i = new CaveFloorCenter();
                i.Location = p;
                i.Map = Map;
                i.Hue = GetDepthHue( i.Z );

                double d = Math.Sqrt( ( p.X - X ) * ( p.X - X ) + ( p.Y - Y ) * ( p.Y - Y ) );
                if( d > m_BlockSize - 0.5 )
                {
                    //put some stalagmites along the boundaries
                    if( Utility.RandomDouble() > 1.0 / m_BlockSize || m_SpawnsToGo == 0 )
                    {
                        // add stalagmites on boundary
                        Rock r = new Rock();
                        r.OreType = CraftResource.None;
                        r.RockType = Utility.RandomMinMax( 11, 21 );
                        r.Movable = false;
                        r.Stackable = true;
                        i = r;
                    }
                    else
                    {
                        // fill up half of the the gaps with new boulders
                        CaveBoulder cb = Activator.CreateInstance( GetType() ) as CaveBoulder;
                        i = cb;
                        if( cb != null )
                        {
                            Dictionary<int, int> cloned = new Dictionary<int, int>( Composition );

                            cb.Composition = cloned;
                            cb.m_SpawnsToGo = m_SpawnsToGo - 1;
                        }
                    }
                }
                // internal tiles
                else if( m_BlockSize >= 3 && Utility.RandomDouble() < 0.05 )
                {
                    CaveBoulder cb = Activator.CreateInstance( GetType() ) as CaveBoulder;
                    i = cb;
                    if( cb != null )
                    {
                        Dictionary<int, int> cloned = new Dictionary<int, int>( Composition );

                        cb.Composition = cloned;
                        cb.m_SpawnsToGo = 0;
                    }
                }
                else
                    Spawn( p ); // spawn animals in the internal tiles

                if( i != null )
                {
                    i.Location = p;
                    i.Map = Map;
                }
            }

            Point3D loc = Location;
            base.Break();

            // spawn a cave passage on last generation if needed
            if( m_Level > 0 && m_SpawnsToGo == 0 && Utility.RandomDouble() < 0.1 )
            {
                CavePassage p = new CavePassage();
                p.MoveToWorld( loc, Map );
            }
        }

        public static int GetDepthHue( int z )
        {
            if( z >= -LevelDepth )
                return 0;
            if( z >= -2 * LevelDepth )
                return 0x289;
            if( z >= -3 * LevelDepth )
                return 0x2ED;
            return 0x2EB;
        }

        /// <summary>
        /// Spawn a NPC according to SpawnInfos data
        /// <summary>
        public virtual object Spawn( Point3D p )
        {
            if( SpawnInfos == null )
                return null;

            double x = Utility.RandomDouble();
            double acc = 0.0;
            foreach( SpawnInfo si in SpawnInfos )
            {
                acc += si.Chance;
                if( x < acc )
                {
                    object o = Activator.CreateInstance( si.Type );
                    if( o is Mobile )
                        ( (Mobile)o ).MoveToWorld( p, Map );
                    else
                        ( (Item)o ).MoveToWorld( p, Map );
                    return o;
                }
            }

            return null;
        }

        #region serialization
        public CaveBoulder( Serial s )
            : base( s )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( m_SpawnsToGo );
            writer.Write( m_BlockSize );
            writer.Write( m_Level );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int v = reader.ReadInt();

            switch( v )
            {
                case 0:
                    m_SpawnsToGo = reader.ReadInt();
                    m_BlockSize = reader.ReadInt();
                    m_Level = reader.ReadInt();
                    break;
            }
        }
        #endregion

        #region Nested type: SpawnInfo
        /// <summary>
        /// Data class defining a cave NPC spawn
        /// <summary>
        public class SpawnInfo
        {
            public double Chance; // spawn probability
            public Type Type; // NPC to be spawned

            public SpawnInfo( Type t, double c )
            {
                Type = t;
                Chance = c;
            }
        }
        #endregion
    }
}