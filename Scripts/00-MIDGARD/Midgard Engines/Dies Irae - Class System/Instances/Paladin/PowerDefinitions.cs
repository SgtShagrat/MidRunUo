using Midgard.Engines.SpellSystem;

namespace Midgard.Engines.Classes
{
    public sealed class SwordOfLightDefinition : PowerDefinition
    {
        /// <summary>
        /// Sword of Light / 257 2 Spirituality 5 Honesty 7 Humilty
        /// </summary>
        public SwordOfLightDefinition()
            : base( typeof( SwordOfLightSpell ), "sword of light", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.SpiritualityReq,
                    PaladinRitual.HonestyReq,
                    PaladinRitual.HumiltyReq
                },
                0.0
            )
        {
        }
    }

    public sealed class BanishEvilDefinition : PowerDefinition
    {
        /// <summary>
        /// Banish Evil / 378 3 Sacrifice 7 Humilty 8 Justice
        /// </summary>
        public BanishEvilDefinition()
            : base( typeof( BanishEvilSpell ), "banish evil", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.SacrificeReq,
                    PaladinRitual.HumiltyReq,
                    PaladinRitual.JusticeReq
                },
                0.0
            )
        {
        }
    }

    public sealed class BlessedDropsDefinition : PowerDefinition
    {
        /// <summary>
        /// Blessed Drops / 568 5 Honesty 6 Valor 8 Justice 
        /// </summary>
        public BlessedDropsDefinition()
            : base( typeof( BlessedDropsSpell ), "blessed drops", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.HonestyReq,
                    PaladinRitual.ValorReq,
                    PaladinRitual.JusticeReq
                },
                0.0
            )
        {
        }
    }

    public sealed class HolyMountDefinition : PowerDefinition
    {
        /// <summary>
        /// Holy Mount / 238 2 Spirituality 3 Sacrifice 8 Justice
        /// </summary>
        public HolyMountDefinition()
            : base( typeof( HolyMountSpell ), "holy mount", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.SpiritualityReq,
                    PaladinRitual.SacrificeReq,
                    PaladinRitual.JusticeReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class HolyWillDefinition : PowerDefinition
    {
        /// <summary>
        /// Holy Will / 145 1 Honor 4 Compassion 5 Honesty 
        /// </summary>
        public HolyWillDefinition()
            : base( typeof( HolyWillSpell ), "holy will", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.HonorReq,
                    PaladinRitual.CompassionReq,
                    PaladinRitual.HonestyReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class InvulnerabilityDefinition : PowerDefinition
    {
        /// <summary>
        /// Invulnerability / 135 1 Honor 3 Sacrifice 5 Honesty 
        /// </summary>
        public InvulnerabilityDefinition()
            : base( typeof( InvulnerabilitySpell ), "invulnerability", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.HonorReq,
                    PaladinRitual.SacrificeReq,
                    PaladinRitual.HonestyReq
                },
                0.0
            )
        {
        }
    }

    public sealed class OneEnemyOneShotDefinition : PowerDefinition
    {
        /// <summary>
        /// One En. One Shot / 126 1 Honor 2 Spirituality 6 Valor 
        /// </summary>
        public OneEnemyOneShotDefinition()
            : base( typeof( HolySmiteSpell ), "one en. one shot", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.HonorReq,
                    PaladinRitual.SpiritualityReq,
                    PaladinRitual.ValorReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class PathToHeavenDefinition : PowerDefinition
    {
        /// <summary>
        /// Path To Heaven / 128 1 Honor 2 Spirituality 8 Justice
        /// </summary>
        public PathToHeavenDefinition()
            : base( typeof( PathToHeavenSpell ), "path to heaven", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.HonorReq,
                    PaladinRitual.SpiritualityReq,
                    PaladinRitual.JusticeReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class SacredBeamDefinition : PowerDefinition
    {
        /// <summary>
        /// Sacred Beam / 137 1 Honor 3 Sacrifice 7 Humilty
        /// </summary>
        public SacredBeamDefinition()
            : base( typeof( SacredBeamSpell ), "sacred beam", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.HonorReq,
                    PaladinRitual.SacrificeReq,
                    PaladinRitual.HumiltyReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class SacredFeastDefinition : PowerDefinition
    {
        /// <summary>
        /// Sacred Feast / 345 3 Sacrifice 4 Compassion 5 Honesty 
        /// </summary>
        public SacredFeastDefinition()
            : base( typeof( SacredFeastSpell ), "sacred feast", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.SacrificeReq,
                    PaladinRitual.CompassionReq,
                    PaladinRitual.HonestyReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class ShieldOfRighteousnessDefinition : PowerDefinition
    {
        /// <summary>
        /// Shield of Right / 246 2 Spirituality 4 Compassion 6 Valor
        /// </summary>
        public ShieldOfRighteousnessDefinition()
            : base( typeof( ShieldOfRighteousnessSpell ), "shield of right.", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.SpiritualityReq,
                    PaladinRitual.CompassionReq,
                    PaladinRitual.ValorReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class LayOfHandsDefinition : PowerDefinition
    {
        /// <summary>
        /// Lay of Hands / 347 3 Sacrifice 4 Compassion 7 Humilty
        /// </summary>
        public LayOfHandsDefinition()
            : base( typeof( LayOfHandsSpell ), "lay of hands", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.SacrificeReq,
                    PaladinRitual.CompassionReq,
                    PaladinRitual.HumiltyReq,
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

    public sealed class CurePoisonDefinition : PowerDefinition
    {
        /// <summary>
        /// Cure Poison / 248 24 28 48 2 Spirituality 4 Compassion 8 Justice
        /// </summary>
        public CurePoisonDefinition()
            : base( typeof( CurePoisonSpell ), "cure poison", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.SpiritualityReq,
                    PaladinRitual.CompassionReq,
                    PaladinRitual.JusticeReq,
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

    public sealed class ChalmChaosDefinition : PowerDefinition
    {
        /// <summary>
        /// Chalm Chaos / 167 1 Honor 6 Valor 7 Humilty
        /// </summary>
        public ChalmChaosDefinition()
            : base( typeof( ChalmChaosSpell ), "chalm chaos", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.HonorReq,
                    PaladinRitual.ValorReq,
                    PaladinRitual.HumiltyReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class HolyCircleDefinition : PowerDefinition
    {
        /// <summary>
        /// Holy Circle / 468 4 Compassion 6 Valor 8 Justice
        /// </summary>
        public HolyCircleDefinition()
            : base( typeof( HolyCircleSpell ), "holy circle", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.CompassionReq,
                    PaladinRitual.ValorReq,
                    PaladinRitual.JusticeReq,
                },
                0.0
            )
        {
        }
    }

    public sealed class LegalThoughtsDefinition : PowerDefinition
    {
        /// <summary>
        /// Legal Thoughts / 567 5 Honesty 6 Valor 7 Humilty
        /// </summary>
        public LegalThoughtsDefinition()
            : base( typeof( LegalThoughtsSpell ), "legal thoughts", 5,
                    new RequirementDefinition[]
                {
                    PaladinRitual.HonestyReq,
                    PaladinRitual.ValorReq,
                    PaladinRitual.HumiltyReq,
                },
                0.0
            )
        {
        }
    }
}