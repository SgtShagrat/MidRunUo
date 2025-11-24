using System;
using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Vampire : StoneDefinition
	{
		public override string Name{get { return "Vampire"; }}
		public override string Prefix{get { return "vampiric"; }}
		public override string Suffix{get { return "vampire"; }}
		public override string NameIta{get { return "Vampiro"; }}
		public override string PrefixIta{get { return "vampiro"; }}
		public override string SuffixIta{get { return "vampiro"; }}

		public override int Hue{get { return 0x87F; }}
		public override StoneTypes StoneType{get { return StoneTypes.Vampirical; }}

		private readonly Type[] m_TypesSupported = new Type[]
		{
			typeof( BaseArmor ), typeof( BaseWeapon ), typeof( BaseClothing )
		};

		public override Type[] TypesSupported{get { return m_TypesSupported; }}

		public override int AttackLevelDamage{get { return 3; }}
		public override int DefenceLevelDamage{get { return 2; }}
		public override int ManaLevelForUse{get { return 1; }}

		public override CustomResType ResistStoneType{get { return CustomResType.General; }}
		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Vampirical; }}

		public override void DoSpecialEffect( Mobile possessor, Mobile victim, Item possessorItem, Item victimItem, int damage )
		{
			if( victim != null )
			{
				int oldHp = victim.Hits;
				int newDamage = damage;

				if( oldHp < damage )
					newDamage = oldHp;

				victim.Damage( newDamage, possessor );
				possessor.Heal( newDamage );

				victim.FixedEffect( 0x37B9, 10, 16 );
				Effects.PlaySound( victim, victim.Map, 0x051 );
			}
		}
	}
}