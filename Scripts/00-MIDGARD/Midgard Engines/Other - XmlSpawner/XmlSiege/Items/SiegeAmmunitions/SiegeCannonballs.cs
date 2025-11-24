namespace Server.Items
{
    public abstract class SiegeCannonball : BaseSiegeProjectile
    {
        protected SiegeCannonball()
            : this( 1 )
        {
        }

        protected SiegeCannonball( int amount )
            : base( amount, 0xE74 )
        {
        }

        #region serialization
        public SiegeCannonball( Serial serial )
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

    public class LightCannonball : SiegeCannonball
    {
        [Constructable]
        public LightCannonball()
            : this( 1 )
        {
        }

        [Constructable]
        public LightCannonball( int amount )
            : base( amount )
        {
            Range = 17;
            Area = 0;
            AccuracyBonus = 0;
            PhysicalDamage = 80;
            FireDamage = 0;
            FiringSpeed = 35;
            Name = "Light Cannonball";
        }

        #region serialization
        public LightCannonball( Serial serial )
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

    public class IronCannonball : SiegeCannonball
    {
        [Constructable]
        public IronCannonball()
            : this( 1 )
        {
        }

        [Constructable]
        public IronCannonball( int amount )
            : base( amount )
        {
            Range = 15;
            Area = 0;
            AccuracyBonus = 0;
            PhysicalDamage = 100;
            FireDamage = 0;
            FiringSpeed = 25;
            Name = "Iron Cannonball";
        }

        #region serialization
        public IronCannonball( Serial serial )
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

    public class ExplodingCannonball : SiegeCannonball
    {
        [Constructable]
        public ExplodingCannonball()
            : this( 1 )
        {
        }

        [Constructable]
        public ExplodingCannonball( int amount )
            : base( amount )
        {
            Range = 11;
            Area = 1;
            AccuracyBonus = -10;
            PhysicalDamage = 10;
            FireDamage = 40;
            FiringSpeed = 20;
            Hue = 46;
            Name = "Exploding Cannonball";
        }

        #region serialization
        public ExplodingCannonball( Serial serial )
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

    public class FieryCannonball : SiegeCannonball
    {
        // use a fireball animation when fired
        public override int AnimationID { get { return 0x36D4; } }

        public override int AnimationHue { get { return 0; } }

        [Constructable]
        public FieryCannonball()
            : this( 1 )
        {
        }

        [Constructable]
        public FieryCannonball( int amount )
            : base( amount )
        {
            Range = 8;
            Area = 2;
            AccuracyBonus = -20;
            PhysicalDamage = 0;
            FireDamage = 30;
            FiringSpeed = 10;
            Hue = 33;
            Name = "Fiery Cannonball";
        }

        #region serialization
        public FieryCannonball( Serial serial )
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

    public class GrapeShot : SiegeCannonball
    {
        // only does damage to mobiles
        public override double StructureDamageMultiplier { get { return 0.0; } } //  damage multiplier for structures

        [Constructable]
        public GrapeShot()
            : this( 1 )
        {
        }

        [Constructable]
        public GrapeShot( int amount )
            : base( amount )
        {
            Range = 17;
            Area = 1;
            AccuracyBonus = 0;
            PhysicalDamage = 20;
            FireDamage = 0;
            FiringSpeed = 35;
            Name = "Grape Shot";
        }

        #region serialization
        public GrapeShot( Serial serial )
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