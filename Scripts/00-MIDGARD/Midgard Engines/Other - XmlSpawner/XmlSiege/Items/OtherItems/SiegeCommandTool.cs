using System;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Midgard.Items
{
    [Flipable( 0x1EBA, 0x1EBB )]
    public class SiegeCommandTool : Item
    {
        private enum SiegeCommand
        {
            None,

            RotatePrev,
            RotateNext,
            Backpack,
            Release,
            Connect
        }

        [Constructable]
        public SiegeCommandTool()
            : base( 0x1EBA )
        {
            Weight = 1.0;
        }

        #region serialization
        public SiegeCommandTool( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        public override string DefaultName
        {
            get { return "siege command tool"; }
        }

        private SiegeCommand Command { get; set; }

        public override void OnDoubleClick( Mobile from )
        {
            if( !CheckUse( this, from ) )
                return;

            if( Command == SiegeCommand.None )
                from.SendGump( new SiegeCommandGump( from, this ) );
            else
            {
                string message = "";
                if( Command == SiegeCommand.Backpack )
                    message = "Select the siege weapon you want to pack.";
                else if( Command == SiegeCommand.Connect )
                    message = "Select the siege weapon you want to drag.";
                else if( Command == SiegeCommand.Release )
                    message = "Select the siege weapon you want to release from being dragged.";
                else if( Command == SiegeCommand.RotateNext || Command == SiegeCommand.RotatePrev )
                    message = "Select the siege weapon you want to rotate.";

                if( message.Length > 0 )
                    from.Target = new InternalTarget( this );
            }
        }

        public bool CheckUse( SiegeCommandTool tool, Mobile from )
        {
            if( tool.Deleted || !tool.IsChildOf( from.Backpack ) )
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            else
                return true;

            return false;
        }

        private class InternalTarget : Target
        {
            private readonly SiegeCommandTool m_Tool;

            public InternalTarget( SiegeCommandTool decorator )
                : base( -1, false, TargetFlags.None )
            {
                CheckLOS = true;

                m_Tool = decorator;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted == m_Tool )
                {
                    m_Tool.Command = SiegeCommand.None;
                    from.SendGump( new SiegeCommandGump( from, m_Tool ) );
                }
                else if( targeted is Item && m_Tool.CheckUse( m_Tool, from ) )
                {
                    Item item = (Item)targeted;

                    switch( m_Tool.Command )
                    {
                        case SiegeCommand.Backpack:
                            Backpack( item, from );
                            break;
                        case SiegeCommand.Connect:
                            ToggleConnection( item, from, true );
                            break;
                        case SiegeCommand.Release:
                            ToggleConnection( item, from, false );
                            break;
                        case SiegeCommand.RotateNext:
                            Rotate( item, from, true );
                            break;
                        case SiegeCommand.RotatePrev:
                            Rotate( item, from, false );
                            break;
                    }
                }
            }

            private static void Rotate( Item item, Mobile mobile, bool next )
            {
                if( item.IsAccessibleTo( mobile ) )
                {
                    SiegeComponent component = item as SiegeComponent;
                    if( component != null )
                    {
                        ISiegeWeapon weapon = component.Addon as ISiegeWeapon;
                        if( weapon != null )
                        {
                            if( !weapon.FixedFacing )
                            {
                                if( next )
                                    weapon.Facing++;
                                else
                                    weapon.Facing--;
                            }
                        }
                    }
                }
            }

            private static void ToggleConnection( Item item, Mobile mobile, bool connect )
            {
                if( item.IsAccessibleTo( mobile ) )
                {
                    SiegeComponent component = item as SiegeComponent;
                    if( component != null )
                    {
                        if( component.IsDraggable )
                        {
                            component.ToggleConnection( mobile, connect );
                        }
                    }
                }
            }

            private static void Backpack( Item item, Mobile mobile )
            {
                if( item.IsAccessibleTo( mobile ) )
                {
                    SiegeComponent component = item as SiegeComponent;
                    if( component != null )
                    {
                        ISiegeWeapon weapon = component.Addon as ISiegeWeapon;
                        if( weapon != null )
                        {
                            if( weapon.IsPackable )
                            {
                                weapon.StoreWeapon( mobile );
                            }
                        }
                    }
                }
            }
        }

        private class SiegeCommandGump : TownGump
        {
            private readonly SiegeCommandTool m_Tool;

            public SiegeCommandGump( Mobile owner, SiegeCommandTool tool )
                : base( null, owner, 50, 50 )
            {
                owner.CloseGump( typeof( SiegeCommandGump ) );

                m_Tool = tool;

                Design();

                base.RegisterUse( typeof( SiegeCommandGump ) );
            }

            private void Design()
            {
                AddPage( 0 );

                AddMainBackground();
                AddMainTitle( 150, "Siege Command" );
                AddMainWindow();

                int labelOffsetX = GetMainWindowLabelsX();
                int labelOffsetY = GetMainWindowFirstLabelY();

                AddLabel( labelOffsetX, labelOffsetY, TitleHue, "Choose from:" );

                int hue = HuePrim;

                int pageX = HorBorder + MainWindowWidth - 25;
                int pageY = GetMainWindowsStartY() + 10;

                for( int i = 0; i < Enum.GetValues( typeof( SiegeCommand ) ).Length; i++ )
                {
                    int page = i / NumLabels;
                    int pos = i % NumLabels;

                    if( pos == 0 )
                    {
                        if( page > 0 )
                            AddButton( pageX, pageY, NextPageBtnIdNormal, NextPageBtnIdPressed, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                        AddPage( page + 1 );

                        if( page > 0 )
                            AddButton( pageX - 20, pageY, PrevPageBtnIdNormal, PrevPageBtnIdPressed, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                    }

                    if( i == 0 )
                        continue; // do not display 'none' case

                    int y = pos * LabelsOffset + labelOffsetY;

                    hue = GetHueFor( hue );

                    AddMainWindowButton( labelOffsetX, y + 5, i, (int) TownAccessFlags.None );
                    AddLabel( labelOffsetX + 20, y, GetHueFor( hue ), Enum.GetName( typeof( SiegeCommand ), i ) );
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                switch( info.ButtonID )
                {
                    case (int)Buttons.RotatePrev:
                        m_Tool.Command = SiegeCommand.RotatePrev;
                        break;
                    case (int)Buttons.RotateNext:
                        m_Tool.Command = SiegeCommand.RotateNext;
                        break;
                    case (int)Buttons.Backpack:
                        m_Tool.Command = SiegeCommand.Backpack;
                        break;
                    case (int)Buttons.Release:
                        m_Tool.Command = SiegeCommand.Release;
                        break;
                    case (int)Buttons.Connect:
                        m_Tool.Command = SiegeCommand.Connect;
                        break;
                }

                sender.Mobile.Target = new InternalTarget( m_Tool );
            }

            private enum Buttons
            {
                Close = 0,

                RotatePrev,
                RotateNext,
                Backpack,
                Release,
                Connect,

                Page = 10000
            }

            protected override int NumLabels
            {
                get { return 7; }
            }

            protected override int NumButtons
            {
                get { return 1; }
            }

            protected override int MainWindowWidth
            {
                get { return 195; }
            }
        }
    }
}