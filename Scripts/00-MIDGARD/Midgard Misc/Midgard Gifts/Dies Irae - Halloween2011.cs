/***************************************************************************
 *                               Dies Irae - Halloween2011.cs
 *
 *   begin                : 31 ottobre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Items;

using Server;
using Server.Items;
using Server.Misc;

namespace Midgard.Misc
{
    public class Halloween2011 : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register( new Halloween2011() );
        }

        public override TimeSpan MinimumAge
        {
            get { return TimeSpan.Zero; }
        }

        public override DateTime Start
        {
            get { return new DateTime( 2011, 10, 31, 12, 0, 0 ); }
        }

        public override DateTime Finish
        {
            get { return new DateTime( 2011, 11, 1, 8, 0, 0 ); }
        }

        private static readonly Type[] m_Portraits = new Type[]
                                                         {
                                                             typeof( CreepyPortraitDeed ),
                                                             typeof( DisturbingPortraitDeed ),
                                                             typeof( UnsettlingPortraitDeed ) 
                                                         };

        private static readonly Type[] m_Pixies = new Type[]
                                                      {
                                                          typeof( MountedPixieBlueDeed ),
                                                          typeof( MountedPixieGreenDeed ),
                                                          typeof( MountedPixieLimeDeed ),
                                                          typeof( MountedPixieOrangeDeed ) 
                                                      };

        public override void GiveGift( Mobile mob )
        {
            GiftBox box = new GiftBox();
            box.Name = "Halloween 2011";
            box.Hue = Utility.RandomList( new int[] { 1175, 1108, 1109, 1997 } );

            box.DropItem( Loot.Construct( m_Portraits ) );
            box.DropItem( Loot.Construct( m_Pixies ) );
            box.DropItem( new HalloweenPumpkin2011() );

            switch( GiveGift( mob, box ) )
            {
                case GiftResult.Backpack:
                    mob.SendMessage( 0x482, "Buahhahaha! Buon halloween dallo staff di Midgard! Cerca un piccolo regalo nel tuo zaino." );
                    break;
                case GiftResult.BankBox:
                    mob.SendMessage( 0x482, "Buahhahaha! Buon halloween dallo staff di Midgard! Cerca un piccolo regalo nella tua banca." );
                    break;
            }
        }
    }
}

namespace Midgard.Items
{
    [FlipableAttribute( 0xC6A, 0xC6B )]
    public class HalloweenPumpkin2011 : Food
    {
        public override string DefaultName
        {
            get { return "a monstrous pumpkin"; }
        }

        [Constructable]
        public HalloweenPumpkin2011()
            : this( 1 )
        {
        }

        [Constructable]
        public HalloweenPumpkin2011( int amount )
            : base( amount, 0xC6A )
        {
            Weight = 1.0;
            FillFactor = 8;
        }

        #region serialization
        public HalloweenPumpkin2011( Serial serial )
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
}