/***************************************************************************
 *							   OldTransformationSpell.cs
 *
 *   begin				: 26 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public abstract class OldTransformationSpell : RPGNecromancerSpell, ITransformationSpell
	{
		public OldTransformationSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public override bool BlockedByHorrificBeast{get { return false; }}

		#region ITransformationSpell Members
		public abstract int Body { get; }

		public virtual int Hue{get { return 0; }}
		public virtual int PhysResistOffset{get { return 0; }}
		public virtual int FireResistOffset{get { return 0; }}
		public virtual int ColdResistOffset{get { return 0; }}
		public virtual int PoisResistOffset{get { return 0; }}
		public virtual int NrgyResistOffset{get { return 0; }}

		public virtual double TickRate
		{
			get { return 1.0; }
		}

		public virtual void OnTick( Mobile m )
		{
		}

		public virtual void DoEffect( Mobile m )
		{
		}

		public virtual void RemoveEffect( Mobile m )
		{
		}
		#endregion

		public override bool CheckCast()
		{
			if( !TransformationSpellHelper.CheckCast( Caster, this ) )
				return false;

			return base.CheckCast();
		}

		public override void OnCast()
		{
			TransformationSpellHelper.OnCast( Caster, this );

			FinishSequence();
		}
	}
}