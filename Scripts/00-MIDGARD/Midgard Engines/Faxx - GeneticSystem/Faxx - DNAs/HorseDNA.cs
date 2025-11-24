/***************************************************************************
FILE:	HorseDNA.cs
AUTHOR: Fabrizio Castellano
EMAIL: fabrizio.castellano (at) gmail.com
DATE: 21/12/2008

CONTENT: 
	class HorseDNA
	
DEPENDENCIES:
	AnimalDNA class
	PhenotypeMap class
	Standard RunUO 2.0 GenericReader class
	
INSTALLATION: Just add to scripts

DESCRIPTION: A sample Horse DNA.
	
USE: Use in the Horse.cs script

SEE ALSO:
	AnimalDNA.cs
	PhenotypeMap.cs
	Horse.cs
***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server.Commands;
using Server.Targeting;

namespace Server.Mobiles
{
    public class HorseDNA : AnimalDNA
    {
        #region tables
        // Horse bodies
        // 0xE2 is the recessive body
        // 0xE4 is the dominant
        private static readonly int[] m_BodyList = new int[]
                                                   { 
                                                       0xc8, 
                                                       0xcc, 
                                                       0xe2, 
                                                       0xe4 
                                                   };

        /*
            White Normal Common:        Body 226 - Hue 0
            White Normal Rare:          Body (200,204,226,228) - Hue 1065
            White Pure Common:          Body (200,204,226,228) - Hue 1592
            White Pure Rare:            Body (200,204,226,228) - Hue 1069
           
            Black Normal Common:        Body 200 - Hue 1108
            Black Normal Rare:          Body (200,204,226,228) - Hue 1190
            Black Pure Common:          Body (200,204,226,228) - Hue 1175
            Black Pure Rare:            Body (200,226,228) - Hue 2510 // con Body 204 - Hue 1094
         
            Brown Normal Common:        Body 204 - Hue 0      
            Brown Normal Rare:          Body (200,226,228) - Hue 2140
            Brown Pure Common:          Body (200,226,228) - Hue 1844
            Brown Pure Rare:            Body (200,226,228) - Hue 2287
        */

        // Horse colors
        // mapping between horse type and hue
        private static readonly int[] m_HueList = new int[] 
                                                  { 
                                                      // black horse
                                                      2510,   // black pure rare
                                                      1175,   // black pure common
                                                      1190,   // black normal rare
                                                      1108,   // black normal common
		
                                                      // brown horse
                                                      2287, 	// brown pure rare
                                                      1844, 	// brown pure common
                                                      2140, 	// brown normal rare
                                                      0, 	    // brown normal common
		
                                                      // white horse
                                                      1069,	// white pure rare
                                                      1592,	// white pure common
                                                      1065,	// white normal rare
                                                      0	    // white normal common
                                                  };
        #endregion

        public static void Initialize()
        {
            CommandSystem.Register( "DisplayHorseMatrix", AccessLevel.Administrator, new CommandEventHandler( DisplayHorseMatrix_OnCommand ) );
            CommandSystem.Register( "ProfileDnaSystem", AccessLevel.Administrator, new CommandEventHandler( ProfileDNA_OnCommand ) );

            CommandSystem.Register( "CheckHorseDNA", AccessLevel.GameMaster, new CommandEventHandler( CheckHorseDNA_OnCommand ) );
        }

        [Usage( "ProfileDnaSystem" )]
        [Description( "Generate useful profiles for dna system." )]
        private static void ProfileDNA_OnCommand( CommandEventArgs e )
        {
            List<HorseDNA> parents = new List<HorseDNA>();

            // generate the first generation
            for( int i = 0; i < 10; i++ )
                parents.Add( new HorseDNA() );

            try
            {
                Dictionary<Point3D, int> dict = new Dictionary<Point3D, int>();

                using( StreamWriter op = new StreamWriter( Path.Combine( "Logs", "testDNA.log" ), true ) )
                {
                    op.WriteLine( string.Format( "DNA log created on: " + DateTime.Now ) );
                    op.WriteLine( "F\tM\tC" );

                    for( int i = 0; i < 100000; i++ )
                    {
                        HorseDNA father = Random( parents );
                        HorseDNA mother = Random( parents );
                        HorseDNA child = new HorseDNA( father, mother );

                        op.WriteLine( string.Format( "{0}\t{1}\t{2}", father.Color, mother.Color, child.Color ) );

                        parents.Add( child );

                        Point3D ennupla = new Point3D( father.Color, mother.Color, child.Color );

                        if( dict.ContainsKey( ennupla ) )
                            dict[ ennupla ] = dict[ ennupla ] + 1;
                        else
                            dict[ ennupla ] = 1;
                    }
                }

                using( StreamWriter op = new StreamWriter( Path.Combine( "Logs", "testDNA-summ.log" ), true ) )
                {
                    op.WriteLine( string.Format( "DNA log created on: " + DateTime.Now ) );
                    op.WriteLine( "F\tM\tC\tN" );

                    foreach( KeyValuePair<Point3D, int> kvp in dict )
                        op.WriteLine( string.Format( "{0}\t{1}\t{2}\t{3}", kvp.Key.X, kvp.Key.Y, kvp.Key.Z, kvp.Value ) );
                }
            }
            catch
            {
            }
        }

        private static HorseDNA Random( IList<HorseDNA> list )
        {
            return list != null && list.Count > 0 ? list[ Utility.Random( list.Count ) ] : new HorseDNA();
        }

        [Usage( "DisplayHorseMatrix" )]
        [Description( "Create a matrix of breedable horses." )]
        private static void DisplayHorseMatrix_OnCommand( CommandEventArgs e )
        {
            DisplayMatrix( e.Mobile );
        }

        [Usage( "CheckHorseDNA" )]
        [Description( "End quicky the mate process for the current pregnant horse." )]
        private static void CheckHorseDNA_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Target the horse you want to force the ." );
            e.Mobile.Target = new CheckHorseDNATarget();
        }

        private class CheckHorseDNATarget : Target
        {
            public CheckHorseDNATarget()
                : base( 15, false, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object targ )
            {
                if( ( targ is Horse ) )
                {
                    Horse horse = (Horse)targ;
                    if( !horse.Female )
                        from.SendMessage( "That is not a female horse." );
                    if( !horse.IsPregnant )
                        from.SendMessage( "That is not a pregnant horse." );
                    else
                        horse.Deliver( true );
                }
                else
                    from.SendMessage( "That is not a horse." );
            }
        }

        private static void DisplayMatrix( IEntity m )
        {
            for( int i = 0; i < m_BodyList.Length; i++ )
            {
                for( int j = 0; j < m_HueList.Length; j++ )
                {
                    Point3D location = m.Location;

                    Horse horse = new Horse();
                    horse.CantWalk = true;

                    horse.Body = m_BodyList[ i ];
                    horse.Hue = m_HueList[ j ];
                    horse.Name = string.Format( "Body: {0} Hue {1}", horse.Body, horse.Hue );

                    horse.DNA.Color = horse.Hue;
                    horse.DNA.Body = horse.Body;

                    bool invalid = false;

                    if( horse.Hue == 0 && ( horse.Body != 204 && horse.Body != 226 ) )
                        invalid = true;

                    if( horse.Hue == 1108 && horse.Body != 200 )
                        invalid = true;

                    if( horse.Hue == 2510 && horse.Body == 204 )
                        invalid = true;

                    if( ( horse.Hue == 2287 || horse.Hue == 1844 || horse.Hue == 2140 ) && ( horse.Body != 200 && horse.Body != 226 && horse.Body != 228 ) )
                        invalid = true;

                    if( invalid )
                    {
                        horse.Delete();
                        continue;
                    }

                    horse.DNA.ApplySpecialCriteria( horse );

                    location.Offset( i * 2, j * 3, 0 );
                    horse.MoveToWorld( location, Map.Felucca );
                }
            }
        }

        public override void ApplySpecialCriteria( Mobile m )
        {
            string check = "";
            int oldHue = m.Hue;
            int oldBody = m.Body;

            int newBody = -1;
            int newHue = -1;

            if( oldHue == 0 && ( oldBody != 204 && oldBody != 226 ) ) // brown normal common and white normal common
            {
                newBody = Utility.RandomList( 204, 226 );
                check = "oldHue == 0 && ( oldBody != 204 && oldBody != 226 )";
            }

            if( oldHue == 1108 && m.Body != 200 ) // black normal common
            {
                newBody = 200;
                check = "oldHue == 1108 && m.Body != 200";
            }

            if( oldHue == 2510 && m.Body == 204 ) // black normal common
            {
                newHue = 1094;
                check = "oldHue == 2510 && m.Body == 204";
            }

            if( ( oldHue == 2287 || oldHue == 1844 || oldHue == 2140 ) && ( oldBody != 200 && oldBody != 226 && oldBody != 228 ) ) // Brown Normal Rare and Pures
            {
                newBody = Utility.RandomList( 200, 226, 228 );
                check = "( oldHue == 2287 || oldHue == 1844 || oldHue == 2140 ) && ( oldBody != 200 && oldBody != 226 && oldBody != 228 )";
            }

            if( newBody != -1 )
            {
                m.Body = newBody;
                Body = newBody;

                if( Core.Debug )
                    Console.WriteLine( "Notice: Horse DNA changes. (type {3} - serial {4}) oldBody: {0} - newBody {1} - oldHue {2}", oldBody, newBody, oldHue, m.GetType().Name, m.Serial );
            }

            if( newHue != -1 )
            {
                m.Hue = newHue;

                if( Array.IndexOf( m_HueList, newHue ) > -1 )
                    Color = newHue;

                if( Core.Debug )
                    Console.WriteLine( "Notice: Horse DNA changes. (type {3} - serial {4}) oldHue: {0} - newHue {1} - oldBody {2}", oldHue, newHue, oldBody, m.GetType().Name, m.Serial );
            }

            if( Core.Debug && !string.IsNullOrEmpty( check ) )
                Console.WriteLine( check );

            if( m is Horse )
                ( (Horse)m ).UpdateItemid();
        }

        public override int[] HueList { get { return m_HueList; } } // override base HueList
        public override int[] BodyList { get { return m_BodyList; } } // override base BodyList

        // standard horse genetic map
        public HorseDNA()
        {
            m_Sequence[ 0 ] = (byte)Utility.RandomMinMax( 22, 98 );     // str
            m_Sequence[ 1 ] = (byte)Utility.RandomMinMax( 56, 75 );     // dex
            m_Sequence[ 2 ] = (byte)Utility.RandomMinMax( 6, 10 );      // int

            // m_Sequence[ 3 ] = 0x55; 				                    // body (2 bits) - hue (6 bits) - looks:  01 01 01 01
            Body = Utility.RandomList( m_BodyList );
            Color = 0;

            m_Sequence[ 4 ] = 3;                                        // damage min
            m_Sequence[ 5 ] = 4;                                        // damage max
            m_Sequence[ 6 ] = 0;                                        // virtual armor
        }

        // hybridization constructor
        public HorseDNA( BaseDNA father, BaseDNA mother )
            : base( father, mother )
        {
            try
            {
                using( StreamWriter op = new StreamWriter( Path.Combine( "Logs", "horseDNA.log" ), true ) )
                {
                    op.WriteLine( string.Format( "DNA created on: " + DateTime.Now ) );

                    op.WriteLine( string.Format( "Father:" ) );
                    op.WriteLine( father.Log() );

                    op.WriteLine( string.Format( "Mother:" ) );
                    op.WriteLine( mother.Log() );

                    op.WriteLine( string.Format( "Child:" ) );
                    op.WriteLine( Log() );

                    op.WriteLine( string.Format( "" ) );
                }
            }
            catch
            {
            }
        }

        // deserialization constructor
        public HorseDNA( GenericReader reader, Mobile owner )
            : base( reader, owner )
        {
        }
    }
}