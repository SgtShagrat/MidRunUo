
using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;

namespace Server.Mobiles
{
    public abstract class BaseHireling : BaseCreature
    {
        #region Variables

        private bool m_IsHired;
        private int m_Pay = 1;
        private int m_HoldGold = 0;
        private int m_GoldOnDeath = 0;
        private int m_PersonalGoldOnDeath = 0;
        private Timer m_PayTimer;

        private static Hashtable m_HireTable = new Hashtable();

        [CommandProperty( AccessLevel.Administrator )]
        public int GoldOnDeath
        {
            get { return m_HoldGold; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public bool IsHired
        {
            get { return m_IsHired; }

            set
            {
                if( m_IsHired == value )
                    return;

                m_IsHired = value;

                Delta( MobileDelta.Noto );

                InvalidateProperties();
            }
        }

        public static Hashtable HireTable
        {
            get { return m_HireTable; }
        }

        #endregion

        public override bool GuardImmune { get { return true; } }

        public BaseHireling()
            : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.1, 4.0 )
        {
            ControlSlots = 2;
        }

        public BaseHireling( AIType AI )
            : base( AI, FightMode.Aggressor, 10, 1, 0.1, 4.0 )
        {
            ControlSlots = 2;
        }

        #region Characteristics

        public override bool KeepsItemsOnDeath
        {
            get { return true; }
        }

        #endregion

        #region Methods

        protected void GenBody( bool female )
        {
            if( female )
            {
                Body = 0x191;
                Name = NameList.RandomName( "female" );
            }

            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );
            }
        }

        protected void SetHair( int itemID, int hue )
        {
            HairItemID = itemID;
            HairHue = hue;
        }

        protected void SetBeard( int itemID, int hue )
        {
            FacialHairItemID = itemID;
            FacialHairHue = hue;
        }

        public override bool OnBeforeDeath()
        {
            if( m_PayTimer != null )
                m_PayTimer.Stop();

            m_PayTimer = null;

            if( Backpack != null )
            {
                Item[] AllGold = Backpack.FindItemsByType( typeof( Gold ), true );

                if( AllGold != null )
                {
                    foreach( Gold g in AllGold )
                        m_PersonalGoldOnDeath += g.Amount;
                }
            }

            return base.OnBeforeDeath();
        }

        public override bool OnDragDrop( Mobile from, Item item )
        {
            if( m_Pay != 0 )
            {
                if( Controlled == false )
                {
                    if( item is Gold )
                    {
                        if( item.Amount >= m_Pay )
                        {
                            BaseHireling hire = (BaseHireling)m_HireTable[ from ];

                            if( hire != null && !hire.Deleted && hire.GetOwner() == from )
                            {
                                SayTo( from, 500896 ); // I see you already have an escort.

                                return false;
                            }

                            if( AddHire( from ) )
                            {
                                SayTo( from, 1043258, string.Format( "{0}", item.Amount / m_Pay ) ); // I thank thee for paying me. I will work for thee for ~1_NUMBER~ days. 
                                m_HireTable[ from ] = this;
                                m_HoldGold += item.Amount;

                                m_PayTimer = new PayTimer( this );
                                m_PayTimer.Start();

                                return true;
                            }
                            else
                                return false;
                        }
                        else
                        {
                            SayHireCost();
                        }
                    }
                    else
                    {
                        SayTo( from, 1043268 ); // Tis crass of me, but I want gold.
                    }
                }
                else
                {
                    Say( 1042495 ); // I have already been hired.
                }
            }
            else
            {
                SayTo( from, 500200 ); // I have no need for that. 
            }

            return base.OnDragDrop( from, item );
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            Mobile Owner = GetOwner();

            if( Owner == null )
            {
                base.GetContextMenuEntries( from, list );
                list.Add( new HireEntry( from, this ) );
            }
            else
                base.GetContextMenuEntries( from, list );
        }

