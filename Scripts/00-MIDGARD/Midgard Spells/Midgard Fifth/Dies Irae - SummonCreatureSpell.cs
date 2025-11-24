/***************************************************************************
 *                                  SummonCreatureSpell.cs
 *                            		----------------------
 *  begin                	: Giugno, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Nuovo sistema per il summon delle creature dei maghi.
 * 			Permette di scegliere il tipo di creatura summonabile.
 * 			La lista di creature e' la seguente con il rispettivo vaore di 
 * 			megery necessaria al summon:
 * 
 * 			Dog 	   			,40
 * 			Eagle       		,55
 * 			Gorilla     		,60
 * 			BlackBear   		,65
 * 			Horse       		,70
 * 			GiantSpider 		,75
 * 			Panther     		,75
 * 			GrizzlyBear  		,80
 * 			Ettin       		,85
 * 			Troll       		,90
 * 			RatmanMage     		,95
 * 			GiantSerpent 		,100
 * 			StoneGargoyle		,110
 * 			Mummy         		,115
 * 			Succubus         	,120
 * 
 * 			La durata dello spell e' identica alla versione vecchia.
 ***************************************************************************/

using System;
using System.Collections;

using Midgard.Engines.Classes;

using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Spells.Fifth
{
    public class SummonCreatureSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Summon Creature", "Kal Xen",
                266,
                9040,
                Reagent.Bloodmoss,
                Reagent.MandrakeRoot,
                Reagent.SpidersSilk
            );

        public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

        public SummonCreatureSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        private static Hashtable m_Table = new Hashtable();

        public static Hashtable Table { get { return m_Table; } }

        public override bool CheckCast()
        {
            BaseCreature check = (BaseCreature)m_Table[ Caster ];

            if( check != null && !check.Deleted && check.SummonMaster == Caster )
            {
                Caster.SendLocalizedMessage( 1064848 ); // You already have a summoned creature.
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            if( CheckSequence() )
            {
                Caster.CloseGump( typeof( SummonCreatureGump ) );
                Caster.SendGump( new SummonCreatureGump( Caster, this ) );
            }

            FinishSequence();
        }

        public const double DelayScalar = 0.5; // mod by Dies Irae

        public override TimeSpan GetCastDelay()
        {
            return TimeSpan.FromTicks( (long) ( base.GetCastDelay().Ticks * 5 * DelayScalar ) );
        }

        private static readonly SummonCreatureEntry[] m_Entries = new SummonCreatureEntry[]
		{
			new SummonCreatureEntry( typeof( Dog ), 			1064851	,40.0 ),	// Dog
			new SummonCreatureEntry( typeof( Eagle ), 			1064852	,50.0 ),	// Eagle
			new SummonCreatureEntry( typeof( Gorilla ), 		1064853	,60.0 ),	// Gorilla
			new SummonCreatureEntry( typeof( BlackBear ), 		1064854	,65.0 ),	// Black Bear
			new SummonCreatureEntry( typeof( Horse ), 			1064855	,70.0 ),	// Horse
			new SummonCreatureEntry( typeof( GiantSpider ), 	1064856	,75.0 ),	// Giant Spider
			new SummonCreatureEntry( typeof( Panther ), 		1064857	,80.0 ),	// Panther
			new SummonCreatureEntry( typeof( GrizzlyBear ), 	1064858	,80.0 ),	// Grizzly Bear
			new SummonCreatureEntry( typeof( Ettin ), 			1064859	,85.0 ),	// Ettin
			new SummonCreatureEntry( typeof( Troll ), 			1064860	,85.0 ),	// Troll
			new SummonCreatureEntry( typeof( RatmanMage ), 		1064861	,90.0 ),	// Ratman Mage
			new SummonCreatureEntry( typeof( GiantSerpent ), 	1064862	,90.0 ), 	// Giant Serpent
			new SummonCreatureEntry( typeof( StoneGargoyle ),	1064863	,95.0 ), 	// Stone Gargoyle
	        new SummonCreatureEntry( typeof( Mummy ), 			1064864	,95.0 ) 	// Mummy
		};

        public static SummonCreatureEntry[] Entries { get { return m_Entries; } }

        public class SummonCreatureEntry
        {
            public Type Type { get; private set; }
            public object Name { get; private set; }
            public double ReqMagery { get; private set; }

            public SummonCreatureEntry( Type type, object name, double reqMagery )
            {
                Type = type;
                Name = name;
                ReqMagery = reqMagery;
            }
        }

        public class SummonCreatureGump : Gump
        {
            private readonly Mobile m_From;
            private readonly SummonCreatureSpell m_Spell;

            private const int EnabledColor16 = 0x0F20;
            private const int DisabledColor16 = 0x262A;

            private const int EnabledColor32 = 0x18CD00;
            private const int DisabledColor32 = 0x4A8B52;

            public SummonCreatureGump( Mobile from, SummonCreatureSpell spell )
                : base( 200, 100 )
            {
                m_From = from;
                m_Spell = spell;

                AddPage( 0 );

                AddBackground( 10, 10, 250, 53 + ( m_Entries.Length * 21 ), 9270 );
                AddAlphaRegion( 20, 20, 230, 33 + ( m_Entries.Length * 21 ) );

                AddHtmlLocalized( 30, 26, 200, 20, 1060147, EnabledColor16, false, false ); // Choose thy creature to summon...

                double magery = from.Skills[ SkillName.Magery ].Base;

                for( int i = 0; i < m_Entries.Length; ++i )
                {
                    object name = m_Entries[ i ].Name;

                    bool enabled = ( magery >= m_Entries[ i ].ReqMagery );

                    if( m_Entries[ i ].Type == typeof( Succubus ) || m_Entries[ i ].Type == typeof( Mummy ) )
                    {
                        if( ClassSystem.IsDruid( from ) )
                            enabled = false;
                    }

                    if( enabled )
                        AddButton( 27, 53 + ( i * 21 ), 9702, 9703, i + 1, GumpButtonType.Reply, 0 );

                    if( name is int )
                        AddHtmlLocalized( 50, 51 + ( i * 21 ), 150, 20, (int)name, enabled ? EnabledColor16 : DisabledColor16, false, false );
                    else if( name is string )
                        AddHtml( 50, 51 + ( i * 21 ), 150, 20, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", enabled ? EnabledColor32 : DisabledColor32, name ), false, false );
                }
            }

            private static void Scale( BaseCreature bc, int scalar )
            {
                int toScale = bc.RawStr;
                bc.RawStr = AOS.Scale( toScale, scalar );

                toScale = bc.HitsMaxSeed;

                if( toScale > 0 )
                    bc.HitsMaxSeed = AOS.Scale( toScale, scalar );

                bc.Hits = bc.Hits; // refresh hits
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                int index = info.ButtonID - 1;

                if( index >= 0 && index < m_Entries.Length )
                {
                    SummonCreatureEntry entry = m_Entries[ index ];

                    double magery = m_From.Skills[ SkillName.Magery ].Base;

                    BaseCreature check = (BaseCreature)Table[ m_From ];

                    if( check != null && !check.Deleted && check.SummonMaster == m_From )
                    {
                        m_From.SendLocalizedMessage( 1064848 ); // You already have a summoned creature.
                    }
                    else if( magery < entry.ReqMagery )
                    {
                        m_From.SendLocalizedMessage( 1064849, String.Format( "{0:F1}", entry.ReqMagery ) ); // That creature requires ~1_MAGERY~ Magery to be summoned.

                        m_From.CloseGump( typeof( SummonCreatureGump ) );
                        m_From.SendGump( new SummonCreatureGump( m_From, m_Spell ) );
                    }
                    else if( entry.Type == null )
                    {
                        m_From.SendLocalizedMessage( 1064847 ); // That Creature has not yet been defined.

                        m_From.CloseGump( typeof( SummonCreatureGump ) );
                        m_From.SendGump( new SummonCreatureGump( m_From, m_Spell ) );
                    }
                    else
                    {
                        TimeSpan duration = TimeSpan.FromSeconds( 4.0 * m_From.Skills[ SkillName.Magery ].Value );

                        try
                        {
                            BaseCreature bc = (BaseCreature)Activator.CreateInstance( entry.Type );

                            bc.Skills.MagicResist = m_From.Skills.MagicResist;
                            bc.Karma = m_From.Karma;

                            if( BaseCreature.Summon( bc, m_From, m_From.Location, -1, duration ) )
                            {
                                m_From.FixedParticles( 0x3728, 1, 10, 9910, EffectLayer.Head );
                                bc.PlaySound( bc.GetIdleSound() );
                                Table[ m_From ] = bc;
                            }

                            if( bc is Succubus )
                                Scale( bc, 40 );
                            else if( bc is Mummy )
                                Scale( bc, 50 );
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }
                    }
                }
                else
                {
                    m_From.SendLocalizedMessage( 1061825 ); // You decide not to summon a Creature.
                }
            }
        }
    }
}