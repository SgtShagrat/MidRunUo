using System;

using Midgard.Engines.SpellSystem;

using Server;
using Server.Gumps;

namespace Midgard.Engines.Classes
{
    public sealed class DruidSystem : ClassSystem
    {
        private static readonly Type[] m_RitualItemsTypes = new Type[]{
                                                              typeof ( AirSigil ),
                                                              typeof ( EarthSigil ),
                                                              typeof ( FireSigil ),
                                                              typeof ( WaterSigil ),
                                                              typeof ( TimeSigil ),
                                                              typeof ( EquilibriumSigil )
                                                              };
        public DruidSystem()
        {
            Definition = new ClassDefinition( "Druid",
                                                Classes.Druid,
                                                0,
                                                DefaultWelcomeMessage,
                                                new PowerDefinition[]
                                                {
                                                    // new LeafWhirlwindDefinition(),
                                                    // new HollowReedSpellDefinition(),
                                                    // new PackOfBeastDefinition(),
                                                    new SpringOfLifeDefinition(),
                                                    new GraspingRootsDefinition(),
                                                    new BlendWithForestdefDefinition(),
                                                    // new SwarmOfInsectsdefDefinition(),
                                                    // new VolcanicEruptionDefinition(),
                                                    new DruidFamiliarDefinition(),
                                                    // new StoneCircleDefinition(),
                                                    new EnchantedGroveDefinition(),
                                                    // new LureStoneCircleDefinition(),
                                                    // new NaturesPassageDefinition(),
                                                    // new MushroomGatewayDefinition(),
                                                    // new RestorativeSoilDefinition(),
                                                    new ShieldOfEarthDefinition(),
                                                    // new BarkSkinDefinition(),
                                                    // new DelayedPoisonDefinition(),
                                                    // new GoodBerryDefinition(),

                                                    new DruidCircleDefinition(),
                                                    new GiftOfRenewalDefinition(),
                                                    new ImmolatingWeaponDefinition(),
                                                    new AttuneWeaponDefinition(),
                                                    new ThunderstormDefinition(),
                                                    new NatureFuryDefinition(),
                                                    // new SummonFeyDefinition(),
                                                    // new SummonFiendDefinition(),
                                                    new ReaperFormDefinition(),
                                                    new WildfireDefinition(),
                                                    new EssenceOfWindDefinition(),
                                                    new DryadAllureDefinition(),
                                                    new EtherealVoyageDefinition(),
                                                    new WordOfDeathDefinition(),
                                                    new GiftOfLifeDefinition(),
                                                    new DruidEmpowermentDefinition(),
                                                    new AnimalFormDefinition()
                                                }
            );
        }

        public override void SetStartingSkills( Mobile mobile )
        {
            mobile.Skills[ SkillName.Spellweaving ].Base = 35.0;

            // momento dell'accettazione della sua richiesta verrano 
            // abbassate allo 0% tutte le sue abilità combattive, compreso magery
            NullifyAndLockSkill( mobile, SkillName.Magery );

            NullifyAndLockSkill( mobile, SkillName.Archery );
            NullifyAndLockSkill( mobile, SkillName.Fencing );
            NullifyAndLockSkill( mobile, SkillName.Macing );
            NullifyAndLockSkill( mobile, SkillName.Parry );
            NullifyAndLockSkill( mobile, SkillName.Swords );
            NullifyAndLockSkill( mobile, SkillName.Tactics );
            NullifyAndLockSkill( mobile, SkillName.Wrestling );
        }

        public override void ResetSkillsLocks( Mobile mobile )
        {
            UnlockSkillAndCap( mobile, SkillName.Magery );

            UnlockSkillAndCap( mobile, SkillName.Archery );
            UnlockSkillAndCap( mobile, SkillName.Fencing );
            UnlockSkillAndCap( mobile, SkillName.Macing );
            UnlockSkillAndCap( mobile, SkillName.Parry );
            UnlockSkillAndCap( mobile, SkillName.Swords );
            UnlockSkillAndCap( mobile, SkillName.Tactics );
            UnlockSkillAndCap( mobile, SkillName.Wrestling );
        }

        public static int GetMageryMalus( Mobile m )
        {
            int percent = RPGSpellsSystem.GetTotalPowerPercent( m );

            if( m.PlayerDebug )
                Config.Pkg.LogInfoLine( "Debug: GetMageryMalus for {0}: {1}", m.Name, percent );

            return percent;
        }

