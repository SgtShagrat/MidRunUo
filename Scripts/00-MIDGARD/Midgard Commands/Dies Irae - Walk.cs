/***************************************************************************
 *                                  WalkCommand.cs
 *                            		-------------------
 *  begin                	: Maggio, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Obbliga un NPC a camminare verso un punto a scelta.
 * 
 ***************************************************************************/
 
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Commands
{
	public class WalkCommand
	{
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "Walk" , AccessLevel.Seer, new CommandEventHandler( Walk_OnCommand ) );
		}
		#endregion
	
		#region callback
		[Usage("Walk")] 
		[Description("Makes a npc walk to the targeted destination.")] 
		public static void Walk_OnCommand( CommandEventArgs e ) 
		{
			e.Mobile.SendMessage("Select the npc you want to move."); 
			e.Mobile.Target = new WalkTarget(); 
		}
		
		private class WalkTarget : Target 
		{
			public WalkTarget( ) : base( 12, false, TargetFlags.None ) 
			{
			}
			
			protected override void OnTarget( Mobile from, object targ ) 
			{
				BaseCreature b = targ as BaseCreature; 
				
				if( b != null ) 
				{
					from.SendMessage("Select the destination."); 
					from.Target = new PointTarget( b ); 
				} 
			} 
		}
		
		private class PointTarget : Target 
		{
			private BaseCreature bc;
		
			public PointTarget( BaseCreature b ) : base( 32, true, TargetFlags.None )
			{
				bc = b; 
			} 
			
			protected override void OnTarget(Mobile from, object targ) 
			{
				WayPoint w; 
				if( targ is WayPoint ) 
					w = targ as WayPoint; 
				else 
				{
					w = new WayPoint(); 
					w.Location = new Point3D( targ as IPoint3D ); 
					w.Map = from.Map; 
				} 
				bc.CurrentWayPoint = w; 
			} 
		} 
		#endregion
	}
}
