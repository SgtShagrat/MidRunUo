/***************************************************************************
 *                                  ListItems.cs
 *                            		------------
 *  begin                	: Settembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Commands
{
	public class ListItems
	{
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "ListItemsName" , AccessLevel.Developer, new CommandEventHandler( ListItems_OnCommand ) );
		}
		#endregion

		#region callback
		[Usage( "ListItemsName" )]
		[Description( "List in a file all items name" )]
		public static void ListItems_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null )
				return;

			List<Item> list = new List<Item>();
			foreach ( Item item in World.Items.Values )
			{
				if( item != null && !item.Deleted )
				{
					if( item.Name != null )
					{
						if( !IsInExList( item ) )
						   list.Add( item );
					}
				}
			}

			list.Sort( new Sorter( false ));

			using ( StreamWriter op = new StreamWriter( "Logs/itemsNameByName.log" ) )
			{
				for( int i = 0; i < list.Count; i++ )
					op.WriteLine( "{0} - {1} - \"{2}\"", list[i].GetType().Name, list[i].Serial.ToString(), list[i].Name );
			}

			list.Sort( new Sorter( true ));

			using ( StreamWriter op = new StreamWriter( "Logs/itemsNamebyType.log" ) )
			{
				for( int i = 0; i < list.Count; i++ )
					op.WriteLine( "{0} - {1} - \"{2}\"", list[i].GetType().Name, list[i].Serial.ToString(), list[i].Name );
			}

			List<ItemEntry> preRestricted = new List<ItemEntry>();
			List<ItemEntry> postRestricted = new List<ItemEntry>();

			for( int i = 0; i < list.Count - 1; i++ )
				preRestricted.Add( new ItemEntry( list[i].GetType(), list[i].Name, list[i].Serial.ToString() ) );

			foreach( ItemEntry re in preRestricted )
			{
				if( !IsAlreadyInList( postRestricted, re ) )
				   postRestricted.Add( re );
			}

			using ( StreamWriter op = new StreamWriter( "Logs/itemsNamebyTypeRestricted.log" ) )
			{
				for( int i = 0; i < postRestricted.Count; i++ )
					op.WriteLine( "{0} - \"{1}\"", postRestricted[i].Type, postRestricted[i].Name );
			}
		}

		private static bool IsAlreadyInList( List<ItemEntry> list, ItemEntry entry )
		{
			foreach( ItemEntry re in list )
			{
				if( re.Name == entry.Name && re.Type == entry.Type )
					return true;
			}

			return false;
		}
		#endregion

		private static Type[] m_ExclusionList = new Type[]
		{
			typeof(XmlSpawner),
			typeof(LOSBlocker),
			typeof(BasePiece),
			typeof(OrcishKinMask), 
			typeof(BraceletOfBinding),
			typeof(ShrinkItem),
			typeof(WayPoint),
			typeof(ArcaneGem),
			typeof(ClockworkAssembly),
			typeof(SHTeleComponent),
		};

		public static bool IsInExList( Item item )
		{
			Type itemType = item.GetType();

			foreach( Type t in m_ExclusionList )
			{
				if( itemType.IsSubclassOf( t ) )
				   return true;
			}

			return Array.IndexOf( m_ExclusionList, itemType ) > -1;
		}

		private class ItemEntry
		{
			private Type m_Type;
			private string m_Name;
			private string m_Serial;

			public Type Type
			{
				get { return m_Type; }
			}

			public string Name
			{
				get { return m_Name; }
			}

			public string Serial
			{
				get { return m_Serial; }
			}

			public ItemEntry( Type type, string name, string serial )
			{
				m_Type = type;
				m_Name = name;
				m_Serial = serial;
			}

			public override bool Equals( object obj )
			{
				if( !( obj is ItemEntry ) )
					return false;

				ItemEntry re = (ItemEntry)obj;
				return re.Name == m_Name && re.Type == m_Type;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}

		private class Sorter : IComparer<Item>
		{
			private bool m_Type;

			public Sorter( bool type )
			{
				m_Type = type;
			}
				
			public int Compare( Item x, Item y )
			{
				if ( x == null || y == null || x.Name == null || y.Name == null  )
					throw new ArgumentException();

				int typeRes = Insensitive.Compare( x.GetType().FullName, y.GetType().FullName );
				int nameRes = Insensitive.Compare( x.Name, y.Name );

				if( m_Type )
					return ( typeRes != 0 ) ? typeRes : nameRes;
				else
					return nameRes;
			}
		}
	}
}
