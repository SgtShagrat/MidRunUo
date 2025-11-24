using System;
using Server;

namespace Server.Items
{
	public class LesserHealPotion : BaseHealPotion
	{
		public override int MinHeal { get { return (Core.AOS ? 6 : 3); } }
		public override int MaxHeal { get { return (Core.AOS ? 8 : 10); } }
		public override double Delay{ get{ return (Core.AOS ? 3.0 : 12.0); } }

        #region Modifica by Dies Irae per le pozioni Stackable
        public override string HealDice { get { return "2d5"; } }

        [Constructable]
        public LesserHealPotion( int amount )
            : base( PotionEffect.HealLesser, amount )
        {
        }

        [Constructable]
        public LesserHealPotion()
            : this( 1 )
        {
        }
        #endregion
		
		public LesserHealPotion( Serial serial ) : base( serial )
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
