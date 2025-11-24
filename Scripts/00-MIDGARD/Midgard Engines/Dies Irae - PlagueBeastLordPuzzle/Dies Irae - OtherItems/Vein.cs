using Server;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class Vein : Item
    {
        public override int LabelNumber { get { return 1066126; } } // a vein
        public override bool DisplayWeight { get { return false; } }

        private BaseOrgan m_Organ;
        private bool m_IsCut;

        public Vein( BaseOrgan organ, int hue, int offSet )
            : base( 0x1B1B + offSet )
        {
            m_Organ = organ;
            m_IsCut = false;

            Movable = false;
            Hue = hue;
        }

        public Vein( Serial serial )
            : base( serial )
        {
        }

        public void DoCutVein( Mobile from )
        {
            if( !m_IsCut )
            {
                m_IsCut = true;
                ItemID = 0x1F0A;

                BubblyOrgan bo = m_Organ as BubblyOrgan;
                if( bo != null && !bo.Deleted )
                    bo.OnVeinCut( from, Hue );
            }
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_Organ );
            writer.Write( m_IsCut );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Organ = (BaseOrgan)reader.ReadItem();
            m_IsCut = reader.ReadBool();

            if( m_Organ == null )
                Delete();
        }
        #endregion
    }
}