using System;
using System.Collections.Generic;
using Midgard.Mobiles;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

/*
1060886 - Your endurance shall protect you from your enemies blows.			+1 to +10 Phys
1060887 - A smile will be upon your lips, as you gaze into the infernos.	+1 to +10 Fire
1060888 - The ice of ages will embrace you, and you will embrace it alike.	+1 to +10 Cold
1060889 - Your blood runs pure and strong.									+1 to +10 Poison
1060890 - Your flesh shall endure the power of storms.						+1 to +10 Energy
1060891 - Seek riches and they will seek you.								+10 to +50 Luck
1060892 - The power of alchemy shall thrive within you.						+5 to +25 Enhance Potions
1060893 - Fate smiles upon you this day.									+10 to +100 Luck
1060894 - A keen mind in battle will help you avoid injury.					+1 to +10 Defense
1060895 - The flow of the ether is strong within you.						+1 to +3 Mana regan

1060901 - Your wounds in battle shall run deep.								-1 to -10 Phys
1060902 - The fires of the abyss shall tear asunder your flesh!				-1 to -10 Fire
1060903 - Winter’s touch shall be your undoing.								-1 to -10 Cold
1060904 - Your veins will freeze with poison’s chill.						-1 to -10 Poison
1060905 - The wise will seek to avoid the anger of storms.					-1 to -10 Energy
1060906 - Your dreams of wealth shall vanish like smoke.					-10 to -50 Luck
1060907 - The strength of alchemy will fail you.							-5 to -25 Enhance Potions
1060908 - Only fools take risks in fate’s shadow.							-50 to -100 Luck
1060909 - Your lack of focus in battle shall be your undoing.				-1 to -10 Defense
1060910 - Your connection with the ether is weak, take heed.				-1 to -3 Mana Regen
*/

namespace Midgard.Gumps
{
    public enum FortuneTypes
    {
        None,

        BonusResPhysical,
        BonusResFire,
        BonusResCold,
        BonusResPoison,
        BonusResEnergy,
        BonusLuckMinor,
        BonusPotions,
        BonusLuckMajor,
        BonusDefence,
        BonusRegenMana,
    }

    public class FortuneGump : Gump
    {
        private Sphynx m_Sphynx;

        public FortuneGump( Sphynx sphynx )
            : base( 150, 50 )
        {
            m_Sphynx = sphynx;

            Closable = false;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );

            AddImage( 0, 0, 3600 );
            AddImageTiled( 0, 14, 15, 200, 3603 );
            AddImageTiled( 380, 14, 14, 200, 3605 );

            AddImage( 0, 201, 3606 );
            AddImageTiled( 15, 201, 370, 16, 3607 );
            AddImageTiled( 15, 0, 370, 16, 3601 );

            AddImage( 380, 0, 3602 );
            AddImage( 380, 201, 3608 );
            AddImageTiled( 15, 15, 365, 190, 2624 );
            AddRadio( 30, 140, 9727, 9730, false, 1 );
            AddHtmlLocalized( 65, 145, 300, 25, 1060863, 32767, false, false ); // Pay for the reading.
            AddRadio( 30, 175, 9727, 9730, true, 2 );
            AddHtmlLocalized( 65, 178, 300, 25, 1060862, 32767, false, false ); // No thanks. I decide my own destiny!
            AddHtmlLocalized( 30, 20, 360, 35, 1060864, 32767, false, false ); // Interested in your fortune, are you?  The ancient Sphynx can read the future for you - for a price of course...
            AddImage( 65, 72, 5605 );
            AddImageTiled( 80, 90, 200, 1, 9107 );
            AddImageTiled( 95, 92, 200, 1, 9157 );
            AddLabel( 90, 70, 140, "5000" );
            AddHtmlLocalized( 140, 70, 100, 25, 1023823, 32767, false, false ); // gold coins
            AddButton( 290, 175, 247, 248, 1, GumpButtonType.Reply, 0 );
            AddImageTiled( 15, 14, 365, 1, 9107 );
            AddImageTiled( 380, 14, 1, 190, 9105 );
            AddImageTiled( 15, 205, 365, 1, 9107 );
            AddImageTiled( 15, 14, 1, 190, 9105 );
            AddImageTiled( 0, 0, 395, 1, 9157 );
            AddImageTiled( 394, 0, 1, 217, 9155 );
            AddImageTiled( 0, 216, 395, 1, 9157 );
            AddImageTiled( 0, 0, 1, 217, 9155 );
            AddHtmlLocalized( 30, 105, 340, 40, 1060865, 0xB5CE6B, false, false ); // Do you accept this offer?  The funds will be withdrawn from your backpack.
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( m_Sphynx == null || m_Sphynx.Deleted )
                return;

