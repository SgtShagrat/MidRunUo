/*
Files sovrascritti:
 	Scripts\Commands\Generic\Commands\BaseCommand.cs
	Scripts\Commands\Generic\Commands\Commands.cs
	Scripts\Commands\Generic\Commands\Interface.cs
	Scripts\Commands\Generic\Implementors\AreaCommandImplementor.cs
	Scripts\Commands\Generic\Implementors\BaseCommandImplementor.cs
	Scripts\Commands\Generic\Implementors\ContainedCommandImplementor.cs
	Scripts\Commands\Generic\Implementors\GlobalCommandImplementor.cs
	Scripts\Commands\Generic\Implementors\MultiCommandImplementor.cs
	Scripts\Commands\Generic\Implementors\SerialCommandImplementor.cs
	Scripts\Commands\Generic\Implementors\SingleCommandImplementor.cs
	Scripts\Commands\Decorate.cs
	Scripts\Commands\GenTeleporter.cs
	Scripts\Commands\SignParser.cs
	Scripts\Commands\Wipe.cs
	Scripts\Items\Misc\PublicMoongate.cs
	Scripts\Misc\DoorGenerator.cs
	Scripts\Misc\uoamVendors.cs
*/

using System;
using System.Collections;
using System.IO;
using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
using Server.Multis;

namespace Midgard.Engines.GroupsHandler
{
    public class GroupsHandler
    {
        public static readonly string GroupsListPath = Path.Combine( Path.Combine( "Saves", "World" ), "GroupsList.bin" );

        private static Hashtable m_GroupsTable = new Hashtable();

        public static Hashtable GroupsTable
        {
            get { return m_GroupsTable; }
        }

        public const AccessLevel SecureLevel = AccessLevel.Administrator;
        public const AccessLevel BaseLevel = AccessLevel.GameMaster;

        public static ItemsGroup DefaultGroup, DecorationGroup, DoorGenGroup, MoonGenGroup, TelGenGroup, SignGenGroup, UOAMVendorsGroup;

        private static BaseCommandImplementor m_CommandImplementor = new ListCommandImplementor();

