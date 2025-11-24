using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.PermaredToBlue
{
	public class PermaredRemover : BaseBlueGiver
	{
		[Constructable]
		public PermaredRemover() : base( "the wanderer" )
		{
			/*
				Equip 0x1F03 1159
				Equip 0x170D
				Equip 0x1718 1159
				Equip 0x13F9 1159
				Equip 0x203C 2386
			 */

			GenerateBody( false, false );
			HairItemID = 0x203C;
			HairHue = 1102;

			SetStr( 151, 175 );
			SetDex( 61, 85 );
			SetInt( 81, 95 );

			VirtualArmor = 32;

			SetSkill( SkillName.Wrestling, 110.0, 120.0 );
			SetSkill( SkillName.MagicResist, 110.0, 120.0 );
			SetSkill( SkillName.Healing, 110.0, 120.0 );
			SetSkill( SkillName.Anatomy, 110.0, 120.0 );

			AddItem( Immovable( Rehued( new HoodedShroudOfShadows(), 0x05A4 ) ) );
			AddItem( Immovable( Rehued( new Sandals(), 0x075D ) ) );
		}

		public PermaredRemover( Serial serial ) : base( serial )
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