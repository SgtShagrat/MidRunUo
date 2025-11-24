using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ShadowFirePitAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get	{	return new ShadowFirePitAddonDeed(); }
		}

		[ Constructable ]
		public ShadowFirePitAddon()
		{
		        AddComponent( new AddonComponent( 13909 ), 0, 0, 0 );
		        AddComponent( new AddonComponent( 13910 ), 1, -1, 1 );
			AddComponent( new AddonComponent( 13905 ), 1, 0, -1 );
			//AddComponent( new AddonComponent( 13908 ), 0, 0, 0 );


			AddonComponent ac;
			ac = new AddonComponent( 13905 );
			AddComponent( ac, 1, 0, -1 );
			//ac = new AddonComponent( 13908 );
			//AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 13909 );
			AddComponent( ac, 0, 0, 0 );
                        ac = new AddonComponent( 13910 );
			AddComponent( ac, 1, -1, 1 );

		}

		public ShadowFirePitAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class ShadowFirePitAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get	{	return new ShadowFirePitAddon();	}
		}

		[Constructable]
		public ShadowFirePitAddonDeed()
		{
			Name = "Shadow Firepit";
			Hue = 1109;
			LootType = LootType.Blessed;
		}

		public ShadowFirePitAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
