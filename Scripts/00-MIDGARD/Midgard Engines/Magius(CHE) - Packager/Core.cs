/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 06 agosto, 2009
 *   author               :	Magius(CHE)
 *   copyright            : (C) Midgard Shard - Magius(CHE)		
 *
 ***************************************************************************/
/*  Install:
 *      To transform a normal script into a packaged one, simply follow
 *      these steps:
 *      1) Add "public static object[] Package_Info" with preferred info
 *          into script main class (where Conficure & Initialize are)
 *      2) Rename Configure into Package_Configure
 *      3) Rename Initialize into Package_Initialize
 *      
 *      have fun.
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using Server;

namespace Midgard.Engines.Packager
{
    public class Core
    {
        public static object[] Package_Info = {
                                                  "Script Title", "Packager Engine",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version(1, 0, 0, 0),
                                                  "Author name", "Magius(CHE)",
                                                  "Creation Date", new DateTime(2009, 08, 06),
                                                  "Author mail-contact", "cheghe@tiscali.it",
                                                  "Author home site", "http://www.magius.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Magius(CHE",
                                                  "Provided packages", new string[] {"Midgard.Engines.Packager"},
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] {"Package", "Engine"}
                                              };

        /// <summary>
        /// All loaded packages are stored here
        /// </summary>
        private readonly List<Package> m_Packages = new List<Package>();

        /// <summary>
        /// Incompatibility Errors coucth during LoadPackages
        /// </summary>
        private Dictionary<Type, Exception> m_Errors = new Dictionary<Type, Exception>();

        /// <summary>
        /// Classes that contain Package_info
        /// </summary>
        private int m_ValidPkgs = 0;

        /// <summary>
        /// Noone can instantiate me.
        /// </summary>
        private Core()
        {
        }

        /// <summary>
        /// Singleton Pattern
        /// </summary>
        public static Core Singleton { get; private set; }

        /// <summary>
        /// Public accessor (m_Packages) :-)
        /// </summary>
        public List<Package> Packages
        {
            get { return m_Packages; }
        }

        /// <summary>
        /// Return package of Pakager class.
        /// </summary>
        public Package MyPackage
        {
            get { return this[ this ]; }
        }

        /// <summary>
        /// Indexer used to retrive package from an object or its Type
        /// </summary>
        /// <param name="refobj">Any object or its type</param>
        /// <returns>Associated Package structure</returns>
        public Package this[ object refobj ]
        {
            get
            {
                if( refobj is Type )
                {
                    lock( m_Packages )
                    {
                        foreach( var pkg in m_Packages )
                            if( pkg.Type == refobj )
                                return pkg;
                        return null;
                    }
                }
                return this[ refobj.GetType() ];
            }
        }

        /// <summary>
        /// Called by ScriptCompiler due to initialization.
        /// </summary>
        public static void Configure()
        {
            Singleton = new Core();
            Singleton.LoadPackages();
            SecureConsole.Write( ">> " );
            Singleton.MyPackage.WriteHeaderToConsole( true );
            SecureConsole.WriteLine( ": configures all packages." );
            SecureConsole.WriteLine();
            var x = SecureConsole.CursorTop;
            Singleton.BroadcastInvokeWithCallPriority( "Package_Configure" );
            if( SecureConsole.CursorTop != x )
                SecureConsole.WriteLine();
            SecureConsole.Write( "<< " );
            Singleton.MyPackage.WriteHeaderToConsole();
            SecureConsole.WriteLine( ": all packages configured." );
            SecureConsole.WriteLine();
        }

        /// <summary>
        /// Called by ScriptCompiler due to initialization (after all configuration is completed).
        /// </summary>
        public static void Initialize()
        {
            SecureConsole.WriteLine();
            SecureConsole.Write( ">> " );
            Singleton.MyPackage.WriteHeaderToConsole( true );
            SecureConsole.WriteLine( " initializes all packages." );
            SecureConsole.WriteLine();
            var x = SecureConsole.CursorTop;
            Singleton.BroadcastInvokeWithCallPriority( "Package_Initialize" );
            if( SecureConsole.CursorTop != x )
                SecureConsole.WriteLine();
            SecureConsole.Write( "<< " );
            Singleton.MyPackage.WriteHeaderToConsole( true );
            SecureConsole.WriteLine( ": all packages initialized." );
            SecureConsole.WriteLine();
        }

        /// <summary>
        /// Check all loaded classes to create package structure. 
        /// </summary>
        private void LoadPackages()
        {
            Utility.PushColor( ConsoleColor.DarkGreen );
            SecureConsole.Write( "" + Package_Info[ 1 ] + ": Loading scripts packages...[ " );
            lock( m_Packages )
            {
                m_Errors.Clear();
                m_ValidPkgs = 0;
                m_Packages.Clear();
                var initialx = SecureConsole.CursorLeft;
                var lastwrittensize = 0;
                int count = 0;
                var lastLogCheck = DateTime.MinValue;
				var loadedTypes = new List<Type>();
                foreach( var asse in ScriptCompiler.Assemblies )
                    foreach( var type in asse.GetTypes() )
					if (!loadedTypes.Contains(type))
                    {
                        /*if(type.FullName.StartsWith("Midgard.Engines.Packages"))
                        {
                            int a = 1;
                        }*/
                        object pkinfo = type.GetField( "Package_Info", BindingFlags.Static | BindingFlags.Public );
                        if( pkinfo == null )
                            pkinfo = type.GetProperty( "Package_Info", BindingFlags.Static | BindingFlags.Public );
                        Package pkg = null;
                        if( pkinfo != null )
                        {
                            //this is a package script....store it
                            pkg = LoadPackage( type, true );
                        }
                        else
                        {
                            //is executable?
                            var m = type.GetMethod( "Configure", BindingFlags.Static | BindingFlags.Public );
                            if( m == null )
                                m = type.GetMethod( "Initialize", BindingFlags.Static | BindingFlags.Public );
                            if( m != null )
                                pkg = LoadPackage( type, false );
                        }
                        if( pkg != null )
					    {
                            m_Packages.Add( pkg );
						    loadedTypes.Add ( type );
					    }

                        if( DateTime.Now.Subtract( lastLogCheck ).TotalMilliseconds > 100 )
                        {
                            var towrite = m_Packages.Count + " / " + count + " ]";
                            SecureConsole.CursorLeft = initialx;
                            SecureConsole.Write( towrite );
                            if( lastwrittensize - towrite.Length > 0 )
                                SecureConsole.Write( new string( ' ', lastwrittensize - towrite.Length ) );
                            lastwrittensize = towrite.Length;
                            lastLogCheck = DateTime.Now;
                        }

                        count++;
                    }
                m_Packages.Sort();

                {
                    var towrite = m_Packages.Count + " / " + count + " ]";
                    SecureConsole.CursorLeft = initialx;
                    SecureConsole.Write( towrite );
                    if( lastwrittensize - towrite.Length > 0 )
                        SecureConsole.Write( new string( ' ', lastwrittensize - towrite.Length ) );
                }
            }
            SecureConsole.WriteLine();
            Utility.PopColor();
            if( m_Errors.Count > 0 )
            {
                Utility.PushColor( ConsoleColor.Red );
                SecureConsole.WriteLine( " - There are " + m_Errors.Count + " classes that cannot be loaded for errors:" );
                Utility.PushColor( ConsoleColor.DarkRed );
                foreach( var elem in m_Errors )
                {
                    SecureConsole.WriteLine( "   [" + elem.Key.FullName + "]: " + elem.Value.GetType().Name + " -> " + elem.Value.Message );
                    SecureConsole.WriteLine("    StackTrace: " + elem.Value.StackTrace);
                }
                Utility.PopColor();
                Utility.PopColor();
            }
            Utility.PushColor( ConsoleColor.DarkYellow );
            SecureConsole.WriteLine( " - Succesfully loaded " + m_Packages.Count + " class. " + m_ValidPkgs + " are valid packages." );
            Utility.PopColor();
            this[ this ].LogInfo( "Resolving conflicts..." );
            CheckPackageConflicts();
            this[ this ].LogInfoLine( "done." );
        }

        /// <summary>
        /// Check conflicts for all packages.
        /// </summary>
        private void CheckPackageConflicts()
        {
            lock( m_Packages )
            {
                foreach( var pkg in m_Packages )
                {
                    var toDisable = false;
                    if( !pkg.IsRequirementsSatisfied )
                    {
                        SecureConsole.WriteLine();
                        SecureConsole.Write( " - " );
                        pkg.WriteHeaderToConsole();
                        SecureConsole.Write( ": " );
                        Utility.PushColor( ConsoleColor.Red );
                        SecureConsole.Write( "missing requirement \"" + pkg.LastRequirementNotSatisfied + "\"." );
                        Utility.PopColor();
                        toDisable = true;
                    }
                    if( !toDisable )
                        foreach( var mpkg in m_Packages )
                            if( pkg != mpkg && pkg.ConflictsWith( mpkg ) )
                            {
                                SecureConsole.WriteLine();
                                SecureConsole.Write( " - " );
                                pkg.WriteHeaderToConsole();
                                SecureConsole.Write( ": " );
                                Utility.PushColor( ConsoleColor.Red );
                                SecureConsole.Write( "conflicts with \"" + mpkg + "\"." );
                                Utility.PopColor();
                                toDisable = true;
                            }
                    if( toDisable )
                    {
                        pkg.Enabled = false;
                        if( !pkg.HasPackageInitiAndConfig )
                            SecureConsole.Write( " (Beware: Initialization & Configuration cannot be disabled!) " );
                        SecureConsole.WriteLine( "Disabled!" );
                        SecureConsole.Write( " ..." );
                    }
                    else
                        pkg.Enabled = pkg.EnabledbyDefault;
                }
            }
        }

        /// <summary>
        /// Loa da single package.
        /// </summary>
        /// <param name="typeToLoad">Type to load</param>
        /// <param name="isPackage">true if passed type is a valid package</param>
        /// <returns>Created package structure</returns>
        private Package LoadPackage( Type typeToLoad, bool isPackage )
        {
            try
            {
                if( isPackage )
                    m_ValidPkgs++;
                return new Package( typeToLoad );
            }
            catch( Exception ex )
            {
                m_Errors.Add( typeToLoad, ex );
                return null;
            }
        }

        /// <summary>
        /// Invoke passed Method on all enabled Packages (DO NOT consider CallPriority)
        /// </summary>
        /// <param name="method">Method to call</param>
        /// <param name="args">Method's Arguments</param>
        public void BroadcastInvoke( string method, params object[] args )
        {
            lock( m_Packages )
            {
                foreach( var pkg in m_Packages )
                {
                    pkg.Invoke( method, args );
                }
            }
        }

        /// <summary>
        /// Invoke passed Method of all enabled Packages (based on CallPriority)
        /// </summary>
        /// <param name="method">Method to call</param>
        /// <param name="args">Method's Arguments</param>
        public void BroadcastInvokeWithCallPriority( string method, params object[] args )
        {
            lock( m_Packages )
            {
                var lst = new List<MethodInfo>();
                var attmtd = new Dictionary<MethodInfo, Package>();
                foreach( var pkg in m_Packages )
                {
                    var m = pkg.ExtractMethod( method );
                    if( m != null )
                    {
                        lst.Add( m );
						attmtd.Add( m, pkg );
                    }
                }

                lst.Sort( new CallPriorityComparer() );
				
				//var skipped_pkgs = new List<Package>();
				var executed_pkgs = new List<Package>();
				
				while(executed_pkgs.Count<lst.Count)				
	                foreach( var invoke in lst )
	                {
						var pkg = attmtd[ invoke ];
						var executed = false;
	
					    foreach( var req in pkg.ExistingRequiredPackages )
						{
							if (!executed_pkgs.Contains(req))
							{
								continue;
							}
						}
					
					    if( method == "Package_Initialize" )
	                        pkg.WriteInvokeBeginToConsole( "Init", ConsoleColor.Gray );
	                    else if( method == "Package_Configure" )
	                        pkg.WriteInvokeBeginToConsole( "Conf", ConsoleColor.Gray );
						
	                    invoke.Invoke( null, args );
						executed = true;
	                    if( method == "Package_Initialize" )
	                    {
	                        if( pkg.Enabled )
							{
	                            pkg.WriteInvokeEndToConsole( "OK", ConsoleColor.DarkGreen );
								//pkg.Initialized = true;
							}
	                        else
	                            pkg.WriteInvokeEndToConsole( "Disabled", ConsoleColor.Red );
							
	                    }
	                    else if( method == "Package_Configure" )
						{
	                        pkg.WriteInvokeEndToConsole( "OK", ConsoleColor.DarkGreen );
							//pkg.Configured = true;
						}
						
						if (executed)
							//skipped_pkgs.Add(pkg);
						//else
							executed_pkgs.Add(pkg);
	                }
            }
        }

        /// <summary>
        /// All package will be enabled or disabled (Prevent BroadcastInvoke)
        /// </summary>
        /// <param name="enable">True or False</param>
        public void EnableAllPackage( bool enable )
        {
            lock( m_Packages )
            {
                foreach( var pkg in m_Packages )
                {
                    pkg.Enabled = false;
                }
            }
        }
    }
}