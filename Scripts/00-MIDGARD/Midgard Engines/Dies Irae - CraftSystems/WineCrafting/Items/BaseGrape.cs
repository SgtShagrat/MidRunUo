using System;
using Midgard.Engines.BrewCrafing;
using Midgard.Engines.PlantSystem;
using Server;
using Server.Items;

namespace Midgard.Engines.WineCrafting
{
    public abstract class BaseGrape : BasePlant, ISowable
    {
        #region proprietà di ISowable
        public virtual bool CanGrowFarm { get { return true; } }
        public virtual bool CanGrowDirt { get { return true; } }
        public virtual bool CanGrowGround { get { return true; } }
        public virtual bool CanGrowSwamp { get { return false; } }
        public virtual bool CanGrowSand { get { return false; } }
        public virtual bool CanGrowGarden { get { return false; } }

        public virtual double RequiredSkillToPlant { get { return 0.0; } }
        #endregion

        public virtual BrewVariety DefaultVariety { get { return BrewVariety.CabernetSauvignon; } }

        private BrewVariety m_Variety;

        [CommandProperty( AccessLevel.GameMaster )]
        public BrewVariety Variety
        {
            get { return m_Variety; }
            set { m_Variety = value; InvalidateProperties(); }
        }

        #region properties from BasePlant
        // General variables about plants
        public override bool IsDestroyable { get { return true; } }
        public override bool NeedWater { get { return true; } }

