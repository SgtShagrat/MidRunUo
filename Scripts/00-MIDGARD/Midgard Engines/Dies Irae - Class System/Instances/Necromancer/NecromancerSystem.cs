using System;
using Server;
using Server.Gumps;
using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public sealed class NecromancerSystem : ClassSystem
    {
        private static readonly Type[] m_RitualItemsTypes = new Type[]{
                                                              typeof ( SealOfDeath ),
                                                              typeof ( SealOfEvil ),
                                                              typeof ( SealOfFear ),
                                                              typeof ( SealOfPain ),
                                                              typeof ( SealOfRevenge ),
                                                              typeof ( SealOfUndead )
                                                              };

        public NecromancerSystem()
        {
            Definition = new ClassDefinition( "Necromancer",
                                                MidgardClasses.Necromancer,
                                                0,
                                                DefaultWelcomeMessage,
                                                new PowerDefinition[]{
                                                                     new BalanceOfDeathDefinition(),
                                                                     new BloodCircledeDefinition(),
                                                                     new BloodOfferingDefinition(),
                                                                     new BonesThrowDefinition(),
                                                                     new DefilerKissDefinition(),
                                                                     new EvilMountDefinition(),
                                                                     new NecropotenceDefinition(),
                                                                     new RodOfIniquityDefinition(),
                                                                     new TimeOfDeathdefiniDefinition(),
                                                                     new VaultOfBloodDefinition(),

                                                                     new BloodConjunctionDefinition(),
                                                                     new DarkOmenDefinition(),
                                                                     new EvilAvatarDefinition(),
                                                                     new LobotomyDefinition(),
                                                                     new PainStrikeDefinition(),
                                                                     new PoisonSpikeDefinition(),
                                                                     new ChokingDefinition(),
                                                                     new SpiritOfRevengeDefinition()
                                                                     }
            );
        }

        public override void SetStartingSkills( Mobile mobile )
        {
            mobile.Skills[ SkillName.Necromancy ].Base = 35.0;
        }

        public override bool IsEligible( Mobile mob )
        {
            // High Elf, Northern Elf, Human of the North, Mountain Dwarf, Sprite, Fairy, WereWolf, Half Elf, Naglor
            if( mob.Race == Races.Core.HighElf )
                return false;
            else if( mob.Race == Races.Core.NorthernElf )
                return false;
            else if( mob.Race == Races.Core.NorthernHuman )
                return false;
            else if( mob.Race == Races.Core.MountainDwarf )
                return false;
            else if( mob.Race == Races.Core.Sprite )
                return false;
            else if( mob.Race == Races.Core.FairyOfAir )
                return false;
            else if( mob.Race == Races.Core.FairyOfEarth )
                return false;
            else if( mob.Race == Races.Core.FairyOfFire )
                return false;
            else if( mob.Race == Races.Core.FairyOfWater )
                return false;
            else if( mob.Race == Races.Core.FairyOfWood )
                return false;
            else if( mob.Race == Races.Core.Werewolf )
                return false;
            else if( mob.Race == Races.Core.HalfElf )
                return false;
            else if( mob.Race == Races.Core.Naglor )
                return false;

            return base.IsEligible( mob );
        }

        public override string IsEligibleString( Mobile mob )
        {
            if( mob.Race == Races.Core.HighElf )
                return (mob.Language == "ITA" ? "Sei un Elfo, non potrai mai diventare Necromante." : "You're an Elf, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.NorthernElf )
                return (mob.Language == "ITA" ? "Sei un Elfo, non potrai mai diventare Necromante." : "You're an Elf, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.NorthernHuman )
                return (mob.Language == "ITA" ? "Sei un Nord, non potrai mai diventare Necromante." : "You're a Nord, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.MountainDwarf )
                return (mob.Language == "ITA" ? "Sei un Nano, non potrai mai diventare Necromante." : "You're a Dwarf, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.Sprite )
                return (mob.Language == "ITA" ? "Sei un Folletto, non potrai mai diventare Necromante." : "You're a Sprite, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.FairyOfAir )
                return (mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Necromante." : "You're a Fairy, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.FairyOfEarth )
                return (mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Necromante." : "You're a Fairy, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.FairyOfFire )
                return (mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Necromante." : "You're a Fairy, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.FairyOfWater )
                return (mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Necromante." : "You're a Fairy, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.FairyOfWood )
                return (mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Necromante." : "You're a Fairy, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.Werewolf )
                return (mob.Language == "ITA" ? "Sei un Lupo Mannaro, non potrai mai diventare Necromante." : "You're a Werewolf, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.HalfElf )
                return (mob.Language == "ITA" ? "Sei un Elfo, non potrai mai diventare Necromante." : "You're an Elf, you cannot become a Necromancer.");
            else if( mob.Race == Races.Core.Naglor )
                return (mob.Language == "ITA" ? "Sei un Naglor, non potrai mai diventare Necromante." : "You're a Naglor, you cannot become a Necromancer.");

            return base.IsEligibleString( mob );
        }

        public override bool IsAllowedSkill( SkillName skillName )
        {
            if( skillName == SkillName.Macing )
                return false;
            else if( skillName == SkillName.Swords )
                return false;
            else if( skillName == SkillName.Fencing )
                return false;
            else if( skillName == SkillName.Archery )
                return false;
            else if( skillName == SkillName.Chivalry )
                return false;
            else if( skillName == SkillName.Spellweaving )
                return false;

            return true;
        }

        public override void MakeRitual( Mobile ritualist, PowerDefinition definition )
        {
            NecromancerRitual ritual = new NecromancerRitual( definition, ritualist );
            ritual.Start();
        }

        public override bool SupportsRitualItems
        {
            get { return true; }
        }

        public override RitualItem RandomRitualItem()
        {
            return Loot.Construct( m_RitualItemsTypes ) as RitualItem;
        }

        public override int GetSpellLabelHueByLevel( int level )
        {
            switch( level )
            {
                case 0:
                    return DisabledLabelHue;
                case 1:
                    return Colors.Orange;
                case 2:
                    return Colors.Darkorange;
                case 3:
                    return Colors.DarkGoldenRod;
                case 4:
                    return Colors.GoldenRod;
                case 5:
                    return Colors.Gold;
                default:
                    return DisabledLabelHue;
            }
        }
    }
}