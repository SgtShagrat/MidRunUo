using System;
using Server;

namespace Midgard.Engines.TownHouses
{
	public class TownHouseConfirmGump : GumpPlusLight
	{
		private TownHouseSign m_Sign;
		private bool m_Items;

		public TownHouseConfirmGump( Mobile m, TownHouseSign sign ) : base( m, 100, 100 )
		{
			m_Sign = sign;
		}

		protected override void BuildGump()
		{
            int width = 200;
			int y = 0;

			AddHtml( 0, y+=10, width, String.Format( "<CENTER>{0} this House?", m_Sign.RentByTime == TimeSpan.Zero ? "Purchase" : "Rent" ));
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			if ( m_Sign.RentByTime == TimeSpan.Zero )
				AddHtml( 0, y+=25, width, String.Format( "<CENTER>{0}: {1}", "Price", m_Sign.Free ? "Free" : "" + m_Sign.Price ));
			else if ( m_Sign.RecurRent )
				AddHtml( 0, y+=25, width, String.Format( "<CENTER>{0}: {1}", "Recurring " + m_Sign.PriceType, m_Sign.Price ));
			else
				AddHtml( 0, y+=25, width, String.Format( "<CENTER>{0}: {1}", "One " + m_Sign.PriceTypeShort, m_Sign.Price ));

			if ( m_Sign.KeepItems )
			{
				AddHtml( 0, y+=20, width, "<CENTER>Cost of Items: " + m_Sign.ItemsPrice);
				AddButton( 20, y, m_Items ? 0xD3 : 0xD2, "Items", new GumpCallback( Items ) );
			}

            AddHtml(0, y += 20, width, "<CENTER>Lockdowns: " + m_Sign.Locks);
			AddHtml( 0, y+=20, width, "<CENTER>Secures: " + m_Sign.Secures);

			AddButton( 10, y+=25, 0xFB1, 0xFB3, "Cancel", new GumpCallback( Cancel ) );
			AddButton( width-40, y, 0xFB7, 0xFB9, "Confirm", new GumpCallback( Confirm ) );

            AddBackgroundZero(0, 0, width, y+40, 0x13BE);
        }

		private void Items()
		{
			m_Items = !m_Items;

			NewGump();
		}

        private void Cancel()
        {
        }

		private void Confirm()
		{
			m_Sign.Purchase( Owner, m_Items );
		}
	}
}