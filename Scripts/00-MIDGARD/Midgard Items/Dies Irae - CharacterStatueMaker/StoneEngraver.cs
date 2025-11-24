using System;

using Server;

namespace Midgard.Items.StatueSystem
{
    public class StoneEngraver : BaseEngravingTool
    {
        public override string EngraverTypeLabel
        {
            get { return "Stone"; }
        }

        public override string DefaultName
        {
            get { return "stone decoration tool"; }
        }

        protected override bool AllowNonLocal
        {
            get { return true; }
        }

        public override Type[] Engraves
        {
            get { return m_Engraves; }
        }

        private static readonly Type[] m_Engraves = new Type[]
                                                        {
                                                            typeof(MidgardStatuePlinth)
                                                        };

        [Constructable]
        public StoneEngraver()
            : base( 0x12B3, 1 )
        {
            Weight = 1.0;
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, "a stone engraving tool" );
        }

        #region serial-deserial
        public StoneEngraver( Serial serial )
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