/***************************************************************************
 *							   DruidSummon.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Spells;
using Server.Mobiles;

namespace Midgard.Engines.SpellSystem
{
	public abstract class DruidSummon<T> : DruidSpell where T : BaseCreature
	{
		public abstract int Sound { get; }

		public DruidSummon( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public override bool CheckCast()
		{
			if( !base.CheckCast() )
				return false;

			if( ( Caster.Followers + 1 ) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1074270 ); // You have too many followers to summon another one.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if( CheckSequence() )
			{
				TimeSpan duration = TimeSpan.FromMinutes( Caster.Skills.Spellweaving.Value / 24 + FocusLevel * 2 );
				int summons = Math.Min( 1 + FocusLevel, Caster.FollowersMax - Caster.Followers );

				for( int i = 0; i < summons; i++ )
				{
					BaseCreature bc;

					try { bc = Activator.CreateInstance<T>(); }
					catch { break; }

					SpellHelper.Summon( bc, Caster, Sound, duration, false, false );
				}

				FinishSequence();
			}
		}
	}
}