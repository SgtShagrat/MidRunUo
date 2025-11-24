using Server;
using Server.Engines.CannedEvil;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Poison=Server.Poison;

namespace Midgard.Mobiles
{
    public interface IHythlothFolk
    {}

    [CorpseName( "a pixie corpse" )]
    public class HythlothPixie : Pixie, IHythlothFolk
    {
        [Constructable]
        public HythlothPixie()
        {
            Karma = -4000;
            AI = AIType.AI_Mage;
            FightMode = FightMode.Closest;
        }

        #region serialization

        public HythlothPixie( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion
    }

    [CorpseName( "a wisp corpse" )]
    public class HythlothWisp : Wisp, IHythlothFolk
    {
        [Constructable]
        public HythlothWisp()
        {
            Karma = -2000;
            AI = AIType.AI_Mage;
            FightMode = FightMode.Aggressor;
        }

        #region serialization

        public HythlothWisp( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion
    }

    [CorpseName( "an elemental corpse" )]
    public class HythlothAirelemental : Efreet, IHythlothFolk
    {
        [Constructable]
        public HythlothAirelemental()
        {
            Name = "a spirit of air";
            Karma = -2000;
        }

        #region serialization

        public HythlothAirelemental( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion
    }

    [CorpseName( "a ki-rin corpse" )]
    public class HythlothKirin : Kirin, IHythlothFolk
    {
        [Constructable]
        public HythlothKirin()
        {
            Name = "a savage ki-rin";
            AI = AIType.AI_Mage;
            FightMode = FightMode.Weakest;
            Karma = -6000;
            Tamable = false;
        }

        #region serialization

        public HythlothKirin( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion
    }

    [CorpseName( "an unicorn corpse" )]
    public class HythlothUnicorn : Unicorn, IHythlothFolk
    {
        [Constructable]
        public HythlothUnicorn()
        {
            Name = "a savage unicorn";
            AI = AIType.AI_Mage;
            FightMode = FightMode.Strongest;
            Karma = -8000;
            Tamable = false;
        }

        #region serialization

        public HythlothUnicorn( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion
    }

    [CorpseName( "an centaur corpse" )]
    public class HythlothCentaur : Centaur, IHythlothFolk
    {
        [Constructable]
        public HythlothCentaur()
        {
            Name = "a centaur unicorn";
            AI = AIType.AI_Archer;
            FightMode = FightMode.Closest;
            Karma = -7000;
        }

        #region serialization

        public HythlothCentaur( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion
    }

    [CorpseName( "an etheral warrior corpse" )]
    public class HythlothEtherealWarrior : EtherealWarrior, IHythlothFolk
    {
        [Constructable]
        public HythlothEtherealWarrior()
        {
            Name = "a fallen warrior";
            AI = AIType.AI_Mage;
            FightMode = FightMode.Closest;
            Karma = -15000;
        }

        #region serialization

        public HythlothEtherealWarrior( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion
    }

    [CorpseName( "an etheral warrior corpse" )]
    public class HythlothSerpentineDragon : SerpentineDragon, IHythlothFolk
    {
        [Constructable]
        public HythlothSerpentineDragon()
        {
            Name = "a savage dragon";
            AI = AIType.AI_Mage;
            FightMode = FightMode.Closest;
            Karma = -15000;
        }

        #region serialization

        public HythlothSerpentineDragon( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion
    }

    [CorpseName( "the corpse of a queen" )]
    public class HythlothQueenOfAirAndDarkness : BaseCreature, IHythlothFolk
    {
        [Constructable]
        public HythlothQueenOfAirAndDarkness()
            : base( AIType.AI_Mage, FightMode.Evil, 18, 1, 0.1, 0.2 )
        {
            Body = 0xB0;
            Name = "The Queen of Air And Darkness";

            SetHits( 25000 );
            SetDamage( 25, 33 );

            Fame = 28000;
            Karma = -28000;
            ActiveSpeed = 0.2;

            XmlAttach.AttachTo( this, new XmlChampionBoss( true, true, true, ChampionSkullType.Enlightenment, 6, 12 ) );
        }

        public override bool AutoDispel
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override bool Unprovokable
        {
            get { return true; }
        }

        public override bool Uncalmable
        {
            get { return true; }
        }


        public override OppositionGroup OppositionGroup
        {
            get { return OppositionGroup.FeyAndUndead; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Deadly; }
        }

        public void SpawnPixies( Mobile target )
        {
            Map map = Map;

            if( map == null )
                return;

            int newPixies = Utility.RandomMinMax( 3, 6 );

            for( int i = 0; i < newPixies; ++i )
            {
                HythlothPixie pixie = new HythlothPixie();

                pixie.Team = Team;
                pixie.FightMode = FightMode.Closest;

                bool validLocation = false;
                Point3D loc = Location;

                for( int j = 0; !validLocation && j < 10; ++j )
                {
                    int x = X + Utility.Random( 3 ) - 1;
                    int y = Y + Utility.Random( 3 ) - 1;
                    int z = map.GetAverageZ( x, y );

                    if( validLocation = map.CanFit( x, y, Z, 16, false, false ) )
                        loc = new Point3D( x, y, Z );
                    else if( validLocation = map.CanFit( x, y, z, 16, false, false ) )
                        loc = new Point3D( x, y, z );
                }

                pixie.MoveToWorld( loc, map );
                pixie.Combatant = target;
            }
        }

        public override int GetAngerSound()
        {
            return 0x2F8;
        }

        public override int GetIdleSound()
        {
            return 0x2F8;
        }

        public override int GetAttackSound()
        {
            return Utility.Random( 0x2F5, 2 );
        }

        public override int GetHurtSound()
        {
            return 0x2F9;
        }

        public override int GetDeathSound()
        {
            return 0x2F7;
        }

        public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
        {
            if( 0.1 >= Utility.RandomDouble() )
                SpawnPixies( caster );
        }

        public override void OnGaveMeleeAttack( Mobile defender )
        {
            base.OnGaveMeleeAttack( defender );

            defender.Damage( Utility.Random( 20, 10 ), this );
            defender.Stam -= Utility.Random( 20, 10 );
            defender.Mana -= Utility.Random( 20, 10 );
        }

        public override void OnGotMeleeAttack( Mobile attacker )
        {
            base.OnGotMeleeAttack( attacker );

            if( 0.1 >= Utility.RandomDouble() )
                SpawnPixies( attacker );

            attacker.Damage( Utility.Random( 20, 10 ), this );
            attacker.Stam -= Utility.Random( 20, 10 );
            attacker.Mana -= Utility.Random( 20, 10 );
        }

        #region serialization

        public HythlothQueenOfAirAndDarkness( Serial serial )
            : base( serial )
        {
        }

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