using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis;

namespace Midgard.Engines.TownHouses
{
    public class RentalContract : TownHouseSign
    {
        private Mobile m_RentalClient;

        public BaseHouse ParentHouse { get; private set; }
        public Mobile RentalClient { get { return m_RentalClient; } set { m_RentalClient = value; InvalidateProperties(); } }
        public Mobile RentalMaster { get; private set; }
        public bool Completed { get; set; }
        public bool EntireHouse { get; set; }

        public RentalContract()
        {
            ItemID = 0x14F0;
            Movable = true;
            RentByTime = TimeSpan.FromDays( 1 );
            RecurRent = true;
            MaxZ = MinZ;
        }

        public bool HasContractedArea( Rectangle2D rect, int z )
        {
            foreach( TownHouseSign item in AllSigns )
                if( item is RentalContract && item != this && item.Map == Map && ParentHouse == ( (RentalContract)item ).ParentHouse )
                    foreach( Rectangle2D rect2 in item.Blocks )
                        for( int x = rect.Start.X; x < rect.End.X; ++x )
                            for( int y = rect.Start.Y; y < rect.End.Y; ++y )
                                if( rect2.Contains( new Point2D( x, y ) ) )
                                    if( item.MinZ <= z && item.MaxZ >= z )
                                        return true;

            return false;
        }

        public bool HasContractedArea( int z )
        {
            foreach( TownHouseSign item in AllSigns )
                if( item is RentalContract && item != this && item.Map == Map && ParentHouse == ( (RentalContract)item ).ParentHouse )
                    if( item.MinZ <= z && item.MaxZ >= z )
                        return true;

            return false;
        }

        public void DepositTo( Mobile m )
        {
            if( m == null )
                return;

            if( Free )
            {
                m.SendMessage( "Since this home is free, you do not receive the deposit." );
                return;
            }

            m.BankBox.DropItem( new Gold( Price ) );
            m.SendMessage( "You have received a {0} gold deposit from your town house.", Price );
        }

        public override void ValidateOwnership()
        {
            if( Completed && RentalMaster == null )
            {
                Delete();
                return;
            }

            if( m_RentalClient != null && ( ParentHouse == null || ParentHouse.Deleted ) )
            {
                Delete();
                return;
            }

            if( m_RentalClient != null && !Owned )
            {
                Delete();
                return;
            }

            if( ParentHouse == null )
                return;

            if( !ValidateLocSec() )
            {
                if( DemolishTimer == null )
                    BeginDemolishTimer( TimeSpan.FromHours( 48 ) );
            }
            else
                ClearDemolishTimer();
        }

        protected override void DemolishAlert()
        {
            if( ParentHouse == null || RentalMaster == null || m_RentalClient == null )
                return;

            RentalMaster.SendMessage( "You have begun to use lockdowns reserved for {0}, and their rental unit will collapse in {1}.", m_RentalClient.Name, Math.Round( ( DemolishTime - DateTime.Now ).TotalHours, 2 ) );
            m_RentalClient.SendMessage( "Alert your land lord, {0}, they are using storage reserved for you.  They have violated the rental agreement, which will end in {1} if nothing is done.", RentalMaster.Name, Math.Round( ( DemolishTime - DateTime.Now ).TotalHours, 2 ) );
        }

        public void FixLocSec()
        {
            int count;

            if( ( count = Core.RemainingSecures( ParentHouse ) + Secures ) < Secures )
                Secures = count;

            if( ( count = Core.RemainingLocks( ParentHouse ) + Locks ) < Locks )
                Locks = count;
        }

        public bool ValidateLocSec()
        {
            if( Core.RemainingSecures( ParentHouse ) + Secures < Secures )
                return false;

            if( Core.RemainingLocks( ParentHouse ) + Locks < Locks )
                return false;

            return true;
        }

        public override void ConvertItems( bool keep )
        {
            if( House == null || ParentHouse == null || RentalMaster == null )
                return;

            foreach( BaseDoor door in new ArrayList( ParentHouse.Doors ) )
                if( door.Map == House.Map && House.Region.Contains( door.Location ) )
                    ConvertDoor( door );

            foreach( SecureInfo info in new ArrayList( ParentHouse.Secures ) )
                if( info.Item.Map == House.Map && House.Region.Contains( info.Item.Location ) )
                    ParentHouse.Release( RentalMaster, info.Item );

            foreach( Item item in new ArrayList( ParentHouse.LockDowns ) )
                if( item.Map == House.Map && House.Region.Contains( item.Location ) )
                    ParentHouse.Release( RentalMaster, item );
        }