        public static BaseCommandImplementor CommandImplementor
        {
            get { return m_CommandImplementor; }
        }

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "HG", AccessLevel.Counselor, new CommandEventHandler( HandleGroups_OnCommand ) );
            CommandSystem.Register( "HandleGroups", AccessLevel.Counselor, new CommandEventHandler( HandleGroups_OnCommand ) );
        }

        [Usage( "HandleGroups" )]
        [Aliases( "HG" )]
        [Description( "Opens a gump which allows to manage or delete all items in groups (example, items of quests that need to be easily deleted later)." )]
        public static void HandleGroups_OnCommand( CommandEventArgs arg )
        {
            Mobile from = arg.Mobile;
            ArrayList list = ListItems( from, null );

            from.CloseGump( typeof( GroupsGump ) );
            from.SendGump( new GroupsGump( from, list, null ) );
        }

        public class SetGroupCommand : BaseCommand
        {
            public override bool UseGroupsSecurity
            {
                get { return true; }
            }

            public SetGroupCommand()
            {
                AccessLevel = AccessLevel.GameMaster;
                Supports = CommandSupport.Simple;
                Commands = new string[] { "SetGroup" };
                ObjectTypes = ObjectTypes.Items;
                Usage = "SetGroup";
                Description = "Adds an item to a specified group (example, items of quests that need to be easily deleted later).";
                ListOptimized = false;
            }

            public override void Execute( CommandEventArgs e, object obj )
            {
                Item item = obj as Item;
                if( item == null )
                    return;

                ItemsGroup group = GetGroup( e.GetString( 0 ) ) ?? DefaultGroup;

                if( !InGroup( item ) )
                {
                    if( group.AddItem( CheckAddon( item ), e.Mobile.AccessLevel >= SecureLevel ) )
                        AddResponse( String.Format( "The item \"{0}\" [{1}] has been added to the group {2}.", item.GetType().Name, item.Serial,
                                                  group.Name ) );
                    else
                        LogFailure( "That cannot be added to the group." );
                }
                else
                {
                    LogFailure( "That item is already in a group." );
                }
            }
        }

        public class ToggleGroupCommand : BaseCommand
        {
            public override bool UseGroupsSecurity
            {
                get { return true; }
            }

            private const int AutoSelectLimit = 10000;

            public ToggleGroupCommand()
            {
                AccessLevel = AccessLevel.GameMaster;
                Supports = CommandSupport.AllItems & ~CommandSupport.Multi;
                Commands = new string[] { "ToggleGroup", "TG" };
                ObjectTypes = ObjectTypes.Items;
                Usage = "ToggleGroup";
                Description =
                    "Opens a gump which allows to add, remove or manage the targeted items in a group (example, items of quests that need to be easily deleted later).";
                ListOptimized = true;
            }

            public override bool ValidateArgs( BaseCommandImplementor impl, CommandEventArgs e )
            {
                if( impl.SupportRequirement == CommandSupport.Single && e.Length == 1 )
                {
                    Mobile from = e.Mobile;
                    Item item = World.FindItem( e.GetInt32( 0 ) );
                    item = CheckAddon( item );

                    if( item == null )
                        from.SendMessage( "No object with that serial was found." );
                    else if( !IsAccessible( from, item ) )
                        from.SendMessage( "That is not accessible." );
                    else
                    {
                        ArrayList list = new ArrayList();
                        ArrayList rads = new ArrayList();

                        if( !IsInvalid( item ) )
                        {
                            list.Add( item );

                            if( !InGroup( item ) )
                                rads.Add( item );
                        }

                        from.CloseGump( typeof( GroupsGump ) );
                        from.SendGump( new GroupsGump( from, list, rads ) );
                    }

                    return false;
                }

                return true;
            }

            public override void ExecuteList( CommandEventArgs e, ArrayList list )
            {
                Hashtable table = new Hashtable(); // for addons list
                ArrayList list2 = new ArrayList();
                ArrayList rads = new ArrayList();

                bool overLimit = list.Count > AutoSelectLimit;

                foreach( Item item in list )
                {
                    Item i = CheckAddon( item );

                    if( IsInvalid( i ) )
                        continue;

                    if( i is IAddon )
                    {
                        if( table.Contains( i ) )
                            continue;

                        table.Add( i, null );
                    }

                    list2.Add( i );

                    if( !overLimit )
                    {
                        ItemsGroup group = GetGroup( i );

                        if( group == null )
                            rads.Add( i );
                    }
                }

                list.Clear();
                list.TrimToSize();
                table.Clear();

                if( list2.Count > 0 )
                {
                    e.Mobile.CloseGump( typeof( GroupsGump ) );
                    e.Mobile.SendGump( new GroupsGump( e.Mobile, list2, rads ) );
                }
                else
                {
                    AddResponse( "No matching objects found." );
                }
            }
        }

        internal static void ConfigSystem()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( Load );
            EventSink.WorldSave += new WorldSaveEventHandler( Save );
        }

        public static void Save( WorldSaveEventArgs e )
        {
            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                string dir = Path.Combine( Path.GetPathRoot( GroupsListPath ), Path.GetDirectoryName( GroupsListPath ) );
                if( !Directory.Exists( dir ) )
                    Directory.CreateDirectory( dir );

                BinaryFileWriter writer = new BinaryFileWriter( GroupsListPath, true );

                IDictionaryEnumerator groups = m_GroupsTable.GetEnumerator();
                while( groups.MoveNext() )
                {
                    ItemsGroup group = groups.Value as ItemsGroup;
                    if( group != null && group.Items.Count > 0 )
                    {
                        writer.Write( group.Name );
                        writer.Write( group.Description );
                        writer.Write( group.Secure );

                        group.Save( writer );
                    }
                }

                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch
            {
                Console.WriteLine( "Error writing Groups List." );
            }
        }

        public static void Load()
        {
            while( !File.Exists( GroupsListPath ) )
            {
                Console.WriteLine( "Warning: {0} not found.", GroupsListPath );
                Console.WriteLine( " - Press return to continue, or R to try again." );
                string str = Console.ReadLine();
                if( str == null || str.ToLower() != "r" )
                {
                    return;
                }
            }

            try
            {
                BinaryReader bReader = new BinaryReader( File.OpenRead( GroupsListPath ) );
                BinaryFileReader reader = new BinaryFileReader( bReader );

                while( !reader.End() )
                {
                    string name = reader.ReadString();
                    string description = reader.ReadString();
                    bool secure = reader.ReadBool();

                    ItemsGroup group = AddGroup( name, description, secure );
                    group.Load( reader );
                }

                bReader.Close();
            }
            catch
            {
                Console.WriteLine( "Error loading Groups List." );
                //m_GroupsTable.Clear();
            }
        }

        internal static void CreateDefaults()
        {
            DefaultGroup = AddGroup( "(default)", "Default group", false );
            DecorationGroup = AddGroup( "decorate", "Default world decorations (Decorate)", true );
            DoorGenGroup = AddGroup( "doorgen", "Default world doors (DoorGen)", true );
            MoonGenGroup = AddGroup( "moongen", "Default world moongates (MoonGen)", true );
            TelGenGroup = AddGroup( "telgen", "Default world teleporters (TelGen)", true );
            SignGenGroup = AddGroup( "signgen", "Default world signs (SignGen)", true );
            UOAMVendorsGroup = AddGroup( "uoamvendors", "Default world vendor spawners (uoamVendors)", true );
        }

        public static ItemsGroup AddGroup( string name, string description, bool secure )
        {
            if( string.IsNullOrEmpty( name ) )
                return null;

            ItemsGroup group;

            if( m_GroupsTable.ContainsKey( name ) )
            {
                group = m_GroupsTable[ name ] as ItemsGroup;
                if( group != null )
                    if( secure || !group.Secure )
                        group.Description = description;
            }
            else
            {
                group = new ItemsGroup( name, description, secure );
                m_GroupsTable[ group.Name ] = group;
            }

            return group;
        }

        public static ItemsGroup GetGroup( Item item )
        {
            if( item != null )
            {
                IDictionaryEnumerator groups = m_GroupsTable.GetEnumerator();
                while( groups.MoveNext() )
                {
                    ItemsGroup group = groups.Value as ItemsGroup;
                    if( group != null )
                        if( group.Contains( CheckAddon( item ) ) )
                            return group;
                }
            }

            return null;
        }

        public static ItemsGroup GetGroup( string name )
        {
            name = name.ToLower();
            if( name != null && m_GroupsTable.ContainsKey( name ) )
                return m_GroupsTable[ name ] as ItemsGroup;

            return null;
        }

        public static bool InGroup( Item item )
        {
            return ( GetGroup( item ) != null );
        }

        public static bool InSecureGroup( Item item )
        {
            ItemsGroup group = GetGroup( item );
            return ( group != null && group.Secure );
        }

        public static bool IsAccessible( Mobile from, Item item )
        {
            if( from.AccessLevel >= SecureLevel )
                return true;

            ItemsGroup group = GetGroup( item );

            return group == null || group.IsAccessible( false );
        }

        public static bool IsInvalid( Item item )
        {
            return ( item == null || item.Deleted || ( item is AddonComponent && ( (AddonComponent)item ).Addon != null ) || item is BaseMulti ||
                    item is HouseSign || item is HouseTeleporter || item is BaseHouseDoor || item is Corpse );
        }

        public static Item CheckAddon( Item item )
        {
            if( item != null && item is AddonComponent && ( (AddonComponent)item ).Addon != null )
                item = ( (AddonComponent)item ).Addon;

            return item;
        }

        public static int DeleteItems( Mobile from, ArrayList list )
        {
            if( list == null )
                return 0;

            bool secure = from.AccessLevel >= SecureLevel;

            int removed = 0;
            foreach( Item item in list )
            {
                if( item == null || item.Deleted )
                    continue;

                ItemsGroup group = GetGroup( item );

                if( group == null || ( group.RemoveItem( item, secure ) ) )
                {
                    CommandLogging.WriteLine( from, "{0} {1} deleting {2}", from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( item ) );
                    item.Delete();
                    removed++;
                }
            }

            list.Clear();

            return removed;
        }

        public static ArrayList ListItems( Mobile from, ArrayList list )
        {
            bool secure = from.AccessLevel >= SecureLevel;
            ArrayList items = new ArrayList();

            if( list == null )
            {
                ArrayList groups = new ArrayList( GroupsTable.Values );
                groups.Sort( ItemsGroupComparer.Instance );
                foreach( ItemsGroup group in groups )
                {
                    if( group != null && !group.IsAccessible( secure ) )
                        continue;

                    if( group != null )
                        items.AddRange( RemoveInvalid( group.Items, false ) );
                }
            }
            else
            {
                foreach( Item item in list )
                {
                    if( IsInvalid( item ) )
                        continue;

                    if( IsAccessible( from, item ) )
                        items.Add( item );
                }
            }

            return items;
        }

        public static ArrayList RemoveInvalid( ArrayList list, bool listOnly )
        {
            if( list == null )
                return null;

            for( int i = list.Count - 1; i >= 0; i-- )
            {
                Item item = list[ i ] as Item;

                if( IsInvalid( item ) )
                {
                    list.RemoveAt( i );
                    if( !listOnly )
                    {
                        ItemsGroup group = GetGroup( item );
                        if( group != null )
                            group.RemoveItem( item, true );
                    }
                }
            }

            return list;
        }
    }
}