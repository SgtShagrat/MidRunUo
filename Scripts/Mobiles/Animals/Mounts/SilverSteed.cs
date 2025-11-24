using System;
using Server.Mobiles;

using Midgard.Engines.PetSystem;

namespace Server.Mobiles
{
	[CorpseName( "a silver steed corpse" )]
	public class SilverSteed : BaseMount
	{
		[Constructable]
		public SilverSteed() : this( "a silver steed" )
		{
		}

		[Constructable]
		public SilverSteed( string name ) : base( name, 0x75, 0x3EA8, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			#region mod by Dies Irae
			/*
			InitStats( Utility.Random( 50, 30 ), Utility.Random( 50, 30 ), 10 );
			Skills[SkillName.MagicResist].Base = 25.0 + (Utility.RandomDouble() * 5.0);
			Skills[SkillName.Wrestling].Base = 35.0 + (Utility.RandomDouble() * 10.0);
			Skills[SkillName.Tactics].Base = 30.0 + (Utility.RandomDouble() * 15.0);

			ControlSlots = 1;
			Tamable = true;
			MinTameSkill = 103.1;
			*/
			BaseSoundID = 0xA8;

			SetDamageType( ResistanceType.Energy, 100 );

			SetSkill( SkillName.MagicResist, 100.0, 120.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			Fame = 20000;
			Karma = 20000;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 106.0;
			#endregion
		}

		public SilverSteed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}