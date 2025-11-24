#region AuthorHeader
//   Bertoldo
//	 29/07/2006
//
//  
//
#endregion AuthorHeader

using Server;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
    public class SpecialCraftGump : Gump
    {
        public static int nbonus1 = 0;
        public static int nbonus2 = 0;
        public static double chance = 0;
        public bool bonusattivo = false;
        private CraftSystem craftsystem = null;

        public SpecialCraftGump( Mobile from, CraftSystem system, bool precisione, int bonus1, int bonus2 )
            : base( 0, 0 )
        {
            bonusattivo = precisione;
            craftsystem = system;
            int offset = 500;
            nbonus1 = bonus1;
            nbonus2 = bonus2;
            chance = 0;

            bool isBonusDiPrecisione = from.Skills[ SkillName.ItemID ].Value >= 100 && from.Skills[ SkillName.ArmsLore ].Value >= 100;
            bool isArmsL = from.Skills[ SkillName.ArmsLore ].Value >= 100;
            bool isItemID = from.Skills[ SkillName.ItemID ].Value >= 100;

            chance += from.Skills[ SkillName.ArmsLore ].Value / 10;
            chance += from.Skills[ SkillName.ItemID ].Value / 10;
            if( isBonusDiPrecisione ) chance += 20;

            if( system.MainSkill == SkillName.Blacksmith )
                chance += from.Skills[ SkillName.Blacksmith ].Value - 100;
            else if( system.MainSkill == SkillName.Tailoring )
                chance += from.Skills[ SkillName.Tailoring ].Value - 100;
            else return;

            if( chance < 0 )
                chance = 0;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            AddPage( 0 );
            AddBackground( 79 + offset, 85, 190, 320, 5120 );//9559
            AddBackground( 90 + offset, 220, 170, 180, 9200 );
            //			AddAlphaRegion(84+offset, 140, 180, 250);

            AddLabel( 110 + offset, 95, 82, "Midgard Craft System" );
            AddLabel( 120 + offset, 115, 82, "** " + system.MainSkill + " **" );
            AddLabel( 111 + offset, 140, 87, "Bonus Speciali Attivi:" );

            AddLabel( 125 + offset, 160, 42, "Bonus Skill ArmsLore" );
            AddImage( 100 + offset, 162, ( ( from.Skills[ SkillName.ArmsLore ].Value >= 100 ) ? 11400 : 11410 ) );
            AddLabel( 125 + offset, 180, 42, "Bonus Skill ItemID" );
            AddImage( 100 + offset, 182, ( ( from.Skills[ SkillName.ItemID ].Value >= 100 ) ? 11400 : 11410 ) );
            AddLabel( 125 + offset, 200, 42, "Bonus di Precisione" );
            AddImage( 100 + offset, 202, ( ( bonusattivo ) ? 11400 : 11410 ) );

            AddLabel( 115 + offset, 225, 87, "Bonus di Precisione" );
            AddGroup( 1 );
            AddRadio( 110 + offset, 250, 1896, 1895, ( nbonus1 == 1 ), 1 );
            AddLabel( 135 + offset, 248, 0, "STR" );
            AddRadio( 110 + offset, 270, 1896, 1895, ( nbonus1 == 2 ), 2 );
            AddLabel( 135 + offset, 268, 0, "DEX" );
            AddRadio( 110 + offset, 290, 1896, 1895, ( nbonus1 == 3 ), 3 );
            AddLabel( 135 + offset, 288, 0, "INT" );
            AddGroup( 2 );
            AddRadio( 170 + offset, 250, 1896, 1895, ( nbonus2 == 1 ), 4 );
            AddLabel( 195 + offset, 248, 0, "HITS" );
            AddRadio( 170 + offset, 270, 1896, 1895, ( nbonus2 == 2 ), 5 );
            AddLabel( 195 + offset, 268, 0, "STAM" );
            AddRadio( 170 + offset, 290, 1896, 1895, ( nbonus2 == 3 ), 6 );
            AddLabel( 195 + offset, 288, 0, "MANA" );

            AddButton( 145 + offset, 310, 2124, 2123, 20, GumpButtonType.Reply, 0 );

            AddLabel( 110 + offset, 330, 73, string.Format( "La tua chance : {0}%", chance ) );
            string str1 = "";
            string str2 = "";
            switch( nbonus1 )
            {
                case 0:
                    str1 = "---";
                    break;
                case 1:
                    str1 = "STR";
                    break;
                case 2:
                    str1 = "DEX";
                    break;
                case 3:
                    str1 = "INT";
                    break;
            }
            switch( nbonus2 )
            {
                case 0:
                    str2 = "---";
                    break;
                case 1:
                    str2 = "HITS";
                    break;
                case 2:
                    str2 = "STAM";
                    break;
                case 3:
                    str2 = "MANA";
                    break;
            }

            AddLabel( 110 + offset, 370, 73, string.Format( "Selezionati : {0}+{1}", str1, str2 ) );
            AddLabel( 110 + offset, 350, 73, bonusattivo ? "Bonus : attivato" : "Bonus : non attivo" );
        }
        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;
            if( info.ButtonID == 20 )
            {
                bonusattivo = true;
                from.SendMessage( "Hai attivato il Bonus di Precisione" );
                if( info.IsSwitched( 1 ) ) nbonus1 = 1;
                if( info.IsSwitched( 2 ) ) nbonus1 = 2;
                if( info.IsSwitched( 3 ) ) nbonus1 = 3;
                if( info.IsSwitched( 4 ) ) nbonus2 = 1;
                if( info.IsSwitched( 5 ) ) nbonus2 = 2;
                if( info.IsSwitched( 6 ) ) nbonus2 = 3;

                //				from.CloseGump( typeof( SpecialCraftGump ) );//bertoldo
                from.SendGump( new SpecialCraftGump( from, craftsystem, bonusattivo, nbonus1, nbonus2 ) );//bertoldo
            }
        }
    }
}