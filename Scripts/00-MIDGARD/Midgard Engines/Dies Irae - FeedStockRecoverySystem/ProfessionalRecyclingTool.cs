/***************************************************************************
 *                               ProfessionalRecyclingTool.cs
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
    public class ProfessionalRecyclingTool : BaseRecyclingTool
    {
        public override string DefaultName
        {
            get { return "a professional recycling toolkit"; }
        }

        public override double DifficultyBonus
        {
            get { return 0.35; }
        }

        [Constructable]
        public ProfessionalRecyclingTool()
            : this( 30 )
        {
        }

        [Constructable]
        public ProfessionalRecyclingTool( int charges )
            : base( 0x1EB8, charges )
        {
            Weight = 3;
            Hue = Utility.RandomMetalHue();
        }

        #region serialization
        public ProfessionalRecyclingTool( Serial serial )
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