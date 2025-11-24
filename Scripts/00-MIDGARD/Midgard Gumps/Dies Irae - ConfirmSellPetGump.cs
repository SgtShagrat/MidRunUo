using Server;
using Server.Gumps;
using Server.Mobiles;

namespace Midgard.Gumps
{
    public class ConfirmSellPetGump : Gump
    {
        private AnimalTrainer m_Receiver;
        private Point3D m_Location;
        private BaseCreature m_Pet;

        private enum Buttons
        {
            Cancel,
            Continue,
        }

        public ConfirmSellPetGump( AnimalTrainer receiver, Point3D location, BaseCreature pet )
            : base( 50, 50 )
        {
            m_Receiver = receiver;
            m_Location = location;
            m_Pet = pet;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );
            AddBackground( 0, 0, 270, 120, 0x13BE );

            int price = receiver.GetSellPriceFor( pet );

            string message = string.Format( "<div align=center>Are you sure you wish to transfer this pet away, with no possibility of recovery for {0} gold?</div>", price );
            AddOldHtml( 10, 10, 250, 75, message );

            AddHtmlLocalized( 55, 90, 75, 20, 1011011, 0x0, false, false ); // CONTINUE
            AddHtmlLocalized( 170, 90, 75, 20, 1011012, 0x0, false, false ); // CANCEL

            AddButton( 20, 90, 0xFA5, 0xFA7, (int)Buttons.Continue, GumpButtonType.Reply, 0 );
            AddButton( 135, 90, 0xFA5, 0xFA7, (int)Buttons.Cancel, GumpButtonType.Reply, 0 );
        }

        public override void OnResponse( Server.Network.NetState state, RelayInfo info )
        {
            if( m_Receiver == null || m_Pet == null || m_Pet.Deleted )
                return;

            if( !state.Mobile.InRange( m_Location, 2 ) )
            {
                state.Mobile.SendMessage( "Thou are too far away from that animal trainer." );
                return;
            }

            if( m_Pet.ControlMaster != state.Mobile )
            {
                state.Mobile.SendMessage( "You don't own that animal." );
                return;
            }

            if( info.ButtonID == (int)Buttons.Continue && state.Mobile is PlayerMobile )
                m_Receiver.SellPet( state.Mobile, m_Pet );
        }
    }
}