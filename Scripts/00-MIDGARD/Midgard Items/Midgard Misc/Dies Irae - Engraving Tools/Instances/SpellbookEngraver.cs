using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class SpellbookEngraver : BaseEngravingTool
    {
        public override int EngraverTypeLabelNumber
        {
            get { return 1065602; } // spellbook
        }

        public override int EngraverTypeCapLabelNumber
        {
            get { return 1065610; } // Spellbook
        }

        public override Type[] Engraves
        {
            get { return m_Engraves; }
        }

        private static readonly Type[] m_Engraves = new Type[]
                       {
                           typeof (Spellbook)
                       };

        [Constructable]
        public SpellbookEngraver()
            : base( 0xFBF, 1 )
        {
        }

        #region serial-deserial
        public SpellbookEngraver( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}