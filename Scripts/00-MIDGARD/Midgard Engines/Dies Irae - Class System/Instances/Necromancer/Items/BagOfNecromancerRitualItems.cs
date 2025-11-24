using Midgard.Engines.Classes;

using Server;

namespace Midgard.Items
{
    public class BagOfNecromancerRitualItems : BaseBagOfRitualItems
    {
        [Constructable]
        public BagOfNecromancerRitualItems()
            : this( 10 )
        {
        }

        [Constructable]
        public BagOfNecromancerRitualItems( int amount )
            : base( amount )
        {
        }

        public override ClassSystem System
        {
            get { return ClassSystem.Necromancer; }
        }

        #region serialization
        public BagOfNecromancerRitualItems( Serial serial )
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