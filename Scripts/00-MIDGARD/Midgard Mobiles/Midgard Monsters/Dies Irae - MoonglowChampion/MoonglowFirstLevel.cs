using System;
using Server.Engines.XmlSpawner2;
using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a gore fiend corpse" )]
    public class MoonglowGoreFiend : GoreFiend, IMoonglowFolk
    {
        [Constructable]
        public MoonglowGoreFiend()
        {
            Name = String.Format( "Corpo Decomposto di un certo {0}", NameList.RandomName( "male" ) );
            Body = 3;
            Hue = Utility.RandomNeutralHue();
            Fame = 1000;
            Karma = -1000;

            XmlAttach.AttachTo( this, new XmlDialog( "Dies_Moonglow_Cimitero" ) );
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );

            base.GenerateLoot();
        }

        #region serialization
        public MoonglowGoreFiend( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }

    [CorpseName( "a ghostly corpse" )]
    public class MoonglowBogle : Bogle, IMoonglowFolk
    {
        [Constructable]
        public MoonglowBogle()
        {
            Name = String.Format( "Corpo Decomposto di un certo {0}", NameList.RandomName( "male" ) );
            Body = 3;
            Hue = Utility.RandomNeutralHue();
            Fame = 1000;
            Karma = -1000;

            XmlAttach.AttachTo( this, new XmlDialog( "Dies_Moonglow_Cimitero" ) );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
        }

        #region serialization
        public MoonglowBogle( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }

    [CorpseName( "a ghostly corpse" )]
    public class MoonglowGhoul : Ghoul, IMoonglowFolk
    {
        [Constructable]
        public MoonglowGhoul()
        {
            Name = String.Format( "Corpo Decomposto di un certo {0}", NameList.RandomName( "male" ) );
            Body = 3;
            Hue = Utility.RandomNeutralHue();
            Fame = 1000;
            Karma = -1000;

            XmlAttach.AttachTo( this, new XmlDialog( "Dies_Moonglow_Cimitero" ) );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
        }

        #region serialization
        public MoonglowGhoul( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }

    [CorpseName( "a ghostly corpse" )]
    public class MoonglowShade : Shade, IMoonglowFolk
    {
        [Constructable]
        public MoonglowShade()
        {
            Name = String.Format( "Corpo Decomposto di un certo {0}", NameList.RandomName( "male" ) );
            Body = 3;
            Hue = Utility.RandomNeutralHue();
            Fame = 1000;
            Karma = -1000;

            XmlAttach.AttachTo( this, new XmlDialog( "Dies_Moonglow_Cimitero" ) );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
        }

        #region serialization
        public MoonglowShade( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }

    [CorpseName( "a ghostly corpse" )]
    public class MoonglowSpectre : Spectre, IMoonglowFolk
    {
        [Constructable]
        public MoonglowSpectre()
        {
            Name = String.Format( "Corpo Decomposto di un certo {0}", NameList.RandomName( "male" ) );
            Body = 3;
            Hue = Utility.RandomNeutralHue();
            Fame = 1000;
            Karma = -1000;

            XmlAttach.AttachTo( this, new XmlDialog( "Dies_Moonglow_Cimitero" ) );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
        }

        #region serialization
        public MoonglowSpectre( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }

    [CorpseName( "a ghostly corpse" )]
    public class MoonglowWraith : Wraith, IMoonglowFolk
    {
        [Constructable]
        public MoonglowWraith()
        {
            Name = String.Format( "Corpo Decomposto di un certo {0}", NameList.RandomName( "male" ) );
            Body = 3;
            Hue = Utility.RandomNeutralHue();
            Fame = 1000;
            Karma = -1000;

            XmlAttach.AttachTo( this, new XmlDialog( "Dies_Moonglow_Cimitero" ) );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
        }

        #region serialization
        public MoonglowWraith( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }

    [CorpseName( "a skeletal corpse" )]
    public class MoonglowWailingBanshee : WailingBanshee, IMoonglowFolk
    {
        [Constructable]
        public MoonglowWailingBanshee()
        {
            string action = "scricchiola";
            switch( Utility.Random( 5 ) )
            {
                case 0: action = "scricchiola"; break;
                case 1: action = "cade a pezzi"; break;
                case 2: action = "barcolla"; break;
                case 3: action = "inciampa"; break;
                case 4: action = "ti minaccia"; break;
            }

            Name = String.Format( "Scheletro che {0}", action );
            Hue = Utility.RandomSnakeHue();
            Body = 50;
            Fame = 2000;
            Karma = -2000;

            XmlAttach.AttachTo( this, new XmlDialog( "Dies_Moonglow_Cimitero" ) );
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );

            base.GenerateLoot();
        }

        #region serialization
        public MoonglowWailingBanshee( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }
}