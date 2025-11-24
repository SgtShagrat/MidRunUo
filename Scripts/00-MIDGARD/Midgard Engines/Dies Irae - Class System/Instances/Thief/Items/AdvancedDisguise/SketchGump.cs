/***************************************************************************
 *                                  SketchGump.cs
 *                            		-------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Text;
using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Engines.XmlPoints;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

using Notoriety = Server.Notoriety;

namespace Midgard.Engines.AdvancedDisguise
{
    public class SketchGump : Gump
    {
        public enum Buttons
        {
            Delete = 1,
            DoCamuflage,
            GetNewInfo,
            AddNewAlias,
        }

        private static readonly string HueString = "***";
        private static readonly string UnknownString = "unknown";

        public bool CanDrawSketch
        {
            get
            {
                return m_Entry != null && m_Entry.IsFullEntry;
            }
        }

        private readonly Mobile m_Owner;
        private readonly SketchBook m_Book;
        private int m_Page;
        private DisguiseEntry m_Entry;

        private static readonly Hashtable m_Timers = new Hashtable();

        public SketchGump( Mobile owner, SketchBook book )
            : this( owner, book, 1 )
        {
        }

        public SketchGump( Mobile owner, SketchBook book, int page )
            : base( 150, 200 )
        {
            m_Book = book;
            m_Owner = owner;
            m_Entry = null;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            Initialize( page );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;

            if( m_Book.Deleted || !from.InRange( m_Book.GetWorldLocation(), 1 ) )
                return;

            if( from.AccessLevel == AccessLevel.Player && !SketchBook.CheckIsThief( from, true ) )
                return;

            if( info.ButtonID == 1 ) // Delete
            {
                if( m_Entry != null && m_Book.Entries != null && m_Page > 0 && m_Page <= m_Book.Entries.Count )
                {
                    m_Book.Entries.RemoveAt( m_Page - 1 );
                    m_Book.InvalidateProperties();

                    m_Owner.SendMessage( "You have successfully cleared that alias from your sketch book." );
                }
            }
            else if( info.ButtonID == 2 ) // DoCamuflage
            {
                DoCamuflage( m_Owner, m_Entry );
            }
            else if( info.ButtonID == 3 ) // GetNewInfo
            {
                m_Owner.SendMessage( "Choose the target you would to study more!" );
                m_Owner.BeginTarget( 2, false, TargetFlags.None, new TargetCallback( GetNewInfo_OnTarget ) );
            }
            else if( info.ButtonID == 4 ) // AddNewAlias
            {
                m_Owner.SendMessage( "Choose the target of your study!" );
                m_Owner.BeginTarget( 2, false, TargetFlags.None, new TargetCallback( AddEntry_OnTarget ) );
            }
            else if( info.ButtonID == 200 ) // Previous Page
            {
                m_Page--;
                m_Owner.CloseGump( typeof( SketchGump ) );
                m_Owner.SendGump( new SketchGump( m_Owner, m_Book, m_Page ) );
            }
            else if( info.ButtonID == 300 ) // NextPage
            {
                m_Page++;
                m_Owner.CloseGump( typeof( SketchGump ) );
                m_Owner.SendGump( new SketchGump( m_Owner, m_Book, m_Page ) );
            }
        }

        private void Initialize( int page )
        {
            m_Page = page;

            bool isEmptyBook = m_Book.IsEmptyBook;

            if( !isEmptyBook && m_Book.Entries.Count < m_Page )
                return;

            if( !isEmptyBook )
                m_Entry = m_Book.Entries[ m_Page - 1 ];

            AddPage( 0 );
            AddBackground( 0, 0, 401, 401, 3500 );

            DrawButtons();

            // Sketch
            AddLabel( 250, 30, 0, "last known sketch" );
            AddBackground( 230, 60, 150, 240, 2620 );
            if( CanDrawSketch )
                DrawSketch( m_Entry );
            else
                AddLabel( 260, 150, 132, "-=unavailable=-" );

            // Labels leftside
            AddLabel( 30, 20, 0, "Alias:" );
            AddLabel( 30, 70, 0, "Sex:" );
            AddLabel( 30, 90, 0, "Skin:" );
            AddLabel( 30, 130, 0, "Hair:" );
            AddLabel( 30, 180, 0, "Facial Hair:" );
            AddLabel( 30, 220, 0, "Karma:" );
            AddLabel( 30, 240, 0, "Fame:" );
            AddLabel( 30, 280, 0, "Town:" );

            if( m_Entry != null )
                DrawAliasInfo();
        }

        private void DrawButtons()
        {
            if( m_Page > 1 )
                AddButton( 350, 13, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0 ); 	// Previous Page

            if( m_Page < m_Book.Entries.Count )
                AddButton( 370, 13, 0x15E1, 0x15E5, 300, GumpButtonType.Reply, 0 ); 	// NextPage

            if( !m_Book.IsEmptyBook )
            {
                AddButton( 20, 320, 2640, 2641, (int)Buttons.Delete, GumpButtonType.Reply, 0 );
                AddButton( 20, 350, 2640, 2641, (int)Buttons.DoCamuflage, GumpButtonType.Reply, 0 );
                AddButton( 350, 320, 2640, 2641, (int)Buttons.GetNewInfo, GumpButtonType.Reply, 0 );
            }

            AddButton( 350, 350, 2640, 2641, (int)Buttons.AddNewAlias, GumpButtonType.Reply, 0 );

            if( !m_Book.IsEmptyBook )
            {
                AddLabel( 60, 322, 0, "delete this alias" );
                AddLabel( 60, 352, 0, "do camuflage" );
                AddLabel( 250, 322, 0, "get new infoes" );
            }

            AddLabel( 240, 352, 0, "add another alias" );
        }

        private void DrawAliasInfo()
        {
            if( !String.IsNullOrEmpty( m_Entry.AliasName ) )
                AddLabel( 90, 20, 0, m_Entry.AliasName );
            else
                AddLabel( 90, 20, 0, UnknownString );

            if( m_Entry.AliasBodyMod != -1 )
            {
                if( m_Entry.AliasBodyMod == 1888 )
                    AddLabel( 120, 70, 0, "female" );
                else
                    AddLabel( 120, 70, 0, "male" );
            }
            else
                AddLabel( 120, 70, 0, UnknownString );

            if( m_Entry.AliasSkinHue != -1 )
                AddLabel( 120, 90, m_Entry.AliasSkinHue - 1, HueString );
            else
                AddLabel( 120, 90, 0, UnknownString );

            if( m_Entry.AliasHairHue != -1 )
                AddLabel( 120, 130, m_Entry.AliasHairHue - 1, HueString );
            else
                AddLabel( 120, 130, 0, UnknownString );

            if( m_Entry.AliasHairID != -1 )
                AddLabel( 120, 150, 0, GetHairName( m_Entry.AliasHairID ) );
            else
                AddLabel( 120, 150, 0, UnknownString );

            if( m_Entry.AliasFacialHairHue != -1 )
                AddLabel( 120, 180, m_Entry.AliasFacialHairHue - 1, HueString );
            else
                AddLabel( 120, 180, 0, UnknownString );

            if( m_Entry.AliasFacialHairID != -1 )
                AddLabel( 120, 200, 0, GetFacialHairName( m_Entry.AliasFacialHairID ) );
            else
                AddLabel( 120, 200, 0, UnknownString );

            if( m_Entry.AliasKarma != -1 )
                AddLabel( 120, 220, 0, m_Entry.AliasKarma.ToString() );
            else
                AddLabel( 120, 220, 0, UnknownString );

            if( m_Entry.AliasFame != -1 )
                AddLabel( 120, 240, 0, m_Entry.AliasFame.ToString() );
            else
                AddLabel( 120, 240, 0, UnknownString );

            if( m_Entry.AliasTown != MidgardTowns.None )
                AddLabel( 120, 280, 0, GetTownName( m_Entry.AliasTown ) );
            else
                AddLabel( 120, 280, 0, UnknownString );
        }

        private void DrawSketch( DisguiseEntry entry )
        {
            if( entry == null )
                return;

            Console.WriteLine( "Debug DrawSketch: {0} - {1} - {2}", m_Entry.AliasBodyMod, m_Entry.AliasHairID, m_Entry.AliasFacialHairID );

            bool isFemale = ( m_Entry.AliasBodyMod == 1888 );

            AddImage( 190, 10, m_Entry.AliasBodyMod, m_Entry.AliasSkinHue - 1 );

            if( m_Entry.AliasHairID > 0 )
            {
                int hairImage = GetHairGumpID( m_Entry.AliasHairID, isFemale );
                if( hairImage > 0 )
                    AddImage( 190, 10, hairImage, m_Entry.AliasHairHue - 1 );
            }

            if( m_Entry.AliasFacialHairID > 0 && !IsBadSketchItem( m_Entry.AliasFacialHairID ) )
            {
                int facialImage = GetFacialGumpID( m_Entry.AliasFacialHairID );
                if( facialImage > 0 )
                    AddImage( 190, 10, facialImage, m_Entry.AliasFacialHairHue - 1 );
            }
        }

        private static bool IsBadSketchItem( int itemID )
        {
            return ( itemID == 785 ||
                itemID == 786 ||
                itemID == 1599 ||
                itemID == 1600 ||
                itemID == 780 ||
                itemID == 781 ||
                itemID == 782 ||
                itemID == 783 ||
                itemID == 5147 );
        }

        private static int GetHairGumpID( int hairID, bool female )
        {
            switch( hairID )
            {
                case 8265:
                    return ( female ? 1836 : 1870 );
                case 8252:
                    return ( female ? 1837 : 1876 );
                case 8266:
                    return ( female ? 1840 : 1874 );
                case 8260:
                    return ( female ? 1843 : 1877 );
                case 8261:
                    return ( female ? 1844 : 1871 );
                case 8253:
                    return ( female ? 1845 : 1879 );
                case 8264:
                    return ( female ? 1846 : 1880 );
                case 8251:
                    return ( female ? 1847 : 1875 );
                default:
                    return 0;
            }
        }

        private static int GetFacialGumpID( int facialHairID )
        {
            switch( facialHairID )
            {
                case 8269:
                    return 1887;
                case 8257:
                    return 1884;
                case 8255:
                    return 1885;
                case 8268:
                    return 1882;
                case 8267:
                    return 1886;
                case 8254:
                    return 1883;
                case 785:
                    return 0xC5A5; 	// HighElf
                case 786:
                    return 0xC5A6; 	// HalfElf
                case 1599:
                    return 0xC599; 	// MountainDwarf
                case 1600:
                    return 0xC59A; 	// MountainDwarf
                case 780:
                    return 0xC5A0; 	// FairyOfWood...
                case 781:
                    return 0xC5A1;
                case 782:
                    return 0xC5A2;
                case 783:
                    return 0xC5A3;
                case 5147:
                    return 0xC4F0; 	// HighOrc
                default:
                    return 0;
            }
        }

        private static string GetHairName( int itemID )
        {
            switch( itemID )
            {
                case 8251:
                    return "short";
                case 8261:
                    return "pageboy";
                case 8252:
                    return "long";
                case 8264:
                    return "receding";
                case 8253:
                    return "ponytail";
                case 8265:
                    return "2-tails";
                case 8260:
                    return "mohawk";
                case 8266:
                    return "topknot";
                case 0:
                    return "none";
                default:
                    return UnknownString;
            }
        }

        private static string GetFacialHairName( int itemID )
        {
            switch( itemID )
            {
                case 8269:
                    return "vandyke";
                case 8257:
                    return "mustache";
                case 8255:
                    return "short beard";
                case 8268:
                    return "long beard";
                case 8267:
                    return "short beard";
                case 8254:
                    return "long beard";
                case 0:
                    return "none";

                case 785: 		// HighElf
                case 786: 		// HalfElf
                case 1599:		// MountainDwarf
                case 1600: 		// MountainDwarf
                case 780: 		// FairyOfWood...
                case 781:
                case 782:
                case 783:
                case 5147:
                    return String.Empty;// HighOrc

                default:
                    return UnknownString;
            }
        }

        private static string GetTownName( MidgardTowns townID )
        {
            TownSystem sys = TownSystem.Find( townID );
            return sys != null ? sys.Definition.TownName.ToString().ToLower() : string.Empty;
        }

        protected void GetNewInfo_OnTarget( Mobile from, object obj )
        {
            if( m_Book == null || m_Book.Deleted )
                return;

            Midgard2PlayerMobile target = obj as Midgard2PlayerMobile;

            if( target != null && !target.Deleted )
            {
                if( from.InRange( target.Location, 10 ) )
                {
                    if( m_Book.HasAlias( target ) )
                    {
                        if( !m_Entry.IsFullEntry )
                        {
                            if( from.BeginAction( typeof( SketchGump ) ) )
                            {
                                from.SendMessage( "Ok guy, now remain close enough your target..." );
                                new GetInfoTimer( m_Entry, from, target, Utility.Random( 3, 2 ) ).Start();
                            }
                            else
                            {
                                from.SendMessage( "Mmmm... you cannot start another investigation at now." );
                            }
                        }
                        else
                        {
                            from.SendMessage( "Hey guy, that alias is completed. If you wish to update any data, just remove it and make a new one." );
                        }
                    }
                    else
                    {
                        from.SendMessage( "You have to start a new alias page in your book before trying to get data from that target." );
                    }
                }
                else
                {
                    from.SendMessage( "You are too far away to get any new alias data." );
                }
            }
            else
            {
                from.SendMessage( "That is not the target you wish to study, isn't it?" );
            }
        }

        protected void AddEntry_OnTarget( Mobile from, object obj )
        {
            if( m_Book == null || m_Book.Deleted )
                return;

            if( obj is Midgard2PlayerMobile )
            {
                Mobile target = obj as Mobile;
                if( target.Deleted )
                    return;

                if( target.AccessLevel > AccessLevel.Player )
                {
                    from.SendMessage( "Hey, are you a foolish player?!" );
                    return;
                }

                if( !m_Book.HasAlias( target ) )
                {
                    if( !String.IsNullOrEmpty( target.Name ) )
                    {
                        DisguiseEntry de = new DisguiseEntry();
                        de.AliasName = target.Name;
                        de.AliasSource = target;
                        m_Book.AddEntry( de );
                        from.SendMessage( "Awesome! You added a new alias into your book!" );
                    }
                    else
                    {
                        from.SendMessage( "Uhm, that an unusual target..." );
                    }
                }
                else
                {
                    from.SendMessage( "Hey, your book contains an alias from that target!" );
                }
            }
            else
            {
                from.SendMessage( "Fool! You cannot camuflate in a such form!" );
            }
        }

        private void DoCamuflage( Mobile mobile, DisguiseEntry entry )
        {
            if( mobile == null || entry == null )
                return;

            Midgard2PlayerMobile m2Pm = mobile as Midgard2PlayerMobile;
            if( m2Pm == null )
                return;

            if( !CheckDisguiseKit( m2Pm, true ) )
                return;

            if( m_Entry.AliasSource == null )
                return;

            if( m_Entry.AliasSource.Blessed || m_Entry.AliasSource.AccessLevel > AccessLevel.Player || XmlPointsAttach.AreInAnyGame( m_Entry.AliasSource ) )
            {
                mobile.SendMessage( "That alias is not available at the moment." );
                return;
            }

            // set the main alias
            m2Pm.Alias = m_Entry.AliasSource;

            // name
            if( !String.IsNullOrEmpty( m_Entry.AliasName ) )
                m2Pm.NameMod = m_Entry.AliasName;

            // body
            if( m_Entry.AliasBodyMod != -1 )
            {
                m2Pm.BodyMod = ( m_Entry.AliasBodyMod == 1888 ) ? 401 : 400;
                m2Pm.FemaleMod = m_Entry.AliasBodyMod == 1888;
            }

            // skin hue
            if( m_Entry.AliasSkinHue != -1 )
                m2Pm.HueMod = m_Entry.AliasSkinHue;

            // facial and hair itemid
            if( m_Entry.AliasHairID != -1 && m_Entry.AliasFacialHairID != -1 )
                m2Pm.SetHairMods( m_Entry.AliasHairID, m_Entry.AliasFacialHairID );
            else if( m_Entry.AliasHairID != -1 )
                m2Pm.SetHairMods( m_Entry.AliasHairID, -2 );
            else if( m_Entry.AliasFacialHairID != -1 )
                m2Pm.SetHairMods( -2, m_Entry.AliasFacialHairID );

            // facial and hair hue
            if( m_Entry.AliasHairHue != -1 )
                m2Pm.HairHue = m_Entry.AliasHairHue;
            if( m_Entry.AliasFacialHairHue != -1 )
                m2Pm.FacialHairHue = m_Entry.AliasFacialHairHue;

            // karma and fame
            if( m_Entry.AliasKarma != -1 )
                m2Pm.KarmaMod = m_Entry.AliasKarma;
            if( m_Entry.AliasFame != -1 )
                m2Pm.FameMod = m_Entry.AliasFame;

            // town
            if( m_Entry.AliasTown != MidgardTowns.None )
                m2Pm.TownMod = m_Entry.AliasTown;

            StopTimer( m_Owner );

            double duration = 120.0 * ( m_Owner.Int / 100.0 ) * ( m_Owner.Dex / 100.0 ) * ( m_Owner.Skills.Stealing.Value / 100.0 );

            m_Owner.SendMessage( "Well done. Your new identity will last exactly for {0} minutes. Be aware {1}!",
                                 duration.ToString( "F0" ), !String.IsNullOrEmpty( m_Owner.Name ) ? m_Owner.Name : String.Empty );

            m_Timers[ m_Owner ] = Timer.DelayCall( TimeSpan.FromMinutes( duration ), new TimerStateCallback( OnDisguiseExpire ), m_Owner );
        }

        private static void RemoveCamuflage( Mobile mobile )
        {
            Midgard2PlayerMobile m2Pm = mobile as Midgard2PlayerMobile;
            if( m2Pm == null )
                return;

            m2Pm.Alias = null;

            m2Pm.NameMod = null; 				// name
            m2Pm.BodyMod = 0;					// body
            m2Pm.FemaleMod = false;             // sex
            m2Pm.HueMod = -1;					// skin hue
            m2Pm.SetHairMods( -1, -1 );			// facial and hair itemid and hue
            m2Pm.KarmaMod = -1;					// karma
            m2Pm.FameMod = -1;					// fame
            m2Pm.TownMod = MidgardTowns.None;	// town

            m2Pm.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
            m2Pm.SendMessage( "Your alias is fired!" );
        }

        public static void HandleDisguiseProperties( Mobile from, ObjectPropertyList list )
        {
            if( from is Midgard2PlayerMobile && IsCamuflated( from ) )
            {
                Mobile alias = ( (Midgard2PlayerMobile)from ).Alias;

                if( alias != null )
                {
                    TownPlayerState state = TownPlayerState.Find( alias );
                    if( state != null )
                    {
                        if( state.DisplayCitizenStatus && state.TownSystem != null )
                        {
                            string town = state.TownSystem.Definition.TownName;

                            if( !string.IsNullOrEmpty( state.CustomTownOffice ) )
                                list.Add( 1064259, string.Format( "{0}\t{1}", state.CustomTownOffice, town ) ); // ~1_TITLE~ of ~2_CITY~
                            else if( state.TownOffice != TownOffices.None )
                                list.Add( 1064259, string.Format( "{0}\t{1}", state.TownOffice, town ) ); // ~1_val~: ~2_val~
                            else
                                list.Add( 1064258, town ); // Citizen of ~1_CITY~

                            if( !string.IsNullOrEmpty( state.CustomProfession ) )
                                list.Add( 1060659, string.Format( "{0}\t{1}", "Profession", state.CustomTownOffice ) ); // ~1_val~: ~2_val~
                            else if( state.TownProfession != Professions.None )
                                list.Add( 1060659, string.Format( "{0}\t{1}", "Profession", state.TownProfession ) ); // ~1_val~: ~2_val~
                        }
                    }
                }
            }
        }

        public static bool HandleMobileNotoriety( Mobile source, Mobile target, out int noto )
        {
            if( IsCamuflated( target ) )
            {
                if( target.Criminal )
                {
                    noto = Notoriety.Criminal;
                    return true;
                }

                if( Server.Misc.NotorietyHandlers.CheckAggressor( source.Aggressors, target ) || Server.Misc.NotorietyHandlers.CheckAggressed( source.Aggressed, target ) )
                {
                    noto = Notoriety.CanBeAttacked;
                    return true;
                }

                Midgard2PlayerMobile alias = ( (Midgard2PlayerMobile)target ).Alias as Midgard2PlayerMobile;
                if( alias == null )
                {
                    noto = -1;
                    return false;
                }

                int aliasNoto = Notoriety.Compute( source, alias );
                if( aliasNoto == Notoriety.Ally || aliasNoto == Notoriety.Enemy )
                {
                    noto = aliasNoto;
                    return true;
                }

                if( alias.PermaRed || alias.Kills > 5 )
                {
                    noto = Notoriety.Murderer;
                    return true;
                }

                noto = Notoriety.Innocent;
                return true;
            }

            noto = -1;
            return false;
        }

        public static void HandleOnSingleClick( Mobile thief, Mobile from )
        {
            if( thief is Midgard2PlayerMobile && IsCamuflated( thief ) )
            {
                Midgard2PlayerMobile alias = ( (Midgard2PlayerMobile)thief ).Alias as Midgard2PlayerMobile;
                if( alias == null )
                    return;

                int hue = alias.NameHue != -1 ? alias.NameHue : Notoriety.GetHue( Notoriety.Compute( from, alias ) );

                if( alias.DisplayCitizenStatus && alias.Town != MidgardTowns.None )
                {
                    TownPlayerState tps = TownPlayerState.Find( alias );
                    if( tps != null )
                    {
                        StringBuilder sb = new StringBuilder();

                        string town = TownHelper.FindTownName( alias.Town );

                        if( tps.IsWarLord )
                            sb.AppendFormat( "WarLord of {0}, ", town );

                        if( !string.IsNullOrEmpty( tps.CustomTownOffice ) )
                            sb.AppendFormat( string.Format( "{0} of {1}", tps.CustomTownOffice, town ) );
                        else if( tps.TownOffice != TownOffices.None )
                            sb.AppendFormat( string.Format( "{0} of {1}", tps.TownOffice, town ) );
                        else
                            sb.AppendFormat( string.Format( "Citizen of {0}", town ) );

                        if( !string.IsNullOrEmpty( tps.CustomProfession ) )
                            sb.AppendFormat( ", {0}", tps.CustomProfession );
                        else if( tps.TownProfession != Professions.None )
                            sb.AppendFormat( string.Format( ", {0}", tps.TownProfession ) );

                        thief.PrivateOverheadMessage( MessageType.Label, hue, true, sb.ToString(), from.NetState );
                    }

                    BaseGuild guild = alias.Guild;
                    if( guild != null && ( alias.DisplayGuildTitle || ( alias.Player && guild.Type != GuildType.Regular ) ) )
                    {
                        string title = alias.GuildTitle;
                        if( title == null )
                            title = "";
                        else
                            title = title.Trim();

                        string text = String.Format( title.Length <= 0 ? "[{1}]{2}" : "[{0}, {1}]{2}", title, guild.Abbreviation, "" );

                        alias.PrivateOverheadMessage( MessageType.Regular, alias.SpeechHue, true, text, from.NetState );
                    }

                    string name = alias.Name ?? String.Empty;

                    string prefix = "";

                    if( thief.ShowFameTitle && ( alias.Player || alias.Body.IsHuman ) && alias.Fame >= 10000 )
                        prefix = ( alias.Female ? "Lady" : "Lord" );

                    string suffix = "";

                    string trueTitle = alias.GetTitle( from );

                    if( thief.ClickTitle && !string.IsNullOrEmpty( trueTitle ) )
                        suffix = trueTitle;

                    suffix = alias.ApplyNameSuffix( suffix );

                    string val;

                    if( prefix.Length > 0 && suffix.Length > 0 )
                        val = String.Concat( prefix, " ", name, " ", suffix );
                    else if( prefix.Length > 0 )
                        val = String.Concat( prefix, " ", name );
                    else if( suffix.Length > 0 )
                        val = String.Concat( name, " ", suffix );
                    else
                        val = name;

                    thief.PrivateOverheadMessage( MessageType.Label, hue, Mobile.AsciiClickMessage, val, from.NetState );
                }
            }
        }

        public static bool CheckDisguiseKit( Mobile from, bool message )
        {
            bool hasKit = false;

            if( from != null && from.Backpack != null )
            {
                hasKit = ( from.Backpack.FindItemByType( typeof( DisguiseKit ), false ) != null );

                if( message && !hasKit )
                    from.SendMessage( "You need a disguise kit in your pack to do that!" );
            }

            return hasKit;
        }

        public static void OnDisguiseExpire( object state )
        {
            Mobile m = (Mobile)state;

            StopTimer( m );
            RemoveCamuflage( m );
        }

        public static bool IsCamuflated( Mobile m )
        {
            return m_Timers.Contains( m );
        }

        public static bool StopTimer( Mobile m )
        {
            Timer t = (Timer)m_Timers[ m ];

            if( t != null )
            {
                t.Stop();
                m_Timers.Remove( m );
            }

            return ( t != null );
        }
    }
}