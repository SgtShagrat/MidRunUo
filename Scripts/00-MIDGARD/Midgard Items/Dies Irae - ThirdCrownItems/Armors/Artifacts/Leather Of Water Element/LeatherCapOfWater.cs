/***************************************************************************
 *                               LeatherCapOfWater.cs
 *
 *   begin                : 01 agosto, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class LeatherCapOfWater : LeatherCap, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Leather Cap Of Water Element"; } }

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        public override int OldStrReq { get { return 50; } }
        public override int OldDexBonus { get { return 0; } }

        public override int ArmorBase { get { return 30; } }

        [Constructable]
        public LeatherCapOfWater()
        {
            Hue = 0x880;
            Weight = 1.0;
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Armor bonus: {0}\n", ArmorBase );
            builder.AppendFormat( "Strenght required: {0}\n", OldStrReq );
            builder.AppendFormat( "Dexterity malus: {0}\n", OldDexBonus );
        }

        #region serialization
        public LeatherCapOfWater( Serial serial )
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
}