using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
	public class MediumNarcoticPotion : BaseNarcoticPotion
	{
		#region proprietà
		public override NarcoticLevel Level{ get{ return NarcoticLevel.Medium; } }
		public override double MinUseSkill{ get{ return 90.0; } }
		#endregion
		
		#region costruttori
		[Constructable]
		public MediumNarcoticPotion( int amount ) : base( PotionEffect.NarcoticMedium, amount )
		{
			Hue = 2124;
		}
		
		[Constructable]
		public MediumNarcoticPotion() : this( 1 )
		{
		}
		
		public MediumNarcoticPotion( Serial serial ) : base( serial )
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
