using Server;

namespace Midgard.Engines.SpellSystem
{
    public class NecromancerTome : CustomSpellbook
    {
        public override Classes.Classes BookClass
        {
            get { return Classes.Classes.Necromancer; }
        }

        public override SchoolInfo MainBookSchool
        {
            get { return SchoolInfo.NecromancerSchoolInfo; }
        }

        public override string DefaultName
        {
            get { return "Necronomicon"; }
        }

        [Constructable]
        public NecromancerTome()
            : base( 0x2253 )
        {
        }

        #region serialization
        public NecromancerTome( Serial serial )
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