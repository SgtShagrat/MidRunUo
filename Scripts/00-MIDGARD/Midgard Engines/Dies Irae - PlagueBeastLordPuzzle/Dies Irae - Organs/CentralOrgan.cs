using Server;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class CentralOrgan : BaseOrgan
    {
        private Item m_CuttedTissue;

        [Constructable]
        public CentralOrgan()
            : base( 0x11BA, 1066112, 0x26, BrainTypes.None ) // core nerve center
        {
        }

        #region members
        public override void CreateTissue()
        {
            m_CuttedTissue = new DecoItem( 0x122A, 1066108, 1, false ); // cutted tissue
            PutGuts( m_CuttedTissue, 42, 21 );

            Brain = new MutationCore();
            PutGuts( Brain, 42, 28 );
        }

        public override void OpenOrganTo( Mobile from )
        {
            if( RootParent == from )
            {
                m_CuttedTissue.Visible = true;
                Brain.Visible = true;

                SendLocalizedMessageTo( from, 1066114 ); // "* The core nerve center opens *"
            }
        }
        #endregion

        #region serial-deserial
        public CentralOrgan( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_CuttedTissue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_CuttedTissue = reader.ReadItem();

            if( m_CuttedTissue == null )
                Delete();
        }
        #endregion
    }
}