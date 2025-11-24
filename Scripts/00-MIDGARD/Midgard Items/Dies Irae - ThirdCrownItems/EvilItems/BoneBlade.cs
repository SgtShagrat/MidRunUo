using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3357 Bone Blade - ( craftabile solo da karma negativo ) 
    /// </summary>
    public class BoneBlade : Cutlass
    {
        public override string DefaultName { get { return "bone blade"; } }

        [Constructable]
        public BoneBlade()
        {
            ItemID = 0x3357;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor || from.Karma < 0;
        }

        #region serialization
        public BoneBlade( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}