using System;
using System.Collections;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class Explosion : StoneDefinition
	{
		private readonly Type[] m_TypesSupported = new Type[]
		{
			typeof (BaseArmor), typeof (BaseWeapon),
			typeof (BaseClothing)
		};

		public override string Name{get { return "Explosion"; }}
		public override string Prefix{get { return "exploding"; }}
		public override string Suffix{get { return "explosion"; }}
		public override string NameIta{get { return "Esplosione"; }}
		public override string PrefixIta{get { return "esplosione"; }}
		public override string SuffixIta{get { return "esplosione"; }}

		public override int Hue{get { return 0x49C; }}
		public override StoneTypes StoneType{get { return StoneTypes.Explosion; }}

		public override Type[] TypesSupported{get { return m_TypesSupported; }}

		public override int AttackLevelDamage{get { return 2; }}
		public override int DefenceLevelDamage{get { return 1; }}
		public override int ManaLevelForUse{get { return 1; }}

		public override CustomResType ResistStoneType{get { return CustomResType.General; }}

		public override StoneAttributeFlag StoneFlag{get { return StoneAttributeFlag.Explosion; }}

		public override void DoSpecialEffect( Mobile possessor, Mobile victim, Item possessorItem, Item victimItem, int damage )
		{
			if( victim == null )
				return;

			Point3D loc = victim.Location;
			Map map = victim.Map;

			Effects.PlaySound( loc, map, 0x207 );
			Effects.SendLocationEffect( loc, map, 0x36BD, 20 );

			IPooledEnumerable eable = map.GetMobilesInRange( loc, 3 );

			foreach( Mobile m in eable )
			{
				if( possessor == null || m == possessor )
					continue;

				if( ( !SpellHelper.ValidIndirectTarget( possessor, m, true, false, false ) || !possessor.CanBeHarmful( m, false ) ) )
					continue;

				if( !m.InLOS( possessor ) )
					continue;

				if (possessor.CanAreaHarmful( m ) )
					possessor.DoHarmful( m );

				AOS.Damage( m, possessor, damage, 0, 100, 0, 0, 0 );

				if( possessor.PlayerDebug )
					possessor.SendMessage( "Debug: explosion damage: {0}", damage );
			}

			eable.Free();
		}
	}
}