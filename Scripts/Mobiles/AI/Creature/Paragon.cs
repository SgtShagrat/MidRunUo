using System;
using Midgard.Items;
using Server;
using Server.Items;

using Midgard.Engines.MidgardTownSystem;

namespace Server.Mobiles
{
	public class Paragon
	{
		public static double ChestChance = .10;         // Chance that a paragon will carry a paragon chest
		public static Map[] Maps         = new Map[]    // Maps that paragons will spawn on
		{
			Map.Ilshenar
		};
		
		#region modifica by Dies Irae per non permettere il paragon di creature troppo deboli o proibite
        public static double MinHitsToConvert = 100;	// Minimo degli hp del mostro per convertirsi

        private static readonly Type[] m_NotParagonableCreatures = new Type[]
		{
			typeof( Cow ),
			typeof( TownGuard ),
            typeof( PatrolGuard ),
            typeof( BarracoonPeerless )
		};

        public static bool IsNotParagonable( BaseCreature toparagon )
        {
            Type type = toparagon.GetType();
            bool contains = false;

            for( int i = 0; !contains && i < m_NotParagonableCreatures.Length; ++i )
                contains = ( m_NotParagonableCreatures[ i ] == type );

            return contains;
        }
		#endregion
		
		public static Type[] Artifacts = new Type[]
		{
			typeof( GoldBricks ), typeof( PhillipsWoodenSteed ), 
			typeof( AlchemistsBauble ), typeof( ArcticDeathDealer ),
			typeof( BlazeOfDeath ), typeof( BowOfTheJukaKing ),
			typeof( BurglarsBandana ), typeof( CavortingClub ),
			typeof( EnchantedTitanLegBone ), typeof( GwennosHarp ),
			typeof( IolosLute ), typeof( LunaLance ),
			typeof( NightsKiss ), typeof( NoxRangersHeavyCrossbow ),
			typeof( OrcishVisage ), typeof( PolarBearMask ),
			typeof( ShieldOfInvulnerability ), typeof( StaffOfPower ),
			typeof( VioletCourage ), typeof( HeartOfTheLion ), 
			typeof( WrathOfTheDryad ), typeof( PixieSwatter ), 
			typeof( GlovesOfThePugilist )
		};

        public static Type[] OldArtifacts = new Type[]
        {
            typeof( GoldBricks ),
            typeof( GlovesOfDexterityOne ),
            typeof( GlovesOfDexterityTwo ),
            typeof( GlovesOfDexterityThree ),
            typeof( OldMagicWizardsHatOne ),
            typeof( OldMagicWizardsHatTwo ),
            typeof( OldMagicWizardsHatThree ),
            typeof( BodySashOfStrengthOne ),
            typeof( BodySashOfStrengthTwo ),
            typeof( BodySashOfStrengthThree ),
            typeof( RingOfProtectionOne ),
            typeof( RingOfProtectionTwo ),
            typeof( RingOfProtectionThree ),
            typeof( RingOfProtectionFour ),
            typeof( RingOfProtectionFive ),
            typeof( RingOfProtectionSix )
        };

		public static int    Hue   = 0x501;        // Paragon hue
		
		// Buffs
		public static double HitsBuff   = 5.0;
		public static double StrBuff    = 1.05;
		public static double IntBuff    = 1.20;
		public static double DexBuff    = 1.20;
		public static double SkillsBuff = 1.20;
		public static double SpeedBuff  = 1.20;
		public static double FameBuff   = 1.40;
		public static double KarmaBuff  = 1.40;
		public static int    DamageBuff = 5;

		public static void Convert( BaseCreature bc )
		{
			if ( bc.IsParagon )
				return;

			// bc.Hue = Core.AOS ? Hue : Utility.RandomList( bc.ParagonHues ); // mod by Dies Irae

			if ( bc.HitsMaxSeed >= 0 )
				bc.HitsMaxSeed = (int)( bc.HitsMaxSeed * HitsBuff );
			
			bc.RawStr = (int)( bc.RawStr * StrBuff );
			bc.RawInt = (int)( bc.RawInt * IntBuff );
			bc.RawDex = (int)( bc.RawDex * DexBuff );

			bc.Hits = bc.HitsMax;
			bc.Mana = bc.ManaMax;
			bc.Stam = bc.StamMax;

			for( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = (Skill)bc.Skills[i];

				if ( skill.Base > 0.0 )
					skill.Base *= SkillsBuff;
			}

			bc.PassiveSpeed /= SpeedBuff;
			bc.ActiveSpeed /= SpeedBuff;

			bc.DamageMin += DamageBuff;
			bc.DamageMax += DamageBuff;

			if ( bc.Fame > 0 )
				bc.Fame = (int)( bc.Fame * FameBuff );

			if ( bc.Fame > 32000 )
				bc.Fame = 32000;

			// TODO: Mana regeneration rate = Sqrt( buffedFame ) / 4

			if ( bc.Karma != 0 )
			{
				bc.Karma = (int)( bc.Karma * KarmaBuff );

				if( Math.Abs( bc.Karma ) > 32000 )
					bc.Karma = 32000 * Math.Sign( bc.Karma );
			}
		}

