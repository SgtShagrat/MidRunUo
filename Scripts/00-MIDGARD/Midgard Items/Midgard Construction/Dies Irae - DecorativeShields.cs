/***************************************************************************
 *                                  Dies Irae - DecoItems.cs
 *                            		-------------------
 *  begin                	: Giugno, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 			Serie di items da decoro:
 * 				CraftableDecorativeShield1
 * 				CraftableDecorativeShield2
 * 				CraftableDecorativeShield3
 * 				CraftableDecorativeShield4
 * 				CraftableDecorativeShield5
 * 				CraftableDecorativeShield6
 * 				CraftableDecorativeShield7
 * 				CraftableDecorativeShield8
 * 				CraftableDecorativeShield9
 * 				CraftableDecorativeShield10
 * 				CraftableDecorativeShield11
 * 				CraftableDecorativeShieldSword1North
 * 				CraftableDecorativeShieldSword1West
 * 				CraftableDecorativeShieldSword2North
 * 				CraftableDecorativeShieldSword2West
 * 
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    [Flipable( 0x156C, 0x156D )]
    public class CraftableDecorativeShield1 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064937; } // decorative shield
        }

        [Constructable]
        public CraftableDecorativeShield1()
            : base( 0x156C )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield1( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x156E, 0x156F )]
    public class CraftableDecorativeShield2 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064938; } // decorative shield
        }

        [Constructable]
        public CraftableDecorativeShield2()
            : base( 0x156E )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield2( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x1570, 0x1571 )]
    public class CraftableDecorativeShield3 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064939; } // decorative shield
        }

        [Constructable]
        public CraftableDecorativeShield3()
            : base( 0x1570 )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield3( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x1572, 0x1573 )]
    public class CraftableDecorativeShield4 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064940; } // decorative shield
        }

        [Constructable]
        public CraftableDecorativeShield4()
            : base( 0x1572 )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield4( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x1574, 0x1575 )]
    public class CraftableDecorativeShield5 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064941; } // decorative shield
        }

        [Constructable]
        public CraftableDecorativeShield5()
            : base( 0x1574 )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield5( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x1576, 0x1577 )]
    public class CraftableDecorativeShield6 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064942; } // decorative shield
        }

        [Constructable]
        public CraftableDecorativeShield6()
            : base( 0x1576 )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield6( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x1578, 0x1579 )]
    public class CraftableDecorativeShield7 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064943; } // decorative shield
        }

        [Constructable]
        public CraftableDecorativeShield7()
            : base( 0x1578 )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield7( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x157A, 0x157B )]
    public class CraftableDecorativeShield8 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064944; } // decorative shield
        }

        [Constructable]
        public CraftableDecorativeShield8()
            : base( 0x157A )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield8( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x157C, 0x157D )]
    public class CraftableDecorativeShield9 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064945; } // decorative shield
        }

        [Constructable]
        public CraftableDecorativeShield9()
            : base( 0x157C )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield9( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x157E, 0x157F )]
    public class CraftableDecorativeShield10 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064946; } // decorative shield
        }

        [Constructable]
        public CraftableDecorativeShield10()
            : base( 0x157E )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield10( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x1580, 0x1581 )]
    public class CraftableDecorativeShield11 : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064947; } // decorative shield and weapon
        }

        [Constructable]
        public CraftableDecorativeShield11()
            : base( 0x1580 )
        {
            Weight = 5;
        }

        public CraftableDecorativeShield11( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x1582, 0x1583, 0x1634, 0x1635 )]
    public class CraftableDecorativeShieldSword1North : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064948; } // decorative shield and weapon
        }

        [Constructable]
        public CraftableDecorativeShieldSword1North()
            : base( 0x1582 )
        {
            Weight = 5;
        }

        public CraftableDecorativeShieldSword1North( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x1634, 0x1635, 0x1582, 0x1583 )]
    public class CraftableDecorativeShieldSword1West : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064949; } // decorative shield and weapon
        }

        [Constructable]
        public CraftableDecorativeShieldSword1West()
            : base( 0x1634 )
        {
            Weight = 5;
        }

        public CraftableDecorativeShieldSword1West( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x1584, 0x1585, 0x1636, 0x1637 )]
    public class CraftableDecorativeShieldSword2North : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064950; } // decorative shield and weapon
        }

        [Constructable]
        public CraftableDecorativeShieldSword2North()
            : base( 0x1584 )
        {
            Weight = 5;
        }

        public CraftableDecorativeShieldSword2North( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Flipable( 0x1636, 0x1637, 0x1584, 0x1585 )]
    public class CraftableDecorativeShieldSword2West : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064951; } // decorative shield and weapon
        }

        [Constructable]
        public CraftableDecorativeShieldSword2West()
            : base( 0x1636 )
        {
            Weight = 5;
        }

        public CraftableDecorativeShieldSword2West( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}