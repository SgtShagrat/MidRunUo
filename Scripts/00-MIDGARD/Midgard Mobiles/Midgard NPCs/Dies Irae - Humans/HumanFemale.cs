/***************************************************************************
 *                               HumanFemale.cs
 *
 *   begin                : 21 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;

namespace Midgard.Mobiles
{
    public class HumanFemale : BaseHuman
    {
        [Constructable]
        public HumanFemale()
            : this( false )
        {
        }

        [Constructable]
        public HumanFemale( bool mounted )
            : base( true, mounted )
        {
        }

        #region serial-deserial
        public HumanFemale( Serial serial )
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
