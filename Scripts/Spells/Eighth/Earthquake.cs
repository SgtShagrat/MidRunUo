using System;
using System.Collections.Generic;

using Midgard;
using Midgard.Engines.SpellSystem;

using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Eighth
{
	public class EarthquakeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Earthquake", "In Vas Por",
				233,
				9012,
				false,
				Reagent.Bloodmoss,
				Reagent.Ginseng,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Eighth; } }

		public EarthquakeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamage{ get{ return !Core.AOS; } }

		public override void OnCast()
		{
			if ( SpellHelper.CheckTown( Caster, Caster ) && CheckSequence() )
			{
				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				if ( map != null )
					foreach ( Mobile m in Caster.GetMobilesInRange( 1 + (int)(Caster.Skills[SkillName.Magery].Value / 15.0) ) )
						if ( Caster != m && Caster.InLOS( m ) )
							targets.Add( m );

				Caster.PlaySound( 0x220 );

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = targets[i];

					int damage;

					if ( Core.AOS )
					{
						damage = m.Hits / 2;

						if ( !m.Player )
							damage = Math.Max( Math.Min( damage, 100 ), 15 );
							damage += Utility.RandomMinMax( 0, 15 );

					}
					else
					{
						damage = DiceRoll.Roll( "20d4" );//(m.Hits * 6) / 10;

						//if ( !m.Player && damage < 20 )
						//	damage = 20;
					}

					if ( Caster.CanAreaHarmful( m ) )
						Caster.DoHarmful( m );

					#region mod by Dies Irae
					if( Core.AOS )
						SpellHelper.Damage( TimeSpan.Zero, m, Caster, damage, 100, 0, 0, 0, 0 );
					else
						MidgardSpellHelper.Damage( TimeSpan.Zero, m, Caster, damage, SpellType.Impact );
					#endregion
				}
			}

			FinishSequence();
		}
	}
}