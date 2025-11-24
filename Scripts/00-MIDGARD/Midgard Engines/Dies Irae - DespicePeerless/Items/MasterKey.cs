/***************************************************************************
 *                               Dies Irae - MasterKey.cs
 *                            -------------------
 *   begin                : 30 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

namespace Server.Items
{
    public class BarracoonFavouriteCheese : MasterKey
    {
        public override int Lifespan { get { return 600; } }
        public override string DefaultName { get { return "Barracoon's favourite cheese"; } }

        [Constructable]
        public BarracoonFavouriteCheese()
            : base( 0x97C )
        {
            Weight = 1.0;
            Hue = Utility.RandomYellowHue();
        }

        public BarracoonFavouriteCheese( Serial serial )
            : base( serial )
        {
        }

        public override bool CanOfferConfirmation( Mobile from )
        {
            if( from.Region != null && from.Region.IsPartOf( "Despise" ) )
                return true;

            return false;
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
    }
}