/***************************************************************************
 *                                   WorldForgingPersistance.cs
 *                            		----------------------------
 *  begin                	: Febbraio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 			Oggetto persistente per tenere il conto delle versioni dei muls
 *
 ***************************************************************************/

using Server;
using Server.Commands;

namespace Midgard.Engines.WorldForging
{
    public class WorldForgingPersistance : Item
    {
        public static WorldForgingPersistance Instance { get; private set; }
        public override string DefaultName { get { return "WorldForging Persistance - Internal"; } }

        [CommandProperty( AccessLevel.Developer )]
        public static int MidgardMulsVersion { get; set; }

        [Constructable]
        public WorldForgingPersistance()
            : base( 1 )
        {
            Movable = false;

            if( Instance == null || Instance.Deleted )
            {
                Instance = this;

                MidgardMulsVersion = 0;
            }
            else
            {
                base.Delete();
            }
        }

        public WorldForgingPersistance( Serial serial )
            : base( serial )
        {
            Instance = this;
        }

        public static void EnsureExistence()
        {
            if( Instance == null )
                Instance = new WorldForgingPersistance();
        }

        public override void Delete()
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (int)MidgardMulsVersion );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        MidgardMulsVersion = reader.ReadInt();
                        //					Console.WriteLine( "La versione dei muls è la numero {0}.\n", m_MidgardMulsVersion );
                        break;
                    }
            }
        }
        #endregion

        #region comandi
        public static void Initialize()
        {
            if( WorldForgingCommands.Enabled )
            {
                CommandSystem.Register( "GetNumeroVersioneMuls", AccessLevel.Administrator, new CommandEventHandler( GetNumeroVersioneMuls_OnCommand ) );
                CommandSystem.Register( "SetNumeroVersioneMuls", AccessLevel.Administrator, new CommandEventHandler( SetNumeroVersioneMuls_OnCommand ) );
                
                EnsureExistence();
            }
        }
        #endregion

        #region callback
        public static void GetNumeroVersioneMuls_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "La versione dei muls è la numero {0}.\n", MidgardMulsVersion );
        }

        public static void SetNumeroVersioneMuls_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null | e.Length != 1 )
            {
                return;
            }

            try
            {
                MidgardMulsVersion = e.GetInt32( 0 );
            }
            catch
            {
            }
        }
        #endregion
    }
}