using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Accounting;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.AuctionSystem
{
    /// <summary>
    ///     The main auction system process
    /// </summary>
    public class AuctionSystem
    {
        #region Variables
        /// <summary>
        ///     Text provider for the auction system
        /// </summary>
        private static StringTable m_StringTable;
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the String Table for the auction system
        /// </summary>
        public static StringTable ST
        {
            get
            {
                if( m_StringTable == null )
                    m_StringTable = new StringTable();
                return m_StringTable;
            }
        }

        /// <summary>
        ///     Gets or sets the auction control stone
        /// </summary>
        public static AuctionControl ControlStone { get; set; }

        /// <summary>
        ///     Gets the listing of the current auctions
        /// </summary>
        public static List<AuctionItem> Auctions
        {
            get
            {
                if( ControlStone != null )
                    return ControlStone.Auctions;
                else
                    return null;
            }
        }

        /// <summary>
        ///     Gets the listing of pending auctions (ended but reserve not met)
        /// </summary>
        public static List<AuctionItem> Pending
        {
            get
            {
                return ControlStone != null ? ControlStone.Pending : null;
            }
        }

        /// <summary>
        ///     Get the max number of auctions for a single account
        /// </summary>
        private static int MaxAuctions
        {
            get
            {
                return ControlStone == null ? 0 : ControlStone.MaxAuctionsParAccount;
            }
        }

        /// <summary>
        ///     Gets the min number of days an auction can last
        /// </summary>
        public static int MinAuctionDays
        {
            get { return ControlStone.MinAuctionDays; }
        }

        /// <summary>
        ///     Gets the max number of days an auction can last
        /// </summary>
        public static int MaxAuctionDays
        {
            get { return ControlStone.MaxAuctionDays; }
        }

        /// <summary>
        ///     States whether the auction system is functional or not
        /// </summary>
        public static bool Running
        {
            get { return ControlStone != null; }
        }
        #endregion

        #region Auction Managment
        /// <summary>
        ///     Adds an auction into the system
        /// </summary>
        /// <param name = "auction">The new auction entry</param>
        public static void Add( AuctionItem auction )
        {
            // Put the item into the control stone
            auction.SoldAuctionItem.Internalize();

            // m_ControlStone.AddItem( auction.SoldAuctionItem );
            auction.SoldAuctionItem.Parent = ControlStone;
            auction.SoldAuctionItem.Visible = true;

            Auctions.Add( auction );

            ControlStone.InvalidateProperties();
        }

        /// <summary>
        ///     Requests the start of a new auction
        /// </summary>
        /// <param name = "mobile">The mobile requesting the auction</param>
        public static void AuctionRequest( Mobile mobile )
        {
            if( CanAuction( mobile ) )
            {
                mobile.SendMessage( Config.MessageHue, ST[ 191 ] );
                mobile.CloseAllGumps();
                mobile.Target = new AuctionTarget( new AuctionTargetCallback( OnNewAuctionTarget ), -1, false );
            }
            else
            {
                mobile.SendMessage( Config.MessageHue, ST[ 192 ], MaxAuctions );
                mobile.SendGump( new AuctionGump( mobile ) );
            }
        }

        private static void OnCreatureAuction( Mobile from, BaseCreature creature )
        {
            MobileStatuette ms = MobileStatuette.Create( from, creature );

            if( ms == null )
                from.Target = new AuctionTarget( new AuctionTargetCallback( OnNewAuctionTarget ), -1, false );

            /*
             * Pets are auctioned within an item (MobileStatuette)
             * 
             * The item's name is the type of the pet, the hue corresponds to the pet
             * and the item id is retrieved from the shrink table.
             * 
             */

            AuctionItem auction = new AuctionItem( ms, from );
            from.SendGump( new NewAuctionGump( from, auction ) );
        }

        private static void OnNewAuctionTarget( Mobile from, object targeted )
        {
            Item item = targeted as Item;
            BaseCreature bc = targeted as BaseCreature;

            if( item == null && !Config.AllowPetsAuction )
            {
                // Can't auction pets and target it invalid
                from.SendMessage( Config.MessageHue, ST[ 193 ] );
                from.Target = new AuctionTarget( new AuctionTargetCallback( OnNewAuctionTarget ), -1, false );
                return;
            }

            if( bc != null )
            {
                // Auctioning a pet
                OnCreatureAuction( from, bc );
                return;
            }

            if( !CheckItem( item ) )
            {
                from.SendMessage( Config.MessageHue, ST[ 194 ] );
                from.Target = new AuctionTarget( new AuctionTargetCallback( OnNewAuctionTarget ), -1, false );
                return;
            }

            if( !CheckIdentified( from, item ) )
            {
                from.SendMessage( Config.MessageHue, ST[ 195 ] );
                from.Target = new AuctionTarget( new AuctionTargetCallback( OnNewAuctionTarget ), -1, false );
                return;
            }

            if( item != null )
            {
                if( !item.Movable )
                {
                    from.SendMessage( Config.MessageHue, ST[ 205 ] );
                    from.Target = new AuctionTarget( new AuctionTargetCallback( OnNewAuctionTarget ), -1, false );
                    return;
                }
            }

            bool ok = true;

            if( item is Container )
            {
                foreach( Item sub in item.Items )
                {
                    if( !CheckItem( sub ) )
                    {
                        from.SendMessage( Config.MessageHue, ST[ 196 ] );
                        ok = false;
                        break;
                    }

                    if( !sub.Movable )
                    {
                        from.SendMessage( Config.MessageHue, ST[ 205 ] );
                        ok = false;
                        break;
                    }

                    if( sub is Container && sub.Items.Count > 0 )
                    {
                        ok = false;
                        from.SendMessage( Config.MessageHue, ST[ 197 ] );
                        break;
                    }
                }
            }

            if( item != null )
            {
                if( !( item.IsChildOf( from.Backpack ) || item.IsChildOf( from.BankBox ) ) )
                {
                    from.SendMessage( Config.MessageHue, ST[ 198 ] );
                    ok = false;
                }
            }

            if( !ok )
                from.Target = new AuctionTarget( new AuctionTargetCallback( OnNewAuctionTarget ), -1, false );
            else
            {
                // Item ok, start auction creation
                AuctionItem auction = new AuctionItem( item, from );

                from.SendGump( new NewAuctionGump( from, auction ) );
            }
        }

        /// <summary>
        ///     Verifies if an item can be sold through the auction
        /// </summary>
        /// <param name = "item">The item being sold</param>
        /// <returns>True if the item is allowed</returns>
        private static bool CheckItem( Item item )
        {
            foreach( Type t in Config.ForbiddenTypes )
            {
                if( t == item.GetType() )
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     This check is intended for non-AOS only. It verifies whether the item has an Identified
        ///     property and in that case it it's set to true.
        /// </summary>
        /// <param name="from"></param>
        /// <param name = "item">The item being evaluated</param>
        /// <remarks>
        ///     This method always returns true if Core.AOS is set to true.
        /// </remarks>
        /// <returns>True if the item can be auctioned, false otherwise</returns>
        private static bool CheckIdentified( Mobile from, Item item )
        {
            if( Core.AOS )
                return true;

            if( !( item is IIdentificable ) )
                return true;

            IIdentificable ident = item as IIdentificable;
            bool identified = ident.IsIdentifiedFor( from );

            if( identified && item.Items.Count > 0 )
            {
                foreach( Item child in item.Items )
                {
                    if( !CheckIdentified( from, child ) )
                    {
                        identified = false;
                        break;
                    }
                }
            }

            /*
            PropertyInfo prop = item.GetType().GetProperty( "Identified" ); // Do not translate this!

            if( prop == null )
                return true;

            bool identified = true;

            try
            {
                identified = (bool)prop.GetValue( item, null );
            }
            catch( Exception ex ) // Possibly there's an Identified property whose value is not bool - allow auction of this
            {
                Console.WriteLine( ex.ToString() );
            }
            */

            if( identified && item.Items.Count > 0 )
            {
                foreach( Item child in item.Items )
                {
                    if( !CheckIdentified( from, child ) )
                    {
                        identified = false;
                        break;
                    }
                }
            }

            return identified;
        }

        /// <summary>
        ///     Removes the auction system from the server. All auctions will end unsuccesfully.
        /// </summary>
        /// <param name = "m">The mobile terminating the system</param>
        public static void ForceDelete( Mobile m )
        {
            Console.WriteLine( "Auction system terminated on {0} at {1} by {2} ({3}, Account: {4})", DateTime.Now.ToShortDateString(),
                              DateTime.Now.ToShortTimeString(), m.Name, m.Serial, ( (Account)m.Account ).Username );

            while( Auctions.Count > 0 || Pending.Count > 0 )
            {
                while( Auctions.Count > 0 )
                    Auctions[ 0 ].ForceEnd();

                while( Pending.Count > 0 )
                    Pending[ 0 ].ForceEnd();
            }

            ControlStone.ForceDelete();
            ControlStone = null;
        }

        /// <summary>
        ///     Finds an auction through its id
        /// </summary>
        /// <param name = "id">The GUID identifying the auction</param>
        /// <returns>An <see cref = "AuctionItem" /> object if the speicifies auction is still in the system</returns>
        public static AuctionItem Find( Guid id )
        {
            if( !Running )
                return null;

            foreach( AuctionItem item in Pending )
            {
                if( item.ID == id )
                    return item;
            }

            foreach( AuctionItem item in Auctions )
            {
                if( item.ID == id )
                    return item;
            }

            return null;
        }

        /// <summary>
        ///     Gets the auctions created by a player
        /// </summary>
        /// <param name = "m">The player requesting the auctions</param>
        public static List<AuctionItem> GetAuctions( Mobile m )
        {
            List<AuctionItem> auctions = new List<AuctionItem>();

            try
            {
                foreach( AuctionItem auction in Auctions )
                {
                    if( auction.Owner == m || ( auction.Owner != null && m.Account.Equals( auction.Owner.Account ) ) )
                        auctions.Add( auction );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }

            return auctions;
        }

        /// <summary>
        ///     Gets the list of auctions a mobile has bids on
        /// </summary>
        public static List<AuctionItem> GetBids( Mobile m )
        {
            List<AuctionItem> bids = new List<AuctionItem>();

            try
            {
                foreach( AuctionItem auction in Auctions )
                {
                    if( auction.MobileHasBids( m ) )
                        bids.Add( auction );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }

            return bids;
        }

        /// <summary>
        ///     Gets the list of pendencies for a mobile
        /// </summary>
        public static List<AuctionItem> GetPendencies( Mobile m )
        {
            List<AuctionItem> list = new List<AuctionItem>();

            try
            {
                foreach( AuctionItem auction in Pending )
                {
                    if( auction.Owner == m || ( auction.Owner != null && m.Account.Equals( auction.Owner.Account ) ) )
                        list.Add( auction );
                    else if( auction.HighestBid.Mobile == m ||
                             ( auction.HighestBid.Mobile != null && m.Account.Equals( auction.HighestBid.Mobile.Account ) ) )
                        list.Add( auction );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }

            return list;
        }
        #endregion

        #region Checks
        /// <summary>
        ///     Verifies if a mobile can create a new auction
        /// </summary>
        /// <param name = "m">The mobile trying to create an auction</param>
        /// <returns>True if allowed</returns>
        public static bool CanAuction( Mobile m )
        {
            if( m.AccessLevel >= AccessLevel.GameMaster ) // Staff can always auction
                return true;

            int count = 0;

            foreach( AuctionItem auction in Auctions )
            {
                if( auction.Account == ( m.Account as Account ) )
                    count++;
            }

            return count < MaxAuctions;
        }
        #endregion

        #region Scheduling
        public static void Initialize()
        {
            try
            {
                if( Running )
                {
                    VerifyIntegrity();
                    VerifyAuctions();
                    VerifyPendencies();
                }
            }
            catch( Exception err )
            {
                ControlStone = null;

                Console.WriteLine( "An error occurred when initializing the Auction System. The system has been temporarily disabled." );
                Console.WriteLine( "Error details: {0}", err );
            }
        }

        public static void OnDeadlineReached()
        {
            if( !Running )
                return;

            VerifyAuctions();
            VerifyPendencies();
        }

        /// <summary>
        ///     Verifies whether any pets in current auctions have been deleted
        /// </summary>
        private static void VerifyIntegrity()
        {
            foreach( AuctionItem auction in Auctions )
                auction.VeirfyIntergrity();
        }

        /// <summary>
        ///     Verifies current auctions ending the ones that expired
        /// </summary>
        public static void VerifyAuctions()
        {
            lock( World.Items )
            {
                lock( World.Mobiles )
                {
                    if( !Running )
                        return;

                    List<AuctionItem> list = new List<AuctionItem>();
                    List<AuctionItem> invalid = new List<AuctionItem>();

                    foreach( AuctionItem auction in Auctions )
                    {
                        if( auction.SoldAuctionItem == null || ( auction.Creature && auction.Pet == null ) )
                            invalid.Add( auction );
                        else if( auction.Expired )
                            list.Add( auction );
                    }

                    foreach( AuctionItem inv in invalid )
                        inv.EndInvalid();

                    foreach( AuctionItem expired in list )
                        expired.End( null );
                }
            }
        }

        /// <summary>
        ///     Verifies pending auctions ending the ones that expired
        /// </summary>
        public static void VerifyPendencies()
        {
            lock( World.Items )
            {
                lock( World.Mobiles )
                {
                    if( !Running )
                        return;

                    List<AuctionItem> list = new List<AuctionItem>();

                    foreach( AuctionItem auction in Pending )
                    {
                        if( auction.PendingExpired )
                            list.Add( auction );
                    }

                    foreach( AuctionItem expired in list )
                        expired.PendingTimeOut();
                }
            }
        }

        /// <summary>
        ///     Disables the system until the next reboot
        /// </summary>
        public static void Disable()
        {
            ControlStone = null;
            AuctionScheduler.Stop();
        }
        #endregion

        /// <summary>
        ///     Outputs all relevant auction data to a text file
        /// </summary>
        public static void ProfileAuctions()
        {
            string file = Path.Combine( Core.BaseDirectory, "AuctionProfile.txt" );

            try
            {
                StreamWriter sw = new StreamWriter( file, false );

                sw.WriteLine( "Auction System Profile" );
                sw.WriteLine( "{0}", DateTime.Now.ToLongDateString() );
                sw.WriteLine( "{0}", DateTime.Now.ToShortTimeString() );
                sw.WriteLine( "{0} Running Auctions", Auctions.Count );
                sw.WriteLine( "{0} Pending Auctions", Pending.Count );
                sw.WriteLine( "Next Deadline : {0} at {1}", AuctionScheduler.Deadline.ToShortDateString(),
                             AuctionScheduler.Deadline.ToShortTimeString() );

                sw.WriteLine();
                sw.WriteLine( "Auctions List" );
                sw.WriteLine();

                List<AuctionItem> auctions = new List<AuctionItem>( Auctions );

                foreach( AuctionItem a in auctions )
                {
                    if( a.SoldAuctionItem != null )
                    {
                        a.SoldAuctionItem.Location = ControlStone.Location;
                        a.SoldAuctionItem.Internalize();
                        ControlStone.AddItem( a.SoldAuctionItem );
                    }
                }

                foreach( AuctionItem a in auctions )
                    a.Profile( sw );

                sw.WriteLine( "Pending Auctions List" );
                sw.WriteLine();

                List<AuctionItem> pending = new List<AuctionItem>( Pending );

                foreach( AuctionItem p in pending )
                    p.Profile( sw );

                sw.WriteLine( "End of profile" );
                sw.Close();
            }
            catch( Exception err )
            {
                Console.WriteLine( "Couldn't output auction profile. Error: {0}", err );
            }
        }
    }
}