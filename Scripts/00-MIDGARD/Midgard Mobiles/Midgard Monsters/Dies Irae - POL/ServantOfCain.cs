using System;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "order of the iron fist corpse" )]
    public class ServantOfCain : BaseCreature
    {
        [Constructable]
        public ServantOfCain()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = NameList.RandomName( "male" );
            Body = 0x190;
            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();
            Title = "order of the iron fist";

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

            VirtualArmor = 65;

            Karma = Utility.RandomMinMax( -3000, -3500 );
            Fame = Utility.RandomMinMax( 1500, 1750 );

            Utility.AssignRandomHair( this );

            if( Utility.RandomBool() )
                Utility.AssignRandomFacialHair( this, HairHue );

            AddItem( new PlateChest() );
            AddItem( new PlateGloves() );
            AddItem( new PlateLegs() );
            AddItem( new PlateArms() );
            AddItem( new PlateGorget() );
            AddItem( new CloseHelm() );

            Halberd weapon = new Halberd();
            weapon.Movable = false;
            weapon.Crafter = this;
            weapon.Quality = WeaponQuality.Exceptional;
            AddItem( weapon );

            if( Backpack == null )
            {
                Container pack = new Backpack();
                pack.Movable = false;
                AddItem( pack );
            }

            PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
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

        public override bool AlwaysMurderer
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
            AddLoot( LootPack.Poor );
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

            if( m.AccessLevel > AccessLevel.Player || !m.Alive || !m.Player )
                return false;

            if( m.Karma < -5000 )
            {
                DebugSay( "My target is a bad karma guy." );
                return false;
            }

            if( m.Criminal )
            {
                DebugSay( "My target is criminal." );
                return false;
            }

            DebugSay( "My target has enough karma and it is not criminal." );

            Midgard2PlayerMobile m2Pm = m as Midgard2PlayerMobile;
            if( m2Pm == null )
                return false;

            if( m2Pm.Town == MidgardTowns.BuccaneersDen )
            {
                DebugSay( "My target is citizen of Bucca." );
                return false;
            }

            if( m2Pm.PermaRed )
            {
                DebugSay( "My target is a known murderer." );
                return false;
            }

            DebugSay( "My target is a valid enemy." );

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
                if( !( m is ServantOfCain ) )
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
        public ServantOfCain( Serial serial )
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