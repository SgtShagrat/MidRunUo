using System;
using Server;
using Server.Mobiles;
using Server.Factions;

using Server.Engines.XmlPoints;

using Midgard.Engines.SkillSystem;
using SkillSystem = Midgard.Engines.SkillSystem;

namespace Server.Misc
{
	public class SkillCheck
	{
		private static readonly bool AntiMacroCode = false;		//Change this to false to disable anti-macro code

		public static TimeSpan AntiMacroExpire = TimeSpan.FromMinutes( 5.0 ); //How long do we remember targets/locations?
		public const int Allowance = 3;	//How many times may we use the same location/target for gain
		private const int LocationSize = 5; //The size of eeach location, make this smaller so players dont have to move as far
		private static bool[] UseAntiMacro = new bool[]
		{
			// true if this skill uses the anti-macro code, false if it does not
			false,// Alchemy = 0,
			true,// Anatomy = 1,
			true,// AnimalLore = 2,
			true,// ItemID = 3,
			true,// ArmsLore = 4,
			false,// Parry = 5,
			true,// Begging = 6,
			false,// Blacksmith = 7,
			false,// Fletching = 8,
			true,// Peacemaking = 9,
			true,// Camping = 10,
			false,// Carpentry = 11,
			false,// Cartography = 12,
			false,// Cooking = 13,
			true,// DetectHidden = 14,
			true,// Discordance = 15,
			true,// EvalInt = 16,
			true,// Healing = 17,
			true,// Fishing = 18,
			true,// Forensics = 19,
			true,// Herding = 20,
			true,// Hiding = 21,
			true,// Provocation = 22,
			false,// Inscribe = 23,
			true,// Lockpicking = 24,
			true,// Magery = 25,
			true,// MagicResist = 26,
			false,// Tactics = 27,
			true,// Snooping = 28,
			true,// Musicianship = 29,
			true,// Poisoning = 30,
			false,// Archery = 31,
			true,// SpiritSpeak = 32,
			true,// Stealing = 33,
			false,// Tailoring = 34,
			true,// AnimalTaming = 35,
			true,// TasteID = 36,
			false,// Tinkering = 37,
			true,// Tracking = 38,
			true,// Veterinary = 39,
			false,// Swords = 40,
			false,// Macing = 41,
			false,// Fencing = 42,
			false,// Wrestling = 43,
			true,// Lumberjacking = 44,
			true,// Mining = 45,
			true,// Meditation = 46,
			true,// Stealth = 47,
			true,// RemoveTrap = 48,
			true,// Necromancy = 49,
			false,// Focus = 50,
			true,// Chivalry = 51
			true,// Bushido = 52
			true,//Ninjitsu = 53
			true // Spellweaving
		};

		public static void Initialize()
		{
			#region ARTEGORDONMOD to enable XmlSpawner skill triggering
			/*			
			Mobile.SkillCheckLocationHandler = new SkillCheckLocationHandler( Mobile_SkillCheckLocation );
			Mobile.SkillCheckDirectLocationHandler = new SkillCheckDirectLocationHandler( Mobile_SkillCheckDirectLocation );

			Mobile.SkillCheckTargetHandler = new SkillCheckTargetHandler( Mobile_SkillCheckTarget );
			Mobile.SkillCheckDirectTargetHandler = new SkillCheckDirectTargetHandler( Mobile_SkillCheckDirectTarget );
			*/
			
			Mobile.SkillCheckLocationHandler = new SkillCheckLocationHandler( XmlSpawnerSkillCheck.Mobile_SkillCheckLocation );
			Mobile.SkillCheckDirectLocationHandler = new SkillCheckDirectLocationHandler( XmlSpawnerSkillCheck.Mobile_SkillCheckDirectLocation );
			
			Mobile.SkillCheckTargetHandler = new SkillCheckTargetHandler( XmlSpawnerSkillCheck.Mobile_SkillCheckTarget );
			Mobile.SkillCheckDirectTargetHandler = new SkillCheckDirectTargetHandler( XmlSpawnerSkillCheck.Mobile_SkillCheckDirectTarget );
			#endregion
		}

		public static bool Mobile_SkillCheckLocation( Mobile from, SkillName skillName, double minSkill, double maxSkill )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			double value = skill.Value;

			if ( value < minSkill )
				return false; // Too difficult
			else if ( value >= maxSkill )
				return true; // No challenge

			double chance = (value - minSkill) / (maxSkill - minSkill);

			Point2D loc = new Point2D( from.Location.X / LocationSize, from.Location.Y / LocationSize );
			return CheckSkill( from, skill, loc, chance );
		}

