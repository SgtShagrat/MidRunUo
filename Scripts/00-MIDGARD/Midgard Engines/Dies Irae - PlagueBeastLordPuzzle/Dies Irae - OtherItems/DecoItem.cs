using Server;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class DecoItem : Item
    {
        private int m_LabelNumber;

        public override int LabelNumber { get { return m_LabelNumber; } }
        public override bool DisplayWeight { get { return false; } }

        public DecoItem( int itemID, object name, int hue )
            : this( itemID, name, hue, false, true )
        {
        }

        public DecoItem( int itemID, object name, int hue, bool isRealDeco )
            : this( itemID, name, hue, false, false )
        {
        }

        public DecoItem( int itemID, object name, int hue, bool movable, bool visible )
            : base( itemID )
        {
            if( name is int )
                m_LabelNumber = (int)name;
            else if( name is string )
                Name = (string)name;

            Movable = movable;
            Visible = visible;
            Hue = hue;

            Weight = 1;
        }

        public DecoItem( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (int)m_LabelNumber );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_LabelNumber = reader.ReadInt();
        }
        #endregion
    }
}