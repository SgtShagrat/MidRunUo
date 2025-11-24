using Server;
using Server.Engines.XmlSpawner2;

namespace Midgard.Engines.RandomEncounterSystem
{
    public class XmlDateCount : XmlDate
    {
        private int m_Count;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Count
        {
            get { return m_Count; }
            set { m_Count = value; }
        }

        public XmlDateCount( ASerial serial )
            : base( serial )
        {
        }

        [Attachable]
        public XmlDateCount( string name )
            : base( name )
        {
            m_Count = 0;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
            writer.Write( m_Count );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
            m_Count = reader.ReadInt();
        }
    }
}