/***************************************************************************
 *							   ReaperForm.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Network;
using Server.Regions;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public class ReaperFormSpell : DruidForm
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Reaper Form", "Porm Helma",
			224,
			9011,
			Reagent.FertileDirt,
			Reagent.PetrifiedWood
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( ReaperFormSpell ),
				"Enhances the caster's spell damage, and resists while penalizing fire resist and movement speed.",
				"Trasforma il druido in una creatura vegetale, amplificando il suo danno magico ma rendendolo lento.",
				0x59e0
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override SpellCircle Circle{get { return SpellCircle.Eighth; }}

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler( OnLogin );
		}

		public static void OnLogin( LoginEventArgs e )
		{
			TransformContext context = TransformationSpellHelper.GetContext( e.Mobile );

			if( context != null && context.Type == typeof( ReaperFormSpell ) )
				e.Mobile.Send( SpeedControl.WalkSpeed );
		}

		public override int Body{get { return 0x11D; }}

		public override int FireResistOffset{get { return -25; }}

		public virtual int SpellDamageBonus
		{
			get { return (int)( Caster.Skills.AnimalLore.Value / 4.0 ) + GetPowerLevel() * 2; }
		}

		public ReaperFormSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void DoEffect( Mobile m )
		{
			m.PlaySound( 0x1BA );

			m.Send( SpeedControl.WalkSpeed );
		}

		public override void RemoveEffect( Mobile m )
		{
			if( !( m.Region is TwistedWealdDesert ) )
				m.Send( SpeedControl.Disable );
		}
	}
}