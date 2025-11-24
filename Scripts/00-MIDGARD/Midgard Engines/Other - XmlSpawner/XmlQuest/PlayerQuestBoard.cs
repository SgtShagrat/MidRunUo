namespace Server.Items
{
    [Flipable( 0x1E5E, 0x1E5F )]
    public class PlayerQuestBoard : XmlQuestBook
    {
        public PlayerQuestBoard( Serial serial )
            : base( serial )
        {
        }

        [Constructable]
        public PlayerQuestBoard()
            : base( 0x1e5e )
        {
            Movable = false;
            Name = "Player Quest Board";
            LiftOverride = true; // allow players to store books in it
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
    }
}