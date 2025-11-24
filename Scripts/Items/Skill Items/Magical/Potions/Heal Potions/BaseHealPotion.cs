using System;
using System.Collections.Generic;

using Midgard;

using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseHealPotion : BasePotion
	{
		public abstract int MinHeal { get; }
		public abstract int MaxHeal { get; }
		public abstract double Delay { get; }

        #region mod by Dies Irae: pre-aos stuff
        public override int DelayUse
        {
            get { return 12; }
        }

        public override int BonusOnDelayAtHundred
        {
            get { return 6; }
        }

        public abstract string HealDice { get; }
        #endregion

        #region Modifica by Dies Irae per le pozioni Stackable
        public BaseHealPotion( PotionEffect effect, int amount ) : base( 0xF0C, effect, amount )
		{
		}
		#endregion
		
		public BaseHealPotion( Serial serial ) : base( serial )
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

		public virtual void DoHeal( Mobile from )
		{
            #region mod by Dies Irae
            if( Core.T2A )
            {
                from.Heal( DiceRoll.Roll( HealDice ) );
                return;
            }
            #endregion

			int min = Scale( from, MinHeal );
			int max = Scale( from, MaxHeal );

			from.Heal( Utility.RandomMinMax( min, max ) );
		}

        #region mod by Dies Irae
        private void LockHealPotionUse( Mobile from )
        {
            from.BeginAction( typeof( BaseHealPotion ) );
            Timer.DelayCall( GetDelayOfHealReuse( from ), new TimerStateCallback( ReleaseHealLock ), from );

            Midgard2PlayerMobile m = from as Midgard2PlayerMobile;
            if( m != null )
                m.LastHealPotion = DateTime.Now;
        }

        private static TimeSpan CoolingDelay = TimeSpan.FromSeconds( 120.0 );

        private TimeSpan GetDelayOfHealReuse( Mobile from )
        {
            int offset = (int)( ( from.Skills[ SkillName.Alchemy ].Value / 100.0 ) * BonusOnDelayAtHundred );

            Midgard2PlayerMobile m = from as Midgard2PlayerMobile;
            if( m != null )
            {
                bool hasAlchemyBonus = from.Skills[ SkillName.Alchemy ].Value == 100.0;

                if( m.LastHealPotion < DateTime.Now - CoolingDelay )
                {
                    m.HealingAddiction = 0;
                }
                else
                {
                    m.HealingAddiction = m.HealingAddiction + ( hasAlchemyBonus ? 2 : 3 );
                }

                int delay = m.HealingAddiction + DelayUse - offset;

                from.SendMessage( "You could drink another heal potion in {0} seconds ( addiction is {1}).", delay, m.HealingAddiction );
                return delay > 0 ? TimeSpan.FromSeconds( delay ) : TimeSpan.Zero;
            }

            return TimeSpan.FromSeconds( DelayUse );
        }

        public override bool CanDrink( Mobile from, bool message )
        {
            if( !base.CanDrink( from, message ) )
                return false;

            bool canDrink = from.CanBeginAction( typeof( BaseHealPotion ) );

            if( !canDrink && message )
            {
                from.SendMessage( "You must wait until you can drink another potion!" );
                return false;
            }

            if( from.Hits >= from.HitsMax )
            {
                from.SendLocalizedMessage( 1049547 ); // You decide against drinking this potion, as you are already at full health.
                return false;
            }

            if( Core.AOS && from.Poisoned ) // mod by Dies Irae
            {
                from.LocalOverheadMessage( MessageType.Regular, 0x22, 1005000 ); // You can not heal yourself in your current state.
                return false;
            }

            return true;
        }

        private void OldDrink( Mobile from )
        {
            LockBasePotionUse( from );
            LockHealPotionUse( from );

            DoHeal( from );

            PlayDrinkEffect( from );

            Consume();
        }
        #endregion

		public override void Drink( Mobile from )
		{
            #region mod by Dies Irae
            if( Core.T2A && from.Player )
            {
                OldDrink( from );
                return;
            }
            #endregion

			if ( from.Hits < from.HitsMax )
			{
				if ( from.Poisoned || MortalStrike.IsWounded( from ) )
				{
					from.LocalOverheadMessage( MessageType.Regular, 0x22, 1005000 ); // You can not heal yourself in your current state.
				}
				else
				{
					if ( from.BeginAction( typeof( BaseHealPotion ) ) )
					{
						DoHeal( from );

						BasePotion.PlayDrinkEffect( from );

						this.Consume();

						Timer.DelayCall( TimeSpan.FromSeconds( Delay ), new TimerStateCallback( ReleaseHealLock ), from );
					}
					else
					{
						from.LocalOverheadMessage( MessageType.Regular, 0x22, 500235 ); // You must wait 10 seconds before using another healing potion.
					}
				}
			}
			else
			{
				from.SendLocalizedMessage( 1049547 ); // You decide against drinking this potion, as you are already at full health.
			}
		}

		private static void ReleaseHealLock( object state )
		{
			((Mobile)state).EndAction( typeof( BaseHealPotion ) );
        }
    }
}