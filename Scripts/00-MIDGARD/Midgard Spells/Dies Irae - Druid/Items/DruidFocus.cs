/***************************************************************************
 *							   DruidFocus.cs
 *
 *   begin				: 28 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

using Midgard.Gumps;

namespace Midgard.Engines.SpellSystem
{
	public class DruidFocus : TransientItem
	{
		private Timer m_Timer;

		public override string DefaultName
		{
			get { return "a druid focus"; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrengthBonus { get; set; }

		public DruidFocus( TimeSpan lifeSpan, int strengthBonus ) : base( 0x3155, lifeSpan )
		{
			Movable = false;
			LootType = LootType.Blessed;
			StrengthBonus = strengthBonus;
			m_Timer = new InternalTimer( this ); 
			m_Timer.Start(); 
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060485, StrengthBonus.ToString() ); // strength bonus ~1_val~
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			LabelTo( from, 1060485, StrengthBonus.ToString() ); // strength bonus ~1_val~
		}

		public override TextDefinition InvalidTransferMessage
		{
			get { return "Your druid focus disappears"; }
		}

		public override bool Nontransferable
		{
			get { return true; }
		}

		public override void OnDelete()
		{
			if( m_Timer != null )
				m_Timer.Stop();

			Mobile parent = RootParent as Mobile;

			if( parent != null )
			{
				if (parent.HasGump(typeof(DruidCircleGump)))
					parent.CloseGump(typeof(DruidCircleGump));

				parent.RemoveStatMod( "[StrDruidFocus]" );
				parent.RemoveStatMod( "[DexDruidFocus]" );
				parent.RemoveStatMod( "[IntDruidFocus]" );
			}

			base.OnDelete();
		}

		#region serialization
		public DruidFocus( Serial serial ) : base( serial )
		{
		}

		private class InternalTimer : Timer
		{
			private DruidFocus m_From;

			public InternalTimer( DruidFocus from ) : base( TimeSpan.FromSeconds( 10.0 ), TimeSpan.FromSeconds( 10.0 ) )
			{
				m_From = from;
			}

			protected override void OnTick()
			{
				Mobile parent = m_From.RootParent as Mobile;

				if( parent != null )
				{
					if (!parent.HasGump(typeof(DruidCircleGump)))
						parent.SendGump(new DruidCircleGump());

					IPooledEnumerable eable = m_From.Map.GetMobilesInRange( parent.Location, 20 );

					List<Mobile> weavers = new List<Mobile>();

					weavers.Add( parent );

					foreach ( Mobile m in eable )
					{
						if( m != parent && (m.Skills.Spellweaving.Value > 0) )//Edit by Arlas, was: && Math.Abs( Caster.Skills.Spellweaving.Value - m.Skills.Spellweaving.Value ) <= 20
						{
							weavers.Add( m );
						}
					}

					eable.Free();

					if (m_From.StrengthBonus != weavers.Count)
					{
						if (weavers.Count == 1 )
							parent.SendMessage( parent.Language == "ITA" ? "Ci sei solo tu nel circolo." : "There is only you in the circle.");
						else
							parent.SendMessage( parent.Language == "ITA" ? "Ci sono {0} druidi nel circolo." : "There are {0} druids in the circle.", weavers.Count.ToString() );
					}
					m_From.StrengthBonus = weavers.Count;
					//m_From.InvalidateProperties();

					//parent.SendMessage( "Your druid focus is renewed." );

					parent.AddStatMod( new StatMod( StatType.Str, "[StrDruidFocus]", m_From.StrengthBonus, DateTime.Now - m_From.CreationTime + m_From.LifeSpan ) );
					parent.AddStatMod( new StatMod( StatType.Dex, "[DexDruidFocus]", m_From.StrengthBonus, DateTime.Now - m_From.CreationTime + m_From.LifeSpan ) );
					parent.AddStatMod( new StatMod( StatType.Int, "[IntDruidFocus]", m_From.StrengthBonus, DateTime.Now - m_From.CreationTime + m_From.LifeSpan ) );
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );

			writer.Write( StrengthBonus );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			StrengthBonus = reader.ReadInt();

			Mobile parent = RootParent as Mobile;

			//if( parent != null && CreationTime + LifeSpan > DateTime.Now )
			//	parent.AddStatMod( new StatMod( StatType.Str, "[DruidFocus]", StrengthBonus, DateTime.Now - CreationTime + LifeSpan ) );

			m_Timer = new InternalTimer( this ); 
			m_Timer.Start(); 
		}
		#endregion
	}
}