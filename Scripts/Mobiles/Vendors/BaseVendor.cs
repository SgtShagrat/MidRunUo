using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Server.Items;
using Server.Network;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Misc;
using Server.Engines.BulkOrders;
using Server.Regions;
using Server.Factions;
using Server.Targeting;

using Midgard.Engines.MidgardTownSystem;
using Server.Engines.Quests;
using CommercialSystem = Midgard.Engines.CommercialSystem;

namespace Server.Mobiles
{
	public enum VendorShoeType
	{
		None,
		Shoes,
		Boots,
		Sandals,
		ThighBoots
	}

	public abstract class BaseVendor : BaseCreature, IVendor, IMidgardQuester
	{
		private const int MaxSell = 500;

		protected abstract List<SBInfo> SBInfos { get; }

		private ArrayList m_ArmorBuyInfo = new ArrayList();
		private ArrayList m_ArmorSellInfo = new ArrayList();

		private DateTime m_LastRestock;

		public override bool CanTeach { get { return true; } }

		public override bool BardImmune { get { return true; } }

		public override bool PlayerRangeSensitive { get { return true; } }

		public virtual bool IsActiveVendor { get { return true; } }
		public virtual bool IsActiveBuyer { get { return IsActiveVendor; } } // response to vendor SELL
		public virtual bool IsActiveSeller { get { return IsActiveVendor; } } // repsonse to vendor BUY

		public virtual NpcGuild NpcGuild { get { return NpcGuild.None; } }

		public virtual bool IsInvulnerable { get { return true; } }

		public override bool ShowFameTitle { get { return false; } }

		public virtual bool IsValidBulkOrder( Item item )
		{
			return false;
		}

		public virtual Item CreateBulkOrder( Mobile from, bool fromContextMenu )
		{
			return null;
		}

		public virtual bool SupportsBulkOrders( Mobile from )
		{
			return false;
		}

		public virtual TimeSpan GetNextBulkOrder( Mobile from )
		{
			return TimeSpan.Zero;
		}

		public virtual void OnSuccessfulBulkOrderReceive( Mobile from )
		{
		}

		#region Faction
		public virtual int GetPriceScalar()
		{
			Town town = Town.FromRegion( this.Region );

			if ( town != null )
				return ( 100 + town.Tax );

			return 100;
		}
        #endregion

        public void UpdateBuyInfo( Mobile from )
		{
            #region mod by Dies Irae
            int guildScalar = ( (PlayerMobile)from ).NpcGuild == NpcGuild ? 90 : 100;
            int karmaScalar = Karma > 0 ? GetKarmaScalar( from ) : 100;
            int regionScalar = GetRegionalScalar();
            #endregion

			int priceScalar = GetPriceScalar();
            bool sameGuild = ((PlayerMobile) from).NpcGuild == NpcGuild;

            priceScalar = (int)( priceScalar * ( guildScalar / 100.0 ) * ( karmaScalar / 100.0 ) * ( regionScalar / 100.0 ) ); // mod by Dies Irae

			IBuyItemInfo[] buyinfo = (IBuyItemInfo[])m_ArmorBuyInfo.ToArray( typeof( IBuyItemInfo ) );

			if ( buyinfo != null )
			{
				foreach ( IBuyItemInfo info in buyinfo )
				{
				    info.PriceScalar = priceScalar;
                    info.KarmaScalar = karmaScalar;
				    info.IsFromSameGuild = sameGuild;
				}
			}

            #region mod by Dies Irae : townsystem
            TownSystem system = TownSystem.Find( this );
            if( system != null && buyinfo != null )
                system.UpdateBuyInfo( from, this, buyinfo );
            #endregion
        }

		private class BulkOrderInfoEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private BaseVendor m_Vendor;

			public BulkOrderInfoEntry( Mobile from, BaseVendor vendor )
				: base( 6152, 6 )
			{
				m_From = from;
				m_Vendor = vendor;
			}

			public override void OnClick()
			{
				if ( m_Vendor.SupportsBulkOrders( m_From ) )
				{
					TimeSpan ts = m_Vendor.GetNextBulkOrder( m_From );

					int totalSeconds = (int)ts.TotalSeconds;
					int totalHours = ( totalSeconds + 3599 ) / 3600;
					int totalMinutes = ( totalSeconds + 59 ) / 60;

					if ( ( ( Core.SE ) ? totalMinutes == 0 : totalHours == 0 ) )
					{
						m_From.SendLocalizedMessage( 1049038 ); // You can get an order now.

						if ( Core.AOS )
						{
							Item bulkOrder = m_Vendor.CreateBulkOrder( m_From, true );

							#region modifica by Berto
							if ( bulkOrder is SmallMobileBOD )
								m_From.SendGump( new SmallMobileBODAcceptGump( m_From, (SmallMobileBOD)bulkOrder ) );
							else if ( bulkOrder is LargeMobileBOD )
								m_From.SendGump( new LargeMobileBODAcceptGump( m_From, (LargeMobileBOD)bulkOrder ) );
							else if ( bulkOrder is LargeBOD )
							// if ( bulkOrder is LargeBOD )
							#endregion
								m_From.SendGump( new LargeBODAcceptGump( m_From, (LargeBOD)bulkOrder ) );
							else if ( bulkOrder is SmallBOD )
								m_From.SendGump( new SmallBODAcceptGump( m_From, (SmallBOD)bulkOrder ) );
						}
					}
					else
					{
						int oldSpeechHue = m_Vendor.SpeechHue;
						m_Vendor.SpeechHue = 0x3B2;

						if ( Core.SE )
							m_Vendor.SayTo( m_From, 1072058, totalMinutes.ToString() ); // An offer may be available in about ~1_minutes~ minutes.
						else
							m_Vendor.SayTo( m_From, 1049039, totalHours.ToString() ); // An offer may be available in about ~1_hours~ hours.

						m_Vendor.SpeechHue = oldSpeechHue;
					}
				}
			}
		}

		public BaseVendor( string title )
			: base( AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2 )
		{
			LoadSBInfo();

			this.Title = title;
			InitBody();
			InitOutfit();

			Container pack;
			//these packs MUST exist, or the client will crash when the packets are sent
			pack = new Backpack();
			pack.Layer = Layer.ShopBuy;
			pack.Movable = false;
			pack.Visible = false;
			AddItem( pack );

			pack = new Backpack();
			pack.Layer = Layer.ShopResale;
			pack.Movable = false;
			pack.Visible = false;
			AddItem( pack );

			m_LastRestock = DateTime.Now;

			IsYoungDealer = false; // Modifica by Dies Irae per i i Vendors Young Dealers
			MidgardTown = MidgardTowns.None; // Modifica by Dies Irae per i i Vendors Cittadini
		}
		
		public BaseVendor( Serial serial )
			: base( serial )
		{
		}

		public DateTime LastRestock
		{
			get
			{
				return m_LastRestock;
			}
			set
			{
				m_LastRestock = value;
			}
		}

		public virtual TimeSpan RestockDelay
		{
			get
			{
				return TimeSpan.FromHours( 1 );
			}
		}

		public Container BuyPack
		{
			get
			{
				Container pack = FindItemOnLayer( Layer.ShopBuy ) as Container;

				if ( pack == null )
				{
					pack = new Backpack();
					pack.Layer = Layer.ShopBuy;
					pack.Visible = false;
					AddItem( pack );
				}

				return pack;
			}
		}

		public abstract void InitSBInfo();

		public virtual bool IsTokunoVendor { get { return ( Map == Map.Tokuno ); } }

