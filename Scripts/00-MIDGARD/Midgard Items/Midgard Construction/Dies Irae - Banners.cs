/***************************************************************************
 *                               Banners.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;

namespace Midgard
{
    [AttributeUsage( AttributeTargets.Class )]
    public class BannerAttribute : Attribute
    {
        public static bool Check( Item item )
        {
            return ( item != null && item.GetType().IsDefined( typeof( BannerAttribute ), false ) );
        }

        public BannerAttribute()
        {
        }
    }
}

namespace Midgard.Items
{
    [Banner]
    public class LargeBanner1WestAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBanner1WestAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBanner1WestAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBanner1WestAddon( int hue )
        {
            AddComponent( new AddonComponent( 5661 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 5662 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 5663 ), 0, -1, 0 );
            Hue = hue;
        }

        public LargeBanner1WestAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner1WestAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBanner1WestAddon( Hue ); } }
        public override int LabelNumber { get { return 1065052; } } // Large Banner Model 1 (West)

        [Constructable]
        public LargeBanner1WestAddonDeed()
        {
        }

        public LargeBanner1WestAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner1SouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBanner1SouthAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBanner1SouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBanner1SouthAddon( int hue )
        {
            AddComponent( new AddonComponent( 5512 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 5510 ), -1, 0, 0 );
            AddComponent( new AddonComponent( 5511 ), 0, 0, 0 );
            Hue = hue;
        }

        public LargeBanner1SouthAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner1SouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBanner1SouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1065051; } } // Large Banner Model 1 (South)

        [Constructable]
        public LargeBanner1SouthAddonDeed()
        {
        }

        public LargeBanner1SouthAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner2WestAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBanner2WestAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBanner2WestAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBanner2WestAddon( int hue )
        {
            AddComponent( new AddonComponent( 5664 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 5666 ), 0, -1, 0 );
            AddComponent( new AddonComponent( 5665 ), 0, 0, 0 );
            Hue = hue;
        }

        public LargeBanner2WestAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner2WestAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBanner2WestAddon( Hue ); } }
        public override int LabelNumber { get { return 1065054; } } // Large Banner Model 2 (West)

        [Constructable]
        public LargeBanner2WestAddonDeed()
        {
        }

        public LargeBanner2WestAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner2SouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBanner2SouthAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBanner2SouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBanner2SouthAddon( int hue )
        {
            AddComponent( new AddonComponent( 5515 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 5514 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 5513 ), -1, 0, 0 );
            Hue = hue;
        }

        public LargeBanner2SouthAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner2SouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBanner2SouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1065053; } } // Large Banner Model 2 (South)

        [Constructable]
        public LargeBanner2SouthAddonDeed()
        {
        }

        public LargeBanner2SouthAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner3WestAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBanner3WestAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBanner3WestAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBanner3WestAddon( int hue )
        {
            AddComponent( new AddonComponent( 5667 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 5669 ), 0, -1, 0 );
            AddComponent( new AddonComponent( 5668 ), 0, 0, 0 );
            Hue = hue;
        }

        public LargeBanner3WestAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner3WestAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBanner3WestAddon( Hue ); } }
        public override int LabelNumber { get { return 1065056; } } // Large Banner Model 3 (West)

        [Constructable]
        public LargeBanner3WestAddonDeed()
        {
        }

        public LargeBanner3WestAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner3SouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBanner3SouthAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBanner3SouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBanner3SouthAddon( int hue )
        {
            AddComponent( new AddonComponent( 5516 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 5517 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 5518 ), 2, 0, 0 );
            Hue = hue;
        }

        public LargeBanner3SouthAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner3SouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBanner3SouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1065055; } } // Large Banner Model 3 (South)

        [Constructable]
        public LargeBanner3SouthAddonDeed()
        {
        }

        public LargeBanner3SouthAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner4WestAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBanner4WestAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBanner4WestAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBanner4WestAddon( int hue )
        {
            AddComponent( new AddonComponent( 5675 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 5519 ), -1, -1, 0 );
            AddComponent( new AddonComponent( 5676 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 5677 ), 2, -1, 0 );
            Hue = hue;
        }

        public LargeBanner4WestAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner4WestAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBanner4WestAddon( Hue ); } }
        public override int LabelNumber { get { return 1065058; } } // Large Banner Model 4 (West)

        [Constructable]
        public LargeBanner4WestAddonDeed()
        {
        }

        public LargeBanner4WestAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner4SouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBanner4SouthAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBanner4SouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBanner4SouthAddon( int hue )
        {
            AddComponent( new AddonComponent( 5526 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 5524 ), -1, 0, 0 );
            AddComponent( new AddonComponent( 5525 ), 0, 0, 0 );
            Hue = hue;
        }

        public LargeBanner4SouthAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class LargeBanner4SouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBanner4SouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1065057; } } // Large Banner Model 4 (South)

        [Constructable]
        public LargeBanner4SouthAddonDeed()
        {
        }

        public LargeBanner4SouthAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class SmallBanner1WestAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SmallBanner1WestAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public SmallBanner1WestAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public SmallBanner1WestAddon( int hue )
        {
            AddComponent( new AddonComponent( 5672 ), 0, -1, 0 );
            AddComponent( new AddonComponent( 5671 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 5670 ), 0, 1, 0 );
            Hue = hue;
        }

        public SmallBanner1WestAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class SmallBanner1WestAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SmallBanner1WestAddon( Hue ); } }
        public override int LabelNumber { get { return 1065060; } } // Small Banner Model 1 (West)

        [Constructable]
        public SmallBanner1WestAddonDeed()
        {
        }

        public SmallBanner1WestAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class SmallBanner1SouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SmallBanner1SouthAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public SmallBanner1SouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public SmallBanner1SouthAddon( int hue )
        {
            AddComponent( new AddonComponent( 5519 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 5521 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 5520 ), -1, 0, 0 );
            Hue = hue;
        }

        public SmallBanner1SouthAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class SmallBanner1SouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SmallBanner1SouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1065059; } } // Small Banner Model 1 (South)

        [Constructable]
        public SmallBanner1SouthAddonDeed()
        {
        }

        public SmallBanner1SouthAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class SmallBanner2WestAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SmallBanner2WestAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public SmallBanner2WestAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public SmallBanner2WestAddon( int hue )
        {
            AddComponent( new AddonComponent( 5673 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 5674 ), 0, -1, 0 );
            AddComponent( new AddonComponent( 5670 ), 0, 1, 0 );
            Hue = hue;
        }

        public SmallBanner2WestAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class SmallBanner2WestAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SmallBanner2WestAddon( Hue ); } }
        public override int LabelNumber { get { return 1065062; } } // Small Banner Model 2 (West)

        [Constructable]
        public SmallBanner2WestAddonDeed()
        {
        }

        public SmallBanner2WestAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class SmallBanner2SouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SmallBanner2SouthAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public SmallBanner2SouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public SmallBanner2SouthAddon( int hue )
        {
            AddComponent( new AddonComponent( 5519 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 5523 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 5522 ), -1, 0, 0 );
            Hue = hue;
        }

        public SmallBanner2SouthAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }

    [Banner]
    public class SmallBanner2SouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SmallBanner2SouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1065061; } } // Small Banner Model 2 (South)

        [Constructable]
        public SmallBanner2SouthAddonDeed()
        {
        }

        public SmallBanner2SouthAddonDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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
        #endregion
    }
}