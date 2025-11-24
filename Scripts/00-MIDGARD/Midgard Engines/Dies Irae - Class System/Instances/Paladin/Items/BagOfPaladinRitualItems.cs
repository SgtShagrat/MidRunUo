using Midgard.Engines.Classes;

using Server;

namespace Midgard.Items
{
    public class BagOfPaladinRitualItems : BaseBagOfRitualItems
    {
        [Constructable]
        public BagOfPaladinRitualItems()
            : this( 10 )
        {
        }

        [Constructable]
        public BagOfPaladinRitualItems( int amount )
            : base( amount )
        {
        }

        public override ClassSystem System
        {
            get { return ClassSystem.Paladin; }
        }

        #region serialization
        public BagOfPaladinRitualItems( Serial serial )
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