		protected void LoadSBInfo()
		{
			m_LastRestock = DateTime.Now;

			for ( int i = 0; i < m_ArmorBuyInfo.Count; ++i )
			{
				GenericBuyInfo buy = m_ArmorBuyInfo[i] as GenericBuyInfo;

				if ( buy != null )
					buy.DeleteDisplayEntity();
			}

			SBInfos.Clear();

			InitSBInfo();

			m_ArmorBuyInfo.Clear();
			m_ArmorSellInfo.Clear();

			for ( int i = 0; i < SBInfos.Count; i++ )
			{
				SBInfo sbInfo = (SBInfo)SBInfos[i];
				m_ArmorBuyInfo.AddRange( sbInfo.BuyInfo );
				m_ArmorSellInfo.Add( sbInfo.SellInfo );
			}

		    LogInfo();
		}

		public virtual bool GetGender()
		{
			return Utility.RandomBool();
		}

		public virtual void InitBody()
		{
			InitStats( 100, 100, 25 );

			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();

            #region mod by Dies Irae
            if( !Core.AOS )
                SpeechHue = ( Utility.RandomMinMax( 0x01, 0xC8 ) * 5 ) - 0x01 + Utility.RandomMinMax( 0x00, 0x02 );
            #endregion

		    #region mod by Dies Irae
		    //if ( IsInvulnerable && !Core.AOS )
		    //    NameHue = 0x35;
            #endregion

			if ( Female = GetGender() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}
		}

