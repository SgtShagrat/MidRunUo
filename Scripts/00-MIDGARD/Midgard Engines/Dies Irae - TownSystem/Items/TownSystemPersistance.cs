/***************************************************************************
 *                                  TownSystemPersistance.cs
 *                            		-------------
 *  begin                	: Gennaio, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Persistenza per la serializzazione dei componenti del TownSystem.
 * 
 ***************************************************************************/

using Server;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownSystemPersistance : Item
    {
        private static TownSystemPersistance m_Instance;

        public static TownSystemPersistance Instance { get { return m_Instance; } }

        public override string DefaultName
        {
            get { return "Midgard Town System Persistance - Internal"; }
        }

        [Constructable]
        public TownSystemPersistance()
            : base( 1 )
        {
            Movable = false;

            if( m_Instance == null || m_Instance.Deleted )
                m_Instance = this;
            else
                base.Delete();
        }

        public TownSystemPersistance( Serial serial )
            : base( serial )
        {
            m_Instance = this;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 2 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 2: 
                    break;
                case 1:
                    {
                        for( int i = 0; i < 4; ++i )
                            TownSystem.TownSystems[ i ].Deserialize( reader );

                        break;
                    }
                case 0:
                    {
                        for( int i = 0; i < 2; ++i )
                            TownSystem.TownSystems[ i ].Deserialize( reader );

                        break;
                    }
            }
        }

        public static void EnsureExistence()
        {
            if( m_Instance == null )
                m_Instance = new TownSystemPersistance();
        }

        public override void Delete()
        {
        }
    }
}