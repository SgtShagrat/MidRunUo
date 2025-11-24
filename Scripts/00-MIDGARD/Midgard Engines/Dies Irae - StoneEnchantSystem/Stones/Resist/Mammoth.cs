using System;

using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Mammoth : StoneDefinition
	{
		public override string Name{get { return "Mammoth"; }}
		public override string Prefix{get { return "mammoth"; }}
		public override string Suffix{get { return "mammoth"; }}
		public override string NameIta{get { return "Mammoth"; }}
		public override string PrefixIta{get { return "mammoth"; }}
		public override string SuffixIta{get { return "mammoth"; }}

		public override int Hue{get { return 0x892; }}

		public override StoneTypes StoneType{get { return StoneTypes.Mammoth; }}
		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Mammoth; }}

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