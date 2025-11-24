using Midgard.Engines.SpellSystem;

namespace Midgard.Engines.Classes
{
    public sealed class LeafWhirlwindDefinition : PowerDefinition
    {
        public LeafWhirlwindDefinition()
            : base( typeof( LeafWhirlwindSpell ), "leaf whirlwind", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class HollowReedSpellDefinition : PowerDefinition
    {
        public HollowReedSpellDefinition()
            : base( typeof( HollowReedSpell ), "hollow reed", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class PackOfBeastDefinition : PowerDefinition
    {
        public PackOfBeastDefinition()
            : base( typeof( PackOfBeastSpell ), "pack of beasts", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class SpringOfLifeDefinition : PowerDefinition
    {
        public SpringOfLifeDefinition()
            : base( typeof( SpringOfLifeSpell ), "spring of life", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.FireReq,
                    DruidRitual.WaterReq,
                    DruidRitual.EquilibriumReq,
                },
                0.0
            )
        {
        }

        public override bool IsGranted
        {
            get { return true; }
        }
    }

    public sealed class GraspingRootsDefinition : PowerDefinition
    {
        public GraspingRootsDefinition()
            : base( typeof( GraspingRootsSpell ), "grasping roots", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.TimeReq,
                    DruidRitual.EarthReq,
                    DruidRitual.WaterReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class BlendWithForestdefDefinition : PowerDefinition
    {
        public BlendWithForestdefDefinition()
            : base( typeof( BlendWithForestSpell ), "bl. with forest", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.TimeReq,
                    DruidRitual.FireReq,
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class SwarmOfInsectsdefDefinition : PowerDefinition
    {
        public SwarmOfInsectsdefDefinition()
            : base( typeof( SwarmOfInsectsSpell ), "swarm of insects", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class VolcanicEruptionDefinition : PowerDefinition
    {
        public VolcanicEruptionDefinition()
            : base( typeof( VolcanicEruptionSpell ), "volcanic eruption", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class DruidFamiliarDefinition : PowerDefinition
    {
        public DruidFamiliarDefinition()
            : base( typeof( DruidFamiliarSpell ), "summ. familiar", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.EquilibriumReq,
                    DruidRitual.FireReq,
                    DruidRitual.TimeReq,
                },
                0.0
            )
        {
        }

        public override bool IsGranted
        {
            get { return true; }
        }
    }

    public sealed class StoneCircleDefinition : PowerDefinition
    {
        public StoneCircleDefinition()
            : base( typeof( StoneCircleSpell ), "stone circle", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class EnchantedGroveDefinition : PowerDefinition
    {
        public EnchantedGroveDefinition()
            : base( typeof( EnchantedGroveSpell ), "ench. grove", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.TimeReq,
                    DruidRitual.FireReq,
                    DruidRitual.EquilibriumReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class LureStoneCircleDefinition : PowerDefinition
    {
        public LureStoneCircleDefinition()
            : base( typeof( LureStoneSpell ), "lure stone", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class NaturesPassageDefinition : PowerDefinition
    {
        public NaturesPassageDefinition()
            : base( typeof( NaturesPassageSpell ), "nature passage", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class MushroomGatewayDefinition : PowerDefinition
    {
        public MushroomGatewayDefinition()
            : base( typeof( MushroomGatewaySpell ), "mush. gateway", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class RestorativeSoilDefinition : PowerDefinition
    {
        public RestorativeSoilDefinition()
            : base( typeof( RestorativeSoilSpell ), "restorative soil", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class ShieldOfEarthDefinition : PowerDefinition
    {
        public ShieldOfEarthDefinition()
            : base( typeof( ShieldOfEarthSpell ), "s. of earth", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.FireReq,
                    DruidRitual.AirReq,
                    DruidRitual.EarthReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class BarkSkinDefinition : PowerDefinition
    {
        public BarkSkinDefinition()
            : base( typeof( BarkSkinSpell ), "bark skin", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class DelayedPoisonDefinition : PowerDefinition
    {
        public DelayedPoisonDefinition()
            : base( typeof( DelayedPoisonSpell ), "delayed poison", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class GoodBerryDefinition : PowerDefinition
    {
        public GoodBerryDefinition()
            : base( typeof( GoodBerrySpell ), "good berry", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                },
                0.0
            )
        {
        }
    }


    public sealed class DruidCircleDefinition : PowerDefinition
    {
        public DruidCircleDefinition()
            : base( typeof( DruidCircleSpell ), "druid circle", 5,
                    new RequirementDefinition[]
                {
                },
                0.0
            )
        {
        }

        public override bool IsGranted
        {
            get { return true; }
        }
    }

    public sealed class GiftOfRenewalDefinition : PowerDefinition
    {
        public GiftOfRenewalDefinition()
            : base( typeof( GiftOfRenewalSpell ), "gift of renew.", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.TimeReq,
                    DruidRitual.EarthReq,
                    DruidRitual.WaterReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class ImmolatingWeaponDefinition : PowerDefinition
    {
        public ImmolatingWeaponDefinition()
            : base( typeof( ImmolatingWeaponSpell ), "immolating w.", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.FireReq,
                    DruidRitual.EarthReq,
                    DruidRitual.EquilibriumReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class AttuneWeaponDefinition : PowerDefinition
    {
        public AttuneWeaponDefinition()
            : base( typeof( AttuneWeaponSpell ), "attune weapon", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.TimeReq,
                    DruidRitual.AirReq,
                    DruidRitual.EquilibriumReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class ThunderstormDefinition : PowerDefinition
    {
        public ThunderstormDefinition()
            : base( typeof( ThunderstormSpell ), "thunderstorm", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.FireReq,
                    DruidRitual.AirReq,
                    DruidRitual.WaterReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class NatureFuryDefinition : PowerDefinition
    {
        public NatureFuryDefinition()
            : base( typeof( NatureFurySpell ), "nature's fury", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                    DruidRitual.EarthReq,
                    DruidRitual.EquilibriumReq,
                },
                0.0
            )
        {
        }

        public override bool IsGranted
        {
            get { return true; }
        }
    }

    public sealed class ReaperFormDefinition : PowerDefinition
    {
        public ReaperFormDefinition()
            : base( typeof( ReaperFormSpell ), "reaper form", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.TimeReq,
                    DruidRitual.EarthReq,
                    DruidRitual.EquilibriumReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class WildfireDefinition : PowerDefinition
    {
        public WildfireDefinition()
            : base( typeof( WildfireSpell ), "wildfire", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.FireReq,
                    DruidRitual.AirReq,
                    DruidRitual.EquilibriumReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class EssenceOfWindDefinition : PowerDefinition
    {
        public EssenceOfWindDefinition()
            : base( typeof( EssenceOfWindSpell ), "ess. of wind", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.TimeReq,
                    DruidRitual.AirReq,
                    DruidRitual.WaterReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class DryadAllureDefinition : PowerDefinition
    {
        public DryadAllureDefinition()
            : base( typeof( DryadAllureSpell ), "dryad allure", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.FireReq,
                    DruidRitual.AirReq,
                    DruidRitual.WaterReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class EtherealVoyageDefinition : PowerDefinition
    {
        public EtherealVoyageDefinition()
            : base( typeof( EtherealVoyageSpell ), "ethereal voyage", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.TimeReq,
                    DruidRitual.AirReq,
                    DruidRitual.WaterReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class WordOfDeathDefinition : PowerDefinition
    {
        public WordOfDeathDefinition()
            : base( typeof( WordOfDeathSpell ), "word of death", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.TimeReq,
                    DruidRitual.EarthReq,
                    DruidRitual.EquilibriumReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class GiftOfLifeDefinition : PowerDefinition
    {
        public GiftOfLifeDefinition()
            : base( typeof( GiftOfLifeSpell ), "gift of life", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.TimeReq,
                    DruidRitual.EarthReq,
                    DruidRitual.WaterReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class DruidEmpowermentDefinition : PowerDefinition
    {
        public DruidEmpowermentDefinition()
            : base( typeof( DruidEmpowermentSpell ), "druid emp.", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.FireReq,
                    DruidRitual.EarthReq,
                    DruidRitual.WaterReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class AnimalFormDefinition : PowerDefinition
    {
        public AnimalFormDefinition()
            : base( typeof( AnimalFormSpell ), "animal form", 5,
                    new RequirementDefinition[]
                {
                    DruidRitual.AirReq,
                    DruidRitual.EarthReq,
                    DruidRitual.WaterReq,
                },
                0.0
            )
        {
        }
    }
}