namespace Server.Items
{
    /*
    public abstract class BaseMagicalGem : Item
    {
        public override double DefaultWeight
        {
            get { return 1.0; }
        }

        public BaseMagicalGem()
            : base( 10299 )
        {
        }

        public BaseMagicalGem( Serial serial )
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
    }

    public class MagicalRuby : BaseMagicalGem
    {
        [Constructable]
        public MagicalRuby()
        {
            Name = "Magical Ruby";
            Hue = 2600;
        }

        public MagicalRuby( Serial serial )
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
    }

    public class MagicalJade : BaseMagicalGem
    {
        [Constructable]
        public MagicalJade()
        {
            Name = "Magical Jade";
            Hue = 2592;
        }

        public MagicalJade( Serial serial )
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
    }

    public class MagicalSapphire : BaseMagicalGem
    {
        [Constructable]
        public MagicalSapphire()
        {
            Name = "Magical Sapphire";
            Hue = 2589;
        }

        public MagicalSapphire( Serial serial )
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
    }

    public class MagicalCitrine : BaseMagicalGem
    {
        [Constructable]
        public MagicalCitrine()
        {
            Name = "Magical Citrine";
            Hue = 2584;
        }

        public MagicalCitrine( Serial serial )
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
    }

    public class MagicalAmethyst : BaseMagicalGem
    {
        [Constructable]
        public MagicalAmethyst()
        {
            Name = "Magical Amethyst";
            Hue = 2582;
        }

        public MagicalAmethyst( Serial serial )
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
    }

    public class MagicalBloodStone : BaseMagicalGem
    {
        [Constructable]
        public MagicalBloodStone()
        {
            Name = "Magical Blood Stone";
            Hue = 2575;
        }

        public MagicalBloodStone( Serial serial )
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
    }

    public class MagicalBlueDiamond : BaseMagicalGem
    {
        [Constructable]
        public MagicalBlueDiamond()
        {
            Name = "Magical Blue Diamond";
            Hue = 2572;
        }

        public MagicalBlueDiamond( Serial serial )
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
    }

    public class MagicalPearl : BaseMagicalGem
    {
        [Constructable]
        public MagicalPearl()
        {
            Name = "Magical Pearl";
            Hue = 2553;
        }

        public MagicalPearl( Serial serial )
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
    }

    public class MagicalOnyx : BaseMagicalGem
    {
        [Constructable]
        public MagicalOnyx()
        {
            Name = "Magical Onyx";
            Hue = 2549;
        }

        public MagicalOnyx( Serial serial )
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
    }

    public class MagicalEmerald : BaseMagicalGem
    {
        [Constructable]
        public MagicalEmerald()
        {
            Name = "Magical Emerald";
            Hue = 2542;
        }

        public MagicalEmerald( Serial serial )
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
    }

    public class MagicalStarRose : BaseMagicalGem
    {
        [Constructable]
        public MagicalStarRose()
        {
            Name = "Magical Star Rose";
            Hue = 2520;
        }

        public MagicalStarRose( Serial serial )
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
    }

    public class MagicalStarSapphire : BaseMagicalGem
    {
        [Constructable]
        public MagicalStarSapphire()
        {
            Name = "Magical Star Sapphire";
            Hue = 2500;
        }

        public MagicalStarSapphire( Serial serial )
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
    }

    public class MagicalTurquoise : BaseMagicalGem
    {
        [Constructable]
        public MagicalTurquoise()
        {
            Name = "Magical Turquoise";
            Hue = 2474;
        }

        public MagicalTurquoise( Serial serial )
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
    }

    public class MagicalFireEmerald : BaseMagicalGem
    {
        [Constructable]
        public MagicalFireEmerald()
        {
            Name = "Magical Fire Emerald";
            Hue = 2279;
        }

        public MagicalFireEmerald( Serial serial )
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
    }

    public class MagicalJasper : BaseMagicalGem
    {
        [Constructable]
        public MagicalJasper()
        {
            Name = "Magical Jasper";
            Hue = 2985;
        }

        public MagicalJasper( Serial serial )
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
    }

    public class MagicalDiamond : BaseMagicalGem
    {
        [Constructable]
        public MagicalDiamond()
        {
            Name = "Magical Diamond";
            Hue = 2973;
        }

        public MagicalDiamond( Serial serial )
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
    }

    public class MagicalEclipseStone : BaseMagicalGem
    {
        [Constructable]
        public MagicalEclipseStone()
        {
            Name = "Magical Eclipse Stone";
            Hue = 2929;
        }

        public MagicalEclipseStone( Serial serial )
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
    }

    public class MagicalMoonStoneGem : BaseMagicalGem
    {
        [Constructable]
        public MagicalMoonStoneGem()
        {
            Name = "Magical Moon Stone";
            Hue = 2880;
        }

        public MagicalMoonStoneGem( Serial serial )
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
    }

    public class MagicalSunStone : BaseMagicalGem
    {
        [Constructable]
        public MagicalSunStone()
        {
            Name = "Magical Sun Stone";
            Hue = 2870;
        }

        public MagicalSunStone( Serial serial )
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
    }

    public class MagicalAmber : BaseMagicalGem
    {
        [Constructable]
        public MagicalAmber()
        {
            Name = "Magical Amber";
            Hue = 2774;
        }

        public MagicalAmber( Serial serial )
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
    }

    public class MagicalOpal : BaseMagicalGem
    {
        [Constructable]
        public MagicalOpal()
        {
            Name = "Magical Opal";
            Hue = 2738;
        }

        public MagicalOpal( Serial serial )
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
    }

    public class MagicalTourmaline : BaseMagicalGem
    {
        [Constructable]
        public MagicalTourmaline()
        {
            Name = "Magical Tourmaline";
            Hue = 2983;
        }

        public MagicalTourmaline( Serial serial )
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
    }

    public class MagicalTopaz : BaseMagicalGem
    {
        [Constructable]
        public MagicalTopaz()
        {
            Name = "Magical Topaz";
            Hue = 2975;
        }

        public MagicalTopaz( Serial serial )
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
    }

    public class MagicalFireOpal : BaseMagicalGem
    {
        [Constructable]
        public MagicalFireOpal()
        {
            Name = "Magical Fire Opal";
            Hue = 2982;
        }

        public MagicalFireOpal( Serial serial )
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
    }

    public class MagicalStarRuby : BaseMagicalGem
    {
        [Constructable]
        public MagicalStarRuby()
        {
            Name = "Magical Star Ruby";
            Hue = 2953;
        }

        public MagicalStarRuby( Serial serial )
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
    }
     */
}