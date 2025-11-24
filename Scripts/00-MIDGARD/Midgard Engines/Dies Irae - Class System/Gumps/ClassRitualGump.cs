/***************************************************************************
 *							   ClassRitualGump.cs
 *
 *   begin				: 10 January, 2010
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Midgard.Engines.Classes
{
	public class ClassRitualGump : Gump
	{
		#region Buttons enum
		public enum Buttons
		{
			Close,

			MakeRitual
		}
		#endregion

		private const int LabelsPerPage = 10;

		private readonly ClassPlayerState m_State;
		private readonly Item m_Owner;
		private List<PowerDefinition> m_Definitions;
		private ClassSystem m_GumpSystem;

		public ClassRitualGump( ClassPlayerState state, Item owner, ClassSystem system )
			: base( 150, 200 )
		{
			m_State = state;
			if( m_State == null )
				return;

			m_Owner = owner;
			if( m_Owner == null )
				return;

			m_GumpSystem = system;

			m_Definitions = new List<PowerDefinition>();
			foreach( PowerDefinition def in m_GumpSystem.Definition.PowersDefinitions )
			{
				if( !def.IsGranted )
					m_Definitions.Add( def );
			}

			AddBackground();
			AddIndex();

			for( int page = 0; page < m_Definitions.Count; ++page )
			{
				AddPage( 2 + page );

				AddButton( 125, 14, 2205, 2205, 0, GumpButtonType.Page, 1 + page );

				if( page < m_Definitions.Count - 1 )
					AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 3 + page );

				for( int half = 0; half < 2; ++half )
					AddDetails( ( page * 2 ) + half, half );
			}
		}

		private void AddBackground()
		{
			AddPage( 0 );

			AddImage( 100, 10, 0x899 ); // Background image of an open book

			// Two separators under first line
			for( int i = 0; i < 2; ++i )
			{
				int xOffset = 125 + ( i * 165 );

				AddImage( xOffset, 50, 57 ); // starting piece
				xOffset += 20;

				for( int j = 0; j < 6; ++j, xOffset += 15 )
					AddImage( xOffset, 50, 58 ); // little hor. line

				AddImage( xOffset - 5, 50, 59 ); // ending piece
			}
		}

		private void AddIndex()
		{
			ClassSystem system = m_GumpSystem;
			PowerDefinition[] entries = m_Definitions.ToArray();
			string language = m_State.Mobile.Language;

			// Index
			AddPage( 1 );

			AddOldHtml( 140, 40, 80, 18, language == "ITA" ? "Classe:" : "Class:" );
			AddFirstCharHuedHtml( 185, 40, 100, 18, system.Definition.ClassName, Colors.Indigo, true );

			// Groups
			AddOldHtml( 300, 40, 100, 18, language == "ITA" ? "Poteri:" : "Powers:" );
			AddOldHtml( 390, 40, 30, 18, entries.Length.ToString() );

			// List of entries
			for( int i = 0; i < entries.Length; ++i )
			{
				PowerDefinition entry = entries[ i ];
				string desc = entry.Name;
				int x = 140 + ( ( i / LabelsPerPage ) * 145 );
				int y = 60 + ( ( i % LabelsPerPage ) * 15 );
				int gumpId = 0x837; // ( i < LabelsPerPage ) ? 0x8B0 : 0x8AF;
				int xBtnOffset = ( ( i / LabelsPerPage ) * 120 );

				// Description label
				AddButton( x - 10 + xBtnOffset, y + 2, gumpId, gumpId, 0, GumpButtonType.Page, 2 + i / 2 );
				AddOldHtml( x + 5, y, 115, 17, desc );
			}

			// Turn page button
			AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 2 );
		}

		private void AddDetails( int index, int half )
		{
			const int aligned = 135;
			const int tabbed = 140;
			int level = 2;
			string language = m_State.Mobile.Language;
			Midgard2PlayerMobile from = (Midgard2PlayerMobile)m_State.Mobile;
			// List of entries
			PowerDefinition[] entries = m_Definitions.ToArray();
			if( index < entries.Length )
			{
				PowerDefinition entry = entries[ index ];
				level = from.ClassState.GetLevel( entry ) + 1;
				int y = 40;

				string name = StringUtility.Capitalize( entry.Name );

				AddOldHtml( 120 + ( half * 160 ), y, 155, 17, HtmlCenter( name ) );

				y += 30;

				if( level - 1 < entry.MaxRituals )
				{
					AddOldHtml( aligned + ( half * 160 ), y, 150, 17, language == "ITA" ? "Requisiti:" : "Requirements:" );

					y += 20;
					for( int i = 0; i < entry.Requirements.Length && i < 4; i++, y += 20 )
					{
						RequirementDefinition req = entry.Requirements[ i ];
						string reqName = req.Name;
						AddOldHtml( tabbed + ( half * 160 ), y, 150, 17, String.Format( "{0} {1}", reqName, req.Quantity * level ) );
					}

					y += 10;
					//double skill = Math.Max( entry.RequiredSkill, 0.0 );
					//AddOldHtml( aligned + ( half * 160 ), y, 150, 17, String.Format( "Diff.: {0}", skill.ToString( "F2" ) ) );
					y += 20;
					AddButton( aligned - 10 + ( half * 160 ), y + 2, 0x8B0, 0x8B0, index + 1, GumpButtonType.Reply, 0 );
					AddFirstCharHuedHtml( tabbed + 10 + ( half * 160 ), y, 150, 17, language == "ITA" ? "Inizia Rituale" : "Make Ritual", Colors.Indigo, true );
				}
				else
				{
					AddOldHtml( aligned + ( half * 160 ), y, 150, 17, language == "ITA" ? "Al massimo." : "Maxed." );
				}
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if( m_Owner.Deleted || !from.InRange( m_Owner.GetWorldLocation(), 1 ) )
				return;

			PowerDefinition[] entries = m_Definitions.ToArray();

			int index = info.ButtonID - 1;
			if( index < entries.Length && index > -1 )
			{
				from.SendMessage( from.Language == "ITA" ? "Hai invocato il rituale {0}." : "Thou invoked the {0} ritual.", entries[ index ].Name );

				m_GumpSystem.MakeRitual( from, entries[ index ] );
			}
		}
	}
}