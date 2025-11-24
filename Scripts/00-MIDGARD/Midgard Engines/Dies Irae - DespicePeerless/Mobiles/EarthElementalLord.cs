/***************************************************************************
 *                                  .cs
 *                            		-------------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a lizarmen lord corpse" )]
    public class EarthElementalLord : EarthElemental
    {
        [Constructable]
        public EarthElementalLord()
        {
            Name = "Rockslide";

            Paragon.Convert( this );
            Hue = Utility.RandomNeutralHue();

            SetDamage( 17, 21 );
            HitsMaxSeed += 3000;
            Hits = HitsMax;

            AI = AIType.AI_BossMelee;
        }

        public override bool CanMoveOverObstacles
        {
            get { return true; }
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override bool BleedImmune
        {
            get { return true; }
        }

        public override int TreasureMapLevel
        {
            get { return 2; }
        }

        public override bool CanAreaDamage
        {
            get { return true; }
        }

        public override TimeSpan AreaDamageDelay
        {
            get { return TimeSpan.FromSeconds( 20 ); }
        }

        public override double AreaDamageScalar
        {
            get { return 0.5; }
        }

        public override int AreaPhysicalDamage
        {
            get { return 100; }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.UltraRich, 1 );
        }

        public override string ApplyNameSuffix( string suffix )
        {
            if( !Core.AOS )
            {
                if( suffix.Length == 0 )
                    suffix = "[lord of elements]";
                else
                    suffix = String.Concat( suffix, " [lord of elements]" );
            }

            return base.ApplyNameSuffix( suffix );
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "lord of elements" );
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
                c.DropItem( new IdolOfTheRats() );
        }

        public EarthElementalLord( Serial serial )
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
    }
}