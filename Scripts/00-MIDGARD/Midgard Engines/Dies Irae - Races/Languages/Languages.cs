/***************************************************************************
 *                                  Languages.cs
 *                            		------------
 *  begin                	: Ottobre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 			Il comando [RL (Race Language) permette di parlare a un razzato
 * 			con gli altri razzati nel raggio senza che i non razzati capiscano
 * 			il messaggio.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
using System.IO;

namespace Midgard.Engines.Races
{
    public class LanguageItem : Item
    {
        private Race m_Race;

        [CommandProperty( AccessLevel.GameMaster )]
        public Race Race
        {
            get
            {
                if( m_Race == null )
                    m_Race = Race.DefaultRace;

                return m_Race;
            }
            set
            {
                m_Race = value ?? Race.DefaultRace;

                InvalidateProperties();
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1060658, "Language\t{0}", m_Race.Name );
        }

        [Constructable]
        public LanguageItem()
            : this( Race.Human )
        {
        }

        [Constructable]
        public LanguageItem( Race race )
            : base( 0x2831 )
        {
            m_Race = race;

            Hue = 1154;
            Name = "Language Knowledge";

            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !from.InRange( GetWorldLocation(), 2 ) )
            {
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
                return;
            }

            if( !IsChildOf( from.Backpack ) )
            {
                from.SendMessage( "La pergamena deve essere nel tuo zaino per essere usata." );
                return;
            }

            if( from is Midgard2PlayerMobile )
            {
                Midgard2PlayerMobile m2pm = from as Midgard2PlayerMobile;

                if( !m2pm.KnowsLanguage( m_Race ) )
                {
                    m2pm.AcquireLanguage( m_Race );
                    m2pm.SendMessage( "Hai imparato un nuovo linguaggio (razza: {0})", m_Race.PluralName );
                    Delete();
                }
                else
                {
                    m2pm.SendMessage( "Conosci gia' tale linguaggio." );
                }
            }
        }

        public LanguageItem( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( m_Race );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Race = reader.ReadRace();
        }
    }

    public class Dictionary : Item
    {
        private Race m_Race;

        [CommandProperty( AccessLevel.GameMaster )]
        public Race Race
        {
            get
            {
                if( m_Race == null )
                    m_Race = Race.DefaultRace;
                return m_Race;
            }
            set
            {
                m_Race = value;
                if( m_Race == null )
                    m_Race = Race.DefaultRace;
                InvalidateProperties();
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1060658, "Language\t{0}", m_Race.Name );
        }

        [Constructable]
        public Dictionary()
            : this( Race.Human )
        {
        }

        [Constructable]
        public Dictionary( Race race )
            : base( 0xEFA )
        {
            m_Race = race;

            Hue = 1154;
            Name = "Dictionary";

            LootType = LootType.Blessed;
        }

        public Dictionary( Serial serial )
            : base( serial )
        {
        }

        #region serialize-deserialize
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( m_Race );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Race = reader.ReadRace();
        }
        #endregion
    }

    public class RacialLanguage
    {
        private const string OverHeadMessage = "* pronuncia una frase incomprensibile *";
        private const int Range = 10;

        public static void Initialize()
        {
            CommandSystem.Register( "Linguaggio", AccessLevel.Player, new CommandEventHandler( RacialLanguage_OnCommand ) );
            CommandSystem.Register( "L", AccessLevel.Player, new CommandEventHandler( RacialLanguage_OnCommand ) );
        }

        [Aliases( "L" )]
        [Usage( "Linguaggio <razza> \"<stringa>\"" )]
        [Description( "Consente di parlare nella lingua razziale." )]
        public static void RacialLanguage_OnCommand( CommandEventArgs e )
        {
            if( !Config.RacialLanguageEnabled )
            {
                e.Mobile.SendMessage( "Questo potere e' stato disabilitato." );
                return;
            }

            if( !e.Mobile.Alive )
            {
                e.Mobile.SendMessage( "Non puoi usare questo comando in questo stato." );
                return;
            }

            try
            {
                Midgard2PlayerMobile m2pm = e.Mobile as Midgard2PlayerMobile;
                if( m2pm == null )
                    return;

                Race race = m2pm.Race;

                if( e.Length == 0 ) // formato [Linguaggio
                {
                    GetCommandHelp( m2pm );
                    return;
                }

                Race language = race; // di default un pg parla nel suo dialetto

                // se il primo parametro e' una razza (o il suo indice) allora parlera' con quel dialetto
                if( e.Length == 2 )
                    TryParseRace( e.GetString( 0 ), out language );

                if( language == Race.Human )
                    m2pm.SendMessage( "La lingua comune e' capita da tutte le razze." );
                else if( language != race && !m2pm.KnowsLanguage( language ) )
                    m2pm.SendMessage( "Non conosci questa lingua." );
                else
                {
                    string message = string.Empty;

                    if( e.Length == 1 )
                        message = e.ArgString; // formato [Linguaggio <messaggio>
                    else if( e.Length == 2 )
                        message = e.GetString( 1 ); // formato [Linguaggio <razza> "<messaggio>"
                    else
                        m2pm.SendMessage( "Uso del comando [Linguaggio <razza> \"<stringa>\"" );

                    if( String.IsNullOrEmpty( message ) )
                        return;
                    else
                    {
                        m2pm.PublicOverheadMessage( MessageType.Emote, 1154, true, OverHeadMessage );
                        m2pm.SendMessage( "Pronunci la frase: {0}", message );

                        IPooledEnumerable inRange = m2pm.Map.GetMobilesInRange( m2pm.Location, Range );

                        foreach( Mobile m in inRange )
                        {
                            Midgard2PlayerMobile m2pmTo = m as Midgard2PlayerMobile;

                            if( m2pmTo != null && m2pmTo != m2pm && m2pmTo.KnowsLanguage( language ) )
                            {
                                m2pmTo.SendMessage( "{0} pronuncia la frase: {1}", m2pm.Name, message );
                            }
                        }

                        inRange.Free();
                    }
                }
            }
            catch( Exception ex )
            {
                TextWriter tw = File.AppendText( "Logs/RaceErrors.log" );
                tw.WriteLine( "Warning: error in Language command." );
                tw.WriteLine( ex.ToString() );
                tw.WriteLine( "" );
                tw.Close();
            }
        }

        private static void GetCommandHelp( Midgard2PlayerMobile m2pm )
        {
            List<Race> knownLanguages = new List<Race>();

            foreach( Race r in Race.AllRaces )
            {
                if( r == Race.Human || r == Race.Elf || r == m2pm.Race )
                    continue;

                if( m2pm.KnowsLanguage( r ) )
                    knownLanguages.Add( r );
            }

            if( knownLanguages.Count == 0 )
                m2pm.SendMessage( "Non conosci altre lingue se non la tua." );
            else
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine( "Conosci le seguenti lingue." );

                foreach( Race r in knownLanguages )
                    sb.AppendLine( String.Format( "Razza: {0} (scorciatoia {1})", r.Name, r.RaceIndex ) );

                sb.AppendLine( "Puoi parlare in una lingua digitando [Linguaggio o [L seguito dal nome dea razza o dalla scorciatoia" +
                    " e dal messaggio che vuoi dire compreso tra virgolette." );

                m2pm.SendMessage( sb.ToString() );
            }
        }

        public static void TryParseRace( string value, out Race parsedRace )
        {
            if( !String.IsNullOrEmpty( value ) )
            {
                bool canBeParsed = false;

                string[] names = Race.GetRaceNames();
                for( int i = 0; i < names.Length; i++ )
                {
                    if( Insensitive.Equals( names[ i ], value ) )
                        canBeParsed = true;
                }

                int index;
                if( int.TryParse( value, out index ) )
                {
                    if( index >= 0 && index < Race.Races.Length && Race.Races[ index ] != null )
                        canBeParsed = true;
                }

                if( canBeParsed )
                {
                    parsedRace = Race.Parse( value );
                    return;
                }
            }

            parsedRace = Race.DefaultRace;
        }
    }
}