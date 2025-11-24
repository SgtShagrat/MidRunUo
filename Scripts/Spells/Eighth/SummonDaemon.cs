using System;

using Midgard.Engines.Classes;

using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Eighth
{
	public class SummonDaemonSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Summon Daemon", "Kal Vas Xen Corp",
				269,
				9050,
				false,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Eighth; } }

		public SummonDaemonSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( (Caster.Followers + /*(Core.SE ? 4 : 5))*/ 4 ) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

            #region mod by Dies Irae
            if( ClassSystem.IsDruid( Caster ) )
            {
                Caster.SendMessage( "Thou may not summon such an evil being" );
                return false;
            }
            #endregion

			return true;
		}

		public override void OnCast()
		{
			if ( CheckSequence() && SpellHelper.CheckUniqueSummon( typeof( SummonedDaemon ), Caster, true ) )
			{	
				TimeSpan duration = TimeSpan.FromSeconds( (2 * Caster.Skills.Magery.Fixed) / 5 );

				//if ( Core.AOS )  /* Why two diff daemons? TODO: solve this */
				//{
					BaseCreature m_Daemon = new SummonedDaemon();
					SpellHelper.Summon( m_Daemon, Caster, 0x216, duration, false, false );
					m_Daemon.FixedParticles(0x3728, 8, 20, 5042, EffectLayer.Head );
                //}
                //else
                //    SpellHelper.Summon( new Daemon(), Caster, 0x216, duration, false, false );
			}

			FinishSequence();
		}

        #region mod by Dies Irae
        public override int ComputeKarmaAward()
        {
            /*
            var karma:=GetKarma(caster);
            var lossKarma:=0;
            if (karma>-625)
                lossKarma:=-RandomInt(1000)+1;
            elseif (karma>-2500)
                lossKarma:=-RandomInt(600)+1;
            elseif (karma>-5000)
                lossKarma:=-RandomInt(300)+1;
            elseif (karma>-10000)
                lossKarma:=-RandomInt(100)+1;
            endif
            AdjustKarma(caster,lossKarma);
            */

            return -( 70 + (int)Circle );
        }
        #endregion
	}
}