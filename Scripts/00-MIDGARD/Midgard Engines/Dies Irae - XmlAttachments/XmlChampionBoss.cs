/***************************************************************************
 *                                  XmlChampionBoss.cs
 *                            		-------------------
 *  begin                	: Dicrembre, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Midgard.Misc;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlChampionBoss : XmlAttachment
    {
        [CommandProperty( AccessLevel.Seer, AccessLevel.Developer )]
        public bool CanGivePowerScrolls { get; set; }

        [CommandProperty( AccessLevel.Seer, AccessLevel.Developer )]
        public bool CanGiveChampionSkull { get; set; }

        [CommandProperty( AccessLevel.Seer, AccessLevel.Developer )]
        public bool CanGiveGoodies { get; set; }

        [CommandProperty( AccessLevel.Seer, AccessLevel.Developer )]
        public ChampionSkullType SkullType { get; set; }

        [CommandProperty( AccessLevel.Seer, AccessLevel.Developer )]
        public int NumScrolls { get; set; }

        [CommandProperty( AccessLevel.Seer, AccessLevel.Developer )]
        public int GoodiesRadius { get; set; }

        [Attachable]
        public XmlChampionBoss()
            : this( true, true, true, (ChampionSkullType)Utility.Random( 6 ), 6, 12 )
        {
        }

        [Attachable]
        public XmlChampionBoss( ChampionSkullType skullType, int numScrolls )
            : this( true, true, true, skullType, numScrolls, 12 )
        {
        }

        [Attachable]
        public XmlChampionBoss( bool canGivePowerScrolls, bool canGiveChampionSkull, bool canGiveGoodies, ChampionSkullType skullType, int numScrolls, int goodiesRadius )
        {
            CanGivePowerScrolls = canGivePowerScrolls;
            CanGiveChampionSkull = canGiveChampionSkull;
            CanGiveGoodies = canGiveGoodies;
            SkullType = skullType;
            NumScrolls = numScrolls;
            GoodiesRadius = goodiesRadius;
        }

        public override void OnAttach()
        {
            base.OnAttach();

            if( AttachedTo is Item )
            {
                Delete();
            }
        }

        public override bool HandlesOnDeath { get { return true; } }

        public override void OnBeforeDeath( BaseCreature creature )
        {
            if( creature == null || creature.Deleted )
                return;

            if( !creature.NoKillAwards )
            {
                if( CanGivePowerScrolls )
                    GivePowerScrolls( creature );

                Map map = creature.Map;

                if( CanGiveGoodies )
                {
                    if( map != null )
                    {
                        for( int x = -12; x <= GoodiesRadius; ++x )
                        {
                            for( int y = -12; y <= GoodiesRadius; ++y )
                            {
                                double dist = Math.Sqrt( x * x + y * y );

                                if( dist <= GoodiesRadius )
                                    new GoodiesTimer( map, creature.X + x, creature.Y + y ).Start();
                            }
                        }
                    }
                }
            }

            base.OnBeforeDeath( creature );
        }

        public override void OnDeath( BaseCreature creature, Container container )
        {
            if( !CanGiveChampionSkull )
                return;

            if( creature == null || creature.Deleted )
                return;

            List<DamageStore> rights = BaseCreature.GetLootingRights( creature.DamageEntries, creature.HitsMax );
            List<Mobile> toGive = new List<Mobile>();

            for( int i = rights.Count - 1; i >= 0; --i )
            {
                DamageStore ds = rights[ i ];

                if( ds.m_HasRight )
                    toGive.Add( ds.m_Mobile );
            }

            if( toGive.Count > 0 )
                toGive[ Utility.Random( toGive.Count ) ].AddToBackpack( new ChampionSkull( SkullType ) );

            base.OnDeath( creature, container );
        }

        #region private members for champion behaviour
        private static PowerScroll CreateRandomPowerScroll()
        {
            if( !Midgard2Persistance.PowerScrollsEnabled )
                return null;

            int level;
            double random = Utility.RandomDouble();

            if( 0.05 >= random )
                level = 20;
            else if( 0.4 >= random )
                level = 15;
            else
                level = 10;

            return PowerScroll.CreateRandomNoCraft( level, level );
        }

        public void GivePowerScrolls( BaseCreature creature )
        {
            if( !Midgard2Persistance.PowerScrollsEnabled )
                return;

            List<Mobile> toGive = new List<Mobile>();
            List<DamageStore> rights = BaseCreature.GetLootingRights( creature.DamageEntries, creature.HitsMax );

            for( int i = rights.Count - 1; i >= 0; --i )
            {
                DamageStore ds = rights[ i ];

                if( ds.m_HasRight )
                    toGive.Add( ds.m_Mobile );
            }

            if( toGive.Count == 0 )
                return;

            for( int i = 0; i < toGive.Count; i++ )
            {
                Mobile m = toGive[ i ];

                if( !( m is PlayerMobile ) )
                    continue;

                bool gainedPath = false;

                int pointsToGain = 800;

                if( VirtueHelper.Award( m, VirtueName.Valor, pointsToGain, ref gainedPath ) )
                {
                    if( gainedPath )
                        m.SendLocalizedMessage( 1054032 ); // You have gained a path in Valor!
                    else
                        m.SendLocalizedMessage( 1054030 ); // You have gained in Valor!

                    //No delay on Valor gains
                }
            }

            // Randomize
            for( int i = 0; i < toGive.Count; ++i )
            {
                int rand = Utility.Random( toGive.Count );
                Mobile hold = toGive[ i ];
                toGive[ i ] = toGive[ rand ];
                toGive[ rand ] = hold;
            }

            for( int i = 0; i < NumScrolls; ++i )
            {
                Mobile m = toGive[ i % toGive.Count ];

                PowerScroll ps = CreateRandomPowerScroll();

                GivePowerScrollTo( m, ps );
            }
        }

        public static void GivePowerScrollTo( Mobile m, PowerScroll ps )
        {
            if( ps == null || m == null )	//sanity
                return;

            m.SendLocalizedMessage( 1049524 ); // You have received a scroll of power!

            if( !Core.SE || m.Alive )
                m.AddToBackpack( ps );
            else
            {
                if( m.Corpse != null && !m.Corpse.Deleted )
                    m.Corpse.DropItem( ps );
                else
                    m.AddToBackpack( ps );
            }

            if( m is PlayerMobile )
            {
                PlayerMobile pm = (PlayerMobile)m;

                for( int j = 0; j < pm.JusticeProtectors.Count; ++j )
                {
                    Mobile prot = pm.JusticeProtectors[ j ];

                    if( prot.Map != m.Map || prot.Kills >= 5 || prot.Criminal || !JusticeVirtue.CheckMapRegion( m, prot ) )
                        continue;

                    int chance = 0;

                    switch( VirtueHelper.GetLevel( prot, VirtueName.Justice ) )
                    {
                        case VirtueLevel.Seeker:
                            chance = 60;
                            break;
                        case VirtueLevel.Follower:
                            chance = 80;
                            break;
                        case VirtueLevel.Knight:
                            chance = 100;
                            break;
                    }

                    if( chance > Utility.Random( 100 ) )
                    {
                        PowerScroll powerScroll = new PowerScroll( ps.Skill, ps.Value );

                        prot.SendLocalizedMessage( 1049368 ); // You have been rewarded for your dedication to Justice!

                        if( !Core.SE || prot.Alive )
                            prot.AddToBackpack( powerScroll );
                        else
                        {
                            if( prot.Corpse != null && !prot.Corpse.Deleted )
                                prot.Corpse.DropItem( powerScroll );
                            else
                                prot.AddToBackpack( powerScroll );
                        }
                    }
                }
            }
        }
        #endregion

        #region serial deserial
        public XmlChampionBoss( ASerial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( CanGivePowerScrolls );
            writer.Write( CanGiveChampionSkull );
            writer.Write( CanGiveGoodies );
            writer.Write( (int)SkullType );
            writer.Write( NumScrolls );
            writer.Write( GoodiesRadius );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            CanGivePowerScrolls = reader.ReadBool();
            CanGiveChampionSkull = reader.ReadBool();
            CanGiveGoodies = reader.ReadBool();
            SkullType = (ChampionSkullType)reader.ReadInt();
            NumScrolls = reader.ReadInt();
            GoodiesRadius = reader.ReadInt();
        }
        #endregion

        private class GoodiesTimer : Timer
        {
            private Map m_Map;
            private int m_X, m_Y;

            public GoodiesTimer( Map map, int x, int y )
                : base( TimeSpan.FromSeconds( Utility.RandomDouble() * 10.0 ) )
            {
                m_Map = map;
                m_X = x;
                m_Y = y;
            }

            protected override void OnTick()
            {
                int z = m_Map.GetAverageZ( m_X, m_Y );
                bool canFit = m_Map.CanFit( m_X, m_Y, z, 6, false, false );

                for( int i = -3; !canFit && i <= 3; ++i )
                {
                    canFit = m_Map.CanFit( m_X, m_Y, z + i, 6, false, false );

                    if( canFit )
                        z += i;
                }

                if( !canFit )
                    return;

                Gold g = new Gold( 500, 1000 );

                g.MoveToWorld( new Point3D( m_X, m_Y, z ), m_Map );

                if( 0.5 >= Utility.RandomDouble() )
                {
                    switch( Utility.Random( 3 ) )
                    {
                        case 0: // Fire column
                            {
                                Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x3709, 10, 30, 5052 );
                                Effects.PlaySound( g, g.Map, 0x208 );

                                break;
                            }
                        case 1: // Explosion
                            {
                                Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x36BD, 20, 10, 5044 );
                                Effects.PlaySound( g, g.Map, 0x307 );

                                break;
                            }
                        case 2: // Ball of fire
                            {
                                Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x36FE, 10, 10, 5052 );

                                break;
                            }
                    }
                }
            }
        }
    }
}
