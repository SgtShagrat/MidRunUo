using Midgard.Engines.Classes;

using Server;

namespace Midgard.Items
{
    public class BagOfDruidRitualItems : BaseBagOfRitualItems
    {
        public override ClassSystem System
        {
            get { return ClassSystem.Druid; }
        }

        [Constructable]
        public BagOfDruidRitualItems()
            : this( 10 )
        {
        }

        [Constructable]
        public BagOfDruidRitualItems( int amount )
            : base( amount )
        {
        }

        #region serialization
        public BagOfDruidRitualItems( Serial serial )
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