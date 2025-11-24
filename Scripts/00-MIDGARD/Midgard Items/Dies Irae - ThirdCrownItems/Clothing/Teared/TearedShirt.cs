using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x10A8 Teared Shirt - ( no craft, oggetti per Seer o nei loot di scheletri, orchi e liche )
    /// </summary>
    public class TearedShirt : BaseShirt
    {
        public override string DefaultName { get { return "teared shirt"; } }

        [Constructable]
        public TearedShirt()
            : this( 0 )
        {
        }

        [Constructable]
        public TearedShirt( int hue )
            : base( 0x10A8, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public TearedShirt( Serial serial )
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