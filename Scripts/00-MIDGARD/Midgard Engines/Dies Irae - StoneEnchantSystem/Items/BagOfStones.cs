using System;
using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
    public class BagOfEnchantingStones : Bag
    {
        #region costruttori
        [Constructable]
        public BagOfEnchantingStones()
            : this( 50 )
        {
        }

        [Constructable]
        public BagOfEnchantingStones( int amount )
        {
            for( int i = 0; i < amount; i++ )
            {
                for( int j = 1; j < Enum.GetNames( typeof( StoneTypes ) ).Length; j++ )
                {
                    DropItem( new EnchantStone( (StoneTypes)j ) );
                }
            }
        }

        public BagOfEnchantingStones( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region serial-deserial
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
