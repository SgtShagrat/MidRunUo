/***************************************************************************
 *                               POLAccounting.cs
 *
 *   begin                : 02 agosto, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using Server.Accounting;
using Server.Misc;
using Server.Mobiles;
using Server.Commands;
using Server.Items;
using Server.Network;
using Server;

namespace Midgard.Commands
{
    public class POLAccounting
    {
        private static bool m_Enabled = false;

        private static string m_AccountPath = "accounts.txt";
        private static string m_PlayersPath = "pcs.txt";

        private static int[] m_RawPointsCurve;
        private static int[] m_RawPointsStatCurve;

        private static StreamWriter m_Writer;

        public static void Initialize()
        {
            if( m_Enabled )
            {
                InitLogger();

                InitRawCurve();

                InitStatCurve();

                CommandSystem.Register( "ImportPolAccounts", AccessLevel.Administrator, new CommandEventHandler( PolAccountingOnCommand ) );
            }
        }

        private static void InitRawCurve()
        {
            List<int> list = new List<int>();
            for( int i = 0; i < 1000; i++ )
            {
                list.Add( Engines.SkillSystem.Core.BaseSkillToRawSkill( i ) );
            }

            m_RawPointsCurve = list.ToArray();

            using( TextWriter tw = File.CreateText( "rawPointsSkills.cfg" ) )
            {
                for( int i = 0; i < m_RawPointsCurve.Length; i++ )
                {
                    tw.WriteLine( "{0}\t{1}", i, m_RawPointsCurve[ i ] );
                }
            }
        }

        private static void InitStatCurve()
        {
            List<int> list = new List<int>();
            for( int i = 0; i < 100; i++ )
            {
                list.Add( Engines.SkillSystem.Core.BaseSkillToRawSkill( ( i * 10 ) + 1 ) );
            }

            m_RawPointsStatCurve = list.ToArray();

            using( TextWriter tw = File.CreateText( "rawPointsStats.cfg" ) )
            {
                for( int i = 0; i < m_RawPointsStatCurve.Length; i++ )
                {
                    tw.WriteLine( "{0}\t{1}", i, m_RawPointsStatCurve[ i ] );
                }
            }
        }

        /*
        Character
        {
	        Account	ragathor
	        Name	Ragathor Seldarine
	        Color	0x3ea
	        CProp	Fame i1328
	        CProp	Karma i-6895
	        CProp	PGDexterity i463863
	        CProp	PGIntelligence i16412
	        CProp	PGStrength i949152
	        CProp	longmurders i0
	        CProp	shortmurders i0
	        TrueColor	0x3ea
	        TrueObjtype	0x190
	        Gender	0
	        STR	948838
	        INT	16384
	        DEX	448204
	        CreatedAt	15366768
	        Alchemy	6148
        }
         */

        [Usage( "ImportPolAccounts" )]
        [Description( "This command import all account from a pol file." )]
        private static void PolAccountingOnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            int accountCount = 0;
            int characterCount = 0;
            string text, text2;
            string[] textArray, text2Array;
            FileStream fsTime, fs2Time;
            Midgard2PlayerMobile newChar;
            Account newCharAcct;

            try
            {
                // Open the file and start to read 
                fsTime = new FileStream( m_AccountPath, FileMode.Open );
                fs2Time = new FileStream( m_PlayersPath, FileMode.Open );
            }
            catch
            {
                Log( from, "Couldn't find one of the required files. " );
                return;
            }

            // READING ACCOUNTS: 
            StreamReader srTime = new StreamReader( fsTime );
            Log( from, "Importing accounts ..." );

            // Scan through the file and create the accounts 
            while( srTime.Peek() > -1 )
            {
                text = srTime.ReadLine().Trim();
                string tmpAccountName = "";
                string tmpAccountPassword = "";
                string tmpEmailAdress = "";
                string tmpAccountBanned = "0";
                if( text == "{" )
                {
                    // Read the account data that we need 
                    while( text != "}" )
                    {
                        text = srTime.ReadLine().Trim();
                        textArray = text.Split( '\t' );
                        switch( textArray[ 0 ] )
                        {
                            case "Name":
                                tmpAccountName = textArray[ 1 ];
                                break;
                            case "Password":
                                tmpAccountPassword = textArray[ 1 ];
                                break;
                            case "Email s":
                                tmpEmailAdress = textArray[ 1 ];
                                break;
                            case "Banned":
                                tmpAccountBanned = textArray[ 1 ];
                                break;
                            default:
                                break;
                        }
                    }

                    // create the account ... 
                    if( tmpAccountBanned != "2" )
                    {
                        Account acct = Accounts.GetAccount( tmpAccountName ) as Account;

                        if( acct == null )
                        {
                            CreateAccount( tmpAccountName, tmpAccountPassword, tmpEmailAdress );
                            accountCount++;
                        }
                        else
                            Log( from, string.Format( "Account {0} already exists and it is skipped", acct.Username ) );
                    }
                }
            }

            // Stop reading and close the file. 
            srTime.Close();
            fsTime.Close();

            Log( from, string.Format( "Done importing {0} accounts.", accountCount ) );

            // READING CHARACTERS:          
            Log( from, "Importing characters ..." );

            StreamReader sr2Time = new StreamReader( fs2Time );

            // Scan through the file and create the characters 
            while( sr2Time.Peek() > -1 )
            {
                text2 = sr2Time.ReadLine().Trim();

                string tmpCharName = "";
                string tmpCharAccountString = "";
                string tmpCharCmdlevel = "0";
                string tmpCharGender = "0"; // defaults as male 
                string tmpCharHue = "0x83ea"; // some arb default :) 
                string tmpCharStr = "30";
                string tmpCharDex = "30";
                string tmpCharInt = "30";
                double tmpRawStat = 0;
                int tmpKarma = 0;
                int tmpFame = 0;

                double[] tSkills = new double[ 52 ];

                if( text2 == "{" )
                {
                    // 1. Read the character data that we need 
                    while( text2 != "}" )
                    {
                        text2 = sr2Time.ReadLine().Trim();
                        text2Array = text2.Split( '\t' );
                        switch( text2Array[ 0 ] )
                        {
                            case "Name":
                                tmpCharName = text2Array[ 1 ];
                                Log( from, string.Format( "character={0}", tmpCharName ) );
                                break;
                            case "Account":
                                tmpCharAccountString = text2Array[ 1 ];
                                Log( from, string.Format( "account={0}", tmpCharAccountString ) );
                                break;
                            case "CmdLevel":
                                tmpCharCmdlevel = text2Array[ 1 ];
                                break;
                            case "Gender":
                                tmpCharGender = text2Array[ 1 ];
                                break;
                            case "Color":
                                tmpCharHue = text2Array[ 1 ];
                                break;
                            case "CProp":
                                string[] cPropString = text2Array[ 1 ].Split( ' ' );
                                string key = cPropString[ 0 ];
                                string value = cPropString[ 1 ];
                                switch( key )
                                {
                                    case "Karma":
                                        key = key.Substring( 1 );
                                        tmpKarma = int.Parse( key );
                                        break;
                                    case "Fame":
                                        key = key.Substring( 1 );
                                        tmpFame = int.Parse( key );
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            #region STATS
                            case "STR":
                                tmpRawStat = Convert.ToDouble( text2Array[ 1 ] );
                                //Base skill 0 = 0 raw skill points
                                if( tmpRawStat < 204 ) { text2Array[ 1 ] = "0"; }
                                else if( tmpRawStat < 409 ) { text2Array[ 1 ] = "1"; }
                                else if( tmpRawStat < 614 ) { text2Array[ 1 ] = "2"; }
                                else if( tmpRawStat < 819 ) { text2Array[ 1 ] = "3"; }
                                else if( tmpRawStat < 1024 ) { text2Array[ 1 ] = "4"; }
                                else if( tmpRawStat < 1228 ) { text2Array[ 1 ] = "5"; }
                                else if( tmpRawStat < 1433 ) { text2Array[ 1 ] = "6"; }
                                else if( tmpRawStat < 1638 ) { text2Array[ 1 ] = "7"; }
                                else if( tmpRawStat < 1843 ) { text2Array[ 1 ] = "8"; }
                                else if( tmpRawStat < 2048 ) { text2Array[ 1 ] = "9"; }
                                else if( tmpRawStat < 2252 ) { text2Array[ 1 ] = "10"; }
                                else if( tmpRawStat < 2457 ) { text2Array[ 1 ] = "11"; }
                                else if( tmpRawStat < 2662 ) { text2Array[ 1 ] = "12"; }
                                else if( tmpRawStat < 2867 ) { text2Array[ 1 ] = "13"; }
                                else if( tmpRawStat < 3072 ) { text2Array[ 1 ] = "14"; }
                                else if( tmpRawStat < 3276 ) { text2Array[ 1 ] = "15"; }
                                else if( tmpRawStat < 3481 ) { text2Array[ 1 ] = "16"; }
                                else if( tmpRawStat < 3686 ) { text2Array[ 1 ] = "17"; }
                                else if( tmpRawStat < 3891 ) { text2Array[ 1 ] = "18"; }
                                else if( tmpRawStat < 4096 ) { text2Array[ 1 ] = "19"; }
                                else if( tmpRawStat < 4505 ) { text2Array[ 1 ] = "20"; }
                                else if( tmpRawStat < 4915 ) { text2Array[ 1 ] = "21"; }
                                else if( tmpRawStat < 5324 ) { text2Array[ 1 ] = "22"; }
                                else if( tmpRawStat < 5734 ) { text2Array[ 1 ] = "23"; }
                                else if( tmpRawStat < 6144 ) { text2Array[ 1 ] = "24"; }
                                else if( tmpRawStat < 6553 ) { text2Array[ 1 ] = "25"; }
                                else if( tmpRawStat < 6963 ) { text2Array[ 1 ] = "26"; }
                                else if( tmpRawStat < 7372 ) { text2Array[ 1 ] = "27"; }
                                else if( tmpRawStat < 7782 ) { text2Array[ 1 ] = "28"; }
                                else if( tmpRawStat < 8192 ) { text2Array[ 1 ] = "29"; }
                                else if( tmpRawStat < 9011 ) { text2Array[ 1 ] = "30"; }
                                else if( tmpRawStat < 9830 ) { text2Array[ 1 ] = "31"; }
                                else if( tmpRawStat < 10649 ) { text2Array[ 1 ] = "32"; }
                                else if( tmpRawStat < 11468 ) { text2Array[ 1 ] = "33"; }
                                else if( tmpRawStat < 12288 ) { text2Array[ 1 ] = "34"; }
                                else if( tmpRawStat < 13107 ) { text2Array[ 1 ] = "35"; }
                                else if( tmpRawStat < 13926 ) { text2Array[ 1 ] = "36"; }
                                else if( tmpRawStat < 14745 ) { text2Array[ 1 ] = "37"; }
                                else if( tmpRawStat < 15564 ) { text2Array[ 1 ] = "38"; }
                                else if( tmpRawStat < 16384 ) { text2Array[ 1 ] = "39"; }
                                else if( tmpRawStat < 18022 ) { text2Array[ 1 ] = "40"; }
                                else if( tmpRawStat < 19660 ) { text2Array[ 1 ] = "41"; }
                                else if( tmpRawStat < 21299 ) { text2Array[ 1 ] = "42"; }
                                else if( tmpRawStat < 22937 ) { text2Array[ 1 ] = "43"; }
                                else if( tmpRawStat < 24576 ) { text2Array[ 1 ] = "44"; }
                                else if( tmpRawStat < 26214 ) { text2Array[ 1 ] = "45"; }
                                else if( tmpRawStat < 27852 ) { text2Array[ 1 ] = "46"; }
                                else if( tmpRawStat < 29491 ) { text2Array[ 1 ] = "47"; }
                                else if( tmpRawStat < 31129 ) { text2Array[ 1 ] = "48"; }
                                else if( tmpRawStat < 32768 ) { text2Array[ 1 ] = "49"; }
                                else if( tmpRawStat < 36044 ) { text2Array[ 1 ] = "50"; }
                                else if( tmpRawStat < 39321 ) { text2Array[ 1 ] = "51"; }
                                else if( tmpRawStat < 42598 ) { text2Array[ 1 ] = "52"; }
                                else if( tmpRawStat < 45875 ) { text2Array[ 1 ] = "53"; }
                                else if( tmpRawStat < 49152 ) { text2Array[ 1 ] = "54"; }
                                else if( tmpRawStat < 52428 ) { text2Array[ 1 ] = "55"; }
                                else if( tmpRawStat < 55705 ) { text2Array[ 1 ] = "56"; }
                                else if( tmpRawStat < 58982 ) { text2Array[ 1 ] = "57"; }
                                else if( tmpRawStat < 62259 ) { text2Array[ 1 ] = "58"; }
                                else if( tmpRawStat < 65536 ) { text2Array[ 1 ] = "59"; }
                                else if( tmpRawStat < 72089 ) { text2Array[ 1 ] = "60"; }
                                else if( tmpRawStat < 78643 ) { text2Array[ 1 ] = "61"; }
                                else if( tmpRawStat < 85196 ) { text2Array[ 1 ] = "62"; }
                                else if( tmpRawStat < 91750 ) { text2Array[ 1 ] = "63"; }
                                else if( tmpRawStat < 98304 ) { text2Array[ 1 ] = "64"; }
                                else if( tmpRawStat < 104857 ) { text2Array[ 1 ] = "65"; }
                                else if( tmpRawStat < 111411 ) { text2Array[ 1 ] = "66"; }
                                else if( tmpRawStat < 117964 ) { text2Array[ 1 ] = "67"; }
                                else if( tmpRawStat < 124518 ) { text2Array[ 1 ] = "68"; }
                                else if( tmpRawStat < 131072 ) { text2Array[ 1 ] = "69"; }
                                else if( tmpRawStat < 144179 ) { text2Array[ 1 ] = "70"; }
                                else if( tmpRawStat < 157286 ) { text2Array[ 1 ] = "71"; }
                                else if( tmpRawStat < 170393 ) { text2Array[ 1 ] = "72"; }
                                else if( tmpRawStat < 183500 ) { text2Array[ 1 ] = "73"; }
                                else if( tmpRawStat < 196608 ) { text2Array[ 1 ] = "74"; }
                                else if( tmpRawStat < 209715 ) { text2Array[ 1 ] = "75"; }
                                else if( tmpRawStat < 222822 ) { text2Array[ 1 ] = "76"; }
                                else if( tmpRawStat < 235929 ) { text2Array[ 1 ] = "77"; }
                                else if( tmpRawStat < 249036 ) { text2Array[ 1 ] = "78"; }
                                else if( tmpRawStat < 262144 ) { text2Array[ 1 ] = "79"; }
                                else if( tmpRawStat < 288358 ) { text2Array[ 1 ] = "80"; }
                                else if( tmpRawStat < 314572 ) { text2Array[ 1 ] = "81"; }
                                else if( tmpRawStat < 340787 ) { text2Array[ 1 ] = "82"; }
                                else if( tmpRawStat < 367001 ) { text2Array[ 1 ] = "83"; }
                                else if( tmpRawStat < 393216 ) { text2Array[ 1 ] = "84"; }
                                else if( tmpRawStat < 419430 ) { text2Array[ 1 ] = "85"; }
                                else if( tmpRawStat < 445644 ) { text2Array[ 1 ] = "86"; }
                                else if( tmpRawStat < 471859 ) { text2Array[ 1 ] = "87"; }
                                else if( tmpRawStat < 498073 ) { text2Array[ 1 ] = "88"; }
                                else if( tmpRawStat < 524288 ) { text2Array[ 1 ] = "89"; }
                                else if( tmpRawStat < 576716 ) { text2Array[ 1 ] = "90"; }
                                else if( tmpRawStat < 629145 ) { text2Array[ 1 ] = "91"; }
                                else if( tmpRawStat < 681574 ) { text2Array[ 1 ] = "92"; }
                                else if( tmpRawStat < 734003 ) { text2Array[ 1 ] = "93"; }
                                else if( tmpRawStat < 786432 ) { text2Array[ 1 ] = "94"; }
                                else if( tmpRawStat < 838860 ) { text2Array[ 1 ] = "95"; }
                                else if( tmpRawStat < 891289 ) { text2Array[ 1 ] = "96"; }
                                else if( tmpRawStat < 943718 ) { text2Array[ 1 ] = "97"; }
                                else if( tmpRawStat < 996147 ) { text2Array[ 1 ] = "98"; }
                                else if( tmpRawStat < 1048576 ) { text2Array[ 1 ] = "99"; }
                                else if( tmpRawStat >= 1048576 ) { text2Array[ 1 ] = "100"; }
                                tmpCharStr = text2Array[ 1 ];
                                break;
                            case "DEX":
                                tmpRawStat = Convert.ToDouble( text2Array[ 1 ] );
                                //Base skill 0 = 0 raw skill points
                                if( tmpRawStat < 204 ) { text2Array[ 1 ] = "0"; }
                                else if( tmpRawStat < 409 ) { text2Array[ 1 ] = "1"; }
                                else if( tmpRawStat < 614 ) { text2Array[ 1 ] = "2"; }
                                else if( tmpRawStat < 819 ) { text2Array[ 1 ] = "3"; }
                                else if( tmpRawStat < 1024 ) { text2Array[ 1 ] = "4"; }
                                else if( tmpRawStat < 1228 ) { text2Array[ 1 ] = "5"; }
                                else if( tmpRawStat < 1433 ) { text2Array[ 1 ] = "6"; }
                                else if( tmpRawStat < 1638 ) { text2Array[ 1 ] = "7"; }
                                else if( tmpRawStat < 1843 ) { text2Array[ 1 ] = "8"; }
                                else if( tmpRawStat < 2048 ) { text2Array[ 1 ] = "9"; }
                                else if( tmpRawStat < 2252 ) { text2Array[ 1 ] = "10"; }
                                else if( tmpRawStat < 2457 ) { text2Array[ 1 ] = "11"; }
                                else if( tmpRawStat < 2662 ) { text2Array[ 1 ] = "12"; }
                                else if( tmpRawStat < 2867 ) { text2Array[ 1 ] = "13"; }
                                else if( tmpRawStat < 3072 ) { text2Array[ 1 ] = "14"; }
                                else if( tmpRawStat < 3276 ) { text2Array[ 1 ] = "15"; }
                                else if( tmpRawStat < 3481 ) { text2Array[ 1 ] = "16"; }
                                else if( tmpRawStat < 3686 ) { text2Array[ 1 ] = "17"; }
                                else if( tmpRawStat < 3891 ) { text2Array[ 1 ] = "18"; }
                                else if( tmpRawStat < 4096 ) { text2Array[ 1 ] = "19"; }
                                else if( tmpRawStat < 4505 ) { text2Array[ 1 ] = "20"; }
                                else if( tmpRawStat < 4915 ) { text2Array[ 1 ] = "21"; }
                                else if( tmpRawStat < 5324 ) { text2Array[ 1 ] = "22"; }
                                else if( tmpRawStat < 5734 ) { text2Array[ 1 ] = "23"; }
                                else if( tmpRawStat < 6144 ) { text2Array[ 1 ] = "24"; }
                                else if( tmpRawStat < 6553 ) { text2Array[ 1 ] = "25"; }
                                else if( tmpRawStat < 6963 ) { text2Array[ 1 ] = "26"; }

                                else if( tmpRawStat < 7372 ) { text2Array[ 1 ] = "27"; }
                                else if( tmpRawStat < 7782 ) { text2Array[ 1 ] = "28"; }
                                else if( tmpRawStat < 8192 ) { text2Array[ 1 ] = "29"; }
                                else if( tmpRawStat < 9011 ) { text2Array[ 1 ] = "30"; }
                                else if( tmpRawStat < 9830 ) { text2Array[ 1 ] = "31"; }
                                else if( tmpRawStat < 10649 ) { text2Array[ 1 ] = "32"; }
                                else if( tmpRawStat < 11468 ) { text2Array[ 1 ] = "33"; }
                                else if( tmpRawStat < 12288 ) { text2Array[ 1 ] = "34"; }
                                else if( tmpRawStat < 13107 ) { text2Array[ 1 ] = "35"; }
                                else if( tmpRawStat < 13926 ) { text2Array[ 1 ] = "36"; }
                                else if( tmpRawStat < 14745 ) { text2Array[ 1 ] = "37"; }
                                else if( tmpRawStat < 15564 ) { text2Array[ 1 ] = "38"; }
                                else if( tmpRawStat < 16384 ) { text2Array[ 1 ] = "39"; }
                                else if( tmpRawStat < 18022 ) { text2Array[ 1 ] = "40"; }
                                else if( tmpRawStat < 19660 ) { text2Array[ 1 ] = "41"; }
                                else if( tmpRawStat < 21299 ) { text2Array[ 1 ] = "42"; }
                                else if( tmpRawStat < 22937 ) { text2Array[ 1 ] = "43"; }
                                else if( tmpRawStat < 24576 ) { text2Array[ 1 ] = "44"; }
                                else if( tmpRawStat < 26214 ) { text2Array[ 1 ] = "45"; }
                                else if( tmpRawStat < 27852 ) { text2Array[ 1 ] = "46"; }
                                else if( tmpRawStat < 29491 ) { text2Array[ 1 ] = "47"; }
                                else if( tmpRawStat < 31129 ) { text2Array[ 1 ] = "48"; }
                                else if( tmpRawStat < 32768 ) { text2Array[ 1 ] = "49"; }
                                else if( tmpRawStat < 36044 ) { text2Array[ 1 ] = "50"; }
                                else if( tmpRawStat < 39321 ) { text2Array[ 1 ] = "51"; }
                                else if( tmpRawStat < 42598 ) { text2Array[ 1 ] = "52"; }
                                else if( tmpRawStat < 45875 ) { text2Array[ 1 ] = "53"; }
                                else if( tmpRawStat < 49152 ) { text2Array[ 1 ] = "54"; }
                                else if( tmpRawStat < 52428 ) { text2Array[ 1 ] = "55"; }
                                else if( tmpRawStat < 55705 ) { text2Array[ 1 ] = "56"; }
                                else if( tmpRawStat < 58982 ) { text2Array[ 1 ] = "57"; }
                                else if( tmpRawStat < 62259 ) { text2Array[ 1 ] = "58"; }
                                else if( tmpRawStat < 65536 ) { text2Array[ 1 ] = "59"; }
                                else if( tmpRawStat < 72089 ) { text2Array[ 1 ] = "60"; }
                                else if( tmpRawStat < 78643 ) { text2Array[ 1 ] = "61"; }
                                else if( tmpRawStat < 85196 ) { text2Array[ 1 ] = "62"; }
                                else if( tmpRawStat < 91750 ) { text2Array[ 1 ] = "63"; }
                                else if( tmpRawStat < 98304 ) { text2Array[ 1 ] = "64"; }
                                else if( tmpRawStat < 104857 ) { text2Array[ 1 ] = "65"; }
                                else if( tmpRawStat < 111411 ) { text2Array[ 1 ] = "66"; }
                                else if( tmpRawStat < 117964 ) { text2Array[ 1 ] = "67"; }
                                else if( tmpRawStat < 124518 ) { text2Array[ 1 ] = "68"; }
                                else if( tmpRawStat < 131072 ) { text2Array[ 1 ] = "69"; }
                                else if( tmpRawStat < 144179 ) { text2Array[ 1 ] = "70"; }
                                else if( tmpRawStat < 157286 ) { text2Array[ 1 ] = "71"; }
                                else if( tmpRawStat < 170393 ) { text2Array[ 1 ] = "72"; }
                                else if( tmpRawStat < 183500 ) { text2Array[ 1 ] = "73"; }
                                else if( tmpRawStat < 196608 ) { text2Array[ 1 ] = "74"; }
                                else if( tmpRawStat < 209715 ) { text2Array[ 1 ] = "75"; }
                                else if( tmpRawStat < 222822 ) { text2Array[ 1 ] = "76"; }
                                else if( tmpRawStat < 235929 ) { text2Array[ 1 ] = "77"; }
                                else if( tmpRawStat < 249036 ) { text2Array[ 1 ] = "78"; }
                                else if( tmpRawStat < 262144 ) { text2Array[ 1 ] = "79"; }
                                else if( tmpRawStat < 288358 ) { text2Array[ 1 ] = "80"; }
                                else if( tmpRawStat < 314572 ) { text2Array[ 1 ] = "81"; }
                                else if( tmpRawStat < 340787 ) { text2Array[ 1 ] = "82"; }
                                else if( tmpRawStat < 367001 ) { text2Array[ 1 ] = "83"; }
                                else if( tmpRawStat < 393216 ) { text2Array[ 1 ] = "84"; }
                                else if( tmpRawStat < 419430 ) { text2Array[ 1 ] = "85"; }
                                else if( tmpRawStat < 445644 ) { text2Array[ 1 ] = "86"; }
                                else if( tmpRawStat < 471859 ) { text2Array[ 1 ] = "87"; }
                                else if( tmpRawStat < 498073 ) { text2Array[ 1 ] = "88"; }
                                else if( tmpRawStat < 524288 ) { text2Array[ 1 ] = "89"; }
                                else if( tmpRawStat < 576716 ) { text2Array[ 1 ] = "90"; }
                                else if( tmpRawStat < 629145 ) { text2Array[ 1 ] = "91"; }
                                else if( tmpRawStat < 681574 ) { text2Array[ 1 ] = "92"; }
                                else if( tmpRawStat < 734003 ) { text2Array[ 1 ] = "93"; }
                                else if( tmpRawStat < 786432 ) { text2Array[ 1 ] = "94"; }
                                else if( tmpRawStat < 838860 ) { text2Array[ 1 ] = "95"; }
                                else if( tmpRawStat < 891289 ) { text2Array[ 1 ] = "96"; }
                                else if( tmpRawStat < 943718 ) { text2Array[ 1 ] = "97"; }
                                else if( tmpRawStat < 996147 ) { text2Array[ 1 ] = "98"; }
                                else if( tmpRawStat < 1048576 ) { text2Array[ 1 ] = "99"; }
                                else if( tmpRawStat >= 1048576 ) { text2Array[ 1 ] = "100"; }
                                tmpCharDex = text2Array[ 1 ];
                                break;
                            case "INT":
                                tmpRawStat = Convert.ToDouble( text2Array[ 1 ] );
                                //Base skill 0 = 0 raw skill points
                                if( tmpRawStat < 204 ) { text2Array[ 1 ] = "0"; }
                                else if( tmpRawStat < 409 ) { text2Array[ 1 ] = "1"; }
                                else if( tmpRawStat < 614 ) { text2Array[ 1 ] = "2"; }
                                else if( tmpRawStat < 819 ) { text2Array[ 1 ] = "3"; }
                                else if( tmpRawStat < 1024 ) { text2Array[ 1 ] = "4"; }
                                else if( tmpRawStat < 1228 ) { text2Array[ 1 ] = "5"; }
                                else if( tmpRawStat < 1433 ) { text2Array[ 1 ] = "6"; }
                                else if( tmpRawStat < 1638 ) { text2Array[ 1 ] = "7"; }
                                else if( tmpRawStat < 1843 ) { text2Array[ 1 ] = "8"; }
                                else if( tmpRawStat < 2048 ) { text2Array[ 1 ] = "9"; }
                                else if( tmpRawStat < 2252 ) { text2Array[ 1 ] = "10"; }
                                else if( tmpRawStat < 2457 ) { text2Array[ 1 ] = "11"; }
                                else if( tmpRawStat < 2662 ) { text2Array[ 1 ] = "12"; }
                                else if( tmpRawStat < 2867 ) { text2Array[ 1 ] = "13"; }
                                else if( tmpRawStat < 3072 ) { text2Array[ 1 ] = "14"; }
                                else if( tmpRawStat < 3276 ) { text2Array[ 1 ] = "15"; }
                                else if( tmpRawStat < 3481 ) { text2Array[ 1 ] = "16"; }
                                else if( tmpRawStat < 3686 ) { text2Array[ 1 ] = "17"; }
                                else if( tmpRawStat < 3891 ) { text2Array[ 1 ] = "18"; }
                                else if( tmpRawStat < 4096 ) { text2Array[ 1 ] = "19"; }
                                else if( tmpRawStat < 4505 ) { text2Array[ 1 ] = "20"; }
                                else if( tmpRawStat < 4915 ) { text2Array[ 1 ] = "21"; }
                                else if( tmpRawStat < 5324 ) { text2Array[ 1 ] = "22"; }
                                else if( tmpRawStat < 5734 ) { text2Array[ 1 ] = "23"; }
                                else if( tmpRawStat < 6144 ) { text2Array[ 1 ] = "24"; }
                                else if( tmpRawStat < 6553 ) { text2Array[ 1 ] = "25"; }
                                else if( tmpRawStat < 6963 ) { text2Array[ 1 ] = "26"; }
                                else if( tmpRawStat < 7372 ) { text2Array[ 1 ] = "27"; }
                                else if( tmpRawStat < 7782 ) { text2Array[ 1 ] = "28"; }
                                else if( tmpRawStat < 8192 ) { text2Array[ 1 ] = "29"; }
                                else if( tmpRawStat < 9011 ) { text2Array[ 1 ] = "30"; }
                                else if( tmpRawStat < 9830 ) { text2Array[ 1 ] = "31"; }
                                else if( tmpRawStat < 10649 ) { text2Array[ 1 ] = "32"; }
                                else if( tmpRawStat < 11468 ) { text2Array[ 1 ] = "33"; }
                                else if( tmpRawStat < 12288 ) { text2Array[ 1 ] = "34"; }
                                else if( tmpRawStat < 13107 ) { text2Array[ 1 ] = "35"; }
                                else if( tmpRawStat < 13926 ) { text2Array[ 1 ] = "36"; }
                                else if( tmpRawStat < 14745 ) { text2Array[ 1 ] = "37"; }
                                else if( tmpRawStat < 15564 ) { text2Array[ 1 ] = "38"; }
                                else if( tmpRawStat < 16384 ) { text2Array[ 1 ] = "39"; }
                                else if( tmpRawStat < 18022 ) { text2Array[ 1 ] = "40"; }
                                else if( tmpRawStat < 19660 ) { text2Array[ 1 ] = "41"; }
                                else if( tmpRawStat < 21299 ) { text2Array[ 1 ] = "42"; }
                                else if( tmpRawStat < 22937 ) { text2Array[ 1 ] = "43"; }
                                else if( tmpRawStat < 24576 ) { text2Array[ 1 ] = "44"; }
                                else if( tmpRawStat < 26214 ) { text2Array[ 1 ] = "45"; }
                                else if( tmpRawStat < 27852 ) { text2Array[ 1 ] = "46"; }
                                else if( tmpRawStat < 29491 ) { text2Array[ 1 ] = "47"; }
                                else if( tmpRawStat < 31129 ) { text2Array[ 1 ] = "48"; }
                                else if( tmpRawStat < 32768 ) { text2Array[ 1 ] = "49"; }
                                else if( tmpRawStat < 36044 ) { text2Array[ 1 ] = "50"; }
                                else if( tmpRawStat < 39321 ) { text2Array[ 1 ] = "51"; }
                                else if( tmpRawStat < 42598 ) { text2Array[ 1 ] = "52"; }
                                else if( tmpRawStat < 45875 ) { text2Array[ 1 ] = "53"; }
                                else if( tmpRawStat < 49152 ) { text2Array[ 1 ] = "54"; }
                                else if( tmpRawStat < 52428 ) { text2Array[ 1 ] = "55"; }
                                else if( tmpRawStat < 55705 ) { text2Array[ 1 ] = "56"; }
                                else if( tmpRawStat < 58982 ) { text2Array[ 1 ] = "57"; }
                                else if( tmpRawStat < 62259 ) { text2Array[ 1 ] = "58"; }
                                else if( tmpRawStat < 65536 ) { text2Array[ 1 ] = "59"; }
                                else if( tmpRawStat < 72089 ) { text2Array[ 1 ] = "60"; }
                                else if( tmpRawStat < 78643 ) { text2Array[ 1 ] = "61"; }
                                else if( tmpRawStat < 85196 ) { text2Array[ 1 ] = "62"; }
                                else if( tmpRawStat < 91750 ) { text2Array[ 1 ] = "63"; }
                                else if( tmpRawStat < 98304 ) { text2Array[ 1 ] = "64"; }
                                else if( tmpRawStat < 104857 ) { text2Array[ 1 ] = "65"; }
                                else if( tmpRawStat < 111411 ) { text2Array[ 1 ] = "66"; }
                                else if( tmpRawStat < 117964 ) { text2Array[ 1 ] = "67"; }
                                else if( tmpRawStat < 124518 ) { text2Array[ 1 ] = "68"; }
                                else if( tmpRawStat < 131072 ) { text2Array[ 1 ] = "69"; }
                                else if( tmpRawStat < 144179 ) { text2Array[ 1 ] = "70"; }
                                else if( tmpRawStat < 157286 ) { text2Array[ 1 ] = "71"; }
                                else if( tmpRawStat < 170393 ) { text2Array[ 1 ] = "72"; }
                                else if( tmpRawStat < 183500 ) { text2Array[ 1 ] = "73"; }
                                else if( tmpRawStat < 196608 ) { text2Array[ 1 ] = "74"; }
                                else if( tmpRawStat < 209715 ) { text2Array[ 1 ] = "75"; }
                                else if( tmpRawStat < 222822 ) { text2Array[ 1 ] = "76"; }
                                else if( tmpRawStat < 235929 ) { text2Array[ 1 ] = "77"; }
                                else if( tmpRawStat < 249036 ) { text2Array[ 1 ] = "78"; }
                                else if( tmpRawStat < 262144 ) { text2Array[ 1 ] = "79"; }
                                else if( tmpRawStat < 288358 ) { text2Array[ 1 ] = "80"; }
                                else if( tmpRawStat < 314572 ) { text2Array[ 1 ] = "81"; }
                                else if( tmpRawStat < 340787 ) { text2Array[ 1 ] = "82"; }
                                else if( tmpRawStat < 367001 ) { text2Array[ 1 ] = "83"; }
                                else if( tmpRawStat < 393216 ) { text2Array[ 1 ] = "84"; }
                                else if( tmpRawStat < 419430 ) { text2Array[ 1 ] = "85"; }
                                else if( tmpRawStat < 445644 ) { text2Array[ 1 ] = "86"; }
                                else if( tmpRawStat < 471859 ) { text2Array[ 1 ] = "87"; }
                                else if( tmpRawStat < 498073 ) { text2Array[ 1 ] = "88"; }
                                else if( tmpRawStat < 524288 ) { text2Array[ 1 ] = "89"; }
                                else if( tmpRawStat < 576716 ) { text2Array[ 1 ] = "90"; }
                                else if( tmpRawStat < 629145 ) { text2Array[ 1 ] = "91"; }
                                else if( tmpRawStat < 681574 ) { text2Array[ 1 ] = "92"; }
                                else if( tmpRawStat < 734003 ) { text2Array[ 1 ] = "93"; }
                                else if( tmpRawStat < 786432 ) { text2Array[ 1 ] = "94"; }
                                else if( tmpRawStat < 838860 ) { text2Array[ 1 ] = "95"; }
                                else if( tmpRawStat < 891289 ) { text2Array[ 1 ] = "96"; }
                                else if( tmpRawStat < 943718 ) { text2Array[ 1 ] = "97"; }
                                else if( tmpRawStat < 996147 ) { text2Array[ 1 ] = "98"; }
                                else if( tmpRawStat < 1048576 ) { text2Array[ 1 ] = "99"; }
                                else if( tmpRawStat >= 1048576 ) { text2Array[ 1 ] = "100"; }
                                tmpCharInt = text2Array[ 1 ];
                                break;
                            #endregion
                            #region skills
                            case "Alchemy":
                                tSkills[ 0 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Anatomy":
                                tSkills[ 1 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "AnimalLore":
                                tSkills[ 2 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "ItemId":
                                tSkills[ 3 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "ArmsLore":
                                tSkills[ 4 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Parry":
                                tSkills[ 5 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Begging":
                                tSkills[ 6 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Blacksmithy":
                                tSkills[ 7 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Bowcraft":
                                tSkills[ 8 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Peacemaking":
                                tSkills[ 9 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Camping":
                                tSkills[ 10 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Carpentry":
                                tSkills[ 11 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Cartography":
                                tSkills[ 12 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Cooking":
                                tSkills[ 13 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "DetectingHidden":
                                tSkills[ 14 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            // Enticement -> Discordance 
                            case "Enticement":
                                tSkills[ 15 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "EvaluatingIntelligence":
                                tSkills[ 16 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Healing":
                                tSkills[ 17 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Fishing":
                                tSkills[ 18 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "ForensicEvaluation":
                                tSkills[ 19 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Herding":
                                tSkills[ 20 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Hiding":
                                tSkills[ 21 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Provocation":
                                tSkills[ 22 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Inscription":
                                tSkills[ 23 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Lockpicking":
                                tSkills[ 24 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Magery":
                                tSkills[ 25 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "MagicResistance":
                                tSkills[ 26 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Tactics":
                                tSkills[ 27 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Snooping":
                                tSkills[ 28 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Musicianship":
                                tSkills[ 29 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Poisoning":
                                tSkills[ 30 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Archery":
                                tSkills[ 31 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "SpiritSpeak":
                                tSkills[ 32 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Stealing":
                                tSkills[ 33 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Tailoring":
                                tSkills[ 34 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "AnimalTaming":
                                tSkills[ 35 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "TasteIdentification":
                                tSkills[ 36 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Tinkering":
                                tSkills[ 37 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Tracking":
                                tSkills[ 38 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Veterinary":
                                tSkills[ 39 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Swordsmanship":
                                tSkills[ 40 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Macefighting":
                                tSkills[ 41 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Fencing":
                                tSkills[ 42 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Wrestling":
                                tSkills[ 43 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Lumberjacking":
                                tSkills[ 44 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Mining":
                                tSkills[ 45 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Meditation":
                                tSkills[ 46 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "Stealth":
                                tSkills[ 47 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            case "RemoveTrap":
                                tSkills[ 48 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            // customize skills from here on - you may want to make some changes to this list 
                            case "Necromancy":
                                tSkills[ 49 ] = Convert.ToDouble( text2Array[ 1 ] );
                                break;
                            #endregion
                            default:
                                break;
                        }
                    }

                    // 2. create the character 
                    newChar = new Midgard2PlayerMobile();

                    // 3. set up the character's specific properties 
                    newCharAcct = Accounts.GetAccount( tmpCharAccountString ) as Account;
                    if( newCharAcct == null )
                        continue;

                    newChar.Name = tmpCharName;
                    newChar.Player = true;

                    newChar.Female = tmpCharGender != "0";

                    newChar.Body = newChar.Female ? 0x191 : 0x190;

                    newChar.Hue = int.Parse( tmpCharHue.Substring( 2 ), NumberStyles.HexNumber );
                    newChar.Hunger = 20;

                    CityInfo city = new CityInfo( "Cove", "Centro", 2230, 1224, 0, Map.Felucca );
                    newChar.Location = city.Location;
                    newChar.Map = city.Map;

                    newChar.Karma = tmpKarma;
                    newChar.Fame = tmpFame;

                    // stats 
                    newChar.InitStats( (int)Math.Round( Convert.ToDecimal( tmpCharStr ) ),
                    (int)Math.Round( Convert.ToDecimal( tmpCharDex ) ),
                    (int)Math.Round( Convert.ToDecimal( tmpCharInt ) ) );

                    // skills
                    for( int j = 0; j < tSkills.Length; j++ )
                    {
                        bool found = false;
                        for( int i = 0; i < m_RawPointsCurve.Length && found == false; i++ )
                        {
                            if( tSkills[ j ] > 0 && tSkills[ j ] < m_RawPointsCurve[ i ] )
                            {
                                found = true;
                                newChar.Skills[ (SkillName)j ].BaseFixedPoint = i;
                                Log( "SkillSet: {0} value {1} (raw {2})", (SkillName)j, i / 10.0, tSkills[ 0 ] );
                            }
                        }
                    }

                    // access level 
                    if( ( tmpCharCmdlevel == "test" ) || ( tmpCharCmdlevel == "admin" ) || ( tmpCharCmdlevel == "gm" ) || ( tmpCharCmdlevel == "coun" ) || ( tmpCharCmdlevel == "seer" ) )
                    {
                        newCharAcct.Banned = true;
                    }

                    // 4. Try to add character to account: 
                    if( ( tmpCharName != "" ) && ( tmpCharAccountString != "" ) )
                    {
                        int i = 0;
                        while( i < 5 )
                        {
                            if( newCharAcct[ i ] == null )
                            {
                                Log( from, string.Format( "Adding character {0} to account {1} [ position {2} ]", tmpCharName, tmpCharAccountString, i ) );
                                newCharAcct[ i ] = newChar;
                                i = 5;
                                characterCount++;

                                // create a backpack & some items for the character: 
                                Container pack = newChar.Backpack;
                                if( pack == null )
                                {
                                    pack = new Backpack();
                                    pack.Movable = false;
                                    newChar.AddItem( pack );
                                }

                                Item2Container( new RedBook( "a book", newChar.Name, 20, true ), pack );
                                Item2Container( new Gold( 200 ), pack );
                                Item2Container( new Dagger(), pack );
                                Item2Container( new Candle(), pack );

                                Race race = newChar.Race;
                                newChar.HairItemID = race.RandomHair( newChar );
                                newChar.FacialHairItemID = race.RandomFacialHair( newChar );
                            }

                            i++;
                        }
                    }
                }
            }

            // Stop reading and close the file. 
            sr2Time.Close();
            fs2Time.Close();

            Log( from, string.Format( "Done importing {0} characters.", characterCount ) );
        }

        #region Logging
        private static void InitLogger()
        {
            // Create the log writer
            try
            {
                string file = string.Format( "PolAccountImporting-_{0}.txt", DateTime.Now.ToLongDateString() );

                m_Writer = new StreamWriter( file, true );
                m_Writer.AutoFlush = true;

                m_Writer.WriteLine( "###############################" );
                m_Writer.WriteLine( "# {0} - {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                m_Writer.WriteLine();
            }
            catch( Exception err )
            {
                Console.WriteLine( "Couldn't initialize pol account import log. Reason:" );
                Console.WriteLine( err.ToString() );
            }
        }

        private static void Log( string toLog )
        {
            try
            {
                if( m_Writer != null )
                    m_Writer.WriteLine( toLog );
            }
            catch
            {
            }
        }

        private static void Log( string format, params object[] args )
        {
            Log( String.Format( format, args ) );
        }

        private static void Log( Mobile from, string toLog )
        {
            from.SendMessage( toLog );

            try
            {
                if( m_Writer != null )
                    m_Writer.WriteLine( toLog );
            }
            catch
            {
            }
        }

        private static void Log( Mobile from, string format, params object[] args )
        {
            Log( from, String.Format( format, args ) );
        }
        #endregion

        private static void Item2Container( Item item, Container container )
        {
            item.LootType = LootType.Newbied;

            if( container != null )
                container.DropItem( item );
            else
                item.Delete();
        }

        private static Account CreateAccount( string un, string pw, string em )
        {
            if( un.Length == 0 || pw.Length == 0 )
                return null;

            bool isSafe = true;

            for( int i = 0; isSafe && i < un.Length; ++i )
                isSafe = ( un[ i ] >= 0x20 && un[ i ] < 0x80 );

            for( int i = 0; isSafe && i < pw.Length; ++i )
                isSafe = ( pw[ i ] >= 0x20 && pw[ i ] < 0x80 );

            if( !isSafe )
                return null;

            Console.WriteLine( "Creating new account '{0}'", un );

            Account a = new Account( un, pw, em );

            return a;
        }
    }
}