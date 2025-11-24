using System;
using System.Collections.Generic;

using Midgard.Multis;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;

namespace Midgard.Mobiles
{
    public class PirateCaptain : BaseCreature
    {
        private readonly string[] m_Phrases = new string[]
                                     {
                                        "Ye Be Dealing With A Pirate, Matey!",
                                        "Find It Wise To Cross Blades With A Pirate, Fool?",
                                        "To The Depths, Ye Shall Be Fallin'!",
                                        "Yer Time Has Come Yeh Mongrel!",
                                        "Me crew shall seek their revenge on ye!",
                        };

        private bool m_Boat;
        private Direction m_EnemyDirection;
        private DateTime m_NextPickup;
        private PirateShipBoat m_PirateBoat;
        private BaseBoat m_EnemyBoat;
        private DateTime NextAbilityTime;

        [Constructable]
        public PirateCaptain()
            : base( AIType.AI_Archer, FightMode.Closest, 15, 1, 0.2, 0.4 )
        {
            Hue = Utility.RandomSkinHue();

            Female = Utility.RandomBool();

            if( Female )
            {
                Body = 0x191;
                Name = NameList.RandomName( "female" );
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );
            }

            Title = ", The Captain";

            SpeechHue = Utility.RandomDyedHue();

            AddItem( new ThighBoots() );
            AddItem( new TricorneHat( Utility.RandomRedHue() ) );

            HairItemID = Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A );
            HairHue = Utility.RandomNondyedHue();

            if( Utility.RandomBool() && !Female )
            {
                FacialHairItemID = Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D );
                FacialHairHue = HairHue;

                Item necklace = new Necklace();
                necklace.Name = "a pirates medallion";
                necklace.Movable = false;
                necklace.Hue = 38;

