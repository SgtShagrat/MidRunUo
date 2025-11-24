/***************************************************************************
 *                                  SawTraps.cs
 *                            		-----------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseSawTrap : BaseCraftableTrap
    {
        public override int AttackMessage { get { return 1010544; } } // The blade cuts deep into your skin!
        public override int DisarmMessage { get { return 1010540; } } // You carefully dismantle the saw mechanism and disable the trap.
        public override int EffectSound { get { return 0x218; } }
        public override int MessageHue { get { return 0x5A; } }

        public abstract int NumDices { get; }
        public abstract int DiceFaces { get; }
        public abstract int DiceBonus { get; }

        public override void DoVisibleEffect()
        {
            Effects.SendLocationEffect( Location, Map, 0x11AD, 25, 10 );
        }

        public override void DoAttackEffect( Mobile m )
        {
            m.Damage( Utility.Dice( NumDices, DiceFaces, DiceBonus ) );
            BleedAttack.BeginBleed( m, null );
        }

        public BaseSawTrap( TownSystem system, Mobile owner )
            : base( system, owner, 0x11AC )
        {
        }

        public BaseSawTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class LightSawTrap : BaseSawTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Light; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 1.0 ); } }
        public override int NumDices { get { return 1; } }
        public override int DiceFaces { get { return 10; } }
        public override int DiceBonus { get { return 0; } }

        public override Type DeedType { get { return typeof( LightSawTrapDeed ); } }

        public LightSawTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public LightSawTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class MediumSawTrap : BaseSawTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Medium; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 2.0 ); } }
        public override int NumDices { get { return 1; } }
        public override int DiceFaces { get { return 15; } }
        public override int DiceBonus { get { return 0; } }

        public override Type DeedType { get { return typeof( MediumSawTrapDeed ); } }

        public MediumSawTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public MediumSawTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class HeavySawTrap : BaseSawTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Heavy; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 3.0 ); } }
        public override int NumDices { get { return 1; } }
        public override int DiceFaces { get { return 20; } }
        public override int DiceBonus { get { return 0; } }

        public override Type DeedType { get { return typeof( HeavySawTrapDeed ); } }

        public HeavySawTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public HeavySawTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class LightSawTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( LightSawTrap ); } }

        [Constructable]
        public LightSawTrapDeed()
        {
            Name = "Light Saw Trap Deed";
        }

        public LightSawTrapDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class MediumSawTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( MediumSawTrap ); } }

        [Constructable]
        public MediumSawTrapDeed()
        {
            Name = "Medium Saw Trap Deed";
        }

        public MediumSawTrapDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class HeavySawTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( HeavySawTrap ); } }

        [Constructable]
        public HeavySawTrapDeed()
        {
            Name = "Heavy Saw Trap Deed";
        }

        public HeavySawTrapDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}
