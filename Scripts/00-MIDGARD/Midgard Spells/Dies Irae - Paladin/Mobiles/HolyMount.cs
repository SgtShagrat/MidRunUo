/***************************************************************************
 *							   HolyMount.cs
 *
 *   begin				: 05 maggio 2011
 *   author			   :	Dies Irae
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Classes;
using Server;
using Server.Mobiles;
using Server.Network;
using Core = Midgard.Engines.Races.Core;

namespace Midgard.Engines.SpellSystem
{
    [CorpseName( "a holy corpse" )]
    public class HolyMount : BaseMount
    {
        public int PowerLevel { get; set; }

        public override bool IsDispellable
        {
            get { return false; }
        }

        public override bool IsBondable
        {
            get { return false; }
        }

        public override bool HasBreath
        {
            get { return PowerLevel > 3; }
        }

        public override bool CanBreath
        {
            get { return PowerLevel > 3; }
        }

        public override void OnRiderChanged( Mobile oldValue, Mobile newValue )
        {
            DebugSay( "My rider changed. old was {0}, new is {1}", 
                oldValue == null ? "none" : oldValue.Name, 
                newValue == null ? "none" : newValue.Name );

            if( oldValue != null && newValue == null )
                Delete();
        }

        [Constructable]
        public HolyMount()
            : base( "a silver steed", 0x75, 0x3EA8, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            SetStr( 22, 98 );
            SetDex( 56, 75 );
            SetInt( 86, 125 );

            SetHits( 28, 45 );

            SetDamage( 3, 4 );

            SetDamageType( ResistanceType.Physical, 40 );
            SetDamageType( ResistanceType.Fire, 40 );
            SetDamageType( ResistanceType.Energy, 20 );

            SetResistance( ResistanceType.Physical, 15, 20 );

            SetSkill( SkillName.Magery, 75.1, 80.0 );
            SetSkill( SkillName.MagicResist, 25.1, 30.0 );
            SetSkill( SkillName.Tactics, 29.3, 44.0 );
            SetSkill( SkillName.Wrestling, 29.3, 44.0 );

            Fame = 14000;
            Karma = 14000;

            VirtualArmor = 30 + PowerLevel * 2;

            Tamable = false;
            ControlSlots = 1;
        }

        public HolyMount( Mobile caster, int powerLevel )
            : this()
        {
            PowerLevel = powerLevel;

            switch( powerLevel )
            {
                case 1:
                case 2:
                    Body = 0xE2;
                    ItemID = 0x3EA0;
                    Hue = 0xA51;
                    break;
                case 3:
                case 4:
                    Body = 0xBE;
                    ItemID = 0x3E9E;
                    Hue = 0xA51;
                    break;
                case 5:
                    Body = 0x11C;
                    ItemID = 0x3E92;
                    Hue = 0xA51;
                    break;
            }

            if( Core.IsElfRace( caster.Race ) )
                Hue = 0x875; // elven horse hue
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override TimeSpan MountAbilityDelay
        {
            get { return TimeSpan.FromMinutes( 30.0 - PowerLevel * 5 ); }
        }

        public override bool DoMountAbility( int damage, Mobile attacker )
        {
            if( Rider == null || attacker == null ) //sanity
                return false;

            if( Rider.Poisoned && ( ( Rider.Hits - damage ) < 40 ) )
            {
                Poison p = Rider.Poison;

                if( p != null )
                {
                    int chanceToCure = 10000 + (int)( Skills[ SkillName.Magery ].Value * 75 ) - ( ( p.Level + 1 ) * 1750 );
                    chanceToCure /= 100;

                    if( chanceToCure > Utility.Random( 100 ) )
                    {
                        if( Rider.CurePoison( this ) )
                        {
                            Rider.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "Your holy mount senses you are in danger and aids you!" );
                            Rider.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
                            Rider.PlaySound( 0x1E0 ); // Cure spell effect.
                            Rider.PlaySound( 0xA9 ); // Unicorn's whinny.

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
        {
            if( RPGPaladinSpell.IsSuperVulnerable( caster ) )
                scalar *= 1 - ( PowerLevel * 0.1 );

            base.AlterDamageScalarFrom( caster, ref scalar );
        }

        public override void AlterDamageScalarTo( Mobile target, ref double scalar )
        {
            if( RPGPaladinSpell.IsSuperVulnerable( target ) )
                scalar *= 1 + PowerLevel * 0.05;

            base.AlterDamageScalarTo( target, ref scalar );
        }

        public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
        {
            if( RPGPaladinSpell.IsSuperVulnerable( from ) )
                damage = (int)( damage * ( 1 - ( PowerLevel * 0.1 ) ) );

            base.AlterMeleeDamageFrom( from, ref damage );
        }

        public override void AlterMeleeDamageTo( Mobile to, ref int damage )
        {
            if( RPGPaladinSpell.IsSuperVulnerable( to ) )
                damage = (int)( damage * ( 1 + PowerLevel * 0.05 ) );

            base.AlterMeleeDamageTo( to, ref damage );
        }

        public override void AlterSpellDamageFrom( Mobile from, ref int damage )
        {
            if( RPGPaladinSpell.IsSuperVulnerable( from ) )
                damage = (int)( damage * ( 1 - ( PowerLevel * 0.1 ) ) );

            base.AlterSpellDamageFrom( from, ref damage );
        }

        public override void AlterSpellDamageTo( Mobile to, ref int damage )
        {
            if( RPGPaladinSpell.IsSuperVulnerable( to ) )
                damage *= (int)( damage * ( 1 + PowerLevel * 0.05 ) );

            base.AlterSpellDamageTo( to, ref damage );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !ClassSystem.IsPaladine( from ) )
                from.SendMessage( "You may not ride this steed because you are not a Paladin." );
            else
                base.OnDoubleClick( from );
        }

        public override void Delete()
        {
            HolyMountSpell.Unregister( SummonMaster, SummonMaster != null );

            base.Delete();
        }

        #region serialization
        public HolyMount( Serial serial )
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
        #endregion

        public bool DoOrderRelease()
        {
            Delete();
            return true;
        }
    }
}