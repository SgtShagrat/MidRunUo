namespace Server.Items
{
	public class DragonTorphyEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new DragonTorphyEastDeed(); } }

		[Constructable]
		public DragonTorphyEastAddon()
		{
			AddComponent( new AddonComponent( 0x2235 ), 0, 0, 10 );
		}

		public DragonTorphyEastAddon( Serial serial ) : base( serial )
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

	public class DragonTorphyEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new DragonTorphyEastAddon(); } }

        public override string DefaultName
        {
            get
            {
                return "dragon head trophy deed [east]";
            }
        }

		[Constructable]
		public DragonTorphyEastDeed()
		{
		}

		public DragonTorphyEastDeed( Serial serial ) : base( serial )
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

            if( Name == "dragon head trophy deed [east]" )
                Name = null;
		}
	}
}