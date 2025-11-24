using Server.Engines.XmlSpawner2;
using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    public interface IMoonglowFolk
    {}

    [CorpseName( "a liche's corpse" )]
    public class MoonglowLichLord : LichLord, IMoonglowFolk
    {
        [Constructable]
        public MoonglowLichLord()
        {
            Name = "Fantasma del Mago di Corte";
            Hue = Utility.RandomNondyedHue();
            Fame = 10000;
            Karma = -10000;

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
            AddLoot( LootPack.UltraRich );

            base.GenerateLoot();
        }

        #region serialization
        public MoonglowLichLord( Serial serial )
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

    [CorpseName( "a rotting corpse" )]
    public class MoonglowRottingCorpse : RottingCorpse, IMoonglowFolk
    {
        [Constructable]
        public MoonglowRottingCorpse()
        {
            Name = "Cherichetto poco ubbidiente";
            Hue = Utility.RandomGreenHue();
            Fame = 7000;
            Karma = -7000;

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
            AddLoot( LootPack.UltraRich );

            base.GenerateLoot();
        }

        #region serialiation
        public MoonglowRottingCorpse( Serial serial )
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

    [CorpseName( "an ancient liche's corpse" )]
    public class MoonglowAncientLich : AncientLich, IMoonglowFolk
    {
        [Constructable]
        public MoonglowAncientLich()
        {
            Name = "Chierico Pazzo";
            Hue = Utility.RandomRedHue();
            BodyValue = 146;
            Fame = 15000;
            Karma = -15000;

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
            AddLoot( LootPack.UltraRich );

            base.GenerateLoot();
        }

        #region serialiation
        public MoonglowAncientLich( Serial serial )
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