/***************************************************************************
 *                               RecyclingTool.cs
 *
 *   begin                : 23 aprile 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.FeedStockRecoverySystem
{
    public class RecyclingTool : BaseRecyclingTool
    {
        public override string DefaultName
        {
            get { return "a recycling toolkit"; }
        }

        public override double DifficultyBonus
        {
            get { return 10.0; }
        }

        [Constructable]
        public RecyclingTool()
            : this( 50 )
        {
        }

        [Constructable]
        public RecyclingTool( int charges )
            : base( 0x1EB8, charges )
        {
            Weight = 2;
            Hue = Utility.RandomNeutralHue();
        }

        #region serialization
        public RecyclingTool( Serial serial )
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