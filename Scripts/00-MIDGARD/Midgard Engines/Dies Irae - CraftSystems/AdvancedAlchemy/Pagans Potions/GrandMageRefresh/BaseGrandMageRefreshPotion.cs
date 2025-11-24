/***************************************************************************
 *                               BaseGrandMageRefreshPotion
 *                            --------------------------------
 *   begin                : 24 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseGrandMageRefreshPotion : BasePaganPotion
    {
        public override int DelayUse
        {
            get { return 14; }
        }

        public override int BonusOnDelayAtHundred
        {
            get { return 6; }
        }

        public abstract double MageryRequired { get; }

        public BaseGrandMageRefreshPotion( PotionEffect effect, int amount )
            : base( 0xF0E, effect, amount )
        {
        }

        #region metodi
        public override bool CanDrink( Mobile from, bool message )
        {
            if( !base.CanDrink( from, message ) )
                return false;

            if( from.Mana >= from.ManaMax )
            {
                from.SendMessage( "You do not need to use this potion now." );
                return false;
            }

            if( from.Skills.Magery.Value < MageryRequired )
            {
                from.SendMessage( "Thou are not enough skilled in magery to use this potion." );
                return false;
            }

            return true;
        }

        public override bool DoPaganEffect( Mobile from )
        {
            int restore = Utility.RandomMinMax( Strength * 3, Strength * 4 );

            from.Mana = ( from.Mana + restore > from.ManaMax ? from.ManaMax : from.Mana + restore );

            LockBasePotionUse( from );
            return true;
        }
        #endregion

        #region serial deserial
        public BaseGrandMageRefreshPotion( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}