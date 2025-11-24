using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
	public class LightNarcoticPotion : BaseNarcoticPotion
	{
		#region proprietà
		public override NarcoticLevel Level{ get{ return NarcoticLevel.Light; } }
		public override double MinUseSkill{ get{ return 50.0; } }
		#endregion
		
		#region costruttori
		[Constructable]
		public LightNarcoticPotion( int amount ) : base( PotionEffect.NarcoticLight, amount )
		{
			Hue = 2034;
		}
		
		[Constructable]
		public LightNarcoticPotion() : this( 1 )
		{
		}
		
		public LightNarcoticPotion( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region serial-deserial
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
		#endregion
	}
}
