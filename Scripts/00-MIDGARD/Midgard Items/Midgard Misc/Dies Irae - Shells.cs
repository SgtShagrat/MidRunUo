using Server;

namespace Midgard.Items
{
    public class CommonShell : Item
    {
        public override int LabelNumber
        {
            get { return 1074598; } // a shell
        }

        [Constructable]
        public CommonShell()
            : base( Utility.RandomList( 0xFC5, 0xFC6, 0xFC8, 0xFC9, 0xFCA, 0xFCB, 0xFCC ) )
        {
            Weight = 1;
        }

        #region serialization
        public CommonShell( Serial serial )
            : base( serial )
        {
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
        #endregion
    }

    public class ConchShell : Item
    {
        [Constructable]
        public ConchShell()
            : base( 0xFC4 )
        {
            Weight = 1;
        }

        #region serialization
        public ConchShell( Serial serial )
            : base( serial )
        {
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
        #endregion

        public override string DefaultName
        {
            get { return "a conch shell"; }
        }
    }

    public class NautilusShell : Item
    {
        [Constructable]
        public NautilusShell()
            : base( 0xFC7 )
        {
            Weight = 1;
        }

        #region serialization
        public NautilusShell( Serial serial )
            : base( serial )
        {
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
        #endregion

        public override string DefaultName
        {
            get { return "a nautilus shell"; }
        }
    }
}