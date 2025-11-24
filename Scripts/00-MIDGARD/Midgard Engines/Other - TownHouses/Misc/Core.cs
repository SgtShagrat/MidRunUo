using System;
using System.Collections.Generic;
using Server;
using Server.Multis;

namespace Midgard.Engines.TownHouses
{
    public class Core
    {
        public static void ConfigSystem()
        {
            EventSink.WorldSave += new WorldSaveEventHandler( OnSave );
        }

        public static void InitSystem()
        {
            EventSink.Login += new LoginEventHandler( OnLogin );

            if( Config.SpeechEventEnabled )
                EventSink.Speech += new SpeechEventHandler( HandleSpeech );

            EventSink.ServerStarted += new ServerStartedEventHandler( OnStarted );
        }

        private static void OnStarted()
        {
            if( Config.Debug )
                Console.Write( "Verifying town houses..." );

            foreach( TownHouse house in TownHouse.AllTownHouses )
            {
                house.InitSectorDefinition();
                RegionHelper.UpdateRegion( house.ForSaleSign );
            }

            if( Config.Debug )
                Console.Write( "done\n" );
        }

        public static void OnSave( WorldSaveEventArgs e )
        {
            if( Config.Debug )
                Console.Write( "Saving town houses..." );

            WorldSaveProfiler.Instance.StartHandlerProfile( OnSave );

            foreach( TownHouseSign sign in TownHouseSign.AllSigns )
                sign.ValidateOwnership();

            foreach( TownHouse house in TownHouse.AllTownHouses )
                if( house.Deleted )
                {
                    TownHouse.AllTownHouses.Remove( house );
                    continue;
                }

            WorldSaveProfiler.Instance.EndHandlerProfile();

            if( Config.Debug )
                Console.Write( "done\n" );
        }

        private static void OnLogin( LoginEventArgs e )
        {
            foreach( BaseHouse house in BaseHouse.GetHouses( e.Mobile ) )
                if( house is TownHouse )
                    ( (TownHouse)house ).ForSaleSign.CheckDemolishTimer();
        }

        private static void HandleSpeech( SpeechEventArgs e )
        {
            List<BaseHouse> houses = BaseHouse.GetHouses( e.Mobile );

            if( houses == null )
                return;

            foreach( BaseHouse house in houses )
            {
                if( !RegionHelper.RegionContains( house.Region, e.Mobile ) )
                    continue;

                if( house is TownHouse )
                    house.OnSpeech( e );

                if( house.Owner == e.Mobile
                 && e.Speech.ToLower() == "create rental contract"
                 && CanRent( e.Mobile, house, true ) )
                {
                    e.Mobile.AddToBackpack( new RentalContract() );
                    e.Mobile.SendMessage( "A rental contract has been placed in your bag." );
                }

                if( house.Owner == e.Mobile
                 && e.Speech.ToLower() == "check storage" )
                {
                    int count;

                    e.Mobile.SendMessage( "You have {0} lockdowns and {1} secures available.", RemainingSecures( house ), RemainingLocks( house ) );

                    if( ( count = AllRentalLocks( house ) ) != 0 )
                        e.Mobile.SendMessage( "Current rentals are using {0} of your lockdowns.", count );
                    if( ( count = AllRentalSecures( house ) ) != 0 )
                        e.Mobile.SendMessage( "Current rentals are using {0} of your secures.", count );
                }
            }
        }

        private static bool CanRent( Mobile m, BaseHouse house, bool say )
        {
            if( house is TownHouse && ( (TownHouse)house ).ForSaleSign.PriceType != "Sale" )
            {
                if( say )
                    m.SendMessage( "You must own your property to rent it." );

                return false;
            }

            if( Config.RequireRenterLicense )
            {
                RentalLicense lic = m.Backpack.FindItemByType( typeof( RentalLicense ) ) as RentalLicense;

                if( lic != null && lic.Owner == null )
                    lic.Owner = m;

                if( lic == null || lic.Owner != m )
                {
                    if( say )
                        m.SendMessage( "You must have a renter's license to rent your property." );

                    return false;
                }
            }

            if( EntireHouseContracted( house ) )
            {
                if( say )
                    m.SendMessage( "This entire house already has a rental contract." );

                return false;
            }

            if( RemainingSecures( house ) < 0 || RemainingLocks( house ) < 0 )
            {
                if( say )
                    m.SendMessage( "You don't have the storage available to rent property." );

                return false;
            }

            return true;
        }

        #region Rental Info

        public static bool EntireHouseContracted( BaseHouse house )
        {
            foreach( TownHouseSign item in TownHouseSign.AllSigns )
                if( item is RentalContract && house == ( (RentalContract)item ).ParentHouse )
                    if( ( (RentalContract)item ).EntireHouse )
                        return true;

            return false;
        }

        public static bool HasContract( BaseHouse house )
        {
            foreach( TownHouseSign item in TownHouseSign.AllSigns )
                if( item is RentalContract && house == ( (RentalContract)item ).ParentHouse )
                    return true;

            return false;
        }

        public static bool HasOtherContract( BaseHouse house, RentalContract contract )
        {
            foreach( TownHouseSign item in TownHouseSign.AllSigns )
                if( item is RentalContract && item != contract && house == ( (RentalContract)item ).ParentHouse )
                    return true;

            return false;
        }

        public static int RemainingSecures( BaseHouse house )
        {
            if( house == null )
                return 0;

            int a, b, c, d;

            return ( Server.Core.AOS ? house.GetAosMaxSecures() - house.GetAosCurSecures( out a, out b, out c, out d ) : house.MaxSecures - house.SecureCount ) - AllRentalSecures( house );
        }

        public static int RemainingLocks( BaseHouse house )
        {
            if( house == null )
                return 0;

            return ( Server.Core.AOS ? house.GetAosMaxLockdowns() - house.GetAosCurLockdowns() : house.MaxLockDowns - house.LockDownCount ) - AllRentalLocks( house );
        }

        public static int AllRentalSecures( BaseHouse house )
        {
            int count = 0;

            foreach( TownHouseSign sign in TownHouseSign.AllSigns )
                if( sign is RentalContract && ( (RentalContract)sign ).ParentHouse == house )
                    count += sign.Secures;

            return count;
        }

        public static int AllRentalLocks( BaseHouse house )
        {
            int count = 0;

            foreach( TownHouseSign sign in TownHouseSign.AllSigns )
                if( sign is RentalContract && ( (RentalContract)sign ).ParentHouse == house )
                    count += sign.Locks;

            return count;
        }

        #endregion
    }
}