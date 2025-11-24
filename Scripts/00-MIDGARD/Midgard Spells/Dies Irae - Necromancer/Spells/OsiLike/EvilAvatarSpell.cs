/***************************************************************************
 *							   EvilAvatarSpell.cs
 *
 *   begin				: 26 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public class EvilAvatarSpell : OldTransformationSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Evil Avatar", "Rel Xen Vas Bal",
			203,
			9031,
			Reagent.BatWing,
			Reagent.DaemonBlood
			);

		public EvilAvatarSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( EvilAvatarSpell ),
			"The caster transforms into a great evil beast.",
			"Con questo potente maleficio il necromante si trasforma in una malvagia creatura.",
			0x500c
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 2.0 ); }}
		public override double RequiredSkill{get { return 40.0; }}
		public override int RequiredMana{get { return 11; }}
		public override double DelayOfReuse{get { return 5.0; }}

		public override int Body{get { return 746; }}

		public override void DoEffect( Mobile m )
		{
			m.PlaySound( 0x165 );
			m.FixedParticles( 0x3728, 1, 13, 9918, 92, 3, EffectLayer.Head );

			m.Delta( MobileDelta.WeaponDamage );
		}

		public override void RemoveEffect( Mobile m )
		{
			m.Delta( MobileDelta.WeaponDamage );
		}
	}
}