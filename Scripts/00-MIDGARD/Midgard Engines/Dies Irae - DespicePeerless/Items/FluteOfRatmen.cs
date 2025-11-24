/***************************************************************************
 *                               FluteOfRatmen.cs
 *                            ----------------------
 *   begin                : 30 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

namespace Server.Items
{
    public class FluteOfRatmen : PeerlessKey
    {	
        public override int Lifespan{ get{ return 21600; } }
        public override string DefaultName { get { return "Flute of the Ratmen"; } }

        [Constructable]
        public FluteOfRatmen() : base( 0x1421 )
        {
            Weight = 1.0;
            Hue = Utility.RandomRedHue();
            // Name = "Flute of the Ratmen";
        }
	
        public FluteOfRatmen( Serial serial ) : base( serial )
        {
        }
	
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
			
            writer.Write( (int) 0 ); // version
        }
	
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
			
            int version = reader.ReadInt();
        }
    }
}