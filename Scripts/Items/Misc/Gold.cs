using System;
using Midgard.Engines.AdvancedSmelting;
using Server.Engines.Craft;
using Server.Targeting;

namespace Server.Items
{
	public class Gold : Item, IBankWeightLess
	{
		public override double DefaultWeight
		{
			get { return ( Core.ML ? ( 0.02 / 3 ) : 0.02 ); }
		}

		[Constructable]
		public Gold() : this( 1 )
		{
		}

		[Constructable]
		public Gold( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public Gold( int amount ) : base( 0xEED )
		{
			Stackable = true;
			Amount = amount;
		}

		public Gold( Serial serial ) : base( serial )
		{
		}

		public override int GetDropSound()
		{
			if ( Amount <= 1 )
				return 0x2E4;
			else if ( Amount <= 5 )
				return 0x2E5;
			else
				return 0x2E6;
		}

		protected override void OnAmountChange( int oldValue )
		{
			int newValue = this.Amount;

			UpdateTotal( this, TotalType.Gold, newValue - oldValue );
		}

		public override int GetTotal( TotalType type )
		{
			int baseTotal = base.GetTotal( type );

			if ( type == TotalType.Gold )
				baseTotal += this.Amount;

			return baseTotal;
		}

		#region mod by Dies Irae : Pre-Aos stuff
		public override void OnSingleClick( Mobile from )
		{
			if ( from.Language == "ITA" )
				LabelTo( from, "{0} monet{1} d'oro", Amount, Amount == 1 ? "a" : "e" );
			else
				LabelTo( from, "{0} gold coin{1}", Amount, Amount == 1 ? "" : "s" );
		}

        public override void OnDoubleClick( Mobile from )
        {
            if( Core.AOS )
                return;

            if( !Movable )
                return;

            if( !IsChildOf( from.Backpack ) )
            {
                from.SendMessage( "The gold must be in your backpack." );
            }
            else if( IsAccessibleTo( from ) )
            {
                from.SendMessage( "Select the forge on which to smelt the gold coins." );
                from.Target = new InternalTarget( this );
            }
            else
            {
                from.SendMessage( "That is too far away." );
            }
        }

        private class InternalTarget : Target
        {
            private Gold m_Gold;

            public InternalTarget( Gold gold )
                : base( 1, false, TargetFlags.None )
            {
                m_Gold = gold;
            }

            private static bool IsForge( object obj )
            {
                if( obj.GetType().IsDefined( typeof( ForgeAttribute ), false ) )
                    return true;

                if( obj is AdvancedForge )
                    return true;

                int itemID = 0;

                if( obj is Item )
                    itemID = ( (Item)obj ).ItemID;
                else if( obj is StaticTarget )
                    itemID = ( (StaticTarget)obj ).ItemID & 0x3FFF;

                return ( itemID == 4017 || ( itemID >= 6522 && itemID <= 6569 ) );
            }

            public const int GoldCoinsForOneIngot = 200;

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Gold.Deleted )
                    return;

                if( !m_Gold.IsChildOf( from.Backpack ) )
                {
                    from.SendMessage( "The gold must be in your backpack." );
                    return;
                }

                if( IsForge( targeted ) )
                {
                    double difficulty = 40.0;
                    double minSkill = difficulty - 25.0;
                    double maxSkill = difficulty + 25.0;

                    if( m_Gold.Amount < 50 )
                    {
                        from.SendMessage( "It doesn't look like you used enough gold!");
                        m_Gold.Delete();
                        return;
                    }

                    if( difficulty > from.Skills[ SkillName.Mining ].Value )
                    {
                        from.SendMessage( "You have no idea how to smelt that!" );
                        return;
                    }

                    if( from.CheckTargetSkill( SkillName.Mining, targeted, minSkill, maxSkill ) )
                    {
                        int toConsume = m_Gold.Amount;

                        if( toConsume < GoldCoinsForOneIngot )
                        {
                            from.SendMessage( "It doesn't look like you used enough gold!");
                            m_Gold.Delete();
                        }
                        else
                        {
                            if( toConsume > 60000 )
                                toConsume = 60000;

                            GoldIngot ingot = new GoldIngot( toConsume / GoldCoinsForOneIngot );
                            m_Gold.Consume( toConsume );
                            from.AddToBackpack( ingot );

                            from.SendMessage( "You create some gold ingots and place them in your pack." );
                        }
                    }
                    else if( m_Gold.Amount < GoldCoinsForOneIngot )
                    {
                        from.SendMessage( "You smelt the gold but there remains no useable metal." );
                        m_Gold.Delete();
                    }
                    else
                    {
                        from.SendMessage( "You destroy some gold coins." );
                        m_Gold.Amount /= 2;
                    }
                }
            }
        }
        #endregion

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