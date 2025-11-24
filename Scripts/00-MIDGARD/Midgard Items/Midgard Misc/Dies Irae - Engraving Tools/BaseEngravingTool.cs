using System;

using Server;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Midgard.Items
{
    public abstract class BaseEngravingTool : Item
    {
        public static readonly int OSIMaxTextLength = 40;

        private int m_CurCharges;
        private int m_MaxCharges;

        public BaseEngravingTool( int itemID, int charges )
            : base( itemID )
        {
            Hue = 0x48D;
            Weight = 2;

            m_MaxCharges = charges < OSIMaxCharges ? charges : OSIMaxCharges;
            m_CurCharges = m_MaxCharges;
        }

        public BaseEngravingTool( Serial serial )
            : base( serial )
        {
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsInDebugMode { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int CurCharges
        {
            get { return m_CurCharges; }
            set
            {
                int oldValue = m_CurCharges;
                if( oldValue != value )
                {
                    if( value < 0 )
                    {
                        m_CurCharges = 0;
                    }
                    else if( value > MaxCharges )
                    {
                        m_CurCharges = MaxCharges;
                    }
                    else
                    {
                        m_CurCharges = value;
                    }

                    OnCurChargesChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int MaxCharges
        {
            get { return m_MaxCharges; }
            set
            {
                int oldValue = m_MaxCharges;
                if( oldValue != value )
                {
                    if( value < 0 )
                    {
                        m_MaxCharges = 0;
                    }
                    else if( value > OSIMaxCharges )
                    {
                        m_MaxCharges = OSIMaxCharges;
                    }
                    else
                    {
                        m_MaxCharges = value;
                    }

                    OnMaxChargesChanged( oldValue );
                }
            }
        }

        public virtual int EngraverTypeLabelNumber { get { return 0; } }
        public virtual string EngraverTypeLabel { get { return "item"; } }

        public virtual int EngraverTypeCapLabelNumber { get { return 0; } }
        public virtual string TipTypeLabel { get { return ""; } }

        public abstract Type[] Engraves { get; }

        public virtual int OSIMaxCharges
        {
            get { return 1; }
        }

        /// <summary>
        /// Check if from can use this tool.
        /// </summary>
        /// <param name="from">mobile who would use our tool</param>
        /// <param name="sendFailureMessage">true if a message should be sent to from</param>
        /// <returns>true if item is accessible</returns>
        public virtual bool CheckAccess( Mobile from, bool sendFailureMessage )
        {
            if( from == null || from.Deleted || Deleted )
            {
                return false;
            }

            int msgNum;

            if( from.AccessLevel > AccessLevel.Counselor )
            {
                return true;
            }

            if( IsChildOf( from.Backpack ) )
            {
                return true;
            }
            else
            {
                msgNum = 1065632; // Engraving tool must be in your backpack to be used.
            }

            if( sendFailureMessage && msgNum > 0 )
            {
                from.SendLocalizedMessage( msgNum );
            }

            return false;
        }

        /// <summary>
        /// Check if item can be engraved from this tool.
        /// </summary>
        /// <param name="item">item to engrave</param>
        /// <returns>true if item is engravable with our tool</returns>
        public virtual bool CheckItem( Item item )
        {
            if( Engraves == null || item == null || item.Deleted )
            {
                return false;
            }

            Type type = item.GetType();

            foreach( Type t in Engraves )
            {
                if( type == t || type.IsSubclassOf( t ) )
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the tip status label.
        /// </summary>
        /// <returns>int for tip label</returns>
        public virtual int GetTipInfo()
        {
            if( m_MaxCharges < 1 )
            {
                return 1065654; // sanity
            }

            int percUsed = ( m_CurCharges * 10 ) / m_MaxCharges;

            percUsed = Math.Min( percUsed, 10 );
            percUsed = Math.Max( percUsed, 0 );

            return 1065654 + percUsed;
        }

        /// <summary>
        /// Event invoked when current charges of our engraving tool changes
        /// </summary>
        public virtual void OnCurChargesChanged( int oldValue )
        {
            InvalidateProperties();

            if( m_CurCharges < 1 )
            {
                Delete();
            }
        }

        /// <summary>
        /// Event invoked when max charges of our engraving tool changes
        /// </summary>
        public virtual void OnMaxChargesChanged( int oldValue )
        {
            InvalidateProperties();

            if( m_MaxCharges < 1 )
            {
                Delete();
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, StringList.GetClilocString( "", EngraverTypeCapLabelNumber, from.Language ) + " engraving tool" );
        }

        public override void OnDoubleClick( Mobile from )
        {
            base.OnDoubleClick( from );

            if( !CheckAccess( from, true ) )
            {
                return;
            }

            if( m_CurCharges > 0 )
            {
                // Target a ~1_ENGRAVER~ in your backpack you would like to engrave.
                if( EngraverTypeLabelNumber > 0 )
                    from.SendLocalizedMessage( 1065638, string.Format( "#{0}", EngraverTypeLabelNumber ) );
                else
                    from.SendMessage( "Target a {0} in your backpack you would like to engrave.", EngraverTypeLabel );

                from.Target = new EngraveTarget( this );
            }
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( IsInDebugMode );
            writer.Write( m_CurCharges );
            writer.Write( m_MaxCharges );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        IsInDebugMode = reader.ReadBool();
                        m_CurCharges = reader.ReadInt();
                        m_MaxCharges = reader.ReadInt();
                        break;
                    }
            }
        }
        #endregion

        private class EngraveTarget : Target
        {
            private readonly BaseEngravingTool m_Tool;

            public EngraveTarget( BaseEngravingTool tool )
                : base( 2, true, TargetFlags.None )
            {
                m_Tool = tool;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Tool == null || m_Tool.Deleted )
                {
                    return;
                }

                if( targeted is Item )
                {
                    Item item = (Item)targeted;

                    if( item.IsChildOf( from.Backpack ) || m_Tool.AllowNonLocal )
                    {
                        if( item.IsAccessibleTo( from ) )
                        {
                            if( item is IEngravable && m_Tool.CheckItem( item ) )
                            {
                                from.CloseGump( typeof( InternalGump ) );
                                from.SendGump( new InternalGump( m_Tool, item ) );
                            }
                            else
                            {
                                from.SendLocalizedMessage( 1072309 ); // The selected item cannot be engraved by this engraving tool.
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage( 1072310 ); // The selected item is not accessible to engrave.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage( 1065676 ); // Item you would to engrave have to be in your backpack.
                    }
                }
                else
                {
                    from.SendLocalizedMessage( 1072309 ); // The selected item cannot be engraved by this engraving tool.
                }
            }

            protected override void OnTargetOutOfRange( Mobile from, object targeted )
            {
                from.SendLocalizedMessage( 1072310 ); // The selected item is not accessible to engrave.
            }
        }

        protected virtual bool AllowNonLocal
        {
            get { return false; }
        }

        private class InternalGump : Gump
        {
            private readonly Item m_Target;
            private readonly BaseEngravingTool m_Tool;

            public InternalGump( BaseEngravingTool tool, Item target )
                : base( 0, 0 )
            {
                m_Tool = tool;
                m_Target = target;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage( 0 );

                AddBackground( 50, 50, 400, 300, 0xA28 );
                AddHtmlLocalized( 50, 70, 400, 20, 1072359, 0x0, false, false );
                AddHtmlLocalized( 75, 95, 350, 145, 1072360, 0x0, true, true );

                AddButton( 125, 300, 0x81A, 0x81B, (int)Buttons.Okay, GumpButtonType.Reply, 0 );
                AddButton( 320, 300, 0x819, 0x818, (int)Buttons.Cancel, GumpButtonType.Reply, 0 );

                AddImageTiled( 75, 245, 350, 40, 0xDB0 );
                AddImageTiled( 76, 245, 350, 2, 0x23C5 );
                AddImageTiled( 75, 245, 2, 40, 0x23C3 );
                AddImageTiled( 75, 285, 350, 2, 0x23C5 );
                AddImageTiled( 425, 245, 2, 42, 0x23C3 );

                AddTextEntry( 78, 245, 345, 40, 0x0, (int)Buttons.Text, "" );
            }

            public override void OnResponse( NetState state, RelayInfo info )
            {
                if( m_Tool == null || m_Tool.Deleted || m_Target == null || m_Target.Deleted )
                {
                    return;
                }

                if( info.ButtonID != (int)Buttons.Okay )
                {
                    return;
                }

                TextRelay relay = info.GetTextEntry( (int)Buttons.Text );
                if( relay == null )
                {
                    return;
                }

                if( String.IsNullOrEmpty( relay.Text ) )
                {
                    ( (IEngravable)m_Target ).EngravedText = null;
                    state.Mobile.SendLocalizedMessage( 1072362 ); // You remove the engraving from the object.
                }
                else if( relay.Text.Length > OSIMaxTextLength )
                {
                    // The description you entered is too long and cannot be applied. Only ~1_MAXCHAR~ characters are allowed.
                    state.Mobile.SendLocalizedMessage( 1065682, OSIMaxTextLength.ToString() );
                }
                else if( relay.Text.Contains( "<" ) )
                {
                    state.Mobile.SendMessage( "Your description contains unacceptable characters." );
                }
                else
                {
                    ( (IEngravable)m_Target ).EngravedText = relay.Text.Trim();

                    state.Mobile.SendLocalizedMessage( 1072361 ); // You engraved the object.	
                    m_Target.InvalidateProperties();
                    m_Tool.CurCharges -= 1;
                }
            }

            private enum Buttons
            {
                Cancel,
                Okay,
                Text
            }
        }
    }
}