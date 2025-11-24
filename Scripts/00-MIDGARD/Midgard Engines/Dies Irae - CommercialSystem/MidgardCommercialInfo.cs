/***************************************************************************
 *                               MidgardCommercialInfo.cs
 *
 *   begin                : 27 aprile 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using Server;

namespace Midgard.Engines.CommercialSystem
{
    public class MidgardCommercialCompactInfo : IComparable<MidgardCommercialInfo>
    {
        /// <summary>
        /// The type which is under commercial modifications
        /// </summary>
        public Type ItemType { get; set; }

        public int SellToVendorPrice { get; set; }

        public int BuyFromVendorPrice { get; set; }

        public MidgardCommercialCompactInfo( Type itemType, int sellToVendorPrice, int buyFromVendorPrice )
        {
            ItemType = itemType;
            SellToVendorPrice = sellToVendorPrice;
            BuyFromVendorPrice = buyFromVendorPrice;
        }

        public int CompareTo( MidgardCommercialInfo other )
        {
            return String.Compare( ItemType.Name, other.ItemType.Name, true );
        }

        public override string ToString()
        {
            return string.Format( "{0}: B/S : {1}/{2}", ItemType.Name, BuyFromVendorPrice, SellToVendorPrice );
        }
    }

    public class MidgardCommercialInfo : IComparable<MidgardCommercialInfo>
    {
        /// <summary>
        /// The type which is under commercial modifications
        /// </summary>
        public Type ItemType { get; set; }

        /// <summary>
        /// The minimum scalar applyed to items bought from vendors
        /// </summary>
        private double MinBuyScalar { get; set; }

        /// <summary>
        /// The maximum scalar applyed to items bought from vendors
        /// </summary>
        private double MaxBuyScalar { get; set; }

        /// <summary>
        /// The minimum scalar applyed to items sold to vendors
        /// </summary>
        private double MinSellScalar { get; set; }

        /// <summary>
        /// The maximum scalar applyed to items sold to vendors
        /// </summary>
        private double MaxSellScalar { get; set; }

        /// <summary>
        /// The total quantity of sold items in the world for our Type
        /// </summary>
        private int ActualQuantitySold { get; set; }

        /// <summary>
        /// The total quantity of sold items in the world for our Type
        /// </summary>
        private int ActualQuantityBougth { get; set; }

        /// <summary>
        /// Time of last info modification
        /// </summary>
        private DateTime LastUpdate { get; set; }

        /// <summary>
        /// The actual scalar applyed to items sold to vendors
        /// </summary>
        private double ActualScalar { get; set; }

        /// <summary>
        /// The last scalar delta
        /// </summary>
        private double LastDeltaScalar { get; set; }

        public MidgardCommercialInfo( Type itemType, double minBuyScalar, double maxBuyScalar, double minSellScalar, double maxSellScalar )
        {
            ItemType = itemType;

            MinBuyScalar = minBuyScalar;
            MaxBuyScalar = maxBuyScalar;
            MinSellScalar = minSellScalar;
            MaxSellScalar = maxSellScalar;

            ActualQuantitySold = 0;
            ActualQuantityBougth = 0;
            LastUpdate = DateTime.Now;

            ActualScalar = 1.0;
            LastDeltaScalar = 0.0;
        }

        public MidgardCommercialInfo( Type itemType )
            : this( itemType, Config.DefaultMinBuyScalar, Config.DefaultMaxBuyScalar, Config.DefaultMinSellScalar, Config.DefaultMaxSellScalar )
        {
        }

        internal void RegisterDelta( CommercialDeltaType delta, int amount )
        {
            LastUpdate = DateTime.Now;

            if( delta == CommercialDeltaType.None )
                return;

            double newDelta = Config.CommercialDelta + GetAmountScalar( amount );

            // We are buying from vendor.
            // Nb: a wavongload of item will increase the price.
            // ActualScalar will increase, and sell/buy price will increase too.
            if( delta == CommercialDeltaType.BuyFromVendor )
            {
                ActualScalar = ActualScalar + newDelta;
                ActualQuantityBougth += amount;
                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "Registered buy for type {0}. ActualScalar: {1:F3}, Delta {2:F3}, Amount {3}.", ItemType.Name, ActualScalar, newDelta, amount );
            }

            // We are selling to vendor.
            // Nb: a wavongload of item will lower the price.
            // ActualScalar will lower, and sell/buy price will lower too.
            if( delta == CommercialDeltaType.SellToVendor )
            {
                ActualScalar = ActualScalar - newDelta;
                ActualQuantitySold += amount;
                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "Registered sell for type {0}. ActualScalar: {1:F3}, Delta {2:F3}, Amount {3}.", ItemType.Name, ActualScalar, newDelta, amount );
            }

            LastDeltaScalar = newDelta;
        }

        /// <summary>
        /// Effective scalar applied for commerce from vendor to player 
        /// </summary>
        internal double BuyFromVendorScalar
        {
            get
            {
                if( ActualScalar > MaxBuyScalar )
                    return MaxBuyScalar; // default is 10.0
                else if( ActualScalar < MinBuyScalar )
                    return MinBuyScalar; // default is 0.95
                else
                    return ActualScalar;
            }
        }

        /// <summary>
        /// Effective scalar applied for commerce from player to vendor
        /// </summary>
        internal double SellToVendorScalar
        {
            get
            {
                if( ActualScalar > MaxSellScalar )
                    return MaxSellScalar; // default is 1.05
                else if( ActualScalar < MinSellScalar )
                    return MinSellScalar; // default is 0.001
                else
                    return ActualScalar;
            }
        }

        /// <summary>
        /// Get the refresh status of this info. If true our info is going to decay
        /// </summary>
        internal bool RequiresRefresh
        {
            get { return DateTime.Now > LastUpdate + Config.CommercialInfoDecay; }
        }

        /// <summary>
        /// Get the scalar modification for amount buy/sell purchases
        /// </summary>
        private static double GetAmountScalar( int amount )
        {
            if( amount > 100 )
                return Config.CommercialDelta * 3;
            else if( amount > 50 )
                return Config.CommercialDelta * 2;
            else if( amount > 10 )
                return Config.CommercialDelta * 1;
            else
                return 0;
        }

        internal void ProcessDecay()
        {
            double scalar = ActualScalar;
            if( ( scalar > ( 1 - Config.CommercialDelta ) && scalar < 1 ) || ( scalar < ( 1 + Config.CommercialDelta ) && scalar > 1 ) )
                ActualScalar = 1.00;
            else if( scalar <= ( 1 - Config.CommercialDelta ) )
                ActualScalar += Config.CommercialDelta;
            else if( scalar >= ( 1 + Config.CommercialDelta ) )
                ActualScalar -= Config.CommercialDelta;

            if( scalar != ActualScalar && Config.Debug )
                Config.Pkg.LogInfoLine( "Actual scalar for type {0} changed to {1:F3} (was {2:F3}) for decay.", ItemType.Name, ActualScalar, scalar );
        }

        public int CompareTo( MidgardCommercialInfo other )
        {
            return String.Compare( ItemType.Name, other.ItemType.Name, true );
        }

        public override string ToString()
        {
            return string.Format( "{0}: Scalar {1:F3} - B/S Scalars: {2:F3}/{3:F3} B/S Quantity: {4}/{5}", ItemType.Name, ActualScalar, BuyFromVendorScalar, SellToVendorScalar, ActualQuantityBougth, ActualQuantitySold );
        }

        public void Reset()
        {
            ActualQuantitySold = 0;
            ActualQuantityBougth = 0;
            LastUpdate = DateTime.Now;

            ActualScalar = 1.0;
            LastDeltaScalar = 0.0;
        }

        #region serialization
        private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
        {
            if( setIf )
                flags |= toSet;
        }

        private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
        {
            return ( ( flags & toGet ) != 0 );
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,

            MinBuyScalar = 0x00000001,
            MaxBuyScalar = 0x00000002,
            MinSellScalar = 0x00000004,
            MaxSellScalar = 0x00000008,

            ActualQuantitySold = 0x00000010,
            ActualQuantityBougth = 0x00000020,
            LastUpdate = 0x00000040,
            ActualScalar = 0x00000080,

            LastDeltaScalar = 0x00000100
        }

        public MidgardCommercialInfo( GenericReader reader )
        {
            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        ItemType = ScriptCompiler.FindTypeByName( reader.ReadString() );

                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        MinBuyScalar = GetSaveFlag( flags, SaveFlag.MinBuyScalar ) ? reader.ReadDouble() : Config.DefaultMinBuyScalar;
                        MaxBuyScalar = GetSaveFlag( flags, SaveFlag.MaxBuyScalar ) ? reader.ReadDouble() : Config.DefaultMaxBuyScalar;
                        MinSellScalar = GetSaveFlag( flags, SaveFlag.MinSellScalar ) ? reader.ReadDouble() : Config.DefaultMinSellScalar;
                        MaxSellScalar = GetSaveFlag( flags, SaveFlag.MaxSellScalar ) ? reader.ReadDouble() : Config.DefaultMaxSellScalar;

                        ActualQuantitySold = GetSaveFlag( flags, SaveFlag.ActualQuantitySold ) ? reader.ReadEncodedInt() : 0;
                        ActualQuantityBougth = GetSaveFlag( flags, SaveFlag.ActualQuantityBougth ) ? reader.ReadEncodedInt() : 0;

                        LastUpdate = GetSaveFlag( flags, SaveFlag.LastUpdate ) ? reader.ReadDateTime() : DateTime.MinValue;
                        ActualScalar = GetSaveFlag( flags, SaveFlag.ActualScalar ) ? reader.ReadDouble() : 1.0;
                        LastDeltaScalar = GetSaveFlag( flags, SaveFlag.LastDeltaScalar ) ? reader.ReadDouble() : 0.0;

                        break;
                    }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( 0 ); // version
            
            writer.Write( ItemType.Name );

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag( ref flags, SaveFlag.MinBuyScalar, MinBuyScalar != Config.DefaultMinBuyScalar );
            SetSaveFlag( ref flags, SaveFlag.MaxBuyScalar, MaxBuyScalar != Config.DefaultMaxBuyScalar );
            SetSaveFlag( ref flags, SaveFlag.MinSellScalar, MinSellScalar != Config.DefaultMinSellScalar );
            SetSaveFlag( ref flags, SaveFlag.MaxSellScalar, MaxSellScalar != Config.DefaultMaxSellScalar );

            SetSaveFlag( ref flags, SaveFlag.ActualQuantitySold, ActualQuantitySold != 0 );
            SetSaveFlag( ref flags, SaveFlag.ActualQuantityBougth, ActualQuantityBougth != 0 );
            SetSaveFlag( ref flags, SaveFlag.LastUpdate, LastUpdate != DateTime.MinValue );
            SetSaveFlag( ref flags, SaveFlag.ActualScalar, ActualScalar != 1.0 );

            SetSaveFlag( ref flags, SaveFlag.LastDeltaScalar, LastDeltaScalar != 0 );

            writer.WriteEncodedInt( (int)flags );

            if( GetSaveFlag( flags, SaveFlag.MinBuyScalar ) )
                writer.Write( MinBuyScalar );

            if( GetSaveFlag( flags, SaveFlag.MaxBuyScalar ) )
                writer.Write( MaxBuyScalar );

            if( GetSaveFlag( flags, SaveFlag.MinSellScalar ) )
                writer.Write( MinSellScalar );

            if( GetSaveFlag( flags, SaveFlag.MaxSellScalar ) )
                writer.Write( MaxSellScalar );

            if( GetSaveFlag( flags, SaveFlag.ActualQuantitySold ) )
                writer.WriteEncodedInt( ActualQuantitySold );

            if( GetSaveFlag( flags, SaveFlag.ActualQuantityBougth ) )
                writer.WriteEncodedInt( ActualQuantityBougth );

            if( GetSaveFlag( flags, SaveFlag.LastUpdate ) )
                writer.Write( LastUpdate );

            if( GetSaveFlag( flags, SaveFlag.ActualScalar ) )
                writer.Write( ActualScalar );

            if( GetSaveFlag( flags, SaveFlag.LastDeltaScalar ) )
                writer.Write( LastDeltaScalar );
        }
        #endregion
    }
}