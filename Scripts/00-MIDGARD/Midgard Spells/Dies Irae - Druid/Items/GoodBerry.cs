/***************************************************************************
 *                               GoodBerry.cs
 *                            -------------------
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Engines.SpellSystem
{
    public class GoodBerry : Food
    {
        public override string DefaultName
        {
            get { return "a good berry"; }
        }

        [Constructable]
        public GoodBerry()
            : base( 0x9D0 )
        {
            Hue = 1378;
            Weight = 2.0;
            FillFactor = 5;
            Stackable = true;
        }

        public GoodBerry( Serial serial )
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
}