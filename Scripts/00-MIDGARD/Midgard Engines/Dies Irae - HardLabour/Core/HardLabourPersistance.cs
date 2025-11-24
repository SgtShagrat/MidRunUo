/***************************************************************************
 *                                    HardLabourPersistance.cs
 *
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.ContextMenus;

using Server;
using Server.Commands;
using Server.ContextMenus;
using Server.Mobiles;

namespace Midgard.Engines.HardLabour
{
    public class HardLabourPersistance : Item
    {
        private static HardLabourPersistance m_Instance;
        private static int m_HardLabourCounter;
        private Region m_ColonyRegion;

        public static HardLabourPersistance Instance { get { return m_Instance; } }
        public override string DefaultName { get { return "HardLabour Persistance - Internal"; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public static int HardLabourCounter
        {
            get { return m_HardLabourCounter; }
            set { m_HardLabourCounter = value; }
        }

        public Region ColonyRegion
        {
            get { return m_ColonyRegion; }
            set { m_ColonyRegion = value; }
        }

        [Constructable]
        public HardLabourPersistance( Region region )
            : base( 1 )
        {
            Movable = false;

            if( m_Instance == null || m_Instance.Deleted )
            {
                m_Instance = this;

                m_HardLabourCounter = 0;
                m_ColonyRegion = region;
            }
            else
            {
                base.Delete();
            }
        }

        public static void EnsureExistence()
        {
            if( m_Instance == null )
                m_Instance = new HardLabourPersistance( HardLabourCommands.GenerateHardLabourColonyRegion() );
        }

        public override void Delete()
        {
        }

        private void RegisterRegion()
        {
            if( m_ColonyRegion == null )
            {
                Region region = new HardLabourColonyRegion();
                region.Register();
                m_ColonyRegion = region;
            }
        }

        public static void RegisterCommands()
        {
            CommandSystem.Register( "GetPrisonerCounter", AccessLevel.Developer, new CommandEventHandler( GetPrisonersNumber_OnCommand ) );
            CommandSystem.Register( "SetPrisonerCounter", AccessLevel.Developer, new CommandEventHandler( SetPrisonersNumber_OnCommand ) );
        }

        #region serial-deserial
        public HardLabourPersistance( Serial serial )
            : base( serial )
        {
            m_Instance = this;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (int)m_HardLabourCounter );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_HardLabourCounter = reader.ReadInt();
                        break;
                    }
            }

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( RegisterRegion ) );
        }
        #endregion

        [Usage( "GetPrisonerCounter" )]
        [Description( "Returns the prisoner number for hard labour system" )]
        public static void GetPrisonersNumber_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Prisoner counter is: {0}", m_HardLabourCounter );
        }

        [Usage( "SetPrisonerCounter" )]
        [Description( "Sets the prisoner number for hard labour system" )]
        public static void SetPrisonersNumber_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null | e.Length != 1 )
            {
                return;
            }

            try
            {
                m_HardLabourCounter = e.GetInt32( 0 );
            }
            catch
            {
            }
        }

        #region context menus
        public static void GetSelfContextMenus( Mobile from, Mobile player, List<ContextMenuEntry> list )
        {
            if( player.Region.IsPartOf( "Hard Labour Penitentiary" ) )
                list.Add( new CallbackPlayerEntry( 1057, new ContextPlayerCallback( HardLabour. HardLabourCommands.GetHardLabourInfo ), player ) ); // Get info on my hard labour status
        }
        #endregion
    }
}