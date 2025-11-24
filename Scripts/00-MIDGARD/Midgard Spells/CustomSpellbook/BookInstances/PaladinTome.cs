using Server;

namespace Midgard.Engines.SpellSystem
{
    public class PaladinTome : CustomSpellbook
    {
        public override Classes.Classes BookClass
        {
            get { return Classes.Classes.Paladin; }
        }

        public override SchoolInfo MainBookSchool
        {
            get { return SchoolInfo.PaladinSchoolInfo; }
        }

        public override string DefaultName
        {
            get { return "Paladin Tome"; }
        }

        [Constructable]
        public PaladinTome()
            : base( 0x2254 )
        {
        }

        #region serialization
        public PaladinTome( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); //version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
        #endregion
    }
}