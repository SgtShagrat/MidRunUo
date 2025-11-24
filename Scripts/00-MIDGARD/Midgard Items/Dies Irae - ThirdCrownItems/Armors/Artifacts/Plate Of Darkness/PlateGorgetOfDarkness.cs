using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class PlateGorgetOfDarkness : PlateGorget, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Plate Gorget Of Darkness"; } }

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        public override int OldStrReq { get { return 70; } }
        public override int OldDexBonus { get { return -1; } }

        public override int ArmorBase { get { return 45; } }

        [Constructable]
        public PlateGorgetOfDarkness()
        {
            Hue = 0x9CE;
            Weight = 7.0;
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Armor bonus: {0}\n", ArmorBase );
            builder.AppendFormat( "Strenght required: {0}\n", OldStrReq );
            builder.AppendFormat( "Dexterity malus: {0}\n", OldDexBonus );
        }

        #region serialization
        public PlateGorgetOfDarkness( Serial serial )
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