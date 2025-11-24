using System;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class TillerMan : Item
	{
		private BaseBoat m_Boat;

		public TillerMan( BaseBoat boat ) : base( 0x3E4E )
		{
			m_Boat = boat;
			Movable = false;
		}

		public TillerMan( Serial serial ) : base(serial)
		{
		}

	    public BaseBoat Boat
	    {
	        get { return m_Boat; }
	        set { m_Boat = value; }
	    }

	    #region mod by Dies Irae
	    public virtual bool CanSpeech
	    {
	        get { return true; }
	    }
	    #endregion
        
	    public void SetFacing( Direction dir )
		{
			switch ( dir )
			{
				case Direction.South: ItemID = 0x3E4B; break;
				case Direction.North: ItemID = 0x3E4E; break;
				case Direction.West:  ItemID = 0x3E50; break;
				case Direction.East:  ItemID = 0x3E53; break;
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( Boat.Status );
		}

		public void Say( string speech )
		{
		    if( !CanSpeech )
		        return;

		    PublicOverheadMessage( MessageType.Regular, 0x3B2, true, speech );
		}

		public void Say( int number )
		{
		    #region mod by Dies Irae
		    if( !CanSpeech )
		        return;
		    #endregion

			PublicOverheadMessage( MessageType.Regular, 0x3B2, number );
		}

		public void Say( int number, string args )
		{
            #region mod by Dies Irae
            if( !CanSpeech )
                return;
            #endregion

			PublicOverheadMessage( MessageType.Regular, 0x3B2, number, args );
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Boat != null && Boat.ShipName != null )
				list.Add( 1042884, Boat.ShipName ); // the tiller man of the ~1_SHIP_NAME~
			else
				base.AddNameProperty( list );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Boat != null && Boat.ShipName != null )
				LabelTo( from, 1042884, Boat.ShipName ); // the tiller man of the ~1_SHIP_NAME~
			else
				base.OnSingleClick( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Boat != null && Boat.Contains( from ) )
				Boat.BeginRename( from );
			else if ( Boat != null )
				Boat.BeginDryDock( from );
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is MapItem && Boat != null && Boat.CanCommand( from ) && Boat.Contains( from ) )
			{
				Boat.AssociateMap( (MapItem) dropped );
			}

			return false;
		}

		public override void OnAfterDelete()
		{
			if ( Boat != null )
			{
				if (Boat is Midgard.Multis.Vessel) //mod by magius(CHE): wtf
					((Midgard.Multis.Vessel)Boat).CanBeDeleted = true;
				
				Boat.Delete();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );//version

			writer.Write( Boat );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Boat = reader.ReadItem() as BaseBoat;

					if ( Boat == null )
						Delete();

					break;
				}
			}
		}
	}
}