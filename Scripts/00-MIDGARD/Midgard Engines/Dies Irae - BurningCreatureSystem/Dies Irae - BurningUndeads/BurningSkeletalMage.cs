using Midgard.Engines.CreatureBurningSystem;
using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
	[CorpseName( "a skeletal corpse burnt" )]
	public class BurningSkeletalMage : SkeletalMage, IBurningCreature
	{
		#region IBurningCreature members
		public LightEffect iEffect { get { return LightEffect.Light; } }
		public int iHue { get { return 1161; } }
		public int iLevel { get { return 3; } }
		#endregion
		
		#region costructors
		[Constructable]
		public BurningSkeletalMage()
		{
			CreatureBurningSystem.DoFireMorph( this );
			PackItem( new Items.BurningBonePile() );
		}

		public BurningSkeletalMage( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region nembers
		public override void GenerateLoot()
		{
			CreatureBurningSystem.GenerateFireLoot( this );
		}
		#endregion

		#region serial-deserial 
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
		#endregion
	}
}
