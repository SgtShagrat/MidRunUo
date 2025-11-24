/***************************************************************************
 *                               ScoutGreenTentRoll.cs
 *
 *   begin                : 08 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Multis;

namespace Midgard.Items
{
    public class ScoutGreenTentRoll : BaseScoutTentRoll
    {
        public override string DefaultName
        {
            get { return "a packed scout tent (green)"; }
        }

        [Constructable]
        public ScoutGreenTentRoll()
            : base( 0x72, new Point3D( 0, 0, 0 ) )
        {
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new ScoutTent( owner, 0x72 );
        }

        #region serialization
        public ScoutGreenTentRoll( Serial serial )
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