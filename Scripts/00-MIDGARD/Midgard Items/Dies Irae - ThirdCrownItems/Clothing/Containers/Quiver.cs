using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3827 Quiver trad. faretra - ( nel vendor provisoner, colorabile da hue, container )
    /// </summary>
    public class Quiver : BaseQuiver, IDyable
    {
        public override string DefaultName { get { return "quiver"; } }

        [Constructable]
        public Quiver()
        {
            ItemID = 0x3827;
            ContainerData = ContainerData.GetData( 0xE75 ); // get data from Backpack class
            GumpID = 60;
        }

        public override bool CanEquip( Mobile m )
        {
            // Do not check for ML requirements!
            return true;
        }

        public bool Dye( Mobile from, DyeTub sender )
        {
            if( Deleted )
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        #region serialization
        public Quiver( Serial serial )
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