        public override void OnDeath( Container c )
        {
            if( m_GoldOnDeath > 0 )
                c.DropItem( new Gold( m_GoldOnDeath ) );

            base.OnDeath( c );
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( !e.Handled && e.Mobile.InRange( this, 6 ) )
            {
                if( e.HasKeyword( 0x003B ) || e.HasKeyword( 0x0162 ) )
                {
                    e.Handled = Payday( this );
                    SayHireCost();
                }
            }

            base.OnSpeech( e );
        }

        internal void SayHireCost()
        {
            Say( 1043256, string.Format( "{0}", m_Pay ) ); // I am available for hire for ~1_AMOUNT~ gold coins a day. If thou dost give me gold, I will work for thee.
        }

        public virtual bool AddHire( Mobile m )
        {
            Mobile owner = GetOwner();

            if( owner != null )
            {
                m.SendLocalizedMessage( 1043283, owner.Name ); // I am following ~1_NAME~. 

                return false;
            }

            if( SetControlMaster( m ) )
            {
                m_IsHired = true;

                return true;
            }

            return false;
        }

        public virtual bool Payday( BaseHireling m )
        {
            m_Pay = (int)m.Skills[ SkillName.Anatomy ].Value + (int)m.Skills[ SkillName.Tactics ].Value;
            m_Pay += (int)m.Skills[ SkillName.Archery ].Value + (int)m.Skills[ SkillName.Fencing ].Value;
            m_Pay += (int)m.Skills[ SkillName.Healing ].Value + (int)m.Skills[ SkillName.MagicResist ].Value;
            m_Pay += (int)m.Skills[ SkillName.Macing ].Value + (int)m.Skills[ SkillName.Swords ].Value;
            m_Pay += (int)m.Skills[ SkillName.Magery ].Value + (int)m.Skills[ SkillName.Parry ].Value;

            m_Pay /= 35;
            m_Pay += 1;

            return true;
        }

        public virtual Mobile GetOwner()
        {
            if( !Controlled )
                return null;

            Mobile Owner = ControlMaster;
            m_IsHired = true;

            if( Owner == null )
                return null;

            if( Owner.Deleted || Owner.Map != Map || !Owner.InRange( Location, 30 ) )
            {
                Say( 1005653 ); // Hmmm. I seem to have lost my master. 

                HireTable.Remove( Owner );
                SetControlMaster( null );

                return null;
            }

            return Owner;
        }

        #endregion

        #region Context Menus

        public class HireEntry : ContextMenuEntry
        {
            #region Variables

            private BaseHireling m_Hire;
            private Mobile m_Mobile;

            #endregion

            [Constructable]
            public HireEntry( Mobile from, BaseHireling hire )
                : base( 6120, 3 )
            {
                m_Hire = hire;
                m_Mobile = from;
            }

            #region Methods

            public override void OnClick()
            {
                m_Hire.Payday( m_Hire );
                m_Hire.SayHireCost();
            }

            #endregion
        }

        #endregion

        #region Timers

        private class PayTimer : Timer
        {
            #region Variables

            private BaseHireling m_Hire;

            #endregion

            public PayTimer( BaseHireling vend )
                : base( TimeSpan.FromMinutes( 30.0 ), TimeSpan.FromMinutes( 30.0 ) )
            {
                m_Hire = vend;
                Priority = TimerPriority.OneMinute;
            }

            #region Methods

            protected override void OnTick()
            {
                int m_Pay = m_Hire.m_Pay;

                if( m_Hire.m_HoldGold <= m_Pay )
                {
                    m_Hire.Say( 503235 ); // I regret nothing! 
                    m_Hire.Delete();
                }

                else
                {
                    m_Hire.m_HoldGold -= m_Pay;
                }
            }

            #endregion
        }

        #endregion

        #region Serialization

        public BaseHireling( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( m_IsHired );
            writer.Write( m_HoldGold );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_IsHired = reader.ReadBool();
            m_HoldGold = reader.ReadInt();
            m_PayTimer = new PayTimer( this );
            m_PayTimer.Start();
        }

        #endregion
    }
}