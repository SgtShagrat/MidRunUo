/***************************************************************************
 *                               VirtualMount.cs
 *
 *   revision             : 03 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Engines.Classes
{
    public class VirtualMount : IMount
    {
        private VirtualMountItem m_Item;

        public VirtualMount( VirtualMountItem item )
        {
            m_Item = item;
        }

        #region IMount Members
        public Mobile Rider
        {
            get { return m_Item.Rider; }
            set { }
        }

        public virtual void OnRiderDamaged( int amount, Mobile from, bool willKill )
        {
        }
        #endregion
    }
}