                AddItem( necklace );
            }

            SetStr( 160, 205 );
            SetDex( 90, 135 );
            SetInt( 90, 125 );

            SetHits( 250, 300 );

            SetDamage( 10, 23 );

            SetSkill( SkillName.Archery, 95.0, 115.5 );
            SetSkill( SkillName.Archery, 95.0, 115.5 );
            SetSkill( SkillName.Tactics, 95.0, 100.5 );
            SetSkill( SkillName.Wrestling, 100.1, 105.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Anatomy, 100.0, 120.0 );

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 50;

            switch( Utility.Random( 1 ) )
            {
                case 0:
                    AddItem( new LongPants( Utility.RandomRedHue() ) );
                    break;
                case 1:
                    AddItem( new ShortPants( Utility.RandomRedHue() ) );
                    break;
            }

            switch( Utility.Random( 3 ) )
            {
                case 0:
                    AddItem( new FancyShirt( Utility.RandomRedHue() ) );
                    break;
                case 1:
                    AddItem( new Shirt( Utility.RandomRedHue() ) );
                    break;
                case 2:
                    AddItem( new Doublet( Utility.RandomRedHue() ) );
                    break;
            }

            switch( Utility.Random( 2 ) )
            {
                case 0: AddItem( new Bow() ); break;
                case 1: AddItem( new Crossbow() ); break;
                case 2: AddItem( new HeavyCrossbow() ); break;
            }

            PackItem( new Arrow( 75 ) );
            PackItem( new Bolt( 75 ) );
        }

        public override bool InitialInnocent
        {
            get { return true; }
        }

        public override bool IsScaredOfScaryThings
        {
            get { return false; }
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Regular; }
        }

        public override bool AutoDispel
        {
            get { return true; }
        }

        public override bool CanRummageCorpses
        {
            get { return true; }
        }

        public override bool PlayerRangeSensitive
        {
            get { return false; }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average, 2 );
        }

        public override void OnThink()
        {
            if( DateTime.Now >= NextAbilityTime && Combatant == null )
            {
                NextAbilityTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 4, 6 ) );
                Talk();
                Emote();
            }

            if( !m_Boat )
            {
                Map map = Map;
                if( map == null )
                    return;
                m_PirateBoat = new PirateShipBoat();

                Point3D loc = new Point3D( X, Y - 1, Map.GetAverageZ( X, Y ) );
                Point3D loccrew = new Point3D( X, Y - 1, Map.GetAverageZ( X, Y ) + 1 );

                m_PirateBoat.MoveToWorld( loc, map );
                m_Boat = true;

                MoveToWorld( loccrew, map );

                for( int i = 0; i < 5; ++i )
                {
                    PirateCrew crew = new PirateCrew();
                    crew.MoveToWorld( loccrew, map );
                }
            }

            base.OnThink();

            if( DateTime.Now < m_NextPickup )
                return;

            if( m_PirateBoat == null )
                return;

            m_NextPickup = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 1, 2 ) );

            m_EnemyDirection = Direction.North;

            foreach( Item enemy in GetItemsInRange( 100 ) )
            {
                if( enemy is BaseBoat && enemy != m_PirateBoat && !( enemy is PirateShipBoat ) )
                {
                    List<Mobile> targets = new List<Mobile>();
                    IPooledEnumerable eable = enemy.GetMobilesInRange( 16 );

                    foreach( Mobile m in eable )
                    {
                        if( m is PlayerMobile )
                            targets.Add( m );
                    }

                    eable.Free();

                    if( targets.Count > 0 )
                    {
                        m_EnemyBoat = enemy as BaseBoat;
                        m_EnemyDirection = GetDirectionTo( m_EnemyBoat );
                        break;
                    }
                }
            }

            if( m_EnemyBoat == null )
                return;

            if( ( m_EnemyDirection == Direction.North ) && m_PirateBoat.Facing != Direction.North )
                m_PirateBoat.Facing = Direction.North;
            else if( ( m_EnemyDirection == Direction.South ) && m_PirateBoat.Facing != Direction.South )
                m_PirateBoat.Facing = Direction.South;
            else if( ( m_EnemyDirection == Direction.East || m_EnemyDirection == Direction.Right || m_EnemyDirection == Direction.Down ) && m_PirateBoat.Facing != Direction.East )
                m_PirateBoat.Facing = Direction.East;
            else if( ( m_EnemyDirection == Direction.West || m_EnemyDirection == Direction.Left || m_EnemyDirection == Direction.Up ) && m_PirateBoat.Facing != Direction.West )
                m_PirateBoat.Facing = Direction.West;

            m_PirateBoat.StartMove( Direction.North, true );

            if( InRange( m_EnemyBoat, 10 ) && m_PirateBoat.IsMoving )
                m_PirateBoat.StopMove( true );
        }

        public override void OnDelete()
        {
            if( m_PirateBoat != null )
            {
                new SinkTimer( m_PirateBoat, this ).Start();
            }
        }

        public override void OnDamagedBySpell( Mobile caster )
        {
            if( caster == this )
                return;

            SpawnPirate( caster );
        }

        public override bool OnBeforeDeath()
        {
            if( m_PirateBoat != null )
                new SinkTimer( m_PirateBoat, this ).Start();

            return true;
        }

        private void SpawnPirate( Mobile target )
        {
            Map map = target.Map;

            if( map == null )
                return;

            int pirates = 0;

            foreach( Mobile m in GetMobilesInRange( 10 ) )
            {
                if( m is PirateCrew )
                    ++pirates;
            }

            if( pirates >= 10 || Utility.RandomDouble() > 0.25 )
                return;

            BaseCreature pirateCrew = new PirateCrew();

            Point3D loc = target.Location;
            bool validLocation = false;

            for( int j = 0; !validLocation && j < 10; ++j )
            {
                int x = target.X + Utility.Random( 3 ) - 1;
                int y = target.Y + Utility.Random( 3 ) - 1;
                int z = map.GetAverageZ( x, y );

                if( validLocation = map.CanFit( x, y, Z, 16, false, false ) )
                    loc = new Point3D( x, y, Z );
                else if( validLocation = map.CanFit( x, y, z, 16, false, false ) )
                    loc = new Point3D( x, y, z );
            }

            pirateCrew.MoveToWorld( loc, map );

            pirateCrew.Combatant = target;
        }

        private void Emote()
        {
            switch( Utility.Random( 85 ) )
            {
                case 1:
                    PlaySound( Female ? 785 : 1056 );
                    Say( "*cough!*" );
                    break;
                case 2:
                    PlaySound( Female ? 818 : 1092 );
                    Say( "*sniff*" );
                    break;
                default:
                    break;
            }
        }

        private void Talk()
        {
            Say( Utility.RandomStringList( m_Phrases ) );
        }

        #region serialization
        public PirateCaptain( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_PirateBoat );
            writer.Write( m_Boat );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_PirateBoat = reader.ReadItem() as PirateShipBoat;
            m_Boat = reader.ReadBool();
        }
        #endregion

        private class SinkTimer : Timer
        {
            private readonly BaseBoat m_Boat;
            private int m_Count;

            public SinkTimer( BaseBoat boat, Mobile m )
                : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 4.0 ) )
            {
                m_Boat = boat;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if( m_Count == 4 )
                {
                    List<Mobile> targets = new List<Mobile>();
                    IPooledEnumerable eable = m_Boat.GetMobilesInRange( 16 );

                    foreach( Mobile m in eable )
                    {
                        if( m is PirateCrew )
                            targets.Add( m );
                    }

                    eable.Free();

                    if( targets.Count > 0 )
                    {
                        foreach( Mobile m in targets )
                        {
                            m.Kill();
                        }
                    }
                }

                if( m_Count >= 15 )
                {
                    m_Boat.Delete();
                    Stop();
                }
                else
                {
                    if( m_Count < 5 )
                    {
                        m_Boat.Location = new Point3D( m_Boat.X, m_Boat.Y, m_Boat.Z - 1 );

                        if( m_Boat.TillerMan != null && m_Count < 5 )
                            m_Boat.TillerMan.Say( 1007168 + m_Count );
                    }
                    else
                        m_Boat.Location = new Point3D( m_Boat.X, m_Boat.Y, m_Boat.Z - 3 );

                    ++m_Count;
                }
            }
        }
    }
}