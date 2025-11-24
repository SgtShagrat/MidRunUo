/***************************************************************************
 *                               ItemExports.cs
 *                            -------------------
 *   begin                : lunedì 13 ottobre 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Server;
using Server.Commands;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Targeting;

namespace Midgard.Engines
{
    public class ItemExportSystem
    {
        /// <summary>
        /// this structure is used to store collections of serial #'s that have been updated
        /// this way, when all items are imported, the parent references can be updated appropriately
        /// </summary>
        private sealed class SerialUpdater
        {
            public int OldSerial;
            public int NewSerial;
        }

        /// <summary>
        /// this structure holds data to be used to externally store item heirarchy so that 
        /// items within containers can be properly rebuilt when importing
        /// </summary>
        private sealed class ContainerInfo
        {
            public int ParentSerial;
            public int X;
            public int Y;
        }

        /// <summary>
        /// this is the object used to load/save the items, and it carries with it the
        /// ContainerInfo class so all items have the associated info needed to propertly 
        /// restore any container heirarchy
        /// </summary>
        private sealed class ItemInfo
        {
            public Item m_Item;
            public ContainerInfo m_Info;

            public ItemInfo( Item item, ContainerInfo info )
            {
                m_Item = item;
                m_Info = info;
            }
        }

        private static bool Debug = false;

        private static string m_TdbPath;
        private static string m_IdxPath;
        private static string m_BinPath;

        private static GenericWriter m_IdxWriter, m_TdbWriter, m_BinWriter;

        private static string FilePath { get; set; }

        private ItemExportSystem()
            : this( "ExportedItems" )
        {
        }

        private ItemExportSystem( string filePath )
        {
            FilePath = filePath;
        }

        private static Item Read( string filename )
        {
            if( !Directory.Exists( FilePath ) )
            {
                Console.WriteLine( "Item import failed - directory does not exist." );
                return null;
            }

            if( filename == null )
                filename = "exportitems";

            m_IdxPath = Path.Combine( FilePath, filename + ".idx" );
            m_TdbPath = Path.Combine( FilePath, filename + ".tdb" );
            m_BinPath = Path.Combine( FilePath, filename + ".bin" );

            //here's the bigass hashtable that will be storing all items imported,
            //and will be used to place them back in their appropriate containers
            List<ItemInfo> importItems = new List<ItemInfo>();

            //looks like these two will need to come along into the DAO.  they're
            //used for instance generation it seems
            object[] ctorArgs = new object[ 1 ];
            Type[] ctorTypes = new Type[] { typeof( Serial ) };

            if( File.Exists( m_IdxPath ) && File.Exists( m_TdbPath ) && File.Exists( m_BinPath ) )
            {
                //this is the collection of all updates to imported items' serial #'s
                //which will be needed to keep track of the final resting place
                //of each item once they're all done loading
                List<SerialUpdater> serialUpdates = new List<SerialUpdater>();

                //open the index file
                //hide this filestream as per DAO standards
                using( FileStream idx = new FileStream( m_IdxPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
                {
                    //here's where the idxReader is set up
                    BinaryReader idxReader = new BinaryReader( idx );

                    using( FileStream tdb = new FileStream( m_TdbPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
                    {
                        //then the type library
                        BinaryReader tdbReader = new BinaryReader( tdb );

                        //and now the binary serial data reader
                        BinaryFileReader binReader = new BinaryFileReader( new BinaryReader( new FileStream( m_BinPath, FileMode.Open, FileAccess.Read, FileShare.Read ) ) );

                        int count = tdbReader.ReadInt32();

                        ArrayList types = new ArrayList( count );

                        //load all types, and make sure they exist in the world
                        for( int i = 0; i < count; ++i )
                        {
                            string typeName = tdbReader.ReadString();

                            //check if the loaded type exists
                            Type t = ScriptCompiler.FindTypeByFullName( typeName );

                            //if not, we have a problem
                            if( t == null )
                            {
                                Console.WriteLine( "failed" );
                                Console.WriteLine( "Error: Type '{0}' was not found. Cannot import" );

                                return null;
                            }

                            //this checks for constructor info in the class type
                            //the item is requesting to use
                            ConstructorInfo ctor = t.GetConstructor( ctorTypes );

                            if( ctor != null )
                            {
                                types.Add( new object[] { ctor, typeName } );
                            }
                            else
                            {
                                Console.WriteLine( String.Format( "Type '{0}' does not have a serialization constructor", t ) );
                            }
                        }

                        //get the item count from the index file
                        int itemCount = idxReader.ReadInt32();

                        for( int i = 0; i < itemCount; ++i )
                        {
                            //read in the header info for each item in the index file
                            int typeID = idxReader.ReadInt32();

                            //this will need some manipulation in the DAO
                            int serial = idxReader.ReadInt32();

                            //prepare a package for container info for the ImportItems class
                            ContainerInfo ci = new ContainerInfo();

                            //read in the parent serial #
                            ci.ParentSerial = idxReader.ReadInt32();

                            //we don't worry about moving items into their containers yet
                            //wait for the entire load to occur first, then do it.							
                            if( ci.ParentSerial != -1 )
                            {
                                ci.X = idxReader.ReadInt32();
                                ci.Y = idxReader.ReadInt32();
                            }

                            //now, need to load index info for this item, so we can
                            //begin to invoke deserialization from the binary file
                            long pos = idxReader.ReadInt64();
                            int length = idxReader.ReadInt32();

                            //ok, here's the new serial handler
                            //if the item serial has already been taken (most likely a yes)
                            if( World.FindItem( serial ) != null )
                            {
                                //this records the change from old serial to new serial
                                SerialUpdater su = new SerialUpdater();

                                su.OldSerial = serial;				//save the old serial number for record keeping
                                su.NewSerial = Serial.NewItem;		//get a unique serial number from the world
                                serialUpdates.Add( su );

                                if( Debug )
                                    Console.WriteLine( String.Format( "Converting from serial {0:X} to {1:X}", su.OldSerial, su.NewSerial ) );

                                serial = su.NewSerial;				//save this new number to be used to create the item
                            }

                            object[] objs = (object[])types[ typeID ];

                            //looks like this is where it ignores items with no type.  just leaves them behind to rot
                            if( objs == null )
                                continue;

                            //here's the birth of the item
                            Item item = null;
                            ConstructorInfo ctor = (ConstructorInfo)objs[ 0 ];
                            string typeName = (string)objs[ 1 ];

                            try
                            {
                                //this connects up the serialization constructor or something
                                ctorArgs[ 0 ] = (Serial)serial;
                                item = (Item)( ctor.Invoke( ctorArgs ) );
                            }
                            catch
                            {
                            }

                            //if we've got a heartbeat, then we can go on
                            if( item != null )
                            {
                                //put the item in the hashtable of imported items
                                importItems.Add( new ItemInfo( item, ci ) );
                            }

                            //here's where we line up for item deserialization

                            //seek to the location in the file
                            binReader.Seek( pos, SeekOrigin.Begin );

                            try
                            {
                                //and here is the real loading part
                                if( item != null )
                                    item.Deserialize( binReader );

                                if( binReader.Position != ( pos + length ) )
                                {
                                    Console.WriteLine( "Error importing from files - checksum error in binary file when deserializing" );
                                    binReader.Close();
                                    tdbReader.Close();
                                    idxReader.Close();
                                    return null;
                                }
                            }
                            catch( Exception e )
                            {
                                Console.WriteLine( "Error importing from files - " + e.Message );
                                binReader.Close();
                                tdbReader.Close();
                                idxReader.Close();
                                return null;
                            }
                        }

                        binReader.Close();
                        tdbReader.Close();
                    }

                    idxReader.Close();
                }

                //and now, put the items in the world ( is this needed? )
                foreach( ItemInfo id in importItems )
                {
                    Item item = id.m_Item;

                    World.AddItem( item );
                }

                //this is the master item to return to the method that called this
                Item returnItem = null;

                if( Debug )
                {
                    Console.WriteLine( String.Format( "# of items that required serial # change: {0}", serialUpdates.Count ) );
                }

                //and next, clean up all parent serial info with the updated serial #'s
                foreach( ItemInfo id in importItems )
                {
                    Item item = id.m_Item;
                    ContainerInfo ci = id.m_Info;

                    if( ci.ParentSerial > -1 )
                    {
                        if( Debug )
                            Console.WriteLine( String.Format( "Serial # {0} with serial #{1:X} as parent", item.Serial, ci.ParentSerial ) );

                        foreach( SerialUpdater su in serialUpdates )
                        {
                            //if this item's parent has undergone a serial reindexing
                            if( ci.ParentSerial == su.OldSerial )
                            {
                                //update it
                                if( Debug )
                                    Console.WriteLine( String.Format( "Updating parent for serial # {0} from serial #{1:X} to serial #{2:X}",
                                                                      item.Serial, ci.ParentSerial, su.NewSerial ) );

                                ci.ParentSerial = su.NewSerial;
                                break;
                            }
                        }
                    }
                    else
                    {
                        returnItem = item;
                    }
                }

                //finally, restructure items back into their proper places
                RestoreItems( importItems );

                return returnItem;
            }
            else
            {
                Console.WriteLine( "Import failed - files missing" );
                return null;
            }
        }

        private static bool Write( string filename, Item exportitem )
        {
            return Write( filename, exportitem, true, false );
        }

        private static bool Write( string filename, Item exportitem, bool haltOnNoDir, bool haltOnDir )
        {
            if( !Directory.Exists( FilePath ) )
            {
                //if the call requests a halt on no directory found
                if( haltOnNoDir )
                    return false;

                Directory.CreateDirectory( FilePath );
            }
            else
            {
                //if the call requests a halt on directory found
                if( haltOnDir )
                    return false;
            }

            if( filename == null )
                filename = "exportitems";

            m_IdxPath = Path.Combine( FilePath, filename + ".idx" );
            m_TdbPath = Path.Combine( FilePath, filename + ".tdb" );
            m_BinPath = Path.Combine( FilePath, filename + ".bin" );

            //set up the file streams for writing
            try
            {
                m_IdxWriter = new BinaryFileWriter( m_IdxPath, false );
                m_TdbWriter = new BinaryFileWriter( m_TdbPath, false );
                m_BinWriter = new BinaryFileWriter( m_BinPath, true );
            }
            catch
            {
                Console.WriteLine( "Error opening files to commence export" );
                return false;
            }

            //new hashtable which holds info about all the items to be exported, and
            //its corresponding 
            List<ItemInfo> exportItems = new List<ItemInfo>();

            //keys: the items to export
            //values: ContainerInfo class, that holds:
            //	-the serial # of the item's parent
            //	-the x-y coordinates of the item with the parent container

            //ok, so the code recursively finds all items in the object to export, 
            //adds them to the hashtable, and puts the items' parents as their keys.
            //Then, on each item, it moves them out of the containers and onto the 
            //world itself. This is done to remove any association of items, so that
            //they do not retain any linkage to each other.  If they did, then when
            //they are imported and are reassigned new serial numbers, things get ugly.
            //here's where the items are recursively catalogued, and their heirarchy
            //is stored, and they are recursively removed from the container

            exportItems = RecurseCatalogue( exportitem );

            if( Debug )
                Console.WriteLine( String.Format( "Found {0} to export", exportItems.Count ) );

            //park the item during this export process, and remove all 
            //heirarchy of parents and children for container info
            foreach( ItemInfo id in exportItems )
            {
                Item item = id.m_Item;
                item.MoveToWorld( new Point3D( 1, 1, 0 ), Map.Internal );
            }

            //now, begin writing
            m_IdxWriter.Write( exportItems.Count );

            for( int i = 0; i < exportItems.Count; i++ )
            {
                ItemInfo id = exportItems[ i ];
                Item item = id.m_Item;

                //this header info is written to the index file, so that the
                //load process knows where to find each item

                //the header info has been modified from the standard world
                //save format.  Included in the header is the serial # of the item's
                //parent, along with the x-y position of the item within the parent

                //This is done since re-indexing of serial #'s are inevitable, and 
                //as each item is loaded, the loaded parent info could point to an old
                //serial number which is no longer valid.  So, to remove this problem,
                //each item has their container heirarchy stored externally in the
                //index file, and the item is removed from its parent during the 
                //serialization process so no parent info is saved to the .bin file.

                //Then, during the import, the item's old indexed parent serial # is
                //compared to a list of all other imported items' serial numbers which
                //had to be updated, and when a match is found, the item is placed
                //back in that container

                long start = m_BinWriter.Position;

                m_IdxWriter.Write( ( (ISerializable)item ).TypeReference ); //this is the item type serial #
                m_IdxWriter.Write( item.Serial );

                if( Debug )
                    Console.WriteLine( String.Format( "exporting item {0}", item.Serial ) );

                ContainerInfo ci = id.m_Info;

                m_IdxWriter.Write( ci.ParentSerial );

                //if there is container info to write
                if( ci.ParentSerial != -1 )
                {
                    m_IdxWriter.Write( ci.X );	//item's container x-position
                    m_IdxWriter.Write( ci.Y );	//item's container y-position
                }

                m_IdxWriter.Write( start );

                //fires off the item save code
                if( Debug )
                    Console.WriteLine( "Serializing item to binary file" );

                item.Serialize( m_BinWriter );

                //TODO: restore all item's parent heirarchy after this is over

                //write the length of the item record into the index file
                m_IdxWriter.Write( (int)( m_BinWriter.Position - start ) );
            }

            //Restore the item container heirarchy to the game proper, or deleted
            RestoreItems( exportItems );

            //writes to the type serial # file the names of all the item types
            //this will be used to check to make sure the world that tries to import
            //the item can support this item.  All item types are saved

            m_TdbWriter.Write( World.ItemTypes.Count );
            for( int i = 0; i < World.ItemTypes.Count; ++i )
                m_TdbWriter.Write( World.ItemTypes[ i ].FullName );

            m_IdxWriter.Close();
            m_TdbWriter.Close();
            m_BinWriter.Close();

            return true;
        }

        private static List<ItemInfo> RecurseCatalogue( Item item )
        {
            return RecurseCatalogue( item, false );
        }

        private static List<ItemInfo> RecurseCatalogue( Item item, bool subitem )
        {
            List<ItemInfo> list = new List<ItemInfo>();

            //add this item to the list
            ContainerInfo ci = new ContainerInfo();

            //only care if it's in a container, not a mobile
            if( item.Parent != null && item.Parent is Item && subitem )
            {
                Item parentItem = (Item)item.Parent;
                ci.ParentSerial = parentItem.Serial.Value;
                ci.X = item.X;
                ci.Y = item.Y;
            }
            else
            {
                //flag that this isn't in a container, or that it's the parent item
                ci.ParentSerial = -1;
                ci.X = -1;
                ci.Y = -1;
            }

            list.Add( new ItemInfo( item, ci ) );

            foreach( Item i in item.Items )
            {
                try
                {
                    ArrayList atts = XmlAttach.FindAttachments( i );

                    if( atts != null )
                    {
                        for( int j = 0; j < atts.Count; j++ )
                        {
                            XmlAttachment a = atts[ j ] as XmlAttachment;
                            if( a == null )
                                continue;

                            Console.WriteLine( "Warning: removing attachment of type {0} from object {1}", a.GetType().Name, i.Serial.ToString() );
                            a.Delete();
                        }
                    }
                }
                catch( Exception e )
                {
                    Console.WriteLine( e );
                }

                //recursive search for items within items
                List<ItemInfo> recurselist = RecurseCatalogue( i, true );

                //this recursively sews up all the lists found for each item
                //if the item does not contain items, recurselist will only
                //have one entry, that being the item itself
                foreach( ItemInfo di in recurselist )
                    list.Add( di );
            }

            return list;
        }

        private static void RestoreItems( IEnumerable<ItemInfo> list )
        {
            foreach( ItemInfo info in list )
            {
                Item item = info.m_Item;
                ContainerInfo ci = info.m_Info;

                Serial serial = ci.ParentSerial;

                object parent;

                //get the actual item from the world
                if( serial.IsMobile )
                    parent = World.FindMobile( serial );
                else if( serial.IsItem )
                    parent = World.FindItem( serial );
                else
                    parent = null;

                //if there is a parent, and its an item
                if( parent != null && parent is Item && parent is Container )
                {
                    Container container = (Container)parent;

                    if( Debug )
                        Console.WriteLine( String.Format( "Moving item {0} into container {1}", item.Serial, container.Serial ) );

                    //drop this item back in the parent
                    container.DropItem( item );

                    //and move the item to where it used to sit
                    item.X = ci.X;
                    item.Y = ci.Y;
                }
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register( "ImportItem", AccessLevel.Administrator, new CommandEventHandler( OnImportItem ) );
            CommandSystem.Register( "ExportItem", AccessLevel.Administrator, new CommandEventHandler( OnExportItem ) );
        }

        [Usage( "ImportItem <filename>" )]
        [Description( "Imports an item or items from specified file." )]
        private static void OnImportItem( CommandEventArgs e )
        {
            if( e.Arguments.Length > 0 )
            {
                try
                {
                    e.Mobile.Target = new ImportItemTarget( e.GetString( 0 ) );
                    e.Mobile.SendMessage( "Target the location to import the item(s)" );
                }
                catch
                {
                    e.Mobile.SendMessage( "An error occurred. Bad path detected." );
                }
            }
        }

        [Usage( "ExportItem <filename>" )]
        [Description( "Exports an item or items to binary files.  Can be imported using ImportItem <filename>" )]
        private static void OnExportItem( CommandEventArgs e )
        {
            if( e.Arguments.Length > 0 )
            {
                try
                {
                    e.Mobile.Target = new ExportItemTarget( e.GetString( 0 ) );
                    e.Mobile.SendMessage( "Target the location to export the item(s)" );
                }
                catch
                {
                    e.Mobile.SendMessage( "An error occurred. Bad path detected." );
                }
            }
        }

        private sealed class ImportItemTarget : Target
        {
            private string m_Filename;

            public ImportItemTarget( string filename )
                : base( -1, true, TargetFlags.None )
            {
                m_Filename = filename;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                Point3D targ = new Point3D( 0, 0, 0 );

                if( targeted is IPoint3D )
                {
                    IPoint3D p = targeted as IPoint3D;
                    targ = new Point3D( p.X, p.Y, p.Z );
                }
                else
                {
                    from.SendMessage( "Can't build that there." );
                    return;
                }

                Item imported = Import( m_Filename );

                if( imported != null )
                {
                    imported.MoveToWorld( targ, from.Map );
                    from.SendMessage( "Import successful" );
                }
                else
                    from.SendMessage( "Import failed" );
            }

            private static Item Import( string filename )
            {
                //Use item data access object 
                ItemExportSystem exportSystem = new ItemExportSystem(); //use default file path

                //perform the read operation using the ItemExportSystem
                try
                {
                    return Read( filename );
                }
                catch
                {
                }

                return null;
            }
        }

        private sealed class ExportItemTarget : Target
        {
            private string m_Filename;

            public ExportItemTarget( string filename )
                : base( -1, true, TargetFlags.None )
            {
                m_Filename = filename;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( !( targeted is Item ) )
                {
                    from.SendMessage( "This only works on items." );
                    return;
                }

                if( targeted is AddonComponent || targeted is BaseAddon )
                {
                    from.SendMessage( "This does not work on addons." );
                    return;
                }

                if( Export( (Item)targeted, m_Filename ) )
                    from.SendMessage( "Export successful" );
                else
                    from.SendMessage( "Export failed..." );
            }

            private static bool Export( Item targitem, string filename )
            {
                ItemExportSystem system = new ItemExportSystem(); //use default file path

                bool success = false;

                try
                {
                    Point3D oldpoint = new Point3D( targitem.X, targitem.Y, targitem.Z );
                    Object oldparent = targitem.Parent;
                    Map oldmap = targitem.Map;

                    success = Write( filename, targitem );

                    if( success )
                    {
                        if( oldparent is Container )
                        {
                            Container container = (Container)oldparent;
                            container.DropItem( targitem );
                            targitem.Location = oldpoint;
                        }
                        else if( oldparent is Mobile )
                        {
                            Mobile mob = (Mobile)oldparent;
                            mob.AddItem( targitem );
                        }
                        else
                            targitem.MoveToWorld( oldpoint, oldmap );
                    }
                }
                catch
                {
                }

                return success;
            }
        }
    }
}