namespace Server.Items
{
    [Flipable( 0x1E5E, 0x1E5F )]
    public class BountyBoard : BaseBountyBoard
    {
        [Constructable]
        public BountyBoard()
            : base( 0x1E5E )
        {
            Name = "a bounty board";
        }

        #region serialization
        public BountyBoard( Serial serial )
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