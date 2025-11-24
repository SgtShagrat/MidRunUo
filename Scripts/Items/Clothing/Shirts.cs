using System;

using Midgard.Engines.Races;

namespace Server.Items
{
	public abstract class BaseShirt : BaseClothing
	{
        public override int ArmorBase{ get{ return 4; } } // mod by Dies Irae

		public BaseShirt( int itemID ) : this( itemID, 0 )
		{
		}

		public BaseShirt( int itemID, int hue ) : base( itemID, Layer.Shirt, hue )
		{
		}

		public BaseShirt( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x1efd, 0x1efe )]
    [RaceAllowance( typeof( MountainDwarf ) )]
	public class FancyShirt : BaseShirt
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 4; } }
	    public override int InitMinHits{ get{ return 21; } }
	    public override int InitMaxHits{ get{ return 23; } }
	    public override string OldInitHits{ get{ return "1d3+20"; } }

	    // mod by Dies Irae
	    #endregion

		[Constructable]
		public FancyShirt() : this( 0 )
		{
		}

		[Constructable]
		public FancyShirt( int hue ) : base( 0x1EFD, hue )
		{
			Weight = 2.0;
		}

		public FancyShirt( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x1517, 0x1518 )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class Shirt : BaseShirt
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 3; } }
	    public override int InitMinHits{ get{ return 21; } }
	    public override int InitMaxHits{ get{ return 23; } }
	    #endregion

		[Constructable]
		public Shirt() : this( 0 )
		{
		}

		[Constructable]
		public Shirt( int hue ) : base( 0x1517, hue )
		{
			Weight = 1.0;
		}

		public Shirt( Serial serial ) : base( serial )
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

			if ( Weight == 2.0 )
				Weight = 1.0;
		}
	}

	[Flipable( 0x2794, 0x27DF )]
	public class ClothNinjaJacket : BaseShirt
	{
		[Constructable]
		public ClothNinjaJacket() : this( 0 )
		{
		}

		[Constructable]
		public ClothNinjaJacket( int hue ) : base( 0x2794, hue )
		{
			Weight = 5.0;
			Layer = Layer.InnerTorso;
		}

		public ClothNinjaJacket( Serial serial ) : base( serial )
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

	public class ElvenShirt : BaseShirt
	{
		public override Race RequiredRace { get { return Race.Elf; } }
		#region modifica by Dies Irae per la label
		public override int LabelNumber{ get{ return 1064475; } } // Elven Shirt
		#endregion
		
		[Constructable]
		public ElvenShirt() : this( 0 )
		{
		}

		[Constructable]
		public ElvenShirt( int hue ) : base( 0x3175, hue )
		{
			Weight = 2.0;
		}

		public ElvenShirt(Serial serial)
			: base( serial )
		{
		}

        #region mod by Dies Irae
        public override bool CanBeCraftedBy( Mobile from )
        {
            return Midgard.Engines.Races.Core.IsElfRace( from.Race );
        }
        #endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class ElvenDarkShirt : BaseShirt
	{
		public override Race RequiredRace { get { return Race.Elf; } }
		#region modifica by Dies Irae per la label
		public override int LabelNumber{ get{ return 1064474; } } // Elven Dark Shirt
		#endregion
				
		[Constructable]
		public ElvenDarkShirt() : this( 0 )
		{
		}

		[Constructable]
		public ElvenDarkShirt( int hue ) : base( 0x3176, hue )
		{
			Weight = 2.0;
		}

        #region mod by Dies Irae
        public override bool CanBeCraftedBy( Mobile from )
        {
            return Midgard.Engines.Races.Core.IsElfRace( from.Race );
        }
        #endregion

		public ElvenDarkShirt( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}