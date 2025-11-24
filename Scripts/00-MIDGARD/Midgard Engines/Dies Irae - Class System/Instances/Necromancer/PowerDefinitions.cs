using Midgard.Engines.SpellSystem;

namespace Midgard.Engines.Classes
{
    public sealed class BalanceOfDeathDefinition : PowerDefinition
    {
        public BalanceOfDeathDefinition()
            : base( typeof( BalanceOfDeathSpell ), "balance of death", 5,
                    new RequirementDefinition[]
                {
                    NecromancerRitual.EvilReq,
                    NecromancerRitual.DeathReq,
                    NecromancerRitual.RevengeReq
                },
                0.0
            )
        {
        }
    }

    public sealed class BloodCircledeDefinition : PowerDefinition
    {
        public BloodCircledeDefinition()
            : base( typeof( BloodCircleSpell ), "blood circle", 5,
                    new RequirementDefinition[]
                {
                    NecromancerRitual.PainReq,
                    NecromancerRitual.UndeadReq,
                    NecromancerRitual.FearReq
                },
                0.0
            )
        {
        }
    }

    public sealed class BloodOfferingDefinition : PowerDefinition
    {
        public BloodOfferingDefinition()
            : base( typeof( BloodOfferingSpell ), "blood offering", 5,
                    new RequirementDefinition[]
                {
                    NecromancerRitual.PainReq,
                    NecromancerRitual.DeathReq,
                    NecromancerRitual.UndeadReq
                },
                0.0
            )
        {
        }
    }

    public sealed class BonesThrowDefinition : PowerDefinition
    {
        public BonesThrowDefinition()
            : base( typeof( BonesThrowSpell ), "bone throw", 5,
                    new RequirementDefinition[]
                {
                    NecromancerRitual.PainReq,
                    NecromancerRitual.UndeadReq,
                    NecromancerRitual.RevengeReq
                },
                0.0
            )
        {
        }
    }

    public sealed class DefilerKissDefinition : PowerDefinition
    {
        public DefilerKissDefinition()
            : base( typeof( DefilerKissSpell ), "defiler kiss", 5,
                    new RequirementDefinition[]
                {
                    NecromancerRitual.EvilReq,
                    NecromancerRitual.DeathReq,
                    NecromancerRitual.FearReq
                },
                0.0
            )
        {
        }
    }

    public sealed class EvilMountDefinition : PowerDefinition
    {
        public EvilMountDefinition()
            : base( typeof( EvilMountSpell ), "evil mount", 5,
                    new RequirementDefinition[]
                {
                    NecromancerRitual.EvilReq,
                    NecromancerRitual.UndeadReq,
                    NecromancerRitual.FearReq
                },
                0.0
            )
        {
        }
    }

    public sealed class NecropotenceDefinition : PowerDefinition
    {
        public NecropotenceDefinition()
            : base( typeof( NecropotenceSpell ), "necropotence", 5,
                    new RequirementDefinition[]
                {
                    NecromancerRitual.EvilReq,
                    NecromancerRitual.DeathReq,
                    NecromancerRitual.UndeadReq
                },
                0.0
            )
        {
        }
    }

    public sealed class RodOfIniquityDefinition : PowerDefinition
    {
        public RodOfIniquityDefinition()
            : base( typeof( RodOfIniquitySpell ), "rod of iniquity", 5,
                    new RequirementDefinition[]
                {
                    NecromancerRitual.EvilReq,
                    NecromancerRitual.FearReq,
                    NecromancerRitual.RevengeReq
                },
                0.0
            )
        {
        }
    }

    public sealed class TimeOfDeathdefiniDefinition : PowerDefinition
    {
        public TimeOfDeathdefiniDefinition()
            : base( typeof( TimeOfDeathSpell ), "time of death", 5,
                    new RequirementDefinition[]
                {
                    NecromancerRitual.EvilReq,
                    NecromancerRitual.DeathReq,
                    NecromancerRitual.RevengeReq
                },
                0.0
            )
        {
        }
    }

