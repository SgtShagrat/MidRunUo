/***************************************************************************
 *								  TownFieldSign.cs
 *									----------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Midgard.Engines.TownHouses;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Multis;

namespace Midgard.Engines.MidgardTownSystem
{
	[Flipable( 0xBC5, 0xBC6 )]
	public class TownFieldSign : TownHouseSign
	{
		[CommandProperty( AccessLevel.Administrator )]
		public bool CanBeDeleted { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRentable { get; set; }

		public static TownFieldDefinition EmptyFieldDefinition = new TownFieldDefinition( null, "no name available", Point3D.Zero, Point3D.Zero, short.MinValue, short.MaxValue, Point3D.Zero, Point3D.Zero, Point3D.Zero, 999999, 0, 0 );

		public TownFieldSign() : this( EmptyFieldDefinition )
		{
		}

		public TownFieldSign( TownFieldDefinition definition )
		{
			Movable = false;

			Name = definition.FieldName;
			BanLoc = definition.BanLocation;
			SignLoc = definition.SignLocation;

			Blocks = new List<Rectangle2D>();

			if( definition.System != null )
			{
				Point3D fixedCorner = new Point3D( definition.SouthEstCorner.X + 1, definition.SouthEstCorner.Y + 1, definition.SouthEstCorner.Z );
				Blocks.Add( TownHouseSetupGump.FixRect( new Rectangle2D( definition.NorthWestCorner, fixedCorner ) ) );
				UpdateBlocks();
			}

			RecurRent = true;
			ItemsPrice = 0;
			Relock = true;
			KeepItems = true;
			RentByTime = TimeSpan.FromDays( 30.0 );

			MinZ = definition.MinZ;
			MaxZ = definition.MaxZ;
			Price = definition.RentalCost;
			Locks = definition.NumLockDowns;
			Secures = definition.NumSecures;

			if( definition.System != null )
				Init( definition.System, definition.ContractLocation );
		}

		private void Init( TownSystem system, Point3D location )
		{
			MoveToWorld( location, Map.Felucca );

			if( system != null )
			{
				System = system;
				System.RegisterField( this );
			}

			IsRentable = false;
		}

		public override void Delete()
		{
			if( !CanBeDeleted )
				PublicOverheadMessage( Server.Network.MessageType.Regular, 0x0, true, "You cannot delete this field." );
			else
				base.Delete();

			if( Deleted && System != null )
				System.UnRegisterField( this );
		}

		public override void OnDoubleClick( Mobile m )
		{
			if( m.AccessLevel >= AccessLevel.Administrator )
				m.SendGump( new PropertiesGump( m, this ) );
			else if( !Visible )
				return;
			else if( CanBuyHouse( m ) )
				new TownHouseConfirmGump( m, this );
			else
				m.SendMessage( "You cannot purchase this field." );
		}

		public override bool CanBuyHouse( Mobile m )
		{
			return ( IsRentable && TownSystem.Find( m ) != null && TownSystem.CanBuyTownField( m, this ) );
		}

		public static bool IsField( BaseHouse house, bool checkSign )
		{
			if( house is TownHouse )
			{
				if( !checkSign )
					return true;

				TownHouse townHouse = (TownHouse)house;

				if( townHouse.ForSaleSign != null )
				{
					if( townHouse.ForSaleSign is TownFieldSign )
					{
						return true;
					}
				}
			}

			return false;
		}

		#region serialization
		public TownFieldSign( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 );

			// Version 1
			// rimosso m_System perche' passato a TownHouse

			// Version 0
			writer.Write( IsRentable );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			IsRentable = reader.ReadBool();

			if( version < 1 )
				reader.ReadInt();

			CanBeDeleted = false;

			if( ItemID == 0xC0B )
				ItemID =0xBC5;
			else if( ItemID == 0xC0C )
				ItemID = 0xBC6;

			TownSystem.CheckTownField( this );
		}
		#endregion
	}
}