using System;
using Server.Engines.XmlSpawner2;
using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a skeletal corpse" )]
    public class MoonglowBoneMagi : BoneMagi, IMoonglowFolk
    {
        [Constructable]
        public MoonglowBoneMagi()
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

        public MoonglowBoneMagi( Serial serial )
            : base( serial )
        {
        }

        #region serialization
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

    [CorpseName( "a mummy corpse" )]
    public class MoonglowMummy : Mummy, IMoonglowFolk
    {
        [Constructable]
        public MoonglowMummy()
        {
            Name = "Mummia di un Cortigiano";
            Title = String.Format( ", fu in vita {0}", NameList.RandomName( "male" ) );
            Hue = Utility.RandomMetalHue();
            Fame = 3000;
            Karma = -3000;

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

        public MoonglowMummy( Serial serial )
            : base( serial )
        {
        }

        #region serialization
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
    public class MoonglowSkeletalMage : SkeletalMage, IMoonglowFolk
    {
        [Constructable]
        public MoonglowSkeletalMage()
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
        public MoonglowSkeletalMage( Serial serial )
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
    public class MoonglowBoneKnight : BoneKnight, IMoonglowFolk
    {
        [Constructable]
        public MoonglowBoneKnight()
        {
            string title = "soldato scelto";
            switch( Utility.Random( 3 ) )
            {
                case 0: title = "soldato scelto"; break;
                case 1: title = "spadaccino provetto"; break;
                case 2: title = "soldato in congedo"; break;
            }

            Name = "Guardia Scelta";
            Title = String.Format( ", era un {0}", title );
            Hue = Utility.RandomYellowHue();
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
        public MoonglowBoneKnight( Serial serial )
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