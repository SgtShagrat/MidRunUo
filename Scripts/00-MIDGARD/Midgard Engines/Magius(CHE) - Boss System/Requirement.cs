using System;
using Server;
using Server.Mobiles;
namespace Midgard.Engines.BossSystem
{
	internal abstract class DungeonJoinRequirement
	{
		protected DungeonJoinRequirement ()
		{
		}
		public abstract bool SatisfiedBy( Dungeon dungeon, Mobile Mobile );
	}
	internal class JRPlayerSkillAttackMinimum : DungeonJoinRequirement
	{
		public readonly int SkillValue;
		public static SkillName[] PrimaryCombatSkills = new SkillName[]{
			SkillName.AnimalTaming,
			SkillName.Swords,
			SkillName.Macing,
			SkillName.Fencing,
			SkillName.Wrestling,
			SkillName.Archery
		};
		public JRPlayerSkillAttackMinimum (int basefixedkillvalue)
		{
			SkillValue=basefixedkillvalue;
		}
		public override bool SatisfiedBy (Dungeon dungeon, Mobile Mobile)
		{
			foreach(var sk in PrimaryCombatSkills)
			{
				if (Mobile.Skills[sk].BaseFixedPoint>=SkillValue)
					return true;
			}
			return false;
		}
	}
	internal class JRPlayerNewbie : DungeonJoinRequirement
	{
		public readonly bool MustBeNewBie;
		public JRPlayerNewbie (bool mustBeNewBie)
		{
			MustBeNewBie = mustBeNewBie;
		}
		public override bool SatisfiedBy (Dungeon dungeon, Mobile Mobile)
		{			
			var player = Mobile as PlayerMobile;
			if (player == null)
				return false;
			
			return (player.Young == MustBeNewBie);
		}
	}
	
}

