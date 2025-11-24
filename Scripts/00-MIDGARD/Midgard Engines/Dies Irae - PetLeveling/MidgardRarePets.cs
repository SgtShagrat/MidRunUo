/***************************************************************************
 *                                   MidgardRarePets.cs
 *                            		-------------------
 *  begin                	: Marzo, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 *  Info: 
 * 			Sistema per lo spawn di pet rari mediante password e gump.
 * 
 ****************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Midgard.Mobiles;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.PetSystem
{
    public class MidgardRarePets
    {
        private const string PassWord = "LOLASD";

        #region lists
        public static Type[] MidgardMustangsTypes;

        public static Type[] MidgardLamasTypes;

        public static Type[] MidgardOstardsTypes;

        public static Type[] MidgardOtherRaresTypes = new Type[]
		{
			typeof(Raptor),
			typeof(RidableDrake),
			typeof(RidablePolarBear),
			typeof(RidableVortex),
			typeof(InfernalLlama)
		};
        #endregion

        public static void RegisterCommands()
        {
            CommandSystem.Register( "SpawnRarePet", AccessLevel.Administrator, new CommandEventHandler( SpawnRarePet_OnCommand ) );

            ProcessTypes();
        }

        [Usage( "SpawnRarePet" )]
        [Description( "Apre un gump per creare un pet raro" )]
        public static void SpawnRarePet_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 1 )
            {
                string toVerify = e.GetString( 0 );

                if( toVerify != null && toVerify == PassWord )
                {
                    from.SendMessage( "Password verificata con successo." );
                    from.SendGump( new InternalGump( from ) );
                }
                else
                {
                    from.SendGump( new NoticeGump( 1060635, 30720, @"La password non e' corretta." +
                                    " Il formato corretto del comando è: " +
                                    "\n [SpawnRarePet <em>password</em>.",
                                    0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[] { } ) );
                }
            }
            else
            {
                from.SendGump( new NoticeGump( 1060635, 30720, @"Uso del comando in maniera errata." +
                                " Il formato corretto del comando è: " +
                                "\n [SpawnRarePet <em>password</em>.",
                                0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[] { } ) );
            }
        }

        private static void CloseNoticeCallback( Mobile from, object state )
        {
        }

        private static void ProcessTypes()
        {
            var mustangs = new List<Type>();
            var ostards = new List<Type>();

            var llamas = new List<Type>();

            var asms = ScriptCompiler.Assemblies;

            foreach( Assembly asm in asms )
            {
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                foreach( var type in types )
                {
                    if( type.IsSubclassOf( typeof( BaseMustang ) ) )
                        mustangs.Add( type );
                    else if( type.IsSubclassOf( typeof( BaseOstard ) ) )
                        ostards.Add( type );
                    else if( type.IsSubclassOf( typeof( BaseLlama ) ) )
                        llamas.Add( type );
                }
            }

            mustangs.Sort( InternalComparer.Instance );
            ostards.Sort( InternalComparer.Instance );
            llamas.Sort( InternalComparer.Instance );

            MidgardMustangsTypes = mustangs.ToArray();
            MidgardLamasTypes = llamas.ToArray();
            MidgardOstardsTypes = ostards.ToArray();
        }

        private class InternalGump : Gump
        {
            private readonly List<Type> m_MidgardRareSpawns;

            public InternalGump( Mobile from )
                : base( 50, 50 )
            {
                m_MidgardRareSpawns = new List<Type>();
                m_MidgardRareSpawns.AddRange( MidgardMustangsTypes );
                m_MidgardRareSpawns.AddRange( MidgardLamasTypes );
                m_MidgardRareSpawns.AddRange( MidgardOstardsTypes );
                m_MidgardRareSpawns.AddRange( MidgardOtherRaresTypes );

                from.CloseGump( typeof( InternalGump ) );

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage( 0 );
                AddBackground( 0, 0, 590, 480, 83 );
                AddBlackAlphaRegion( 10, 10, 570, 460 );
                AddLabel( 210, 30, 37, @"Midgard Rare Pets Gump:" );

                int hue = 87;
                for( int i = 0; i < m_MidgardRareSpawns.Count; i++ )
                {
                    string tmp = MidgardUtility.GetFriendlyClassName( m_MidgardRareSpawns[ i ].Name );
                    hue = ( hue == 87 ? 92 : 87 );

                    if( i < 20 )
                    {
                        AddButton( 160, 60 + i * 20, 4023, 4024, i + 1, GumpButtonType.Reply, 0 );
                        AddLabel( 20, 64 + i * 20, hue, tmp );
                    }
                    else if( i < 40 )
                    {
                        AddButton( 350, 60 + ( i - 20 ) * 20, 4023, 4024, i + 1, GumpButtonType.Reply, 0 );
                        AddLabel( 210, 64 + ( i - 20 ) * 20, hue, tmp );
                    }
                    else
                    {
                        AddButton( 540, 60 + ( i - 40 ) * 20, 4023, 4024, i + 1, GumpButtonType.Reply, 0 );
                        AddLabel( 400, 64 + ( i - 40 ) * 20, hue, tmp );
                    }
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == 0 )
                {
                    from.CloseGump( typeof( InternalGump ) );
                    return;
                }
                from.Target = new InternalTarget( m_MidgardRareSpawns[ info.ButtonID - 1 ] );
            }
        }

        private class InternalComparer : IComparer<Type>
        {
            public static readonly IComparer<Type> Instance = new InternalComparer();

            public int Compare( Type x, Type y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
        }

        private class InternalTarget : Target
        {
            private readonly Type m_PetType;

            public InternalTarget( Type pettype )
                : base( 12, true, TargetFlags.None )
            {
                m_PetType = pettype;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                IPoint3D ip = o as IPoint3D;
                Map fromMap = from.Map;
                Point3D p = from.Location;

                if( ip != null && fromMap != null )
                {

                    if( ip is Item )
                        ip = ( (Item)ip ).GetWorldTop();

                    p = new Point3D( ip );
                }

                if( p == from.Location )
                {
                    from.SendMessage( "Hai targettato male... il comando non puo' eseguire la creazione del pet." );
                    return;
                }

                try
                {
                    BaseCreature pet = Activator.CreateInstance( m_PetType ) as BaseCreature;
                    if( pet != null )
                    {
                        pet.MoveToWorld( p, fromMap );

                        from.SendMessage( "Creazione del pet raro avvenuta con successo." );
                        Console.WriteLine( "Pet raro di tipo {0} (seriale {1}) creato in data {2} dal pg {3} (seriale {4}).",
                                           pet.GetType().Name, pet.Serial, DateTime.Now,
                                           from.Name, from.Serial );
                    }

                    try
                    {
                        TextWriter tw = File.AppendText( "Logs/Midgard2RarePets.txt" );
                        if( pet != null )
                        {
                            tw.WriteLine( "Pet raro di tipo {0} (seriale {1}) creato in data {2} dal pg {3} (seriale {4}).",
                                             pet.GetType().Name, pet.Serial, DateTime.Now,
                                             from.Name, from.Serial );
                        }
                        tw.Close();
                    }
                    catch( Exception ex )
                    {
                        Console.Write( "Log della creazione di un pet raro fallito: {0}", ex );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "La creazione di un pet raro di tipo {0} da parte del pg staff {1} in data {2} " +
                                       "ha creato la seguente eccezione: {3}",
                                       m_PetType.Name, from.Name, DateTime.Now, ex );
                }
            }
        }
    }
}