        // Variables concerning plant evolution and death
        public override bool CanGrow { get { return true; } }
        public override int GrowthTick { get { return 24; } }
        public override bool LimitedLifeSpan { get { return true; } }
        public override TimeSpan LifeSpan { get { return TimeSpan.FromDays( 30 ); } }
        public override TimeSpan LongDrought { get { return TimeSpan.FromDays( 14 ); } }
        public override TimeSpan DormantDrought { get { return TimeSpan.FromDays( 5 ); } }

        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD94 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "an grape wine sapling", "a young grape wine", "a grape wine" }; } }

        // Variables concerning produce action
        public override bool CanProduce { get { return true; } }
        public override int ProduceTick { get { return 1; } }
        public override int Capacity { get { return 5; } }
        public override string CropName { get { return "grape"; } }
        public override string CropPluralName { get { return "grapes"; } }
        public override int ProductPhaseID { get { return 0; } }

        // Variables concerning harvesting
        public override double MinSkillToHarvest { get { return GetHarvestSkill(); } }
        public override bool HasParentSeed { get { return false; } }
        public override Type TypeOfParentSeed { get { return null; } }
        public override Type HarvestingTool { get { return typeof( Fists ); } }
        public override double HarvestDelay { get { return 2.0; } }
        public override bool HarvestInPack { get { return true; } }
        public override double MinDiffSkillToCare { get { return GetHarvestSkill() - 30.0; } }
        #endregion

        #region costruttori
        [Constructable]
        public BaseGrape( Mobile owner )
            : base( 1, owner )
        {
            Movable = false;
            m_Variety = DefaultVariety;
        }

        public BaseGrape( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void AddNameProperty( ObjectPropertyList list )
        {
            list.Add( 1060658, "Variety\t{0}", BrewingResources.GetName( m_Variety ) );

            base.AddNameProperty( list );
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, "Variety : {0} ", BrewingResources.GetName( m_Variety ) );
        }

        public double GetHarvestSkill()
        {
            switch( m_Variety )
            {
                case BrewVariety.CabernetSauvignon: return 80.0;
                case BrewVariety.Chardonnay: return 80.0;
                case BrewVariety.CheninBlanc: return 80.0;
                case BrewVariety.Merlot: return 80.0;
                case BrewVariety.Riesling: return 80.0;
                case BrewVariety.Sangiovese: return 80.0;
                case BrewVariety.SauvignonBlanc: return 80.0;
                case BrewVariety.Shiraz: return 80.0;
                case BrewVariety.Viognier: return 80.0;
                case BrewVariety.Zinfandel: return 80.0;
                default: return 80.0;
            }
        }

        public override Item GetCropObject()
        {
            switch( m_Variety )
            {
                case BrewVariety.CabernetSauvignon: return new CabernetSauvignonGrapes( 1 );
                case BrewVariety.Chardonnay: return new ChardonnayGrapes( 1 );
                case BrewVariety.CheninBlanc: return new CheninBlancGrapes( 1 );
                case BrewVariety.Merlot: return new MerlotGrapes( 1 );
                case BrewVariety.Riesling: return new RieslingGrapes( 1 );
                case BrewVariety.Sangiovese: return new SangioveseGrapes( 1 );
                case BrewVariety.SauvignonBlanc: return new SauvignonBlancGrapes( 1 );
                case BrewVariety.Shiraz: return new ShirazGrapes( 1 );
                case BrewVariety.Viognier: return new ViognierGrapes( 1 );
                case BrewVariety.Zinfandel: return new ZinfandelGrapes( 1 );
                default: return new Grapes( 1 );
            }
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );

            writer.Write( (int)m_Variety );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            m_Variety = (BrewVariety)reader.ReadInt();
        }
        #endregion
    }

    #region grapevine branch
    public class GrapeVineBranchEast : BaseGrape
    {
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD23 }; } }

        [Constructable]
        public GrapeVineBranchEast( Mobile owner )
            : base( owner )
        {
            Name = "East Branch (1)";
        }

        public GrapeVineBranchEast( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class GrapeVineBranchEast2 : BaseGrape
    {
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD24 }; } }

        [Constructable]
        public GrapeVineBranchEast2( Mobile owner )
            : base( owner )
        {
            Name = "East Branch (2)";
        }

        public GrapeVineBranchEast2( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class GrapeVineBranchNorth : BaseGrape
    {
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD1E }; } }

        [Constructable]
        public GrapeVineBranchNorth( Mobile owner )
            : base( owner )
        {
            Name = "North Branch (1)";
        }

        public GrapeVineBranchNorth( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class GrapeVineBranchNorth2 : BaseGrape
    {
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD1F }; } }

        [Constructable]
        public GrapeVineBranchNorth2( Mobile owner )
            : base( owner )
        {
            Name = "North Branch (2)";
        }

        public GrapeVineBranchNorth2( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
    #endregion

    #region grapevine pole
    public class GrapeVinePoleEast : BaseGrape
    {
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD20 }; } }

        [Constructable]
        public GrapeVinePoleEast( Mobile owner )
            : base( owner )
        {
            Name = "East Pole (End 2)";
        }

        public GrapeVinePoleEast( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class GrapeVinePoleEast2 : BaseGrape
    {
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD21 }; } }

        [Constructable]
        public GrapeVinePoleEast2( Mobile owner )
            : base( owner )
        {
            Name = "East Pole (Center)";
        }

        public GrapeVinePoleEast2( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class GrapeVinePoleEast3 : BaseGrape
    {
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD22 }; } }

        [Constructable]
        public GrapeVinePoleEast3( Mobile owner )
            : base( owner )
        {
            Name = "East Pole (End 1)";
        }

        public GrapeVinePoleEast3( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class GrapeVinePoleNorth : BaseGrape
    {
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD1B }; } }

        [Constructable]
        public GrapeVinePoleNorth( Mobile owner )
            : base( owner )
        {
            Name = "North Pole (End 2)";
        }

        public GrapeVinePoleNorth( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class GrapeVinePoleNorth2 : BaseGrape
    {
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD1D }; } }

        [Constructable]
        public GrapeVinePoleNorth2( Mobile owner )
            : base( owner )
        {
            Name = "North Pole (End 1)";
        }

        public GrapeVinePoleNorth2( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class GrapeVinePoleNorth3 : BaseGrape
    {
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD1C }; } }

        [Constructable]
        public GrapeVinePoleNorth3( Mobile owner )
            : base( owner )
        {
            Name = "North Pole (Center)";
        }

        public GrapeVinePoleNorth3( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
    #endregion
}