		public virtual int GetRandomHue()
		{
			switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
			}
		}

		public virtual int GetShoeHue()
		{
			if ( 0.1 > Utility.RandomDouble() )
				return 0;

			return Utility.RandomNeutralHue();
		}

		public virtual VendorShoeType ShoeType
		{
			get { return VendorShoeType.Shoes; }
		}

		public virtual int RandomBrightHue()
		{
			if ( 0.1 > Utility.RandomDouble() )
				return Utility.RandomList( 0x62, 0x71 );

			return Utility.RandomList( 0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59 );
		}

		public virtual void CheckMorph()
		{
			if ( CheckGargoyle() )
				return;

			if ( CheckNecromancer() )
				return;

			#region modifica by Dies Irae
			if( CheckMidgardTown() )
				return;
			#endregion

			CheckTokuno();
		}

		public virtual bool CheckTokuno()
		{
			if ( this.Map != Map.Tokuno )
				return false;

			NameList n;

			if ( Female )
				n = NameList.GetNameList( "tokuno female" );
			else
				n = NameList.GetNameList( "tokuno male" );

			if ( !n.ContainsName( this.Name ) )
				TurnToTokuno();

			return true;
		}

		public virtual void TurnToTokuno()
		{
			if ( Female )
				this.Name = NameList.RandomName( "tokuno female" );
			else
				this.Name = NameList.RandomName( "tokuno male" );
		}

		public virtual bool CheckGargoyle()
		{
			Map map = this.Map;

			if ( map != Map.Ilshenar )
				return false;

			if ( !Region.IsPartOf( "Gargoyle City" ) )
				return false;

			if ( Body != 0x2F6 || ( Hue & 0x8000 ) == 0 )
				TurnToGargoyle();

			return true;
		}

		public virtual bool CheckNecromancer()
		{
			Map map = this.Map;

			if ( map != Map.Malas )
				return false;

			if ( !Region.IsPartOf( "Umbra" ) )
				return false;

			if ( Hue != 0x83E8 )
				TurnToNecromancer();

			return true;
		}

		public override void OnAfterSpawn()
		{
			CheckMorph();
		}

		protected override void OnMapChange( Map oldMap )
		{
			base.OnMapChange( oldMap );

			CheckMorph();

			LoadSBInfo();
		}

		public virtual int GetRandomNecromancerHue()
		{
			switch ( Utility.Random( 20 ) )
			{
				case 0: return 0;
				case 1: return 0x4E9;
				default: return Utility.RandomList( 0x485, 0x497 );
			}
		}

		public virtual void TurnToNecromancer()
		{
			for ( int i = 0; i < this.Items.Count; ++i )
			{
				Item item = this.Items[i];

				if ( item is Hair || item is Beard )
					item.Hue = 0;
				else if ( item is BaseClothing || item is BaseWeapon || item is BaseArmor || item is BaseTool )
					item.Hue = GetRandomNecromancerHue();
			}

			HairHue = 0;
			FacialHairHue = 0;

			Hue = 0x83E8;
		}

		public virtual void TurnToGargoyle()
		{
			for ( int i = 0; i < this.Items.Count; ++i )
			{
				Item item = this.Items[i];

				if ( item is BaseClothing || item is Hair || item is Beard )
					item.Delete();
			}

			HairItemID = 0;
			FacialHairItemID = 0;

			Body = 0x2F6;
			Hue = RandomBrightHue() | 0x8000;
			Name = NameList.RandomName( "gargoyle vendor" );

			CapitalizeTitle();
		}

		public virtual void CapitalizeTitle()
		{
			string title = this.Title;

			if ( title == null )
				return;

			string[] split = title.Split( ' ' );

			for ( int i = 0; i < split.Length; ++i )
			{
				if ( Insensitive.Equals( split[i], "the" ) )
					continue;

				if ( split[i].Length > 1 )
					split[i] = Char.ToUpper( split[i][0] ) + split[i].Substring( 1 );
				else if ( split[i].Length > 0 )
					split[i] = Char.ToUpper( split[i][0] ).ToString();
			}

			this.Title = String.Join( " ", split );
		}

		public virtual int GetHairHue()
		{
			return Utility.RandomHairHue();
		}

		public virtual void InitOutfit()
		{
			switch ( Utility.Random( 3 ) )
			{
				case 0: AddItem( new FancyShirt( GetRandomHue() ) ); break;
				case 1: AddItem( new Doublet( GetRandomHue() ) ); break;
				case 2: AddItem( new Shirt( GetRandomHue() ) ); break;
			}

			switch ( ShoeType )
			{
				case VendorShoeType.Shoes: AddItem( new Shoes( GetShoeHue() ) ); break;
				case VendorShoeType.Boots: AddItem( new Boots( GetShoeHue() ) ); break;
				case VendorShoeType.Sandals: AddItem( new Sandals( GetShoeHue() ) ); break;
				case VendorShoeType.ThighBoots: AddItem( new ThighBoots( GetShoeHue() ) ); break;
			}

			int hairHue = GetHairHue();

			Utility.AssignRandomHair( this, hairHue );
			Utility.AssignRandomFacialHair( this, hairHue );

			if ( Female )
			{
				switch ( Utility.Random( 6 ) )
				{
					case 0: AddItem( new ShortPants( GetRandomHue() ) ); break;
					case 1:
					case 2: AddItem( new Kilt( GetRandomHue() ) ); break;
					case 3:
					case 4:
					case 5: AddItem( new Skirt( GetRandomHue() ) ); break;
				}
			}
			else
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: AddItem( new LongPants( GetRandomHue() ) ); break;
					case 1: AddItem( new ShortPants( GetRandomHue() ) ); break;
				}
			}

			PackGold( 100, 200 );
		}

		public virtual void Restock()
		{
			m_LastRestock = DateTime.Now;

			IBuyItemInfo[] buyInfo = this.GetBuyInfo();

			foreach ( IBuyItemInfo bii in buyInfo )
				bii.OnRestock();

            #region mod by Dies Irae : town system
            TownSystem t = TownSystem.Find( this );
            if( t != null )
            {
                TownVendorState state = t.CommercialStatus.FindVendorState( this );
                if( state != null )
                    state.EvaluateTownFinancialStatus( this );
            }
            #endregion
		}

		private static TimeSpan InventoryDecayTime = TimeSpan.FromHours( 1.0 );

		public virtual void VendorBuy( Mobile from )
		{
			if ( !IsActiveSeller )
				return;

			if ( !from.CheckAlive() )
				return;

			#region modifica by Dies per i Vendors YoungDealers
			if( !YoungDeal( this, from ) )
		    	return;
			#endregion
			
			#region modifica by Dies per i Vendor cittadini
            TownSystem t = TownSystem.Find( this );
            if( t != null && !t.VendorBuyAllowed )
                return;

            if( t != null )
            {
                TownVendorState state = t.CommercialStatus.FindVendorState( this );
                if( state != null && !state.CheckAccess( false, true, this ) )
                    return;
            }

			if( !CitizenDeal( this, from, false, true ) )
				return;
			#endregion

			if ( !CheckVendorAccess( from ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return;
			}

			if ( DateTime.Now - m_LastRestock > RestockDelay )
				Restock();

			UpdateBuyInfo( from ); // mod by Dies Irae

			int count = 0;
			List<BuyItemState> list;
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			IShopSellInfo[] sellInfo = this.GetSellInfo();

            list = new List<BuyItemState>( buyInfo.Length );
			Container cont = this.BuyPack;

			List<ObjectPropertyList> opls = null;

			for ( int idx = 0; idx < buyInfo.Length; idx++ )
			{
				IBuyItemInfo buyItem = (IBuyItemInfo)buyInfo[idx];

				if ( buyItem.Amount <= 0 || list.Count >= 250 )
					continue;

				// NOTE: Only GBI supported; if you use another implementation of IBuyItemInfo, this will crash
				GenericBuyInfo gbi = (GenericBuyInfo)buyItem;
				IEntity disp = gbi.GetDisplayEntity();

				list.Add( new BuyItemState( buyItem.Name, cont.Serial, disp == null ? (Serial)0x7FC0FFEE : disp.Serial, buyItem.Price, buyItem.Amount, buyItem.ItemID, buyItem.Hue ) );
				count++;

				if ( opls == null ) {
					opls = new List<ObjectPropertyList>();
				}

				if ( disp is Item ) {
					opls.Add( ( ( Item ) disp ).PropertyList );
				} else if ( disp is Mobile ) {
					opls.Add( ( ( Mobile ) disp ).PropertyList );
				}
			}

			List<Item> playerItems = cont.Items;

			for ( int i = playerItems.Count - 1; i >= 0; --i )
			{
				if ( i >= playerItems.Count )
					continue;

				Item item = playerItems[i];

				if ( ( item.LastMoved + InventoryDecayTime ) <= DateTime.Now )
					item.Delete();
			}

			for ( int i = 0; i < playerItems.Count; ++i )
			{
				Item item = playerItems[i];

				int price = 0;
				string name = null;

				foreach ( IShopSellInfo ssi in sellInfo )
				{
					if ( ssi.IsSellable( item ) )
					{
						price = ssi.GetBuyPriceFor( item );
						name = ssi.GetNameFor( item );
						break;
					}
				}

				if ( name != null && list.Count < 250 )
				{
					list.Add( new BuyItemState( name, cont.Serial, item.Serial, price, item.Amount, item.ItemID, item.Hue ) );
					count++;

					if ( opls == null ) {
						opls = new List<ObjectPropertyList>();
					}

					opls.Add( item.PropertyList );
				}
			}

			//one (not all) of the packets uses a byte to describe number of items in the list.  Osi = dumb.
			//if ( list.Count > 255 )
			//	Console.WriteLine( "Vendor Warning: Vendor {0} has more than 255 buy items, may cause client errors!", this );

			if ( list.Count > 0 )
			{
				list.Sort( new BuyItemStateComparer() );

				SendPacksTo( from );

				if ( from.NetState == null )
					return;

				if ( from.NetState.ContainerGridLines )
					from.Send( new VendorBuyContent6017( list ) );
				else
					from.Send( new VendorBuyContent( list ) );
				from.Send( new VendorBuyList( this, list ) );
				from.Send( new DisplayBuyList( this ) );
				from.Send( new MobileStatusExtended( from ) );//make sure their gold amount is sent

				if ( opls != null ) {
					for ( int i = 0; i < opls.Count; ++i ) {
						from.Send( opls[i] );
					}
				}

				SayTo( from, 500186 ); // Greetings.  Have a look around.
			}
		}

		public virtual void SendPacksTo( Mobile from )
		{
			Item pack = FindItemOnLayer( Layer.ShopBuy );

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Layer = Layer.ShopBuy;
				pack.Movable = false;
				pack.Visible = false;
				AddItem( pack );
			}

			from.Send( new EquipUpdate( pack ) );

			pack = FindItemOnLayer( Layer.ShopSell );

			if ( pack != null )
				from.Send( new EquipUpdate( pack ) );

			pack = FindItemOnLayer( Layer.ShopResale );

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Layer = Layer.ShopResale;
				pack.Movable = false;
				pack.Visible = false;
				AddItem( pack );
			}

			from.Send( new EquipUpdate( pack ) );
		}

		public virtual void VendorSell( Mobile from )
		{
			if ( !IsActiveBuyer )
				return;

			if ( !from.CheckAlive() )
				return;

			#region modifica by Dies per i Vendors YoungDealers
			if( !YoungDeal( this, from ) )
	    		return;
			#endregion
			
			#region modifica by Dies per i Vendor cittadini
            TownSystem t = TownSystem.Find( this );
            if( t != null && !t.VendorSellAllowed )
                return;

            if( t != null )
            {
                TownVendorState state = t.CommercialStatus.FindVendorState( this );
                if( state != null && !state.CheckAccess( true, true, this ) )
                    return;
            }

			if( !CitizenDeal( this, from, false, true ) )
				return;
			#endregion

			if ( !CheckVendorAccess( from ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return;
			}

			Container pack = from.Backpack;

			if ( pack != null )
			{
				IShopSellInfo[] info = GetSellInfo();

				Hashtable table = new Hashtable();

				foreach ( IShopSellInfo ssi in info )
				{
					Item[] items = pack.FindItemsByType( ssi.Types );

					foreach ( Item item in items )
					{
						if ( item is Container && ( (Container)item ).Items.Count != 0 )
							continue;

						if ( item.IsStandardLoot() && item.Movable && ssi.IsSellable( item ) )
							table[item] = new SellItemState( item, ssi.GetSellPriceFor( item ), ssi.GetNameFor( item ) );
					}
				}

				if ( table.Count > 0 )
				{
					SendPacksTo( from );

					from.Send( new VendorSellList( this, table ) );
				}
				else
				{
					Say( true, "You have nothing I would be interested in." );
				}
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is SmallBOD || dropped is LargeBOD || dropped is SmallMobileBOD || dropped is LargeMobileBOD ) // modifica by Berto
			{
				if ( !IsValidBulkOrder( dropped ) || !SupportsBulkOrders( from ) )
				{
					SayTo( from, 1045130 ); // That order is for some other shopkeeper.
					return false;
				}
				else if ( (dropped is SmallBOD && !((SmallBOD)dropped).Complete) || (dropped is LargeBOD && !((LargeBOD)dropped).Complete) || 
                          (dropped is SmallMobileBOD && !((SmallMobileBOD)dropped).Complete) || (dropped is LargeMobileBOD && !((LargeMobileBOD)dropped).Complete) ) // modifica by Berto
				{
					SayTo( from, 1045131 ); // You have not completed the order yet.
					return false;
				}

				Item reward;
				int gold, fame;

				if ( dropped is SmallBOD )
					((SmallBOD)dropped).GetRewards( out reward, out gold, out fame );
				else if ( dropped is LargeBOD )
					((LargeBOD)dropped).GetRewards( out reward, out gold, out fame );
				#region modifica by Berto
				else if ( dropped is LargeMobileBOD )
					((LargeMobileBOD)dropped).GetRewards( out reward, out gold, out fame );
				else
					((SmallMobileBOD)dropped).GetRewards( out reward, out gold, out fame );
				#endregion

				from.SendSound( 0x3D );

				SayTo( from, 1045132 ); // Thank you so much!  Here is a reward for your effort.

				if ( reward != null )
					from.AddToBackpack( reward );

				if ( gold > 1000 )
					from.AddToBackpack( new BankCheck( gold ) );
				else if ( gold > 0 )
					from.AddToBackpack( new Gold( gold ) );

				Titles.AwardFame( from, fame, true );

				#region mod by Dies Irae
				if( dropped is SmallBOD || dropped is LargeBOD )
					OnSuccessfulBulkOrderReceive( from, dropped );
				else
					OnSuccessfulBulkOrderReceive( from );
				#endregion

				dropped.Delete();
				return true;
			}

			return base.OnDragDrop( from, dropped );
		}

		private GenericBuyInfo LookupDisplayObject( object obj )
		{
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();

			for ( int i = 0; i < buyInfo.Length; ++i ) {
				GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[i];

				if ( gbi.GetDisplayEntity() == obj )
					return gbi;
			}

			return null;
		}

        private void ProcessSinglePurchase( BuyItemResponse buy, IBuyItemInfo bii, List<BuyItemResponse> validBuy, ref int controlSlots, ref bool fullPurchase, ref int totalCost )
		{
			int amount = buy.Amount;

			if ( amount > bii.Amount )
				amount = bii.Amount;

			if ( amount <= 0 )
				return;

			int slots = bii.ControlSlots * amount;

			if ( controlSlots >= slots )
			{
				controlSlots -= slots;
			}
			else
			{
				fullPurchase = false;
				return;
			}

			totalCost += bii.Price * amount;
			validBuy.Add( buy );
		}

		private void ProcessValidPurchase( int amount, IBuyItemInfo bii, Mobile buyer, Container cont )
		{
			if ( amount > bii.Amount )
				amount = bii.Amount;

			if ( amount < 1 )
				return;

			bii.Amount -= amount;

			IEntity o = bii.GetEntity();

			InitializeBuyEntity( o, buyer ); // mod by Faxx

            TownSystem system = TownSystem.Find( MidgardTown ); // mod by Dies

			if ( o is Item )
			{
				Item item = (Item)o;

				#region mod by Dies irae
                CommercialSystem.Core.RegisterPurchase( buyer, item.GetType(), amount, bii.Price, true );

                if( item is Midgard.Items.PersonalRecipeScroll )
                    ( (Midgard.Items.PersonalRecipeScroll)item ).Owner = buyer;

                if( system != null )
                    system.RegisterPurchase( this, item.GetType(), amount );
				#endregion

				if ( item.Stackable )
				{
					item.Amount = amount;

					if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
						item.MoveToWorld( buyer.Location, buyer.Map );
				}
				else
				{
					item.Amount = 1;

					if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
						item.MoveToWorld( buyer.Location, buyer.Map );

					for ( int i = 1; i < amount; i++ )
					{
						item = bii.GetEntity() as Item;

						if ( item != null )
						{
							item.Amount = 1;

							if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
								item.MoveToWorld( buyer.Location, buyer.Map );
						}
					}
				}
			}
			else if ( o is Mobile )
			{
				Mobile m = (Mobile)o;

                #region mod by Dies Irae
                if( system != null )
                    system.RegisterPurchase( this, m.GetType(), 1 );
				#endregion

				m.Direction = (Direction)Utility.Random( 8 );
				m.MoveToWorld( buyer.Location, buyer.Map );
				m.PlaySound( m.GetIdleSound() );

				if ( m is BaseCreature )
					( (BaseCreature)m ).SetControlMaster( buyer );

				for ( int i = 1; i < amount; ++i )
				{
					m = bii.GetEntity() as Mobile;

					if ( m != null )
					{
						m.Direction = (Direction)Utility.Random( 8 );
						m.MoveToWorld( buyer.Location, buyer.Map );

						if ( m is BaseCreature )
							( (BaseCreature)m ).SetControlMaster( buyer );
					}
				}
			}
		}

        public virtual bool OnBuyItems( Mobile buyer, List<BuyItemResponse> list )
		{
			if ( !IsActiveSeller )
				return false;

			if ( !buyer.CheckAlive() )
				return false;

			if ( !CheckVendorAccess( buyer ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return false;
			}

			UpdateBuyInfo( buyer ); // mod by Dies Irae

			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			IShopSellInfo[] info = GetSellInfo();
			int totalCost = 0;
            List<BuyItemResponse> validBuy = new List<BuyItemResponse>( list.Count );
			Container cont;
			bool bought = false;
			bool fromBank = false;
			bool fullPurchase = true;
			int controlSlots = buyer.FollowersMax - buyer.Followers;

			foreach ( BuyItemResponse buy in list )
			{
				Serial ser = buy.Serial;
				int amount = buy.Amount;

				if ( ser.IsItem )
				{
					Item item = World.FindItem( ser );

					if ( item == null )
						continue;

					GenericBuyInfo gbi = LookupDisplayObject( item );

					if ( gbi != null )
					{
						ProcessSinglePurchase( buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost );
					}
					else if ( item != this.BuyPack && item.IsChildOf( this.BuyPack ) )
					{
						if ( amount > item.Amount )
							amount = item.Amount;

						if ( amount <= 0 )
							continue;

						foreach ( IShopSellInfo ssi in info )
						{
							if ( ssi.IsSellable( item ) )
							{
								if ( ssi.IsResellable( item ) )
								{
									totalCost += ssi.GetBuyPriceFor( item ) * amount;
									validBuy.Add( buy );
									break;
								}
							}
						}
					}
				}
				else if ( ser.IsMobile )
				{
					Mobile mob = World.FindMobile( ser );

					if ( mob == null )
						continue;

					GenericBuyInfo gbi = LookupDisplayObject( mob );

					if ( gbi != null )
						ProcessSinglePurchase( buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost );
				}
			}//foreach

			if ( fullPurchase && validBuy.Count == 0 )
				SayTo( buyer, 500190 ); // Thou hast bought nothing!
			else if ( validBuy.Count == 0 )
				SayTo( buyer, 500187 ); // Your order cannot be fulfilled, please try again.

			if ( validBuy.Count == 0 )
				return false;

			bought = ( buyer.AccessLevel >= AccessLevel.GameMaster );

            #region mod by Dies Irae
            cont = buyer.Waistpack;
            if( !bought && cont != null )
            {
                if( cont.ConsumeTotal( typeof( Gold ), totalCost ) )
                    bought = true;
            }
            #endregion

			cont = buyer.Backpack;
			if ( !bought && cont != null )
			{
				if ( cont.ConsumeTotal( typeof( Gold ), totalCost ) )
					bought = true;
				else if ( totalCost < 2000 )
					SayTo( buyer, 500192 );//Begging thy pardon, but thou casnt afford that.
			}

			if ( !bought && totalCost >= 2000 )
			{
				cont = buyer.FindBankNoCreate();
				if ( cont != null && cont.ConsumeTotal( typeof( Gold ), totalCost ) )
				{
					bought = true;
					fromBank = true;
				}
				else
				{
					SayTo( buyer, 500191 ); //Begging thy pardon, but thy bank account lacks these funds.
				}
			}

			if ( !bought )
				return false;
			else
				buyer.PlaySound( 0x32 );

			cont = buyer.Backpack;
			if ( cont == null )
				cont = buyer.BankBox;

			foreach ( BuyItemResponse buy in validBuy )
			{
				Serial ser = buy.Serial;
				int amount = buy.Amount;

				if ( amount < 1 )
					continue;

				if ( ser.IsItem )
				{
					Item item = World.FindItem( ser );

					if ( item == null )
						continue;

					GenericBuyInfo gbi = LookupDisplayObject( item );

					if ( gbi != null )
					{
						ProcessValidPurchase( amount, gbi, buyer, cont );
					}
					else
					{
						if ( amount > item.Amount )
							amount = item.Amount;

						foreach ( IShopSellInfo ssi in info )
						{
							if ( ssi.IsSellable( item ) )
							{
								if ( ssi.IsResellable( item ) )
								{
									Item buyItem;
									if ( amount >= item.Amount )
									{
										buyItem = item;
									}
									else
									{
										buyItem = Mobile.LiftItemDupe( item, item.Amount - amount );

										if ( buyItem == null )
											buyItem = item;
									}

									#region mod by Dies irae
									CommercialSystem.Core.RegisterPurchase( buyer, item.GetType(), amount, gbi != null ? gbi.Price : -1, true );

                                    TownSystem system = TownSystem.Find( MidgardTown );

                                    if( system != null )
                                        system.RegisterPurchase( this, buyItem.GetType(), buyItem.Amount );
									#endregion

									if ( cont == null || !cont.TryDropItem( buyer, buyItem, false ) )
										buyItem.MoveToWorld( buyer.Location, buyer.Map );

									break;
								}
							}
						}
					}
				}
				else if ( ser.IsMobile )
				{
					Mobile mob = World.FindMobile( ser );

					if ( mob == null )
						continue;

					GenericBuyInfo gbi = LookupDisplayObject( mob );

					if ( gbi != null )
						ProcessValidPurchase( amount, gbi, buyer, cont );
				}
			}//foreach

			if ( fullPurchase )
			{
				if ( buyer.AccessLevel >= AccessLevel.GameMaster )
					SayTo( buyer, true, "I would not presume to charge thee anything.  Here are the goods you requested." );
				else if ( fromBank )
					SayTo( buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.", totalCost );
				else
					SayTo( buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.", totalCost );
			}
			else
			{
				if ( buyer.AccessLevel >= AccessLevel.GameMaster )
					SayTo( buyer, true, "I would not presume to charge thee anything.  Unfortunately, I could not sell you all the goods you requested." );
				else if ( fromBank )
					SayTo( buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost );
				else
					SayTo( buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost );
			}

			return true;
		}

		public virtual bool CheckVendorAccess( Mobile from )
		{
			GuardedRegion reg = (GuardedRegion)this.Region.GetRegion( typeof( GuardedRegion ) );

			if ( reg != null && !reg.CheckVendorAccess( this, from ) )
				return false;

			if ( this.Region != from.Region )
			{
				reg = (GuardedRegion)from.Region.GetRegion( typeof( GuardedRegion ) );

				if ( reg != null && !reg.CheckVendorAccess( this, from ) )
					return false;
			}

			return true;
		}

        public virtual bool OnSellItems( Mobile seller, List<SellItemResponse> list )
		{
			if ( !IsActiveBuyer )
				return false;

			if ( !seller.CheckAlive() )
				return false;

			if ( !CheckVendorAccess( seller ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return false;
			}

			seller.PlaySound( 0x32 );

			IShopSellInfo[] info = GetSellInfo();
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			int GiveGold = 0;
			int Sold = 0;
			Container cont;

			foreach ( SellItemResponse resp in list )
			{
				if ( resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || ( resp.Item is Container && ( (Container)resp.Item ).Items.Count != 0 ) )
					continue;

				foreach ( IShopSellInfo ssi in info )
				{
					if ( ssi.IsSellable( resp.Item ) )
					{
						Sold++;
						break;
					}
				}
			}

			if ( Sold > MaxSell )
			{
				SayTo( seller, true, "You may only sell {0} items at a time!", MaxSell );
				return false;
			}
			else if ( Sold == 0 )
			{
				return true;
			}

			foreach ( SellItemResponse resp in list )
			{
				if ( resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || ( resp.Item is Container && ( (Container)resp.Item ).Items.Count != 0 ) )
					continue;

				foreach ( IShopSellInfo ssi in info )
				{
					if ( ssi.IsSellable( resp.Item ) )
					{
						int amount = resp.Amount;

						if ( amount > resp.Item.Amount )
							amount = resp.Item.Amount;

						if ( ssi.IsResellable( resp.Item ) )
						{
							bool found = false;

							foreach ( IBuyItemInfo bii in buyInfo )
							{
								if ( bii.Restock( resp.Item, amount ) )
								{
									resp.Item.Consume( amount );
									found = true;

									break;
								}
							}

							if ( !found )
							{
								cont = this.BuyPack;

								if ( amount < resp.Item.Amount )
								{
									Item item = Mobile.LiftItemDupe( resp.Item, resp.Item.Amount - amount );

									if ( item != null )
									{
										item.SetLastMoved();
										cont.DropItem( item );
									}
									else
									{
										resp.Item.SetLastMoved();
										cont.DropItem( resp.Item );
									}
								}
								else
								{
									resp.Item.SetLastMoved();
									cont.DropItem( resp.Item );
								}
							}
						}
						else
						{
							if ( amount < resp.Item.Amount )
								resp.Item.Amount -= amount;
							else
								resp.Item.Delete();
						}

						GiveGold += ssi.GetSellPriceFor( resp.Item ) * amount;

                        CommercialSystem.Core.RegisterPurchase( seller, resp.Item.GetType(), amount, ssi.GetSellPriceFor( resp.Item ), false ); // mod by Dies Irae
						break;
					}
				}
			}

			if ( GiveGold > 0 )
			{
                #region mod by Dies Irae
                int sound;
                if( GiveGold < 10 )
                    sound = 53;
                else if( GiveGold < 100 )
                    sound = 54;
                else
                    sound = 55;
                #endregion

				while ( GiveGold > 60000 )
				{
					seller.AddToBackpack( new Gold( 60000 ) );
					GiveGold -= 60000;
				}

				seller.AddToBackpack( new Gold( GiveGold ) );

				seller.PlaySound( Core.AOS ? 0x0037 : sound ); //Gold dropping sound // mod by Dies Irae

				if ( SupportsBulkOrders( seller ) )
				{
					Item bulkOrder = CreateBulkOrder( seller, false );

					if ( bulkOrder is LargeBOD )
						seller.SendGump( new LargeBODAcceptGump( seller, (LargeBOD)bulkOrder ) );
					else if ( bulkOrder is SmallBOD )
						seller.SendGump( new SmallBODAcceptGump( seller, (SmallBOD)bulkOrder ) );
					#region modifica by Berto
					else if ( bulkOrder is SmallMobileBOD )
						seller.SendGump( new SmallMobileBODAcceptGump( seller, (SmallMobileBOD)bulkOrder ) );
					else if ( bulkOrder is LargeMobileBOD )
						seller.SendGump( new LargeMobileBODAcceptGump( seller, (LargeMobileBOD)bulkOrder ) );
					#endregion
				}
			}
			//no cliloc for this?
			SayTo( seller, true, "Thank you! I bought {0} item{1}. Here is your {2}gp.", Sold, (Sold > 1 ? "s" : ""), GiveGold ); // mod by Dies Irae

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

			#region modifica by Dies per i Vendors Cittadini
			writer.Write( (int) MidgardTown );
			#endregion

			#region modifica by Dies per i Vendors YoungDealers
			writer.Write( (bool) IsYoungDealer );
			#endregion

            List<SBInfo> sbInfos = this.SBInfos;

			for ( int i = 0; sbInfos != null && i < sbInfos.Count; ++i )
			{
				SBInfo sbInfo = sbInfos[i];
                List<GenericBuyInfo> buyInfo = sbInfo.BuyInfo;

				for ( int j = 0; buyInfo != null && j < buyInfo.Count; ++j )
				{
					GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[j];

					int maxAmount = gbi.MaxAmount;
					int doubled = 0;

					switch ( maxAmount )
					{
						case  40: doubled = 1; break;
						case  80: doubled = 2; break;
						case 160: doubled = 3; break;
						case 320: doubled = 4; break;
						case 640: doubled = 5; break;
						case 999: doubled = 6; break;
					}

					if ( doubled > 0 )
					{
						writer.WriteEncodedInt( 1 + ( ( j * sbInfos.Count ) + i ) );
						writer.WriteEncodedInt( doubled );
					}
				}
			}

			writer.WriteEncodedInt( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			LoadSBInfo();

			List<SBInfo> sbInfos = this.SBInfos;

			switch ( version )
			{
				#region modifica by Dies per i Vendors Cittadini
				case 3:
					MidgardTown = (MidgardTowns)reader.ReadInt();
					goto case 2;
				#endregion
				
				#region modifica by Dies per i Vendors YoungDealers
				case 2:
					IsYoungDealer = reader.ReadBool();
					goto case 1;
				#endregion
				case 1:
					{
						int index;

						while ( ( index = reader.ReadEncodedInt() ) > 0 )
						{
							int doubled = reader.ReadEncodedInt();

							if ( sbInfos != null )
							{
								index -= 1;
								int sbInfoIndex = index % sbInfos.Count;
								int buyInfoIndex = index / sbInfos.Count;

								if ( sbInfoIndex >= 0 && sbInfoIndex < sbInfos.Count )
								{
									SBInfo sbInfo = sbInfos[sbInfoIndex];
                                    List<GenericBuyInfo> buyInfo = sbInfo.BuyInfo;

									if ( buyInfo != null && buyInfoIndex >= 0 && buyInfoIndex < buyInfo.Count )
									{
										GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[buyInfoIndex];

										int amount = 20;

										switch ( doubled )
										{
											case 1: amount = 40; break;
											case 2: amount = 80; break;
											case 3: amount = 160; break;
											case 4: amount = 320; break;
											case 5: amount = 640; break;
											case 6: amount = 999; break;
										}

										gbi.Amount = gbi.MaxAmount = amount;
									}
								}
							}
						}

						break;
					}
			}

			if ( IsParagon )
				IsParagon = false;

			if ( Core.AOS && NameHue == 0x35 )
				NameHue = -1;

			// Timer.DelayCall( TimeSpan.Zero, new TimerCallback( CheckMorph ) );
		}

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive && IsActiveVendor )
			{
				if ( IsActiveSeller )
					list.Add( new VendorBuyEntry( from, this ) );

				if ( IsActiveBuyer )
					list.Add( new VendorSellEntry( from, this ) );

				if ( SupportsBulkOrders( from ) )
					list.Add( new BulkOrderInfoEntry( from, this ) );
			}

			base.AddCustomContextEntries( from, list );
		}

		public virtual IShopSellInfo[] GetSellInfo()
		{
			return (IShopSellInfo[])m_ArmorSellInfo.ToArray( typeof( IShopSellInfo ) );
		}

		public virtual IBuyItemInfo[] GetBuyInfo()
		{
			return (IBuyItemInfo[])m_ArmorBuyInfo.ToArray( typeof( IBuyItemInfo ) );
		}

		public override bool CanBeDamaged()
		{
			return !IsInvulnerable;
		}

        #region Quests by Dies Irae
        public override bool GuardImmune { get { return true; } }

        public virtual int AutoTalkRange { get { return -1; } }
        public virtual int AutoSpeakRange { get { return 10; } }
        public virtual TimeSpan SpeakDelay { get { return TimeSpan.FromMinutes( 1 ); } }

        public virtual Type[] Quests { get { return null; } }

        private DateTime m_Spoken;

        public virtual void OnTalk( PlayerMobile player )
        {
            if( QuestHelper.DeliveryArrived( player, this ) )
                return;

            if( QuestHelper.InProgress( player, this ) )
                return;

            if( QuestHelper.QuestLimitReached( player ) )
                return;

            // check if this quester can offer any quest chain (already started)
            foreach( KeyValuePair<QuestChain, BaseChain> pair in player.Chains )
            {
                BaseChain chain = pair.Value;

                if( chain != null && chain.Quester != null && chain.Quester == GetType() )
                {
                    BaseQuest quest = QuestHelper.RandomQuest( player, new Type[] { chain.CurrentQuest }, this );

                    if( quest != null )
                    {
                        player.CloseGump( typeof( MondainQuestGump ) );
                        player.SendGump( new MondainQuestGump( quest ) );
                        return;
                    }
                }
            }

            BaseQuest questt = QuestHelper.RandomQuest( player, Quests, this );

            if( questt != null )
            {
                player.CloseGump( typeof( MondainQuestGump ) );
                player.SendGump( new MondainQuestGump( questt ) );
            }
        }

        public virtual void OnOfferFailed()
        {
            Say( 1075575 ); // I'm sorry, but I don't have anything else for you right now. Could you check back with me in a few minutes?
        }

        public virtual void Advertise()
        {
            Say( Utility.RandomMinMax( 1074183, 1074223 ) );
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            if( Quests == null || Quests.Length == 0 || !CanOfferQuestTo( m ) )
                return;

            if( m.Alive && !m.Hidden && m is PlayerMobile )
            {
                PlayerMobile pm = (PlayerMobile)m;

                int range = AutoTalkRange;

                if( range >= 0 && InRange( m, range ) && !InRange( oldLocation, range ) )
                    OnTalk( pm );

                range = AutoSpeakRange;

                if( range >= 0 && InRange( m, range ) && !InRange( oldLocation, range ) && DateTime.Now >= m_Spoken + SpeakDelay )
                {
                    if( Utility.Random( 100 ) < 50 )
                        Advertise();

                    m_Spoken = DateTime.Now;
                }
            }
        }

        public virtual bool CanOfferQuestTo( Mobile m )
        {
            return TownSystem.Find( this ) == TownSystem.Find( m );
        }

        public override void OnDoubleClick( Mobile m )
        {
            if( m.Alive && m is PlayerMobile && CanOfferQuestTo( m ) )
                OnTalk( (PlayerMobile)m );

            base.OnDoubleClick( m );
        }

        public void FocusTo( Mobile to )
        {
            QuestSystem.FocusTo( this, to );
        }
        #endregion

        #region mod by Dies Irae
        private static bool PayToOlder = false;

        public static bool YoungDeal( BaseVendor dealer, Mobile from ) // modifica by Dies per i Vendors YoungDealers
        {
            if( dealer == null )
                return false;

            if( !Core.AOS )
                return true;

            int AccessCost = 2000;

            if( dealer.IsYoungDealer )
            {
                PlayerMobile player = from as PlayerMobile;

                if( player != null && !player.Young )
                {
                    if( ( (Midgard2PlayerMobile)player ).QuestDeltaTimeExpiration > TimeSpan.Zero )
                    {
                        dealer.Say( "For other {0} hours you will be able to access the city services.",
                                    ( (Midgard2PlayerMobile)player ).QuestDeltaTimeExpiration.TotalHours.ToString( "F0" ) );
                        return true;
                    }
                    else if( PayToOlder )
                    {
                        if( Banker.GetBalance( player ) < AccessCost )
                        {
                            // Se il Player non e' Young E non puo' permettersi i servigi allora NON procedere
                            dealer.Say( "I'm sorry. You lack funds to access my services." );
                            return false;
                        }
                        else
                        {
                            // Se il Player non e' Young ma puo' permettersi i servigi allora procedi
                            Banker.Withdraw( player, AccessCost );
                            dealer.Say( "I've taken " + AccessCost + " gold from your bank box" );
                            dealer.Say( "You have " + Banker.GetBalance( player ) + " gold in tou bank account." );
                            return true;
                        }
                    }
                    else
                    {
                        dealer.Say( "Your status grants you access to my services." );
                        return false;
                    }
                }
                else
                {
                    // Se il Player e' Young allora procedi
                    dealer.Say( "Your status grants you access to my services for free." );
                    return true;
                }
            }
            else
            {
                // Se il vendor non e' un YoungDealer allora procedi
                return true;
            }
        }

        public void UpdateBuyInfo()
        {
            UpdateBuyInfo( null );
        }

        private bool m_NoDeltaRecursion;

        public void ValidateEquipment()
        {
            if( m_NoDeltaRecursion || Map == null || Map == Map.Internal )
                return;

            if( Items == null )
                return;

            m_NoDeltaRecursion = true;

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ValidateEquipmentSandbox ) );
        }

        private void ValidateEquipmentSandbox()
        {
            try
            {
                if( Map == null || Map == Map.Internal )
                    return;

                List<Item> items = Items;
                if( items == null )
                    return;

                for( int i = items.Count - 1; i >= 0; --i )
                {
                    if( i >= items.Count )
                        continue;

                    Item item = items[ i ];
                    if( !CheckEquip( item ) )
                        AddToBackpack( item );

                    Console.WriteLine( "Warning: item {0} dropped to pack for {1} instance.", item.GetType().Name, GetType().Name );
                }
            }
            catch( Exception e )
            {
                Console.WriteLine( e );
            }
            finally
            {
                m_NoDeltaRecursion = false;
            }
        }

        public virtual void TurnToEvilVendor()
        {
            NameHue = Notoriety.Hues[ Notoriety.Murderer ];
        }

        public virtual void VendorSellBag( Mobile from )
        {
            if( !IsActiveBuyer )
                return;

            if( !from.CheckAlive() )
                return;

            if( !YoungDeal( this, from ) )
                return;

            if( !CitizenDeal( this, from, false, true ) )
                return;

            TownSystem t = TownSystem.Find( this );
            if( t != null )
            {
                TownVendorState state = t.CommercialStatus.FindVendorState( this );
                if( state != null && !state.CheckAccess( true, true, this ) )
                    return;
            }

            if( !CheckVendorAccess( from ) )
            {
                Say( 501522 ); // I shall not treat with scum like thee!
                return;
            }

            Say( true, "Choose a bag in your pack you wish to sell." );
            from.Target = new SellBagTarget( from, this );
        }

        private class SellBagTarget : Target
        {
            private readonly Mobile m_From;
            private readonly BaseVendor m_Vendor;

            public SellBagTarget( Mobile from, BaseVendor vendor )
                : base( -1, false, TargetFlags.None )
            {
                m_From = from;
                m_Vendor = vendor;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_From.Deleted || m_Vendor == null || m_Vendor.Deleted )
                    return;

                if( m_From.InRange( m_Vendor, 1 ) )
                {
                    BaseContainer bag = targeted as BaseContainer;

                    if( bag != null && !bag.Deleted )
                    {
                        if( bag.IsChildOf( from.Backpack ) )
                        {
                            IShopSellInfo[] info = m_Vendor.GetSellInfo();

                            List<SellItemResponse> sellList = new List<SellItemResponse>();

                            foreach( IShopSellInfo ssi in info )
                            {
                                Item[] items = bag.FindItemsByType( ssi.Types );

                                foreach( Item item in items )
                                {
                                    if( item is Container && item.Items.Count != 0 )
                                        continue;

                                    if( item.IsStandardLoot() && item.Movable && ssi.IsSellable( item ) )
                                    {
                                        sellList.Add( new SellItemResponse( item, item.Amount ) );
                                    }
                                }
                            }

                            if( sellList.Count > 0 )
                            {
                                m_Vendor.OnSellItems( m_From, sellList );
                            }
                            else
                            {
                                m_Vendor.Say( true, "You have nothing I would be interested in." );
                            }
                        }
                        else
                        {
                            m_Vendor.Say( true, "The bag you would to sell must be in your backpack." );
                        }
                    }
                    else
                    {
                        m_Vendor.Say( true, "That is not a valid container to sell." );
                    }
                }
                else
                {
                    m_From.SendMessage( "You are too far away from the vendor." );
                }
            }
        }

        public virtual void VendorSellAll( Mobile from )
        {
            if( !IsActiveBuyer )
                return;

            if( !from.CheckAlive() )
                return;

            if( !YoungDeal( this, from ) )
                return;

            if( !CitizenDeal( this, from, false, true ) )
                return;

            TownSystem t = TownSystem.Find( this );
            if( t != null )
            {
                TownVendorState state = t.CommercialStatus.FindVendorState( this );
                if( state != null && !state.CheckAccess( true, true, this ) )
                    return;
            }

            if( !CheckVendorAccess( from ) )
            {
                Say( 501522 ); // I shall not treat with scum like thee!
                return;
            }

            Say( true, "Choose an object in your pack you wish to sell all of that type." );
            from.Target = new SellAllTarget( from, this );
        }

        private class SellAllTarget : Target
        {
            private Mobile m_From;
            private BaseVendor m_Vendor;

            public SellAllTarget( Mobile from, BaseVendor vendor )
                : base( -1, false, TargetFlags.None )
            {
                m_From = from;
                m_Vendor = vendor;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_From == null || m_From.Deleted || m_Vendor == null || m_Vendor.Deleted )
                    return;

                if( m_From.InRange( m_Vendor, 1 ) )
                {
                    Container pack = m_From.Backpack;
                    if( pack == null || pack.Deleted )
                        return;

                    Item protoptype = targeted as Item;
                    if( protoptype == null )
                        return;

                    if( protoptype.IsChildOf( pack ) && protoptype.Parent == pack )
                    {
                        Item[] items = pack.FindItemsByType( protoptype.GetType(), false );

                        if( items.Length > 0 )
                        {
                            List<SellItemResponse> sellList = new List<SellItemResponse>();

                            foreach( IShopSellInfo ssi in m_Vendor.GetSellInfo() )
                            {
                                foreach( Item item in items )
                                {
                                    if( item is Container && item.Items.Count != 0 )
                                        continue;

                                    if( item.IsStandardLoot() && item.Movable && ssi.IsSellable( item ) )
                                    {
                                        sellList.Add( new SellItemResponse( item, item.Amount ) );
                                    }
                                }
                            }

                            if( sellList.Count > 0 )
                            {
                                m_Vendor.OnSellItems( m_From, sellList );
                            }
                            else
                            {
                                m_Vendor.Say( true, "You have nothing I would be interested in." );
                            }
                        }
                        else
                        {
                            m_Vendor.Say( true, "I could not find any valid item in your backpac. Sorry." );
                        }
                    }
                    else
                    {
                        m_Vendor.Say( true, "Choosen item you would to sell must be in your backpack (and not in any container)." );
                    }
                }
                else
                {
                    m_From.SendMessage( "You are too far away from the vendor." );
                }
            }
        }

        public static bool CitizenDeal( BaseVendor dealer, Mobile from, bool message )
        {
            return CitizenDeal( dealer, from, message, false );
        }

        public static bool CitizenDeal( BaseVendor dealer, Mobile from, bool message, bool dropAccessCost )
        {
            if( dealer == null )
                return false;

            if( from.AccessLevel > AccessLevel.Player )
                return true;

            if( !Core.AOS )
                return true;

            TownSystem system = TownSystem.Find( dealer );

            return system != null ? system.CanAccessTownServices( dealer, from, message, dropAccessCost ) : true;
        }

        public static bool CitizenDeal( BaseVendor dealer, Mobile from )
        {
            return CitizenDeal( dealer, from, true );
        }

        public virtual void OnSuccessfulBulkOrderReceive( Mobile from, Item dropped ) // mod by Dies Irae
		{
		}

        public virtual int GetKarmaScalar( Mobile from )
        {
            /* 
             * implementata una funzione lineare spezzata.
             * tra -2000 e 2000 vale 0
             * tra -10000 e -2000 passa da 125 a 100
             * tra 2000 e 10000 passa da 100 a 75
             * altrimenti vale 100
             */

            bool isBadGuy = from.Karma < 0;
            int absKarma = Math.Abs( from.Karma );
            int karmaScalar = 100;

            if( absKarma <= 2000 )
                return karmaScalar;
            else if( absKarma >= 10000 )
                karmaScalar = isBadGuy ? ( 100 + MaxKarmaScalar ) : ( 100 - MaxKarmaScalar );
            else
            {
                int delta = (int)( ( ( absKarma - KarmaLevel ) / (double)( 10000 - KarmaLevel ) ) * MaxKarmaScalar );
                karmaScalar = isBadGuy ? ( 100 + delta ) : ( 100 - delta );
            }

            from.DebugMessage( "Your karma scalar is {0}", karmaScalar );

            return karmaScalar;
        }

        public static int MaxKarmaScalar = 25;
        public static int KarmaLevel = 2000;

        public static int MaxPositiveKarmaScalar = 75;
        public static int MaxNegativeKarmaScalar = 125;

        public static int KarmaPositiveLevel = 2000;
        public static int KarmaNegativeLevel = -2000;

        public virtual int GetRegionalScalar()
        {
            /*
            if( TownSystem.Find( this ) == null )
                return 200;
            */

            return 100;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsYoungDealer { get; set; }

        [CommandProperty( AccessLevel.Administrator )]
        public MidgardTowns MidgardTown { get; set; }

        public virtual bool CheckMidgardTown() // modifica by Dies Irae
        {
            if( Map != Map.Felucca || Map == Map.Internal || Map == null )
                return false;

            TownSystem system = TownSystem.Find( Location, Map );
            if( system != null )
            {
                MidgardTown = system.Definition.Town; // Set town to our vendor
                // CantWalk = true;

                system.DressTownVendor( this );
                system.CommercialStatus.RegisterTownVendor( this );

                if( system.IsMurdererTown )
                    TurnToEvilVendor();
            }

            return ( system != null );
        }

        public virtual void InitializeBuyEntity( IEntity o, Mobile buyer ) { } // Modifica by Fax: Utilizzabile per modificare l'oggetto venduto appena prima di darlo al PG
        #endregion

	    private static Dictionary<Type, List<GenericBuyInfo>> ToFixList;

        private void LogInfo()
        {
            if( ToFixList == null )
                ToFixList = new Dictionary<Type, List<GenericBuyInfo>>();

            if( ToFixList.ContainsKey( GetType() ) )
                return;

            IBuyItemInfo[] buyInfo = GetBuyInfo(); // what vendor sell
            IShopSellInfo[] sellInfo = GetSellInfo(); // what vendor buy
            Type vendorType = GetType();

            foreach( IBuyItemInfo info in buyInfo )
            {
                if( !( info is GenericBuyInfo ) )
                    continue;

                GenericBuyInfo genInfo = (GenericBuyInfo)info;

                foreach( IShopSellInfo shopSellInfo in sellInfo )
                {
                    if( !( shopSellInfo is GenericSellInfo ) )
                        continue;

                    GenericSellInfo genSellIfo = (GenericSellInfo)shopSellInfo;
                    foreach( Type type in genSellIfo.Types )
                    {
                        if( type != genInfo.Type )
                            continue;

                        int sellPrice; // vendor sell price for an item
                        genSellIfo.GetSellPriceFor( type, out sellPrice );

                        if( CommercialSystem.Config.BuyInfoScalarnabled && genSellIfo.Demultiplicator > 0.0 )
                            sellPrice = (int)( sellPrice * genSellIfo.Demultiplicator );

                        if( sellPrice < 1 )
                            sellPrice = 1;

                        if( sellPrice > genInfo.OriginalPrice )
                        {
                            if( !ToFixList.ContainsKey( vendorType ) )
                            {
                                List<GenericBuyInfo> list = new List<GenericBuyInfo>();
                                list.Add( genInfo );
                                ToFixList[ vendorType ] = list;
                            }
                            else
                            {
                                List<GenericBuyInfo> list = ToFixList[ vendorType ];

                                bool contains = false;
                                foreach( GenericBuyInfo toAdd in list )
                                {
                                    if( toAdd.Type == type )
                                        contains = true;
                                }

                                if( !contains )
                                    list.Add( genInfo );
                            }

                            Console.WriteLine( "Warning: inconsistent sbInfo. Vendor: {0} Type {1}. Old price: {2} New price: {3}", GetType().Name, type.Name, genInfo.OriginalPrice, sellPrice * 1.90 );
                            genInfo.Price = (int)( sellPrice * 1.90 );
                        }
                    }
                }
            }
        }
#if false
        public static void Initialize()
        {
            LogConsistency();
        }
#endif

        /*
        private static void LogConsistency()
        {
            foreach( KeyValuePair<Type, List<GenericBuyInfo>> keyValuePair in ToFixList )
            {
                using( StreamWriter op = new StreamWriter( "sell-buy-errors.log", true ) )
                {
                    op.WriteLine( keyValuePair.Key );

                    foreach( GenericBuyInfo g in keyValuePair.Value )
                    {
                        string message = "Add( new GenericBuyInfo( typeof( {0} ), {1}, {2}, 0x{3:X3}, 0 ) );";
                        op.WriteLine( string.Format( message, g.Type.Name, g.Price, g.Amount, g.ItemID ) );
                    }
                }
            }
        }
        */
	}
}

