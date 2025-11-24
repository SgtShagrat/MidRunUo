/***************************************************************************
 *                                  FixCaps.cs
 *                            		-------------------
 *  begin                	: Settembre, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  Fixa i Skills.Cap per i primi 10 account importati su Midgard.
 *  
 ***************************************************************************/

using System;
using System.Collections;
using System.IO;

using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Commands
{
	public class FixCaps
	{
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "FixCaps" , AccessLevel.Developer, new CommandEventHandler( FixCaps_OnCommand ) );
		}
		#endregion

		#region callback
		[Usage( "FixCaps" )]
		[Description( "Fixa i caps" )]
		public static void FixCaps_OnCommand( CommandEventArgs e )
		{
			string NomeFileLog = "UsoFixCaps.txt";
      		ArrayList MobileArray = null;
      		
            try
            {
                MobileArray = new ArrayList(World.Mobiles.Values);
            }
            catch 
            { 
            	e.Mobile.SendMessage( "Errore di lettura della tabella dei Mobiles." );
            	return;
            }
            
            TextWriter tw = File.AppendText(NomeFileLog);
			tw.WriteLine( 	"L'utente: " + e.Mobile.Name + " (Account: " + e.Mobile.Account + " ) ha usato il comando [fixCaps" +
	       	  				" alle ore: " + DateTime.Now.ToShortTimeString() + " del " +
	          				DateTime.Now.Date.ToShortDateString() + "." );
            
            foreach( Mobile m in MobileArray )
 			{
      			if( m == null || m.Deleted ) 
      					continue;
      			
      			PlayerMobile From = m as PlayerMobile;
      			if( From == null || From.Deleted ) 
      					continue;
      			
	            for( int s=0; s<From.Skills.Length; s++ )
				{
	      			SkillName sn = (SkillName)s;
	      			double valore = From.Skills[s].Base;
	      			
	      			if( valore <= 100.0 )
	      				continue;
	      			
	      			if( valore > 100.0 && valore <= 110.0 )
	      			{
	      				tw.WriteLine( "Il comando ha settato a 110 il caps della skill " + sn.ToString() + " al pg " + From.Name + "." );
	      				From.Skills[s].Cap = 110.0;
	      			}
	      			else if( valore > 110.0 && valore <= 115.0 )
	      			{
	      				tw.WriteLine( "Il comando ha settato a 115 il caps della skill " + sn.ToString() + " al pg " + From.Name + "." );
	      				From.Skills[s].Cap = 115.0;
	      			}
	      			else if( valore > 115.0 && valore <= 120.0 )
	      			{
	      				tw.WriteLine( "Il comando ha settato a 120 il caps della skill " + sn.ToString() + " al pg " + From.Name + "." );
	      				From.Skills[s].Cap = 120.0;
	      			}
	    			else
	    			{
	    				tw.WriteLine( "Il pg {0} ha una skill {1} a {2} con skillcap anomalo ( {3} )" , 
	    				               From.Name, sn.ToString(),valore.ToString(), From.Skills[s].Cap.ToString() );
	    			}
				}
            }
            tw.Close();
		}
		#endregion
	}
}

