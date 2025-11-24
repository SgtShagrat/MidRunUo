/***************************************************************************
 *                               OgresLord.cs
 *                            -------------------
 *   begin                : 30 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "an ogre lord corpse" )]
    public class OgresLord : Ogre
    {
        [Constructable]
        public OgresLord()
        {
            Name = "Skullcrusher";

            Paragon.Convert( this );
            Hue = Utility.RandomGreenHue();

            SetDamage( 17, 21 );
            HitsMaxSeed += 3000;
            Hits = HitsMax;

            AI = AIType.AI_BossMelee;
        }

        public OgresLord( Serial serial )
            : base( serial )
        {
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
            get { return 50; }
        }

        public override int AreaFireDamage
        {
            get { return 50; }
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
                    suffix = "[lord of ogres]";
                else
                    suffix = String.Concat( suffix, " [lord of ogres]" );
            }

            return base.ApplyNameSuffix( suffix );
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "lord of ogres" );
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
                c.DropItem( new RatsRattle() );
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