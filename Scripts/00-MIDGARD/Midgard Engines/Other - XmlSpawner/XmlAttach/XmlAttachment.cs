using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public abstract class XmlAttachment : IXmlAttachment
    {
        // ----------------------------------------------
        // Private fields
        // ----------------------------------------------
        private ASerial m_Serial;

        private string m_Name;

        private object m_AttachedTo;

        private object m_OwnedBy;

        private string m_AttachedBy;

        private AttachmentTimer m_ExpirationTimer;

        private TimeSpan m_Expiration = TimeSpan.Zero; // no expiration by default

        // ----------------------------------------------
        // Public properties
        // ----------------------------------------------
        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime CreationTime { get; private set; }

        public bool Deleted { get; private set; }

        public bool DoDelete
        {
            get { return false; }
            set { if( value ) Delete(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int SerialValue
        {
            get { return m_Serial.Value; }
        }

        public ASerial Serial
        {
            get { return m_Serial; }
            set { m_Serial = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan Expiration
        {
            get
            {
                // if the expiration timer is running then return the remaining time
                if( m_ExpirationTimer != null )
                {
                    return ExpirationEnd - DateTime.Now;
                }
                else
                    return m_Expiration;
            }
            set
            {
                m_Expiration = value;
                // if it is already attached to something then set the expiration timer
                if( m_AttachedTo != null )
                {
                    DoTimer( m_Expiration );
                }
            }
        }

        public DateTime ExpirationEnd { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool CanActivateInBackpack
        {
            get { return true; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool CanActivateEquipped
        {
            get { return true; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool CanActivateInWorld
        {
            get { return true; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool HandlesOnSpeech
        {
            get { return false; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool HandlesOnMovement
        {
            get { return false; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool HandlesOnKill
        {
            get { return false; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool HandlesOnKilled
        {
            get { return false; }
        }

        /*
		[CommandProperty( AccessLevel.GameMaster )]
		public virtual bool HandlesOnSkillUse { get{return false; } }
		*/

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual object Attached
        {
            get { return m_AttachedTo; }
        }

        public virtual object AttachedTo
        {
            get { return m_AttachedTo; }
            set { m_AttachedTo = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual string AttachedBy
        {
            get { return m_AttachedBy; }
        }

        public virtual object OwnedBy
        {
            get { return m_OwnedBy; }
            set { m_OwnedBy = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual object Owner
        {
            get { return m_OwnedBy; }
        }

        // ----------------------------------------------
        // Private methods
        // ----------------------------------------------
        private void DoTimer( TimeSpan delay )
        {
            ExpirationEnd = DateTime.Now + delay;

            if( m_ExpirationTimer != null )
                m_ExpirationTimer.Stop();

            m_ExpirationTimer = new AttachmentTimer( this, delay );
            m_ExpirationTimer.Start();
        }

        // a timer that can be implement limited lifetime attachments
        private class AttachmentTimer : Timer
        {
            private XmlAttachment m_Attachment;

            public AttachmentTimer( XmlAttachment attachment, TimeSpan delay )
                : base( delay )
            {
                Priority = TimerPriority.OneSecond;

                m_Attachment = attachment;
            }

            protected override void OnTick()
            {
                m_Attachment.Delete();
            }
        }

        // ----------------------------------------------
        // Constructors
        // ----------------------------------------------
        protected XmlAttachment()
        {
            CreationTime = DateTime.Now;

            // get the next unique serial id
            m_Serial = ASerial.NewSerial();

            // register the attachment in the serial keyed hashtable
            XmlAttach.HashSerial( m_Serial, this );
        }

        // needed for deserialization
        public XmlAttachment( ASerial serial )
        {
            m_Serial = serial;
        }

        // ----------------------------------------------
        // Public methods
        // ----------------------------------------------

        public static void Initialize()
        {
            XmlAttach.CleanUp();
        }

        public virtual bool CanEquip( Mobile from )
        {
            return true;
        }

        public virtual void OnEquip( Mobile from )
        {
        }


        public virtual void OnRemoved( object parent )
        {
        }

        public virtual void OnAttach()
        {
            // start up the expiration timer on attachment
            if( m_Expiration > TimeSpan.Zero )
                DoTimer( m_Expiration );
        }

        public virtual void OnReattach()
        {
        }

        public virtual void OnUse( Mobile from )
        {
        }

        public virtual void OnUser( object target )
        {
        }

        public virtual bool BlockDefaultOnUse( Mobile from, object target )
        {
            return false;
        }

        public virtual bool OnDragLift( Mobile from, Item item )
        {
            return true;
        }

        public void SetAttachedBy( string name )
        {
            m_AttachedBy = name;
        }

        public virtual void OnSpeech( SpeechEventArgs args )
        {
        }

        public virtual void OnMovement( MovementEventArgs args )
        {
        }

        public virtual void OnKill( Mobile killed, Mobile killer )
        {
        }

        public virtual void OnBeforeKill( Mobile killed, Mobile killer )
        {
        }

        public virtual void OnKilled( Mobile killed, Mobile killer )
        {
        }

        public virtual void OnBeforeKilled( Mobile killed, Mobile killer )
        {
        }

        #region mod by Dies Irae

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool HandlesOnDeath
        {
            get { return false; }
        }

        /// <summary>
        /// Event invoked only one time before target creature death
        /// </summary>
        public virtual void OnBeforeDeath( BaseCreature creature )
        {
        }

        /// <summary>
        /// Event invoked only one time on target creature death
        /// </summary>
        public virtual void OnDeath( BaseCreature creature, Container container )
        {
        }

       /// <summary>
        /// Event invoked only one time before target midgard player death
        /// </summary>
        public virtual void OnBeforeDeath( Midgard2PlayerMobile player )
        {
        }

        /// <summary>
        /// Event invoked only one time on target midgard player death
        /// </summary>
        public virtual void OnDeath( Midgard2PlayerMobile player, Container container )
        {
        }
        #endregion

        /*
		public virtual void OnSkillUse( Mobile m, Skill skill, bool success)
		{
		}
		*/

        public virtual void OnWeaponHit( Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven )
        {
        }

        public virtual int OnArmorHit( Mobile attacker, Mobile defender, Item armor, BaseWeapon weapon, int damageGiven )
        {
            return 0;
        }

        public virtual string OnIdentify( Mobile from )
        {
            return null;
        }

        public virtual string DisplayedProperties( Mobile from )
        {
            return OnIdentify( from );
        }


        public virtual void AddProperties( ObjectPropertyList list )
        {
        }

        public void InvalidateParentProperties()
        {
            if( AttachedTo is Item )
            {
                ( (Item)AttachedTo ).InvalidateProperties();
            }
        }

        public void Delete()
        {
            if( Deleted )
                return;

            Deleted = true;

            if( m_ExpirationTimer != null )
                m_ExpirationTimer.Stop();

            OnDelete();

            // dereference the attachment object
            AttachedTo = null;
            OwnedBy = null;
        }

        public virtual void OnDelete()
        {
        }

        public virtual void OnTrigger( object activator, Mobile from )
        {
        }

        public virtual void Serialize( GenericWriter writer )
        {
            writer.Write( 2 );
            // version 2
            writer.Write( m_AttachedBy );
            // version 1
            if( OwnedBy is Item )
            {
                writer.Write( 0 );
                writer.Write( (Item)OwnedBy );
            }
            else if( OwnedBy is Mobile )
            {
                writer.Write( 1 );
                writer.Write( (Mobile)OwnedBy );
            }
            else
                writer.Write( -1 );

            // version 0
            writer.Write( Name );
            // if there are any active timers, then serialize
            writer.Write( m_Expiration );
            if( m_ExpirationTimer != null )
            {
                writer.Write( ExpirationEnd - DateTime.Now );
            }
            else
            {
                writer.Write( TimeSpan.Zero );
            }
            writer.Write( CreationTime );
        }

        public virtual void Deserialize( GenericReader reader )
        {
            int version = reader.ReadInt();

            switch( version )
            {
                case 2:
                    m_AttachedBy = reader.ReadString();
                    goto case 1;
                case 1:
                    int owned = reader.ReadInt();
                    if( owned == 0 )
                    {
                        OwnedBy = reader.ReadItem();
                    }
                    else if( owned == 1 )
                    {
                        OwnedBy = reader.ReadMobile();
                    }
                    else
                        OwnedBy = null;

                    goto case 0;
                case 0:
                    // version 0
                    Name = reader.ReadString();
                    m_Expiration = reader.ReadTimeSpan();
                    TimeSpan remaining = reader.ReadTimeSpan();

                    if( remaining > TimeSpan.Zero )
                        DoTimer( remaining );

                    CreationTime = reader.ReadDateTime();
                    break;
            }
        }
    }
}