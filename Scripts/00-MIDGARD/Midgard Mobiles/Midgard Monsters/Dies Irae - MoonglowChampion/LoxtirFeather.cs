using Server;
using Server.Engines.CannedEvil;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
	[CorpseName( "a demon knight corpse" )]
	public class LoxtirFeather : DemonKnight, IMoonglowFolk
	{
		[Constructable]
		public LoxtirFeather()
		{
			Name = "Loxtir Feather";
			Title = ", Antico Signore di Moonglow";
			BodyValue = 0x3DF;
			SetHits( 50000 );
			SetDamage( 25, 31 );

			Fame = 28000;
			Karma = -28000;
            ActiveSpeed = 0.12;

            XmlAttach.AttachTo( this, new XmlChampionBoss( true, false, true, ChampionSkullType.Greed, 4, 12 ) );
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
			AddLoot( LootPack.SuperBoss, 3 );

            base.GenerateLoot();
        }

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			c.DropItem( new Gold( 50000, 55000 ) );

            Item statue = Loot.RandomStatue();
            statue.Name = "Effige di Loxtir Feather";
            statue.Hue = 2356;
            c.DropItem( statue );
		}

        #region serialization
        public LoxtirFeather( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
        }
        #endregion
    }
}