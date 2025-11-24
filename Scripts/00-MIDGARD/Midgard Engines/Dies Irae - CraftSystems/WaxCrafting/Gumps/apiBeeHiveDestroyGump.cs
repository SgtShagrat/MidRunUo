using Midgard.Items;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.Apiculture
{
    public class ApiBeeHiveDestroyGump : Gump
    {
        ApiBeeHive m_Hive;

        public ApiBeeHiveDestroyGump( Mobile from, ApiBeeHive hive )
            : base( 20, 20 )
        {
            m_Hive = hive;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );

            AddBackground( 37, 26, 205, 137, 3600 );

            AddItem( 11, 20, 3307 );
            AddItem( 205, 20, 3307 );
            AddItem( 12, 65, 3307 );
            AddItem( 206, 69, 3307 );

            AddLabel( 84, 43, 92, "Destory the hive?" );

            AddItem( 73, 68, 2330 );
            AddItem( 160, 68, 5359 );

            AddImage( 131, 74, 5601 );  //arrow

            AddButton( 83, 114, 1150, 1152, (int)Buttons.ButCancel, GumpButtonType.Reply, 0 );
            AddButton( 166, 115, 1153, 1155, (int)Buttons.ButOkay, GumpButtonType.Reply, 0 );
        }

        public enum Buttons
        {
            ButCancel = 1,
            ButOkay,
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == 0 || m_Hive.Deleted || !from.InRange( m_Hive.GetWorldLocation(), 3 ) )
                return;

            if( !m_Hive.IsAccessibleTo( from ) )
            {
                m_Hive.LabelTo( from, "You cannot use that." );
                return;
            }

            switch( info.ButtonID )
            {
                case (int)Buttons.ButCancel: //cancel
                    {
                        from.SendGump( new ApiBeeHiveMainGump( from, m_Hive ) );
                        break;
                    }
                case (int)Buttons.ButOkay: //okay
                    {
                        ApiBeeHiveDeed deed = new ApiBeeHiveDeed();

                        if( !from.PlaceInBackpack( deed ) )
                        {
                            deed.Delete();

                            m_Hive.LabelTo( from, "You cannot destroy the hive with a full backpack!" );
                            from.SendGump( new ApiBeeHiveMainGump( from, m_Hive ) );

                            break;
                        }

                        m_Hive.Delete();
                        break;
                    }
            }
        }
    }
}