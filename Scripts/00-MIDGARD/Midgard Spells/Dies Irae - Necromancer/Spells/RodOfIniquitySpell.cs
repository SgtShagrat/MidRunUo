/***************************************************************************
 *								  RosOfIniquitySpell.cs
 *									---------------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Questo potente incantesimo evoca per un certo tempo l'arma del
 * 			maleficio.
 * 
 * 			Ha le seguenti proprietà:
 * 				- Hit Life Leech ( Necromancy - 20 ) %;
 * 				- Use Best Weapon Skill
 * 
 * 			Quando colpisce ha il SpiritSpeak / 120 % possibilità di eseguire:
 * 				- Buio sulla vittima;
 * 				- Curse;
 * 				- Poison (lesser)
 * 
 * 			L'arma dura per SpiritSpeak secondi e puo' essere indossata solo
 * 			dai necromanti.
 * 		
 * 			Se danneggia un Paladino gli infligge il 150% dei danni.
 * 
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Network;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public class RodOfIniquitySpell : RPGNecromancerSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
			"Rod Of Iniquity", "In Char An Sanct",
			-1,
			9002,
			true,
			Reagent.PigIron,
			Reagent.BatWing
			);

		private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( RodOfIniquitySpell ),
			"This curse summons the Weapon of the Evil.",
			"Questo potente maleficio evoca per un certo tempo l'arma del maleficio.",
			0x5008
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override int RequiredMana{get { return 15; }}
		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 3.0 ); }}
		public override double DelayOfReuse{get { return 600.0; }}
		public override double RequiredSkill{get { return 40.0; }}
		public override bool BlocksMovement{get { return true; }}

		public RodOfIniquitySpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if( Caster.CanBeginAction( typeof( RodOfIniquitySpell ) ) )
			{
				if( CheckSequence() )
				{
					int level = RPGSpellsSystem.GetPowerLevel( Caster, typeof( RodOfIniquitySpell ) );

					Caster.BeginAction( typeof( RodOfIniquitySpell ) );
					RodOfIniquity rod = new RodOfIniquity( Caster );
					if (level == 1)
						rod.Resource = CraftResource.Walnut;
					else if  (level == 2)
						rod.Resource = CraftResource.Cypress;
					else if  (level == 3)
						rod.Resource = CraftResource.Yew;
					else if  (level == 4)
						rod.Resource = CraftResource.Peach;
					else if  (level == 5)
						rod.Resource = CraftResource.Blood;

					Caster.AddToBackpack( rod );
					Caster.PublicOverheadMessage( MessageType.Regular, 37, true, (Caster.Language == "ITA" ? "*Appare un'arma malefica dal nulla*" : "*A powerfull Iniquity Rod has come*") );

					Timer.DelayCall( GetDelayOfReuse(), new TimerStateCallback( ReleaseRodOfIniquityLock ), Caster );
				}

				FinishSequence();
			}
			else
				Caster.SendMessage( (Caster.Language == "ITA" ? "Devi aspettare un altro pò per evocare quest'arma." : "You cannot summon another rod yet.") );
		}

		private static void ReleaseRodOfIniquityLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( RodOfIniquitySpell ) );
			( (Mobile)state ).SendMessage( (((Mobile)state).Language == "ITA" ? "Puoi evocare un'altra asta malefica." : "You can summon a new rod of iniquity now.") );
		}
	}
}