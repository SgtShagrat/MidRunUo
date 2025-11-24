using Server.Engines.XmlSpawner2;
using Server.Network;

namespace Server.Engines.XmlPoints
{
    public class DeathBall : Item
    {
        public const int OwnershipHue = 0xB;

        [Constructable]
        public DeathBall()
            : base( 0x2257 )
        {
            Hue = 1289;
            Name = "DeathBall";
            LootType = LootType.Cursed;
        }

        public DeathBall( Serial serial )
            : base( serial )
        {
        }

        public override bool CheckLift( Mobile from, Item item, ref LRReason reject )
        {
            // allow staff to pick it up
            if( from != null && from.AccessLevel > AccessLevel.Player )
            {
                return base.CheckLift( from, item, ref reject );
            }

            // prevent non-participants from picking it up
            var afrom = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );
            if( afrom != null && afrom.ChallengeGame != null &&
                ( ( afrom.ChallengeGame is DeathBallGauntlet && ( ( (DeathBallGauntlet)( afrom.ChallengeGame ) ).Ball == item ) ) ||
                 ( afrom.ChallengeGame is TeamDeathballGauntlet &&
                  ( ( (TeamDeathballGauntlet)( afrom.ChallengeGame ) ).Ball == item ) ) ) )
            {
                return base.CheckLift( from, item, ref reject );
            }
            else
                return false;
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