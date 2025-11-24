using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2309 Fur Cape - GIA' ESISTENTE - ( inserire solo nel craft del sarto, non colorabile )
    /// </summary>
    public class TraditionalFurCape : FurCape
    {
        public override string DefaultName { get { return "fur cape"; } }

        [Constructable]
        public TraditionalFurCape()
        {
            ItemID = 0x2309;
        }

        public override bool Dye( Mobile from, DyeTub sender )
        {
            from.SendLocalizedMessage( sender.FailMessage );
            return false;
        }

        #region serialization
        public TraditionalFurCape( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}