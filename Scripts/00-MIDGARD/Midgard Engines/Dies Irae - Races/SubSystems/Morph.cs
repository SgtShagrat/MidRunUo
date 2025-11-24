/***************************************************************************
 *                                  Morph.cs
 *                            		--------
 *  begin                	: Ottobre, 2006
 *  version					: 2.1 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Spells.Seventh;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.Races
{
    public class Morph
    {
        private static bool MorphChangeName = false;

        public static void RegisterCommands()
        {
            CommandSystem.Register( "Aspetto", AccessLevel.Player, new CommandEventHandler( Morph_OnCommand ) );
            EventSink.Login += new LoginEventHandler( OnLogin );
        }

        [Usage( "Aspetto" )]
        [Description( "Permette di usare il cambiamento di forma alle razze che lo supportano." )]
        public static void Morph_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( !Config.RaceMorphEnabled )
            {
                from.SendMessage( from.Language == "ITA" ? "Questo potere e' stato disabilitato." : "This power had been disabled." );
                return;
            }

            if( from == null || e.Length != 0 )
                return;

            if( from.Mounted )
            {
                from.SendMessage( from.Language == "ITA" ? "Non puoi polimorfarti mentre cavalchi." : "You cannot polymorf while mounted." );
            }
            else if( !from.CanBeginAction( typeof( Morph ) ) )
            {
                from.SendMessage( from.Language == "ITA" ? "Non puoi cambiare forma ancora." : "You cannot polymorf yet." );
            }
            if( !from.CanBeginAction( typeof( PolymorphSpell ) ) )
            {
                from.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
            }
            else if( TransformationSpellHelper.UnderTransformation( from ) )
            {
                from.SendLocalizedMessage( 1063219 ); // You cannot mimic an animal while in that form.
            }
            else if( DisguiseTimers.IsDisguised( from ) )
            {
                from.SendMessage( from.Language == "ITA" ? "Non puoi cambiare forma mentre sei camuffato." : "You cannot polymorf while disguised." );
            }
            else if( from.BodyMod == 183 || from.BodyMod == 184 )
            {
                from.SendMessage( "Non puoi cambiare forma mentre il tuo corpo e' dipinto." );
            }
            else if( UnderTransformation( from ) )
            {
                RemoveContext( from, true );
                Timer.DelayCall( TimeSpan.FromSeconds( 120.0 ), new TimerStateCallback( ReleaseMorphLock ), from );
                from.BeginAction( typeof( Morph ) );
            }
            else
            {
                MorphEntry[] list = BuildList( from );

                if( list.Length == 0 )
                    from.SendMessage( from.Language == "ITA" ? "Non hai l'abilita' di cambiare forma." : "You cannot change your phisical appearance." );
                else if( list.Length == 1 )
                    DoMorph( from, list[ 0 ] );
                else
                    from.SendGump( new RacePolymorphGump( from, list ) );
            }
        }

        private static void ReleaseMorphLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( Morph ) );
            ( (Mobile)state ).SendMessage( "Ora puoi di nuovo cambiare forma." );
        }

        public static void OnLogin( LoginEventArgs e )
        {
            MorphContext context = GetContext( e.Mobile );

            if( context != null && context.SpeedBoost )
                e.Mobile.Send( SpeedControl.MountSpeed );
        }

        private static void Disarm( Mobile defender, double duration )
        {
            Item toDisarm = defender.FindItemOnLayer( Layer.OneHanded );

            if( toDisarm == null || !toDisarm.Movable )
                toDisarm = defender.FindItemOnLayer( Layer.TwoHanded );

            Container pack = defender.Backpack;

            if( pack == null || ( toDisarm != null && !toDisarm.Movable ) )
                return;

            pack.DropItem( toDisarm );

            BaseWeapon.BlockEquip( defender, TimeSpan.FromMinutes( duration ) );
        }

        private static void DoMorph( Mobile from, MorphEntry entry )
        {
            from.BeginAction( typeof( Morph ) );

            BaseMount.Dismount( from );
            Disarm( from, entry.Duration );

            from.BodyMod = entry.BodyValue;

            if( entry.Hue > 0 && !entry.BodyHasHairHue )
                from.HueMod = entry.Hue;

            if( entry.BodyHasHairHue )
                from.HueMod = from.HairHue;

            if( entry.SpeedBoost )
                from.Send( SpeedControl.MountSpeed );

            if( entry.Sound > 0 )
                from.PlaySound( entry.Sound );

            if( MorphChangeName )
                from.NameMod = entry.Name;

            SkillMod mod = null;

            if( entry.StealthBonus )
            {
                mod = new DefaultSkillMod( SkillName.Stealth, true, 20.0 );
                mod.ObeyCap = true;
                from.AddSkillMod( mod );
            }
            else if( entry.StealingBonus )
            {
                mod = new DefaultSkillMod( SkillName.Stealing, true, 10.0 );
                mod.ObeyCap = true;
                from.AddSkillMod( mod );
            }
            else if( entry.WrestlingBonus )
            {
                mod = new DefaultSkillMod( SkillName.Wrestling, true, 110.0 );
                mod.ObeyCap = false;
                from.AddSkillMod( mod );
            }

            if( from is PlayerMobile )
                ( (PlayerMobile)from ).ValidateEquipment();

            Timer timer = new MorphTimer( from, entry );
            timer.Start();

            AddContext( from, new MorphContext( timer, mod, entry ) );
        }

        internal static MorphEntry[] EmptyList = new MorphEntry[] { };

        public static MorphEntry[] BuildList( Mobile from )
        {
            return from.Race is MidgardRace ? ( (MidgardRace)from.Race ).GetMorphList() : EmptyList;
        }

        public class RacePolymorphGump : Gump
        {
            private readonly Mobile m_From;
            private readonly MorphEntry[] m_List;

            public RacePolymorphGump( Mobile from, MorphEntry[] list )
                : base( 50, 50 )
            {
                m_From = from;
                m_List = list;

                int length = m_List.Length;

                m_From.CloseGump( typeof( RacePolymorphGump ) );

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage( 0 );
                AddBackground( 10, 20, 250, 58 + 20 + ( length + 2 ) * 20, 9260 );
		if (m_From.Language == "ITA")
                	AddLabel( 30, 30, 0, @"What will thou be?" );
		else
                	AddLabel( 30, 30, 0, @"Cosa diventerai?" );

                // Bottone Close
                AddButton( 210, 58 + ( length + 2 ) * 20, 4023, 4024, 0, GumpButtonType.Reply, 0 );
                AddLabel( 30, 62 + ( length + 2 ) * 20, 0, m_From.Language == "ITA" ? "Chiudi" : "Close" );

                if( !m_From.CanBeginAction( typeof( Morph ) ) )
                {
                    // Bottone UnMorph
                    AddButton( 210, 58 + ( length + 1 ) * 20, 4023, 4024, length + 1, GumpButtonType.Reply, 0 );
                    AddLabel( 30, 62 + ( length + 1 ) * 20, 0, m_From.Language == "ITA" ? "Torna normale" : "Unmorph" );
                }

                for( int i = 0; i < length; i++ )
                {
                    if( m_List[ i ].AgeRequired > TimeSpan.Zero && ( DateTime.Now - m_From.CreationTime ) < m_List[ i ].AgeRequired )
                        continue;

                    AddButton( 210, 58 + i * 20, 4023, 4024, i + 1, GumpButtonType.Reply, 0 );
                    AddLabel( 30, 62 + i * 20, 0, m_List[ i ].Name );
                }
            }

            public override void OnResponse( NetState state, RelayInfo info )
            {
                Mobile from = state.Mobile;

                if( info.ButtonID == 0 )
                    from.CloseGump( typeof( RacePolymorphGump ) );
                else if( info.ButtonID < m_List.Length )
                {
                    MorphEntry entry = m_List[ info.ButtonID - 1 ];
                    DoMorph( from, entry );
                }
            }
        }

        private static readonly Hashtable m_Table = new Hashtable();

        public static void AddContext( Mobile m, MorphContext context )
        {
            m_Table[ m ] = context;
        }

        public static void RemoveContext( Mobile m, bool resetGraphics )
        {
            MorphContext context = GetContext( m );

            if( context != null )
                RemoveContext( m, context, resetGraphics );
        }

        public static void RemoveContext( Mobile m, MorphContext context, bool resetGraphics )
        {
            m_Table.Remove( m );

            if( context.SpeedBoost )
            {
                if( m.Region is Server.Regions.TwistedWealdDesert )
                    m.Send( SpeedControl.WalkSpeed );
                else
                    m.Send( SpeedControl.Disable );
            }

            m.EndAction( typeof( BaseWeapon ) );

            SkillMod mod = context.Mod;

            if( mod != null )
                m.RemoveSkillMod( mod );

            if( resetGraphics )
            {
                m.HueMod = -1;
                m.BodyMod = 0;

                if( MorphChangeName )
                    m.NameMod = null;
            }

            context.Timer.Stop();
        }

        public static MorphContext GetContext( Mobile m )
        {
            return ( m_Table[ m ] as MorphContext );
        }

        public static bool UnderTransformation( Mobile m )
        {
            return ( GetContext( m ) != null );
        }

        public static int AlterDamage( Mobile m )
        {
            if( !m.Player )
                return -1;

            MorphContext context = GetContext( m );
            if( context != null )
            {
                if( context.Entry.MinDamage != -1 && context.Entry.MaxDamage != -1 )
                {
                    return Utility.RandomMinMax( context.Entry.MinDamage, context.Entry.MaxDamage );
                }
            }

            return -1;
        }

        public class MorphTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly int m_Body;
            private readonly int m_Hue;
            private readonly DateTime m_LastUntil;
            private readonly int m_Delay;

            public MorphTimer( Mobile from, MorphEntry entry )
                : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
            {
                m_Mobile = from;
                m_Body = entry.BodyValue;
                m_Hue = entry.Hue;
                m_Delay = entry.Pause;
                m_LastUntil = DateTime.Now + TimeSpan.FromMinutes( entry.Duration );

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if( m_Mobile.Deleted || !m_Mobile.Alive || m_Mobile.Body != m_Body || ( m_Hue != 0 && m_Mobile.Hue != m_Hue ) )
                {
                    RemoveContext( m_Mobile, true );
                    Stop();
                    return;
                }

                if( DateTime.Now > m_LastUntil )
                {
                    RemoveContext( m_Mobile, true );
                    Stop();

                    if( m_Delay > 0 )
                    {
                        DelayCall( TimeSpan.FromSeconds( m_Delay ), new TimerStateCallback( ReleaseMorphLock ), m_Mobile );
                        m_Mobile.BeginAction( typeof( Morph ) );
                    }
                }
            }
        }

        public class MorphContext
        {
            public Timer Timer { get; private set; }
            public SkillMod Mod { get; private set; }
            public MorphEntry Entry { get; set; }
            public bool SpeedBoost { get; private set; }
            public bool PackBonus { get; private set; }
            public bool BlocksHeal { get; private set; }

            public MorphContext( Timer timer, SkillMod mod, MorphEntry entry )
            {
                Timer = timer;
                Mod = mod;
                Entry = entry;
                SpeedBoost = entry.SpeedBoost;
                PackBonus = entry.PackBonus;
                BlocksHeal = entry.RestrictItemUse;
            }

            public void AlterDamage( out int min, out int max )
            {
                min = Entry.MinDamage;
                max = Entry.MaxDamage;
            }
        }

        public static bool CheckArmorAllowed( PlayerMobile m )
        {
            if( !m.Player )
                return true;

            MorphContext context = GetContext( m );
            return context == null || context.Entry.AllowArmor;
        }

        public static bool CheckItemAllowed( Item i, Mobile from, bool message )
        {
            if( i == null || from == null )
                return false;

            if( UnderTransformation( from ) )
            {
                MorphContext context = GetContext( from );

                if( context != null && context.Entry.RestrictItemUse && context.Entry.IsRestrictedItem( i ) )
                {
                    if( message )
                        from.SendMessage( "Thou cannot do that in a such form." );
                    return false;
                }
            }

            return true;
        }
        
        public static bool CheckSpellAllowed( Spell spell, Mobile from, bool message )
        {
            if( spell == null || from == null )
                return false;

            if( UnderTransformation( from ) )
            {
                MorphContext context = GetContext( from );

                if( context != null && context.Entry.RestrictSpells && context.Entry.IsRestrictedSpell( spell ) )
                {
                    if( message )
                        from.SendMessage( "Thou cannot do that in a such form." );
                    return false;
                }
            }

            return true;
        }
    }

    public class MorphEntry
    {
        public TextDefinition Name { get; private set; }
        public int Hue { get; private set; }
        public double ReqSkill { get; private set; }
        public int BodyValue { get; private set; }
        public bool StealthBonus { get; private set; }
        public bool StealingBonus { get; private set; }
        public bool WrestlingBonus { get; set; }
        public bool SpeedBoost { get; private set; }
        public int Duration { get; private set; }
        public int Pause { get; private set; }
        public TimeSpan AgeRequired { get; private set; }
        public int Sound { get; private set; }

        public int MinDamage { get; private set; }
        public int MaxDamage { get; private set; }

        public bool AllowArmor { get; private set; }
        public int VirtualArmorMod { get; private set; }

        public bool BodyHasHairHue { get; private set; }

        public bool PackBonus { get; private set; }

        public bool RestrictItemUse { get; private set; }

        public bool RestrictSpells { get; private set; }

        public MorphEntry( int bodyMod, int hue, TextDefinition name, int duration, int pause, double reqSkill, bool stealthBonus,
            bool speedBoost, bool stealingBonus, bool wrestlingBonus, TimeSpan ageRequired, int sound, int minDamage, int maxDamage,
            bool allowArmor, int virtualArmorMod, bool bodyHasHairHue, bool packBonus, bool restrictItemUse, bool restrictSpells )
        {
            Name = name;
            Hue = hue;
            ReqSkill = reqSkill;
            BodyValue = bodyMod;
            StealthBonus = stealthBonus;
            SpeedBoost = speedBoost;
            StealingBonus = stealingBonus;
            WrestlingBonus = wrestlingBonus;
            Duration = duration;
            Pause = pause;
            AgeRequired = ageRequired;
            Sound = sound;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            AllowArmor = allowArmor;
            VirtualArmorMod = virtualArmorMod;
            BodyHasHairHue = bodyHasHairHue;
            PackBonus = packBonus;
            RestrictItemUse = restrictItemUse;
            RestrictSpells = restrictSpells;
        }

        public MorphEntry( int bodyMod, int hue, TextDefinition name, int duration, int pause, TimeSpan ageRequired )
            : this( bodyMod, hue, name, duration, pause, 0.0, false, false, false, false, ageRequired, 0, -1, -1, true, -1, false, false, false, false )
        {
        }

        public MorphEntry( int bodyMod, int hue, TextDefinition name, int duration, int pause )
            : this( bodyMod, hue, name, duration, pause, 0.0, false, false, false, false, TimeSpan.Zero, 0, -1, -1, true, -1, false, false, false, false )
        {
        }

        public override string ToString()
        {
            return string.Format( "Name: {0}, Hue: {1}, Body: {2}, Speedboost: {3}, WrestlingBonus: {4}, Duration: {5}",
                                 Name, Hue, BodyValue, SpeedBoost, WrestlingBonus, Duration );
        }

        public virtual int GetArmorBonusByPack( Mobile from )
        {
            return 0;
        }

        public virtual bool IsRestrictedItem( Item i )
        {
            return false;
        }

        public virtual bool IsRestrictedSpell( Spell spell )
        {
            return false;
        }
    }
}