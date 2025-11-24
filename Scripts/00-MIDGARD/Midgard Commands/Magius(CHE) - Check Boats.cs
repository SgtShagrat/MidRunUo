using System;
using Server;
using Server.Commands;
using System.Collections.Generic;
using Server.Multis;
using Midgard.Multis;
namespace Midgard.Commands
{
	public class Check_Boats
	{
		public static void Initialize()
        {
            CommandSystem.Register( "checkboats", AccessLevel.GameMaster, new CommandEventHandler( OnCommand ) );
			CheckAllInvalidBoats(null);
        }
		
		private static Point3D BlueStartingPoint = new Point3D(2701,2862,-3);
		private static Point3D RedStartingPoint = new Point3D(2741,2164,-2);
        
		static void CheckAllInvalidBoats (Mobile controller)
		{
			Log(controller,"CheckBoats: Inizio controllo...");
			
			var toremove = new List<BaseBoat>();
			foreach(var item in Server.World.Items)
			{
				var bboat = item.Value as BaseBoat;
				if(bboat!=null)
				{
					if (bboat.TillerMan == null || bboat.Hold == null) // no tillerman invalid boat.
					{
						
						toremove.Add(bboat); 
					}
				}				
			}
			foreach(var boat in toremove)
			{
				var mobiles  = boat.GetMobilesOnBoat();
				foreach(var mob in mobiles)
				{
					mob.MoveToWorld( BlueStartingPoint , Server.Map.Felucca);
				}
				if (boat is Vessel)
					((Vessel)boat).CanBeDeleted=true;
				boat.Delete();
			}
			
			Log(controller,"CheckBoats: Controllo completato. Eliminate {0} boats non valide.",toremove.Count);
		}

        [Usage( "checkboats" )]
        [Description( "Controlla le barche e le cancella se non sono valide" )]
        private static void OnCommand( CommandEventArgs e )
		{
			
			CheckAllInvalidBoats(e.Mobile);
			
		}
		private static void Log(Mobile receiver, string format, params object[] args)
		{
			if (receiver==null)
				Console.WriteLine(string.Format(format,args));
			else
				receiver.SendMessage(string.Format(format,args));
		}
	}
}

