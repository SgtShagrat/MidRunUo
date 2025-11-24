using System;

using Midgard.Engines.Races;

namespace Server.Items
{
	public abstract class BaseMiddleTorso : BaseClothing
	{
		public BaseMiddleTorso( int itemID ) : this( itemID, 0 )
		{
		}

		public BaseMiddleTorso( int itemID, int hue ) : base( itemID, Layer.MiddleTorso, hue )
		{
		}

		public BaseMiddleTorso( Serial serial ) : base( serial )
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

	[Flipable( 0x1541, 0x1542 )]
    [RaceAllowance( typeof( MountainDwarf ) )]
	public class BodySash : BaseMiddleTorso
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 1; } }
	    public override int InitMinHits{ get{ return 11; } }
	    public override int InitMaxHits{ get{ return 40; } }
	    public override string OldInitHits{ get{ return "1d30+10"; } }

	    // mod by Dies Irae
	    #endregion

		[Constructable]
		public BodySash() : this( 0 )
		{
		}

		[Constructable]
		public BodySash( int hue ) : base( 0x1541, hue )
		{
			Weight = 1.0;
		}

		public BodySash( Serial serial ) : base( serial )
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

	[Flipable( 0x153d, 0x153e )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class FullApron : BaseMiddleTorso
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 4; } }
	    public override int InitMinHits{ get{ return 21; } }
	    public override int InitMaxHits{ get{ return 30; } }
	    public override string OldInitHits{ get{ return "1d10+20"; } }

	    // mod by Dies Irae
	    #endregion

		[Constructable]
		public FullApron() : this( 0 )
		{
		}

		[Constructable]
		public FullApron( int hue ) : base( 0x153d, hue )
		{
			Weight = 4.0;
		}

		public FullApron( Serial serial ) : base( serial )
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

	[Flipable( 0x1f7b, 0x1f7c )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class Doublet : BaseMiddleTorso
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 3; } }
	    public override int InitMinHits{ get{ return 11; } }
	    public override int InitMaxHits{ get{ return 40; } }
	    public override string OldInitHits{ get{ return "1d30+10"; } }

	    // mod by Dies Irae
	    #endregion

		[Constructable]
		public Doublet() : this( 0 )
		{
		}

		[Constructable]
		public Doublet( int hue ) : base( 0x1F7B, hue )
		{
			Weight = 2.0;
		}

		public Doublet( Serial serial ) : base( serial )
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

	[Flipable( 0x1ffd, 0x1ffe )]
	public class Surcoat : BaseMiddleTorso
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 3; } }
	    public override int InitMinHits{ get{ return 11; } }
	    public override int InitMaxHits{ get{ return 40; } }
	    public override string OldInitHits{ get{ return "1d30+10"; } }
	    #endregion

		[Constructable]
		public Surcoat() : this( 0 )
		{
		}

		[Constructable]
		public Surcoat( int hue ) : base( 0x1FFD, hue )
		{
			Weight = 6.0;
		}

		public Surcoat( Serial serial ) : base( serial )
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

			if ( Weight == 3.0 )
				Weight = 6.0;
		}
	}

	[Flipable( 0x1fa1, 0x1fa2 )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class Tunic : BaseMiddleTorso
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 3; } }
	    public override int InitMinHits{ get{ return 11; } }
	    public override int InitMaxHits{ get{ return 40; } }
	    public override string OldInitHits{ get{ return "1d30+10"; } }
	    #endregion

		[Constructable]
		public Tunic() : this( 0 )
		{
		}

		[Constructable]
		public Tunic( int hue ) : base( 0x1FA1, hue )
		{
			Weight = 5.0;
		}

		public Tunic( Serial serial ) : base( serial )
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

	[Flipable( 0x2310, 0x230F )]
	public class FormalShirt : BaseMiddleTorso
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 3; } }
	    public override int InitMinHits{ get{ return 11; } }
	    public override int InitMaxHits{ get{ return 40; } }
	    public override string OldInitHits{ get{ return "1d30+10"; } }
	    #endregion

		[Constructable]
		public FormalShirt() : this( 0 )
		{
		}

		[Constructable]
		public FormalShirt( int hue ) : base( 0x2310, hue )
		{
			Weight = 1.0;
		}

		public FormalShirt( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			if ( Weight == 2.0 )
				Weight = 1.0;
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[Flipable( 0x1f9f, 0x1fa0 )]
	public class JesterSuit : BaseMiddleTorso
	{
	    #region mod by Dies Irae : pre-aos stuff
        public override int ArmorBase{ get{ return 7; } }
	    public override int InitMinHits{ get{ return 11; } }
	    public override int InitMaxHits{ get{ return 40; } }
	    public override string OldInitHits{ get{ return "1d30+10"; } }
	    #endregion

		[Constructable]
		public JesterSuit() : this( 0 )
		{
		}

		[Constructable]
		public JesterSuit( int hue ) : base( 0x1F9F, hue )
		{
			Weight = 4.0;
		}

		public JesterSuit( Serial serial ) : base( serial )
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

	[Flipable( 0x27A1, 0x27EC )]
	public class JinBaori : BaseMiddleTorso
	{
		[Constructable]
		public JinBaori() : this( 0 )
		{
		}

		[Constructable]
		public JinBaori( int hue ) : base( 0x27A1, hue )
		{
			Weight = 3.0;
		}

		public JinBaori( Serial serial ) : base( serial )
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