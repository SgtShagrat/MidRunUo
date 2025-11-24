using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
	public class HeavyNarcoticPotion : BaseNarcoticPotion
	{
		#region proprietà
		public override NarcoticLevel Level{ get{ return NarcoticLevel.Heavy; } }
		public override double MinUseSkill{ get{ return 100.0; } }
		#endregion
		
		#region costruttori
		[Constructable]
		public HeavyNarcoticPotion( int amount ) : base( PotionEffect.NarcoticHeavy, amount )
		{
			Hue = 2437;
		}
		
		[Constructable]
		public HeavyNarcoticPotion() : this( 1 )
		{
		}
		
		public HeavyNarcoticPotion( Serial serial ) : base( serial )
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
