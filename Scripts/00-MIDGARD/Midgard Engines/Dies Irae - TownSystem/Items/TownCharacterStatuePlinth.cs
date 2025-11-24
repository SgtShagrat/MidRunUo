using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownCharacterStatuePlinth : CharacterStatuePlinth
    {
        public TownCharacterStatuePlinth( CharacterStatue statue )
            : base( statue )
        {
        }

        public TownCharacterStatuePlinth( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( Statue != null && from == Statue.SculptedBy )
                from.SendGump( new TownCharacterStatueGump( Statue ) );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
    }
}