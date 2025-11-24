/***************************************************************************
 *                                  RPGSpellscs
 *                            		-----------------------
 *  begin                	: Gennaio, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;

using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public class RPGSpellsInitializer
	{
		public static void RegisterSpells()
		{
			//		Registrazioni standard
			//
			// 0-63 		Spell Mago standard
			// 100-116 		Spell Necro standard
			// 200-209		Spell Paladino standard
			// 400-405		Bushido
			// 500-507		Nunjitsu
			//
			
			//		Registrazioni MidgardRPG
			//
			// 150-159		Spell Necro RPG
			// 250-265 		Spell Paladino RPG
			// 301-312		Spell Chierico
			// 551-569 		Spell Druido
			// 

			#region Necromante
			Register( 150, typeof( BalanceOfDeathSpell ) );
			Register( 151, typeof( BloodCircleSpell ) );
			Register( 152, typeof( BloodOfferingSpell ) );
			Register( 153, typeof( BonesThrowSpell ) );
			Register( 154, typeof( DefilerKissSpell ) );
			Register( 155, typeof( EvilMountSpell ) );
			Register( 156, typeof( NecropotenceSpell ) );
			Register( 157, typeof( RodOfIniquitySpell ) );
			Register( 158, typeof( TimeOfDeathSpell ) );
			Register( 159, typeof( VaultOfBloodSpell ) );

            Register( 160, typeof( BloodConjunctionSpell ) );
            Register( 161, typeof( DarkOmenSpell ) );
            Register( 162, typeof( EvilAvatarSpell ) );
            Register( 163, typeof( LobotomySpell ) );
            Register( 164, typeof( PainStrikeSpell ) );
            Register( 165, typeof( PoisonSpikeSpell ) );
            Register( 166, typeof( ChokingSpell ) );
            Register( 167, typeof( SpiritOfRevengeSpell ) );
			#endregion

			#region Druido
			// Register( 551, typeof( LeafWhirlwindSpell ) );
            // Register( 552, typeof( HollowReedSpell ) );
            // Register( 553, typeof( PackOfBeastSpell ) );
		    Register( 554, typeof( SpringOfLifeSpell ) );
		    Register( 555, typeof( GraspingRootsSpell ) );
		    Register( 556, typeof( BlendWithForestSpell ) );
            // Register( 557, typeof( SwarmOfInsectsSpell ) );
            // Register( 558, typeof( VolcanicEruptionSpell ) );
            Register( 559, typeof( DruidFamiliarSpell ) );
            // Register( 560, typeof( StoneCircleSpell ) );
		    Register( 561, typeof( EnchantedGroveSpell ) );
            // Register( 562, typeof( LureStoneSpell ) );
            // Register( 563, typeof( NaturesPassageSpell ) );
            // Register( 564, typeof( MushroomGatewaySpell ) );
            // Register( 565, typeof( RestorativeSoilSpell ) );
		    Register( 566, typeof( ShieldOfEarthSpell ) );
            // Register( 567, typeof( BarkSkinSpell ) );
            // Register( 568, typeof( DelayedPoisonSpell ) );
            // Register( 569, typeof( GoodBerrySpell ) );

            Register( 570, typeof( DruidCircleSpell ) );
            Register( 571, typeof( GiftOfRenewalSpell ) );
            Register( 572, typeof( ImmolatingWeaponSpell ) );
            Register( 573, typeof( AttuneWeaponSpell ) );
            Register( 574, typeof( ThunderstormSpell ) );
            Register( 575, typeof( NatureFurySpell ) );
            // Register( 576, typeof( SummonFeySpell ) );
            // Register( 577, typeof( SummonFiendSpell ) );
            Register( 578, typeof( ReaperFormSpell ) );
            Register( 579, typeof( WildfireSpell ) );
            Register( 580, typeof( EssenceOfWindSpell ) );
            Register( 581, typeof( DryadAllureSpell ) );
            Register( 582, typeof( EtherealVoyageSpell ) );
            Register( 583, typeof( WordOfDeathSpell ) );
            Register( 584, typeof( GiftOfLifeSpell ) );
            Register( 585, typeof( DruidEmpowermentSpell ) );

            Register( 586, typeof( AnimalFormSpell ) );
			#endregion
			
			#region Paladino
			Register( 250, typeof( BanishEvilSpell ) );
			Register( 251, typeof( BlessedDropsSpell ) );
			Register( 252, typeof( HolyMountSpell ) );
			Register( 253, typeof( HolyWillSpell ) );
			Register( 254, typeof( InvulnerabilitySpell ) );
			Register( 255, typeof( HolySmiteSpell ) );
			Register( 256, typeof( PathToHeavenSpell ) );			
			Register( 257, typeof( SacredBeamSpell ) );
			Register( 258, typeof( SacredFeastSpell ) );
			Register( 259, typeof( ShieldOfRighteousnessSpell ) );
			Register( 260, typeof( SwordOfLightSpell ) );
            Register( 261, typeof( LayOfHandsSpell ) );
            Register( 262, typeof( CurePoisonSpell ) );
            Register( 263, typeof( ChalmChaosSpell ) );
            Register( 264, typeof( HolyCircleSpell ) );
            Register( 265, typeof( LegalThoughtsSpell ) );
			#endregion

			#region Chierico
//			Register( 301, typeof( ClericAngelicFaithSpell ) );
//			Register( 302, typeof( ClericBanishEvilSpell ) );
//			Register( 303, typeof( ClericDampenSpiritSpell ) );
//			Register( 304, typeof( ClericDivineFocusSpell ) );
//			Register( 305, typeof( ClericHammerOfFaithSpell ) );
//			Register( 306, typeof( ClericPurgeSpell ) );
//			Register( 307, typeof( ClericRestorationSpell ) );
//			Register( 308, typeof( ClericSacredBoonSpell ) );
//			Register( 309, typeof( ClericSacrificeSpell ) );
//			Register( 310, typeof( ClericSmiteSpell ) );
//			Register( 311, typeof( ClericTouchOfLifeSpell ) );
//			Register( 312, typeof( ClericTrialByFireSpell ) );
			
//			RegDef( 301, "Angelic Faith",   "The priest calls upon the divine powers of the heavens to transform himself into a holy angel.  The priest gains better regeneration rates and increased stats and skills.", null, "Skill: 80; Tithing: 100" );
//			RegDef( 302, "Banish Evil",     "The priest calls forth a divine fire to banish his undead or demonic foe from the earth.", null, "Skill: 60; Tithing: 30" );
//			RegDef( 303, "Dampen Spirit",   "The priest's enemy is slowly drained of his stamina, greatly hindering their ability to fight in combat or flee.", null, "Skill: 35; Tithing: 15" );
//			RegDef( 304, "Divine Focus",    "The priest's mind focuses on his divine faith increasing the effect of his prayers.  However, the priest becomes mentally fatigued much faster.", null, "Skill: 35; Tithing: 15" );
//			RegDef( 305, "Hammer Of Faith", "Summons forth a divine hammer of pure energy blessed with the ability to vanquish undead foes with greater efficiency.", null, "Skill: 40; Tithing: 20" );
//			RegDef( 306, "Purge",           "The target is cured of all poisons and has all negative stat curses removed.", null, "Skill: 10; Tithing: 5" );
//			RegDef( 307, "Restoration",     "The priest's target is resurrected and fully healed and refreshed.", null, "Skill: 50; Tithing: 40" );
//			RegDef( 308, "Sacred Boon",     "The priest's target is surrounded by a divine wind that heals small amounts of lost life over time.", null, "Skill: 25; Tithing: 15" );
//			RegDef( 309, "Sacrifice",       "The priest sacrifices himself for his allies. Whenever damaged, all party members are healed a small percent of the damage dealt. The priest still takes damage.", null, "Skill: 5; Tithing: 5" );
//			RegDef( 310, "Smite",           "The priest calls to the heavens to send a deadly bolt of lightning to strike down his opponent.", null, "Skill: 80; Tithing: 60" );
//			RegDef( 311, "Touch Of Life",   "The priest's target is healed by the heavens for a significant amount.", null, "Skill: 30; Tithing: 10" );
//			RegDef( 312, "Trial By Fire",   "The priest is surrounded by a divine flame that damages the priest's enemy when hit by a weapon.", null, "Skill: 45; Tithing: 25" );	
			#endregion
		}

        public static void Register( int spellID, Type type )
        {
            Initializer.Register( spellID, type );
            RPGSpellsSystem.RegisterCustomSpell( spellID, type );
        }
	}
}