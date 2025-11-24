using System;

using Midgard.Items;

using Server;
using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public sealed class ScoutSystem : ClassSystem
    {
        public ScoutSystem()
        {
            Definition = new ClassDefinition( "Scout",
                                                MidgardClasses.Scout,
                                                0,
                                                DefaultWelcomeMessage,
                                                new PowerDefinition[] { }
            );
        }

        public override void MakeRitual( Mobile ritualist, PowerDefinition definition )
        {
            // Scouts have no ritual until now...
        }

        public static RitualItem RandomScoutRitualItem()
        {
            return null;
        }

        private static string[] m_ForestRegionNames = new string[]
        {
            "Forest Near Minoc", "Forest Near Cove", "Forest Near Vesper", "Forest Near Yew", "Forest Near Britain", "Forest Near Trinsic",
            "Forest Near Skara Brae", "Forest South of Yew", "Forest Near Minoc", "Trinsic Jungles"
        };

        public static bool IsInForest( Point3D p, Map map )
        {
            Region r = Region.Find( p, map );
            if( r == null )
                return false;

            string toCheck = r.Name;

            return toCheck != null && Array.LastIndexOf( m_ForestRegionNames, toCheck ) > -1;
        }

        public static bool IsInForest( Mobile m )
        {
            if( m == null || m.Deleted )
                return false;

            if( IsInForest( m.Location, m.Map ) )
            {
                m.SendMessage( "Thou are in a famous forest of Britannian: {0}", m.Region.Name );
                return true;
            }
            else if( CheckTreesInRange( m.Map, 2, 3, m.Location ) )
            {
                m.SendMessage( "Thou are near trees." );
                return true;
            }

            return false;
        }

        #region m_TreeTiles
        private static int[] m_TreeTiles = new int[]
        {
            0x4CCA, 0x4CCB, 0x4CCC, 0x4CCD, 0x4CD0, 0x4CD3, 0x4CD6, 0x4CD8,
            0x4CDA, 0x4CDD, 0x4CE0, 0x4CE3, 0x4CE6, 0x4CF8, 0x4CFB, 0x4CFE,
            0x4D01, 0x4D41, 0x4D42, 0x4D43, 0x4D44, 0x4D57, 0x4D58, 0x4D59,
            0x4D5A, 0x4D5B, 0x4D6E, 0x4D6F, 0x4D70, 0x4D71, 0x4D72, 0x4D84,
            0x4D85, 0x4D86, 0x52B5, 0x52B6, 0x52B7, 0x52B8, 0x52B9, 0x52BA,
            0x52BB, 0x52BC, 0x52BD,

            0x4CCE, 0x4CCF, 0x4CD1, 0x4CD2, 0x4CD4, 0x4CD5, 0x4CD7, 0x4CD9,
            0x4CDB, 0x4CDC, 0x4CDE, 0x4CDF, 0x4CE1, 0x4CE2, 0x4CE4, 0x4CE5,
            0x4CE7, 0x4CE8, 0x4CF9, 0x4CFA, 0x4CFC, 0x4CFD, 0x4CFF, 0x4D00,
            0x4D02, 0x4D03, 0x4D45, 0x4D46, 0x4D47, 0x4D48, 0x4D49, 0x4D4A,
            0x4D4B, 0x4D4C, 0x4D4D, 0x4D4E, 0x4D4F, 0x4D50, 0x4D51, 0x4D52,
            0x4D53, 0x4D5C, 0x4D5D, 0x4D5E, 0x4D5F, 0x4D60, 0x4D61, 0x4D62,
            0x4D63, 0x4D64, 0x4D65, 0x4D66, 0x4D67, 0x4D68, 0x4D69, 0x4D73,
            0x4D74, 0x4D75, 0x4D76, 0x4D77, 0x4D78, 0x4D79, 0x4D7A, 0x4D7B,
            0x4D7C, 0x4D7D, 0x4D7E, 0x4D7F, 0x4D87, 0x4D88, 0x4D89, 0x4D8A,
            0x4D8B, 0x4D8C, 0x4D8D, 0x4D8E, 0x4D8F, 0x4D90, 0x4D95, 0x4D96,
            0x4D97, 0x4D99, 0x4D9A, 0x4D9B, 0x4D9D, 0x4D9E, 0x4D9F, 0x4DA1,
            0x4DA2, 0x4DA3, 0x4DA5, 0x4DA6, 0x4DA7, 0x4DA9, 0x4DAA, 0x4DAB,
            0x52BE, 0x52BF, 0x52C0, 0x52C1, 0x52C2, 0x52C3, 0x52C4, 0x52C5,
            0x52C6, 0x52C7, 0x4C9E, // O'hii tree
            0x4CA8, 0x4CAA, 0x4CAB,	// Banana Tree
            0x4D94, 0x4D98, 		// Apple tree
            0x4D9C, 0x4DA0, 		// Peach tree
            0x4DA4, 0x4DA8 			// Pear
        };
        #endregion

        public static bool CheckTreesInRange( Map map, int range, int min, Point3D p )
        {
            int count = 0;

            for( int x = -range; count < min && x <= range; ++x )
            {
                for( int y = -range; count < min && y <= range; ++y )
                {
                    Tile[] tiles = map.Tiles.GetStaticTiles( p.X + x, p.Y + y, false );

                    for( int i = 0; count < min && i < tiles.Length; ++i )
                    {
                        if( IsTree( tiles[ i ].ID & 0x3FFF ) )
                        {
                            if( ( p.Z + 16 ) < tiles[ i ].Z || ( tiles[ i ].Z + 16 ) < p.Z )
                                continue;

                            Console.WriteLine( "Tree found" );
                            count++;
                        }
                    }
                }
            }

            Console.WriteLine( "Tree check: {0} {1}", count, min );
            return count >= min;
        }

        public static bool IsTree( int tileID )
        {
            tileID = tileID | 0x4000;

            return Array.LastIndexOf( m_TreeTiles, tileID ) > -1;
        }

        public static bool WalksHiddenInForest = false;

        public static bool HandleOnMove( Mobile from, Direction d )
        {
            if( ( d & Direction.Running ) != 0 )
            {
                from.RevealingAction();
            }
            else
            {
                if( from.AllowedStealthSteps-- <= 0 && ScoutMimeticPaint.IsUnderMimetism( from ) && IsInForest( from ) )
                {
                    from.SendMessage( "You continue walking in the shadows." );
                    Server.SkillHandlers.Stealth.OnUse( from );
                    return true;
                }
            }

            return false;
        }

        public static bool MimetismEnabled = false;

        public static bool HandleStealth( Mobile m, out TimeSpan scoutResult )
        {
            if( !MimetismEnabled )
            {
                scoutResult = TimeSpan.FromSeconds( 1.0 );
                return false;
            }

            if( IsScout( m ) && ScoutMimeticPaint.IsUnderMimetism( m ) && IsInForest( m ) )
            {
                if( (int)m.ArmorRating > 26 )
                {
                    m.SendLocalizedMessage( 502727 ); // You could not hope to move quietly wearing this much armor.
                    m.RevealingAction();
                    scoutResult = TimeSpan.FromSeconds( 1.0 );
                }
                else
                {
                    m.AllowedStealthSteps = 20;
                    m.SendLocalizedMessage( 502730 ); // You begin to move quietly.
                    scoutResult = TimeSpan.FromSeconds( 10.0 );
                }

                return true;
            }

            scoutResult = TimeSpan.FromSeconds( 1.0 );
            return false;
        }
    }
}