using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
	public abstract class BaseNarcoticPotion : BasePotion
	{
		#region proprietà
		public abstract NarcoticLevel Level{ get; }
		public abstract double MinUseSkill{ get; }
		
		public override int LabelNumber
		{
			get { return 1065690; } // narcotic potion
		}
		#endregion
		
		#region costruttori
		public BaseNarcoticPotion( PotionEffect effect, int amount ) : base( 0xF0A, effect, amount )
		{
		}
		
		public BaseNarcoticPotion( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region metodi
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if( Level != NarcoticLevel.None )
				list.Add( 1065691, Enum.GetName( typeof(NarcoticLevel), Level) ); // venom level: ~1_LEVEL~
		}

		public override void Drink( Mobile from )
		{
			from.SendMessage( "That is no a good use of a narcotic potion!" );
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
