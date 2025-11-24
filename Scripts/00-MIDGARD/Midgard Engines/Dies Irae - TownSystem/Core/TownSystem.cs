/***************************************************************************
 *                                  TownSystem.cs
 *                            		-------------
 *  begin                	: Gennaio, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Classe base per il TownSystem di Midgard.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Midgard.Engines.TownHouses;
using Midgard.Items;

using Server;
using Server.Accounting;
using Server.Guilds;
using Server.Items;
using Server.Menus.Questions;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;
using Server.Targeting;
using Server.ContextMenus;
using Midgard.ContextMenus;

namespace Midgard.Engines.MidgardTownSystem
{
    [CustomEnum( new string[] { "SerpentsHold", "Britain", "Trinsic", "BuccaneersDen","Cove","Jhelom",
        "Minoc","Ocllo","Vesper","Yew","Wind","SkaraBrae","Nujelm","Moonglow","Magincia","Delucia","Papua",
        "Marble","Aserark","Daekerthane","DalBaraz","Ahnor",/*"Umbra","Luna",*/"Sshamath","Justiceburg",
        "CalenSul","Vinyamar","Wolfsbane","Darklore","Naglund" } )]
    public abstract class TownSystem : ITownCrierEntryList, IComparable
    {
        public static readonly bool Enabled = Config.Enabled;

        public static readonly bool Debug = Config.Debug;

        public static bool TownAccessEnabled { get; set; }

        public virtual bool AcceptCitizens
        {
            get { return m_AcceptCitizens; }
            set { m_AcceptCitizens = value; }
        }

        private bool m_AcceptCitizens;

        public static void ConfigSystem()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( Load );
            EventSink.WorldSave += new WorldSaveEventHandler( Save );
        }

        #region static instances
        public static readonly TownSystem SerpentsHold = new SerpentsHold();
        public static readonly TownSystem Britain = new Britain();
        public static readonly TownSystem Trinsic = new Trinsic();
        public static readonly TownSystem BuccaneersDen = new BuccaneersDen();

        public static readonly TownSystem Cove = new Cove();
        public static readonly TownSystem Jhelom = new Jhelom();
        public static readonly TownSystem Minoc = new Minoc();
        public static readonly TownSystem Ocllo = new Ocllo();
        public static readonly TownSystem Vesper = new Vesper();
        public static readonly TownSystem Yew = new Yew();
        public static readonly TownSystem Wind = new Wind();
        public static readonly TownSystem SkaraBrae = new SkaraBrae();
        public static readonly TownSystem Nujelm = new Nujelm();
        public static readonly TownSystem Moonglow = new Moonglow();
        public static readonly TownSystem Magincia = new Magincia();
        public static readonly TownSystem Delucia = new Delucia();
        public static readonly TownSystem Papua = new Papua();
        public static readonly TownSystem Marble = new Marble();
        public static readonly TownSystem Aserark = new Aserark();
        public static readonly TownSystem Daekerthane = new Daekerthane();
        public static readonly TownSystem DalBaraz = new DalBaraz();
        public static readonly TownSystem Ahnor = new Ahnor();
        /*
        public static readonly TownSystem Umbra = new Umbra();
        public static readonly TownSystem Luna = new Luna();
        */
        public static readonly TownSystem Sshamath = new Sshamath();
        public static readonly TownSystem Justiceburg = new Justiceburg();
        public static readonly TownSystem CalenSul = new CalenSul();
        public static readonly TownSystem Vinyamar = new Vinyamar();
        public static readonly TownSystem Wolfsbane = new Wolfsbane();
        public static readonly TownSystem Darklore = new Darklore();
        public static readonly TownSystem Naglund = new Naglund();

        public static readonly TownSystem[] TownSystems = new TownSystem[]
		{
			SerpentsHold,
			Britain,
			Trinsic,
			BuccaneersDen,

    	    Cove,
		    Jhelom,
		    Minoc,
		    Ocllo,

		    Vesper,
		    Yew,
		    Wind,
		    SkaraBrae,

		    Nujelm,
		    Moonglow,
		    Magincia,
		    Delucia,

		    Papua,
		    Marble,
		    Aserark,
		    Daekerthane,

		    DalBaraz,
		    Ahnor,
		    /*
            Umbra,
		    Luna,
             */
		    Sshamath,
		    Justiceburg,

		    CalenSul,
		    Vinyamar,
            Wolfsbane,
            Darklore,
        
            Naglund
		};
        #endregion

        #region system town definition
        protected TownDefinition m_Definition;

        public TownDefinition Definition
        {
            get { return m_Definition; }
        }

        public static string DefaultWelcomeMessage
        {
            get
            {
                return
                "Benvenuto tra le nostre mura.<br>" +
                "Hai chiesto di divenire cittadino.<br>" +
                "Ricordati di osservare la legge, se desideri salvaguardare i tuoi affari.<br>" +
                "Per confermare la tua scelta procedi premendo il tasto poco piu' sotto.<br>";
            }
        }

        public string WelcomeMessage
        {
            get { return m_Definition.WelcomeMessage ?? DefaultWelcomeMessage; }
        }
        #endregion

        #region player states
        public TownPlayerStateCollection Players { get; protected set; }

        public void ClearInactiveStates()
        {
            List<TownPlayerState> list = GetInactiveStates( false );

            for( int i = 0; i < list.Count; i++ )
            {
                TownPlayerState state = list[ i ];

                if( state != null && state.IsInactive )
                {
                    Account a = state.Mobile.Account as Account;

                    if( a != null )
                        TownHelper.DoAccountReset( a );
                }
            }
        }

        public static int GetTotalCitizensInAllSystems()
        {
            int counter = 0;
            foreach( TownSystem t in TownSystems )
            {
                if( t != null && t.Players != null )
                    counter += t.Players.Count;
            }

            return counter;
        }

        public List<TownPlayerState> GetInactiveStates( bool sort )
        {
            List<TownPlayerState> list = new List<TownPlayerState>();

            if( Players != null )
            {
                foreach( TownPlayerState state in Players )
                {
                    if( state.IsInactive )
                        list.Add( state );
                }
            }

            if( sort )
                list.Sort();

            return list;
        }

        public int GetInactiveStatesCount()
        {
            int counter = 0;
            foreach( TownPlayerState state in Players )
            {
                if( state.IsInactive )
                    counter++;
            }

            return counter;
        }

        public List<Mobile> GetMobilesFromStates()
        {
            List<Mobile> list = new List<Mobile>();

            foreach( TownPlayerState tps in Players )
            {
                if( tps.Mobile != null )
                    list.Add( tps.Mobile );
            }

            return list;
        }

        public static List<TownPlayerState> GetAllInactiveStates( bool sort )
        {
            List<TownPlayerState> list = new List<TownPlayerState>();

            foreach( TownSystem t in TownSystems )
            {
                list.AddRange( t.GetInactiveStates( false ) );
            }

            if( sort )
                list.Sort();

            return list;
        }

        public static List<TownPlayerState> GetPlayerStatesOnServer( bool sort )
        {
            List<TownPlayerState> list = new List<TownPlayerState>();

            foreach( TownSystem t in TownSystems )
            {
                if( t.Players != null )
                    list.AddRange( t.Players );
            }

            if( sort )
                list.Sort();

            return list;
        }

        private void VerifyPlayerStates()
        {
            if( Debug )
                Config.Pkg.LogInfo( "Verifying {0} playerStates...", Definition.TownName );

            /* no longer required
            foreach( TownPlayerState state in Players )
            {
                if( state != null && state.Mobile != null )
                {
                    Mobile m = state.Mobile;
                    TownSystem sys = TownHelper.FindTownSystemFromAccountTag( m );

                    if( sys != null )
                    {
                        if( sys != this )
                        {
                            Config.Pkg.LogWarningLine(
                                "Warning: player {0} (account {1}) has town setted to {2} with account set to {3}",
                                m.Name, m.Account.Username, Definition.TownName, sys.Definition.TownName );
                        }
                    }
                    else
                    {
                        Account a = m.Account as Account;

                        if( a != null && ( a.GetTag( "Cittadinanza" ) == null ) )
                        {
                            Config.Pkg.LogWarningLine( "Warning: null town flag tag for account {0}", a.Username );
                            a.AddTag( "Cittadinanza", Enum.GetName( typeof( MidgardTowns ), Definition.Town ) );
                        }
                    }
                }
            }
            */

            if( Debug )
                Config.Pkg.LogInfo( "done\n" );
        }
        #endregion

        #region town bans
        public List<Mobile> ExiledPlayers { get; protected set; }
        public List<Mobile> PermaExiledPlayers { get; protected set; }

        public void RegisterBan( Mobile m )
        {
            if( ExiledPlayers == null )
                ExiledPlayers = new List<Mobile>();

            if( m != null && !ExiledPlayers.Contains( m ) )
            {
                ExiledPlayers.Add( m );
                TownLog.Log( LogType.Bans, String.Format( "Ban Registered. System: {0} - Name: {1} - Account {2} - DateTime {3}",
                    Definition.TownName, m.Name, m.Account.Username, DateTime.Now ) );
            }
        }

        public void UnRegisterBan( Mobile m )
        {
            if( m != null && ExiledPlayers != null && ExiledPlayers.Contains( m ) )
            {
                ExiledPlayers.Remove( m );
            }
        }

        public void RegisterPermaBan( Mobile m )
        {
            if( PermaExiledPlayers == null )
                PermaExiledPlayers = new List<Mobile>();

            if( m != null && !PermaExiledPlayers.Contains( m ) )
            {
                PermaExiledPlayers.Add( m );
                TownLog.Log( LogType.Bans, String.Format( "PermaBan Registered. System: {0} - Name: {1} - Account {2} - DateTime {3}",
                    Definition.TownName, m.Name, m.Account.Username, DateTime.Now ) );
            }
        }

        public void UnRegisterPermaBan( Mobile m )
        {
            if( m != null && PermaExiledPlayers != null && PermaExiledPlayers.Contains( m ) )
            {
                PermaExiledPlayers.Remove( m );
            }
        }
        #endregion

        #region housing
        public virtual bool IsUnderNoHousingCriteria( Point3D point )
        {
            return false;
        }

        public List<TownHouseSign> SystemTownHouses { get; protected set; }

        public static void CheckTownHouse( TownHouseSign sign )
        {
            if( Debug )
                Config.Pkg.LogInfo( "Registering TownHouse." );

            if( sign is TownFieldSign ) // i campi sono registrati separatamente
                return;

            if( sign.System != null )
                sign.System.RegisterTownHouse( sign );

            if( Debug )
                Config.Pkg.LogInfoLine( "done\n" );
        }

        public void RegisterTownHouse( TownHouseSign houseSign )
        {
            if( SystemTownHouses == null )
                SystemTownHouses = new List<TownHouseSign>();

            if( houseSign != null && !SystemTownHouses.Contains( houseSign ) )
            {
                SystemTownHouses.Add( houseSign );

                TownLog.Log( LogType.Houses, String.Format( "TowhHouse Registered. System: {0} - Name: {1} - Contract Location: {2} - DateTime {3}",
                    Definition.TownName, houseSign.Name, houseSign.Location, DateTime.Now ) );
            }
        }

        public void UnRegisterTownHouse( TownHouseSign houseSign )
        {
            if( houseSign != null && SystemTownHouses != null && SystemTownHouses.Contains( houseSign ) )
            {
                SystemTownHouses.Remove( houseSign );

                TownLog.Log( LogType.Houses, String.Format( "TowhHouse UnRegistered. System: {0} - Name: {1} - Contract Location: {2}  - DateTime {3}",
                     Definition.TownName, houseSign.Name, houseSign.Location, DateTime.Now ) );
            }
            else
                TownLog.Log( LogType.Houses, String.Format( "Warning: trying to unregister field without any field list." ) );

        }

        public static int MaxHousesPerAccount = 2;

        public static bool CanBuyTownHouse( Mobile m, TownHouseSign sign )
        {
            // We have already 2 houses
            if( BaseHouse.HasAccountHouses( m ) )
                return false;

            TownSystem system = Find( m );

            if( system != null )
            {
                if( sign != null && system.SystemTownHouses != null )
                {
                    if( system.SystemTownHouses.Contains( sign ) )
                    {
                        List<TownHouseSign> houses = GetAccountTownHouseSigns( m );

                        return houses != null && houses.Count < MaxHousesPerAccount;
                    }
                }
            }

            return false;
        }

        public static List<TownHouseSign> GetTownHouse( Mobile m )
        {
            if( m == null )
                return null;

            List<TownHouseSign> list = new List<TownHouseSign>();

            foreach( TownHouseSign ths in TownHouseSign.AllSigns )
            {
                if( ths != null && ths.House != null && ths.House.Owner != null && ths.House.Owner == m )
                    list.Add( ths );
            }

            return list;
        }

        public static List<TownHouseSign> GetAccountTownHouseSigns( Mobile m )
        {
            Account a = m.Account as Account;

            if( a == null )
                return null;

            List<TownHouseSign> list = new List<TownHouseSign>();

            for( int i = 0; i < a.Length; ++i )
            {
                if( a[ i ] != null )
                {
                    list.AddRange( GetTownHouse( a[ i ] ) );
                }
            }

            return list;
        }

        public virtual bool CanPlaceHouseModel( BaseHouse house )
        {
            return true;
        }
        #endregion

        #region fields
        public TownFieldDefinition[] FieldDefinitions { get; protected set; }

        public List<TownFieldSign> SystemFields { get; protected set; }

        public static void CheckTownField( TownFieldSign sign )
        {
            if( Debug )
                Config.Pkg.LogInfo( "Registering town field." );

            if( sign.System != null )
                sign.System.RegisterField( sign );

            if( Debug )
                Config.Pkg.LogInfo( "done\n" );
        }

        public void RegisterField( TownFieldSign field )
        {
            if( SystemFields == null )
                SystemFields = new List<TownFieldSign>();

            if( field != null && !SystemFields.Contains( field ) )
            {
                SystemFields.Add( field );
                TownLog.Log( LogType.Fields, String.Format( "TownField Registered. System: {0} - Name: {1} - Contract Location: {2} - DateTime {3}",
                    Definition.TownName, field.Name, field.Location, DateTime.Now ) );
            }
        }

        public void UnRegisterField( TownFieldSign field )
        {
            if( field != null && SystemFields != null && SystemFields.Contains( field ) )
            {
                SystemFields.Remove( field );
                TownLog.Log( LogType.Fields, String.Format( "TownField UnRegistered. System: {0} - Name: {1} - Contract Location: {2}  - DateTime {3}",
                     Definition.TownName, field.Name, field.Location, DateTime.Now ) );
            }
            else
                TownLog.Log( LogType.Fields, String.Format( "Warning: trying to unregister field without any field list." ) );

        }

        public static int MaxFieldsPerAccount = 1;

        public static bool CanBuyTownField( Mobile m, TownFieldSign sign )
        {
            TownSystem system = Find( m );

            if( system != null )
            {
                if( sign != null && system.SystemFields != null )
                {
                    if( system.SystemFields.Contains( sign ) )
                    {
                        List<TownFieldSign> fields = GetAccountTownFieldSigns( m );

                        return fields != null && fields.Count < MaxFieldsPerAccount;
                    }
                }
            }

            return false;
        }

        public static List<TownFieldSign> GetTownField( Mobile m )
        {
            if( m == null )
                return null;

            List<TownFieldSign> list = new List<TownFieldSign>();

            foreach( TownHouseSign sign in TownHouseSign.AllSigns )
            {
                if( sign != null && sign is TownFieldSign )
                {
                    if( sign.House != null && sign.House.Owner != null && sign.House.Owner == m )
                        list.Add( (TownFieldSign)sign );
                }
            }

            return list;
        }

        public static List<TownFieldSign> GetAccountTownFieldSigns( Mobile m )
        {
            Account a = m.Account as Account;

            if( a == null )
                return null;

            List<TownFieldSign> list = new List<TownFieldSign>();

            for( int i = 0; i < a.Length; ++i )
            {
                if( a[ i ] != null )
                {
                    list.AddRange( GetTownField( a[ i ] ) );
                }
            }

            return list;
        }
        #endregion

        #region town treasure
        public int TownTreasure
        {
            get { return (int)m_RawTownTreasure; }
        }

        protected double m_RawTownTreasure;

        public double RawTownTreasure
        {
            get { return m_RawTownTreasure; }
            set
            {
                double oldValue = m_RawTownTreasure;
                if( oldValue != value )
                {
                    m_RawTownTreasure = value;
                    OnRawTownTreasureChanged( oldValue );
                }
            }
        }

        /// <summary>
        /// Event invoked when town treasure changes
        /// </summary>
        public virtual void OnRawTownTreasureChanged( double oldLevel )
        {
            double toEdit = Math.Abs( oldLevel - m_RawTownTreasure );

            TownLog.Log( LogType.Treasure, String.Format( "TownTreasure of {0} {1} of {2} gp in datetime {3}",
                Definition.TownName, ( oldLevel > m_RawTownTreasure ? "DECREASED" : "INCREASED" ), toEdit.ToString( "F2" ), DateTime.Now ) );
        }

        public virtual void EditTownTreasure( double toEdit )
        {
            RawTownTreasure += toEdit;
        }
        #endregion

        #region commercials
        public List<ItemCommercialInfo> ItemPrices { get; protected set; }

        public void RegisterItemPrice( ItemCommercialInfo info )
        {
            if( ItemPrices == null )
                ItemPrices = new List<ItemCommercialInfo>();

            if( info == null )
            {
                Config.Pkg.LogWarningLine( "Warning: Null reference for commercial info for townsystem {0}.", Definition.TownName );
                return;
            }

            if( info.ItemType == null )
            {
                if( Config.Debug )
                    Config.Pkg.LogWarningLine( "Warning: Null info.ItemType for commercial info for townsystem {0}.", Definition.TownName );
                else
                    TownLog.Log( LogType.Commercial, String.Format( "Warning: Null info.ItemType for commercial info for townsystem {0}.", Definition.TownName ) );
                return;
            }

            if( !IsAlreadyInList( info.ItemType ) )
            {
                ItemPrices.Add( info );
                if( Debug )
                    TownLog.Log( LogType.Commercial, String.Format( "ItemPrice Registered for townsystem {3}. Type: {0} - APrice: {1} - TotalSold: {2}",
                        info.ItemType.Name, info.ActualPrice, info.TotalSold, Definition.TownName ) );
            }
            else
                Config.Pkg.LogWarningLine( "Warning: Duplicate for info {0} for townsystem {1}", info.ItemType, Definition.TownName );
        }

        public void VerifyCommercialsCallback()
        {
            if( Debug )
                Config.Pkg.LogInfo( "Commercial system of {0} verifing...", Definition.TownName );

            foreach( TownItemPriceDefinition def in TownItemPriceDefinition.TownPriceDefinitionsList )
            {
                if( IsAlreadyInList( def.ItemType ) )
                    continue;

                RegisterItemPrice( new ItemCommercialInfo( def.ItemType, def.OriginalPrice, 0 ) );
                if( Debug )
                    Config.Pkg.LogInfoLine( "Warning: New commercial registration: {0} - {1}", Definition.TownName, def.ItemType );
            }
        }

        public bool IsAlreadyInList( Type type )
        {
            return GetInfoFromType( type ) != null;
        }

        public ItemCommercialInfo GetInfoFromType( Type t )
        {
            if( t == null )
                return null;

            if( ItemPrices == null )
                return null;

            foreach( ItemCommercialInfo i in ItemPrices )
            {
                if( i.ItemType == t )
                    return i;
            }

            return null;
        }

        protected List<ItemCommercialInfo> m_EditableItemPrices;

        public List<ItemCommercialInfo> EditableItemPrices
        {
            get
            {
                if( m_EditableItemPrices == null )
                    InitEditableItemPrices();

                return m_EditableItemPrices;
            }
        }

        private void InitEditableItemPrices()
        {
            m_EditableItemPrices = new List<ItemCommercialInfo>();

            foreach( ItemCommercialInfo info in ItemPrices )
            {
                if( info.Definition != null && info.Definition.Editable )
                    m_EditableItemPrices.Add( info );
            }
        }

        public int GetTownPrice( Type type )
        {
            ItemCommercialInfo info = GetInfoFromType( type );
            if( info != null )
                return info.ActualPrice;
            else
                return -1;
        }

        /// <summary>
        /// Registra una transazione monetaria avvenuta in citta'.
        /// Ad esempio il cashout di un vendor cittadino o il getFollowers dell'animalTrainer
        /// </summary>
        /// <param name="transaction">quantità totale scambiata</param>
        public void RegisterTransaction( int transaction )
        {
            EditTownTreasure( (double)( transaction * PercMercTaxes ) / 100 );

            TownLog.Log( LogType.Commercial, String.Format( "Registered transaction. Transaction {0}.", transaction ) );
        }

        /// <summary>
        /// Registra una compravendita di oggetti.
        /// </summary>
        /// <param name="vendor">the vendor who wants to register a purchase</param>
        /// <param name="type">tipo di oggetto comprato</param>
        /// <param name="amount">amount dell'oggetto comprato</param>
        public void RegisterPurchase( BaseVendor vendor, Type type, int amount )
        {
            if( amount < 1 )
                return;

            ItemCommercialInfo info = GetInfoFromType( type );
            if( info != null )
            {
                info.TotalSold += amount;

                TownItemPriceDefinition tpd = TownItemPriceDefinition.GetDefFromType( type );
                if( tpd != null )
                {
                    // variazione del tesoro dovuta al sovrapprezzo o sottocosto della merce
                    // puo' essere positiva o negativa
                    EditTownTreasure( ( info.ActualPrice - tpd.OriginalPrice ) * amount );

                    // registra il cambiamento di bilancio nel vendor state
                    TownVendorState state = CommercialStatus.FindVendorState( vendor );
                    if( state != null )
                        state.RegisterVendorStatusPurchase( type, amount, tpd.OriginalPrice, info.ActualPrice );
                }

                TownLog.Log( LogType.Commercial, String.Format( "Purchase registered. Type {0} - Amount {1} - Price {2}.",
                                                              type.Name, amount, info.ActualPrice ) );

                // variazione del tesoro dovuta alle tasse sulla merce venduta (vedi IVA)
                // puo' essere solo positiva
                RegisterTransaction( info.ActualPrice * amount );
            }
            else
            {
                TownLog.Log( LogType.Commercial, String.Format( "Purchase NOT registered. Type {0} - Amount {1}", type.Name, amount ) );
            }
        }

        public virtual void DressTownVendor( Mobile mobile )
        {
        }

        private int m_AccessCost = 1000;

        /// <summary>
        /// If this value is different from 0, access to town function is granted 
        /// even to non citizen.
        /// </summary>
        public virtual int AccessCost
        {
            get { return m_AccessCost; }
            set { m_AccessCost = value; }
        }

        private double m_ScalarCost = 1.0;

        /// <summary>
        /// If this value is different from 0, it is multiplied to each item sold price
        /// </summary>
        public virtual double ScalarCost
        {
            get { return m_ScalarCost; }
            set { m_ScalarCost = value; }
        }

        public virtual bool ShouldScalarCostBeAppliedTo( Mobile buyer )
        {
            return true; // ( buyer != null && Find( buyer ) != this );
        }

        private bool m_VendorBuyAllowed = true;

        /// <summary>
        /// If true vendors can active SELL items (is vendor 'buy' allowed).
        /// </summary>
        public virtual bool VendorBuyAllowed
        {
            get { return m_VendorBuyAllowed; }
            set { m_VendorBuyAllowed = value; }
        }

        private bool m_VendorSellAllowed = true;

        /// <summary>
        /// If true vendors can active BUY items (is vendor 'sell' allowed).
        /// </summary>
        public virtual bool VendorSellAllowed
        {
            get { return m_VendorSellAllowed; }
            set { m_VendorSellAllowed = value; }
        }

        public virtual bool CanAccessTownServices( Mobile citizenDealer, Mobile from, bool message, bool dropAccessCost )
        {
            TownSystem system = Find( from );

            if( system == this )
                return true;
            else if( AccessCost > 0 )
            {
                if( Banker.GetBalance( from ) < AccessCost )
                {
                    // Se il Player non puo' permettersi i servigi allora NON procedere
                    citizenDealer.Say( true, "I'm sorry. You lack funds to access my services." );
                    return false;
                }
                else
                {
                    if( dropAccessCost )
                    {
                        // Se il Player non e' Young ma puo' permettersi i servigi allora procedi
                        DropAccessCost( citizenDealer, from, message );
                    }
                    return true;
                }
            }
            //else if( message )
            //    citizenDealer.Say( true, "You are not co-citizen of mine. I cannot give any help." );

            return true;
        }

        public static void DropAccessCost( Mobile citizenDealer, Mobile from, bool message )
        {
            TownSystem t = Find( citizenDealer );
            if( t == null )
                return;

            Banker.Withdraw( from, t.AccessCost );
            if( message )
            {
                citizenDealer.Say( true, string.Format( "I've taken {0} gold from your bank box", t.AccessCost ) );
                citizenDealer.Say( true, string.Format( "You have {0} gold in tou bank account.", Banker.GetBalance( from ) ) );
            }
        }

        public virtual double GetTownVendorScalar( Mobile from, BaseVendor vendor )
        {
            TownSystem t = Find( vendor );

            if( t == null )
                return 1.0;

            TownVendorState state = CommercialStatus.FindVendorState( vendor );
            if( state != null )
            {
                from.DebugMessage( "Vendor scalar for type {0} is {1}", vendor.GetType().Name, state.Scalar.ToString( "F2" ) );
                return state.Scalar;
            }
            else
                from.DebugMessage( "Vendor scalar for type {0} is 1.0", vendor.GetType().Name );

            return 1.0;
        }

        public virtual void UpdateBuyInfo( Mobile from, BaseVendor vendor, IBuyItemInfo[] buyinfo )
        {
            if( buyinfo == null )
                return;

            double scalar = 1.0;
            if( ShouldScalarCostBeAppliedTo( from ) )
                scalar = ScalarCost; // Global city scalar

            double vendorScalar = GetTownVendorScalar( from, vendor );

            foreach( IBuyItemInfo info in buyinfo )
            {
                if( info == null )
                    continue;

                try
                {
                    if( info is GenericBuyInfo )
                    {
                        ItemCommercialInfo comInfo = GetInfoFromType( ( (GenericBuyInfo)info ).Type );

                        if( comInfo != null )
                        {
                            // case of items fully governed buy citizens
                            info.IsTownBuyInfo = true;
                            info.TownPrice = (int)( comInfo.ActualPrice * scalar * vendorScalar );
                            from.DebugMessage( "Info: {0} - {1} - {2}", comInfo.Definition.ItemName, comInfo.Definition.OriginalPrice, info.TownPrice );
                        }
                        else
                        {
                            // case of items not under commercial rules but under town region
                            info.PriceScalar = (int)( ( scalar * vendorScalar ) * 100 );
                            from.DebugMessage( " Commercials: item not in list: {3}. scalar: {0} vendorScalar: {1} PriceScalar: {2}",
                                               scalar, vendorScalar, info.PriceScalar, ( (GenericBuyInfo)info ).Type.Name );
                        }
                    }
                }
                catch( Exception e )
                {
                    Console.WriteLine( e );
                    Config.Pkg.LogWarningLine( "Error in UpdateBuyInfo for vendor {0} of {1}", vendor.GetType().Name, Definition.TownName );
                }
            }
        }
        #endregion

        #region citizen dress methods
        public static Item Renamed( Item item, string name )
        {
            item.Name = name;
            return item;
        }

        public static Item Immovable( Item item )
        {
            item.Movable = false;
            return item;
        }

        public static Item Newbied( Item item )
        {
            item.LootType = LootType.Newbied;
            return item;
        }

        public static Item Rehued( Item item, int hue )
        {
            item.Hue = hue;
            return item;
        }

        public static Item Layered( Item item, Layer layer )
        {
            item.Layer = layer;
            return item;
        }

        public static Item Resourced( BaseWeapon weapon, CraftResource resource )
        {
            weapon.Resource = resource;
            return weapon;
        }

        public static Item Resourced( BaseArmor armor, CraftResource resource )
        {
            armor.Resource = resource;
            return armor;
        }
        #endregion

        #region taxes
        private int m_PercMercTaxes = 5;

        public virtual int PercMercTaxes
        {
            get { return m_PercMercTaxes; }
            set { m_PercMercTaxes = value; }
        }

        private int m_PercPlayerVendorTaxes;

        public virtual int PercPlayerVendorTaxes
        {
            get { return m_PercPlayerVendorTaxes; }
            set { m_PercPlayerVendorTaxes = value; }
        }

        private int m_LandCost;

        public virtual int LandCost
        {
            get { return m_LandCost; }
            set { m_LandCost = value; }
        }

        private int m_ServiceAccessCost = 1000;

        public virtual int ServiceAccessCost
        {
            get { return m_ServiceAccessCost; }
            set { m_ServiceAccessCost = value; }
        }
        #endregion

        #region traps
        public virtual int MaximumTraps { get { return 15; } }

        public List<BaseCraftableTrap> TownTraps { get; set; }

        public void RegisterTrap( BaseCraftableTrap trap )
        {
            if( TownTraps == null )
                TownTraps = new List<BaseCraftableTrap>();

            if( !TownTraps.Contains( trap ) )
            {
                TownTraps.Add( trap );
                TownLog.Log( LogType.Traps, String.Format( "TownTraps Registered. Serial {0}.", trap.Serial ) );
            }
            else
                TownLog.Log( LogType.Traps, String.Format( "Warning: trying to register trap already in trap list." ) );
        }

        public virtual bool CanPlaceTownDoorTrap( Mobile m, BaseDoor door )
        {
            if( m.AccessLevel > AccessLevel.Player )
                return true;

            TownSystem t = Find( m );
            if( t == null )
                return false;

            if( t == this )
                return door.CanBeTrapped;
            else
            {
                m.SendMessage( "You cannot place a trap in a town different from your." );
                return false;
            }
        }

        public virtual bool IsDoorEnemy( Mobile m, BaseDoor door )
        {
            if( m.AccessLevel > AccessLevel.Player )
                return false;

            TownSystem t = Find( m );
            if( t == null )
                return false;

            if( IsEnemyTo( t ) )
                return true;

            return false;
        }
        #endregion

        #region warfare
        public List<TownSystem> TownEnemies { get; set; }
        public List<TownSystem> TownAllies { get; set; }

        /// <summary>
        /// Metodo invocato per verificare se due <see cref="TownSystem"/> sono in guerra.
        /// </summary>
        public bool IsEnemyTo( TownSystem other )
        {
		if ( other == null )
			return false;

            if( other == this )
                return false;

            if( m_Type != VirtueType.Regular && other.Type != VirtueType.Regular && m_Type != other.Type )
                return true;

            return other != null && TownEnemies != null && TownEnemies.Contains( other );
        }

        /// <summary>
        /// Metodo invocato per verificare se due <see cref="TownSystem"/> sono alleati.
        /// </summary>
        public bool IsAlliedTo( TownSystem other )
        {
            if( other == this )
                return true;

            return other != null && TownAllies != null && TownAllies.Contains( other );
        }

        public bool IsNeutralTo( TownSystem other )
        {
            return other != null && !IsEnemyTo( other ) && !IsAlliedTo( other );
        }

        public void RegisterAlly( TownSystem alliedSystem )
        {
            if( TownAllies == null )
                TownAllies = new List<TownSystem>();

            if( alliedSystem != null && !TownAllies.Contains( alliedSystem ) )
            {
                TownAllies.Add( alliedSystem );
                TownLog.Log( LogType.General, String.Format( "Town Alliance Registered. System: {0} - Allied: {1} - DateTime {2}",
                    Definition.TownName, alliedSystem.Definition.TownName, DateTime.Now ) );
            }
        }

        public void RegisterEnemy( TownSystem enemySystem )
        {
            if( TownEnemies == null )
                TownEnemies = new List<TownSystem>();

            if( enemySystem != null && !TownEnemies.Contains( enemySystem ) )
            {
                TownEnemies.Add( enemySystem );
                TownLog.Log( LogType.General, String.Format( "Town Enemy Registered. System: {0} - Enemy: {1} - DateTime {2}",
                    Definition.TownName, enemySystem.Definition.TownName, DateTime.Now ) );
            }
        }

        public void RemoveAlly( TownSystem toRemove )
        {
            if( TownAllies == null )
                return;

            if( toRemove != null && TownAllies.Contains( toRemove ) )
            {
                TownAllies.Remove( toRemove );
                TownLog.Log( LogType.General, String.Format( "Town Alliance Removed. System: {0} - Allied: {1} - DateTime {2}",
                                                             Definition.TownName, toRemove.Definition.TownName, DateTime.Now ) );
            }
        }

        public void RemoveEnemy( TownSystem toRemove )
        {
            if( TownEnemies == null )
                return;

            if( toRemove != null && TownEnemies.Contains( toRemove ) )
            {
                TownEnemies.Remove( toRemove );
                TownLog.Log( LogType.General, String.Format( "Town Enemy Removed. System: {0} - Enemy: {1} - DateTime {2}",
                                                             Definition.TownName, toRemove.Definition.TownName, DateTime.Now ) );
            }
        }
        #endregion

        #region rank-kills
        public void RefreshWarLordStatus()
        {
            try
            {
                List<TownPlayerState> list = new List<TownPlayerState>( Players );
                list.Sort();

                if( list.Count > 0 && list[ list.Count - 1 ].Mobile != null )
                    m_Warlord = list[ list.Count - 1 ].Mobile;
            }
            catch( Exception ex )
            {
                Config.Pkg.LogErrorLine( ex.ToString() );
            }
        }

        private Mobile m_Warlord;

        public Mobile WarLord
        {
            get
            {
                return m_Warlord;
            }
            set
            {
                Mobile oldValue = m_Warlord;
                if( oldValue != value )
                {
                    m_Warlord = value;
                    OnWarLordChanged( oldValue );
                }
            }
        }

        public virtual void OnWarLordChanged( Mobile oldValue )
        {
            if( m_Warlord != null )
                m_Warlord.InvalidateProperties();
        }

        public static void RegisterCitizenKill( Mobile killer, Mobile victim )
        {
            TownPlayerState tps = TownPlayerState.Find( killer );

            if( tps != null )
            {
                tps.CitizenKills++;

                if( victim != null )
                {
                    tps.LastCitizenKilled = victim;
                    tps.LastCitizenKilledDateTime = DateTime.Now;
                }

                TownLog.Log( LogType.Kills, String.Format( "CITIZEN Kill registered for player {0}. Its CITIZEN kills are now {1}.", killer.Name, tps.CitizenKills ) );
            }
        }

        public static void RegisterEnemyKill( Mobile killer, Mobile victim )
        {
            TownPlayerState tps = TownPlayerState.Find( killer );

            if( tps != null )
            {
                tps.EnemyKills++;

                if( victim != null )
                {
                    tps.Deaths++;
                    tps.LastEnemyKilled = victim;
                    tps.LastEnemyKilledDateTime = DateTime.Now;
                }

                TownLog.Log( LogType.Kills, String.Format( "ENEMY Kill registered for player {0}. Its ENEMY kills are now {1}.", killer.Name, tps.TownRankPoints ) );
            }
        }

        public int TownKillState
        {
            get
            {
                int counter = 0;

                foreach( TownPlayerState t in Players )
                {
                    if( t.TownRankPoints > 0 )
                        counter += t.TownRankPoints;
                }

                return counter;
            }
        }

        public static void HandleTownDeath( Mobile victim )
        {
            HandleTownDeath( victim, null );
        }

        public static void HandleTownDeath( Mobile victim, Mobile killer )
        {
            if( killer == null )
                killer = victim.FindMostRecentDamager( true );

            if( killer is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)killer;

                Mobile master = bc.GetMaster();
                if( master != null )
                    killer = master;
            }

            TownPlayerState killerState = TownPlayerState.Find( killer );
            TownPlayerState victimState = TownPlayerState.Find( victim );
            if( victimState == null || killerState == null )
                return;

            TownLog.Log( LogType.Kills, String.Format( "Killer: {0} - Victim: {1} - DateTime: {2}", killer.Name, victim.Name, DateTime.Now.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" ) ) );

            if( Notoriety.AreSystemsAtWar( killerState.TownSystem, victimState.TownSystem ) )
            {
                RegisterEnemyKill( killer, victim );

                if( victimState.TownRankPoints <= -6 )
                {
                    killer.SendLocalizedMessage( 501693 ); // This victim is not worth enough to get kill points from. 
                }
                else
                {
                    int award = Math.Max( victimState.TownRankPoints / 10, 1 );

                    if( award > 40 )
                        award = 40;

                    if( victimState.CanGivePointsTo( killer ) )
                    {
                        victimState.TownRankPoints -= award;
                        killerState.TownRankPoints += award;

                        int offset = ( award != 1 ? 0 : 2 ); // for pluralization

                        string args = String.Format( "{0}\t{1}\t{2}", award, victim.Name, killer.Name );

                        killer.SendLocalizedMessage( 1042737 + offset, args ); // Thou hast been honored with ~1_KILL_POINTS~ kill point(s) for vanquishing ~2_DEAD_PLAYER~!
                        victim.SendLocalizedMessage( 1042738 + offset, args ); // Thou has lost ~1_KILL_POINTS~ kill point(s) to ~3_ATTACKER_NAME~ for being vanquished!

                        victimState.OnGivenKillPointsTo( killer );
                    }
                    else
                    {
                        killer.SendLocalizedMessage( 1042231 ); // You have recently defeated this enemy and thus their death brings you no honor.
                    }
                }
            }
            /*
            else if( TownHelper.AreAlliedSystems( killerState.TownSystem, victimState.TownSystem ) )
            {
                RegisterCitizenKill( killer, victim );

                int loss = Math.Max( victimState.TownRankPoints / 10, 1 );

                if( loss > 40 )
                    loss = 40;

                killerState.TownRankPoints -= loss;

                killer.SendMessage( String.Format( "Thou hast been punished with with a loss of {0} kill point(s)" +
                                                   " for killing {1}, a cocitizen of yours.", loss, victim.Name ) );
            }
            */
        }
        #endregion

        #region news system and ITownCrierEntryList
        public bool IsEmptyNews
        {
            get { return ( Entries == null || Entries.Count == 0 ); }
        }

        public List<TownCrierEntry> Entries { get; protected set; }

        public TownCrierEntry GetRandomEntry()
        {
            if( IsEmptyNews )
                return null;

            for( int i = Entries.Count - 1; i >= 0; --i )
            {
                if( IsEmptyNews )
                    return null;

                if( i >= Entries.Count )
                    continue;

                TownCrierEntry tce = Entries[ i ];
                if( tce != null && tce.Expired )
                    RemoveEntry( tce ); // mod by Magius(che): questa funzione causa Entris=null effettuare un controllo dopo
            }

            return IsEmptyNews ? null : Entries[ Utility.Random( Entries.Count ) ];
        }

        public TownCrierEntry AddEntry( string[] lines, TimeSpan duration )
        {
            if( Entries == null )
                Entries = new List<TownCrierEntry>();

            TownCrierEntry tce = new TownCrierEntry( lines, duration );

            Entries.Add( tce );

            List<TownCrier> instances = TownCrier.Instances;

            if( instances != null && instances.Count > 0 )
            {
                foreach( TownCrier t in instances )
                    t.ForceBeginAutoShout();
            }

            return tce;
        }

        public void RemoveEntry( TownCrierEntry tce )
        {
            if( Entries == null )
                return;

            Entries.Remove( tce );

            if( Entries.Count == 0 )
                Entries = null;
        }

        public void ClearNews()
        {
            if( Entries == null )
                return;

            Entries.Clear();

            if( Entries.Count == 0 )
                Entries = null;
        }

        public List<MidgardTownCrier> TownCrierInstances { get; protected set; }

        public void RegisterTownCrier( MidgardTownCrier crier )
        {
            if( TownCrierInstances == null )
                TownCrierInstances = new List<MidgardTownCrier>();

            if( crier != null && !TownCrierInstances.Contains( crier ) )
            {
                TownCrierInstances.Add( crier );
            }
        }

        public void UnRegisterTownCrier( MidgardTownCrier crier )
        {
            if( TownCrierInstances == null )
                return;

            if( crier != null && !TownCrierInstances.Contains( crier ) )
            {
                TownCrierInstances.Remove( crier );
            }
        }
        #endregion

        #region guilds
        public int NumTownGuilds
        {
            get
            {
                int counter = 0;
                foreach( BaseGuild bg in BaseGuild.List.Values )
                {
                    Guild g = bg as Guild;
                    if( g != null && !g.Disbanded && Find( g ) == this )
                        counter++;
                }
                return counter;
            }

        }

        /*
        public static void VerifyGuildAccountCoherence( Account a, TownSystem t )
        {
            if( a == null )
                return;

            for( int i = 0; i < a.Count; i++ )
            {
                Mobile m = a[ i ];
                if( m == null || m.Deleted )
                    continue;

                Guild g = m.Guild as Guild;
                if( g != null && !g.Disbanded && Find( g ) != t )
                {
                    g.RemoveMember( m );
                    m.SendMessage( "You have been removed from your guild because of your new town state." );
                }
            }
        }
        */
        #endregion

        #region stuck system
        private static readonly StuckMenuEntry[] m_DefaultStuckEntries = new StuckMenuEntry[]
		{
			// Cove
			new StuckMenuEntry( 1011033, new Point3D[]
				{
					new Point3D( 2231, 1224, 0 ),
					new Point3D( 2228, 1170, 0 ),
                    //new Point3D( 2247, 1194, 0 ),
                    //new Point3D( 2236, 1224, 0 ),
                    //new Point3D( 2273, 1231, 0 )
				} )
        };

        public virtual StuckMenuEntry[] GetStuckEntries()
        {
            return m_DefaultStuckEntries;
        }

        public static StuckMenuEntry[] GetDefaultStuckEntries()
        {
            return m_DefaultStuckEntries;
        }
        #endregion

        #region events On...
        public void OnTownSystemInitialized()
        {
            if( Debug )
                Config.Pkg.LogInfo( "TownSystem of {0} initializing...", Definition.TownName );

            VerifyCommercialsCallback();

            VerifyPlayerStates();

            ConstructGuardLists();

            StartIncomeTimer();

            TownLog.Log( LogType.General, String.Format( "TownSystem of {0} initialized.", Definition.TownName ) );

            if( Debug )
                Config.Pkg.LogInfo( "done\n" );
        }

        public virtual void OnTownKillStateChanged( int oldValue )
        {
        }

        public virtual void OnTownSystemJoined( TownPlayerState tps )
        {
            #region check sulla gilda
            /*
            if( tps.Mobile != null )
            {
                Guild g = tps.Mobile.Guild as Guild;
                if( g != null && !g.Disbanded && Find( g ) != this )
                    g.RemoveMember( tps.Mobile );
            }
            */
            #endregion
        }
        #endregion

        #region find townsystem
        public static TownSystem Find( Mobile mob )
        {
            return Find( mob, false, false );
        }

        public static TownSystem Find( Mobile mob, bool inherit )
        {
            return Find( mob, inherit, false );
        }

        public static TownSystem Find( Mobile mob, bool inherit, bool allegiance )
        {
            TownPlayerState pl = TownPlayerState.Find( mob );

            if( pl != null )
                return pl.TownSystem;

            if( mob is BaseVendor )
                return Find( ( (BaseVendor)mob ).MidgardTown );

            if( mob is TownGuard )
                return Find( ( (TownGuard)mob ).Town );

            if( mob is BaseTownGuard )
                return ( (BaseTownGuard)mob ).System;

            if( inherit && mob is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)mob;

                if( bc.Controlled )
                    return Find( bc.ControlMaster, false );
                else if( bc.Summoned )
                    return Find( bc.SummonMaster, false );
            }

            return null;
        }

        public static TownSystem Find( Point3D p, Map map )
        {
            return Find( Region.Find( p, map ) );
        }

        public static TownSystem Find( HouseRegion r )
        {
            if( r.House != null )
            {
                TownSystem t = Find( r.House );
                if( t != null )
                    return t;
            }

            return null;
        }

        public static TownSystem Find( Region r )
        {
            foreach( TownSystem t in TownSystems )
            {
                if( r.IsPartOf( t.Definition.StandardRegionName ) )
                    return t;
            }

            foreach( TownSystem t in TownSystems )
            {
                if( t.Definition.ExtraRegionNames != null )
                {
                    foreach( string s in t.Definition.ExtraRegionNames )
                    {
                        if( !String.IsNullOrEmpty( s ) && r.IsPartOf( s ) )
                            return t;
                    }
                }
            }

            return null;
        }

        public static TownSystem Find( MidgardTowns town )
        {
            foreach( TownSystem t in TownSystems )
            {
                if( t.Definition.Town == town )
                    return t;
            }

            return null;
        }

        public static TownSystem Find( Guild guild )
        {
            if( guild == null || guild.Disbanded )
                return null;

            Guildstone gs = guild.Guildstone as Guildstone;

            if( gs != null && !gs.Deleted )
            {
                return Find( gs.Town );
            }
            else if( guild.Leader != null )
            {
                return Find( guild.Leader );
            }

            return null;
        }

        public static TownSystem Find( Corpse corpse )
        {
            if( corpse != null && corpse.Owner != null && !corpse.Owner.Deleted )
                return Find( corpse.Owner );
            return null;
        }

        public static TownSystem Find( BaseHouse house )
        {
            if( house != null && house.Sign != null && house.Sign.Map != null )
                return Find( house.Sign.Location, house.Sign.Map );
            return null;
        }

        public static TownSystem Find( BaseDoor door )
        {
            return Find( door.Location, door.Map );
        }

        public static TownSystem Find( TownBanFlag flag )
        {
            foreach( TownSystem system in TownSystems )
            {
                if( system.Definition.BanFlag == flag )
                    return system;
            }

            return null;
        }
        #endregion

        #region arenas
        public static string[] SpecialArenas = new string[] { "Arena di Luxor", "Arena dell'Acqua" };

        public virtual string[] GetTownArenas()
        {
            return null;
        }

        public static List<string> GetAllArenas()
        {
            List<string> arenas = new List<string>();

            foreach( TownSystem t in TownSystems )
            {
                string[] arenasStrings = t.GetTownArenas();
                if( arenasStrings != null )
                    arenas.AddRange( t.GetTownArenas() );
            }

            arenas.AddRange( SpecialArenas );

            return arenas;
        }

        public bool IsInTownArena( Mobile m )
        {
            if( m == null || m.Map == Map.Internal )
                return false;

            string[] arenas = GetTownArenas();
            if( arenas == null || arenas.Length == 0 )
                return false;

            foreach( string s in arenas )
            {
                if( m.Region.IsPartOf( s ) )
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Verify if our mobile is in an arena
        /// </summary>
        public static bool IsInAnyTownArena( Mobile m )
        {
            foreach( TownSystem t in TownSystems )
            {
                if( t.IsInTownArena( m ) )
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a player is IN an arena and the other is OUT
        /// </summary>
        public static bool CheckArenaBounds( Mobile playerIn, Mobile playerOut )
        {
            if( playerIn.Region != playerOut.Region )
            {
                foreach( string s in GetAllArenas() )
                {
                    if( playerIn.Region.IsPartOf( s ) || playerIn.Region.IsPartOf( s ) )
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region vendors
        protected TownCommercialStatus m_CommercialStatus;

        public TownCommercialStatus CommercialStatus
        {
            get
            {
                if( m_CommercialStatus == null )
                    m_CommercialStatus = new TownCommercialStatus( this );

                return m_CommercialStatus;
            }
        }

        public virtual double GetDefaultScalar( Type vendorType )
        {
            return CommercialStatus.GetDefaultScalar( vendorType );
        }
        #endregion

        #region healers
        private bool m_AllowNoCitizenResurrection = true;

        public virtual bool AllowNoCitizenResurrection
        {
            get { return m_AllowNoCitizenResurrection; }
            set { m_AllowNoCitizenResurrection = value; }
        }

        private bool m_AllowCriminalResurrection = true;

        public virtual bool AllowCriminalResurrection
        {
            get { return m_AllowCriminalResurrection; }
            set { m_AllowCriminalResurrection = value; }
        }

        private bool m_AllowMurderResurrection = true;

        public virtual bool AllowMurderResurrection
        {
            get { return m_AllowMurderResurrection; }
            set { m_AllowMurderResurrection = value; }
        }

        private bool m_AllowEnemyResurrection = true;

        public virtual bool AllowEnemyResurrection
        {
            get { return m_AllowEnemyResurrection; }
            set { m_AllowEnemyResurrection = value; }
        }

        public virtual bool CheckResurrect( BaseHealer healer, Mobile target )
        {
            Midgard2PlayerMobile m2Pm = target as Midgard2PlayerMobile;
            TownSystem system = Find( target );

            if( m2Pm == null )
                return false;

            bool shouldResurrect = true;

            if( !AllowNoCitizenResurrection && m2Pm.Town == MidgardTowns.None )
            {
                healer.Say( true, "Thou art not a citizen. I shall not resurrect thee." );
                shouldResurrect = false;
            }

            if( !AllowCriminalResurrection && m2Pm.Criminal )
            {
                healer.Say( true, "Thou art a criminal. I shall not resurrect thee." );
                shouldResurrect = false;
            }

            if( !AllowMurderResurrection && Server.Notoriety.Compute( target, target ) == Server.Notoriety.Murderer )
            {
                healer.Say( true, "Thou'rt not a decent and good person. I shall not resurrect thee." );
                shouldResurrect = false;
            }

            if( !AllowEnemyResurrection && IsEnemyTo( system ) )
            {
                healer.Say( true, "Thou'rt not a friend of mine. I shall not resurrect thee." );
                shouldResurrect = false;
            }

            return shouldResurrect;
        }
        #endregion

        #region commandable guards
        public List<TownWayPoint> WayPoints { get; set; }

        public void RegisterWayPoint( TownWayPoint wayPoint )
        {
            if( WayPoints == null )
                WayPoints = new List<TownWayPoint>();

            if( !WayPoints.Contains( wayPoint ) )
            {
                WayPoints.Add( wayPoint );
            }
        }

        public virtual void ToggleWayPointsVisibility( bool show )
        {
            if( WayPoints == null )
                return;

            foreach( TownWayPoint t in WayPoints )
                t.Visible = show;
        }

        public virtual void ResetWayPoints()
        {
            if( WayPoints == null )
                return;

            for( int i = 0; i < WayPoints.Count; i++ )
            {
                if( !WayPoints[ i ].Deleted )
                    WayPoints[ i ].Delete();
            }
        }

        public virtual void DressTownGuard( BaseCreature guard )
        {
            Console.WriteLine( "DressTownGuard for town: {0}", Definition.TownName );

            guard.InitStats( 1000, 1000, 1000 );

            guard.Title = "the guard";
            guard.SpeechHue = Utility.RandomDyedHue();

            guard.Hue = Utility.RandomSkinHue();
            guard.Female = Utility.RandomBool();

            guard.SetSkill( SkillName.Swords, 110.0, 120.0 );
            guard.SetSkill( SkillName.Macing, 110.0, 120.0 );
            guard.SetSkill( SkillName.Wrestling, 110.0, 120.0 );
            guard.SetSkill( SkillName.Tactics, 110.0, 120.0 );
            guard.SetSkill( SkillName.MagicResist, 110.0, 120.0 );
            guard.SetSkill( SkillName.Healing, 110.0, 120.0 );
            guard.SetSkill( SkillName.Anatomy, 110.0, 120.0 );

            guard.SetSkill( SkillName.Magery, 110.0, 120.0 );
            guard.SetSkill( SkillName.EvalInt, 110.0, 120.0 );
            guard.SetSkill( SkillName.Meditation, 110.0, 120.0 );
            guard.SetSkill( SkillName.DetectHidden, 110.0, 120.0 );

            if( guard.Female )
            {
                guard.Body = 0x191;
                guard.Name = NameList.RandomName( "female" );

                switch( Utility.Random( 2 ) )
                {
                    case 0:
                        guard.AddItem( new LeatherSkirt() );
                        break;
                    case 1:
                        guard.AddItem( new LeatherShorts() );
                        break;
                }

                switch( Utility.Random( 5 ) )
                {
                    case 0:
                        guard.AddItem( new FemaleLeatherChest() );
                        break;
                    case 1:
                        guard.AddItem( new FemaleStuddedChest() );
                        break;
                    case 2:
                        guard.AddItem( new LeatherBustierArms() );
                        break;
                    case 3:
                        guard.AddItem( new StuddedBustierArms() );
                        break;
                    case 4:
                        guard.AddItem( new FemalePlateChest() );
                        break;
                }
            }
            else
            {
                guard.Body = 0x190;
                guard.Name = NameList.RandomName( "male" );

                guard.AddItem( new PlateChest() );
                guard.AddItem( new PlateArms() );
                guard.AddItem( new PlateLegs() );

                switch( Utility.Random( 3 ) )
                {
                    case 0:
                        guard.AddItem( new Doublet( Utility.RandomNondyedHue() ) );
                        break;
                    case 1:
                        guard.AddItem( new Tunic( Utility.RandomNondyedHue() ) );
                        break;
                    case 2:
                        guard.AddItem( new BodySash( Utility.RandomNondyedHue() ) );
                        break;
                }
            }
            Utility.AssignRandomHair( guard );

            if( Utility.RandomBool() )
                Utility.AssignRandomFacialHair( guard, guard.HairHue );

            Halberd weapon = new Halberd();

            weapon.Movable = false;
            weapon.Crafter = guard;
            weapon.Quality = WeaponQuality.Exceptional;
            weapon.CustomQuality = Quality.Exceptional;

            guard.AddItem( weapon );

            Container pack = new Backpack();

            pack.Movable = false;
            guard.AddItem( pack );

            Console.WriteLine( "Weapon is Halberd: {0}", guard.Weapon is Halberd );
        }

        public int MilitiaCost
        {
            get
            {
                List<GuardList> guardLists = GuardLists;
                int upkeep = 0;

                foreach( GuardList t in guardLists )
                    upkeep += t.Guards.Count * t.Definition.Upkeep;

                return upkeep;
            }
        }

        public List<BaseTownGuard> BuildFinanceList()
        {
            List<BaseTownGuard> list = new List<BaseTownGuard>();

            List<GuardList> guardLists = GuardLists;

            foreach( GuardList t in guardLists )
                list.AddRange( t.Guards );

            return list;
        }

        public List<GuardList> GuardLists { get; set; }

        public void ConstructGuardLists()
        {
            GuardDefinition[] defs = Definition.Guards;

            GuardLists = new List<GuardList>();

            foreach( GuardDefinition t in defs )
                GuardLists.Add( new GuardList( t ) );
        }

        public GuardList FindGuardList( Type type )
        {
            if( GuardLists == null )
                return null;

            List<GuardList> guardLists = GuardLists;

            foreach( GuardList guardList in guardLists )
            {
                if( guardList.Definition.Type == type )
                    return guardList;
            }

            return null;
        }

        public bool RegisterGuard( BaseTownGuard guard )
        {
            if( guard == null )
                return false;

            GuardList guardList = FindGuardList( guard.GetType() );

            if( guardList == null )
                return false;

            guardList.Guards.Add( guard );
            return true;
        }

        public bool UnregisterGuard( BaseTownGuard guard )
        {
            if( guard == null )
                return false;

            GuardList guardList = FindGuardList( guard.GetType() );

            if( guardList == null )
                return false;

            if( !guardList.Guards.Contains( guard ) )
                return false;

            guardList.Guards.Remove( guard );
            return true;
        }

        public bool CanCommandGuards( Mobile mob )
        {
            if( mob == null || mob.Deleted )
                return false;

            if( Find( mob.Region ) != this )
                return false;

            return HasAccess( TownAccessFlags.CommandGuards, mob );
        }

        public void BeginOrderFiring( Mobile from )
        {
            if( !CanCommandGuards( from ) )
                return;

            from.SendMessage( "Target the guard you wish to dismiss." );
            from.BeginTarget( 12, false, TargetFlags.None, new TargetCallback( EndOrderFiring ) );
        }

        public void EndOrderFiring( Mobile from, object obj )
        {
            if( !CanCommandGuards( from ) )
                return;

            if( obj is BaseTownGuard )
            {
                BaseTownGuard guard = (BaseTownGuard)obj;

                if( guard.System == this )
                    guard.Delete();
            }
            else
            {
                from.SendMessage( "That is not a town guard!" );
            }
        }

        public int NetCashFlow
        {
            get { return TownTreasure - MilitiaCost; }
        }

        private Timer m_IncomeTimer;

        public void StartIncomeTimer()
        {
            if( m_IncomeTimer != null )
                m_IncomeTimer.Stop();

            if( !AcceptCitizens )
                return;

            if( Debug )
                Config.Pkg.LogInfoLine( "Starting town income timer." );

            m_IncomeTimer = Timer.DelayCall( TimeSpan.FromMinutes( 1.0 ), TimeSpan.FromMinutes( 1.0 ), new TimerCallback( CheckIncome ) );
        }

        public void StopIncomeTimer()
        {
            if( m_IncomeTimer != null )
                m_IncomeTimer.Stop();

            m_IncomeTimer = null;
        }

        public DateTime LastIncome { get; set; }

        public static readonly TimeSpan IncomePeriod = TimeSpan.FromDays( 1.0 );

        public void CheckIncome()
        {
            if( ( LastIncome + IncomePeriod ) > DateTime.Now )
                return;

            ProcessIncome();
        }

        public void ProcessIncome()
        {
            LastIncome = DateTime.Now;

            int flow = NetCashFlow;

            if( flow < 0 )
            {
                List<BaseTownGuard> toDelete = BuildFinanceList();

                while( flow < 0 && toDelete.Count > 0 )
                {
                    int index = Utility.Random( toDelete.Count );
                    Mobile mob = toDelete[ index ];

                    mob.Delete();

                    toDelete.RemoveAt( index );
                    flow = NetCashFlow;
                }
            }

            RawTownTreasure = NetCashFlow;
        }
        #endregion

        #region access
        public virtual bool HasAccess( TownAccessFlags flag, Mobile from )
        {
            if( from != null && from.AccessLevel > AccessLevel.Counselor )
                return true;

            if( TestCenter.Enabled )
                return true;

            TownPlayerState tps = TownPlayerState.Find( from );

            if( tps != null )
            {
                if( tps.TownLevel != null )
                    return tps.TownLevel.GetFlag( flag );
                else
                    Config.Pkg.LogWarningLine( "Warning: TownPlayerState not null with null TownLevel." );
            }

            return false;
        }

        public virtual bool HasAccess( TownAccessLevel lvl, Mobile from )
        {
            if( TestCenter.Enabled )
                return true;

            TownPlayerState tps = TownPlayerState.Find( from );

            if( tps != null )
            {
                if( lvl == TownAccessLevel.Staff )
                    return from.AccessLevel > AccessLevel.Counselor;
                else if( lvl == TownAccessLevel.Citizen )
                    return tps.TownSystem == this;
            }

            return false;
        }
        #endregion

        #region TravelAgentLocations
        public virtual bool IsRestricted( MidgardTravelAgent.LocationEntry entry )
        {
            return false;
        }
        #endregion

        #region thievery
        public List<CriminalProfile> CriminalProfiles { get; set; }

        public void RegisterCriminal( Mobile criminal, CrimeType crimeType )
        {
            if( criminal == null || criminal.Deleted )
                return;

            if( CriminalProfiles == null )
                CriminalProfiles = new List<CriminalProfile>();

            CriminalProfile profile = GetCriminalProfile( criminal ) ?? new CriminalProfile( criminal, this );

            profile.AddCrime( criminal, crimeType );

            TownLog.Log( LogType.General, String.Format( "Criminal Registered. System: {0} - Name: {1} - DateTime {2}",
                                                         Definition.TownName, criminal.Name, DateTime.Now ) );
        }

        public virtual bool IsKnownCriminalForTown( Mobile m )
        {
            return GetCriminalProfile( m ) != null;
        }

        public virtual CriminalProfile GetCriminalProfile( Mobile m )
        {
            if( CriminalProfiles == null )
                return null;

            foreach( CriminalProfile criminalProfile in CriminalProfiles )
            {
                if( criminalProfile.Criminal == m )
                    return criminalProfile;
            }

            return null;
        }

        public virtual bool TryArrest( Mobile from, Mobile criminal, bool message )
        {
            if( from == null || from.Deleted )
                return false;

            TownSystem fromSys = Find( from );
            // TownSystem criminalSys = Find( criminal );

            if( fromSys != this )
            {
                if( message )
                    from.SendMessage( "Thou cannot try that because thou are not citizen of {0}.", Definition.TownName );
            }
            else if( !IsKnownCriminalForTown( criminal ) )
            {
                if( message )
                    from.SendMessage( "That is not a knwon criminal for {0}.", Definition.TownName );
            }
            else if( !CanCommandGuards( from ) )
            {
                if( message )
                    from.SendMessage( "Thou cannot try that because thou are not a sherriff of {0}.", Definition.TownName );
            }
            else if( !from.InLOS( criminal ) || !Utility.InRange( from.Location, criminal.Location, 15 ) )
            {
                if( message )
                    from.SendMessage( "Thou cannot see that criminal.", Definition.TownName );
            }
            else if( Find( criminal.Location, criminal.Map ) != this )
            {
                if( message )
                    from.SendMessage( "He is not under the jurisdiction of {0}.", Definition.TownName );
            }
            else
            {
                CriminalProfile profile = GetCriminalProfile( criminal );
                if( profile != null && profile.IsCatchable )
                {
                    TownJailSystem.Instance.Arrest( profile, from );
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region virtues
        private VirtueType m_Type;
        private DateTime m_TypeLastChange;

        [CommandProperty( AccessLevel.GameMaster )]
        public VirtueType Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                if( m_Type != value )
                {
                    m_Type = value;
                    m_TypeLastChange = DateTime.Now;
                }
            }
        }
        #endregion

        #region context menus
        public static void GetSelfContextMenus( Mobile from, Mobile player, List<ContextMenuEntry> list )
        {
            list.Add( new CallbackPlayerEntry( 1039, new ContextPlayerCallback( ToggleCitizenStatusDisplay ), player ) ); // Toggle Citizen Status Display
        }

        public static bool IsShowingTownTitle( Mobile from )
        {
            Midgard2PlayerMobile player = from as Midgard2PlayerMobile;

            return player != null && player.DisplayCitizenStatus;
        }

        /// <summary>
        /// Metodo invocato quando un cittadino vuole ebilitare o disabilitare la context
        /// per il titolo cittadino.
        /// </summary>
        public static void ToggleCitizenStatusDisplay( Mobile from )
        {
            if( !from.CheckAlive() || TownPlayerState.Find( from ) == null )
                return;

            Midgard2PlayerMobile player = from as Midgard2PlayerMobile;
            if( player == null )
                return;

            // 1064250 You have chosen to hide your citizen status.
            // 1064251 You have chosen to display your citizen status.
            from.SendLocalizedMessage( player.DisplayCitizenStatus ? 1064250 : 1064251 );

            from.SendLocalizedMessage( 1064252 ); // You will be disconnected to update your status.

            player.DisplayCitizenStatus = !player.DisplayCitizenStatus;
        }
        #endregion

        #region other
        public override string ToString()
        {
            return Definition.TownName;
        }

        public static TownSystem Parse( string name )
        {
            if( String.IsNullOrEmpty( name ) )
                return null;

            foreach( TownSystem t in TownSystems )
            {
                if( t.Definition.TownName == name )
                    return t;
            }

            foreach( TownSystem t in TownSystems )
            {
                if( Utility.InsensitiveCompare( Enum.GetName( typeof( MidgardTowns ), t.Definition.Town ), name ) == 0 )
                    return t;
            }

            Config.Pkg.LogWarningLine( "Warning: Null Parse in TownSystem.Parse. Name: {0}", name );

            return null;
        }

        public int CompareTo( object obj )
        {
            return Insensitive.Compare( Definition.TownName, ( (TownSystem)obj ).Definition.TownName );
        }

        public virtual bool IsMurdererTown
        {
            get { return false; }
        }

        public virtual bool IsEvilAlignedTown
        {
            get { return false; }
        }

        public virtual bool IsGoodAlignedTown
        {
            get { return false; }
        }

        public virtual bool IsEligible( Mobile mob )
        {
            return ( mob != null && mob is Midgard2PlayerMobile && !(TownHelper.IsTownBanned( (Midgard2PlayerMobile)mob, this ) || TownHelper.IsTownPermaBanned( (Midgard2PlayerMobile)mob, this ) ) );
        }

        public bool IsInTown( Mobile mob )
        {
            return Find( mob.Region ) == this;
        }

        public Point3D GoLocation
        {
            get { return m_Definition.StartCityInfo.Location; }
        }

        public Point3D TownStoneLocation
        {
            get { return m_Definition.TownstoneLocation; }
        }

        public void VerifyItemPrices()
        {
            foreach( ItemCommercialInfo t in ItemPrices )
                RegisterItemPrice( t );
        }
        #endregion

        protected TownSystem()
        {
            m_LandCost = 0;
            TownCrierInstances = new List<MidgardTownCrier>();
            SystemFields = new List<TownFieldSign>();
            SystemTownHouses = new List<TownHouseSign>();
            ExiledPlayers = new List<Mobile>();
            PermaExiledPlayers = new List<Mobile>();
            Players = new TownPlayerStateCollection();
            ItemPrices = new List<ItemCommercialInfo>();
            //SystemFields = new List<TownFieldSign>();
            TownTraps = new List<BaseCraftableTrap>();
            //ExiledPlayers = new List<Mobile>();
            TownEnemies = new List<TownSystem>();
            TownAllies = new List<TownSystem>();
            //TownCrierInstances = new List<MidgardTownCrier>();
            Entries = new List<TownCrierEntry>();
            //SystemTownHouses = new List<TownHouseSign>();
        }

        #region serial-deserial
        private static readonly string TownSystemSavePath = Path.Combine( Path.Combine( "Saves", "TownSystem" ), "TownSystemSave.bin" );

        static TownSystem()
        {
            TownAccessEnabled = true;
        }

        public static void Save( WorldSaveEventArgs e )
        {
            if( Debug )
                Config.Pkg.LogInfo( "{0}: Saving...", Config.Pkg.Title );

            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                if( !Directory.Exists( Path.GetDirectoryName( TownSystemSavePath ) ) )
                    Directory.CreateDirectory( Path.GetDirectoryName( TownSystemSavePath ) );

                BinaryFileWriter writer = new BinaryFileWriter( TownSystemSavePath, true );

                writer.Write( 2 ); // version

                writer.Write( TownSystems.Length );

                foreach( TownSystem t in TownSystems )
                    t.Serialize( writer );

                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogErrorLine( "Error serializing {0}.", Config.Pkg.Title );
                Config.Pkg.LogErrorLine( ex.ToString() );
            }

            if( Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        public static void Load()
        {
            if( Debug )
                Config.Pkg.LogInfo( "{0}: Loading...", Config.Pkg.Title );

            while( !File.Exists( TownSystemSavePath ) )
            {
                Config.Pkg.LogWarningLine( "Warning: {0} not found.", TownSystemSavePath );
                Config.Pkg.LogInfoLine( " - Press return to continue, or R to try again." );
                string str = Console.ReadLine();

                if( str == null || str.ToLower() != "r" )
                    return;
            }

            try
            {
                BinaryReader bReader = new BinaryReader( File.OpenRead( TownSystemSavePath ) );
                BinaryFileReader reader = new BinaryFileReader( bReader );

                int version = reader.ReadInt();

                switch( version )
                {
                    case 2:
                        {
                            int count = reader.ReadInt();

                            for( int i = 0; i < count; ++i )
                                TownSystems[ i ].Deserialize( reader );

                            break;
                        }
                    case 1:
                        {
                            for( int i = 0; i < 4; ++i )
                                TownSystems[ i ].Deserialize( reader );

                            break;
                        }
                    case 0:
                        {
                            for( int i = 0; i < 2; ++i )
                                TownSystems[ i ].Deserialize( reader );

                            break;
                        }
                }

                bReader.Close();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogInfoLine( "Error de-serializing {0}.", Config.Pkg.Title );
                Config.Pkg.LogInfoLine( ex.ToString() );
            }

            if( Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        enum SaveFlag
        {
            None = 0x00000000,

            AcceptCitizens = 0x00000001,
            AccessCost = 0x00000002,
            ScalarCost = 0x00000004,
            VendorBuyAllowed = 0x00000008,

            VendorSellAllowed = 0x00000010,
            PercMercTaxes = 0x00000020,
            PercPlayerVendorTaxes = 0x00000040,
            LandCost = 0x00000080,

            ServiceAccessCost = 0x00000100,
            AllowNoCitizenResurrection = 0x00000200,
            AllowCriminalResurrection = 0x00000400,
            AllowMurderResurrection = 0x00000800,

            AllowEnemyResurrection = 0x00001000,
            LastIncome = 0x00002000,
            CommercialStatus = 0x00004000,
            Warlord = 0x00008000,

            News = 0x00010000,
            ExiledPlayers = 0x00020000,
            ItemCommercialInfo = 0x00040000,
            Traps = 0x00080000,

            Treasure = 0x00100000,
            Players = 0x00200000,
            Alliances = 0x00400000,
            Enemies = 0x00800000,

            CriminalProfiles = 0x01000000,
            VirtueType = 0x02000000,
            PermaExiledPlayers = 0x04000000
        }

        private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
        {
            if( setIf )
                flags |= toSet;
        }

        private static bool GetSaveFlag( ref SaveFlag flags, SaveFlag toGet )
        {
            return ( ( flags & toGet ) != 0 );
        }

        public virtual void Deserialize( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 11:
                    goto case 10;
                case 10:
                    goto case 9;
                case 9:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadInt();

                        if( GetSaveFlag( ref flags, SaveFlag.AcceptCitizens ) )
                            AcceptCitizens = reader.ReadBool();

                        if( GetSaveFlag( ref flags, SaveFlag.AccessCost ) )
                            AccessCost = reader.ReadInt();

                        if( GetSaveFlag( ref flags, SaveFlag.ScalarCost ) )
                            ScalarCost = reader.ReadDouble();

                        if( GetSaveFlag( ref flags, SaveFlag.VendorBuyAllowed ) )
                            VendorBuyAllowed = reader.ReadBool();

                        if( GetSaveFlag( ref flags, SaveFlag.VendorSellAllowed ) )
                            VendorSellAllowed = reader.ReadBool();

                        if( GetSaveFlag( ref flags, SaveFlag.PercMercTaxes ) )
                            PercMercTaxes = reader.ReadInt();

                        if( GetSaveFlag( ref flags, SaveFlag.PercPlayerVendorTaxes ) )
                            PercPlayerVendorTaxes = reader.ReadInt();

                        if( GetSaveFlag( ref flags, SaveFlag.LandCost ) )
                            LandCost = reader.ReadInt();

                        if( GetSaveFlag( ref flags, SaveFlag.ServiceAccessCost ) )
                            ServiceAccessCost = reader.ReadInt();

                        if( GetSaveFlag( ref flags, SaveFlag.AllowNoCitizenResurrection ) )
                            AllowNoCitizenResurrection = reader.ReadBool();

                        if( GetSaveFlag( ref flags, SaveFlag.AllowCriminalResurrection ) )
                            AllowCriminalResurrection = reader.ReadBool();

                        if( GetSaveFlag( ref flags, SaveFlag.AllowMurderResurrection ) )
                            AllowMurderResurrection = reader.ReadBool();

                        if( GetSaveFlag( ref flags, SaveFlag.AllowEnemyResurrection ) )
                            AllowEnemyResurrection = reader.ReadBool();

                        if( GetSaveFlag( ref flags, SaveFlag.LastIncome ) )
                            LastIncome = reader.ReadDateTime();

                        if( GetSaveFlag( ref flags, SaveFlag.CommercialStatus ) )
                        {
                            if( version < 10 )
                            {
                                int stateCount = reader.ReadInt();

                                if( stateCount > 0 )
                                {
                                    for( int i = 0; i < stateCount; i++ )
                                    {
                                        bool stateIsNotNull = reader.ReadBool();

                                        if( stateIsNotNull )
                                        {
                                            TownVendorState state = new TownVendorState();
                                            state.Deserialize( this, reader );
                                        }
                                    }
                                }
                            }
                            else
                                m_CommercialStatus = new TownCommercialStatus( reader );
                        }

                        if( GetSaveFlag( ref flags, SaveFlag.Warlord ) )
                            m_Warlord = reader.ReadMobile();

                        if( GetSaveFlag( ref flags, SaveFlag.News ) )
                        {
                            int newsCount = reader.ReadInt();

                            for( int i = 0; i < newsCount; i++ )
                            {
                                int newsLenght = reader.ReadInt();
                                string[] lines = new string[ newsLenght ];

                                for( int j = 0; j < newsLenght; j++ )
                                    lines[ j ] = reader.ReadString();

                                DateTime newsExpiration = reader.ReadDateTime();

                                AddEntry( lines, ( newsExpiration - DateTime.Now ) );
                            }
                        }

                        if( GetSaveFlag( ref flags, SaveFlag.ExiledPlayers ) )
                            ExiledPlayers = reader.ReadStrongMobileList();

                        if( GetSaveFlag( ref flags, SaveFlag.PermaExiledPlayers ) )
                            PermaExiledPlayers = reader.ReadStrongMobileList();

                        if( GetSaveFlag( ref flags, SaveFlag.ItemCommercialInfo ) )
                        {
                            int itemsInfoesCount = reader.ReadInt();

                            for( int i = 0; i < itemsInfoesCount; ++i )
                            {
                                RegisterItemPrice( new ItemCommercialInfo( this, reader ) );
                            }
                        }

                        if( GetSaveFlag( ref flags, SaveFlag.Traps ) )
                        {
                            int trapCount = reader.ReadInt();

                            for( int i = 0; i < trapCount; ++i )
                            {
                                BaseCraftableTrap trap = reader.ReadItem() as BaseCraftableTrap;

                                if( trap != null && !trap.CheckDecay() )
                                    RegisterTrap( trap );
                            }
                        }

                        if( GetSaveFlag( ref flags, SaveFlag.Treasure ) )
                            m_RawTownTreasure = reader.ReadDouble();

                        if( GetSaveFlag( ref flags, SaveFlag.Players ) )
                        {
                            int playerCount = reader.ReadEncodedInt();

                            for( int i = 0; i < playerCount; ++i )
                            {
                                TownPlayerState pl = new TownPlayerState( this, reader );

                                if( pl.Mobile != null )
                                    pl.CheckAttach();
                            }
                        }

                        if( GetSaveFlag( ref flags, SaveFlag.Alliances ) )
                        {
                            int alliedCount = reader.ReadEncodedInt();

                            for( int i = 0; i < alliedCount; ++i )
                                RegisterAlly( ReadReference( reader ) );
                        }

                        if( GetSaveFlag( ref flags, SaveFlag.Enemies ) )
                        {
                            int enemCount = reader.ReadEncodedInt();

                            for( int i = 0; i < enemCount; ++i )
                                RegisterEnemy( ReadReference( reader ) );
                        }

                        if( GetSaveFlag( ref flags, SaveFlag.VirtueType ) )
                            Type = (VirtueType)reader.ReadEncodedInt();
                        break;
                    }
                case 8:
                    {
                        if( version < 9 )
                            reader.ReadDateTime();

                        goto case 7;
                    }
                case 7:
                    {
                        if( version < 9 )
                        {
                            int stateCount = reader.ReadInt();

                            if( stateCount > 0 )
                            {
                                for( int i = 0; i < stateCount; i++ )
                                {
                                    bool stateIsNotNull = reader.ReadBool();

                                    if( stateIsNotNull )
                                    {
                                        TownVendorState state = new TownVendorState();
                                        state.Deserialize( this, reader );
                                    }
                                }
                            }
                        }
                        goto case 6;
                    }
                case 6:
                    {
                        if( version < 9 )
                            m_Warlord = reader.ReadMobile();

                        goto case 5;
                    }
                case 5:
                    {
                        if( version < 9 )
                        {
                            int newsCount = reader.ReadInt();

                            for( int i = 0; i < newsCount; i++ )
                            {
                                int newsLenght = reader.ReadInt();
                                string[] lines = new string[ newsLenght ];

                                for( int j = 0; j < newsLenght; j++ )
                                    lines[ j ] = reader.ReadString();

                                DateTime newsExpiration = reader.ReadDateTime();

                                AddEntry( lines, ( newsExpiration - DateTime.Now ) );
                            }
                        }
                        goto case 4;
                    }
                case 4:
                    {
                        if( version < 9 )
                            ExiledPlayers = reader.ReadStrongMobileList();
                        goto case 3;
                    }
                case 3:
                    {
                        if( version < 9 )
                        {
                            int itemsInfoesCount = reader.ReadInt();

                            for( int i = 0; i < itemsInfoesCount; ++i )
                            {
                                ItemCommercialInfo info = new ItemCommercialInfo( this, reader );

                                Timer.DelayCall( TimeSpan.Zero, delegate { RegisterItemPrice( info ); } );
                            }
                        }
                        goto case 2;
                    }
                case 2:
                    {
                        if( version < 9 )
                        {
                            int trapCount = reader.ReadInt();

                            for( int i = 0; i < trapCount; ++i )
                            {
                                BaseCraftableTrap trap = reader.ReadItem() as BaseCraftableTrap;

                                if( trap != null && !trap.CheckDecay() )
                                    Timer.DelayCall( TimeSpan.Zero, delegate { RegisterTrap( trap ); } );
                            }
                        }
                        goto case 1;
                    }
                case 1:
                    {
                        if( version < 9 )
                        {
                            m_RawTownTreasure = reader.ReadDouble();
                        }
                        goto case 0;
                    }
                case 0:
                    {
                        if( version < 9 )
                        {
                            int playerCount = reader.ReadEncodedInt();

                            for( int i = 0; i < playerCount; ++i )
                            {
                                TownPlayerState pl = new TownPlayerState( this, reader );

                                if( pl.Mobile != null )
                                    Timer.DelayCall( TimeSpan.Zero, new TimerCallback( pl.CheckAttach ) );
                            }
                        }
                        break;
                    }
            }
        }

        public virtual void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 11 ); // version

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag( ref flags, SaveFlag.AcceptCitizens, m_AcceptCitizens );
            SetSaveFlag( ref flags, SaveFlag.AccessCost, m_AccessCost > 0 );
            SetSaveFlag( ref flags, SaveFlag.ScalarCost, m_ScalarCost > 0 );
            SetSaveFlag( ref flags, SaveFlag.VendorBuyAllowed, !m_VendorBuyAllowed );
            SetSaveFlag( ref flags, SaveFlag.VendorSellAllowed, !m_VendorSellAllowed );

            SetSaveFlag( ref flags, SaveFlag.PercMercTaxes, m_PercMercTaxes > 0 );
            SetSaveFlag( ref flags, SaveFlag.PercPlayerVendorTaxes, m_PercPlayerVendorTaxes > 0 );
            SetSaveFlag( ref flags, SaveFlag.LandCost, m_LandCost > 0 );
            SetSaveFlag( ref flags, SaveFlag.ServiceAccessCost, m_ServiceAccessCost > 0 );
            SetSaveFlag( ref flags, SaveFlag.AllowNoCitizenResurrection, !m_AllowNoCitizenResurrection );

            SetSaveFlag( ref flags, SaveFlag.AllowCriminalResurrection, !m_AllowCriminalResurrection );
            SetSaveFlag( ref flags, SaveFlag.AllowMurderResurrection, !m_AllowMurderResurrection );
            SetSaveFlag( ref flags, SaveFlag.AllowEnemyResurrection, !m_AllowEnemyResurrection );
            SetSaveFlag( ref flags, SaveFlag.LastIncome, true );
            SetSaveFlag( ref flags, SaveFlag.CommercialStatus, m_CommercialStatus != null );

            SetSaveFlag( ref flags, SaveFlag.Warlord, m_Warlord != null );
            SetSaveFlag( ref flags, SaveFlag.News, !IsEmptyNews );
            SetSaveFlag( ref flags, SaveFlag.ExiledPlayers, ExiledPlayers.Count > 0 );
            SetSaveFlag( ref flags, SaveFlag.PermaExiledPlayers, PermaExiledPlayers.Count > 0 );
            SetSaveFlag( ref flags, SaveFlag.ItemCommercialInfo, ItemPrices.Count > 0 );
            SetSaveFlag( ref flags, SaveFlag.Traps, TownTraps.Count > 0 );

            SetSaveFlag( ref flags, SaveFlag.Treasure, m_RawTownTreasure > 0 );
            SetSaveFlag( ref flags, SaveFlag.Players, Players.Count > 0 );
            SetSaveFlag( ref flags, SaveFlag.Alliances, TownAllies != null && TownAllies.Count > 0 );
            SetSaveFlag( ref flags, SaveFlag.Enemies, TownEnemies != null && TownEnemies.Count > 0 );

            SetSaveFlag( ref flags, SaveFlag.CriminalProfiles, CriminalProfiles != null && CriminalProfiles.Count > 0 );
            SetSaveFlag( ref flags, SaveFlag.VirtueType, Type != VirtueType.Regular );

            writer.Write( (int)flags );

            if( GetSaveFlag( ref flags, SaveFlag.AcceptCitizens ) )
                writer.Write( AcceptCitizens );

            if( GetSaveFlag( ref flags, SaveFlag.AccessCost ) )
                writer.Write( AccessCost );

            if( GetSaveFlag( ref flags, SaveFlag.ScalarCost ) )
                writer.Write( ScalarCost );

            if( GetSaveFlag( ref flags, SaveFlag.VendorBuyAllowed ) )
                writer.Write( VendorBuyAllowed );

            if( GetSaveFlag( ref flags, SaveFlag.VendorSellAllowed ) )
                writer.Write( VendorSellAllowed );

            if( GetSaveFlag( ref flags, SaveFlag.PercMercTaxes ) )
                writer.Write( PercMercTaxes );

            if( GetSaveFlag( ref flags, SaveFlag.PercPlayerVendorTaxes ) )
                writer.Write( PercPlayerVendorTaxes );

            if( GetSaveFlag( ref flags, SaveFlag.LandCost ) )
                writer.Write( LandCost );

            if( GetSaveFlag( ref flags, SaveFlag.ServiceAccessCost ) )
                writer.Write( ServiceAccessCost );

            if( GetSaveFlag( ref flags, SaveFlag.AllowNoCitizenResurrection ) )
                writer.Write( AllowNoCitizenResurrection );

            if( GetSaveFlag( ref flags, SaveFlag.AllowCriminalResurrection ) )
                writer.Write( AllowCriminalResurrection );

            if( GetSaveFlag( ref flags, SaveFlag.AllowMurderResurrection ) )
                writer.Write( AllowMurderResurrection );

            if( GetSaveFlag( ref flags, SaveFlag.AllowEnemyResurrection ) )
                writer.Write( AllowEnemyResurrection );

            if( GetSaveFlag( ref flags, SaveFlag.LastIncome ) )
                writer.Write( LastIncome );

            if( GetSaveFlag( ref flags, SaveFlag.CommercialStatus ) )
                CommercialStatus.Serialize( writer );

            if( GetSaveFlag( ref flags, SaveFlag.Warlord ) )
                writer.WriteMobile( m_Warlord );

            if( GetSaveFlag( ref flags, SaveFlag.News ) )
            {
                int entriesCount = Entries.Count;
                writer.Write( entriesCount );

                for( int i = 0; i < entriesCount; i++ )
                {
                    TownCrierEntry entry = Entries[ i ];

                    writer.Write( entry.Lines.Length );

                    foreach( string t in entry.Lines )
                        writer.Write( t );

                    writer.Write( entry.ExpireTime );
                }
            }

            if( GetSaveFlag( ref flags, SaveFlag.ExiledPlayers ) )
                writer.WriteMobileList( ExiledPlayers );

            if( GetSaveFlag( ref flags, SaveFlag.PermaExiledPlayers ) )
                writer.WriteMobileList( PermaExiledPlayers );

            if( GetSaveFlag( ref flags, SaveFlag.ItemCommercialInfo ) )
            {
                writer.Write( ItemPrices.Count );

                foreach( ItemCommercialInfo t in ItemPrices )
                    t.Serialize( writer );
            }

            if( GetSaveFlag( ref flags, SaveFlag.Traps ) )
            {
                writer.Write( TownTraps.Count );

                foreach( BaseCraftableTrap t in TownTraps )
                    writer.Write( t );
            }

            if( GetSaveFlag( ref flags, SaveFlag.Treasure ) )
                writer.Write( m_RawTownTreasure );

            if( GetSaveFlag( ref flags, SaveFlag.Players ) )
            {
                writer.WriteEncodedInt( Players.Count );

                foreach( TownPlayerState t in Players )
                    t.Serialize( writer );
            }

            if( GetSaveFlag( ref flags, SaveFlag.Alliances ) )
            {
                if( TownAllies != null )
                {
                    writer.WriteEncodedInt( TownAllies.Count );

                    foreach( TownSystem t in TownAllies )
                        WriteReference( writer, t );
                }
                else
                    writer.WriteEncodedInt( 0 );
            }

            if( GetSaveFlag( ref flags, SaveFlag.Enemies ) )
            {
                if( TownEnemies != null )
                {
                    writer.WriteEncodedInt( TownEnemies.Count );

                    foreach( TownSystem t in TownEnemies )
                        WriteReference( writer, t );
                }
                else
                    writer.WriteEncodedInt( 0 );
            }

            if( GetSaveFlag( ref flags, SaveFlag.VirtueType ) )
                writer.WriteEncodedInt( (int)Type );
        }

        public static void WriteReference( GenericWriter writer, TownSystem sys )
        {
            int idx = Array.IndexOf( TownSystems, sys );

            writer.WriteEncodedInt( idx + 1 );
        }

        public static TownSystem ReadReference( GenericReader reader )
        {
            int idx = reader.ReadEncodedInt() - 1;

            if( idx >= 0 && idx < TownSystems.Length )
                return TownSystems[ idx ];

            return null;
        }
        #endregion
    }
}
