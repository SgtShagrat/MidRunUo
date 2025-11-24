/***************************************************************************
 *                                 Pillows.cs
 *                            	  ------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BasePillow : Item, IDyable
    {
        public override string DefaultName { get { return "a pillow"; } }

        public BasePillow( int itemID )
            : base( itemID )
        {
            Weight = 1.0;
        }

        public BasePillow( Serial serial )
            : base( serial )
        {
        }

        public bool Dye( Mobile from, DyeTub sender )
        {
            if( Deleted )
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        #region serial-deserial
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

    [Flipable( 0x13A4, 0x13A5 )]
    public class Pillow1 : BasePillow
    {
        [Constructable]
        public Pillow1()
            : base( 0x13A4 )
        {
        }

        public Pillow1( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    public class Pillow2 : BasePillow
    {
        [Constructable]
        public Pillow2()
            : base( 0x13A6 )
        {
        }

        public Pillow2( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    public class Pillow3 : BasePillow
    {
        [Constructable]
        public Pillow3()
            : base( 0x13A7 )
        {
        }

        public Pillow3( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    public class Pillow4 : BasePillow
    {
        [Constructable]
        public Pillow4()
            : base( 0x13A8 )
        {
        }

        public Pillow4( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    [Flipable( 0x13A9, 0x13AA )]
    public class Pillow5 : BasePillow
    {
        [Constructable]
        public Pillow5()
            : base( 0x13A9 )
        {
        }

        public Pillow5( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    public class Pillow6 : BasePillow
    {
        [Constructable]
        public Pillow6()
            : base( 0x13AB )
        {
        }

        public Pillow6( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    public class Pillow7 : BasePillow
    {
        [Constructable]
        public Pillow7()
            : base( 0x13AC )
        {
        }

        public Pillow7( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    [Flipable( 0x13AD, 0x13AE )]
    public class Pillow8 : BasePillow
    {
        #region costruttori
        [Constructable]
        public Pillow8()
            : base( 0x13AD )
        {
        }

        public Pillow8( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region serial-deserial
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

    public class Pillow12 : BasePillow
    {
        [Constructable]
        public Pillow12()
            : base( 0x163A )
        {
        }

        public Pillow12( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    public class Pillow13 : BasePillow
    {
        [Constructable]
        public Pillow13()
            : base( 0x163B )
        {
        }

        public Pillow13( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    public class Pillow14 : BasePillow
    {
        [Constructable]
        public Pillow14()
            : base( 0x163C )
        {
        }

        public Pillow14( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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
}