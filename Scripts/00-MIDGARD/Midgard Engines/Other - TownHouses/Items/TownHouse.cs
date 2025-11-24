using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;
using Server.Targeting;

namespace Midgard.Engines.TownHouses
{
    public class TownHouse : VersionHouse
    {
        public static List<TownHouse> AllTownHouses { get; private set; }

        private Item m_Hanger;
        private List<Sector> m_Sectors = new List<Sector>();

        public TownHouseSign ForSaleSign { get; private set; }

        public Item Hanger
        {
            get
            {
                if( m_Hanger == null )
                {
                    m_Hanger = new Item( 0xB98 );
                    m_Hanger.Movable = false;
                    m_Hanger.Location = Sign.Location;
                    m_Hanger.Map = Sign.Map;
                }

                return m_Hanger;
            }
            set { m_Hanger = value; }
        }

        public TownHouse( Mobile m, TownHouseSign sign, int locks, int secures )
            : base( 0x1DD6 | 0x4000, m, locks, secures )
        {
            ForSaleSign = sign;

            SetSign( 0, 0, 0 );

            AllTownHouses.Add( this );
        }

        public void InitSectorDefinition()
        {
            if( ForSaleSign == null || ForSaleSign.Blocks.Count == 0 )
                return;

            int minX = ForSaleSign.Blocks[ 0 ].Start.X;
            int minY = ForSaleSign.Blocks[ 0 ].Start.Y;
            int maxX = ForSaleSign.Blocks[ 0 ].End.X;
            int maxY = ForSaleSign.Blocks[ 0 ].End.Y;

            foreach( Rectangle2D rect in ForSaleSign.Blocks )
            {
                if( rect.Start.X < minX )
                    minX = rect.Start.X;
                if( rect.Start.Y < minY )
                    minY = rect.Start.Y;
                if( rect.End.X > maxX )
                    maxX = rect.End.X;
                if( rect.End.Y > maxY )
                    maxY = rect.End.Y;
            }

            bool isBadSign = Math.Abs( maxY - minY ) > 100 || Math.Abs( maxX - minX ) > 100;

            foreach( Sector sector in m_Sectors )
                sector.OnMultiLeave( this );

            m_Sectors.Clear();

            if( !isBadSign )
            {
                for( int x = minX; x < maxX; ++x )
                    for( int y = minY; y < maxY; ++y )
                        if( !m_Sectors.Contains( Map.GetSector( new Point2D( x, y ) ) ) )
                            m_Sectors.Add( Map.GetSector( new Point2D( x, y ) ) );
            }
            else
                Console.WriteLine( "Warning: bad townhouse initialize. MinX {0}, MaxX {1}, MinY {2}, MaxY {3}. Serial {4}", minX, maxX, minY, maxY, ForSaleSign.Serial );

            foreach( Sector sector in m_Sectors )
                sector.OnMultiEnter( this );

            Components.Resize( maxX - minX, maxY - minY );
            Components.Add( 0x520, Components.Width - 1, Components.Height - 1, -5 );
        }

        public override Rectangle2D[] Area
        {
            get
            {
                if( ForSaleSign == null )
                    return new Rectangle2D[ 100 ];

                Rectangle2D[] rects = new Rectangle2D[ ForSaleSign.Blocks.Count ];

                for( int i = 0; i < ForSaleSign.Blocks.Count && i < rects.Length; ++i )
                    rects[ i ] = ForSaleSign.Blocks[ i ];

                return rects;
            }
        }

        public override bool IsInside( Point3D p, int height )
        {
            if( ForSaleSign == null )
                return false;

            if( Map == null || Region == null )
            {
                Delete();
                return false;
            }

            Sector sector = null;

            try
            {
                if( ForSaleSign is RentalContract && Region.Contains( p ) )
                    return true;

                sector = Map.GetSector( p );

                foreach( BaseMulti m in sector.Multis )
                {
                    if( m != this
                    && m is TownHouse
                    && ( (TownHouse)m ).ForSaleSign is RentalContract
                    && ( (TownHouse)m ).IsInside( p, height ) )
                        return false;
                }

                return Region.Contains( p );
            }
            catch( Exception e )
            {
                Errors.Report( "Error occured in IsInside().  More information on the console." );
                if( Region != null )
                    Console.WriteLine( "Info:{0}, {1}, {2}", Map, sector, Region );
                Console.WriteLine( e.Source );
                Console.WriteLine( e.Message );
                Console.WriteLine( e.StackTrace );
                return false;
            }
        }

        public override int GetNewVendorSystemMaxVendors()
        {
            return 50;
        }

        public override int GetAosMaxSecures()
        {
            return MaxSecures;
        }

        public override int GetAosMaxLockdowns()
        {
            return MaxLockDowns;
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if( m_Hanger != null )
                m_Hanger.Map = Map;
        }

        public override void OnLocationChange( Point3D oldLocation )
        {
            base.OnLocationChange( oldLocation );

            if( m_Hanger != null )
                m_Hanger.Location = Sign.Location;
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( e.Mobile != Owner || !IsInside( e.Mobile ) )
                return;

            if( e.Speech.ToLower() == "check house rent" )
                ForSaleSign.CheckRentTimer();

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( AfterSpeech ), e.Mobile );
        }

        private void AfterSpeech( object o )
        {
            if( !( o is Mobile ) )
                return;

            if( ( (Mobile)o ).Target is HouseBanTarget && ForSaleSign != null && ForSaleSign.NoBanning )
            {
                ( (Mobile)o ).Target.Cancel( (Mobile)o, TargetCancelType.Canceled );
                ( (Mobile)o ).SendMessage( 0x161, "You cannot ban people from this house." );
            }
        }

        public override void OnDelete()
        {
            if( m_Hanger != null )
                m_Hanger.Delete();

            foreach( Item item in Sign.GetItemsInRange( 0 ) )
                if( item != Sign )
                    item.Visible = true;

            ForSaleSign.ClearHouse();
            Doors.Clear();

            AllTownHouses.Remove( this );

            base.OnDelete();
        }

        #region serialization
        public TownHouse( Serial serial )
            : base( serial )
        {
            AllTownHouses.Add( this );
        }

        static TownHouse()
        {
            AllTownHouses = new List<TownHouse>();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 3 );

            // Version 2

            writer.Write( m_Hanger );

            // Version 1

            writer.Write( ForSaleSign );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( version >= 2 )
                m_Hanger = reader.ReadItem();

            ForSaleSign = (TownHouseSign)reader.ReadItem();

            if( version <= 2 )
            {
                int count = reader.ReadInt();
                for( int i = 0; i < count; ++i )
                    reader.ReadRect2D();
            }

            if( Price == 0 )
                Price = 1;

            ItemID = 0x1DD6 | 0x4000;
        }
        #endregion
    }
}