		public static void UnConvert( BaseCreature bc )
		{
			if ( !bc.IsParagon )
				return;

			bc.Hue = 0;

			if ( bc.HitsMaxSeed >= 0 )
				bc.HitsMaxSeed = (int)( bc.HitsMaxSeed / HitsBuff );
			
			bc.RawStr = (int)( bc.RawStr / StrBuff );
			bc.RawInt = (int)( bc.RawInt / IntBuff );
			bc.RawDex = (int)( bc.RawDex / DexBuff );

			bc.Hits = bc.HitsMax;
			bc.Mana = bc.ManaMax;
			bc.Stam = bc.StamMax;

			for( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = (Skill)bc.Skills[i];

				if ( skill.Base > 0.0 )
					skill.Base /= SkillsBuff;
			}
			
			bc.PassiveSpeed *= SpeedBuff;
			bc.ActiveSpeed *= SpeedBuff;

			bc.DamageMin -= DamageBuff;
			bc.DamageMax -= DamageBuff;

			if ( bc.Fame > 0 )
				bc.Fame = (int)( bc.Fame / FameBuff );
			if ( bc.Karma != 0 )
				bc.Karma = (int)( bc.Karma / KarmaBuff );
		}

		public static bool CheckConvert( BaseCreature bc )
		{
			return CheckConvert( bc, bc.Location, bc.Map );
		}

		public static bool CheckConvert( BaseCreature bc, Point3D location, Map m )
		{
            // mod by Dies Irae
            //if ( !Core.AOS )
            //    return false;

			if ( Array.IndexOf( Maps, m ) == -1 )
				return false;

			#region modifica by Dies Irae per non permettere il paragon di creature troppo deboli o proibite
			if( bc.HitsMaxSeed < MinHitsToConvert || IsNotParagonable( bc ) )
			{
				return false;
			}
			#endregion
			
			if ( bc is BaseChampion || bc is Harrower || bc is BaseVendor || bc is BaseEscortable || bc is Clone )
				return false;

			int fame = bc.Fame;

			if ( fame > 32000 )
				fame = 32000;

			double chance = 1 / Math.Round( 20.0 - ( fame / 3200 ));

			return ( chance > Utility.RandomDouble() );
		}

		public static bool CheckArtifactChance( Mobile m, BaseCreature bc )
		{
            // mod by Dies Irae
            //if ( !Core.AOS )
            //    return false;

			double fame = (double)bc.Fame;

			if ( fame > 32000 )
				fame = 32000;

			double chance = 1 / ( Math.Max( 10, 100 * ( 0.83 - Math.Round( Math.Log( Math.Round( fame / 6000, 3 ) + 0.001, 10 ), 3 ) ) ) * ( 100 - Math.Sqrt( m.Luck ) ) / 100.0 );

		    chance *= 0.5; // mod by Dies Irae

			return chance > Utility.RandomDouble();
		}

		public static void GiveArtifactTo( Mobile m )
		{
		    Item item = null;

            if( Core.AOS )
                item = (Item)Activator.CreateInstance( Artifacts[ Utility.Random( Artifacts.Length ) ] );
            else
                item = (Item)Activator.CreateInstance( OldArtifacts[ Utility.Random( OldArtifacts.Length ) ] );

			#region modifica by Dies Irae per il drop artefatti
			Loot.StringToLog( string.Format( "Artefatto {0} (seriale {1}) droppato in data {2} al pg {3} (seriale {4} locazione {5}).",
				              		 item.GetType().Name, item.Serial, DateTime.Now, 
				              		 m.Name, m.Serial, m.Location ), "Logs/Midgard2ArtifactsLog.txt" );
			#endregion
			
			if ( m.AddToBackpack( item ) )
				m.SendMessage( "As a reward for slaying the mighty paragon, an artifact has been placed in your backpack." );
			else
				m.SendMessage( "As your backpack is full, your reward for destroying the legendary paragon has been placed at your feet." );
		}
	}
}