using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class SellableIDWandLesser : BaseWand
	{
	    protected override TimeSpan UseDelay{ get{ return TimeSpan.Zero; } }

		[Constructable]
		public SellableIDWandLesser() : base( WandEffect.Identification, 10, 10 )
		{
			Name = "wand of identification";
		}

	    #region serialization
	    public SellableIDWandLesser( Serial serial ) : base( serial )
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
        #endregion

        public override bool OnWandTarget( Mobile from, object o )
		{
            if( o is IIdentificable && !( (IIdentificable)o ).IsIdentifiedFor( from ) )
            {
                ( (IIdentificable)o ).AddIdentifier( from );
                return true;
            }

			if (  o is Item )
				((Item)o).OnSingleClick( from );

			return false;
		}
	}

	public class SellableIDWandNormal : BaseWand
	{
	    protected override TimeSpan UseDelay{ get{ return TimeSpan.Zero; } }

		[Constructable]
		public SellableIDWandNormal() : base( WandEffect.Identification, 10, 175 )
		{
			Name = "wand of identification";
		}

	    #region serialization
	    public SellableIDWandNormal( Serial serial ) : base( serial )
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
        #endregion

        public override bool OnWandTarget( Mobile from, object o )
		{
            if( o is IIdentificable && !( (IIdentificable)o ).IsIdentifiedFor( from ) )
            {
                ( (IIdentificable)o ).AddIdentifier( from );
                return true;
            }

			if (  o is Item )
				((Item)o).OnSingleClick( from );

			return false;
		}
	}

	public class SellableIDWandGreater : BaseWand
	{
	    protected override TimeSpan UseDelay{ get{ return TimeSpan.Zero; } }

		[Constructable]
		public SellableIDWandGreater() : base( WandEffect.Identification, 10, 175 )
		{
			Name = "wand of identification";
		}

	    #region serialization
	    public SellableIDWandGreater( Serial serial ) : base( serial )
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
        #endregion

        public override bool OnWandTarget( Mobile from, object o )
		{
            if( o is IIdentificable && !( (IIdentificable)o ).IsIdentifiedFor( from ) )
            {
                ( (IIdentificable)o ).AddIdentifier( from );
                return true;
            }

			if (  o is Item )
				((Item)o).OnSingleClick( from );

			return false;
		}
	}
}