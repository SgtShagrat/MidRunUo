/***************************************************************************
 *                                      Emote.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 *  
 ***************************************************************************/

using System;
using Midgard.Engines.Classes;
using Server;
using Server.Commands;
using Server.Items;

namespace Midgard.Commands
{
    public class Emote
    {
        public static void Initialize()
        {
            CommandSystem.Register( "E", AccessLevel.Player, new CommandEventHandler( Emote_OnCommand ) );
        }

        [Usage( "e \"<suono>\"" )]
        [Description( "Esegue un emote con scritta sul pg, animazione e suono" )]
        public static void Emote_OnCommand( CommandEventArgs e )
        {
            Mobile mobile = e.Mobile;
            if( mobile == null )
                return;

            if( !mobile.Alive )
            {
                mobile.SendMessage( "You cannot do that while dead." );
                return;
            }

            if( e.Length != 1 )
            {
                mobile.SendMessage( "Command use: [e \"<soung>\" " );
                return;
            }

            string emote = e.ArgString.Trim();

            if( ClassSystem.IsPaladine( mobile ) && ( emote.ToLower() == "prega" || emote.ToLower() == "pray" ) )
            {
                PaladinSystem.EmotePray( mobile );
                return;
            }

            foreach( EmoteEntry t in EmoteList )
            {
                if( !Insensitive.Equals( emote, t.Alias ) )
                    continue;

                // Dice la frase in emote
                // TODO verificare se si puo' usare PublicOverHead 
                // in modalita' emote.
                // Pronuncia il suono maschile o femminile
                if( mobile.Female )
                {
                    mobile.Say( t.FraseFemale );
                    mobile.PlaySound( t.SuonoFemale );
                }
                else
                {
                    mobile.Say( t.FraseMale );
                    mobile.PlaySound( t.SuonoMale );
                }

                // Se non a cavallo fa anke l'animzione
                if( !mobile.Mounted && t.Anim > 0 )
                    mobile.Animate( t.Anim, 5, 1, true, false, 0 );

                // Se puke vomita
                if( emote == "Puke" || emote == "puke" )
                    Puke( mobile );
            }
        }

        private static void Puke( Mobile puker )
        {
            Point3D p = new Point3D( puker.Location );
            switch( puker.Direction )
            {
                case Direction.North:
                    p.Y--; break;
                case Direction.South:
                    p.Y++; break;
                case Direction.East:
                    p.X++; break;
                case Direction.West:
                    p.X--; break;
                case Direction.Right:
                    p.X++; p.Y--; break;
                case Direction.Down:
                    p.X++; p.Y++; break;
                case Direction.Left:
                    p.X--; p.Y++; break;
                case Direction.Up:
                    p.X--; p.Y--; break;
                default:
                    break;
            }

            Puke puke = new Puke();
            puke.MoveToWorld( new Point3D( p.X, p.Y, puker.Map.GetAverageZ( p.X, p.Y ) ), puker.Map );
        }

        private class EmoteEntry
        {
            public string Alias { get; private set; }
            public string FraseMale { get; private set; }
            public string FraseFemale { get; private set; }
            public int SuonoMale { get; private set; }
            public int SuonoFemale { get; private set; }
            public int Anim { get; private set; }

            public EmoteEntry( string alias, string frasefemale, string frasemale, int suonofemale, int suonomale, int anim )
            {
                Alias = alias;
                FraseFemale = frasefemale;
                FraseMale = frasemale;
                SuonoFemale = suonofemale;
                SuonoMale = suonomale;
                Anim = anim;
            }

            public EmoteEntry( string alias, string frase, int suonofemale, int suonomale, int anim ) :
                this( alias, frase, frase, suonofemale, suonomale, anim )
            {
            }

            public EmoteEntry( string alias, string frase, int suonofemale, int suonomale ) :
                this( alias, frase, frase, suonofemale, suonomale, 0 )
            {
            }
        }

