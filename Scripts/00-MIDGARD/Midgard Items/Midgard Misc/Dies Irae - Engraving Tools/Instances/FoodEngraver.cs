using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class FoodEngraver : BaseEngravingTool
    {
        public override int EngraverTypeLabelNumber
        {
            get { return 1065608; } // food
        }

        public override int EngraverTypeCapLabelNumber
        {
            get { return 1065616; } // Food
        }

        public override int LabelNumber
        {
            get { return 1072951; } // food decoration tool
        }

        public override Type[] Engraves
        {
            get { return m_Engraves; }
        }

        private static readonly Type[] m_Engraves = new Type[]
                       {
                           typeof (Cake), typeof (CheesePizza), typeof (SausagePizza),
                           typeof (Cookies)
                       };

        [Constructable]
        public FoodEngraver()
            : base( 0x1BD1, 1 )
        {
            Weight = 1;
        }

        #region serial-deserial
        public FoodEngraver( Serial serial )
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