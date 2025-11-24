// #define DebugAuctionItem

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using Server;
using Server.Accounting;
using Server.Items;
using Server.Mobiles;

using Ultima;

using Skill = Server.Skill;
using StringList = Server.StringList;
using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.AuctionSystem
{
    /// <summary>
    ///     Defines situations for pending situations
    /// </summary>
    public enum AuctionPendency : byte
    {
        /// <summary>
        ///     The user still has to make a decision
        /// </summary>
        Pending = 0,
        /// <summary>
        ///     The user OKs the auction
        /// </summary>
        Accepted = 1,
        /// <summary>
        ///     The user didn't accept the auction
        /// </summary>
        NotAccepted = 2
    }

    /// <summary>
    ///     Defines what happens with the auction item if the auction is ended by the staff
    /// </summary>
    public enum ItemFate
    {
        /// <summary>
        ///     The item is returned to the owner
        /// </summary>
        ReturnToOwner,
        /// <summary>
        ///     The item is taken by the staff
        /// </summary>
        ReturnToStaff,
        /// <summary>
        ///     The item is deleted
        /// </summary>
        Delete
    }

    /// <summary>
    ///     Specifies the type of message that should be dispatched for the buyer or the owner
    /// </summary>
    public enum AuctionMessage : byte
    {
        /// <summary>
        ///     No message should be dispatched
        /// </summary>
        None = 0,
        /// <summary>
        ///     An information message should be dispatched
        /// </summary>
        Information = 1,
        /// <summary>
        ///     A feedback message should be dispatched
        /// </summary>
        Response = 2
    }

    /// <summary>
    ///     An auction entry, holds all the information about a single auction process
    /// </summary>
    public class AuctionItem
    {
        private static readonly StringList m_ClilocList;

        static AuctionItem()
        {
            if( Config.Debug )
                Console.WriteLine( "DEBUG AuctionItem()." );

            string clilocFolder;

            if( !string.IsNullOrEmpty( Config.ClilocLocation ) )
            {
                if( Config.Debug )
                    Console.WriteLine( "\tClilocLocation Value: \"{0}\"", Config.ClilocLocation );

                clilocFolder = Path.GetDirectoryName( Config.ClilocLocation );

                Client.Directories.Insert( 0, clilocFolder );

                if( Config.Debug )
                    Console.WriteLine( "\tClilocFolder set to: \"{0}\"", clilocFolder );
            }
            else
            {
                if( Config.Debug )
                    Console.WriteLine( "\tClilocLocation is NULL, auto-detection..." );

                clilocFolder = Core.FindDataFile( "cliloc.enu" );

                if( clilocFolder != null )
                {
                    Client.Directories.Insert( 0, clilocFolder );
                    if( Config.Debug )
                        Console.WriteLine( "\tClilocFolder set to: \"{0}\"", clilocFolder );
                }
            }

            if( string.IsNullOrEmpty( clilocFolder ) )
                throw new InvalidOperationException( "Error: AuctionItem cliloc location is null." );

            m_ClilocList = StringList.Localization;

            Client.Directories.RemoveAt( 0 );
        }

        /// <summary>
        ///     Creates a new AuctionItem
        /// </summary>
        /// <param name = "item">The item being sold</param>
        /// <param name = "owner">The owner of the item</param>
        public AuctionItem( Item item, Mobile owner )
        {
            Description = "";
            Reserve = 100;
            MinBid = 100;
            ID = Guid.NewGuid();
            SoldAuctionItem = item;
            Owner = owner;
            Bids = new List<Bid>();

            if( !Creature )
            {
                SoldAuctionItem.Visible = false;
                Owner.SendMessage( Config.MessageHue, AuctionSystem.ST[ 172 ] );
            }

            // Item name
            if( !string.IsNullOrEmpty( SoldAuctionItem.Name ) )
                ItemName = SoldAuctionItem.Name;
            else
                ItemName = m_ClilocList.Table[ SoldAuctionItem.LabelNumber ];

            if( SoldAuctionItem.Amount > 1 )
                ItemName = string.Format( "{0} {1}", SoldAuctionItem.Amount.ToString( "#,0" ), ItemName );
        }

        /// <summary>
        ///     Creates an AuctionItem - for use in deserialization
        /// </summary>
        private AuctionItem()
        {
            Description = "";
            Reserve = 100;
            MinBid = 100;
            Bids = new List<Bid>();
        }

        /// <summary>
        /// Gets the item info corresponding to the index value
        /// </summary>
        [IndexerName( "SoldItem" )]
        public ItemInfo this[ int index ]
        {
            get
            {
                if( index > -1 && index < Items.Length )
                    return Items[ index ];
                else
                    return null;
            }
        }

        public XElement ToXElement()
        {
            return new XElement( "auction", new XAttribute( "owner", Utility.SafeString( Owner.Name ?? "" ) ),
                                new XAttribute( "start", StartTime.ToString() ),
                                new XAttribute( "end", EndTime.ToString() ),
                                new XAttribute( "duration", Duration.ToString() ),
                                new XAttribute( "minbid", MinBid.ToString() ),
                                new XAttribute( "desc", Utility.SafeString( Description ?? "" ) ),
                                new XAttribute( "web", Utility.SafeString( WebLink ?? "" ) ),
                                new XAttribute( "pending", Pending ),
                                new XAttribute( "pendingend", PendingEnd ),
                                new XAttribute( "name", Utility.SafeString( ItemName ?? "" ) ),
                                new XElement( "items",
                                             from i in Items
                                             where i != null
                                             select new XElement( "item", new XAttribute( "name", Utility.SafeString( i.Name ?? "" ) ),
                                                                 new XAttribute( "props", Utility.SafeString( i.Properties ?? "" ) ) ) ) );
        }

        /// <summary>
        ///     Gets an html formatted string with all the properies for the item
        /// </summary>
        /// <returns>A string object containing the html structure corresponding to the item properties</returns>
        private static string GetItemProperties( Item item )
        {
            if( item == null || item.PropertyList == null )
                return AuctionSystem.ST[ 78 ];

            if( Core.AOS )
            {
                #region AoS
                ObjectPropertyList plist = new ObjectPropertyList( item );
                item.GetProperties( plist );

                byte[] data = plist.UnderlyingStream.UnderlyingStream.ToArray();
                List<string> list = new List<string>();

                int index = 15; // First localization number index

                while( true )
                {
                    if( index + 4 >= data.Length )
                        break;

                    uint number = (uint)( data[ index++ ] << 24 | data[ index++ ] << 16 | data[ index++ ] << 8 | data[ index++ ] );

                    if( index + 2 > data.Length )
                        break;

                    ushort length = (ushort)( data[ index++ ] << 8 | data[ index++ ] );

                    // Read the string
                    int end = index + length;

                    if( end >= data.Length )
                        end = data.Length - 1;

                    StringBuilder s = new StringBuilder();
                    while( index + 2 <= end + 1 )
                    {
                        short next = (short)( data[ index++ ] | data[ index++ ] << 8 );

                        if( next == 0 )
                            break;

                        s.Append( Encoding.Unicode.GetString( BitConverter.GetBytes( next ) ) );
                    }

                    list.Add( ComputeProperty( (int)number, s.ToString() ) );
                }

                StringBuilder sb = new StringBuilder();
                sb.Append( "<basefont color=#FFFFFF><p>" );

                foreach( string prop in list )
                    sb.AppendFormat( "{0}<br>", prop );

                return sb.ToString();
                #endregion
            }
            else
            {
                #region Non AoS
                StringBuilder sb = new StringBuilder();
                sb.Append( "<basefont color=#FFFFFF><p>" );

                // Get the item name
                if( !string.IsNullOrEmpty( item.Name ) )
                    sb.AppendFormat( "{0}<br>", item.Name );
                else
                    sb.AppendFormat( "{0}<br>", Capitalize( m_ClilocList.Table[ item.LabelNumber ] ) );

                // Amount
                if( item.Amount > 1 )
                    sb.AppendFormat( AuctionSystem.ST[ 152 ], item.Amount.ToString( "#,0" ) );

                // Loot type
                if( item.LootType != LootType.Regular )
                    sb.AppendFormat( "{0}<br>", item.LootType );

                if( item is IUsesRemaining )
                    sb.AppendFormat( AuctionSystem.ST[ 153 ], ( item as IUsesRemaining ).UsesRemaining );

                // Manage item types

                if( item is BaseWand )
                {
                    #region Wands
                    BaseWand bw = item as BaseWand;
                    sb.AppendFormat( AuctionSystem.ST[ 154 ], bw.Effect );
                    sb.AppendFormat( AuctionSystem.ST[ 155 ], bw.Charges );
                    #endregion
                }
                else if( item is BaseArmor )
                {
                    #region Armor
                    BaseArmor ba = item as BaseArmor;

                    if( ba.PlayerConstructed )
                    {
                        if( ba.Crafter != null )
                            sb.AppendFormat( AuctionSystem.ST[ 156 ], ba.Crafter.Name );
                        sb.AppendFormat( AuctionSystem.ST[ 157 ], ba.Resource );
                    }

                    sb.AppendFormat( AuctionSystem.ST[ 158 ], ba.Quality );
                    sb.AppendFormat( AuctionSystem.ST[ 159 ], ba.HitPoints, ba.MaxHitPoints );

                    if( ba.Durability != ArmorDurabilityLevel.Regular )
                        sb.AppendFormat( AuctionSystem.ST[ 160 ], ba.Durability );

                    if( ba.ProtectionLevel != ArmorProtectionLevel.Regular )
                        sb.AppendFormat( AuctionSystem.ST[ 161 ], ba.ProtectionLevel );
                    #endregion
                }
                else if( item is BaseWeapon )
                {
                    #region Weapons
                    BaseWeapon bw = item as BaseWeapon;

                    if( bw.PlayerConstructed )
                    {
                        if( bw.Crafter != null )
                            sb.AppendFormat( AuctionSystem.ST[ 156 ], bw.Crafter.Name );
                        sb.AppendFormat( AuctionSystem.ST[ 157 ], bw.Resource );
                    }

                    sb.AppendFormat( AuctionSystem.ST[ 158 ], bw.Quality );
                    sb.AppendFormat( AuctionSystem.ST[ 159 ], bw.HitPoints, bw.MaxHitPoints );

                    if( bw.PoisonCharges > 0 )
                        sb.AppendFormat( AuctionSystem.ST[ 162 ], bw.PoisonCharges, bw.Poison );

                    if( item is BaseRanged )
                        sb.AppendFormat( AuctionSystem.ST[ 163 ], bw.MaxRange );

                    if( bw.DamageLevel != WeaponDamageLevel.Regular )
                        sb.AppendFormat( AuctionSystem.ST[ 164 ], bw.DamageLevel );

                    if( bw.DurabilityLevel != WeaponDurabilityLevel.Regular )
                        sb.AppendFormat( AuctionSystem.ST[ 160 ], bw.DurabilityLevel );

                    if( bw.AccuracyLevel != WeaponAccuracyLevel.Regular )
                    {
                        if( bw.AccuracyLevel == WeaponAccuracyLevel.Accurate )
                            sb.AppendFormat( AuctionSystem.ST[ 165 ] );
                        else
                            sb.AppendFormat( AuctionSystem.ST[ 166 ], bw.AccuracyLevel );
                    }

                    if( bw.Slayer != SlayerName.None )
                        sb.AppendFormat( AuctionSystem.ST[ 167 ], bw.Slayer );
                    #endregion
                }
                else if( item is TreasureMap )
                {
                    #region Treasure Map
                    TreasureMap tm = item as TreasureMap;
                    sb.AppendFormat( AuctionSystem.ST[ 168 ], tm.ChestMap );
                    #endregion
                }
                else if( item is Spellbook )
                {
                    #region Spellbook
                    Spellbook sp = item as Spellbook;
                    sb.AppendFormat( AuctionSystem.ST[ 169 ], sp.SpellCount );
                    #endregion
                }
                else if( item is PotionKeg )
                {
                    #region Potion Keg
                    PotionKeg pk = item as PotionKeg;

                    int number;

                    if( pk.Held <= 0 )
                        number = 502246; // The keg is empty.
                    else if( pk.Held < 5 )
                        number = 502248; // The keg is nearly empty.
                    else if( pk.Held < 20 )
                        number = 502249; // The keg is not very full.
                    else if( pk.Held < 30 )
                        number = 502250; // The keg is about one quarter full.
                    else if( pk.Held < 40 )
                        number = 502251; // The keg is about one third full.
                    else if( pk.Held < 47 )
                        number = 502252; // The keg is almost half full.
                    else if( pk.Held < 54 )
                        number = 502254; // The keg is approximately half full.
                    else if( pk.Held < 70 )
                        number = 502253; // The keg is more than half full.
                    else if( pk.Held < 80 )
                        number = 502255; // The keg is about three quarters full.
                    else if( pk.Held < 96 )
                        number = 502256; // The keg is very full.
                    else if( pk.Held < 100 )
                        number = 502257; // The liquid is almost to the top of the keg.
                    else
                        number = 502258; // The keg is completely full.

                    sb.AppendFormat( Capitalize( m_ClilocList.Table[ number ] ) );
                    #endregion
                }

                return sb.ToString();
                #endregion
            }
        }

        /// <summary>
        ///     Capitalizes each word in a string
        /// </summary>
        /// <param name = "property">The input string</param>
        /// <returns>The output string </returns>
        private static string Capitalize( string property )
        {
            string[] parts = property.Split( ' ' );
            StringBuilder sb = new StringBuilder();

            for( int i = 0; i < parts.Length; i++ )
            {
                string part = parts[ i ];

                if( part.Length == 0 )
                    continue;

                char c = char.ToUpper( part[ 0 ] );

                part = part.Substring( 1 );
                sb.AppendFormat( "{0}{1}", string.Concat( c, part ), i < parts.Length - 1 ? " " : "" );
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Converts a localization number and its arguments to a valid string
        /// </summary>
        /// <param name = "number">The localization number of the label</param>
        /// <param name = "arguments">The arguments for the label</param>
        /// <returns>The translated string</returns>
        private static string ComputeProperty( int number, string arguments )
        {
            string prop = m_ClilocList.Table[ number ];

            if( prop == null )
                return AuctionSystem.ST[ 170 ];

            if( string.IsNullOrEmpty( arguments ) )
                return Capitalize( prop );

            string[] args = arguments.Split( '\t' );

            try
            {
                return StringList.Localization.Format( number, args );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }

            Regex reg = new Regex( @"~\d+\w*~", RegexOptions.None );
            MatchCollection matches = reg.Matches( prop, 0 );

            if( matches.Count == args.Length )
            {
                // Valid
                for( int i = 0; i < matches.Count; i++ )
                {
                    if( args[ i ].StartsWith( "#" ) )
                    {
                        int loc = -1;

                        try
                        {
                            loc = int.Parse( args[ i ].Substring( 1 ) );
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }

                        if( loc != -1 )
                            args[ i ] = m_ClilocList.Table[ loc ];
                    }

                    Match m = matches[ i ];

                    prop = prop.Replace( m.Value, args[ i ] );
                }

                return Capitalize( prop );
            }
            else
                return AuctionSystem.ST[ 171 ];
        }

        /// <summary>
        ///     Confirms the auction item and adds it into the system
        /// </summary>
        public void Confirm()
        {
            StartTime = DateTime.Now;
            EndTime = StartTime + m_Duration;

            if( Creature && Pet != null )
            {
                Pet.ControlTarget = null;
                Pet.ControlOrder = OrderType.Stay;
                Pet.Internalize();

                Pet.SetControlMaster( null );
                Pet.SummonMaster = null;
            }

            // Calculate all the ItemInfo
            if( SoldAuctionItem is Container && SoldAuctionItem.Items.Count > 0 )
            {
                // Container with items
                Items = new ItemInfo[ SoldAuctionItem.Items.Count ];

                for( int i = 0; i < Items.Length; i++ )
                    Items[ i ] = new ItemInfo( SoldAuctionItem.Items[ i ] );
            }
            else
            {
                Items = new ItemInfo[ 1 ];

                Items[ 0 ] = new ItemInfo( SoldAuctionItem );
            }

            AuctionSystem.Add( this );
            AuctionScheduler.UpdateDeadline( EndTime );

            AuctionLog.WriteNewAuction( this );
        }

        /// <summary>
        ///     Cancels the new auction and returns the item to the owner
        /// </summary>
        public void Cancel()
        {
            if( !Creature )
            {
                SoldAuctionItem.Visible = true;
                Owner.SendMessage( Config.MessageHue, AuctionSystem.ST[ 173 ] );
            }
            else
            {
                ( (MobileStatuette)SoldAuctionItem ).GiveCreatureTo( Owner );
                Owner.SendMessage( Config.MessageHue, AuctionSystem.ST[ 174 ] );
            }
        }

        /// <summary>
        ///     Sends the associated web link to a mobile
        /// </summary>
        /// <param name = "m">The mobile that should receive the web link</param>
        public void SendLinkTo( Mobile m )
        {
            if( m != null && m.NetState != null )
            {
                if( !string.IsNullOrEmpty( m_WebLink ) )
                    m.LaunchBrowser( string.Format( "http://{0}", m_WebLink ) );
            }
        }

        /// <summary>
        ///     Verifies if a mobile can place a bid on this item
        /// </summary>
        /// <param name = "m">The Mobile trying to bid</param>
        /// <returns>True if the mobile is allowed to bid</returns>
        public bool CanBid( Mobile m )
        {
            if( m.AccessLevel > AccessLevel.Player )
                return false; // Staff shoudln't bid. This will also give the bids view to staff members.

            if( Account == ( m.Account as Account ) ) // Same account as auctioneer
                return false;

            if( Creature )
                return ( Pet != null && Pet.CanBeControlledBy( m ) );

            return true;
        }

        /// <summary>
        ///     Verifies if a mobile is the owner of this auction (checks accounts)
        /// </summary>
        /// <param name = "m">The mobile being checked</param>
        /// <returns>True if the mobile is the owner of the auction</returns>
        public bool IsOwner( Mobile m )
        {
            return ( Account == ( m.Account as Account ) );
        }

        /// <summary>
        ///     Places a new bid
        /// </summary>
        /// <param name = "from">The Mobile bidding</param>
        /// <param name = "amount">The bid amount</param>
        /// <returns>True if the bid has been added and accepted</returns>
        public bool PlaceBid( Mobile from, int amount )
        {
            if( !CanBid( from ) )
                return false;

            if( HighestBid != null )
            {
                if( amount <= HighestBid.Amount )
                {
                    from.SendMessage( Config.MessageHue, AuctionSystem.ST[ 176 ] );
                    return false;
                }
            }
            else if( amount <= MinBid )
            {
                from.SendMessage( Config.MessageHue, AuctionSystem.ST[ 177 ] );
                return false;
            }

            int delta;

            if( HighestBid != null )
                delta = amount - HighestBid.Amount;
            else
                delta = amount - MinBid;

            if( BidIncrement > delta )
            {
                from.SendMessage( Config.MessageHue, AuctionSystem.ST[ 204 ], BidIncrement );
                return false;
            }

            // Ok, do bid
            Bid bid = Bid.CreateBid( from, amount );

            if( bid != null )
            {
                if( HighestBid != null )
                    HighestBid.Outbid( this ); // Return money to previous highest bidder

                Bids.Insert( 0, bid );
                AuctionLog.WriteBid( this );

                // Check for auction extension
                if( Config.LateBidExtention > TimeSpan.Zero )
                {
                    TimeSpan timeLeft = EndTime - DateTime.Now;

                    if( timeLeft < TimeSpan.FromMinutes( 5.0 ) )
                    {
                        EndTime += Config.LateBidExtention;
                        bid.Mobile.SendMessage( Config.MessageHue, AuctionSystem.ST[ 230 ] );
                    }
                }
            }

            return bid != null;
        }

        /// <summary>
        ///     Forces the end of the auction when the item or creature has been deleted
        /// </summary>
        public void EndInvalid()
        {
            AuctionSystem.Auctions.Remove( this );

            if( HighestBid != null )
            {
                AuctionGoldCheck gold = new AuctionGoldCheck( this, AuctionResult.ItemDeleted );
                GiveItemTo( HighestBid.Mobile, gold );
            }

            // The item has been deleted, no need to return it to the owner.
            // If it's a statuette, delete it
            if( Creature && SoldAuctionItem != null )
                SoldAuctionItem.Delete();

            AuctionLog.WriteEnd( this, AuctionResult.ItemDeleted, null, null );

            // Over.
        }

        /// <summary>
        ///     Forces the end of the auction and removes it from the system
        /// </summary>
        /// <param name = "m">The staff member deleting the auction</param>
        /// <param name = "itemfate">Specifies what should occur with the item</param>
        public void StaffDelete( Mobile m, ItemFate itemfate )
        {
            if( AuctionSystem.Auctions.Contains( this ) )
                AuctionSystem.Auctions.Remove( this );
            else if( AuctionSystem.Pending.Contains( this ) )
                AuctionSystem.Pending.Remove( this );

            if( HighestBid != null )
            {
                AuctionGoldCheck gold = new AuctionGoldCheck( this, AuctionResult.StaffRemoved );
                GiveItemTo( HighestBid.Mobile, gold );
            }

            AuctionItemCheck check = new AuctionItemCheck( this, AuctionResult.StaffRemoved );
            string comments = null;

            switch( itemfate )
            {
                case ItemFate.Delete:

                    // check.ForceDelete();
                    check.Delete();
                    comments = AuctionSystem.ST[ 239 ]; // "The item has been deleted";
                    break;

                case ItemFate.ReturnToOwner:

                    GiveItemTo( Owner, check );
                    comments = AuctionSystem.ST[ 240 ]; // "The item has been returned to the owner";
                    break;

                case ItemFate.ReturnToStaff:

                    GiveItemTo( m, check );
                    comments = AuctionSystem.ST[ 241 ]; // "The item has been claimed by the staff";
                    break;
            }

            AuctionLog.WriteEnd( this, AuctionResult.StaffRemoved, m, comments );

            // OVer.
        }

        /// <summary>
        ///     Ends the auction.
        ///     This function is called by the auction system during its natural flow
        /// </summary>
        /// <param name = "m">The Mobile eventually forcing the ending</param>
        public void End( Mobile m )
        {
            AuctionSystem.Auctions.Remove( this );

            if( HighestBid == null )
            {
                // No bids, simply return the item
                AuctionCheck item = new AuctionItemCheck( this, AuctionResult.NoBids );
                GiveItemTo( Owner, item );

                // Over, this auction no longer exists
                AuctionLog.WriteEnd( this, AuctionResult.NoBids, m, null );
            }
            else
            {
                // Verify that all items still exist too, otherwise make it pending
                if( IsValid() && ReserveMet )
                {
                    // Auction has been succesful
                    AuctionCheck item = new AuctionItemCheck( this, AuctionResult.Succesful );
                    GiveItemTo( HighestBid.Mobile, item );

                    AuctionCheck gold = new AuctionGoldCheck( this, AuctionResult.Succesful );
                    GiveItemTo( Owner, gold );

                    // Over, this auction no longer exists
                    AuctionLog.WriteEnd( this, AuctionResult.Succesful, m, null );
                }
                else
                {
                    // Reserve hasn't been met or auction isn't valid, this auction is pending
                    Pending = true;
                    PendingEnd = DateTime.Now + TimeSpan.FromDays( Config.DaysForConfirmation );
                    AuctionSystem.Pending.Add( this );

                    DoOwnerMessage();
                    DoBuyerMessage();

                    Mobile owner = GetOnlineMobile( Owner );
                    Mobile buyer = GetOnlineMobile( HighestBid.Mobile );

                    SendMessage( owner );
                    SendMessage( buyer );

                    AuctionScheduler.UpdateDeadline( PendingEnd );

                    AuctionLog.WritePending( this, ReserveMet ? "Item deleted" : "Reserve not met" );
                }
            }
        }

        /// <summary>
        ///     Gets the online mobile belonging to a mobile's account
        /// </summary>
        private static Mobile GetOnlineMobile( Mobile m )
        {
            if( m == null || m.Account == null )
                return null;

            if( m.NetState != null )
                return m;

            Account acc = m.Account as Account;

            for( int i = 0; i < 5; i++ )
            {
                if( acc != null )
                {
                    Mobile mob = acc[ i ];

                    if( mob != null && mob.NetState != null )
                        return mob;
                }
            }

            return null;
        }

        /// <summary>
        ///     Ends the auction.
        ///     This function is called when the system is being disbanded and all auctions must be forced close
        ///     The item will be returned to the original owner, and the highest bidder will receive the money back
        /// </summary>
        public void ForceEnd()
        {
            AuctionSystem.Auctions.Remove( this );

            // Turn the item into a deed and give it to the auction owner
            AuctionCheck item = new AuctionItemCheck( this, AuctionResult.SystemStopped );

            GiveItemTo( Owner, item ); // This in case the item has been wiped or whatever

            if( HighestBid != null )
                HighestBid.AuctionCanceled( this );

            AuctionLog.WriteEnd( this, AuctionResult.SystemStopped, null, null );
        }

        /// <summary>
        ///     This function will put an item in a player's backpack, and if full put it inside their bank.
        ///     If the mobile is null, this will delete the item.
        /// </summary>
        /// <param name = "m">The mobile receiving the item</param>
        /// <param name = "item">The item being given</param>
        private static void GiveItemTo( Mobile m, Item item )
        {
            if( m == null || item == null )
            {
                if( item != null )
                    item.Delete();

                return;
            }

            if( m.Backpack == null || !m.Backpack.TryDropItem( m, item, false ) )
            {
                if( m.BankBox != null )
                    m.BankBox.AddItem( item );
                else
                    item.Delete(); // Sucks to be you
            }
        }

        /// <summary>
        ///     Verifies if all the items being sold through this auction still exist
        /// </summary>
        /// <returns>True if all the items still exist</returns>
        public bool IsValid()
        {
            bool valid = true;

            foreach( ItemInfo info in Items )
            {
                if( info.AItem == null )
                    valid = false;
            }

            return valid;
        }

        /// <summary>
        ///     Defines what kind of message the auction owner should receive. Doesn't send any messages.
        /// </summary>
        public void DoOwnerMessage()
        {
            if( Owner == null || Owner.Account == null )
            {
                // If owner deleted the character, accept the auction by default
                m_OwnerPendency = AuctionPendency.Accepted;
            }
            else if( !IsValid() && ReserveMet )
            {
                // Assume the owner will sell even if invalid when reserve is met
                m_OwnerPendency = AuctionPendency.Accepted;
            }
            else if( !ReserveMet )
            {
                m_OwnerPendency = AuctionPendency.Pending;
                m_OwnerMessage = AuctionMessage.Response; // This is always reserve not met for the owner
            }
            else if( !IsValid() )
            {
                m_OwnerPendency = AuctionPendency.Accepted;
                m_OwnerMessage = AuctionMessage.Information; // This is always about validty for the owner
            }
        }

        /// <summary>
        ///     Defines what kind of message the buyer should receive. Doesn't send any messages.
        /// </summary>
        public void DoBuyerMessage()
        {
            if( HighestBid.Mobile == null || HighestBid.Mobile.Account == null )
            {
                // Buyer deleted the character, accept the auction by default
                m_BuyerPendency = AuctionPendency.Accepted;
            }
            else if( !IsValid() )
            {
                // Send the buyer a message about missing items in the auction
                m_BuyerMessage = AuctionMessage.Response;
                m_BuyerPendency = AuctionPendency.Pending;
            }
            else if( !ReserveMet )
            {
                // Assume the buyer will buy even if the reserve hasn't been met
                m_BuyerPendency = AuctionPendency.Accepted;
                // Send the buyer a message to inform them of the reserve issue
                m_BuyerMessage = AuctionMessage.Information;
            }
        }

        /// <summary>
        ///     Validates the pending status of the auction. This method should be called whenever a pendency
        ///     value is changed. If the auction has been validated, it will finalize items and remove the auction from the system.
        ///     This is the only method that should be used to finalize a pending auction.
        /// </summary>
        public void Validate()
        {
            if( !AuctionSystem.Pending.Contains( this ) )
                return;

            if( m_OwnerPendency == AuctionPendency.Accepted && m_BuyerPendency == AuctionPendency.Accepted )
            {
                // Both parts confirmed the auction
                Pending = false;
                AuctionSystem.Pending.Remove( this );

                AuctionCheck item = new AuctionItemCheck( this, AuctionResult.PendingAccepted );
                AuctionCheck gold = new AuctionGoldCheck( this, AuctionResult.PendingAccepted );

                GiveItemTo( HighestBid.Mobile, item ); // Item to buyer

                GiveItemTo( Owner, gold ); // Gold to owner

                // Over, this auction no longer exists
                AuctionLog.WriteEnd( this, AuctionResult.PendingAccepted, null, null );
            }
            else if( m_OwnerPendency == AuctionPendency.NotAccepted || m_BuyerPendency == AuctionPendency.NotAccepted )
            {
                // At least one part refused
                Pending = false;
                AuctionSystem.Pending.Remove( this );

                AuctionCheck item = new AuctionItemCheck( this, AuctionResult.PendingRefused );
                AuctionCheck gold = new AuctionGoldCheck( this, AuctionResult.PendingRefused );

                GiveItemTo( Owner, item ); // Give item back to owner

                GiveItemTo( HighestBid.Mobile, gold ); // Give gold to highest bidder

                // Over, this auction no longer exists
                AuctionLog.WriteEnd( this, AuctionResult.PendingRefused, null, null );
            }
        }

        /// <summary>
        ///     Sends any message this auction might have in store for a given mobile
        /// </summary>
        /// <param name = "to">The Mobile logging into the server</param>
        public void SendMessage( Mobile to )
        {
            if( !Pending || to == null )
                return;

            if( to == Owner || ( Owner != null && to.Account.Equals( Owner.Account ) ) )
            {
                // This is the owner loggin in
                if( m_OwnerMessage != AuctionMessage.None )
                {
                    // Owner needs a message
                    if( m_OwnerMessage == AuctionMessage.Information )
                    {
                        // Send information message about validity condition
                        AuctionMessaging.SendInvalidMessageToOwner( this );
                    }
                    else if( m_OwnerMessage == AuctionMessage.Response )
                    {
                        // Send reserve not met confirmation request
                        AuctionMessaging.SendReserveMessageToOwner( this );
                    }
                }
            }
            else if( to == HighestBid.Mobile || ( HighestBid.Mobile != null && to.Account.Equals( HighestBid.Mobile.Account ) ) )
            {
                // This is the buyer logging in
                if( m_BuyerMessage != AuctionMessage.None )
                {
                    // Buyer should receive a message
                    if( m_BuyerMessage == AuctionMessage.Information )
                    {
                        // Send message about reserve not met condition
                        AuctionMessaging.SendReserveMessageToBuyer( this );
                    }
                    else if( m_BuyerMessage == AuctionMessage.Response )
                    {
                        // Send request to confirm invalid items auction
                        AuctionMessaging.SendInvalidMessageToBuyer( this );
                    }
                }
            }
        }

        /// <summary>
        ///     Confirms an information message
        /// </summary>
        /// <param name = "owner">True if the message was sent to the owner, false if to the buyer</param>
        public void ConfirmInformationMessage( bool owner )
        {
            if( owner )
            {
                // Owner
                m_OwnerMessage = AuctionMessage.None; // Don't resent
            }
            else
            {
                // Buyer
                m_BuyerMessage = AuctionMessage.None;
            }
        }

        /// <summary>
        ///     Gives a response to a message
        /// </summary>
        /// <param name = "owner">True if the message was sent to the owner, false if to the buyer</param>
        /// <param name = "ok">The response to the message</param>
        public void ConfirmResponseMessage( bool owner, bool ok )
        {
            if( owner )
            {
                if( ok )
                    m_OwnerPendency = AuctionPendency.Accepted;
                else
                    m_OwnerPendency = AuctionPendency.NotAccepted;
            }
            else
            {
                if( ok )
                    m_BuyerPendency = AuctionPendency.Accepted;
                else
                    m_BuyerPendency = AuctionPendency.NotAccepted;
            }

            Validate();
        }

        /// <summary>
        ///     The pending period has timed out and the auction must end unsuccesfully
        /// </summary>
        public void PendingTimeOut()
        {
            AuctionSystem.Pending.Remove( this );

            m_OwnerPendency = AuctionPendency.NotAccepted;
            m_BuyerPendency = AuctionPendency.NotAccepted;
            m_OwnerMessage = AuctionMessage.None;
            m_BuyerMessage = AuctionMessage.None;

            AuctionCheck item = new AuctionItemCheck( this, AuctionResult.PendingTimedOut );
            AuctionCheck gold = new AuctionGoldCheck( this, AuctionResult.PendingTimedOut );

            GiveItemTo( Owner, item );
            GiveItemTo( HighestBid.Mobile, gold );

            // Over, this auction no longer exists
            AuctionLog.WriteEnd( this, AuctionResult.PendingTimedOut, null, null );
        }

        /// <summary>
        ///     Verifies is a mobile has bid on this auction
        /// </summary>
        public bool MobileHasBids( Mobile m )
        {
            List<Bid> bids = new List<Bid>( Bids );

            foreach( Bid bid in bids )
            {
                if( bid.Mobile == m )
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Outputs relevant information about this auction
        /// </summary>
        public void Profile( StreamWriter writer )
        {
            writer.WriteLine( "ID : {0}", ID );
            writer.WriteLine( "Name : {0}", ItemName );

            if( Owner != null && Owner.Account != null )
                writer.WriteLine( "Owner : {0} [ Account {1} - Serial {2} ]", Owner.Name, Owner.Account.Username, Owner.Serial );
            else
                writer.WriteLine( "Owner : no longer existing" );

            writer.WriteLine( "Starting bid: {0}", MinBid );
            writer.WriteLine( "Reserve : {0}", Reserve );

            writer.WriteLine( "Created on {0} at {1}", StartTime.ToShortDateString(), StartTime.ToShortTimeString() );
            writer.WriteLine( "Duration: {0}", m_Duration );
            writer.WriteLine( "End Time: {0} at {1}", EndTime.ToShortDateString(), EndTime.ToShortTimeString() );

            writer.WriteLine( "Expired : {0}", Expired );
            writer.WriteLine( "Pending : {0}", Pending );
            writer.WriteLine( "Next Deadline : {0} at {1}", Deadline.ToShortDateString(), Deadline.ToShortTimeString() );

            writer.WriteLine();

            if( Creature )
            {
                writer.WriteLine( "** This auction is selling a pet" );

                // Pet
                if( SoldAuctionItem != null && Pet != null )
                {
                    writer.WriteLine( "Creature: {0}", Pet.Serial );
                    writer.WriteLine( "Statuette : {0}", SoldAuctionItem.Serial );
                    writer.WriteLine( "Type : {0}", SoldAuctionItem.Name );
                }
                else
                    writer.WriteLine( "Pet deleted, this auction is invalid" );
            }
            else
            {
                // Items
                writer.WriteLine( "{0} Items", Items.Length );

                foreach( ItemInfo item in Items )
                {
                    writer.Write( "- {0}", item.Name );

                    if( item.AItem != null )
                        writer.WriteLine( " [{0}]", item.AItem.Serial );
                    else
                        writer.WriteLine( " [Deleted]" );
                }
            }

            writer.WriteLine();
            writer.WriteLine( "{0} Bids", Bids.Count );

            foreach( Bid bid in Bids )
                bid.Profile( writer );

            writer.WriteLine();
        }

        /// <summary>
        ///     Attempts to buy now
        /// </summary>
        /// <param name = "m">The user trying to purchase</param>
        /// <returns>True if the item has been sold</returns>
        public bool DoBuyNow( Mobile m )
        {
            if( !Banker.Withdraw( m, m_BuyNow ) )
            {
                m.SendMessage( Config.MessageHue, AuctionSystem.ST[ 211 ] );
                return false;
            }

            AuctionSystem.Auctions.Remove( this );

            if( HighestBid != null )
                HighestBid.Outbid( this );

            // Simulate bid
            Bid bid = new Bid( m, BuyNow );
            Bids.Insert( 0, bid );

            AuctionGoldCheck gold = new AuctionGoldCheck( this, AuctionResult.BuyNow );
            AuctionItemCheck item = new AuctionItemCheck( this, AuctionResult.BuyNow );

            GiveItemTo( m, item );
            GiveItemTo( Owner, gold );

            // Over.
            AuctionLog.WriteEnd( this, AuctionResult.BuyNow, m, null );

            return true;
        }

        /// <summary>
        ///     Verifies if the eventual pets in this auction are gone
        /// </summary>
        public void VeirfyIntergrity()
        {
            foreach( ItemInfo ii in Items )
                ii.VeirfyIntegrity();
        }

        #region Variables
        private int m_BuyNow;
        private AuctionMessage m_BuyerMessage = AuctionMessage.None;
        private AuctionPendency m_BuyerPendency = AuctionPendency.Pending;
        private TimeSpan m_Duration = TimeSpan.FromDays( 7 );
        private AuctionMessage m_OwnerMessage = AuctionMessage.None;
        private AuctionPendency m_OwnerPendency = AuctionPendency.Pending;
        private string m_WebLink = "";

        #region Props
        /// <summary>
        /// States whether this auction allows the buy now feature
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public bool AllowBuyNow
        {
            get { return m_BuyNow > 0; }
        }

        /// <summary>
        /// Gets the buy now value
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public int BuyNow
        {
            get { return m_BuyNow; }
            set { m_BuyNow = value; }
        }

        /// <summary>
        /// Gets the date and time corrsponding to the moment when the pending situation will automatically end
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public DateTime PendingEnd { get; private set; }

        /// <summary>
        /// Gets the item being sold at the auction
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public Item SoldAuctionItem { get; private set; }

        /// <summary>
        /// Gets the owner of the item
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public Mobile Owner { get; private set; }

        /// <summary>
        /// Gets the starting time for this auction
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Gets the end time for this auction
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public DateTime EndTime { get; private set; }

        /// <summary>
        ///     Gets the running length of the auction for this item
        /// </summary>
        public TimeSpan Duration
        {
            get { return m_Duration; }
            set
            {
                try
                {
                    m_Duration = value;
                }
                catch
                {
                    m_Duration = TimeSpan.Zero;
                }
            }
        }

        /// <summary>
        /// Gets the time to live left for this auction
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public TimeSpan TimeLeft
        {
            get { return EndTime - DateTime.Now; }
        }

        /// <summary>
        ///     Gets or sets the minimum bid allowed for this item
        /// </summary>
        public int MinBid { get; set; }

        /// <summary>
        ///     Gets or sets the reserve price for the item
        /// </summary>
        public int Reserve { get; set; }

        [CommandProperty( AccessLevel.Administrator )]
        public string Description { get; set; }

        /// <summary>
        /// A web link associated with this auction item
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public string WebLink
        {
            get { return m_WebLink; }
            set
            {
                if( !string.IsNullOrEmpty( value ) )
                {
                    if( value.ToLower().StartsWith( "http://" ) && value.Length > 7 )
                        value = value.Substring( 7 );
                }

                m_WebLink = value;
            }
        }

        /// <summary>
        ///     Gets or sets the list of existing bids
        /// </summary>
        public List<Bid> Bids { get; set; }

        /// <summary>
        /// Gets the account that's selling this item
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public Account Account
        {
            get
            {
                if( Owner != null && Owner.Account != null )
                    return Owner.Account as Account;
                else
                    return null;
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public string ItemName { get; set; }

        /// <summary>
        /// True if the auction is over but the reserve hasn't been met and the owner still haven't decided
        /// if to sell the item or not. This value makes no sense before the auction is over.
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public bool Pending { get; private set; }

        /// <summary>
        ///     Gets the definitions of the items sold
        /// </summary>
        public ItemInfo[] Items { get; private set; }

        /// <summary>
        /// Gets the number of items sold
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public int ItemCount
        {
            get { return Items.Length; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public Guid ID { get; private set; }
        #endregion

        #endregion

        #region Serialization
        /// <summary>
        ///     Saves the auction item into the world file
        /// </summary>
        /// <param name = "writer"></param>
        public void Serialize( GenericWriter writer )
        {
            // Version 1
            writer.Write( m_BuyNow );

            // Version 0
            writer.Write( Owner );
            writer.Write( StartTime );
            writer.Write( m_Duration );
            writer.Write( MinBid );
            writer.Write( Reserve );
            writer.Write( m_Duration );
            writer.Write( Description );
            writer.Write( m_WebLink );
            writer.Write( Pending );
            writer.Write( ItemName );
            writer.Write( SoldAuctionItem );
            writer.Write( ID.ToString() );
            writer.WriteDeltaTime( EndTime );
            writer.Write( (byte)m_OwnerPendency );
            writer.Write( (byte)m_BuyerPendency );
            writer.Write( (byte)m_OwnerMessage );
            writer.Write( (byte)m_BuyerMessage );
            writer.WriteDeltaTime( PendingEnd );

            writer.Write( Items.Length );
            // Items
            foreach( ItemInfo ii in Items )
                ii.Serialize( writer );

            // Bids
            writer.Write( Bids.Count );
            foreach( Bid bid in Bids )
                bid.Serialize( writer );
        }

        /// <summary>
        ///     Loads an <see cref = "AuctionItem" />
        /// </summary>
        /// <returns>An <c>AuctionItem</c></returns>
        public static AuctionItem Deserialize( GenericReader reader, int version )
        {
            AuctionItem auction = new AuctionItem();

            switch( version )
            {
                case 1:
                    auction.m_BuyNow = reader.ReadInt();
                    goto case 0;

                case 0:
                    auction.Owner = reader.ReadMobile();
                    auction.StartTime = reader.ReadDateTime();
                    auction.m_Duration = reader.ReadTimeSpan();
                    auction.MinBid = reader.ReadInt();
                    auction.Reserve = reader.ReadInt();
                    auction.m_Duration = reader.ReadTimeSpan();
                    auction.Description = reader.ReadString();
                    auction.m_WebLink = reader.ReadString();
                    auction.Pending = reader.ReadBool();
                    auction.ItemName = reader.ReadString();
                    auction.SoldAuctionItem = reader.ReadItem();
                    auction.ID = new Guid( reader.ReadString() );
                    auction.EndTime = reader.ReadDeltaTime();
                    auction.m_OwnerPendency = (AuctionPendency)reader.ReadByte();
                    auction.m_BuyerPendency = (AuctionPendency)reader.ReadByte();
                    auction.m_OwnerMessage = (AuctionMessage)reader.ReadByte();
                    auction.m_BuyerMessage = (AuctionMessage)reader.ReadByte();
                    auction.PendingEnd = reader.ReadDeltaTime();

                    int count = reader.ReadInt();
                    auction.Items = new ItemInfo[ count ];

                    for( int i = 0; i < count; i++ )
                        auction.Items[ i ] = ItemInfo.InternalDeserialize( reader, version );

                    count = reader.ReadInt();

                    for( int i = 0; i < count; i++ )
                        auction.Bids.Add( Bid.Deserialize( reader, version ) );
                    break;
            }

            return auction;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the minimum increment required for the auction
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public int BidIncrement
        {
            get
            {
                if( MinBid <= 100 )
                    return 10;

                if( MinBid <= 500 )
                    return 20;

                if( MinBid <= 1000 )
                    return 50;

                if( MinBid <= 5000 )
                    return 100;

                if( MinBid <= 10000 )
                    return 200;

                if( MinBid <= 20000 )
                    return 250;

                if( MinBid <= 50000 )
                    return 500;

                return 1000;
            }
        }

        /// <summary>
        /// States whether an item has at least one bid
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public bool HasBids
        {
            get { return Bids.Count > 0; }
        }

        /// <summary>
        ///     Gets the highest bid for this item
        /// </summary>
        public Bid HighestBid
        {
            get
            {
                if( Bids.Count > 0 )
                    return Bids[ 0 ];
                else
                    return null;
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public Mobile HighestBidder
        {
            get
            {
                if( Bids.Count > 0 )
                    return Bids[ 0 ].Mobile;
                else
                    return null;
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public int HighestBidValue
        {
            get
            {
                if( Bids.Count > 0 )
                    return Bids[ 0 ].Amount;
                else
                    return 0;
            }
        }

        /// <summary>
        /// States whether the reserve has been met for this item
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public bool ReserveMet
        {
            get { return HighestBid != null && HighestBid.Amount >= Reserve; }
        }

        /// <summary>
        /// States whether this auction has expired
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public bool Expired
        {
            get { return DateTime.Now > EndTime; }
        }

        /// <summary>
        ///     Gets the minimum bid that a player can place
        /// </summary>
        public int MinNewBid
        {
            get
            {
                if( HighestBid != null )
                    return HighestBid.Amount;
                else
                    return MinBid;
            }
        }

        /// <summary>
        /// Gets the next deadline required by this auction
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public DateTime Deadline
        {
            get
            {
                if( !Expired )
                    return EndTime;
                else if( Pending )
                    return PendingEnd;
                else
                    return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Specifies if the pending period has timed out
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public bool PendingExpired
        {
            get { return DateTime.Now >= PendingEnd; }
        }

        /// <summary>
        /// Gets the time left before the pending period expired
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public TimeSpan PendingTimeLeft
        {
            get { return PendingEnd - DateTime.Now; }
        }

        /// <summary>
        /// States whether this auction is selling a pet
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public bool Creature
        {
            get { return SoldAuctionItem is MobileStatuette; }
        }

        /// <summary>
        /// Gets the BaseCreature sold through this auction. This will be null when selling an item.
        /// </summary>
        [CommandProperty( AccessLevel.Administrator )]
        public BaseCreature Pet
        {
            get
            {
                if( Creature )
                    return ( (MobileStatuette)SoldAuctionItem ).ShrunkenPet;

                return null;
            }
        }
        #endregion

        #region Nested type: ItemInfo
        public class ItemInfo
        {
            private string m_Name;
            private string m_Props;

            public ItemInfo( Item item )
            {
                AItem = item;

                m_Name = item.Name ?? m_ClilocList.Table[ item.LabelNumber ];

                if( item.Amount > 1 )
                    m_Name = string.Format( "{0} {1}", item.Amount.ToString( "#,0" ), m_Name );

                if( item is MobileStatuette )
                    m_Props = GetCreatureProperties( ( item as MobileStatuette ).ShrunkenPet );
                else
                    m_Props = GetItemProperties( item );
            }

            private ItemInfo()
            {
            }

            public string Name
            {
                get
                {
                    if( AItem != null )
                        return m_Name;
                    return "N/A";
                }
            }

            public Item AItem { get; private set; }

            public string Properties
            {
                get
                {
                    if( AItem != null )
                        return m_Props;
                    return AuctionSystem.ST[ 146 ];
                }
            }

            private static string GetCreatureProperties( BaseCreature creature )
            {
                StringBuilder sb = new StringBuilder();

                sb.Append( "<basefont color=#FFFFFF>" );

                if( creature.Name != null )
                    sb.AppendFormat( "Name : {0}<br", creature.Name );

                sb.AppendFormat( AuctionSystem.ST[ 147 ], creature.ControlSlots );
                sb.AppendFormat( AuctionSystem.ST[ 148 ], creature.IsBondable ? "Yes" : "No" );
                sb.AppendFormat( AuctionSystem.ST[ 149 ], creature.Str );
                sb.AppendFormat( AuctionSystem.ST[ 150 ], creature.Dex );
                sb.AppendFormat( AuctionSystem.ST[ 151 ], creature.Int );

                int index = 0;
                Skill skill;

                while( ( skill = creature.Skills[ index++ ] ) != null )
                {
                    if( skill.Value > 0 )
                        sb.AppendFormat( "{0} : {1}<br>", skill.Name, skill.Value );
                }

                return sb.ToString();
            }

            public void Serialize( GenericWriter writer )
            {
                // Version 1
                // Version 0
                writer.Write( m_Name );
                writer.Write( AItem );
                writer.Write( m_Props );
            }

            public static ItemInfo InternalDeserialize( GenericReader reader, int version )
            {
                ItemInfo item = new ItemInfo();

                switch( version )
                {
                    case 1:
                    case 0:
                        item.m_Name = reader.ReadString();
                        item.AItem = reader.ReadItem();
                        item.m_Props = reader.ReadString();
                        break;
                }

                return item;
            }

            /// <summary>
            ///     Verifies if the mobile referenced by this item is still valid
            /// </summary>
            public void VeirfyIntegrity()
            {
                IShrinkItem shrinkItem = AItem as IShrinkItem;

                if( null != shrinkItem && null == shrinkItem.ShrunkenPet )
                {
                    AItem.Delete();
                    AItem = null; // This will make this item invalid
                }

                if( AItem != null )
                    m_Props = GetItemProperties( AItem );
            }
        }
        #endregion
    }
}