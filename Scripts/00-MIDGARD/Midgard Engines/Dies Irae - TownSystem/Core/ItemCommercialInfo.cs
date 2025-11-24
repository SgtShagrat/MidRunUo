using System;

using Server;

namespace Midgard.Engines.MidgardTownSystem
{
    public class ItemCommercialInfo
    {
        public static readonly int MaxPrice = 999999;
        public static readonly int MinPrice = 1;

        private int m_ActualPrice;
        private int m_TotalSold;

        public ItemCommercialInfo( Type type, int price, int sold )
        {
            ItemType = type;
            m_ActualPrice = price;
            m_TotalSold = sold;
        }

        public ItemCommercialInfo( TownSystem system, GenericReader reader )
        {
            System = system;

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    string tmp = reader.ReadString();

                    if( String.IsNullOrEmpty( tmp ) )
                        ItemType = null;
                    else
                        ItemType = ScriptCompiler.FindTypeByFullName( tmp );

                    m_ActualPrice = reader.ReadInt();
                    m_TotalSold = reader.ReadInt();

                    break;
                case 0:
                    string tmp2 = reader.ReadString();

                    if( String.IsNullOrEmpty( tmp2 ) )
                        ItemType = null;
                    else
                        ItemType = ScriptCompiler.FindTypeByName( tmp2 );

                    m_ActualPrice = reader.ReadInt();
                    m_TotalSold = reader.ReadInt();
                    break;
            }
        }

        public Type ItemType { get; private set; }

        public int ActualPrice
        {
            get
            {
                if( m_ActualPrice < MinPrice )
                    return MinPrice;
                else if( m_ActualPrice > MaxPrice )
                    return MaxPrice;
                else
                    return m_ActualPrice;
            }
            set
            {
                int oldValue = m_ActualPrice;

                if( oldValue != value )
                {
                    if( m_ActualPrice < MinPrice )
                        m_ActualPrice = MinPrice;
                    else if( m_ActualPrice > MaxPrice )
                        m_ActualPrice = MaxPrice;
                    else
                        m_ActualPrice = value;

                    OnPriceChanged( oldValue );
                }
            }
        }

        public int TotalSold
        {
            get { return m_TotalSold; }
            set
            {
                int oldValue = m_TotalSold;

                if( oldValue != value )
                {
                    m_TotalSold = value;

                    OnTotalSoldChanged( oldValue );
                }
            }
        }

        public TownItemPriceDefinition Definition
        {
            get { return TownItemPriceDefinition.GetDefFromType( ItemType ); }
        }

        public TownSystem System { get; private set; }

        public virtual void OnPriceChanged( int oldValue )
        {
            if( System != null && ItemType.Name != null )
            {
                TownLog.Log( LogType.Commercial, String.Format( "Price of type {0} changed from {1} to {2} for town {3} in datetime {4}.",
                                                                ItemType.Name, oldValue, m_ActualPrice, System.Definition.TownName, DateTime.Now ) );
            }
        }

        public virtual void OnTotalSoldChanged( int oldValue )
        {
            if( System != null && ItemType.Name != null )
            {
                TownLog.Log( LogType.Commercial, String.Format( "Sold quantity of type {0} changed from {1} to {2} for town {3} in datetime {4}.",
                                                                ItemType.Name, oldValue, m_TotalSold, System.Definition.TownName, DateTime.Now ) );
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( 1 ); // version

            writer.Write( ItemType.FullName );
            writer.Write( m_ActualPrice );
            writer.Write( m_TotalSold );
        }
    }
}