/***************************************************************************
 *                                 CarpetCraftingSystem.cs
 *                            	---------------------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    #region LargeArabesque 5x5 East
    [Carpet]
    public class LargeArabesqueCarpetEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeArabesqueCarpetEastDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeArabesqueCarpetEastAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeArabesqueCarpetEastAddon( int hue )
        {
            AddComponent( new AddonComponent( 2780 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2784 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2784 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2782 ), 3, 0, 0 ); //
            AddComponent( new AddonComponent( 2783 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2778 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2778 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2785 ), 3, 1, 0 ); //
            AddComponent( new AddonComponent( 2783 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2778 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2778 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2785 ), 3, 2, 0 ); //
            AddComponent( new AddonComponent( 2783 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2778 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2778 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2785 ), 3, 3, 0 ); //
            AddComponent( new AddonComponent( 2781 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2786 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2786 ), 2, 4, 0 );
            AddComponent( new AddonComponent( 2779 ), 3, 4, 0 ); //
            Hue = hue;
        }

        public LargeArabesqueCarpetEastAddon( Serial serial )
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

    [Carpet]
    public class LargeArabesqueCarpetEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeArabesqueCarpetEastAddon( Hue ); } }
        public override int LabelNumber { get { return 1064000; } } // Large Square Arabesque Carpet (East)

        [Constructable]
        public LargeArabesqueCarpetEastDeed()
        {
        }

        public LargeArabesqueCarpetEastDeed( Serial serial )
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
    #endregion

    #region LargeArabesque 5x5 South
    [Carpet]
    public class LargeArabesqueCarpetSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeArabesqueCarpetSouthDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeArabesqueCarpetSouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeArabesqueCarpetSouthAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2780 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2784 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2784 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2784 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2782 ), 4, 0, 0 ); //
            AddComponent( new AddonComponent( 2783 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2778 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2778 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2778 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2785 ), 4, 1, 0 ); //
            AddComponent( new AddonComponent( 2783 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2778 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2778 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2778 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2785 ), 4, 2, 0 ); //
            AddComponent( new AddonComponent( 2781 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2786 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2786 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2786 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2779 ), 4, 3, 0 ); //
            Hue = hue;
        }

        public LargeArabesqueCarpetSouthAddon( Serial serial )
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

    [Carpet]
    public class LargeArabesqueCarpetSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeArabesqueCarpetSouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1064001; } } // Large Square Arabesque Carpet (South)

        [Constructable]
        public LargeArabesqueCarpetSouthDeed()
        {
        }

        public LargeArabesqueCarpetSouthDeed( Serial serial )
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
    #endregion

    #region LargeBlueArabesque East
    [Carpet]
    public class LargeBlueArabesqueCarpetEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBlueArabesqueCarpetEastDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBlueArabesqueCarpetEastAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBlueArabesqueCarpetEastAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2771 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2775 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2775 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2775 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2773 ), 4, 0, 0 ); //
            AddComponent( new AddonComponent( 2774 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2769 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2769 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2769 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2776 ), 4, 1, 0 ); //
            AddComponent( new AddonComponent( 2774 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2769 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2769 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2769 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2776 ), 4, 2, 0 ); //
            AddComponent( new AddonComponent( 2774 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2769 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2769 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2769 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2776 ), 4, 3, 0 ); //
            AddComponent( new AddonComponent( 2774 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2769 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2769 ), 2, 4, 0 );
            AddComponent( new AddonComponent( 2769 ), 3, 4, 0 );
            AddComponent( new AddonComponent( 2776 ), 4, 4, 0 ); //
            AddComponent( new AddonComponent( 2772 ), 0, 5, 0 );
            AddComponent( new AddonComponent( 2777 ), 1, 5, 0 );
            AddComponent( new AddonComponent( 2777 ), 2, 5, 0 );
            AddComponent( new AddonComponent( 2777 ), 3, 5, 0 );
            AddComponent( new AddonComponent( 2770 ), 4, 5, 0 ); //
            Hue = hue;
        }

        public LargeBlueArabesqueCarpetEastAddon( Serial serial )
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

    [Carpet]
    public class LargeBlueArabesqueCarpetEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBlueArabesqueCarpetEastAddon( Hue ); } }
        public override int LabelNumber { get { return 1064002; } } // Large Blue Arabesque Carpet (East)

        [Constructable]
        public LargeBlueArabesqueCarpetEastDeed()
        {
        }

        public LargeBlueArabesqueCarpetEastDeed( Serial serial )
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
    #endregion

    #region LargeBlueArabesque South
    [Carpet]
    public class LargeBlueArabesqueCarpetSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBlueArabesqueCarpetSouthDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBlueArabesqueCarpetSouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBlueArabesqueCarpetSouthAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2771 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2775 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2775 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2775 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2775 ), 4, 0, 0 );
            AddComponent( new AddonComponent( 2773 ), 5, 0, 0 ); //
            AddComponent( new AddonComponent( 2774 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2769 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2769 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2769 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2769 ), 4, 1, 0 );
            AddComponent( new AddonComponent( 2776 ), 5, 1, 0 ); //
            AddComponent( new AddonComponent( 2774 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2769 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2769 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2769 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2769 ), 4, 2, 0 );
            AddComponent( new AddonComponent( 2776 ), 5, 2, 0 ); //
            AddComponent( new AddonComponent( 2774 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2769 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2769 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2769 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2769 ), 4, 3, 0 );
            AddComponent( new AddonComponent( 2776 ), 5, 3, 0 ); //
            AddComponent( new AddonComponent( 2772 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2777 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2777 ), 2, 4, 0 );
            AddComponent( new AddonComponent( 2777 ), 3, 4, 0 );
            AddComponent( new AddonComponent( 2777 ), 4, 4, 0 );
            AddComponent( new AddonComponent( 2770 ), 5, 4, 0 ); //
            Hue = hue;
        }

        public LargeBlueArabesqueCarpetSouthAddon( Serial serial )
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

    [Carpet]
    public class LargeBlueArabesqueCarpetSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBlueArabesqueCarpetSouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1064003; } } // Large Blue Arabesque Carpet (South)

        [Constructable]
        public LargeBlueArabesqueCarpetSouthDeed()
        {
        }

        public LargeBlueArabesqueCarpetSouthDeed( Serial serial )
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
    #endregion

    #region LargeBlueCarpet East
    [Carpet]
    public class LargeBlueCarpetEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBlueCarpetEastDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBlueCarpetEastAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBlueCarpetEastAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2755 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2757 ), 4, 0, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2750 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2753 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2750 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2808 ), 4, 1, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2750 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2753 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2750 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2808 ), 4, 2, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2752 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2750 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2749 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2808 ), 4, 3, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2750 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2753 ), 2, 4, 0 );
            AddComponent( new AddonComponent( 2750 ), 3, 4, 0 );
            AddComponent( new AddonComponent( 2808 ), 4, 4, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 5, 0 );
            AddComponent( new AddonComponent( 2750 ), 1, 5, 0 );
            AddComponent( new AddonComponent( 2753 ), 2, 5, 0 );
            AddComponent( new AddonComponent( 2750 ), 3, 5, 0 );
            AddComponent( new AddonComponent( 2808 ), 4, 5, 0 ); //
            AddComponent( new AddonComponent( 2756 ), 0, 6, 0 );
            AddComponent( new AddonComponent( 2809 ), 1, 6, 0 );
            AddComponent( new AddonComponent( 2809 ), 2, 6, 0 );
            AddComponent( new AddonComponent( 2809 ), 3, 6, 0 );
            AddComponent( new AddonComponent( 2754 ), 4, 6, 0 ); //
            Hue = hue;
        }

        public LargeBlueCarpetEastAddon( Serial serial )
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

    [Carpet]
    public class LargeBlueCarpetEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBlueCarpetEastAddon( Hue ); } }
        public override int LabelNumber { get { return 1064004; } } // Large Blue Carpet (East)

        [Constructable]
        public LargeBlueCarpetEastDeed()
        {
        }

        public LargeBlueCarpetEastDeed( Serial serial )
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
    #endregion

    #region LargeBlueCarpet South
    [Carpet]
    public class LargeBlueCarpetSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeBlueCarpetSouthDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeBlueCarpetSouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeBlueCarpetSouthAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2755 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 4, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 5, 0, 0 );
            AddComponent( new AddonComponent( 2757 ), 6, 0, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2750 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2750 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2753 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2750 ), 4, 1, 0 );
            AddComponent( new AddonComponent( 2750 ), 5, 1, 0 );
            AddComponent( new AddonComponent( 2808 ), 6, 1, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2752 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2752 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2750 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2749 ), 4, 2, 0 );
            AddComponent( new AddonComponent( 2749 ), 5, 2, 0 );
            AddComponent( new AddonComponent( 2808 ), 6, 2, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2750 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2750 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2751 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2750 ), 4, 3, 0 );
            AddComponent( new AddonComponent( 2750 ), 5, 3, 0 );
            AddComponent( new AddonComponent( 2808 ), 6, 3, 0 ); //
            AddComponent( new AddonComponent( 2756 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2809 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2809 ), 2, 4, 0 );
            AddComponent( new AddonComponent( 2809 ), 3, 4, 0 );
            AddComponent( new AddonComponent( 2809 ), 4, 4, 0 );
            AddComponent( new AddonComponent( 2809 ), 5, 4, 0 );
            AddComponent( new AddonComponent( 2754 ), 6, 4, 0 ); //
            Hue = hue;
        }

        public LargeBlueCarpetSouthAddon( Serial serial )
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

    [Carpet]
    public class LargeBlueCarpetSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBlueCarpetSouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1064005; } } // Large Blue Carpet (South)

        [Constructable]
        public LargeBlueCarpetSouthDeed()
        {
        }

        public LargeBlueCarpetSouthDeed( Serial serial )
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
    #endregion

    #region LargeDecoratedCarpet East
    [Carpet]
    public class LargeDecoratedCarpetEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeDecoratedCarpetEastDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeDecoratedCarpetEastAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeDecoratedCarpetEastAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2788 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2792 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2792 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2792 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2790 ), 4, 0, 0 ); //
            AddComponent( new AddonComponent( 2791 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2795 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2795 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2795 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2793 ), 4, 1, 0 ); //
            AddComponent( new AddonComponent( 2791 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2795 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2795 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2795 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2793 ), 4, 2, 0 ); //
            AddComponent( new AddonComponent( 2791 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2795 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2795 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2795 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2793 ), 4, 3, 0 ); //
            AddComponent( new AddonComponent( 2791 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2795 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2795 ), 2, 4, 0 );
            AddComponent( new AddonComponent( 2795 ), 3, 4, 0 );
            AddComponent( new AddonComponent( 2793 ), 4, 4, 0 ); //
            AddComponent( new AddonComponent( 2789 ), 0, 5, 0 );
            AddComponent( new AddonComponent( 2794 ), 1, 5, 0 );
            AddComponent( new AddonComponent( 2794 ), 2, 5, 0 );
            AddComponent( new AddonComponent( 2794 ), 3, 5, 0 );
            AddComponent( new AddonComponent( 2787 ), 4, 5, 0 ); //
            Hue = hue;
        }

        public LargeDecoratedCarpetEastAddon( Serial serial )
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

    [Carpet]
    public class LargeDecoratedCarpetEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeDecoratedCarpetEastAddon( Hue ); } }
        public override int LabelNumber { get { return 1064006; } } // Large Decorated Carpet (East)

        [Constructable]
        public LargeDecoratedCarpetEastDeed()
        {
        }

        public LargeDecoratedCarpetEastDeed( Serial serial )
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
    #endregion

    #region LargeDecoratedCarpet South
    [Carpet]
    public class LargeDecoratedCarpetSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeDecoratedCarpetSouthDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeDecoratedCarpetSouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeDecoratedCarpetSouthAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2788 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2792 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2792 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2792 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2792 ), 4, 0, 0 );
            AddComponent( new AddonComponent( 2790 ), 5, 0, 0 ); //
            AddComponent( new AddonComponent( 2791 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2795 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2795 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2795 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2795 ), 4, 1, 0 );
            AddComponent( new AddonComponent( 2793 ), 5, 1, 0 ); //
            AddComponent( new AddonComponent( 2791 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2795 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2795 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2795 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2795 ), 4, 2, 0 );
            AddComponent( new AddonComponent( 2793 ), 5, 2, 0 ); //
            AddComponent( new AddonComponent( 2791 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2795 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2795 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2795 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2795 ), 4, 3, 0 );
            AddComponent( new AddonComponent( 2793 ), 5, 3, 0 ); //
            AddComponent( new AddonComponent( 2789 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2794 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2794 ), 2, 4, 0 );
            AddComponent( new AddonComponent( 2794 ), 3, 4, 0 );
            AddComponent( new AddonComponent( 2794 ), 4, 4, 0 );
            AddComponent( new AddonComponent( 2787 ), 5, 4, 0 ); //
            Hue = hue;
        }

        public LargeDecoratedCarpetSouthAddon( Serial serial )
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

    [Carpet]
    public class LargeDecoratedCarpetSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeDecoratedCarpetSouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1064007; } } // Large Decorated Carpet (South)

        [Constructable]
        public LargeDecoratedCarpetSouthDeed()
        {
        }

        public LargeDecoratedCarpetSouthDeed( Serial serial )
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
    #endregion

    #region RedCarpet East
    [Carpet]
    public class RedCarpetEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new RedCarpetEastDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public RedCarpetEastAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public RedCarpetEastAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2762 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2766 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2764 ), 2, 0, 0 ); //
            AddComponent( new AddonComponent( 2765 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2760 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2767 ), 2, 1, 0 ); //
            AddComponent( new AddonComponent( 2765 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2760 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2767 ), 2, 2, 0 ); //
            AddComponent( new AddonComponent( 2765 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2760 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2767 ), 2, 3, 0 ); //
            AddComponent( new AddonComponent( 2763 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2768 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2761 ), 2, 4, 0 ); //
            Hue = hue;
        }

        public RedCarpetEastAddon( Serial serial )
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

    [Carpet]
    public class RedCarpetEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new RedCarpetEastAddon( Hue ); } }
        public override int LabelNumber { get { return 1064008; } } // Red Carpet (East)

        [Constructable]
        public RedCarpetEastDeed()
        {
        }

        public RedCarpetEastDeed( Serial serial )
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
    #endregion

    #region RedCarpet South
    [Carpet]
    public class RedCarpetSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new RedCarpetSouthDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public RedCarpetSouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public RedCarpetSouthAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2762 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2766 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2766 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2766 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2764 ), 4, 0, 0 ); //
            AddComponent( new AddonComponent( 2765 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2760 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2760 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2760 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2767 ), 4, 1, 0 ); //
            AddComponent( new AddonComponent( 2763 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2768 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2768 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2768 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2761 ), 4, 2, 0 ); //
            Hue = hue;
        }

        public RedCarpetSouthAddon( Serial serial )
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

    [Carpet]
    public class RedCarpetSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new RedCarpetSouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1064009; } } // Red Carpet (South)

        [Constructable]
        public RedCarpetSouthDeed()
        {
        }

        public RedCarpetSouthDeed( Serial serial )
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
    #endregion

    #region RedDecoratedCarpet East
    [Carpet]
    public class RedDecoratedCarpetEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new RedDecoratedCarpetEastDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public RedDecoratedCarpetEastAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public RedDecoratedCarpetEastAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2762 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2766 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2764 ), 2, 0, 0 ); //
            AddComponent( new AddonComponent( 2765 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2759 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2767 ), 2, 1, 0 ); //
            AddComponent( new AddonComponent( 2765 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2759 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2767 ), 2, 2, 0 ); //
            AddComponent( new AddonComponent( 2765 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2759 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2767 ), 2, 3, 0 ); //
            AddComponent( new AddonComponent( 2763 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2768 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2761 ), 2, 4, 0 ); //
            Hue = hue;
        }

        public RedDecoratedCarpetEastAddon( Serial serial )
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

    [Carpet]
    public class RedDecoratedCarpetEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new RedDecoratedCarpetEastAddon( Hue ); } }
        public override int LabelNumber { get { return 1064010; } } // Red Decorated Carpet (East)

        [Constructable]
        public RedDecoratedCarpetEastDeed()
        {
        }

        public RedDecoratedCarpetEastDeed( Serial serial )
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
    #endregion

    #region RedDecoratedCarpet South
    [Carpet]
    public class RedDecoratedCarpetSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new RedDecoratedCarpetSouthDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public RedDecoratedCarpetSouthAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public RedDecoratedCarpetSouthAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2762 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2766 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2766 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2766 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2764 ), 4, 0, 0 ); //
            AddComponent( new AddonComponent( 2765 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2758 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2758 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2758 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2767 ), 4, 1, 0 ); //
            AddComponent( new AddonComponent( 2763 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2768 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2768 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2768 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2761 ), 4, 2, 0 ); //
            Hue = hue;
        }

        public RedDecoratedCarpetSouthAddon( Serial serial )
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

    [Carpet]
    public class RedDecoratedCarpetSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new RedDecoratedCarpetSouthAddon( Hue ); } }
        public override int LabelNumber { get { return 1064011; } } // Red Decorated Carpet (South)

        [Constructable]
        public RedDecoratedCarpetSouthDeed()
        {
        }

        public RedDecoratedCarpetSouthDeed( Serial serial )
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
    #endregion

    #region SquareArabesqueCarpetSmall
    [Carpet]
    public class SquareArabesqueCarpetSmallAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SquareArabesqueCarpetSmallDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public SquareArabesqueCarpetSmallAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public SquareArabesqueCarpetSmallAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2780 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2784 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2782 ), 2, 0, 0 ); //
            AddComponent( new AddonComponent( 2783 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2778 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2785 ), 2, 1, 0 ); //
            AddComponent( new AddonComponent( 2781 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2786 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2779 ), 2, 2, 0 ); //
            Hue = hue;
        }

        public SquareArabesqueCarpetSmallAddon( Serial serial )
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

    [Carpet]
    public class SquareArabesqueCarpetSmallDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SquareArabesqueCarpetSmallAddon( Hue ); } }
        public override int LabelNumber { get { return 1064012; } } // Square Arabesque Carpet (Small)

        [Constructable]
        public SquareArabesqueCarpetSmallDeed()
        {
        }

        public SquareArabesqueCarpetSmallDeed( Serial serial )
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
    #endregion

    #region SquareBlueArabesqueCarpet
    [Carpet]
    public class SquareBlueArabesqueCarpetAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SquareBlueArabesqueCarpetDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public SquareBlueArabesqueCarpetAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public SquareBlueArabesqueCarpetAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2771 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2775 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2775 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2775 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2773 ), 4, 0, 0 ); //
            AddComponent( new AddonComponent( 2774 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2769 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2769 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2769 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2776 ), 4, 1, 0 ); //
            AddComponent( new AddonComponent( 2774 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2769 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2769 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2769 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2776 ), 4, 2, 0 ); //
            AddComponent( new AddonComponent( 2774 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2769 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2769 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2769 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2776 ), 4, 3, 0 ); //
            AddComponent( new AddonComponent( 2772 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2777 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2777 ), 2, 4, 0 );
            AddComponent( new AddonComponent( 2777 ), 3, 4, 0 );
            AddComponent( new AddonComponent( 2770 ), 4, 4, 0 ); //
            Hue = hue;
        }

        public SquareBlueArabesqueCarpetAddon( Serial serial )
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

    [Carpet]
    public class SquareBlueArabesqueCarpetDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SquareBlueArabesqueCarpetAddon( Hue ); } }
        public override int LabelNumber { get { return 1064013; } } // large square blue arabesque carpet

        [Constructable]
        public SquareBlueArabesqueCarpetDeed()
        {
        }

        public SquareBlueArabesqueCarpetDeed( Serial serial )
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
    #endregion

    #region SquareBlueCarpetLarge
    [Carpet]
    public class SquareBlueCarpetLargeAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SquareBlueCarpetLargeDeed(); } }

        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public SquareBlueCarpetLargeAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public SquareBlueCarpetLargeAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2755 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2757 ), 4, 0, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2750 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2753 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2750 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2808 ), 4, 1, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2752 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2750 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2749 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2808 ), 4, 2, 0 ); //
            AddComponent( new AddonComponent( 2806 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2750 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2751 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2750 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2808 ), 4, 3, 0 ); //
            AddComponent( new AddonComponent( 2756 ), 0, 4, 0 );
            AddComponent( new AddonComponent( 2809 ), 1, 4, 0 );
            AddComponent( new AddonComponent( 2809 ), 2, 4, 0 );
            AddComponent( new AddonComponent( 2809 ), 3, 4, 0 );
            AddComponent( new AddonComponent( 2754 ), 4, 4, 0 );
            Hue = hue;
        }

        public SquareBlueCarpetLargeAddon( Serial serial )
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

    [Carpet]
    public class SquareBlueCarpetLargeDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SquareBlueCarpetLargeAddon( Hue ); } }
        public override int LabelNumber { get { return 1064014; } } // Square Blue Carpet (Medium)

        [Constructable]
        public SquareBlueCarpetLargeDeed()
        {
        }

        public SquareBlueCarpetLargeDeed( Serial serial )
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
    #endregion

    #region SquareDecoratedCarpetMedium
    [Carpet]
    public class SquareDecoratedCarpetMediumAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SquareDecoratedCarpetMediumDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public SquareDecoratedCarpetMediumAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public SquareDecoratedCarpetMediumAddon( int hue )
        {
            //x, y, z
            AddComponent( new AddonComponent( 2788 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2792 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2792 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2790 ), 3, 0, 0 ); //
            AddComponent( new AddonComponent( 2791 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2795 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2795 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2793 ), 3, 1, 0 ); //
            AddComponent( new AddonComponent( 2791 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2795 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2795 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2793 ), 3, 2, 0 ); //
            AddComponent( new AddonComponent( 2789 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2794 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2794 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2787 ), 3, 3, 0 ); //
            Hue = hue;
        }

        public SquareDecoratedCarpetMediumAddon( Serial serial )
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

    [Carpet]
    public class SquareDecoratedCarpetMediumDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SquareDecoratedCarpetMediumAddon( Hue ); } }
        public override int LabelNumber { get { return 1064015; } } // Square Decorated Carpet (Medium)

        [Constructable]
        public SquareDecoratedCarpetMediumDeed()
        {
        }

        public SquareDecoratedCarpetMediumDeed( Serial serial )
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
    #endregion

    #region SmallSquareBlueDecoratedCarpet
    [Carpet]
    public class SmallSquareBlueDecoratedCarpetAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SmallSquareBlueDecoratedCarpetAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public SmallSquareBlueDecoratedCarpetAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public SmallSquareBlueDecoratedCarpetAddon( int hue )
        {
            AddComponent( new AddonComponent( 2806 ), -2, 1, 0 );
            AddComponent( new AddonComponent( 2806 ), -2, 0, 0 );
            AddComponent( new AddonComponent( 2806 ), -2, 0, 0 );
            AddComponent( new AddonComponent( 2806 ), -2, -1, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, -1, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, 1, 0 );
            AddComponent( new AddonComponent( 2809 ), -1, 2, 0 );
            AddComponent( new AddonComponent( 2756 ), -2, 2, 0 );
            AddComponent( new AddonComponent( 2809 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, -1, 0 );
            AddComponent( new AddonComponent( 2807 ), 1, -2, 0 );
            AddComponent( new AddonComponent( 2809 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, -1, 0 );
            AddComponent( new AddonComponent( 2807 ), 0, -2, 0 );
            AddComponent( new AddonComponent( 2754 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2808 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2808 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2808 ), 2, -1, 0 );
            AddComponent( new AddonComponent( 2757 ), 2, -2, 0 );
            AddComponent( new AddonComponent( 2807 ), -1, -2, 0 );
            AddComponent( new AddonComponent( 2755 ), -2, -2, 0 );
            Hue = hue;
        }

        public SmallSquareBlueDecoratedCarpetAddon( Serial serial )
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

    public class SmallSquareBlueDecoratedCarpetAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SmallSquareBlueDecoratedCarpetAddon( Hue ); } }
        public override int LabelNumber { get { return 1064016; } } // Square Blue Decorated Carpet (Small)

        [Constructable]
        public SmallSquareBlueDecoratedCarpetAddonDeed()
        {
        }

        public SmallSquareBlueDecoratedCarpetAddonDeed( Serial serial )
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
    #endregion

    #region MediumSquareBlueDecoratedCarpetAddon
    [Carpet]
    public class MediumSquareBlueDecoratedCarpetAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new MediumSquareBlueDecoratedCarpetAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public MediumSquareBlueDecoratedCarpetAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public MediumSquareBlueDecoratedCarpetAddon( int hue )
        {
            AddComponent( new AddonComponent( 2808 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2808 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2754 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2810 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2809 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2810 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2807 ), -1, -2, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, -1, 0 );
            AddComponent( new AddonComponent( 2807 ), 1, -2, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, -1, 0 );
            AddComponent( new AddonComponent( 2810 ), 2, -1, 0 );
            AddComponent( new AddonComponent( 2807 ), 2, -2, 0 );
            AddComponent( new AddonComponent( 2809 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2809 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2806 ), -2, 1, 0 );
            AddComponent( new AddonComponent( 2806 ), -2, 1, 0 );
            AddComponent( new AddonComponent( 2806 ), -2, 0, 0 );
            AddComponent( new AddonComponent( 2806 ), -2, 0, 0 );
            AddComponent( new AddonComponent( 2756 ), -2, 3, 0 );
            AddComponent( new AddonComponent( 2806 ), -2, 2, 0 );
            AddComponent( new AddonComponent( 2806 ), -2, -1, 0 );
            AddComponent( new AddonComponent( 2755 ), -2, -2, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, -1, 0 );
            AddComponent( new AddonComponent( 2807 ), 0, -2, 0 );
            AddComponent( new AddonComponent( 2757 ), 3, -2, 0 );
            AddComponent( new AddonComponent( 2808 ), 3, -1, 0 );
            AddComponent( new AddonComponent( 2808 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, 1, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, 2, 0 );
            AddComponent( new AddonComponent( 2809 ), -1, 3, 0 );
            AddComponent( new AddonComponent( 2810 ), 2, 1, 0 );
            Hue = hue;
        }

        public MediumSquareBlueDecoratedCarpetAddon( Serial serial )
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

    public class MediumSquareBlueDecoratedCarpetAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new MediumSquareBlueDecoratedCarpetAddon( Hue ); } }
        public override int LabelNumber { get { return 1064017; } } // Square Blue Decorated Carpet (Medium)

        [Constructable]
        public MediumSquareBlueDecoratedCarpetAddonDeed()
        {
        }

        public MediumSquareBlueDecoratedCarpetAddonDeed( Serial serial )
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
    #endregion

    #region LargeSquareBlueDecoratedCarpetAddon
    [Carpet]
    public class LargeSquareBlueDecoratedCarpetAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeSquareBlueDecoratedCarpetAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public LargeSquareBlueDecoratedCarpetAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public LargeSquareBlueDecoratedCarpetAddon( int hue )
        {
            AddComponent( new AddonComponent( 2806 ), -3, 2, 0 );
            AddComponent( new AddonComponent( 2756 ), -3, 3, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 2754 ), 3, 3, 0 );
            AddComponent( new AddonComponent( 2808 ), 3, 2, 0 );
            AddComponent( new AddonComponent( 2808 ), 3, -1, 0 );
            AddComponent( new AddonComponent( 2810 ), 2, -1, 0 );
            AddComponent( new AddonComponent( 2810 ), 2, -2, 0 );
            AddComponent( new AddonComponent( 2807 ), 2, -3, 0 );
            AddComponent( new AddonComponent( 2809 ), 2, 3, 0 );
            AddComponent( new AddonComponent( 2810 ), 2, 2, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, -2, 0 );
            AddComponent( new AddonComponent( 2807 ), 0, -3, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, -1, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, 2, 0 );
            AddComponent( new AddonComponent( 2809 ), 1, 3, 0 );
            AddComponent( new AddonComponent( 2809 ), 0, 3, 0 );
            AddComponent( new AddonComponent( 2808 ), 3, 0, 0 );
            AddComponent( new AddonComponent( 2757 ), 3, -3, 0 );
            AddComponent( new AddonComponent( 2808 ), 3, -2, 0 );
            AddComponent( new AddonComponent( 2808 ), 3, 1, 0 );
            AddComponent( new AddonComponent( 2810 ), 2, 1, 0 );
            AddComponent( new AddonComponent( 2810 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, -1, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, 0, 0 );
            AddComponent( new AddonComponent( 2807 ), -1, -3, 0 );
            AddComponent( new AddonComponent( 2807 ), -2, -3, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2810 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, 1, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, 2, 0 );
            AddComponent( new AddonComponent( 2807 ), 1, -3, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, -2, 0 );
            AddComponent( new AddonComponent( 2810 ), 1, -1, 0 );
            AddComponent( new AddonComponent( 2755 ), -3, -3, 0 );
            AddComponent( new AddonComponent( 2806 ), -3, -2, 0 );
            AddComponent( new AddonComponent( 2806 ), -3, -1, 0 );
            AddComponent( new AddonComponent( 2806 ), -3, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), -2, 0, 0 );
            AddComponent( new AddonComponent( 2810 ), -2, -1, 0 );
            AddComponent( new AddonComponent( 2810 ), -2, -2, 0 );
            AddComponent( new AddonComponent( 2810 ), -1, -2, 0 );
            AddComponent( new AddonComponent( 2809 ), -1, 3, 0 );
            AddComponent( new AddonComponent( 2809 ), -2, 3, 0 );
            AddComponent( new AddonComponent( 2810 ), -2, 2, 0 );
            AddComponent( new AddonComponent( 2810 ), -2, 1, 0 );
            AddComponent( new AddonComponent( 2806 ), -3, 1, 0 );
            Hue = hue;
        }

        public LargeSquareBlueDecoratedCarpetAddon( Serial serial )
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

    public class LargeSquareBlueDecoratedCarpetAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeSquareBlueDecoratedCarpetAddon( Hue ); } }
        public override int LabelNumber { get { return 1064018; } } // Square Blue Decorated Carpet (Large)

        [Constructable]
        public LargeSquareBlueDecoratedCarpetAddonDeed()
        {
        }

        public LargeSquareBlueDecoratedCarpetAddonDeed( Serial serial )
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
    #endregion
}