namespace Server.ContextMenus
{
	public class VendorBuyEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;

		public VendorBuyEntry( Mobile from, BaseVendor vendor )
			: base( 6103, 8 )
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.VendorBuy( this.Owner.From );
		}
	}

	public class VendorSellEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;

		public VendorSellEntry( Mobile from, BaseVendor vendor )
			: base( 6104, 8 )
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.VendorSell( this.Owner.From );
		}
	}
}

namespace Server
{
	public interface IShopSellInfo
	{
		//get display name for an item
		string GetNameFor( Item item );

		//get price for an item which the player is selling
		int GetSellPriceFor( Item item );

		//get price for an item which the player is buying
		int GetBuyPriceFor( Item item );

		//can we sell this item to this vendor?
		bool IsSellable( Item item );

		//What do we sell?
		Type[] Types { get; }

		//does the vendor resell this item?
		bool IsResellable( Item item );
	}

	public interface IBuyItemInfo
	{
		//get a new instance of an object (we just bought it)
		IEntity GetEntity();

		int ControlSlots { get; }

        int KarmaScalar{ get; set; } // mod by Dies Irae

        int TownPrice{ get; set; } // mod by Dies Irae

        bool IsTownBuyInfo{ get; set; } // mod by Dies Irae

		int PriceScalar { get; set; }

        bool IsFromSameGuild{ get; set; } // mod by Dies Irae

		//display price of the item
		int Price { get; }

		//display name of the item
		string Name { get; }

		//display hue
		int Hue { get; }

		//display id
		int ItemID { get; }

		//amount in stock
		int Amount { get; set; }

		//max amount in stock
		int MaxAmount { get; }

		//Attempt to restock with item, (return true if restock sucessful)
		bool Restock( Item item, int amount );

		//called when its time for the whole shop to restock
		void OnRestock();
	}
}