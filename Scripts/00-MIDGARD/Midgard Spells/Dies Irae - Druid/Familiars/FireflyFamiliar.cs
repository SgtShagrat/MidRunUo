/***************************************************************************
 *							   FireflyFamiliar.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.SpellSystem
{
	[CorpseName( "a firefly corpse" )]
	public class FireflyFamiliar : BaseFamiliar
	{
		public override double DispelDifficulty
		{
			get { return 75.0; }
		}

		public FireflyFamiliar()
		{
			Name = "a firefly";
			Body = 58;
			BaseSoundID = 466;
			Hue = 1161;

			SetStr( 5 );
			SetDex( 6 );
			SetInt( 10 );

			SetHits( 100 );
			SetStam( 50 );
			SetMana( 0 );

			SetDamage( 1, 2 );

			SetDamageType( ResistanceType.Fire, 100 );

			SetResistance( ResistanceType.Physical, 10, 15 );
			SetResistance( ResistanceType.Fire, 75, 99 );
			SetResistance( ResistanceType.Cold, 10, 15 );
			SetResistance( ResistanceType.Poison, 10, 15 );
			SetResistance( ResistanceType.Energy, 0, 3 );

			SetSkill( SkillName.Wrestling, 10, 11 );
			SetSkill( SkillName.Tactics, 10 );

			AddItem( new LightSource() );

			ControlSlots = 1;
		}

		public override bool IsDispellable { get { return false; } }
		private DateTime m_NextRestore;

		public override void OnThink()
		{
			base.OnThink();

			if( DateTime.Now < m_NextRestore )
				return;

			m_NextRestore = DateTime.Now + TimeSpan.FromSeconds( 2.0 );

			Mobile caster = ControlMaster;

			if( caster == null )
				caster = SummonMaster;

			if( caster != null )
			{
				caster.FixedEffect( 0x37C4, 1, 12, 1109, 3 ); // At player
				++caster.Stam;
				if ( DruidEmpowermentSpell.HasBonus( caster ) )
					++caster.Mana;
			}
		}

		#region serialization
		public FireflyFamiliar( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		#endregion
	}
}