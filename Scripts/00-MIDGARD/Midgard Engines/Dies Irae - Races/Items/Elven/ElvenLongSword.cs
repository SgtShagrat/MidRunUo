using Server;
using Server.Items;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    /// <summary>
    /// 0xF60 Elven Long Sword - ( craftabile solo da razza: elfo )
    /// </summary>
    public class ElvenLongSword : Longsword
    {
        public override string DefaultName { get { return "elven long sword"; } }

        [Constructable]
        public ElvenLongSword()
        {
            ItemID = 0xF60;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor
                   || from.Race == Core.HighElf
                   || from.Race == Core.NorthernElf;
        }

        #region serialization
        public ElvenLongSword( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}