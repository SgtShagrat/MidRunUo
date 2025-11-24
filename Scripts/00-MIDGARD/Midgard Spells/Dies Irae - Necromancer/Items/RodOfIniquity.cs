/***************************************************************************
 *							   RodOfIniquity.cs
 *
 *   begin				: 26 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;

using Server;
using Server.Items;
using Server.Network;
using Server.Spells;

using Midgard.Items;

namespace Midgard.Engines.SpellSystem
{
	public class RodOfIniquity : MageStaff
	{
		private Timer m_Timer;
		//private Level m_Level;

		[Constructable]
		public RodOfIniquity( Mobile necromancer )
		{
			ItemID = 0x256D;
			Owner = necromancer;

			// Hue = 0x1;
			LootType = LootType.Blessed;
			Attributes.SpellChanneling = 1;

			if( Owner != null )
			{
				double spirit = Owner.Skills[ SkillName.SpiritSpeak ].Base;

				if( Core.AOS )
					WeaponAttributes.HitLeechHits = Math.Max( (int)( Owner.Skills[ SkillName.Necromancy ].Base ) - 20, 0 );

				Lifespan = (int)( Core.AOS ? spirit : ( 300 * RPGSpellsSystem.GetPowerLevel( Owner, typeof( RodOfIniquitySpell ) ) ) );
			}

			if( Lifespan > 0 )
				StartTimer();
		}

		public RodOfIniquity( Serial serial ) : base( serial )
		{
		}

		public override int BlockCircle{get { return -1; }}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner { get; private set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Lifespan { get; private set; }

		//[CommandProperty( AccessLevel.GameMaster )]
		//public int Level { get; set; }

		public override int InitMinHits{get { return 255; }}
		public override int InitMaxHits{get { return 255; }}
		public override string DefaultName{get { return "Rod of Iniquity"; }}
		public override bool DisplayLootType{get { return false; }}
		public override bool DisplayWeight{get { return false; }}
		public override SkillName DefSkill{get { return SkillName.Necromancy; }}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if( Owner != null )
				LabelTo( from, from.Language == "ITA" ? "Posseduta da {0}" : "Owned by {0}", Owner.Name ?? "Evil" );

			if( Lifespan > 0 )
				LabelTo( from, 1072517, Lifespan.ToString() );
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos,
											out int direct )
		{
			phys = fire = nrgy = chaos = direct = 0;
			cold = pois = 50;
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			if( RPGNecromancerSpell.IsSuperVulnerable( defender ) )
				damageBonus *= 1.5;

			double spiritSpeak = attacker.Skills[ SkillName.SpiritSpeak ].Base;

			if( Utility.RandomDouble() <= spiritSpeak / 120.0 )
			{
				attacker.PublicOverheadMessage( MessageType.Regular, 37, true, attacker.Language == "ITA"? "*il bastone prende vita*" : "* the iniquity slayed *" );

				switch( Utility.Random( 3 ) )
				{
					case 0:
						DoPoison( attacker, defender );
						break;
					case 1:
						DoNight( attacker, defender );
						break;
					case 2:
						DoAoSCurse( attacker, defender );
						break;
					default:
						break;
				}
			}

			base.OnHit( attacker, defender, damageBonus );
		}

		public override bool OnEquip( Mobile from )
		{
			return Validate( from ) && base.OnEquip( from );
		}

		public bool Validate( Mobile m )
		{
			if( m == null || !m.Player || Owner == null )
				return true;

			return m == Owner;
		}

		#region serial-deserial
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 );

			writer.WriteMobile( Owner );
			writer.Write( Lifespan );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			Owner = reader.ReadMobile();
			Lifespan = reader.ReadInt();

			StartTimer();
		}
		#endregion

		#region effects
		private static readonly Hashtable m_UnderEffect = new Hashtable();

		private static void DoPoison( Mobile attacker, Mobile defender )
		{
			if( defender == null || attacker == null )
				return;

			defender.ApplyPoison( attacker, Poison.Lesser );
		}

		private static void DoNight( Mobile attacker, Mobile defender )
		{
			if( defender == null || attacker == null )
				return;

			new LightCycle.NightSightTimer( defender ).Start();
			defender.LightLevel = LightCycle.DungeonLevel;
		}

		private static void DoAoSCurse( Mobile attacker, Mobile defender )
		{
			if( defender == null || attacker == null )
				return;

			double spiritSpeak = attacker.Skills[ SkillName.SpiritSpeak ].Base;

			SpellHelper.AddStatCurse( attacker, defender, StatType.Str );
			SpellHelper.DisableSkillCheck = true;
			SpellHelper.AddStatCurse( attacker, defender, StatType.Dex );
			SpellHelper.AddStatCurse( attacker, defender, StatType.Int );
			SpellHelper.DisableSkillCheck = false;

			Timer t = (Timer)m_UnderEffect[ defender ];

			if( attacker.Player && defender.Player && t == null )
			{
				TimeSpan duration = TimeSpan.FromSeconds( spiritSpeak / 10.0 );

				m_UnderEffect[ defender ] = t = Timer.DelayCall( duration, new TimerStateCallback( RemoveEffect ), defender );
				defender.UpdateResistances();
			}
		}

		public static void RemoveEffect( object state )
		{
			Mobile m = (Mobile)state;

			m_UnderEffect.Remove( m );

			m.UpdateResistances();
		}

		public static bool UnderEffect( Mobile m )
		{
			return m_UnderEffect.Contains( m );
		}
		#endregion

		#region decay
		public void StartTimer()
		{
			if( m_Timer != null )
				return;

			m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 10 ), TimeSpan.FromSeconds( 10 ), new TimerCallback( Slice ) );
			m_Timer.Priority = TimerPriority.OneSecond;
		}

		public void StopTimer()
		{
			if( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;
		}

		public void Slice()
		{
			Lifespan -= 10;

			InvalidateProperties();

			if( Lifespan <= 0 )
				Decay();
		}

		public void Decay()
		{
			if( RootParent is Mobile )
			{
				Mobile parent = (Mobile)RootParent;

				if( Name == null )
					parent.SendLocalizedMessage( 1072515, "#" + LabelNumber ); // The ~1_name~ expired...
				else
					parent.SendLocalizedMessage( 1072515, Name ); // The ~1_name~ expired...

				Effects.SendLocationParticles( EffectItem.Create( parent.Location, parent.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
				Effects.PlaySound( parent.Location, parent.Map, 0x201 );
			}
			else
			{
				Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
				Effects.PlaySound( Location, Map, 0x201 );
			}

			StopTimer();
			Delete();
		}
		#endregion
	}
}