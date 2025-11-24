using System;
using System.Collections.Generic;
using Server.Commands;

namespace Server.Mobiles
{
    public class SpawnObject
    {
        private string m_TypeName;
        private int m_MaxCount;
        private int m_SubGroup;
        private int m_SequentialResetTo;
        private int m_KillsNeeded;
        private bool m_RestrictKillsToSubgroup;
        private bool m_ClearOnAdvance = true;
        private double m_MinDelay = -1;
        private double m_MaxDelay = -1;
        private int m_SpawnsPerTick = 1;
        private bool m_Disabled;
        private int m_PackRange = -1;
        private bool m_Ignore;
        // temporary variable used to calculate weighted spawn probabilities
        public bool Available;

        public List<object> SpawnedObjects;
        public string[] PropertyArgs;
        public double SequentialResetTime;
        public int EntryOrder; // used for sorting
        public bool RequireSurface = true;
        public DateTime NextSpawn;
        public bool SpawnedThisTick;

        // these are externally accessible to the SETONSPAWNENTRY keyword
        public string TypeName
        {
            get { return m_TypeName; }
            set { m_TypeName = value; }
        }

        public int MaxCount
        {
            get
            {
                if( Disabled )
                {
                    return 0;
                }
                else
                {
                    return m_MaxCount;
                }
            }
            set { m_MaxCount = value; }
        }

        public int ActualMaxCount
        {
            get { return m_MaxCount; }
            set { m_MaxCount = value; }
        }

        public int SubGroup
        {
            get { return m_SubGroup; }
            set { m_SubGroup = value; }
        }

        public int SpawnsPerTick
        {
            get { return m_SpawnsPerTick; }
            set { m_SpawnsPerTick = value; }
        }

        public int SequentialResetTo
        {
            get { return m_SequentialResetTo; }
            set { m_SequentialResetTo = value; }
        }

        public int KillsNeeded
        {
            get { return m_KillsNeeded; }
            set { m_KillsNeeded = value; }
        }

        public bool RestrictKillsToSubgroup
        {
            get { return m_RestrictKillsToSubgroup; }
            set { m_RestrictKillsToSubgroup = value; }
        }

        public bool ClearOnAdvance
        {
            get { return m_ClearOnAdvance; }
            set { m_ClearOnAdvance = value; }
        }

        public double MinDelay
        {
            get { return m_MinDelay; }
            set { m_MinDelay = value; }
        }

        public double MaxDelay
        {
            get { return m_MaxDelay; }
            set { m_MaxDelay = value; }
        }

        public bool Disabled
        {
            get { return m_Disabled; }
            set { m_Disabled = value; }
        }

        public bool Ignore
        {
            get { return m_Ignore; }
            set { m_Ignore = value; }
        }

        public int PackRange
        {
            get { return m_PackRange; }
            set { m_PackRange = value; }
        }


