using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;

/*
** XmlAdd
** Version 1.00
** updated 4/19/04
** ArteGordon
**
** version 1.04
** update 8/08/04
** - the add button now uses targeting to place spawners rather than just putting them at the location of the placer.
**
** version 1.03
** update 8/07/04
**
** - changed the selection buttons to checkboxes, and changed their gump art for space reasons.
** - added the smartspawning option
**
** Changelog
** version 1.02
** update 7/14/04
** - the duration entry was not being applied to added spawners  (Thanks to BlackNova for pointing this out).
**
** version 1.02
** update 7/14/04
** - added the SkillTrigger entry
**
** version 1.01
** update 4/22/04
** - added a spawn entry selection list
** - added named save and load defaults options
**
*/

namespace Server.Mobiles
{
    public class XmlSpawnerDefaults
    {
        public class DefaultEntry
        {
            public string AccountName;
            public string PlayerName;
            public TimeSpan MinDelay = TimeSpan.FromMinutes( 5 );
            public TimeSpan MaxDelay = TimeSpan.FromMinutes( 10 );
            public TimeSpan RefractMin = TimeSpan.FromMinutes( 0 );
            public TimeSpan RefractMax = TimeSpan.FromMinutes( 0 );
            public TimeSpan TODStart = TimeSpan.FromMinutes( 0 );
            public TimeSpan TODEnd = TimeSpan.FromMinutes( 0 );
            public TimeSpan Duration = TimeSpan.FromMinutes( 0 );
            public TimeSpan DespawnTime = TimeSpan.FromHours( 0 );
            public bool Group;
            public int Team;
            public int ProximitySound = 0x1F4;
            public string SpeechTrigger;
            public string SkillTrigger;
            public int SequentialSpawn = -1;
            public bool HomeRangeIsRelative = true;
            public int SpawnRange = 5;
            public int HomeRange = 5;
            public int ProximityRange = -1;
            public XmlSpawner.TODModeType TODMode = XmlSpawner.TODModeType.Realtime;
            public int KillReset = 1;
            public string SpawnerName = "Spawner";
            public bool AllowGhostTrig;
            public bool AllowNPCTrig;
            public bool SpawnOnTrigger;
            public bool SmartSpawning;
            public bool ExternalTriggering;
            public string TriggerOnCarried;
            public string NoTriggerOnCarried;
            public string ProximityMsg;
            public double TriggerProbability = 1;
            public string PlayerTriggerProp;
            public string TriggerObjectProp;
            public string DefsExt;
            public string[] NameList;
            public bool[] SelectionList;
            public int AddGumpX = 440;
            public int AddGumpY;
            public int SpawnerGumpX;
            public int SpawnerGumpY;
            public int FindGumpX;
            public int FindGumpY;

            // these are additional defaults that are not set by XmlAdd but can be used by other routines such as the custom properties gump to determine 
            // whether properties have been changed from spawner default values
            public string ConfigFile;
            public int StackAmount = 1;
            public string MobTriggerName;
            public string MobTriggerProp;
            public string RegionName;
            public bool Running = true;
            public Item SetItem;
            public AccessLevel TriggerAccessLevel = AccessLevel.Player;
            public Item TriggerObject;
            public WayPoint WayPoint;
            public bool ShowBounds;

            public bool AutoNumber;
            public int AutoNumberValue;

            public XmlAddCAGCategory CurrentCategory;
            public int CurrentCategoryPage;
            public int CategorySelectionIndex = -1;

            public XmlSpawner LastSpawner;
            public Map StartingMap;
            public Point3D StartingLoc;
            public bool ShowExtension;

            public bool IgnoreUpdate;
        }

        public static List<DefaultEntry> DefaultEntryList;

        public static DefaultEntry GetDefaults( string account, string name )
        {
            // find the default entry corresponding to the account and username
            if( DefaultEntryList != null )
            {
                for( int i = 0; i < DefaultEntryList.Count; i++ )
                {
                    DefaultEntry entry = DefaultEntryList[ i ];
                    if( entry != null && string.Compare( entry.PlayerName, name, true ) == 0 &&
                        string.Compare( entry.AccountName, account, true ) == 0 )
                    {
                        return entry;
                    }
                }
            }
            // if not found then add one
            var newentry = new DefaultEntry();
            newentry.PlayerName = name;
            newentry.AccountName = account;
            if( DefaultEntryList == null )
                DefaultEntryList = new List<DefaultEntry>();
            DefaultEntryList.Add( newentry );
            return newentry;
        }

        public static void RestoreDefs( DefaultEntry defs )
        {
            if( defs == null )
                return;
            defs.MinDelay = TimeSpan.FromMinutes( 5 );
            defs.MaxDelay = TimeSpan.FromMinutes( 10 );
            defs.RefractMin = TimeSpan.FromMinutes( 0 );
            defs.RefractMax = TimeSpan.FromMinutes( 0 );
            defs.TODStart = TimeSpan.FromMinutes( 0 );
            defs.TODEnd = TimeSpan.FromMinutes( 0 );
            defs.Duration = TimeSpan.FromMinutes( 0 );
            defs.DespawnTime = TimeSpan.FromHours( 0 );
            defs.Group = false;
            defs.Team = 0;
            defs.ProximitySound = 0x1F4;
            defs.SpeechTrigger = null;
            defs.SkillTrigger = null;
            defs.SequentialSpawn = -1;
            defs.HomeRangeIsRelative = true;
            defs.SpawnRange = 5;
            defs.HomeRange = 5;
            defs.ProximityRange = -1;
            defs.TODMode = XmlSpawner.TODModeType.Realtime;
            defs.KillReset = 1;
            defs.SpawnerName = "Spawner";
            defs.AllowGhostTrig = false;
            defs.AllowNPCTrig = false;
            defs.SpawnOnTrigger = false;
            defs.SmartSpawning = false;
            defs.ExternalTriggering = false;
            defs.TriggerOnCarried = null;
            defs.NoTriggerOnCarried = null;
            defs.ProximityMsg = null;
            defs.TriggerProbability = 1;
            defs.PlayerTriggerProp = null;
            defs.TriggerObjectProp = null;
            defs.DefsExt = null;
            defs.AddGumpX = 440;
            defs.AddGumpY = 0;
            defs.SpawnerGumpX = 0;
            defs.SpawnerGumpY = 0;
            defs.FindGumpX = 0;
            defs.FindGumpY = 0;
            defs.AutoNumber = false;
            defs.AutoNumberValue = 0;


            if( defs.SelectionList != null )
                Array.Clear( defs.SelectionList, 0, defs.SelectionList.Length );
            if( defs.NameList != null )
                Array.Clear( defs.NameList, 0, defs.NameList.Length );
        }
    }


    public class XmlAddGump : Gump
    {
        private const int MaxEntries = 40;
        private const int MaxEntriesPerColumn = 20;
        private const string DefsDataSetName = "Defs";
        private const string DefsTablePointName = "Values";
        private const string DefsDir = "SpawnerDefs";

        private static int StartingXoffset = 440;
        private Mobile m_From;

        public XmlSpawnerDefaults.DefaultEntry defs;


        private string NameListToString()
        {
            if( defs.NameList == null || defs.NameList.Length == 0 )
                return "0";
            var sb = new StringBuilder();
            sb.AppendFormat( "{0}", defs.NameList.Length );
            for( int i = 0; i < defs.NameList.Length; i++ )
            {
                sb.AppendFormat( ":{0}", defs.NameList[ i ] );
            }
            return sb.ToString();
        }

