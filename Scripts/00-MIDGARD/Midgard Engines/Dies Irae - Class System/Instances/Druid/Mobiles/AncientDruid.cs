using Server;
using Server.Items;
using Server.Network;
using Midgard.Engines.SpellSystem;

namespace Midgard.Engines.Classes
{
	public class AncientDruid : BaseClassGiver
	{
		public override string Greetings
		{
			get { return "Would you like to serve our mother, the Nature?"; }
		}

		public override Item Book
		{
			get { return new DruidTome(); }
		}

		public override ClassSystem System
		{
			get { return ClassSystem.Druid; }
		}

		[Constructable]
		public AncientDruid() : base( "the ancient druid" )
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
			AddItem( Immovable( Rehued( Layered( new Item( 0x256C ), Layer.TwoHanded ), 0 ) ) ); // druid staff
			AddItem( Immovable( Rehued( Layered( new Item( 0x3DB7 ), Layer.Neck ), 0 ) ) ); // druid symbol
		}

		public override void EndJoin( Mobile joiner, bool join )
		{
			base.EndJoin( joiner, join );

			if( join )
			{
				joiner.FixedEffect( 0x373A, 10, 30 );
				joiner.PlaySound( 0x209 );
				joiner.PublicOverheadMessage( MessageType.Regular, Utility.RandomMinMax( 90, 95 ), true, joiner.Language == "ITA" ? "*Ora sei un candidato druido*" : "*You are now a druid candidate*" );

				// joiner.AddToBackpack( new DruidTome() );
			}
		}

		#region serialization
		public AncientDruid( Serial serial ) : base( serial )
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
		#endregion
	}
}