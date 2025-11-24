// #define DebugMidgard2SkillBall

/***************************************************************************
 *                                  Midgard2SkillBall.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.1
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 			La Skillball permette di aumentare le skill del pg nel backpack 
 * 			del quale è inserita.
 * 			Dopo la data di scadenza non è piu' usabile.
 * 			Se si hanno piu' skillball nello zaino o se la skillball stessa 
 * 			è spostabile (Movable true) essa non permette di essere usata.
 * 
 * Changelog:
 * 			2.1
 * 			Messo decay a 1 gg.
 * 			Aggiunta Lockpicking alle skill cappate a 80.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class Midgard2SkillBall : Item
    {
        #region campi
#if DebugMidgard2SkillBall
		private TimeSpan Durata = TimeSpan.FromSeconds( 100 );
#else
        private readonly TimeSpan Durata = TimeSpan.FromDays( 1.0 );
#endif
        private DateTime m_CreationTime;
        private double m_Skills;
        private double m_SkillsIniziali;
        #endregion

        #region proprieta
        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime CreationTime
        {
            get { return m_CreationTime; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public double Skills
        {
            get { return m_Skills; }
            set
            {
                m_Skills = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public double SkillsIniziali
        {
            get { return m_SkillsIniziali; }
        }
        #endregion

        #region costruttori
        [Constructable]
        public Midgard2SkillBall( double skills )
            : base( 0x1870 )
        {
            m_Skills = skills; // Valore che si decrementa ad ogni up della skill
            m_SkillsIniziali = skills; // Valore di reset della palla
            m_CreationTime = DateTime.Now;

            Movable = false;

            Name = "Midgard Skill Ball";
            Hue = 1154;
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public Midgard2SkillBall( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void GetProperties( ObjectPropertyList list )
        {
            DateTime scadenza = CreationTime + Durata;
            TimeSpan rimanenza = scadenza - DateTime.Now;

            base.GetProperties( list );

            list.Add( 1060658, "Punti skill nella gemma\t{0}", m_Skills.ToString() );
            list.Add( 1060659, "Ore rimanenti\t{0}", rimanenza.TotalHours.ToString( "F0" ) );
        }

        public override void OnDoubleClick( Mobile from )
        {
            PlayerMobile playerMobile = from as PlayerMobile;
            if( playerMobile == null )
                return;

            // Controllo se la skillball è nello zaino
            if( !IsChildOf( playerMobile.Backpack ) )
            {
                playerMobile.SendMessage( "La gemma deve essere nel tuo zaino per essere usata." );
                return;
            }

            // Controllo se la skillball non è muovibile
            if( Movable )
            {
                playerMobile.SendMessage( "La gemma deve essere lockata nel tuo zaino. Contatta al piu' presto un Amministratore di Midgard." );
                return;
            }

            // Controllo se il pg è vivo
            if( !playerMobile.CheckAlive() )
            {
                playerMobile.SendMessage( "I morti non imparano." );
                return;
            }

            // Controllo se ci sono piu' skillball nello zaino.
            List<Item> items = playerMobile.Backpack.Items;
            int numGemmeInPack = 0;
            foreach( Item i in items )
            {
                if( i is Midgard2SkillBall )
                    numGemmeInPack++;
            }
            if( numGemmeInPack > 1 )
            {
                playerMobile.SendMessage( "Non puoi usare la gemma se ne hai un'altra nello zaino." );
                return;
            }

            // Controllo se la gemma è scaduta
            if( DateTime.Now >= CreationTime + Durata )
            {
                playerMobile.SendMessage( "La gemma ha esaurito il suo potere." );
                return;
            }

            // Controllo se si sta già usando la skillball
            if( from.HasGump( typeof( Midgard2SkillGump ) ) )
                from.SendMessage( "La gemma è già in uso." );
            else
                from.SendGump( new Midgard2SkillGump( playerMobile, this, true, 1 ) );
        }
        #endregion

        #region serialize deserialize
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_SkillsIniziali );
            writer.Write( m_Skills );
            writer.Write( m_CreationTime );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_SkillsIniziali = reader.ReadDouble();
            m_Skills = reader.ReadDouble();
            m_CreationTime = reader.ReadDateTime();
        }
        #endregion
    }
}

namespace Server.Gumps
{
    public class Midgard2SkillGump : Gump
    {
        #region Campi
        private readonly Mobile m_From;
        private Skill m_Skill;
        //		private double m_DeltaSkill;
        private readonly Midgard2SkillBall m_MiSkBa;
        private static double LimiteSup = 100.0;
        private static int m_Campi = 20;
        private int m_Page;
        private readonly bool m_First;
        #endregion

        #region Costruttori
        public Midgard2SkillGump( Mobile from, Midgard2SkillBall msb, bool first, int page )
            : base( 20, 20 )
        {
            m_Page = page;
            m_MiSkBa = msb;
            m_From = from;
            m_First = first;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_From.CloseGump( typeof( Midgard2SkillGump ) );

            // Annulla tutte le skills.
            Skills skills = m_From.Skills;
            if( m_First )
            {
                for( int i = 0; i < skills.Length; i++ )
                    skills[ i ].Base = 0.0;
                // E riempie la skillball
                m_MiSkBa.Skills = m_MiSkBa.SkillsIniziali;
            }

            AddPage( 0 );
            AddBackground( 0, 0, 300, 525, 5054 );

            AddImageTiled( 10, 10, 280, 21, 3004 );
            AddLabel( 13, 11, 0, "Scegli la skill da alzare:" );

            if( m_Page > 1 )
                AddButton( ( 290 - 16 - 20 ), 13, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0 ); // Pagina Precedente

            if( m_Page < 3 )
                AddButton( ( 290 - 16 ), 13, 0x15E1, 0x15E5, 300, GumpButtonType.Reply, 0 ); // Pagina Successiva

            AddImageTiled( ( 290 - 30 - 50 ), ( 515 - 22 ), 48, 21, 0xBBC );
            AddLabel( ( 290 - 30 - 50 + 3 ), ( 515 - 22 + 1 ), 0, "Chiudi:" );
            AddButton( ( 290 - 30 ), ( 515 - 22 ), 4018, 4019, 0, GumpButtonType.Reply, 0 ); // Chiudi

            AddImageTiled( 10, ( 515 - 22 ), 150, 21, 0xBBC );
            AddLabel( 13, ( 515 - 22 + 1 ), 0, "hai ancora " + m_MiSkBa.Skills + " punti." );

            int indMax = ( m_Page * m_Campi ) - 1;
            int indMin = ( m_Page * m_Campi ) - m_Campi;
            int indTemp = 0;

            for( int i = 0; i < skills.Length - 1; ++i ) //-1 Per togliere SpellWeaving
            {
                if( i >= indMin && i <= indMax )
                {
                    AddImageTiled( 10, 32 + ( indTemp * 22 ), 238, 21, 0xBBC );
                    AddLabelCropped( 13, 33 + ( indTemp * 22 ), 150, 21, 0, skills[ i ].Name );

                    AddImageTiled( 181, 32 + ( indTemp * 22 ), 48, 21, 0xBBC );
                    AddLabelCropped( 182, 33 + ( indTemp * 22 ), 234, 21, 0, skills[ i ].Base.ToString( "F1" ) );

                    if( ( m_From.Skills[ i ] ).Base < LimiteSup )
                    {
                        if( m_From.Skills[ i ].SkillName == SkillName.Poisoning || m_From.Skills[ i ].SkillName == SkillName.AnimalTaming || m_From.Skills[ i ].SkillName == SkillName.Lockpicking )
                        {
                            if( m_From.Skills[ i ].Base >= 80.0 )
                            {
                                indTemp++;
                                continue;
                            }
                        }
                        AddButton( 231, 33 + ( indTemp * 22 ) + 3, 0x15E1, 0x15E5, i + 1, GumpButtonType.Reply, 0 );
                    }

                    indTemp++;
                }
            }
        }
        #endregion

        #region Metodi
        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;

            if( ( from == null ) || ( m_MiSkBa.Deleted ) )
                return;

            if( !m_MiSkBa.IsChildOf( from.Backpack ) )
            {
                from.SendMessage( "La gemma deve essere nel tuo zaino per essere usata." );
                return;
            }

            if( info.ButtonID == 0 )
                return;
            else if( info.ButtonID == 200 ) // Previous Page
            {
                m_Page--;
                from.SendGump( new Midgard2SkillGump( from, m_MiSkBa, false, m_Page ) );
                return;
            }
            else if( info.ButtonID == 300 ) // Next Page
            {
                m_Page++;
                from.SendGump( new Midgard2SkillGump( from, m_MiSkBa, false, m_Page ) );
                return;
            }

            if( info.ButtonID > 0 )
            {
                if( m_MiSkBa.Skills <= 0 )
                {
                    from.SendMessage( "La gemma è vuota." );
                    return;
                }

                m_Skill = from.Skills[ ( info.ButtonID - 1 ) ];

                double count = from.Skills.Total / 10;
                double cap = from.SkillsCap / 10;
                List<Skill> decreased = new List<Skill>();
                double decreaseamount = 0.0;

                // Incremento per ogni click
                double quantita = 1.0;

                // Controllo se è possibile alzare la skill
                if( ( count + quantita ) > cap )
                {
                    for( int i = 0; i < from.Skills.Length; i++ )
                    {
                        if( from.Skills[ i ].Lock == SkillLock.Down )
                        {
                            decreased.Add( from.Skills[ i ] );
                            decreaseamount += from.Skills[ i ].Base;
                            break;
                        }
                    }
                    if( decreased.Count == 0 )
                    {
                        from.SendMessage( "Con questo incremento supereresti " + "lo skillcap ma non hai settato delle skill in discesa." );
                        return;
                    }
                }

                // Se Quantità + SkillBase < 100
                if( m_Skill.Base + quantita <= 100 )
                {
                    // Se Quantità è maggiore di SkillRimanenti + SkillInDIscesa
                    if( ( cap - count ) + decreaseamount >= quantita )
                    {
                        // Se Quantità è minore di SkillRimanenti
                        if( cap - count >= quantita )
                        {
                            m_Skill.Base += quantita;
                            m_MiSkBa.Skills -= quantita;

                            // Apre un nuovo gump per alzare la skill
                            from.SendGump( new Midgard2SkillGump( from, m_MiSkBa, false, m_Page ) );
                            return;
                        }
                        // Se Quantità è maggiore di SkillRimanenti...
                        else
                        {
                            m_Skill.Base += quantita;
                            m_MiSkBa.Skills -= quantita;

                            // ...Distribuisce l'incremento negativo
                            foreach( Skill s in decreased )
                            {
                                if( s.Base >= quantita )
                                {
                                    s.Base -= quantita;
                                    quantita = 0;
                                }
                                else
                                {
                                    quantita -= s.Base;
                                    s.Base = 0;
                                }
                                if( quantita == 0 )
                                    break;
                            }

                            // Apre un nuovo gump per alzare la skill
                            from.SendGump( new Midgard2SkillGump( from, m_MiSkBa, false, m_Page ) );
                            return;
                        }
                    }
                    else
                        from.SendMessage( "Non hai abbastanza skill settate in discesa per compensare la salita della skill scelta" );
                }
                else
                    from.SendMessage( "La skill scelta non puo' essere alzata perchè il suo valore superebbe 100.0" );
            }
        }
        #endregion
    }
}