        private string SelectionListToString()
        {
            if( defs.SelectionList == null || defs.SelectionList.Length == 0 )
                return "0";
            var sb = new StringBuilder();
            sb.AppendFormat( "{0}", defs.SelectionList.Length );
            for( int i = 0; i < defs.SelectionList.Length; i++ )
            {
                sb.AppendFormat( ":{0}", ( defs.SelectionList[ i ] ? 1 : 0 ) );
            }
            return sb.ToString();
        }

        private static string[] StringToNameList( string namelist )
        {
            var newlist = new string[ MaxEntries ];
            string[] tmplist = namelist.Split( ':' );
            for( int i = 1; i < tmplist.Length; i++ )
            {
                if( i - 1 >= newlist.Length )
                    break;
                newlist[ i - 1 ] = tmplist[ i ];
            }
            return newlist;
        }

        private static bool[] StringToSelectionList( string selectionlist )
        {
            var newlist = new bool[ MaxEntries ];
            string[] tmplist = selectionlist.Split( ':' );
            for( int i = 1; i < tmplist.Length; i++ )
            {
                if( i - 1 >= newlist.Length )
                    break;
                if( tmplist[ i ] == "1" )
                    newlist[ i - 1 ] = true;
                else
                    newlist[ i - 1 ] = false;
            }
            return newlist;
        }


        private void DoSaveDefs( Mobile from, string filename )
        {
            if( string.IsNullOrEmpty( filename ) )
                return;

            // Create the data set
            var ds = new DataSet( DefsDataSetName );

            // Load the data set up
            ds.Tables.Add( DefsTablePointName );

            // Create spawn point schema
            //ds.Tables[DefsTablePointName].Columns.Add( "AccountName" );
            //ds.Tables[DefsTablePointName].Columns.Add( "PlayerName" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "MinDelay" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "MaxDelay" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "SpawnRange" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "HomeRange" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "MinRefractory" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "MaxRefractory" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "TODStart" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "TODEnd" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "Duration" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "DespawnTime" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "RelativeHome" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "IsGroup" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "Team" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "ProximityTriggerSound" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "SpeechTrigger" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "SkillTrigger" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "SequentialSpawn" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "ProximityRange" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "TODMode" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "KillReset" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "SpawnerName" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "AllowGhost" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "AllowNPC" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "SpawnOnTrigger" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "SmartSpawn" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "ExtTrig" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "TrigOnCarried" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "NoTrigOnCarried" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "ProximityMessage" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "TrigProb" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "PlayerTrigProp" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "TrigObjectProp" );
            //ds.Tables[DefsTablePointName].Columns.Add( "DefsExt" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "NameList" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "SelectionList" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "AddGumpX" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "AddGumpY" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "SpawnerGumpX" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "SpawnerGumpY" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "FindGumpX" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "FindGumpY" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "AutoNumber" );
            ds.Tables[ DefsTablePointName ].Columns.Add( "AutoNumberValue" );

            // Create a new data row
            DataRow dr = ds.Tables[ DefsTablePointName ].NewRow();

            // Populate the data
            //dr["AccountName"] = (string)defs.AccountName;
            //dr["PlayerName"] = (string)defs.PlayerName;
            dr[ "SpawnerName" ] = defs.SpawnerName;
            dr[ "MinDelay" ] = defs.MinDelay.TotalMinutes;
            dr[ "MaxDelay" ] = defs.MaxDelay.TotalMinutes;
            dr[ "SpawnRange" ] = defs.SpawnRange;
            dr[ "HomeRange" ] = defs.HomeRange;
            dr[ "RelativeHome" ] = defs.HomeRangeIsRelative;
            dr[ "IsGroup" ] = defs.Group;
            dr[ "Team" ] = defs.Team;
            dr[ "MinRefractory" ] = defs.RefractMin.TotalMinutes;
            dr[ "MaxRefractory" ] = defs.RefractMax.TotalMinutes;
            dr[ "TODStart" ] = defs.TODStart.TotalMinutes;
            dr[ "TODEnd" ] = defs.TODEnd.TotalMinutes;
            dr[ "TODMode" ] = defs.TODMode;
            dr[ "Duration" ] = defs.Duration.TotalMinutes;
            dr[ "DespawnTime" ] = defs.Duration.TotalHours;
            dr[ "ProximityRange" ] = defs.ProximityRange;
            dr[ "ProximityTriggerSound" ] = defs.ProximitySound;
            dr[ "ProximityMessage" ] = defs.ProximityMsg;
            dr[ "SpeechTrigger" ] = defs.SpeechTrigger;
            dr[ "SkillTrigger" ] = defs.SkillTrigger;
            dr[ "SequentialSpawn" ] = defs.SequentialSpawn;
            dr[ "KillReset" ] = defs.KillReset;
            dr[ "TrigProb" ] = defs.TriggerProbability;
            dr[ "AllowGhost" ] = defs.AllowGhostTrig;
            dr[ "AllowNPC" ] = defs.AllowNPCTrig;
            dr[ "SpawnOnTrigger" ] = defs.SpawnOnTrigger;
            dr[ "SmartSpawn" ] = defs.SmartSpawning;
            dr[ "ExtTrig" ] = defs.ExternalTriggering;
            dr[ "TrigOnCarried" ] = defs.TriggerOnCarried;
            dr[ "NoTrigOnCarried" ] = defs.NoTriggerOnCarried;
            dr[ "PlayerTrigProp" ] = defs.PlayerTriggerProp;
            dr[ "TrigObjectProp" ] = defs.TriggerObjectProp;
            dr[ "NameList" ] = NameListToString();
            dr[ "SelectionList" ] = SelectionListToString();
            dr[ "AddGumpX" ] = defs.AddGumpX;
            dr[ "AddGumpY" ] = defs.AddGumpY;
            dr[ "SpawnerGumpX" ] = defs.SpawnerGumpX;
            dr[ "SpawnerGumpY" ] = defs.SpawnerGumpY;
            dr[ "FindGumpX" ] = defs.FindGumpX;
            dr[ "FindGumpY" ] = defs.FindGumpY;
            dr[ "AutoNumber" ] = defs.AutoNumber;
            dr[ "AutoNumberValue" ] = defs.AutoNumberValue;

            // Add the row the the table
            ds.Tables[ DefsTablePointName ].Rows.Add( dr );

            // Write out the file
            bool fileError = false;
            string dirname;
            if( Directory.Exists( DefsDir ) )
            {
                // put it in the defaults directory if it exists
                dirname = String.Format( "{0}/{1}.defs", DefsDir, filename );
            }
            else
            {
                // otherwise just put it in the main installation dir
                dirname = String.Format( "{0}.defs", filename );
            }
            try
            {
                ds.WriteXml( dirname );
            }
            catch
            {
                fileError = true;
            }

            if( fileError )
            {
                if( from != null && !from.Deleted )
                    from.SendMessage( "Error trying to save to file {0}", dirname );
                return;
            }
            else
            {
                if( from != null && !from.Deleted )
                    from.SendMessage( "Saved defs to file {0}", dirname );
            }
        }