		public static bool Mobile_SkillCheckDirectLocation( Mobile from, SkillName skillName, double chance )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			if ( chance < 0.0 )
				return false; // Too difficult
			else if ( chance >= 1.0 )
				return true; // No challenge

			Point2D loc = new Point2D( from.Location.X / LocationSize, from.Location.Y / LocationSize );
			return CheckSkill( from, skill, loc, chance );
		}

		public static bool CheckSkill( Mobile from, Skill skill, object amObj, double chance )
		{
            if( !Core.AOS )
            {
                if( XmlPointsAttach.AreInAnyGame( from ) )
                    return false;

                try
                {
                    OldStatGainSystem.CheckGain( from, (SkillName)skill.SkillID );
                    return SkillSystem.Core.PolCheckSkill( from, skill, amObj, chance );
                }
                catch( Exception e )
                {
                    Console.WriteLine( e );
                    Console.WriteLine( "Warning: error in PolCheckSkill: from: " + from.Name + " skill: " + skill.Name );
                    return false;
                }
            }

			if ( from.Skills.Cap == 0 )
				return false;

			bool success = ( chance >= Utility.RandomDouble() );
			double gc = (double)(from.Skills.Cap - from.Skills.Total) / from.Skills.Cap;
			gc += ( skill.Cap - skill.Base ) / skill.Cap;
			gc /= 2;

			gc += ( 1.0 - chance ) * ( success ? 0.5 : (Core.AOS ? 0.0 : 0.2) );
			gc /= 2;

			gc *= skill.Info.GainFactor;

			if ( gc < 0.01 )
				gc = 0.01;

			if ( from is BaseCreature && ((BaseCreature)from).Controlled )
				gc *= 2;

			if ( from.Alive && ( ( gc >= Utility.RandomDouble() && AllowGain( from, skill, amObj ) ) || skill.Base < 10.0 ) )
				Gain( from, skill );

			return success;
		}

		public static bool Mobile_SkillCheckTarget( Mobile from, SkillName skillName, object target, double minSkill, double maxSkill )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			double value = skill.Value;

			if ( value < minSkill )
				return false; // Too difficult
			else if ( value >= maxSkill )
				return true; // No challenge

			double chance = (value - minSkill) / (maxSkill - minSkill);

			return CheckSkill( from, skill, target, chance );
		}

		public static bool Mobile_SkillCheckDirectTarget( Mobile from, SkillName skillName, object target, double chance )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			if ( chance < 0.0 )
				return false; // Too difficult
			else if ( chance >= 1.0 )
				return true; // No challenge

			return CheckSkill( from, skill, target, chance );
		}

	    public static bool AllowGain( Mobile from, Skill skill, object obj )
		{
			if ( Core.AOS && Faction.InSkillLoss( from ) )	//Changed some time between the introduction of AoS and SE.
				return false;

			if ( AntiMacroCode && from is PlayerMobile && UseAntiMacro[skill.Info.SkillID] )
				return ((PlayerMobile)from).AntiMacroCheck( skill, obj );
			else
				return true;
		}

		public enum Stat { Str, Dex, Int }

		public static void Gain( Mobile from, Skill skill )
		{
			if ( from.Region.IsPartOf( typeof( Regions.Jail ) ) )
				return;

			if ( from is BaseCreature && ((BaseCreature)from).IsDeadPet )
				return;

			if ( skill.SkillName == SkillName.Focus && from is BaseCreature )
				return;

            if( from.Player && ( SkillSubCap.IsUnderAnySubCap( skill.SkillName ) && !SkillSubCap.CanGainUnderSubCaps( from, skill.SkillName ) ) )
                return;

			if ( skill.Base < skill.Cap && skill.Lock == SkillLock.Up )
			{
                bool gainAllowed = ( SkillSystem.Config.RateOverTimeEnabled ? RateOverTime.SkillGainAllowed( from, skill ) : true ); // mod by Dies Irae

				int toGain = 1;

				if ( skill.Base <= 10.0 )
					toGain = Utility.Random( 4 ) + 1;

				Skills skills = from.Skills;

                if( from.Player && gainAllowed && ( skills.Total / skills.Cap ) >= Utility.RandomDouble() )//( skills.Total >= skills.Cap ) // mod by Dies Irae
				{
					for ( int i = 0; i < skills.Length; ++i )
					{
						Skill toLower = skills[i];

						if ( toLower != skill && toLower.Lock == SkillLock.Down && toLower.BaseFixedPoint >= toGain )
						{
							toLower.BaseFixedPoint -= toGain;
							break;
						}
					}
				}			
				
				#region Mondain's Legacy
				if ( from is PlayerMobile )
					if ( Engines.Quests.QuestHelper.EnhancedSkill( (PlayerMobile) from, skill ) )
						toGain *= Utility.RandomMinMax( 2, 4 );
				#endregion

				#region Scroll of Alacrity
				PlayerMobile pm = from as PlayerMobile;

				if ( from is PlayerMobile )
					if (pm != null && skill.SkillName == pm.AcceleratedSkill && pm.AcceleratedStart > DateTime.Now)
					toGain *= Utility.RandomMinMax(2, 5);
					#endregion

				if ( gainAllowed && ( !from.Player || (skills.Total + toGain) <= skills.Cap ) )// mod by Dies Irae
				{
					skill.BaseFixedPoint += toGain;

                    #region mod by Dies Irae
                    if( SkillSystem.Config.GuaranteedGainSystemEnabled )
                        GuaranteedGainSystem.RegisterSkillGain( from, skill );
                    #endregion
				}
			}
				
			#region Mondain's Legacy
			if ( from is PlayerMobile )
				Engines.Quests.QuestHelper.CheckSkill( (PlayerMobile) from, skill );
			#endregion

			if ( skill.Lock == SkillLock.Up )
			{
				SkillInfo info = skill.Info;

                #region mod by Dies Irae
                /*
                if( SkillSystem.Config.GuaranteedGainSystemEnabled && SkillSystem.GuaranteedGainSystem.ForceStatGain( from ) )
                {
                    switch( Utility.Random( 3 ) )
                    {
                        case 0:
                            {
                                if( from.StrLock == StatLockType.Up )
                                    GainStat( from, Stat.Str );
                                break;
                            }
                        case 1:
                            {
                                if( from.DexLock == StatLockType.Up )
                                    GainStat( from, Stat.Dex );
                                break;
                            }
                        case 2:
                            {
                                if( from.IntLock == StatLockType.Up )
                                    GainStat( from, Stat.Int );
                                break;
                            }
                    }
                }
                */
                #endregion
				/* 
                else  if ( from.StrLock == StatLockType.Up && (info.StrGain / 33.3) > Utility.RandomDouble() )
					GainStat( from, Stat.Str );
				else if ( from.DexLock == StatLockType.Up && (info.DexGain / 33.3) > Utility.RandomDouble() )
					GainStat( from, Stat.Dex );
				else if ( from.IntLock == StatLockType.Up && (info.IntGain / 33.3) > Utility.RandomDouble() )
					GainStat( from, Stat.Int );
                */
			}
		}

		public static bool CanLower( Mobile from, Stat stat )
		{
			switch ( stat )
			{
				case Stat.Str: return ( from.StrLock == StatLockType.Down && from.RawStr > 10 );
				case Stat.Dex: return ( from.DexLock == StatLockType.Down && from.RawDex > 10 );
				case Stat.Int: return ( from.IntLock == StatLockType.Down && from.RawInt > 10 );
			}

			return false;
		}

		public static bool CanRaise( Mobile from, Stat stat )
		{
			if ( !(from is BaseCreature && ((BaseCreature)from).Controlled) )
			{
				if ( from.RawStatTotal >= from.StatCap )
					return false;
			}
			else//allenamento tamati
			{
				int maxstat = 200;
				Mobile master = ((BaseCreature)from).GetMaster();
				if ( master != null )
					maxstat = (int)(( master.Skills[ SkillName.AnimalTaming ].Value )* 2 + master.Skills[ SkillName.AnimalLore ].Value + (master.Skills[ SkillName.Herding ].Value)*3);
				//from.Say("MaxSTAT = {0}", maxstat);
				switch ( stat )
				{
					case Stat.Str: return ( from.StrLock == StatLockType.Up && from.RawStr < maxstat );
					case Stat.Dex: return ( from.DexLock == StatLockType.Up && from.RawDex < maxstat );
					case Stat.Int: return ( from.IntLock == StatLockType.Up && from.RawInt < maxstat );
				}
			}

			switch ( stat )
			{
				case Stat.Str: return ( from.StrLock == StatLockType.Up && from.RawStr < ( Core.AOS ? 125 : 100 ) );
				case Stat.Dex: return ( from.DexLock == StatLockType.Up && from.RawDex < ( Core.AOS ? 125 : 100 ) );
				case Stat.Int: return ( from.IntLock == StatLockType.Up && from.RawInt < ( Core.AOS ? 125 : 100 ) );
			}

			return false;
		}

		public static void IncreaseStat( Mobile from, Stat stat, bool atrophy )
		{
			atrophy = atrophy || (from.RawStatTotal >= from.StatCap);

			switch ( stat )
			{
				case Stat.Str:
				{
					if ( atrophy )
					{
						if ( CanLower( from, Stat.Dex ) && (from.RawDex < from.RawInt || !CanLower( from, Stat.Int )) )
							--from.RawDex;
						else if ( CanLower( from, Stat.Int ) )
							--from.RawInt;
					}

					if ( CanRaise( from, Stat.Str ) )
					{
 						++from.RawStr;

                        #region mod by Dies Irae
                        if ( SkillSystem.Config.GuaranteedGainSystemEnabled )
							GuaranteedGainSystem.RegisterStatGain( from );
                        #endregion
                    }

					break;
				}
				case Stat.Dex:
				{
					if ( atrophy )
					{
						if ( CanLower( from, Stat.Str ) && (from.RawStr < from.RawInt || !CanLower( from, Stat.Int )) )
							--from.RawStr;
						else if ( CanLower( from, Stat.Int ) )
							--from.RawInt;
					}

                    if( CanRaise( from, Stat.Dex ) )
                    {
                        ++from.RawDex;

                        #region mod by Dies Irae
                        if( SkillSystem.Config.GuaranteedGainSystemEnabled )
                            GuaranteedGainSystem.RegisterStatGain( from );
                        #endregion
                    }
					break;
				}
				case Stat.Int:
				{
					if ( atrophy )
					{
						if ( CanLower( from, Stat.Str ) && (from.RawStr < from.RawDex || !CanLower( from, Stat.Dex )) )
							--from.RawStr;
						else if ( CanLower( from, Stat.Dex ) )
							--from.RawDex;
					}

                    if( CanRaise( from, Stat.Int ) )
                    {
                        ++from.RawInt;

                        #region mod by Dies Irae
                        if( SkillSystem.Config.GuaranteedGainSystemEnabled )
                            GuaranteedGainSystem.RegisterStatGain( from );
                        #endregion
                    }
					break;
				}
			}
		}

		private static TimeSpan m_StatGainDelay = TimeSpan.FromMinutes( 15.0 );
		private static TimeSpan m_PetStatGainDelay = TimeSpan.FromMinutes( 15.0 );

		public static double GetStatGainDelay( Mobile from, Stat stat )
		{
			int val = 0;
			switch( stat )
			{
				case Stat.Str:
					val = from.Str;
					break;
				case Stat.Dex:
					val = from.Dex;
					break;
				case Stat.Int:
					val = from.Int;
					break;
			}

			if( val <= 50.0 )
				return 15.0;
			else if( val <= 70.0 )
				return 30.0;
			else if( val <= 90.0 )
				return 40.0;
			else
				return 50.0;
		}

		public static void GainStat( Mobile from, Stat stat )
		{
			switch( stat )
			{
				case Stat.Str:
				{
					if ( from is BaseCreature && ((BaseCreature)from).Controlled )
					{
						if ( (from.LastStrGain + m_PetStatGainDelay) >= DateTime.Now )
							return;
					}
					else if( ( from.LastStrGain + TimeSpan.FromMinutes( GetStatGainDelay( from, stat ) ) ) >= DateTime.Now || ( SkillSystem.Config.RateOverTimeEnabled && !RateOverTime.StatGainAllowed( from ) ) ) // mod by Dies Irae
						return;

					from.LastStrGain = DateTime.Now;
					break;
				}
				case Stat.Dex:
				{
					if ( from is BaseCreature && ((BaseCreature)from).Controlled )
					{
						if ( (from.LastDexGain + m_PetStatGainDelay) >= DateTime.Now )
							return;
					}
					else if( ( from.LastDexGain + TimeSpan.FromMinutes( GetStatGainDelay( from, stat ) ) ) >= DateTime.Now || ( SkillSystem.Config.RateOverTimeEnabled && !RateOverTime.StatGainAllowed( from ) ) ) // mod by Dies Irae
						return;

					from.LastDexGain = DateTime.Now;
					break;
				}
				case Stat.Int:
				{
					if ( from is BaseCreature && ((BaseCreature)from).Controlled )
					{
						if ( (from.LastIntGain + m_PetStatGainDelay) >= DateTime.Now )
							return;
					}
					else if( ( from.LastIntGain + TimeSpan.FromMinutes( GetStatGainDelay( from, stat ) ) ) >= DateTime.Now || ( SkillSystem.Config.RateOverTimeEnabled && !RateOverTime.StatGainAllowed( from ) ) ) // mod by Dies Irae
						return;

					from.LastIntGain = DateTime.Now;
					break;
				}
			}

			bool atrophy = Server.Core.AOS && ( (from.RawStatTotal / (double)from.StatCap) >= Utility.RandomDouble() ); // mod by Dies Irae

			IncreaseStat( from, stat, atrophy );
		}
	}
}