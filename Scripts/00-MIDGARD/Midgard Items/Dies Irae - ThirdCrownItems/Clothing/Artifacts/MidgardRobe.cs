using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class MidgardRobe : Robe, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Midgard Robe"; } }

        public override int ArtifactRarity { get { return 10; } }

        [Constructable]
        public MidgardRobe()
        {
            Weight = 3.0;
            Hue = 0x48D;
        }

        public override void OnAdded( object parent )
        {
            base.OnAdded( parent );

            if( parent is Mobile )
                ( (Mobile)parent ).VirtualArmorMod += 30;
        }

        public override void OnRemoved( object parent )
        {
            base.OnRemoved( parent );

            if( parent is Mobile )
                ( (Mobile)parent ).VirtualArmorMod -= 30;
        }

        public override bool Dye( Mobile from, DyeTub sender )
        {
            from.SendLocalizedMessage( sender.FailMessage );
            return false;
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Armor bonus: 30\n" );
        }

        #region serialization
        public MidgardRobe( Serial serial )
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

            if( Parent is Mobile )
                ( (Mobile)Parent ).VirtualArmorMod += 30;
        }
        #endregion
    }
}