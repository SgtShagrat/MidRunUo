/***************************************************************************
 *                               BaseDrow.cs
 *
 *   begin                : 03 luglio 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

using Core = Midgard.Engines.Races.Core;

namespace Midgard.Mobiles
{
    public abstract class BaseDrow : BaseCreature
    {
        protected BaseDrow( AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed )
            : base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
        {
            Female = Utility.RandomBool();
            Race = Core.Drow;
            SpeechHue = Utility.RandomDyedHue();
            Name = string.Format( "{0} {1}", Female ? NameList.RandomName( "drowMale" ) : NameList.RandomName( "drowFemale" ), NameList.RandomName( "drowSurname" ) );
        }

        #region serialization
        public BaseDrow( Serial serial )
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

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }
    }
}