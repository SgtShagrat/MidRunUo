/***************************************************************************
 *                               ShieldOfRighteousness.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

/*
using System;

using Server;
using Server.Items;

namespace Midgard.Engines.SpellSystem
{
    public class ShieldOfRighteousness : HeaterShield
    {
        private Timer m_Timer;

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Lifespan { get; private set; }

        [Constructable]
        public ShieldOfRighteousness( Mobile paladine, int powerLevelScaled )
        {
            Owner = paladine;
            Hue = 0x482;
            LootType = LootType.Blessed;

            if( Owner != null )
                Lifespan = powerLevelScaled;

            if( Lifespan > 0 )
                StartTimer();
        }

        public override int InitMinHits
        {
            get { return 255; }
        }

        public override int InitMaxHits
        {
            get { return 255; }
        }

        public override string DefaultName
        {
            get { return "Holy shield of Righteousness"; }
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            if( Owner != null )
                LabelTo( from, "Owned by {0}", Owner.Name ?? "Virtues" );

            if( Lifespan > 0 )
                LabelTo( from, 1072517, Lifespan.ToString() );
        }

        public override bool DisplayLootType
        {
            get { return false; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        public override int OnHit( BaseWeapon weapon, int damage )
        {
            double chival = Owner.Skills[ SkillName.Chivalry ].Base;

            if( Utility.RandomDouble() <= chival / Owner.Skills[ SkillName.Chivalry ].Cap )
            {
                switch( Utility.Random( 2 ) )
                {
                    case 0:
                        DoHeal( Owner );
                        break;
                    case 1:
                        DoLight( Owner );
                        break;
                    default:
                        break;
                }
            }

            return base.OnHit( weapon, damage );
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        #region effects
        private static void DoHeal( Mobile attacker )
        {
            if( attacker == null )
                return;

            int toHeal = Utility.Dice( 1, 10, 10 );
            attacker.Heal( toHeal );
        }

        private static void DoLight( Mobile attacker )
        {
            if( attacker == null )
                return;

            if( attacker.BeginAction( typeof( LightCycle ) ) )
            {
                new LightCycle.NightSightTimer( attacker ).Start();
                attacker.LightLevel = LightCycle.DayLevel;

                attacker.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
                attacker.PlaySound( 0x1EA );
            }
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
            Lifespan -= 10;

            InvalidateProperties();

            if( Lifespan <= 0 )
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
            if( m == null || !m.Player || Owner == null )
                return true;

            return m == Owner;
        }

        #region serial-deserial
        public ShieldOfRighteousness( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.WriteMobile( Owner );
            writer.Write( Lifespan );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Owner = reader.ReadMobile();
            Lifespan = reader.ReadInt();

            StartTimer();
        }
        #endregion
    }
}
*/