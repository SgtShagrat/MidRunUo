/***************************************************************************
 *                                  SwordOfLight.cs
 *                            		---------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Arma evocabile dai paladini.
 * 
 ***************************************************************************/

/*
using System;

using Server;
using Server.Items;
using Server.Network;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class SwordOfLight : Longsword
    {
        private Timer m_Timer;

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Lifespan { get; private set; }

        [Constructable]
        public SwordOfLight( Mobile paladine )
        {
            Owner = paladine;
            Hue = 0x482;
            LootType = LootType.Blessed;

            WeaponAttributes.UseBestSkill = 1;

            Slayer = SlayerName.Silver;
            Slayer2 = SlayerName.Exorcism;

            if( Owner != null )
            {
                double chival = Owner.Skills[ SkillName.Chivalry ].Base;

                if( Core.AOS )
                {
                    Attributes.WeaponDamage = 20 + (int)( chival / 8 );
                    Attributes.AttackChance = (int)( chival / 8 );
                }

                Lifespan = (int)( Core.AOS ? chival : ( 60 * RPGSpellsSystem.GetPowerLevel( paladine, typeof( SwordOfLightSpell ) ) ) );
            }

            if( Lifespan > 0 )
                StartTimer();
        }

        public SwordOfLight( Serial serial )
            : base( serial )
        {
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
            get { return "Holy sword of Light"; }
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            base.AddNameProperty( list );

            if( Owner != null )
                list.Add( "Owned by {0}", Owner.Name );

            if( Lifespan > 0 )
                list.Add( 1072517, Lifespan.ToString() ); // Lifespan: ~1_val~ seconds
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

        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = cold = pois = chaos = direct = 0;
            fire = nrgy = 50;
        }

        public override SkillName GetUsedSkill( Mobile m, bool checkSkillAttrs )
        {
            double swrd = m.Skills[ SkillName.Swords ].Value;
            double fenc = m.Skills[ SkillName.Fencing ].Value;
            double mcng = m.Skills[ SkillName.Macing ].Value;

            SkillName sk = SkillName.Swords;
            double val = swrd;

            if( fenc > val )
            {
                sk = SkillName.Fencing;
                val = fenc;
            }
            if( mcng > val )
            {
                sk = SkillName.Macing;
            }

            return sk;
        }

        public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
        {
            if( RPGPaladinSpell.IsSuperVulnerable( defender ) )
                damageBonus *= 1.5;

            double chival = attacker.Skills[ SkillName.Chivalry ].Base;

            if( Utility.RandomDouble() <= chival / 120.0 )
            {
                attacker.PublicOverheadMessage( MessageType.Regular, 1154, true, "* the power of light will prevail *" );

                switch( Utility.Random( 3 ) )
                {
                    case 0:
                        DoCure( attacker, defender );
                        break;
                    case 1:
                        DoLight( attacker, defender );
                        break;
                    case 2:
                        DoAoSBless( attacker, defender );
                        break;
                    default:
                        break;
                }
            }

            base.OnHit( attacker, defender, damageBonus );
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        #region effects
        private static void DoCure( Mobile attacker, Mobile defender )
        {
            if( defender == null || attacker == null )
                return;

            attacker.CurePoison( attacker );
        }

        private static void DoLight( Mobile attacker, Mobile defender )
        {
            if( defender == null || attacker == null )
                return;

            if( attacker.BeginAction( typeof( LightCycle ) ) )
            {
                new LightCycle.NightSightTimer( attacker ).Start();
                attacker.LightLevel = LightCycle.DayLevel;

                attacker.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
                attacker.PlaySound( 0x1EA );
            }
        }

        private static void DoAoSBless( Mobile attacker, Mobile defender )
        {
            if( defender == null || attacker == null )
                return;

            SpellHelper.AddStatBonus( attacker, attacker, StatType.Str );
            SpellHelper.DisableSkillCheck = true;
            SpellHelper.AddStatBonus( attacker, attacker, StatType.Dex );
            SpellHelper.AddStatBonus( attacker, attacker, StatType.Int );
            SpellHelper.DisableSkillCheck = false;

            attacker.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
            attacker.PlaySound( 0x1EA );
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