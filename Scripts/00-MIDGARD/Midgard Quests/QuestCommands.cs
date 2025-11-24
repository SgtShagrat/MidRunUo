/***************************************************************************
 *                               QuestCommands.cs
 *
 *   begin                : 08 ottobre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Commands;
using Server.Engines.Quests;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Commands
{
    public class QuestCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register( "OggettoQuest", AccessLevel.Player, new CommandEventHandler( ToggleQuestItem_OnCommand ) );
            CommandSystem.Register( "ObiettivoQuest", AccessLevel.Player, new CommandEventHandler( ViewQuest_OnCommand ) );
            CommandSystem.Register( "CancellaQuest", AccessLevel.Player, new CommandEventHandler( CancelQuest_OnCommand ) );
            CommandSystem.Register( "Quest", AccessLevel.Player, new CommandEventHandler( ManageStaticQuest_OnCommand ) );
        }

        [Usage( "Quest <oggetto|obiettivo|cancella>" )]
        [Description( "Gestisce le quest statiche." )]
        public static void ManageStaticQuest_OnCommand( CommandEventArgs e )
        {
            PlayerMobile player = e.Mobile as PlayerMobile;
            if( player == null )
                return;

            if( e.Length != 1 )
            {
                player.SendMessage( "Uso del comando: Quest <oggetto|obiettivo|cancella>" );
                return;
            }

            switch( e.GetString( 0 ) )
            {
                case "oggetto":
                    {
                        player.SendMessage( "Target the item you wish to toggle Quest Item status on <ESC> to cancel." );
                        player.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleQuestItem_Callback ) );
                    }
                    break;
                case "obiettivo":
                    {
                        QuestSystem qs = player.Quest;
                        if( qs != null && qs.Objectives != null && qs.Objectives.Count > 0 )
					        qs.ShowQuestLog(); //è meglio mostrare tutto il log? (magiusche)
                            //qs.ShowQuestLogUpdated();
                    }
                    break;
                case "cancella":
                    {
                        QuestSystem qs = player.Quest;
                        if( qs != null )
                            qs.BeginCancelQuest();
                    }
                    break;
                default:
                    player.SendMessage( "Uso del comando: Quest <oggetto|obiettivo|cancella>" );
                    break;
            }
        }

        [Usage( "ObiettivoQuest" )]
        [Description( "Visualizza l'obiettivo della quest storica attuale" )]
        public static void ViewQuest_OnCommand( CommandEventArgs e )
        {
            PlayerMobile player = e.Mobile as PlayerMobile;
            if( player == null )
                return;

            QuestSystem qs = player.Quest;
            if( qs != null && qs.Objectives != null && qs.Objectives.Count > 0 )
                qs.ShowQuestLogUpdated();
        }

        [Usage( "OggettoQuest" )]
        [Description( "Setta un oggetto come usabile in una quest." )]
        public static void ToggleQuestItem_OnCommand( CommandEventArgs e )
        {
            Mobile m = e.Mobile;
            if( m == null || !m.CheckAlive() )
                return;

            m.SendMessage( "Target the item you wish to toggle Quest Item status on <ESC> to cancel" );
            m.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleQuestItem_Callback ) );
        }

        private static void ToggleQuestItem_Callback( Mobile from, object obj )
        {
            if( from is PlayerMobile )
            {
                PlayerMobile player = (PlayerMobile)from;

                if( obj is Item )
                {
                    Item item = (Item)obj;

                    if( item.IsChildOf( player.Backpack ) )
                    {
                        if( !QuestHelper.CheckItem( player, item ) )
                            player.SendMessage( 0x23, "That item does not match any of your quest criteria." );
                    }
                }
                else
                    player.SendMessage( 0x23, "An item must be in your backpack (and not in a container within) to be toggled as a quest item." );

                player.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleQuestItem_Callback ) );
            }
        }

        [Usage( "CancellaQuest" )]
        [Description( "Forza l'abbandono della quest storica in corso" )]
        public static void CancelQuest_OnCommand( CommandEventArgs e )
        {
            PlayerMobile player = e.Mobile as PlayerMobile;
            if( player == null )
                return;

            QuestSystem qs = player.Quest;
            if( qs != null )
                qs.BeginCancelQuest();
        }
    }
}