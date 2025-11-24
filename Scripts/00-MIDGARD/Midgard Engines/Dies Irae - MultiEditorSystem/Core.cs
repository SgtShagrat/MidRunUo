/***************************************************************************
 *                                     Core.cs
 *                            		-------------------
 *  begin                	: Agosto, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 *          Command class for multi management.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server;
using Server.Commands;
using Server.Items;
using Server.Targeting;

namespace Midgard.Engines.MultiEditor
{
    public static class Core
    {
        #region template
        private const string m_Template = @"using Server.Multis.Deeds;

namespace Server.Multis
{
    public class House_{0} : BaseHouse
    {
        // Area is {4}
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { {1} };

        public House_{0}( Mobile owner )
            : base( {2}, owner, 86, 4 )
        {
            uint keyValue = CreateKeys( owner );

{3}
        }

        public override int DefaultPrice
        {
            get { return 1; }
        }

        public override Rectangle2D[] Area
        {
            get { return AreaArray; }
        }

        public override Point3D BaseBanLocation
        {
            get { return new Point3D( {7} ); }
        }

        public override HouseDeed GetDeed()
        {
            return new House_{0}Deed();
        }

        #region serialization
        public House_{0}( Serial serial )
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

    public class House_{0}Deed : HouseDeed
    {
        [Constructable]
        public House_{0}Deed()
            : base( {0}, new Point3D( 0, {6}, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return {5}; }
        }

        public override Rectangle2D[] Area
        {
            get { return House_{0}.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new House_{0}( owner );
        }

        #region serialization
        public House_{0}Deed( Serial serial )
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
}";
        #endregion

        public static void RegisterCommands()
        {
            CommandSystem.Register( "DesignMulti", AccessLevel.Administrator, new CommandEventHandler( DesignMulti_OnCommand ) );
            CommandSystem.Register( "LoadMulti", AccessLevel.Administrator, new CommandEventHandler( LoadMulti_OnCommand ) );
            CommandSystem.Register( "GenerateMultiInfo", AccessLevel.Administrator, new CommandEventHandler( GenerateInfo_OnCommand ) );
            CommandSystem.Register( "FixMulti", AccessLevel.Administrator, new CommandEventHandler( FixMulti_OnCommand ) );
            CommandSystem.Register( "ClearMulti", AccessLevel.Administrator, new CommandEventHandler( ClearMulti_OnCommand ) );
        }

        /// <summary>
        /// check for <c>MuleditorPath</c>, multis.mul and multis.idx existance and
        /// </summary>
        /// <returns>true if folder and files exist</returns>
        private static bool CheckExistance()
        {
            if( !Directory.Exists( Config.MuleditorPath ) )
                return false;

            if( !File.Exists( Path.Combine( Config.MuleditorPath, "multi.mul" ) ) )
                return false;

            if( !File.Exists( Path.Combine( Config.MuleditorPath, "multi.idx" ) ) )
                return false;

            return true;
        }

        /// <summary>
        /// this reads a multi of index <paramref name="multiID"/> and returns it thru the <see cref="Server.MultiComponentList"/> 
        /// class defined in core class <c>MultiData</c>
        /// </summary>
        /// <param name="idxPath">path of multi.idx</param>
        /// <param name="mulPath">path of multi.mul</param>
        /// <param name="multiID">index of the multi we want to read</param>
        /// <returns>the multi read</returns>
        private static MultiComponentList Read( string idxPath, string mulPath, int multiID )
        {
            if( CheckExistance() )
            {
                //this sets up the filestream and the reader dealies for the desired files
                BinaryReader indexReader, streamReader;

                try
                {
                    indexReader = new BinaryReader( new FileStream( idxPath, FileMode.Open, FileAccess.Read, FileShare.Read ) );
                    streamReader = new BinaryReader( new FileStream( mulPath, FileMode.Open, FileAccess.Read, FileShare.Read ) );
                }
                catch
                {
                    Console.WriteLine( "File access error." );
                    return MultiComponentList.Empty;
                }

                //gets the index to find it in.  there's 12 bytes between each
                //record in the index file (3 32-bit ints )
                // so the multiID * 12 is needed to go to the appropriate spot
                indexReader.BaseStream.Seek( multiID * 12, SeekOrigin.Begin );

                //here's where we can stick that MultiIdxRecord struct..
                int lookup = indexReader.ReadInt32();
                int length = indexReader.ReadInt32();

                Console.WriteLine( String.Format( "Using index #{0}, which gives lookup offset {1:X} and length {2:X}", multiID, lookup, length ) );

                //FF is used for blank spots
                if( lookup < 0 || length <= 0 )
                {
                    indexReader.Close();
                    streamReader.Close();
                    return MultiComponentList.Empty;
                }

                //point the .mul reader at the right spot
                streamReader.BaseStream.Seek( lookup, SeekOrigin.Begin );

                //load it, using the MultiComponentList's constructor for a binary file stream
                MultiComponentList mcl = new MultiComponentList( streamReader, length / 12 );

                indexReader.Close();
                streamReader.Close();

                return mcl;
            }
            else
            {
                Console.WriteLine( "Error loading from multi files.." );
                return null;
            }
        }

        private static MultiComponentList Read( int multiID )
        {
            return Read( Path.Combine( Config.MuleditorPath, "multi.idx" ), Path.Combine( Config.MuleditorPath, "multi.mul" ), multiID );
        }

        private static void EmptyIndex( int multiID )
        {
            if( !CheckExistance() )
                return;

            //this sets up the filestream and the reader dealies for the desired files
            BinaryWriter binaryWriter;

            try
            {
                binaryWriter = new BinaryWriter( new FileStream( Path.Combine( Config.MuleditorPath, "multi.idx" ), FileMode.Open, FileAccess.ReadWrite,
                                                                 FileShare.ReadWrite ) );
            }
            catch
            {
                Console.WriteLine( "File access error." );
                return;
            }

            binaryWriter.BaseStream.Seek( multiID * 12, SeekOrigin.Begin );
            binaryWriter.Write( 0 );
            binaryWriter.Write( 0 );
            binaryWriter.Close();
        }

        /// <summary>
        /// this attempts to writes the multi given in <c>mcl</c> with
        /// the specified multi index number into the workbench multi files
        /// </summary>
        /// <param name="idxPath">path of multi.idx</param>
        /// <param name="mulPath">path of multi.mul</param>
        /// <param name="multiID">index where we want to place our multi</param>
        /// <param name="mcl">multi to write</param>
        /// <returns>true if successful</returns>
        private static bool Write( string idxPath, string mulPath, int multiID, MultiComponentList mcl )
        {
            MultiData.DisposeStreams();

            BinaryWriter streamWriter;
            BinaryWriter indexWriter;

            try
            {
                //this writes to multi.mul, so it appends any new data to it
                indexWriter = new BinaryWriter( new FileStream( idxPath, FileMode.Open, FileAccess.Write, FileShare.Write ) );
                streamWriter = new BinaryWriter( new FileStream( mulPath, FileMode.Append, FileAccess.Write, FileShare.Write ) );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
                Console.WriteLine( "Cannot open files.  Make sure no other programs are currently accessing them, like InsideUO." );
                return false;
            }

            //get the file length before appending the new data (to determine the offset)
            FileInfo fi = new FileInfo( mulPath );
            int offset = (int)fi.Length;

            //this writes the multi in MultiComponentList mcl to the multi.mul file
            //get each multi tile entry (data blocks for each tile) and write to file
            int recordlength = 0;
            foreach( MultiTileEntry mte in mcl.List )
            {
                streamWriter.Write( mte.m_ItemID );
                streamWriter.Write( mte.m_OffsetX );
                streamWriter.Write( mte.m_OffsetY );
                streamWriter.Write( mte.m_OffsetZ );
                streamWriter.Write( mte.m_Flags );

                recordlength += 12;// 12 bytes per record
            }

            Console.WriteLine( String.Format( "Writing to index#: {0}", multiID ) );
            Console.WriteLine( String.Format( "Start: {0}  Record Length: {1}", offset, recordlength ) );

            indexWriter.BaseStream.Seek( multiID * 12, SeekOrigin.Begin ); // go to the spot in the index file to write this down	
            indexWriter.Write( offset ); // write the starting point of the new multi data
            indexWriter.Write( recordlength ); // write the length of the new multi data
            indexWriter.Write( 0 ); // write that stupid blank space thing

            //close the writer
            indexWriter.Flush();
            streamWriter.Flush();

            indexWriter.Close();
            streamWriter.Close();

            Console.WriteLine( "Reloading data..." );
            try
            {
                MultiData.ReLoad();
            }
            catch( Exception e )
            {
                Console.WriteLine( e );
            }
            Console.WriteLine( "done." );

            return true;
        }

        private static bool Write( int multiID, MultiComponentList mcl )
        {
            return Write( Path.Combine( Config.MuleditorPath, "multi.idx" ), Path.Combine( Config.MuleditorPath, "multi.mul" ), multiID, mcl );
        }

        private static Rectangle2D[] DecomposeArea( MultiComponentList mcl, int[ , ] matrix )
        {
            List<Rectangle2D> list = new List<Rectangle2D>();
            Point2D validCorner;

            while( FindFirstValid( matrix, out validCorner ) )
            {
                Point2D firstCorner = validCorner;
                Point2D lastBlack = new Point2D( FindLastBlackOnRow( matrix, firstCorner ), firstCorner.Y );
                Point2D secondCorner = new Point2D( lastBlack.X, FindLastBlackOnColumn( matrix, firstCorner, lastBlack ) );
                Rectangle2D rect = new Rectangle2D( firstCorner, secondCorner );

                if( Config.Debug )
                    Console.WriteLine( rect.ToString() );

                for( int i = rect.Start.X; i <= rect.End.X; i++ )
                {
                    for( int j = rect.Start.Y; j <= rect.End.Y; j++ )
                    {
                        matrix[ i, j ] = 0;
                    }
                }

                Point2D newNECorner = new Point2D( firstCorner.X - mcl.Center.X, firstCorner.Y - mcl.Center.Y );
                Point2D newSWCorner = new Point2D( secondCorner.X - mcl.Center.X + 1, secondCorner.Y - mcl.Center.Y + 1 );
                Rectangle2D offsetRect = new Rectangle2D( newNECorner, newSWCorner );

                if( Config.Debug )
                    Console.WriteLine( offsetRect.ToString() );

                list.Add( offsetRect );
            }

            return list.ToArray();
        }

        /// <summary>
        /// Find first not empty point in the multi component matrix
        /// Cord: Absolute.
        /// Point.Zero is the north east corner
        /// </summary>
        private static bool FindFirstValid( int[ , ] matrix, out Point2D result )
        {
            result = new Point2D( -1, -1 );

            for( int i = 0; i < matrix.GetLength( 0 ); i++ )
            {
                for( int j = 0; j < matrix.GetLength( 1 ); j++ )
                {
                    if( matrix[ i, j ] != 1 )
                        continue;

                    result = new Point2D( i, j );
                    return true;
                }
            }

            return false;
        }

        private static int FindLastBlackOnRow( int[ , ] matrix, IPoint2D p )
        {
            int x = p.X;
            while( x < matrix.GetLength( 0 ) && matrix[ x, p.Y ] == 1 )
                x++;

            return x - 1;
        }

        private static int FindLastBlackOnColumn( int[ , ] matrix, IPoint2D first, IPoint2D second )
        {
            int startY = Math.Min( first.Y, second.Y );

            int y = startY;
            while( y < matrix.GetLength( 1 ) && matrix[ first.X, y ] == 1 && matrix[ second.X, y ] == 1 )
                y++;

            y--;

            // inverse check. convess figures must be checked!
            for( int i = y; i >= startY; i-- )
            {
                for( int x = first.X; x < second.X; x++ )
                {
                    if( matrix[ x, y ] == 0 )
                        y--;
                }
            }

            return y;
        }

        private static int GetMinZ( MultiComponentList mcl )
        {
            int z = int.MaxValue;

            for( int i = 1; i < mcl.List.Length; i++ )
            {
                if( mcl.List[ i ].m_OffsetZ < z )
                    z = mcl.List[ i ].m_OffsetZ;
            }

            return z;
        }

        /// <summary>
        /// Check if a point has one or more items in mcl for that position
        /// </summary>
        private static bool Find( int x, int y, int z, MultiComponentList mcl )
        {
            if( Config.Debug )
                Console.WriteLine( "Find: " + x + " " + y );

            foreach( MultiTileEntry entry in mcl.List )
            {
                if( entry.m_OffsetX == x && entry.m_OffsetY == y && entry.m_OffsetZ == z
                    && entry.m_Flags == 1
                    && !IsSignPost( entry.m_ItemID )
                    && !IsSign( entry.m_ItemID )
                    && !IsDoor( entry.m_ItemID )
                    )
                {
                    if( Config.Debug )
                        Console.WriteLine( "Found!" );
                    return true;
                }
            }

            return false;
        }

        private static string FoorPrint( MultiComponentList mcl, out int[ , ] mat )
        {
            int xCenter = mcl.Center.X;
            int yCenter = mcl.Center.Y;

            int minZ = GetMinZ( mcl );

            if( Config.Debug )
            {
                Console.WriteLine( "Center: " + mcl.Center );
                Console.WriteLine( "GetMinZ: " + minZ );
            }

            mat = new int[ mcl.Width, mcl.Height ];

            for( int i = 0; i < mat.GetLength( 0 ); i++ )
            {
                for( int j = 0; j < mat.GetLength( 1 ); j++ )
                {
                    if( Config.Debug )
                        Console.WriteLine( "ij: " + i + " " + j );

                    mat[ i, j ] = 0; // init our cell

                    for( int z = minZ; z <= minZ + 7; z++ )
                    {
                        if( Find( i - xCenter, j - yCenter, z, mcl ) )
                            mat[ i, j ] = 1;
                    }

                    if( Config.Debug )
                        Console.WriteLine( "" );
                }
            }

            StringBuilder sb = new StringBuilder();

            for( int j = 0; j < mat.GetLength( 1 ); j++ )
            {
                for( int i = 0; i < mat.GetLength( 0 ); i++ )
                {
                    sb.AppendFormat( "{0}\t", mat[ i, j ] );
                }

                sb.Append( "\n" );
            }

            return sb.ToString();
        }

        private static bool IsDoor( int itemID )
        {
            DoorFacing facing;

            return IsDoor( itemID, out facing );
        }

        private static bool IsDoor( int itemID, out DoorFacing facing )
        {
            facing = 0;

            if( itemID >= 0x6A5 && itemID <= 0x6F4 )
            {
                facing = (DoorFacing)( ( itemID - 0x6A5 ) / 2 );
                return true;
            }

            return false;
        }

        private static bool IsSign( int itemID )
        {
            if( itemID >= 0xB95 && itemID <= 0xB0E )
            {
                return true;
            }

            if( itemID >= 0xBA3 && itemID <= 0xC0E )
            {
                return true;
            }

            return false;
        }

        private static bool IsSignPost( int itemID )
        {
            if( itemID >= 0xB97 && itemID <= 0xBA2 )
            {
                return true;
            }

            return false;
        }

        private static int GetArea( IEnumerable<Rectangle2D> rect )
        {
            int area = 0;
            foreach( Rectangle2D r in rect )
            {
                area = area + r.Height * r.Width;
            }

            return area;
        }

        [Usage( "GenerateMultiInfo <index>" )]
        [Description( "Generate info for a multi of specified index" )]
        private static void GenerateInfo_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( e.Length == 1 )
            {
                try
                {
                    int multiID = e.GetInt32( 0 );

                    if( multiID > 0 || multiID < Config.MaxMulti )
                    {
                        if( CheckExistance() )
                        {
                            MultiComponentList mcl = Read( multiID );

                            if( mcl != null )
                            {
                                using( TextWriter tw = File.CreateText( string.Format( "info_id_m_{0}.txt", multiID ) ) )
                                {
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( string.Format( "Multi info for id: {0}", multiID ) );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );

                                    tw.WriteLine( "Center: {0}", mcl.Center.ToString() );
                                    tw.WriteLine( "Height: {0}", mcl.Height );
                                    tw.WriteLine( "Width: {0}", mcl.Width );
                                    tw.WriteLine( "Min: {0}", mcl.Min.ToString() );
                                    tw.WriteLine( "Max: {0}", mcl.Max.ToString() );

                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );

                                    int index = 0;
                                    foreach( MultiTileEntry mte in mcl.List )
                                    {
                                        tw.WriteLine( String.Format( "Multi #{0} component ({1}/{2}), ItemID# {3}, ({4}, {5}, {6}), flags: {7:X}",
                                                                    multiID, index, mcl.List.Length, mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ, mte.m_Flags ) );
                                        index++;
                                    }

                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );

                                    DoorFacing facing;
                                    foreach( MultiTileEntry mte in mcl.List )
                                    {
                                        if( IsDoor( mte.m_ItemID, out facing ) )
                                            tw.WriteLine( "Door: ItemID# {0}, ({1}, {2}, {3})", mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ );
                                    }

                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );

                                    foreach( MultiTileEntry mte in mcl.List )
                                    {
                                        if( IsSign( mte.m_ItemID ) )
                                            tw.WriteLine( "Sign: ItemID# {0}, ({1}, {2}, {3})", mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ );
                                    }

                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );

                                    int[ , ] mat;
                                    string footPrint = FoorPrint( mcl, out mat );

                                    tw.WriteLine( footPrint );

                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "" );
                                    tw.WriteLine( "########################" );
                                    tw.WriteLine( "" );

                                    Rectangle2D[] area = DecomposeArea( mcl, mat );
                                    foreach( Rectangle2D rec in area )
                                    {
                                        tw.WriteLine( rec.ToString() );
                                    }

                                    using( TextWriter scriptWriter = File.CreateText( string.Format( "{0}.cs", multiID ) ) )
                                    {
                                        StringBuilder sb = new StringBuilder();

                                        string output = m_Template.Replace( "{0}", multiID.ToString() );

                                        foreach( Rectangle2D rec in area )
                                        {
                                            sb.AppendFormat( "new Rectangle2D( {0}, {1}, {2}, {3} ), ",
                                                rec.Start.X, rec.Start.Y, rec.Width, rec.Height );
                                        }

                                        output = output.Replace( "{1}", sb.ToString() );
                                        output = output.Replace( "{2}", multiID.ToString() );

                                        sb.Remove( 0, sb.Length );
                                        foreach( MultiTileEntry mte in mcl.List )
                                        {
                                            if( IsDoor( mte.m_ItemID, out facing ) )
                                            {
                                                sb.Append( String.Format(
                                                        "\t\t\t// Door: ItemID# {0}, ({1}, {2}, {3}) Facing: {4}\n",
                                                        mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ,
                                                        facing ) );

                                                if( facing == DoorFacing.WestCW || facing == DoorFacing.EastCCW
                                                    || facing == DoorFacing.WestCCW || facing == DoorFacing.EastCW )
                                                {
                                                    sb.Append( string.Format( "\t\t\tAddSouthDoor( {0}, {1}, {2} );\n",
                                                                            mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ ) );
                                                }
                                                else if( facing == DoorFacing.EastCCW || facing == DoorFacing.EastCW
                                                         || facing == DoorFacing.NorthCCW || facing == DoorFacing.SouthCW )
                                                {
                                                    sb.Append( string.Format( "\t\t\tAddEastDoor( {0}, {1}, {2} );\n",
                                                                            mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ ) );
                                                }

                                                sb.AppendLine();
                                            }
                                        }

                                        foreach( MultiTileEntry mte in mcl.List )
                                        {
                                            if( IsSign( mte.m_ItemID ) )
                                            {
                                                sb.Append( string.Format( "\t\t\tSetSign( {0}, {1}, {2} );\n", mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ ) );
                                                if( mte.m_ItemID != 0xBD2 )
                                                    sb.Append( string.Format( "\t\t\tChangeSignType( {0} );", mte.m_ItemID ) );
                                            }
                                        }

                                        output = output.Replace( "{3}", sb.ToString() );
                                        output = output.Replace( "{4}", GetArea( area ).ToString() );
                                        output = output.Replace( "{5}", String.Format( "\"A deed for a house. (ID: {0})\"", multiID ) );
                                        output = output.Replace( "{6}", ( mcl.Max.Y + 1 ).ToString() );
                                        output = output.Replace( "{7}", String.Format( " {0}, {1}, {2} ", mcl.Max.X, mcl.Max.Y + 1, 0 ) );

                                        scriptWriter.WriteLine( output );
                                    }
                                }
                            }
                            else
                            {
                                from.SendMessage( "Error loading multi component list from that index." );
                            }
                        }
                        else
                        {
                            from.SendMessage( "The folder Core/MultiEditor does not exist or is empty. Multi.mul and Multi.idx must be there." );
                        }
                    }
                    else
                    {
                        from.SendMessage( String.Format( "Invalid index number. Your index number must be positive, and below {0}", Config.MaxMulti ) );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "Error: GenerateInfo" );
                    Console.WriteLine( ex.ToString() );
                }
            }
            else
            {
                from.SendMessage( "CommandUse: [GenerateMultiInfo <index>" );
            }
        }

        [Usage( "FixMulti <index>" )]
        [Description( "Fix invisible items of a multi of specified index" )]
        private static void FixMulti_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( e.Length == 1 )
            {
                try
                {
                    int multiID = e.GetInt32( 0 );

                    if( multiID > 0 || multiID < Config.MaxMulti )
                    {
                        if( CheckExistance() )
                        {
                            MultiComponentList mcl = Read( multiID );

                            if( mcl != null )
                            {
                                List<MultiTileEntry> validList = new List<MultiTileEntry>( mcl.List.Length );

                                using( TextWriter logger = File.CreateText( string.Format( "multi-errors-id_{0}.txt", multiID ) ) )
                                {
                                    for( int i = 0; i < mcl.List.Length; i++ )
                                    {
                                        MultiTileEntry mte = mcl.List[ i ];

                                        if( mte.m_ItemID == 1 || IsDoor( mte.m_ItemID ) || IsSign( mte.m_ItemID ) )
                                        {
                                            logger.WriteLine( "ItemID# {0}, ({1}, {2}, {3})", mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ );
                                            validList.Add( new MultiTileEntry( mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ, 0 ) );
                                        }
                                        else
                                            validList.Add( new MultiTileEntry( mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ, 1 ) );
                                    }

                                    logger.WriteLine( "" );

                                    for( int i = 0; i < validList.Count; i++ )
                                    {
                                        MultiTileEntry mte = validList[ i ];
                                        if( mte.m_Flags == 0 )
                                            logger.WriteLine( "One-Flag-Mte ItemID# {0}, ({1}, {2}, {3})", mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ );
                                    }
                                }

                                MultiData.DisposeStreams();

                                if( !Write( multiID, new MultiComponentList( mcl.Min, mcl.Max, mcl.Center, mcl.Width, mcl.Height, validList.ToArray() ) ) )
                                    from.SendMessage( "Error writing multi to file stream." );
                                else
                                    from.SendMessage( "Operation successed." );

                                from.SendMessage( "Reloading data..." );
                                try
                                {
                                    MultiData.ReLoad();
                                }
                                catch( Exception exception )
                                {
                                    Console.WriteLine( exception );
                                }
                                from.SendMessage( "done." );
                            }
                            else
                            {
                                from.SendMessage( "Error loading multi component list from that index." );
                            }
                        }
                        else
                        {
                            from.SendMessage( "The folder Core/MultiEditor does not exist or is empty. Multi.mul and Multi.idx must be there." );
                        }
                    }
                    else
                    {
                        from.SendMessage( String.Format( "Invalid index number. Your index number must be positive, and below {0}", Config.MaxMulti ) );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "Error: OnCaptureMulti" );
                    Console.WriteLine( ex.ToString() );
                }
            }
            else
            {
                from.SendMessage( "CommandUse: [FixMulti <index>" );
            }
        }

        [Usage( "DesignMulti <index>" )]
        [Description( "Captures a collection of items and stores them into a multi of specified index" )]
        private static void DesignMulti_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( e.Length == 1 )
            {
                try
                {
                    int multiID = e.GetInt32( 0 );

                    if( multiID > 0 || multiID < Config.MaxMulti )
                    {
                        if( CheckExistance() )
                        {
                            MultiComponentList mcl = Read( multiID );

                            if( mcl != null )
                            {
                                if( mcl == MultiComponentList.Empty )
                                {
                                    BoundingBoxPicker.Begin( from, new BoundingBoxCallback( PickerCallback ), multiID );
                                }
                                else
                                {
                                    from.SendMessage( "Cannot use that index number. That multi already exists." );
                                }
                            }
                            else
                            {
                                from.SendMessage( "Error loading multi component list from that index." );
                            }
                        }
                        else
                        {
                            from.SendMessage( "The folder Core/MultiEditor does not exist or is empty. Multi.mul and Multi.idx must be there." );
                        }
                    }
                    else
                    {
                        from.SendMessage( String.Format( "Invalid index number. Your index number must be positive, and below {0}", Config.MaxMulti ) );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "Error: OnCaptureMulti" );
                    Console.WriteLine( ex.ToString() );
                }
            }
            else
            {
                from.SendMessage( "CommandUse: [DesignMulti <index>" );
            }
        }

        private static void PickerCallback( Mobile from, Map map, Point3D start, Point3D end, object state )
        {
            if( start.X > end.X )
            {
                int x = start.X;
                start.X = end.X;
                end.X = x;
            }

            if( start.Y > end.Y )
            {
                int y = start.Y;
                start.Y = end.Y;
                end.Y = y;
            }

            Rectangle2D bounds = new Rectangle2D( start, end );

            IPooledEnumerable eable = map.GetItemsInBounds( bounds );
            List<MultiDataGrabber> target = new List<MultiDataGrabber>();

            bool fail = false;

            try
            {
                foreach( Item item in eable )
                    target.Add( new MultiDataGrabber( item.Location, item.ItemID ) );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
                from.SendMessage( 0x40, "The targeted items have been modified. Please retry." );
                fail = true;
            }
            finally
            {
                eable.Free();
            }

            int count = 0;

            for( int i = start.Y; i <= end.Y; i++ )
            {
                for( int j = start.X; j <= end.X; j++ )
                {
                    Tile[] tiles = map.Tiles.GetStaticTiles( j, i, true ); //3rd parameter "true" checks multi's.

                    foreach( Tile tile in tiles )
                    {
                        count++;
                        target.Add( new MultiDataGrabber( new Point3D( j, i, tile.Z ), tile.ID & 0x3FFF ) );
                    }
                }
            }

            from.SendMessage( string.Format( "Found {0} tiles", count ) );

            if( fail )
            {
                from.SendMessage( 0x40, "Something went wrong during rectangle pick parsing." );
                return;
            }

            if( target.Count == 0 )
            {
                from.SendMessage( 0x40, "No items have been selected." );
                return;
            }

            // Get center
            Point3D worldcenter = new Point3D();
            worldcenter.Z = 127;

            //used to calculate the boundaris of the items.  Start from the opposite sides
            int x1 = bounds.End.X;
            int y1 = bounds.End.Y;
            int z1 = -127;
            int x2 = bounds.Start.X;
            int y2 = bounds.Start.Y;
            int z2 = -127;

            // Get correct bounds
            foreach( MultiDataGrabber mdg in target )
            {
                if( mdg.Z < worldcenter.Z )
                    worldcenter.Z = mdg.Z; //take lowest point as z center

                x1 = Math.Min( x1, mdg.X );	//move back as items are found
                y1 = Math.Min( y1, mdg.Y );
                z1 = Math.Min( z1, mdg.Z );
                x2 = Math.Max( x2, mdg.X );
                y2 = Math.Max( y2, mdg.Y );
                z2 = Math.Max( z2, mdg.Z );
            }

            worldcenter.X = ( x1 + x2 ) / 2; //center is just the average
            worldcenter.Y = ( y1 + y2 ) / 2;

            from.SendMessage( String.Format( "Found object center at ({0}, {1}, {2})", worldcenter.X, worldcenter.Y, worldcenter.Z ) );

            //this is the header info needed for MultiComponentList
            Point2D min = new Point2D( x1, y1 );
            Point2D max = new Point2D( x2, y2 );
            Point2D center = new Point2D( worldcenter.X - x1, worldcenter.Y - y1 );
            int width = x2 - x1 + 1;  //count tiles inclusive
            int height1 = y2 - y1 + 1;

            MultiTileEntry[] mte = new MultiTileEntry[ target.Count ]; // set up the tile data array to be fed into MultiComponentList

            //get the items at the center, load first, and remove from the collection target
            IPooledEnumerable findCenterItem = from.Map.GetItemsInRange( new Point3D( worldcenter.X, worldcenter.Y, 0 ), 0 );

            int counter = 0;

            foreach( Item centeritem in findCenterItem )
            {
                mte[ counter ].m_ItemID = (short)centeritem.ItemID;
                mte[ counter ].m_OffsetX = (short)( centeritem.X - worldcenter.X );
                mte[ counter ].m_OffsetY = (short)( centeritem.Y - worldcenter.Y );
                mte[ counter ].m_OffsetZ = (short)( centeritem.Z - worldcenter.Z );
                mte[ counter ].m_Flags = ( counter == 0 ? 0 : 1 );

                target.Remove( MultiDataGrabber.GetMultiDataGrabber( target, centeritem ) ); //make sure not to render it more than once

                counter++;
            }

            foreach( MultiDataGrabber mdg in target )
            {
                mte[ counter ].m_ItemID = (short)mdg.ID;
                mte[ counter ].m_OffsetX = (short)( mdg.X - worldcenter.X );
                mte[ counter ].m_OffsetY = (short)( mdg.Y - worldcenter.Y );
                mte[ counter ].m_OffsetZ = (short)( mdg.Z - worldcenter.Z );

                // 0 tells the server not to load it in if the item count > 0
                //this looks to be used as a way to disregard components of a multi
                //that will be replaced by nonstatic items.  Eg.  tillerman, planks, 
                //and hatch of a gump.  It also looks like the very first item is
                //set to 0.  All others are set to 1.
                mte[ counter ].m_Flags = ( counter == 0 ? 0 : 1 );

                if( IsDoor( mdg.ID ) || IsSign( mdg.ID ) )
                    mte[ counter ].m_Flags = 0;

                /* and thats how flags are defined..
                * 0x00000001	1 = render, 0 = don't render  except first item in set
                * 0x00000002    unused
                * 0x00000004    unused
                * ...
                * 0x80000000    unused    
                */

                counter++;
            }

            from.SendMessage( String.Format( "Width: {0} Height:{1}, number of items: {2}", width, height1, counter ) );

            //here's where the MultiComponentList is made to allow for file writing
            // Point2D Min				-smallest x/y coords of multi
            // Point2D Max				-largest x/y coords of multi
            // int MinZ					-smallest z coord of multi
            // int MaxZ					-largest z coord of multi
            // Point2D Center			-x/y coord of center of multi
            // int Width				-# of tiles in the x direction (inclusive: x = 250 to x = 251 is a Width of 2)
            // int Height				-# of tiles in the y direction (inclusive: y = 250 to y = 251 is a Height of 2)
            // MultiTileEntries[] mte	-collection of MultiTileEntries for each tile

            if( !Write( (int)state, new MultiComponentList( min, max, center, width, height1, mte ) ) )
                from.SendMessage( "Error writing multi to file stream." );

            Blocker blocker = new Blocker();
            blocker.MoveToWorld( worldcenter, map );
            from.SendMessage( "A blocker has been placed in the center of the multi." );
        }

        [Usage( "LoadMulti <index> <debug: true|false>" )]
        [Description( "Reads in a multi of given index, and allows the user to place a collection of items that represent the multi" )]
        private static void LoadMulti_OnCommand( CommandEventArgs e )
        {
            bool debug = false;

            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( e.Length == 2 )
                debug = e.GetBoolean( 2 );

            if( e.Length <= 2 )
            {
                try
                {
                    int multiID = e.GetInt32( 0 );

                    if( multiID > 0 || multiID < Config.MaxMulti )
                    {
                        if( CheckExistance() )
                        {
                            MultiComponentList mcl = Read( multiID );

                            if( mcl != null )
                            {
                                if( mcl != MultiComponentList.Empty )
                                {
                                    from.Target = new LoadMultiTarget( mcl, multiID, debug );
                                }
                                else
                                {
                                    from.SendMessage( "Cannot use that index number. That multi index is empty." );
                                }
                            }
                            else
                            {
                                from.SendMessage( "Error loading multi component list from that index." );
                            }
                        }
                        else
                        {
                            from.SendMessage( "The folder Core/MultiEditor does not exist or is empty. Multis.mul and Multis.idx must be there." );
                        }
                    }
                    else
                    {
                        from.SendMessage( String.Format( "Invalid index number. Your index number must be positive, and below {0}", Config.MaxMulti ) );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "Error: OnCaptureMulti" );
                    Console.WriteLine( ex.ToString() );
                }
            }
            else
            {
                from.SendMessage( "CommandUse: [LoadMulti <index> <debug: true|false>" );
            }
        }

        [Usage( "ClearMulti <index>" )]
        [Description( "Clear the given multi from multi.idx file" )]
        private static void ClearMulti_OnCommand( CommandEventArgs e )
        {
            bool debug = false;

            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( e.Length == 2 )
                debug = e.GetBoolean( 2 );

            if( e.Length <= 2 )
            {
                try
                {
                    int multiID = e.GetInt32( 0 );

                    if( multiID > 0 || multiID < Config.MaxMulti )
                    {
                        if( CheckExistance() )
                        {
                            MultiComponentList mcl = Read( multiID );

                            if( mcl != null )
                            {
                                if( mcl != MultiComponentList.Empty )
                                {
                                    EmptyIndex( multiID );
                                }
                                else
                                {
                                    from.SendMessage( "Cannot use that index number. That multi index is empty." );
                                }
                            }
                            else
                            {
                                from.SendMessage( "Error loading multi component list from that index." );
                            }
                        }
                        else
                        {
                            from.SendMessage( "The folder Core/MultiEditor does not exist or is empty. Multis.mul and Multis.idx must be there." );
                        }
                    }
                    else
                    {
                        from.SendMessage( String.Format( "Invalid index number. Your index number must be positive, and below {0}", Config.MaxMulti ) );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "Error: OnCaptureMulti" );
                    Console.WriteLine( ex.ToString() );
                }
            }
            else
            {
                from.SendMessage( "CommandUse: [LoadMulti <index> <debug: true|false>" );
            }
        }

        internal class MultiDataGrabber
        {
            public Point3D Location;
            public int ID;

            public int X
            {
                get { return Location.X; }
                set { Location.X = value; }
            }

            public int Y
            {
                get { return Location.Y; }
                set { Location.Y = value; }
            }

            public int Z
            {
                get { return Location.Z; }
                set { Location.Z = value; }
            }

            public MultiDataGrabber( Point3D location, int id )
            {
                Location = location;
                ID = id;
            }

            public static MultiDataGrabber GetMultiDataGrabber( List<MultiDataGrabber> list, Item item )
            {
                foreach( MultiDataGrabber mdg in list )
                {
                    if( mdg.Location == item.Location && mdg.ID == item.ItemID )
                    {
                        return mdg;
                    }
                }
                return null;
            }
        }

        internal class LoadMultiTarget : Target
        {
            private MultiComponentList m_Mcl;
            private int m_MultiID;
            private bool m_Debug;

            public LoadMultiTarget( MultiComponentList mcl, int multiID, bool debug )
                : base( -1, true, TargetFlags.None )
            {
                m_Mcl = mcl;
                m_MultiID = multiID;
                m_Debug = debug;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                Point3D targ;

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

                from.SendMessage( String.Format( "Building {0} item{1}", m_Mcl.List.Length, m_Mcl.List.Length == 1 ? "" : "s" ) );

                int itemscreated = 0;

                foreach( MultiTileEntry mte in m_Mcl.List )
                {
                    Item item = new Item();

                    item.ItemID = mte.m_ItemID;
                    item.Movable = false;

                    int x = targ.X + mte.m_OffsetX;
                    int y = targ.Y + mte.m_OffsetY;
                    int z = targ.Z + mte.m_OffsetZ;

                    //fills the items with verbose info on the label
                    if( m_Debug || true )
                    {
                        item.Name = String.Format( "Multi #{0} component ({1}/{2}), ItemID# {3}, ({4}, {5}, {6}), flags: {7:X}",
                            m_MultiID, itemscreated, m_Mcl.List.Length, item.ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ, mte.m_Flags );

                        if( mte.m_Flags == 0 )
                            item.Hue = 1;
                        else if( mte.m_Flags != 1 )
                            item.Hue = 1971;
                    }

                    item.Location = new Point3D( x, y, z );
                    item.Map = from.Map;

                    itemscreated++;
                }

                from.SendMessage( String.Format( "{0} item{1} created", itemscreated, itemscreated == 1 ? "" : "s" ) );
            }
        }
    }
}