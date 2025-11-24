using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x10B5 Teared Pants - ( no craft, oggetti per Seer o nei loot di scheletri, orchi e liche )
    /// </summary>
    public class TearedPants : BasePants
    {
        public override string DefaultName { get { return "teared pants"; } }

        [Constructable]
        public TearedPants()
            : this( 0 )
        {
        }

        [Constructable]
        public TearedPants( int hue )
            : base( 0x10B5, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public TearedPants( Serial serial )
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