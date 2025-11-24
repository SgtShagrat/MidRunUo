/***************************************************************************
 *							   SshamathGuard.cs
 *
 *   begin				: 03 luglio 2010
 *   author			   :	Dies Irae
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Items;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a drow corpse" )]
    public class SshamathGuard : BaseDrow
    {
        [Constructable]
        public SshamathGuard()
            : base( AIType.AI_Melee, FightMode.Closest, 18, 1, 0.175, 0.350 )
        {
            Title = ", the Sshamath guard";

            VirtualArmor = 50;

            SetStr( 800 );
            SetDex( 150 );
            SetInt( 100 );

            SetHits( 800 );
            SetMana( 100 );
            SetStam( 150 );

            SetDamage( "9d6" );

            SetSkill( SkillName.MagicResist, 100.0 );
            SetSkill( SkillName.Swords, 100.0 );
            SetSkill( SkillName.Tactics, 100.0 );
            SetSkill( SkillName.Wrestling, 15.0, 37.5 );

            Karma = Utility.RandomMinMax( -3000, -3500 );
            Fame = Utility.RandomMinMax( 1500, 1750 );

            SetSkill( SkillName.Swords, 110.0, 120.0 );
            SetSkill( SkillName.Macing, 110.0, 120.0 );
            SetSkill( SkillName.Wrestling, 110.0, 120.0 );
            SetSkill( SkillName.Tactics, 110.0, 120.0 );
            SetSkill( SkillName.MagicResist, 110.0, 120.0 );
            SetSkill( SkillName.Healing, 110.0, 120.0 );
            SetSkill( SkillName.Anatomy, 110.0, 120.0 );
            SetSkill( SkillName.DetectHidden, 110.0, 120.0 );

            AddItem( Immovable( new HoodedCloak( 0x09CE ) ) );
            AddItem( Immovable( new Hood( 0x09CE ) ) );
            AddItem( Immovable( new Shirt( 0x0497 ) ) );
            AddItem( Immovable( new LongPants( 0x0497 ) ) );
            AddItem( Immovable( new Belt( 0x09CE ) ) );
            AddItem( Immovable( new Muzzle( 0x09CE ) ) );
            AddItem( Immovable( new ThighBoots() ) );

            AddItem( Immovable( Rehued( new StuddedArms(), 0x04DF ) ) );
            AddItem( Immovable( Rehued( new StuddedGloves(), 0x04DF ) ) );

            DrowPike weapon = new DrowPike();
            weapon.Movable = false;
            weapon.Crafter = this;
            weapon.Quality = WeaponQuality.Exceptional;
            weapon.CustomQuality = Quality.Exceptional;
            AddItem( weapon );

            if( Backpack == null )
            {
                Container pack = new Backpack();
                pack.Movable = false;
                AddItem( pack );
            }

            PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
        }

        private static Item Immovable( Item item )
        {
            item.Movable = false;
            return item;
        }

        private static Item Rehued( Item item, int hue )
        {
            item.Hue = hue;
            return item;
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override bool GuardImmune
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override bool CanRummageCorpses
        {
            get { return true; }
        }

        public override bool AutoDispel
        {
            get { return true; }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Dismount;
        }

        public override bool IsEnemy( Mobile m )
        {
            if( m == null )
                return false;

            // from midgard POL
            // (k>-5000 or !critter.criminal))
            bool drowInParty = false;

            Server.Engines.PartySystem.Party p = Server.Engines.PartySystem.Party.Get( m );

            if( p != null )
            {
                foreach( Server.Engines.PartySystem.PartyMemberInfo info in p.Members )
                {
                    if( info == null )
                        continue;
                    Mobile owner = info.Mobile;
                    if( owner.Race == Engines.Races.Core.Drow )
                        drowInParty = true;
                }
            }

            if( m.AccessLevel > AccessLevel.Player || !m.Alive || !m.Player || m.Race == Engines.Races.Core.Drow || drowInParty )
                return false;

            DebugSay( "My enemy is not a drow." );

            Midgard2PlayerMobile m2Pm = m as Midgard2PlayerMobile;
            if( m2Pm == null )
                return false;

            if( m2Pm.Town == MidgardTowns.Sshamath )
                return false;

            DebugSay( "My enemy is not citizen of Sshamath." );

            return true;
        }

        public override void AlterMeleeDamageTo( Mobile to, ref int damage )
        {
            if( to is BaseCreature )
                damage *= 3;
        }

        public override void OnThink()
        {
            if( CanCallToArms() )
                CallToArms();

            base.OnThink();
        }

        private DateTime m_NextCallToArms;

        private bool CanCallToArms()
        {
            return DateTime.Now >= m_NextCallToArms && Combatant != null;
        }

        private void CallToArms()
        {
            foreach( Mobile m in GetMobilesInRange( 30 ) )
            {
                if( !( m is SshamathGuard ) )
                    continue;

                if( m.Combatant == null && Combatant != null )
                {
                    Say( Utility.RandomMinMax( 1075102, 1075115 ) ); // Muahahahaha!  I'll feast on your flesh.
                    m.Attack( Combatant );
                }
            }

            m_NextCallToArms = DateTime.Now + TimeSpan.FromSeconds( 5 + Utility.RandomDouble() * 5 );
        }

        #region serialization
        public SshamathGuard( Serial serial )
            : base( serial )
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