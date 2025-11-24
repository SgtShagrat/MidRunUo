/***************************************************************************
 *                               GalleonPlank.cs
 *
 *   begin                : 20 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Multis;
using Server.Spells;

namespace Midgard.Multis
{
    public class GalleonPlank : Plank
    {
        private List<PlankSubItem> m_TempItems;

        [Constructable]
        public GalleonPlank()
            : this( null, PlankSide.Starboard, 0 )
        {
        }

        public GalleonPlank( BaseBoat boat, PlankSide side, uint keyValue )
            : base( boat, side, keyValue )
        {
            ItemID = 0x886;

            m_TempItems = new List<PlankSubItem>();
        }

        /// <summary>
        /// Store the new temporary item at the given offset
        /// </summary>
        /// <param name="offset"></param>
        public void AddTempItem( int offset )
        {
            if( !FindItem( X + offset, Y, Z, Map, this ) )
            {
                PlankSubItem i = new PlankSubItem( this, Boat, Side, KeyValue );
                i.MoveToWorld( new Point3D( X + offset, Y, Z ), Map );
                i.Movable = false;

                if( m_TempItems == null )
                    m_TempItems = new List<PlankSubItem>();

                m_TempItems.Add( i );
            }
        }

        /// <summary>
        /// Update all plank items
        /// </summary>
        public void OffsetPlank( int x, int y )
        {
            if( m_TempItems == null || m_TempItems.Count < 1 )
                return;

            for( int i = 0; i < m_TempItems.Count; i++ )
            {
                Item item = m_TempItems[ i ];
                if( item != null && !item.Deleted )
                    item.Location.Offset( x, y, 0 );
            }
        }

        public void ClearTempItems()
        {
            if( m_TempItems == null || m_TempItems.Count < 1 )
                return;

            for( int i = 0; i < m_TempItems.Count; i++ )
            {
                Item item = m_TempItems[ i ];
                if( item != null && !item.Deleted )
                    item.Delete();
            }
        }

        public static bool FindItem( int x, int y, int z, Map map, Item test )
        {
            return FindItem( new Point3D( x, y, z ), map, test );
        }

        public static bool FindItem( Point3D p, Map map, Item test )
        {
            IPooledEnumerable eable = map.GetItemsInRange( p, 0 );

            foreach( Item item in eable )
            {
                if( item.Z == p.Z && item.ItemID == test.ItemID )
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public new bool IsOpen
        {
            get { return ItemID == 0x4a9; }
        }

        public override void SetFacing( Direction dir )
        {
        }

        public override void Open()
        {
            base.Open();

            switch( ItemID )
            {
                case 0x886:
                    ItemID = 0x4a9;
                    if( Side == PlankSide.Starboard )
                        MoveToWorld( new Point3D( X + 1, Y, Z ), Map );
                    break;
            }

            for( int i = 0; i < 4; i++ )
                AddTempItem( Side == PlankSide.Starboard ? i : -i );

            if( Boat != null )
                Boat.Refresh();
        }

        public override bool OnMoveOver( Mobile from )
        {
            if( IsOpen )
            {
                if( ( from.Direction & Direction.Running ) != 0 || ( Boat != null && !Boat.Contains( from ) ) )
                    return true;

                Map map = Map;

                if( map == null )
                    return false;

                int rx = 0, ry = 0;

                if( Side == PlankSide.Starboard )
                    rx = 5;
                else
                    rx = -5;

                for( int i = 1; i <= 6; ++i )
                {
                    int x = from.X + ( i * rx );
                    int y = Y + ( i * ry );
                    int z;

                    for( int j = -8; j <= 8; ++j )
                    {
                        z = from.Z + j;

                        if( map.CanFit( x, y, z, 16, false, false ) && !SpellHelper.CheckMulti( new Point3D( x, y, z ), map ) )
                        {
                            if( i == 1 && j >= -2 && j <= 2 )
                                return true;

                            from.Location = new Point3D( x, y, z );
                            return false;
                        }
                    }

                    z = map.GetAverageZ( x, y );

                    if( map.CanFit( x, y, z, 16, false, false ) && !SpellHelper.CheckMulti( new Point3D( x, y, z ), map ) )
                    {
                        if( i == 1 )
                            return true;

                        from.Location = new Point3D( x, y, z );
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Close()
        {
            base.Close();

            switch( ItemID )
            {
                case 0x4a9:
                    ItemID = 0x886;
                    if( Side == PlankSide.Starboard )
                        MoveToWorld( new Point3D( Boat.X + Boat.StarboardOffset.X, Boat.Y + Boat.StarboardOffset.Y, Boat.Z + 25 ), Boat.Map );
                    break;
            }

            if( Boat != null )
                Boat.Refresh();

            try
            {
                MovePlayersOnPlank();
                ClearTempItems();
            }
            catch( Exception e )
            {
                Console.WriteLine( e );
            }
        }

        private void MovePlayersOnPlank()
        {
            if( Boat == null || m_TempItems == null || m_TempItems.Count == 0 )
                return;

            List<Mobile> list = new List<Mobile>();
            Point3D markedLocation = Boat.GetMarkedLocation();
            Point3D location = new Point3D( markedLocation.X, markedLocation.Y, Z );

            foreach( PlankSubItem item in m_TempItems )
            {
                if( item == null || item.Deleted )
                    continue;

                foreach( Mobile mobile in Map.GetMobilesInRange( item.Location, 0 ) )
                {
                    if( mobile != null )
                        list.Add( mobile );
                }
            }

            foreach( Mobile mobile in list )
            {
                if( mobile != null )
                    mobile.MoveToWorld( location, Map );
            }
        }

        public override double GetCloseDelay()
        {
            return 10.0;
        }

        #region serialization
        public GalleonPlank( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); //version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        internal class PlankSubItem : Item
        {
            public GalleonPlank Plank { get; set; }
            public BaseBoat Boat { get; set; }
            public PlankSide Side { get; set; }
            public uint KeyValue { get; set; }

            public PlankSubItem( GalleonPlank plank, BaseBoat boat, PlankSide side, uint keyValue )
                : base( 0x4a9 )
            {
                Plank = plank;
                Boat = boat;
                Side = side;
                KeyValue = keyValue;
            }

            public override void OnDoubleClickDead( Mobile from )
            {
                OnDoubleClick( from );
            }

            public override void OnDoubleClick( Mobile from )
            {
                if( Boat == null )
                    return;

                if( from.InRange( GetWorldLocation(), 8 ) )
                {
                    if( Boat.Contains( from ) )
                    {
                        if( Plank.IsOpen )
                            Plank.Close();
                        else
                            Plank.Open();
                    }
                    else
                    {
                        if( !Plank.IsOpen )
                        {
                            if( !Plank.Locked )
                            {
                                Plank.Open();
                            }
                            else if( from.AccessLevel >= AccessLevel.GameMaster )
                            {
                                from.LocalOverheadMessage( Server.Network.MessageType.Regular, 0x00, 502502 ); // That is locked but your godly powers allow access
                                Plank.Open();
                            }
                            else
                            {
                                from.LocalOverheadMessage( Server.Network.MessageType.Regular, 0x00, 502503 ); // That is locked.
                            }
                        }
                        else if( !Plank.Locked )
                        {
                            from.Location = new Point3D( X, Y, Z + 3 );
                        }
                        else if( from.AccessLevel >= AccessLevel.GameMaster )
                        {
                            from.LocalOverheadMessage( Server.Network.MessageType.Regular, 0x00, 502502 ); // That is locked but your godly powers allow access
                            from.Location = new Point3D( X, Y, Z + 3 );
                        }
                        else
                        {
                            from.LocalOverheadMessage( Server.Network.MessageType.Regular, 0x00, 502503 ); // That is locked.
                        }
                    }
                }
            }

            public override bool OnMoveOver( Mobile from )
            {
                if( Plank.IsOpen )
                {
                    if( ( from.Direction & Direction.Running ) != 0 || ( Boat != null && !Boat.Contains( from ) ) )
                        return true;
                }

                return base.OnMoveOver( from );
            }

            #region serialization
            public PlankSubItem( Serial serial )
                : base( serial )
            {
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( 0 ); //version
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();
            }
            #endregion
        }
    }
}