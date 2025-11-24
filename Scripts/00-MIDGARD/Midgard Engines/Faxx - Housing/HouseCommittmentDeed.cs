/*using System;
using System.IO;
using Midgard.Items;
using Server.Multis;

namespace Server.Items
{
    public abstract class HouseCommittmentDeed : CommittmentDeed
    {
        public int GlobalCostMultiplier = 300; // moltiplicatore globale, 1000 = 1.

        public abstract Type Type { get; }
        public abstract int Description { get; }
        public abstract int Storage { get; }
        public abstract int Lockdowns { get; }
        public abstract int NewStorage { get; }
        public abstract int NewLockdowns { get; }
        public abstract int Vendors { get; }
        public abstract int Cost { get; }
        public abstract int XOffset { get; }
        public abstract int YOffset { get; }
        public abstract int ZOffset { get; }
        public abstract int MultiID { get; }

        public Point3D Offset { get { return new Point3D( XOffset, YOffset, ZOffset ); } }

        public HouseCommittmentDeed()
        {
            Name = "House Committment Deed";
        }

        public HouseCommittmentDeed( Serial s ) : base( s ) { }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsFulfilled && from == Committed )
            {
                from.SendMessage( "Select a place for your house to be built" );
                from.Target = new MidgardHousePlacementTarget( this );
                return;
            }

            base.OnDoubleClick( from );
        }

        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            reader.ReadByte();

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( VerifyRequirements_CallBack ), this );
        }

        public static void VerifyRequirements_CallBack( object state )
        {
            HouseCommittmentDeed oldDeed = (HouseCommittmentDeed)state;
            if( oldDeed == null )
                return;

            HouseCommittmentDeed newDeed = (HouseCommittmentDeed)Activator.CreateInstance( oldDeed.GetType() );
            Requirement oldReq;

            foreach( Requirement newReq in newDeed.Requirements )
            {
                oldReq = null;

                foreach( Requirement req in oldDeed.Requirements )
                {
                    if( req.PubRequirement.ItemType == newReq.PubRequirement.ItemType )
                        oldReq = req;
                }

                if( oldReq != null )
                {
                    if( newReq.PubRequirement.Amount != oldReq.PubRequirement.Amount )
                    {
                        if( oldReq.ComputeAmount() > newReq.PubRequirement.Amount )
                        {
                            try
                            {
                                using( TextWriter t = new StreamWriter( "FixHouseCommittmentDeeds_WARNINGS.log", true ) )
                                {
                                    t.WriteLine( "Serial {0} - Type {1} - RequirementType {2} - Old value {3} - New value {4} - Committed {5} (account {6}). TO REFUND: {7}",
                                        oldDeed.Serial, oldDeed.GetType().Name, newReq.PubRequirement.ItemType.Name,
                                        oldReq.PubRequirement.Amount, newReq.PubRequirement.Amount, oldDeed.Committed.Name,
                                        oldDeed.Committed.Account.Username,
                                        ( oldReq.ComputeAmount() - newReq.PubRequirement.Amount ) );
                                }
                            }
                            catch( Exception ex )
                            {
                                Console.WriteLine( ex );
                            }
                        }

                        try
                        {
                            using( TextWriter t = new StreamWriter( "FixHouseCommittmentDeeds.log", true ) )
                            {
                                t.WriteLine( "Serial {0} - Type {1} - RequirementType {2} - From {3} - To {4}",
                                    oldDeed.Serial, oldDeed.GetType().Name, newReq.PubRequirement.ItemType.Name,
                                    oldReq.PubRequirement.Amount, newReq.PubRequirement.Amount );
                            }
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex );
                        }

                        oldReq.PubRequirement.Amount = newReq.PubRequirement.Amount;
                    }
                }
            }

            if( !newDeed.Deleted )
                newDeed.Delete();
        }
    }

    public abstract class HCD_TwoStoryFoundation : HouseCommittmentDeed
    {

        public static int VendorPrice { get { return 10000; } }

        public static int TilePrice { get { return 300; } }
        public static int TileLogs { get { return 220; } }
        public static int TileStones { get { return 75; } }

        public override Type Type { get { return typeof( HouseFoundation ); } }
        public override int Cost { get { return ( Area * TilePrice / 1000 ) * 1000 - VendorPrice; } }

        // Questi dati vengono calcolati a partire dalla superficie della casa
        public override int Vendors { get { return Area / 5; } }
        public override int Storage { get { return ( Area * 850 ) / 100; } }
        public override int Lockdowns { get { return ( Area * 425 ) / 100; } }
        public override int NewStorage { get { return ( Area * 1265 ) / 100; } }
        public override int NewLockdowns { get { return ( Area * 632 ) / 100; } }

        public override int XOffset { get { return 0; } }
        public override int YOffset { get { return (int)Math.Floor( Length / 2.0 ) + 1; } }
        public override int ZOffset { get { return 0; } }

        // Questi dati vanno specificati per ogni tipo di base
        public override int Description { get { return 1011314; } }
        public override int MultiID { get { return 0x007E; } }

        public abstract int Width { get; }
        public abstract int Length { get; }
        public int Area { get { return Width * Length; } }

        public HCD_TwoStoryFoundation()
        {
            Name = Width + "x" + Length + " 2 Story Foundation";
            AddRequirement( typeof( Gold ), ( Cost * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( Log ), ( Area * TileLogs * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( Area * TileStones * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public abstract class HCD_ThreeStoryFoundation : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 20000; } }

        public static int TilePrice { get { return 350; } }
        public static int TileLogs { get { return 220; } }
        public static int TileStones { get { return 75; } }

        public override Type Type { get { return typeof( HouseFoundation ); } }
        public override int Cost { get { return ( Area * TilePrice / 1000 ) * 1000 - VendorPrice; } }

        // Questi dati vengono calcolati a partire dalla superficie della casa
        public override int Vendors { get { return Area / 5; } }
        public override int Storage { get { return ( Area * 850 ) / 100; } }
        public override int Lockdowns { get { return ( Area * 425 ) / 100; } }
        public override int NewStorage { get { return ( Area * 1265 ) / 100; } }
        public override int NewLockdowns { get { return ( Area * 632 ) / 100; } }

        public override int XOffset { get { return 0; } }
        public override int YOffset { get { return (int)Math.Floor( Length / 2.0 ) + 1; } }
        public override int ZOffset { get { return 0; } }

        // Questi dati vanno specificati per ogni tipo di base
        public override int Description { get { return 1011314; } }
        public override int MultiID { get { return 0x007E; } }

        public abstract int Width { get; }
        public abstract int Length { get; }
        public int Area { get { return Width * Length; } }

        public HCD_ThreeStoryFoundation()
        {
            Name = Width + "x" + Length + " 3 Story Foundation";
            AddRequirement( typeof( Gold ), ( Cost * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( Log ), ( Area * TileLogs * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( Area * TileStones * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_TwoStoryVilla : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 60000; } }

        public override Type Type { get { return typeof( TwoStoryVilla ); } }
        public override int Description { get { return 1011319; } }
        public override int Storage { get { return 1100; } }
        public override int Lockdowns { get { return 550; } }
        public override int NewStorage { get { return 1265; } }
        public override int NewLockdowns { get { return 632; } }
        public override int Vendors { get { return 24; } }
        public override int Cost { get { return 60000 - VendorPrice; } }
        public override int XOffset { get { return 3; } }
        public override int YOffset { get { return 6; } }
        public override int ZOffset { get { return 0; } }
        public override int MultiID { get { return 0x009E; } }

        [Constructable]
        public HCD_TwoStoryVilla()
        {
            Name = "Two Story Villa";
            AddRequirement( typeof( Log ), ( 75000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( 12500 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( IronIngot ), ( 12500 * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_TwoStoryVilla( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }

    }

    public class HCD_SandStonePatio : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 35000; } }

        public override Type Type { get { return typeof( SandStonePatio ); } }
        public override int Description { get { return 1011320; } }
        public override int Storage { get { return 850; } }
        public override int Lockdowns { get { return 425; } }
        public override int NewStorage { get { return 1265; } }
        public override int NewLockdowns { get { return 632; } }
        public override int Vendors { get { return 24; } }
        public override int Cost { get { return 35000 - VendorPrice; } }
        public override int XOffset { get { return -1; } }
        public override int YOffset { get { return 4; } }
        public override int ZOffset { get { return 0; } }
        public override int MultiID { get { return 0x009C; } }

        [Constructable]
        public HCD_SandStonePatio()
        {
            Name = "Sand Stone Patio";
            AddRequirement( typeof( Log ), ( 50000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( 9000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( IronIngot ), ( 9000 * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_SandStonePatio( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }

    }


    public class HCD_GuildHouse : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 60000; } }

        public override Type Type { get { return typeof( GuildHouse ); } }
        public override int Description { get { return 1011309; } }
        public override int Storage { get { return 1370; } }
        public override int Lockdowns { get { return 685; } }
        public override int NewStorage { get { return 1576; } }
        public override int NewLockdowns { get { return 788; } }
        public override int Vendors { get { return 28; } }
        public override int Cost { get { return 60000 - VendorPrice; } }
        public override int XOffset { get { return -1; } }
        public override int YOffset { get { return 7; } }
        public override int ZOffset { get { return 0; } }
        public override int MultiID { get { return 0x0074; } }

        [Constructable]
        public HCD_GuildHouse()
        {
            Name = "Guild House";
            AddRequirement( typeof( Log ), ( 75000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( 12500 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( IronIngot ), ( 12500 * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_GuildHouse( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_TwoStoryHouseA : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 80000; } }

        public override Type Type { get { return typeof( TwoStoryHouse ); } }
        public override int Description { get { return 1011310; } }
        public override int Storage { get { return 1370; } }
        public override int Lockdowns { get { return 685; } }
        public override int NewStorage { get { return 1576; } }
        public override int NewLockdowns { get { return 788; } }
        public override int Vendors { get { return 28; } }
        public override int Cost { get { return 80000 - VendorPrice; } }
        public override int XOffset { get { return -3; } }
        public override int YOffset { get { return 7; } }
        public override int ZOffset { get { return 0; } }
        public override int MultiID { get { return 0x0076; } }

        [Constructable]
        public HCD_TwoStoryHouseA()
        {
            Name = "Two Story House";
            AddRequirement( typeof( Log ), ( 90000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( 15000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( IronIngot ), ( 15000 * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_TwoStoryHouseA( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }


    public class HCD_TwoStoryHouseB : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 80000; } }

        public override Type Type { get { return typeof( TwoStoryHouse ); } }
        public override int Description { get { return 1011310; } }
        public override int Storage { get { return 1370; } }
        public override int Lockdowns { get { return 685; } }
        public override int NewStorage { get { return 1576; } }
        public override int NewLockdowns { get { return 788; } }
        public override int Vendors { get { return 28; } }
        public override int Cost { get { return 80000 - VendorPrice; } }
        public override int XOffset { get { return -3; } }
        public override int YOffset { get { return 7; } }
        public override int ZOffset { get { return 0; } }
        public override int MultiID { get { return 0x0078; } }

        [Constructable]
        public HCD_TwoStoryHouseB()
        {
            Name = "Two Story House";
            AddRequirement( typeof( Log ), ( 90000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( 15000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( IronIngot ), ( 15000 * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_TwoStoryHouseB( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_LargePatioHouse : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 60000; } }

        public override Type Type { get { return typeof( LargePatioHouse ); } }
        public override int Description { get { return 1011315; } }
        public override int Storage { get { return 1370; } }
        public override int Lockdowns { get { return 685; } }
        public override int NewStorage { get { return 1576; } }
        public override int NewLockdowns { get { return 788; } }
        public override int Vendors { get { return 28; } }
        public override int Cost { get { return 60000 - VendorPrice; } }
        public override int XOffset { get { return -4; } }
        public override int YOffset { get { return 7; } }
        public override int ZOffset { get { return 0; } }
        public override int MultiID { get { return 0x008C; } }

        [Constructable]
        public HCD_LargePatioHouse()
        {
            Name = "Large Patio House";
            AddRequirement( typeof( Log ), ( 75000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( 12500 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( IronIngot ), ( 12500 * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_LargePatioHouse( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_LargeMarbleHouse : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 80000; } }

        public override Type Type { get { return typeof( LargeMarbleHouse ); } }
        public override int Description { get { return 1011316; } }
        public override int Storage { get { return 1370; } }
        public override int Lockdowns { get { return 685; } }
        public override int NewStorage { get { return 1576; } }
        public override int NewLockdowns { get { return 788; } }
        public override int Vendors { get { return 28; } }
        public override int Cost { get { return 80000 - VendorPrice; } }
        public override int XOffset { get { return -4; } }
        public override int YOffset { get { return 7; } }
        public override int ZOffset { get { return 0; } }
        public override int MultiID { get { return 0x0096; } }

        [Constructable]
        public HCD_LargeMarbleHouse()
        {
            Name = "Large Marble House";
            AddRequirement( typeof( Log ), ( 90000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( 15000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( IronIngot ), ( 15000 * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_LargeMarbleHouse( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_Tower : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 180000; } }

        public override Type Type { get { return typeof( Tower ); } }
        public override int Description { get { return 1011312; } }
        public override int Storage { get { return 2119; } }
        public override int Lockdowns { get { return 1059; } }
        public override int NewStorage { get { return 2437; } }
        public override int NewLockdowns { get { return 1218; } }
        public override int Vendors { get { return 42; } }
        public override int Cost { get { return 180000 - VendorPrice; } }
        public override int XOffset { get { return 0; } }
        public override int YOffset { get { return 7; } }
        public override int ZOffset { get { return 0; } }
        public override int MultiID { get { return 0x007A; } }

        [Constructable]
        public HCD_Tower()
        {
            Name = "Tower";
            AddRequirement( typeof( Log ), ( 120000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( 20000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( IronIngot ), ( 20000 * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_Tower( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_Keep : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 572750; } }

        public override Type Type { get { return typeof( Keep ); } }
        public override int Description { get { return 1011313; } }
        public override int Storage { get { return 2625; } }
        public override int Lockdowns { get { return 1312; } }
        public override int NewStorage { get { return 3019; } }
        public override int NewLockdowns { get { return 1509; } }
        public override int Vendors { get { return 52; } }
        public override int Cost { get { return 572750 - VendorPrice; } }
        public override int XOffset { get { return 0; } }
        public override int YOffset { get { return 11; } }
        public override int ZOffset { get { return 0; } }
        public override int MultiID { get { return 0x007C; } }

        [Constructable]
        public HCD_Keep()
        {
            Name = "Keep";
            AddRequirement( typeof( Log ), ( 95000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( 25000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( IronIngot ), ( 25000 * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_Keep( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_Castle : HouseCommittmentDeed
    {
        public static int VendorPrice { get { return 865000; } }

        public override Type Type { get { return typeof( Castle ); } }
        public override int Description { get { return 1011314; } }
        public override int Storage { get { return 4076; } }
        public override int Lockdowns { get { return 2038; } }
        public override int NewStorage { get { return 4688; } }
        public override int NewLockdowns { get { return 2344; } }
        public override int Vendors { get { return 78; } }
        public override int Cost { get { return 865000 - VendorPrice; } }
        public override int XOffset { get { return 0; } }
        public override int YOffset { get { return 16; } }
        public override int ZOffset { get { return 0; } }
        public override int MultiID { get { return 0x007E; } }

        [Constructable]
        public HCD_Castle()
        {
            Name = "Castle";
            AddRequirement( typeof( Log ), ( 140000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( RawStone ), ( 45000 * GlobalCostMultiplier ) / 1000 );
            AddRequirement( typeof( IronIngot ), ( 45000 * GlobalCostMultiplier ) / 1000 );
        }

        public HCD_Castle( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_7x7TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060241; } }
        public override int MultiID { get { return 0x13EC; } }
        public override int Width { get { return 7; } }
        public override int Length { get { return 7; } }

        [Constructable]
        public HCD_7x7TwoStoryFoundation() { }
        public HCD_7x7TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_7x8TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060242; } }
        public override int MultiID { get { return 0x13ED; } }
        public override int Width { get { return 7; } }
        public override int Length { get { return 8; } }

        [Constructable]
        public HCD_7x8TwoStoryFoundation() { }
        public HCD_7x8TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_7x9TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060243; } }
        public override int MultiID { get { return 0x13EE; } }
        public override int Width { get { return 7; } }
        public override int Length { get { return 9; } }

        [Constructable]
        public HCD_7x9TwoStoryFoundation() { }
        public HCD_7x9TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_7x10TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060244; } }
        public override int MultiID { get { return 0x13EF; } }
        public override int Width { get { return 7; } }
        public override int Length { get { return 10; } }

        [Constructable]
        public HCD_7x10TwoStoryFoundation() { }
        public HCD_7x10TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_7x11TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060245; } }
        public override int MultiID { get { return 0x13F0; } }
        public override int Width { get { return 7; } }
        public override int Length { get { return 11; } }

        [Constructable]
        public HCD_7x11TwoStoryFoundation() { }
        public HCD_7x11TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_7x12TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060246; } }
        public override int MultiID { get { return 0x13F1; } }
        public override int Width { get { return 7; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_7x12TwoStoryFoundation() { }
        public HCD_7x12TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_8x7TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060253; } }
        public override int MultiID { get { return 0x13F8; } }
        public override int Width { get { return 8; } }
        public override int Length { get { return 7; } }

        [Constructable]
        public HCD_8x7TwoStoryFoundation() { }
        public HCD_8x7TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_8x8TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060254; } }
        public override int MultiID { get { return 0x13F9; } }
        public override int Width { get { return 8; } }
        public override int Length { get { return 8; } }

        [Constructable]
        public HCD_8x8TwoStoryFoundation() { }
        public HCD_8x8TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_8x9TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060255; } }
        public override int MultiID { get { return 0x13FA; } }
        public override int Width { get { return 8; } }
        public override int Length { get { return 9; } }

        [Constructable]
        public HCD_8x9TwoStoryFoundation() { }
        public HCD_8x9TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_8x10TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060256; } }
        public override int MultiID { get { return 0x13FB; } }
        public override int Width { get { return 8; } }
        public override int Length { get { return 10; } }

        [Constructable]
        public HCD_8x10TwoStoryFoundation() { }
        public HCD_8x10TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_8x11TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060257; } }
        public override int MultiID { get { return 0x13FC; } }
        public override int Width { get { return 8; } }
        public override int Length { get { return 11; } }

        [Constructable]
        public HCD_8x11TwoStoryFoundation() { }
        public HCD_8x11TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_8x12TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060258; } }
        public override int MultiID { get { return 0x13FD; } }
        public override int Width { get { return 8; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_8x12TwoStoryFoundation() { }
        public HCD_8x12TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_8x13TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060259; } }
        public override int MultiID { get { return 0x13FE; } }
        public override int Width { get { return 8; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_8x13TwoStoryFoundation() { }
        public HCD_8x13TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_9x7TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060265; } }
        public override int MultiID { get { return 0x1404; } }
        public override int Width { get { return 9; } }
        public override int Length { get { return 7; } }

        [Constructable]
        public HCD_9x7TwoStoryFoundation() { }
        public HCD_9x7TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_9x8TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060266; } }
        public override int MultiID { get { return 0x1405; } }
        public override int Width { get { return 9; } }
        public override int Length { get { return 8; } }

        [Constructable]
        public HCD_9x8TwoStoryFoundation() { }
        public HCD_9x8TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_9x9TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060267; } }
        public override int MultiID { get { return 0x1406; } }
        public override int Width { get { return 9; } }
        public override int Length { get { return 9; } }

        [Constructable]
        public HCD_9x9TwoStoryFoundation() { }
        public HCD_9x9TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_9x10TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060268; } }
        public override int MultiID { get { return 0x1407; } }
        public override int Width { get { return 9; } }
        public override int Length { get { return 10; } }

        [Constructable]
        public HCD_9x10TwoStoryFoundation() { }
        public HCD_9x10TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_9x11TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060269; } }
        public override int MultiID { get { return 0x1408; } }
        public override int Width { get { return 9; } }
        public override int Length { get { return 11; } }

        [Constructable]
        public HCD_9x11TwoStoryFoundation() { }
        public HCD_9x11TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_9x12TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060270; } }
        public override int MultiID { get { return 0x1409; } }
        public override int Width { get { return 9; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_9x12TwoStoryFoundation() { }
        public HCD_9x12TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_9x13TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060271; } }
        public override int MultiID { get { return 0x140A; } }
        public override int Width { get { return 9; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_9x13TwoStoryFoundation() { }
        public HCD_9x13TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_10x7TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060277; } }
        public override int MultiID { get { return 0x1410; } }
        public override int Width { get { return 10; } }
        public override int Length { get { return 7; } }

        [Constructable]
        public HCD_10x7TwoStoryFoundation() { }
        public HCD_10x7TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_10x8TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060278; } }
        public override int MultiID { get { return 0x1411; } }
        public override int Width { get { return 10; } }
        public override int Length { get { return 8; } }

        [Constructable]
        public HCD_10x8TwoStoryFoundation() { }
        public HCD_10x8TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_10x9TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060279; } }
        public override int MultiID { get { return 0x1412; } }
        public override int Width { get { return 10; } }
        public override int Length { get { return 9; } }

        [Constructable]
        public HCD_10x9TwoStoryFoundation() { }
        public HCD_10x9TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_10x10TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060280; } }
        public override int MultiID { get { return 0x1413; } }
        public override int Width { get { return 10; } }
        public override int Length { get { return 10; } }

        [Constructable]
        public HCD_10x10TwoStoryFoundation() { }
        public HCD_10x10TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_10x11TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060281; } }
        public override int MultiID { get { return 0x1414; } }
        public override int Width { get { return 10; } }
        public override int Length { get { return 11; } }

        [Constructable]
        public HCD_10x11TwoStoryFoundation() { }
        public HCD_10x11TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_10x12TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060282; } }
        public override int MultiID { get { return 0x1415; } }
        public override int Width { get { return 10; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_10x12TwoStoryFoundation() { }
        public HCD_10x12TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_10x13TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060283; } }
        public override int MultiID { get { return 0x1416; } }
        public override int Width { get { return 10; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_10x13TwoStoryFoundation() { }
        public HCD_10x13TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_11x7TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060289; } }
        public override int MultiID { get { return 0x141C; } }
        public override int Width { get { return 11; } }
        public override int Length { get { return 7; } }

        [Constructable]
        public HCD_11x7TwoStoryFoundation() { }
        public HCD_11x7TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_11x8TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060290; } }
        public override int MultiID { get { return 0x141D; } }
        public override int Width { get { return 11; } }
        public override int Length { get { return 8; } }

        [Constructable]
        public HCD_11x8TwoStoryFoundation() { }
        public HCD_11x8TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_11x9TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060291; } }
        public override int MultiID { get { return 0x141E; } }
        public override int Width { get { return 11; } }
        public override int Length { get { return 9; } }

        [Constructable]
        public HCD_11x9TwoStoryFoundation() { }
        public HCD_11x9TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_11x10TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060292; } }
        public override int MultiID { get { return 0x141F; } }
        public override int Width { get { return 11; } }
        public override int Length { get { return 10; } }

        [Constructable]
        public HCD_11x10TwoStoryFoundation() { }
        public HCD_11x10TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_11x11TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060293; } }
        public override int MultiID { get { return 0x1420; } }
        public override int Width { get { return 11; } }
        public override int Length { get { return 11; } }

        [Constructable]
        public HCD_11x11TwoStoryFoundation() { }
        public HCD_11x11TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_11x12TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060294; } }
        public override int MultiID { get { return 0x1421; } }
        public override int Width { get { return 11; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_11x12TwoStoryFoundation() { }
        public HCD_11x12TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_11x13TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060295; } }
        public override int MultiID { get { return 0x1422; } }
        public override int Width { get { return 11; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_11x13TwoStoryFoundation() { }
        public HCD_11x13TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_12x7TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060301; } }
        public override int MultiID { get { return 0x1428; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 7; } }

        [Constructable]
        public HCD_12x7TwoStoryFoundation() { }
        public HCD_12x7TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_12x8TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060302; } }
        public override int MultiID { get { return 0x1429; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 8; } }

        [Constructable]
        public HCD_12x8TwoStoryFoundation() { }
        public HCD_12x8TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_12x9TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060303; } }
        public override int MultiID { get { return 0x142A; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 9; } }

        [Constructable]
        public HCD_12x9TwoStoryFoundation() { }
        public HCD_12x9TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_12x10TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060304; } }
        public override int MultiID { get { return 0x142B; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 10; } }

        [Constructable]
        public HCD_12x10TwoStoryFoundation() { }
        public HCD_12x10TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_12x11TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060305; } }
        public override int MultiID { get { return 0x142C; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 11; } }

        [Constructable]
        public HCD_12x11TwoStoryFoundation() { }
        public HCD_12x11TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_12x12TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060306; } }
        public override int MultiID { get { return 0x142D; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_12x12TwoStoryFoundation() { }
        public HCD_12x12TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_12x13TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060307; } }
        public override int MultiID { get { return 0x142E; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_12x13TwoStoryFoundation() { }
        public HCD_12x13TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_13x8TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060314; } }
        public override int MultiID { get { return 0x1435; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 8; } }

        [Constructable]
        public HCD_13x8TwoStoryFoundation() { }
        public HCD_13x8TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_13x9TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060315; } }
        public override int MultiID { get { return 0x1436; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 9; } }

        [Constructable]
        public HCD_13x9TwoStoryFoundation() { }
        public HCD_13x9TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_13x10TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060316; } }
        public override int MultiID { get { return 0x1437; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 10; } }

        [Constructable]
        public HCD_13x10TwoStoryFoundation() { }
        public HCD_13x10TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_13x11TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060317; } }
        public override int MultiID { get { return 0x1438; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 11; } }

        [Constructable]
        public HCD_13x11TwoStoryFoundation() { }
        public HCD_13x11TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_13x12TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060318; } }
        public override int MultiID { get { return 0x1439; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_13x12TwoStoryFoundation() { }
        public HCD_13x12TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    public class HCD_13x13TwoStoryFoundation : HCD_TwoStoryFoundation
    {
        public override int Description { get { return 1060319; } }
        public override int MultiID { get { return 0x143A; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_13x13TwoStoryFoundation() { }
        public HCD_13x13TwoStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060272,	1150,	575,	1323,	661,	24,	36000,	27000,	9000,	0,	8,	0,	0x140B	), // 9x14 3-Story Customizable House
    public class HCD_9x14ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060272; } }
        public override int MultiID { get { return 0x140B; } }
        public override int Width { get { return 9; } }
        public override int Length { get { return 14; } }

        [Constructable]
        public HCD_9x14ThreeStoryFoundation() { }
        public HCD_9x14ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }

    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060284,	1200,	600,	1380,	690,	26,	40000,	30000,	10000,	0,	8,	0,	0x1417	), // 10x14 3-Story Customizable House
    public class HCD_10x14ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060284; } }
        public override int MultiID { get { return 0x1417; } }
        public override int Width { get { return 10; } }
        public override int Length { get { return 14; } }

        [Constructable]
        public HCD_10x14ThreeStoryFoundation() { }
        public HCD_10x14ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060285,	1250,	625,	1438,	719,	26,	43000,	32250,	10750,	0,	8,	0,	0x1418	), // 10x15 3-Story Customizable House
    public class HCD_10x15ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060285; } }
        public override int MultiID { get { return 0x1418; } }
        public override int Width { get { return 10; } }
        public override int Length { get { return 15; } }

        [Constructable]
        public HCD_10x15ThreeStoryFoundation() { }
        public HCD_10x15ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060296,	1250,	625,	1438,	719,	26,	44000,	33000,	11000,	0,	8,	0,	0x1423	), // 11x14 3-Story Customizable House
    public class HCD_11x14ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060296; } }
        public override int MultiID { get { return 0x1423; } }
        public override int Width { get { return 11; } }
        public override int Length { get { return 14; } }

        [Constructable]
        public HCD_11x14ThreeStoryFoundation() { }
        public HCD_11x14ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060297,	1300,	650,	1495,	747,	28,	47000,	35250,	11750,	0,	8,	0,	0x1424	), // 11x15 3-Story Customizable House
    public class HCD_11x15ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060297; } }
        public override int MultiID { get { return 0x1424; } }
        public override int Width { get { return 11; } }
        public override int Length { get { return 15; } }

        [Constructable]
        public HCD_11x15ThreeStoryFoundation() { }
        public HCD_11x15ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060298,	1350,	675,	1553,	776,	28,	50000,	37500,	12500,	0,	9,	0,	0x1425	), // 11x16 3-Story Customizable House
    public class HCD_11x16ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060298; } }
        public override int MultiID { get { return 0x1425; } }
        public override int Width { get { return 11; } }
        public override int Length { get { return 16; } }

        [Constructable]
        public HCD_11x16ThreeStoryFoundation() { }
        public HCD_11x16ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060308,	1300,	650,	1495,	747,	28,	48000,	36000,	12000,	0,	8,	0,	0x142F	), // 12x14 3-Story Customizable House
    public class HCD_12x14ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060308; } }
        public override int MultiID { get { return 0x142F; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 14; } }

        [Constructable]
        public HCD_12x14ThreeStoryFoundation() { }
        public HCD_12x14ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060309,	1350,	675,	1553,	776,	28,	51000,	38250,	12750,	0,	8,	0,	0x1430	), // 12x15 3-Story Customizable House
    public class HCD_12x15ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060309; } }
        public override int MultiID { get { return 0x1430; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 15; } }

        [Constructable]
        public HCD_12x15ThreeStoryFoundation() { }
        public HCD_12x15ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060310,	1370,	685,	1576,	788,	28,	55000,	41250,	13750,	0,	9,	0,	0x1431	), // 12x16 3-Story Customizable House
    public class HCD_12x16ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060310; } }
        public override int MultiID { get { return 0x1431; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 16; } }

        [Constructable]
        public HCD_12x16ThreeStoryFoundation() { }
        public HCD_12x16ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060311,	1370,	685,	1576,	788,	28,	58000,	43500,	14500,	0,	9,	0,	0x1432	), // 12x17 3-Story Customizable House
    public class HCD_12x17ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060311; } }
        public override int MultiID { get { return 0x1432; } }
        public override int Width { get { return 12; } }
        public override int Length { get { return 17; } }

        [Constructable]
        public HCD_12x17ThreeStoryFoundation() { }
        public HCD_12x17ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060320,	1350,	675,	1553,	776,	28,	52000,	39000,	13000,	0,	8,	0,	0x143B	), // 13x14 3-Story Customizable House
    public class HCD_13x14ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060320; } }
        public override int MultiID { get { return 0x143B; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 14; } }

        [Constructable]
        public HCD_13x14ThreeStoryFoundation() { }
        public HCD_13x14ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060321,	1370,	685,	1576,	788,	28,	55000,	41250,	13750,	0,	8,	0,	0x143C	), // 13x15 3-Story Customizable House
    public class HCD_13x15ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060321; } }
        public override int MultiID { get { return 0x143C; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 15; } }

        [Constructable]
        public HCD_13x15ThreeStoryFoundation() { }
        public HCD_13x15ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060322,	1370,	685,	1576,	788,	28,	59000,	44250,	14750,	0,	9,	0,	0x143D	), // 13x16 3-Story Customizable House
    public class HCD_13x16ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060322; } }
        public override int MultiID { get { return 0x143D; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 16; } }

        [Constructable]
        public HCD_13x16ThreeStoryFoundation() { }
        public HCD_13x16ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060323,	2119,	1059,	2437,	1218,	42,	62000,	46500,	15500,	0,	9,	0,	0x143E	), // 13x17 3-Story Customizable House
    public class HCD_13x17ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060323; } }
        public override int MultiID { get { return 0x143E; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 17; } }

        [Constructable]
        public HCD_13x17ThreeStoryFoundation() { }
        public HCD_13x17ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060324,	2119,	1059,	2437,	1218,	42,	66000,	49500,	16500,	0,	10,	0,	0x143F	), // 13x18 3-Story Customizable House
    public class HCD_13x18ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060324; } }
        public override int MultiID { get { return 0x143F; } }
        public override int Width { get { return 13; } }
        public override int Length { get { return 18; } }

        [Constructable]
        public HCD_13x18ThreeStoryFoundation() { }
        public HCD_13x18ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060327,	1150,	575,	1323,	661,	24,	36000,	27000,	9000,	0,	5,	0,	0x1442	), // 14x9 3-Story Customizable House
    public class HCD_14x9ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060327; } }
        public override int MultiID { get { return 0x1442; } }
        public override int Width { get { return 14; } }
        public override int Length { get { return 9; } }

        [Constructable]
        public HCD_14x9ThreeStoryFoundation() { }
        public HCD_14x9ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060328,	1200,	600,	1380,	690,	26,	41000,	30750,	10250,	0,	6,	0,	0x1443	), // 14x10 3-Story Customizable House
    public class HCD_14x10ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060328; } }
        public override int MultiID { get { return 0x1443; } }
        public override int Width { get { return 14; } }
        public override int Length { get { return 10; } }

        [Constructable]
        public HCD_14x10ThreeStoryFoundation() { }
        public HCD_14x10ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060329,	1250,	625,	1438,	719,	26,	44000,	33000,	11000,	0,	6,	0,	0x1444	), // 14x11 3-Story Customizable House
    public class HCD_14x11ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060329; } }
        public override int MultiID { get { return 0x1444; } }
        public override int Width { get { return 14; } }
        public override int Length { get { return 11; } }

        [Constructable]
        public HCD_14x11ThreeStoryFoundation() { }
        public HCD_14x11ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060330,	1300,	650,	1495,	747,	28,	48000,	36000,	12000,	0,	7,	0,	0x1445	), // 14x12 3-Story Customizable House
    public class HCD_14x12ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060330; } }
        public override int MultiID { get { return 0x1445; } }
        public override int Width { get { return 14; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_14x12ThreeStoryFoundation() { }
        public HCD_14x12ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060331,	1350,	675,	1553,	776,	28,	52000,	39000,	13000,	0,	7,	0,	0x1446	), // 14x13 3-Story Customizable House
    public class HCD_14x13ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060331; } }
        public override int MultiID { get { return 0x1446; } }
        public override int Width { get { return 14; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_14x13ThreeStoryFoundation() { }
        public HCD_14x13ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060332,	1370,	685,	1576,	788,	28,	56000,	42000,	14000,	0,	8,	0,	0x1447	), // 14x14 3-Story Customizable House
    public class HCD_14x14ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060332; } }
        public override int MultiID { get { return 0x1447; } }
        public override int Width { get { return 14; } }
        public override int Length { get { return 14; } }

        [Constructable]
        public HCD_14x14ThreeStoryFoundation() { }
        public HCD_14x14ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060333,	1370,	685,	1576,	788,	28,	59000,	44250,	14750,	0,	8,	0,	0x1448	), // 14x15 3-Story Customizable House
    public class HCD_14x15ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060333; } }
        public override int MultiID { get { return 0x1448; } }
        public override int Width { get { return 14; } }
        public override int Length { get { return 15; } }

        [Constructable]
        public HCD_14x15ThreeStoryFoundation() { }
        public HCD_14x15ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060334,	2119,	1059,	2437,	1218,	42,	63000,	47250,	15750,	0,	9,	0,	0x1449	), // 14x16 3-Story Customizable House
    public class HCD_14x16ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060334; } }
        public override int MultiID { get { return 0x1449; } }
        public override int Width { get { return 14; } }
        public override int Length { get { return 16; } }

        [Constructable]
        public HCD_14x16ThreeStoryFoundation() { }
        public HCD_14x16ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060335,	2119,	1059,	2437,	1218,	42,	67000,	50250,	16750,	0,	9,	0,	0x144A	), // 14x17 3-Story Customizable House
    public class HCD_14x17ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060335; } }
        public override int MultiID { get { return 0x144A; } }
        public override int Width { get { return 14; } }
        public override int Length { get { return 17; } }

        [Constructable]
        public HCD_14x17ThreeStoryFoundation() { }
        public HCD_14x17ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060336,	2119,	1059,	2437,	1218,	42,	70000,	52250,	17500,	0,	10,	0,	0x144B	), // 14x18 3-Story Customizable House
    public class HCD_14x18ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060336; } }
        public override int MultiID { get { return 0x144B; } }
        public override int Width { get { return 14; } }
        public override int Length { get { return 18; } }

        [Constructable]
        public HCD_14x18ThreeStoryFoundation() { }
        public HCD_14x18ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060340,	1250,	625,	1438,	719,	26,	43000,	32250,	10750,	0,	6,	0,	0x144F	), // 15x10 3-Story Customizable House
    public class HCD_15x10ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060340; } }
        public override int MultiID { get { return 0x144F; } }
        public override int Width { get { return 15; } }
        public override int Length { get { return 10; } }

        [Constructable]
        public HCD_15x10ThreeStoryFoundation() { }
        public HCD_15x10ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060341,	1300,	650,	1495,	747,	28,	47000,	35250,	11750,	0,	6,	0,	0x1450	), // 15x11 3-Story Customizable House
    public class HCD_15x11ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060341; } }
        public override int MultiID { get { return 0x1450; } }
        public override int Width { get { return 15; } }
        public override int Length { get { return 11; } }

        [Constructable]
        public HCD_15x11ThreeStoryFoundation() { }
        public HCD_15x11ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060342,	1350,	675,	1553,	776,	28,	51000,	38250,	12750,	0,	7,	0,	0x1451	), // 15x12 3-Story Customizable House
    public class HCD_15x12ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060342; } }
        public override int MultiID { get { return 0x1451; } }
        public override int Width { get { return 15; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_15x12ThreeStoryFoundation() { }
        public HCD_15x12ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060343,	1370,	685,	1576,	788,	28,	55000,	41250,	13750,	0,	7,	0,	0x1452	), // 15x13 3-Story Customizable House
    public class HCD_15x13ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060343; } }
        public override int MultiID { get { return 0x1452; } }
        public override int Width { get { return 15; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_15x13ThreeStoryFoundation() { }
        public HCD_15x13ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060344,	1370,	685,	1576,	788,	28,	59000,	44250,	14750,	0,	8,	0,	0x1453	), // 15x14 3-Story Customizable House
    public class HCD_15x14ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060344; } }
        public override int MultiID { get { return 0x1453; } }
        public override int Width { get { return 15; } }
        public override int Length { get { return 14; } }

        [Constructable]
        public HCD_15x14ThreeStoryFoundation() { }
        public HCD_15x14ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060345,	2119,	1059,	2437,	1218,	42,	63000,	47250,	15750,	0,	8,	0,	0x1454	), // 15x15 3-Story Customizable House
    public class HCD_15x15ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060345; } }
        public override int MultiID { get { return 0x1454; } }
        public override int Width { get { return 15; } }
        public override int Length { get { return 15; } }

        [Constructable]
        public HCD_15x15ThreeStoryFoundation() { }
        public HCD_15x15ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060346,	2119,	1059,	2437,	1218,	42,	67000,	50250,	16750,	0,	9,	0,	0x1455	), // 15x16 3-Story Customizable House
    public class HCD_15x16ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060346; } }
        public override int MultiID { get { return 0x1455; } }
        public override int Width { get { return 15; } }
        public override int Length { get { return 16; } }

        [Constructable]
        public HCD_15x16ThreeStoryFoundation() { }
        public HCD_15x16ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060347,	2119,	1059,	2437,	1218,	42,	71000,	53250,	17750,	0,	9,	0,	0x1456	), // 15x17 3-Story Customizable House
    public class HCD_15x17ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060347; } }
        public override int MultiID { get { return 0x1456; } }
        public override int Width { get { return 15; } }
        public override int Length { get { return 17; } }

        [Constructable]
        public HCD_15x17ThreeStoryFoundation() { }
        public HCD_15x17ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060348,	2119,	1059,	2437,	1218,	42,	75000,	56250,	18750,	0,	10,	0,	0x1457	), // 15x18 3-Story Customizable House
    public class HCD_15x18ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060348; } }
        public override int MultiID { get { return 0x1457; } }
        public override int Width { get { return 15; } }
        public override int Length { get { return 18; } }

        [Constructable]
        public HCD_15x18ThreeStoryFoundation() { }
        public HCD_15x18ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060353,	1350,	675,	1553,	776,	28,	50000,	37500,	12500,	0,	6,	0,	0x145C	), // 16x11 3-Story Customizable House
    public class HCD_16x11ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060353; } }
        public override int MultiID { get { return 0x145C; } }
        public override int Width { get { return 16; } }
        public override int Length { get { return 11; } }

        [Constructable]
        public HCD_16x11ThreeStoryFoundation() { }
        public HCD_16x11ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060354,	1370,	685,	1576,	788,	28,	55000,	41250,	13750,	0,	7,	0,	0x145D	), // 16x12 3-Story Customizable House
    public class HCD_16x12ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060354; } }
        public override int MultiID { get { return 0x145D; } }
        public override int Width { get { return 16; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_16x12ThreeStoryFoundation() { }
        public HCD_16x12ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060355,	1370,	685,	1576,	788,	28,	59000,	44250,	14750,	0,	7,	0,	0x145E	), // 16x13 3-Story Customizable House
    public class HCD_16x13ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060355; } }
        public override int MultiID { get { return 0x145E; } }
        public override int Width { get { return 16; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_16x13ThreeStoryFoundation() { }
        public HCD_16x13ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060356,	2119,	1059,	2437,	1218,	42,	63000,	47250,	15750,	0,	8,	0,	0x145F	), // 16x14 3-Story Customizable House
    public class HCD_16x14ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060356; } }
        public override int MultiID { get { return 0x145F; } }
        public override int Width { get { return 16; } }
        public override int Length { get { return 14; } }

        [Constructable]
        public HCD_16x14ThreeStoryFoundation() { }
        public HCD_16x14ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060357,	2119,	1059,	2437,	1218,	42,	67000,	50250,	16750,	0,	8,	0,	0x1460	), // 16x15 3-Story Customizable House
    public class HCD_16x15ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060357; } }
        public override int MultiID { get { return 0x1460; } }
        public override int Width { get { return 16; } }
        public override int Length { get { return 15; } }

        [Constructable]
        public HCD_16x15ThreeStoryFoundation() { }
        public HCD_16x15ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060358,	2119,	1059,	2437,	1218,	42,	72000,	54000,	18000,	0,	9,	0,	0x1461	), // 16x16 3-Story Customizable House
    public class HCD_16x16ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060358; } }
        public override int MultiID { get { return 0x1461; } }
        public override int Width { get { return 16; } }
        public override int Length { get { return 16; } }

        [Constructable]
        public HCD_16x16ThreeStoryFoundation() { }
        public HCD_16x16ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060359,	2119,	1059,	2437,	1218,	42,	75000,	56250,	18750,	0,	9,	0,	0x1462	), // 16x17 3-Story Customizable House
    public class HCD_16x17ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060359; } }
        public override int MultiID { get { return 0x1462; } }
        public override int Width { get { return 16; } }
        public override int Length { get { return 17; } }

        [Constructable]
        public HCD_16x17ThreeStoryFoundation() { }
        public HCD_16x17ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060360,	2119,	1059,	2437,	1218,	42,	80000,	60000,	20000,	0,	10,	0,	0x1463	), // 16x18 3-Story Customizable House
    public class HCD_16x18ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060360; } }
        public override int MultiID { get { return 0x1463; } }
        public override int Width { get { return 16; } }
        public override int Length { get { return 18; } }

        [Constructable]
        public HCD_16x18ThreeStoryFoundation() { }
        public HCD_16x18ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060366,	1370,	685,	1576,	788,	28,	58000,	43500,	14500,	0,	7,	0,	0x1469	), // 17x12 3-Story Customizable House
    public class HCD_17x12ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060366; } }
        public override int MultiID { get { return 0x1469; } }
        public override int Width { get { return 17; } }
        public override int Length { get { return 12; } }

        [Constructable]
        public HCD_17x12ThreeStoryFoundation() { }
        public HCD_17x12ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060367,	2119,	1059,	2437,	1218,	42,	62000,	46500,	15500,	0,	7,	0,	0x146A	), // 17x13 3-Story Customizable House
    public class HCD_17x13ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060367; } }
        public override int MultiID { get { return 0x146A; } }
        public override int Width { get { return 17; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_17x13ThreeStoryFoundation() { }
        public HCD_17x13ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060368,	2119,	1059,	2437,	1218,	42,	66000,	49500,	16500,	0,	8,	0,	0x146B	), // 17x14 3-Story Customizable House
    public class HCD_17x14ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060368; } }
        public override int MultiID { get { return 0x146B; } }
        public override int Width { get { return 17; } }
        public override int Length { get { return 14; } }

        [Constructable]
        public HCD_17x14ThreeStoryFoundation() { }
        public HCD_17x14ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060369,	2119,	1059,	2437,	1218,	42,	71000,	53250,	17750,	0,	8,	0,	0x146C	), // 17x15 3-Story Customizable House
    public class HCD_17x15ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060369; } }
        public override int MultiID { get { return 0x146C; } }
        public override int Width { get { return 17; } }
        public override int Length { get { return 15; } }

        [Constructable]
        public HCD_17x15ThreeStoryFoundation() { }
        public HCD_17x15ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060370,	2119,	1059,	2437,	1218,	42,	75000,	56250,	18750,	0,	9,	0,	0x146D	), // 17x16 3-Story Customizable House
    public class HCD_17x16ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060370; } }
        public override int MultiID { get { return 0x146D; } }
        public override int Width { get { return 17; } }
        public override int Length { get { return 16; } }

        [Constructable]
        public HCD_17x16ThreeStoryFoundation() { }
        public HCD_17x16ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060371,	2119,	1059,	2437,	1218,	42,	80000,	60000,	20000,	0,	9,	0,	0x146E	), // 17x17 3-Story Customizable House
    public class HCD_17x17ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060371; } }
        public override int MultiID { get { return 0x146E; } }
        public override int Width { get { return 17; } }
        public override int Length { get { return 17; } }

        [Constructable]
        public HCD_17x17ThreeStoryFoundation() { }
        public HCD_17x17ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060372,	2119,	1059,	2437,	1218,	42,	85000,	63750,	21250,	0,	10,	0,	0x146F	), // 17x18 3-Story Customizable House
    public class HCD_17x18ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060372; } }
        public override int MultiID { get { return 0x146F; } }
        public override int Width { get { return 17; } }
        public override int Length { get { return 18; } }

        [Constructable]
        public HCD_17x18ThreeStoryFoundation() { }
        public HCD_17x18ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060379,	2119,	1059,	2437,	1218,	42,	66000,	49500,	16500,	0,	7,	0,	0x1476	), // 18x13 3-Story Customizable House
    public class HCD_18x13ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060379; } }
        public override int MultiID { get { return 0x1476; } }
        public override int Width { get { return 18; } }
        public override int Length { get { return 13; } }

        [Constructable]
        public HCD_18x13ThreeStoryFoundation() { }
        public HCD_18x13ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060380,	2119,	1059,	2437,	1218,	42,	70000,	52250,	17500,	0,	8,	0,	0x1477	), // 18x14 3-Story Customizable House
    public class HCD_18x14ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060380; } }
        public override int MultiID { get { return 0x1477; } }
        public override int Width { get { return 18; } }
        public override int Length { get { return 14; } }

        [Constructable]
        public HCD_18x14ThreeStoryFoundation() { }
        public HCD_18x14ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060381,	2119,	1059,	2437,	1218,	42,	75000,	56250,	18750,	0,	8,	0,	0x1478	), // 18x15 3-Story Customizable House
    public class HCD_18x15ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060381; } }
        public override int MultiID { get { return 0x1478; } }
        public override int Width { get { return 18; } }
        public override int Length { get { return 15; } }

        [Constructable]
        public HCD_18x15ThreeStoryFoundation() { }
        public HCD_18x15ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060382,	2119,	1059,	2437,	1218,	42,	80000,	60000,	20000,	0,	9,	0,	0x1479	), // 18x16 3-Story Customizable House
    public class HCD_18x16ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060382; } }
        public override int MultiID { get { return 0x1479; } }
        public override int Width { get { return 18; } }
        public override int Length { get { return 16; } }

        [Constructable]
        public HCD_18x16ThreeStoryFoundation() { }
        public HCD_18x16ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060383,	2119,	1059,	2437,	1218,	42,	85000,	63750,	21250,	0,	9,	0,	0x147A	), // 18x17 3-Story Customizable House
    public class HCD_18x17ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060383; } }
        public override int MultiID { get { return 0x147A; } }
        public override int Width { get { return 18; } }
        public override int Length { get { return 17; } }

        [Constructable]
        public HCD_18x17ThreeStoryFoundation() { }
        public HCD_18x17ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
    //new MidgardHousePlacementEntry( typeof( HouseFoundation ),	1060384,	2119,	1059,	2437,	1218,	42,	90000,	67500,	22500,	0,	10,	0,	0x147B	)  // 18x18 3-Story Customizable House
    public class HCD_18x18ThreeStoryFoundation : HCD_ThreeStoryFoundation
    {
        public override int Description { get { return 1060384; } }
        public override int MultiID { get { return 0x147B; } }
        public override int Width { get { return 18; } }
        public override int Length { get { return 18; } }

        [Constructable]
        public HCD_18x18ThreeStoryFoundation() { }
        public HCD_18x18ThreeStoryFoundation( Serial s ) : base( s ) { }
        public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (byte)0 ); }
        public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); reader.ReadByte(); }
    }
}*/