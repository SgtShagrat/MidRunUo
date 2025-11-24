using System;
using System.Collections.Generic;
using Midgard.Gumps;
using Server;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Mobiles
{
	[CorpseName( "a sphynx corpse" )]
	public class Sphynx : BaseCreature
	{
        public override bool CanSpeakMantra
        {
            get { return true; }
        }

		[Constructable]
		public Sphynx() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Body = 788;
			Name = "a sphynx";

			SetStr( 1001, 1200 );
			SetDex( 176, 195 );
			SetInt( 301, 400 );
			SetHits( 1001, 1200 );
			SetDamage( 10, 16 );

			SetDamageType( ResistanceType.Physical, 85 );
			SetDamageType( ResistanceType.Energy, 15 );

			SetResistance( ResistanceType.Physical, 60, 80 );
			SetResistance( ResistanceType.Fire, 30, 50 );
			SetResistance( ResistanceType.Cold, 40, 60 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Wrestling, 90.1, 100 );
			SetSkill( SkillName.Tactics, 90.1, 100 );
			SetSkill( SkillName.MagicResist, 100.5, 150 );
			SetSkill( SkillName.Anatomy, 25.1, 50 );
			SetSkill( SkillName.Magery, 95.5, 100 );
			SetSkill( SkillName.EvalInt, 90.1, 100 );
			SetSkill( SkillName.Meditation, 95.1, 120 );

			VirtualArmor = 78;
			Fame = 15000;
			Karma = 0;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			bool enabled = ( from != null && from is Midgard2PlayerMobile && ((Midgard2PlayerMobile)from).SphynxBonusExpiration == TimeSpan.Zero );

			if ( from is Midgard2PlayerMobile )
				list.Add( new FortuneEntry( this, enabled ) );
		}

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );

			if ( aggressor is Midgard2PlayerMobile )
			{
				((Midgard2PlayerMobile)aggressor).SphynxBonusExpiration = TimeSpan.Zero;
				aggressor.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
				aggressor.PlaySound( 0x307 );
				aggressor.SendMessage( "Sphynx look at you an removes its oath!" );
			}
		}

		public class FortuneEntry : ContextMenuEntry
		{
			private Sphynx m_Sphynx;
			private bool m_Enabled;

			public FortuneEntry( Sphynx sphynx, bool enabled ) : base( 6199, 3 )
			{
				m_Sphynx = sphynx;
				m_Enabled = enabled;

				if ( !m_Enabled )
					Flags |= CMEFlags.Disabled;
			}

			public override void OnClick()
			{
				if( m_Sphynx != null && !m_Sphynx.Deleted && m_Enabled )
				{
					Owner.From.CloseGump( typeof( FortuneGump ) );
					Owner.From.SendGump( new FortuneGump( m_Sphynx ) );
				}
			}
		}

		public Sphynx( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
