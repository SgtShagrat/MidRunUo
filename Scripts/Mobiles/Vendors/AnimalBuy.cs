using System;
using System.Collections;

using Midgard;
using CommercialSystem = Midgard.Engines.CommercialSystem;

using Server.Items;

namespace Server.Mobiles
{
	public class AnimalBuyInfo : GenericBuyInfo
	{
		private int m_ControlSlots;

		public AnimalBuyInfo( int controlSlots, Type type, int price, int amount, int itemID, int hue ) : this( controlSlots, null, type, price, amount, itemID, hue )
		{
		}

		public AnimalBuyInfo( int controlSlots, string name, Type type, int price, int amount, int itemID, int hue ) : base( name, type, price, amount, itemID, hue )
		{
			m_ControlSlots = controlSlots;
		}

		public override int ControlSlots
		{
			get
			{
				return m_ControlSlots;
			}
		}
	}

    #region mod by Dies Irae
    public class AnimalSellInfo : GenericSellInfo
    {
        public int GetSellPriceFor( BaseCreature creature )
        {
            int price = 0;
            GetSellPriceFor(creature.GetType(), out price);

            #region mod by Dies Irae
            if( CommercialSystem.Config.BuyInfoScalarnabled && Demultiplicator > 0.0 )
            {
                price = (int)( price * Demultiplicator );
                if( price < 1 )
                    price = 1;
            }
            #endregion

            return price;
        }

        public int GetBuyPriceFor( BaseCreature creature )
        {
            return (int)( 10.0 * GetSellPriceFor( creature ) );
        }

        public string GetNameFor( BaseCreature creature )
        {
            return creature.Name ?? MidgardUtility.GetFriendlyClassName( creature.GetType().Name );
        }

        public bool IsSellable( BaseCreature creature )
        {
            return IsInList( creature.GetType() );
        }

        public bool IsResellable( BaseCreature creature )
        {
            return IsInList( creature.GetType() );
        }
    }
    #endregion
}