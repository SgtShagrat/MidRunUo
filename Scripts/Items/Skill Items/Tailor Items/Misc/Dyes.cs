using System;
using Server.Targeting;
using Server.HuePickers;

namespace Server.Items
{
	public class Dyes : Item
	{
		[Constructable]
		public Dyes() : base( 0xFA9 )
		{
			Weight = 3.0;
		}

		public Dyes( Serial serial ) : base( serial )
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

			if ( Weight == 0.0 )
				Weight = 3.0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendLocalizedMessage( 500856 ); // Select the dye tub to use the dyes on.
			from.Target = new InternalTarget();
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 1, false, TargetFlags.None )
			{
			}

			private class InternalPicker : HuePicker
			{
				private DyeTub m_Tub;

				public InternalPicker( DyeTub tub ) : base( tub.ItemID )
				{
					m_Tub = tub;
				}

				public override void OnResponse( int hue )
				{
					m_Tub.DyedHue = hue;
				}
			}

		    #region mod by Dies Irae
            private class InternalTownDecoratorPicker : HuePicker
		    {
		        private TownDecorator m_Decorator;

		        public InternalTownDecoratorPicker( TownDecorator decorator ) : base( decorator.ItemID )
		        {
		            m_Decorator = decorator;
		        }

		        public override void OnResponse( int hue )
		        {
		            m_Decorator.DyedHue = hue;
		        }
		    }

			private static void SetDecoratorHue( Mobile from, object state, int hue )
			{
				((TownDecorator)state).DyedHue = hue;
			}
            #endregion
            
			private static void SetTubHue( Mobile from, object state, int hue )
			{
				((DyeTub)state).DyedHue = hue;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is DyeTub )
				{
					DyeTub tub = (DyeTub) targeted;

					if ( tub.Redyable )
					{
						if ( tub.CustomHuePicker == null )
							from.SendHuePicker( new InternalPicker( tub ) );
						else
							from.SendGump( new CustomHuePickerGump( from, tub.CustomHuePicker, new CustomHuePickerCallback( SetTubHue ), tub ) );
					}
					else if ( tub is BlackDyeTub )
					{
						from.SendLocalizedMessage( 1010092 ); // You can not use this on a black dye tub.
					}
					else
					{
						from.SendMessage( "That dye tub may not be redyed." );
					}
				}
                #region mod by Dies Irae
                else if( targeted is TownDecorator )
                {
                    TownDecorator decorator = (TownDecorator)targeted;

                    if( decorator.CustomHuePicker == null )
                        from.SendHuePicker( new InternalTownDecoratorPicker( decorator ) );
                    else
                        from.SendGump( new CustomHuePickerGump( from, decorator.CustomHuePicker, new CustomHuePickerCallback( SetDecoratorHue ), decorator ) );
                }
                #endregion
                else
				{
					from.SendLocalizedMessage( 500857 ); // Use this on a dye tub.
				}
			}
		}
	}
}