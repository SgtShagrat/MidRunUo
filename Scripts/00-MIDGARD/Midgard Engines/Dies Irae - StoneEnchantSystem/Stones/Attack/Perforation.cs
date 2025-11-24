using System;
using Server.Items;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Perforation : StoneDefinition
	{
		public override string Name{get { return "Perforation"; }}
		public override string Prefix{get { return "perforate"; }}
		public override string Suffix{get { return "perforation"; }}
		public override string NameIta{get { return "Perforazione"; }}
		public override string PrefixIta{get { return "perforazione"; }}
		public override string SuffixIta{get { return "perforazione"; }}

		public override int Hue{get { return 0x877; }}
		public override StoneTypes StoneType{get { return StoneTypes.Perforate; }}

		private readonly Type[] m_TypesSupported = new Type[]
		{
			typeof( BaseArmor ), typeof( BaseWeapon ),
			typeof( BaseClothing )
		};

		public override Type[] TypesSupported{get { return m_TypesSupported; }}
		public override int AttackLevelDamage{get { return 3; }}
		public override int DefenceLevelDamage{get { return 3; }}
		public override int ManaLevelForUse{get { return 1; }}

		public override CustomResType ResistStoneType{get { return CustomResType.Impact; }}

		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Perforate; }}

		public override void DoSpecialEffect( Mobile possessor, Mobile victim, Item possessorItem, Item victimItem, int damage )
		{
			if( possessor == null || victim == null || victimItem == null )
				return;

			victim.PlaySound( 0x206 );
			Effects.SendLocationEffect( victim, victim.Map, 14201, 16 );

			double chance = Utility.RandomDouble();
			Item armorItem;

			if( chance < 0.07 )
				armorItem = victim.NeckArmor;
			else if( chance < 0.14 )
				armorItem = victim.HandArmor;
			else if( chance < 0.28 )
				armorItem = victim.ArmsArmor;
			else if( chance < 0.43 )
				armorItem = victim.HeadArmor;
			else if( chance < 0.65 )
				armorItem = victim.LegsArmor;
			else
				armorItem = victim.ChestArmor;

			if( possessor.PlayerDebug && armorItem != null )
				possessor.SendMessage( "Debug: item {0}, perforate damage {1}", armorItem.GetType().Name, damage );

			if( armorItem is IDurability )
				RuinItem( victim, (IDurability)armorItem, damage );

			if( armorItem == null )
			{
				BaseShield shield = victim.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;
				if( shield != null )
					RuinItem( victim, shield, damage );
			}
		}

		private static void RuinItem( Mobile parent, IDurability item, int damage )
		{
			if( item == null )
				return;

			if( item.MaxHitPoints <= 0 )
				return;

			if( item.HitPoints >= damage )
			{
				item.HitPoints -= damage;
				damage = 0;
			}
			else
			{
				damage -= item.HitPoints;
				item.HitPoints = 0;
			}

			if( damage <= 0 )
				return;

			if( item.MaxHitPoints > damage )
			{
				item.MaxHitPoints -= damage;
				parent.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061121 ); // Your equipment is severely damaged.
			}
			else
			{
				parent.SendMessage( "Your equipment is too old and ruined! It is broken!" );
				parent.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "Houch!!" );

				if( item is Item )
					( (Item)item ).Delete();
			}
		}
	}
}