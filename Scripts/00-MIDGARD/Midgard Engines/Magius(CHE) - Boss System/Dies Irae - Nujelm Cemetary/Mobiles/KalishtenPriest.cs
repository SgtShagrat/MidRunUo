/***************************************************************************
 *                               KalishtenPriest.cs
 *                            ------------------------
 *   begin                : 06 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Midgard.Engines.MonsterMasterySystem;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a priest corpse" )]
    public class KalishtenPriest : AncientLich
    {
        [Constructable]
        public KalishtenPriest()
        {
            Name = NameList.RandomName( "ancient lich" );
            Title = "the old Kalish'ten Priest";

            Mastery = MasteryLevel.Boss;
            Hue = Utility.RandomGreenHue();

            SetDamage( 17, 21 );
            HitsMaxSeed += 3000;
            Hits = HitsMax;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich, 2 );
            AddLoot( LootPack.UltraRich, 1 );
        }

        public override void AlterMeleeDamageTo( Mobile to, ref int damage )
        {
            if( to != null && !to.Player )
                damage *= 5;
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if( Utility.RandomDouble() < 0.3 )
                c.DropItem( new BloodyBandageKey() );
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool BardImmune { get { return true; } }
        public override bool BleedImmune { get { return true; } }
        public override int TreasureMapLevel { get { return 3; } }
        public override bool CanMoveOverObstacles { get { return true; } }

        public override bool CanAreaDamage { get { return true; } }
        public override TimeSpan AreaDamageDelay { get { return TimeSpan.FromSeconds( 20 ); } }
        public override double AreaDamageScalar { get { return 0.5; } }
        public override int AreaPhysicalDamage { get { return 100; } }

        #region serialization
        public KalishtenPriest( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}
