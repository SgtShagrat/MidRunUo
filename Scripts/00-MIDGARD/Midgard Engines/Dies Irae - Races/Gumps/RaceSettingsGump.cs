using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.Races
{
    public class RaceSettingsGump : Gump
    {
        public RaceSettingsGump()
            : base( 50, 50 )
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );
            AddBackground( 0, 0, 308, 345, 0x2454 );

            AddOldHtmlHued( 50, 10, 200, 20, "Midgard Race Settings", 100 );
            AddImage( 256, 5, 0x9E1 );

            AddButton( 20, 60, Config.BiteEnabled ? 0x939 : 0x938, Config.BiteEnabled ? 0x939 : 0x938, 1, GumpButtonType.Reply, 0 );
            AddButton( 20, 85, Config.BlessEnabled ? 0x939 : 0x938, Config.BlessEnabled ? 0x939 : 0x938, 2, GumpButtonType.Reply, 0 );
            AddButton( 20, 110, Config.FireWorksEnabled ? 0x939 : 0x938, Config.FireWorksEnabled ? 0x939 : 0x938, 3, GumpButtonType.Reply, 0 );
            AddButton( 20, 135, Config.StatBonusesEnabled ? 0x939 : 0x938, Config.StatBonusesEnabled ? 0x939 : 0x938, 4, GumpButtonType.Reply, 0 );
            AddButton( 20, 160, Config.RaceGainFactorBonusEnabled ? 0x939 : 0x938, Config.RaceGainFactorBonusEnabled ? 0x939 : 0x938, 5, GumpButtonType.Reply, 0 );
            AddButton( 20, 185, Config.RaceMorphEnabled ? 0x939 : 0x938, Config.RaceMorphEnabled ? 0x939 : 0x938, 6, GumpButtonType.Reply, 0 );
            AddButton( 20, 210, Config.RaceResistancesBonusEnabled ? 0x939 : 0x938, Config.RaceResistancesBonusEnabled ? 0x939 : 0x938, 7, GumpButtonType.Reply, 0 );
            AddButton( 20, 235, Config.RaceVisionEnabled ? 0x939 : 0x938, Config.RaceVisionEnabled ? 0x939 : 0x938, 8, GumpButtonType.Reply, 0 );
            AddButton( 20, 260, Config.RacialLanguageEnabled ? 0x939 : 0x938, Config.RacialLanguageEnabled ? 0x939 : 0x938, 9, GumpButtonType.Reply, 0 );
            AddButton( 20, 285, Config.VampireSystemEnabled ? 0x939 : 0x938, Config.VampireSystemEnabled ? 0x939 : 0x938, 10, GumpButtonType.Reply, 0 );
            AddButton( 20, 310, Config.SkillBonusesEnabled ? 0x939 : 0x938, Config.SkillBonusesEnabled ? 0x939 : 0x938, 11, GumpButtonType.Reply, 0 );

            AddLabel( 45, 56, 0x226, "Bite" );
            AddLabel( 45, 81, 0x226, "Bless" );
            AddLabel( 45, 106, 0x226, "Fire Works" );
            AddLabel( 45, 131, 0x226, "Stat Bonuses" );
            AddLabel( 45, 156, 0x226, "Gain Factor Bonus" );
            AddLabel( 45, 181, 0x226, "Morph" );
            AddLabel( 45, 206, 0x226, "Resistance Bonuses" );
            AddLabel( 45, 231, 0x226, "Infravision" );
            AddLabel( 45, 256, 0x226, "Racial Languages" );
            AddLabel( 45, 281, 0x226, "Vampire System" );
            AddLabel( 45, 306, 0x226, "Skill Bonuses" );

            AddOldHtmlHued( 218, 205, 200, 20, "Legend:", 100 );
            AddImage( 218, 235, 0x938 );
            AddLabel( 243, 231, 0x226, "disabled" );
            AddImage( 218, 260, 0x939 );
            AddLabel( 243, 256, 0x226, "enabled" );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            switch( info.ButtonID )
            {
                case 0: Config.SaveSetings(); break;

                case 1: Config.BiteEnabled = !Config.BiteEnabled; break;
                case 2: Config.BlessEnabled = !Config.BlessEnabled; break;
                case 3: Config.FireWorksEnabled = !Config.FireWorksEnabled; break;
                case 4: Config.StatBonusesEnabled = !Config.StatBonusesEnabled; break;
                case 5: Config.RaceGainFactorBonusEnabled = !Config.RaceGainFactorBonusEnabled; break;
                case 6: Config.RaceMorphEnabled = !Config.RaceMorphEnabled; break;
                case 7: Config.RaceResistancesBonusEnabled = !Config.RaceResistancesBonusEnabled; break;
                case 8: Config.RaceVisionEnabled = !Config.RaceVisionEnabled; break;
                case 9: Config.RacialLanguageEnabled = !Config.RacialLanguageEnabled; break;
                case 10: Config.VampireSystemEnabled = !Config.VampireSystemEnabled; break;
                case 11: Config.SkillBonusesEnabled = !Config.SkillBonusesEnabled; break;
            }

            if( info.ButtonID > 0 )
                sender.Mobile.SendGump( new RaceSettingsGump() );
        }
    }
}