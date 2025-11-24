namespace Server.Items
{
    [Furniture]
    public class CarpenteryTableSouthAddon : BaseAddon
    {
        #region proprietà
        public override BaseAddonDeed Deed { get { return new CarpenteryTableSouthDeed(); } }
        public override bool RetainDeedHue { get { return true; } }
        #endregion

        #region costruttori

        [Constructable]
        public CarpenteryTableSouthAddon()
        {
            AddComponent( new AddonComponent( 6647 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 6646 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 6645 ), 2, 0, 0 );
        }

        public CarpenteryTableSouthAddon( Serial serial )
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

    public class CarpenteryTableSouthDeed : BaseAddonDeed
    {
        #region proprietà
        public override BaseAddon Addon { get { return new CarpenteryTableSouthAddon(); } }
        public override int LabelNumber { get { return 1064512; } } // Carpentery Table
        #endregion

        #region costruttori
        [Constructable]
        public CarpenteryTableSouthDeed()
        {
        }

        public CarpenteryTableSouthDeed( Serial serial )
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