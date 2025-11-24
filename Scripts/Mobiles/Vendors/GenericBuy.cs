// #define TRACKTRACE
// #define DebugGenericBuyInfo

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Server.Items;
using Server.Engines.XmlSpawner2;
using CommercialSystem = Midgard.Engines.CommercialSystem;

namespace Server.Mobiles
{
	public class GenericBuyInfo : IBuyItemInfo
    {
        #region modifica by Dies Irae
        public static void ClearCache()
        {
            DisplayCache.Cache.Delete();
        }

        public static int GetCacheMobilesCount()
        {
            return DisplayCache.Cache.MobilesCount;
        }

        public static int GetCacheItemsCount()
        {
            return DisplayCache.Cache.ItemsCount;
        }
        #endregion

        private class DisplayCache : Container
		{
			private static DisplayCache m_Cache;

			public static DisplayCache Cache {
				get {
					if ( m_Cache == null || m_Cache.Deleted )
						m_Cache = new DisplayCache();

					return m_Cache;
				}
			}

			private Dictionary<Type, IEntity> m_Table;
			private List<Mobile> m_Mobiles;

            #region mod by Dies Irae
            public int MobilesCount
            {
                get
                {
                    if( m_Mobiles == null )
                        return 0;

                    return m_Mobiles.Count;
                }
            }

            public int ItemsCount
            {
                get
                {
                    return Items.Count;
                }
            }

            public void ClearCache()
            {
				for ( int i = 0; i < m_Mobiles.Count; ++i )
					m_Mobiles[i].Delete();

				m_Mobiles.Clear();

				for ( int i = Items.Count - 1; i >= 0; --i )
					if ( i < Items.Count )
						Items[i].Delete();
            }
            #endregion

            public DisplayCache() : base( 0 )
			{
				m_Table = new Dictionary<Type, IEntity>();
				m_Mobiles = new List<Mobile>();
			}

			public IEntity Lookup( Type key )
			{
				IEntity e = null;
				m_Table.TryGetValue( key, out e );
				return e;
			}

			public void Store( Type key, IEntity obj, bool cache )
			{
#if TRACKTRACE
                StackTrace st = new StackTrace( true );
                string stackIndent = String.Empty;
                for( int i = 0; i < st.FrameCount; i++ )
                {
                    StackFrame sf = st.GetFrame( i );
                    Console.WriteLine();
                    Console.WriteLine( stackIndent + " Method: {0}", sf.GetMethod().Name );
                    Console.WriteLine( stackIndent + " File: {0}", sf.GetFileName() );
                    Console.WriteLine( stackIndent + " Line Number: {0}", sf.GetFileLineNumber() );
                    Console.WriteLine( stackIndent + " Key {0} - Object {1} - Cache {2}", key.Name, obj.GetType().Name, cache.ToString() );
                    stackIndent += "  ";
                }
#endif
				if ( cache )
					m_Table[key] = obj;

                XmlAttach.AttachTo( obj, new XmlCachedItem() ); // mod by Dies Irae

				if ( obj is Item )
					AddItem( (Item)obj );
				else if ( obj is Mobile )
					m_Mobiles.Add( (Mobile)obj );
			}

			public DisplayCache( Serial serial ) : base( serial )
			{
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				for ( int i = 0; i < m_Mobiles.Count; ++i )
					m_Mobiles[i].Delete();

				m_Mobiles.Clear();

				for ( int i = Items.Count - 1; i >= 0; --i )
					if ( i < Items.Count )
						Items[i].Delete();

				if ( m_Cache == this )
					m_Cache = null;
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 0 ); // version

				writer.Write( m_Mobiles );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				m_Mobiles = reader.ReadStrongMobileList();

                if( Core.Debug )
                    Console.WriteLine( "Deleting {0} mobiles from DisplayCache container.", m_Mobiles.Count );

				for ( int i = 0; i < m_Mobiles.Count; ++i )
					m_Mobiles[i].Delete();

				m_Mobiles.Clear();

                if( Core.Debug )
                    Console.WriteLine( "Deleting {0} items from DisplayCache container.", Items.Count );

				for ( int i = Items.Count - 1; i >= 0; --i )
					if ( i < Items.Count )
						Items[i].Delete();

				if ( m_Cache == null )
					m_Cache = this;
				else
					Delete();

				m_Table = new Dictionary<Type, IEntity>();
			}
		}