        public override BaseClassAttributes GetNewPowerAttributes( ClassPlayerState state )
        {
            return new DruidAttributes( state );
        }

        public override bool IsEligible( Mobile mob )
        {
            // Drow, Vampire, Undead, Orc, HalfDeamon, WereWolf
            if( mob.Race == Races.Core.Drow )
                return false;
            else if( mob.Race == Races.Core.Vampire )
                return false;
            else if( mob.Race == Races.Core.Undead )
                return false;
            else if( mob.Race == Races.Core.HighOrc )
                return false;
            else if( mob.Race == Races.Core.HalfDaemon )
                return false;

            // druid candidate must have 50.0 animal taming and lore
            // do not use skill.Value because it generates an infinite loop
            if( mob.Skills[ SkillName.AnimalTaming ].Base < 50.0 || mob.Skills[ SkillName.AnimalLore ].Base < 50.0 )
                return false;

            return base.IsEligible( mob );
        }

        public override string IsEligibleString( Mobile mob )
        {
            // Drow, Vampire, Undead, Orc, HalfDeamon, WereWolf
            if( mob.Race == Races.Core.Drow )
                return (mob.Language == "ITA" ? "Sei un inaffidabile Drow, non potrai mai diventare Druido." : "You're a disrespectable Drow, you cannot become a Druid.");
            else if( mob.Race == Races.Core.Vampire )
                return (mob.Language == "ITA" ? "Sei un inaffidabile Vampiro, non potrai mai diventare Druido." : "You're a disrespectable Vampire, you cannot become a Druid.");
            else if( mob.Race == Races.Core.Undead )
                return (mob.Language == "ITA" ? "Sei un inaffidabile Non-morto, non potrai mai diventare Druido." : "You're a disrespectable Undead, you cannot become a Druid.");
            else if( mob.Race == Races.Core.HighOrc )
                return (mob.Language == "ITA" ? "Sei un inaffidabile Orco, non potrai mai diventare Druido." : "You're a disrespectable Orc, you cannot become a Druid.");
            else if( mob.Race == Races.Core.HalfDaemon )
                return (mob.Language == "ITA" ? "Sei un inaffidabile Demone, non potrai mai diventare Druido." : "You're a disrespectable Daemon, you cannot become a Druid.");

            // druid candidate must have 50.0 animal taming and lore
            // do not use skill.Value because it generates an infinite loop
            if( mob.Skills[ SkillName.AnimalTaming ].Base < 50.0 )
                return (mob.Language == "ITA" ? "Non hai abbastanza affinità con gli animali per essere un aspirante Druido." : "You don't have enough experience in Animal Taming to become a Druid.");
		if ( mob.Skills[ SkillName.AnimalLore ].Base < 50.0 )
                return (mob.Language == "ITA" ? "Non hai abbastanza affinità con gli animali per essere un aspirante Druido." : "You don't have enough experience in Animal Lore to become a Druid.");

            return base.IsEligibleString( mob );
        }

        public override bool IsAllowedSkill( SkillName skillName )
        {
            if( skillName == SkillName.Magery )
                return /* false */ true;
            else if( skillName == SkillName.Poisoning )
                return false;
            else if( skillName == SkillName.Necromancy )
                return false;
            else if( skillName == SkillName.Chivalry )
                return false;

            return true;
        }

        public override void MakeRitual( Mobile ritualist, PowerDefinition definition )
        {
            DruidRitual ritual = new DruidRitual( definition, ritualist );
            ritual.Start();
        }

        public override int GetSpellLabelHueByLevel( int level )
        {
            switch( level )
            {
                case 0:
                    return DisabledLabelHue;
                case 1:
                    return Colors.Olive;
                case 2:
                    return Colors.OliveDrab;
                case 3:
                    return Colors.ForestGreen;
                case 4:
                    return Colors.Green;
                case 5:
                    return Colors.LawnGreen;
                default:
                    return DisabledLabelHue;
            }
        }

        public override bool SupportsRitualItems
        {
            get { return true; }
        }

        public override RitualItem RandomRitualItem()
        {
            return Loot.Construct( m_RitualItemsTypes ) as RitualItem;
        }
    }
}
