/***************************************************************************
 *                                  TownCommercials.cs
 *                            		------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    public enum VendorStatus
    {
        Active,
        Closed,
    }

    public enum BuySellStatus
    {
        BothEnabled,
        SellEnabled,
        BuyEnabled,
        BothDisabled
    }

    public enum RefundModeStatus
    {
        BothAutomatic,
        AutomaticPositive,
        AutomaticNegative,
        BothManual
    }

    [PropertyObject]
    public class TownVendorState
    {
        public static readonly long DefaultPositiveRefundLevel = 25000;
        public static readonly long DefaultNegativeRefundLevel = 25000;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public TownSystem TownSystem { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public Type VendorType { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public long Balance { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public VendorStatus Status { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool ActiveSeller { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool ActiveBuyer { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool AutoPositiveRefund { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool AutoNegativeRefund { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public long PositiveRefundLevel { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public long NegativeRefundLevel { get; set; }

        public List<Mobile> Instances { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsOutOfGold
        {
            get
            {
                return Balance <= NegativeRefundLevel;
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public long AvailableGold
        {
            get
            {
                return Balance - NegativeRefundLevel;
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public BuySellStatus BSStatus
        {
            get
            {
                if( ActiveSeller && ActiveBuyer )
                    return BuySellStatus.BothEnabled;
                else if( ActiveSeller )
                    return BuySellStatus.SellEnabled;
                else if( ActiveBuyer )
                    return BuySellStatus.BuyEnabled;
                else
                    return BuySellStatus.BothDisabled;
            }
            set
            {
                switch( value )
                {
                    case BuySellStatus.BothEnabled:
                        ActiveSeller = true;
                        ActiveBuyer = true;
                        break;
                    case BuySellStatus.SellEnabled:
                        ActiveSeller = true;
                        ActiveBuyer = false;
                        break;
                    case BuySellStatus.BuyEnabled:
                        ActiveSeller = false;
                        ActiveBuyer = true;
                        break;
                    case BuySellStatus.BothDisabled:
                        ActiveSeller = false;
                        ActiveBuyer = false;
                        break;
                    default:
                        break;
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public string BSStatusString
        {
            get
            {
                switch( BSStatus )
                {
                    case BuySellStatus.BothEnabled:
                        return "buy and sells";
                    case BuySellStatus.SellEnabled:
                        return "only sells";
                    case BuySellStatus.BuyEnabled:
                        return "only buys";
                    case BuySellStatus.BothDisabled:
                        return "neither buy nor sells";
                    default:
                        return "";
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public RefundModeStatus RefundMode
        {
            get
            {
                if( AutoPositiveRefund && AutoNegativeRefund )
                    return RefundModeStatus.BothAutomatic;
                else if( AutoPositiveRefund )
                    return RefundModeStatus.AutomaticPositive;
                else if( AutoNegativeRefund )
                    return RefundModeStatus.AutomaticNegative;
                else
                    return RefundModeStatus.BothManual;
            }
            set
            {
                switch( value )
                {
                    case RefundModeStatus.BothAutomatic:
                        AutoPositiveRefund = true;
                        AutoNegativeRefund = true;
                        break;
                    case RefundModeStatus.AutomaticPositive:
                        AutoPositiveRefund = true;
                        AutoNegativeRefund = false;
                        break;
                    case RefundModeStatus.AutomaticNegative:
                        AutoPositiveRefund = false;
                        AutoNegativeRefund = true;
                        break;
                    case RefundModeStatus.BothManual:
                        AutoPositiveRefund = false;
                        AutoNegativeRefund = false;
                        break;
                    default:
                        break;
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public string RefundModeString
        {
            get
            {
                switch( RefundMode )
                {
                    case RefundModeStatus.BothAutomatic:
                        return "refunds positive surplus and negative debts automatically";
                    case RefundModeStatus.AutomaticPositive:
                        return "refunds positive surplus automatically but negative debts manually";
                    case RefundModeStatus.AutomaticNegative:
                        return "refunds negative debts automatically but positive surplus manually";
                    case RefundModeStatus.BothManual:
                        return "refunds positive surplus and negative debts manually";
                    default:
                        return "";
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public double Scalar { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double DefaultScalar
        {
            get
            {
                if( VendorType == null )
                    return 1.0;

                if( TownSystem != null )
                    return TownSystem.GetDefaultScalar( VendorType );

                return 1.0;
            }
        }

        public TownVendorState( TownSystem town, Mobile mobile )
        {
            TownSystem = town;
            VendorType = mobile.GetType();

            Balance = 9999;
            Status = VendorStatus.Active;
            ActiveSeller = true;
            ActiveBuyer = true;
            AutoPositiveRefund = false;
            AutoNegativeRefund = false;
            PositiveRefundLevel = 25000;
            NegativeRefundLevel = -25000;
            Scalar = DefaultScalar;
        }

        public override string ToString()
        {
            return "...";
        }

        public void RegisterInstance( Mobile m )
        {
            TownSystem t = TownSystem.Find( m );
            if( t == null || t != TownSystem )
                return;

            if( Instances == null )
                Instances = new List<Mobile>();

            if( !Instances.Contains( m ) )
                Instances.Add( m );
        }

        public bool RefundDebit( Mobile from, bool message )
        {
            if( Balance >= 0 )
            {
                if( message )
                    from.SendMessage( "This vendor servire does not require any financial help." );
                return false;
            }

            if( TownSystem.RawTownTreasure < Math.Abs( Balance ) )
            {
                if( message )
                    from.SendMessage( "Town treasure cannot affort this debit." );
                return false;
            }

            TownSystem.EditTownTreasure( Balance );
            Balance = 0;

            if( message )
                from.SendMessage( "Balance fot this commercial service is now 0." );
            return true;
        }

        public bool GetGain( Mobile from, bool message )
        {
            if( IsOutOfGold )
            {
                if( message )
                    from.SendMessage( "This vendor servire is out of gold." );
                return false;
            }

            if( Balance <= PositiveRefundLevel )
            {
                if( message )
                    from.SendMessage( "This vendor servire cannot give any gold to the town treasure until it has positive balance of {0} gp.", PositiveRefundLevel );
                return false;
            }

            TownSystem.EditTownTreasure( Balance - PositiveRefundLevel );
            Balance = PositiveRefundLevel;

            if( message )
                from.SendMessage( "Balance fot this commercial service is now {0}.", Balance );

            if( message )
                from.SendMessage( "Balance fot this {0} service is now {1}.", TownSystem.Definition.TownName, TownSystem.TownTreasure );
            return true;
        }

        public string ToStringCompact()
        {
            return string.Format( "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
                VendorType.Name, Balance, Status,
                ActiveSeller, ActiveBuyer,
                AutoPositiveRefund, AutoNegativeRefund,
                PositiveRefundLevel, NegativeRefundLevel, Scalar );
        }

        public bool CheckAccess( bool vendorIsBuying, bool message, Mobile vendor )
        {
            if( Status == VendorStatus.Closed )
            {
                if( message )
                    vendor.Say( true, "I'm out of service" );
                return false;
            }

            if( vendorIsBuying )
            {
                if( !ActiveBuyer )
                {
                    if( message )
                        vendor.Say( true, "I'm not buying anything" );
                    return false;
                }

                if( IsOutOfGold )
                {
                    if( message )
                        vendor.Say( true, "I cannot afford any other financial exposure." );
                    return false;
                }
            }
            else
            {
                if( !ActiveSeller )
                {
                    if( message )
                        vendor.Say( true, "I'm not selling anything" );
                    return false;
                }
            }

            return true;
        }

        public void EvaluateTownFinancialStatus( BaseVendor vendor )
        {
            if( Balance <= NegativeRefundLevel )
            {
                if( AutoNegativeRefund )
                {
                    RefundDebit( null, false );
                }
                else
                {
                    ActiveBuyer = false;
                }
            }
            else if( Balance >= PositiveRefundLevel )
            {
                if( AutoPositiveRefund )
                {
                    GetGain( null, false );
                }
            }
        }

        public void RegisterVendorStatusPurchase( Type type, int amount, int originalPrice, int price )
        {
            int delta = ( price - originalPrice ) * amount;
            Balance = Balance + delta;

            TownLog.Log( LogType.Commercial, String.Format( "Vendor status changed. Type {0} - Amount {1} - Delta {2}.",
                                                          VendorType.Name, amount, delta ) );
        }

        #region serialization
        private static void SetSaveFlag( ref TownVendorFlag flags, TownVendorFlag toSet, bool setIf )
        {
            if( setIf )
                flags |= toSet;
        }

        private static bool GetSaveFlag( TownVendorFlag flags, TownVendorFlag toGet )
        {
            return ( ( flags & toGet ) != 0 );
        }

        [Flags]
        private enum TownVendorFlag
        {
            None = 0x00000000,
            VendorType = 0x00000001,
            Balance = 0x00000002,
            Status = 0x00000004,
            ActiveSeller = 0x00000008,
            ActiveBuyer = 0x00000010,
            AutoPositiveRefund = 0x00000020,
            AutoNegativeRefund = 0x00000040,
            PositiveRefundLevel = 0x00000080,
            NegativeRefundLevel = 0x00000100,
            Scalar = 0x00000200
        }

        public TownVendorState()
        {
        }

        public virtual void Deserialize( TownSystem town, GenericReader reader )
        {
            TownSystem = town;

            int version = reader.ReadEncodedInt();

            TownVendorFlag flags = (TownVendorFlag)reader.ReadEncodedInt();

            if( GetSaveFlag( flags, TownVendorFlag.VendorType ) )
            {
                string typeName = reader.ReadString();
                VendorType = ScriptCompiler.FindTypeByFullName( typeName, false );
            }

            if( GetSaveFlag( flags, TownVendorFlag.Balance ) )
                Balance = reader.ReadLong();
            else
                Balance = 0;

            if( GetSaveFlag( flags, TownVendorFlag.Status ) )
                Status = (VendorStatus)reader.ReadInt();
            else
                Status = VendorStatus.Active;

            if( GetSaveFlag( flags, TownVendorFlag.ActiveSeller ) )
                ActiveSeller = reader.ReadBool();
            else
                ActiveSeller = true;

            if( GetSaveFlag( flags, TownVendorFlag.ActiveBuyer ) )
                ActiveBuyer = reader.ReadBool();
            else
                ActiveBuyer = false;

            if( GetSaveFlag( flags, TownVendorFlag.AutoPositiveRefund ) )
                AutoPositiveRefund = reader.ReadBool();
            else
                AutoPositiveRefund = false;

            if( GetSaveFlag( flags, TownVendorFlag.AutoNegativeRefund ) )
                AutoNegativeRefund = reader.ReadBool();
            else
                AutoNegativeRefund = false;

            if( GetSaveFlag( flags, TownVendorFlag.PositiveRefundLevel ) )
                PositiveRefundLevel = reader.ReadLong();
            else
                PositiveRefundLevel = DefaultPositiveRefundLevel;

            if( GetSaveFlag( flags, TownVendorFlag.NegativeRefundLevel ) )
                NegativeRefundLevel = reader.ReadLong();
            else
                NegativeRefundLevel = DefaultNegativeRefundLevel;

            if( GetSaveFlag( flags, TownVendorFlag.Scalar ) )
                Scalar = reader.ReadDouble();
            else
                Scalar = DefaultScalar;

            if( VendorType == null )
            {
                Config.Pkg.LogWarningLine( "Warning: Vendorstate with null type detected. Removing..." );
            }

            if( version < 1 )
            {
                Balance = 10000;
                BSStatus = BuySellStatus.BothEnabled;
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 1 ); // version

            TownVendorFlag mflags = TownVendorFlag.None;

            SetSaveFlag( ref mflags, TownVendorFlag.VendorType, VendorType != null );
            SetSaveFlag( ref mflags, TownVendorFlag.Balance, Balance != 0 );
            SetSaveFlag( ref mflags, TownVendorFlag.Status, Status != VendorStatus.Active );
            SetSaveFlag( ref mflags, TownVendorFlag.ActiveSeller, ActiveSeller != true );
            SetSaveFlag( ref mflags, TownVendorFlag.ActiveBuyer, ActiveBuyer != false );
            SetSaveFlag( ref mflags, TownVendorFlag.AutoPositiveRefund, AutoPositiveRefund != false );
            SetSaveFlag( ref mflags, TownVendorFlag.AutoNegativeRefund, AutoNegativeRefund != false );
            SetSaveFlag( ref mflags, TownVendorFlag.PositiveRefundLevel, PositiveRefundLevel != DefaultPositiveRefundLevel );
            SetSaveFlag( ref mflags, TownVendorFlag.NegativeRefundLevel, NegativeRefundLevel != DefaultNegativeRefundLevel );
            SetSaveFlag( ref mflags, TownVendorFlag.Scalar, Scalar != DefaultScalar );

            writer.WriteEncodedInt( (int)mflags );

            if( GetSaveFlag( mflags, TownVendorFlag.VendorType ) )
                writer.Write( VendorType == null ? null : VendorType.FullName );

            if( GetSaveFlag( mflags, TownVendorFlag.Balance ) )
                writer.Write( Balance );

            if( GetSaveFlag( mflags, TownVendorFlag.Status ) )
                writer.Write( (int)Status );

            if( GetSaveFlag( mflags, TownVendorFlag.ActiveSeller ) )
                writer.Write( ActiveSeller );

            if( GetSaveFlag( mflags, TownVendorFlag.ActiveBuyer ) )
                writer.Write( ActiveBuyer );

            if( GetSaveFlag( mflags, TownVendorFlag.AutoPositiveRefund ) )
                writer.Write( AutoPositiveRefund );

            if( GetSaveFlag( mflags, TownVendorFlag.AutoNegativeRefund ) )
                writer.Write( AutoNegativeRefund );

            if( GetSaveFlag( mflags, TownVendorFlag.PositiveRefundLevel ) )
                writer.Write( PositiveRefundLevel );

            if( GetSaveFlag( mflags, TownVendorFlag.NegativeRefundLevel ) )
                writer.Write( NegativeRefundLevel );

            if( GetSaveFlag( mflags, TownVendorFlag.Scalar ) )
                writer.Write( Scalar );
        }
        #endregion
    }

    public class TownCommercialStatus
    {
        private TownSystem m_System;

        private Dictionary<Type, TownVendorState> m_CommercialStatus;

        public TownCommercialStatus( TownSystem system )
        {
            m_System = system;
        }

        public Dictionary<Type, TownVendorState> CommercialStatus
        {
            get
            {
                if( m_CommercialStatus == null )
                    m_CommercialStatus = new Dictionary<Type, TownVendorState>();

                return m_CommercialStatus;
            }
            set { m_CommercialStatus = value; }
        }

        private static BuySellStatus m_TempGlobalStatus = BuySellStatus.BothEnabled;

        public void ToggleGlobalVendorStates( Mobile from )
        {
            BuySellStatus status = m_TempGlobalStatus;
            switch( status )
            {
                case BuySellStatus.BothEnabled:
                    m_TempGlobalStatus = BuySellStatus.SellEnabled; break;
                case BuySellStatus.SellEnabled:
                    m_TempGlobalStatus = BuySellStatus.BuyEnabled; break;
                case BuySellStatus.BuyEnabled:
                    m_TempGlobalStatus = BuySellStatus.BothDisabled; break;
                case BuySellStatus.BothDisabled:
                    m_TempGlobalStatus = BuySellStatus.BothEnabled; break;
            }

            from.SendMessage( "All vendors has now this state: {0}", m_TempGlobalStatus );

            foreach( KeyValuePair<Type, TownVendorState> keyValuePair in CommercialStatus )
            {
                keyValuePair.Value.BSStatus = m_TempGlobalStatus;
            }
        }

        public void RegisterVendorStatus( TownVendorState status )
        {
            if( status == null || status.VendorType == null )
                return;

            if( m_CommercialStatus == null )
                m_CommercialStatus = new Dictionary<Type, TownVendorState>();

            if( !m_CommercialStatus.ContainsKey( status.VendorType ) )
            {
                m_CommercialStatus.Add( status.VendorType, status );
                TownLog.Log( LogType.Commercial, String.Format( "VendorStatus Registered. Type {0}.", status.VendorType.Name ) );
            }
            else
                TownLog.Log( LogType.Commercial, String.Format( "Warning: trying to register vendor status already in town commercial status." ) );
        }

        public virtual double GetDefaultScalar( Type vendorType )
        {
            return 1.0;
        }

        public List<Mobile> TownMobiles { get; set; }

        public void RegisterTownVendor( Mobile m )
        {
            if( TownMobiles == null )
                TownMobiles = new List<Mobile>();

            if( !TownMobiles.Contains( m ) )
            {
                TownMobiles.Add( m );
                TownLog.Log( LogType.Mobiles, String.Format( "Mobile Registered. Type {0} Serial {1}.", m.GetType().Name, m.Serial ) );

                TownVendorState state;
                if( !CommercialStatus.ContainsKey( m.GetType() ) )
                {
                    state = new TownVendorState( m_System, m );
                    RegisterVendorStatus( state );
                }
                else
                    CommercialStatus.TryGetValue( m.GetType(), out state );

                if( state != null )
                    state.RegisterInstance( m );
            }
            else
                TownLog.Log( LogType.Traps, String.Format( "Warning: trying to register mobile already in mobile list." ) );
        }

        public string GetDirectionToMobile( Type mobileType, Mobile from )
        {
            if( TownMobiles == null )
                TownMobiles = new List<Mobile>();

            Mobile result = null;
            double distance = -1;

            foreach( Mobile m in TownMobiles )
            {
                if( m.GetType() == mobileType && from.Map == m.Map )
                {
                    double tempDist = from.GetDistanceToSqrt( m );

                    if( distance == -1 || ( tempDist > 0 && tempDist < distance ) )
                    {
                        result = m;
                        distance = tempDist;
                    }
                }
            }

            if( result != null )
            {
                Direction dir = from.GetDirectionTo( result.Location );

                string distString;

                if( distance < 50 )
                    distString = "a short distance to the";
                else if( distance < 200 )
                    distString = "a fair distance to the";
                else
                    distString = "a great distance to the";

                return string.Format( "The nearest {0} is {1} {2} from here.",
                    MidgardUtility.GetFriendlyClassName( mobileType.Name ),
                    distString,
                    MidgardUtility.GetFriendlyDirectionName( dir ) );
            }

            return string.Empty;
        }

        public void GenerateVendors( Mobile from )
        {
            if( from == null || from.Map == Map.Internal )
                return;

            Region r = from.Region.GetRegion( m_System.Definition.StandardRegionName );

            if( r != Map.Felucca.DefaultRegion )
                UOAMVendorGenerator.Parse( from, r );
        }

        public TownVendorState FindVendorState( BaseVendor vendor )
        {
            Type t = vendor.GetType();
            TownVendorState state = null;

            if( CommercialStatus.ContainsKey( t ) )
                CommercialStatus.TryGetValue( t, out state );
            return state;
        }

        public List<TownVendorState> States
        {
            get
            {
                if( m_CommercialStatus == null )
                    return null;

                return new List<TownVendorState>( m_CommercialStatus.Values );
            }
        }

        public TownCommercialStatus( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    {
                        m_System = TownSystem.ReadReference( reader );

                        int stateCount = reader.ReadInt();

                        if( stateCount > 0 )
                        {
                            for( int i = 0; i < stateCount; i++ )
                            {
                                bool stateIsNotNull = reader.ReadBool();

                                if( stateIsNotNull )
                                {
                                    TownVendorState state = new TownVendorState();
                                    state.Deserialize( m_System, reader );
                                }
                            }
                        }
                        break;
                    }
            }
        }

        public void RegisterVendorStatuses()
        {
            if( States != null )
            {
                foreach( TownVendorState state in States )
                {
                    if( state != null )
                        RegisterVendorStatus( state );
                }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( (int)0 ); // version

            TownSystem.WriteReference( writer, m_System );

            writer.Write( CommercialStatus.Count );

            foreach( KeyValuePair<Type, TownVendorState> kvp in CommercialStatus )
            {
                bool notNull = ( kvp.Value != null );

                writer.Write( notNull );
                if( notNull )
                    kvp.Value.Serialize( writer );
            }
        }
    }
}