        #region Lista di emote
        private static readonly EmoteEntry[] EmoteList = new EmoteEntry[]
		{	
			new EmoteEntry( "ah", 			"*ah!*", 					778, 1049 ),
			new EmoteEntry( "ahha", 		"*ah ha!*", 				779, 1050 ),
			new EmoteEntry( "applaud",  	"*applaude*", 				780, 1051 ),
			new EmoteEntry( "asd",			"*se la ride*",				794, 1066 ),
			new EmoteEntry( "blownose", 	"*si soffia il naso*",		781, 1052, 	34	),
			new EmoteEntry( "burp",			"*burp!*", "*rutta!*",		782, 1053,  33	),
			new EmoteEntry( "woohoo",		"*woohoo!*",				783, 1054 ),
			new EmoteEntry( "clearthroat",	"*si schiarisce la voce*",	784, 1055, 	33	),
			new EmoteEntry( "cough",		"*tossisce*",				785, 1056, 	33	),
			new EmoteEntry( "bscough",		"*colpo di tosse*",			786, 1057 ),
			new EmoteEntry( "cry",			"*piange*",					787, 1058 ),
			new EmoteEntry( "fart",			"*prot!*",					792, 1064 ),
			new EmoteEntry( "gasp",			"*trasale*",				793, 1065 ),
			new EmoteEntry( "giggle",		"*ridacchia*",				794, 1066 ),
			new EmoteEntry( "groan",		"*si lamenta*",				795, 1067 ),
			new EmoteEntry( "growl",		"*ringhia*",				796, 1068 ),
			new EmoteEntry( "hey",			"*hey!*",					797, 1069 ),
			new EmoteEntry( "hiccup",		"*singhiozza*",				798, 1070 ),
			new EmoteEntry( "huh",			"*huh?*",					799, 1071 ),
			new EmoteEntry( "kiss",			"*bacia*",					800, 1072 ),
			new EmoteEntry( "no",		 	"*no!*",					802, 1074 ),  	                                                                                                                                                                                                               		
			new EmoteEntry( "oh",			"*oh!*",					803, 1075 ),
			new EmoteEntry( "oooh",			"*oooh!*",					811, 1085 ),
			new EmoteEntry(	"oops",	      	"*oops!*",					812, 1086 ),                                                                                                                                                                                                            		               	
			new EmoteEntry(	"puke",			"*vomita*",					813, 1087, 	32	),	                                                                                                                                                                                                                  		               	
			new EmoteEntry(	"scream" ,		"*aaahh!*",					814, 1088 ),		                                                                                                                                                                                                                  		               	
			new EmoteEntry(	"shush",		"*shh!*",					815, 1089 ),                                                                                                                                                                                                                 		               	
			new EmoteEntry(	"sigh",	    	"*sigh*", 					816, 1090 ),                                                                                                                                                                                                             		               	
			new EmoteEntry(	"sneeze",		"*starnutisce*", 			817, 1091, 	32	),
			new EmoteEntry( "sniff",		"*annusa*", 				818, 1092 ),
			new EmoteEntry(	"snore",		"*russa*", 					819, 1093 ),	                                                                                                                                                                                                                  		
			new EmoteEntry( "spit",			"*sputa*",					820, 1094, 	6	),
			new EmoteEntry(	"whistle",		"*fischia*",				821, 1095, 	5	),	                                                                                                                                                                                                                  		               	
			new EmoteEntry(	"yawn",		 	"*sbadiglia*",				822, 1096 ),                                                                                                                                                                                                                 		               	
			new EmoteEntry(	"yea",		 	"*yea!*",					823, 1097 ),                                                                                                                                                                                                                		               	
			new EmoteEntry(	"yell",			"*urla*",					824, 1098 )
		};
        #endregion
    }
}

namespace Server.Items
{
    public class Puke : Item
    {
        [Constructable]
        public Puke()
            : base( Utility.RandomList( 0xf3b, 0xf3c ) )
        {
            Name = "A Pile of Puke";
            Hue = 0x557;
            Movable = false;

            new InternalTimer( this ).Start();
        }

        public Puke( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, Name );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Delete();
        }

        private class InternalTimer : Timer
        {
            private readonly Item m_Item;

            public InternalTimer( Item item )
                : base( TimeSpan.FromSeconds( 10.0 ) )
            {
                Priority = TimerPriority.OneSecond;
                m_Item = item;
            }

            protected override void OnTick()
            {
                if( m_Item != null && !m_Item.Deleted )
                    m_Item.Delete();
            }
        }
    }
}