		private Type m_Type;
		private string m_Name;
		private int m_Price;
		private int m_MaxAmount, m_Amount;
		private int m_ItemID;
		private int m_Hue;
		private object[] m_Args;
		private IEntity m_DisplayEntity;

		public virtual int ControlSlots{ get{ return 0; } }

		public virtual bool CanCacheDisplay{ get{ return false; } } //return ( m_Args == null || m_Args.Length == 0 ); } 

		private bool IsDeleted( IEntity obj )
		{
			if ( obj is Item )
				return ((Item)obj).Deleted;
			else if ( obj is Mobile )
				return ((Mobile)obj).Deleted;

			return false;
		}

		public void DeleteDisplayEntity()
		{
			if ( m_DisplayEntity == null )
				return;

			m_DisplayEntity.Delete();
			m_DisplayEntity = null;
		}

		public IEntity GetDisplayEntity()
		{
			if ( m_DisplayEntity != null && !IsDeleted( m_DisplayEntity ) )
				return m_DisplayEntity;

			bool canCache = this.CanCacheDisplay;

			if ( canCache )
				m_DisplayEntity = DisplayCache.Cache.Lookup( m_Type );

			if ( m_DisplayEntity == null || IsDeleted( m_DisplayEntity ) )
				m_DisplayEntity = GetEntity();

			DisplayCache.Cache.Store( m_Type, m_DisplayEntity, canCache );

			return m_DisplayEntity;
		}

		public Type Type
		{
			get{ return m_Type; }
			set{ m_Type = value; }
		}

		public string Name
		{
			get{ return m_Name; }
			set{ m_Name = value; }
		}

		public int DefaultPrice{ get{ return m_PriceScalar; } }

		private int m_PriceScalar;

		public int PriceScalar
		{
			get{ return m_PriceScalar; }
			set{ m_PriceScalar = value; }
        }

        #region mod by Dies Irae
	    public int TownPrice { get; set; }

	    public int KarmaScalar { get; set; }

	    public bool IsTownBuyInfo { get; set; }

	    public bool IsFromSameGuild { get; set; }

	    public int OriginalPrice
	    {
	        get { return m_Price; }
	    }
        #endregion

        public int Price
		{
			get
			{
			    #region modifica by Dies irae
#if DebugGenericBuyInfo
                Console.WriteLine( "Name: {0} - IsTown: {1} TS: {2} - KS: {3} - SameG: {4}", m_Type.Name, IsTownBuyInfo, TownPrice, KarmaScalar, IsFromSameGuild );
#endif          
                int customPrice = -1;
                if( IsTownBuyInfo && TownPrice > 0 )
                    customPrice = TownPrice;
#if DebugGenericBuyInfo
                Console.WriteLine( "Name: {0} - customPrice: {1} - bool {2}", m_Type.Name, customPrice, ( IsTownBuyInfo && TownPrice > 0 ) );
#endif
                if( customPrice != -1 )
                {
                    if( KarmaScalar > 0 )
                        customPrice = Math.Max( (int)( ( customPrice * KarmaScalar ) / 100.0 ), 1 );

                    if( IsFromSameGuild )
                        customPrice = Math.Max( (int)( customPrice * 0.90 ), 1 );

                    if( m_Type != null && CommercialSystem.Core.IsUnderCommercialRules( m_Type ) )
                        customPrice = (int)( customPrice * CommercialSystem.Core.GetBuyFromVendorScalarFor( m_Type ) );

                    return customPrice;
                }
                #endregion

				if ( m_PriceScalar != 0 )
				{
					if ( m_Price > 5000000 )
					{
						long price = m_Price;

						price *= m_PriceScalar;
						price += 50;
						price /= 100;

						if ( price > int.MaxValue )
							price = int.MaxValue;

						return (int)price;
					}

					return ( ((m_Price * m_PriceScalar) + 50) / 100 );
				}

                if( m_Type != null && CommercialSystem.Core.IsUnderCommercialRules( m_Type ) )
                    return (int)( m_Price * CommercialSystem.Core.GetBuyFromVendorScalarFor( m_Type ) );

#if DebugGenericBuyInfo
                Console.WriteLine( "Name: {0} - finalPrice", m_Type.Name, m_Price );
#endif

				return m_Price;
			}
			set{ m_Price = value; }
		}

		public int ItemID
		{
			get{ return m_ItemID; }
			set{ m_ItemID = value; }
		}

		public int Hue
		{
			get{ return m_Hue; }
			set{ m_Hue = value; }
		}

