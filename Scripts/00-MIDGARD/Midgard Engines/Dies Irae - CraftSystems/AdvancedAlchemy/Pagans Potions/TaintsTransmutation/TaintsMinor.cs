/***************************************************************************
 *                               TaintsMinorTransmutationPotion
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
    public class TaintsMinorTransmutationPotion : BaseTaintsTransmutationPotion
    {
        public override int Strength
        {
            get { return 3; }
        }

        public override double AlchemyRequiredToDrink
        {
            get { return 50.0; }
        }

        [Constructable]
        public TaintsMinorTransmutationPotion( int amount )
            : base( PotionEffect.TrasmutationLesser, amount )
        {
            // Name = "Taint's Minor Transmutaion Potion";
            Hue = 1920;
        }

        [Constructable]
        public TaintsMinorTransmutationPotion()
            : this( 1 )
        {
        }

        #region serial deserial
        public TaintsMinorTransmutationPotion( Serial serial )
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