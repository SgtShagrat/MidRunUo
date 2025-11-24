/***************************************************************************
 *                                  Specials.cs
 *                            		-------------
 *  begin                	: Gennaio, 2008
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
using Server;
using Server.Items;
using Server.Multis;
using Server.Network;

namespace Midgard.Items
{
    public class PurpleCrystal : Item
    {
        [Constructable]
        public PurpleCrystal()
            : base( 0x1F1C )
        {
            Name = "Purple Crystal";
            Weight = 2.0;
            Light = LightType.Circle150;
        }

        public PurpleCrystal( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 0x3B2, 1007000 + Utility.Random( 28 ) );
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
    }

    public class GreenCrystal : Item
    {
        [Constructable]
        public GreenCrystal()
            : base( 0x1F19 )
        {
            Name = "Green Crystal";
            Weight = 2.0;
            Light = LightType.Circle150;
        }

        public GreenCrystal( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 0x3B2, 1007000 + Utility.Random( 28 ) );
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
    }

    public class MagicCrystalBall : Item
    {
        [Constructable]
        public MagicCrystalBall()
            : base( 0xE2D )
        {
            Name = "a crystal ball";
            Weight = 10.0;
            Light = LightType.Circle150;
        }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 0x3B2, 1007000 + Utility.Random( 28 ) );
        }

        public MagicCrystalBall( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version 
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class MagicBrazier : Item
    {
        [Constructable]
        public MagicBrazier()
            : base( 0xe31 )
        {
            Name = "a magic brazier";
            Light = LightType.Circle225;
            Weight = 20.0;
        }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 0x3B2, 1007000 + Utility.Random( 28 ) );
        }

        public MagicBrazier( Serial serial )
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
    }

    public class MagicStatue : Item
    {
        [Constructable]
        public MagicStatue()
            : base( 0xe31 )
        {
            Name = "a magic statue";
            Weight = 10.0;
        }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 0x3B2, 1007000 + Utility.Random( 28 ) );
        }

        public MagicStatue( Serial serial )
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
    }

    public class GlobeOfSosariaAddon : BaseAddon
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new GlobeOfSosariaAddonDeed();
            }
        }

        [Constructable]
        public GlobeOfSosariaAddon()
        {
            AddComponent( new AddonComponent( 0x3658 ), -1, 0, 0 );
            AddComponent( new AddonComponent( 0x3657 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 0x3659 ), 0, -1, 0 );

            GlobeOfSosaria globe = new GlobeOfSosaria();
            globe.Light = LightType.Circle300;
            AddComponent( globe, 0, 0, 0 );
        }

        public GlobeOfSosariaAddon( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version 
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }

        private class GlobeOfSosaria : AddonComponent
        {
            private AnimationTimer m_AnimateTimer;

            public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled; } }
            public static readonly TimeSpan Duration = TimeSpan.FromSeconds( 10.0 );

            public bool IsAnimating
            {
                get { return m_AnimateTimer != null; }
            }

            [Constructable]
            public GlobeOfSosaria()
                : base( 0x3660 )
            {
                Light = LightType.Circle300;
                Name = "Globe of Sosaria";
            }

            public void Animate()
            {
                if( ItemID == 0x366F )
                    ItemID = 0x3660;
                else
                    ItemID++;
            }

            public void ToggleTimer( bool hasToStart )
            {
                if( IsAnimating && !hasToStart )
                {
                    PublicOverheadMessage( MessageType.Regular, Utility.RandomMinMax( 90, 120 ), true, "* the Globe of Sosaria stops *" );
                    if( m_AnimateTimer != null && m_AnimateTimer.Running )
                        m_AnimateTimer.Stop();
                    m_AnimateTimer = null;
                }
                else if( !IsAnimating && hasToStart )
                {
                    PublicOverheadMessage( MessageType.Regular, Utility.RandomMinMax( 90, 120 ), true, "* the Globe of Sosaria begins spinning *" );
                    m_AnimateTimer = new AnimationTimer( Duration, this );
                    m_AnimateTimer.Start();
                }
            }

            public bool IsOwner( Mobile mob )
            {
                if( mob.AccessLevel >= AccessLevel.GameMaster )
                    return true;

                BaseHouse house = BaseHouse.FindHouseAt( this );

                return ( house != null && house.IsOwner( mob ) );
            }

            public override void OnDoubleClick( Mobile from )
            {
                if( !from.Player )
                    return;

                if( IsOwner( from ) )
                {
                    ToggleTimer( true );
                }
                else
                {
                    from.SendLocalizedMessage( 502691 ); // You must be the owner to use this.
                }
            }

            public GlobeOfSosaria( Serial serial )
                : base( serial )
            {
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );
                writer.Write( (int)0 ); // version 
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );
                int version = reader.ReadInt();
            }

            internal class AnimationTimer : Timer
            {
                private GlobeOfSosaria m_Globe;
                private DateTime m_Until;

                public AnimationTimer( TimeSpan duration, GlobeOfSosaria globe )
                    : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
                {
                    m_Globe = globe;
                    m_Until = DateTime.Now + duration;

                    Priority = TimerPriority.TwoFiftyMS;
                }

                protected override void OnTick()
                {
                    if( DateTime.Now > m_Until )
                    {
                        if( m_Globe != null && !m_Globe.Deleted )
                            m_Globe.ToggleTimer( false );
                        else
                            Stop();
                    }
                    else if( m_Globe != null && !m_Globe.Deleted )
                        m_Globe.Animate();
                }
            }
        }
    }

    public class GlobeOfSosariaAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new GlobeOfSosariaAddon();
            }
        }

        [Constructable]
        public GlobeOfSosariaAddonDeed()
        {
            Name = "Globe Of Sosaria";
        }

        public GlobeOfSosariaAddonDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 ); // Version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}
