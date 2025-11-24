/***************************************************************************
 *                               VirtueSymbolAddon.cs
 *
 *   begin                : 14 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public abstract class VirtueSymbolAddon : BaseAddon
    {
        protected VirtueSymbolAddon( bool east, int startIDEast )
            : this( east, startIDEast, startIDEast + 4 )
        {
        }

        protected VirtueSymbolAddon( bool east, int startIDEast, int startIDNorth )
        {
            if( east )
            {
                AddComponent( new AddonComponent( startIDEast + 0 ), 0, 0, 0 );
                AddComponent( new AddonComponent( startIDEast + 1 ), 0, 1, 0 );
                AddComponent( new AddonComponent( startIDEast + 3 ), 1, 0, 0 );
                AddComponent( new AddonComponent( startIDEast + 2 ), 1, 1, 0 );
            }
            else
            {
                AddComponent( new AddonComponent( startIDNorth + 0 ), 0, 0, 0 );
                AddComponent( new AddonComponent( startIDNorth + 3 ), 1, 0, 0 );
                AddComponent( new AddonComponent( startIDNorth + 1 ), 0, 1, 0 );
                AddComponent( new AddonComponent( startIDNorth + 2 ), 1, 1, 0 );
            }
        }

        #region serialization
        public VirtueSymbolAddon( Serial serial )
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
        #endregion
    }

    public class HonestyAddon : VirtueSymbolAddon
    {
        [Constructable]
        public HonestyAddon()
            : this( true )
        {
        }

        [Constructable]
        public HonestyAddon( bool east )
            : base( east, 0x149F )
        {
        }

        #region serialization
        public HonestyAddon( Serial serial )
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
        #endregion
    }

    public class CompassionAddon : VirtueSymbolAddon
    {
        [Constructable]
        public CompassionAddon()
            : this( true )
        {
        }

        [Constructable]
        public CompassionAddon( bool east )
            : base( east, 0x14A7 )
        {
        }

        #region serialization
        public CompassionAddon( Serial serial )
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
        #endregion
    }

    public class JusticeAddon : VirtueSymbolAddon
    {
        [Constructable]
        public JusticeAddon()
            : this( true )
        {
        }

        [Constructable]
        public JusticeAddon( bool east )
            : base( east, 0x14AF )
        {
        }

        #region serialization
        public JusticeAddon( Serial serial )
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
        #endregion
    }

    public class SacrificeAddon : VirtueSymbolAddon
    {
        [Constructable]
        public SacrificeAddon()
            : this( true )
        {
        }

        [Constructable]
        public SacrificeAddon( bool east )
            : base( east, 0x150A )
        {
        }

        #region serialization
        public SacrificeAddon( Serial serial )
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
        #endregion
    }

    public class ValorAddon : VirtueSymbolAddon
    {
        [Constructable]
        public ValorAddon()
            : this( true )
        {
        }

        [Constructable]
        public ValorAddon( bool east )
            : base( east, 0x14B7 )
        {
        }

        #region serialization
        public ValorAddon( Serial serial )
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
        #endregion
    }

    public class SpiritualityAddon : VirtueSymbolAddon
    {
        [Constructable]
        public SpiritualityAddon()
            : this( true )
        {
        }

        [Constructable]
        public SpiritualityAddon( bool east )
            : base( east, 0x14BF )
        {
        }

        #region serialization
        public SpiritualityAddon( Serial serial )
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
        #endregion
    }

    public class HonorAddon : VirtueSymbolAddon
    {
        [Constructable]
        public HonorAddon()
            : this( true )
        {
        }

        [Constructable]
        public HonorAddon( bool east )
            : base( east, 0x14C7 )
        {
        }

        #region serialization
        public HonorAddon( Serial serial )
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
        #endregion
    }

    public class HumilityAddon : VirtueSymbolAddon
    {
        [Constructable]
        public HumilityAddon()
            : this( true )
        {
        }

        [Constructable]
        public HumilityAddon( bool east )
            : base( east, 0x14CF )
        {
        }

        #region serialization
        public HumilityAddon( Serial serial )
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
        #endregion
    }
}