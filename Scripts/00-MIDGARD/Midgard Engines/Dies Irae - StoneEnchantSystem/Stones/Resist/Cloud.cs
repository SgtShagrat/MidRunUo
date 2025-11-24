using System;

using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Cloud : StoneDefinition
	{
		public override string Name{get { return "Cloud"; }}
		public override string Prefix{get { return "cloud"; }}
		public override string Suffix{get { return "cloud"; }}
		public override string NameIta{get { return "Cloud"; }}
		public override string PrefixIta{get { return "cloud"; }}
		public override string SuffixIta{get { return "cloud"; }}

		public override int Hue{get { return 0x88B; }}
		public override StoneTypes StoneType{get { return StoneTypes.Cloud; }}

		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Cloud; }}

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