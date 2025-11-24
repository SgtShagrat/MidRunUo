using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x10AB Ian Platemail arms - ( no craft, oggetto unico di Sire Ian )
    /// </summary>
    public class IanPlatemailArms : PlateArms
    {
        public override string DefaultName { get { return "ian platemail arms"; } }

        [Constructable]
        public IanPlatemailArms()
            : this( 0 )
        {
        }

        [Constructable]
        public IanPlatemailArms( int hue )
        {
            ItemID = 0x10AB;
            Hue = hue;
            LootType = LootType.Blessed;
        }

        #region serialization
        public IanPlatemailArms( Serial serial )
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