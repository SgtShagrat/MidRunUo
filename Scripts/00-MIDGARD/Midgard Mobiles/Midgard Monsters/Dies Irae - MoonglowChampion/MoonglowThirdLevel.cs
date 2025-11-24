using System;
using Server.Engines.XmlSpawner2;
using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a skeletal corpse" )]
    public class MoonglowSkeletalPoisoner : SkeletalPoisoner, IMoonglowFolk
    {
        [Constructable]
        public MoonglowSkeletalPoisoner()
        {
            string title = "arcere fallito";
            switch( Utility.Random( 5 ) )
            {
                case 0: title = "arcere fallito"; break;
                case 1: title = "spadaccino"; break;
                case 2: title = "soldato in pensione"; break;
                case 3: title = "capitano"; break;
                case 4: title = "soldato semplice"; break;
            }

            Name = "Armigero Disturbato";
            Title = String.Format( ", era un {0}", title );
            Hue = Utility.RandomYellowHue();
            Fame = 4000;
            Karma = -4000;

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
            AddLoot( LootPack.Rich );

            base.GenerateLoot();
        }

        #region serialization
        public MoonglowSkeletalPoisoner( Serial serial )
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

    [CorpseName( "a liche's corpse" )]
    public class MoonglowLich : Lich, IMoonglowFolk
    {
        [Constructable]
        public MoonglowLich()
        {
            Name = "Fantasmima del Castello";
            Title = String.Format( ", era chiamata {0}", NameList.RandomName( "female" ) );
            Hue = 20000;
            BodyValue = 772;
            Fame = 6000;
            Karma = -6000;

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
            AddLoot( LootPack.FilthyRich );

            base.GenerateLoot();
        }

        #region serialization
        public MoonglowLich( Serial serial )
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
    public class MoonglowSkeletalKnight : SkeletalKnight, IMoonglowFolk
    {
        [Constructable]
        public MoonglowSkeletalKnight()
        {
            string title = "soldato fallito";
            switch( Utility.Random( 5 ) )
            {
                case 0: title = "soldato fallito"; break;
                case 1: title = "spadaccino"; break;
                case 2: title = "soldato in pensione"; break;
                case 3: title = "capitano"; break;
                case 4: title = "soldato semplice"; break;
            }

            Name = "Armigero Disturbato";
            Title = String.Format( ", era un {0}", title );
            Hue = Utility.RandomBlueHue();
            Fame = 4000;
            Karma = -4000;

            XmlAttach.AttachTo( this, new XmlDialog( "Dies_Moonglow_Cimitero" ) );
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }

        #region serialization
        public MoonglowSkeletalKnight( Serial serial )
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