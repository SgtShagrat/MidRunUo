using System;
using System.Collections.Generic;
using Server.Items;

using CommercialSystem = Midgard.Engines.CommercialSystem;

namespace Server.Mobiles
{
	public class GenericSellInfo : IShopSellInfo
	{
	    private Dictionary<Type, int> m_Table = new Dictionary<Type, int>();
		private Type[] m_Types;

		public GenericSellInfo()
		{
		}

		public void Add( Type type, int price )
		{
			m_Table[type] = price;
			m_Types = null;

            CommercialSystem.Core.RegisterCommercialSell( type, price ); // mod by Dies Irae
		}

        #region mod by Dies Irae
	    public virtual double Demultiplicator
	    {
	        get { return -1.0; }
	    }

        public void SetSellPriceFor( Type t, int newPrice )
        {
            if( !IsInList( t ) )
            {
                Console.WriteLine( "Warning: trying to set an invalid price. Type {0}", t.Name );
                return;
            }

            m_Table[ t ] = newPrice;
        }
        #endregion

        public bool GetSellPriceFor( Type t, out int price )
        {
            if( !m_Table.TryGetValue( t, out price ) )
                return false;

            if( CommercialSystem.Core.IsUnderCommercialRules( t ) )
                price = (int)( price * CommercialSystem.Core.GetSellToVendorScalarFor( t ) );

            return true;
        }

        public int GetSellPriceFor( Item item )
        {
            return GetSellPriceFor( item, false );
        }

		public int GetSellPriceFor( Item item, bool ignoreScalar )
		{
			int price = 0;
			// m_Table.TryGetValue( item.GetType(), out price );
            GetSellPriceFor( item.GetType(), out price );

			if ( item is BaseArmor ) {
				BaseArmor armor = (BaseArmor)item;

				if ( armor.Quality == ArmorQuality.Low )
					price = (int)( price * 0.60 );
				else if ( armor.Quality == ArmorQuality.Exceptional )
					price = (int)( price * 1.25 );

				price += 100 * (int)armor.Durability;

				price += 100 * (int)armor.ProtectionLevel;

                #region mod by Dies Irae
                CraftResourceInfo resInfo = CraftResources.GetInfo( armor.Resource );
                if( resInfo != null )
                {
                    CraftAttributeInfo attrInfo = resInfo.AttributeInfo;
                    if( attrInfo != null && attrInfo.OldStaticMultiply > 1.0 )
                        price = (int)( price * attrInfo.OldStaticMultiply );
                }
                #endregion

				if ( price < 1 )
					price = 1;
			}
			else if ( item is BaseWeapon ) {
				BaseWeapon weapon = (BaseWeapon)item;

				if ( weapon.Quality == WeaponQuality.Low )
					price = (int)( price * 0.60 );
				else if ( weapon.Quality == WeaponQuality.Exceptional )
					price = (int)( price * 1.25 );

				price += 100 * (int)weapon.DurabilityLevel;

				price += 100 * (int)weapon.DamageLevel;

                #region mod by Dies Irae
                CraftResourceInfo resInfo = CraftResources.GetInfo( weapon.Resource );
                if( resInfo != null )
                {
                    CraftAttributeInfo attrInfo = resInfo.AttributeInfo;
                    if( attrInfo != null && attrInfo.OldStaticMultiply > 1.0 )
                        price = (int)( price * attrInfo.OldStaticMultiply );
                }
                #endregion

				if ( price < 1 )
					price = 1;
			}
			else if ( item is BaseBeverage ) {
				int price1 = price, price2 = price;

				if ( item is Pitcher )
				{ price1 = 3; price2 = 5; }
				else if ( item is BeverageBottle )
				{ price1 = 3; price2 = 3; }
				else if ( item is Jug )
				{ price1 = 6; price2 = 6; }

				BaseBeverage bev = (BaseBeverage)item;

				if ( bev.IsEmpty || bev.Content == BeverageType.Milk )
					price = price1;
				else
					price = price2;
			}

            #region mod by Dies Irae
            if( !ignoreScalar && CommercialSystem.Config.SellInfoScalarnabled && Demultiplicator > 0.0 )
            {
                price = (int)( price * Demultiplicator );
				if ( price < 1 )
					price = 1;
            }
            #endregion

			return price;
		}

		public int GetBuyPriceFor( Item item )
		{
			return (int)( 1.90 * GetSellPriceFor( item, true ) );
		}

		public Type[] Types
		{
			get
			{
				if ( m_Types == null )
				{
					m_Types = new Type[m_Table.Keys.Count];
					m_Table.Keys.CopyTo( m_Types, 0 );
				}

				return m_Types;
			}
		}

		public string GetNameFor( Item item )
		{
			if ( item.Name != null )
				return item.Name;
			else
				return item.LabelNumber.ToString();
		}

		public bool IsSellable( Item item )
		{
			#region Mondain's Legacy
			if ( item.QuestItem )
				return false;
			#endregion
			
			//if ( item.Hue != 0 )
				//return false;

			return IsInList( item.GetType() );
		}
	 
		public bool IsResellable( Item item )
		{
			#region Mondain's Legacy
			if ( item.QuestItem )
				return false;
			#endregion
			
			//if ( item.Hue != 0 )
				//return false;

			return IsInList( item.GetType() );
		}

		public bool IsInList( Type type )
		{
			return m_Table.ContainsKey( type );
		}
	}
}
