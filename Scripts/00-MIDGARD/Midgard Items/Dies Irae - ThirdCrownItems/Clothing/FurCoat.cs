using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2676 Fur Coat trad. pelliccia - ( craft sarto e loot orchi , colorabile )
    /// </summary>
    public class FurCoat : BaseOuterTorso
    {
        public override string DefaultName { get { return "fur coat"; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        [Constructable]
        public FurCoat()
            : this( 0 )
        {
        }

        [Constructable]
        public FurCoat( int hue )
            : base( 0x2676, hue )
        {
            Weight = 1.0;
        }

        public override bool CanBeCraftedWith( Type t )
        {
            return t is IFurLeather;
        }

        #region serialization
        public FurCoat( Serial serial )
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