using Server;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseGlasses : BaseArmor
    {
        public override int OldStrReq { get { return 40; } }
        public override int ArmorBase { get { return 10; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Glass; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }
        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

        protected BaseGlasses( int itemID )
            : base( itemID )
        {
        }

        #region serialization
        public BaseGlasses( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version			
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}