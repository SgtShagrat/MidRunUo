/***************************************************************************
 *                                  MidgardTravelAgent.cs
 *                            		-------------------
 *  begin                	: Settembre, 2006
 *  version					: 1.3
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  L'Npc MidgardTravelAgent apre un gate a pagamento per la locazione scelta.
 *  Ogni portale ha un costo determinato nella lista delle locazioni.
 *  Puo' succedere nel PercFailure-% dei casi che il gate sia buggato e vada
 *  da un'altra parte. Nel PercCriticalFailure-% dei casi finisce in un dungeon
 *  a caso.
 *  
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Gumps;

using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    public class IlshenarTravelAgent : MidgardTravelAgent
    {
        [Constructable]
        public IlshenarTravelAgent()
        {
            Title = ", the Ilshenar travel agent";
        }

        public IlshenarTravelAgent( Serial serial )
            : base( serial )
        {
        }

        public override LocationEntry[] GetLocations()
        {
            return LocationEntry.IlshenarLocations;
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class MidgardTravelAgent : BaseCreature
    {
        public static readonly int PercFailure = 5;			// 5% dei gate vanno in un'altra città a caso
        public static readonly int PercCriticalFailure = 1;	// 1% dei gate vanno in un'altra locazione a caso

        public override bool CanTeach { get { return true; } }

        [Constructable]
        public MidgardTravelAgent()
            : base( AIType.AI_Mage, FightMode.Aggressor, 12, 1, 0.2, 0.8 )
        {
            // Stats
            SetStr( 304, 400 );
            SetDex( 102, 150 );
            SetInt( 204, 300 );

            SetHits( 66, 125 );
            SetDamage( 30, 50 );

            // Resistances
            SetResistance( ResistanceType.Physical, 90, 100 );
            SetResistance( ResistanceType.Fire, 90, 100 );
            SetResistance( ResistanceType.Cold, 90, 100 );
            SetResistance( ResistanceType.Poison, 90, 100 );
            SetResistance( ResistanceType.Energy, 90, 100 );

            // Skills
            SetSkill( SkillName.EvalInt, 90.0, 100.0 );
            SetSkill( SkillName.Inscribe, 90.0, 100.0 );
            SetSkill( SkillName.Magery, 90.0, 100.0 );
            SetSkill( SkillName.Meditation, 90.0, 100.0 );
            SetSkill( SkillName.MagicResist, 90.0, 100.0 );
            SetSkill( SkillName.Wrestling, 90.0, 100.0 );

            // Fama e Karma
            Fame = 10000;
            Karma = 10000;

            // Hair
            HairItemID = 0x203B;
            HairHue = Utility.RandomHairHue();

            // Speech & Skin hues
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            // Genre
            Female = Utility.RandomBool();

            if( Female )
            {
                Name = NameList.RandomName( "female" );
                Title = ", la Viaggiatrice Dimensionale";
                Body = 0x191;
            }
            else
            {
                Name = NameList.RandomName( "male" );
                Title = ", il Viaggiatore Dimensionale";
                Body = 0x190;
            }

            // Equip:
            // 	Robe
            Item robe = new Item( 778 );
            robe.Name = " a Dimensional Robe";
            robe.Hue = 2667;
            robe.Layer = Layer.OuterTorso;
            robe.LootType = LootType.Blessed;
            robe.Movable = false;
            AddItem( robe );
            // 	Sandals
            Sandals sandals = new Sandals();
            sandals.Hue = 2667;
            sandals.LootType = LootType.Blessed;
            sandals.Movable = false;
            AddItem( sandals );
            // Staff
            StaffOfPower staff = new StaffOfPower();
            staff.LootType = LootType.Blessed;
            staff.Name = "Staffa delle Ere";
            AddItem( staff );
        }

        public MidgardTravelAgent( Serial serial )
            : base( serial )
        {
        }

        public override bool CheckTeach( SkillName skill, Mobile from )
        {
            if( !base.CheckTeach( skill, from ) )
                return false;

            return ( skill == SkillName.Magery )
                || ( skill == SkillName.Forensics )
                || ( skill == SkillName.SpiritSpeak );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich, 2 );
        }

        public override void OnDeath( Container c )
        {
            c.DropItem( new BagOfReagents() );
            c.DropItem( new RecallScroll( Utility.RandomMinMax( 5, 10 ) ) );
            c.DropItem( new GateTravelScroll( Utility.RandomMinMax( 5, 10 ) ) );
            c.DropItem( new Gold( Utility.RandomMinMax( 123, 456 ) ) );
            if( Utility.Random( 20 ) == 0 )
            {
                Spellbook book = new Spellbook();
                book.Name = "Grimorium of Planar Traveling";
                book.Hue = Utility.RandomMetalHue();
                c.DropItem( book );
            }
            base.OnDeath( c );
        }

        public override bool HandlesOnSpeech( Mobile from )
        {
            return true;
        }

        public virtual LocationEntry[] GetLocations()
        {
            return LocationEntry.Locations;
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            PlayerMobile user = e.Mobile as PlayerMobile;

            if( user == null )
                return;

            if( e.Handled || !user.Alive || user.GetDistanceToSqrt( this ) > 4 )
                return;

            if( !e.Handled && user.InRange( this, 4 ) && CanSee( user ) )
            {
                LocationEntry[] locations = GetLocations();

                foreach( LocationEntry le in locations )
                {
                    if( Insensitive.Equals( e.Speech, le.Text ) )
                    {
                        e.Handled = true;

                        Point3D locGateOut = le.Location;
                        Map mapGateOut = le.Map;

                        if( !le.Enabled )
                        {
                            Say( "I'm sorry, the location is not available." );
                        }
                        else if( Factions.Sigil.ExistsOn( user ) )
                        {
                            Say( "You can't do that while carrying the sigil." );
                        }
                        else if( user.Criminal )
                        {
                            Say( "Thou'rt a criminal and cannot escape so easily." );
                        }
                        else if( !mapGateOut.CanSpawnMobile( locGateOut.X, locGateOut.Y, locGateOut.Z ) )
                        {
                            Say( "That location is blocked." );
                        }
                        else if( Banker.GetBalance( user ) < le.Cost )
                        {
                            Say( "Dimensional travelling has a cost... and you cannot pay that." );
                        }
                        else
                        {
                            user.CloseGump( typeof( ConfirmTravelGump ) );
                            user.SendGump( new ConfirmTravelGump( user, this, le ) );
                        }
                    }
                    else
                    {
                        base.OnSpeech( e );
                    }
                }
            }
        }

        public void OpenPortal( Mobile from, bool confirm, LocationEntry le )
        {
            if( confirm )
            {
                Say( "Well, my friend." );
                AnimDelay animTimer = new AnimDelay( this, (PlayerMobile)from, le );
                animTimer.Start();
            }
            else
            {
                Say( "Not a trouble. May the gods guide your journey." );
            }
        }

        public bool CheckLocationEnabled( LocationEntry entry )
        {
            TownSystem sys = TownSystem.Find( Region.Find( Location, Map ) );
            if( sys != null && sys.IsRestricted( entry ) )
                return false;

            return entry.Enabled;
        }

        public LocationEntry PickRandomLocation()
        {
            LocationEntry[] locations = LocationEntry.Locations;

            for( int i = 0; i < 20; i++ )
            {
                LocationEntry temp = locations[ Utility.Random( locations.Length ) ];

                if( !CheckLocationEnabled( temp ) )
                    return temp;
            }

            return locations[ 0 ];
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        private sealed class AnimDelay : Timer
        {
            private MidgardTravelAgent m_MiTrAg;
            private PlayerMobile m_User;
            private LocationEntry m_Le;

            public AnimDelay( MidgardTravelAgent travelagent, PlayerMobile user, LocationEntry le )
                : base( TimeSpan.FromSeconds( 5.0 ) )
            {
                Priority = TimerPriority.OneSecond;
                m_MiTrAg = travelagent;
                m_User = user;
                m_Le = le;
                //Animate( int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay )
                m_MiTrAg.Animate( 263, 7, 1, true, false, 0 );
            }

            protected override void OnTick()
            {
                // Genera un punto a caso attorno al Player che compra il gate
                Point3D locGateIn = m_User.Location;
                locGateIn.X += Utility.RandomMinMax( -2, 2 );
                locGateIn.Y += Utility.RandomMinMax( -2, 2 );

                Map mapGateIn = m_User.Map;

                int random = Utility.RandomMinMax( 1, 100 );
                int chance = m_Le.Difficulty * 5;

                LocationEntry mask = null;
                if( random <= chance )
                    mask = m_MiTrAg.PickRandomLocation();

                Point3D locGateOut = mask != null ? mask.Location : m_Le.Location;
                Map mapGateOut = mask != null ? mask.Map : m_Le.Map;
                int cost = m_Le.Cost;

                Effects.PlaySound( m_MiTrAg.Location, m_MiTrAg.Map, 0x20E );

                InternalItem firstGate = new InternalItem( locGateOut, mapGateOut );
                firstGate.MoveToWorld( locGateIn, mapGateIn );

                InternalItem secondGate = new InternalItem( locGateIn, mapGateIn );
                secondGate.MoveToWorld( locGateOut, mapGateOut );

                m_MiTrAg.Say( "Here it is a dimensional gate..." );
                Banker.Withdraw( m_User, cost );
                m_MiTrAg.Say( "Thank you for {0} gold!", m_Le.Cost );
                m_MiTrAg.Say( "Thy current bank balance is {0} gold.", Banker.GetBalance( m_User ) );

                Stop();
            }
        }

        [DispellableField]
        private sealed class InternalItem : Moongate
        {
            public override bool ShowFeluccaWarning { get { return Core.AOS; } }

            public InternalItem( Point3D target, Map map )
                : base( target, map )
            {
                Map = map;

                if( ShowFeluccaWarning && map == Map.Felucca )
                    ItemID = 0xDDA;

                Dispellable = true;

                InternalTimer t = new InternalTimer( this );
                t.Start();
            }

            #region serialize-deserialize
            public InternalItem( Serial serial )
                : base( serial )
            {
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );
                Delete();
            }
            #endregion

            private class InternalTimer : Timer
            {
                private Item m_Item;

                public InternalTimer( Item item )
                    : base( TimeSpan.FromSeconds( 10.0 ) )
                {
                    Priority = TimerPriority.OneSecond;
                    m_Item = item;
                }

                protected override void OnTick()
                {
                    if( !m_Item.Deleted )
                        m_Item.Delete();
                }
            }
        }

        private sealed class ConfirmTravelGump : SmallConfirmGump
        {
            private Mobile m_From;
            private MidgardTravelAgent m_Agent;
            private LocationEntry m_Entry;

            public ConfirmTravelGump( Mobile from, MidgardTravelAgent agent, LocationEntry le )
                : base( string.Format( "Will I continue for {0} gold?", le.Cost ), null )
            {
                from.CloseGump( typeof( ConfirmTravelGump ) );

                m_From = from;
                m_Agent = agent;
                m_Entry = le;
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                bool join = ( info.ButtonID == 1 );

                m_Agent.OpenPortal( m_From, join, m_Entry );
            }
        }

        public class LocationEntry
        {
            public static LocationEntry[] Locations = new LocationEntry[] { };

            public static LocationEntry[] IlshenarLocations = new LocationEntry[] { };

            public static void Initialize()
            {
                // CommandSystem.Register( "GenTravellerXml", AccessLevel.Developer, delegate { GenerateXml(); } );

                Load();
            }

            public Map Map { get; private set; }

            public Point3D Location { get; private set; }

            public string Text { get; private set; }

            public int Cost { get; private set; }

            public bool Enabled { get; set; }

            public int Difficulty { get; set; }

            public LocationEntry( Map map, Point3D loc, string text, int cost, bool enabled, int difficulty )
            {
                Map = map;
                Location = loc;
                Text = text;
                Cost = cost;
                Enabled = enabled;
                Difficulty = difficulty;
            }

            public LocationEntry( Map map, Point3D loc, string text, int cost )
                : this( map, loc, text, cost, true, 1 )
            {
            }

            public LocationEntry( Map map, Point3D loc, string text, int cost, bool enabled )
                : this( map, loc, text, cost, enabled, 1 )
            {
            }

            public static LocationEntry Parse( XmlElement node )
            {
                Map map = Map.Felucca;
                Point3D location = Point3D.Zero;
                string text = "";
                int cost = 0;
                bool enabled = true;
                int difficulty = 1;

                Region.ReadMap( node, "map", ref map, true );
                Region.ReadPoint3D( node, map, ref location, true );
                Region.ReadString( node, "text", ref text, true );
                Region.ReadInt32( node, "cost", ref cost, true );
                Region.ReadBoolean( node, "enabled", ref enabled, false );
                Region.ReadInt32( node, "difficulty", ref difficulty, false );

                return new LocationEntry( map, location, text, cost, enabled, difficulty );
            }

            internal static void Load()
            {
                if( !File.Exists( "Data/travelAgentDefinitions.xml" ) )
                {
                    Console.WriteLine( "Error: Data/travelAgentDefinitions.xml does not exist" );
                    return;
                }

                if( Core.Debug )
                    Console.Write( "Travel Agent Definitions: Loading..." );

                XmlDocument doc = new XmlDocument();
                doc.Load( Path.Combine( Core.BaseDirectory, "Data/travelAgentDefinitions.xml" ) );

                XmlElement root = doc[ "definitions" ];
                int disabled = 0;

                if( root == null )
                    Console.WriteLine( "Could not find root element 'definitions' in travelAgentDefinitions.xml" );
                else
                {
                    List<LocationEntry> defList = new List<LocationEntry>();

                    foreach( XmlElement info in root.SelectNodes( "entry" ) )
                    {
                        LocationEntry entry = Parse( info );
                        if( !entry.Enabled )
                            disabled++;

                        defList.Add( entry );
                    }

                    Locations = defList.ToArray();
                }

                var ilshenar = new List<LocationEntry>();
                foreach( var entry in Locations )
                {
                    if( entry.Map == Map.Ilshenar )
                        ilshenar.Add( entry );
                }

                IlshenarLocations = ilshenar.ToArray();

                if( Core.Debug )
                {
                    Console.WriteLine( "done" );
                    Console.WriteLine( "Travel Agent List has {0} members, {1} are disabled.", Locations.Length, disabled );
                }
            }

            private static void GenerateXml()
            {
                const string path = "travelAgentDefinitions.xml";

                XmlDocument doc = new XmlDocument();

                using( XmlTextWriter textWriter = new XmlTextWriter( path, null ) )
                {
                    textWriter.Formatting = Formatting.Indented;
                    textWriter.WriteStartElement( "definitions" );
                    textWriter.WriteEndElement();
                    textWriter.Flush();
                }

                using( FileStream fileStream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
                    doc.Load( fileStream );

                foreach( LocationEntry definition in Locations )
                {
                    try
                    {
                        XmlElement element = doc.CreateElement( "entry" );

                        if( definition.Map != null )
                            element.SetAttribute( "map", definition.Map.ToString() );
                        else
                            element.SetAttribute( "map", Map.Felucca.Name );

                        element.SetAttribute( "text", definition.Text );
                        element.SetAttribute( "cost", definition.Cost.ToString() );
                        element.SetAttribute( "x", definition.Location.X.ToString() );
                        element.SetAttribute( "y", definition.Location.Y.ToString() );
                        element.SetAttribute( "z", definition.Location.Z.ToString() );
                        element.SetAttribute( "difficulty", definition.Difficulty.ToString() );

                        if( !definition.Enabled )
                            element.SetAttribute( "enabled", definition.Enabled.ToString() );

                        doc.DocumentElement.InsertAfter( element, doc.DocumentElement.LastChild );
                    }
                    catch( Exception e )
                    {
                        Console.WriteLine( e.ToString() );
                    }
                }

                using( FileStream outStream = new FileStream( path, FileMode.Truncate, FileAccess.Write, FileShare.Write ) )
                    doc.Save( outStream );

                Load();
            }
        }
    }
}