using System;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using System.Collections.Generic;
using Server;
namespace Midgard.Commands
{
	public class Preferences
	{
		public Preferences ()
		{
		}
		
		private static List<object[]> Notifications = new List<object[]>();
		
		public static void Initialize()
		{
			CommandSystem.Register( "prefs", AccessLevel.Player, new CommandEventHandler( OnCommand ) );
			
			Notifications.Add ( new []{ "Tank" , "NotificationPVMTankEnabled" });
			Notifications.Add ( new []{ "Raw Points" , "NotificationPolRawPointsEnabled" });
			Notifications.Add ( new []{ "Double Weapons" , "UseDoubleWeaponsTogether" });
		}
		
		[Usage( "prefs" )]
		[Description( "Mostra le preferenze" )]
		private static void OnCommand( CommandEventArgs e )
		{
			var m2m = e.Mobile as Midgard2PlayerMobile;
			if ( m2m == null )
				return;
			e.Mobile.SendGump( new PreferencesGump(m2m));
		}
		
		private class PreferencesGump : Midgard.Gumps.MidgardComplexGump
		{
			#region Design
			// Background
			protected override int MainBgId { get { return 9350; } }
			protected override int WindowsBgId { get { return 2620; } }
			
			// Hues
			protected override int TitleHue { get { return 662; } }
			protected override int GroupsHue { get { return 92; } }
			protected override int ColsHue { get { return 15; } }
			protected override int LabelsHue { get { return 87; } }
			protected override int DefaultValueHue { get { return 0x3F; } } // verde acido
			protected override int DoActionHue { get { return 414; } }
			protected override int HuePrim { get { return 92; } }
			protected override int HueSec { get { return 87; } }
			protected override int DisabledHue { get { return Colors.DarkGray; } } //999; // grigio
			
			// Buttons
			protected override int ListBtnIdNormal { get { return 2103; } }
			protected override int ListBtnIdPressed { get { return 2104; } }
			
			// bottone con la X
			protected override int DoActionBtnIdNormal { get { return 0xA50; } }
			protected override int DoActionBtnIdPressed { get { return 0xA51; } }
			
			// bottone con la -
			protected override int DoAction2BtnIdNormal { get { return 0xA54; } }
			protected override int DoAction2BtnIdPressed { get { return 0xA55; } }
			
			protected override int NextPageBtnIdNormal { get { return 0x15E1; } }
			protected override int NextPageBtnIdPressed { get { return 0x15E5; } }
			protected override int PrevPageBtnIdNormal { get { return 0x15E3; } }
			protected override int PrevPageBtnIdPressed { get { return 0x15E7; } }
			
			protected override int UpBtnIdNormal { get { return 5600; } }
			protected override int UpBtnIdPressed { get { return 5604; } }
			protected override int DownBtnIdNormal { get { return 5602; } }
			protected override int DownBtnIdPressed { get { return 5606; } }
			
			
			protected override int VertOffset { get { return 15; } }                // spazio generico verticale
			protected override int BtnWidth { get { return 27; } }                  // larghezza del generico bottone di azione
			protected override int BtnHeight { get { return 27; } }                 // altezza del generico bottone di azione
			
			protected override int LabelBtnOffsetX { get { return 5; } }            // distanza tra fine del bottone e inizio della sua label in X
			protected override int LabelBtnOffsetY { get { return 5; } }            // distanza tra fine del bottone e inizio della sua label in Y
			
			protected override int LabelsOffset { get { return 20; } }              // distanza tra un label nella main window e la successiva
			protected override int BtnsOffset { get { return 30; } }                // distanza tra un bottone e il successivo
			protected override int TitleHeight { get { return 40; } }               // altezza della finestra del titolo
			protected override int SubTitlesHeight { get { return 20; } }           // offset verticale per i sottotitoli
			protected override int HorBorder { get { return 10; } }                 // bordo tra la mainwindow e la fine del background
			
			protected override int MainWindowExtraVertOffset { get { return 15; } } // distanza in X tra inizio della MW e prima label
			protected override int MainWindowExtraHorOffset { get { return 15; } }  // distanza in Y tra inizio della MW e prima label

			protected override bool HasSubtitles { get { return false; } }          // se ha i sottotitoli
			protected override bool HasCloseBtn { get { return true; } }            // se ha il bottone 'close'
			
			protected override int NumLabels { get { return 10; } }
			protected override int NumButtons { get { return 2; } }
			protected override int MainWindowWidth { get { return 325; } }
			protected override string CloseBtnText { get { return "Annulla"; } }
			
			#region implemented abstract members of Midgard.Gumps.MidgardComplexGump
			public override bool CanUseActionButton (Mobile owner)
			{
				return true;
			}
			public override bool HasAccess (int flag)
			{
				return true;
			}
			#endregion
			#endregion
			private readonly Midgard2PlayerMobile Player;
			public PreferencesGump(Midgard2PlayerMobile player): base(player, 50, 50)
			{
				Player = player;
				Player.CloseGump( typeof( Midgard2PlayerMobile ) );
				
				Design();

				base.RegisterUse( typeof( Midgard2PlayerMobile ) );
			}
			private void Design()
			{
				AddPage( 0 );
	
				AddMainBackground();
				AddMainTitle( 190, "Preferenze Utente:" );
				AddMainWindow();
				
				int labelOffsetX = GetMainWindowLabelsX();
				int labelOffsetY = GetMainWindowFirstLabelY();

				AddLabel( labelOffsetX, labelOffsetY, TitleHue, "Notifiche:" );
				var offset = labelOffsetY + LabelsOffset;
				var count = 0;
				foreach(var not in Notifications)
				{
					var txt = (string)not[0];
					var prop = (string)not[1];
					
					AddLabel( labelOffsetX + 10, offset, LabelsHue, txt );
					AddCheck ( labelOffsetX + 100 , offset, DoActionBtnIdNormal, DoActionBtnIdPressed, NotificationValueBool( count ), count );
					offset+=LabelsOffset;
					count++;
				}
				
				AddActionButton( 1, "Salva", 1 );
				AddCloseButton();
			}
			private bool NotificationValueBool ( int notifyindex )
			{
				var notify = Notifications[notifyindex];
				var prop = Player.GetType().GetProperty( (string)notify[1] );
				if (prop == null)
					return false;
				return (bool) prop.GetValue(Player, null);
			}
			private void NotificationValue ( int notifyindex, bool newvalue )
			{
				var notify = Notifications[notifyindex];
				var prop = Player.GetType().GetProperty( (string)notify[1] );
				if (prop != null)
					prop.SetValue(Player, newvalue, null);
			}
			public override void OnResponse( NetState sender, RelayInfo info )
			{
				int id = info.ButtonID;
				
				switch (id)
				{
					case 1:
						SaveState(info);
						sender.Mobile.SendMessage("Preferenze: salvataggio riuscito.");
						//Player.SendGump( new PreferencesGump( Player ) );
						break;
					case 0:
						sender.Mobile.SendMessage("Preferenze: nessun cambiamento è stato salvato.");
						return;
				}
			}
			private void SaveState(RelayInfo info)
			{
				for(var h = 0 ; h< Notifications.Count; h++)
				{
					var founded = false;
					foreach(var sw in info.Switches)
					{
						if (sw == h)
						{
							//if (info.IsSwitched(h))
							//	sender.Mobile.SendMessage( "switched: "+h.ToString());
							NotificationValue( h , true);
							founded = true;
						}
					}
					if (!founded)
						NotificationValue( h , false);
				}
			}
		}		
	}
}

