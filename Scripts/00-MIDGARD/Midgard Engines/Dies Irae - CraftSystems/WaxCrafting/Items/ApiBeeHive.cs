using System;

using Midgard.Engines.Apiculture;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class ApiBeeHive : BaseAddon
    {
        public static readonly bool LessWax = true; //wax production is slower then honey (realistic)
        public static readonly int MaxHoney = 20; //maximum amount of honey

        public static readonly int MaxWax = 10; //maximum amount of wax

        private ApiBeesComponent m_Bees; //for displaying bee swarm
        private ApiBeeHiveComponent m_Comp; //for storing the top of the hive

        private int m_Disease; //disease level (0, 1, 2)

        private int m_Health = 10; //current health
        private int m_Honey; //amount of Honey
        private int m_Parasite; //parasite level(0, 1, 2)
        private int m_Population = 1; //bee population (*10k)

        private int m_PotAgility; //number of agility potions
        private int m_PotCure; //number of cure potions
        private int m_PotHeal; //number of heal potions
        private int m_PotPoison; //number of poison potions
        private int m_PotStr; //number of stength potions

        private int m_Wax; //amount of Wax

        [Constructable]
        public ApiBeeHive()
        {
            LastGrowth = 0;
            HiveStage = HiveStatus.Stage1;
            AddComponent( new AddonComponent( 2868 ), 0, 0, 0 ); //table

            m_Comp = new ApiBeeHiveComponent( this );
            AddComponent( m_Comp, 0, 0, +6 );

            m_Bees = new ApiBeesComponent( this );
            m_Bees.Visible = false;
            AddComponent( m_Bees, 0, 0, +6 );
        }

        public ApiBeeHive( Serial serial )
            : base( serial )
        {
            LastGrowth = 0;
            HiveStage = HiveStatus.Stage1;
        }

        public override BaseAddonDeed Deed
        {
            get { return new ApiBeeHiveDeed(); }
        }

        public int HiveAge { get; set; }

        public DateTime NextCheck { get; set; }

        public HiveStatus HiveStage { get; set; }

        public HiveGrowthIndicator LastGrowth { get; set; }

        public int Wax
        {
            get { return m_Wax; }
            set
            {
                if( value < 0 )
                {
                    m_Wax = 0;
                }
                else if( value > MaxWax )
                {
                    m_Wax = MaxWax;
                }
                else
                {
                    m_Wax = value;
                }
            }
        }

        public int Honey
        {
            get { return m_Honey; }
            set
            {
                if( value < 0 )
                {
                    m_Honey = 0;
                }
                else if( value > MaxHoney )
                {
                    m_Honey = MaxHoney;
                }
                else
                {
                    m_Honey = value;
                }
            }
        }

        public int Health
        {
            get { return m_Health; }
            set
            {
                if( value < 0 )
                {
                    m_Health = 0;
                }
                else if( value > MaxHealth )
                {
                    m_Health = MaxHealth;
                }
                else
                {
                    m_Health = value;
                }

                if( m_Health == 0 )
                {
                    Die();
                }

                m_Comp.InvalidateProperties();
            }
        }

        public int MaxHealth
        {
            get { return 10 + ( (int)HiveStage * 2 ); }
        }

        public HiveHealth OverallHealth
        {
            get
            {
                int perc = m_Health * 100 / MaxHealth;

                if( perc < 33 )
                {
                    return HiveHealth.Dying;
                }
                else if( perc < 66 )
                {
                    return HiveHealth.Sickly;
                }
                else if( perc < 100 )
                {
                    return HiveHealth.Healthy;
                }
                else
                {
                    return HiveHealth.Thriving;
                }
            }
        }

        public int Population
        {
            get { return m_Population; }
            set
            {
                if( value < 0 )
                {
                    m_Population = 0;
                }
                else if( value > 10 )
                {
                    m_Population = 10;
                }
                else
                {
                    m_Population = value;
                }
            }
        }

        public int ParasiteLevel
        {
            get { return m_Parasite; }
            set
            {
                if( value < 0 )
                {
                    m_Parasite = 0;
                }
                else if( value > 2 )
                {
                    m_Parasite = 2;
                }
                else
                {
                    m_Parasite = value;
                }
            }
        }

        public int DiseaseLevel
        {
            get { return m_Disease; }
            set
            {
                if( value < 0 )
                {
                    m_Disease = 0;
                }
                else if( value > 2 )
                {
                    m_Disease = 2;
                }
                else
                {
                    m_Disease = value;
                }
            }
        }

        public bool IsFullAgilityPotion
        {
            get { return m_PotAgility >= 2; }
        }

        public int PotAgility
        {
            get { return m_PotAgility; }
            set
            {
                if( value < 0 )
                {
                    m_PotAgility = 0;
                }
                else if( value > 2 )
                {
                    m_PotAgility = 2;
                }
                else
                {
                    m_PotAgility = value;
                }
            }
        }

        public bool IsFullHealPotion
        {
            get { return m_PotHeal >= 2; }
        }

        public int PotHeal
        {
            get { return m_PotHeal; }
            set
            {
                if( value < 0 )
                {
                    m_PotHeal = 0;
                }
                else if( value > 2 )
                {
                    m_PotHeal = 2;
                }
                else
                {
                    m_PotHeal = value;
                }
            }
        }

        public bool IsFullCurePotion
        {
            get { return m_PotCure >= 2; }
        }

        public int PotCure
        {
            get { return m_PotCure; }
            set
            {
                if( value < 0 )
                {
                    m_PotCure = 0;
                }
                else if( value > 2 )
                {
                    m_PotCure = 2;
                }
                else
                {
                    m_PotCure = value;
                }
            }
        }

        public bool IsFullStrengthPotion
        {
            get { return m_PotStr >= 2; }
        }

        public int PotStrength
        {
            get { return m_PotStr; }
            set
            {
                if( value < 0 )
                {
                    m_PotStr = 0;
                }
                else if( value > 2 )
                {
                    m_PotStr = 2;
                }
                else
                {
                    m_PotStr = value;
                }
            }
        }

        public bool IsFullPoisonPotion
        {
            get { return m_PotPoison >= 2; }
        }

        public int PotPoison
        {
            get { return m_PotPoison; }
            set
            {
                if( value < 0 )
                {
                    m_PotPoison = 0;
                }
                else if( value > 2 )
                {
                    m_PotPoison = 2;
                }
                else
                {
                    m_PotPoison = value;
                }
            }
        }

        public int FlowersInRange { get; set; }

        public int WaterInRange { get; set; }

        public int Range
        {
            get { return m_Population + 2 + PotAgility; } //bees work harder
        }

        public bool IsCheckable
        {
            get { return HiveStage != HiveStatus.Empty; }
        }

        public bool IsGrowable
        {
            get { return HiveStage != HiveStatus.Empty; }
        }

        public bool HasMaladies
        {
            get { return DiseaseLevel > 0 || ParasiteLevel > 0; }
        }

        public void InvalidateHiveProperties()
        {
            m_Comp.InvalidateProperties();
        }

        public override void Delete()
        {
            //make sure we delete any swarms
            if( m_Bees != null )
            {
                m_Bees.Delete();
            }

            base.Delete();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_Bees ); //swarm item
            writer.Write( m_Health ); //hive health
            writer.Write( NextCheck ); //next update check
            writer.Write( (int)HiveStage ); //growth stage
            writer.Write( (int)LastGrowth ); //last growth
            writer.Write( HiveAge ); //age of hive
            writer.Write( m_Population ); //bee population (*10k)
            writer.Write( m_Parasite ); //parasite level(0, 1, 2)
            writer.Write( m_Disease ); //disease level (0, 1, 2)
            writer.Write( FlowersInRange ); //amount of water tiles in range (during last check)
            writer.Write( WaterInRange ); //number of flowers in range (during last check)
            writer.Write( m_Wax ); //amount of Wax
            writer.Write( m_Honey ); //amount of Hone
            writer.Write( m_PotAgility ); //number of agility potions
            writer.Write( m_PotHeal ); //number of heal potions
            writer.Write( m_PotCure ); //number of cure potions
            writer.Write( m_PotStr ); //number of stength potions
            writer.Write( m_PotPoison ); //number of poison potions
            writer.Write( m_Comp ); //for storing the top of the hive
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Bees = (ApiBeesComponent)reader.ReadItem(); //for displaying bee swarm
            m_Health = reader.ReadInt(); //current health
            NextCheck = reader.ReadDateTime(); //next update check
            HiveStage = (HiveStatus)reader.ReadInt(); //growth stage
            LastGrowth = (HiveGrowthIndicator)reader.ReadInt(); //last growth
            HiveAge = reader.ReadInt(); //age of hive
            m_Population = reader.ReadInt(); //bee population (*10k)
            m_Parasite = reader.ReadInt(); //parasite level(0, 1, 2)
            m_Disease = reader.ReadInt(); //disease level (0, 1, 2)
            FlowersInRange = reader.ReadInt(); //amount of water tiles in range (during last check)
            WaterInRange = reader.ReadInt(); //number of flowers in range (during last check)
            m_Wax = reader.ReadInt(); //amount of Wax
            m_Honey = reader.ReadInt(); //amount of Honey
            m_PotAgility = reader.ReadInt(); //number of agility potions
            m_PotHeal = reader.ReadInt(); //number of heal potions
            m_PotCure = reader.ReadInt(); //number of cure potions
            m_PotStr = reader.ReadInt(); //number of stength potions
            m_PotPoison = reader.ReadInt(); //number of poison potions
            m_Comp = (ApiBeeHiveComponent)reader.ReadItem(); //for storing the top of the hive
        }

        public ResourceStatus ScaleWater()
        {
            //scale amount of water for bee population
            if( WaterInRange == 0 )
            {
                return ResourceStatus.None;
            }

            int perc = WaterInRange * 250 / Population;

            if( perc < 33 )
            {
                return ResourceStatus.VeryLow;
            }
            else if( perc < 66 )
            {
                return ResourceStatus.Low;
            }
            else if( perc < 101 )
            {
                return ResourceStatus.Normal;
            }
            else if( perc < 133 )
            {
                return ResourceStatus.High;
            }
            else
            {
                return ResourceStatus.VeryHigh;
            }
        }

        public ResourceStatus ScaleFlower()
        {
            //scale amount of flowers for bee population
            if( FlowersInRange == 0 )
            {
                return ResourceStatus.None;
            }

            int perc = FlowersInRange * 100 / Population;

            if( perc < 33 )
            {
                return ResourceStatus.VeryLow;
            }
            else if( perc < 66 )
            {
                return ResourceStatus.Low;
            }
            else if( perc < 101 )
            {
                return ResourceStatus.Normal;
            }
            else if( perc < 133 )
            {
                return ResourceStatus.High;
            }
            else
            {
                return ResourceStatus.VeryHigh;
            }
        }

        public bool IsUsableBy( Mobile from )
        {
            var root = RootParent as Item;
            return IsChildOf( from.Backpack ) || IsChildOf( from.BankBox ) || IsLockedDown && IsAccessibleTo( from ) || root != null && root.IsSecure && root.IsAccessibleTo( from );
        }

        public void Pour( Mobile from, Item item )
        {
            if( !IsAccessibleTo( from ) )
            {
                LabelTo( from, 1065368 ); //  You cannot pour potions on that.
                return;
            }

            if( item is BasePotion )
            {
                var potion = (BasePotion)item;

                string message;
                if( ApplyPotion( potion.PotionEffect, false, out message ) )
                {
                    potion.Consume();
                    from.PlaySound( 0x240 );
                    from.AddToBackpack( new Bottle() );
                }
                LabelTo( from, message );
            }
            else if( item is PotionKeg )
            {
                var keg = (PotionKeg)item;

                if( keg.Held <= 0 )
                {
                    LabelTo( from, 1065369 ); // You cannot use that on a beehive!
                    return;
                }

                string message;
                if( ApplyPotion( keg.Type, false, out message ) )
                {
                    keg.Held--;
                    from.PlaySound( 0x240 );
                }
                LabelTo( from, message );
            }
            else
            {
                LabelTo( from, 1065370 ); // You cannot use that on a beehive!
            }
        }

        public bool ApplyPotion( PotionEffect effect, bool testOnly, out string message )
        {
            bool full = false;

            if( effect == PotionEffect.PoisonGreater || effect == PotionEffect.PoisonDeadly )
            {
                if( IsFullPoisonPotion )
                {
                    full = true;
                }
                else if( !testOnly )
                {
                    PotPoison++;
                }
            }
            else if( effect == PotionEffect.CureGreater )
            {
                if( IsFullCurePotion )
                {
                    full = true;
                }
                else if( !testOnly )
                {
                    PotCure++;
                }
            }
            else if( effect == PotionEffect.HealGreater )
            {
                if( IsFullHealPotion )
                {
                    full = true;
                }
                else if( !testOnly )
                {
                    PotHeal++;
                }
            }
            else if( effect == PotionEffect.StrengthGreater )
            {
                if( IsFullStrengthPotion )
                {
                    full = true;
                }
                else if( !testOnly )
                {
                    PotStrength++;
                }
            }
            else if( effect == PotionEffect.AgilityGreater )
            {
                if( IsFullAgilityPotion )
                {
                    full = true;
                }
                else if( !testOnly )
                {
                    PotAgility++;
                }
            }
            else if( effect == PotionEffect.PoisonLesser || effect == PotionEffect.Poison || effect == PotionEffect.CureLesser || effect == PotionEffect.Cure ||
                     effect == PotionEffect.HealLesser || effect == PotionEffect.Heal || effect == PotionEffect.Strength )
            {
                message = "This potion is not powerful enough to use on a beehive!";
                return false;
            }
            else
            {
                message = "You cannot use that on a beehive!";
                return false;
            }

            if( full )
            {
                message = "The beehive is already soaked with this type of potion!";
                return false;
            }
            else
            {
                message = "You pour the potion into the beehive.";
                return true;
            }
        }

        public void FindWaterInRange()
        {
            //check area around hive for water (WATER)

            WaterInRange = 0;

            Map map = Map;

            if( map == null )
            {
                return;
            }

            IPooledEnumerable eable = map.GetItemsInRange( Location, Range );

            foreach( Item item in eable )
            {
                string iName = item.ItemData.Name.ToUpper();

                if( iName.IndexOf( "WATER" ) != -1 )
                {
                    WaterInRange++;
                }
            }

            eable.Free();
        }

        public void FindFlowersInRange()
        {
            //check area around hive for flowers (flower, snowdrop, poppie)

            FlowersInRange = 0;

            Map map = Map;

            if( map == null )
            {
                return;
            }

            IPooledEnumerable eable = map.GetItemsInRange( Location, Range );

            foreach( Item item in eable )
            {
                string iName = item.ItemData.Name.ToUpper();

                if( iName.IndexOf( "FLOWER" ) != -1 || iName.IndexOf( "SNOWDROP" ) != -1 || iName.IndexOf( "POPPIE" ) != -1 )
                {
                    FlowersInRange++;
                }
            }

            eable.Free();
        }

        public void Grow()
        {
            if( OverallHealth < HiveHealth.Healthy )
            {
                //not healthy enough to grow or produce
                if( LastGrowth != HiveGrowthIndicator.PopulationDown ) //population down takes precedence
                {
                    LastGrowth = HiveGrowthIndicator.NotHealthy;
                }
            }
            else if( ScaleFlower() < ResourceStatus.Low || ScaleWater() < ResourceStatus.Low )
            {
                //resources too low to grow or produce
                if( LastGrowth != HiveGrowthIndicator.PopulationDown ) //population down takes precedence
                {
                    LastGrowth = HiveGrowthIndicator.LowResources;
                }
            }
            else if( HiveStage < HiveStatus.Stage5 )
            {
                //not producing yet, so just grow
                var curStage = (int)HiveStage;
                HiveStage = (HiveStatus)( curStage + 1 );

                LastGrowth = HiveGrowthIndicator.Grown;
            }
            else
            {
                //production
                if( Wax < MaxWax )
                {
                    int baseWax = 1;

                    if( OverallHealth == HiveHealth.Thriving )
                    {
                        baseWax++;
                    }

                    baseWax += PotAgility; //bees work harder

                    baseWax *= Population;

                    if( LessWax )
                    {
                        baseWax = Math.Max( 1, ( baseWax / 3 ) ); //wax production is slower then honey
                    }

                    Wax += baseWax;
                    LastGrowth = HiveGrowthIndicator.Grown;
                }

                if( Honey < MaxHoney )
                {
                    int baseHoney = 1;

                    if( OverallHealth == HiveHealth.Thriving )
                    {
                        baseHoney++;
                    }

                    baseHoney += PotAgility; //bees work harder

                    baseHoney *= Population;

                    Honey += baseHoney;
                    LastGrowth = HiveGrowthIndicator.Grown;
                }

                PotAgility = 0;

                if( Population < 10 && !( ScaleFlower() < ResourceStatus.Normal ) && !( ScaleWater() < ResourceStatus.Normal ) )
                {
                    LastGrowth = HiveGrowthIndicator.PopulationUp;
                    Population++;
                }
            }

            if( HiveStage >= HiveStatus.Producing && !m_Bees.Visible )
            {
                m_Bees.Visible = true;
            }
        }

        public void ApplyBenefitEffects()
        {
            if( PotPoison >= ParasiteLevel )
            {
                PotPoison -= ParasiteLevel;
                ParasiteLevel = 0;
            }
            else
            {
                ParasiteLevel -= PotPoison;
                PotPoison = 0;
            }

            if( PotCure >= DiseaseLevel )
            {
                PotCure -= DiseaseLevel;
                DiseaseLevel = 0;
            }
            else
            {
                DiseaseLevel -= PotCure;
                PotCure = 0;
            }

            if( !HasMaladies )
            {
                if( PotHeal > 0 )
                {
                    Health += PotHeal * 7;
                }
                else
                {
                    Health += 2;
                }
            }

            PotHeal = 0;
        }

        public bool ApplyMaladiesEffects()
        {
            int damage = 0;

            if( ParasiteLevel > 0 )
            {
                damage += ParasiteLevel * Utility.RandomMinMax( 3, 6 );
            }

            if( DiseaseLevel > 0 )
            {
                damage += DiseaseLevel * Utility.RandomMinMax( 3, 6 );
            }

            if( ScaleWater() < ResourceStatus.Low )
            {
                damage += ( 2 - (int)ScaleWater() ) * Utility.RandomMinMax( 3, 6 );
            }

            if( ScaleFlower() < ResourceStatus.Low )
            {
                damage += ( 2 - (int)ScaleFlower() ) * Utility.RandomMinMax( 3, 6 );
            }

            Health -= damage;

            return IsGrowable;
        }

        public void UpdateMaladies()
        {
            //more water = more chance to come into contact with parasites?
            double parasiteChance = 0.30 - ( PotStrength * 0.075 ) + ( ( (int)ScaleWater() - 3 ) * 0.10 ) + ( HiveAge * 0.01 ); //Older hives are more susceptible to infestation 

            if( Utility.RandomDouble() < parasiteChance )
            {
                ParasiteLevel++;
            }

            //more flowers = more chance to come into conctact with disease carriers
            double diseaseChance = 0.30 - ( PotStrength * 0.075 ) + ( ( (int)ScaleFlower() - 3 ) * 0.10 ) + ( HiveAge * 0.01 ); //Older hives are more susceptible to disease 

            if( Utility.RandomDouble() < diseaseChance )
            {
                DiseaseLevel++;
            }

            if( PotPoison > 0 ) //there are still poisons to apply
            {
                if( PotCure > 0 ) //cures can negate poisons
                {
                    PotPoison -= PotCure;
                    PotCure = 0;
                }
                if( PotPoison > 0 ) //if there are still poisons, hurt the hive
                {
                    Health -= PotPoison * Utility.RandomMinMax( 3, 6 );
                }
                PotPoison = 0;
            }

            PotStrength = 0; //reset strength potions
        }

        public void Die()
        {
            if( HiveStage >= HiveStatus.Producing )
            {
                Population--;
                LastGrowth = HiveGrowthIndicator.PopulationDown;

                if( Population == 0 )
                {
                    HiveStage = HiveStatus.Empty;
                }
            }
            else
            {
                HiveStage = HiveStatus.Empty;
            }
            m_Bees.Visible = false;
        }

        #region Nested type: ApiBeeHiveComponent
        private class ApiBeeHiveComponent : AddonComponent
        {
            private ApiBeeHive m_Hive;

            public ApiBeeHiveComponent( ApiBeeHive hive )
                : base( 2330 )
            {
                m_Hive = hive;
            }

            public ApiBeeHiveComponent( Serial serial )
                : base( serial )
            {
            }

            public override bool ForceShowProperties
            {
                get { return true; }
            }

            public override void OnDoubleClick( Mobile from )
            {
                if( m_Hive == null )
                {
                    LabelTo( from, 1065376 ); // That beehive is invalid. Use an axe to redeed it.
                    return;
                }

                if( m_Hive.HiveStage == HiveStatus.Empty )
                {
                    LabelTo( from, 1065377 ); // That beehive is empty. Use an axe to redeed it.
                    return;
                }
                from.SendGump( new ApiBeeHiveMainGump( from, m_Hive ) );
            }

            public override void AddNameProperty( ObjectPropertyList list )
            {
                if( m_Hive == null )
                {
                    list.Add( "Invalid Hive" );
                    return;
                }

                if( m_Hive.HiveStage == HiveStatus.Empty )
                {
                    list.Add( "BeeHive" );
                }
                else
                {
                    list.Add( m_Hive.OverallHealth + " BeeHive" );
                }
            }

            public override void GetProperties( ObjectPropertyList list )
            {
                base.GetProperties( list );

                if( m_Hive == null )
                {
                    return;
                }

                if( m_Hive.HiveStage >= HiveStatus.Producing )
                {
                    list.Add( 1049644, "Producing" );
                }
                else if( m_Hive.HiveStage >= HiveStatus.Brooding )
                {
                    list.Add( 1049644, "Brooding" );
                }
                else if( m_Hive.HiveStage >= HiveStatus.Colonizing )
                {
                    list.Add( 1049644, "Colonizing" );
                }
                else
                {
                    list.Add( 1049644, "Empty" );
                }

                if( m_Hive.HiveStage != HiveStatus.Empty )
                {
                    list.Add( 1060663, "{0}\t{1}", "Age", m_Hive.HiveAge + ( m_Hive.HiveAge == 1 ? " day" : " days" ) );
                }
                if( m_Hive.HiveStage >= HiveStatus.Producing )
                {
                    list.Add( 1060662, "{0}\t{1}", "Colony", m_Hive.Population + "0k bees" );
                }
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( 0 ); // version
                writer.Write( m_Hive );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();
                m_Hive = (ApiBeeHive)reader.ReadItem();
            }
        }
        #endregion

        #region Nested type: ApiBeesComponent
        private class ApiBeesComponent : AddonComponent
        {
            private ApiBeeHive m_Hive;

            public ApiBeesComponent( ApiBeeHive hive )
                : base( 0x91b )
            {
                m_Hive = hive;
                Movable = false;
            }

            public ApiBeesComponent( Serial serial )
                : base( serial )
            {
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( 0 ); // version
                writer.Write( m_Hive );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();
                m_Hive = (ApiBeeHive)reader.ReadItem();
            }
        }
        #endregion
    }

    public class ApiBeeHiveDeed : BaseAddonDeed
    {
        [Constructable]
        public ApiBeeHiveDeed()
        {
        }

        public ApiBeeHiveDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1065265; }
        } // beehive deed

        public override BaseAddon Addon
        {
            get { return new ApiBeeHive(); }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}