        private void DoLoadDefs( Mobile from, string filename )
        {
            if( string.IsNullOrEmpty( filename ) )
                return;
            string dirname;
            if( Directory.Exists( DefsDir ) )
            {
                // look for it in the defaults directory
                dirname = String.Format( "{0}/{1}.defs", DefsDir, filename );
                // Check if the file exists
                if( File.Exists( dirname ) == false )
                {
                    // didnt find it so just look in the main install dir
                    dirname = String.Format( "{0}.defs", filename );
                }
            }
            else
            {
                // look in the main installation dir
                dirname = String.Format( "{0}.defs", filename );
            }
            // Check if the file exists
            if( File.Exists( dirname ) )
            {
                FileStream fs = null;
                try
                {
                    fs = File.Open( dirname, FileMode.Open, FileAccess.Read );
                }
                catch
                {
                }

                if( fs == null )
                {
                    from.SendMessage( "Unable to open {0} for loading", dirname );
                    return;
                }

                // Create the data set
                var ds = new DataSet( DefsDataSetName );

                // Read in the file
                //ds.ReadXml( e.Arguments[0].ToString() );
                bool fileerror = false;
                try
                {
                    ds.ReadXml( fs );
                }
                catch
                {
                    fileerror = true;
                }
                // close the file
                fs.Close();
                if( fileerror )
                {
                    if( from != null && !from.Deleted )
                        from.SendMessage( 33, "Error reading defs file {0}", dirname );
                    return;
                }

                // Check that at least a single table was loaded
                if( ds.Tables != null && ds.Tables.Count > 0 )
                {
                    // Add each spawn point to the current map
                    if( ds.Tables[ DefsTablePointName ] != null && ds.Tables[ DefsTablePointName ].Rows.Count > 0 )
                    {
                        //foreach( DataRow dr in ds.Tables[DefsTablePointName].Rows ){
                        DataRow dr = ds.Tables[ DefsTablePointName ].Rows[ 0 ];

                        try
                        {
                            defs.SpawnerName = (string)dr[ "SpawnerName" ];
                        }
                        catch
                        {
                        }

                        double mindelay = defs.MinDelay.TotalMinutes;
                        try
                        {
                            mindelay = double.Parse( (string)dr[ "MinDelay" ] );
                        }
                        catch
                        {
                        }
                        defs.MinDelay = TimeSpan.FromMinutes( mindelay );

                        double maxdelay = defs.MaxDelay.TotalMinutes;
                        try
                        {
                            maxdelay = double.Parse( (string)dr[ "MaxDelay" ] );
                        }
                        catch
                        {
                        }
                        defs.MaxDelay = TimeSpan.FromMinutes( maxdelay );

                        try
                        {
                            defs.SpawnRange = int.Parse( (string)dr[ "SpawnRange" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.HomeRange = int.Parse( (string)dr[ "HomeRange" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.HomeRangeIsRelative = bool.Parse( (string)dr[ "RelativeHome" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.Group = bool.Parse( (string)dr[ "IsGroup" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.Team = int.Parse( (string)dr[ "Team" ] );
                        }
                        catch
                        {
                        }

                        double minrefract = defs.RefractMin.TotalMinutes;
                        try
                        {
                            minrefract = double.Parse( (string)dr[ "MinRefractory" ] );
                        }
                        catch
                        {
                        }
                        defs.RefractMin = TimeSpan.FromMinutes( minrefract );

                        double maxrefract = defs.RefractMax.TotalMinutes;
                        try
                        {
                            maxrefract = double.Parse( (string)dr[ "MaxRefractory" ] );
                        }
                        catch
                        {
                        }
                        defs.RefractMax = TimeSpan.FromMinutes( maxrefract );

                        double todstart = defs.TODStart.TotalMinutes;
                        try
                        {
                            todstart = double.Parse( (string)dr[ "TODStart" ] );
                        }
                        catch
                        {
                        }
                        defs.TODStart = TimeSpan.FromMinutes( todstart );

                        double todend = defs.TODEnd.TotalMinutes;
                        try
                        {
                            todend = double.Parse( (string)dr[ "TODEnd" ] );
                        }
                        catch
                        {
                        }
                        defs.TODEnd = TimeSpan.FromMinutes( todend );

                        string todmode = null;
                        try
                        {
                            todmode = (string)dr[ "TODMode" ];
                        }
                        catch
                        {
                        }
                        if( todmode != null )
                        {
                            if( todmode == "Realtime" )
                                defs.TODMode = XmlSpawner.TODModeType.Realtime;
                            else if( todmode == "Gametime" )
                                defs.TODMode = XmlSpawner.TODModeType.Gametime;
                        }

                        double duration = defs.Duration.TotalMinutes;
                        try
                        {
                            duration = double.Parse( (string)dr[ "Duration" ] );
                        }
                        catch
                        {
                        }
                        defs.Duration = TimeSpan.FromMinutes( duration );

                        double despawnTime = defs.DespawnTime.TotalHours;
                        try
                        {
                            despawnTime = double.Parse( (string)dr[ "DespawnTime" ] );
                        }
                        catch
                        {
                        }
                        defs.DespawnTime = TimeSpan.FromHours( despawnTime );

                        try
                        {
                            defs.ProximityRange = int.Parse( (string)dr[ "ProximityRange" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.ProximitySound = int.Parse( (string)dr[ "ProximityTriggerSound" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.ProximityMsg = (string)dr[ "ProximityMessage" ];
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.SpeechTrigger = (string)dr[ "SpeechTrigger" ];
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.SkillTrigger = (string)dr[ "SkillTrigger" ];
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.SequentialSpawn = int.Parse( (string)dr[ "SequentialSpawn" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.KillReset = int.Parse( (string)dr[ "KillReset" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.TriggerProbability = double.Parse( (string)dr[ "TrigProb" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.AllowGhostTrig = bool.Parse( (string)dr[ "AllowGhost" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.AllowNPCTrig = bool.Parse( (string)dr[ "AllowNPC" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.SpawnOnTrigger = bool.Parse( (string)dr[ "SpawnOnTrigger" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.SmartSpawning = bool.Parse( (string)dr[ "SmartSpawn" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.ExternalTriggering = bool.Parse( (string)dr[ "ExtTrig" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.TriggerOnCarried = (string)dr[ "TrigOnCarried" ];
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.NoTriggerOnCarried = (string)dr[ "NoTrigOnCarried" ];
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.PlayerTriggerProp = (string)dr[ "PlayerTrigProp" ];
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.TriggerObjectProp = (string)dr[ "TrigObjectProp" ];
                        }
                        catch
                        {
                        }

                        try
                        {
                            defs.NameList = StringToNameList( (string)dr[ "NameList" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.SelectionList = StringToSelectionList( (string)dr[ "SelectionList" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.AddGumpX = int.Parse( (string)dr[ "AddGumpX" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.AddGumpY = int.Parse( (string)dr[ "AddGumpY" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.SpawnerGumpX = int.Parse( (string)dr[ "SpawnerGumpX" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.SpawnerGumpY = int.Parse( (string)dr[ "SpawnerGumpY" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.FindGumpX = int.Parse( (string)dr[ "FindGumpX" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.FindGumpY = int.Parse( (string)dr[ "FindGumpY" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.AutoNumber = bool.Parse( (string)dr[ "AutoNumber" ] );
                        }
                        catch
                        {
                        }
                        try
                        {
                            defs.AutoNumberValue = int.Parse( (string)dr[ "AutoNumberValue" ] );
                        }
                        catch
                        {
                        }

                        if( from != null && !from.Deleted )
                            from.SendMessage( "Loaded defs from file {0}", dirname );
                    }
                }
            }
            else
            {
                if( from != null && !from.Deleted )
                    from.SendMessage( 33, "File not found: {0}", dirname );
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register( "XmlAdd", AccessLevel.GameMaster, new CommandEventHandler( XmlAdd_OnCommand ) );
        }

        [Usage( "XmlAdd [-defaults]" )]
        [Description( "Opens a gump that can add Xmlspawners with specified default settings" )]
        public static void XmlAdd_OnCommand( CommandEventArgs e )
        {
            var acct = e.Mobile.Account as Account;
            int x = 440;
            int y = 0;
            XmlSpawnerDefaults.DefaultEntry defs = null;
            if( acct != null )
                defs = XmlSpawnerDefaults.GetDefaults( acct.ToString(), e.Mobile.Name );
            if( defs != null )
            {
                x = defs.AddGumpX;
                y = defs.AddGumpY;
            }
            // Check if there is an argument provided (load criteria)
            try
            {
                // Check if there is an argument provided (load criteria)
                for( int nxtarg = 0; nxtarg < e.Arguments.Length; nxtarg++ )
                {
                    // is it a defaults option?
                    if( e.Arguments[ nxtarg ].ToLower() == "-defaults" )
                    {
                        XmlSpawnerDefaults.RestoreDefs( defs );
                        if( defs != null )
                        {
                            x = defs.AddGumpX;
                            y = defs.AddGumpY;
                        }
                    }
                }
            }
            catch
            {
            }

            e.Mobile.SendGump( new XmlAddGump( e.Mobile, e.Mobile.Location, e.Mobile.Map, true, false, x, y ) );
        }

        public XmlAddGump( Mobile from, Point3D startloc, Map startmap )
            : this( from, startloc, startmap, true, false, StartingXoffset, 0 )
        {
        }

        public XmlAddGump( Mobile from, Point3D startloc, Map startmap, bool firststart, bool extension, int gumpx,
                          int gumpy )
            : base( gumpx, gumpy )
        {
            if( from == null || from.Deleted )
                return;


            defs = null;

            m_From = from;

            // read the text entries for default values
            var acct = from.Account as Account;
            if( acct != null )
                defs = XmlSpawnerDefaults.GetDefaults( acct.ToString(), from.Name );

            if( defs == null )
                return;

            if( firststart )
            {
                defs.StartingMap = from.Map;
                defs.StartingLoc = from.Location;
            }
            else
            {
                defs.StartingMap = startmap;
                defs.StartingLoc = startloc;
            }

            defs.IgnoreUpdate = false;
            defs.ShowExtension = extension;

            if( defs.SelectionList == null )
            {
                defs.SelectionList = new bool[ MaxEntries ];
            }
            if( defs.NameList == null )
                defs.NameList = new string[ MaxEntries ];


            // prepare the page

            AddPage( 0 );
            if( defs.ShowExtension )
            {
                AddBackground( 0, 0, 520, 500, 5054 );
                AddAlphaRegion( 0, 0, 520, 500 );
            }
            else
            {
                AddBackground( 0, 0, 200, 500, 5054 );
                AddAlphaRegion( 0, 0, 200, 500 );
            }

            int y = 3;
            int yinc = 20;
            // add the min/maxdelay entries
            AddImageTiled( 5, y, 40, 19, 0xBBC );
            AddTextEntry( 5, y, 40, 19, 0, 100, defs.MinDelay.TotalMinutes.ToString() );
            AddLabel( 45, y, 0x384, "MinDelay(m)" );

            AddImageTiled( 105, y, 40, 19, 0xBBC );
            AddTextEntry( 105, y, 40, 19, 0, 101, defs.MaxDelay.TotalMinutes.ToString() );
            AddLabel( 145, y, 0x384, "MaxDelay(m)" );

            y += yinc;
            AddImageTiled( 5, y, 40, 19, 0xBBC );
            AddTextEntry( 5, y, 40, 19, 0, 107, defs.HomeRange.ToString() );
            AddLabel( 45, y, 0x384, "HomeRng" );

            AddImageTiled( 105, y, 40, 19, 0xBBC );
            AddTextEntry( 105, y, 40, 19, 0, 108, defs.SpawnRange.ToString() );
            AddLabel( 145, y, 0x384, "SpawnRng" );

            y += yinc;
            AddImageTiled( 5, y, 40, 19, 0xBBC );
            AddTextEntry( 5, y, 40, 19, 0, 109, defs.ProximityRange.ToString() );
            AddLabel( 45, y, 0x384, "ProxRng" );

            AddImageTiled( 105, y, 40, 19, 0xBBC );
            AddTextEntry( 105, y, 40, 19, 0, 110, defs.Team.ToString() );
            AddLabel( 145, y, 0x384, "Team" );

            y += yinc;
            AddImageTiled( 5, y, 40, 19, 0xBBC );
            AddTextEntry( 5, y, 40, 19, 0, 113, defs.KillReset.ToString() );
            AddLabel( 45, y, 0x384, "KillReset" );

            AddImageTiled( 105, y, 40, 19, 0xBBC );
            AddTextEntry( 105, y, 40, 19, 0, 121, defs.TriggerProbability.ToString() );
            AddLabel( 145, y, 0x384, "TrigProb" );

            y += yinc;
            AddImageTiled( 5, y, 40, 19, 0xBBC );
            AddTextEntry( 5, y, 40, 19, 0, 111, defs.Duration.TotalMinutes.ToString() );
            AddLabel( 45, y, 0x384, "Duration(m)" );

            AddImageTiled( 105, y, 40, 19, 0xBBC );
            AddTextEntry( 105, y, 40, 19, 0, 112, defs.ProximitySound.ToString() );
            AddLabel( 145, y, 0x384, "ProxSnd" );

            y += yinc;
            AddImageTiled( 5, y, 40, 19, 0xBBC );
            AddTextEntry( 5, y, 40, 19, 0, 102, defs.RefractMin.TotalMinutes.ToString() );
            AddLabel( 45, y, 0x384, "MinRefr(m)" );

            AddImageTiled( 105, y, 40, 19, 0xBBC );
            AddTextEntry( 105, y, 40, 19, 0, 103, defs.RefractMax.TotalMinutes.ToString() );
            AddLabel( 145, y, 0x384, "MaxRefr(m)" );

            y += yinc;
            AddImageTiled( 5, y, 40, 19, 0xBBC );
            AddTextEntry( 5, y, 40, 19, 0, 104, defs.TODStart.TotalHours.ToString() );
            AddLabel( 45, y, 0x384, "TODStart(h)" );

            AddImageTiled( 105, y, 40, 19, 0xBBC );
            AddTextEntry( 105, y, 40, 19, 0, 105, defs.TODEnd.TotalHours.ToString() );
            AddLabel( 145, y, 0x384, "TODEnd(h)" );

            y += yinc;
            AddImageTiled( 5, y, 40, 19, 0xBBC );
            AddTextEntry( 5, y, 40, 19, 0, 123, defs.DespawnTime.TotalHours.ToString() );
            AddLabel( 45, y, 0x384, "Despawn(h)" );


            // AllowNPC
            AddLabel( 125, y, 0x384, "AllowNPC" );
            AddCheck( 105, y, 0xD2, 0xD3, defs.AllowNPCTrig, 312 );

            //y = 164;
            yinc = 21;
            y += yinc;
            // TOD
            if( defs.TODMode == XmlSpawner.TODModeType.Gametime )
                AddLabel( 25, y, 0x384, "GameTOD" );
            else if( defs.TODMode == XmlSpawner.TODModeType.Realtime )
                AddLabel( 25, y, 0x384, "RealTOD" );
            AddButton( 5, y, 0xD3, 0xD3, 306, GumpButtonType.Reply, 0 );


            // Sequentialspawn
            AddLabel( 125, y, 0x384, "SeqSpawn" );
            AddCheck( 105, y, 0xD2, 0xD3, ( defs.SequentialSpawn == 0 ), 307 );

            y += yinc;
            // IsGroup
            AddLabel( 25, y, 0x384, "Group" );
            AddCheck( 5, y, 0xD2, 0xD3, defs.Group, 304 );

            // HomeRangeRelative
            AddLabel( 125, y, 0x384, "HomeRngRel" );
            AddCheck( 105, y, 0xD2, 0xD3, defs.HomeRangeIsRelative, 305 );

            y += yinc;
            // smart spawning
            AddLabel( 25, y, 0x384, "SmartSpawn" );
            AddCheck( 5, y, 0xD2, 0xD3, defs.SmartSpawning, 310 );

            // AllowGhost
            AddLabel( 125, y, 0x384, "AllowGhost" );
            AddCheck( 105, y, 0xD2, 0xD3, defs.AllowGhostTrig, 309 );

            y += yinc;
            // ExtTrig
            AddLabel( 25, y, 0x384, "ExtTrig" );
            AddCheck( 5, y, 0xD2, 0xD3, defs.ExternalTriggering, 308 );

            // SpawnOnTrig
            AddLabel( 125, y, 0x384, "SpawnOnTrig" );
            AddCheck( 105, y, 0xD2, 0xD3, defs.SpawnOnTrigger, 311 );

            y += yinc;
            // AutoNumber
            AddLabel( 25, y, 0x384, "AutoNumber" );
            AddCheck( 5, y, 0xD2, 0xD3, defs.AutoNumber, 306 );
            AddImageTiled( 105, y, 80, 19, 0xBBC );
            AddTextEntry( 105, y, 80, 19, 0, 125, defs.AutoNumberValue.ToString() );

            //y = 270;
            yinc = 20;
            y += yinc;
            // Name
            AddImageTiled( 5, y, 95, 19, 0xBBC );
            AddTextEntry( 5, y, 85, 19, 0, 114, defs.SpawnerName );
            AddLabel( 105, y, 0x384, "SpawnerName" );

            y += yinc;
            // speech trigger
            AddLabel( 105, y, 0x384, "SpeechTrigger" );
            AddImageTiled( 5, y, 95, 19, 0xBBC );
            AddTextEntry( 5, y, 85, 19, 0, 106, defs.SpeechTrigger );

            y += yinc;
            // skill trigger
            AddLabel( 105, y, 0x384, "SkillTrigger" );
            AddImageTiled( 5, y, 95, 19, 0xBBC );
            AddTextEntry( 5, y, 85, 19, 0, 124, defs.SkillTrigger );

            y += yinc;
            AddImageTiled( 5, y, 95, 19, 0xBBC );
            AddTextEntry( 5, y, 85, 19, 0, 117, defs.TriggerOnCarried );
            AddLabel( 105, y, 0x384, "TrigOnCarried" );

            y += yinc;
            AddImageTiled( 5, y, 95, 19, 0xBBC );
            AddTextEntry( 5, y, 85, 19, 0, 118, defs.NoTriggerOnCarried );
            AddLabel( 105, y, 0x384, "NoTrigOnCarried" );

            y += yinc;
            AddImageTiled( 5, y, 95, 19, 0xBBC );
            AddTextEntry( 5, y, 85, 19, 0, 119, defs.ProximityMsg );
            AddLabel( 105, y, 0x384, "ProximityMsg" );

            y += yinc;
            AddImageTiled( 5, y, 95, 19, 0xBBC );
            AddTextEntry( 5, y, 85, 19, 0, 120, defs.PlayerTriggerProp );
            AddLabel( 105, y, 0x384, "PlayerTrigProp" );

            y += yinc;
            AddImageTiled( 5, y, 95, 19, 0xBBC );
            AddTextEntry( 5, y, 85, 19, 0, 122, defs.TriggerObjectProp );
            AddLabel( 105, y, 0x384, "TrigObjectProp" );

            //y = 429;
            yinc = 23;
            y += yinc;
            // add the RestoreDefs button
            AddButton( 5, y, 0xFAE, 0xFAF, 117, GumpButtonType.Reply, 0 );
            AddLabel( 35, y, 0x384, "Restore Defs" );

            // add the RestoreDefs button
            AddButton( 125, y, 0xFAE, 0xFAF, 180, GumpButtonType.Reply, 0 );
            AddLabel( 155, y, 0x384, "Options" );

            y += yinc;
            // add the SaveDefs button
            AddButton( 5, y, 0xFAE, 0xFAF, 115, GumpButtonType.Reply, 0 );
            AddLabel( 35, y, 0x384, "Save" );

            // add the LoadDefs button
            AddButton( 65, y, 0xFAE, 0xFAF, 116, GumpButtonType.Reply, 0 );
            AddLabel( 95, y, 0x384, "Load" );

            // add the DefsExt entry
            AddImageTiled( 127, y, 68, 21, 0xBBC );
            AddTextEntry( 129, y, 64, 21, 0, 115, defs.DefsExt );

            y += yinc;
            // add the Add button
            AddButton( 5, y, 0xFAE, 0xFAF, 100, GumpButtonType.Reply, 0 );
            AddLabel( 35, y, 0x384, "Add" );

            // add the Goto button
            AddButton( 64, y, 0xFAE, 0xFAF, 1000, GumpButtonType.Reply, 0 );
            AddLabel( 94, y, 0x384, "Goto" );

            // add the Delete button
            AddButton( 125, y, 0xFAE, 0xFAF, 156, GumpButtonType.Reply, 0 );
            AddLabel( 155, y, 0x384, "Del" );

            // add the Edit button
            // add the Find button

            // add gump extension button
            if( defs.ShowExtension )
            {
                AddButton( 480, y + 5, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0 );
            }
            else
            {
                AddButton( 180, y + 5, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0 );
            }

            if( defs.ShowExtension )
            {
                AddLabel( 300, 5, 0x384, "Spawn Entries" );
                // display the clear all toggle
                AddButton( 475, 5, 0xD2, 0xD3, 3999, GumpButtonType.Reply, 0 );
                // display the selection entries
                for( int i = 0; i < MaxEntries; i++ )
                {
                    int xpos = ( i / MaxEntriesPerColumn ) * 155;
                    int ypos = ( i % MaxEntriesPerColumn ) * 22 + 30;


                    // background for search results area
                    AddImageTiled( xpos + 205, ypos, 116, 23, 0x52 );

                    // has this been selected for category info specification?
                    if( i == defs.CategorySelectionIndex )
                    {
                        AddImageTiled( xpos + 206, ypos + 1, 114, 21, 0x1436 );
                    }
                    else
                        AddImageTiled( xpos + 206, ypos + 1, 114, 21, 0xBBC );

                    bool sel = false;
                    if( defs.SelectionList != null && i < defs.SelectionList.Length )
                    {
                        sel = defs.SelectionList[ i ];
                    }
                    int texthue = 0;
                    if( sel )
                        texthue = 68;
                    string namestr = null;
                    if( defs.NameList != null && i < defs.NameList.Length )
                        namestr = defs.NameList[ i ];

                    // display the name
                    //AddLabel( 280, 22 * i + 31, texthue, namestr );

                    AddTextEntry( xpos + 208, ypos + 1, 110, 21, texthue, 1000 + i, namestr );
                    // display the selection button
                    AddButton( xpos + 320, ypos + 2, ( sel ? 0xD3 : 0xD2 ), ( sel ? 0xD2 : 0xD3 ), 4000 + i,
                              GumpButtonType.Reply, 0 );
                    // display the info button
                    //AddButton( xpos + 340, ypos+2, 0x5689, 0x568A, 5000+i, GumpButtonType.Reply, 0);
                    AddButton( xpos + 340, ypos + 2, 0x15E1, 0x15E5, 5000 + i, GumpButtonType.Reply, 0 );
                }
            }
        }

        private void DoGoTo( Item x )
        {
            if( m_From == null || m_From.Deleted )
                return;
            if( x == null || x.Deleted || x.Map == null )
                return;
            Point3D itemloc;
            if( x.Parent != null )
            {
                if( x.RootParent is Container )
                {
                    itemloc = ( (Container)( x.RootParent ) ).Location;
                }
                else
                {
                    return;
                }
            }
            else
            {
                itemloc = x.Location;
            }
            m_From.Location = itemloc;
            m_From.Map = x.Map;
        }

        private void DoShowProps( Item x )
        {
            if( m_From == null || m_From.Deleted )
                return;
            if( x == null || x.Deleted || x.Map == null )
                return;
            m_From.SendGump( new PropertiesGump( m_From, x ) );
        }

        private static void DoShowGump( Mobile from, Item x )
        {
            if( from == null || from.Deleted )
                return;
            if( x == null || x.Deleted || x.Map == null || x.Map == Map.Internal )
                return;
            x.OnDoubleClick( from );
        }

        public static void Refresh( Mobile from )
        {
            Refresh( from, false );
        }

        public static void Refresh( Mobile from, bool ignoreupdate )
        {
            if( from == null )
                return;

            // read the text entries for default values
            XmlSpawnerDefaults.DefaultEntry defs = null;

            var acct = from.Account as Account;
            if( acct != null )
                defs = XmlSpawnerDefaults.GetDefaults( acct.ToString(), from.Name );

            if( defs == null )
                return;

            int x = defs.AddGumpX;
            int y = defs.AddGumpY;
            if( defs.ShowExtension )
            {
                // shift the starting point
                x = defs.AddGumpX - 140;
            }

            defs.IgnoreUpdate = ignoreupdate;

            from.CloseGump( typeof( XmlAddGump ) );

            defs.IgnoreUpdate = false;
            from.SendGump( new XmlAddGump( from, defs.StartingLoc, defs.StartingMap, false, defs.ShowExtension, x, y ) );
        }


        private class PlaceSpawnerTarget : Target
        {
            private XmlSpawnerDefaults.DefaultEntry m_Defs;
            private NetState m_State;

            public PlaceSpawnerTarget( NetState state )
                : base( 30, true, TargetFlags.None )
            {
                if( state == null || state.Mobile == null )
                    return;

                // read the text entries for default values
                m_Defs = null;

                var acct = state.Mobile.Account as Account;
                if( acct != null )
                    m_Defs = XmlSpawnerDefaults.GetDefaults( acct.ToString(), state.Mobile.Name );

                if( m_Defs == null )
                    return;

                m_State = state;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( from == null )
                    return;

                // assign it a unique id
                Guid spawnId = Guid.NewGuid();
                // count the number of entries to be added for maxcount
                int maxcount = 0;
                for( int i = 0; i < MaxEntries; i++ )
                {
                    if( m_Defs.SelectionList != null && i < m_Defs.SelectionList.Length && m_Defs.SelectionList[ i ] &&
                        m_Defs.NameList != null && i < m_Defs.NameList.Length && !string.IsNullOrEmpty( m_Defs.NameList[ i ] ) )
                    {
                        maxcount++;
                    }
                }

                // if autonumbering is enabled, name the spawner with the name+number
                string sname = m_Defs.SpawnerName;
                if( m_Defs.AutoNumber )
                {
                    sname = String.Format( "{0}#{1}", m_Defs.SpawnerName, m_Defs.AutoNumberValue );
                }

                var spawner = new XmlSpawner( spawnId, from.Location.X, from.Location.Y, 0, 0, sname, maxcount,
                                                    m_Defs.MinDelay, m_Defs.MaxDelay, m_Defs.Duration, m_Defs.ProximityRange,
                                                    m_Defs.ProximitySound, 1,
                                                    m_Defs.Team, m_Defs.HomeRange, m_Defs.HomeRangeIsRelative,
                                                    new SpawnObject[ 0 ], m_Defs.RefractMin, m_Defs.RefractMax,
                                                    m_Defs.TODStart, m_Defs.TODEnd, null, m_Defs.TriggerObjectProp,
                                                    m_Defs.ProximityMsg, m_Defs.TriggerOnCarried, m_Defs.NoTriggerOnCarried,
                                                    m_Defs.SpeechTrigger, null, null, m_Defs.PlayerTriggerProp,
                                                    m_Defs.TriggerProbability, null, m_Defs.Group, m_Defs.TODMode,
                                                    m_Defs.KillReset, m_Defs.ExternalTriggering,
                                                    m_Defs.SequentialSpawn, null, m_Defs.AllowGhostTrig, m_Defs.AllowNPCTrig,
                                                    m_Defs.SpawnOnTrigger, null, m_Defs.DespawnTime, m_Defs.SkillTrigger,
                                                    m_Defs.SmartSpawning, null );

                spawner.PlayerCreated = true;

                // if the object is a container, then place it in the container
                if( targeted is Container )
                {
                    ( (Container)targeted ).DropItem( spawner );
                }
                else
                {
                    // place the spawner at the targeted location
                    var p = targeted as IPoint3D;
                    if( p == null )
                    {
                        spawner.Delete();
                        return;
                    }
                    if( p is Item )
                        p = ( (Item)p ).GetWorldTop();

                    spawner.MoveToWorld( new Point3D( p ), from.Map );
                }

                spawner.SpawnRange = m_Defs.SpawnRange;
                // add entries from the name list
                for( int i = 0; i < MaxEntries; i++ )
                {
                    if( m_Defs.SelectionList != null && i < m_Defs.SelectionList.Length && m_Defs.SelectionList[ i ] &&
                        m_Defs.NameList != null && i < m_Defs.NameList.Length && !string.IsNullOrEmpty( m_Defs.NameList[ i ] ) )
                    {
                        spawner.AddSpawn = m_Defs.NameList[ i ];
                    }
                }

                m_Defs.LastSpawner = spawner;

                if( m_Defs.AutoNumber )
                    // bump the autonumber
                    m_Defs.AutoNumberValue++;

                //from.CloseGump(typeof(XmlAddGump));
                Refresh( m_State.Mobile, true );

                // open the spawner gump 
                DoShowGump( from, spawner );
            }
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( info == null || state == null || state.Mobile == null )
                return;

            int radiostate = -1;
            if( info.Switches.Length > 0 )
            {
                radiostate = info.Switches[ 0 ];
            }
            // read the text entries for default values
            XmlSpawnerDefaults.DefaultEntry defs = XmlSpawnerDefaults.GetDefaults( state.Account.ToString(),
                                                                                  state.Mobile.Name );
            if( defs.IgnoreUpdate )
            {
                return;
            }

            TextRelay tr = info.GetTextEntry( 100 ); // mindelay
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.MinDelay = TimeSpan.FromMinutes( double.Parse( tr.Text ) );
                }
                catch
                {
                }
            }
            tr = info.GetTextEntry( 101 ); // maxdelay info
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.MaxDelay = TimeSpan.FromMinutes( double.Parse( tr.Text ) );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 102 ); // min refractory
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.RefractMin = TimeSpan.FromMinutes( double.Parse( tr.Text ) );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 103 ); // max refractory
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.RefractMax = TimeSpan.FromMinutes( double.Parse( tr.Text ) );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 104 ); // TOD start
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.TODStart = TimeSpan.FromHours( double.Parse( tr.Text ) );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 105 ); // TOD end
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.TODEnd = TimeSpan.FromHours( double.Parse( tr.Text ) );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 106 ); // Speech trigger
            if( tr != null )
            {
                string txt = tr.Text;
                if( txt != null && txt.Length == 0 )
                    txt = null;
                defs.SpeechTrigger = txt;
            }


            tr = info.GetTextEntry( 107 ); // HomeRange
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.HomeRange = int.Parse( tr.Text );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 108 ); // SpawnRange
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.SpawnRange = int.Parse( tr.Text );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 109 ); // ProximityRange
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.ProximityRange = int.Parse( tr.Text );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 110 ); // Team
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.Team = int.Parse( tr.Text );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 111 ); // Duration
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.Duration = TimeSpan.FromMinutes( double.Parse( tr.Text ) );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 112 ); // ProximitySound
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.ProximitySound = int.Parse( tr.Text );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 113 ); // Kill reset
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.KillReset = int.Parse( tr.Text );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 114 ); // Spawner name
            if( tr != null )
                defs.SpawnerName = tr.Text;

            // DefsExt entry
            tr = info.GetTextEntry( 115 ); // save def str
            if( tr != null )
                defs.DefsExt = tr.Text;

            tr = info.GetTextEntry( 117 ); // trigger on carried
            if( tr != null )
            {
                string txt = tr.Text;
                if( txt != null && txt.Length == 0 )
                    txt = null;
                defs.TriggerOnCarried = txt;
            }

            tr = info.GetTextEntry( 118 ); // no trigger on carried
            if( tr != null )
            {
                string txt = tr.Text;
                if( txt != null && txt.Length == 0 )
                    txt = null;
                defs.NoTriggerOnCarried = txt;
            }

            tr = info.GetTextEntry( 119 ); // proximity message
            if( tr != null )
            {
                string txt = tr.Text;
                if( txt != null && txt.Length == 0 )
                    txt = null;
                defs.ProximityMsg = txt;
            }

            tr = info.GetTextEntry( 120 ); // player trig prop
            if( tr != null )
            {
                string txt = tr.Text;
                if( txt != null && txt.Length == 0 )
                    txt = null;
                defs.PlayerTriggerProp = txt;
            }

            tr = info.GetTextEntry( 121 ); // Trigger probability
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.TriggerProbability = double.Parse( tr.Text );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 122 ); // trig object prop
            if( tr != null )
            {
                string txt = tr.Text;
                if( txt != null && txt.Length == 0 )
                    txt = null;
                defs.TriggerObjectProp = txt;
            }

            tr = info.GetTextEntry( 123 ); // DespawnTime
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.DespawnTime = TimeSpan.FromHours( double.Parse( tr.Text ) );
                }
                catch
                {
                }
            }

            tr = info.GetTextEntry( 124 ); // Skill trigger
            if( tr != null )
            {
                string txt = tr.Text;
                if( txt != null && txt.Length == 0 )
                    txt = null;
                defs.SkillTrigger = txt;
            }

            tr = info.GetTextEntry( 125 ); // AutoNumberValue
            if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
            {
                try
                {
                    defs.AutoNumberValue = int.Parse( tr.Text );
                }
                catch
                {
                }
            }


            // fill the NameList from the text entries
            if( defs.ShowExtension )
                for( int i = 0; i < MaxEntries; i++ )
                {
                    tr = info.GetTextEntry( 1000 + i );
                    if( defs.NameList != null && i < defs.NameList.Length && tr != null )
                    {
                        defs.NameList[ i ] = tr.Text;
                    }
                }

            defs.Group = info.IsSwitched( 304 );
            defs.HomeRangeIsRelative = info.IsSwitched( 305 );
            defs.AutoNumber = info.IsSwitched( 306 );
            defs.SequentialSpawn = ( info.IsSwitched( 307 ) ? 0 : -1 );
            defs.ExternalTriggering = info.IsSwitched( 308 );
            defs.AllowGhostTrig = info.IsSwitched( 309 );
            defs.SpawnOnTrigger = info.IsSwitched( 311 );
            defs.SmartSpawning = info.IsSwitched( 310 );
            defs.AllowNPCTrig = info.IsSwitched( 312 );

            switch( info.ButtonID )
            {
                case 0: // Close
                    {
                        return;
                    }
                case 100: // Add spawner
                    {
                        state.Mobile.Target = new PlaceSpawnerTarget( state );

                        break;
                    }
                case 115: // SaveDefs
                    {
                        string filename;
                        if( !string.IsNullOrEmpty( defs.DefsExt ) )
                        {
                            filename = String.Format( "{0}-{1}-{2}", defs.AccountName, defs.PlayerName, defs.DefsExt );
                        }
                        else
                        {
                            filename = String.Format( "{0}-{1}", defs.AccountName, defs.PlayerName );
                        }
                        DoSaveDefs( state.Mobile, filename );
                        break;
                    }
                case 116: // LoadDefs
                    {
                        string filename;
                        if( !string.IsNullOrEmpty( defs.DefsExt ) )
                        {
                            filename = String.Format( "{0}-{1}-{2}", defs.AccountName, defs.PlayerName, defs.DefsExt );
                        }
                        else
                        {
                            filename = String.Format( "{0}-{1}", defs.AccountName, defs.PlayerName );
                        }
                        DoLoadDefs( state.Mobile, filename );
                        break;
                    }
                case 117: // Restore Defaults
                    {
                        state.Mobile.SendMessage( String.Format( "Restoring defaults" ) );
                        XmlSpawnerDefaults.RestoreDefs( defs );
                        break;
                    }
                case 155: // Return the player to the starting loc
                    {
                        m_From.Location = defs.StartingLoc;
                        m_From.Map = defs.StartingMap;
                        break;
                    }
                case 156: // Delete last spawner
                    {
                        if( defs.LastSpawner == null || defs.LastSpawner.Deleted )
                            break;
                        Refresh( state.Mobile );
                        state.Mobile.SendGump( new XmlAddConfirmDeleteGump( state.Mobile, defs.LastSpawner ) );
                        return;
                    }
                case 157: // Reset last spawner
                    {
                        if( defs.LastSpawner != null && !defs.LastSpawner.Deleted )
                            defs.LastSpawner.DoReset = true;
                        break;
                    }
                case 158: // Respawn last spawner
                    {
                        if( defs.LastSpawner != null && !defs.LastSpawner.Deleted )
                            defs.LastSpawner.DoRespawn = true;
                        break;
                    }
                case 180: // Set Options
                    {
                        Refresh( state.Mobile );
                        state.Mobile.SendGump( new XmlAddOptionsGump( state.Mobile, defs.LastSpawner ) );
                        return;
                    }
                case 200: // gump extension
                    {
                        defs.ShowExtension = !defs.ShowExtension;
                        break;
                    }
                case 306: // TOD mode
                    {
                        if( defs.TODMode == XmlSpawner.TODModeType.Realtime )
                            defs.TODMode = XmlSpawner.TODModeType.Gametime;
                        else
                            defs.TODMode = XmlSpawner.TODModeType.Realtime;
                        break;
                    }
                case 1000: // GoTo
                    {
                        // then go to it
                        DoGoTo( defs.LastSpawner );
                        break;
                    }
                case 1001: // Show Gump
                    {
                        Refresh( state.Mobile );
                        DoShowGump( state.Mobile, defs.LastSpawner );
                        break;
                    }
                case 1002: // Show Props
                    {
                        Refresh( state.Mobile );
                        DoShowProps( defs.LastSpawner );
                        break;
                    }
                case 3999: // clear selections
                    {
                        // clear the selections
                        if( defs.SelectionList != null )
                            Array.Clear( defs.SelectionList, 0, defs.SelectionList.Length );
                        break;
                    }
                case 9998: // refresh the gump
                    {
                        break;
                    }
                default:
                    {
                        if( info.ButtonID >= 4000 && info.ButtonID < 4000 + MaxEntries )
                        {
                            int i = info.ButtonID - 4000;
                            if( defs.SelectionList != null && i >= 0 && i < defs.SelectionList.Length )
                            {
                                defs.SelectionList[ i ] = !defs.SelectionList[ i ];
                            }
                        }
                        if( info.ButtonID >= 5000 && info.ButtonID < 5000 + MaxEntries )
                        {
                            int i = info.ButtonID - 5000;
                            /*
						string match = null;
						if(defs.NameList[i] != null)
						{
							match = defs.NameList[i].Trim();
						}
						*/

                            defs.CategorySelectionIndex = i;
                            var newg = new XmlAddGump( state.Mobile, defs.StartingLoc, defs.StartingMap, false,
                                                             defs.ShowExtension, 0, 0 );

                            state.Mobile.SendGump( newg );

                            if( string.IsNullOrEmpty( defs.NameList[ i ] ) )
                            {
                                // if no string has been entered then just use the full categorized add gump
                                state.Mobile.CloseGump( typeof( XmlCategorizedAddGump ) );
                                state.Mobile.SendGump( new XmlCategorizedAddGump( state.Mobile, defs.CurrentCategory,
                                                                                defs.CurrentCategoryPage, i, newg ) );
                            }
                            else
                            {
                                // use the XmlPartialCategorizedAddGump
                                state.Mobile.CloseGump( typeof( XmlPartialCategorizedAddGump ) );

                                //Type [] types = (Type[])XmlPartialCategorizedAddGump.Match( defs.NameList[i] ).ToArray( typeof( Type ) );
                                List<SearchEntry> types = XmlPartialCategorizedAddGump.Match( defs.NameList[ i ] );
                                state.Mobile.SendGump( new XmlPartialCategorizedAddGump( state.Mobile, defs.NameList[ i ], 0,
                                                                                       types, true, i, newg ) );
                            }
                            return;
                            /*
						if(match == null || match.Length == 0)
						{
							state.Mobile.SendGump( new Server.Gumps.XmlCategorizedAddGump( state.Mobile, i, this ) );
						} else
						{
							state.Mobile.SendGump( new Server.Gumps.XmlLookupCategorizedAddGump( state.Mobile, match, 0,
							(Type[])Server.Gumps.AddGump.Match( match ).ToArray( typeof( Type ) ), true ) );
						}
						*/
                        }
                        break;
                    }
            }
            // Create a new gump
            //m_Spawner.OnDoubleClick( state.Mobile);
            Refresh( state.Mobile );
        }

        private class XmlAddOptionsGump : Gump
        {
            private Mobile m_From;
            private XmlSpawner m_LastSpawner;

            public XmlAddOptionsGump( Mobile from, XmlSpawner lastSpawner )
                : base( 0, 0 )
            {
                // read the text entries for default values
                // read the text entries for default values
                var acct = from.Account as Account;
                XmlSpawnerDefaults.DefaultEntry defs = null;
                if( acct != null )
                {
                    defs = XmlSpawnerDefaults.GetDefaults( acct.ToString(), from.Name );
                }

                if( defs == null )
                    return;

                m_From = from;
                m_LastSpawner = lastSpawner;
                Closable = true;
                Dragable = true;
                AddPage( 0 );
                AddBackground( 0, 0, 300, 130, 5054 );


                AddLabel( 20, 5, 0, String.Format( "Options" ) );
                // add the AddGumpX/Y entries
                AddImageTiled( 5, 30, 40, 21, 0xBBC );
                AddTextEntry( 5, 30, 40, 21, 0, 100, defs.AddGumpX.ToString() );
                AddLabel( 45, 30, 0x384, "AddGumpX" );

                AddImageTiled( 135, 30, 40, 21, 0xBBC );
                AddTextEntry( 135, 30, 40, 21, 0, 101, defs.AddGumpY.ToString() );
                AddLabel( 175, 30, 0x384, "AddGumpY" );

                // add the SpawnerGumpX/Y entries
                AddImageTiled( 5, 55, 40, 21, 0xBBC );
                AddTextEntry( 5, 55, 40, 21, 0, 102, defs.SpawnerGumpX.ToString() );
                AddLabel( 45, 55, 0x384, "SpawnerGumpX" );

                AddImageTiled( 135, 55, 40, 21, 0xBBC );
                AddTextEntry( 135, 55, 40, 21, 0, 103, defs.SpawnerGumpY.ToString() );
                AddLabel( 175, 55, 0x384, "SpawnerGumpY" );

                // add the FindGumpX/Y entries
                AddImageTiled( 5, 80, 40, 21, 0xBBC );
                AddTextEntry( 5, 80, 40, 21, 0, 104, defs.FindGumpX.ToString() );
                AddLabel( 45, 80, 0x384, "FindGumpX" );

                AddImageTiled( 135, 80, 40, 21, 0xBBC );
                AddTextEntry( 135, 80, 40, 21, 0, 105, defs.FindGumpY.ToString() );
                AddLabel( 175, 80, 0x384, "FindGumpY" );
            }

            public override void OnResponse( NetState state, RelayInfo info )
            {
                if( info == null || state == null || state.Mobile == null )
                    return;

                // read the text entries for default values
                XmlSpawnerDefaults.DefaultEntry defs = XmlSpawnerDefaults.GetDefaults( state.Account.ToString(),
                                                                                      state.Mobile.Name );
                if( defs == null )
                    return;

                TextRelay tr = info.GetTextEntry( 100 ); // AddGumpX
                if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
                {
                    try
                    {
                        defs.AddGumpX = int.Parse( tr.Text );
                    }
                    catch
                    {
                    }
                }
                tr = info.GetTextEntry( 101 ); // AddGumpY info
                if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
                {
                    try
                    {
                        defs.AddGumpY = int.Parse( tr.Text );
                    }
                    catch
                    {
                    }
                }
                tr = info.GetTextEntry( 102 ); // SpawnerGumpX info
                if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
                {
                    try
                    {
                        defs.SpawnerGumpX = int.Parse( tr.Text );
                    }
                    catch
                    {
                    }
                }
                tr = info.GetTextEntry( 103 ); // SpawnerGumpY info
                if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
                {
                    try
                    {
                        defs.SpawnerGumpY = int.Parse( tr.Text );
                    }
                    catch
                    {
                    }
                }
                tr = info.GetTextEntry( 104 ); // FindGumpX info
                if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
                {
                    try
                    {
                        defs.FindGumpX = int.Parse( tr.Text );
                    }
                    catch
                    {
                    }
                }
                tr = info.GetTextEntry( 105 ); // FindGumpY info
                if( tr != null && !string.IsNullOrEmpty( tr.Text ) )
                {
                    try
                    {
                        defs.FindGumpY = int.Parse( tr.Text );
                    }
                    catch
                    {
                    }
                }
            }
        }

        private class XmlAddConfirmDeleteGump : Gump
        {
            private Mobile m_From;
            private XmlSpawner m_LastSpawner;

            public XmlAddConfirmDeleteGump( Mobile from, XmlSpawner lastSpawner )
                : base( 0, 0 )
            {
                m_From = from;
                m_LastSpawner = lastSpawner;
                Closable = false;
                Dragable = true;
                AddPage( 0 );
                AddBackground( 10, 200, 200, 130, 5054 );


                AddLabel( 20, 225, 33, String.Format( "Delete Last Spawner?" ) );
                AddRadio( 35, 255, 9721, 0x86A, false, 1 ); // accept/yes radio
                AddRadio( 135, 255, 9721, 0x86A, true, 2 ); // decline/no radio
                AddHtmlLocalized( 72, 255, 200, 30, 1049016, 0x7fff, false, false ); // Yes
                AddHtmlLocalized( 172, 255, 200, 30, 1049017, 0x7fff, false, false ); // No
                AddButton( 80, 289, 2130, 2129, 3, GumpButtonType.Reply, 0 ); // Okay button
            }

            public override void OnResponse( NetState state, RelayInfo info )
            {
                if( info == null || state == null || state.Mobile == null )
                    return;

                int radiostate = -1;
                if( info.Switches.Length > 0 )
                {
                    radiostate = info.Switches[ 0 ];
                }
                switch( info.ButtonID )
                {
                    default:
                        {
                            if( radiostate == 1 && m_LastSpawner != null && !m_LastSpawner.Deleted )
                            {
                                // accept
                                // delete it
                                m_LastSpawner.Delete();
                            }
                            break;
                        }
                }
            }
        }
    }
}