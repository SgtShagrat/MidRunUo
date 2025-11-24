using System;

using Midgard.Engines.Races;

namespace Server.Items
{
	public abstract class BasePants : BaseClothing
	{
		public BasePants( int itemID ) : this( itemID, 0 )
		{
		}

		public BasePants( int itemID, int hue ) : base( itemID, Layer.Pants, hue )
		{
		}

		public BasePants( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x152e, 0x152f )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class ShortPants : BasePants
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 3; } }
	    public override int InitMinHits{ get{ return 11; } }
	    public override int InitMaxHits{ get{ return 40; } }
	    public override string OldInitHits{ get{ return "1d30+10"; } }
	    #endregion

		[Constructable]
		public ShortPants() : this( 0 )
		{
		}

		[Constructable]
		public ShortPants( int hue ) : base( 0x152E, hue )
		{
			Weight = 2.0;
		}

		public ShortPants( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x1539, 0x153a )]
    [RaceAllowance( typeof( MountainDwarf ) )]
	public class LongPants : BasePants
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 4; } }
	    public override int InitMinHits{ get{ return 11; } }
	    public override int InitMaxHits{ get{ return 40; } }
	    public override string OldInitHits{ get{ return "1d30+10"; } }

	    // mod by Dies Irae
	    #endregion

		[Constructable]
		public LongPants() : this( 0 )
		{
		}

		[Constructable]
		public LongPants( int hue ) : base( 0x1539, hue )
		{
			Weight = 2.0;
		}

		public LongPants( Serial serial ) : base( serial )
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

	[Flipable( 0x279B, 0x27E6 )]
	public class TattsukeHakama : BasePants
	{
		[Constructable]
		public TattsukeHakama() : this( 0 )
		{
		}

		[Constructable]
		public TattsukeHakama( int hue ) : base( 0x279B, hue )
		{
			Weight = 2.0;
		}

		public TattsukeHakama( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x2FC3, 0x3179 )]
	public class ElvenPants : BasePants
	{
		public override Race RequiredRace { get { return Race.Elf; } }
		#region modifica by Dies Irae per la label
		public override int LabelNumber{ get{ return 1064473; } }	// Elven Pants
		#endregion

		[Constructable]
		public ElvenPants() : this( 0 )
		{
		}

		[Constructable]
		public ElvenPants( int hue ) : base( 0x2FC3, hue )
		{
			Weight = 2.0;
		}

        #region mod by Dies Irae
        public override bool CanBeCraftedBy( Mobile from )
        {
            return Midgard.Engines.Races.Core.IsElfRace( from.Race );
        }
        #endregion

		public ElvenPants( Serial serial ) : base( serial )
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