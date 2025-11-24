/***************************************************************************
 *                               DefensorOfTheCodex.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class DefensorOfTheCodex : Robe
    {
        public override TownSystem RequiredTownSystem { get { return TownSystem.SerpentsHold; } }

        public override string DefaultName { get { return "Difensore del Codice"; } }

        [Constructable]
        public DefensorOfTheCodex()
            : this( 0 )
        {
        }

        [Constructable]
        public DefensorOfTheCodex( int hue )
            : base( hue )
        {
            ItemID = 0x3DBC;
        }

        #region serialization
        public DefensorOfTheCodex( Serial serial )
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