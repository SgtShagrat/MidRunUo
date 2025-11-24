using Server.Commands;

/// Allows staff to quickly switch between player and their assigned staff levels by equipping or removing the cloak
/// Also allows instant teleportation to a specified destination when double-clicked by the staff member.
namespace Server.Items
{
    public class StaffCloak : Cloak
    {
        public static void Initialize()
        {
            CommandSystem.Register( "GenStaffCloak", AccessLevel.GameMaster, new CommandEventHandler( GenStaffCloak_OnCommand ) );
        }

        [Usage( "GenStaffCloak" )]
        [Description( "Generate a staff cloak." )]
        private static void GenStaffCloak_OnCommand( CommandEventArgs e )
        {
            e.Mobile.AddToBackpack( new StaffCloak( e.Mobile.AccessLevel ) );
        }

        public override string DefaultName { get { return "a wonderous cloak"; } }

        private AccessLevel m_StaffLevel;

        [CommandProperty( AccessLevel.Administrator )]
        public AccessLevel StaffLevel
        {
            get { return m_StaffLevel; }
            set
            {
                m_StaffLevel = value;
                InvalidateProperties();
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1060658, "Level\t{0}", StaffLevel ); // ~1_val~: ~2_val~
        }

        public override void OnSingleClick( Mobile from )
        {
            if( from.AccessLevel > AccessLevel.Player )
                LabelTo( from, "Level: {0}", StaffLevel );

            base.OnSingleClick( from );
        }

        public override void OnAdded( object parent )
        {
            base.OnAdded( parent );

            // delete this if someone without the necessary access level picks it up or tries to equip it
            if( RootParent is Mobile )
            {
                if( ( (Mobile)RootParent ).AccessLevel != StaffLevel )
                {
                    Delete();
                    return;
                }
            }

            if( parent is Mobile )
            {
                Mobile m = (Mobile)parent;

                if( m.AccessLevel == StaffLevel )
                {
                    m.AccessLevel = AccessLevel.Player;
                    m.Blessed = false;
                }
            }
        }

        public override void OnRemoved( object parent )
        {
            base.OnRemoved( parent );

            if( parent is Mobile && !Deleted )
            {
                Mobile m = (Mobile)parent;
                m.AccessLevel = StaffLevel;
                m.Blessed = true;
            }
        }

        [Constructable]
        public StaffCloak()
        {
            StaffLevel = AccessLevel.Administrator; // assign admin staff level by default
            LootType = LootType.Blessed;
            Weight = 0;
        }

        public StaffCloak( AccessLevel level )
        {
            StaffLevel = level;
            LootType = LootType.Blessed;
            Weight = 0;
        }

        #region serialization
        public StaffCloak( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 );

            // version 0
            writer.Write( (int)m_StaffLevel );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    goto case 0;
                case 0:
                    m_StaffLevel = (AccessLevel)reader.ReadInt();

                    if( version < 1 )
                    {
                        reader.ReadPoint3D();
                        reader.ReadString();
                    }
                    break;
            }
        }
        #endregion
    }
}