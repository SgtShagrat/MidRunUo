using System;
using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Spectre : StoneDefinition
	{
		public override string Name{get { return "Spectre"; }}
		public override string Prefix{get { return "spectral"; }}
		public override string Suffix{get { return "spectre"; }}
		public override string NameIta{get { return "Spettro"; }}
		public override string PrefixIta{get { return "spettro"; }}
		public override string SuffixIta{get { return "spettro"; }}

		public override int Hue{get { return 0x9CE; }}
		public override StoneTypes StoneType{get { return StoneTypes.Spectral; }}

		private readonly Type[] m_TypesSupported = new Type[]
		{
			typeof( BaseArmor ), typeof( BaseWeapon ),
			typeof( BaseClothing )
		};

		public override Type[] TypesSupported{get { return m_TypesSupported; }}

		public override int AttackLevelDamage{get { return 2; }}
		public override int DefenceLevelDamage{get { return 1; }}
		public override int ManaLevelForUse{get { return 1; }}

		public override CustomResType ResistStoneType{get { return CustomResType.General; }}
		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Spectral; }}

		public override void DoSpecialEffect( Mobile possessor, Mobile victim, Item possessorItem, Item victimItem, int damage )
		{
			if( victim == null )
				return;

			int manaDamage = damage;

			if( manaDamage > victim.Mana )
				manaDamage = victim.Mana;

			victim.Mana -= manaDamage;
			possessor.Heal( manaDamage );

			victim.FixedParticles( 0x374A, 10, 15, 5054, EffectLayer.Head );
			victim.PlaySound( 0x1F9 );
		}
	}
}