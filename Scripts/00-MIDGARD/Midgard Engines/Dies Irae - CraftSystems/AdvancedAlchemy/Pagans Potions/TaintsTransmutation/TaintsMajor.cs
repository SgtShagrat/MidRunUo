/***************************************************************************
 *                               TaintsMajorTransmutationPotion
 *                            ------------------------------------
 *   begin                : 24 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class TaintsMajorTransmutationPotion : BaseTaintsTransmutationPotion
    {
        public override int Strength
        {
            get { return 5; }
        }

        public override double AlchemyRequiredToDrink
        {
            get { return 70.0; }
        }

        [Constructable]
        public TaintsMajorTransmutationPotion( int amount )
            : base( PotionEffect.TrasmutationGreater, amount )
        {
            // Name = "Taint's Major Transmutaion Potion";
            Hue = 2748;
        }

        [Constructable]
        public TaintsMajorTransmutationPotion()
            : this( 1 )
        {
        }

        #region serial deserial
        public TaintsMajorTransmutationPotion( Serial serial )
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