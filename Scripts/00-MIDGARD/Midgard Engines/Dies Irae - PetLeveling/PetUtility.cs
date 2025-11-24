/***************************************************************************
 *                                 		PetUtility.cs
 *                            		-------------------
 *  begin                	: Febbraio, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.IO;
using Midgard.Mobiles;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.PetSystem
{
    public class PetUtility
    {
        public static readonly double MinParPerc = 0.8;
        public static readonly double MaxParPerc = 1.0;

        public static void ScalePet( BaseCreature pet )
        {
            if( !PetLeveling.AosPetSystemEnabled )
                return;

            int strmin = (int)( GetParMin( PetPar.PetStr, pet ) * MinParPerc );
            int strmax = (int)( GetParMin( PetPar.PetStr, pet ) * MaxParPerc );

            int dexmin = (int)( GetParMin( PetPar.PetDex, pet ) * MinParPerc );
            int dexmax = (int)( GetParMin( PetPar.PetDex, pet ) * MaxParPerc );

            int intmin = (int)( GetParMin( PetPar.PetInt, pet ) * MinParPerc );
            int intmax = (int)( GetParMin( PetPar.PetInt, pet ) * MaxParPerc );

            int hitsmin = (int)( GetParMin( PetPar.PetHits, pet ) * MinParPerc );
            int hitsmax = (int)( GetParMin( PetPar.PetHits, pet ) * MaxParPerc );

            int dmgmmin = (int)( GetParMin( PetPar.PetMinDam, pet ) * MinParPerc );
            int dmgmmax = (int)( GetParMin( PetPar.PetMinDam, pet ) * MaxParPerc );

            int dmgMmin = (int)( GetParMin( PetPar.PetMaxDam, pet ) * MinParPerc );
            int dmgMmax = (int)( GetParMin( PetPar.PetMaxDam, pet ) * MaxParPerc );

            int resphysmin = (int)( GetParMin( PetPar.PetResPhys, pet ) * MinParPerc );
            int resphysmax = (int)( GetParMin( PetPar.PetResPhys, pet ) * MaxParPerc );

            int resfiremin = (int)( GetParMin( PetPar.PetResFire, pet ) * MinParPerc );
            int resfiremax = (int)( GetParMin( PetPar.PetResFire, pet ) * MaxParPerc );

            int rescoldmin = (int)( GetParMin( PetPar.PetResCold, pet ) * MinParPerc );
            int rescoldmax = (int)( GetParMin( PetPar.PetResCold, pet ) * MaxParPerc );

            int respoismin = (int)( GetParMin( PetPar.PetResPois, pet ) * MinParPerc );
            int respoismax = (int)( GetParMin( PetPar.PetResPois, pet ) * MaxParPerc );

            int resnrgymin = (int)( GetParMin( PetPar.PetResNrgy, pet ) * MinParPerc );
            int resnrgymax = (int)( GetParMin( PetPar.PetResNrgy, pet ) * MaxParPerc );

            int virarmin = (int)( GetParMin( PetPar.PetVirArm, pet ) * MinParPerc );
            int virarmax = (int)( GetParMin( PetPar.PetVirArm, pet ) * MaxParPerc );

            int lvlmin = (int)( GetParMin( PetPar.PetMaxLev, pet ) * MinParPerc );
            int lvlmax = (int)( GetParMin( PetPar.PetMaxLev, pet ) * MaxParPerc );

            pet.SetStr( strmin, strmax );
            pet.SetDex( dexmin, dexmax );
            pet.SetInt( intmin, intmax );

            pet.SetHits( hitsmin, hitsmax );
            pet.SetDamage( Utility.RandomMinMax( dmgmmin, dmgmmax ), Utility.RandomMinMax( dmgMmin, dmgMmax ) );

            pet.SetResistance( ResistanceType.Physical, resphysmin, resphysmax );
            pet.SetResistance( ResistanceType.Fire, resfiremin, resfiremax );
            pet.SetResistance( ResistanceType.Cold, rescoldmin, rescoldmax );
            pet.SetResistance( ResistanceType.Poison, respoismin, respoismax );
            pet.SetResistance( ResistanceType.Energy, resnrgymin, resnrgymax );

            pet.VirtualArmor = Utility.RandomMinMax( virarmin, virarmax );

            pet.MaxLevel = Utility.RandomMinMax( lvlmin, lvlmax );

            pet.RoarAttack = GetParMin( PetPar.RoarAttack, pet );
            pet.PetPoisonAttack = GetParMin( PetPar.PetPoisonAttack, pet );
            pet.FireBreathAttack = GetParMin( PetPar.FireBreathAttack, pet );
        }

        public static bool IsPackAnimal( Mobile pet )
        {
            for( int i = 0; i < m_PackAnimals.Length; i++ )
            {
                if( pet.GetType() == m_PackAnimals[ i ] )
                {
                    return true;
                }
            }
            return false;
        }

        #region m_PackAnimals
        private static Type[] m_PackAnimals = 
		{
			typeof(PackHorse),
			typeof(PackLlama),
			typeof(Beetle)
		};
        #endregion

        public static bool IsAlwaysFemale( Mobile pet )
        {
            for( int i = 0; i < m_AlwaysFemalePets.Length; i++ )
            {
                if( pet.GetType() == m_AlwaysFemalePets[ i ] )
                {
                    return true;
                }
            }
            return false;
        }

        #region m_AlwaysFemalePets
        private static Type[] m_AlwaysFemalePets =
		{
			typeof(Cow),
			typeof(Hind)
		};
        #endregion

        public static bool IsAlwaysMale( Mobile pet )
        {
            for( int i = 0; i < m_AlwaysMalePets.Length; i++ )
            {
                if( pet.GetType() == m_AlwaysMalePets[ i ] )
                {
                    return true;
                }
            }
            return false;
        }

        #region m_AlwaysMalePets
        private static Type[] m_AlwaysMalePets =
		{
			typeof(Bull),
			typeof(GreatHart)
		};
        #endregion

        public static void HandleSex( BaseCreature creature )
        {
            if( IsAlwaysMale( creature ) )
                creature.Female = false;
            else if( IsAlwaysFemale( creature ) )
                creature.Female = true;
            else
                creature.Female = Utility.RandomBool();
        }

        public static void HandleStartMaxLevel( BaseCreature creature )
        {
            int lvlmin = (int)( GetParMin( PetPar.PetMaxLev, creature ) * MinParPerc );
            int lvlmax = (int)( GetParMin( PetPar.PetMaxLev, creature ) * MaxParPerc );

            creature.MaxLevel = Utility.RandomMinMax( lvlmin, lvlmax );
        }

        public static void HandleAddNameProperties( BaseCreature creature, ObjectPropertyList list )
        {
            if( creature.Tamable && IsLevelablePet( creature ) )
            {
                if( creature.Female )
                    list.Add( 1060658, "Gender\tFemale" );
                else
                    list.Add( 1060658, "Gender\tMale" );

                if( creature.Generation > 1 )
                    list.Add( 1060658, "Generation\t{0}", creature.Generation );

                if( !creature.Controlled )
                    list.Add( 1060659, "Max Level\t{0}", creature.MaxLevel );
            }
        }

        public static bool DontRequireTaming( Mobile pet )
        {
            for( int i = 0; i < m_NoTamingRequiredPets.Length; i++ )
            {
                if( pet.GetType() == m_NoTamingRequiredPets[ i ] )
                {
                    return true;
                }
            }
            return false;
        }

        #region m_NoTamingRequiredPets
        private static Type[] m_NoTamingRequiredPets =
		{
			typeof(Horse),
			typeof(RidableLlama),
			typeof(ForestOstard),
			typeof(DesertOstard),
			typeof(SwampDragon),
			typeof(Ridgeback),
			typeof(PackHorse),
			typeof(PackLlama),
			typeof(Beetle)
		};
        #endregion

        public static bool IsLevelablePet( Mobile pet )
        {
            for( int i = 0; i < m_NonLevelablePets.Length; i++ )
            {
                if( pet.GetType() == m_NonLevelablePets[ i ] || pet.GetType().IsSubclassOf( m_NonLevelablePets[ i ] ) )
                {
                    return false;
                }
            }
            return true;
        }

        #region m_NonLevelablePets
        private static Type[] m_NonLevelablePets =
		{
			typeof(Golem),
			typeof(CoMWarHorse),
			typeof(MinaxWarHorse),
			typeof(SLWarHorse),
			typeof(TBWarHorse),
            typeof(BaseEscortable),
            typeof(BaseHireling)
		};
        #endregion

        public enum PetPar
        {
            PetStr,
            PetDex,
            PetInt,

            PetHits,
            PetStam,
            PetMana,

            PetResPhys,
            PetResFire,
            PetResCold,
            PetResPois,
            PetResNrgy,

            PetMinDam,
            PetMaxDam,

            PetVirArm,
            PetMaxLev,

            PetYoungTime,
            PetMinBreedDelay,

            RoarAttack,
            PetPoisonAttack,
            FireBreathAttack,
        }

        public static int GetValueForCreature( BaseCreature creature, PetPar parameter )
        {
            switch( parameter )
            {
                case PetPar.PetStr: return creature.RawStr;
                case PetPar.PetDex: return creature.RawDex;
                case PetPar.PetInt: return creature.RawInt;

                case PetPar.PetHits: return creature.HitsMax;
                case PetPar.PetStam: return creature.StamMax;
                case PetPar.PetMana: return creature.ManaMax;

                case PetPar.PetResPhys: return creature.PhysicalResistance;
                case PetPar.PetResFire: return creature.FireResistance;
                case PetPar.PetResCold: return creature.ColdResistance;
                case PetPar.PetResPois: return creature.PoisonResistance;
                case PetPar.PetResNrgy: return creature.EnergyResistance;

                case PetPar.PetMinDam: return creature.DamageMin;
                case PetPar.PetMaxDam: return creature.DamageMax;

                case PetPar.PetVirArm: return creature.VirtualArmor;
                case PetPar.PetMaxLev: return creature.MaxLevel;

                case PetPar.PetYoungTime: return -1;
                case PetPar.PetMinBreedDelay: return -1;

                case PetPar.RoarAttack: return creature.RoarAttack;
                case PetPar.PetPoisonAttack: return creature.PetPoisonAttack;
                case PetPar.FireBreathAttack: return creature.FireBreathAttack;
            }

            return -1;
        }

        public static void SetValueForCreature( BaseCreature creature, PetPar parameter, int value )
        {
            switch( parameter )
            {
                case PetPar.PetStr: creature.RawStr = value; break;
                case PetPar.PetDex: creature.RawDex = value; break;
                case PetPar.PetInt: creature.RawInt = value; break;

                case PetPar.PetHits: creature.HitsMaxSeed = value; break;
                case PetPar.PetStam: creature.StamMaxSeed = value; break;
                case PetPar.PetMana: creature.ManaMaxSeed = value; break;

                case PetPar.PetResPhys: creature.PhysicalResistanceSeed = value; break;
                case PetPar.PetResFire: creature.FireResistSeed = value; break;
                case PetPar.PetResCold: creature.ColdResistSeed = value; break;
                case PetPar.PetResPois: creature.PoisonResistSeed = value; break;
                case PetPar.PetResNrgy: creature.EnergyResistSeed = value; break;

                case PetPar.PetMinDam: creature.DamageMin = value; break;
                case PetPar.PetMaxDam: creature.DamageMax = value; break;

                case PetPar.PetVirArm: creature.VirtualArmor = value; break;
                case PetPar.PetMaxLev: creature.MaxLevel = value; break;

                case PetPar.PetYoungTime: break;
                case PetPar.PetMinBreedDelay: break;

                case PetPar.RoarAttack: creature.RoarAttack = value; break;
                case PetPar.PetPoisonAttack: creature.PetPoisonAttack = value; break;
                case PetPar.FireBreathAttack: creature.FireBreathAttack = value; break;
            }
        }

        #region CannotBeBreeded
        public static bool CannotBeBreeded( Mobile pet )
        {
            for( int i = 0; i < m_CannotBeBreededList.Length; i++ )
            {
                if( pet.GetType() == m_CannotBeBreededList[ i ] || pet.GetType().IsSubclassOf( m_CannotBeBreededList[ i ] ) )
                {
                    return true;
                }
            }
            return false;
        }

        private static Type[] m_CannotBeBreededList =
		{
			typeof(BaseMustang),
			typeof(BaseOstard),
			typeof(BaseLlama),
			};
        #endregion

        #region m_PetList
        private static Type[] m_PetList =
		{
			// 2 grandi
			typeof(Dragon),
			typeof(WhiteWyrm),
			
			// 6 medi
			typeof(Nightmare),
			typeof(Kirin),
			typeof(Unicorn),
			typeof(SilverSteed),
			typeof(FireSteed),
			typeof(InfernalLlama),
			
			// 2 tokuno
			typeof(Hiryu),
			typeof(LesserHiryu),
			
			// 12 Mustang
			typeof(BloodMustang),
			typeof(CelestialMustang),
			typeof(CloudMustang),
			typeof(DwarvenMustang),
			typeof(ElvenMustang),
			typeof(GlacialMustang),
			typeof(GreyishMustang),
			typeof(JungleMustang),
			typeof(OldriverMustang),
			typeof(SpectralMustang),
			typeof(StormMustang),
			typeof(VulcanMustang),
			
			// 13 Lama
			typeof(BloodRidableLlama),
			typeof(EmeraldRidableLlama),
			typeof(FireRidableLlama),
			typeof(GoldenRidableLlama),
			typeof(GreyishRidableLlama),
			typeof(HighlandRidableLlama),
			typeof(MountainRidableLlama),
			typeof(PlainRidableLlama),
			typeof(ShadowRidableLlama),
			typeof(ShiningRidableLlama),
			typeof(SnowRidableLlama),
			typeof(StoneRidableLlama),
			typeof(VulcanRidableLlama),
				
			// 15 Ostard
			typeof(EmeraldFrenziedOstard),
			typeof(FireFrenziedOstard),
			typeof(GoldenFrenziedOstard),
			typeof(HeavenlyFrenziedOstard),
			typeof(HighlandFrenziedOstard),
			typeof(IceFrenziedOstard),
			typeof(MountainFrenziedOstard),
			typeof(PlainFrenziedOstard),
			typeof(RubyFrenziedOstard),
			typeof(ShadowFrenziedOstard),
			typeof(SnowFrenziedOstard),
			typeof(StoneFrenziedOstard),
			typeof(SwampFrenziedOstard),
			typeof(TropicalFrenziedOstard),
			typeof(ValleyFrenziedOstard),		
			
			// 10 vari
			typeof(Raptor),
			typeof(RidableDrake),
			typeof(RidablePolarBear),
			typeof(RidableVortex),
			typeof(Beetle),
			typeof(FireBeetle),
			typeof(RuneBeetle),
			typeof(Drake),
			typeof(Chimera),
			typeof(CuSidhe),
			};
        #endregion

        #region tables
        private static int[ , ] m_TableMax = 
		{
		//	  str  dex  int  hit   sta  man   rp  rf  rc  rp  re  dm  dM  va  mL  yt  mbd ra pa fb
			// 2 grandi
			{ 300, 100, 150, 1000, 500, 1000, 50, 50, 30, 50, 50, 16, 22, 60, 30, 30, 30, 100, 0, 100 },  // Dragon
			{ 300, 100, 150, 1000, 500, 1000, 50, 30, 50, 50, 50, 17, 25, 64, 30, 30, 60, 100, 0, 100 },  // WhiteWyrm
			
			// 6 medi
			{ 200,  50, 100, 1000, 500, 1000, 50, 50, 50, 50, 30, 16, 22, 60, 30, 30, 30, 0, 100, 100 },  // Nightmare
			{ 200,  50, 100, 1000, 500, 1000, 50, 50, 30, 50, 50, 16, 22, 60, 30, 30, 30, 50, 0, 50  },  // Kirin
			{ 200,  50, 100, 1000, 500, 1000, 50, 50, 50, 50, 50, 16, 22, 60, 30, 30, 30, 30, 0, 0 },  // Unicorn
			{ 200,  50, 100, 1000, 500, 1000, 50, 50, 50, 50, 50, 16, 22, 60, 30, 600, 600, 50, 0, 50 },  // SilverSteed
			{ 200,  50, 100, 1000, 500, 1000, 50, 50, 50, 50, 50, 16, 22, 60, 30, 600, 600, 30, 0, 100 },  // Fire steed
			{ 200,  50, 100, 1000, 500, 1000, 50, 50, 50, 50, 50, 16, 22, 60, 30, 600, 600, 0, 50, 100 },  // InfernalLama
			
			// 2 tokuno
			{ 300, 100, 150, 1000, 500, 1000, 50, 50, 30, 50, 50, 16, 22, 60, 30, 30, 30, 50, 50, 50 },  // Hiryu
			{ 200,  50, 100, 1000, 500, 1000, 50, 50, 30, 50, 50, 16, 22, 60, 30, 30, 30, 30, 30, 30 },  // LesserHiryu
			
			// 12 mustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // BloodMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // CelestialMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // CloudMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // DwarvenMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // ElvenMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // GlacialMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // GreyishMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // JungleMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // OldriverMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // SpectralMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // StormMustang
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 30 },  // VulcanMustang
			
			// 13 lama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // BloodRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // EmeraldRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // FireRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // GoldenRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // GreyishRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // HighlandRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // MountainRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // PlainRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // ShadowRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // ShiningRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // SnowRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // StoneRidableLlama
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // VulcanRidableLlama
							
			// 15 ostard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // EmeraldFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // FireFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // GoldenFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // HeavenlyFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // HighlandFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // IceFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // MountainFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // PlainFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // RubyFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // ShadowFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // SnowFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // StoneFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // SwampFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // TropicalFrenziedOstard
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // ValleyFrenziedOstard
			
			// 10 vari
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // Raptor
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 30, 0, 30 },  // RidableDrake
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // RidablePolarBear
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 5, 600, 600, 0, 0, 0 },  // RidableVortex
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5, 6,  30, 30, 15, 15, 0, 0, 0 },  // Beetle
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5, 6,  30, 30, 30, 30, 0, 0, 30 },  // FireBeetle
			{ 200,  50, 100, 1000, 500, 1000, 50, 70, 30, 50, 50, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // RuneBeetle
			{ 200,  50, 100, 1000, 500, 1000, 50, 50, 50, 50, 30, 16, 22, 60, 30, 30, 30, 30, 0, 30 },  // Drake
			{ 200,  50, 100, 1000, 500, 1000, 50, 50, 50, 50, 30, 16, 22, 60, 30, 30, 30, 100, 100, 100 },  // Chimera
			{ 200,  50, 100, 1000, 500, 1000, 50, 50, 50, 50, 30, 16, 22, 60, 30, 30, 30, 30, 30, 30 },  // Cu-Shide
			
			{ 100,  50, 100, 1000, 500, 1000, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // Altri pet
		};

        private static int[ , ] m_TableMin = 
		{
		//	  str  dex  int  hit  sta  man  rp  rf  rc  rp  re  dm  dM  va  mL  yt  mbd  ra pa fb
			// 2 grandi
			{ 300, 100, 150, 350, 150, 200, 50, 50, 30, 50, 50, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // Dragon
			{ 300, 100, 150, 350, 150, 200, 50, 30, 50, 50, 50, 17, 25, 64, 30, 30, 60, 0, 0, 0 },  // WhiteWyrm
			
			// 6 medi 
			{ 200,  50, 100, 250, 100, 150, 50, 50, 50, 50, 30, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // Nightmare
			{ 200,  50, 100, 250, 100, 150, 50, 50, 30, 50, 50, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // Kirin
			{ 200,  50, 100, 250, 100, 150, 50, 50, 50, 50, 50, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // Unicorn
			{ 200,  50, 100, 250, 100, 150, 50, 50, 50, 50, 50, 16, 22, 60, 30, 600, 600, 0, 0, 0},  // Silver Steed
			{ 200,  50, 100, 250, 100, 150, 50, 50, 50, 50, 50, 16, 22, 60, 30, 600, 600, 0, 0, 0 },  // Fire steed
			{ 200,  50, 100, 250, 100, 150, 50, 50, 50, 50, 50, 16, 22, 60, 30, 600, 600, 0, 0, 0 },  // InfernalLama
			
			// 2 tokuno
			{ 300, 100, 150, 350, 150, 200, 50, 50, 30, 50, 50, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // Hiryu
			{ 200,  50, 100, 250, 100, 150, 50, 50, 30, 50, 50, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // LesserHiryu
			
			// 12 Mustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // BloodMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // CelestialMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // CloudMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // DwarvenMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // ElvenMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // GlacialMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // GreyishMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // JungleMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // OldriverMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // SpectralMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // StormMustang
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // VulcanMustang
			
			// 13 Lama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // BloodRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // EmeraldRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // FireRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // GoldenRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // GreyishRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // HighlandRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // MountainRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // PlainRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // ShadowRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // ShiningRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // SnowRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // StoneRidableLlama
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // VulcanRidableLlama
								
			// 15 Ostard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // EmeraldFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // FireFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // GoldenFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // HeavenlyFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // HighlandFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // IceFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // MountainFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // PlainFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // RubyFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // ShadowFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // SnowFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // StoneFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // SwampFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // TropicalFrenziedOstard
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // ValleyFrenziedOstard
			
			// 10 vari
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // Raptor
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // RidableDrake
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // RidablePolarBear
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // RidableVortex
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 15, 15, 0, 0, 0 },  // Beetle
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 30, 30, 0, 0, 0 },  // FireBeetle
			{ 200,  50, 100, 250, 100, 150, 50, 50, 30, 50, 50, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // RuneBeetle
			{ 200,  50, 100, 250, 100, 150, 50, 50, 50, 50, 30, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // Drake
			{ 200,  50, 100, 250, 100, 150, 50, 50, 50, 50, 30, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // Chimera
			{ 200,  50, 100, 250, 100, 150, 50, 50, 50, 50, 30, 16, 22, 60, 30, 30, 30, 0, 0, 0 },  // CuShide
			
			{ 20,   15,  15,  20,  20,  20, 40, 40, 40, 40, 40,  5,  6, 30, 10, 60, 60, 0, 0, 0 },  // Altri pet
		};
        #endregion

        public static bool IsScalablePet( Mobile pet )
        {
            for( int i = 0; i < m_PetList.Length; i++ )
            {
                if( pet.GetType() == m_PetList[ i ] )
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetParMax( PetPar parametro, BaseCreature pet )
        {
            // NB: In una matrice bidimensionale gli indici sono [X,Y]
            for( int i = 0; i < m_PetList.Length; i++ )
            {
                if( pet.GetType() == m_PetList[ i ] )
                {
                    return m_TableMax[ i, ( (int)parametro ) ];
                }
            }

            // Se non e' nei parametri precedenti allora applica l'ultima categoria
            return m_TableMax[ m_PetList.Length, ( (int)parametro ) ];
        }

        public static int GetParMin( PetPar parametro, BaseCreature pet )
        {
            // NB: In una matrice bidimensionale gli indici sono [X,Y]
            for( int i = 0; i < m_PetList.Length; i++ )
            {
                if( pet.GetType() == m_PetList[ i ] )
                {
                    return m_TableMin[ i, ( (int)parametro ) ];
                }
            }

            // Se non e' nei parametri precedenti allora applica l'ultima categoria
            return m_TableMin[ m_PetList.Length, ( (int)parametro ) ];
        }

        public static void CheckParameter( Mobile from, BaseCreature bc, PetPar parameter, bool message, bool resendGump )
        {
            if( !PetLeveling.AosPetSystemEnabled )
                return;

            if( GetValueForCreature( bc, parameter ) >= GetParMax( parameter, bc ) )
            {
                if( message )
                    from.SendMessage( "This cannot gain any farther." );

                if( resendGump && bc.AbilityPoints > 0 )
                    from.SendGump( new PetLevelGump( bc ) );
            }
            else
            {
                if( bc.AbilityPoints <= 0 )
                {
                    if( message )
                        from.SendMessage( "Your pet lacks the ability points to do that." );
                }
                else
                    IncreaseParameter( from, bc, parameter, message, resendGump );
            }
        }

        public static void IncreaseParameter( Mobile from, BaseCreature m_Pet, PetPar parameter, bool message, bool resendGump )
        {
            if( !PetLeveling.AosPetSystemEnabled )
                return;

            switch( parameter )
            {
                case PetPar.PetHits:
                    if( m_Pet.HitsMaxSeed != -1 )
                        m_Pet.HitsMaxSeed += 1;
                    else
                        m_Pet.HitsMaxSeed = m_Pet.HitsMax + 1;
                    break;
                case PetPar.PetStam:
                    if( m_Pet.StamMaxSeed != -1 )
                        m_Pet.StamMaxSeed += 1;
                    else
                        m_Pet.StamMaxSeed = m_Pet.StamMax + 1;
                    break;
                case PetPar.PetMana:
                    if( m_Pet.ManaMaxSeed != -1 )
                        m_Pet.ManaMaxSeed += 1;
                    else
                        m_Pet.ManaMaxSeed = m_Pet.ManaMax + 1;
                    break;
                case PetPar.PetResPhys:
                    m_Pet.PhysicalResistanceSeed += 1;
                    break;
                case PetPar.PetResFire:
                    m_Pet.FireResistSeed += 1;
                    break;
                case PetPar.PetResCold:
                    m_Pet.ColdResistSeed += 1;
                    break;
                case PetPar.PetResPois:
                    m_Pet.PoisonResistSeed += 1;
                    break;
                case PetPar.PetResNrgy:
                    m_Pet.EnergyResistSeed += 1;
                    break;
                case PetPar.PetMinDam:
                    m_Pet.DamageMin += 1;
                    break;
                case PetPar.PetMaxDam:
                    m_Pet.DamageMax += 1;
                    break;
                case PetPar.PetVirArm:
                    m_Pet.VirtualArmor += 1;
                    break;
                case PetPar.RoarAttack:
                    m_Pet.RoarAttack++;
                    break;
                case PetPar.PetPoisonAttack:
                    m_Pet.PetPoisonAttack++;
                    break;
                case PetPar.FireBreathAttack:
                    m_Pet.FireBreathAttack++;
                    break;
                default: break;
            }

            m_Pet.AbilityPoints--;

            if( resendGump && m_Pet.AbilityPoints > 0 )
                from.SendGump( new PetLevelGump( m_Pet ) );
        }

        public static void PetNormalize( BaseCreature pet )
        {
            if( !PetLeveling.AosPetSystemEnabled )
                return;

            PetNormalize( pet, false );
        }

        public static void PetNormalize( BaseCreature pet, bool refundPoints )
        {
            if( !PetLeveling.AosPetSystemEnabled )
                return;

            // SOLO Forza Destrezza e Intelligenza vengono scalate secondo la rarita'.
            double RarInc = ( 1.0 + ( (int)pet.RarityLevel * 5.0 / 100.0 ) );

            if( pet.RawStr > (int)( GetParMax( PetPar.PetStr, pet ) * RarInc ) )
                pet.RawStr = (int)( GetParMax( PetPar.PetStr, pet ) * RarInc );

            if( pet.RawDex > (int)( GetParMax( PetPar.PetDex, pet ) * RarInc ) )
                pet.RawDex = (int)( GetParMax( PetPar.PetDex, pet ) * RarInc );

            if( pet.RawInt > (int)( GetParMax( PetPar.PetInt, pet ) * RarInc ) )
                pet.RawInt = (int)( GetParMax( PetPar.PetInt, pet ) * RarInc );

            foreach( int i in Enum.GetValues( typeof( PetPar ) ) )
            {
                if( i == (int)PetPar.PetMaxLev || i == (int)PetPar.PetYoungTime || i == (int)PetPar.PetMinBreedDelay )
                    continue;

                int value = GetValueForCreature( pet, (PetPar)i );
                int maxValue = GetParMax( (PetPar)i, pet );

                if( value > maxValue )
                {
                    if( refundPoints )
                        pet.AbilityPoints += value - maxValue;
                    SetValueForCreature( pet, (PetPar)i, maxValue );
                }
            }

            int ml = GetParMax( PetPar.PetMaxLev, pet );
            if( pet.MaxLevel > ml )
                pet.MaxLevel = ml;

            if( pet.Level > pet.MaxLevel )
                pet.Level = pet.MaxLevel;
        }

        public static void VerifyPoints_Callback( object state )
        {
            if( !PetLeveling.AosPetSystemEnabled )
                return;

            BaseCreature pet = (BaseCreature)state;
            if( pet == null )
                return;

            if( !pet.Tamable || pet.Summoned || !pet.Controlled || !IsScalablePet( pet ) || pet.ControlMaster == null )
                return;

            double RarInc = ( 1.0 + ( (int)pet.RarityLevel * 5.0 / 100.0 ) );

            int str = (int)( GetParMax( PetPar.PetStr, pet ) * RarInc );
            if( pet.RawStr > str )
            {
                Log( pet, PetPar.PetStr, str, pet.RawStr );
                pet.RawStr = GetParMax( PetPar.PetStr, pet );
            }

            int dex = (int)( GetParMax( PetPar.PetDex, pet ) * RarInc );
            if( pet.RawDex > dex )
            {
                Log( pet, PetPar.PetDex, dex, pet.RawDex );
                pet.RawDex = dex;
            }

            int inte = (int)( GetParMax( PetPar.PetInt, pet ) * RarInc );
            if( pet.RawInt > inte )
            {
                Log( pet, PetPar.PetInt, inte, pet.RawInt );
                pet.RawInt = inte;
            }

            foreach( int i in Enum.GetValues( typeof( PetPar ) ) )
            {
                if( i == (int)PetPar.PetMaxLev || i == (int)PetPar.PetYoungTime || i == (int)PetPar.PetMinBreedDelay )
                    continue;

                int value = GetValueForCreature( pet, (PetPar)i );
                int maxValue = GetParMax( (PetPar)i, pet );

                if( value > maxValue )
                {
                    Log( pet, (PetPar)i, value, maxValue );
                    pet.AbilityPoints += value - maxValue;
                    SetValueForCreature( pet, (PetPar)i, maxValue );
                }
            }

            int ml = GetParMax( PetPar.PetMaxLev, pet );
            if( pet.MaxLevel > ml )
            {
                Log( pet, PetPar.PetMaxLev, ml, pet.MaxLevel );
                pet.MaxLevel = ml;
            }

            if( pet.Level > pet.MaxLevel )
                pet.Level = pet.MaxLevel;
        }

        private static void Log( Mobile pet, PetPar par, int newValue, int oldValue )
        {
            try
            {
                using( StreamWriter op = new StreamWriter( "Logs/pet-parameters-errors.log", true ) )
                {
                    op.WriteLine( DateTime.Now );
                    op.WriteLine( "pet type {0}, name {1}, serial {2}, has wrong {3} value ({4}). Now it is set to {5}.",
                        pet.GetType().Name, string.IsNullOrEmpty( pet.Name ) ? "null" : pet.Name, pet.Serial,
                        par, oldValue, newValue );
                    op.WriteLine();
                }
            }
            catch
            {
            }
        }
    }
}