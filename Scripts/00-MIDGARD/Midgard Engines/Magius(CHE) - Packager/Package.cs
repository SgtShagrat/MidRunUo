using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Server;
using Server.Network;

namespace Midgard.Engines.Packager
{
    public class Package : IComparable
    {
        public enum ConsoleHeaderStyles
        {
            None,
            Short,
            Long,
        }
		
		/*public bool Configured{get;private set;}
		public bool Initialized{get;private set;}*/
		

        /// <summary>
        /// A copy of package_info
        /// </summary>
        private readonly Dictionary<string, object> m_PackageInfo = new Dictionary<string, object>();

        /// <summary>
        /// Contruct package by type.
        /// </summary>
        /// <param name="typeToLoad"></param>
        internal Package( Type typeToLoad )
        {
            Type = typeToLoad;
            object[] arrPackageInfo;
            var field = typeToLoad.GetField( "Package_Info", BindingFlags.Static | BindingFlags.Public );
            if( field == null )
            {
                var prop = typeToLoad.GetProperty( "Package_Info", BindingFlags.Static | BindingFlags.Public );
                if( prop != null )
                    arrPackageInfo = (object[])prop.GetValue( null, null );
                else
                {
                    arrPackageInfo = new object[]
                                         {
                                             "Script Title", typeToLoad.FullName,
                                             "Enabled by Default", true,
                                             "Script Version", typeToLoad.Assembly.GetName().Version,
                                             //"Author name",            null, 
                                             "Creation Date", string.IsNullOrEmpty(typeToLoad.Assembly.Location) ? DateTime.MinValue : File.GetCreationTime(typeToLoad.Assembly.Location),
                                             //"Author mail-contact",    null, 
                                             //"Author home site",       null,
                                             //"Author notes",           null,
                                             //"Script Copyrights",      null,
                                             "Provided packages", new string[] {typeToLoad.FullName},
                                             //"Required packages",      new string[0],
                                             //"Conflicts with packages",new string[0],
                                             "Research tags", typeToLoad.FullName.Split('.')
                                         };
                }
            }
            else
                arrPackageInfo = (object[])field.GetValue( null );


            if( arrPackageInfo.Length % 2 != 0 )
            {
                throw new Exception( "Package has invalid Package_Info. Must have Package_Info.Length disibile by 2. (Key,Value)" );
            }

            for( var h = 0; h < arrPackageInfo.Length; h += 2 )
            {
                if( !( arrPackageInfo[ h ] is string ) )
                    throw new Exception( "Package has invalid Pakage_Info. Must be in format of (string)Key,(object)value. Element " + h +
                                        " must be string." );

                m_PackageInfo.Add( (string)arrPackageInfo[ h ], arrPackageInfo[ h + 1 ] );
            }


            //check enabling method...
            //is executable?
            var configure = Type.GetMethod( "Configure", BindingFlags.Static | BindingFlags.Public );
            var initialize = Type.GetMethod( "Initialize", BindingFlags.Static | BindingFlags.Public );
            var pkgConfigure = ExtractMethod( "Package_Configure" );
            var pkgInitialize = ExtractMethod( "Package_Initialize" );

            HasStandardInitiAndConfig = ( configure != null || initialize != null );

            var cancontroled = ( pkgConfigure != null || pkgInitialize != null );

            if( HasStandardInitiAndConfig == cancontroled && cancontroled )
                throw new Exception( "Package cannot have both of Configure & Package_Configure nor Initialize & Package_Initialize." );

            m_Enabled = EnabledbyDefault;
        }

        /// <summary>
        /// Base type has generated this package.
        /// </summary>
        public Type Type { get; private set; }

        #region IComparable Members

        public int CompareTo( object obj )
        {
            return Type.ToString().CompareTo( "" + obj );
        }

        #endregion

        #region Properties

        private bool m_Enabled = true;

