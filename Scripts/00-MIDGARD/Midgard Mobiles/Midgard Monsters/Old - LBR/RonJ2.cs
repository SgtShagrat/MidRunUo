using System;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

namespace Server.Mobiles
{
    public class RonJ2 : BaseChampion
    {
        public override ChampionSkullType SkullType
        {
            get { return ChampionSkullType.Greed; }
        }

        public override Type[] UniqueList
        {
            get { return new Type[] { }; }
        }

        public override Type[] SharedList
        {
            get { return new Type[] { }; }
        }

        public override Type[] DecorativeList
        {
            get { return new Type[] { }; }
        }

        public override MonsterStatuetteType[] StatueTypes
        {
            get { return new MonsterStatuetteType[] { }; }
        }

        [Constructable]
        public RonJ2()
            : base( AIType.AI_Melee )
        {
            Hue = 0x83EC;
            Body = 0x190;
            Name = "Sir RonJ";
            Title = "the mad";

            Kills = 500;

            SetFameLevel( 5 );
            SetKarmaLevel( 5 );

            SetStr( 200, 200 );
            SetDex( 90, 100 );
            SetInt( 50, 120 );

            VirtualArmor = 70;

            SetSkill( SkillName.Wrestling, 98, 100 );
            SetSkill( SkillName.Tactics, 98, 99 );
            SetSkill( SkillName.MagicResist, 100, 100 );
            SetSkill( SkillName.Magery, 15.8, 15.8 );

            EquipItem( new FancyShirt( Utility.RandomGreenHue() ) );
            EquipItem( new LongPants( Utility.RandomYellowHue() ) );
            EquipItem( new Cloak( Utility.RandomPinkHue() ) );
            EquipItem( new Boots() );

            EquipItem( new ShortHair( 148 ) );

            PackItem( new Gold( 4000, 6000 ) );

            PackGem();
            PackGem();
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override int HitsMax
        {
            get { return 1000; }
        }

        public override bool ShowFameTitle
        {
            get { return false; }
        }

        public override bool AlwaysAttackable
        {
            get { return true; }
        }

        public override bool ClickTitle
        {
            get { return false; }
        }

        public void Polymorph( Mobile m )
        {
            if( !m.CanBeginAction( typeof( PolymorphSpell ) ) || !m.CanBeginAction( typeof( IncognitoSpell ) ) || m.IsBodyMod )
                return;

            IMount mount = m.Mount;

            if( mount != null )
                mount.Rider = null;

            if( m.Mounted )
                return;

            if( m.BeginAction( typeof( PolymorphSpell ) ) )
            {
                Item disarm = m.FindItemOnLayer( Layer.OneHanded );

                if( disarm != null && disarm.Movable )
                    m.AddToBackpack( disarm );

                disarm = m.FindItemOnLayer( Layer.TwoHanded );

                if( disarm != null && disarm.Movable )
                    m.AddToBackpack( disarm );

                m.BodyMod = 225;
                m.HueMod = 0;

                new ExpirePolymorphTimer( m ).Start();
            }
        }

        private class ExpirePolymorphTimer : Timer
        {
            private Mobile m_Owner;

            public ExpirePolymorphTimer( Mobile owner )
                : base( TimeSpan.FromMinutes( 3.0 ) )
            {
                m_Owner = owner;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Owner.BodyMod = 0;
                m_Owner.HueMod = -1;
                m_Owner.EndAction( typeof( PolymorphSpell ) );
            }
        }

        public void SpawnRatmen( Mobile target )
        {
            Map map = Map;

            if( map == null )
                return;

            IPooledEnumerable eable = map.GetMobilesInRange( Location, 10 );
            int rats = 0;

            foreach( Mobile m in eable )
            {
                if( m is Ratman || m is RatmanArcher || m is RatmanMage )
                    ++rats;
            }

            eable.Free();

            if( rats < 16 )
            {
                int newRats = Utility.RandomMinMax( 3, 6 );

                for( int i = 0; i < newRats; ++i )
                {
                    BaseCreature rat;

                    switch( Utility.Random( 5 ) )
                    {
                        default:
                        case 0:
                        case 1:
                            rat = new HellHound();
                            break;
                        case 2:
                        case 3:
                            rat = new HellHound();
                            break;
                        case 4:
                            rat = new HellHound();
                            break;
                    }

                    rat.Team = Team;
                    rat.Map = map;

                    bool validLocation = false;

                    for( int j = 0; !validLocation && j < 10; ++j )
                    {
                        int x = X + Utility.Random( 3 ) - 1;
                        int y = Y + Utility.Random( 3 ) - 1;
                        int z = map.GetAverageZ( x, y );

                        if( validLocation = map.CanFit( x, y, Z, 16, false, false ) )
                            rat.Location = new Point3D( x, y, Z );
                        else if( validLocation = map.CanFit( x, y, z, 16, false, false ) )
                            rat.Location = new Point3D( x, y, z );
                    }

                    if( !validLocation )
                        rat.Location = Location;

                    rat.Combatant = target;
                }
            }
        }

        public void DoSpecialAbility( Mobile target )
        {
            if( 0.6 >= Utility.RandomDouble() ) // 60% chance to polymorph attacker into a ratman
                Polymorph( target );

            if( 0.2 >= Utility.RandomDouble() ) // 20% chance to more ratmen
                SpawnRatmen( target );

            if( Hits < 500 && !IsBodyMod ) // Baracoon is low on life, polymorph into a ratman
                Polymorph( this );
        }

        public override void OnGotMeleeAttack( Mobile attacker )
        {
            DoSpecialAbility( attacker );
        }

        public override void OnGaveMeleeAttack( Mobile defender )
        {
            DoSpecialAbility( defender );
        }

        public RonJ2( Serial serial )
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
    }
}