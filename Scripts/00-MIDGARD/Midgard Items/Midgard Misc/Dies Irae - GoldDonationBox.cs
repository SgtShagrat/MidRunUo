/***************************************************************************
 *                               GoldDonationBox.cs
 *
 *   begin                : 04 settembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System.Collections.Generic;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Midgard.Items
{
    public class GoldDonationBox : AddonComponent
    {
        public override string DefaultName
        {
            get { return "a gold donation box"; }
        }

        public override bool Decays
        {
            get { return false; }
        }

        public Dictionary<Mobile, int> Donators { get; set; }

        [Constructable]
        public GoldDonationBox()
            : base( 0x9A8 )
        {
            Donators = new Dictionary<Mobile, int>();
            Hue = 0x576;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int AllGold
        {
            get
            {
                int allGold = 0;
                foreach( KeyValuePair<Mobile, int> kvp in Donators )
                    allGold += kvp.Value;

                return allGold;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TownSystem System { get; set; }

        private bool IsOwner( Mobile from )
        {
            if( from.AccessLevel >= AccessLevel.GameMaster )
                return true;

            BaseHouse house = BaseHouse.FindHouseAt( this );
            if( ( house != null && house.IsOwner( from ) ) )
                return true;

            return System != null && System.HasAccess( TownAccessFlags.CanEditItemPrice, from );
        }

        private bool AddGold( Mobile from, int amount )
        {
            if( from != null && amount > 0 )
            {
                if( Donators.ContainsKey( from ) )
                    Donators[ from ] += amount;
                else
                    Donators.Add( from, amount );

                Emotionate( from );

                return true;
            }

            return false;
        }

        private int GetAllGold()
        {
            int total = 0;

            foreach( KeyValuePair<Mobile, int> donation in Donators )
                total += donation.Value;

            Donators.Clear();

            return total;
        }

        private static void Emotionate( Mobile from )
        {
            from.SendMessage( Utility.RandomMinMax( 2, 600 ), "Thanks for your donation!" );
            from.PlaySound( from.Female ? 823 : 1097 );
            from.PlaySound( 1051 );
            from.Emote( "*{0} donates an item*", from.Name );
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if( from == null || dropped == null || !( from is PlayerMobile ) )
                return false;

            if( dropped is BankCheck )
                return AddGold( (PlayerMobile)from, ( (BankCheck)dropped ).Worth );

            if( dropped is Gold )
                return AddGold( (PlayerMobile)from, ( dropped ).Amount );

            return false;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsOwner( from ) )
                from.SendGump( new OwnerGump( this ) );
            else
                from.SendGump( new ContributorGump( this ) );
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Donations: {0} gold", AllGold );
        }

        #region serialization
        public GoldDonationBox( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( Donators.Count );

            foreach( KeyValuePair<Mobile, int> kvp in Donators )
            {
                writer.Write( kvp.Key );
                writer.Write( kvp.Value );
            }

            TownSystem.WriteReference( writer, System );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            int count = reader.ReadInt();

            Donators = new Dictionary<Mobile, int>();

            for( int i = 0; i < count; i++ )
                Donators.Add( reader.ReadMobile(), reader.ReadInt() );

            System = TownSystem.ReadReference( reader );
        }
        #endregion

        private class ContributorGump : Gump
        {
            public ContributorGump( GoldDonationBox box )
                : base( 0, 0 )
            {

                AddBackground( 0, 0, 280, 300, 9200 );
                AddAlphaRegion( 0, 0, 280, 300 );

                string tmp = string.Empty;
                int i = 0;

                foreach( KeyValuePair<Mobile, int> dons in box.Donators )
                {
                    if( dons.Key != null && dons.Key.Name != null )
                        tmp += ( "<b>" + dons.Key.Name + "</b>    " + dons.Value.ToString( "#,#" ) + "<br />" );
                    if( i++ > 100 )
                        break;
                }

                if( i == 0 )
                    tmp = "(Nobody has donated yet!)";

                AddHtml( 10, 41, 256, 209, tmp, true, true );

                AddBackground( 12, 6, 254, 29, 9200 );
                AddLabel( 89, 10, 0, "Top Contributors" );

                AddBackground( 9, 254, 259, 32, 9350 );
                AddLabel( 16, 260, 0, string.Format( "Total Gold: {0}", box.AllGold.ToString( "#,#" ) ) );
            }
        }

        private class OwnerGump : Gump
        {
            private enum Buttons
            {
                Close = 0,
                WithdrawlButton = 1,
                DonatorListButton = 2
            }

            private readonly GoldDonationBox m_DonationBox;

            public OwnerGump( GoldDonationBox donationBox )
                : base( 0, 0 )
            {
                m_DonationBox = donationBox;

                AddBackground( 0, 0, 266, 141, 9200 );
                AddBackground( 12, 36, 243, 36, 9350 );

                AddLabel( 14, 8, 0, "Your donation barrel" );

                AddLabel( 23, 45, 0, "Total Gold: " + donationBox.AllGold.ToString( "#,#" ) );

                AddButton( 14, 81, 4005, 4007, (int)Buttons.WithdrawlButton, GumpButtonType.Reply, 0 );
                AddLabel( 52, 82, 0, "Withdraw All" );

                AddButton( 14, 107, 4005, 4007, (int)Buttons.DonatorListButton, GumpButtonType.Reply, 0 );
                AddLabel( 52, 108, 0, "View Donator List" );
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                switch( info.ButtonID )
                {
                    default:
                    case (int)Buttons.Close:
                        break;
                    case (int)Buttons.WithdrawlButton:
                        {
                            if( m_DonationBox.AllGold > 0 )
                                sender.Mobile.AddToBackpack( new BankCheck( m_DonationBox.GetAllGold() ) );
                            else
                                sender.Mobile.SendMessage( "There isn't any gold to take out!" );
                            break;
                        }
                    case (int)Buttons.DonatorListButton:
                        {
                            sender.Mobile.SendGump( new ContributorGump( m_DonationBox ) );
                            break;
                        }
                }
            }
        }

        private class GoldDonationBoxAddon : BaseAddon
        {
            public GoldDonationBoxAddon()
            {
                AddComponent( new GoldDonationBox(), 0, 0, 0 );
            }

            public override BaseAddonDeed Deed
            {
                get { return new GoldDonationBoxAddonDeed(); }
            }

            #region serialization
            public GoldDonationBoxAddon( Serial s )
                : base( s )
            {
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.WriteEncodedInt( 0 ); // version
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadEncodedInt();
            }
            #endregion
        }

        public class GoldDonationBoxAddonDeed : BaseAddonDeed
        {
            [Constructable]
            public GoldDonationBoxAddonDeed()
            {
                Name = "a gold donation box addon deed";
            }

            public override BaseAddon Addon
            {
                get { return new GoldDonationBoxAddon(); }
            }

            #region serialization
            public GoldDonationBoxAddonDeed( Serial s )
                : base( s )
            {
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.WriteEncodedInt( 0 ); // version
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadEncodedInt();
            }
            #endregion
        }
    }
}