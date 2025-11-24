using System;
using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Electric : StoneDefinition
	{
		public override string Name{get { return "Electric"; }}
		public override string Prefix{get { return "electrical"; }}
		public override string Suffix{get { return "electricity"; }}
		public override string NameIta{get { return "Elettrica"; }}
		public override string PrefixIta{get { return "elettrica"; }}
		public override string SuffixIta{get { return "elettricità"; }}

		public override int Hue{get { return 0xBA8; }}

		public override StoneTypes StoneType{get { return StoneTypes.Electrical; }}

		private readonly Type[] m_TypesSupported = new Type[]
		   {
			   typeof( BaseArmor ), typeof( BaseWeapon ),
			   typeof( BaseClothing )
		   };

		public override Type[] TypesSupported{get { return m_TypesSupported; }}
		public override int AttackLevelDamage{get { return 2; }}
		public override int DefenceLevelDamage{get { return 3; }}
		public override int ManaLevelForUse{get { return 1; }}

		public override CustomResType ResistStoneType{get { return CustomResType.Electric; }}
		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Electrical; }}

		public override void DoSpecialEffect( Mobile possessor, Mobile victim, Item possessorItem, Item victimItem, int damage )
		{
			if( victim != null )
			{
				victim.BoltEffect( 0 );
				victim.Damage( damage );
			}
		}
	}
}