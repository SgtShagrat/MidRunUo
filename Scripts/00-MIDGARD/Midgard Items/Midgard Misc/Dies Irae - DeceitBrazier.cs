using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class DeceitBrazier : Brazier
    {
        private Type[] m_SpawnTypes = new Type[]
			{
				typeof( Bogle ), typeof( BoneKnight ), typeof( EarthElemental ),
				typeof( Ettin ), typeof( GazerLarva ), typeof( Ghoul ),
				typeof( Golem ), typeof( HeadlessOne ), typeof( Mummy ),
				typeof( Ogre ), typeof( OgreLord ), typeof( RottingCorpse ),
				typeof( Sewerrat ), typeof( Skeleton ), typeof( Slime ),
				typeof( Zombie ), typeof( AirElemental ), typeof( DreadSpider ),
				typeof( Efreet ), typeof( Lich ), typeof( Nightmare ),
				typeof( PoisonElemental ), typeof( GiantBlackWidow ), typeof( SilverSerpent ),
				typeof( ToxicElemental ), typeof( Alligator ), typeof( BloodElemental ),
				typeof( BoneMagi ), typeof( Cyclops ), typeof( Daemon ),
				typeof( DireWolf ), typeof( Dragon ), typeof( Drake ),
				typeof( ElderGazer ), typeof( FireElemental ), typeof( FireGargoyle ),
				typeof( Gargoyle ), typeof( Gazer ),
				typeof( GiantRat ), typeof( GiantSerpent ), typeof( GiantSpider ),
				typeof( Harpy ), typeof( HellHound ), typeof( Imp ),
				typeof( PredatorHellCat ), typeof( LavaLizard ), typeof( LavaSerpent ),
				typeof( LavaSnake ), typeof( Lizardman ), typeof( Mongbat ),
				typeof( StrongMongbat ), typeof( Orc ), typeof( OrcBomber ),
				typeof( OrcBrute ), typeof( OrcCaptain ), typeof( OrcishLord ),
				typeof( OrcishMage ), typeof( Ratman ), typeof( RatmanArcher ),
				typeof( RatmanMage ), typeof( Scorpion ), typeof( Shade ),
				typeof( SkeletalMage ), typeof( HellCat ), typeof( Snake ),
				typeof( Spectre ), typeof( StoneGargoyle ), typeof( StoneHarpy ),
				typeof( Titan ), typeof( Troll ), typeof( Wraith ), typeof( Wyvern ),
				typeof( LichLord ), typeof( SkeletalKnight ), typeof( SwampTentacle ),
                typeof( Daemon ), typeof( GargoyleDestroyer ), typeof( GargoyleEnforcer ),
                typeof( StoneGargoyle ), typeof( Brigand ), typeof( PoisonElemental ),
                typeof( IceElemental ), typeof( SnowElemental ), typeof( IceFiend ),
                typeof( RuneBeetle ), typeof( Cyclops ), typeof( SkeletalDragon ),
                typeof( ShadowWyrm ), typeof( AncientWyrm ), typeof( Balron ), typeof( WhiteWyrm ),
                typeof( AntLion ), typeof( Corpser ), typeof( CrystalElemental )
			};

        private DateTime m_NextSummon;

        [Constructable]
        public DeceitBrazier()
        {
        }

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            if( DateTime.Now > m_NextSummon && m.Player && !( m.AccessLevel > AccessLevel.Player && m.Hidden ) && m.InRange( this, 2 ) && !PointsInRange( oldLocation.X, oldLocation.Y, X, Y, 2 ) )
                PublicOverheadMessage( MessageType.Regular, 0x3B2, 500761 ); // Heed this warning well, and use this brazier at your own peril.

            base.OnMovement( m, oldLocation );
        }

        public static bool PointsInRange( int px, int py, int qx, int qy, int distance )
        {
            return !( Math.Abs( px - qx ) > distance || Math.Abs( py - qy ) > distance );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( DateTime.Now > m_NextSummon && from.InRange( this, 2 ) )
                BrazierSummon();
            else
                PublicOverheadMessage( MessageType.Regular, 0x3B2, 500760 ); // The brazier fizzes and pops, but nothing seems to happen.
        }

        public void BrazierSummon()
        {
            Mobile summon = Activator.CreateInstance( m_SpawnTypes[ Utility.Random( m_SpawnTypes.Length ) ] ) as Mobile;

            if( summon is BaseCreature )
                ( (BaseCreature)summon ).Tamable = false;

            Point3D point = new Point3D( X - 4, Y, Z );
            summon.MoveToWorld( point, Map );
            summon.Paralyze( TimeSpan.FromSeconds( 2 ) );

            GraphicalEffects.SendGraphicalEffect( point, Map, EffectType.FixedXYZ, 0x3709, 2, 56, true, false );

            m_NextSummon = DateTime.Now + TimeSpan.FromMinutes( 15 );
        }

        public DeceitBrazier( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            writer.Write( (DateTime)m_NextSummon );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            m_NextSummon = reader.ReadDateTime();
        }
    }

    public static class GraphicalEffects
    {
        public static void SendGraphicalEffect( Point3D origin, Map map, EffectType type, int graphic, int speed, int duration, bool fixedDirection, bool explode )
        {
            if( map != null )
            {
                IPooledEnumerable eable = map.GetClientsInRange( origin );

                Packet p = new GraphicalEffect( type, Serial.Zero, Serial.Zero, graphic, origin, origin, speed, duration, fixedDirection, explode );
                p.Acquire();

                foreach( NetState state in eable )
                {
                    state.Mobile.ProcessDelta();

                    state.Send( p );
                }

                p.Release();

                eable.Free();
            }
        }
    }

    public sealed class GraphicalEffect : Packet
    {
        public GraphicalEffect( EffectType type, Serial from, Serial to, int graphic, Point3D fromPoint, Point3D toPoint, int speed, int duration, bool fixedDirection, bool explode )
            : base( 0x70, 28 )
        {
            m_Stream.Write( (byte)type );
            m_Stream.Write( (int)from );
            m_Stream.Write( (int)to );
            m_Stream.Write( (short)graphic );
            m_Stream.Write( (short)fromPoint.X );
            m_Stream.Write( (short)fromPoint.Y );
            m_Stream.Write( (sbyte)fromPoint.Z );
            m_Stream.Write( (short)toPoint.X );
            m_Stream.Write( (short)toPoint.Y );
            m_Stream.Write( (sbyte)toPoint.Z );
            m_Stream.Write( (byte)speed );
            m_Stream.Write( (byte)duration );

            m_Stream.Write( (byte)0 );
            m_Stream.Write( (byte)0 );

            m_Stream.Write( (bool)fixedDirection );
            m_Stream.Write( (bool)explode );
        }
    }
}