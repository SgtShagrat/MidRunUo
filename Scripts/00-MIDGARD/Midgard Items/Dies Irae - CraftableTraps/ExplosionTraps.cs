/***************************************************************************
 *                                  ExplosionTraps.cs
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

namespace Midgard.Items
{
    public abstract class BaseExplosionTrap : BaseCraftableTrap
    {
        public override int AttackMessage { get { return 1010543; } } // You are enveloped in an explosion of fire!
        public override int DisarmMessage { get { return 1010539; } } // You carefully remove the pressure trigger and disable the trap.
        public override int EffectSound { get { return 0x307; } }
        public override int MessageHue { get { return 0x78; } }

        public abstract int NumDices { get; }
        public abstract int DiceFaces { get; }
        public abstract int DiceBonus { get; }

        public override void DoVisibleEffect()
        {
            Effects.SendLocationEffect( GetWorldLocation(), Map, 0x36BD, 15, 10 );
        }

        public override void DoAttackEffect( Mobile m )
        {
            m.Damage( Utility.Dice( NumDices, DiceFaces, DiceBonus ), m );
        }

        public BaseExplosionTrap( TownSystem system, Mobile owner )
            : base( system, owner, 0x11C1 )
        {
        }

        public BaseExplosionTrap( Serial serial )
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

    public class LightExplosionTrap : BaseExplosionTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Light; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 1.0 ); } }
        public override int NumDices { get { return 2; } }
        public override int DiceFaces { get { return 6; } }
        public override int DiceBonus { get { return 20; } }

        public override Type DeedType { get { return typeof( LightExplosionTrapDeed ); } }

        public LightExplosionTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public LightExplosionTrap( Serial serial )
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

    public class MediumExplosionTrap : BaseExplosionTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Medium; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 2.0 ); } }
        public override int NumDices { get { return 4; } }
        public override int DiceFaces { get { return 8; } }
        public override int DiceBonus { get { return 30; } }

        public override Type DeedType { get { return typeof( MediumExplosionTrapDeed ); } }

        public MediumExplosionTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public MediumExplosionTrap( Serial serial )
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

    public class HeavyExplosionTrap : BaseExplosionTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Heavy; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 3.0 ); } }
        public override int NumDices { get { return 6; } }
        public override int DiceFaces { get { return 10; } }
        public override int DiceBonus { get { return 40; } }

        public override Type DeedType { get { return typeof( HeavyExplosionTrapDeed ); } }

        public HeavyExplosionTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public HeavyExplosionTrap( Serial serial )
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

    public class LightExplosionTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( LightExplosionTrap ); } }

        [Constructable]
        public LightExplosionTrapDeed()
        {
            Name = "Light Explosion Trap Deed";
        }

        public LightExplosionTrapDeed( Serial serial )
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

    public class MediumExplosionTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( MediumExplosionTrap ); } }

        [Constructable]
        public MediumExplosionTrapDeed()
        {
            Name = "Medium Explosion Trap Deed";
        }

        public MediumExplosionTrapDeed( Serial serial )
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

    public class HeavyExplosionTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( HeavyExplosionTrap ); } }

        [Constructable]
        public HeavyExplosionTrapDeed()
        {
            Name = "Heavy Explosion Trap Deed";
        }

        public HeavyExplosionTrapDeed( Serial serial )
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
