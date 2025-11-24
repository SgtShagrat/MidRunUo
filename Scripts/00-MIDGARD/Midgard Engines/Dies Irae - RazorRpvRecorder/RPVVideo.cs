using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Xml.Linq;

using Midgard.Misc;

using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.RazorRpvRecorder
{
    internal class RPVVideo
    {
        private readonly string m_FileName;
        private readonly List<Serial> m_Involvedmobiles = new List<Serial>();
        private readonly IPAddress m_LocalHost = IPAddress.Parse( "127.0.0.1" );
        private readonly string m_Name;
        private readonly RecorderMobile m_Owner;
        private readonly string m_ShardName;
        private byte[] m_Buffer;
        private GZBlockOut m_GZBlockOut;
        private XDocument m_Infodoc;
        private DateTime m_LastTime;
        private DateTime m_StartTime;

        public RPVVideo( RecorderMobile owner, IEnumerable<XElement> customElements )
            : this( owner, FormatPath( owner ), customElements )
        {
        }

        private RPVVideo( RecorderMobile owner, string fileName, IEnumerable<XElement> customElements )
        {
            m_Owner = owner;
            m_FileName = fileName;
            m_ShardName = ServerList.ServerName;
            CustomInfo = customElements;
            m_StartTime = DateTime.Now;
            m_Name = m_Owner.Name ?? "";
        }

        public IEnumerable<XElement> CustomInfo { get; private set; }

        /// <summary>
        ///     Get the current timestamp
        /// </summary>
        private static string GetTimeStamp()
        {
            DateTime now = DateTime.Now;
            return String.Format( "{0}-{1}-{2} {3}-{4:D2}-{5:D2}", now.Day, now.Month, now.Year, now.Hour, now.Minute, now.Second );
        }

        /// <summary>
        ///     Format the file name we want to record to
        /// </summary>
        private static string FormatFileName()
        {
            return String.Format( "{0}.rpv", GetTimeStamp() );
        }

        /// <summary>
        ///     Set the current save path for the recording owner
        /// </summary>
        private static string FormatPath( IEntity owner )
        {
            string path = Path.Combine( Core.BaseDirectory, Config.StoragePath );
            string fullpath = null;

            //creatre unique id for a file.
            while( string.IsNullOrEmpty( fullpath ) || File.Exists( fullpath ) )
                fullpath = Path.Combine( path, FormatFileName() );

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Storage fullpath: {0}", fullpath );

            //create filename in order to lock it
            File.WriteAllBytes( fullpath, new byte[ 0 ] ); //empty file 

            return fullpath;
#if false
            string name = string.Format( "0x{0:X}", owner.Serial.Value );

            AppendPath( ref path, Config.StoragePath );
            AppendPath( ref path, name );
            path = Path.Combine( path, FormatFileName() );

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Storage path: {0}", path );

            return path;
#endif
        }

        /// <summary>
        ///     Appent the toAppend path to path
        /// </summary>
        private static void AppendPath( ref string path, string toAppend )
        {
            path = Path.Combine( path, toAppend );

            if( !Directory.Exists( path ) )
                Directory.CreateDirectory( path );
        }

        /// <summary>
        ///     Start the video recording
        /// </summary>
        internal void Start()
        {
            Init();

            Timer.DelayCall( TimeSpan.FromSeconds( 1 ), new TimerCallback( CycleSecondForcePacket ) );

            try
            {
                m_GZBlockOut = new GZBlockOut( m_FileName, 0x800 );
            }
            catch( Exception exception )
            {
                Config.Pkg.LogErrorLine( string.Format( "Error Saving RPV: {0}", exception ) );
                return;
            }

            m_StartTime = m_LastTime = DateTime.Now;

            WriteHeader();
        }

        private void Init()
        {
            lock( m_Involvedmobiles )
                m_Involvedmobiles.Clear();

            m_Infodoc = new XDocument( new XDeclaration( "1.0", "utf-8", "yes" ) );
            m_Infodoc.Add( new XElement( "video", new XAttribute( "creation", DateTime.Now.Ticks ),
                                       new XElement( "recorder",
                                                    new XAttribute( "serial", m_Owner.Serial ),
                                                    new XAttribute( "name", m_Owner.Name ),
                                                    new XAttribute( "region", m_Owner.Region.Name ),
                                                    new XAttribute( "location", m_Owner.Location.X + "," + m_Owner.Location.Y + "," + m_Owner.Location.Z ) ),
                                       new XElement( "involvedmobiles" ),
                                       new XElement( "custom", CustomInfo )
                              ) );
        }

        /// <summary>
        ///     register a fake packet... :-) to sincronixe video timer
        /// </summary>
        private void CycleSecondForcePacket()
        {
            if( DateTime.Now.Subtract( m_LastTime ).TotalSeconds > 1.0 && m_Owner.Recording )
            {
                PingAck p = new PingAck( 0 );
                int len;
                byte[] buff = p.RawUncompressedBuffer( out len );
                WritePacket( buff, len );
            }

            if( m_Owner.Recording )
                Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( CycleSecondForcePacket ) );
        }

        /// <summary>
        ///     Write the header of the rpv video into the GK strem
        /// </summary>
        private void WriteHeader()
        {
            lock( m_GZBlockOut )
            {
                m_GZBlockOut.Raw.Write( (byte)4 );
                m_GZBlockOut.Raw.Seek( 0x10, SeekOrigin.Current );

                // Write the start time of our video
                m_GZBlockOut.Raw.Write( m_StartTime.ToFileTime() );

                // Write the duration placeholder of our video
                m_GZBlockOut.Raw.Write( 0 );

                m_GZBlockOut.BufferAll = true;

                m_GZBlockOut.Compressed.Write( m_Name );
                m_GZBlockOut.Compressed.Write( m_ShardName );

                byte[] addressBytes = m_LocalHost.GetAddressBytes();
                m_GZBlockOut.Compressed.Write( addressBytes, 0, 4 );

                SaveWorldState();

                m_GZBlockOut.BufferAll = false;
                m_GZBlockOut.Flush();
            }
        }

        /// <summary>
        ///     Serialize the recorder nearby items, mobiles and other infos
        /// </summary>
        private void SaveWorldState()
        {
            lock( m_GZBlockOut )
            {
                long position = m_GZBlockOut.Position;

                // In this list we will save all items that must be serialized
                List<Item> items = new List<Item>();

                // Items that must be destroyed after serialization (Hair, Beard...)
                List<Item> tempItems = new List<Item>();

                // in this int we will write the number of items saved
                m_GZBlockOut.Compressed.Write( 0 );

                // Save our recorder data state
                SaveMobileState( m_Owner, items, tempItems, m_GZBlockOut.Compressed );

                // Save nearby mobiles data (NB: they are never PlayerMobile for razor!)
                foreach( Mobile m in m_Owner.GetMobilesInRange( Config.RecordingRange ) )
                {
                    if( m != m_Owner && !m.Hidden )
                        WriteMobile( m, items, tempItems );
                }

                // Save nearby item data, with no hidden mobile parent
                foreach( Item item in m_Owner.GetItemsInRange( Config.RecordingRange ) )
                {
                    if( !( item.Parent is Item ) && ( item.Parent is Mobile ) && !( (Mobile)item.Parent ).Hidden )
                        items.Add( item );
                }

                // Save all items we cached
                foreach( Item item in items )
                    WriteItem( item );

                // Remove items created during serialization
                for( int i = 0; i < tempItems.Count; i++ )
                {
                    Item item = tempItems[ i ];

                    if( item != null && !item.Deleted )
                        item.Delete();
                }

                // Write the number of items saved before their serialization
                long num2 = m_GZBlockOut.Position;
                m_GZBlockOut.Seek( (int)position, SeekOrigin.Begin );
                m_GZBlockOut.Compressed.Write( (int)( num2 - ( position + 4L ) ) );
                m_GZBlockOut.Seek( 0L, SeekOrigin.End );
            }
        }

        /// <summary>
        ///     Filter the buffer data we 'would' to send to rpv stream according to Razor PacketPlayer protocol
        /// </summary>
        /// <returns>return true if the buffer is sent to the stream</returns>
        internal bool ServerPacket( Packet packet )
        {
            int length = 0;
            byte[] buffer = packet.RawUncompressedBuffer( out length );

            if( length > 0 && buffer != null && length <= buffer.Length )
            {
                using( Stream stream = new MemoryStream( buffer ) )
                using( BinaryReader reader = new BinaryReader( stream ) )
                {
                    int packetId = buffer[ 0 ];
                    if( m_Owner == null )
                    {
                        Stop();
                        return true;
                    }
                    if( packet is MobileIncomingOld )
                    {
                        MobileIncomingOld incold = (MobileIncomingOld)packet;
                        AddInvolvedMobile( incold.m_Beheld );
                    }

                    switch( packetId )
                    {
                        //Register( 0x34,  10,  true, new OnPacketReceive( MobileQuery ) );					
                        case 0x6c:
                        case 0x7c:
                        case 0x21:
                        case 0x27:
                        case 0x88:
                        case 0xb2:
                        case 0xba:
                            if( Config.Debug )
                                Config.Pkg.LogWarningLine( "Skip packed with id: " + packetId );
                            return true;

                        #region Unused Razor Packet
                        /*
                            case 0x22:
                                {
                                    // The rpv recorder does not move
                                    byte seq = p.ReadByte();
                                    if( World.Player.HasWalkEntry( seq ) )
                                    {
                                        WritePacket( new ForceWalk( World.Player.GetMoveEntry( seq ).Dir & Direction.Up ) );
                                    }
                                    return true;
                                }
                            case 0xbf:
                                {
                                    // ExtendedCommand
                                    switch( p.ReadInt16() )
                                    {
                                        case 6:
                                            switch( p.ReadByte() )
                                            {
                                                case 3:
                                                case 4:
                                                    {
                                                        Mobile mobile = World.FindMobile( p.ReadUInt32() );
                                                        string str = p.ReadUnicodeStringSafe();
                                                        string text = string.Format( "[{0}]: {1}", ( ( ( mobile != null ) && ( mobile.Name != null ) ) && ( mobile.Name.Length > 0 ) ) ? mobile.Name : "Party", str );
                                                        WritePacket( new UnicodeMessage( Serial.MinusOne, 0, MessageType.System, 0x3b2, 3, "ENU", "Party", text ) );
                                                        break;
                                                    }
                                            }
                                            return true;

                                        case 0x1d:
                                            {
                                                Item item = World.FindItem( p.ReadUInt32() );
                                                if( item != null )
                                                {
                                                    item.HouseRevision = p.ReadInt32();
                                                    if( m_HouseDataWritten[ item.Serial ] == null )
                                                    {
                                                        if( item.HousePacket == null )
                                                        {
                                                            item.MakeHousePacket();
                                                        }
                                                        if( item.HousePacket != null )
                                                        {
                                                            m_HouseDataWritten[ item.Serial ] = true;
                                                            WritePacket( new Packet( item.HousePacket, item.HousePacket.Length, true ) );
                                                            return true;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 0xd8:
                                {
                                    p.ReadByte();
                                    p.ReadByte();
                                    Serial serial = p.ReadUInt32();
                                    m_HouseDataWritten[ serial ] = true;
                                    break;
                                }
                            */
                        #endregion
                    }
                }

                WritePacket( buffer, length );
            }
            return true;
        }

        private void AddInvolvedMobile( Mobile mobile )
        {
            lock( m_Involvedmobiles )
            {
                if( mobile != m_Owner && !mobile.Hidden && !m_Involvedmobiles.Contains( mobile.Serial ) )
                {
                    m_Involvedmobiles.Add( mobile.Serial );
                    if( Config.Debug )
                        Config.Pkg.LogInfoLine( "AddInvolvedMobile {0}", mobile );
                }
            }
        }

        /// <summary>
        ///     Serialize an Item instance to our video stream
        /// </summary>
        /// <param name = "item">runuo item we want to serialize</param>
        private void WriteItem( Item item )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( " Complete Item: [{0}]", item );

            lock( m_GZBlockOut )
            {
                m_GZBlockOut.Compressed.Write( (byte)0 );
                SaveItemState( item, m_GZBlockOut.Compressed );
            }
        }

        /// <summary>
        ///     Serialize a Mobile instance to our video stream
        /// </summary>
        /// <param name = "m">runuo mobile we want to serialize</param>
        /// <param name = "owneditems">the mobile owned items</param>
        /// <param name = "tempItems">the items which will be deleted after serialization</param>
        private void WriteMobile( Mobile m, ICollection<Item> owneditems, ICollection<Item> tempItems )
        {
            lock( m_GZBlockOut )
            {
                m_GZBlockOut.Compressed.Write( (byte)1 );
                SaveMobileState( m, owneditems, tempItems, m_GZBlockOut.Compressed );
            }
        }

        /// <summary>
        ///     Write down a packet to the video stream
        /// </summary>
        private void WritePacket( byte[] buffer, int length )
        {
            // Evaluate the time span from last packet write
            int totalMilliseconds = (int)DateTime.Now.Subtract( m_LastTime ).TotalMilliseconds;

            m_LastTime = DateTime.Now;

            m_GZBlockOut.Compressed.Write( totalMilliseconds );

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "WritePacket: {0} bytes", length );

            m_GZBlockOut.Compressed.Write( buffer, 0, length );
        }

        /// <summary>
        ///     Stop the video recording and format the file closure
        /// </summary>
        internal void Stop ()
        {
        	if (Config.Debug)
        		Config.Pkg.LogInfo ("Stop recording...");

            lock (m_GZBlockOut)
            {
        		TimeSpan span = DateTime.Now.Subtract (m_LastTime);
        		m_GZBlockOut.Compressed.Write ((int)span.TotalMilliseconds);

                // Write termination char 0xff
        		m_GZBlockOut.Compressed.Write ((byte)0xff);

                m_GZBlockOut.ForceFlush ();
        		m_GZBlockOut.BufferAll = true;

                // Update video duration
        		m_GZBlockOut.RawStream.Seek (0x19L, SeekOrigin.Begin);
        		span = DateTime.Now - m_StartTime;
        		m_GZBlockOut.Raw.Write ((int)span.TotalMilliseconds);

                // Write down the hash of the video at 8 bytes from beginning
        		m_GZBlockOut.RawStream.Seek (0x11L, SeekOrigin.Begin);
        		using (MD5 md = MD5.Create ())
        			m_Buffer = md.ComputeHash (m_GZBlockOut.RawStream);
        		m_GZBlockOut.RawStream.Seek (1L, SeekOrigin.Begin);
        		m_GZBlockOut.Raw.Write (m_Buffer);

                // Flush and close the stream
        		m_GZBlockOut.RawStream.Flush ();
        		m_GZBlockOut.Close ();
        		m_GZBlockOut = null;

                //saveinvolvedmobiles
        		XElement invelem = m_Infodoc.Element ("video").Element ("involvedmobiles");

                foreach (Serial mobser in m_Involvedmobiles)
                {
        			Mobile mobile = World.FindEntity (mobser) as Mobile;
        			if (mobile != null)
        				invelem.Add (new XElement ("mobile", new XAttribute ("serial", mobser), new XAttribute ("name", mobile.Name), new XAttribute ("account", mobile.Account != null ? mobile.Account.Username : "")));
        		}

                m_Infodoc.Element ("video").Add (new XAttribute ("file", Path.GetFileName (m_FileName)));
        		var hash = MD5Helper.CalculateMD5Hash (m_FileName);
        		m_Infodoc.Element ("video").Add (new XAttribute ("hash", hash));
        		m_Infodoc.Element ("video").Add (new XAttribute ("duration", span.Ticks));
    
                m_Infodoc.Save (Path.Combine (Path.GetDirectoryName (m_FileName), Path.GetFileNameWithoutExtension (m_FileName) + ".xml"));
    
				//rename to hashone :-)
        		var oldname = Path.Combine (Path.GetDirectoryName (m_FileName), Path.GetFileNameWithoutExtension (m_FileName));
        		var newname = Path.Combine (Path.GetDirectoryName (m_FileName), hash);
        		File.Move (oldname + ".rpv" , newname + ".rpv");
				File.Move (oldname + ".xml" , newname + ".xml");

                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "done." );
            }
        }

        /// <summary>
        ///     Get the runuo flags for our item
        /// </summary>
        private static byte GetPacketFlags( Item item )
        {
            byte num = 0;

            if( !item.Visible )
                num = (byte)( num | 0x80 );

            if( item.Movable )
                num = (byte)( num | 0x20 );

            return num;
        }

        /// <summary>
        ///     Get the runuo flags for our mobile
        /// </summary>
        private static int GetPacketFlags( Mobile mobile )
        {
            int num = 0;

            if( mobile.Female )
                num |= 2;
            if( mobile.Poisoned )
                num |= 4;
            if( mobile.Blessed )
                num |= 8;
            if( mobile.Warmode )
                num |= 0x40;
            if( mobile.Hidden )
                num |= 0x80;

            return num;
        }

        /// <summary>
        ///     Serialize with writer the mobile data
        /// </summary>
        private void SaveMobileState( Mobile mobile, ICollection<Item> ownedItems, ICollection<Item> tempItems, BinaryWriter writer )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "  Mobile: [{0}]", mobile );

            AddInvolvedMobile( mobile );

            #region from UOEntityData.SaveState()
            writer.Write( (uint)mobile.Serial.Value );
            writer.Write( mobile.Location.X );
            writer.Write( mobile.Location.Y );
            writer.Write( mobile.Location.Z );
            writer.Write( (ushort)mobile.Hue );
            #endregion

            #region from Mobile.SaveState()
            writer.Write( (ushort)( ( mobile != m_Owner ) ? mobile.Body.BodyID : Config.RecorderBody ) );
            writer.Write( (byte)mobile.Direction );
            writer.Write( mobile.Name ?? "" );
            writer.Write( (byte)Notoriety.Compute( mobile, mobile ) ); // is it right?
            writer.Write( (byte)GetPacketFlags( mobile ) );
            writer.Write( (ushort)mobile.HitsMax );
            writer.Write( (ushort)mobile.Hits );
            writer.Write( (byte)mobile.Map.MapID ); // is it right?

            List<Item> allitems = new List<Item>( mobile.Items );

            // hair must be saved apart...
            if( mobile.HairItemID != 0 )
            {
                Hair hair = new GenericHair( mobile.HairItemID );
                tempItems.Add( hair );
                allitems.Add( hair );
            }

            // even beard must be saved apart...
            if( mobile.FacialHairItemID != 0 )
            {
                Beard beard = new GenericBeard( mobile.FacialHairItemID );
                tempItems.Add( beard );
                allitems.Add( beard );
            }

            writer.Write( allitems.Count );
            foreach( Item item in allitems )
            {
                writer.Write( (uint)item.Serial.Value );

                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "  - ItemRef: [{0}]", item );

                ownedItems.Add( item );
            }
            #endregion

            #region from PlayerData.SaveState()
            if( mobile != m_Owner )
                return;

            writer.Write( (ushort)mobile.Str );
            writer.Write( (ushort)mobile.Dex );
            writer.Write( (ushort)mobile.Int );
            writer.Write( (ushort)mobile.StamMax );
            writer.Write( (ushort)mobile.Stam );
            writer.Write( (ushort)mobile.ManaMax );
            writer.Write( (ushort)mobile.Mana );
            writer.Write( (byte)mobile.StrLock );
            writer.Write( (byte)mobile.DexLock );
            writer.Write( (byte)mobile.IntLock );
            writer.Write( (uint)mobile.TotalGold );
            writer.Write( (ushort)mobile.TotalWeight );
            writer.Write( (byte)mobile.Skills.Length );

            for( int i = 0; i < mobile.Skills.Length; i++ )
            {
                writer.Write( (ushort)mobile.Skills[ i ].BaseFixedPoint );
                writer.Write( (ushort)mobile.Skills[ i ].CapFixedPoint );
                writer.Write( (ushort)mobile.Skills[ i ].Fixed );
                writer.Write( (byte)mobile.Skills[ i ].Lock );
            }

            writer.Write( (ushort)mobile.ArmorRating );
            writer.Write( (ushort)mobile.StatCap );
            writer.Write( (byte)mobile.Followers );
            writer.Write( (byte)mobile.FollowersMax );
            writer.Write( mobile.TithingPoints );
            writer.Write( (sbyte)mobile.LightLevel );
            writer.Write( (byte)LightCycle.ComputeLevelFor( mobile as PlayerMobile ) );

            if( mobile.NetState == null )
            {
                Config.Pkg.LogWarning( "Warning: netstate is null in RPVVideo." );
                writer.Write( (ushort)0 );
            }
            else
                writer.Write( (ushort)mobile.NetState.Flags /*m_Features*/);

            writer.Write( (byte)mobile.Map.Season /*mobile.Season*/);

            // Don't know about map patches
            writer.Write( (byte)0 );

            //writer.Write((byte) mobile.MapPatches.Length);
            //for (int j = 0; j < mobile.MapPatches.Length; j++)
            //{
            //    writer.Write((int)mobile.MapPatches[j]);
            //}
            #endregion
        }

        /// <summary>
        ///     Serialize with writer the item data
        /// </summary>
        private static void SaveItemState( Item item, BinaryWriter writer )
        {
            #region from UOEntityData.SaveState()
            writer.Write( (uint)item.Serial.Value );
            writer.Write( item.Location.X );
            writer.Write( item.Location.Y );
            writer.Write( item.Location.Z );
            writer.Write( (ushort)item.Hue );
            #endregion

            #region from Item.SaveState()
            writer.Write( (ushort)item.ItemID );
            writer.Write( (ushort)item.Amount );
            writer.Write( (byte)item.Direction );
            writer.Write( GetPacketFlags( item ) );
            writer.Write( (byte)item.Layer );
            writer.Write( item.Name ?? "" );

            if( item.Parent is IEntity )
                writer.Write( (uint)( (IEntity)item.Parent ).Serial.Value );
            else if( item.Parent is Serial )
                writer.Write( (uint)( (Serial)item.Parent ).Value );
            else
                writer.Write( (uint)0 );

            writer.Write( 0 );

            // We do not support house packet

            //if( ( m_HouseRev != 0 ) && ( m_HousePacket == null ) )
            //    MakeHousePacket( item );

            //if( ( m_HouseRev != 0 ) && ( m_HousePacket != null ) )
            //{
            //    writer.Write( m_HouseRev );
            //    writer.Write( (ushort)m_HousePacket.Length );
            //    writer.Write( m_HousePacket );
            //}
            //else
            writer.Write( 0 );
            #endregion
        }
    }
}