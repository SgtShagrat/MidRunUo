using System;

using Server;

namespace Midgard.Engines.Events
{
    public class ChristmasHelper
    {
        public static readonly string StaticMidgardGreetings = "Auguri di un Buon Natale 2010 da tutto lo staff di Midgard!";
        public static readonly DateTime EndOfChristmasPeriod = new DateTime( 2011, 1, 6 );

        public static void VerifyEndPeriod_Callback( object state )
        {
            Item item = (Item)state;

            if( item == null )
            {
                Console.WriteLine( "Warning: null cast in VerifyEndPeriod_Callback" );
            }
            else
            {
                if( DateTime.Now > EndOfChristmasPeriod && !item.Deleted )
                {
                    item.Delete();
                }
            }
        }

        private static readonly string[] MidgardGreetings = new string[]
                                                            {
                                                                "Buon Natale 2010 su Midgard!",
                                                                "Buone feste e felice anno nuovo!",
                                                                "Un augurio di buon Natale dallo staff di Midgard!",
                                                                "Buone feste su Midgard!",
                                                                "Uno splendido Natale a tutti dallo staff di Midgard!"
                                                            };

        public static string GetMidgardGreetings()
        {
            return MidgardGreetings[ Utility.Random( MidgardGreetings.Length ) ];
        }

        public enum MidgardStaff
        {
            Lex,
            Saerial,
            Belnar,
            DiesIrae,
            Kuroro,
            Papclems,
            Liriel
        }

        private static readonly string[] m_Names = new string[]
                                                   {
                                                       "Lex", "Saerial", "Belnar", "Dies Irae", "Kuroro", "Papclems", "Liriel"
                                                   };

        public static string GetStaffMember( MidgardStaff member )
        {
            return m_Names[ (int)member ];
        }

        public static MidgardStaff GetRandomStaffMember()
        {
            return (MidgardStaff)Utility.Random( Enum.GetNames( typeof( MidgardStaff ) ).Length );
        }

        public static string GetRandomStaffMemberName()
        {
            return GetStaffMember( GetRandomStaffMember() );
        }

        public enum SnowScenes
        {
            Minoc,
            Vesper,
            Cove,
            Yew,
            Britain,
            SkaraBrae,
            Trinsic,
            SerpentsHold,
            Nejelm,
            Haven,
            BuccaneersDen,
            Jhelom,
            Moonglow,
            Delucia,
            Papua,
            Occlo,
            EmpathsAbbey,
            TheLycaeum,
            Wind,
            Magincia,
            Luna,
            Umbra,
            CityOfMistas,
            CityOfMontor,
            EtherealFortress,
            AncientCitadel,
            ShrineOfValor,
            ShrineOfSpirtuality,
            ShrineOfSacifice,
            ShrineOfJustice,
            ShrineOfHumility,
            ShrineOfHonor,
            ShrineOfHonesty,
            ShrineOfCompassion,
            PassOfKarnaugh
        }

        public static string GetSceneName( SnowScenes type )
        {
            string name;

            switch( type )
            {
                case ( SnowScenes.Minoc ):
                    name = "Minoc";
                    break;
                case ( SnowScenes.Vesper ):
                    name = "Vesper";
                    break;
                case ( SnowScenes.Cove ):
                    name = "Cove";
                    break;
                case ( SnowScenes.Yew ):
                    name = "Yew";
                    break;
                case ( SnowScenes.Britain ):
                    name = "Britain";
                    break;
                case ( SnowScenes.SkaraBrae ):
                    name = "Skara Brae";
                    break;
                case ( SnowScenes.Trinsic ):
                    name = "Trinsic";
                    break;
                case ( SnowScenes.SerpentsHold ):
                    name = "Serpent's Hold";
                    break;
                case ( SnowScenes.Nejelm ):
                    name = "Nejelm";
                    break;
                case ( SnowScenes.Haven ):
                    name = "Haven";
                    break;
                case ( SnowScenes.BuccaneersDen ):
                    name = "Buccaneers Den";
                    break;
                case ( SnowScenes.Jhelom ):
                    name = "Jhelom";
                    break;
                case ( SnowScenes.Moonglow ):
                    name = "Moonglow";
                    break;
                case ( SnowScenes.Delucia ):
                    name = "Delucia";
                    break;
                case ( SnowScenes.Papua ):
                    name = "Papua";
                    break;
                case ( SnowScenes.Occlo ):
                    name = "Occlo";
                    break;
                case ( SnowScenes.EmpathsAbbey ):
                    name = "Empaths Abbey";
                    break;
                case ( SnowScenes.TheLycaeum ):
                    name = "The Lycaeum";
                    break;
                case ( SnowScenes.Wind ):
                    name = "Wind";
                    break;
                case ( SnowScenes.Magincia ):
                    name = "Magincia";
                    break;
                case ( SnowScenes.Luna ):
                    name = "Luna";
                    break;
                case ( SnowScenes.Umbra ):
                    name = "Umbra";
                    break;
                case ( SnowScenes.CityOfMistas ):
                    name = "City Of Mistas";
                    break;
                case ( SnowScenes.CityOfMontor ):
                    name = "City Of Montor";
                    break;
                case ( SnowScenes.EtherealFortress ):
                    name = "Ethereal Fortress";
                    break;
                case ( SnowScenes.AncientCitadel ):
                    name = "Ancient Citadel";
                    break;
                case ( SnowScenes.ShrineOfValor ):
                    name = "Shrine Of Valor";
                    break;
                case ( SnowScenes.ShrineOfSpirtuality ):
                    name = "Shrine Of Spirtuality";
                    break;
                case ( SnowScenes.ShrineOfSacifice ):
                    name = "Shrine Of Sacifice";
                    break;
                case ( SnowScenes.ShrineOfJustice ):
                    name = "Shrine Of Justice";
                    break;
                case ( SnowScenes.ShrineOfHumility ):
                    name = "Shrine Of Humility";
                    break;
                case ( SnowScenes.ShrineOfHonor ):
                    name = "Shrine Of Honor";
                    break;
                case ( SnowScenes.ShrineOfHonesty ):
                    name = "Shrine Of Honesty";
                    break;
                case ( SnowScenes.ShrineOfCompassion ):
                    name = "Shrine Of Compassion";
                    break;
                case ( SnowScenes.PassOfKarnaugh ):
                    name = "Pass Of Karnaugh";
                    break;
                default:
                    name = "Merry Christmass from Midgard Staff!";
                    break;
            }

            return name;
        }

        public static int GetCurrentYear
        {
            get { return 2010; }
        }
    }
}