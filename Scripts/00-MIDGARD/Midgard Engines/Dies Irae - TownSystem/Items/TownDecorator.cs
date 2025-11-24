using System;

using Midgard;
using Midgard.Engines.MidgardTownSystem;

using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public enum TownDecorateCommand
    {
        None,
        Turn,
        Up,
        Down,
        Lock,
        Unlock,
        Hue,
        Block,
        Release,
        Waypoints
    }

    public class TownDecorator : Item
    {
        private TownDecorateCommand m_Command;
        private int m_DyedHue;

        [Constructable]
        public TownDecorator()
            : base( 0xFC1 )
        {
            Weight = 1.0;
        }

        #region serialization
        public TownDecorator( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_DyedHue );

            TownSystem.WriteReference( writer, System );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_DyedHue = reader.ReadInt();

            System = TownSystem.ReadReference( reader );
        }
        #endregion

        [CommandProperty( AccessLevel.GameMaster )]
        public TownSystem System { get; set; }

        public override string DefaultName
        {
            get { return "Town Decorator"; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TownDecorateCommand Command
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
            if( !CheckUse( this, from ) )
                return;

            if( m_Command == TownDecorateCommand.None )
                from.SendGump( new TownDecoratorGump( System, from, this ) );
            else
                from.Target = new InternalTarget( this );
        }

        public bool CheckUse( TownDecorator tool, Mobile from )
        {
            if( tool.Deleted || !tool.IsChildOf( from.Backpack ) )
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            else
            {
                if( from.AccessLevel > AccessLevel.Seer )
                    return true;

                TownSystem system = TownSystem.Find( from );
                if( system != null && system.HasAccess( TownAccessFlags.DecorateTown, from ) )
                    return true;
                else
                    from.SendMessage( "You cannot use this item." );
            }

            return false;
        }

        private class InternalTarget : Target
        {
            private readonly TownDecorator m_Decorator;

            public InternalTarget( TownDecorator decorator )
                : base( -1, false, TargetFlags.None )
            {
                CheckLOS = true;

                m_Decorator = decorator;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                bool fromGM = from.AccessLevel > AccessLevel.GameMaster;

                if( targeted == m_Decorator )
                {
                    m_Decorator.Command = TownDecorateCommand.None;
                    from.SendGump( new TownDecoratorGump( m_Decorator.System, from, m_Decorator ) );
                }
                else if( targeted is Item && m_Decorator.CheckUse( m_Decorator, from ) )
                {
                    Item item = (Item)targeted;

                    if( !fromGM && ( item.Parent != null || TownSystem.Find( item.Location, item.Map ) != m_Decorator.System ) )
                    {
                        from.SendMessage( "That is not in your town." );
                    }
                    else
                    {
                        switch( m_Decorator.Command )
                        {
                            case TownDecorateCommand.Up:
                                Up( item, from );
                                break;
                            case TownDecorateCommand.Down:
                                Down( item, from );
                                break;
                            case TownDecorateCommand.Turn:
                                Turn( item, from );
                                break;
                            case TownDecorateCommand.Lock:
                                Lock( item, from );
                                break;
                            case TownDecorateCommand.Unlock:
                                Unlock( item, from );
                                break;
                            case TownDecorateCommand.Hue:
                                Hue( item, from );
                                break;
                            case TownDecorateCommand.Block:
                                Block( item, from );
                                break;
                            case TownDecorateCommand.Release:
                                Release( item, from );
                                break;
                            case TownDecorateCommand.Waypoints:
                                TownWayPoint.StartWayPointChain( item, from );
                                break;
                        }
                    }
                }
            }

            private static void Unlock( Item item, Mobile from )
            {
                if( !item.IsAccessibleTo( from ) )
                    return;

                XmlData deco = XmlAttach.FindAttachment( item, typeof( XmlData ), "TownDecoration" ) as XmlData;
                if( deco != null )
                {
                    if( item is ILockable )
                    {
                        ( (ILockable)item ).Locked = false;
                        from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "You unlocked that." );

                        if( item is BaseDoor )
                        {
                            BaseDoor door = (BaseDoor)item;
                            Key.RemoveKeys( from, door.KeyValue );

                            door.KeyValue = 0;
                            if( door.Link != null )
                            {
                                door.Link.KeyValue = 0;
                                door.Link.Locked = false;
                            }
                        }
                    }
                    else
                        from.SendMessage( "That is not lockable." );
                }
                else
                    from.SendMessage( "That is not a town decoration." );
            }

            /// <summary>
            /// Lock the town decoration. Usable only on ILockable items.
            /// </summary>
            private static void Lock( Item item, Mobile from )
            {
                if( !item.IsAccessibleTo( from ) )
                    return;

                if( item is BaseDoor )
                    XmlAttach.AttachTo( item, new XmlData( "TownDecoration" ) );

                if( XmlAttach.FindAttachment( item, typeof( XmlData ), "TownDecoration" ) != null )
                {
                    if( item is ILockable )
                    {
                        ( (ILockable)item ).Locked = true;

                        if( item is BaseDoor )
                        {
                            BaseDoor door = (BaseDoor)item;

                            // get a random key value
                            uint value = Key.RandomValue();

                            // set the door
                            door.KeyValue = value;
                            if( door.Link != null )
                            {
                                door.Link.KeyValue = value;
                                door.Link.Locked = true;
                            }

                            // make a new kay for that door, assign value and place in owner's pack
                            Key packKey = new Key( KeyType.Iron, value, door.Link );
                            Key bankKey = new Key( KeyType.Iron, value, door.Link );

                            string name = String.Format( "key for \"{0}\", location {1}", MidgardUtility.GetFriendlyClassName( item.GetType().Name ), item.Location );

                            packKey.Name = name;
                            bankKey.Name = name;

                            BankBox box = from.BankBox;
                            if( !box.TryDropItem( from, bankKey, false ) )
                                bankKey.Delete();
                            else
                                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "A key has been placed in your bankbox." );

                            if( from.AddToBackpack( packKey ) )
                                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "A key has been placed in your backpack" );
                            else
                                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "A key has been placed at your feet." );
                        }
                    }
                    else
                        from.SendMessage( "That is not lockable." );
                }
                else
                    from.SendMessage( "That is not a town decoration." );
            }

            /// <summary>
            /// Change the town decoration hue.
            /// </summary>
            private void Hue( Item item, Mobile mobile )
            {
                if( !item.IsAccessibleTo( mobile ) )
                    return;

                if( XmlAttach.FindAttachment( item, typeof( XmlData ), "TownDecoration" ) != null )
                    item.Hue = m_Decorator.Hue;
                else
                    mobile.SendMessage( "That is not a town decoration." );
            }

            /// <summary>
            /// Flip the town decoration.
            /// </summary>
            private static void Turn( Item item, Mobile from )
            {
                if( !item.IsAccessibleTo( from ) )
                    return;

                if( XmlAttach.FindAttachment( item, typeof( XmlData ), "TownDecoration" ) != null )
                {
                    FlipableAttribute[] attributes = (FlipableAttribute[])item.GetType().GetCustomAttributes( typeof( FlipableAttribute ), false );

                    Item addon = null;

                    if( attributes.Length > 0 )
                        attributes[ 0 ].Flip( item );

                    #region Heritage Items
                    else if( item is AddonComponent )
                    {
                        addon = ( (AddonComponent)item ).Addon;
                    }
                    else if( item is AddonContainerComponent )
                    {
                        addon = ( (AddonContainerComponent)item ).Addon;
                    }
                    else if( item is BaseAddonContainer )
                    {
                        addon = item;
                    }
                    #endregion

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
                    from.SendMessage( "That is not a town decoration." );
            }

            /// <summary>
            /// Increase the town decoration altitude.
            /// </summary>
            private static void Up( Item item, Mobile mobile )
            {
                if( !item.IsAccessibleTo( mobile ) )
                    return;

                if( XmlAttach.FindAttachment( item, typeof( XmlData ), "TownDecoration" ) != null )
                {
                    item.Location = new Point3D( item.Location, item.Z + 1 );
                }
                else
                    mobile.SendMessage( "That is not a town decoration." );
            }

            /// <summary>
            /// Decrease the town decoration altitude.
            /// </summary>
            private static void Down( Item item, Mobile from )
            {
                if( !item.IsAccessibleTo( from ) )
                    return;

                if( XmlAttach.FindAttachment( item, typeof( XmlData ), "TownDecoration" ) != null )
                {
                    item.Location = new Point3D( item.Location, item.Z - 1 );
                }
                else
                    from.SendMessage( "That is not a town decoration." );
            }

            /// <summary>
            /// Set a town decoration movable.
            /// Also remove decoration hue.
            /// </summary>
            private static void Release( Item item, Mobile mobile )
            {
                if( !item.IsAccessibleTo( mobile ) )
                    return;

                XmlData deco = XmlAttach.FindAttachment( item, typeof( XmlData ), "TownDecoration" ) as XmlData;
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

                    item.Movable = true;

                    mobile.SendMessage( "That is now movable." );

                    if( !deco.Deleted )
                        deco.Delete();
                }
                else
                    mobile.SendMessage( "That is not a town decoration." );
            }

            /// <summary>
            /// Set an item locked down (not movable)
            /// </summary>
            private static void Block( Item item, Mobile mobile )
            {
                if( !item.IsAccessibleTo( mobile ) )
                    return;

                XmlAttach.AttachTo( item, new XmlData( "TownDecoration" ) );
                XmlAttach.AttachTo( item, new XmlData( "OriginalHue", item.Hue.ToString() ) );

                if( item.Movable )
                {
                    item.Movable = false;
                    mobile.SendMessage( "That is now not movable anymore." );
                }
            }
        }

        private class TownDecoratorGump : TownGump
        {
            private readonly TownDecorator m_Decorator;
            private readonly bool m_FromGM;

            public TownDecoratorGump( TownSystem system, Mobile owner, TownDecorator decorator )
                : base( system, owner, 50, 50 )
            {
                owner.CloseGump( typeof( TownDecoratorGump ) );

                m_Decorator = decorator;
                m_FromGM = owner.AccessLevel > AccessLevel.GameMaster;

                Design();

                base.RegisterUse( typeof( TownDecoratorGump ) );
            }

            private void Design()
            {
                AddPage( 0 );

                AddMainBackground();
                AddMainTitle( 150, "Town Decoration" );
                AddMainWindow();

                int labelOffsetX = GetMainWindowLabelsX();
                int labelOffsetY = GetMainWindowFirstLabelY();

                AddLabel( labelOffsetX, labelOffsetY, TitleHue, "Choose from:" );

                int hue = HuePrim;

                int pageX = HorBorder + MainWindowWidth - 25;
                int pageY = GetMainWindowsStartY() + 10;

                for( int i = 0; i < Enum.GetValues( typeof( TownDecorateCommand ) ).Length; i++ )
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

                    AddMainWindowButton( labelOffsetX, y + 5, i, (int)TownAccessFlags.DecorateTown );
                    AddLabel( labelOffsetX + 20, y, GetHueFor( hue ), Enum.GetName( typeof( TownDecorateCommand ), i ) );
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                TownPlayerState tps = TownPlayerState.Find( Owner );
                if( !FromGm && tps == null )
                {
                    sender.Mobile.SendMessage( "You are not a valid citizen." );
                    return;
                }

                TownDecorateCommand command = TownDecorateCommand.None;

                switch( info.ButtonID )
                {
                    case (int)Buttons.Turn:
                        command = TownDecorateCommand.Turn;
                        break;
                    case (int)Buttons.Up:
                        command = TownDecorateCommand.Up;
                        break;
                    case (int)Buttons.Down:
                        command = TownDecorateCommand.Down;
                        break;
                    case (int)Buttons.Lock:
                        command = TownDecorateCommand.Lock;
                        break;
                    case (int)Buttons.Unlock:
                        command = TownDecorateCommand.Unlock;
                        break;
                    case (int)Buttons.Hue:
                        command = TownDecorateCommand.Hue;
                        break;
                    case (int)Buttons.Block:
                        command = TownDecorateCommand.Block;
                        break;
                    case (int)Buttons.Release:
                        command = TownDecorateCommand.Release;
                        break;
                    case (int)Buttons.Waypoints:
                        command = TownDecorateCommand.Waypoints;
                        break;
                }

                if( command != TownDecorateCommand.None )
                {
                    m_Decorator.Command = command;
                    sender.Mobile.Target = new InternalTarget( m_Decorator );
                }
            }

            #region Nested type: Buttons
            private enum Buttons
            {
                Close = 0,

                Turn,
                Up,
                Down,
                Lock,
                Unlock,
                Hue,
                Block,
                Release,
                Waypoints,

                Page = 10000
            }
            #endregion

            #region design variables
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

            private bool FromGm
            {
                get { return m_FromGM; }
            }
            #endregion
        }
    }
}