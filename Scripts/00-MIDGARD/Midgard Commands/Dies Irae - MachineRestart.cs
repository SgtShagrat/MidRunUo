/***************************************************************************
 *                                      MachineRestart.cs
 *                            		------------------------
 *  begin                	: Febbraio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 *  
 ***************************************************************************/

using System;
using System.Diagnostics;
using System.IO;

using Server;
using Server.Commands;
using Server.Gumps;

namespace Midgard.Commands
{
	public class MachineRestart
	{
		#region constants
		public static readonly string batPath = Path.Combine( Core.BaseDirectory, "ShutDownStarter.bat" );
		#endregion
		
		#region registration
		public static void Initialize()
		{
			CommandSystem.Register( "MachineRestart" , AccessLevel.Developer, new CommandEventHandler( MachineRestart_OnCommand ) );
			CommandSystem.Register( "BuildShutDownStarterFile" , AccessLevel.Developer, new CommandEventHandler( BuildShutDownStarterFile_OnCommand ) );
		}
		#endregion

		#region metodi
		[Usage( "BuildShutDownStarterFile" )]
		[Description( "Build ShutDownStarter.bat " )]
		public static void BuildShutDownStarterFile_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null  )
				return;
			
			if( e.Length == 0 )
			{ 
				using( TextWriter tw = File.CreateText( batPath ) )
				{
					tw.WriteLine( "\"PsTools\\pskill\" RunUO /accepteula" );
					tw.WriteLine( "\"PsTools\\psshutdown\" -c -r -f -t 60 -m \"Riavvio di sistema " +
					              "invocato da Dies Irae, Developer di Midgard\" /accepteula" );
				}
				from.SendMessage( "File ShutDownStarter.bat created." );
		    }
			else
			{
				from.SendMessage( "Command use: [BuildShutDownStarterFile" );
			}
		}
		
		[Usage( "MachineRestart" )]
		[Description( "Restart the entire machine" )]
		public static void MachineRestart_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null )
				return;
			
			if( e.Length == 0 )
			{  		
				if( File.Exists( batPath ) )
				{
					from.SendGump( new WarningGump( 1060635, 30720, "You are going to force a restart on this Machine.<br>" +
					               "This instance of RunUO will save and then shutdown.<br>" +
					               "Any other instance of RunUO will terminate before restarting without any save.<br>" +
					               "Are you sure do you want to proceed?",
					               0xFFC000, 420, 280, new WarningGumpCallback( ConfirmRestartCallBack ), new object[]{}, true ) );
		    	}
				else
				{
					from.SendMessage( "Unable to proceed: RunUO\\ShutDownStarter.bat does not exist. Create it with BuildShutDownStarterFile." );
				}				
		    }
			else
			{
				from.SendMessage( "Command use: [MachineRestart" );
			}
		}
		
      	private static void ConfirmRestartCallBack( Mobile from, bool okay, object state )
      	{
      		if( okay )
      		{
      			from.SendMessage( "You have decided to proceede." );
      			
				CommandSystem.Handle( from, "[Save" );
				ShellExecute( batPath, string.Empty, string.Empty, Path.GetDirectoryName( batPath ), false, false );
				Core.Process.Kill();
      		}
      	}

		/// <summary>
		/// Start a process with optional parameters
		/// </summary>
		/// <param name="filepath">path of file to execute</param>
		/// <param name="verb">verb of the new process</param>
		/// <param name="parameters">optionals parameters for the new process</param>
		/// <param name="workingdir">optional working directory of the process</param>
		/// <param name="WaitforExit">true if process will wait before exit</param>
		/// <param name="Hidden">true if process will be in hidden mode</param>
	 	public static int ShellExecute( string filepath, string verb, string parameters, string workingdir, bool WaitforExit, bool Hidden )
        {
            Process myprocess = new Process();
            myprocess.StartInfo.FileName = filepath;
            myprocess.StartInfo.WorkingDirectory = workingdir;
            myprocess.StartInfo.Verb = verb;
            myprocess.StartInfo.Arguments = parameters;
            if( Hidden ) 
            	myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            myprocess.Start();
            if( WaitforExit )
            {
                myprocess.WaitForExit();
                return myprocess.ExitCode;
            }
            
            return 0;
        }
		#endregion
	}
}
