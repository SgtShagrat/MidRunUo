/***************************************************************************
 *                                  SpikeTraps.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;

using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Spells;

namespace Midgard.Items
{
    public abstract class BaseSpikeTrap : BaseCraftableTrap
    {
        public override int AttackMessage { get { return 1010545; } } // Large spikes in the ground spring up piercing your skin!
        public override int DisarmMessage { get { return 1010541; } } // You carefully dismantle the trigger on the spikes and disable the trap.
        public override int EffectSound { get { return 0x22E; } }
        public override int MessageHue { get { return 0x5A; } }

        public abstract int Intensity { get; }

        public override void DoVisibleEffect()
        {
            Effects.SendLocationEffect( Location, Map, 0x11A4, 12, 6 );
        }

        public override void DoAttackEffect( Mobile m )
        {
			foreach ( Mobile mob in GetMobilesInRange( 0 ) )
			{
				if ( mob.Alive && !mob.IsDeadBondedPet )
					SpellHelper.Damage( TimeSpan.FromTicks( 1 ), mob, mob, Utility.Dice( Intensity, Intensity, 3 ) );
			}
        }

        public BaseSpikeTrap( TownSystem system, Mobile owner )
            : base( system, owner, 0x11A0 )
        {
        }

        public BaseSpikeTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class LightSpikeTrap : BaseSpikeTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Light; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 1.0 ); } }
        public override int Intensity { get { return 3; } }

        public override Type DeedType { get { return typeof( LightSpikeTrapDeed ); } }

        public LightSpikeTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public LightSpikeTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class MediumSpikeTrap : BaseSpikeTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Medium; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 2.0 ); } }
        public override int Intensity { get { return 5; } }

        public override Type DeedType { get { return typeof( MediumSpikeTrapDeed ); } }

        public MediumSpikeTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public MediumSpikeTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class HeavySpikeTrap : BaseSpikeTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Heavy; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 3.0 ); } }
        public override int Intensity { get { return 7; } }

        public override Type DeedType { get { return typeof( HeavySpikeTrapDeed ); } }

        public HeavySpikeTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public HeavySpikeTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class LightSpikeTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( LightSpikeTrap ); } }

        [Constructable]
        public LightSpikeTrapDeed()
        {
            Name = "Light Spike Trap Deed";
        }

        public LightSpikeTrapDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class MediumSpikeTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( MediumSpikeTrap ); } }

        [Constructable]
        public MediumSpikeTrapDeed()
        {
            Name = "Medium Spike Trap Deed";
        }

        public MediumSpikeTrapDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class HeavySpikeTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( HeavySpikeTrap ); } }

        [Constructable]
        public HeavySpikeTrapDeed()
        {
            Name = "Heavy Spike Trap Deed";
        }

        public HeavySpikeTrapDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}