        public override void UnconvertDoors()
        {
            if( House == null || ParentHouse == null )
                return;

            foreach( BaseDoor door in new ArrayList( House.Doors ) )
                House.Doors.Remove( door );
        }

        protected override void OnRentPaid()
        {
            if( RentalMaster == null || m_RentalClient == null )
                return;

            if( Free )
                return;

            RentalMaster.BankBox.DropItem( new Gold( Price ) );
            RentalMaster.SendMessage( "The bank has transfered your rent from {0}.", m_RentalClient.Name );
        }

        public override void ClearHouse()
        {
            if( !Deleted )
                Delete();

            base.ClearHouse();
        }

        public override void OnDoubleClick( Mobile m )
        {
            ValidateOwnership();

            if( Deleted )
                return;

            if( RentalMaster == null )
                RentalMaster = m;

            BaseHouse house = BaseHouse.FindHouseAt( m );

            if( ParentHouse == null )
                ParentHouse = house;

            if( house == null || ( house != ParentHouse && house != House ) )
            {
                m.SendMessage( "You must be in the home to view this contract." );
                return;
            }

            if( m == RentalMaster
             && !Completed
             && house is TownHouse
             && ( (TownHouse)house ).ForSaleSign.PriceType != "Sale" )
            {
                ParentHouse = null;
                m.SendMessage( "You can only rent property you own." );
                return;
            }

            if( m == RentalMaster && !Completed && Core.EntireHouseContracted( ParentHouse ) )
            {
                m.SendMessage( "This entire house already has a rental contract." );
                return;
            }

            if( Completed )
                new ContractConfirmGump( m, this );
            else if( m == RentalMaster )
                new ContractSetupGump( m, this );
            else
                m.SendMessage( "This rental contract has not yet been completed." );
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            if( m_RentalClient != null )
                list.Add( "a house rental contract with " + m_RentalClient.Name );
            else if( Completed )
                list.Add( "a completed house rental contract" );
            else
                list.Add( "an uncompleted house rental contract" );
        }

        public override void Delete()
        {
            if( ParentHouse == null )
            {
                base.Delete();
                return;
            }

            if( !Owned && !ParentHouse.IsFriend( m_RentalClient ) )
            {
                if( m_RentalClient != null && RentalMaster != null )
                {
                    RentalMaster.SendMessage( "{0} has ended your rental agreement.  Because you revoked their access, their last payment will be refunded.", RentalMaster.Name );
                    m_RentalClient.SendMessage( "You have ended your rental agreement with {0}.  Because your access was revoked, your last payment is refunded.", m_RentalClient.Name );
                }

                DepositTo( m_RentalClient );
            }
            else if( Owned )
            {
                if( m_RentalClient != null && RentalMaster != null )
                {
                    m_RentalClient.SendMessage( "{0} has ended your rental agreement.  Since they broke the contract, your are refunded the last payment.", RentalMaster.Name );
                    RentalMaster.SendMessage( "You have ended your rental agreement with {0}.  They will be refunded their last payment.", m_RentalClient.Name );
                }

                DepositTo( m_RentalClient );

                PackUpHouse();
            }
            else
            {
                if( m_RentalClient != null && RentalMaster != null )
                {
                    RentalMaster.SendMessage( "{0} has ended your rental agreement.", m_RentalClient.Name );
                    m_RentalClient.SendMessage( "You have ended your rental agreement with {0}.", RentalMaster.Name );
                }

                DepositTo( RentalMaster );
            }

            ClearRentTimer();
            base.Delete();
        }

        #region serialization
        public RentalContract( Serial serial )
            : base( serial )
        {
            RecurRent = true;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version

            // Version 1

            writer.Write( EntireHouse );

            writer.Write( RentalMaster );
            writer.Write( m_RentalClient );
            writer.Write( ParentHouse );
            writer.Write( Completed );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( version >= 1 )
                EntireHouse = reader.ReadBool();

            RentalMaster = reader.ReadMobile();
            m_RentalClient = reader.ReadMobile();
            ParentHouse = reader.ReadItem() as BaseHouse;
            Completed = reader.ReadBool();
        }
        #endregion
    }
}
