/***************************************************************************
 *                               AdvancedForge.cs
 *                            ----------------------
 *   begin                : 16 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.AdvancedSmelting
{
    public enum Temperature
    {
        Cold,
        Warm,
        Hot,
        ReallyHot,
        Incandescent
    }

    public class AdvancedForge : Item
    {
        public void IncreaseTemperature()
        {
            if( Temperature >= Temperature.Incandescent )
                return;

            Temperature++;
        }

        public void DecreaseTemperature()
        {
            if( Temperature <= Temperature.Cold )
            {
                if( HasTimer )
                    StopTimer();

                return;
            }

            Temperature--;
        }

        public void ProcessCoil()
        {
            if( ForgeCoilAmount > (int)Temperature )
            {
                IncreaseTemperature();
                ForgeCoilAmount -= (int)Temperature;
            }
            else
            {
                DecreaseTemperature();
                ForgeCoilAmount = 0;
            }
        }

        private InternalTimer m_TemperatureTimer;

        public bool HasTimer
        {
            get
            {
                return m_TemperatureTimer != null && m_TemperatureTimer.Running;
            }
        }

        public void StartTimer()
        {
            if( HasTimer )
                return;

            m_TemperatureTimer = new InternalTimer( this );
            m_TemperatureTimer.Start();
        }

        public void StopTimer()
        {
            if( !HasTimer )
                return;

            m_TemperatureTimer.Stop();
            m_TemperatureTimer = null;
        }

        private static readonly TimeSpan CoilProcessDelay = TimeSpan.FromSeconds( 30.0 );

        private class InternalTimer : Timer
        {
            private readonly AdvancedForge m_Forge;

            public InternalTimer( AdvancedForge forge )
                : base( CoilProcessDelay, CoilProcessDelay )
            {
                m_Forge = forge;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Forge.ProcessCoil();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsSmelting { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ForgeResourceAmount1 { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ForgeResourceAmount2 { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ForgeCoilAmount { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource ForgeInfo1 { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource ForgeInfo2 { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int TotalAmount
        {
            get { return ForgeResourceAmount1 + ForgeResourceAmount2; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Temperature Temperature { get; private set; }

        [Constructable]
        public AdvancedForge()
            : base( 0xFB1 )
        {
        }

        private const int ForgeLimit = 50;

        public static void Initialize()
        {
            m_ValidResources[ typeof( DullCopperOre ) ] = CraftResource.OldDullCopper;
            m_ValidResources[ typeof( ShadowIronOre ) ] = CraftResource.OldShadowIron;
            m_ValidResources[ typeof( CopperOre ) ] = CraftResource.OldCopper;
            m_ValidResources[ typeof( BronzeOre ) ] = CraftResource.OldBronze;
            m_ValidResources[ typeof( GoldOre ) ] = CraftResource.OldGold;
            m_ValidResources[ typeof( AgapiteOre ) ] = CraftResource.OldAgapite;
            m_ValidResources[ typeof( VeriteOre ) ] = CraftResource.OldVerite;
            m_ValidResources[ typeof( ValoriteOre ) ] = CraftResource.OldValorite;
            m_ValidResources[ typeof( GraphiteOre ) ] = CraftResource.OldGraphite;
            m_ValidResources[ typeof( PyriteOre ) ] = CraftResource.OldPyrite;
            m_ValidResources[ typeof( AzuriteOre ) ] = CraftResource.OldAzurite;
            m_ValidResources[ typeof( VanadiumOre ) ] = CraftResource.OldVanadium;
            m_ValidResources[ typeof( SilverOre ) ] = CraftResource.OldSilver;
            m_ValidResources[ typeof( PlatinumOre ) ] = CraftResource.OldPlatinum;
            m_ValidResources[ typeof( AmethystOre ) ] = CraftResource.OldAmethyst;
            m_ValidResources[ typeof( TitaniumOre ) ] = CraftResource.OldTitanium;
            m_ValidResources[ typeof( XenianOre ) ] = CraftResource.OldXenian;
            m_ValidResources[ typeof( RubidianOre ) ] = CraftResource.OldRubidian;
            m_ValidResources[ typeof( ObsidianOre ) ] = CraftResource.OldObsidian;
            m_ValidResources[ typeof( EbonSapphireOre ) ] = CraftResource.OldEbonSapphire;
            m_ValidResources[ typeof( DarkRubyOre ) ] = CraftResource.OldDarkRuby;
            m_ValidResources[ typeof( RadiantDiamondOre ) ] = CraftResource.OldRadiantDiamond;

            m_ValidResources[ typeof( DullCopperIngot ) ] = CraftResource.OldDullCopper;
            m_ValidResources[ typeof( ShadowIronIngot ) ] = CraftResource.OldShadowIron;
            m_ValidResources[ typeof( CopperIngot ) ] = CraftResource.OldCopper;
            m_ValidResources[ typeof( BronzeIngot ) ] = CraftResource.OldBronze;
            m_ValidResources[ typeof( GoldIngot ) ] = CraftResource.OldGold;
            m_ValidResources[ typeof( AgapiteIngot ) ] = CraftResource.OldAgapite;
            m_ValidResources[ typeof( VeriteIngot ) ] = CraftResource.OldVerite;
            m_ValidResources[ typeof( ValoriteIngot ) ] = CraftResource.OldValorite;
            m_ValidResources[ typeof( GraphiteIngot ) ] = CraftResource.OldGraphite;
            m_ValidResources[ typeof( PyriteIngot ) ] = CraftResource.OldPyrite;
            m_ValidResources[ typeof( AzuriteIngot ) ] = CraftResource.OldAzurite;
            m_ValidResources[ typeof( VanadiumIngot ) ] = CraftResource.OldVanadium;
            m_ValidResources[ typeof( SilverIngot ) ] = CraftResource.OldSilver;
            m_ValidResources[ typeof( PlatinumIngot ) ] = CraftResource.OldPlatinum;
            m_ValidResources[ typeof( AmethystIngot ) ] = CraftResource.OldAmethyst;
            m_ValidResources[ typeof( TitaniumIngot ) ] = CraftResource.OldTitanium;
            m_ValidResources[ typeof( XenianIngot ) ] = CraftResource.OldXenian;
            m_ValidResources[ typeof( RubidianIngot ) ] = CraftResource.OldRubidian;
            m_ValidResources[ typeof( ObsidianIngot ) ] = CraftResource.OldObsidian;
            m_ValidResources[ typeof( EbonSapphireIngot ) ] = CraftResource.OldEbonSapphire;
            m_ValidResources[ typeof( DarkRubyIngot ) ] = CraftResource.OldDarkRuby;
            m_ValidResources[ typeof( RadiantDiamondIngot ) ] = CraftResource.OldRadiantDiamond;
        }

        private static readonly Dictionary<Type, CraftResource> m_ValidResources = new Dictionary<Type, CraftResource>();

        private static CraftResource GetResourceFromType( Type t )
        {
            CraftResource defaultValue;
            if( m_ValidResources.TryGetValue( t, out defaultValue ) )
                return defaultValue;
            else
                return CraftResource.None;
        }

        public void CheckAddResource( Item resource )
        {
            if( resource == null || resource.Deleted )
                return;

            if( TotalAmount + resource.Amount > ForgeLimit )
                SendMessage( "This forge is full. Try to smelt the ore inside." );
            else if( IsSmelting )
                SendMessage( "You cannot use this forge now." );
            else if( !IsValidResource( resource ) && !IsCoil( resource ) )
                SendMessage( "You cannot add that item to smelting process." );
            else
            {
                CraftResource craftResource = GetResourceFromType( resource.GetType() );

                if( IsCoil( resource ) )
                {
                    SendMessage( "You added some coil in the liquid mixture." );
                    ForgeCoilAmount += resource.Amount;
                    resource.Delete();
                }
                else if( craftResource == CraftResource.None )
                    SendMessage( "You cannot add that item to smelting process." );
                else if( IsValidResource( resource ) )
                {
                    if( ForgeInfo1 == craftResource || ForgeInfo1 == CraftResource.None )
                    {
                        SendMessage( "You added some ore in the liquid mixture." );

                        if( ForgeInfo1 == CraftResource.None )
                            ForgeInfo1 = craftResource;

                        ForgeResourceAmount1 += resource.Amount;
                        resource.Delete();
                    }
                    else if( ForgeInfo2 == craftResource || ForgeInfo2 == CraftResource.None )
                    {
                        SendMessage( "You added some ore in the liquid mixture." );

                        if( ForgeInfo2 == CraftResource.None )
                            ForgeInfo2 = craftResource;

                        ForgeResourceAmount2 += resource.Amount;
                        resource.Delete();
                    }
                    else
                        SendMessage( "You cannot add that ore to smelting process. The maximum ore types are already in the forge." );
                }
            }
        }

        public void SendMessage( string message )
        {
            PublicOverheadMessage( MessageType.Regular, 0x3B2, true, message );
        }

        private static bool IsValidResource( Item item )
        {
            return m_ValidResources.ContainsKey( item.GetType() );
        }

        private static readonly Type m_CoilType = typeof( CoilOre );

        private static bool IsCoil( Item item )
        {
            return item.GetType() == m_CoilType;
        }

        private double GetSmeltDuration()
        {
            return TotalAmount;
        }

        private const double AnimateDelay = 1.5;

        public override void OnDoubleClick( Mobile from )
        {
            if( Movable )
                return;

            if( !Deleted || !from.InRange( GetWorldLocation(), 2 ) )
                from.SendGump( new ConfirmSmeltGump( from, this ) );
        }

        public void ClearForge()
        {
            ForgeInfo1 = CraftResource.None;
            ForgeInfo2 = CraftResource.None;
            ForgeResourceAmount1 = 0;
            ForgeResourceAmount2 = 0;
            ForgeCoilAmount = 0;
        }

        public virtual void BeginSmelt( Mobile from )
        {
            if( !IsSmelting )
            {
                if( !CheckTemperature( true, from ) )
                    return;

                int count = (int)Math.Ceiling( GetSmeltDuration() / AnimateDelay );

                if( count > 0 )
                {
                    EffectTimer timer = new EffectTimer( this, count );
                    timer.Start();

                    IsSmelting = true;

                    double effectiveDuration = ( count * AnimateDelay ) + 1.0;
                    Timer.DelayCall( TimeSpan.FromSeconds( effectiveDuration ), new TimerStateCallback( Smelt_Callback ), new object[] { from, this } );
                }
            }
            else
            {
                SendMessage( "This forge is already being used." );
            }
        }

        private static void Smelt_Callback( object state )
        {
            object[] states = (object[])state;

            Mobile from = (Mobile)states[ 0 ];
            AdvancedForge forge = (AdvancedForge)states[ 1 ];

            forge.DoSmelt( from );
        }

        private bool CheckTemperature( bool message, Mobile from )
        {
            int amount;
            SmeltInfo info = SmeltInfo.GetRecipe( this, out amount );

            if( info != null && Temperature < info.Temperature )
            {
                if( message )
                    from.SendMessage( "The forge is not enought hot to produce the metal." );
                return false;
            }

            return true;
        }

        private bool CheckDifficulty( SmeltInfo info, int amount, bool message, Mobile from )
        {
            double difficulty = GetRealDifficulty( from, info, amount );

            if( difficulty > from.Skills[ SkillName.Mining ].Value )
            {
                if( message )
                {
                    SendMessage( "You have no idea how to complete the work..." );
                    from.SendMessage( "You need {0} in mining to smelt this ores.", difficulty.ToString( "F2" ) );
                }

                return false;
            }

            amount = (int)( amount * DefaultMineralLoss );

            if( from.CheckTargetSkill( SkillName.Mining, this, difficulty - 25.0, difficulty + 25.0 ) && amount > 0 )
            {
                SendMessage( "Process ended successfully." );
                return true;
            }
            else
            {
                SendMessage( "Process failded and the mineral is lost." );
                return false;
            }
        }

        private static double GetRealDifficulty( Mobile from, SmeltInfo info, int amount )
        {
            double difficulty = 100.0;
            CraftResourceInfo resourceInfo = CraftResources.GetInfo( info.ResultRes );
            if( resourceInfo != null )
                difficulty = resourceInfo.AttributeInfo.OldSmeltingRequiredSkill;

            Console.WriteLine( "difficulty: " + difficulty );
            Console.WriteLine( "GetScalarDiffForQuantity: " + GetScalarDiffForQuantity( amount ) );
            Console.WriteLine( "GetArmsloreBonus: " + GetArmsloreBonus( from ) );
            Console.WriteLine( "GetItemIDBonus: " + GetItemIDBonus( from ) );

            double realDiff = difficulty * ( 1.0 + GetScalarDiffForQuantity( amount ) ) *
                              ( 1.0 - GetArmsloreBonus( from ) ) *
                              ( 1.0 - GetItemIDBonus( from ) );

            Console.WriteLine( "realDiff: " + realDiff );

            return difficulty;
        }

        private void DoSmelt( Mobile from )
        {
            if( !CheckTemperature( true, from ) )
                return;

            int amount;
            SmeltInfo info = SmeltInfo.GetRecipe( this, out amount );

            if( info != null )
            {
                if( CheckDifficulty( info, amount, true, from ) )
                {
                    BaseIngot ingot = info.GetIngot();
                    ingot.Amount = amount;
                    from.AddToBackpack( ingot );
                }
            }
            else
            {
                SendMessage( "There is no usable combination of ores in the forge... they are lost." );
            }

            ClearForge();
            IsSmelting = false;
        }

        private const double DefaultMineralLoss = 0.8;
        private const double ScalarAtLimit = 0.5;
        private const double ArmsloreBonusAtLimit = 0.2;
        private const double ItemIDBonusAtLimit = 0.2;

        private static double GetScalarDiffForQuantity( int quantity )
        {
            if( quantity <= 10 )
                return 0;

            // linear between (10.0,0) and (50.0,ScalarAtLimit)
            return Math.Min( ScalarAtLimit * ( ( quantity - 10 ) / 40.0 ), ScalarAtLimit );
        }

        private static double GetArmsloreBonus( Mobile from )
        {
            // linear between (0.0,0.0) and (100.0,ScalarAtLimit)
            return Math.Min( ArmsloreBonusAtLimit * ( from.Skills[ SkillName.ArmsLore ].Value / 100.0 ), ArmsloreBonusAtLimit );
        }

        private static double GetItemIDBonus( Mobile from )
        {
            // linear between (0.0,0.0) and (100.0,ScalarAtLimit)
            return Math.Min( ItemIDBonusAtLimit * ( from.Skills[ SkillName.ItemID ].Value / 100.0 ), ItemIDBonusAtLimit );
        }

        public int GetAnimHue()
        {
            int amount;
            SmeltInfo info = SmeltInfo.GetRecipe( this, out amount );

            return info != null ? CraftResources.GetHue( info.ResultRes ) : Utility.RandomMetalHue();
        }

        private class EffectTimer : Timer
        {
            private readonly AdvancedForge m_Forge;
            private readonly int m_Hue;

            public EffectTimer( AdvancedForge forge, int count )
                : base( TimeSpan.Zero, TimeSpan.FromSeconds( AnimateDelay ), count )
            {
                m_Forge = forge;
                m_Hue = m_Forge.GetAnimHue();

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                int renderMode = Utility.RandomList( 0, 2, 3, 4, 5, 7 );
                Effects.SendLocationEffect( m_Forge, m_Forge.Map, 14201, 16, m_Hue, renderMode );
            }
        }

        #region serialization
        public AdvancedForge( Serial serial )
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
        #endregion
    }
}