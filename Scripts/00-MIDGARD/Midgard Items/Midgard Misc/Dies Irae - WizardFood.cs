/***************************************************************************
 *                               WizardFood.cs.cs
 *                            ----------------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
    public class WizardFood : Food
    {
        #region fields
        private int m_Lifespan;
        private Mobile m_Owner;
        private Timer m_Timer;
        #endregion

        #region properties
        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner
        {
            get { return m_Owner; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Lifespan
        {
            get { return m_Lifespan; }
        }
        #endregion

        #region constructors
        [Constructable]
        public WizardFood()
            : base( 1, 0xF8E )
        {
            Hue = 2024;
            Weight = 1.0;
            FillFactor = 1 + Math.Min( 200 / 60, 4 );
            Stackable = true;
        }

        [Constructable]
        public WizardFood( Mobile mage )
            : this( mage, 1 )
        {
        }

        [Constructable]
        public WizardFood( Mobile mage, int amount )
            : base( amount, 0xF8E )
        {
            m_Owner = mage;

            int sum = m_Owner == null ? 0 : (int)m_Owner.Skills[ SkillName.Magery ].Base + m_Owner.Int;

            Hue = 2024;
            Weight = 1.0;
            FillFactor = 1 + Math.Min( sum / 60, 4 );
            Stackable = true;

            if( m_Owner != null )
                m_Lifespan = sum;

            if( m_Lifespan > 0 )
                StartTimer();
        }

        public WizardFood( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region overrides
        public override string DefaultName
        {
            get { return "Magical Food"; }
        }

        public override bool DisplayLootType
        {
            get { return false; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        public override bool Eat( Mobile from )
        {
            return Validate( from ) && base.Eat( from );
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            base.AddNameProperty( list );

            if( m_Owner != null )
                list.Add( "Owned by {0}", m_Owner.Name );

            if( m_Lifespan > 0 )
                list.Add( 1072517, m_Lifespan.ToString() ); // Lifespan: ~1_val~ seconds
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Owned by {0}", m_Owner.Name );
            LabelTo( from, 1072517, m_Lifespan.ToString() ); // Lifespan: ~1_val~ seconds
        }

        public override bool StackWith( Mobile from, Item dropped, bool playSound )
        {
            if( !Validate( from ) )
                return false;

            bool success = dropped is WizardFood && base.StackWith( from, dropped, playSound );

            if( success )
            {
                int delta = (int)( from.Skills[ SkillName.Magery ].Base + from.Int ) - m_Lifespan;
                if( delta > 0 )
                    m_Lifespan += delta;
            }

            return success;
        }
        #endregion

        #region decay
        public void StartTimer()
        {
            if( m_Timer != null )
                return;

            m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 10 ), TimeSpan.FromSeconds( 10 ), new TimerCallback( Slice ) );
            m_Timer.Priority = TimerPriority.OneSecond;
        }

        public void StopTimer()
        {
            if( m_Timer != null )
                m_Timer.Stop();

            m_Timer = null;
        }

        public void Slice()
        {
            m_Lifespan -= 10;

            InvalidateProperties();

            if( m_Lifespan <= 0 )
                Decay();
        }

        public void Decay()
        {
            if( RootParent is Mobile )
            {
                Mobile parent = (Mobile)RootParent;

                if( Name == null )
                    parent.SendLocalizedMessage( 1072515, "#" + LabelNumber ); // The ~1_name~ expired...
                else
                    parent.SendLocalizedMessage( 1072515, Name ); // The ~1_name~ expired...

                Effects.SendLocationParticles( EffectItem.Create( parent.Location, parent.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
                Effects.PlaySound( parent.Location, parent.Map, 0x201 );
            }
            else
            {
                Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
                Effects.PlaySound( Location, Map, 0x201 );
            }

            StopTimer();
            Delete();
        }
        #endregion

        public bool Validate( Mobile m )
        {
            if( m == null || !m.Player || m_Owner == null )
                return true;

            return m == m_Owner;
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.WriteMobile( m_Owner );
            writer.Write( m_Lifespan );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Owner = reader.ReadMobile();
            m_Lifespan = reader.ReadInt();

            StartTimer();
        }
        #endregion
    }
}