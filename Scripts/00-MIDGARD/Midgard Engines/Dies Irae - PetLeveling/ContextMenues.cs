/***************************************************************************
 *                                   PetSystemContextMenues.cs
 *                            		---------------------------
 *  begin                	: Febbraio, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;
using Server.ContextMenus;

namespace Midgard.Engines.PetSystem
{
    public class PetMenu : ContextMenuEntry
    {
        #region campi
        private Mobile m_From;
        private BaseCreature m_Bc;
        #endregion

        #region costruttori
        public PetMenu( Mobile from, BaseCreature Bc )
            : base( 5031, 5 )
        {
            m_From = from;
            m_Bc = Bc;
        }
        #endregion

        #region metodi
        public override void OnClick()
        {
            m_From.SendGump( new PetStatGump( m_Bc, m_From ) );
        }
        #endregion
    }

    public class BreedingCancelMenu : ContextMenuEntry
    {
        #region campi
        private Mobile m_From;
        private PetClaimTicket m_Cancel;
        #endregion

        #region costruttori
        public BreedingCancelMenu( Mobile from, PetClaimTicket cancel )
            : base( 0091, 5 )
        {
            m_From = from;
            m_Cancel = cancel;
        }
        #endregion

        #region metodi
        public override void OnClick()
        {
            if( m_From is PlayerMobile )
            {
                BaseCreature bc = (BaseCreature)m_Cancel.Pet;
                PlayerMobile pm = (PlayerMobile)m_From;
                bc.IsStabled = true;
                pm.Stabled.Add( m_Cancel.Pet );

                m_From.SendMessage( "You have canceled breeding, Your pet has been returned to the stable, You may claim it their." );
                m_Cancel.Delete();
            }
        }
        #endregion
    }
}
