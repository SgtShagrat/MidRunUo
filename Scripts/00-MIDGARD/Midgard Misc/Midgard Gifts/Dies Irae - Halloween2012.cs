/***************************************************************************
 *                               Dies Irae - Halloween2012.cs
 *
 *   begin                : 1 novembre 2012
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Items;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Network;

namespace Midgard.Misc
{
    public class Halloween2012 : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register( new Halloween2012() );
        }

        public override TimeSpan MinimumAge
        {
            get { return TimeSpan.Zero; }
        }

        public override DateTime Start
        {
            get { return new DateTime( 2012, 11, 1, 13, 0, 0 ); }
        }

        public override DateTime Finish
        {
            get { return new DateTime( 2012, 11, 1, 23, 0, 0 ); }
        }

        public override void GiveGift( Mobile mob )
        {
            GiftBox box = new GiftBox();
            box.Name = "Halloween 2012";
            box.Hue = Utility.RandomList( new int[] { 1175, 1108, 1109, 1997 } );

            box.DropItem( new HangingSkeletonDeed() );
            box.DropItem( new HalloweenPumpkin2012() );

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
    public class HalloweenPumpkin2012 : Food
    {
        public override string DefaultName
        {
            get { return "a putrified pumpkin"; }
        }

        [Constructable]
        public HalloweenPumpkin2012()
            : this( 1 )
        {
        }

        [Constructable]
        public HalloweenPumpkin2012( int amount )
            : base( amount, 0xC6A )
        {
            Weight = 1.0;
            FillFactor = 8;
        }

        #region serialization
        public HalloweenPumpkin2012( Serial serial )
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

    public class HangingSkeleton : Item, IAddon
    {
        public override int LabelNumber { get { return 1065534; } } // hanging skeleton

        [Constructable]
        public HangingSkeleton()
            : this( 0x1A02 )
        {
        }

        [Constructable]
        public HangingSkeleton( int itemID )
            : base( itemID )
        {
            Movable = false;
        }

        public HangingSkeleton( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }

        public Item Deed
        {
            get
            {
                HangingSkeletonDeed deed = new HangingSkeletonDeed();
                return deed;
            }
        }

        public bool CouldFit( IPoint3D p, Map map )
        {
            if( !map.CanFit( p.X, p.Y, p.Z, ItemData.Height ) )
                return false;

            if( ItemID == 0x1A02 || ItemID == 0x1A03 || ItemID == 0x1A05 )
                return BaseAddon.IsWall( p.X, p.Y - 1, p.Z, map ); // North wall
            else
                return BaseAddon.IsWall( p.X - 1, p.Y, p.Z, map ); // West wall
        }

        public override void OnDoubleClick( Mobile from )
        {
            BaseHouse house = BaseHouse.FindHouseAt( this );

            if( house != null && house.IsOwner( from ) )
            {
                if( from.InRange( GetWorldLocation(), 3 ) )
                {
                    from.CloseGump( typeof( DecoRedeedGump ) );
                    from.SendGump( new DecoRedeedGump( this ) );
                }
                else
                {
                    from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
                }
            }
        }
    }

    public class HangingSkeletonDeed : Item, IAddonTargetDeed
    {
        public override int LabelNumber { get { return 1065535; } } // deed for a hanging skeleton

        [Constructable]
        public HangingSkeletonDeed()
            : base( 0x14F0 )
        {
            LootType = LootType.Blessed;
        }

        public HangingSkeletonDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsChildOf( from.Backpack ) )
            {
                BaseHouse house = BaseHouse.FindHouseAt( from );

                if( house != null && house.IsOwner( from ) )
                {
                    from.CloseGump( typeof( ChooseDecoGump ) );
                    from.SendGump( new ChooseDecoGump( this, 0, 0x1A01, 0x1A05, "Hanging Skeleton" ) );
                }
                else
                {
                    from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
                }
            }
            else
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
        }

        public int TargetLocalized { get { return 1049780; } } // Where would you like to place this decoration?

        public void Placement_OnTarget( Mobile from, object targeted, object state )
        {
            if( !IsChildOf( from.Backpack ) )
                return;

            IPoint3D p = targeted as IPoint3D;

            if( p == null )
                return;

            Point3D loc = new Point3D( p );
            if( !from.Map.CanFit( loc.X, loc.Y, loc.Z, 1 ) )
                return;

            BaseHouse house = BaseHouse.FindHouseAt( loc, from.Map, 16 );

            if( house != null && house.IsOwner( from ) )
            {
                int itemID = (int)state;
                if( ( itemID == 0x1A02 || itemID == 0x1A03 || itemID == 0x1A05 ) ? BaseAddon.IsWall( loc.X, loc.Y - 1, loc.Z, from.Map ) : BaseAddon.IsWall( p.X - 1, p.Y, p.Z, from.Map ) )
                {
                    HangingSkeleton skeleton = new HangingSkeleton( itemID );
                    skeleton.MoveToWorld( loc, from.Map );
                    house.Addons.Add( skeleton );
                    Delete();
                }
                else
                    from.SendLocalizedMessage( 1062840 ); // The decoration must be placed next to a wall.
            }
            else
            {
                from.SendLocalizedMessage( 1042036 ); // That location is not in your house.
            }
        }
    }
}