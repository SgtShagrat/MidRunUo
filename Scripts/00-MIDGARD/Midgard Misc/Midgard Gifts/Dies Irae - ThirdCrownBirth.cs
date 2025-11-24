using System;

using Server;
using Server.Items;
using Server.Misc;
using Midgard.Items;

namespace Midgard.Misc
{
    public class ThirdCrownBirth : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register( new ThirdCrownBirth() );
        }

        public override DateTime Start { get { return new DateTime( 2010, 03, 14, 12, 40, 00 ); } }
        public override DateTime Finish { get { return new DateTime( 2010, 03, 15 ); } }
        public override TimeSpan MinimumAge{ get{ return TimeSpan.Zero; } }

        public override void GiveGift( Mobile mob )
        {
            switch( GiveGift( mob, new ThirdCrownBirthSash() ) )
            {
                case GiftResult.Backpack:
                case GiftResult.BankBox:
                    mob.SendMessage( 0x482, "Che questa fascia ti ricordi l'inizio di una nuova era, piena di avventure ed emozioni." );
                    break;
            }
        }
    }
}

namespace Midgard.Items
{
    public class ThirdCrownBirthSash : BodySash
    {
        [Constructable]
        public ThirdCrownBirthSash()
        {
            Hue = Utility.RandomList( 1952, 1763, 1933, 2589, 1082 );
            Weight = 1.0;

            LootType = LootType.Blessed;
        }

        public override void OnSingleClick( Mobile from )
        {
            string message = from.TrueLanguage == LanguageType.Ita ?
                "In memoria dell'inizio della Terza Era - 14 Marzo 2010" :
                "In memory of the birth of Midgard Third Crown Era - 14th April 2010";

            LabelTo( from, message );
        }

        #region serialization
        public ThirdCrownBirthSash( Serial serial )
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