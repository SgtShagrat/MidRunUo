/***************************************************************************
 *                               DruidAttributes.cs
 *
 *   begin                : 10 ottobre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;

namespace Midgard.Engines.Classes
{
    [PropertyObject]
    public sealed class DruidAttributes : BaseClassAttributes
    {
        private static readonly Type typeofSpringOfLife = typeof( SpringOfLifeDefinition );
        private static readonly Type typeofGraspingRoots = typeof( GraspingRootsDefinition );
        private static readonly Type typeofBlendWithForest = typeof( BlendWithForestdefDefinition );
        private static readonly Type typeofDruidFamiliar = typeof( DruidFamiliarDefinition );
        private static readonly Type typeofEnchantedGrove = typeof( EnchantedGroveDefinition );
        private static readonly Type typeofShieldOfEarth = typeof( ShieldOfEarthDefinition );
        private static readonly Type typeofDruidCircle = typeof( DruidCircleDefinition );
        private static readonly Type typeofGiftOfRenewal = typeof( GiftOfRenewalDefinition );
        private static readonly Type typeofImmolatingWeapon = typeof( ImmolatingWeaponDefinition );
        private static readonly Type typeofAttuneWeapon = typeof( AttuneWeaponDefinition );
        private static readonly Type typeofThunderstorm = typeof( ThunderstormDefinition );
        private static readonly Type typeofNatureFury = typeof( NatureFuryDefinition );
        private static readonly Type typeofReaperForm = typeof( ReaperFormDefinition );
        private static readonly Type typeofWildfire = typeof( WildfireDefinition );
        private static readonly Type typeofEssenceOfWind = typeof( EssenceOfWindDefinition );
        private static readonly Type typeofDryadAllure = typeof( DryadAllureDefinition );
        private static readonly Type typeofEtherealVoyage = typeof( EtherealVoyageDefinition );
        private static readonly Type typeofWordOfDeath = typeof( WordOfDeathDefinition );
        private static readonly Type typeofGiftOfLife = typeof( GiftOfLifeDefinition );
        private static readonly Type typeofDruidEmpowerment = typeof( DruidEmpowermentDefinition );
        private static readonly Type typeofAnimalForm = typeof( AnimalFormDefinition );

        public DruidAttributes( ClassPlayerState state )
            : base( state )
        {
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int SpringOfLife
        {
            get { return GetLevel( typeofSpringOfLife ); }
            set { SetLevel( typeofSpringOfLife, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int GraspingRoots
        {
            get { return GetLevel( typeofGraspingRoots ); }
            set { SetLevel( typeofGraspingRoots, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int BlendWithForestdef
        {
            get { return GetLevel( typeofBlendWithForest ); }
            set { SetLevel( typeofBlendWithForest, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DruidFamiliar
        {
            get { return GetLevel( typeofDruidFamiliar ); }
            set { SetLevel( typeofDruidFamiliar, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int EnchantedGrove
        {
            get { return GetLevel( typeofEnchantedGrove ); }
            set { SetLevel( typeofEnchantedGrove, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ShieldOfEarth
        {
            get { return GetLevel( typeofShieldOfEarth ); }
            set { SetLevel( typeofShieldOfEarth, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DruidCircle
        {
            get { return GetLevel( typeofDruidCircle ); }
            set { SetLevel( typeofDruidCircle, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int GiftOfRenewal
        {
            get { return GetLevel( typeofGiftOfRenewal ); }
            set { SetLevel( typeofGiftOfRenewal, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ImmolatingWeapon
        {
            get { return GetLevel( typeofImmolatingWeapon ); }
            set { SetLevel( typeofImmolatingWeapon, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int AttuneWeapon
        {
            get { return GetLevel( typeofAttuneWeapon ); }
            set { SetLevel( typeofAttuneWeapon, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Thunderstorm
        {
            get { return GetLevel( typeofThunderstorm ); }
            set { SetLevel( typeofThunderstorm, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int NatureFury
        {
            get { return GetLevel( typeofNatureFury ); }
            set { SetLevel( typeofNatureFury, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ReaperForm
        {
            get { return GetLevel( typeofReaperForm ); }
            set { SetLevel( typeofReaperForm, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Wildfire
        {
            get { return GetLevel( typeofWildfire ); }
            set { SetLevel( typeofWildfire, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int EssenceOfWind
        {
            get { return GetLevel( typeofEssenceOfWind ); }
            set { SetLevel( typeofEssenceOfWind, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DryadAllure
        {
            get { return GetLevel( typeofDryadAllure ); }
            set { SetLevel( typeofDryadAllure, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int EtherealVoyage
        {
            get { return GetLevel( typeofEtherealVoyage ); }
            set { SetLevel( typeofEtherealVoyage, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int WordOfDeath
        {
            get { return GetLevel( typeofWordOfDeath ); }
            set { SetLevel( typeofWordOfDeath, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int GiftOfLife
        {
            get { return GetLevel( typeofGiftOfLife ); }
            set { SetLevel( typeofGiftOfLife, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DruidEmpowerment
        {
            get { return GetLevel( typeofDruidEmpowerment ); }
            set { SetLevel( typeofDruidEmpowerment, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int AnimalForm
        {
            get { return GetLevel( typeofAnimalForm ); }
            set { SetLevel( typeofAnimalForm, value ); }
        }
    }
}