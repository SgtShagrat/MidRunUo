/***************************************************************************
 *                               KalishtenPeerless.cs
 *                            --------------------------
 *   begin                : 06 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Mobiles
{
    public class KalishtenPeerless : BasePeerless
    {
        [Constructable]
        public KalishtenPeerless()
            : base( AIType.AI_BossMelee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "Kalish'ten";
            Title = "the Elder Mummy";

            Body = 154;
            BaseSoundID = 471;

            Hue = Utility.RandomNeutralHue();

            SetStr( 305, 425 );
            SetDex( 72, 150 );
            SetInt( 505, 750 );

            SetHits( 7500 );
            SetStam( 102, 300 );

            SetDamage( 25, 35 );

            SetDamageType( ResistanceType.Poison, 100 );

            SetResistance( ResistanceType.Physical, 60, 70 );
            SetResistance( ResistanceType.Fire, 50, 60 );
            SetResistance( ResistanceType.Cold, 50, 60 );
            SetResistance( ResistanceType.Poison, 40, 50 );
            SetResistance( ResistanceType.Energy, 40, 50 );

            SetSkill( SkillName.MagicResist, 100.0 );
            SetSkill( SkillName.Tactics, 97.6, 100.0 );
            SetSkill( SkillName.Wrestling, 97.6, 100.0 );

            Fame = 18000;
            Karma = -18000;

            VirtualArmor = 60;
        }

        public override void OnDamage( int amount, Mobile from, bool willKill )
        {
            if( willKill )
            {
                SpawnHelper( new Mummy(), 6490, 948, 19 );
                SpawnHelper( new SkeletalPoisoner(), 6497, 946, 17 );
            }

            base.OnDamage( amount, from, willKill );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.UltraRich, 3 );
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override double AutoDispelChance { get { return 1.0; } }
        public override bool BardImmune { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override bool Uncalmable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override bool ShowFameTitle { get { return false; } }
        public override bool ClickTitle { get { return false; } }

        public override void OnThink()
        {
            base.OnThink();

            if( CanTakeLife( Combatant ) )
                TakeLife( Combatant );

            if( CanSmackTalk() )
                SmackTalk();

            if( CanPutridNausea() )
                PutridNausea();
        }

        #region Smack Talk
        private DateTime m_NextSmackTalk;

        public bool CanSmackTalk()
        {
            if( m_NextSmackTalk > DateTime.Now )
                return false;

            if( Combatant == null )
                return false;

            return Hits > 0.5 * HitsMax;
        }

        public void SmackTalk()
        {
            Say( Utility.RandomMinMax( 1075102, 1075115 ) ); // Muahahahaha!  I'll feast on your flesh.

            m_NextSmackTalk = DateTime.Now + TimeSpan.FromSeconds( 2 + Utility.RandomDouble() * 3 );
        }
        #endregion

        #region Putrid Nausea
        private static Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();
        private DateTime m_NextPutridNausea;

        public bool CanPutridNausea()
        {
            if( m_NextPutridNausea > DateTime.Now )
                return false;

            return Combatant != null;
        }

        public void PutridNausea()
        {
            List<Mobile> list = new List<Mobile>();

            foreach( Mobile m in GetMobilesInRange( 4 ) )
            {
                if( CanBeHarmful( m ) && m.Player )
                    list.Add( m );
            }

            for( int i = 0; i < list.Count; i++ )
            {
                Mobile m = list[ i ];

                if( m_Table.ContainsKey( m ) )
                {
                    Timer timer = m_Table[ m ];

                    if( timer != null )
                        timer.Stop();

                    m_Table[ m ] = Timer.DelayCall( TimeSpan.FromSeconds( 30 ), new TimerStateCallback<Mobile>( EndPutridNausea ), m );
                }
                else
                    m_Table.Add( m, Timer.DelayCall( TimeSpan.FromSeconds( 30 ), new TimerStateCallback<Mobile>( EndPutridNausea ), m ) );

                m.Animate( 32, 5, 1, true, false, 0 ); // bow animation
                m.SendLocalizedMessage( 1072068 ); // Your enemy's putrid presence envelops you, overwhelming you with nausea.
            }

            m_NextPutridNausea = DateTime.Now + TimeSpan.FromSeconds( 40 + Utility.RandomDouble() * 30 );
        }

        public void EndPutridNausea( Mobile m )
        {
            m_Table.Remove( m );
        }

        public static void HandleDeath( Mobile m )
        {
            if( m_Table.ContainsKey( m ) )
            {
                Timer timer = m_Table[ m ];

                if( timer != null )
                    timer.Stop();

                m_Table.Remove( m );
            }
        }

        public static bool UnderPutridNausea( Mobile m )
        {
            return m_Table.ContainsKey( m );
        }
        #endregion

        #region Take Life
        private DateTime m_NextTakeLife;

        public bool CanTakeLife( Mobile from )
        {
            if( m_NextTakeLife > DateTime.Now )
                return false;

            if( !CanBeHarmful( from ) )
                return false;

            if( Hits > 0.1 * HitsMax || Hits < 0.025 * HitsMax )
                return false;

            return true;
        }

        public void TakeLife( Mobile from )
        {
            Hits += from.Hits / ( from.Player ? 2 : 6 );

            FixedParticles( 0x376A, 9, 32, 5005, EffectLayer.Waist );
            PlaySound( 0x1F2 );

            Say( 1075117 );  // Muahahaha!  Your life essence is MINE!
            PublicOverheadMessage( MessageType.Regular, 37, true, "* An unholy aura surrounds Kalish'ten as its wounds begin to close." );

            m_NextTakeLife = DateTime.Now + TimeSpan.FromSeconds( 15 + Utility.RandomDouble() * 45 );
        }
        #endregion

        #region Helpers
        public override bool CanSpawnHelpers { get { return true; } }
        public override int MaxHelpersWaves { get { return 3; } }

        public override void SpawnHelpers()
        {
            int count = 4;

            if( Altar != null )
            {
                count = Math.Min( Altar.Fighters.Count, 4 );

                for( int i = 0; i < count; i++ )
                {
                    Mobile fighter = Altar.Fighters[ i ];

                    if( CanBeHarmful( fighter ) )
                    {
                        BaseCreature undead;

                        switch( Utility.Random( 5 ) )
                        {
                            default: undead = new Zombie(); break;
                            case 2:
                            case 3: undead = new Skeleton(); break;
                            case 4: undead = new SkeletalKnight(); break;
                        }

                        undead.Team = Team;
                        undead.Combatant = fighter;
                        SpawnHelper( undead, GetSpawnPosition( fighter.Location, fighter.Map,  2 ) );
                    }
                }
            }
            else
            {
                for( int i = 0; i < count; i++ )
                    SpawnHelper( new Skeleton(), 4 );
            }
        }

        public void SpawnSatyrs()
        {
            SpawnHelper( new EnslavedSatyr(), 6485, 945, 19 );
            SpawnHelper( new EnslavedSatyr(), 6486, 948, 22 );
            SpawnHelper( new EnslavedSatyr(), 6487, 945, 17 );
            SpawnHelper( new EnslavedSatyr(), 6488, 947, 23 );
        }
        #endregion

        #region serialization
        public KalishtenPeerless( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}