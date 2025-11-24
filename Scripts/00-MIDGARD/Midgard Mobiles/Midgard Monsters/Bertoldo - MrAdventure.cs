using System;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public class MrAdventure : BaseChampion
    {
        public override ChampionSkullType SkullType { get { return ChampionSkullType.Greed; } }

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
        public MrAdventure()
            : base( AIType.AI_Melee )
        {
            Name = "Lord Adventure";
            Title = "La sfida";
            Body = 0x3DB;
            Hue = 2433;
            ActiveSpeed = 0.16;

            SetStr( 305, 425 );
            SetDex( 72, 150 );
            SetInt( 505, 750 );

            SetHits( 4200 );
            SetStam( 102, 300 );

            SetDamage( 25, 35 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 60, 70 );
            SetResistance( ResistanceType.Fire, 50, 60 );
            SetResistance( ResistanceType.Cold, 50, 60 );
            SetResistance( ResistanceType.Poison, 40, 50 );
            SetResistance( ResistanceType.Energy, 40, 50 );

            SetSkill( SkillName.MagicResist, 100.0 );
            SetSkill( SkillName.Tactics, 97.6, 100.0 );
            SetSkill( SkillName.Wrestling, 97.6, 100.0 );

            Fame = 22500;
            Karma = -22500;

            VirtualArmor = 70;

            //			AddItem( new FancyShirt( Utility.RandomGreenHue() ) );
            //			AddItem( new LongPants( Utility.RandomYellowHue() ) );
            //			AddItem( new JesterHat( Utility.RandomPinkHue() ) );
            //			AddItem( new Cloak( Utility.RandomPinkHue() ) );
            //			AddItem( new Sandals() );
            //
            //			HairItemID = 0x203B; // Short Hair
            //			HairHue = 0x94;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.UltraRich, 3 );
        }

        public override void OnDeath( Container c )
        {

            for( int i = 0; i < 7; i++ )
            {
                Item veste = new MaleKimono();
                veste.Name = "Primo Conquistatore di Midgard Adventure";
                veste.Hue = 1990;
                veste.LootType = LootType.Blessed;
                c.DropItem( veste );
                Item assegno = new BankCheck( 100000 );
                c.DropItem( assegno );
            }
            base.OnDeath( c );
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override double AutoDispelChance { get { return 1.0; } }
        public override bool BardImmune { get { return !Core.SE; } }
        public override bool Unprovokable { get { return Core.SE; } }
        public override bool Uncalmable { get { return Core.SE; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

        public override bool ShowFameTitle { get { return false; } }
        public override bool ClickTitle { get { return false; } }


        public void SpawnExecutioner( Mobile target )
        {

            Map map = Map;

            if( map == null )
                return;

            PublicOverheadMessage( MessageType.Regular, 64, false, "Avanti miei prodi!!" );

            int cattivi = 0;

            foreach( Mobile m in GetMobilesInRange( 10 ) )
            {
                if( m is Executioner )
                    ++cattivi;
            }

            if( cattivi < 16 )
            {
                PlaySound( 0x3D );

                int newcattivi = Utility.RandomMinMax( 3, 6 );

                for( int i = 0; i < newcattivi; ++i )
                {
                    BaseCreature executioner;

                    switch( Utility.Random( 1 ) )
                    {
                        default:
                            executioner = new Executioner(); break;

                    }

                    executioner.Team = Team;
                    executioner.Title = "Adventure Defender";
                    executioner.Hits = 150;

                    bool validLocation = false;
                    Point3D loc = Location;

                    for( int j = 0; !validLocation && j < 10; ++j )
                    {
                        int x = X + Utility.Random( 3 ) - 1;
                        int y = Y + Utility.Random( 3 ) - 1;
                        int z = map.GetAverageZ( x, y );

                        if( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
                            loc = new Point3D( x, y, Z );
                        else if( validLocation = map.CanFit( x, y, z, 16, false, false ) )
                            loc = new Point3D( x, y, z );
                    }

                    executioner.MoveToWorld( loc, map );
                    executioner.Combatant = target;
                }
            }
        }


        public void TrasportaDaMe( Mobile target )
        {
            PublicOverheadMessage( MessageType.Regular, 64, false, "Via di qui!!" );
            //			this.Say(64,"Via di qui!!" );
            Map map = Map;

            if( map == null )
                return;
            bool validLocation = false;
            Point3D loc = Location;


            Effects.SendLocationEffect( target.Location, target.Map, 0x3728, 10, 10 );
            for( int j = 0; !validLocation && j < 10; ++j )
            {
                int x = X + Utility.Random( 3 ) - 1;
                int y = Y + Utility.Random( 3 ) - 1;
                int z = map.GetAverageZ( x, y );

                if( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
                    loc = new Point3D( x, y, Z );
                else if( validLocation = map.CanFit( x, y, z, 16, false, false ) )
                    loc = new Point3D( x, y, z );
            }
            try
            {
                target.MoveToWorld( loc, map );
                Effects.SendLocationEffect( target.Location, target.Map, 0x3728, 10, 10 );
            }
            catch( Exception e )
            {
                Console.WriteLine( e.ToString() );
            }

        }

        public void FrizzaTutti( Mobile target )
        {
            PublicOverheadMessage( MessageType.Regular, 64, false, "Siete tutti paralizzati" );
            Map map = Map;

            if( map == null )
                return;

            //			bool validLocation = false;

            foreach( Mobile m in GetMobilesInRange( 10 ) )
            {
                if( m is PlayerMobile )
                {
                    m.Frozen = true;
                    new ExpireFrozenTimer( m ).Start();
                }
            }
        }
        private class ExpireFrozenTimer : Timer
        {
            private Mobile m_Owner;

            public ExpireFrozenTimer( Mobile owner )
                : base( TimeSpan.FromSeconds( 7.0 ) )
            {
                m_Owner = owner;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {

                m_Owner.Frozen = false;

            }
        }


        public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
        {
            base.AlterDamageScalarFrom( caster, ref scalar );

            if( 0.2 >= Utility.RandomDouble() ) // 
                SpawnExecutioner( caster );
            else if( 0.2 >= Utility.RandomDouble() ) // 
                FrizzaTutti( caster );
            else if( 0.2 >= Utility.RandomDouble() ) // 
                TrasportaDaMe( caster );
        }

        public void DoSpecialAbility( Mobile target )
        {
            if( target == null || target.Deleted ) //sanity
                return;


            if( 0.2 >= Utility.RandomDouble() ) // 20% chance to more ratmen
                SpawnExecutioner( target );
            else if( 0.6 >= Utility.RandomDouble() ) // 
                TrasportaDaMe( target );

            if( 0.2 >= Utility.RandomDouble() ) // 
                FrizzaTutti( target );
        }

        public override void OnGotMeleeAttack( Mobile attacker )
        {
            base.OnGotMeleeAttack( attacker );

            DoSpecialAbility( attacker );
        }

        public override void OnGaveMeleeAttack( Mobile defender )
        {
            base.OnGaveMeleeAttack( defender );

            DoSpecialAbility( defender );
        }

        public MrAdventure( Serial serial )
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
