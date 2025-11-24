using System;
using Midgard.Misc;
using Server.Factions;
using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class Guildstone : Item, IAddon, IChopable
	{
		private Guild m_Guild;
		private string m_GuildName;
		private string m_GuildAbbrev;

		[CommandProperty( AccessLevel.Developer )]
		public Guild Guild
		{
			get
			{
				return m_Guild;
			}
		}

		#region modifica by Dies Irae
		[CommandProperty( AccessLevel.Seer, AccessLevel.Seer )]
		public MidgardTowns Town
		{
			get { return m_Guild != null ? m_Guild.Town : MidgardTowns.None; }
			set
			{
				if( m_Guild != null && !m_Guild.Disbanded )
				{
					MidgardTowns oldValue = m_Guild.Town;
			    	if( oldValue != value )
			    		m_Guild.Town = value;
				}
			}
		}

		[CommandProperty( AccessLevel.Seer, AccessLevel.Seer )]
		public MidgardGuildTypes MidgardGuildType
		{
			get { return m_Guild != null ? m_Guild.MidgardGuildType : MidgardGuildTypes.None; }
			set
			{
				if( m_Guild != null && !m_Guild.Disbanded )
				{
					MidgardGuildTypes oldValue = m_Guild.MidgardGuildType;
			    	if( oldValue != value )
			    		m_Guild.MidgardGuildType = value;
				}
			}
		}

		[CommandProperty( AccessLevel.Seer )]
		public int NumUniformCloths
		{
			get { return ( m_Guild != null && m_Guild.UniformList != null ) ? m_Guild.UniformList.Count : -1; }
		}

		[CommandProperty( AccessLevel.Seer )]
		public int NumUniformsInGame
		{
			get { return ( m_Guild != null && m_Guild.GuildUniformInstances != null ) ? m_Guild.GuildUniformInstances.Count : -1; }
		}

		[CommandProperty( AccessLevel.Seer )]
		public int NumMembers
		{
			get { return ( m_Guild != null ) ? m_Guild.Members.Count : -1; }
		}
		#endregion

		public override int LabelNumber { get { return 1041429; } } // a guildstone

		#region modifica by Dies Irae
		public override bool DisplayWeight { get { return false; } }

	    [CommandProperty( AccessLevel.Administrator )]
        public bool CanBeDeleted { get; set; }

        public override void Delete()
        {
            GuildsHelper.StringToLog( string.Format( "Trying to delete a guildstone. Location {0}", Location ) );
            if( !CanBeDeleted )
                return;

            base.Delete();
        }
	    #endregion

		public Guildstone( Guild g ) : this( g, g.Name, g.Abbreviation )
		{
		}

		public Guildstone( Guild g, string guildName, string abbrev ) : base( Guild.NewGuildSystem ? 0xED6 : 0xED4 )
		{
			m_Guild = g;
			m_GuildName = guildName;
			m_GuildAbbrev = abbrev;

			Movable = false;
		    CanBeDeleted = false; // mod by Dies Irae
		}

		public Guildstone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			if( m_Guild != null && !m_Guild.Disbanded )
			{
				m_GuildName = m_Guild.Name;
				m_GuildAbbrev = m_Guild.Abbreviation;
			}

			writer.Write( (int)3 ); // version

			writer.Write( m_BeforeChangeover );

			writer.Write( m_GuildName );
			writer.Write( m_GuildAbbrev );

			writer.Write( m_Guild );
		}

		private bool m_BeforeChangeover;
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch( version )
			{
				case 3:
				{
					m_BeforeChangeover = reader.ReadBool();
					goto case 2;
				}
				case 2:
				{
					m_GuildName = reader.ReadString();
					m_GuildAbbrev = reader.ReadString();

					goto case 1;
				}
				case 1:
				{
					m_Guild = reader.ReadGuild() as Guild;

					goto case 0;
				}
				case 0:
				{
					break;
				}
			}

			if( Guild.NewGuildSystem && ItemID == 0xED4 )
				ItemID = 0xED6;

			if( m_Guild != null )
			{
				m_GuildName = m_Guild.Name;
				m_GuildAbbrev = m_Guild.Abbreviation;
			}

			if( version <= 2 )
				m_BeforeChangeover = true;

			if( Guild.NewGuildSystem && m_BeforeChangeover )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( AddToHouse ) );

			if( !Guild.NewGuildSystem && m_Guild == null )
				this.Delete();
		}

		private void AddToHouse()
		{
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if( Guild.NewGuildSystem && m_BeforeChangeover && house != null && !house.Addons.Contains( this ) )
			{
				house.Addons.Add( this );
				m_BeforeChangeover = false;
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if( m_Guild != null && !m_Guild.Disbanded )
			{
				string name;
				string abbr;

				if( (name = m_Guild.Name) == null || (name = name.Trim()).Length <= 0 )
					name = "(unnamed)";

				if( (abbr = m_Guild.Abbreviation) == null || (abbr = abbr.Trim()).Length <= 0 )
					abbr = "";

				//list.Add( 1060802, Utility.FixHtml( name ) ); // Guild name: ~1_val~
				list.Add( 1060802, String.Format( "{0} [{1}]", Utility.FixHtml( name ), Utility.FixHtml( abbr ) ) );

				#region modifica by Dies Irae
				if( m_Guild.Town != Server.Mobiles.MidgardTowns.None )
				{
					string townLabel = string.Format( "midgard guild town: {0}", m_Guild.Town.ToString() );
					list.Add( townLabel );
				}

				if( m_Guild.MidgardGuildType != MidgardGuildTypes.None )
				{
					string typeLabel = string.Format( "{0} guild", m_Guild.MidgardGuildType.ToString() );
					list.Add( typeLabel );					
				}
				#endregion
			}
			else
			{

				list.Add( 1060802, String.Format( "{0} [{1}]", Utility.FixHtml( m_GuildName ), Utility.FixHtml( m_GuildAbbrev ) ) );
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			string name;

			if( m_Guild == null )
				name = "(unfounded)";
			else if( (name = m_Guild.Name) == null || (name = name.Trim()).Length <= 0 )
				name = "(unnamed)";

			this.LabelTo( from, name );
		}

		public override void OnAfterDelete()
		{
			if( !Guild.NewGuildSystem && m_Guild != null && !m_Guild.Disbanded )
				m_Guild.Disband();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( Guild.NewGuildSystem )
				return;

			if( m_Guild == null || m_Guild.Disbanded )
			{
				Delete();
			}
			else if( !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			}
			else if( m_Guild.Accepted.Contains( from ) )
			{
				#region Factions
				PlayerState guildState = PlayerState.Find( m_Guild.Leader );
				PlayerState targetState = PlayerState.Find( from );

				Faction guildFaction = (guildState == null ? null : guildState.Faction);
				Faction targetFaction = (targetState == null ? null : targetState.Faction);

				if( guildFaction != targetFaction || (targetState != null && targetState.IsLeaving) )
					return;

				if( guildState != null && targetState != null )
					targetState.Leaving = guildState.Leaving;
				#endregion

				#region modifica by Dies Irae
				if( !GuildsHelper.CanJoinMidgardGuild( from, m_Guild ) )
				{
					from.SendMessage( from.Language == "ITA" ? "{0}, non puoi far parte di questa gilda di Midgard." : "{0}, you cannot join this Midgard guild.", from.Name );
					m_Guild.Accepted.Remove( from );
					return;
				}
				#endregion

				m_Guild.Accepted.Remove( from );
				m_Guild.AddMember( from );

				#region modifica by Dies Irae
				GuildsHelper.DeleteUniformForMember( from, m_Guild );
				GuildsHelper.AddUniformToPack( from, m_Guild );
				#endregion

				GuildGump.EnsureClosed( from );
				from.SendGump( new GuildGump( from, m_Guild ) );
			}
			else if( from.AccessLevel < AccessLevel.GameMaster && !m_Guild.IsMember( from ) )
			{
				from.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, 501158, "", "" ) ); // You are not a member ...
			}
			else
			{
				GuildGump.EnsureClosed( from );
				from.SendGump( new GuildGump( from, m_Guild ) );

				#region modifica by Dies Irae
				if( m_Guild.IsMember( from ) && !GuildsHelper.HasUniformItems( from, m_Guild ) )
					GuildsHelper.AddUniformToPack( from, m_Guild );
				#endregion
			}
		}

		#region IAddon Members
		public Item Deed
		{
			get { return new GuildstoneDeed( m_Guild, m_GuildName, m_GuildAbbrev ); }
		}

		public bool CouldFit( IPoint3D p, Map map )
		{
			return map.CanFit( p.X, p.Y, p.Z, this.ItemData.Height );
		}

		#endregion

		#region IChopable Members

		public void OnChop( Mobile from )
		{
			if( !Guild.NewGuildSystem )
				return;

			BaseHouse house = BaseHouse.FindHouseAt( this );

			if( ( house == null && m_BeforeChangeover ) || ( house != null && house.IsOwner( from ) && house.Addons.Contains( this ) ))
			{
				Effects.PlaySound( GetWorldLocation(), Map, 0x3B3 );
				from.SendLocalizedMessage( 500461 ); // You destroy the item.

				Delete();

				if( house != null && house.Addons.Contains( this ) )
					house.Addons.Remove( this );

				Item deed = Deed;

				if( deed != null )
				{
					from.AddToBackpack( deed );
				}
			}
		}

		#endregion
	}

	[Flipable( 0x14F0, 0x14EF )]
	public class GuildstoneDeed : Item
	{
		public override int LabelNumber { get { return 1041233; } } // deed to a guildstone

		private Guild m_Guild;
		private string m_GuildName;
		private string m_GuildAbbrev;

		[Constructable]
		public GuildstoneDeed( Guild g, string guildName, string abbrev ) : base( 0x14F0 )
		{
			m_Guild = g;
			m_GuildName = guildName;
			m_GuildAbbrev = abbrev;

			Weight = 1.0;
		}

		public GuildstoneDeed( Serial serial ) : base( serial )
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


		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if( m_Guild != null && !m_Guild.Disbanded )
			{
				string name;
				string abbr;

				if( (name = m_Guild.Name) == null || (name = name.Trim()).Length <= 0 )
					name = "(unnamed)";

				if( (abbr = m_Guild.Abbreviation) == null || (abbr = abbr.Trim()).Length <= 0 )
					abbr = "";

				//list.Add( 1060802, Utility.FixHtml( name ) ); // Guild name: ~1_val~
				list.Add( 1060802, String.Format( "{0} [{1}]", Utility.FixHtml( name ), Utility.FixHtml( abbr ) ) );
			}
			else
			{
				list.Add( 1060802, String.Format( "{0} [{1}]", Utility.FixHtml( m_GuildName ), Utility.FixHtml( m_GuildAbbrev ) ) );
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( IsChildOf( from.Backpack ) )
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if( house != null && house.IsOwner( from ) )
				{
					from.SendLocalizedMessage( 1062838 ); // Where would you like to place this decoration?
					from.BeginTarget( -1, true, Targeting.TargetFlags.None, new TargetStateCallback( Placement_OnTarget ), null );
				}
				else
				{
					from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
				}
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

		public void Placement_OnTarget( Mobile from, object targeted, object state )
		{
			IPoint3D p = targeted as IPoint3D;

			if( p == null || Deleted )
				return;

			Point3D loc = new Point3D( p );

			BaseHouse house = BaseHouse.FindHouseAt( loc, from.Map, 16 );

			if( IsChildOf( from.Backpack ) )
			{
				if( house != null && house.IsOwner( from ) )
				{
					Item addon = new Guildstone( m_Guild, m_GuildName, m_GuildAbbrev );

					addon.MoveToWorld( loc, from.Map );

					house.Addons.Add( addon );
					Delete();
				}
				else
				{
					from.SendLocalizedMessage( 1042036 ); // That location is not in your house.
				}
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

	}
}
