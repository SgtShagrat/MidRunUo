/***************************************************************************
 *                               ScoutBlueTentRoll.cs
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
    public class ScoutBlueTentRoll : BaseScoutTentRoll
    {
        public override string DefaultName
        {
            get { return "a packed scout tent (blue)"; }
        }

        [Constructable]
        public ScoutBlueTentRoll()
            : base( 0x70, new Point3D( 0, 0, 0 ) )
        {
        }

        public ScoutBlueTentRoll( Serial serial )
            : base( serial )
        {
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new ScoutTent( owner, 0x70 );
        }

        public override Rectangle2D[] Area
        {
            get { return ScoutTent.AreaArray; }
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