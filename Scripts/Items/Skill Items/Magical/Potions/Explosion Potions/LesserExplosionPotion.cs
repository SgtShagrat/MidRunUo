using System;
using Server;

namespace Server.Items
{
	public class LesserExplosionPotion : BaseExplosionPotion
	{
        // mod by Dies Irae
        // http://web.archive.org/web/20000207232134/uo.stratics.com/alchemy/tactics/tactic1.html
		public override int MinDamage { get { return Core.AOS ? 5 : 1; } }
		public override int MaxDamage { get { return Core.AOS ? 10 : 5; } }
	
		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public LesserExplosionPotion( int amount ) : base( PotionEffect.ExplosionLesser, amount )
		{
		}
		
		[Constructable]
		public LesserExplosionPotion() : this(1)
		{
		}
		#endregion

		public LesserExplosionPotion( Serial serial ) : base( serial )
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