        // command loggable constructor
        public SpawnObject( Mobile from, XmlSpawner spawner, string name, int maxamount )
        {
            if( from != null && spawner != null )
            {
                bool found = false;
                // go through the current spawner objects and see if this is a new entry
                if( spawner.m_SpawnObjects != null )
                {
                    for( int i = 0; i < spawner.m_SpawnObjects.Count; i++ )
                    {
                        SpawnObject s = spawner.m_SpawnObjects[ i ];
                        if( s != null && s.TypeName == name )
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if( !found )
                {
                    CommandLogging.WriteLine( from, "{0} {1} added to XmlSpawner {2} '{3}' [{4}, {5}] ({6}) : {7}",
                                             from.AccessLevel, CommandLogging.Format( from ), spawner.Serial, spawner.Name,
                                             spawner.GetWorldLocation().X, spawner.GetWorldLocation().Y, spawner.Map,
                                             name );
                }
            }

            TypeName = name;
            MaxCount = maxamount;
            SubGroup = 0;
            SequentialResetTime = 0;
            SequentialResetTo = 0;
            KillsNeeded = 0;
            RestrictKillsToSubgroup = false;
            ClearOnAdvance = true;
            SpawnedObjects = new List<object>();
        }

        public SpawnObject( string name, int maxamount )
        {
            TypeName = name;
            MaxCount = maxamount;
            SubGroup = 0;
            SequentialResetTime = 0;
            SequentialResetTo = 0;
            KillsNeeded = 0;
            RestrictKillsToSubgroup = false;
            ClearOnAdvance = true;
            SpawnedObjects = new List<object>();
        }

        public SpawnObject( string name, int maxamount, int subgroup, double sequentialresettime, int sequentialresetto,
                           int killsneeded,
                           bool restrictkills, bool clearadvance, double mindelay, double maxdelay, int spawnsper,
                           int packrange )
        {
            TypeName = name;
            MaxCount = maxamount;
            SubGroup = subgroup;
            SequentialResetTime = sequentialresettime;
            SequentialResetTo = sequentialresetto;
            KillsNeeded = killsneeded;
            RestrictKillsToSubgroup = restrictkills;
            ClearOnAdvance = clearadvance;
            MinDelay = mindelay;
            MaxDelay = maxdelay;
            SpawnsPerTick = spawnsper;
            PackRange = packrange;
            SpawnedObjects = new List<object>();
        }

        internal static string GetParm( string str, string separator )
        {
            // find the parm separator in the string
            // then look for the termination at the ':'  or end of string
            // and return the stuff between
            string[] arg = BaseXmlSpawner.SplitString( str, separator );
            //should be 2 args
            if( arg.Length > 1 )
            {
                // look for the end of parm terminator (could also be eol)
                string[] parm = arg[ 1 ].Split( ':' );
                if( parm.Length > 0 )
                {
                    return ( parm[ 0 ] );
                }
            }
            return ( null );
        }


        internal static SpawnObject[] LoadSpawnObjectsFromString( string objectList )
        {
            // Clear the spawn object list
            List<SpawnObject> newSpawnObjects = new List<SpawnObject>();

            if( !string.IsNullOrEmpty( objectList ) )
            {
                // Split the string based on the object separator first ':'
                string[] spawnObjectList = objectList.Split( ':' );

                // Parse each item in the array
                foreach( string s in spawnObjectList )
                {
                    // Split the single spawn object item by the max count '='
                    string[] spawnObjectDetails = s.Split( '=' );

                    // Should be two entries
                    if( spawnObjectDetails.Length == 2 )
                    {
                        // Validate the information

                        // Make sure the spawn object name part has a valid length
                        if( spawnObjectDetails[ 0 ].Length > 0 )
                        {
                            // Make sure the max count part has a valid length
                            if( spawnObjectDetails[ 1 ].Length > 0 )
                            {
                                int maxCount = 1;

                                try
                                {
                                    maxCount = int.Parse( spawnObjectDetails[ 1 ] );
                                }
                                catch( Exception )
                                {
                                    // Something went wrong, leave the default amount }
                                }

                                // Create the spawn object and store it in the array list
                                SpawnObject so = new SpawnObject( spawnObjectDetails[ 0 ], maxCount );
                                newSpawnObjects.Add( so );
                            }
                        }
                    }
                }
            }

            return newSpawnObjects.ToArray();
        }


        internal static SpawnObject[] LoadSpawnObjectsFromString2( string objectList )
        {
            // Clear the spawn object list
            List<SpawnObject> newSpawnObjects = new List<SpawnObject>();

            // spawn object definitions will take the form typestring:MX=int:SB=int:RT=double:TO=int:KL=int
            // or typestring:MX=int:SB=int:RT=double:TO=int:KL=int:OBJ=typestring...
            if( !string.IsNullOrEmpty( objectList ) )
            {
                string[] spawnObjectList = BaseXmlSpawner.SplitString( objectList, ":OBJ=" );

                // Parse each item in the array
                foreach( string s in spawnObjectList )
                {
                    // at this point each spawn string will take the form typestring:MX=int:SB=int:RT=double:TO=int:KL=int
                    // Split the single spawn object item by the max count to get the typename and the remaining parms
                    string[] spawnObjectDetails = BaseXmlSpawner.SplitString( s, ":MX=" );

                    // Should be two entries
                    if( spawnObjectDetails.Length == 2 )
                    {
                        // Validate the information

                        // Make sure the spawn object name part has a valid length
                        if( spawnObjectDetails[ 0 ].Length > 0 )
                        {
                            // Make sure the parm part has a valid length
                            if( spawnObjectDetails[ 1 ].Length > 0 )
                            {
                                // now parse out the parms
                                // MaxCount
                                string parmstr = GetParm( s, ":MX=" );
                                int maxCount = 1;
                                try
                                {
                                    maxCount = int.Parse( parmstr );
                                }
                                catch
                                {
                                }

                                // SubGroup
                                parmstr = GetParm( s, ":SB=" );

                                int subGroup = 0;
                                try
                                {
                                    subGroup = int.Parse( parmstr );
                                }
                                catch
                                {
                                }

                                // SequentialSpawnResetTime
                                parmstr = GetParm( s, ":RT=" );
                                double resetTime = 0;
                                try
                                {
                                    resetTime = double.Parse( parmstr );
                                }
                                catch
                                {
                                }

                                // SequentialSpawnResetTo
                                parmstr = GetParm( s, ":TO=" );
                                int resetTo = 0;
                                try
                                {
                                    resetTo = int.Parse( parmstr );
                                }
                                catch
                                {
                                }

                                // KillsNeeded
                                parmstr = GetParm( s, ":KL=" );
                                int killsNeeded = 0;
                                try
                                {
                                    killsNeeded = int.Parse( parmstr );
                                }
                                catch
                                {
                                }

                                // RestrictKills
                                parmstr = GetParm( s, ":RK=" );
                                bool restrictKills = false;
                                if( parmstr != null )
                                    try
                                    {
                                        restrictKills = ( int.Parse( parmstr ) == 1 );
                                    }
                                    catch
                                    {
                                    }

                                // ClearOnAdvance
                                parmstr = GetParm( s, ":CA=" );
                                bool clearAdvance = true;
                                // if kills needed is zero, then set CA to false by default.  This maintains consistency with the
                                // previous default behavior for old spawn specs that havent specified CA
                                if( killsNeeded == 0 )
                                    clearAdvance = false;
                                if( parmstr != null )
                                    try
                                    {
                                        clearAdvance = ( int.Parse( parmstr ) == 1 );
                                    }
                                    catch
                                    {
                                    }

                                // MinDelay
                                parmstr = GetParm( s, ":DN=" );
                                double minD = -1;
                                try
                                {
                                    minD = double.Parse( parmstr );
                                }
                                catch
                                {
                                }

                                // MaxDelay
                                parmstr = GetParm( s, ":DX=" );
                                double maxD = -1;
                                try
                                {
                                    maxD = double.Parse( parmstr );
                                }
                                catch
                                {
                                }

                                // SpawnsPerTick
                                parmstr = GetParm( s, ":SP=" );
                                int spawnsPer = 1;
                                try
                                {
                                    spawnsPer = int.Parse( parmstr );
                                }
                                catch
                                {
                                }

                                // PackRange
                                parmstr = GetParm( s, ":PR=" );
                                int packRange = -1;
                                try
                                {
                                    packRange = int.Parse( parmstr );
                                }
                                catch
                                {
                                }

                                // Create the spawn object and store it in the array list
                                SpawnObject so = new SpawnObject( spawnObjectDetails[ 0 ], maxCount, subGroup, resetTime,
                                                                 resetTo, killsNeeded,
                                                                 restrictKills, clearAdvance, minD, maxD, spawnsPer,
                                                                 packRange );

                                newSpawnObjects.Add( so );
                            }
                        }
                    }
                }
            }

            return newSpawnObjects.ToArray();
        }
    }
}