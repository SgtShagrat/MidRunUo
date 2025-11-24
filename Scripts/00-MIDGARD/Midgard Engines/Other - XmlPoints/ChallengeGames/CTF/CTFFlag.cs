using System;
using Server.Items;
using Server.Network;

namespace Server.Engines.XmlPoints
{
    public class CTFFlag : Item
    {
        public const int OwnershipHue = 0xB;
        public CTFBase HomeBase;

        public CTFFlag( CTFBase homebase, int team )
            : base( 0x161D )
        {
            Hue = BaseChallengeGame.TeamColor( team );
            Name = String.Format( "Team {0} Flag", team );
            HomeBase = homebase;
        }

        public CTFFlag( Serial serial )
            : base( serial )
        {
        }

        private static Mobile FindOwner( object parent )
        {
            if( parent is Item )
                return ( (Item)parent ).RootParent as Mobile;

            if( parent is Mobile )
                return (Mobile)parent;

            return null;
        }

        public override void OnAdded( object parent )
        {
            base.OnAdded( parent );

            Mobile mob = FindOwner( parent );

            if( mob != null )
                mob.SolidHueOverride = OwnershipHue;
        }

        public override void OnRemoved( object parent )
        {
            base.OnRemoved( parent );

            Mobile mob = FindOwner( parent );

            if( mob != null )
                mob.SolidHueOverride = -1;
        }

        public override bool OnDroppedInto( Mobile from, Container target, Point3D p )
        {
            // allow movement within a players backpack
            if( from != null && from.Backpack == target )
            {
                return base.OnDroppedInto( from, target, p );
            }

            return false;
        }

        public override bool OnDroppedOnto( Mobile from, Item target )
        {
            return false;
        }

        public override bool OnDroppedToMobile( Mobile from, Mobile target )
        {
            return false;
        }

        public override bool CheckLift( Mobile from, Item item, ref LRReason reject )
        {
            // only allow staff to pick it up when at a base
            if( ( from != null && from.AccessLevel > AccessLevel.Player ) || RootParent != null )
            {
                return base.CheckLift( from, item, ref reject );
            }
            return false;
        }

        public override bool OnDroppedToWorld( Mobile from, Point3D point )
        {
            return false;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( HomeBase );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            HomeBase = reader.ReadItem() as CTFBase;
        }
    }
}