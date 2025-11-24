using Midgard.Engines.MidgardTownSystem;

using Server;

namespace Midgard.Engines.Academies
{
    public sealed class SerpentsHoldAcademy : AcademySystem
    {
        #region [Trainings]
        private static readonly Disciplines[] m_Trainings = new Disciplines[] { Disciplines.ArtOfMining };

        public override Disciplines[] Trainings { get { return m_Trainings; } }
        #endregion

        public SerpentsHoldAcademy()
        {
            Definition = new AcademyDefinition( "Serpent's Hold Academy",
                                                DefaultWelcomeMessage
            );

            AddClass( Disciplines.ArtOfMining, "the art of mining" );
        }

        public override void SetStartingSkills( Mobile mobile )
        {
        }

        public override bool IsEligible( Mobile mob )
        {
            if( mob.Race == Races.Core.Drow )
                return false;
            else if( mob.Race == Races.Core.Vampire )
                return false;
            else if( mob.Race == Races.Core.Undead )
                return false;
            else if( mob.Race == Races.Core.HighOrc )
                return false;
            else if( mob.Race == Races.Core.HalfDaemon )
                return false;
            else if( mob.Race == Races.Core.FairyOfAir )
                return false;
            else if( mob.Race == Races.Core.FairyOfEarth )
                return false;
            else if( mob.Race == Races.Core.FairyOfFire )
                return false;
            else if( mob.Race == Races.Core.FairyOfWater )
                return false;
            else if( mob.Race == Races.Core.FairyOfWood )
                return false;
            else if( mob.Race == Races.Core.Sprite )
                return false;

            TownSystem townSystem = TownSystem.Find( mob );
            if( townSystem != null && ( townSystem.IsEnemyTo( TownSystem.SerpentsHold ) || townSystem.IsKnownCriminalForTown( mob ) ) )
                return false;

            return base.IsEligible( mob );
        }

        public override bool IsAllowedSkill( SkillName skillName )
        {
            if( skillName == SkillName.Necromancy )
                return false;

            return true;
        }
    }
}