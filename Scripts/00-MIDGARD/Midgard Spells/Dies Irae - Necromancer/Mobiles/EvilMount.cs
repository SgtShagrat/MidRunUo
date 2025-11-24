/***************************************************************************
 *                               EvilMount.cs
 *
 *   begin                : 26 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Engines.Classes;

using Server;
using Server.Mobiles;

namespace Midgard.Engines.SpellSystem
{
    [CorpseName( "a cursed corpse" )]
    public class EvilMount : BaseMount
    {
        [Constructable]
        public EvilMount()
            : base( "an undead steed", 793, 0x3EBB, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            SetStr( 496, 525 );
            SetDex( 86, 105 );
            SetInt( 86, 125 );

            SetHits( 298, 315 );

            SetDamage( 16, 22 );

            SetDamageType( ResistanceType.Physical, 40 );
            SetDamageType( ResistanceType.Fire, 40 );
            SetDamageType( ResistanceType.Energy, 20 );

            SetResistance( ResistanceType.Physical, 55, 65 );
            SetResistance( ResistanceType.Fire, 20, 20 );
            SetResistance( ResistanceType.Cold, 30, 40 );
            SetResistance( ResistanceType.Poison, 30, 40 );
            SetResistance( ResistanceType.Energy, 20, 30 );

            SetSkill( SkillName.MagicResist, 25.1, 30.0 );
            SetSkill( SkillName.Tactics, 97.6, 100.0 );
            SetSkill( SkillName.Wrestling, 80.5, 92.5 );

            Fame = 14000;
            Karma = 14000;

            VirtualArmor = 60;

            Tamable = false;
            ControlSlots = 1;
        }

        public EvilMount( Serial serial )
            : base( serial )
        {
        }

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
            get { return true; }
        }

        public override bool CanBreath
        {
            get { return true; }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
        {
            if( RPGNecromancerSpell.IsSuperVulnerable( caster ) )
                scalar *= 0.01;

            base.AlterDamageScalarFrom( caster, ref scalar );
        }

        public override void AlterDamageScalarTo( Mobile target, ref double scalar )
        {
            if( RPGNecromancerSpell.IsSuperVulnerable( target ) )
                scalar *= 1.25;

            base.AlterDamageScalarTo( target, ref scalar );
        }

        public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
        {
            if( RPGNecromancerSpell.IsSuperVulnerable( from ) )
                damage = (int)( damage * 0.01 );

            base.AlterMeleeDamageFrom( from, ref damage );
        }

        public override void AlterMeleeDamageTo( Mobile to, ref int damage )
        {
            if( RPGNecromancerSpell.IsSuperVulnerable( to ) )
                damage = (int)( damage * 1.25 );

            base.AlterMeleeDamageTo( to, ref damage );
        }

        public override void AlterSpellDamageFrom( Mobile from, ref int damage )
        {
            if( RPGNecromancerSpell.IsSuperVulnerable( from ) )
                damage = (int)( damage * 0.01 );

            base.AlterSpellDamageFrom( from, ref damage );
        }

        public override void AlterSpellDamageTo( Mobile to, ref int damage )
        {
            if( RPGNecromancerSpell.IsSuperVulnerable( to ) )
                damage *= (int)( damage * 1.25 );

            base.AlterSpellDamageTo( to, ref damage );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !ClassSystem.IsNecromancer( from ) )
                from.SendMessage( "You may not ride this steed because you are not a Necromancer." );
            else
                base.OnDoubleClick( from );
        }

        public override bool OnBeforeDeath()
        {
            if( SummonMaster != null )
                EvilMountSpell.Unregister( SummonMaster );

            return base.OnBeforeDeath();
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