using Server.Targeting;

namespace Server.Items
{
    /*
        Queste classi derivano tutte da quelle standard di runuo e le estendono in modo
    da implementare il piazzamento case di midgard.

    I file originali sono modificati solo per poter ereditare queste classi ma funzionano
    ancora alla maniera vecchia se questo file viene eliminato.
    */

    /*	public class MidgardHousePlacementCategoryGump : HousePlacementCategoryGump
        {
            private HouseCommittmentDeed m_Deed;
            private Mobile m_Buyer;
            private BaseVendor m_Vendor;
            private ArrayList m_List;

            public MidgardHousePlacementCategoryGump( HouseCommittmentDeed deed, Mobile buyer, BaseVendor vendor, ArrayList list ) : base( buyer ) 
            { 
                m_Deed = deed;
                m_Buyer = buyer;
                m_Vendor = vendor;
                m_List = list;
            }

            public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
            {
                if ( !m_Buyer.CheckAlive() )
                    return;

                switch ( info.ButtonID )
                {
                    case 1: // Classic Houses
                    {
                        m_Buyer.SendGump( new MidgardHousePlacementListGump( m_Deed, m_Buyer, m_Vendor, m_List, MidgardHousePlacementEntry.ClassicHouses ) );
                        break;
                    }
                    case 2: // 2-Story Customizable Houses
                    {
                        m_Buyer.SendGump( new MidgardHousePlacementListGump( m_Deed, m_Buyer, m_Vendor, m_List, MidgardHousePlacementEntry.TwoStoryFoundations ) );
                        break;
                    }
                    case 3: // 3-Story Customizable Houses
                    {
                        m_Buyer.SendGump( new MidgardHousePlacementListGump( m_Deed, m_Buyer, m_Vendor, m_List, MidgardHousePlacementEntry.ThreeStoryFoundations ) );
                        break;
                    }
                }
            }
        }
    */

    /*
    Questo è il secondo menu di scelta della casa

    La differenza con l'HousePlacementListGump standard è che i dati interni tengono
    traccia del vendor e del deed che si sta comprando.

    Quando il PG sceglie la casa viene personalizzato il deed e generato un MidgardHousePlacementTool che
    permetterà di piazzare la casa 

    TODO:
    - cambiare il menu in modo che mostri tutti i materiali richiesti
    - 
    */

    /*	public class MidgardHousePlacementListGump : HousePlacementListGump
        {
            private HouseCommittmentDeed m_Deed;
            private Mobile m_Buyer;
            private BaseVendor m_Vendor;
            private ArrayList m_List;
            private MidgardHousePlacementEntry[] m_Entries;
		
            public MidgardHousePlacementListGump( HouseCommittmentDeed deed, Mobile buyer, BaseVendor vendor, ArrayList list, MidgardHousePlacementEntry[] entries ) : base( buyer, entries )
            {
                m_Deed = deed;
                m_Buyer = buyer;
                m_Vendor = vendor;
                m_List = list;
                m_Entries = entries;
            }

            public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
            {
                if ( !m_Buyer.CheckAlive() )
                    return;

                int index = info.ButtonID - 1;

                if ( index == 0 || index >= m_Entries.Length )
                {
                    m_Buyer.SendGump( new MidgardHousePlacementCategoryGump( m_Deed, m_Buyer, m_Vendor, m_List ) );
                    return;
                }
			
                if ( m_Buyer.AccessLevel < AccessLevel.GameMaster && BaseHouse.HasAccountHouse( m_Buyer ) )
                    m_Buyer.SendLocalizedMessage( 501271 ); // You already own a house, you may not place another!
                else
                {
                    MidgardHousePlacementEntry entry = m_Entries[index];
                    MidgardHousePlacementTool tool = new MidgardHousePlacementTool(entry);
                    m_Deed.AddReward(tool);
				
                    // Committente e commesso vanno settati qui e non prima
                    // per evitare che il deed venga personalizzato anche se il PG
                    // alla fine non compra niente
                    m_Deed.Committant = m_Vendor;
                    m_Deed.Committed = m_Buyer;
				
                    // TODO: non è bellissimo fare riferimento qui ai tipi di materiali richiesti, ma per ora può andare
                    // Si può spostare tutta la parte di compilazione del deed in un MidgardHousePlacementEntry.FillDeed ad esempio
                    m_Deed.AddRequirement(typeof(Log),entry.Logs);
                    m_Deed.AddRequirement(typeof(IronIngot),entry.Ingots);
				
                    // TODO:
                    // - bisogna modificare il tutto in modo che vengano accettati anche soldi 
                    //	magari come check, oppure si setta Gold come ICommodity che sarebbe la soluzione
                    //	più pulita ma permetterebbe di fare dei CommodityDeed coi soldi e magari non vogliamo.
                    //	In tal caso basta levare il commento qui sotto
                    //m_Deed.AddRequirement(typeof(Gold),entry.Cost);
				
                    // finalizza l'acquisto
                    // TODO: mettere tutti i check del caso per controllare che il PG
                    // sia ancora vivo, non sia andato a spasso, se è GM ecc ...
                    m_Vendor.OnBuyItems(m_Buyer, m_List);
                }
            }
        }
    */

    /*public class MidgardHousePlacementTarget : MultiTarget
    {
        private HouseCommittmentDeed m_Deed;

        public MidgardHousePlacementTarget( HouseCommittmentDeed deed )
            : base( deed.MultiID, deed.Offset )
        {
            Range = 14;
            m_Deed = deed;
        }

        protected override void OnTarget( Mobile from, object o )
        {
            if( !from.CheckAlive() || from.Backpack == null || m_Deed.RootParent != from )
                return;

            IPoint3D ip = o as IPoint3D;

            if( ip == null )
                return;

            if( ip is Item )
                ip = ( (Item)ip ).GetWorldTop();

            Point3D p = new Point3D( ip );

            if( from.AccessLevel < AccessLevel.Seer && !Midgard.Engines.MidgardTownSystem.TownHelper.CanBuildHouseOnLocation( p, from ) )
            {
                from.SendMessage( "You cannot build any house here." );
                return;
            }

            Region reg = Region.Find( new Point3D( p ), from.Map );

            if( from.AccessLevel >= AccessLevel.GameMaster || reg.AllowHousing( from, p ) )
            {
                HousePlacementEntry e = new HousePlacementEntry( m_Deed.Type,
                                        m_Deed.Description,
                                        m_Deed.Storage,
                                        m_Deed.Lockdowns,
                                        m_Deed.NewStorage,
                                        m_Deed.NewLockdowns,
                                        m_Deed.Vendors,
                                        m_Deed.Cost, // VendorPrice è già stato pagato all'acquisto del deed 
                                        m_Deed.XOffset,
                                        m_Deed.YOffset,
                                        m_Deed.ZOffset,
                                        m_Deed.MultiID );

                #region mod by Dies Irae
                
                //if( e.OnPlacement( from, p ) )
                //    m_Deed.Delete();
                //else
                //    from.SendMessage( "You cannot place it there" );
                 

                if( !e.OnPlacement( from, p, m_Deed ) )
                    from.SendMessage( "You cannot place it there" );
                #endregion

            }
            else if( reg.IsPartOf( typeof( TreasureRegion ) ) )
                from.SendLocalizedMessage( 1043287 ); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
            else
                from.SendLocalizedMessage( 501265 ); // Housing can not be created in this area.
        }
    }*/
}