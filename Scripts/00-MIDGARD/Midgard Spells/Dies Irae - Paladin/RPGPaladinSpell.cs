/***************************************************************************
 *                                  RPGPaladinSpell.cs
 *                            		------------------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Midgard.Engines.Classes;
using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public abstract class RPGPaladinSpell : Spell, ICustomSpell
    {
        private static readonly Type[] SpellTypes = new Type[]
                                              {
                                                    typeof(BanishEvilSpell),
                                                    typeof(BlessedDropsSpell),
                                                    typeof(HolyMountSpell),
                                                    typeof(HolyWillSpell),
                                                    typeof(InvulnerabilitySpell),
                                                    typeof(HolySmiteSpell),
                                                    typeof(PathToHeavenSpell),
                                                    typeof(SacredBeamSpell),
                                                    typeof(SacredFeastSpell),
                                                    typeof(ShieldOfRighteousnessSpell),
                                                    typeof(SwordOfLightSpell),
                                                    typeof(LayOfHandsSpell),
                                                    typeof(CurePoisonSpell),
                                                    typeof(ChalmChaosSpell),
                                                    typeof(HolyCircleSpell),
                                                    typeof(LegalThoughtsSpell)
                                            };

        #region debug
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler( OnLogin );

            EventSink.Crashed += new CrashedEventHandler( HolyMountSpell.EventSink_Crashed );
            EventSink.Shutdown += new ShutdownEventHandler( HolyMountSpell.EventSink_Shutdown );

            InvulnerabilitySpell.RegisterOnLoginEvent();
        }

        private static readonly List<string> m_ChangeLog = new List<string>();

        public static void RegisterChange( string change )
        {
            m_ChangeLog.Add( change );
        }

        private static string Red( string toColor )
        {
            return string.Format( "<basefont color=red>{0}</basefont>", toColor );
        }

        /*
        private static string Em( string toColor )
        {
            return string.Format( "<em>{0}</em>", toColor );
        }
        */

        private static void OnLogin( LoginEventArgs e )
        {
            /*
            if( ClassSystem.IsPaladine( e.Mobile ) )
                e.Mobile.SendGump( new NoticeGump( 1060637, 30720, GetChangeLog(), 0xFFC000, 420, 280, new NoticeGumpCallback( CloseNotice_Callback ), null ) );
             */
        }

        private static string GetChangeLog()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat( "Il sistema del <em>paladino</em> e' in test.<br>" +
                "Molti spell sono stati modificati per uso, potenza, meccaniche, efficacia.<br>" +
                "Ciò significa che è severamente vietato abusare degli spell in test per farmare denaro, oggetti o quantaltro ovvero per " +
                "importunare il gioco altrui.<br> Ogni abuso verrà <b>severamente</b> punito.<br> Per ogni segnalazione fate riferimento " +
                "al forum collaboratori, allo scripter che sta seguendo la revisione (Dies Irae), agli admin e ai seer.<br>" +
                "E' severamente vietato postare commenti sulle spell in test sui forum pubblici (pena ban).<br><br>" +
                "Buon test.<br> Di seguito vengono elencate le modifiche apportate:<br>" );

            foreach( Type type in SpellRegistry.Types )
            {
                Spell spell = SpellRegistry.GetSpellByType( type );

                if( spell == null || !( spell is RPGPaladinSpell ) )
                    continue;

                string mod = ( (RPGPaladinSpell)spell ).GetModificationsForChangelog();
                if( string.IsNullOrEmpty( mod ) )
                    continue;

                sb.AppendFormat( string.Format( "{0}</br>", Red( spell.Name ) ) );
                sb.AppendFormat( mod );

                if( !mod.EndsWith( "<br>" ) )
                    sb.AppendFormat( "<br>" );
            }

            return sb.ToString();
        }

        private static void CloseNotice_Callback( Mobile from, object state )
        {
        }

        public virtual string GetModificationsForChangelog()
        {
            return string.Empty;
        }

        internal static void Document()
        {
            string file = Path.Combine( RPGSpellsSystem.SaveDataPath, "paladinStaff.log" );

            if( !Directory.Exists( RPGSpellsSystem.SaveDataPath ) )
                Directory.CreateDirectory( RPGSpellsSystem.SaveDataPath );

            using( TextWriter tw = File.AppendText( file ) )
            {
                foreach( Type type in SpellTypes )
                {
                    Spell spell = SpellRegistry.GetSpellByType( type );
                    tw.WriteLine( ( (RPGPaladinSpell)spell ).GetStaffInfo() );
                }

                tw.WriteLine( "" );
                tw.WriteLine( "" );

                foreach( Type type in SpellTypes )
                {
                    Spell spell = SpellRegistry.GetSpellByType( type );
                    tw.WriteLine( "typeof({0})", spell.GetType().Name );
                }

                tw.WriteLine( "" );
                tw.WriteLine( "" );

                foreach( Type type in SpellTypes )
                {
                    Spell spell = SpellRegistry.GetSpellByType( type );

                    string mod = ( (RPGPaladinSpell)spell ).GetModificationsForChangelog();
                    if( !string.IsNullOrEmpty( mod ) )
                    {
                        tw.WriteLine( "[b]{0}[/b]", spell.Name );
                        tw.WriteLine( mod.Replace( "<br>", "\n" ) );
                        tw.WriteLine( "" );
                    }
                }

                tw.WriteLine( "" );
                tw.WriteLine( "" );

                foreach( Type type in SpellTypes )
                {
                    Spell spell = SpellRegistry.GetSpellByType( type );

                    tw.WriteLine( ( (RPGPaladinSpell)spell ).GetCommaSeparatedInfo() );
                }

                tw.WriteLine( "" );
                tw.WriteLine( "" );

                tw.WriteLine( GetChangeLog() );
            }
        }
        #endregion

        protected const int MessageHue = 0x22;

        protected RPGPaladinSpell( Mobile caster, Item scroll, SpellInfo info )
            : base( caster, scroll, info )
        {
        }

        private static readonly int[] m_ManaTable = new int[] { 10, 15, 20 };

        private static readonly int[] m_TihtesTable = new int[] { 10, 20, 30 };

        private static readonly int[] m_SkillsTable = new int[] { 30, 50, 70 };

        private static readonly double[] m_RecoveryTable = new double[] { 2.0, 2.5, 3.0 };

        public abstract SpellCircle Circle { get; }

        public override SkillName CastSkill
        {
            get { return SkillName.Chivalry; }
        }

        public override SkillName DamageSkill
        {
            get { return SkillName.Chivalry; }
        }

        public override bool ClearHandsOnCast
        {
            get { return false; }
        }

        public override bool RevealOnCast { get { return true; } }
        public override bool DisruptOnCast { get { return true; } }
        public override bool ShowHandMovement { get { return true; } }

        public override TimeSpan CastDelayBase
        {
            get
            {
                // First  = 2.00s
                // Second = 2.50s
                // Third  = 3.00s
                double delay = 2.0 + ( (int)Circle ) * 0.50;

                if( IsEligibleForCastBonus )
                    delay -= CastDelayBonus;

                if( delay < 0.5 )
                    delay = 0.5;

                return TimeSpan.FromSeconds( delay );
            }
        }

        public override int CastRecoveryBase
        {
            get { return 6; }
        }

        public override TimeSpan GetCastRecovery()
        {
            // actually 6 / 4 ? 1,5 secondi
            // return TimeSpan.FromSeconds( (double)CastRecoveryBase / CastRecoveryPerSecond );

            if( (int)Circle > m_RecoveryTable.Length || Circle < 0 )
                return TimeSpan.FromSeconds( (double)CastRecoveryBase / CastRecoveryPerSecond );
            else
                return TimeSpan.FromSeconds( m_RecoveryTable[ (int)Circle ] + ( GetPowerLevel() * 0.20 ) );
        }

        #region ICustomSpell Members
        public abstract ExtendedSpellInfo ExtendedInfo { get; }

        public SchoolFlag SpellSchool
        {
            get { return SchoolFlag.Paladin; }
        }

        public virtual double RequiredSkill
        {
            get { return GetRequiredSkills(); }
        }
        #endregion

        #region spell info
        public string GetInfo()
        {
            ExtendedSpellInfo ei = ExtendedInfo;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine( Name );
            stringBuilder.AppendLine( string.Format( "ID: {0}", SpellRegistry.GetRegistryNumber( this ) ) );
            stringBuilder.AppendLine( string.Format( "Mantra: {0}", Mantra ) );
            stringBuilder.AppendLine( string.Format( "RequiredMana: {0}", GetMana() ) );
            stringBuilder.AppendLine( string.Format( "DelayOfReuse: {0}", DelayOfReuseInSeconds ) );
            stringBuilder.AppendLine( string.Format( "RequiredSkill: {0}", RequiredSkill ) );
            stringBuilder.AppendLine( string.Format( "BlocksMovement: {0}", BlocksMovement ) );
            stringBuilder.AppendLine( string.Format( "RequiredTithing: {0}", GetTithes() ) );
            stringBuilder.AppendLine( string.Format( "Description: {0}", ei.Description ) );
            stringBuilder.AppendLine( string.Format( "Description Ita: {0}", ei.DescriptionIta ) );
            stringBuilder.AppendLine( string.Format( "Description Staff: {0}", ei.DescriptionStaff ?? "" ) );
            stringBuilder.AppendLine( string.Format( "SpellIcon: {0}", ei.SpellIcon ) );
            stringBuilder.AppendLine( string.Format( "Reagents: {0}", ei.Reagents ) );
            stringBuilder.AppendLine( "" );

            return stringBuilder.ToString();
        }

        public string GetStaffInfo()
        {
            ExtendedSpellInfo ei = ExtendedInfo;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine( string.Format( "[b]{0}[/b]", Name ) );
            stringBuilder.AppendLine( string.Format( "[i]Mantra / Faith:[/i] {0} / {1}", Mantra, RequiredSkill ) );
            stringBuilder.AppendLine( string.Format( "[i]Mana / Tithes:[/i] {0} / {1}", GetBaseMana(), GetTithes() ) );
            stringBuilder.AppendLine( string.Format( "[i]Cast delay / Delay of reuse:[/i] {0} / {1}", CastDelayBase.TotalSeconds, DelayOfReuseInSeconds.ToString( "F0" ) ) );
            stringBuilder.AppendLine( string.Format( "[i]Blocks Movement / Clear hands:[/i] {0} / {1}", BlocksMovement, ClearHandsOnCast ) );
            stringBuilder.AppendLine( string.Format( "[i]Description:[/i] {0}", ei.DescriptionStaff ?? "" ) );
            stringBuilder.AppendLine( "" );

            return stringBuilder.ToString();
        }

        public string GetCommaSeparatedInfo()
        {
            ExtendedSpellInfo ei = ExtendedInfo;

            return string.Format( "{0}, {1}, {2}, {3}, {4}, {5}, \"{6}\", \"{7}\", {8}, {9}, {10}",
                                  Name, Mantra, Circle, RequiredSkill, GetMana(), GetTithes(), CastDelayBase.TotalSeconds, DelayOfReuseInSeconds.ToString( "F0" ), BlocksMovement, ClearHandsOnCast, ei.DescriptionStaff ?? "" );
        }
        #endregion

        #region resist
        public SkillName ResistSkill
        {
            get { return SkillName.SpiritSpeak; }
        }

        public virtual double GetResistPercent( Mobile target )
        {
            return GetResistPercentForLevel( target, GetPowerLevel() );
        }

        public double GetResistPercentForLevel( Mobile target, int level )
        {
            double res = target.Skills[ ResistSkill ].Value;
            double chi = Caster.Skills[ CastSkill ].Value;

            double firstPercent = res / 5.0;
            double secondPercent = res - ( ( ( chi - 20.0 ) / 5.0 ) + ( 1 + level ) * 5.0 );

            return ( firstPercent > secondPercent ? firstPercent : secondPercent );
        }

        private const int ResistOffsetA = 16;
        private const int ResistOffsetB = 25;

        public bool CheckResisted( Mobile target )
        {
            double n = GetResistPercent( target );

            n /= 100.0;

            if( n <= 0.0 )
                return false;

            if( n >= 1.0 )
                return true;

            int level = GetPowerLevel();

            int maxSkill = ( 1 + level ) * ResistOffsetA;
            maxSkill += ( 1 + ( level / 6 ) ) * ResistOffsetB;

            if( Caster.PlayerDebug )
                Caster.SendMessage( "Debug: Max respell {0:F2}", maxSkill );

            bool isCreatureVsPlayer = Caster is BaseCreature && target is PlayerMobile;

            if( target.Skills[ ResistSkill ].Value < maxSkill )
                target.CheckSkill( ResistSkill, 0.0, 120 + ( isCreatureVsPlayer ? 20 : 0 ) );

            bool success = n >= Utility.RandomDouble();

            if( success && target.Player )
            {
                if( Caster != target && Caster != null )
                    Caster.SendMessage( "Your enemy resisted the spell!" );

                target.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
                target.PlaySound( 0x1EA );
            }

            return success;
        }

        public override double GetResistScalar( Mobile target )
        {
            double resSkill = target.Skills[ ResistSkill ].Value;
            double castSkill = Caster.Skills[ CastSkill ].Value;

            // resValue: from -50 to 100
            double resValue = 0.5 * ( 2 * castSkill - resSkill );

            // perc: from 150 to 0
            double perc = 100.0 - resValue;

            perc = perc / 100.0;

            // sanity
            if( perc > 1.0 )
                perc = 1.0;
            else if( perc < 0.0 )
                perc = 0.0;

            if( Caster.PlayerDebug )
                Caster.SendMessage( "Debug GetResistScalar: resist {0:F2} - cast {1:F2} - final scalar {2:F2}", resSkill, castSkill, perc );

            return perc;
        }
        #endregion

        #region cast bonus
        public bool IsEligibleForCastBonus
        {
            get
            {
                if( Caster != null )
                    return Caster.Skills[ SkillName.Chivalry ].Value >= 90.0;

                return false;
            }
        }

        public virtual double CastDelayBonus
        {
            get { return 0.1 * GetPowerLevel(); }
        }

        public virtual int CastManaBonus
        {
            get { return GetPowerLevel(); }
        }
        #endregion

        public static int HolyHue = 0x920;

        public static int HolyHueAtCap = 0x47E;

        public static int HolyHueForElves = 0x6DB;

        public static int GetMeditationPoints( Mobile m )
        {
            if( m == null )
                return 0;

            double stats = Math.Min( m.Int + m.Str + m.Dex, 225 ) / 225.0;
            double levs = RPGSpellsSystem.GetTotalPowerPercent( m ) / 100.0;
            double karma = Math.Min( m.Karma, 10000.0 ) / 10000.0;
            double faith = Math.Min( m.Skills[ SkillName.Chivalry ].Value, 100.0 ) / 100.0;

            double scalar = Math.Min( ( stats + levs + karma + faith ) / 4.0, 1.0 );

            // Console.WriteLine( "{0:F2} - {1:F2} - {2:F2} - {3:F2} - {4:F2}", stats, levs, karma, faith, scalar );

            return (int)( scalar * 100 );
        }

        public override bool CheckCast()
        {
            if( !base.CheckCast() )
                return false;

            if( Caster.Skills[ CastSkill ].Value < RequiredSkill )
            {
                Caster.SendMessage( "Thou must have at least " + RequiredSkill + " Faith to invoke this power." );
                return false;
            }
            else if( Caster.TithingPoints < GetTithes() )
            {
                Caster.SendMessage( "Thou must have at least " + GetTithes() + " Tithe to invoke this power." );
                return false;
            }
            else if( Caster.Mana < ScaleMana( GetMana() ) )
            {
                Caster.SendMessage( "Thou must have at least " + GetMana() + " Mana to invoke this praryer." );
                return false;
            }

            return true;
        }

        public override bool CheckFizzle()
        {
            int mana = ScaleMana( GetMana() );

            double min, max;
            GetCastSkills( out min, out max );

            if( Caster.Skills[ CastSkill ].Value < RequiredSkill )
            {
                Caster.SendMessage( "You must have at least " + RequiredSkill + " Faith to invoke this power." );
                return false;
            }
            else if( Caster.TithingPoints < GetTithes() )
            {
                Caster.SendMessage( "You must have at least " + GetTithes() + " Tithe to invoke this power." );
                return false;
            }
            else if( Caster.Mana < mana )
            {
                Caster.SendMessage( "You must have at least " + mana + " Mana to invoke this praryer." );
                return false;
            }
            else
            {
                if( DamageSkill != CastSkill )
                    Caster.CheckSkill( DamageSkill, min, max );

                /*
                double skvalue = Caster.Skills[ CastSkill ].Value;
                bool success = Utility.Random( 101 ) < skvalue;
                double rawPercent = ( min > 0 ? ( 1 - ( min / 100 ) ) : 0.90 );

                if ( skvalue < max )
                    Caster.CheckSkill( CastSkill, ( success ? rawPercent : ( rawPercent + 1 ) / 2 ) );

                if ( Caster != null && Caster.PlayerDebug )
                    Caster.SendMessage( "Skills: m {0:F2} - M {1:F2} -- skill {2:F2} -- checkskill success {3}", min,
                                        max, Caster.Skills[ CastSkill ].Value, success );
                */

                bool success = Caster.CheckSkill( CastSkill, min, max );

                if( Caster != null && Caster.PlayerDebug )
                    Caster.SendMessage( "Skills: m {0:F2} - M {1:F2} -- skill {2:F2} -- checkskill success {3}", min, max, Caster.Skills[ CastSkill ].Value, success );

                return success;
            }
        }

        public override bool ConsumeReagents()
        {
            int tithing = GetTithes();
            if( Caster.TithingPoints >= tithing )
            {
                Caster.TithingPoints -= tithing;
                return true;
            }

            return false;
        }

        public virtual int DelayOfReuseInSeconds
        {
            get { return 0; }
        }

        public override void DoFizzle()
        {
            Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "You fail to invoke the Virtue Power" );
            Caster.PlaySound( 0x1D6 );
            Caster.NextSpellTime = DateTime.Now;
        }

        public override void DoHurtFizzle()
        {
            Caster.FixedEffect( 0x3735, 6, 30 );
            Caster.PlaySound( 0x1D6 );
        }

        public override void GetCastSkills( out double min, out double max )
        {
            int offset = 5 * ( GetPowerLevel() - 1 );

            min = offset + RequiredSkill - 25.0;
            max = offset + RequiredSkill + 25.0;
        }

        public TimeSpan GetDelayOfReuseInSeconds()
        {
            return TimeSpan.FromSeconds( DelayOfReuseInSeconds );
        }

        public virtual int GetBaseMana()
        {
            return m_ManaTable[ (int)Circle ];
        }

        public override int GetMana()
        {
            int mana = GetBaseMana();

            if( IsEligibleForCastBonus )
                mana -= CastManaBonus;

            if( mana < 1 )
                mana = 1;

            return mana;
        }

        public int GetPowerLevel()
        {
            ClassPlayerState playerState = ClassPlayerState.Find( Caster );
            return playerState != null ? playerState.GetLevel( GetType() ) : 0;
        }

        public static int GetPowerLevelByType( Mobile caster, Type t )
        {
            ClassPlayerState playerState = ClassPlayerState.Find( caster );
            return playerState != null ? playerState.GetLevel( t ) : 0;
        }

        public virtual int GetTithes()
        {
            int tithes = m_TihtesTable[ (int)Circle ];

            if( tithes < 1 )
                tithes = 1;

            return tithes;
        }

        public virtual int GetRequiredSkills()
        {
            int skills = m_SkillsTable[ (int)Circle ];

            if( skills < 1 )
                skills = 1;

            return skills;
        }

        public override void OnDisturb( DisturbType type, bool message )
        {
            base.OnDisturb( type, message );

            if( message )
                Caster.PlaySound( 0x1D6 );
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            SendCastEffect();
        }

        public override void OnCasterDamaged( Mobile from, int damage )
        {
            double difficulty = ( (double)Circle * 100.0 ) / 2.0;
            double skill = Caster.Skills[ CastSkill ].Value;
            double chance = ( skill - ( difficulty + ( damage * 2.0 ) ) ) / 100.0;

            // return true means disrupt resisted
            bool resist = Utility.RandomDouble() < chance;

            if( Caster.PlayerDebug )
                Caster.SendMessage( "Debug PaladinSpell: chance to resist disruption: {0}% - successed: {1}", chance * 100, resist );

            if( !resist )
                OnCasterHurt();
        }

        public override void SayMantra()
        {
        }

        public virtual void SendCastEffect()
        {
            Caster.FixedEffect( 0x37C4, 10, 42, 4, 3 );
        }

        public static bool IsSuperVulnerable( Mobile m )
        {
            return ( ClassSystem.IsUndead( m ) || ClassSystem.IsNecromancer( m ) || ClassSystem.IsEvilOne( m ) );
        }

        public static bool IsImmune( Mobile m )
        {
            return ( ClassSystem.IsGoodOne( m ) || ClassSystem.IsPaladine( m ) );
        }

        public virtual bool IsEnemy( Mobile target )
        {
            return IsEnemy( Caster, target );
        }

        /// <summary>
        /// Funzione per vlautare se il target m è un nemico valido per il paladino
        /// </summary>
        public static bool IsEnemy( Mobile from, Mobile m )
        {
            if( m == null || from == null || m == from )
                return false;

            // esclusi a priori praticolari tipi di creature
            if( m is SilverSteed || m is HolyMount || m is BaseGuard || m is BaseVendor || m is BaseTownSoldier )
                return false;

            // non si possono colpire innocenti in città
            if( SpellHelper.IsTown( m.Location, from ) && from.IsHarmfulCriminal( m ) )
                return false;

            // summon e pet sono nemici se il loro master è nemico
            if( m is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)m;
                if( bc.AlwaysMurderer || bc.AlwaysAttackable )
                    return true;

                Mobile master = bc.GetMaster();
                if( master != null )
                    return IsEnemy( from, master );
            }

            // il party è sempre salvo
            if( from.Party is Party )
            {
                if( ( (Party)from.Party ).Contains( m ) )
                    return false;
            }

            // i gildati alleati sono salvi, i nemici di gilda sono targettati
            Guild fromGuild = from.Guild as Guild;
            Guild targetGuild = m.Guild as Guild;

            if( fromGuild != null && targetGuild != null )
            {
                if( targetGuild == fromGuild || fromGuild.IsAlly( targetGuild ) )
                    return false;

                if( fromGuild.IsEnemy( targetGuild ) || fromGuild.IsTempEnemy( m ) )
                    return true;
            }

            // aggressori, giocatori con karma negativo o kills sono sempre validi nemici
            return IsAggressor( from, m ) || m.Karma < 0 || m.Kills > 0;
        }

        public virtual bool IsAlly( Mobile target )
        {
            return IsAlly( Caster, target );
        }

        /// <summary>
        /// Funzione per vlautare se il target m è un alleato valido per il paladino
        /// </summary>
        public static bool IsAlly( Mobile from, Mobile m )
        {
            if( m == null || from == null )
                return false;

            // inclusi a priori praticolari tipi di creature
            if( m is SilverSteed || m is HolyMount )
                return true;

            // summon e pet sono alleati se il loro master è alleato
            if( m is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)m;
                Mobile master = bc.GetMaster();
                if( master != null )
                    return IsAlly( from, master );
            }

            // il party è sempre salvo
            if( from.Party is Party )
            {
                if( ( (Party)from.Party ).Contains( m ) )
                    return true;
            }

            // i gildati alleati sono alleati, i nemici di gilda no
            Guild fromGuild = from.Guild as Guild;
            Guild targetGuild = m.Guild as Guild;

            if( fromGuild != null && targetGuild != null )
            {
                if( targetGuild == fromGuild || fromGuild.IsAlly( targetGuild ) )
                    return true;

                if( fromGuild.IsEnemy( targetGuild ) || fromGuild.IsTempEnemy( m ) )
                    return false;
            }

            // non aggressori, giocatori con karma positivo e senza kills sono sempre validi alleati
            return !IsAggressor( from, m ) && m.Karma >= 0 && m.Kills <= 0;
        }

        private static bool IsAggressor( Mobile from, Mobile m )
        {
            foreach( AggressorInfo info in from.Aggressors )
            {
                if( m == info.Attacker && !info.Expired )
                    return true;
            }

            return false;
        }

        /*
        private bool IsAggressed( Mobile m )
        {
            foreach( AggressorInfo info in Caster.Aggressed )
            {
                if( m == info.Defender && !info.Expired )
                    return true;
            }

            return false;
        }
        */

        public static void AlterDamage( BaseWeapon weapon, Mobile attacker, Mobile defender, ref int damage )
        {
            if( attacker.Player && weapon.IsXmlHolyWeapon )
            {
                int level = GetPowerLevelByType( attacker, typeof( SwordOfLightSpell ) );
                if( level > 0 )
                    damage = (int)( damage * ( 1 + ( 0.02 * level ) ) );
            }
        }

        #region powervalue
        private const int ScalarOne = 6014;

        private const int KarmaScalar = 3;

        private const int SkillScalar = 10;

        private const int LevelScalar = 5000;

        public int PowerValueScaled
        {
            get
            {
                if( Caster != null )
                    return (int)Math.Sqrt( ScalarOne + ( KarmaScalar * Caster.Karma ) + ( Caster.Skills.Chivalry.Fixed * SkillScalar ) + ( GetPowerLevel() * LevelScalar ) );
                else
                    return 0;
            }
        }

        public static int GetPowerValueScaledByType( Mobile caster, Type t )
        {
            ClassPlayerState playerState = ClassPlayerState.Find( caster );

            if( playerState != null )
                return (int)Math.Sqrt( ScalarOne + ( KarmaScalar * caster.Karma ) + ( caster.Skills.Chivalry.Fixed * SkillScalar ) + ( playerState.GetLevel( t ) * LevelScalar ) );
            else
                return 0;
        }
        #endregion
    }
}