		public int Amount
		{
			get{ return m_Amount; }
			set{ if ( value < 0 ) value = 0; m_Amount = value; }
		}

		public int MaxAmount
		{
			get{ return m_MaxAmount; }
			set{ m_MaxAmount = value; }
		}

		public object[] Args
		{
			get{ return m_Args; }
			set{ m_Args = value; }
		}

		public GenericBuyInfo( Type type, int price, int amount, int itemID, int hue ) : this( null, type, price, amount, itemID, hue, null )
		{
		}

		public GenericBuyInfo( string name, Type type, int price, int amount, int itemID, int hue ) : this( name, type, price, amount, itemID, hue, null )
		{
		}

		public GenericBuyInfo( Type type, int price, int amount, int itemID, int hue, object[] args ) : this( null, type, price, amount, itemID, hue, args )
		{
		}

		public GenericBuyInfo( string name, Type type, int price, int amount, int itemID, int hue, object[] args )
		{
			m_Type = type;
			m_Price = price;
			m_MaxAmount = m_Amount = amount;
			m_ItemID = itemID;
			m_Hue = hue;
			m_Args = args;

			if ( name == null )
				m_Name = (1020000 + (itemID & 0x3FFF)).ToString();
			else
				m_Name = name;

            CommercialSystem.Core.RegisterCommercialBuy( this ); // mod by Dies Irae
		}

		//get a new instance of an object (we just bought it)
		public virtual IEntity GetEntity()
		{
		    try
		    {
		        if ( m_Args == null || m_Args.Length == 0 )
		            return (IEntity)Activator.CreateInstance( m_Type );

		        return (IEntity)Activator.CreateInstance( m_Type, m_Args );
		    }
            catch( Exception e )
            {
                AppendSafetyLog( m_Type );
                Console.WriteLine( e.ToString() );
                return null;
            }
			//return (Item)Activator.CreateInstance( m_Type );
		}

        #region mod by Dies Irae
        private static void AppendSafetyLog( Type t )
        {
            string message = String.Format( "Warning: Nessun costruttore senza parametri definito per l'oggetto \"{0}\".", t.Name );
            Console.WriteLine( message );

            try
            {
                using( StreamWriter op = new StreamWriter( "Logs/system-errors.log", true ) )
                {
                    op.WriteLine( "{0}\t{1}", DateTime.Now, message );
                    op.WriteLine( new StackTrace( 2 ).ToString() );
                    op.WriteLine();
                }
            }
            catch
            {
            }
        }
        #endregion

	    //Attempt to restock with item, (return true if restock sucessful)
	    public bool Restock( Item item, int amount )
	    {
	        return false;
	        /*if ( item.GetType() == m_Type )
			{
				if ( item is BaseWeapon )
				{
					BaseWeapon weapon = (BaseWeapon)item;

					if ( weapon.Quality == WeaponQuality.Low || weapon.Quality == WeaponQuality.Exceptional || (int)weapon.DurabilityLevel > 0 || (int)weapon.DamageLevel > 0 || (int)weapon.AccuracyLevel > 0 )
						return false;
				}

				if ( item is BaseArmor )
				{
					BaseArmor armor = (BaseArmor)item;

					if ( armor.Quality == ArmorQuality.Low || armor.Quality == ArmorQuality.Exceptional || (int)armor.Durability > 0 || (int)armor.ProtectionLevel > 0 )
						return false;
				}

				m_Amount += amount;

				return true;
			}
			else
			{
				return false;
			}*/
	    }

	    public void OnRestock()
	    {
	        if( Core.AOS )
	        {
	            m_Amount = m_MaxAmount;
	            return;
	        }

	        if ( m_Amount <= 0 )
	        {
	            m_MaxAmount *= 2;

	            if ( m_MaxAmount >= 999 )
	                m_MaxAmount = 999;
	        }
	        else
	        {
	            /* NOTE: According to UO.com, the quantity is halved if the item does not reach 0
				 * Here we implement differently: the quantity is halved only if less than half
				 * of the maximum quantity was bought. That is, if more than half is sold, then
				 * there's clearly a demand and we should not cut down on the stock.
				 */

	            int halfQuantity = m_MaxAmount;

	            if ( halfQuantity >= 999 )
	                halfQuantity = 640;
	            else if ( halfQuantity > 100 )
	                halfQuantity /= 2;

	            if ( m_Amount >= halfQuantity )
	                m_MaxAmount = halfQuantity;
	        }

	        m_Amount = m_MaxAmount;
	    }
    }
}