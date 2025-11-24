/***************************************************************************
 *                                  TestNewBodSystem.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Diagnostics;
using System.IO;

using Server;
using Server.Commands;
using Server.Engines.BulkOrders;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines
{
    public class TestNewBodSystem
    {
        private static readonly int NumBulks = 1000;

        private enum Prop
        {
            IsArmor,
            IsWeapon,
            IsCloth,

            Is10,
            Is15,
            Is20,

            IsNone,
            IsDullCopper,
            IsShadowIron,
            IsCopper,
            IsBronze,
            IsGold,
            IsAgapite,
            IsVerite,
            IsValorite,
            IsSpined,
            IsHorned,
            IsBarbed,

            IsNormal,
            IsExc
        }

        private static int PropLength = Enum.GetValues( typeof( Prop ) ).Length;

        private static int[] PropCounter = new int[ 20 ];
        private static int m_Scenaio;

        public static void Initialize()
        {
            // CommandSystem.Register( "TestMidgardBulks", AccessLevel.Administrator, new CommandEventHandler( TestMidgardBulks_OnCommand ) );
        }

        private static void DoScenario( Midgard2PlayerMobile m2pm, bool isBlackTest, bool isLargeTest )
        {
            System.Array.Clear( PropCounter, 0, PropCounter.Length );

            SmallBOD smallBOD;
            LargeBOD largeBOD;

            if( isBlackTest )
            {
                for( int i = 0; i < NumBulks; i++ )
                {
                    if( isLargeTest )
                    {
                        largeBOD = Midgard.Engines.Craft.BodHelper.CreateRandomLargeSmithBodFor( m2pm );
                        UpdateTotals( largeBOD );
                        largeBOD.Delete();
                    }
                    else
                    {
                        smallBOD = Midgard.Engines.Craft.BodHelper.CreateRandomSmallBlackFor( m2pm );
                        UpdateTotals( smallBOD );
                        smallBOD.Delete();
                    }
                }
            }
            else
            {
                for( int i = 0; i < NumBulks; i++ )
                {
                    if( isLargeTest )
                    {
                        largeBOD = Midgard.Engines.Craft.BodHelper.CreateRandomLargeTailorBodFor( m2pm );
                        UpdateTotals( largeBOD );
                        largeBOD.Delete();
                    }
                    else
                    {
                        smallBOD = Midgard.Engines.Craft.BodHelper.CreateRandomSmallTailorFor( m2pm );
                        UpdateTotals( smallBOD );
                        smallBOD.Delete();
                    }
                }
            }

            WriteReport( m2pm, isBlackTest, isLargeTest );
            m_Scenaio++;
        }

        private static void UpdateTotals( SmallBOD smallBOD )
        {
            if( smallBOD == null )
                return;

            if( smallBOD.Type.IsSubclassOf( typeof( BaseWeapon ) ) )
                PropCounter[ (int)Prop.IsWeapon ]++;
            else if( smallBOD.Type.IsSubclassOf( typeof( BaseArmor ) ) )
                PropCounter[ (int)Prop.IsArmor ]++;
            else if( smallBOD.Type.IsSubclassOf( typeof( BaseClothing ) ) )
                PropCounter[ (int)Prop.IsCloth ]++;
            else
                Console.WriteLine( "Warning: Type not armor or weapon or clothing" );

            if( smallBOD.AmountMax == 10 )
                PropCounter[ (int)Prop.Is10 ]++;
            else if( smallBOD.AmountMax == 15 )
                PropCounter[ (int)Prop.Is15 ]++;
            else if( smallBOD.AmountMax == 20 )
                PropCounter[ (int)Prop.Is20 ]++;
            else
                Console.WriteLine( "Warning: Type not 10 or 15 or 20" );

            PropCounter[ (int)smallBOD.Material + 6 ]++;

            if( smallBOD.RequireExceptional )
                PropCounter[ (int)Prop.IsExc ]++;
            else
                PropCounter[ (int)Prop.IsNormal ]++;
        }

        private static void UpdateTotals( LargeBOD largeBOD )
        {
            if( largeBOD == null )
                return;

            if( largeBOD.Entries[ 0 ].Details.Type.IsSubclassOf( typeof( BaseWeapon ) ) )
                PropCounter[ (int)Prop.IsWeapon ]++;
            else if( largeBOD.Entries[ 0 ].Details.Type.IsSubclassOf( typeof( BaseArmor ) ) )
                PropCounter[ (int)Prop.IsArmor ]++;
            else if( largeBOD.Entries[ 0 ].Details.Type.IsSubclassOf( typeof( BaseClothing ) ) )
                PropCounter[ (int)Prop.IsCloth ]++;
            else
                Console.WriteLine( "Warning: Type not armor or weapon or clothing" );

            if( largeBOD.AmountMax == 10 )
                PropCounter[ (int)Prop.Is10 ]++;
            else if( largeBOD.AmountMax == 15 )
                PropCounter[ (int)Prop.Is15 ]++;
            else if( largeBOD.AmountMax == 20 )
                PropCounter[ (int)Prop.Is20 ]++;
            else
                Console.WriteLine( "Warning: Type not 10 or 15 or 20" );

            PropCounter[ (int)largeBOD.Material + 6 ]++;

            if( largeBOD.RequireExceptional )
                PropCounter[ (int)Prop.IsExc ]++;
            else
                PropCounter[ (int)Prop.IsNormal ]++;
        }

        private static void WriteReport( Midgard2PlayerMobile m2pm, bool isBlackTest, bool isLargeTest )
        {
            TextWriter tw = File.AppendText( "Logs/TestBulks.log" );

            tw.WriteLine( "######################################" );
            tw.WriteLine( "" );
            tw.WriteLine( "######### {0} TEST {1} #########", isLargeTest ? "LARGE" : "SMALL", isBlackTest ? "BLACKSMITH" : "TAILORING" );
            tw.WriteLine( "" );

            if( isBlackTest )
                tw.WriteLine( "Blacksmith: {0:F2}", m2pm.Skills[ SkillName.Blacksmith ].Base );
            else
                tw.WriteLine( "Tailoring: {0:F2}", m2pm.Skills[ SkillName.Tailoring ].Base );

            tw.WriteLine( "ArmsLore: {0:F2}", m2pm.Skills[ SkillName.ArmsLore ].Base );
            tw.WriteLine( "ItemID: {0:F2}", m2pm.Skills[ SkillName.ItemID ].Base );

            if( isBlackTest )
                tw.WriteLine( "LastSmithBulkOrderValue: {0}", m2pm.LastSmithBulkOrderValue );
            else
                tw.WriteLine( "LastTailorBulkOrderValue: {0}", m2pm.LastTailorBulkOrderValue );

            for( int i = 0; i < PropLength; i++ )
            {
                tw.WriteLine( "{0} - {1} - {2}%", Enum.GetName( typeof( Prop ), i ), PropCounter[ i ], ( ( PropCounter[ i ] / (double)NumBulks ) * 100.0 ).ToString( "F2" ) );
            }

            tw.WriteLine( "" );
            tw.WriteLine( "######################################" );
            tw.WriteLine( "" );

            tw.Close();

            Console.WriteLine( "Scenario {0} completed.", m_Scenaio );
        }

        public static void TestMidgardBulks_OnCommand( CommandEventArgs e )
        {
            m_Scenaio = 1;

            Midgard2PlayerMobile m2pm = new Midgard2PlayerMobile();

            Stopwatch sw = new Stopwatch(); // Timer per indicare la durata dell'operazione
            sw.Start();

            bool isBlackTest = true;
            bool isLargeTest = false;

            if( e.Length == 2 )
            {
                if( e.Arguments[ 1 ] == "large" )
                    isLargeTest = true;
                else
                {
                    e.Mobile.SendMessage( "Command Use: TestMidgardBulks ( black | tailor ) <large>" );
                    return;
                }
            }

            if( e.Length > 0 && e.Length <= 2 && e.Arguments[ 0 ] == "black" )
                isBlackTest = true;
            else if( e.Length > 0 && e.Length <= 2 && e.Arguments[ 0 ] == "tailor" )
                isBlackTest = false;
            else
            {
                e.Mobile.SendMessage( "Command Use: TestMidgardBulks ( black | tailor ) <large>" );
                return;
            }

            #region case 1
            if( isBlackTest )
                m2pm.Skills[ SkillName.Blacksmith ].Base = 0.0;
            else
                m2pm.Skills[ SkillName.Tailoring ].Base = 0.0;

            m2pm.Skills[ SkillName.ArmsLore ].Base = 0.0;
            m2pm.Skills[ SkillName.ItemID ].Base = 0.0;

            DoScenario( m2pm, isBlackTest, isLargeTest );
            #endregion

            #region case 2
            if( isBlackTest )
                m2pm.Skills[ SkillName.Blacksmith ].Base = 100.0;
            else
                m2pm.Skills[ SkillName.Tailoring ].Base = 100.0;

            m2pm.Skills[ SkillName.ArmsLore ].Base = 0.0;
            m2pm.Skills[ SkillName.ItemID ].Base = 0.0;

            DoScenario( m2pm, isBlackTest, isLargeTest );
            #endregion

            #region case 3
            if( isBlackTest )
                m2pm.Skills[ SkillName.Blacksmith ].Base = 100.0;
            else
                m2pm.Skills[ SkillName.Tailoring ].Base = 100.0;

            m2pm.Skills[ SkillName.ArmsLore ].Base = 100.0;
            m2pm.Skills[ SkillName.ItemID ].Base = 0.0;

            DoScenario( m2pm, isBlackTest, isLargeTest );
            #endregion

            #region case 4
            if( isBlackTest )
                m2pm.Skills[ SkillName.Blacksmith ].Base = 100.0;
            else
                m2pm.Skills[ SkillName.Tailoring ].Base = 100.0;

            m2pm.Skills[ SkillName.ArmsLore ].Base = 100.0;
            m2pm.Skills[ SkillName.ItemID ].Base = 100.0;

            DoScenario( m2pm, isBlackTest, isLargeTest );
            #endregion

            #region case 5
            if( isBlackTest )
                m2pm.Skills[ SkillName.Blacksmith ].Base = 120.0;
            else
                m2pm.Skills[ SkillName.Tailoring ].Base = 120.0;

            m2pm.Skills[ SkillName.ArmsLore ].Base = 100.0;
            m2pm.Skills[ SkillName.ItemID ].Base = 100.0;

            DoScenario( m2pm, isBlackTest, isLargeTest );
            #endregion

            #region case 5
            if( isBlackTest )
                m2pm.Skills[ SkillName.Blacksmith ].Base = 120.0;
            else
                m2pm.Skills[ SkillName.Tailoring ].Base = 120.0;

            m2pm.Skills[ SkillName.ArmsLore ].Base = 100.0;
            m2pm.Skills[ SkillName.ItemID ].Base = 100.0;

            if( isBlackTest )
                m2pm.LastSmithBulkOrderValue = 1000;
            else
                m2pm.LastTailorBulkOrderValue = 1000;

            DoScenario( m2pm, isBlackTest, isLargeTest );
            #endregion

            #region case 6
            if( isBlackTest )
                m2pm.Skills[ SkillName.Blacksmith ].Base = 120.0;
            else
                m2pm.Skills[ SkillName.Tailoring ].Base = 120.0;

            m2pm.Skills[ SkillName.ArmsLore ].Base = 100.0;
            m2pm.Skills[ SkillName.ItemID ].Base = 100.0;

            if( isBlackTest )
                m2pm.LastSmithBulkOrderValue = 0;
            else
                m2pm.LastTailorBulkOrderValue = 0;

            DoScenario( m2pm, isBlackTest, isLargeTest );
            #endregion

            Console.WriteLine( "Bod system tested in {0:F2} seconds.", ( sw.ElapsedMilliseconds / 1000 ) );
            sw.Stop();
        }
    }
}