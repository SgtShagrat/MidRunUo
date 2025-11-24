/***************************************************************************
 *                               FancyLargeForgeAddonDeed.cs
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
    public class FancyLargeForgeAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new FancyLargeForgeAddon( Hue ); } }
        public override int LabelNumber { get { return 1064959; } } // Fancy Large Forge

        [Constructable]
        public FancyLargeForgeAddonDeed()
        {
        }

        public FancyLargeForgeAddonDeed( Serial serial )
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