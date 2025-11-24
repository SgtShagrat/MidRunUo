using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
	public class RegularNarcoticPotion : BaseNarcoticPotion
	{
		#region proprietà
		public override NarcoticLevel Level{ get{ return NarcoticLevel.Regular; } }
		public override double MinUseSkill{ get{ return 60.0; } }
		#endregion
		
		#region costruttori
		[Constructable]
		public RegularNarcoticPotion( int amount ) : base( PotionEffect.NarcoticRegular, amount )
		{
			Hue = 2119;
		}
		
		[Constructable]
		public RegularNarcoticPotion() : this( 1 )
		{
		}
		
		public RegularNarcoticPotion( Serial serial ) : base( serial )
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
