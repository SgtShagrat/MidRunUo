using System;
using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Sonic : StoneDefinition
	{
		public override string Name{get { return "Sonic"; }}
		public override string Prefix{get { return "sonical"; }}
		public override string Suffix{get { return "sonic"; }}
		public override string NameIta{get { return "Sonica"; }}
		public override string PrefixIta{get { return "sonica"; }}
		public override string SuffixIta{get { return "sonica"; }}

		public override int Hue{get { return 0x7BA; }}

		public override StoneTypes StoneType{get { return StoneTypes.Sonical; }}

		private readonly Type[] m_TypesSupported = new Type[]
		{
			typeof( BaseArmor ), typeof( BaseWeapon ),
			typeof( BaseClothing )
		};

		public override Type[] TypesSupported{get { return m_TypesSupported; }}
		public override int AttackLevelDamage{get { return 3; }}
		public override int DefenceLevelDamage{get { return 2; }}
		public override int ManaLevelForUse{get { return 1; }}

		public override CustomResType ResistStoneType{get { return CustomResType.General; }}
		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Sonical; }}

		public override void DoSpecialEffect( Mobile possessor, Mobile victim, Item possessorItem, Item victimItem, int damage )
		{
			if( victim != null )
			{
				int manaDamage = damage;

				if( damage > victim.Stam )
					damage = victim.Stam;
				if( manaDamage > victim.Mana )
					manaDamage = victim.Mana;

				victim.Stam -= damage;
				victim.Mana -= manaDamage;

				Effects.SendLocationParticles( EffectItem.Create( victim.Location, victim.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
				Effects.PlaySound( victim, victim.Map, 0x201 );
			}
		}
	}
}