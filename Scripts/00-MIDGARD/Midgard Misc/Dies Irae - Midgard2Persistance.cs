/***************************************************************************
 *                                    Midgard2Persistance.cs
 *                            		--------------------------
 *  begin                	: Aprile, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Server;
using Server.Commands;
using Server.Items;
using Server.Network;

namespace Midgard.Misc
{
    public class Midgard2Persistance : Item
    {
        #region Script settings
        /// <summary>
        /// Server Name
        /// </summary>
        public static string ServerName = "Midgard";

        /// <summary>
        ///  Server ip
        /// </summary>		
        public static string Address = "midgardshard.ddns.net";

        [CommandProperty( AccessLevel.Administrator, AccessLevel.Developer )]
        public static string RealmAddress
        {
            get
            {
                return Address;
            }
            set
            {
                Address = value;
            }
        }

        /// <summary>
        /// Port
        /// </summary>
        public static int MidgardSocketPort = 5003;

        /// <summary>
        /// Is test center enabled
        /// </summary>
        public static bool TestCenterEnabled = false;

        /// <summary>
        /// Is Factionsystem enabled
        /// </summary>
        public static bool Factions = false;

        /// <summary>
        /// Is Myrunuo system enabled
        /// </summary>
        public static bool MyRunUoEnabled = false;

        /// <summary>
        /// Expansion Supported
        /// </summary>
        public static Expansion Expansion = Expansion.None;

        public static bool ToTEnabled = false;
        public static double ToTDemultiplier = 0.3;
        public static int ToTMinFameNoGain = 3000;

        public static bool ToMEnabled = true;
        public static double ToMDemultiplier = 0.01;
        public static int ToMMinFameNoGain = 1000;

        public static bool FactionStatLossEnabled = false;
        public static double FactionStatLossPercentage = 0.33;
        public static TimeSpan FactionStatLossDuration = TimeSpan.FromMinutes( 20.0 );

        public static bool TownSystemStatLossEnabled = false;
        public static double TownSystemStatLossPercentage = 0.50;
        public static TimeSpan TownSystemStatLossDuration = TimeSpan.FromHours( 48.0 );
        public static bool TownSystemJailEnabled = false;

        public static bool InstanceLootEnabled = true;

        public static bool VeteranRewards = false;
        public static bool SkillCapRewards = false;
        public static TimeSpan RewardInterval = TimeSpan.FromDays( 120.0 );

        public static bool SmithBulksEnabled = false;
        public static bool TailorBulksEnabled = false;
        public static bool TamingBulksEnabled = false;

        public static bool DailySaveBackup = false; //mod by Magius(CHE) (Muletto): false=disabilitato perchè rallenta il server. Questo metodo dovrebbe creare un thread che fail backup e non backluppare syncronicamente

        public static bool FtpService = false; //mod by Magius(CHE) (Muletto): false=disabilitato perchè rallenta il server. Questo metodo dovrebbe creare un thread che fail backup e non backluppare syncronicamente

        public static bool MidgardAdvancedCraftEnabled = false;

        public static double GoldDemultiplier = 0.400;

        public static bool RecipeEnabled = false;
        public static bool TalismansEnabled = false;
        public static bool IsTokunoLootEnabled = false;
        public static bool IsMondainLootEnabled = false;

        public static bool IsParagonEnabled = true;
        public static bool IsParagonMinorArtifactsEnabled = false;

        public static bool IsDemonKnightArtifactsEnabled = false;

        public static bool PowerScrollsEnabled = false;
        public static bool StatScrollsEnabled = false;

        public static bool PaganPotionsEnabled = true;

        public static bool AdvSmeltingEnabled = true;

        public static bool SpecialLogsEnabled = true;

        public static bool Commercials = false;
        #endregion

        #region combat
        [CommandProperty( AccessLevel.Administrator )]
        public static double DamageScalarVsPlayerSummons
        {
            get { return BaseWeapon.DamageScalarVsPlayerSummons; }
            set
            {
                double oldValue = BaseWeapon.DamageScalarVsPlayerSummons;

                if( oldValue != value )
                {
                    BaseWeapon.DamageScalarVsPlayerSummons = value;
                    BroadcastChange( "DamageScalarVsPlayerSummons", BaseWeapon.DamageScalarVsPlayerSummons, oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public static double CreatureVsRangedPlayerDamageScalar
        {
            get { return BaseWeapon.CreatureVsRangedPlayerDamageScalar; }
            set
            {
                double oldValue = BaseWeapon.CreatureVsRangedPlayerDamageScalar;

                if( oldValue != value )
                {
                    BaseWeapon.DamageScalarVsPlayerSummons = value;
                    BroadcastChange( "CreatureVsRangedPlayerDamageScalar", BaseWeapon.CreatureVsRangedPlayerDamageScalar, oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public static double CreatureVsMeleePlayerDamageScalar
        {
            get { return BaseWeapon.CreatureVsMeleePlayerDamageScalar; }
            set
            {
                double oldValue = BaseWeapon.CreatureVsMeleePlayerDamageScalar;

                if( oldValue != value )
                {
                    BaseWeapon.CreatureVsMeleePlayerDamageScalar = value;
                    BroadcastChange( "CreatureVsMeleePlayerDamageScalar", BaseWeapon.CreatureVsMeleePlayerDamageScalar, oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public static double CreatureVsPlayerDamageScalar
        {
            get { return BaseWeapon.CreatureVsPlayerDamageScalar; }
            set
            {
                double oldValue = BaseWeapon.CreatureVsPlayerDamageScalar;

                if( oldValue != value )
                {
                    BaseWeapon.CreatureVsPlayerDamageScalar = value;
                    BroadcastChange( "CreatureVsPlayerDamageScalar", BaseWeapon.CreatureVsPlayerDamageScalar, oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public static double DamageScalarCreaturesVsPlayerSummons
        {
            get { return BaseWeapon.DamageScalarVsPlayerSummons; }
            set
            {
                double oldValue = BaseWeapon.DamageScalarCreaturesVsPlayerSummons;

                if( oldValue != value )
                {
                    BaseWeapon.DamageScalarCreaturesVsPlayerSummons = value;
                    BroadcastChange( "DamageScalarCreaturesVsPlayerSummons", BaseWeapon.DamageScalarCreaturesVsPlayerSummons, oldValue );
                }
            }
        }
        #endregion

        #region broadcast
        public static void BroadcastChange( string parName, double oldValue, double newValue )
        {
            BroadcastToStaff( "Warning: '{0}' value changed to {1:F3} (was {2:F3}).", parName, oldValue, newValue );
        }

        public static void BroadcastToStaff( int hue, string format, params object[] args )
        {
            List<NetState> list = NetState.Instances;

            foreach( NetState netState in list )
            {
                if( netState != null && netState.Mobile != null && netState.Mobile.AccessLevel > AccessLevel.Player )
                    netState.Mobile.SendAsciiMessage( hue, format, args );
            }
        }

        public static void BroadcastToStaff( string message )
        {
            BroadcastToStaff( 0x35, message );
        }

        public static void BroadcastToStaff( string format, params object[] args )
        {
            BroadcastToStaff( 0x35, String.Format( format, args ) );
        }
        #endregion

        private static readonly string m_ConfigFile = Path.Combine( Core.BaseDirectory, "Data/ScriptsSettings.xml" );
        private const string RootName = "Settings";
        public override string DefaultName { get { return "Midgard Persistance - Internal"; } }

        #region map rules

        [CommandProperty( AccessLevel.Administrator, AccessLevel.Developer )]
        public static MapRules FeluccaMapRules
        {
            get { return Map.Felucca.Rules; }
            set
            {
                MapRules oldValue = Map.Felucca.Rules;
                if( oldValue != value )
                {
                    Map.Felucca.Rules = value;
                }
            }
        }

        [CommandProperty( AccessLevel.Administrator, AccessLevel.Developer )]
        public static MapRules TrammelMapRules
        {
            get { return Map.Trammel.Rules; }
            set
            {
                MapRules oldValue = Map.Trammel.Rules;
                if( oldValue != value )
                {
                    Map.Trammel.Rules = value;
                }
            }
        }

        [CommandProperty( AccessLevel.Administrator, AccessLevel.Developer )]
        public static MapRules IlshenarMapRules
        {
            get { return Map.Ilshenar.Rules; }
            set
            {
                MapRules oldValue = Map.Ilshenar.Rules;
                if( oldValue != value )
                {
                    Map.Ilshenar.Rules = value;
                }
            }
        }

        [CommandProperty( AccessLevel.Administrator, AccessLevel.Developer )]
        public static MapRules MalasMapRules
        {
            get { return Map.Malas.Rules; }
            set
            {
                MapRules oldValue = Map.Malas.Rules;
                if( oldValue != value )
                {
                    Map.Malas.Rules = value;
                }
            }
        }

        [CommandProperty( AccessLevel.Administrator, AccessLevel.Developer )]
        public static MapRules TokunoMapRules
        {
            get { return Map.Tokuno.Rules; }
            set
            {
                MapRules oldValue = Map.Tokuno.Rules;
                if( oldValue != value )
                {
                    Map.Tokuno.Rules = value;
                }
            }
        }

        #endregion

        #region Singleton pattern
        private static Midgard2Persistance m_Instance;

        public static Midgard2Persistance Instance
        {
            get
            {
                if( m_Instance == null )
                {
                    m_Instance = new Midgard2Persistance();
                }
                return m_Instance;
            }
        }
        #endregion

        [CallPriority( -100 )]
        public static void Configure()
        {
            if( !File.Exists( m_ConfigFile ) )
            {
                Console.WriteLine( "Warning: ScriptsSettings.xml not found." );
                return;
            }

            Element element = ConfigParser.GetConfig( m_ConfigFile, RootName );

            if( null == element || element.ChildElements.Count <= 0 )
                return;

            foreach( Element child in element.ChildElements )
            {
                object parsed = Parse( child );

                if( parsed != null )
                {
                    ReflectionHelper.SetPublicStaticFieldValue( child.TagName, typeof( Midgard2Persistance ), parsed );
                }
            }
        }

        public static object Parse( Element element )
        {
            if( element == null )
                return null;

            string tagName = element.TagName;
            string typeName = element.Attribute( "type" );

            if( tagName == null || typeName == null )
                return null;

            switch( typeName )
            {
                case "string":
                    return !String.IsNullOrEmpty( element.Text ) ? element.Text : string.Empty;
                case "int":
                    {
                        int i;
                        if( element.GetIntValue( out i ) )
                            return i;
                        else
                        {
                            Console.WriteLine( "Warning: failed integer parsing in Midgard2Persistance" );
                            break;
                        }
                    }
                case "double":
                    double d;
                    if( element.GetDoubleValue( out d ) )
                        return d;
                    else
                    {
                        Console.WriteLine( "Warning: failed double parsing in Midgard2Persistance" );
                        break;
                    }
                case "bool":
                    {
                        bool b;
                        if( element.GetBoolValue( out b ) )
                            return b;
                        else
                        {
                            Console.WriteLine( "Warning: failed bool parsing in Midgard2Persistance" );
                            break;
                        }
                    }
                case "timespan":
                    {
                        TimeSpan ts;
                        if( element.GetTimeSpan( out ts ) )
                            return ts;
                        else
                        {
                            Console.WriteLine( "Warning: failed time span parsing in Midgard2Persistance" );
                            break;
                        }
                    }
                case "enum":
                    {
                        try
                        {
                            int i = 0;

                            string enumType = element.Attribute( "enumtype" );
                            Type t = Assembly.GetAssembly( typeof( Core ) ).GetType( enumType );
                            if( t == null )
                            {
                                Console.WriteLine( "enumType is null" );
                                return i;
                            }

                            if( element.GetEnum( t, out i ) )
                                return i;
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }
                        break;
                    }
                default:
                    Console.WriteLine( "Warning: non supported type parsing in Midgard2Persistance" );
                    return null;
            }

            return null;
        }

        public static void Initialize()
        {
            CommandSystem.Register( "GenerateMidgard2Persistance", AccessLevel.Developer, new CommandEventHandler( GenerateMidgard2Persistance_OnCommand ) );
            CommandSystem.Register( "MidgardSettings", AccessLevel.Administrator, new CommandEventHandler( PersistanceProps_OnCommand ) );

            if( Factions )
                CommandSystem.Register( "CheckFactionStatLoss", AccessLevel.Player, new CommandEventHandler( CheckFactionStatLoss_OnCommand ) );

            // CommandSystem.Register( "VerificaStatLossCittadino", AccessLevel.Player, new CommandEventHandler( CheckTownSystemStatLoss_OnCommand ) );
            // CommandSystem.Register( "VerificaRegoleDiIngaggio", AccessLevel.Player, new CommandEventHandler( CheckMapEngageRules_OnCommand ) );

            EnsureExistence();
        }

        #region constructors
        [Constructable]
        public Midgard2Persistance()
            : base( 1 )
        {
            Movable = false;

            if( m_Instance == null || m_Instance.Deleted )
                m_Instance = this;
            else
                base.Delete();
        }
        #endregion

        #region members
        public static void EnsureExistence()
        {
            if( m_Instance == null )
                m_Instance = new Midgard2Persistance();
        }

        public override void Delete()
        {
        }
        #endregion

        #region serialization
        public Midgard2Persistance( Serial serial )
            : base( serial )
        {
            m_Instance = this;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 6 ); // version

            // version 6: removed race attribute
            // version 5: removed unused variables
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 6:
                    goto case 5;
                case 5:
                    goto case 4;
                case 4:
                    {
                        if( version < 6 )
                        {
                            reader.ReadInt();
                            reader.ReadInt();
                        }
                        goto case 3;
                    }
                case 3:
                    {
                        if( version < 5 )
                            reader.ReadBool();
                        goto case 2;
                    }
                case 2:
                    {
                        if( version < 5 )
                        {
                            reader.ReadBool();
                            reader.ReadDouble();
                            reader.ReadInt();
                        }
                        goto case 1;
                    }
                case 1:
                    {
                        if( version < 5 )
                            reader.ReadBool();
                        goto case 0;
                    }
                case 0:
                    {
                        if( version < 5 )
                        {
                            reader.ReadBool();
                            reader.ReadDouble();
                            reader.ReadTimeSpan();

                            reader.ReadBool();
                            reader.ReadDouble();
                            reader.ReadTimeSpan();
                        }
                        break;
                    }
            }
        }
        #endregion

        #region callbacks
        [Usage( "GenerateMidgard2Persistance" )]
        [Description( "Generate a Midgard2Persistance object." )]
        public static void GenerateMidgard2Persistance_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null || e.Length != 0 )
                return;

            Mobile from = e.Mobile;

            try
            {
                new Midgard2Persistance();
                from.SendMessage( 37, "Midgard2Persistance object created succesfully." );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        [Usage( "CheckFactionStatLoss" )]
        [Description( "Returns a value indicating if faction statloss in enabled or not." )]
        public static void CheckFactionStatLoss_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null || e.Length != 0 )
                return;

            Mobile from = e.Mobile;

            try
            {
                string enabled = FactionStatLossEnabled ? "enabled" : "disabled";
                from.SendMessage( 37, "Faction statloss is {0}.", enabled );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        [Usage( "VerificaStatLossCittadino" )]
        [Description( "Ritorna un valore booleano che indica se lo statloss cittadino e' abilitato o no." )]
        public static void CheckTownSystemStatLoss_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null || e.Length != 0 )
                return;

            Mobile from = e.Mobile;

            try
            {
                string enabled = TownSystemStatLossEnabled ? "abilitato" : "disabilitato";
                from.SendMessage( 37, "Lo statloss cittadino e' {0}.", enabled );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        [Usage( "VerificaRegoleDiIngaggio" )]
        [Description( "Ritorna un valore che indica quali siano le regole di ingaggio per una data mappa." )]
        public static void CheckMapEngageRules_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile != null && e.Length == 0 )
                e.Mobile.SendMessage( "Sulla tua mappa ({0}) sono attive queste regole di ingaggio: {1}", e.Mobile.Map.Name, e.Mobile.Map.Rules.ToString() );
        }

        [Usage( "PersistanceProps" )]
        [Description( "Opens props gump for Midgard Persistance." )]
        public static void PersistanceProps_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile != null && e.Length == 0 )
                e.Mobile.SendGump( new Server.Gumps.PropertiesGump( e.Mobile, Instance ) );
        }
        #endregion
    }
}