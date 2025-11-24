using System;

using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Serpent : StoneDefinition
	{
		public override string Name{get { return "Serpent"; }}
		public override string Prefix{get { return "serpent"; }}
		public override string Suffix{get { return "serpent"; }}
		public override string NameIta{get { return "Serpent"; }}
		public override string PrefixIta{get { return "serpent"; }}
		public override string SuffixIta{get { return "serpent"; }}

		public override int Hue{get { return 0x780; }}

		public override StoneTypes StoneType{get { return StoneTypes.Serpent; }}
		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Serpent; }}

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