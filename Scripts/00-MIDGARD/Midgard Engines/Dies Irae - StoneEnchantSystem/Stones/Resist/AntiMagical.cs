using System;
using Server.Items;
using Server;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class AntiMagical : StoneDefinition
	{
		public override string Name{get { return "AntiMagical"; }}
		public override string Prefix{get { return "antimagical"; }}
		public override string Suffix{get { return "antimagic"; }}
		public override string NameIta{get { return "Antimagia"; }}
		public override string PrefixIta{get { return "antimagia"; }}
		public override string SuffixIta{get { return "antimagia"; }}

		public override int Hue{get { return 0x7C6; }}

		public override StoneTypes StoneType{get { return StoneTypes.Antimagical; }}
		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Antimagical; }}

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