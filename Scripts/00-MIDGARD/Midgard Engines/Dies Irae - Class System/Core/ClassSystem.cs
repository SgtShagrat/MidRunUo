using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public abstract class ClassSystem
    {
        #region instances
        public static readonly ClassSystem Druid = new DruidSystem();
        public static readonly ClassSystem Necromancer = new NecromancerSystem();
        public static readonly ClassSystem Paladin = new PaladinSystem();
        public static readonly ClassSystem Scout = new ScoutSystem();
        public static readonly ClassSystem Thief = new ThiefSystem();

        public static readonly ClassSystem[] ClassSystems = new ClassSystem[]
        {
            Druid,
            Paladin,
            Necromancer,
            Thief,
            Scout
        };
        #endregion

        public virtual int MessageHue { get { return 37; } }

        #region Is... something
        public static bool IsDruid( Mobile m )
        {
            ClassPlayerState state = ClassPlayerState.Find( m );
            return state != null && state.ClassSystem == Druid;
        }

        public static bool IsThief( Mobile m )
        {
            ClassPlayerState state = ClassPlayerState.Find( m );
            return state != null && state.ClassSystem == Thief;
        }

        public static bool IsNecromancer( Mobile m )
        {
            ClassPlayerState state = ClassPlayerState.Find( m );
            return state != null && state.ClassSystem == Necromancer;
        }

        public static bool IsPaladine( Mobile m )
        {
            ClassPlayerState state = ClassPlayerState.Find( m );
            return state != null && state.ClassSystem == Paladin;
        }

        public static bool IsUndead( Mobile m )
        {
            SlayerEntry undeadSlayer = SlayerGroup.GetEntryByName( SlayerName.Silver );
            if( undeadSlayer != null && undeadSlayer.Slays( m ) )
                return true;

            SlayerEntry demonSlayer = SlayerGroup.GetEntryByName( SlayerName.Exorcism );
            if( demonSlayer != null && demonSlayer.Slays( m ) )
                return true;

            return false;
        }

        public static bool IsScout( Mobile m )
        {
            ClassPlayerState state = ClassPlayerState.Find( m );
            return state != null && state.ClassSystem == Scout;
        }

        public static bool IsGoodOne( Mobile m )
        {
            return ( m != null && m.Karma > 0 );
        }

        public static bool IsEvilOne( Mobile m )
        {
            return ( m != null && m.Karma < 0 );
        }
        #endregion

        protected ClassSystem()
        {
            InitializeClassSystem();
        }

        public ClassDefinition Definition { get; protected set; }

        public static string DefaultWelcomeMessage
        {
            get { return "Benvenuto in questa classe.<br>"; }
        }

        public virtual bool SupportsRitualItems
        {
            get { return false; }
        }

        public static void RegisterEventSink()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( Load );
            EventSink.WorldSave += new WorldSaveEventHandler( Save );

            EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
        }

        public void InitializeClassSystem()
        {
            m_Players = new ClassPlayerStateCollection();
            m_Candidates = new List<Mobile>();
        }

        public static void EventSink_Speech( SpeechEventArgs args )
        {
            ClassSystem sys = Find( args.Mobile );
            if( sys != null && sys.HandlesOnSpeech )
                sys.OnSpeech( args );
        }

        public virtual bool HandlesOnSpeech { get { return false; } }

        public virtual void OnSpeech( SpeechEventArgs e )
        {
        }

        public virtual bool IsAllowedSkill( SkillName skillName )
        {
            return true;
        }

        public virtual void NullifyAndLockSkill( Mobile from, SkillName skillName )
        {
            from.Skills[ skillName ].Base = 0.0;
            from.Skills[ skillName ].SetLockNoRelay( SkillLock.Locked );
            from.Skills[ skillName ].Cap = 0.0;
        }

        public virtual void UnlockSkillAndCap( Mobile from, SkillName skillName )
        {
            from.Skills[ skillName ].SetLockNoRelay( SkillLock.Up );
            from.Skills[ skillName ].Cap = 100.0;
        }

        public virtual ClassSkillMod[] GetSkillBonuses()
        {
            return SkillBonuses.EmptyList;
        }

        public abstract void MakeRitual( Mobile ritualist, PowerDefinition definition );

        public void SendOverheadMessage( Mobile sender, string message )
        {
            sender.PublicOverheadMessage( MessageType.Regular, MessageHue, true, message );
        }

        public virtual RitualItem RandomRitualItem()
        {
            return null;
        }

        public virtual bool CanEquipWeapon( Mobile from, BaseWeapon weapon, bool message )
        {
            return true;
        }

        public static bool CheckEquip( Mobile from, ClassSystem system, bool message )
        {
            if( Find( from ) != system )
            {
                if( message )
                    from.SendMessage( "Only a {0} can wear this.", system.Definition.ClassName );

                return false;
            }
            else
                return true;
        }

        #region other
        public static int DisabledLabelHue = Colors.DimGray;

        public virtual bool IsMurdererClass
        {
            get { return false; }
        }

        public virtual bool IsEvilAlignedClass
        {
            get { return false; }
        }

        public virtual bool IsGoodAlignedClass
        {
            get { return false; }
        }

        public override string ToString()
        {
            return Definition.ClassName;
        }

        public static ClassSystem Parse( string name )
        {
            if( String.IsNullOrEmpty( name ) )
                return null;

            foreach( ClassSystem t in ClassSystems )
            {
                if( t.Definition.ClassName == name )
                    return t;
            }

            foreach( ClassSystem t in ClassSystems )
            {
                if( Utility.InsensitiveCompare( Enum.GetName( typeof( MidgardClasses ), t.Definition.Class ), name ) == 0 )
                    return t;
            }

            Config.Pkg.LogInfoLine( "Warning: Null Parse in ClassSystem.Parse. Name: {0}", name );

            return null;
        }

        public int CompareTo( object obj )
        {
            return Insensitive.Compare( Definition.ClassName, ( (ClassSystem)obj ).Definition.ClassName );
        }

        public static ClassSystem Find( Mobile mob )
        {
            return Find( mob, false, false );
        }

        public static ClassSystem Find( Mobile mob, bool inherit )
        {
            return Find( mob, inherit, false );
        }

        public static ClassSystem Find( Mobile mob, bool inherit, bool allegiance )
        {
            ClassPlayerState pl = ClassPlayerState.Find( mob );

            if( pl != null )
                return pl.ClassSystem;

            if( mob is BaseClassGiver )
                return ( (BaseClassGiver)mob ).System;

            if( inherit && mob is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)mob;

                if( bc.Controlled )
                    return Find( bc.ControlMaster, false );
                else if( bc.Summoned )
                    return Find( bc.SummonMaster, false );
            }

            return null;
        }

        public static ClassSystem Find( MidgardClasses classToFind )
        {
            foreach( ClassSystem t in ClassSystems )
            {
                if( t.Definition.Class == classToFind )
                    return t;
            }

            return null;
        }

        public virtual int GetSpellLabelHueByLevel( int level )
        {
            switch( level )
            {
                case 0:
                    return DisabledLabelHue;
                case 1:
                    return Colors.YellowGreen;
                case 2:
                    return Colors.Yellow;
                case 3:
                    return Colors.GreenYellow;
                case 4:
                    return Colors.Darkorange;
                case 5:
                    return Colors.OrangeRed;
                default:
                    return DisabledLabelHue;
            }
        }

        public virtual bool IsEligible( Mobile mob )
        {
            return ( mob != null && mob.Player );
        }

        public virtual string IsEligibleString( Mobile mob )
        {
            return ( mob.Language == "ITA" ? "Non puoi far parte della nostra congrega." : "Thou are not eligible to become a member of this congregation." );
        }

        public virtual void OnClassSystemInitialized()
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "ClassSystem of {0} initializing...", Definition.ClassName );

            VerifyPlayerStates();

            if( Config.Debug )
                Config.Pkg.LogInfo( "done\n" );
        }

        public virtual void OnClassSystemJoined( ClassPlayerState tps )
        {
            #region check sulla gilda
            if( tps.Mobile != null )
            {
                Account a = tps.Mobile.Account as Account;
                if( a != null )
                {
                    // TODO Verify?
                }
            }
            #endregion
        }
        #endregion

        #region serial-deserial
        public static void Save( WorldSaveEventArgs e )
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "{0}: Saving...", Config.Pkg.Title );

            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                string dir = Path.Combine( Path.GetPathRoot( Config.ClassSystemSavePath ), Path.GetDirectoryName( Config.ClassSystemSavePath ) );
                if( !Directory.Exists( dir ) )
                    Directory.CreateDirectory( dir );

                BinaryFileWriter writer = new BinaryFileWriter( Config.ClassSystemSavePath, true );

                writer.Write( 0 ); // version

                writer.Write( ClassSystems.Length );

                foreach( ClassSystem system in ClassSystems )
                    system.Serialize( writer );

                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogInfoLine( "Error serializing {0}.", Config.Pkg.Title );
                Config.Pkg.LogInfoLine( ex.ToString() );
            }

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        public static void Load()
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "{0}: Loading...", Config.Pkg.Title );

            while( !File.Exists( Config.ClassSystemSavePath ) )
            {
                Config.Pkg.LogInfoLine( "Warning: {0} not found.", Config.ClassSystemSavePath );
                Config.Pkg.LogInfoLine( " - Press return to continue, or R to try again." );
                string str = Console.ReadLine();

                if( str == null || str.ToLower() != "r" )
                    return;
            }

            try
            {
                BinaryReader bReader = new BinaryReader( File.OpenRead( Config.ClassSystemSavePath ) );
                BinaryFileReader reader = new BinaryFileReader( bReader );

                int version = reader.ReadInt();

                switch( version )
                {
                    case 0:
                        {
                            int count = reader.ReadInt();

                            for( int i = 0; i < count; ++i )
                                ClassSystems[ i ].Deserialize( reader );

                            break;
                        }
                }

                bReader.Close();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogInfoLine( "Error de-serializing {0}.", Config.Pkg.Title );
                Config.Pkg.LogInfoLine( ex.ToString() );
            }

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        public virtual void Deserialize( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 1:
                    {
                        m_Candidates = reader.ReadStrongMobileList();
                        goto case 0;
                    }
                case 0:
                    {
                        int playerCount = reader.ReadEncodedInt();

                        for( int i = 0; i < playerCount; ++i )
                        {
                            ClassPlayerState state = IstantiateState();
                            state.Deserialize( this, reader );
                            state.Attach( false );
                            state.Invalidate();
                        }

                        break;
                    }
            }

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( OnClassSystemInitialized ) );
        }

        public virtual void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 1 ); // version

            // Version 1
            writer.Write( m_Candidates, true );

            // Version 0
            writer.WriteEncodedInt( m_Players.Count );

            foreach( ClassPlayerState playerState in m_Players )
                playerState.Serialize( writer );
        }

        public static void WriteReference( GenericWriter writer, ClassSystem sys )
        {
            int idx = Array.IndexOf( ClassSystems, sys );

            writer.WriteEncodedInt( idx + 1 );
        }

        public static ClassSystem ReadReference( GenericReader reader )
        {
            int idx = reader.ReadEncodedInt() - 1;

            if( idx >= 0 && idx < ClassSystems.Length )
                return ClassSystems[ idx ];

            return null;
        }
        #endregion

        #region player states
        public virtual ClassPlayerState IstantiateState( Mobile m )
        {
            return new ClassPlayerState( this, m );
        }

        public virtual ClassPlayerState IstantiateState()
        {
            return new ClassPlayerState();
        }

        private List<Mobile> m_Candidates;
        protected ClassPlayerStateCollection m_Players;

        public ClassPlayerStateCollection Players
        {
            get { return m_Players; }
        }

        public List<Mobile> Candidates
        {
            get { return m_Candidates; }
        }

        public virtual bool ShowsTitle
        {
            get { return true; }
        }

        public bool IsCandidate( Mobile from )
        {
            return m_Candidates != null && m_Candidates.Contains( from );
        }

        public void AddCandidate( Mobile m )
        {
            if( m_Candidates != null && m_Candidates.Contains( m ) )
                return;

            if( m_Candidates == null )
                m_Candidates = new List<Mobile>();

            m_Candidates.Add( m );
        }

        public void ClearInactiveStates()
        {
            List<ClassPlayerState> list = GetInactiveStates( false );

            foreach( ClassPlayerState state in list )
            {
                if( state == null || !state.IsInactive )
                    continue;

                Account a = state.Mobile.Account as Account;

                if( a != null )
                {
                    // TODO
                    // ClassHelper.DoAccountReset( a );
                }
            }
        }

        public static int GetTotalClassedInAllSystems()
        {
            int counter = 0;
            foreach( ClassSystem t in ClassSystems )
            {
                if( t != null && t.Players != null )
                    counter += t.Players.Count;
            }

            return counter;
        }

        public List<ClassPlayerState> GetInactiveStates( bool sort )
        {
            List<ClassPlayerState> list = new List<ClassPlayerState>();

            if( Players != null )
            {
                foreach( ClassPlayerState state in Players )
                {
                    if( state.IsInactive )
                        list.Add( state );
                }
            }

            if( sort )
                list.Sort();

            return list;
        }

        public int GetInactiveStatesCount()
        {
            int counter = 0;
            foreach( ClassPlayerState state in Players )
            {
                if( state.IsInactive )
                    counter++;
            }

            return counter;
        }

        public List<Mobile> GetMobilesFromStates()
        {
            List<Mobile> list = new List<Mobile>();

            foreach( ClassPlayerState tps in Players )
            {
                if( tps.Mobile != null )
                    list.Add( tps.Mobile );
            }

            return list;
        }

        public void RemoveMobileFromState( Mobile toremove )
        {
            foreach( ClassPlayerState tps in Players )
            {
                if( tps.Mobile == toremove )
                {
                    tps.Detach();
                    break;
                }
            }
        }

        public static List<ClassPlayerState> GetAllInactiveStates( bool sort )
        {
            List<ClassPlayerState> list = new List<ClassPlayerState>();

            foreach( ClassSystem t in ClassSystems )
            {
                list.AddRange( t.GetInactiveStates( false ) );
            }

            if( sort )
                list.Sort();

            return list;
        }

        public static List<ClassPlayerState> GetPlayerStatesOnServer( bool sort )
        {
            List<ClassPlayerState> list = new List<ClassPlayerState>();

            foreach( ClassSystem t in ClassSystems )
            {
                if( t.Players != null )
                    list.AddRange( t.Players );
            }

            if( sort )
                list.Sort();

            return list;
        }

        public virtual void VerifyPlayerStates()
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "Verifying {0} class player states...", Definition.ClassName );

            foreach( ClassPlayerState state in Players )
            {
                if( state != null )
                {
                    // TODO Verify?
                }
            }

            if( Config.Debug )
                Config.Pkg.LogInfo( "done\n" );
        }
        #endregion

        public virtual void SetStartingSkills( Mobile mobile )
        {
        }

        public virtual BaseClassAttributes GetNewPowerAttributes( ClassPlayerState state )
        {
            return null;
        }

        public virtual void ResetSkillsLocks( Mobile mobile )
        {
        }

        public const double MaxRitualDropChance = 0.20; // 20%

        public static double GetRitualItemDropChance( BaseCreature bc, Mobile m )
        {
            double hpBonus = Math.Min( 1.00, bc.HitsMax / 500.0 );
            double famaBonus = Math.Min( 1.00, bc.Fame / 15000.0 );

            return ( ( hpBonus + famaBonus ) / 2.0 ) * MaxRitualDropChance;
        }

        public static void HandleCorpseDrop( BaseCreature bc, Container c, Mobile m )
        {
            if( m == null || m.Deleted )
                return;

            if( bc == null || bc.Summoned || bc.Controlled )
                return;

            double chance = GetRitualItemDropChance( bc, m );
            if( chance < 0.03 )
                chance = 0.03;

            if( m.PlayerDebug )
                m.SendMessage( "Debug ClassSystem.HandleCorpseDrop: chance - {0:F3}", chance );

            if( IsPaladine( m ) )
                m.SendMessage( "Hai una chance di {0:F2}% di ottenere unoggetto rituale.", chance * 100 );

            if( Utility.RandomDouble() < chance )
            {
                ClassSystem sys = Find( m );
                if( sys != null && sys.SupportsRitualItems )
                {
                    RitualItem item = sys.RandomRitualItem();
                    if( item != null )
                    {
                        c.DropItem( BaseCreature.MakeInstanceOwner( item, m ) );
                        m.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, ( m.Language == "ITA" ? "*Hai trovato un oggetto rituale nel cadavere*" : "*You found a ritual item in the corpse*" ) );
                    }
                }
            }
        }
    }
}