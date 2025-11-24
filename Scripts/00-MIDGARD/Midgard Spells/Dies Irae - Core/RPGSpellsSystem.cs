/***************************************************************************
 *                                  RPGSpellsSystem.cs
 *                            		-------------------
 *  begin                	: Ottobre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Midgard.Engines.Classes;

using Server;
using Server.Commands;
using Server.Network;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class RPGSpellsSystem
    {
        private static readonly Dictionary<string, int> m_MatraDict = new Dictionary<string, int>();

        private static readonly List<int> IdsNecromancer = new List<int>();
        private static readonly List<int> IdsPaladins = new List<int>();
        private static readonly List<int> IdsDruids = new List<int>();

        public static bool SpeechEventEnabled = true;

        public static void Initialize()
        {
            if( SpeechEventEnabled )
                EventSink.Speech += new SpeechEventHandler( EventSink_OnCast );

            CommandSystem.Register( "GenerateSpellsInfo", AccessLevel.Developer, new CommandEventHandler( GenerateInfo_OnCommand ) );
            CommandSystem.Register( "ShowSpellRegistry", AccessLevel.Developer, new CommandEventHandler( ShowSpellRegistry_OnCommand ) );

            RPGSpellsInitializer.RegisterSpells();
        }

        [Usage( "GenerateSpellsInfo" )]
        [Description( "Generate the spell infos." )]
        private static void GenerateInfo_OnCommand( CommandEventArgs e )
        {
            GenerateInfo();

            Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( RPGPaladinSpell.Document ) );
        }

        private static void GenerateInfo()
        {
            foreach( KeyValuePair<string, int> keyValuePair in m_MatraDict )
            {
                int i = keyValuePair.Value;

                if( IsPaladineSpell( i ) )
                    GenerateInfoForSpell( i, "PaladinSpellInfos.txt" );
                else if( IsDruidSpell( i ) )
                    GenerateInfoForSpell( i, "DruidSpellInfos.txt" );
                else if( IsNecromanticSpell( i ) )
                    GenerateInfoForSpell( i, "NecromancersSpellInfos.txt" );
            }
        }

        internal static readonly string SaveDataPath = Path.Combine( "docs3c", "SpellSystem" );

        private static void GenerateInfoForSpell( int spellID, string fileName )
        {
            Spell spell = SpellRegistry.GetSpellByID( spellID );
            if( spell == null )
                return;

            string file = Path.Combine( SaveDataPath, fileName );

            if( !Directory.Exists( SaveDataPath ) )
                Directory.CreateDirectory( SaveDataPath );

            using( TextWriter tw = File.AppendText( file ) )
            {
                if( spell is RPGPaladinSpell )
                    tw.WriteLine( ( (RPGPaladinSpell)spell ).GetInfo() );
                else if( spell is RPGNecromancerSpell )
                    tw.WriteLine( ( (RPGNecromancerSpell)spell ).GetInfo() );
                else if( spell is DruidSpell )
                    tw.WriteLine( ( (DruidSpell)spell ).GetInfo() );
            }
        }

        [Usage( "ShowSpellRegistry" )]
        [Description( "Generate spellRegistry.log." )]
        private static void ShowSpellRegistry_OnCommand( CommandEventArgs e )
        {
            using( TextWriter tw = File.AppendText( "Logs/spellRegistry.log" ) )
            {
                foreach( KeyValuePair<int, Spell> kvp in SpellRegistry.SpellInstances )
                {
                    tw.WriteLine( "{0} - {1}", kvp.Key, kvp.Value.Name );
                }
            }
        }

        public static void LoadMidgardLocalization()
        {
            TextHelper.LoadLocalization( "PaladinSystem.cfg" );
        }

        public static void EventSink_OnCast( SpeechEventArgs args )
        {
            Mobile caster = args.Mobile;
            if( caster == null || !caster.Player || !caster.Alive )
                return;

            if( caster.Spell != null && caster.Spell.IsCasting )
            {
                caster.SendLocalizedMessage( 502642 ); // You are already casting a spell.
                return;
            }

            int spellID = GetSpellIdFromMantra( caster, args.Speech.Trim() );
            if( spellID <= -1 )
                return;

            CastSpellByID( spellID, caster );
            args.Blocked = true;
            args.Handled = true;
        }

        public static bool HandleClassMobileNotoriety( Mobile source, Mobile target, out int noto )
        {
            noto = -1;

            /*
            if( source.Player && target.Player )
            {
                // Il necro e' visto arancio dai paladini
                if( ClassSystem.IsPaladine( source ) && ClassSystem.IsNecromancer( target ) )
                    noto = Notoriety.Enemy;

                // Il paladino e' visto arancio dai necromanti
                if( ClassSystem.IsNecromancer( source ) && ClassSystem.IsPaladine( target ) )
                    noto = Notoriety.Enemy;
            }
            */

            return noto != -1;
        }

        public static int GetSpellIdFromMantra( Mobile caster, string mantra )
        {
            if( caster == null || mantra == null )
                return -1;

            int id;

            return m_MatraDict.TryGetValue( mantra.ToLower(), out id ) ? id : -1;
        }

        public static int[] GetIdsFromSystem( ClassSystem system )
        {
            if( system == ClassSystem.Paladin )
                return IdsPaladins.ToArray();
            else if( system == ClassSystem.Necromancer )
                return IdsNecromancer.ToArray();
            else if( system == ClassSystem.Druid )
                return IdsDruids.ToArray();
            else
                return new int[] { };
        }

        #region Is... something
        public static bool IsPaladineSpell( int spellID )
        {
            return IdsPaladins.Contains( spellID );
        }

        public static bool IsNecromanticSpell( int spellID )
        {
            return IdsNecromancer.Contains( spellID );
        }

        public static bool IsDruidSpell( int spellID )
        {
            return IdsDruids.Contains( spellID );
        }

        public static bool IsPaladineSpell( Type type )
        {
            return type.IsSubclassOf( typeof( RPGPaladinSpell ) );
        }

        public static bool IsNecromanticSpell( Type type )
        {
            return type.IsSubclassOf( typeof( RPGNecromancerSpell ) );
        }

        public static bool IsDruidSpell( Type type )
        {
            return type.IsSubclassOf( typeof( DruidSpell ) );
        }

        public static bool IsRestrictedSpell( int spellID )
        {
            return IsPaladineSpell( spellID ) || IsNecromanticSpell( spellID ) || IsDruidSpell( spellID );
        }
        #endregion

        public static void RegisterCustomSpell( int spellID, Type type )
        {
            SpellInfo info = SpellRegistry.GetSpellInfoByType( type );
            if( info == null )
                Console.WriteLine( "Error: SpellInfo null for id {0}", spellID );
            else
            {
                if( info.Mantra != null )
                {
                    if( m_MatraDict.ContainsKey( info.Mantra.ToLower() ) )
                        Console.WriteLine( "Error: m_MatraDict contains mantra {0}", info.Mantra );
                    else
                        m_MatraDict[ info.Mantra.ToLower() ] = spellID;
                }
            }

            if( IsPaladineSpell( type ) )
                IdsPaladins.Add( spellID );
            else if( IsNecromanticSpell( type ) )
                IdsNecromancer.Add( spellID );
            else if( IsDruidSpell( type ) )
                IdsDruids.Add( spellID );
        }

        public static bool CanSpellBeCastBy( Mobile caster, int spellID, bool message )
        {
            if( !IsRestrictedSpell( spellID ) )
                return true;

            ClassPlayerState state = ClassPlayerState.Find( caster );
            if( state == null )
                return false;

            Type t = SpellRegistry.GetTypeFromRegNumber( spellID );
            if( t == null )
                return false;

            if( !state.HasPower( t ) )
            {
                if( message )
                    caster.SendLangMessage( 10000001 ); // "Thou do not have this power."
                return false;
            }

            return true;
        }

        public static int GetSpellLabelHueBySpellID( Mobile caster, int spellID )
        {
            ClassPlayerState state = ClassPlayerState.Find( caster );
            if( state == null )
                return ClassSystem.DisabledLabelHue;

            Type t = SpellRegistry.GetTypeFromRegNumber( spellID );
            if( t == null )
                return ClassSystem.DisabledLabelHue;

            if( state.HasPower( t ) )
                return state.ClassSystem.GetSpellLabelHueByLevel( state.GetLevel( t ) );
            else
                return ClassSystem.DisabledLabelHue;
        }

        public static int GetSpellLevelBySpellID( Mobile caster, int spellID )
        {
            ClassPlayerState state = ClassPlayerState.Find( caster );
            if( state == null )
                return 0;

            Type t = SpellRegistry.GetTypeFromRegNumber( spellID );
            if( t == null )
                return 0;

            return state.HasPower( t ) ? state.GetLevel( t ) : 0;
        }

        public static void CastSpellByID( int spellID, Mobile caster )
        {
            CastSpellByID( spellID, caster, true, false );
        }

        public static void CastSpellByID( int spellID, Mobile caster, bool check, bool sayMantra )
        {
            if( caster == null )
                return;

            if( check && !CanSpellBeCastBy( caster, spellID, true ) )
                return;

            Spell spell = SpellRegistry.NewSpell( spellID, caster, null );

            if( spell == null )
                caster.SendLangMessage( 10000002 ); // "This spell has been temporarily disabled."
            else
            {
                if( sayMantra )
                    ForceSayMantra( caster, spell );
                spell.Cast();
            }
        }

        public static void ForceSayMantra( Mobile caster, Spell spell )
        {
            if( spell.Info == null || string.IsNullOrEmpty( spell.Info.Mantra ) || caster == null || caster.Map == null )
                return;

            IPooledEnumerable eable = caster.Map.GetClientsInRange( caster.Location );

            foreach( NetState state in eable )
            {
                if( state == null || state.Mobile == caster || !state.Mobile.CanSee( caster ) )
                    continue;

                caster.PrivateOverheadMessage( MessageType.Spell, caster.SpeechHue, true, Spell.PerCheck( state.Mobile.Int ) ? spell.Info.Mantra : "* You hear strange power words! *", state );
            }

            eable.Free();

            caster.LocalOverheadMessage( MessageType.Spell, caster.SpeechHue, true, spell.Info.Mantra );
        }

        public static int GetPowerLevel( Mobile caster, Type spellType )
        {
            ClassPlayerState playerState = ClassPlayerState.Find( caster );
            return playerState != null ? playerState.GetLevel( spellType ) : 0;
        }

        public static int GetTotalPowerLevel( Mobile caster )
        {
            ClassPlayerState playerState = ClassPlayerState.Find( caster );
            return playerState != null ? playerState.TotalPowers : 0;
        }

        public static int GetTotalPowerPercent( Mobile caster )
        {
            ClassPlayerState playerState = ClassPlayerState.Find( caster );
            return playerState != null ? playerState.TotalPowersPercent : 0;
        }

        public static bool IsAtCap( Mobile m, SkillName skill )
        {
            return m != null && ( m.Skills != null && ( m.Skills[ skill ].Value == m.Skills[ skill ].Cap ) );
        }
    }
}