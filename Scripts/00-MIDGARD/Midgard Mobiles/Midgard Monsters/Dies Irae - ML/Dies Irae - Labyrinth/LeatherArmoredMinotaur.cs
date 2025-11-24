namespace Server.Mobiles
{
	[CorpseName( "a leather armored minotaur corpse" )]
	public class LeatherArmoredMinotaur : Minotaur
	{
		[Constructable]
		public LeatherArmoredMinotaur()
		{
			Name = "a leather armored minotaur";
			Body = 281;

			SetStr( 1267, 1545 );
			SetDex( 366, 475 );
			SetInt( 246, 270 );

			SetHits( 1976, 2352 );

			SetDamage( 50, 75 );

			SetDamageType( ResistanceType.Physical, 100 );			

			SetResistance( ResistanceType.Physical, 66, 75 );
			SetResistance( ResistanceType.Fire, 35, 45 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 41, 50 );
			SetResistance( ResistanceType.Energy, 41, 50 );

			SetSkill( SkillName.Wrestling, 90.5, 105.2 );
			SetSkill( SkillName.Tactics, 92.0, 107.1 );
			SetSkill( SkillName.MagicResist, 66.5, 74.9 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 4 );
		}

		public LeatherArmoredMinotaur( Serial serial ) : base( serial )
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
	}
}
