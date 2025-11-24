using System;

using Midgard.Engines.Apiculture;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Midgard.Items
{
    public class ApiWaxProcessingPot : Item
    {
        public override int LabelNumber { get { return 1065264; } } // Wax Processing Pot

        public static readonly int MaxWax = 5; //the maximuum amount the pot can hold

        private int m_UsesRemaining;
        private int m_RawBeeswax;
        private int m_PureBeeswax;

        [CommandProperty( AccessLevel.GameMaster )]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int RawBeeswax
        {
            get { return m_RawBeeswax; }
            set { if( value < 0 )value = 0; if( value > MaxWax )value = MaxWax; m_RawBeeswax = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PureBeeswax
        {
            get { return m_PureBeeswax; }
            set { if( value < 0 )value = 0; if( value > MaxWax )value = MaxWax; m_PureBeeswax = value; InvalidateProperties(); }
        }

        [Constructable]
        public ApiWaxProcessingPot()
            : this( 50 )
        {
        }

        [Constructable]
        public ApiWaxProcessingPot( int uses )
            : base( 2532 )
        {
            m_UsesRemaining = uses;
            Weight = 3.0;
            m_RawBeeswax = 0;
            m_PureBeeswax = 0;
        }

        public ApiWaxProcessingPot( Serial serial )
            : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~

            if( PureBeeswax < 1 && RawBeeswax < 1 )
                list.Add( 1049644, "Empty" );
            else if( PureBeeswax > 0 )
            {
                list.Add( 1060663, "{0}\t{1}", "Wax", PureBeeswax.ToString() );
                list.Add( 1049644, "Rendered" );
            }
            else
            {
                list.Add( 1060663, "{0}\t{1}", "Wax", RawBeeswax.ToString() );
                list.Add( 1049644, "Raw" );
            }
        }

        public virtual void DisplayDurabilityTo( Mobile m )
        {
            LabelToAffix( m, 1017323, AffixType.Append, ": " + m_UsesRemaining ); // Durability
        }

        public override void OnSingleClick( Mobile from )
        {
            DisplayDurabilityTo( from );

            base.OnSingleClick( from );
        }

        public override void OnDoubleClick( Mobile from )
        {
            from.SendGump( new ApiBeeHiveSmallPotGump( from, this ) );
        }

        public void BeginAdd( Mobile from )
        {
            if( m_RawBeeswax < MaxWax )
                from.Target = new AddWaxTarget( this );
            else
                from.PrivateOverheadMessage( 0, 1154, 1065350, from.NetState ); // The pot cannot hold any more raw beeswax.
        }

        public void EndAdd( Mobile from, object o )
        {
            if( o is Item && ( (Item)o ).IsChildOf( from.Backpack ) )
            {
                if( o is RawBeeswax )
                {
                    RawBeeswax wax = (RawBeeswax)o;

                    if( ( wax.Amount + RawBeeswax ) > MaxWax )
                    {
                        wax.Amount -= ( MaxWax - RawBeeswax );
                        RawBeeswax = MaxWax;
                    }
                    else
                    {
                        RawBeeswax += wax.Amount;
                        wax.Delete();
                    }

                    from.PrivateOverheadMessage( 0, 1154, 1065351, from.NetState ); // You put raw beeswax in the pot

                    if( from.HasGump( typeof( ApiBeeHiveSmallPotGump ) ) )
                        from.CloseGump( typeof( ApiBeeHiveSmallPotGump ) );

                    from.SendGump( new ApiBeeHiveSmallPotGump( from, this ) ); //resend the gump

                    if( m_RawBeeswax < MaxWax )
                        BeginAdd( from );
                }
                else
                    from.PrivateOverheadMessage( 0, 1154, 1065352, from.NetState ); // You can only put raw beeswax in the pot.
            }
            else
            {
                from.PrivateOverheadMessage( 0, 1154, 1065353, from.NetState ); // The wax must be in your pack to target it.
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
            writer.Write( (int)m_UsesRemaining );
            writer.Write( (int)m_RawBeeswax );
            writer.Write( (int)m_PureBeeswax );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_UsesRemaining = reader.ReadInt();
                        m_RawBeeswax = reader.ReadInt();
                        m_PureBeeswax = reader.ReadInt();
                        break;
                    }
            }
        }

        private class AddWaxTarget : Target
        {
            private ApiWaxProcessingPot m_pot;

            public AddWaxTarget( ApiWaxProcessingPot pot )
                : base( 18, false, TargetFlags.None )
            {
                m_pot = pot;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_pot.Deleted || !m_pot.IsChildOf( from.Backpack ) )
                    return;

                m_pot.EndAdd( from, targeted );
            }
        }

        private class ApiBeeHiveSmallPotGump : Gump
        {
            ApiWaxProcessingPot m_pot = null;

            private static bool GiveSlumgum = true;  //does rendering produce slumgum? (impurities in wax)

            public ApiBeeHiveSmallPotGump( Mobile from, ApiWaxProcessingPot pot )
                : base( 20, 20 )
            {
                m_pot = pot;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage( 0 );

                AddBackground( 15, 12, 352, 140, 9270 );
                AddAlphaRegion( 30, 27, 321, 109 );
                AddImage( 326, 110, 210 );

                //vines
                AddItem( 10, 5, 3311 );
                AddItem( 11, 49, 3311 );
                AddItem( 328, 50, 3307 );
                AddItem( 327, 3, 3307 );

                //pot image
                if( m_pot.PureBeeswax > 0 )
                    AddItem( 231, 105, 0x142B );
                else
                    AddItem( 231, 105, 2532 );

                //labels
                AddLabel( 76, 71, 1153, "Render Beeswax" );
                AddLabel( 76, 40, 1153, "Add Raw Beeswax" );
                AddLabel( 76, 101, 1153, "Empty Pot" );
                AddLabel( 331, 110, 1153, "?" );

                //buttons
                AddButton( 42, 39, 4005, 4006, (int)Buttons.CmdAddRaw, GumpButtonType.Reply, 0 );
                AddButton( 42, 70, 4005, 4006, (int)Buttons.CmdRenderWax, GumpButtonType.Reply, 0 );
                AddButton( 42, 102, 4005, 4006, (int)Buttons.CmdEmptyPot, GumpButtonType.Reply, 0 );
                AddButton( 326, 110, 212, 212, (int)Buttons.CmdHelp, GumpButtonType.Reply, 0 );

                //wax amounts
                AddLabel( 207, 40, 1153, "Raw Beeswax: " + m_pot.RawBeeswax );
                AddLabel( 207, 71, 1153, "Pure Beeswax: " + m_pot.PureBeeswax );
            }

            private enum Buttons
            {
                CmdAddRaw = 1,
                CmdRenderWax,
                CmdEmptyPot,
                CmdHelp
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == 0 || m_pot.Deleted || !from.InRange( m_pot.GetWorldLocation(), 3 ) )
                    return;

                if( !m_pot.IsAccessibleTo( from ) )
                {
                    from.PrivateOverheadMessage( 0, 1154, 1065354, from.NetState ); // I cannot use that.
                    return;
                }

                switch( info.ButtonID )
                {
                    case (int)Buttons.CmdHelp:
                        {
                            from.SendGump( new ApiBeeHiveSmallPotGump( from, m_pot ) );
                            from.SendGump( new ApiBeeHiveHelpGump( from, 1 ) );
                            break;
                        }
                    case (int)Buttons.CmdAddRaw: //Add Raw Honey
                        {
                            from.SendGump( new ApiBeeHiveSmallPotGump( from, m_pot ) );

                            if( m_pot.PureBeeswax > 0 )
                            {
                                from.PrivateOverheadMessage( 0, 1154, 1065355, from.NetState ); // You cannot mix raw beeswax with rendered wax.  Please empty the pot first.
                                return;
                            }

                            from.PrivateOverheadMessage( 0, 1154, 1065356, from.NetState ); // Choose the raw beeswax you wish to add to the pot.
                            m_pot.BeginAdd( from );

                            break;
                        }
                    case (int)Buttons.CmdEmptyPot: //Empty the pot
                        {
                            if( m_pot.PureBeeswax < 1 && m_pot.RawBeeswax < 1 )
                            {
                                from.PrivateOverheadMessage( 0, 1154, 1065357, from.NetState ); // There is no wax in the pot.
                                from.SendGump( new ApiBeeHiveSmallPotGump( from, m_pot ) );
                                return;
                            }

                            Item wax;

                            if( m_pot.PureBeeswax > 0 )
                            {
                                wax = new Beeswax( m_pot.PureBeeswax );
                            }
                            else
                            {
                                wax = new RawBeeswax( m_pot.RawBeeswax );
                            }

                            if( !from.PlaceInBackpack( wax ) )
                            {
                                wax.Delete();
                                from.PrivateOverheadMessage( 0, 1154, 1065358, from.NetState ); // There is not enough room in your backpack for the wax!
                                from.SendGump( new ApiBeeHiveSmallPotGump( from, m_pot ) );
                                break;
                            }

                            m_pot.RawBeeswax = 0;
                            m_pot.PureBeeswax = 0;

                            m_pot.ItemID = 2532; //empty pot

                            from.SendGump( new ApiBeeHiveSmallPotGump( from, m_pot ) );
                            from.PrivateOverheadMessage( 0, 1154, 1065359, from.NetState ); // You place the beeswax in your pack

                            break;
                        }
                    case (int)Buttons.CmdRenderWax: //render the wax
                        {
                            if( m_pot.UsesRemaining < 1 )
                            {//no uses remaining
                                from.PrivateOverheadMessage( 0, 1154, 1065360, from.NetState ); // The pot is too damamged to render beeswax
                                from.SendGump( new ApiBeeHiveSmallPotGump( from, m_pot ) );
                                return;
                            }
                            else if( m_pot.PureBeeswax > 1 )
                            {//already rendered
                                from.PrivateOverheadMessage( 0, 1154, 1065361, from.NetState ); // The pot is already full of rendered beeswax.
                                from.SendGump( new ApiBeeHiveSmallPotGump( from, m_pot ) );
                                return;
                            }
                            else if( m_pot.RawBeeswax < 1 )
                            {//not enough raw beeswax
                                from.PrivateOverheadMessage( 0, 1154, 1065362, from.NetState ); // There is not enough raw beeswax in the pot.
                                from.SendGump( new ApiBeeHiveSmallPotGump( from, m_pot ) );
                                return;
                            }
                            else if( !BeeHiveHelper.Find( from, BeeHiveHelper.m_HeatSources ) )
                            {//need a heat source to melt the wax
                                from.PrivateOverheadMessage( 0, 1154, 1065363, from.NetState ); // You must be near a heat source to render beeswax.
                                from.SendGump( new ApiBeeHiveSmallPotGump( from, m_pot ) );
                                return;
                            }

                            m_pot.ItemID = 0x142b; //pot overflowing with wax

                            m_pot.UsesRemaining--;
                            if( m_pot.UsesRemaining < 0 )
                                m_pot.UsesRemaining = 0;

                            int waste = Utility.RandomMinMax( 1, m_pot.RawBeeswax / 5 );

                            if( GiveSlumgum )
                            {//give slumgum
                                Item gum = new Slumgum( Math.Max( 1, waste ) );

                                if( !from.PlaceInBackpack( gum ) )
                                    gum.Delete();
                            }

                            from.PlaySound( 0x21 );
                            from.PrivateOverheadMessage( 0, 1154, 1065364, from.NetState ); // You slowly melt the raw beeswax and remove the impurities.

                            m_pot.PureBeeswax = m_pot.RawBeeswax - waste;
                            m_pot.RawBeeswax = 0;

                            break;
                        }
                }
            }
        }
    }
}
