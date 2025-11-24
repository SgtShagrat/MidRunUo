using System;

using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Lockmind : StoneDefinition
	{
		public override string Name{get { return "Lockmind"; }}
		public override string Prefix{get { return "Lockmind"; }}
		public override string Suffix{get { return "mindblock"; }}
		public override string NameIta{get { return "Lockmind"; }}
		public override string PrefixIta{get { return "Lockmind"; }}
		public override string SuffixIta{get { return "mindblock"; }}

		public override int Hue{get { return 0x9F4; }}

		public override StoneTypes StoneType{get { return StoneTypes.Lockmind; }}
		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Lockmind; }}

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