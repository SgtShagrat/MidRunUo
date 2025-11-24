using Server;
using Server.Network;

namespace Midgard.Items
{
    public class HiveTool : Item
    {
        public override int LabelNumber{ get{ return 1065264; } } // Hive Tool
		
        private int m_UsesRemaining;

        [CommandProperty( AccessLevel.GameMaster )]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        [Constructable]
        public HiveTool() : this( 50 )
        {
        }
		
        [Constructable]
        public HiveTool( int uses ) : base( 2549 )
        {
            m_UsesRemaining = uses;
        }

        public HiveTool( Serial serial ) : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
        }

        public virtual void DisplayDurabilityTo( Mobile m )
        {
            LabelToAffix( m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString() ); // Durability
        }

        public override void OnSingleClick( Mobile from )
        {
            DisplayDurabilityTo( from );

            base.OnSingleClick( from );
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.PrivateOverheadMessage( 0, 1154, 1065366, from.NetState ); // This tool is used to harvest resources from a beehive.
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version

            writer.Write( (int) m_UsesRemaining );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_UsesRemaining = reader.ReadInt();
        }
    }
}