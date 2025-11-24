using System;
using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Fire : StoneDefinition
	{
		public override string Name{get { return "Fire"; }}
		public override string Prefix{get { return "flaming"; }}
		public override string Suffix{get { return "fire"; }}
		public override string NameIta{get { return "Fuoco"; }}
		public override string PrefixIta{get { return "fuoco"; }}
		public override string SuffixIta{get { return "fuoco"; }}

		public override int Hue{get { return 0x98E; }}
		public override StoneTypes StoneType{get { return StoneTypes.Flamming; }}

		private readonly Type[] m_TypesSupported = new Type[]
		{
			typeof( BaseArmor ), typeof( BaseWeapon ),
			typeof( BaseClothing )
		};

		public override Type[] TypesSupported{get { return m_TypesSupported; }}
		public override int AttackLevelDamage{get { return 3; }}
		public override int DefenceLevelDamage{get { return 2; }}
		public override int ManaLevelForUse{get { return 1; }}

		public override CustomResType ResistStoneType{get { return CustomResType.Fire; }}
		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Flamming; }}

		public override void DoSpecialEffect( Mobile possessor, Mobile victim, Item possessorItem, Item victimItem, int damage )
		{
			if( victim != null )
			{
				victim.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
				victim.PlaySound( 0x208 );

				victim.Damage( damage, possessor );
			}
		}
	}
}