/***************************************************************************
 *                                   FletcherKnife.cs
 *                            		------------------
 *  begin                	: July, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 *  
 ***************************************************************************/

using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{	
	public class FletcherKnife : Item
	{
		[Constructable]
		public FletcherKnife() : base( 3922 )
		{
			Name= "Fletcher Knife";
			Weight = 1.0;
		}

		public FletcherKnife( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			from.SendMessage( "What should I use these knife on?" ); // What should I use these knife on?

			from.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private FletcherKnife m_Item;

			public InternalTarget( FletcherKnife item ) : base( 1, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				Item objtarget = targeted as Item;
				PlayerMobile pfrom = from as PlayerMobile;
				
				if( m_Item.Deleted )
					return;
					
				if( targeted.GetType().IsSubclassOf( typeof(BaseLog) ) )
				{
					int number = objtarget.Amount;
					objtarget.Delete();
					pfrom.AddToBackpack( new Shaft( number * 5 ) );				
				}
				else
				{
					from.SendMessage( "Fletcher Knife can not be used on that to produce anything." ); // Scissors can not be used on that to produce anything.
				}
			}
		}
	}
}
