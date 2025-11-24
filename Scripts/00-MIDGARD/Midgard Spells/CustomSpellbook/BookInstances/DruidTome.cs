using Server;

namespace Midgard.Engines.SpellSystem
{
    public class DruidTome : CustomSpellbook
    {
        public override Classes.Classes BookClass
        {
            get { return Classes.Classes.Druid; }
        }

        public override SchoolInfo MainBookSchool
        {
            get { return SchoolInfo.DruidSchoolInfo; }
        }

        public override string DefaultName
        {
            get { return "Druid Staff"; }
        }

        [Constructable]
        public DruidTome()
            : base( 0x256C )
        {
            Layer = Layer.TwoHanded;
        }

        #region serialization
        public DruidTome( Serial serial )
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