    public sealed class VaultOfBloodDefinition : PowerDefinition
    {
        public VaultOfBloodDefinition()
            : base( typeof( VaultOfBloodSpell ), "vault of blood", 5,
                    new RequirementDefinition[]
                {
                    NecromancerRitual.PainReq,
                    NecromancerRitual.EvilReq,
                    NecromancerRitual.DeathReq
                },
                0.0
            )
        {
        }
    }

    #region osi like spells
    public sealed class BloodConjunctionDefinition : PowerDefinition
    {
        public BloodConjunctionDefinition()
            : base( typeof( BloodConjunctionSpell ), "blood conjunct.", 5,
                    new RequirementDefinition[]
                    {
                        NecromancerRitual.PainReq,
                        NecromancerRitual.DeathReq,
                        NecromancerRitual.UndeadReq
                    },
                    0.0
                )
        {
        }
        public override bool FirstLevelGranted
        {
            get { return true; }
        }
    }

    public sealed class DarkOmenDefinition : PowerDefinition
    {
        public DarkOmenDefinition()
            : base( typeof( DarkOmenSpell ), "dark omen", 5,
                    new RequirementDefinition[]
                    {
                        NecromancerRitual.EvilReq,
                        NecromancerRitual.FearReq,
                        NecromancerRitual.RevengeReq
                    },
                    0.0
                )
        {
        }
    }

    public sealed class EvilAvatarDefinition : PowerDefinition
    {
        public EvilAvatarDefinition()
            : base( typeof( EvilAvatarSpell ), "evil avatar", 5,
                    new RequirementDefinition[]
                    {
                        NecromancerRitual.PainReq,
                        NecromancerRitual.EvilReq,
                        NecromancerRitual.DeathReq
                    },
                    0.0
                )
        {
        }
    }

    public sealed class LobotomyDefinition : PowerDefinition
    {
        public LobotomyDefinition()
            : base( typeof( LobotomySpell ), "lobotomy", 5,
                    new RequirementDefinition[]
                    {
                        NecromancerRitual.PainReq,
                        NecromancerRitual.UndeadReq,
                        NecromancerRitual.FearReq
                    },
                    0.0
                )
        {
        }
    }

    public sealed class PainStrikeDefinition : PowerDefinition
    {
        public PainStrikeDefinition()
            : base( typeof( PainStrikeSpell ), "pain strike", 5,
                    new RequirementDefinition[]
                    {
                        NecromancerRitual.PainReq,
                        NecromancerRitual.EvilReq,
                        NecromancerRitual.RevengeReq
                    },
                    0.0
                )
        {
        }

        public override bool FirstLevelGranted
        {
            get { return true; }
        }
    }

    public sealed class PoisonSpikeDefinition : PowerDefinition
    {
        public PoisonSpikeDefinition()
            : base( typeof( PoisonSpikeSpell ), "poison spike", 5,
                    new RequirementDefinition[]
                    {
                        NecromancerRitual.PainReq,
                        NecromancerRitual.DeathReq,
                        NecromancerRitual.RevengeReq
                    },
                    0.0
                )
        {
        }
    }

    public sealed class ChokingDefinition : PowerDefinition
    {
        public ChokingDefinition()
            : base( typeof( ChokingSpell ), "choking", 5,
                    new RequirementDefinition[]
                    {
                        NecromancerRitual.PainReq,
                        NecromancerRitual.DeathReq,
                        NecromancerRitual.FearReq
                    },
                    0.0
                )
        {
        }
    }

    public sealed class SpiritOfRevengeDefinition : PowerDefinition
    {
        public SpiritOfRevengeDefinition()
            : base( typeof( SpiritOfRevengeSpell ), "sp. of revenge", 5,
                    new RequirementDefinition[]
                    {
                        NecromancerRitual.EvilReq,
                        NecromancerRitual.UndeadReq,
                        NecromancerRitual.FearReq
                    },
                    0.0
                )
        {
        }
    }
    #endregion
}