            Midgard2PlayerMobile m2pm = sender.Mobile as Midgard2PlayerMobile;

            if( m2pm == null )
                return;

            if( m2pm.SphynxBonusExpiration > TimeSpan.Zero )
                return;

            if( info.ButtonID == 1 && info.IsSwitched( 1 ) )
            {
                Container pack = m2pm.Backpack;

                if( pack != null && pack.ConsumeTotal( typeof( Gold ), 5000 ) )
                {
                    m2pm.SendLocalizedMessage( 1060867 ); // You pay the fee.
                    TellFortune( m2pm );
                    m2pm.SphynxBonusExpiration = TimeSpan.FromDays( 2.0 );
                }
                else
                    m2pm.SendLocalizedMessage( 1061006 ); // You haven't got the coin to make the proper donation to the Sphynx.  Your fortune has not been read.
            }
            else
                m2pm.SendLocalizedMessage( 1061007 ); // You decide against having your fortune told.
        }

        public static void TellFortune( Midgard2PlayerMobile m2pm )
        {
            List<int> temp = new List<int>();
            foreach( int i in Enum.GetValues( typeof( FortuneTypes ) ) )
            {
                if( i > 0 )
                    temp.Add( i );
            }

            int fortune = Utility.RandomList( temp.ToArray() );
            bool isMisfortune = Utility.RandomBool();
            int intensity = GetIntensity( (FortuneTypes)fortune ) * ( isMisfortune ? -1 : 1 );

            // AnnounceFortune
            int offSet = isMisfortune ? 15 : 0;
            m2pm.SendLocalizedMessage( 1060885 + offSet + fortune );

            m2pm.FortuneType = (FortuneTypes)fortune;
            m2pm.FortuneValue = intensity;
        }

        private static int GetIntensity( FortuneTypes fortune )
        {
            switch( fortune )
            {
                case FortuneTypes.BonusResPhysical: return Utility.RandomMinMax( 1, 10 );
                case FortuneTypes.BonusResFire: return Utility.RandomMinMax( 1, 10 );
                case FortuneTypes.BonusResCold: return Utility.RandomMinMax( 1, 10 );
                case FortuneTypes.BonusResPoison: return Utility.RandomMinMax( 1, 10 );
                case FortuneTypes.BonusResEnergy: return Utility.RandomMinMax( 1, 10 );
                case FortuneTypes.BonusLuckMinor: return Utility.RandomMinMax( 1, 10 );
                case FortuneTypes.BonusPotions: return Utility.RandomMinMax( 1, 5 ) * 5;
                case FortuneTypes.BonusLuckMajor: return Utility.RandomMinMax( 50, 100 );
                case FortuneTypes.BonusDefence: return Utility.RandomMinMax( 1, 10 );
                case FortuneTypes.BonusRegenMana: return Utility.RandomMinMax( 1, 3 );
                default: return 0;
            }
        }

        public static int ComputeAoSFortuneBonus( Midgard2PlayerMobile m2pm, AosAttribute a )
        {
            if( m2pm.SphynxBonusExpiration == TimeSpan.Zero )
                return 0;

            if( ( a == AosAttribute.Luck && IsFortuneBonus( m2pm.FortuneType ) ) ||
                ( a == AosAttribute.DefendChance && m2pm.FortuneType == FortuneTypes.BonusDefence ) ||
                ( a == AosAttribute.EnhancePotions && m2pm.FortuneType == FortuneTypes.BonusPotions ) ||
                ( a == AosAttribute.RegenMana && m2pm.FortuneType == FortuneTypes.BonusRegenMana ) )
                return m2pm.FortuneValue;
            else
                return 0;
        }

        public static int ComputeElementFortuneBonus( Midgard2PlayerMobile m2pm, AosElementAttribute res )
        {
            if( m2pm.SphynxBonusExpiration == TimeSpan.Zero )
                return 0;

            if( ( res == AosElementAttribute.Physical && m2pm.FortuneType == FortuneTypes.BonusResPhysical ) ||
                ( res == AosElementAttribute.Fire && m2pm.FortuneType == FortuneTypes.BonusResFire ) ||
                ( res == AosElementAttribute.Cold && m2pm.FortuneType == FortuneTypes.BonusResCold ) ||
                ( res == AosElementAttribute.Poison && m2pm.FortuneType == FortuneTypes.BonusResPoison ) ||
                ( res == AosElementAttribute.Energy && m2pm.FortuneType == FortuneTypes.BonusResEnergy ) )
                return m2pm.FortuneValue;
            else
                return 0;
        }

        private static bool IsFortuneBonus( FortuneTypes type )
        {
            return ( type == FortuneTypes.BonusLuckMajor || type == FortuneTypes.BonusLuckMinor );
        }
    }
}
