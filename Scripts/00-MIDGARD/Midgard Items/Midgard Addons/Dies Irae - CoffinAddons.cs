/***************************************************************************
 *                                  BaseCoffin.cs
 *                            		-------------
 *  begin                	: Aprile, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Bare apribili.
 * 
 *          BaseCoffin implementa l'apertura del CoffinComponent
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseCoffin : BaseAddon
    {
        private bool m_IsClosed;

        [Constructable]
        public BaseCoffin()
        {
            AddComponents();

            m_IsClosed = true;
        }

        public abstract void AddComponents();

        private void Close()
        {
            foreach( CoffinComponent c in Components )
                c.TurnToClosed();
        }

        private void Open()
        {
            foreach( CoffinComponent c in Components )
                c.TurnToOpened();
        }

        private void SwitchState()
        {
            if( m_IsClosed )
                Open();
            else
                Close();

            m_IsClosed = !m_IsClosed;
        }

        public override void OnComponentUsed( AddonComponent c, Mobile from )
        {
            Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), new TimerCallback( SwitchState ) );
        }

        public BaseCoffin( Serial serial )
            : base( serial )
        {
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

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Close ) );
        }
    }

    public class CoffinComponent : AddonComponent
    {
        private int m_OpenItemID;
        private int m_ClosedItemID;

        public CoffinComponent( int closedItemID )
            : this( closedItemID, closedItemID )
        {
        }

        public CoffinComponent( int openItemID, int closedItemID )
            : base( closedItemID )
        {
            m_OpenItemID = openItemID;
            m_ClosedItemID = closedItemID;

            if( m_ClosedItemID == 0x0000 )
            {
                ItemID = m_OpenItemID;
                Visible = false;
            }
        }

        public void TurnToClosed()
        {
            if( m_ClosedItemID == 0x0000 )
                Visible = false;
            else
            {
                if( !Visible )
                    Visible = true;
                ItemID = m_ClosedItemID;
            }
        }

        public void TurnToOpened()
        {
            if( m_OpenItemID == 0x0000 )
                Visible = false;
            else
            {
                if( !Visible )
                    Visible = true;
                ItemID = m_OpenItemID;
            }
        }

        public CoffinComponent( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (int)m_OpenItemID );
            writer.Write( (int)m_ClosedItemID );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_OpenItemID = reader.ReadInt();
            m_ClosedItemID = reader.ReadInt();
        }
    }

    public class WoodenCoffinSouthAddon : BaseCoffin
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new WoodenCoffinSouthAddonDeed();
            }
        }

        [Constructable]
        public WoodenCoffinSouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public WoodenCoffinSouthAddon( int hue )
        {
            Hue = hue;
        }

        public override void AddComponents()
        {
            AddComponent( new CoffinComponent( 0x1C2B, 0x0000 ), -1, -1, 0 );
            AddComponent( new CoffinComponent( 0x1C2C, 0x1C25 ), 0, -1, 0 );
            AddComponent( new CoffinComponent( 0x1C2D, 0x1C26 ), 1, -1, 0 );
            AddComponent( new CoffinComponent( 0x1C2E, 0x1C27 ), 2, -1, 0 );
            AddComponent( new CoffinComponent( 0x1C2A, 0x1C24 ), 0, 0, 0 );
            AddComponent( new CoffinComponent( 0x1C29, 0x1C23 ), 1, 0, 0 );
            AddComponent( new CoffinComponent( 0x1C28, 0x1C22 ), 2, 0, 0 );
        }

        public WoodenCoffinSouthAddon( Serial serial )
            : base( serial )
        {
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

    public class WoodenCoffinSouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new WoodenCoffinSouthAddon( Hue );
            }
        }

        [Constructable]
        public WoodenCoffinSouthAddonDeed()
        {
            Name = "a wooden coffin addon deed ( south )";
        }

        public WoodenCoffinSouthAddonDeed( Serial serial )
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

    public class WoodenCoffinWestAddon : BaseCoffin
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new WoodenCoffinWestAddonDeed();
            }
        }

        [Constructable]
        public WoodenCoffinWestAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public WoodenCoffinWestAddon( int hue )
        {
            Hue = hue;
        }

        public override void AddComponents()
        {
            AddComponent( new CoffinComponent( 0x1C38, 0x0000 ), -1, -2, 0 );

            AddComponent( new CoffinComponent( 0x1C39, 0x1C32 ), -1, -1, 0 );
            AddComponent( new CoffinComponent( 0x1C3A, 0x1C33 ), -1, 0, 0 );
            AddComponent( new CoffinComponent( 0x1C3B, 0x1C34 ), -1, 1, 0 );

            AddComponent( new CoffinComponent( 0x1C37, 0x1C31 ), 0, -1, 0 );
            AddComponent( new CoffinComponent( 0x1C36, 0x1C30 ), 0, 0, 0 );
            AddComponent( new CoffinComponent( 0x1C35, 0x1C2F ), 0, 1, 0 );
        }

        public WoodenCoffinWestAddon( Serial serial )
            : base( serial )
        {
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

    public class WoodenCoffinWestAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new WoodenCoffinWestAddon( Hue );
            }
        }

        [Constructable]
        public WoodenCoffinWestAddonDeed()
        {
            Name = "a wooden coffin addon deed ( west )";
        }

        public WoodenCoffinWestAddonDeed( Serial serial )
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

    public class PoorWoodenCoffinSouthAddon : BaseCoffin
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new PoorWoodenCoffinSouthAddonDeed();
            }
        }

        [Constructable]
        public PoorWoodenCoffinSouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public PoorWoodenCoffinSouthAddon( int hue )
        {
            Hue = hue;
        }

        public override void AddComponents()
        {
            AddComponent( new CoffinComponent( 7230, 7238 ), 1, 0, 0 );
            AddComponent( new CoffinComponent( 7231, 7237 ), 0, 0, 0 );
            AddComponent( new CoffinComponent( 7232, 7236 ), -1, 0, 0 );
            AddComponent( new CoffinComponent( 7228, 0000 ), 2, 0, 0 );
        }

        public PoorWoodenCoffinSouthAddon( Serial serial )
            : base( serial )
        {
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

    public class PoorWoodenCoffinSouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new PoorWoodenCoffinSouthAddon( Hue );
            }
        }

        [Constructable]
        public PoorWoodenCoffinSouthAddonDeed()
        {
            Name = "a poor wooden coffin addon deed ( south )";
        }

        public PoorWoodenCoffinSouthAddonDeed( Serial serial )
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

    public class PoorWoodenCoffinSouthAnkhAddon : BaseCoffin
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new PoorWoodenCoffinSouthAnkhAddonDeed();
            }
        }

        [Constructable]
        public PoorWoodenCoffinSouthAnkhAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public PoorWoodenCoffinSouthAnkhAddon( int hue )
        {
            Hue = hue;
        }

        public override void AddComponents()
        {
            AddComponent( new CoffinComponent( 7229, 7235 ), 1, 0, 0 );
            AddComponent( new CoffinComponent( 7231, 7234 ), 0, 0, 0 );
            AddComponent( new CoffinComponent( 7232, 7233 ), -1, 0, 0 );
            AddComponent( new CoffinComponent( 7228, 0000 ), 2, 0, 0 );
        }

        public PoorWoodenCoffinSouthAnkhAddon( Serial serial )
            : base( serial )
        {
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

    public class PoorWoodenCoffinSouthAnkhAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new PoorWoodenCoffinSouthAnkhAddon( Hue );
            }
        }

        [Constructable]
        public PoorWoodenCoffinSouthAnkhAddonDeed()
        {
            Name = "a poor ankh wooden coffin addon deed ( south )";
        }

        public PoorWoodenCoffinSouthAnkhAddonDeed( Serial serial )
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