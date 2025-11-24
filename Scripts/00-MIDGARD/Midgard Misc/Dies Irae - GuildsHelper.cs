/***************************************************************************
 *									GuildsHelper.cs
 *									-------------------
 *  begin					: Settembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  	Classe di supporto per le gilde.
 *  
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Commands.Generic;
using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Midgard.Misc
{
	public class GuildsHelper
	{
		public static void Initialize()
		{
			CommandSystem.Register( "VerifyUniforms", AccessLevel.Developer, new CommandEventHandler( VerifyUniforms_OnCommand ) );
			CommandSystem.Register( "GuildsList", AccessLevel.Seer, new CommandEventHandler( GuildsList_OnCommand ) );
			CommandSystem.Register( "GuildsReset", AccessLevel.Seer, new CommandEventHandler( GuildsReset_OnCommand ) );
			CommandSystem.Register( "GuildDisband", AccessLevel.Seer, new CommandEventHandler( GuildDisband_OnCommand ) );
			CommandSystem.Register( "AddGuildCloth", AccessLevel.Seer, new CommandEventHandler( AddGuildCloth_OnCommand ) );
			CommandSystem.Register( "ClearGuildUniform", AccessLevel.Seer, new CommandEventHandler( ClearGuildUniform_OnCommand ) );
			CommandSystem.Register( "SetMidgardGuildType", AccessLevel.Seer, new CommandEventHandler( SetMidgardGuildType_OnCommand ) );
			CommandSystem.Register( "SetTownGuild", AccessLevel.Seer, new CommandEventHandler( SetTownGuild_OnCommand ) );

			CommandSystem.Register( "GuildList", AccessLevel.Player, new CommandEventHandler( GuildOnline_OnCommand ) );
			CommandSystem.Register( "GCL", AccessLevel.Player, new CommandEventHandler( GuildOnline_OnCommand ) );

			CommandSystem.Register( "GuildChat", AccessLevel.Player, new CommandEventHandler( GuildChat_OnCommand ) );
			CommandSystem.Register( "GC", AccessLevel.Player, new CommandEventHandler( GuildChat_OnCommand ) );

			TargetCommands.Register( new GuildKickCommand( GuildKickType.Kick ) );
			TargetCommands.Register( new GuildKickCommand( GuildKickType.Ban ) );
			TargetCommands.Register( new GuildKickCommand( GuildKickType.Unban ) );

			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( GuildButtonOverride ) );
		}

		#region handlers
		public static void GuildButtonOverride()
		{
			PacketHandlers.RegisterEncoded( 0x28, true, new OnEncodedPacketReceive( GuildButtonOverride ) );
		}

		public static void GuildButtonOverride( NetState state, IEntity e, EncodedReader reader )
		{
			if( state == null || state.Mobile == null )
				return;

			Midgard2PlayerMobile from = state.Mobile as Midgard2PlayerMobile;

			if( from != null )
			{
				if( from.DisplayRegionalInfo )
					from.SendMessage( from.Language == "ITA" ? "Hai scelto di disabilitare le info regionali." : "You have chosen to disable the regional infos." );
				else
					from.SendMessage( from.Language == "ITA" ? "Hai scelto di abilitare le info regionali." : "You have chosen to enable the regional infos." );

				from.DisplayRegionalInfo = !from.DisplayRegionalInfo;
			}
		}

		[Usage( "VerifyUniforms" )]
		[Description( "Verify uniforms for all guilds" )]
		public static void VerifyUniforms_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 0 )
			{
				List<Item> toDelete = new List<Item>();

				foreach( BaseGuild bg in BaseGuild.List.Values )
				{
					Guild g = bg as Guild;

					if( g != null && g.GuildUniformInstances != null )
					{
						for( int i = 0; i < g.GuildUniformInstances.Count; i++ )
						{
							Item item = g.GuildUniformInstances[ i ] as Item;

							if( item == null || item.Deleted )
							{
								StringToLog( "Warning: Null pointer in UniformList list. Removing..." );
								UnRegisterUniform( null, g, item );
								continue;
							}

							IGuildUniform gItem = item as IGuildUniform;

							if( gItem == null )
								continue;
							else if( gItem.OwnerGuild == null || gItem.OwnerGuild != bg )
							{
								StringToLog( "Warning: Guild item with wrong or null guild. Removing..." );
								toDelete.Add( item );
							}
							else if( gItem.GuildedOwner == null )
							{
								StringToLog( "Warning: Guild item without Owner. Removing..." );
								toDelete.Add( item );
							}
							else if( !g.IsMember( gItem.GuildedOwner ) )
							{
								StringToLog( "Warning: Guild item with owner not member of cloth guild. Removing..." );
								toDelete.Add( item );
							}
						}

						for( int j = 0; j < toDelete.Count; j++ )
						{
							toDelete[ j ].Delete();
						}
					}
				}
			}
			else
				from.SendMessage( from.Language == "ITA" ? "Usa: [VerifyUniforms" : "Command Use: [VerifyUniforms" );
		}

		[Usage( "GuildsList" )]
		[Description( "List all guilds on the server" )]
		public static void GuildsList_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 0 )
			{
				from.SendMessage( from.Language == "ITA" ? "Ci sono le seguenti gilde su Midgard:" : "There are these guilds on Midgard Shard:" );
				foreach( BaseGuild bg in BaseGuild.List.Values )
				{
					from.SendMessage( from.Language == "ITA" ? "{0} - [{1}] - Tipo: {2} - TipoMidgard: {3} - Città: {4}" : "{0} - [{1}] - Type: {2} - MidgardType: {3} - Town: {4}", bg.Name, bg.Abbreviation, bg.Type,
									  ( (Guild)bg ).MidgardGuildType.ToString(), ( (Guild)bg ).Town.ToString() );
				}
			}
			else
				from.SendMessage( from.Language == "ITA" ? "Usa: [GuildsList" : "Command Use: [GuildsList" );
		}

		[Usage( "GuildDisband <GuildName>" )]
		[Description( "Disband a Guild by its name" )]
		public static void GuildDisband_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 1 )
			{
				string guildName = e.GetString( 0 );
				Guild guild = BaseGuild.FindByName( guildName ) as Guild;

				if( guild != null )
				{
					from.SendGump( new WarningGump( 1060635, 30720, from.Language == "ITA" ? "Sei sicuro di voler cancellare <em><basefont color=red>permanentemente</basefont></em>" +
													" la gilda " + guild.Name + " ?" : "Are you sure you want to disband <em><basefont color=red>permanently</basefont></em>" +
													" guild " + guild.Name + " ?", 0xFFC000, 420, 280,
													new WarningGumpCallback( ConfirmGuildDisbandCallBack ),
													new object[] { guild } ) );
				}
				else
				{
					from.SendGump( new NoticeGump( 1060635, 30720, from.Language == "ITA" ? "Nessuna gilda con quel nome presente su Midgard" : "No guild found on Midgard Shard with that name.",
														0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[] { } ) );
				}
			}
			else
			{
				from.SendMessage( from.Language == "ITA" ? "Usa: [GuildDisband <nomeDellaGilda>" : "Command Use: [GuildDisband <GuildName>" );
			}
		}

		[Usage( "GuildsReset" )]
		[Description( "Disband All Guilds" )]
		public static void GuildsReset_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 0 )
			{
				List<BaseGuild> toDisband = new List<BaseGuild>();
				StringBuilder sb = new StringBuilder();
				foreach( BaseGuild bg in BaseGuild.List.Values )
				{
					toDisband.Add( bg );
					sb.Append( bg.Name + "<br>" );
				}

				if( toDisband.Count > 0 )
				{
					from.SendGump( new WarningGump( 1060635, 30720, from.Language == "ITA" ? "Sei sicuro di voler sciogliere <em><basefont color=red>permanentemente</basefont></em>" +
													" queste gilde:<br>" + sb + "?" : "Are you sure you want to disband <em><basefont color=red>permanently</basefont></em>" +
													" these guilds:<br>" + sb + "?", 0xFFC000, 420, 280,
													new WarningGumpCallback( ConfirmGuildsResetCallBack ),
													new object[] { toDisband } ) );
				}
				else
				{
					from.SendGump( new NoticeGump( 1060635, 30720, from.Language == "ITA" ? "Non ci sono gilde da sciogliere su Midgard." : "There are no Guilds to disband on Midgard Shard.",
														0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[] { } ) );
				}
			}
			else
				from.SendMessage( from.Language == "ITA" ? "Usa: [GuildsReset" : "Command Use: [GuildsReset" );
		}

		[Usage( "AddGuildCloth <GuildName>" )]
		[Description( "Add target item in a guild uniform" )]
		public static void AddGuildCloth_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 1 )
			{
				string guildName = e.GetString( 0 );
				BaseGuild guild = BaseGuild.FindByName( guildName );

				if( guild == null )
				{
					from.SendGump( new NoticeGump( 1060635, 30720, from.Language == "ITA" ? "Hai inserito un nome di gilda errato.<br>" +
													   "Non esistono gilde con questo nome su Midgard." : "You have entered a wrong guild name.<br>" +
													   "There are not guilds with that name on Midgard Shard.",
														0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[] { } ) );
				}
				else
				{
					from.SendMessage( from.Language == "ITA" ? "Seleziona l'oggetto da aggiungere all'uniforme." : "Target an item you would like to add to this uniform." );
					from.Target = new GuildUniformTarget( guild );
				}
			}
			else
				from.SendMessage( from.Language == "ITA" ? "Usa: [AddGuildCloth <nomeDellaGilda>" : "Command Use: [AddGuildCloth <GuildName>" );
		}

		[Usage( "ClearGuildUniform <GuildName>" )]
		[Description( "Reset Uniform Items from a guild" )]
		public static void ClearGuildUniform_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 1 )
			{
				string guildName = e.GetString( 0 );
				BaseGuild guild = BaseGuild.FindByName( guildName );

				if( guild != null )
				{
					string msg = string.Format( from.Language == "ITA" ? "Sicuro di voler cancellare <em><basefont color=red>permanentemente</basefont></em> " +
												"l'uniforme della gilda {0} ?<br>" : "Are you sure you want to clear <em><basefont color=red>permanently</basefont></em> " +
												"uniform for guild {0} ?<br>", guild.Name );

					from.SendGump( new WarningGump( 1060635, 30720, msg, 0xFFC000, 420, 280, new WarningGumpCallback( ConfirmGuildUniformClearCallBack ),
													new object[] { guild } ) );
				}
				else
				{
					from.SendGump( new NoticeGump( 1060635, 30720, from.Language == "ITA" ? "Hai inserito un nome di gilda errato.<br>" +
													   "Non esistono gilde con questo nome su Midgard." : "You have entered a wrong guild name.<br>" +
												   "There are not guilds with that name on Midgard Shard.",
												   0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[] { } ) );
				}
			}
			else
				from.SendMessage( from.Language == "ITA" ? "Usa: [ClearGuildUniform <nomeDellaGilda>" : "Command Use: [ClearGuildUniform <GuildName>" );
		}

		[Usage( "SetMidgardGuildType <GuildName> <MidgardGuildType>" )]
		[Description( "Set the Midgard Guild Type ot target guild" )]
		public static void SetMidgardGuildType_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 2 )
			{
				string guildName = e.GetString( 0 );

				if( !string.IsNullOrEmpty( guildName ) )
				{
					Guild guild = (Guild)BaseGuild.FindByName( guildName );

					if( guild != null )
					{
						string guildType = e.GetString( 1 );

						if( !string.IsNullOrEmpty( guildType ) )
						{
							if( Enum.IsDefined( typeof( MidgardGuildTypes ), guildType ) )
							{
								guild.MidgardGuildType = (MidgardGuildTypes)Enum.Parse( typeof( MidgardGuildTypes ), guildType );
							}
							else
								from.SendMessage( from.Language == "ITA" ? "Il tipo inserito non è definito." : "That Midgard Guild Type is not defined." );
						}
						else
							from.SendMessage( from.Language == "ITA" ? "Hai inserito un tipo errato." : "You have supplied a wrong guild type string." );
					}
					else
						from.SendMessage( from.Language == "ITA" ? "Hai sbagliato il nome della gilda." : "You have entered a wrong guild name." );
				}
				else
					from.SendMessage( from.Language == "ITA" ? "Hai sbagliato il nome della gilda." : "You have supplied a wrong guild name." );
			}
			else
				from.SendMessage( from.Language == "ITA" ? "Usa: [SetMidgardGuildType <nomeGilda> <TipoGildaMidgard>" : "Command Use: [SetMidgardGuildType <GuildName> <MidgardGuildType>" );
		}

		[Usage( "SetTownGuild <GuildName> <Town>" )]
		[Description( "Set the Midgard Town of target guild" )]
		public static void SetTownGuild_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 2 )
			{
				string guildName = e.GetString( 0 );

				if( !string.IsNullOrEmpty( guildName ) )
				{
					Guild guild = (Guild)BaseGuild.FindByName( guildName );

					if( guild != null )
					{
						string town = e.GetString( 1 );

						if( !string.IsNullOrEmpty( town ) )
						{
							if( Enum.IsDefined( typeof( MidgardTowns ), town ) )
							{
								guild.Town = (MidgardTowns)Enum.Parse( typeof( MidgardTowns ), town );
							}
							else
								from.SendMessage( from.Language == "ITA" ? "Quella città non è definita." : "That Midgard Guild Town is not defined." );
						}
						else
							from.SendMessage( from.Language == "ITA" ? "Hai sbagliato il nome della città." : "You have supplied a wrong town string." );
					}
					else
						from.SendMessage( from.Language == "ITA" ? "Hai sbagliato il nome della gilda." : "You have entered a wrong guild name." );
				}
				else
					from.SendMessage( from.Language == "ITA" ? "Hai sbagliato il nome della gilda." : "You have supplied a wrong guild name." );
			}
			else
				from.SendMessage( from.Language == "ITA" ? "Usa: [SetMidgardGuildType <nomeGilda> <Città>" : "Command Use: [SetMidgardGuildType <GuildName> <MidgardTown>" );
		}

		[Aliases( "GCL" )]
		[Usage( "GuildList" )]
		[Description( "Lista tutti i gildati online." )]
		public static void GuildOnline_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( from.Guild == null || from.Guild.Disbanded )
				return;

			if( e.Length == 0 )
			{
				List<Mobile> list = new List<Mobile>();
				List<NetState> states = NetState.Instances;

				for( int i = 0; i < states.Count; ++i )
				{
					Mobile m = states[ i ].Mobile;
					if( m != null && !m.Deleted )
					{
						if( ( (Guild)from.Guild ).IsMember( m ) )
							list.Add( m );
					}
				}

				if( list.Count == 1 )
					from.SendMessage( from.Language == "ITA" ? "Non ci sono compagni di gilda online." : "There are no guild members online other then you." );
				else
				{
					from.SendMessage( from.Language == "ITA" ? "Membri di Gilda Online:" : "Guild Members Online:" );
					foreach( Mobile m in list )
					{
						if( m != null && !String.IsNullOrEmpty( m.Name ) && m.Map != null && m.Map != Map.Internal )
						{
							if( m.Region != m.Map.DefaultRegion && !String.IsNullOrEmpty( m.Region.Name ) )
								from.SendMessage( String.Format( "{0} ({1})", m.Name, m.Region.Name ) );
							else
								from.SendMessage( String.Format( "{0} ({1})", m.Name, m.Map.Name ) );
						}
					}
				}
			}
			else
				from.SendMessage( from.Language == "ITA" ? "Usa: [GuildList oppure [GCL" : "Command Use: [GuildList or [GCL" );
		}

		public static readonly bool GuildChatLoggedEnabled = true;

		[Aliases( "GC" )]
		[Usage( "GuildChat <testo>" )]
		[Description( "Manda un messaggio in broadcast a tutta la gilda." )]
		public static void GuildChat_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			Guild g = from.Guild as Guild;
			if( g == null || g.Disbanded )
				return;

			string message = e.ArgString;

			if( !string.IsNullOrEmpty( message ) )
			{
				g.GuildChat( from, message );

				SendToStaffMessage( from, "[Gilda]: {0}", message );

				if( GuildChatLoggedEnabled )
				{
					StringToLog( string.Format( "{0} - {1} - {2} - {3} - {4}",
						   DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(),
						   ( String.IsNullOrEmpty( g.Name ) ? "null  guild name" : g.Name ),
						   ( String.IsNullOrEmpty( from.Name ) ? "null name" : from.Name ),
						   ( String.IsNullOrEmpty( message ) ? "null message" : message ) ),
						   "Logs/GuildsChatLog.log" );
				}
			}
			else
				from.SendMessage( from.Language == "ITA" ? "Usa [GuildChat <messaggio> oppure [GC <messaggio>" : "Command Use: [GuildChat <message> or [GC <message>" );
		}

		private static void SendToStaffMessage( Mobile from, string text )
		{
			Packet p = null;

			foreach( NetState ns in from.GetClientsInRange( 8 ) )
			{
				Mobile mob = ns.Mobile;

				if( mob != null && mob.AccessLevel >= AccessLevel.GameMaster && mob.AccessLevel > from.AccessLevel )
				{
					if( p == null )
						p = Packet.Acquire( new UnicodeMessage( from.Serial, from.Body, MessageType.Regular, from.SpeechHue, 3, from.Language, from.Name, text ) );

					ns.Send( p );
				}
			}

			Packet.Release( p );
		}

		private static void SendToStaffMessage( Mobile from, string format, params object[] args )
		{
			SendToStaffMessage( from, String.Format( format, args ) );
		}

		public enum GuildKickType
		{
			Kick,
			Ban,
			Unban
		}

		public class GuildKickCommand : BaseCommand
		{
			private GuildKickType m_KickType;

			public GuildKickCommand( GuildKickType kickType )
			{
				m_KickType = kickType;

				AccessLevel = AccessLevel.Seer;
				Supports = CommandSupport.AllMobiles;
				ObjectTypes = ObjectTypes.Mobiles;

				switch( m_KickType )
				{
					case GuildKickType.Kick:
						{
							Commands = new string[] { "GuildKick" };
							Usage = "GuildKick";
							Description = "Kicks the targeted player out of his current guild. This does not prevent them from rejoining.";
							break;
						}
					case GuildKickType.Ban:
						{
							Commands = new string[] { "GuildBan" };
							Usage = "GuildBan";
							Description = "Bans the account of a targeted player from joining guilds. All players on the account are removed from their current guild, if any.";
							break;
						}
					case GuildKickType.Unban:
						{
							Commands = new string[] { "GuildUnban" };
							Usage = "GuildUnban";
							Description = "Unbans the account of a targeted player from joining guilds.";
							break;
						}
				}
			}

			public override void Execute( CommandEventArgs e, object obj )
			{
				Mobile mob = (Mobile)obj;

				switch( m_KickType )
				{
					case GuildKickType.Kick:
						{
							Guild g = mob.Guild as Guild;

							if( g != null )
							{
								g.RemoveMember( mob );
								mob.Guild = null;
								AddResponse( "They have been kicked from their guild." );
							}
							else
							{
								LogFailure( "They are not in a guild." );
							}

							break;
						}
					case GuildKickType.Ban:
						{
							Account acct = mob.Account as Account;

							if( acct != null )
							{
								if( acct.GetTag( "GuildBanned" ) == null )
								{
									acct.SetTag( "GuildBanned", "true" );
									AddResponse( "The account has been banned from joining guilds." );
								}
								else
								{
									AddResponse( "The account is already banned from joining guilds." );
								}

								for( int i = 0; i < acct.Length; ++i )
								{
									mob = acct[ i ];

									if( mob != null )
									{
										Guild g = mob.Guild as Guild;

										if( g != null )
										{
											g.RemoveMember( mob );
											AddResponse( "They have been kicked from their guild." );
										}
									}
								}
							}
							else
							{
								LogFailure( "They have no assigned account." );
							}

							break;
						}
					case GuildKickType.Unban:
						{
							Account acct = mob.Account as Account;

							if( acct != null )
							{
								if( acct.GetTag( "GuildBanned" ) == null )
								{
									AddResponse( "The account is not already banned from joining guilds." );
								}
								else
								{
									acct.RemoveTag( "GuildBanned" );
									AddResponse( "The account may now freely join guilds." );
								}
							}
							else
							{
								LogFailure( "They have no assigned account." );
							}

							break;
						}
				}
			}
		}
		#endregion

		#region callbacks
		private static void CloseNoticeCallback( Mobile From, object state )
		{
		}

		private static void ConfirmGuildDisbandCallBack( Mobile from, bool okay, object state )
		{
			object[] states = (object[])state;

			Guild guild = (Guild)states[ 0 ];

			if( okay )
			{
				from.SendMessage( from.Language == "ITA" ? "Hai deciso di procedere." : "You have decided to proceede." );
				if( guild != null )
				{
					from.SendMessage( from.Language == "ITA" ? "La gilda {0} è sciolta." : "Guild {0} has been disbanded.", guild.Name );
					guild.Disband();
				}
			}
		}

		private static void ConfirmGuildsResetCallBack( Mobile from, bool okay, object state )
		{
			object[] states = (object[])state;

			List<BaseGuild> ToDisband = (List<BaseGuild>)states[ 0 ];

			if( okay )
			{
				from.SendMessage( from.Language == "ITA" ? "Hai deciso di procedere." : "You have decided to proceede." );

				for( int i = 0; i < ToDisband.Count; i++ )
				{
					Guild g = ToDisband[ i ] as Guild;
					if( g != null )
					{
						from.SendMessage( from.Language == "ITA" ? "La gilda {0} è sciolta." : "Guild {0} has been disbanded.", g.Name );
						g.Disband();
					}
				}
			}
		}

		private static void ConfirmGuildUniformClearCallBack( Mobile from, bool okay, object state )
		{
			object[] states = (object[])state;

			BaseGuild guild = (BaseGuild)states[ 0 ];

			if( okay )
			{
				from.SendMessage( from.Language == "ITA" ? "Hai deciso di procedere." : "You have decided to proceede." );
				if( guild != null )
					ClearGuildUniform( (Guild)guild );
				else
					from.SendMessage( from.Language == "ITA" ? "Nessuna gilda modificata." : "No guild were processed." );
			}
		}
		#endregion

		/// <summary>
		/// Static. Initialize Midgard guild variables
		/// <param name="guild" >guild to initialize</param>
		/// </summary>
		public static void InitializeMidgardGuild( Guild guild )
		{
			guild.UniformList = new List<UniformEntry>();
			guild.GuildUniformInstances = new ArrayList();
			guild.Town = MidgardTowns.None;
			guild.MidgardGuildType = MidgardGuildTypes.None;
		}

		public static readonly int MaxCharsGuildedPerAccount = 6;

		/// <summary>
		/// Static. Check if from is a valid guild member
		/// <param name="mob" >mobile to check</param>
		/// </summary>
		public static bool CanJoinMidgardGuild( Mobile mob, Guild guild )
		{
			if( mob == null || mob.Deleted || guild == null || guild.Disbanded )
				return false;

			if( mob.AccessLevel > AccessLevel.Player )
				return true;

			if( IsGuildsBanned( mob ) )
				return false;

			//if( mob is Midgard2PlayerMobile && DateTime.Now <= ( (Midgard2PlayerMobile)mob ).LastGuildLeaving + TimeSpan.FromDays( 1.0 ) )
			//	return false;

			if( HasSameTownOfGuild( mob, guild ) )
			{
				if( !HasAlreadyMaxGuilded( mob ) )
				{
					// rimosso il criterio per cui si voleva un solo pg
					// warrior o worker per gilda
					/*
					switch( guild.MidgardGuildType )
					{
						case MidgardGuildTypes.Corporation: return GuildsHelper.CanJoinCommercialGuild( mob );
						case MidgardGuildTypes.Military: 	return GuildsHelper.CanJoinMilitarGuild( mob );
						default: 							return GuildsHelper.CanJoinAnyGuild( mob );
					}
					*/

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Static. Check if from is a valid guild leader for our guild
		/// <param name="mob" >mobile to check</param>
		/// </summary>
		public static bool IsValidGuildLeader( Mobile mob )
		{
			if( mob == null || mob.Deleted )
				return false;

			if( mob.AccessLevel > AccessLevel.Player )
				return true;

			if( IsGuildsBanned( mob ) )
				return false;

			if( !HasAlreadyMaxGuilded( mob ) )
				return true;

			return false;
		}

		/// <summary>
		/// Static. Check if from has been banned from guild system
		/// <param name="mob" >mobile to check</param>
		/// </summary>
		public static bool IsGuildsBanned( Mobile mob )
		{
			Account acct = mob.Account as Account;

			if( acct == null )
				return false;

			return ( acct.GetTag( "GuildBanned" ) != null );
		}

		/// <summary>
		/// Static. Check if from has same town status of our guild
		/// <param name="from" >mobile to check</param>
		/// <param name="guild" >guild to check</param>
		/// </summary>
		public static bool HasSameTownOfGuild( Mobile from, Guild guild )
		{/*
			if( from == null || from.Deleted )
				return false;

			if( guild != null && !guild.Disbanded )
			{
				if( guild.Town == MidgardTowns.None )
					return true;

				Midgard2PlayerMobile m2pm = from as Midgard2PlayerMobile;
				if( m2pm != null )
					return m2pm.Town == guild.Town;
			}

			return false;*/
			return true;//edit by arlas decisione di protezione in città diverse.
		}

		/// <summary>
		/// Static. Check if from has reached maximum chars guilded
		/// <param name="from" >mobile to check</param>
		/// </summary>
		public static bool HasAlreadyMaxGuilded( Mobile from )
		{
			if( from == null || from.Deleted )
				return false;

			Account acct = from.Account as Account;

			if( acct == null )
				return false;

			int charsInGuilds = 0;
			for( int i = 0; i < acct.Count; i++ )
			{
				Mobile m = acct[ i ];
				if( m != null && !m.Deleted )
				{
					if( m.Guild != null )
					{
						Guild g = m.Guild as Guild;
						if( g != null && !g.Disbanded )
							charsInGuilds++;
					}
				}
			}

			return charsInGuilds >= MaxCharsGuildedPerAccount;
		}

		/// <summary>
		/// Static. Check if from can join a militar guild
		/// <param name="from" >mobile to check</param>
		/// </summary>
		public static bool CanJoinMilitarGuild( Mobile from )
		{
			if( from == null || from.Deleted )
				return false;

			Account acct = from.Account as Account;

			if( acct == null )
				return false;

			bool hasMilitarGuilded = false;
			for( int i = 0; i < acct.Length && !hasMilitarGuilded; i++ )
			{
				Mobile m = acct[ i ];
				if( m != null && !m.Deleted )
				{
					if( m.Guild != null )
					{
						Guild g = m.Guild as Guild;
						if( g != null && !g.Disbanded )
						{
							if( g.MidgardGuildType == MidgardGuildTypes.Military )
								hasMilitarGuilded = true;
						}
					}
				}
			}

			return !hasMilitarGuilded;
		}

		/// <summary>
		/// Static.Check if from can join a commercial guild
		/// <param name="from" >mobile to check</param>
		/// </summary>
		public static bool CanJoinCommercialGuild( Mobile from )
		{
			if( from == null || from.Deleted )
				return false;

			Account acct = from.Account as Account;

			if( acct == null )
				return false;

			bool hasWorkerGuilded = false;
			for( int i = 0; i < acct.Length && !hasWorkerGuilded; i++ )
			{
				Mobile m = acct[ i ];
				if( m != null && !m.Deleted )
				{
					if( m.Guild != null )
					{
						Guild g = m.Guild as Guild;
						if( g != null && !g.Disbanded )
						{
							if( g.MidgardGuildType == MidgardGuildTypes.Military )
								hasWorkerGuilded = true;
						}
					}
				}
			}

			return !hasWorkerGuilded;
		}

		/// <summary>
		/// Static. Check if from can wear our guild cloth
		/// <param name="from" >mobile to check</param>
		/// <param name="guild" >guild for checked cloth</param>
		/// </summary>
		public static bool CheckEquip( Mobile from, BaseGuild guild, IGuildUniform cloth )
		{
			if( from == null || from.Deleted )
				return false;

			if( from.AccessLevel > AccessLevel.Player || guild == null )
				return true;

			if( from.Guild == null )
			{
				from.SendMessage( from.Language == "ITA" ? "Solo i membri della gilda possono vestire uniformi!" : "Only guild members can wear uniforms!" );
				return false;
			}

			if( from.Guild == guild && cloth.GuildedOwner != null && from == cloth.GuildedOwner )
				return true;
			else
			{
				from.SendMessage( from.Language == "ITA" ? "Non puoi indossare questa uniforme." : "You cannot wear this guild uniform." );
				return false;
			}
		}

		/// <summary>
		/// Static. Clear uniform for our guild. Delete also all instances of guild uniform
		/// <param name="guild" >our guild we want to clear its uniform</param>
		/// </summary>
		public static void ClearGuildUniform( Guild guild )
		{
			if( guild.GuildUniformInstances != null )
			{
				for( int i = 0; i < guild.GuildUniformInstances.Count; i++ )
				{
					Item item = (Item)guild.GuildUniformInstances[ i ];
					if( item != null && !item.Deleted )
						item.Delete();
				}
			}

			if( guild.UniformList != null )
			{
				for( int i = 0; i < guild.UniformList.Count; i++ )
					guild.UniformList.Clear();
			}
		}

		/// <summary>
		/// Static. Add one cloth to our guild uniform
		/// <param name="toAdd">item to add to our guild uniform</param>
		/// <param name="guild" >our guild we want to add an item to uniform</param>
		/// </summary>
		public static void AddGuildCloth( Item toAdd, Guild guild )
		{
			if( toAdd != null && !toAdd.Deleted && toAdd is IGuildUniform )
			{
				if( guild.UniformList == null )
					guild.UniformList = new List<UniformEntry>();

				UniformEntry entry = new UniformEntry( toAdd.GetType(), toAdd.Hue );

				bool isJustGuildUniform = false;
				for( int i = 0; ( i < guild.UniformList.Count ) && ( isJustGuildUniform == false ); i++ )
				{
					if( entry.CompareTo( guild.UniformList[ i ] ) == 0 )
						isJustGuildUniform = true;
				}

				if( !isJustGuildUniform )
					guild.UniformList.Add( entry );
				else
					toAdd.PublicOverheadMessage( MessageType.Regular, 37, true, "Questo oggetto fa già parte dell'uniforme." /*"This item is already in that guild uniform"*/ );
			}
		}

		/// <summary>
		/// Static. Add a guild uniform to from pack.
		/// <param name="from" >mobile to add uniform to</param>
		/// <param name="guild" >our guild with that uniform</param>
		/// </summary>
		public static void AddUniformToPack( Mobile from, Guild guild )
		{
			if( guild.UniformList == null )
				guild.UniformList = new List<UniformEntry>();

			for( int i = 0; i < guild.UniformList.Count; i++ )
			{
				try
				{
					Item item = Activator.CreateInstance( guild.UniformList[ i ].ItemType ) as Item;
					if( item != null )
					{
						RegisterUniform( from, guild, item );

						item.Hue = guild.UniformList[ i ].ItemHue;

						if( item is IGuildUniform )
						{
							( (IGuildUniform)item ).IsGuildItem = true;
							( (IGuildUniform)item ).OwnerGuild = guild;
							( (IGuildUniform)item ).GuildedOwner = from;
						}

						from.AddToBackpack( item );
					}
				}
				catch( Exception e )
				{
					Console.WriteLine( e.ToString() );
				}
			}
		}

		/// <summary>
		/// Static. Register an Item in GuildUniformInstances list
		/// </summary>
		public static void RegisterUniform( Mobile from, Guild guild, Item item )
		{
			if( guild == null || guild.Disbanded )
				return;

			if( guild.GuildUniformInstances == null )
				guild.GuildUniformInstances = new ArrayList();

			guild.GuildUniformInstances.Add( item );
			StringToLog( string.Format( "Uniform with type {0} and serial {1} registered for guild {2}.", item.GetType().Name, item.Serial, guild.Name ) );
		}

		/// <summary>
		/// Static. Remove an Item from GuildUniformInstances list
		/// </summary>		
		public static void UnRegisterUniform( Mobile from, Guild guild, Item item )
		{
			if( guild == null || guild.Disbanded )
				return;

			if( guild.GuildUniformInstances == null )
				return;

			if( guild.GuildUniformInstances.Contains( item ) )
			{
				guild.GuildUniformInstances.Remove( item );
				StringToLog( string.Format( "Uniform with type {0} and serial {1} UNregistered for guild {2}.", item.GetType().Name, item.Serial, guild.Name ) );
			}
		}

		/// <summary>
		/// Static. Remove all items bonded to "from" from GuildUniformInstances list.
		/// After removing delete them.
		/// </summary>	
		public static void DeleteUniformForMember( Mobile from, Guild guild )
		{
			if( guild == null || guild.Disbanded )
				return;

			if( guild.GuildUniformInstances == null )
				return;

			List<Item> toDelete = new List<Item>();
			for( int i = 0; i < guild.GuildUniformInstances.Count; i++ )
			{
				Item item = guild.GuildUniformInstances[ i ] as Item;
				if( item != null && item is IGuildUniform && ( (IGuildUniform)item ).GuildedOwner == from )
					toDelete.Add( item );
			}

			foreach( Item i in toDelete )
			{
				if( i != null && !i.Deleted )
				{
					try
					{
						i.Delete();
					}
					catch( Exception ex )
					{
						Console.WriteLine( ex.ToString() );
					}
				}
			}
		}

		/// <summary>
		/// Static. Check if mobile has any GuildItem
		/// </summary>
		public static bool HasUniformItems( Mobile from, Guild guild )
		{
			if( guild == null || guild.Disbanded )
				return false;

			if( guild.GuildUniformInstances == null )
				return false;

			for( int i = 0; i < guild.GuildUniformInstances.Count; i++ )
			{
				Item item = guild.GuildUniformInstances[ i ] as Item;
				if( item != null && item is IGuildUniform )
				{
					if( ( (IGuildUniform)item ).GuildedOwner != null && ( (IGuildUniform)item ).GuildedOwner == from )
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Static. Check if mobile is an Adept
		/// </summary>
		public static bool IsGuildAdept( Mobile from )
		{
			return from is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)from ).IsGuildAdept;
		}

		/// <summary>
		/// Static. Set leaving time for our mobile
		/// </summary>
		public static void DoRejoinPrevention( Mobile from )
		{
			Midgard2PlayerMobile m2pm = from as Midgard2PlayerMobile;
			if( m2pm != null )
				m2pm.LastGuildLeaving = DateTime.Now;
		}

		/// <summary>
		/// Static. Member invoked to log a string on a log file with try{} catch{}.
		/// <param name="toLog">  string to log </param>
		/// <param name="fileNamePath"> runUO core relative path where log has to be created or appended. </param>
		/// </summary>
		public static void StringToLog( string toLog, string fileNamePath )
		{
			try
			{
				TextWriter tw = File.AppendText( fileNamePath );
				tw.WriteLine( toLog );
				tw.Close();
			}
			catch( Exception ex )
			{
				Console.Write( "Log failed: {0}", ex );
			}
		}

		public static void StringToLog( string toLog )
		{
			StringToLog( toLog, "Logs/GuildsEvents.log" );
		}

		private class GuildUniformTarget : Target
		{
			#region fields
			private BaseGuild m_Guild;
			#endregion

			#region constructors
			public GuildUniformTarget( BaseGuild guild )
				: base( 10, false, TargetFlags.None )
			{
				m_Guild = guild;
			}
			#endregion

			#region members
			protected override void OnTarget( Mobile from, object targeted )
			{
				if( from == null || from.Deleted )
					return;

				if( targeted is Item )
				{
					Item item = (Item)targeted;

					if( item.IsChildOf( from.Backpack ) )
					{
						if( item.IsAccessibleTo( from ) )
						{
							if( item is IGuildUniform )
							{
								( (IGuildUniform)item ).OwnerGuild = m_Guild;
								AddGuildCloth( item, (Guild)m_Guild );
								from.SendMessage( from.Language == "ITA" ? "Fatto. L'oggetto ora fa parte dell'uniforme della gilda {0}." : "That's done. That item is now part of {0} guild uniform.", m_Guild.Name );
							}
							else
								from.SendMessage( from.Language == "ITA" ? "Quell'oggetto non può far parte di una uniforme di gilda." : "That item cannot be a guild uniform." );
						}
						else
							from.SendMessage( from.Language == "ITA" ? "Quell'oggetto non è accessibile." : "That item is not accessible." );
					}
					else
						from.SendMessage( from.Language == "ITA" ? "L'oggetto deve essere nel tuo zaino." : "That item must be in your backpack." );
				}
				else
					from.SendMessage( from.Language == "ITA" ? "Non è un oggetto valido." : "That target is not a valid item." );
			}

			protected override void OnTargetOutOfRange( Mobile from, object targeted )
			{
				from.SendMessage( from.Language == "ITA" ? "Quell'oggetto non è accessibile." : "That item is not accessible." );
			}
			#endregion
		}
	}

	/// <summary>
	/// Interface giving properties to verify if an item is a guild uniform
	/// </summary>
	public interface IGuildUniform
	{
		bool IsGuildItem { get; set; }
		BaseGuild OwnerGuild { get; set; }
		Mobile GuildedOwner { get; set; }
	}

	/// <summary>
	/// Little class for storing uniform infoes
	/// </summary>
	public class UniformEntry : IComparable
	{
		public Type ItemType { get; set; }
		public int ItemHue { get; set; }

		public UniformEntry( Type type, int hue )
		{
			ItemType = type;
			ItemHue = hue;
		}

		public UniformEntry( GenericReader reader )
		{
			int version = reader.ReadInt();

			switch( version )
			{
				case 0:
					{
						ItemType = ScriptCompiler.FindTypeByName( reader.ReadString() );
						ItemHue = reader.ReadInt();
						break;
					}
			}
		}

		public int CompareTo( object obj )
		{
			if( obj is UniformEntry )
			{
				UniformEntry entry = (UniformEntry)obj;

				if( entry.ItemHue == ItemHue && entry.ItemType == ItemType )
					return 0;
				else
					return -1;
			}
			else
				return -1;
		}

		#region serial-deserial
		public void Serialize( GenericWriter writer )
		{
			writer.Write( 0 ); //version

			writer.Write( ItemType.Name );
			writer.Write( ItemHue );
		}
		#endregion
	}

	public enum MidgardGuildTypes
	{
		None,
		Military,
		Corporation
	}
}