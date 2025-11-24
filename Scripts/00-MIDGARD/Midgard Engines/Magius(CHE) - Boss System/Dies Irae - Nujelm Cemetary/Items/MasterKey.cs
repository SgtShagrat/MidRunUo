/***************************************************************************
 *                               MasterKey.cs
 *                            -------------------
 *   begin                : 06 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

namespace Server.Items
{
    public class GoldenScrabble : MasterKey
    {
        public override int Lifespan { get { return 600; } }
        public override string DefaultName { get { return "The Golden Scrabble"; } }

        [Constructable]
        public GoldenScrabble()
            : base( 0x108a ) // TODO change id to a scrabble onw
        {
            Weight = 1.0;
            Hue = Utility.RandomRedHue();
        }

        public override bool CanOfferConfirmation( Mobile from )
        {
            if( from.Region != null && from.Region.IsPartOf( "Nujel'm" ) )
                return true;

            from.SendMessage( "This powerful item can be used only in Mujel'm." );
            return false;
        }

        #region serialization
        public GoldenScrabble( Serial serial )
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