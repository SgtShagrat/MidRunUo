using Midgard.Items;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.Apiculture
{
    public class ApiBeeHiveProductionGump : Gump
    {
        public static readonly bool NeedHiveTool = true; //need a hivetool to harvest resources?

        public static readonly bool PureWax = false; //does the hive produce pure (instead of raw) wax?

        ApiBeeHive m_Hive;

        public ApiBeeHiveProductionGump( Mobile from, ApiBeeHive hive )
            : base( 20, 20 )
        {
            m_Hive = hive;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );

            AddBackground( 37, 133, 205, 54, 3600 );
            AddBackground( 37, 67, 205, 80, 3600 );
            AddBackground( 37, 26, 205, 55, 3600 );


            AddItem( 12, 91, 3307 );
            AddItem( 11, 24, 3307 );
            AddItem( 206, 87, 3307 );
            AddItem( 205, 20, 3307 );

            AddItem( 76, 99, 5154 );
            AddItem( 149, 97, 2540 );

            //honey
            if( m_Hive.HiveStage < HiveStatus.Producing )
            {
                AddLabel( 185, 97, 37, "X" );
            }
            else
            {
                AddLabel( 185, 97, 0x481, m_Hive.Honey.ToString() );
            }

            //wax
            if( m_Hive.HiveStage < HiveStatus.Producing )
            {
                AddLabel( 113, 97, 37, "X" );
            }
            else
            {
                AddLabel( 113, 97, 0x481, m_Hive.Wax.ToString() );
            }

            AddLabel( 110, 43, 92, "Production" );  //title

            AddItem( 44, 47, 6256 );
            AddItem( 191, 151, 2540 );

            AddItem( 42, 153, 5154 );

            AddImage( 162, 96, 212 );
            AddImage( 90, 96, 212 );

            AddButton( 204, 150, 212, 212, (int)Buttons.ButHoney, GumpButtonType.Reply, 0 );
            AddButton( 57, 43, 212, 212, (int)Buttons.ButExit, GumpButtonType.Reply, 0 );
            AddButton( 56, 150, 212, 212, (int)Buttons.ButWax, GumpButtonType.Reply, 0 );
        }

        public enum Buttons
        {
            ButHoney = 1,
            ButExit,
            ButWax,
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
                case (int)Buttons.ButExit: //Exit back to main gump
                    {
                        from.SendGump( new ApiBeeHiveMainGump( from, m_Hive ) );
                        break;
                    }
                case (int)Buttons.ButHoney: //Honey
                    {
                        Item hivetool = GetHiveTool( from );

                        if( NeedHiveTool )
                        {
                            if( hivetool == null || !( hivetool is HiveTool ) )
                            {
                                m_Hive.LabelTo( from, 1065380 ); // You need a hive tool to extract the excess honey!
                                from.SendGump( new ApiBeeHiveProductionGump( from, m_Hive ) );
                                return;
                            }
                        }

                        if( m_Hive.Honey < 3 )
                        {
                            m_Hive.LabelTo( from, 1065381 ); // There isn't enough honey in the hive to fill a bottle!
                            from.SendGump( new ApiBeeHiveProductionGump( from, m_Hive ) );
                            break;
                        }

                        Container pack = from.Backpack;

                        if( pack != null && pack.ConsumeTotal( typeof( Bottle ), 1 ) )
                        {
                            JarHoney honey = new JarHoney();

                            if( !from.PlaceInBackpack( honey ) )
                            {
                                honey.Delete();
                                from.PlaceInBackpack( new Bottle() ); //add the consumed bottle
                                m_Hive.LabelTo( from, 1065382 ); // There is not enough room in your backpack for the honey!
                                from.SendGump( new ApiBeeHiveProductionGump( from, m_Hive ) );
                                break;
                            }

                            if( NeedHiveTool )
                            {
                                ( (HiveTool)hivetool ).UsesRemaining--;
                                if( ( (HiveTool)hivetool ).UsesRemaining < 1 )
                                {
                                    from.SendMessage( "You wear out the hive tool." );
                                    hivetool.Delete();
                                }
                            }

                            m_Hive.Honey -= 3;
                            m_Hive.LabelTo( from, "You fill a bottle with golden honey and place it in your pack." );
                            from.SendGump( new ApiBeeHiveProductionGump( from, m_Hive ) );
                            break;
                        }
                        else
                        {
                            m_Hive.LabelTo( from, "You need a bottle to fill with honey!" );
                            from.SendGump( new ApiBeeHiveProductionGump( from, m_Hive ) );
                            break;
                        }
                    }
                case (int)Buttons.ButWax: //Wax
                    {
                        Item hivetool = GetHiveTool( from );

                        if( NeedHiveTool )
                        {
                            if( hivetool == null || !( hivetool is HiveTool ) )
                            {
                                m_Hive.LabelTo( from, "You need a hive tool to scrape the excess beeswax!" );
                                from.SendGump( new ApiBeeHiveProductionGump( from, m_Hive ) );
                                return;
                            }
                        }

                        if( m_Hive.Wax < 1 )
                        {
                            m_Hive.LabelTo( from, "There isn't enough excess wax in the hive to harvest!" );
                            return;
                        }

                        Item wax;

                        if( PureWax )
                        {
                            wax = new Beeswax( m_Hive.Wax );
                        }
                        else
                        {
                            wax = new RawBeeswax( m_Hive.Wax );
                        }

                        if( !from.PlaceInBackpack( wax ) )
                        {
                            wax.Delete();

                            m_Hive.LabelTo( from, "There is not enough room in your backpack for the wax!" );
                            from.SendGump( new ApiBeeHiveProductionGump( from, m_Hive ) );
                            break;
                        }

                        if( NeedHiveTool )
                        {
                            ( (HiveTool)hivetool ).UsesRemaining--;
                            if( ( (HiveTool)hivetool ).UsesRemaining < 1 )
                            {
                                from.SendMessage( "You wear out the hive tool." );
                                hivetool.Delete();
                            }
                        }

                        m_Hive.Wax = 0;
                        m_Hive.LabelTo( from, "You collect the excess beeswax and place it in your pack." );
                        from.SendGump( new ApiBeeHiveProductionGump( from, m_Hive ) );
                        break;
                    }
            }
        }

        public static Item GetHiveTool( Mobile from )
        {
            if( from.Backpack == null )
                return null;

            Item item = from.Backpack.FindItemByType( typeof( HiveTool ) );

            if( item != null )
                return item;

            return null;
        }
    }
}