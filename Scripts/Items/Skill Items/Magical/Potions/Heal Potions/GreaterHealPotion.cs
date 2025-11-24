using System;
using Server;

namespace Server.Items
{
	public class GreaterHealPotion : BaseHealPotion
	{
		public override int MinHeal { get { return (Core.AOS ? 20 : 9); } }
		public override int MaxHeal { get { return (Core.AOS ? 25 : 30); } }
		public override double Delay{ get{ return 16.0; } }

        #region Modifica by Dies Irae per le pozioni Stackable
        public override string HealDice { get { return "4d5+15"; } }

        [Constructable]
        public GreaterHealPotion( int amount )
            : base( PotionEffect.HealGreater, amount )
        {
        }

        [Constructable]
        public GreaterHealPotion()
            : this( 1 )
        {
        }

        public override void DoHeal( Mobile from )
        {
            base.DoHeal( from );

			from.FixedParticles( 0x376A, 9, 32, 5007, EffectLayer.Waist );
			from.PlaySound( 0x1E3 );
        }
        #endregion

		public GreaterHealPotion( Serial serial ) : base( serial )
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
