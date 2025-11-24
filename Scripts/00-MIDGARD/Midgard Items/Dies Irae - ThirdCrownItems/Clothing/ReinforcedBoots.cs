using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3822 Reinforced Boots - ( nel vendor cuoio e nel menu del craft )
    /// </summary>
    public class ReinforcedBoots : BaseShoes
    {
        public override string DefaultName { get { return "reinforced boots"; } }

        [Constructable]
        public ReinforcedBoots()
            : this( 0 )
        {
        }

        [Constructable]
        public ReinforcedBoots( int hue )
            : base( 0x3822, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public ReinforcedBoots( Serial serial )
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