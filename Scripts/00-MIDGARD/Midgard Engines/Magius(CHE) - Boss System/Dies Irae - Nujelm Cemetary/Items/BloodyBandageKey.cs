/***************************************************************************
 *                               BloodyBandageKey.cs
 *                            ------------------------
 *   begin                : 06 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

namespace Server.Items
{
    public class BloodyBandageKey : PeerlessKey
    {
        public override int Lifespan { get { return 60 * 60; } } // 1 hour
        public override string DefaultName { get { return "a bloody bandage"; } }

        [Constructable]
        public BloodyBandageKey()
            : base( 0xE21 )
        {
            Weight = 10.0;
            Hue = 37;
        }

        #region serialization
        public BloodyBandageKey( Serial serial )
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