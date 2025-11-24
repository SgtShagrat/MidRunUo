/***************************************************************************
 *                                  ContextMenu.cs
 *                            		--------------
 *  begin                	: Febbraio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using Server;
using Server.ContextMenus;
using Server.Items;

namespace Midgard.ContextMenus
{
    public class UnLockShrinkItem : ContextMenuEntry
    {
        private Mobile m_From;
        private ShrinkItem m_ShrinkItem;

        public UnLockShrinkItem( Mobile from, ShrinkItem shrink )
            : base( 2033, 5 )
        {
            m_From = from;
            m_ShrinkItem = shrink;
        }

        public override void OnClick()
        {
            if( m_From != m_ShrinkItem.PetOwner )
            {
                m_From.SendLocalizedMessage( 1064355 ); // You do not own this pet.
            }
            else
            {
                m_ShrinkItem.Lock = false;
                m_From.SendLocalizedMessage( 1064356 ); // You have unlocked this shrunken pet, Now anyone can reclaim it as theirs.
            }
        }
    }

    public class LockShrinkItem : ContextMenuEntry
    {
        private Mobile m_From;
        private ShrinkItem m_ShrinkItem;

        public LockShrinkItem( Mobile from, ShrinkItem shrink )
            : base( 2029, 5 )
        {
            m_From = from;
            m_ShrinkItem = shrink;
        }

        public override void OnClick()
        {
            if( m_From != m_ShrinkItem.PetOwner )
            {
                m_From.SendLocalizedMessage( 1064355 ); // You do not own this pet.
            }
            else
            {
                m_ShrinkItem.Lock = true;
                m_From.SendLocalizedMessage( 1064357 ); // You have locked the pet so only you can reclaim it.
            }
        }
    }
}
