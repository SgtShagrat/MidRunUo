/***************************************************************************
 *                               FancyLargeForgeAddon.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class FancyLargeForgeAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new FancyLargeForgeAddonDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public FancyLargeForgeAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public FancyLargeForgeAddon( int hue )
        {
            AddComponent( new AddonComponent( 15554 ), -1, 1, 0 );
            AddComponent( new AddonComponent( 15555 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 15556 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 15557 ), 1, 0, 0 );
            Hue = hue;
        }

        public FancyLargeForgeAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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