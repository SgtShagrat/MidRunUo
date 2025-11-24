using System;
using Server;

namespace Server.Items
{
	public class ExplosionPotion : BaseExplosionPotion
	{
        // mod by Dies Irae
        // http://web.archive.org/web/20000207232134/uo.stratics.com/alchemy/tactics/tactic1.html
		public override int MinDamage { get { return Core.AOS ? 10 : 6; } }
        public override int MaxDamage { get { return Core.AOS ? 15 : 10; } }

		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public ExplosionPotion( int amount ) : base( PotionEffect.Explosion, amount )
		{
		}
		
		[Constructable]
		public ExplosionPotion() : this(1)
		{
		}
		#endregion
		
		public ExplosionPotion( Serial serial ) : base( serial )
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
