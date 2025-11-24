using Server;
using Server.Items;

using Core = Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    /// <summary>
    /// 0x10AA Barbarian Chest - ( craftabile solo da razza : Nordico )
    /// </summary>
    public class BarbarianChest : PlateChest
    {
        // 30 * ( 0.44 + 0.14 )
        public override int ArmorBase { get { return 52; } }

        [Constructable]
        public BarbarianChest()
            : this( 0 )
        {
        }

        [Constructable]
        public BarbarianChest( int hue )
        {
            ItemID = 0x10AA;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "barbarian chest"; }
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor
                   || from.Race == Core.NorthernHuman;
        }

        public override bool CanEquip( Mobile from )
        {
            if( from != null && ( from.Player && from.ArmsArmor != null ) )
                return false;

            return base.CanEquip( from );
        }

        #region serialization

        public BarbarianChest( Serial serial )
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