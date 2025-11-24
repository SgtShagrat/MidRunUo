/***************************************************************************
 *                               Dies Irae - Gravestones.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Targeting;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseGravestone : CraftableFurniture
    {
        public override string DefaultName { get { return "gravestone"; } }
        public override bool DisplayWeight { get { return false; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set
            {
                m_EngravedText = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Filled
        {
            get { return m_Filled; }
            set
            {
                m_Filled = value;
                InvalidateProperties();
            }
        }

        private string m_EngravedText;
        private bool m_Filled;

        public BaseGravestone( int itemID )
            : base( itemID )
        {
            Weight = 10.0;
            Stackable = false;
        }

        public BaseGravestone( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( Filled )
                return;

            if( !IsChildOf( from.Backpack ) )
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            else
            {
                from.Target = new InternalTarget( this );
                from.SendMessage( "This tombstone is blank. Whom should it be carved for?" );
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            if( !string.IsNullOrEmpty( m_EngravedText ) )
                LabelTo( from, m_EngravedText );
        }

        protected void OnTarget( Mobile from, object targeted )
        {
            if( targeted is Item )
            {
                Item item = (Item)targeted;
                if( item is Head )
                {
                    if( item.IsChildOf( from.Backpack ) )
                    {
                        Head head = item as Head;
                        if( head.PlayerName != null )
                        {
                            Name = string.Format( "the tombstone of {0}", head.PlayerName );
                            Filled = true;
                            from.PlaySound( 0x23E );
                            from.SendMessage( "You etch the name into the tombstone." );
                            head.Delete();
                        }
                        else
                            from.SendMessage( "You cannot use that head for the tombstone." );
                    }
                    else
                        from.SendMessage( "That head must be in your backpack." );
                }
                else
                    from.SendMessage( "That is not a head." );
            }
            else
                from.SendMessage( "That is not an item." );
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 2 ); // version

            writer.Write( m_EngravedText );
            writer.Write( m_Filled );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            switch( version )
            {
                case 2:
                    {
                        goto case 1;
                    }
                case 1:
                    {
                        m_EngravedText = reader.ReadString();
                        if( version < 2 )
                        {
                            reader.ReadInt();
                            reader.ReadInt();
                            reader.ReadMobile();
                        }
                        goto case 0;
                    }
                case 0:
                    {
                        m_Filled = reader.ReadBool();
                        break;
                    }
            }
        }
        #endregion

        private class InternalTarget : Target
        {
            private readonly BaseGravestone m_Gravestone;

            public InternalTarget( BaseGravestone gravestone )
                : base( 1, false, TargetFlags.None )
            {
                m_Gravestone = gravestone;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Gravestone != null && !m_Gravestone.Deleted)
                    m_Gravestone.OnTarget( from, targeted );
            }
        }
    }

    [Flipable( 0xED7, 0xED8 )]
    public class GraveStone1 : BaseGravestone
    {
        [Constructable]
        public GraveStone1()
            : base( 0xED7 )
        {
        }

        public GraveStone1( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0xED9, 0xEDA )]
    public class GraveStone2 : BaseGravestone
    {
        [Constructable]
        public GraveStone2()
            : base( 0xED9 )
        {
        }

        public GraveStone2( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0xED4, 0xED5 )]
    public class GraveStone3 : BaseGravestone
    {
        [Constructable]
        public GraveStone3()
            : base( 0xED4 )
        {
        }

        public GraveStone3( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0xEDB, 0xEDC )]
    public class GraveStone4 : BaseGravestone
    {
        [Constructable]
        public GraveStone4()
            : base( 0xEDB )
        {
        }

        public GraveStone4( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0xEDD, 0xEDE )]
    public class GraveStone5 : BaseGravestone
    {
        [Constructable]
        public GraveStone5()
            : base( 0xEDD )
        {
        }

        public GraveStone5( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x1165, 0x1166 )]
    public class GraveStone6 : BaseGravestone
    {
        [Constructable]
        public GraveStone6()
            : base( 0x1165 )
        {
        }

        public GraveStone6( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x1167, 0x1168 )]
    public class GraveStone7 : BaseGravestone
    {
        [Constructable]
        public GraveStone7()
            : base( 0x1167 )
        {
        }

        public GraveStone7( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x1169, 0x116A )]
    public class GraveStone8 : BaseGravestone
    {
        [Constructable]
        public GraveStone8()
            : base( 0x1169 )
        {
        }

        public GraveStone8( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x116B, 0x116C )]
    public class GraveStone9 : BaseGravestone
    {
        [Constructable]
        public GraveStone9()
            : base( 0x116B )
        {
        }

        public GraveStone9( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x116D, 0x116E )]
    public class GraveStone10 : BaseGravestone
    {
        [Constructable]
        public GraveStone10()
            : base( 0x116D )
        {
        }

        public GraveStone10( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    public class GraveStone12 : BaseGravestone
    {
        [Constructable]
        public GraveStone12()
            : base( 0xED6 )
        {
        }

        public GraveStone12( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x116F, 0x1170 )]
    public class GraveStone14 : BaseGravestone
    {
        [Constructable]
        public GraveStone14()
            : base( 0x116F )
        {
        }

        public GraveStone14( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x1171, 0x1172 )]
    public class GraveStone15 : BaseGravestone
    {
        [Constructable]
        public GraveStone15()
            : base( 0x1171 )
        {
        }

        public GraveStone15( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x1173, 0x1174 )]
    public class GraveStone16 : BaseGravestone
    {
        [Constructable]
        public GraveStone16()
            : base( 0x1173 )
        {
        }

        public GraveStone16( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x1175, 0x1176 )]
    public class GraveStone17 : BaseGravestone
    {
        [Constructable]
        public GraveStone17()
            : base( 0x1175 )
        {
        }

        public GraveStone17( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x1177, 0x1178 )]
    public class GraveStone18 : BaseGravestone
    {
        [Constructable]
        public GraveStone18()
            : base( 0x1177 )
        {
        }

        public GraveStone18( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x1179, 0x117A )]
    public class GraveStone19 : BaseGravestone
    {
        [Constructable]
        public GraveStone19()
            : base( 0x1179 )
        {
        }

        public GraveStone19( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x117B, 0x117C )]
    public class GraveStone20 : BaseGravestone
    {
        [Constructable]
        public GraveStone20()
            : base( 0x117B )
        {
        }

        public GraveStone20( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x117D, 0x117E )]
    public class GraveStone21 : BaseGravestone
    {
        [Constructable]
        public GraveStone21()
            : base( 0x117D )
        {
        }

        public GraveStone21( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x117F, 0x1180 )]
    public class GraveStone22 : BaseGravestone
    {
        [Constructable]
        public GraveStone22()
            : base( 0x117F )
        {
        }

        public GraveStone22( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x1181, 0x1182 )]
    public class GraveStone23 : BaseGravestone
    {
        [Constructable]
        public GraveStone23()
            : base( 0x1181 )
        {
        }

        public GraveStone23( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }

    [Flipable( 0x1183, 0x1184 )]
    public class GraveStone24 : BaseGravestone
    {
        [Constructable]
        public GraveStone24()
            : base( 0x1183 )
        {
        }

        public GraveStone24( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
    }
}