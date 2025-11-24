/***************************************************************************
 *                               FillableHouseDeed.cs
 *
 *   begin                : 23 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Text;

using Server;
using Server.Items;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Targeting;
using Midgard.Items;

namespace Midgard.Multis.Deeds
{
    public abstract class FillableHouseDeed : HouseDeed
    {
        public const int IronGoldValue = 10;
        public const int WoodGoldValue = 6;
        public const int StoneGoldValue = 10;

        protected FillableHouseDeed( int id, Point3D offset )
            : base( id, offset )
        {
            Iron = Wood = Stone = 0;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Iron { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Wood { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Stone { get; set; }

        public virtual int IronReq
        {
            get { return (int) ( ( Price * IronPerc * 0.01 ) / IronGoldValue ); }
        }

        public virtual int StoneReq
        {
            get { return (int)( ( Price * StonePerc * 0.01 ) / StoneGoldValue ); }
        }

        public virtual int WoodReq
        {
            get { return (int)( ( Price * WoodPerc * 0.01 ) / WoodGoldValue ); }
        }

        public abstract int Price{ get;}

        public abstract int IronPerc { get; }

        public abstract int StonePerc { get; }

        public abstract int WoodPerc { get; }

        public bool Filled
        {
            get
            {
                if( IronReq > 0 && Iron < IronReq )
                    return false;
                else if( StoneReq > 0 && Stone < StoneReq )
                    return false;
                else if( WoodReq > 0 && Wood < WoodReq )
                    return false;
                else
                    return true;
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
            else if( !Filled )
            {
                from.SendMessage( "Which material you want to fill this deed with?" );
                from.Target = new InternalTarget( this );
                return;
            }

            base.OnDoubleClick( from );
        }

        private static bool IsIron( Item i )
        {
            if( i == null )
                return false;

            Type t = i.GetType();
            return t == typeof( BaseIngot ) || t.IsSubclassOf( typeof( BaseIngot ) );
        }

        private static bool IsWood( Item i )
        {
            if( i == null )
                return false;

            Type t = i.GetType();
            return t == typeof( BaseBoards ) || t.IsSubclassOf( typeof( BaseBoards ) ) || t == typeof( BaseLog ) || t.IsSubclassOf( typeof( BaseLog ) );
        }

        private static bool IsStone( Item i )
        {
            if( i == null )
                return false;

            Type t = i.GetType();
            return t == typeof( BaseGranite ) || t.IsSubclassOf( typeof( BaseGranite ) ) || t == typeof( RawStone );
        }

        public virtual bool TryFulfill( Mobile m, Item i )
        {
            if( Filled )
                return false;

            int amount = 0;
            if( IronReq > 0 && Iron < IronReq && IsIron( i ) )
            {
                amount = Math.Min( IronReq - Iron, i.Amount );
                i.Consume( amount );
                Iron += amount;
                m.SendMessage( "You added {0} {1} to the deed.", amount, "ores" );
                return true;
            }
            else if( StoneReq > 0 && Stone < StoneReq && IsStone( i ) )
            {
                amount = Math.Min( StoneReq - Stone, i.Amount );
                i.Consume( amount );
                Stone += amount;
                m.SendMessage( "You added {0} {1} to the deed.", amount, "stones" );
                return true;
            }
            else if( WoodReq > 0 && Wood < WoodReq && IsWood( i ) )
            {
                amount = Math.Min( WoodReq - Wood, i.Amount );
                i.Consume( amount );
                Wood += amount;
                m.SendMessage( "You added {0} {1} to the deed.", amount, "logs" );
                return true;
            }

            return false;
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            StringBuilder sb = new StringBuilder();
            if( IronReq > 0 )
                sb.AppendFormat( "Metal: {0}/{1}", Iron, IronReq );
            if( IronReq > 0 && StoneReq > 0 )
                sb.Append( " " );
            if( StoneReq > 0 )
                sb.AppendFormat( "Stone: {0}/{1}", Stone, StoneReq );
            if( StoneReq > 0 && WoodReq > 0 )
                sb.Append( " " );
            if( WoodReq > 0 )
                sb.AppendFormat( "Wood: {0}/{1}", Wood, WoodReq );

            LabelTo( from, sb.ToString() );
        }

        #region serialization
        public FillableHouseDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( Iron );
            writer.Write( Stone );
            writer.Write( Wood );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Iron = reader.ReadInt();
            Stone = reader.ReadInt();
            Wood = reader.ReadInt();
        }
        #endregion

        private class InternalTarget : Target
        {
            private FillableHouseDeed m_Deed;

            public InternalTarget( FillableHouseDeed deed )
                : base( 2, false, TargetFlags.None )
            {
                m_Deed = deed;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( m_Deed.TryFulfill( from, o as Item ) )
                    from.SendMessage( "The requirement has been fulfilled." );
                else
                    from.SendMessage( "The requirement could not be fulfilled." );
            }
        }
    }

    public class FillableStonePlasterHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableStonePlasterHouseDeed()
            : base( 0x64, new Point3D( 0, 4, 0 ) )
        {
        }

        public FillableStonePlasterHouseDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041211; }
        }

        public override Rectangle2D[] Area
        {
            get { return SmallOldHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new SmallOldHouse( owner, 0x64 );
        }

        public override int Price
        {
            get { return 43800; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 60; }
        }

        public override int WoodPerc
        {
            get { return 30; }
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
    }

    public class FillableFieldStoneHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableFieldStoneHouseDeed()
            : base( 0x66, new Point3D( 0, 4, 0 ) )
        {
        }

        public FillableFieldStoneHouseDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041212; }
        }

        public override Rectangle2D[] Area
        {
            get { return SmallOldHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new SmallOldHouse( owner, 0x66 );
        }

        public override int Price
        {
            get { return 43800; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 50; }
        }

        public override int WoodPerc
        {
            get { return 40; }
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
    }

    public class FillableSmallBrickHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSmallBrickHouseDeed()
            : base( 0x68, new Point3D( 0, 4, 0 ) )
        {
        }

        public FillableSmallBrickHouseDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041213; }
        }

        public override Rectangle2D[] Area
        {
            get { return SmallOldHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new SmallOldHouse( owner, 0x68 );
        }

        public override int Price
        {
            get { return 43800; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 70; }
        }

        public override int WoodPerc
        {
            get { return 20; }
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
    }

    public class FillableWoodHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableWoodHouseDeed()
            : base( 0x6A, new Point3D( 0, 4, 0 ) )
        {
        }

        public FillableWoodHouseDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041214; }
        }

        public override Rectangle2D[] Area
        {
            get { return SmallOldHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new SmallOldHouse( owner, 0x6A );
        }

        public override int Price
        {
            get { return 43800; }
        }

        public override int IronPerc
        {
            get { return 20; }
        }

        public override int StonePerc
        {
            get { return 10; }
        }

        public override int WoodPerc
        {
            get { return 70; }
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
    }

    public class FillableWoodPlasterHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableWoodPlasterHouseDeed()
            : base( 0x6C, new Point3D( 0, 4, 0 ) )
        {
        }

        public FillableWoodPlasterHouseDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041215; }
        }

        public override Rectangle2D[] Area
        {
            get { return SmallOldHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new SmallOldHouse( owner, 0x6C );
        }

        public override int Price
        {
            get { return 43800; }
        }

        public override int IronPerc
        {
            get { return 30; }
        }

        public override int StonePerc
        {
            get { return 30; }
        }

        public override int WoodPerc
        {
            get { return 40; }
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
    }

    public class FillableThatchedRoofCottageDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableThatchedRoofCottageDeed()
            : base( 0x6E, new Point3D( 0, 4, 0 ) )
        {
        }

        public FillableThatchedRoofCottageDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041216; }
        }

        public override Rectangle2D[] Area
        {
            get { return SmallOldHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new SmallOldHouse( owner, 0x6E );
        }

        public override int Price
        {
            get { return 43800; }
        }

        public override int IronPerc
        {
            get { return 20; }
        }

        public override int StonePerc
        {
            get { return 40; }
        }

        public override int WoodPerc
        {
            get { return 40; }
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
    }

    public class FillableBrickHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableBrickHouseDeed()
            : base( 0x74, new Point3D( -1, 7, 0 ) )
        {
        }

        public FillableBrickHouseDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041219; }
        }

        public override Rectangle2D[] Area
        {
            get { return GuildHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new GuildHouse( owner );
        }

        public override int Price
        {
            get { return 144500; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 70; }
        }

        public override int WoodPerc
        {
            get { return 20; }
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
    }

    public class FillableTwoStoryWoodPlasterHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableTwoStoryWoodPlasterHouseDeed()
            : base( 0x76, new Point3D( -3, 7, 0 ) )
        {
        }

        public FillableTwoStoryWoodPlasterHouseDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041220; }
        }

        public override Rectangle2D[] Area
        {
            get { return TwoStoryHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new TwoStoryHouse( owner, 0x76 );
        }

        public override int Price
        {
            get { return 192400; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 20; }
        }

        public override int WoodPerc
        {
            get { return 70; }
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
    }

    public class FillableTwoStoryStonePlasterHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableTwoStoryStonePlasterHouseDeed()
            : base( 0x78, new Point3D( -3, 7, 0 ) )
        {
        }

        public FillableTwoStoryStonePlasterHouseDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041221; }
        }

        public override Rectangle2D[] Area
        {
            get { return TwoStoryHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new TwoStoryHouse( owner, 0x78 );
        }

        public override int Price
        {
            get { return 192400; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 70; }
        }

        public override int WoodPerc
        {
            get { return 20; }
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
    }

    public class FillableTowerDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableTowerDeed()
            : base( 0x7A, new Point3D( 0, 7, 0 ) )
        {
        }

        public FillableTowerDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041222; }
        }

        public override Rectangle2D[] Area
        {
            get { return Tower.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new Tower( owner );
        }

        public override int Price
        {
            get { return 433200; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 70; }
        }

        public override int WoodPerc
        {
            get { return 20; }
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
    }

    public class FillableKeepDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableKeepDeed()
            : base( 0x7C, new Point3D( 0, 11, 0 ) )
        {
        }

        public FillableKeepDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041223; }
        }

        public override Rectangle2D[] Area
        {
            get { return Keep.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new Keep( owner );
        }

        public override int Price
        {
            get { return 665200; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 70; }
        }

        public override int WoodPerc
        {
            get { return 20; }
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
    }

    public class FillableCastleDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableCastleDeed()
            : base( 0x7E, new Point3D( 0, 16, 0 ) )
        {
        }

        public FillableCastleDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041224; }
        }

        public override Rectangle2D[] Area
        {
            get { return Castle.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new Castle( owner );
        }

        public override int Price
        {
            get { return 1022800; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 60; }
        }

        public override int WoodPerc
        {
            get { return 30; }
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
    }

    public class FillableLargePatioDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableLargePatioDeed()
            : base( 0x8C, new Point3D( -4, 7, 0 ) )
        {
        }

        public FillableLargePatioDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041231; }
        }

        public override Rectangle2D[] Area
        {
            get { return LargePatioHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new LargePatioHouse( owner );
        }

        public override int Price
        {
            get { return 152800; }
        }

        public override int IronPerc
        {
            get { return 20; }
        }

        public override int StonePerc
        {
            get { return 40; }
        }

        public override int WoodPerc
        {
            get { return 40; }
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
    }

    public class FillableLargeMarbleDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableLargeMarbleDeed()
            : base( 0x96, new Point3D( -4, 7, 0 ) )
        {
        }

        public FillableLargeMarbleDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041236; }
        }

        public override Rectangle2D[] Area
        {
            get { return LargeMarbleHouse.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new LargeMarbleHouse( owner );
        }

        public override int Price
        {
            get { return 192000; }
        }

        public override int IronPerc
        {
            get { return 30; }
        }

        public override int StonePerc
        {
            get { return 60; }
        }

        public override int WoodPerc
        {
            get { return 10; }
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
    }

    public class FillableSmallTowerDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSmallTowerDeed()
            : base( 0x98, new Point3D( 3, 4, 0 ) )
        {
        }

        public FillableSmallTowerDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041237; }
        }

        public override Rectangle2D[] Area
        {
            get { return SmallTower.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new SmallTower( owner );
        }

        public override int Price
        {
            get { return 88500; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 60; }
        }

        public override int WoodPerc
        {
            get { return 30; }
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
    }

    public class FillableLogCabinDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableLogCabinDeed()
            : base( 0x9A, new Point3D( 1, 6, 0 ) )
        {
        }

        public FillableLogCabinDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041238; }
        }

        public override Rectangle2D[] Area
        {
            get { return LogCabin.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new LogCabin( owner );
        }

        public override int Price
        {
            get { return 97800; }
        }

        public override int IronPerc
        {
            get { return 30; }
        }

        public override int StonePerc
        {
            get { return 10; }
        }

        public override int WoodPerc
        {
            get { return 60; }
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
    }

    public class FillableSandstonePatioDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstonePatioDeed()
            : base( 0x9C, new Point3D( -1, 4, 0 ) )
        {
        }

        public FillableSandstonePatioDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041239; }
        }

        public override Rectangle2D[] Area
        {
            get { return SandStonePatio.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new SandStonePatio( owner );
        }

        public override int Price
        {
            get { return 90900; }
        }

        public override int IronPerc
        {
            get { return 20; }
        }

        public override int StonePerc
        {
            get { return 50; }
        }

        public override int WoodPerc
        {
            get { return 30; }
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
    }

    public class FillableVillaDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableVillaDeed()
            : base( 0x9E, new Point3D( 3, 6, 0 ) )
        {
        }

        public FillableVillaDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041240; }
        }

        public override Rectangle2D[] Area
        {
            get { return TwoStoryVilla.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new TwoStoryVilla( owner );
        }

        public override int Price
        {
            get { return 136500; }
        }

        public override int IronPerc
        {
            get { return 30; }
        }

        public override int StonePerc
        {
            get { return 40; }
        }

        public override int WoodPerc
        {
            get { return 30; }
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
    }

    public class FillableStoneWorkshopDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableStoneWorkshopDeed()
            : base( 0xA0, new Point3D( -1, 4, 0 ) )
        {
        }

        public FillableStoneWorkshopDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041241; }
        }

        public override Rectangle2D[] Area
        {
            get { return SmallShop.AreaArray2; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new SmallShop( owner, 0xA0 );
        }

        public override int Price
        {
            get { return 60600; }
        }

        public override int IronPerc
        {
            get { return 20; }
        }

        public override int StonePerc
        {
            get { return 60; }
        }

        public override int WoodPerc
        {
            get { return 20; }
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
    }

    public class FillableMarbleWorkshopDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableMarbleWorkshopDeed()
            : base( 0xA2, new Point3D( -1, 4, 0 ) )
        {
        }

        public FillableMarbleWorkshopDeed( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1041242; }
        }

        public override Rectangle2D[] Area
        {
            get { return SmallShop.AreaArray1; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new SmallShop( owner, 0xA2 );
        }

        public override int Price
        {
            get { return 63000; }
        }

        public override int IronPerc
        {
            get { return 10; }
        }

        public override int StonePerc
        {
            get { return 60; }
        }

        public override int WoodPerc
        {
            get { return 30; }
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
    }
}