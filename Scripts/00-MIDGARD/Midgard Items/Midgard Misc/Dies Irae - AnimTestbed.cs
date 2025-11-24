/***************************************************************************
 *                                  AnimTestbed.cs
 *                            		--------------
 *  begin                	: Febbraio, 2008
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/
/***************************************************************************
 * 
 * 	Info:
 * 		Comando per testare animazioni, bodyvalue e colori.
 * 
 ***************************************************************************/
using Server;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.TestBed
{
    public class AnimTestbed : Item
    {
        public int m_Action;
        public int m_FrameCount;
        public int m_RepeatCount;
        public bool m_Forward;
        public bool m_Repeat;
        public int m_Delay;
        public int m_BodyValue;
        public int m_TextHue;
        public string m_Notes;
        public int m_HueMobile;

        [Constructable]
        public AnimTestbed()
            : base( 0x139C )
        {
            Weight = 1.0;
            LootType = LootType.Blessed;

            m_Action = 1;
            m_FrameCount = 5;
            m_RepeatCount = 1;
            m_Forward = true;
            m_Repeat = true;
            m_Delay = 0;
            m_BodyValue = 400;
            m_TextHue = 2417;
            m_Notes = "";
            m_HueMobile = 2117;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.AccessLevel >= AccessLevel.Administrator )
            {
                from.CloseGump( typeof( AnimTestbedGump ) );
                from.SendGump( new AnimTestbedGump( from, this ) );
            }
            else
            {
                from.SendMessage( "This is for Administrators or higher ONLY!" );
                Delete();
            }
        }

        public AnimTestbed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.WriteEncodedInt( 0 ); // version

            writer.Write( m_Action );
            writer.Write( m_FrameCount );
            writer.Write( m_RepeatCount );
            writer.Write( m_Forward );
            writer.Write( m_Repeat );
            writer.Write( m_Delay );
            writer.Write( m_BodyValue );
            writer.Write( m_TextHue );
            writer.Write( m_Notes );
            writer.Write( m_HueMobile );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadEncodedInt();

            m_Action = reader.ReadInt();
            m_FrameCount = reader.ReadInt();
            m_RepeatCount = reader.ReadInt();
            m_Forward = reader.ReadBool();
            m_Repeat = reader.ReadBool();
            m_Delay = reader.ReadInt();
            m_BodyValue = reader.ReadInt();
            m_TextHue = reader.ReadInt();
            m_Notes = reader.ReadString();
            m_HueMobile = reader.ReadInt();
        }
    }

    public class AnimTestbedGump : Gump
    {
        private AnimTestbed m_AT;

        public static int NegProtection( string numberstring )
        {
            int ns = Utility.ToInt32( numberstring );

            if( ns < 0 )
                return 0;
            else
                return ns;
        }

        public AnimTestbedGump( Mobile from, AnimTestbed at )
            : base( 0, 0 )
        {
            m_AT = at;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );

            AddLabel( 25, 40, 1168, @"Animate and BodyValue" );
            AddLabel( 543, 40, 1168, @"Testbed" );

            AddBackground( 467, 128, 133, 48, 2620 );
            AddBackground( 467, 86, 104, 48, 2620 ); //GumpText Hue box
            AddBackground( 359, 86, 113, 90, 2620 ); //BodyValue box
            AddBackground( 20, 86, 344, 90, 2620 ); //Large Animate work box
            AddBackground( 20, 58, 580, 35, 2620 ); //Long Animate label box

            AddLabel( 30, 65, m_AT.m_TextHue,
                     @"Animate( int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay )" );

            AddButton( 48, 100, 8012, 8012, (int)Buttons.ApplyTarget, GumpButtonType.Reply, 0 );
            AddButton( 43, 132, 2095, 2094, (int)Buttons.Apply, GumpButtonType.Reply, 0 );
            AddLabel( 30, 120, m_AT.m_TextHue, @"Animate(" );

            AddButton( 90, 95, 250, 251, (int)Buttons.ActionPlus, GumpButtonType.Reply, 0 );
            AddTextEntry( 90, 120, 26, 20, m_AT.m_TextHue, 1, @"" + m_AT.m_Action );
            AddButton( 90, 145, 252, 253, (int)Buttons.ActionMinus, GumpButtonType.Reply, 0 );
            AddLabel( 118, 120, m_AT.m_TextHue, @"," );

            AddButton( 125, 95, 250, 251, (int)Buttons.FrameCountPlus, GumpButtonType.Reply, 0 );
            AddTextEntry( 125, 120, 25, 20, m_AT.m_TextHue, 2, @"" + m_AT.m_FrameCount );
            AddButton( 125, 145, 252, 253, (int)Buttons.FrameCountMinus, GumpButtonType.Reply, 0 );
            AddLabel( 154, 120, m_AT.m_TextHue, @"," );

            AddButton( 162, 95, 250, 251, (int)Buttons.RepeatCountPlus, GumpButtonType.Reply, 0 );
            AddTextEntry( 162, 120, 23, 20, m_AT.m_TextHue, 3, @"" + m_AT.m_RepeatCount );
            AddButton( 162, 145, 252, 253, (int)Buttons.RepeatCountMinus, GumpButtonType.Reply, 0 );
            AddLabel( 187, 120, m_AT.m_TextHue, @"," );

            AddButton( 200, 115, 254, 254, (int)Buttons.ForwardSwitch, GumpButtonType.Reply, 0 );
            AddLabel( 214, 120, m_AT.m_TextHue, @"" + m_AT.m_Forward );
            AddLabel( 252, 120, m_AT.m_TextHue, @"," );

            AddButton( 265, 115, 254, 254, (int)Buttons.RepeatSwitch, GumpButtonType.Reply, 0 );
            AddLabel( 280, 120, m_AT.m_TextHue, @"" + m_AT.m_Repeat );
            AddLabel( 315, 120, m_AT.m_TextHue, @"," );

            AddButton( 325, 95, 250, 251, (int)Buttons.DelayPlus, GumpButtonType.Reply, 0 );
            AddTextEntry( 325, 120, 23, 20, m_AT.m_TextHue, 4, @"" + m_AT.m_Delay );
            AddButton( 325, 145, 252, 253, (int)Buttons.DelayMinus, GumpButtonType.Reply, 0 );
            AddLabel( 351, 120, m_AT.m_TextHue, @")" );

            AddButton( 392, 100, 8012, 8012, (int)Buttons.BodyApplyTarget, GumpButtonType.Reply, 0 );
            AddButton( 388, 132, 2095, 2094, (int)Buttons.BodyValueApply, GumpButtonType.Reply, 0 );
            AddLabel( 370, 120, m_AT.m_TextHue, @"BodyValue" );

            AddButton( 440, 95, 250, 251, (int)Buttons.BodyPlus, GumpButtonType.Reply, 0 );
            AddTextEntry( 440, 120, 23, 20, m_AT.m_TextHue, 5, @"" + m_AT.m_BodyValue );
            AddButton( 440, 145, 252, 253, (int)Buttons.BodyMinus, GumpButtonType.Reply, 0 );

            AddLabel( 475, 90, m_AT.m_TextHue, @"GumpText Hue" );
            AddTextEntry( 500, 105, 43, 20, m_AT.m_TextHue, 6, @"" + m_AT.m_TextHue );
            AddButton( 480, 110, 2223, 2223, (int)Buttons.TextHueMinus, GumpButtonType.Reply, 0 );
            AddButton( 542, 110, 2224, 2224, (int)Buttons.TextHuePlus, GumpButtonType.Reply, 0 );

            AddButton( 577, 148, 8012, 8012, (int)Buttons.HueMobileTarget, GumpButtonType.Reply, 0 );
            AddButton( 472, 141, 2095, 2094, (int)Buttons.HueMobileApply, GumpButtonType.Reply, 0 );
            AddLabel( 500, 132, m_AT.m_TextHue, @"Hue Mobile" );

            AddTextEntry( 520, 147, 43, 20, m_AT.m_HueMobile, 7, @"" + m_AT.m_HueMobile );
            AddButton( 495, 150, 2223, 2223, (int)Buttons.HueMobileMinus, GumpButtonType.Reply, 0 );
            AddButton( 557, 150, 2224, 2224, (int)Buttons.HueMobilePlus, GumpButtonType.Reply, 0 );

            AddButton( 586, 90, 4035, 4035, (int)Buttons.NotesPage, GumpButtonType.Reply, 0 );
        }

        public enum Buttons
        {
            Exit,
            ApplyTarget,
            Apply,
            ActionPlus,
            ActionMinus,
            FrameCountPlus,
            FrameCountMinus,
            RepeatCountPlus,
            RepeatCountMinus,
            ForwardSwitch,
            RepeatSwitch,
            DelayPlus,
            DelayMinus,
            BodyApplyTarget,
            BodyValueApply,
            BodyPlus,
            BodyMinus,
            TextHueMinus,
            TextHuePlus,
            NotesPage,
            HueMobileTarget,
            HueMobileApply,
            HueMobileMinus,
            HueMobilePlus,
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( m_AT == null )
                return;

            Mobile from = state.Mobile;

            #region TextRelays
            TextRelay entry1 = info.GetTextEntry( 1 );
            int Action = NegProtection( entry1 == null ? "" : entry1.Text.Trim() );
            if( !( Action == m_AT.m_Action ) )
                m_AT.m_Action = Action;

            TextRelay entry2 = info.GetTextEntry( 2 );
            int FrameCount = NegProtection( entry2 == null ? "" : entry2.Text.Trim() );
            if( !( FrameCount == m_AT.m_FrameCount ) )
                m_AT.m_FrameCount = FrameCount;

            TextRelay entry3 = info.GetTextEntry( 3 );
            int RepeatCount = NegProtection( entry3 == null ? "" : entry3.Text.Trim() );
            if( !( RepeatCount == m_AT.m_RepeatCount ) )
                m_AT.m_RepeatCount = RepeatCount;

            TextRelay entry4 = info.GetTextEntry( 4 );
            int Delay = NegProtection( entry4 == null ? "" : entry4.Text.Trim() );
            if( !( Delay == m_AT.m_Delay ) )
                m_AT.m_Delay = Delay;

            TextRelay entry5 = info.GetTextEntry( 5 );
            int BodyValue = NegProtection( entry5 == null ? "" : entry5.Text.Trim() );
            if( !( BodyValue == m_AT.m_BodyValue ) )
                m_AT.m_BodyValue = BodyValue;

            TextRelay entry6 = info.GetTextEntry( 6 );
            int TextHue = NegProtection( entry6 == null ? "" : entry6.Text.Trim() );
            if( TextHue > 2999 )
                from.SendMessage( "There are no hues above 2999." );
            else if( !( TextHue == m_AT.m_TextHue ) )
                m_AT.m_TextHue = TextHue;

            TextRelay entry7 = info.GetTextEntry( 7 );
            int HueMobile = NegProtection( entry7 == null ? "" : entry7.Text.Trim() );
            if( HueMobile > 2999 )
                from.SendMessage( "There are no hues above 2999." );
            else if( !( HueMobile == m_AT.m_HueMobile ) )
                m_AT.m_HueMobile = HueMobile;
            #endregion

            #region switch ( info.ButtonID )
            switch( info.ButtonID )
            {
                case (int)Buttons.ApplyTarget:
                    from.Target = new DoApplyTarget( m_AT );
                    break;
                case (int)Buttons.Apply:
                    DoApply( from, m_AT );
                    break;

                case (int)Buttons.ActionPlus:
                    DoActionPlus( from, m_AT );
                    break;
                case (int)Buttons.ActionMinus:
                    DoActionMinus( from, m_AT );
                    break;

                case (int)Buttons.FrameCountPlus:
                    DoFrameCountPlus( from, m_AT );
                    break;
                case (int)Buttons.FrameCountMinus:
                    DoFrameCountMinus( from, m_AT );
                    break;

                case (int)Buttons.RepeatCountPlus:
                    DoRepeatCountPlus( from, m_AT );
                    break;
                case (int)Buttons.RepeatCountMinus:
                    DoRepeatCountMinus( from, m_AT );
                    break;

                case (int)Buttons.ForwardSwitch:
                    DoForwardSwitch( from, m_AT );
                    break;

                case (int)Buttons.RepeatSwitch:
                    DoRepeatSwitch( from, m_AT );
                    break;

                case (int)Buttons.DelayPlus:
                    DoDelayPlus( from, m_AT );
                    break;
                case (int)Buttons.DelayMinus:
                    DoDelayMinus( from, m_AT );
                    break;

                case (int)Buttons.BodyApplyTarget:
                    from.Target = new DoBodyApplyTarget( m_AT );
                    break;
                case (int)Buttons.BodyValueApply:
                    DoBodyValueApply( from, m_AT );
                    break;

                case (int)Buttons.BodyPlus:
                    DoBodyPlus( from, m_AT );
                    break;
                case (int)Buttons.BodyMinus:
                    DoBodyMinus( from, m_AT );
                    break;

                case (int)Buttons.TextHueMinus:
                    DoTextHueMinus( from, m_AT );
                    break;
                case (int)Buttons.TextHuePlus:
                    DoTextHuePlus( from, m_AT );
                    break;

                case (int)Buttons.NotesPage:
                    DoNotesPage( from, m_AT );
                    break;

                case (int)Buttons.HueMobileTarget:
                    from.Target = new DoHueMobileTarget( m_AT );
                    break;
                case (int)Buttons.HueMobileApply:
                    DoHueMobileApply( from, m_AT );
                    break;

                case (int)Buttons.HueMobileMinus:
                    DoHueMobileMinus( from, m_AT );
                    break;
                case (int)Buttons.HueMobilePlus:
                    DoHueMobilePlus( from, m_AT );
                    break;
            }
            #endregion
        }

        public class DoApplyTarget : Target
        {
            private Mobile mob;
            private AnimTestbed m_AT;

            public DoApplyTarget( AnimTestbed at )
                : base( -1, false, TargetFlags.None )
            {
                m_AT = at;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted is Mobile )
                {
                    mob = (Mobile)targeted;
                    mob.Animate( m_AT.m_Action, m_AT.m_FrameCount, m_AT.m_RepeatCount, m_AT.m_Forward, m_AT.m_Repeat,
                                m_AT.m_Delay );
                }
                else
                    from.SendMessage( "You can only Animate Mobiles." );

                from.CloseGump( typeof( AnimTestbedGump ) );
                from.SendGump( new AnimTestbedGump( from, m_AT ) );
            }
        }

        public void DoApply( Mobile from, AnimTestbed at )
        {
            from.Animate( at.m_Action, at.m_FrameCount, at.m_RepeatCount, at.m_Forward, at.m_Repeat, at.m_Delay );

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoActionPlus( Mobile from, AnimTestbed at )
        {
            at.m_Action++;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoActionMinus( Mobile from, AnimTestbed at )
        {
            if( at.m_Action >= 1 )
                at.m_Action--;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoFrameCountPlus( Mobile from, AnimTestbed at )
        {
            at.m_FrameCount++;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoFrameCountMinus( Mobile from, AnimTestbed at )
        {
            if( at.m_FrameCount >= 1 )
                at.m_FrameCount--;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoRepeatCountPlus( Mobile from, AnimTestbed at )
        {
            at.m_RepeatCount++;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoRepeatCountMinus( Mobile from, AnimTestbed at )
        {
            if( at.m_RepeatCount >= 1 )
                at.m_RepeatCount--;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoForwardSwitch( Mobile from, AnimTestbed at )
        {
            if( at.m_Forward )
                at.m_Forward = false;
            else
                at.m_Forward = true;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoRepeatSwitch( Mobile from, AnimTestbed at )
        {
            if( at.m_Repeat )
                at.m_Repeat = false;
            else
                at.m_Repeat = true;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoDelayPlus( Mobile from, AnimTestbed at )
        {
            at.m_Delay++;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoDelayMinus( Mobile from, AnimTestbed at )
        {
            if( at.m_Delay >= 1 )
                at.m_Delay--;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public class DoBodyApplyTarget : Target
        {
            private Mobile mob;
            private AnimTestbed m_AT;

            public virtual void ChangeBack( Mobile from, int oldBodyValue )
            {
                if( from != null )
                    from.BodyValue = oldBodyValue;
            }

            public DoBodyApplyTarget( AnimTestbed at )
                : base( -1, false, TargetFlags.None )
            {
                m_AT = at;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted is Mobile )
                {
                    mob = (Mobile)targeted;

                    if( from.BodyValue == m_AT.m_BodyValue )
                    {
                        mob.BodyValue = m_AT.m_BodyValue;
                    }
                    else
                        from.SendMessage(
                            "For safety reasons, you must already be that bodyvalue to change another mobile to it." );
                }
                else
                    from.SendMessage( "Only Mobiles have BodyValues." );

                from.CloseGump( typeof( AnimTestbedGump ) );
                from.SendGump( new AnimTestbedGump( from, m_AT ) );
            }
        }

        public void DoBodyValueApply( Mobile from, AnimTestbed at )
        {
            if( from.BodyValue != at.m_BodyValue )
                from.BodyValue = at.m_BodyValue;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoBodyPlus( Mobile from, AnimTestbed at )
        {
            at.m_BodyValue++;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoBodyMinus( Mobile from, AnimTestbed at )
        {
            if( at.m_BodyValue >= 1 )
                at.m_BodyValue--;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoTextHueMinus( Mobile from, AnimTestbed at )
        {
            if( at.m_TextHue >= 1 )
                at.m_TextHue--;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoTextHuePlus( Mobile from, AnimTestbed at )
        {
            if( at.m_TextHue > 2998 )
            {
                from.SendMessage( "There are no hues above 2999." );
            }
            else
                at.m_TextHue++;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoNotesPage( Mobile from, AnimTestbed at )
        {
            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );

            from.CloseGump( typeof( AnimTestbedNotesGump ) );
            from.SendGump( new AnimTestbedNotesGump( from, at ) );
        }

        public class DoHueMobileTarget : Target
        {
            private Mobile mob;
            private AnimTestbed m_AT;

            public DoHueMobileTarget( AnimTestbed at )
                : base( -1, false, TargetFlags.None )
            {
                m_AT = at;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted is Mobile )
                {
                    mob = (Mobile)targeted;
                    mob.Hue = m_AT.m_HueMobile;
                }
                else
                    from.SendMessage( "Mobiles only with this tool please." );

                from.CloseGump( typeof( AnimTestbedGump ) );
                from.SendGump( new AnimTestbedGump( from, m_AT ) );
            }
        }

        public void DoHueMobileApply( Mobile from, AnimTestbed at )
        {
            from.Hue = m_AT.m_HueMobile;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoHueMobilePlus( Mobile from, AnimTestbed at )
        {
            if( at.m_HueMobile > 2998 )
            {
                from.SendMessage( "There are no hues above 2999." );
            }
            else
                at.m_HueMobile++;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }

        public void DoHueMobileMinus( Mobile from, AnimTestbed at )
        {
            if( at.m_HueMobile >= 1 )
                at.m_HueMobile--;

            from.CloseGump( typeof( AnimTestbedGump ) );
            from.SendGump( new AnimTestbedGump( from, at ) );
        }
    }

    public class AnimTestbedNotesGump : Gump
    {
        private AnimTestbed m_AT;

        public AnimTestbedNotesGump( Mobile from, AnimTestbed at )
            : base( 0, 0 )
        {
            m_AT = at;

            Closable = true;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );
            AddBackground( 20, 169, 250, 250, 2620 );
            AddLabel( 29, 175, m_AT.m_TextHue, @"Notes:" );
            AddTextEntry( 28, 194, 240, 240, m_AT.m_TextHue, 7, @"" + m_AT.m_Notes );
        }

        public enum Buttons
        {
            Exit,
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( m_AT == null )
                return;

            TextRelay entry7 = info.GetTextEntry( 7 );
            string Notes = ( entry7 == null ? "" : entry7.Text.Trim() );
            if( !( Notes == m_AT.m_Notes ) )
                m_AT.m_Notes = Notes;
        }
    }
}