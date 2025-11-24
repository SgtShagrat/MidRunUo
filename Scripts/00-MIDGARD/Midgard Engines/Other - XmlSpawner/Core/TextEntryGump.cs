using Server.Gumps;
using Server.Network;

namespace Server.Mobiles
{
    public class TextEntryGump : Gump
    {
        private XmlSpawner m_Spawner;
        private int m_Index;
        private XmlSpawnerGump m_SpawnerGump;

        public TextEntryGump( XmlSpawner spawner, XmlSpawnerGump spawnergump, int index, int x, int y )
            : base( x, y )
        {
            if( spawner == null || spawner.Deleted )
                return;
            m_Spawner = spawner;
            m_Index = index;
            m_SpawnerGump = spawnergump;

            AddPage( 0 );

            AddBackground( 20, 0, 220, 354, 5054 );
            AddAlphaRegion( 20, 0, 220, 354 );
            AddImageTiled( 23, 5, 214, 270, 0x52 );
            AddImageTiled( 24, 6, 213, 261, 0xBBC );

            string label = spawner.Name + " entry " + index;
            AddLabel( 28, 10, 0x384, label );

            // OK button
            AddButton( 25, 325, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0 );
            // Close button
            AddButton( 205, 325, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0 );
            // Edit button
            AddButton( 100, 325, 0xEF, 0xEE, 2, GumpButtonType.Reply, 0 );
            string str = null;
            if( index < m_Spawner.SpawnObjects.Length )
            {
                str = m_Spawner.SpawnObjects[ index ].TypeName;
            }
            // main text entry area
            AddTextEntry( 35, 30, 200, 251, 0, 0, str );

            // editing text entry areas
            // background for text entry area
            AddImageTiled( 23, 275, 214, 23, 0x52 );
            AddImageTiled( 24, 276, 213, 21, 0xBBC );
            AddImageTiled( 23, 300, 214, 23, 0x52 );
            AddImageTiled( 24, 301, 213, 21, 0xBBC );

            AddTextEntry( 35, 275, 200, 21, 0, 1, null );
            AddTextEntry( 35, 300, 200, 21, 0, 2, null );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( info == null || state == null || state.Mobile == null )
                return;

            if( m_Spawner == null || m_Spawner.Deleted )
                return;
            bool updateEntry = false;
            bool editEntry = false;
            switch( info.ButtonID )
            {
                case 0: // Close
                    {
                        break;
                    }
                case 1: // Okay
                    {
                        updateEntry = true;
                        break;
                    }
                case 2: // Edit
                    {
                        editEntry = true;
                        break;
                    }
                default:
                    updateEntry = true;
                    break;
            }
            if( editEntry )
            {
                // get the old text
                TextRelay entry = info.GetTextEntry( 1 );
                string oldtext = entry.Text;
                // get the new text
                entry = info.GetTextEntry( 2 );
                string newtext = entry.Text;
                // make the substitution
                entry = info.GetTextEntry( 0 );
                string origtext = entry.Text;
                if( origtext != null && oldtext != null && newtext != null )
                {
                    try
                    {
                        int firstindex = origtext.IndexOf( oldtext );
                        if( firstindex >= 0 )
                        {
                            int secondindex = firstindex + oldtext.Length;

                            int lastindex = origtext.Length - 1;

                            string editedtext;
                            if( firstindex > 0 )
                            {
                                editedtext = origtext.Substring( 0, firstindex ) + newtext +
                                             origtext.Substring( secondindex, lastindex - secondindex + 1 );
                            }
                            else
                            {
                                editedtext = newtext + origtext.Substring( secondindex, lastindex - secondindex + 1 );
                            }

                            if( m_Index < m_Spawner.SpawnObjects.Length )
                            {
                                m_Spawner.SpawnObjects[ m_Index ].TypeName = editedtext;
                            }
                            else
                            {
                                // Update the creature list
                                m_Spawner.SpawnObjects = m_SpawnerGump.CreateArray( info, state.Mobile );
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                // open a new text entry gump
                state.Mobile.SendGump( new TextEntryGump( m_Spawner, m_SpawnerGump, m_Index, X, Y ) );
                return;
            }
            if( updateEntry )
            {
                TextRelay entry = info.GetTextEntry( 0 );
                if( m_Index < m_Spawner.SpawnObjects.Length )
                {
                    m_Spawner.SpawnObjects[ m_Index ].TypeName = entry.Text;
                }
                else
                {
                    // Update the creature list
                    m_Spawner.SpawnObjects = m_SpawnerGump.CreateArray( info, state.Mobile );
                }
            }
            // Create a new gump

            //m_Spawner.OnDoubleClick( state.Mobile);
            // open a new spawner gump
            state.Mobile.SendGump( new XmlSpawnerGump( m_Spawner, X, Y, m_SpawnerGump.ShowGump, m_SpawnerGump.Xoffset,
                                                       m_SpawnerGump.Page ) );
        }
    }
}