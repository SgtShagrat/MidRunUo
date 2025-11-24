namespace Server.Items
{
    [Furniture]
    public class CarpenteryTableEastAddon : BaseAddon
    {
        #region proprietà
        public override BaseAddonDeed Deed { get { return new CarpenteryTableEastDeed(); } }
        public override bool RetainDeedHue { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public CarpenteryTableEastAddon()
        {
            AddComponent( new AddonComponent( 6643 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 6642 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 6641 ), 0, 2, 0 );
        }

        public CarpenteryTableEastAddon( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class CarpenteryTableEastDeed : BaseAddonDeed
    {
        #region proprietà
        public override BaseAddon Addon { get { return new CarpenteryTableEastAddon(); } }
        public override int LabelNumber { get { return 1064512; } } // Carpentery Table
        #endregion

        #region costruttori
        [Constructable]
        public CarpenteryTableEastDeed()
        {
        }

        public CarpenteryTableEastDeed( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}