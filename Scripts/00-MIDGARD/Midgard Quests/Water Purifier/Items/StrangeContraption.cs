using System;

using Midgard.Engines.PlagueBeastLordPuzzle;

using Server.Engines.Quests.Hag;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class StrangeContraption : Item
    {
        [Constructable]
        public StrangeContraption()
            : base( 0x1922 )
        {
            Name = "a strange contraption";
            Movable = false;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !from.InRange( Location, 2 ) || !from.InLOS( this ) )
            {
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
                return;
            }

            MutationCore pc = from.Backpack.FindItemByType( typeof( MutationCore ) ) as MutationCore;
            Obsidian ob = from.Backpack.FindItemByType( typeof( Obsidian ) ) as Obsidian;
            MoonfireBrew mb = from.Backpack.FindItemByType( typeof( MoonfireBrew ) ) as MoonfireBrew;

            PlayerMobile pm = from as PlayerMobile;
            if( pm == null || pm.CompassionGains >= 5 ) // have already gained 5 points in one day, can gain no more
            {
                from.SendLocalizedMessage( 1060001 ); // You throw the switch, but the mechanism cannot be engaged again so soon.
                return;
            }

            if( pc != null && ob != null && mb != null )
            {
                from.SendLocalizedMessage( 1055143, "", 0x59 );
                //You add the required ingredients and activate the contraption. 
                // It rumbles and smokes and then falls silent. 
                // The water shines for a brief moment, and you feel confident that it is now much less tainted than before.

                pc.Consume();
                ob.Consume();
                mb.Consume();

                bool gainedPath = false;

                if( VirtueHelper.Award( from, VirtueName.Compassion, 5, ref gainedPath ) )
                {
                    if( gainedPath )
                        from.SendLocalizedMessage( 1053005 ); // You have achieved a path in compassion!
                    else
                        from.SendLocalizedMessage( 1053002 ); // You have gained in compassion.

                    pm.NextCompassionDay = DateTime.Now + TimeSpan.FromDays( 1.0 ); // in one day CompassionGains gets reset to 0
                }
                else
                    from.SendLocalizedMessage( 1053003 ); // You have achieved the highest path of compassion and can no longer gain any further.
            }
            else
                from.SendLocalizedMessage( 1055142, "", 0x59 ); //You do not have the necessary ingredients. The contraptions rumbles angrily but does nothing.
        }

        #region serialization
        public StrangeContraption( Serial serial )
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