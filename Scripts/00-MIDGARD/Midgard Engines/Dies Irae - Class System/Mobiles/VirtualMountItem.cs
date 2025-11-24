/***************************************************************************
 *                               VirtualMountItem.cs
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
    public class VirtualMountItem : Item, IMountItem
    {
        private VirtualMount m_Mount;
        private Mobile m_Rider;

        public VirtualMountItem( Mobile mob )
            : base( 0x3EA0 )
        {
            Layer = Layer.Mount;

            m_Rider = mob;
            m_Mount = new VirtualMount( this );
        }

        public VirtualMountItem( Serial serial )
            : base( serial )
        {
            m_Mount = new VirtualMount( this );
        }

        public Mobile Rider
        {
            get { return m_Rider; }
        }

        #region IMountItem Members
        public IMount Mount
        {
            get { return m_Mount; }
        }
        #endregion

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (Mobile)m_Rider );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Rider = reader.ReadMobile();

            if( m_Rider == null )
                Delete();
        }
    }
}