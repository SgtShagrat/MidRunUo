/***************************************************************************
 *                                  MidgardPaintings.cs
 *                            		-------------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Quadri craftabili. Tutti flippabili tranne il MidgardPainting5
 *  TODO:
 * 			Implementare il crafting con penna e easel.
 * 			Da uppare tavolozza, colori, pennello e penna.
 * 
 ***************************************************************************/

using System;

using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BasePainting : Item, ICraftable
    {
        private Mobile m_Crafter;
        private string m_DedicatedTo;
        private string m_Dedication;

        public override int LabelNumber { get { return 1064577; } } // a painting

        [CommandProperty( AccessLevel.GameMaster )]
        public string DedicatedTo
        {
            get { return m_DedicatedTo; }
            set
            {
                m_DedicatedTo = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public string Dedication
        {
            get { return m_Dedication; }
            set
            {
                m_Dedication = value;
            }
        }

        #region costruttori
        public BasePainting( int basePaintingID )
            : base( basePaintingID )
        {
            Weight = 5;
        }

        public BasePainting( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region ICraftable Members

        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes,
                            BaseTool tool, CraftItem craftItem, int resHue )
        {
            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            Hue = 0;
            return quality;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }

        #endregion

        public override void OnSingleClick( Mobile from )
        {
            if( m_Crafter != null )
                LabelTo( from, 1064578, m_Crafter.Name ); // author: ~1_AUTHOR~
            if( m_DedicatedTo != null )
                LabelTo( from, 1064579, m_DedicatedTo ); // dedicated to: ~1_DEDTO~
            if( m_Dedication != null )
                LabelTo( from, 1064580, m_Dedication ); // dedication: ~1_DED~
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_Crafter );
            writer.Write( m_DedicatedTo );
            writer.Write( m_Dedication );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Crafter = reader.ReadMobile();
            m_DedicatedTo = reader.ReadString();
            m_Dedication = reader.ReadString();
        }
        #endregion
    }

    [Flipable( 0x2293, 0x2294 )]
    public class MidgardPainting1 : BasePainting
    {
        [Constructable]
        public MidgardPainting1()
            : base( 0x2293 )
        {
        }

        public MidgardPainting1( Serial serial )
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

    [Flipable( 0x2295, 0x2296 )]
    public class MidgardPainting2 : BasePainting
    {
        [Constructable]
        public MidgardPainting2()
            : base( 0x2295 )
        {
        }

        public MidgardPainting2( Serial serial )
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

    [Flipable( 0x2297, 0x2298 )]
    public class MidgardPainting3 : BasePainting
    {
        [Constructable]
        public MidgardPainting3()
            : base( 0x2297 )
        {
        }

        public MidgardPainting3( Serial serial )
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

    [Flipable( 0x2299, 0x229A )]
    public class MidgardPainting4 : BasePainting
    {
        [Constructable]
        public MidgardPainting4()
            : base( 0x2299 )
        {
        }

        public MidgardPainting4( Serial serial )
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

    public class MidgardPainting5 : BasePainting
    {
        [Constructable]
        public MidgardPainting5()
            : base( 0x229B )
        {
        }

        public MidgardPainting5( Serial serial )
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

    [Flipable( 0x229C, 0x229D )]
    public class MidgardPainting6 : BasePainting
    {
        [Constructable]
        public MidgardPainting6()
            : base( 0x229C )
        {
        }

        public MidgardPainting6( Serial serial )
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

    [Flipable( 0x229E, 0x229F )]
    public class MidgardPainting7 : BasePainting
    {
        [Constructable]
        public MidgardPainting7()
            : base( 0x229E )
        {
        }

        public MidgardPainting7( Serial serial )
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

    [Flipable( 0x22A0, 0x22A1 )]
    public class MidgardPainting8 : BasePainting
    {
        [Constructable]
        public MidgardPainting8()
            : base( 0x22A0 )
        {
        }

        public MidgardPainting8( Serial serial )
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

    [Flipable( 0x22A2, 0x22A3 )]
    public class MidgardPainting9 : BasePainting
    {
        [Constructable]
        public MidgardPainting9()
            : base( 0x22A2 )
        {
        }

        public MidgardPainting9( Serial serial )
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

    [Flipable( 0x22A4, 0x22A5 )]
    public class MidgardPainting10 : BasePainting
    {
        [Constructable]
        public MidgardPainting10()
            : base( 0x22A4 )
        {
        }

        public MidgardPainting10( Serial serial )
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

    [Flipable( 0x22A6, 0x22A7 )]
    public class MidgardPainting11 : BasePainting
    {
        [Constructable]
        public MidgardPainting11()
            : base( 0x22A6 )
        {
        }

        public MidgardPainting11( Serial serial )
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

    [Flipable( 0x22A8, 0x22A9 )]
    public class MidgardPainting12 : BasePainting
    {
        [Constructable]
        public MidgardPainting12()
            : base( 0x22A8 )
        {
        }

        public MidgardPainting12( Serial serial )
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

    [Flipable( 0x22AA, 0x22AB )]
    public class MidgardPainting13 : BasePainting
    {
        [Constructable]
        public MidgardPainting13()
            : base( 0x22AA )
        {
        }

        public MidgardPainting13( Serial serial )
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

    [Flipable( 0x22AC, 0x22AD )]
    public class MidgardPainting14 : BasePainting
    {
        [Constructable]
        public MidgardPainting14()
            : base( 0x22AC )
        {
        }

        public MidgardPainting14( Serial serial )
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

    [Flipable( 0x22AE, 0x22AF )]
    public class MidgardPainting15 : BasePainting
    {
        [Constructable]
        public MidgardPainting15()
            : base( 0x22AE )
        {
        }

        public MidgardPainting15( Serial serial )
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

    [Flipable( 0x22B0, 0x22B1 )]
    public class MidgardPainting16 : BasePainting
    {
        [Constructable]
        public MidgardPainting16()
            : base( 0x22B0 )
        {
        }

        public MidgardPainting16( Serial serial )
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

    [Flipable( 0x22B2, 0x22B3 )]
    public class MidgardPainting17 : BasePainting
    {
        [Constructable]
        public MidgardPainting17()
            : base( 0x22B2 )
        {
        }

        public MidgardPainting17( Serial serial )
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

    [Flipable( 0x22B4, 0x22B5 )]
    public class MidgardPainting18 : BasePainting
    {
        [Constructable]
        public MidgardPainting18()
            : base( 0x22B4 )
        {
        }

        public MidgardPainting18( Serial serial )
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

    [Flipable( 0x22B6, 0x22B7 )]
    public class MidgardPainting19 : BasePainting
    {
        [Constructable]
        public MidgardPainting19()
            : base( 0x22B6 )
        {
        }

        public MidgardPainting19( Serial serial )
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

    [Flipable( 0x22B8, 0x22B9 )]
    public class MidgardPainting20 : BasePainting
    {
        [Constructable]
        public MidgardPainting20()
            : base( 0x22B8 )
        {
        }

        public MidgardPainting20( Serial serial )
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

    [Flipable( 0x22BA, 0x22BB )]
    public class MidgardPainting21 : BasePainting
    {
        [Constructable]
        public MidgardPainting21()
            : base( 0x22BA )
        {
        }

        public MidgardPainting21( Serial serial )
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

    [Flipable( 0x22BC, 0x22BD )]
    public class MidgardPainting22 : BasePainting
    {
        [Constructable]
        public MidgardPainting22()
            : base( 0x22BC )
        {
        }

        public MidgardPainting22( Serial serial )
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

    [Flipable( 0x22BE, 0x22BF )]
    public class MidgardPainting23 : BasePainting
    {
        [Constructable]
        public MidgardPainting23()
            : base( 0x22BE )
        {
        }

        public MidgardPainting23( Serial serial )
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

    [Flipable( 0x22C0, 0x22C1 )]
    public class MidgardPainting24 : BasePainting
    {
        [Constructable]
        public MidgardPainting24()
            : base( 0x22C0 )
        {
        }

        public MidgardPainting24( Serial serial )
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

    [Flipable( 0x22C2, 0x22C3 )]
    public class MidgardPainting25 : BasePainting
    {
        [Constructable]
        public MidgardPainting25()
            : base( 0x22C2 )
        {
        }

        public MidgardPainting25( Serial serial )
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

    public class MidgardPainting26 : BasePainting
    {
        [Constructable]
        public MidgardPainting26()
            : base( 0x0EA0 )
        {
        }

        public MidgardPainting26( Serial serial )
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

    [Flipable( 0x0E9F, 0x0EC8 )]
    public class MidgardPainting27 : BasePainting
    {
        [Constructable]
        public MidgardPainting27()
            : base( 0x0E9F )
        {
        }

        public MidgardPainting27( Serial serial )
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

    [Flipable( 0x0EE7, 0x0EC9 )]
    public class MidgardPainting28 : BasePainting
    {
        [Constructable]
        public MidgardPainting28()
            : base( 0x0EE7 )
        {
        }

        public MidgardPainting28( Serial serial )
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

    [Flipable( 0x0EA2, 0x0EA1 )]
    public class MidgardPainting29 : BasePainting
    {
        [Constructable]
        public MidgardPainting29()
            : base( 0x0EA2 )
        {
        }

        public MidgardPainting29( Serial serial )
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

    [Flipable( 0x0EA6, 0x0EA5, 0x0EA7, 0x0EA8 )]
    public class MidgardPainting30 : BasePainting
    {
        [Constructable]
        public MidgardPainting30()
            : base( 0x0EA6 )
        {
        }

        public MidgardPainting30( Serial serial )
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