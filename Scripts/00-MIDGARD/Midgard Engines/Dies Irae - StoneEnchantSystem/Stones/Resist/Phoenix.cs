using System;

using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Phoenix : StoneDefinition
	{
		public override string Name{get { return "Phoenix"; }}
		public override string Prefix{get { return "phoenix"; }}
		public override string Suffix{get { return "phoenix"; }}
		public override string NameIta{get { return "Phoenix"; }}
		public override string PrefixIta{get { return "phoenix"; }}
		public override string SuffixIta{get { return "phoenix"; }}

		public override int Hue{get { return 0x50B; }}

		public override StoneTypes StoneType{get { return StoneTypes.Phoenix; }}

		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Phoenix; }}

		private readonly Type[] m_TypesSupported = new Type[]
		{
			typeof( BaseArmor ), typeof( BaseWeapon ),
			typeof( BaseJewel), typeof( BaseClothing )
		};

		public override Type[] TypesSupported{get { return m_TypesSupported; }}
		public override int ResistLevel{get { return 1; }}
		public override SkillName EnchantSkill{get { return SkillName.MagicResist; }}
	}
}