        /// <summary>
        /// Enable ot Disable this package. Invoke and Packager.BroadcastInvoke will not affect this package
        ///  if it is disabled.
        /// </summary>
        public bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                var old = m_Enabled;
                m_Enabled = value;
                if( value != old )
                {
                    //Inform script of changing event.
                    Invoke( "Package_OnEnabledChanged", old );
                }
            }
        }


        /// <summary>
        /// Return True if this backage HAS Initialize instead of Package_Initialize
        ///  and/or Configure instead of Package_Configure
        /// </summary>
        public bool HasStandardInitiAndConfig { get; private set; }

        /// <summary>
        /// Return True if this backage HAS Package_Initialize instead of Initialize
        ///  and/or Package_Configure instead of Configure.
        /// Enable or disable this package durinf Core.Configuration disable Package_Config and PAckag_Initialize too.
        /// </summary>
        public bool HasPackageInitiAndConfig { get; private set; }

        public string Title
        {
            get { return (string)GetPackageInfo( "Script Title", null ); }
        }

        public Version Version
        {
            get { return (Version)GetPackageInfo( "Script Version", null ); }
        }

        public string AuthorName
        {
            get { return (string)GetPackageInfo( "Author name", null ); }
        }

        public DateTime CreationTime
        {
            get { return (DateTime)GetPackageInfo( "Creation Date", DateTime.MinValue ); }
        }

        public string AuthorEMail
        {
            get { return (string)GetPackageInfo( "Author mail-contact", null ); }
        }

        public string AuthorHomeSite
        {
            get { return (string)GetPackageInfo( "Author home site", null ); }
        }

        public string AuthorNotes
        {
            get { return (string)GetPackageInfo( "Author notes", null ); }
        }

        public string Copyrights
        {
            get { return (string)GetPackageInfo( "Script Copyrights", null ); }
        }

        public string[] ProvidedPackages
        {
            get { return (string[])GetPackageInfo( "Provided packages", new string[ 0 ] ); }
        }

        public string[] RequiredPackages
        {
            get { return (string[])GetPackageInfo( "Required packages", new string[ 0 ] ); }
        }
		
		public Package[] ExistingRequiredPackages
        {
            get { 
				lock( Core.Singleton.Packages )
                {
					var ret = new List<Package>();
                    foreach( var req in RequiredPackages )
                        if( !string.IsNullOrEmpty( req ) )
                        {
                            // var provided = false;
                            foreach( var pkg in Core.Singleton.Packages )
                                if( pkg != this )
                                    if( pkg.Provides( req ) )
                                    {
                                        ret.Add(pkg);
                                        break;
                                    }                            
                        }
					return ret.ToArray();
                }
			}
        }

        public string[] ConflictedPackages
        {
            get { return (string[])GetPackageInfo( "Conflicts with packages", new string[ 0 ] ); }
        }

        public bool EnabledbyDefault
        {
            get { return (bool)GetPackageInfo( "Enabled by Default", false ); }
        }

        #endregion

        #region Methods

        private string m_LastRequirementNotSatisfied;

        /// <summary>
        /// Returns last requirements not be satisfied after each "IsRequirementsSatisfied" call.
        /// </summary>
        public string LastRequirementNotSatisfied
        {
            get { return m_LastRequirementNotSatisfied; }
        }


        /// <summary>
        /// Return true is all Requirements are stisfied.
        /// </summary>
        public bool IsRequirementsSatisfied
        {
            get
            {
                lock( Core.Singleton.Packages )
                {
                    foreach( var req in RequiredPackages )
                        if( !string.IsNullOrEmpty( req ) )
                        {
                            var provided = false;
                            foreach( var pkg in Core.Singleton.Packages )
                                if( pkg != this )
                                    if( pkg.Provides( req ) /*&& pkg.Configured*/ )
                                    {
                                        provided = true;
                                        break;
                                    }
                            if( !provided )
                            {
                                m_LastRequirementNotSatisfied = req;
                                return false;
                            }
                        }
                }
                return true;
            }
        }

        /// <summary>
        /// Returns info based on Package_Info
        /// </summary>
        /// <param name="key">key to search</param>
        /// <param name="defaultvalue">default value case not found</param>
        /// <returns>Found value or defaultvalue</returns>
        private object GetPackageInfo( string key, object defaultvalue )
        {
            if( m_PackageInfo.ContainsKey( key ) )
                return m_PackageInfo[ key ];
            return defaultvalue;
        }

        /// <summary>
        /// Check package conficts.
        /// </summary>
        /// <param name="toTest">Package to test with</param>
        /// <returns>True means conficts</returns>
        public bool ConflictsWith( Package toTest )
        {
            foreach( var str in ConflictedPackages )
                if( !string.IsNullOrEmpty( str ) )
                    foreach( var provided in toTest.ProvidedPackages )
                        if( !string.IsNullOrEmpty( str ) && str.Equals( provided ) )
                            return true;
            return false;
        }

        /// <summary>
        /// Check package provide.
        /// </summary>
        /// <param name="packageToProvide">Provide to check</param>
        /// <returns>True meas package provides requested one</returns>
        public bool Provides( string packageToProvide )
        {
            foreach( var provided in ProvidedPackages )
                if( !string.IsNullOrEmpty( provided ) && provided.Equals( packageToProvide ) )
                    return true;
            return false;
        }

        #region PackagerConsoleLogSystem

        private const string DefaultIndentString = "   ";

        /// <summary>
        /// Memorize last package calls FullLog or WriteHeaderToConsole
        /// </summary>
        private static Package m_LastPackageLogWritten;

        private static Point2D m_LastPackageLogWrittenPos = Point2D.Zero;
        private Point2D m_LastProgressPoint = Point2D.Zero;
        private int m_FirstInvokeTop;
        private string m_FirstInvokeTxt;

        /// <summary>
        /// Perform a colored SecureConsole.Write with package signature (no linefeed will be written)
        /// </summary>
        /// <param name="showLongInfo">True to show version and author name</param>
        public void WriteHeaderToConsole( bool showLongInfo )
        {
            Utility.PushColor( ConsoleColor.White );
            SecureConsole.Write( Title );
            Utility.PopColor();
            if( showLongInfo )
            {
                Utility.PushColor( ConsoleColor.DarkGreen );
                SecureConsole.Write( " v" + Version );
                Utility.PopColor();
                if( !string.IsNullOrEmpty( AuthorName ) )
                {
                    Utility.PushColor( ConsoleColor.DarkGray );
                    SecureConsole.Write( " by " + AuthorName );
                    Utility.PopColor();
                }
            }

            StorePos();
        }

        /// <summary>
        /// Perform a colored SecureConsole.Write with package signature (no linefeed will be written)
        /// </summary>
        public void WriteHeaderToConsole()
        {
            WriteHeaderToConsole( false );
        }

        /// <summary>
        /// Perform a colored SecureConsole.WriteLine with package signature and Enabled Disabled string.
        /// </summary>
        /// <param name="longdesc">True = Show version and Author</param>
        public void WriteEnableStatusToConsole( bool longdesc )
        {
            FullLog( ( Enabled ? "enabled." : "disabled." ), longdesc ? ConsoleHeaderStyles.Long : ConsoleHeaderStyles.Short, 0, ( Enabled ? ConsoleColor.Green : ConsoleColor.Red ), true );
        }
		
		
		private static int ConsoleBufferWidthOffset = 0;
		
        public void WriteInvokeBeginToConsole( string txt, ConsoleColor color )
        {
			
            SecureConsole.Write( ":: " );
            WriteHeaderToConsole( true );

            m_FirstInvokeTop = SecureConsole.CursorTop;
            m_FirstInvokeTxt = txt;

            SecureConsole.CursorLeft = SecureConsole.BufferWidth + ConsoleBufferWidthOffset - ( txt.Length + 2 );
						
            SecureConsole.Write( "[" );
            Utility.PushColor( color );
            SecureConsole.Write( txt );
            Utility.PopColor();
            SecureConsole.Write( "]" );
			
            StorePos();
        }

        public void WriteInvokeEndToConsole( string txt, ConsoleColor color )
        {
            var nowpt = new Point2D( SecureConsole.CursorLeft, SecureConsole.CursorTop );
            //SecureConsole.CursorLeft = mFirstInitializePoint.X;
            SecureConsole.CursorTop = m_FirstInvokeTop;
            SecureConsole.CursorLeft = SecureConsole.BufferWidth + ConsoleBufferWidthOffset - ( m_FirstInvokeTxt.Length + 2 );
            if( m_FirstInvokeTxt.Length > txt.Length )
                SecureConsole.Write( new string( ' ', m_FirstInvokeTxt.Length - txt.Length ) );

            SecureConsole.CursorLeft = SecureConsole.BufferWidth + ConsoleBufferWidthOffset - ( txt.Length + 2 );
			
			var memy = SecureConsole.CursorTop;
			
            SecureConsole.Write( "[" );
            Utility.PushColor( color );
            SecureConsole.Write( txt );
            Utility.PopColor();
            SecureConsole.Write( "]" );
			
			if (memy == SecureConsole.CursorTop)
			{
				//buffer scrolled down...
				if ( Server.Core.IsUnderLinux )
					SecureConsole.WriteLine();				
			}

            SecureConsole.CursorLeft = nowpt.X;
			
            SecureConsole.CursorTop = nowpt.Y;

            StorePos();
        }
		
		public override string ToString()
		{
			return this.Title + " v" + this.Version;
		}

        /// <summary>
        /// Perform a colored SecureConsole.Write
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="writeheader">Header Diaply type</param>
        /// <param name="indents">Valid only with writeheader = ConsoleHeaderStyles.None, print 'DefaultIndentString' indents times</param>
        /// <param name="msgcolor">Color to write</param>
        /// <param name="linefeed">True cause WriteLine else Write</param>
        /// <param name="args">Args used to format string</param>
        private void FullLog( string format, ConsoleHeaderStyles writeheader, int indents, ConsoleColor msgcolor, bool linefeed, params object[] args )
        {
            if( writeheader != ConsoleHeaderStyles.None )
            {
                WriteHeaderToConsole( writeheader == ConsoleHeaderStyles.Long );
                SecureConsole.Write( ": " );
            }
            else
            {
                for( var h = 0; h < indents; h++ )
                    SecureConsole.Write( DefaultIndentString );
            }
            Utility.PushColor( msgcolor );
			if (args==null || args.Length==0)
				SecureConsole.Write( format );
			else
				SecureConsole.Write( String.Format( format, args ) );
            Utility.PopColor();
            if( linefeed )
                SecureConsole.WriteLine();

            StorePos();
        }

        private void StorePos()
        {
            m_LastPackageLogWritten = this;
            m_LastPackageLogWrittenPos = new Point2D( SecureConsole.CursorLeft, SecureConsole.CursorTop );
        }

        /// <summary>
        /// Perform a Gray SecureConsole.Write (no line feed)
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="indents">Valid only with writeheader = ConsoleHeaderStyles.None, print 'DefaultIndentString' indents times</param>
        /// <param name="args">Args used to format string</param>
        public void LogInfoIndent( string format, int indents, params object[] args )
        {
            FullLog( format, ( m_LastPackageLogWritten != this || m_LastPackageLogWrittenPos.Y != SecureConsole.CursorTop ) ? ConsoleHeaderStyles.Short : ConsoleHeaderStyles.None, indents, ConsoleColor.Gray, false, args );
        }

        /// <summary>
        /// Perform a Gray SecureConsole.Write (no line feed)
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="args">Args used to format string</param>
        public void LogInfo( string format, params object[] args )
        {
            LogInfoIndent( format, ( SecureConsole.CursorLeft == 0 ) ? 1 : 0, args );
        }

        /// <summary>
        /// Perform a Gray SecureConsole.WriteLine
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="indents">Valid only with writeheader = ConsoleHeaderStyles.None, print 'DefaultIndentString' indents times</param>
        /// <param name="args">Args used to format string</param>
        public void LogInfoIndentLine( string format, int indents, params object[] args )
        {
            FullLog( format, ( m_LastPackageLogWritten != this || m_LastPackageLogWrittenPos.Y != SecureConsole.CursorTop ) ? ConsoleHeaderStyles.Short : ConsoleHeaderStyles.None, indents, ConsoleColor.Gray, true, args );
        }

        /// <summary>
        /// Perform a Gray SecureConsole.WriteLine
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="args">Args used to format string</param>
        public void LogInfoLine( string format, params object[] args )
        {
            LogInfoIndentLine( format, ( SecureConsole.CursorLeft == 0 ) ? 1 : 0, args );
        }

        public void LogProgressStart( string format, params object[] args )
        {
            LogInfo( format + "...", args );
            m_LastProgressPoint = new Point2D( SecureConsole.CursorLeft, SecureConsole.CursorTop );
        }

        public void LogProgressContinue( int value, int maximum )
        {
            SecureConsole.CursorLeft = m_LastProgressPoint.X;
            SecureConsole.CursorTop = m_LastProgressPoint.Y;
            LogInfo( "{0:P} [{1}/{2}]", value / (float)maximum, value, maximum );
        }

        /// <summary>
        /// Perform a Red SecureConsole.Write (no line feed)
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="indents">Valid only with writeheader = ConsoleHeaderStyles.None, print 'DefaultIndentString' indents times</param>
        /// <param name="args">Args used to format string</param>
        public void LogErrorIndent( string format, int indents, params object[] args )
        {
            FullLog( format, ( m_LastPackageLogWritten != this || m_LastPackageLogWrittenPos.Y != SecureConsole.CursorTop ) ? ConsoleHeaderStyles.Short : ConsoleHeaderStyles.None, indents, ConsoleColor.Red, false, args );
        }

        /// <summary>
        /// Perform a Red SecureConsole.Write (no line feed)
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="args">Args used to format string</param>
        public void LogError( string format, params object[] args )
        {
            LogErrorIndent( format, ( SecureConsole.CursorLeft == 0 ) ? 1 : 0, args );
        }



        /// <summary>
        /// Perform a sequence of LogErrorLine printing error
        /// </summary>
        /// <param name="ex">catched exception</param>
        public void LogError( Exception ex )
        {
            var e = ex;
            while( e != null )
            {
                LogErrorLine( e.GetType().FullName + ": " + e.Message );
                LogErrorLine( "--- StackTrace ---" );
                LogErrorLine( e.StackTrace );
                e = e.InnerException;
                if( e != null )
                {
                    LogErrorLine( "--- INNER Exception of {0} ---", e.GetType().FullName );
                }
            }
        }

        /// <summary>
        /// Perform a Red SecureConsole.WriteLine
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="indents">Valid only with writeheader = ConsoleHeaderStyles.None, print 'DefaultIndentString' indents times</param>
        /// <param name="args">Args used to format string</param>        
        public void LogErrorIndentLine( string format, int indents, params object[] args )
        {
            FullLog( format, ( m_LastPackageLogWritten != this || m_LastPackageLogWrittenPos.Y != SecureConsole.CursorTop ) ? ConsoleHeaderStyles.Short : ConsoleHeaderStyles.None, indents, ConsoleColor.Red, true, args );
        }

        /// <summary>
        /// Perform a Red SecureConsole.WriteLine
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="args">Args used to format string</param>        
        public void LogErrorLine( string format, params object[] args )
        {
            LogErrorIndentLine( format, ( SecureConsole.CursorLeft == 0 ) ? 1 : 0, args );
        }

        /// <summary>
        /// Perform a Yellow SecureConsole.Write (no line feed)
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="indents">Valid only with writeheader = ConsoleHeaderStyles.None, print 'DefaultIndentString' indents times</param>
        /// <param name="args">Args used to format string</param>
        public void LogWarningIndent( string format, int indents, params object[] args )
        {
            FullLog( format, ( m_LastPackageLogWritten != this || m_LastPackageLogWrittenPos.Y != SecureConsole.CursorTop ) ? ConsoleHeaderStyles.Short : ConsoleHeaderStyles.None, indents, ConsoleColor.Yellow, false, args );
        }

        /// <summary>
        /// Perform a Yellow SecureConsole.Write (no line feed)
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="args">Args used to format string</param>
        public void LogWarning( string format, params object[] args )
        {
            LogWarningIndent( format, ( SecureConsole.CursorLeft == 0 ) ? 1 : 0, args );
        }

        /// <summary>
        /// Perform a Yellow SecureConsole.WriteLine
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="indents">Valid only with writeheader = ConsoleHeaderStyles.None, print 'DefaultIndentString' indents times</param>
        /// <param name="args">Args used to format string</param>
        public void LogWarningIndentLine( string format, int indents, params object[] args )
        {
            FullLog( format, ( m_LastPackageLogWritten != this || m_LastPackageLogWrittenPos.Y != SecureConsole.CursorTop ) ? ConsoleHeaderStyles.Short : ConsoleHeaderStyles.None, indents, ConsoleColor.Yellow, true, args );
        }

        /// <summary>
        /// Perform a Yellow SecureConsole.WriteLine
        /// </summary>
        /// <param name="format">formatted string to write</param>
        /// <param name="args">Args used to format string</param>
        public void LogWarningLine( string format, params object[] args )
        {
            LogWarningIndentLine( format, ( SecureConsole.CursorLeft == 0 ) ? 1 : 0, args );
        }

        #endregion

        #endregion

        #region StopWatch methods

       private Stopwatch m_InternalWatcher;

        public void StartWatcher( string logLine, bool pauseNetStates )
        {
            if( m_InternalWatcher == null )
                m_InternalWatcher = Stopwatch.StartNew();
            else
            {
                m_InternalWatcher.Reset();
                m_InternalWatcher.Start();
            }

            LogInfo( "{0}...", logLine );

            if( pauseNetStates )
            {
                NetState.FlushAll();
                NetState.Pause();
            }
        }

        public void StopWatcher( bool pauseNetStates )
        {
            if( m_InternalWatcher == null )
                return;

            if( m_InternalWatcher.IsRunning )
                m_InternalWatcher.Stop();

            LogInfoLine( "done in {0:F2} seconds.", m_InternalWatcher.Elapsed.TotalSeconds );

            if( pauseNetStates )
                NetState.Resume();
        }

        #endregion

        /// <summary>
        /// Invoke a Static method of this package (package must be enabled).
        ///  Do not use Overwritten method, because this function extract only first method of passed name.
        ///  BindingFlags: Static | Public | NonPublic
        /// </summary>
        /// <param name="method">Method to call</param>
        /// <param name="args">Arguments passed to method</param>
        /// <returns>Method returns or NULL if this package is disabled</returns>
        public object Invoke( string method, params object[] args )
        {
            if( !Enabled )
                return null;

            var m = ExtractMethod( method );
            if( m == null )
                return null;

            return m.Invoke( null, args );
        }

        internal MethodInfo ExtractMethod( string method )
        {
            return Type.GetMethod( method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );
        }
    }
}