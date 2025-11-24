/***************************************************************************
 *                               BoatDecorator.cs
 *
 *   begin                : 26 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Midgard.Multis
{
    public class BoatDecorator : Item
    {
        #region BoatDecorateCommand enum
        public enum BoatDecorateCommand
        {
            None,

            Turn,
            Up,
            Down,
            LockDown,
            Release,
            Hue
        }
        #endregion

        private BoatDecorateCommand m_Command;

        private int m_DyedHue;

        [Constructable]
        public BoatDecorator()
            : base( 0xFC1 )
        {
            Weight = 1.0;
        }

        #region serialization
        public BoatDecorator( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_DyedHue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_DyedHue = reader.ReadInt();
        }
        #endregion

        public override string DefaultName
        {
            get { return "Boat Decorator"; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public BoatDecorateCommand Command
        {
            get { return m_Command; }
            set
            {
                m_Command = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DyedHue
        {
            get { return m_DyedHue; }
            set
            {
                m_DyedHue = value;
                Hue = value;
            }
        }

        public virtual CustomHuePicker CustomHuePicker
        {
            get { return null; }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !CheckUse( from ) )
                return;

            if( m_Command == BoatDecorateCommand.None )
                from.SendGump( new InternalGump( from, this ) );
            else
                from.Target = new InternalTarget( this );
        }

        private bool CheckUse( Mobile from )
        {
            if( Deleted || !IsChildOf( from.Backpack ) )
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            else
            {
                BaseBoat boat = BaseBoat.FindBoatAt( from.Location, from.Map );

                if( boat != null && ( boat.CanCommand( from ) || from.AccessLevel > AccessLevel.Seer ) )
                    return true;
                else
                    from.SendMessage( "You cannot use this item." );
            }

            return false;
        }

        #region Nested type: InternalGump
        private class InternalGump : TownGump
        {
            private readonly BoatDecorator m_Decorator;

            public InternalGump( Mobile owner, BoatDecorator decorator )
                : base( null, owner, 50, 50 )
            {
                owner.CloseGump( typeof( InternalGump ) );

                m_Decorator = decorator;

                Design();

                base.RegisterUse( typeof( InternalGump ) );
            }

            private void Design()
            {
                AddPage( 0 );

                AddMainBackground();
                AddMainTitle( 150, "Boat Decoration" );
                AddMainWindow();

                int labelOffsetX = GetMainWindowLabelsX();
                int labelOffsetY = GetMainWindowFirstLabelY();

                AddLabel( labelOffsetX, labelOffsetY, TitleHue, "Choose from:" );

                int hue = HuePrim;

                int pageX = HorBorder + MainWindowWidth - 25;
                int pageY = GetMainWindowsStartY() + 10;

                for( int i = 0; i < Enum.GetValues( typeof( BoatDecorateCommand ) ).Length; i++ )
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
                    AddLabel( labelOffsetX + 20, y, GetHueFor( hue ), Enum.GetName( typeof( BoatDecorateCommand ), i ) );
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                BoatDecorateCommand command = BoatDecorateCommand.None;

                sender.Mobile.SendMessage( "You are chosen option number: {0}", info.ButtonID );

                switch( info.ButtonID )
                {
                    case (int)Buttons.Turn:
                        command = BoatDecorateCommand.Turn;
                        break;
                    case (int)Buttons.Up:
                        command = BoatDecorateCommand.Up;
                        break;
                    case (int)Buttons.Down:
                        command = BoatDecorateCommand.Down;
                        break;
                    case (int)Buttons.Lock:
                        command = BoatDecorateCommand.LockDown;
                        break;
                    case (int)Buttons.Release:
                        command = BoatDecorateCommand.Release;
                        break;
                    case (int)Buttons.Hue:
                        command = BoatDecorateCommand.Hue;
                        break;
                }

                if( command != BoatDecorateCommand.None )
                {
                    m_Decorator.Command = command;
                    sender.Mobile.Target = new InternalTarget( m_Decorator );
                }
            }

            private enum Buttons
            {
                Close = 0,

                Turn,
                Up,
                Down,
                Lock,
                Release,
                Hue,

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
        #endregion

        private class InternalTarget : Target
        {
            private readonly BoatDecorator m_Decorator;

            public InternalTarget( BoatDecorator decorator )
                : base( -1, false, TargetFlags.None )
            {
                CheckLOS = true;

                m_Decorator = decorator;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                bool fromGM = from.AccessLevel > AccessLevel.GameMaster;
                BaseBoat boat = BaseBoat.FindBoatAt( from.Location, from.Map );
                if( boat == null )
                    return;

                if( targeted == m_Decorator )
                {
                    m_Decorator.Command = BoatDecorateCommand.None;
                    from.SendGump( new InternalGump( from, m_Decorator ) );
                }
                else if( targeted is Item && m_Decorator.CheckUse( from ) )
                {
                    Item item = (Item)targeted;

                    if( !fromGM && ( item.Parent != null || ( !boat.CanCommand( from ) ) ) )
                    {
                        from.SendMessage( "You cannot do that." );
                    }
                    else
                    {
                        switch( m_Decorator.Command )
                        {
                            case BoatDecorateCommand.Up:
                                Up( item, from );
                                break;
                            case BoatDecorateCommand.Down:
                                Down( item, from );
                                break;
                            case BoatDecorateCommand.Turn:
                                Turn( item, from );
                                break;
                            case BoatDecorateCommand.LockDown:
                                LockDown( item, from );
                                break;
                            case BoatDecorateCommand.Release:
                                Release( item, from );
                                break;
                            case BoatDecorateCommand.Hue:
                                Hue( item, from );
                                break;
                        }
                    }
                }
            }

            private static void Release( Item item, Mobile mobile )
            {
                if( item.IsAccessibleTo( mobile ) )
                {
                    XmlData deco = XmlAttach.FindAttachment( item, typeof( XmlData ), "BoatDecoration" ) as XmlData;
                    if( deco != null )
                    {
                        XmlData hue = XmlAttach.FindAttachment( item, typeof( XmlData ), "OriginalHue" ) as XmlData;
                        if( hue != null )
                        {
                            try
                            {
                                item.Hue = int.Parse( hue.Data );
                            }
                            catch
                            {
                            }

                            if( !hue.Deleted )
                                hue.Delete();

                            mobile.SendMessage( "Original hue restored." );
                        }

                        if( !item.Movable )
                            item.Movable = true;

                        if( !deco.Deleted )
                            deco.Delete();
                    }
                    else
                        mobile.SendMessage( "That is not a boat decoration." );
                }
                else
                    mobile.SendMessage( "Thatis not accessible." );
            }

            private static void LockDown( Item item, Mobile mobile )
            {
                if( item.IsAccessibleTo( mobile ) )
                {
                    XmlAttach.AttachTo( item, new XmlData( "BoatDecoration" ) );
                    XmlAttach.AttachTo( item, new XmlData( "OriginalHue", item.Hue.ToString() ) );

                    item.Movable = false;
                }
                else
                    mobile.SendMessage( "That is not accessible." );
            }

            private void Hue( Item item, Mobile mobile )
            {
                if( item.IsAccessibleTo( mobile ) )
                {
                    if( XmlAttach.FindAttachment( item, typeof( XmlData ), "BoatDecoration" ) != null )
                        item.Hue = m_Decorator.Hue;
                    else
                        mobile.SendMessage( "That is not a boat decoration." );
                }
                else
                    mobile.SendMessage( "Thatis not accessible." );
            }

            private static void Turn( Item item, Mobile from )
            {
                if( item.IsAccessibleTo( from ) )
                {
                    if( item is ISiegeWeapon )
                    {
                        ( (ISiegeWeapon)item ).Turn();
                        return;
                    }
                    else if( item is SiegeComponent )
                    {
                        ( (SiegeComponent)item ).Facing++;
                        return;
                    }

                    if( XmlAttach.FindAttachment( item, typeof( XmlData ), "BoatDecoration" ) != null )
                    {
                        FlipableAttribute[] attributes = (FlipableAttribute[])item.GetType().GetCustomAttributes( typeof( FlipableAttribute ), false );

                        Item addon = null;

                        if( attributes.Length > 0 )
                            attributes[ 0 ].Flip( item );
                        else if( item is AddonComponent )
                            addon = ( (AddonComponent)item ).Addon;
                        else if( item is AddonContainerComponent )
                            addon = ( (AddonContainerComponent)item ).Addon;
                        else if( item is BaseAddonContainer )
                            addon = item;
                        else
                            from.SendLocalizedMessage( 1042273 ); // You cannot turn that.

                        if( addon != null )
                        {
                            FlipableAddonAttribute[] fattributes = (FlipableAddonAttribute[])addon.GetType().GetCustomAttributes( typeof( FlipableAddonAttribute ), false );

                            if( fattributes.Length > 0 )
                                fattributes[ 0 ].Flip( from, addon );
                        }
                    }
                    else
                        from.SendMessage( "That is not a boat decoration." );
                }
                else
                    from.SendMessage( "That is not accessible." );
            }

            private static void Up( Item item, Mobile mobile )
            {
                if( item.IsAccessibleTo( mobile ) )
                {
                    if( XmlAttach.FindAttachment( item, typeof( XmlData ), "BoatDecoration" ) != null )
                        item.Location = new Point3D( item.Location, item.Z + 1 );
                    else
                        mobile.SendMessage( "That is not a boat decoration." );
                }
                else
                    mobile.SendMessage( "Thatis not accessible." );
            }

            private static void Down( Item item, Mobile from )
            {
                if( item.IsAccessibleTo( from ) )
                {
                    if( XmlAttach.FindAttachment( item, typeof( XmlData ), "BoatDecoration" ) != null )
                        item.Location = new Point3D( item.Location, item.Z - 1 );
                    else
                        from.SendMessage( "That is not a boat decoration." );
                }
                else
                    from.SendMessage( "Thatis not accessible." );
            }
        }
    }
}