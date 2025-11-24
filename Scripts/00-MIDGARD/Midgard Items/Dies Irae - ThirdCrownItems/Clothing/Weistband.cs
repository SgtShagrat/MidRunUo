using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3365 Weistband trad. cintura a fascie - ( nel vendor provisoner , colorabile da hue )
    /// </summary>
    public class Weistband : BaseWaist
    {
        public override string DefaultName { get { return "weistband"; } }

        [Constructable]
        public Weistband()
            : this( 0 )
        {
        }

        [Constructable]
        public Weistband( int hue )
            : base( 0x3365, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public Weistband( Serial serial )
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