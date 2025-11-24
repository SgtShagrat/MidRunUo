using System.Collections.Generic;
using Server.Commands;

namespace Server.Engines.XmlPoints
{
    public class LBSStone : Item
    {
        [Constructable]
        public LBSStone()
            : base( 0xED4 )
        {
            Movable = false;
            Visible = false;
            Name = "LeaderboardSave Stone";

            // is there already another?
            var dlist = new List<Item>();
            foreach( Item i in World.Items.Values )
            {
                if( i is LBSStone && i != this )
                {
                    dlist.Add( i );
                }
            }

            foreach( Item d in dlist )
            {
                d.Delete();
            }
        }

        public LBSStone( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile m )
        {
            if( m == null || m.AccessLevel < AccessLevel.GameMaster )
                return;

            var e = new CommandEventArgs( m, "", "", new string[ 0 ] );
            XmlPoints.XmlPointsAttach.LeaderboardSave_OnCommand( e );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            XmlPoints.XmlPointsAttach.LBSSerialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            XmlPoints.XmlPointsAttach.LBSDeserialize( reader );

            int version = reader.ReadInt();
        }
    }
}