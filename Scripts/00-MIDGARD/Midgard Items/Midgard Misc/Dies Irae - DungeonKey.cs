/***************************************************************************
 *                               DungeonKey.cs
 *                            ----------------------
 *   begin                : 31 luglio, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;
using Server.Regions;
using Server.Targeting;

namespace Midgard.Items
{
    public class DungeonKey : Key
    {
        private Timer m_Timer;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Lifespan { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DungeonRegion Region { get; private set; }

        [Constructable]
        public DungeonKey( DungeonRegion region )
            : this( region, 60 * 60 )
        {
        }

        [Constructable]
        public DungeonKey( DungeonRegion region, int lifespan )
        {
            Region = region;

            Hue = Utility.RandomMetalHue();
            Weight = 1.0;

            Lifespan = lifespan;

            if( Lifespan > 0 )
                StartTimer();
        }

        public override string DefaultName
        {
            get { return "a dungeon key"; }
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Region != null && !string.IsNullOrEmpty( Region.Name ) )
                LabelTo( from, string.Format( "a dungeon key for {0}", Region.Name.ToLower() ) );
            else
                base.OnSingleClick( from );

            LabelTo( from, 1072517, Lifespan.ToString() ); // Lifespan: ~1_val~ seconds
        }

        #region decay
        public void StartTimer()
        {
            if( m_Timer != null )
                return;

            m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 10 ), TimeSpan.FromSeconds( 10 ), new TimerCallback( Slice ) );
            m_Timer.Priority = TimerPriority.OneSecond;
        }

        public void StopTimer()
        {
            if( m_Timer != null )
                m_Timer.Stop();

            m_Timer = null;
        }

        public void Slice()
        {
            Lifespan -= 10;

            InvalidateProperties();

            if( Lifespan <= 0 )
                Decay();
        }

        public void Decay()
        {
            if( RootParent is Mobile )
            {
                Mobile parent = (Mobile)RootParent;

                if( Name == null )
                    parent.SendLocalizedMessage( 1072515, "#" + LabelNumber ); // The ~1_name~ expired...
                else
                    parent.SendLocalizedMessage( 1072515, Name ); // The ~1_name~ expired...

                Effects.SendLocationParticles( EffectItem.Create( parent.Location, parent.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
                Effects.PlaySound( parent.Location, parent.Map, 0x201 );
            }
            else
            {
                Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
                Effects.PlaySound( Location, Map, 0x201 );
            }

            StopTimer();
            Delete();
        }
        #endregion

        public override void OnDoubleClick( Mobile from )
        {
            if( !Validate( from ) )
            {
                from.SendMessage( "You cannot use this key here." );
                return;
            }

            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 501661 ); // That key is unreachable.
                return;
            }

            from.SendLocalizedMessage( 501662 ); // What shall I use this key on?
            from.Target = new UnlockTarget( this );
        }

        public bool Validate( Mobile m )
        {
            if( m == null || !m.Player )
                return true;

            DungeonRegion r = Server.Region.Find( m.Location, m.Map ) as DungeonRegion;

            return r != null && r == Region;
        }

        public bool UseOn( Mobile from, MidgardTreasureChest chest )
        {
            if( Validate( from ) )
            {
                if( chest.Locked )
                {
                    chest.Locked = false;

                    chest.SendLocalizedMessageTo( from, 1048001 ); // You unlock it.
                    chest.PublicOverheadMessage( 0, 0, true, "* Click *" );
                    return true;
                }
                else
                    from.SendMessage( "That is already unlocked." );
            }
            else
                from.SendMessage( "You can use it only in the key dungeon!" );

            return false;
        }

        #region serial-deserial
        public DungeonKey( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( Lifespan );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Lifespan = reader.ReadInt();

            StartTimer();
        }
        #endregion

        private class UnlockTarget : Target
        {
            private readonly DungeonKey m_Key;

            public UnlockTarget( DungeonKey key )
                : base( key.MaxRange, false, TargetFlags.None )
            {
                m_Key = key;
                CheckLOS = false;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Key.Deleted || !m_Key.IsChildOf( from.Backpack ) )
                {
                    from.SendLocalizedMessage( 501661 ); // That key is unreachable.
                    return;
                }

                if( targeted is MidgardTreasureChest )
                {
                    if( m_Key.UseOn( from, (MidgardTreasureChest)targeted ) )
                        m_Key.Delete();
                }
                else
                    from.SendLocalizedMessage( 501666 ); // You can't unlock that!
            }
        }
    }
}