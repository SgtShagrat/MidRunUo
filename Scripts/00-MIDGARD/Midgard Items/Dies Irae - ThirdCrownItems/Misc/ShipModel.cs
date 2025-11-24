namespace Server.Items
{
	public class ShipModel : CraftableFurniture
	{	
		[Constructable]
		public ShipModel() : base( 0x14F3 )
		{
		}

	    #region serialization
	    public ShipModel( Serial serial ) : base( serial )
	    {
	    }

	    public override void Serialize( GenericWriter writer )
	    {
	        base.Serialize( writer );

	        writer.Write( (int) 0 );
	    }
		
	    public override void Deserialize(GenericReader reader)
	    {
	        base.Deserialize( reader );

	        int version = reader.ReadInt();
	    }
	    #endregion
	}

	public class WigStand : CraftableFurniture
	{	
		[Constructable]
		public WigStand() : base( 0x0E05 )
		{
		}

	    #region serialization
	    public WigStand( Serial serial ) : base( serial )
	    {
	    }

	    public override void Serialize( GenericWriter writer )
	    {
	        base.Serialize( writer );

	        writer.Write( (int) 0 );
	    }
		
	    public override void Deserialize(GenericReader reader)
	    {
	        base.Deserialize( reader );

	        int version = reader.ReadInt();
	    }
	    #endregion
	}

	public class Pot : CraftableFurniture
	{	
		[Constructable]
		public Pot() : base( 0x09E4 )
		{
		}

	    #region serialization
	    public Pot( Serial serial ) : base( serial )
	    {
	    }

	    public override void Serialize( GenericWriter writer )
	    {
	        base.Serialize( writer );

	        writer.Write( (int) 0 );
	    }
		
	    public override void Deserialize(GenericReader reader)
	    {
	        base.Deserialize( reader );

	        int version = reader.ReadInt();
	    }
	    #endregion
	}

	public class Pan : CraftableFurniture
	{	
		[Constructable]
		public Pan() : base( 0x09F3 )
		{
		}

	    #region serialization
	    public Pan( Serial serial ) : base( serial )
	    {
	    }

	    public override void Serialize( GenericWriter writer )
	    {
	        base.Serialize( writer );

	        writer.Write( (int) 0 );
	    }
		
	    public override void Deserialize(GenericReader reader)
	    {
	        base.Deserialize( reader );

	        int version = reader.ReadInt();
	    }
	    #endregion
	}

	public class Kettle : CraftableFurniture
	{	
		[Constructable]
		public Kettle() : base( 0x09ED )
		{
		}

	    #region serialization
	    public Kettle( Serial serial ) : base( serial )
	    {
	    }

	    public override void Serialize( GenericWriter writer )
	    {
	        base.Serialize( writer );

	        writer.Write( (int) 0 );
	    }
		
	    public override void Deserialize(GenericReader reader)
	    {
	        base.Deserialize( reader );

	        int version = reader.ReadInt();
	    }
	    #endregion
	}
}