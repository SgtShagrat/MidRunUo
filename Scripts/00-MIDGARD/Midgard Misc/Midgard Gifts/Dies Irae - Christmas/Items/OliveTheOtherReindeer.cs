using System;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.Events.Items
{
    public class SantasSleigh : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SantasSleighDeed(); } }

        [Constructable]
        public SantasSleigh( bool east )
        {
            AddonComponent ac;

            if( east )
            {
                ac = new AddonComponent( 14964 );
                ac.Name = "Santa's Sleigh";
                AddComponent( ac, 0, 0, 0 );

                ac = new AddonComponent( 14963 );
                ac.Name = "Santa's Sleigh";
                AddComponent( ac, 1, 0, 0 );
            }
            else
            {
                ac = new AddonComponent( 14984 );
                ac.Name = "Santa's Sleigh";
                AddComponent( ac, 0, 0, 0 );

                ac = new AddonComponent( 14983 );
                ac.Name = "Santa's Sleigh";
                AddComponent( ac, 0, 1, 0 );
            }
        }

        public SantasSleigh( Serial serial )
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

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    public class SantasSleighDeed : BaseAddonDeed
    {
        private bool m_East;

        public override BaseAddon Addon { get { return new SantasSleigh( m_East ); } }

        [Constructable]
        public SantasSleighDeed()
        {
            Name = "A deed for Santa's Sleigh";
            LootType = LootType.Blessed;
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( ChristmasHelper.StaticMidgardGreetings );
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsChildOf( from.Backpack ) )
            {
                from.CloseGump( typeof( InternalGump ) );
                from.SendGump( new InternalGump( this ) );
            }
            else
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
        }

        private void SendTarget( Mobile m )
        {
            base.OnDoubleClick( m );
        }

        private class InternalGump : Gump
        {
            private readonly SantasSleighDeed m_Deed;

            public InternalGump( SantasSleighDeed deed )
                : base( 150, 50 )
            {
                m_Deed = deed;

                AddBackground( 0, 0, 350, 250, 0xA28 );

                AddItem( 90, 52, 14984 );
                AddItem( 73, 53, 14983 );
                AddButton( 70, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0 ); // South

                AddItem( 217, 51, 14964 );
                AddItem( 244, 52, 14963 );
                AddButton( 185, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0 ); // East
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                if( m_Deed.Deleted || info.ButtonID == 0 )
                    return;

                m_Deed.m_East = ( info.ButtonID != 1 );
                m_Deed.SendTarget( sender.Mobile );
            }
        }

        public SantasSleighDeed( Serial serial )
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

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    [Flipable( 0x3A5F, 0x3A65 )]
    public class OliveTheOtherReindeer : BaseReindeer
    {
        [Constructable]
        public OliveTheOtherReindeer()
            : this( 0 )
        {
        }

        [Constructable]
        public OliveTheOtherReindeer( int hue )
            : base( 0x3A5F, hue, "Olive The Other Reindeer" )
        {
        }

        public OliveTheOtherReindeer( Serial serial )
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

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }
    
    public class BaseReindeer : Item
    {
        private static readonly TimeSpan MessageDelay = TimeSpan.FromSeconds( 5.0 );

        private DateTime m_NextMessage;

        public BaseReindeer( int itemID, int hue, string name )
            : base( itemID )
        {
            Name = name;
            Hue = hue;
            Weight = 10.0;
            LootType = LootType.Blessed;
        }

        public BaseReindeer( Serial serial )
            : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( ChristmasHelper.StaticMidgardGreetings );
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        private static readonly string[] m_Shouts = new string[]
                                                   {
                                                   "* Yuk *",
                                                   "* Sgrunt *",
                                                   "* Brrrrr *",
                                                   "* Yawn *"
                                                   };

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            if( ( !m.Hidden || m.AccessLevel == AccessLevel.Player ) && Utility.InRange( m.Location, Location, 2 ) && !Utility.InRange( oldLocation, Location, 2 ) )
            {
                if( DateTime.Now > m_NextMessage )
                {
                    m_NextMessage = DateTime.Now + MessageDelay;
                    PublicOverheadMessage( MessageType.Regular, 37, true, m_Shouts[ Utility.Random( m_Shouts.Length ) ] );
                }
            }

            base.OnMovement( m, oldLocation );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( DateTime.Now > m_NextMessage )
            {
                m_NextMessage = DateTime.Now + MessageDelay;
                PublicOverheadMessage( MessageType.Regular, 37, true, m_Shouts[ Utility.Random( m_Shouts.Length ) ] );
            }

            base.OnDoubleClick( from );
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

    [Flipable( 0x3A55, 0x3A56 )]
    public class SantasReindeer1 : BaseReindeer
    {
        [Constructable]
        public SantasReindeer1()
            : this( 0 )
        {
        }

        [Constructable]
        public SantasReindeer1( int hue )
            : base( 0x3A55, hue, NameList.RandomName( "reindeer" ) )
        {
        }

        public SantasReindeer1( Serial serial )
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

            if( String.IsNullOrEmpty( Name ) )
                Name = NameList.RandomName( "reindeer" );

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    [Flipable( 0x3A67, 0x3A68 )]
    public class SantasReindeer2 : BaseReindeer
    {
        [Constructable]
        public SantasReindeer2()
            : this( 0 )
        {
        }

        [Constructable]
        public SantasReindeer2( int hue )
            : base( 0x3A67, hue, NameList.RandomName( "reindeer" ) )
        {
        }

        public SantasReindeer2( Serial serial )
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

            if( String.IsNullOrEmpty( Name ) )
                Name = NameList.RandomName( "reindeer" );

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    [Flipable( 0x3A6F, 0x3A72 )]
    public class SantasReindeer3 : BaseReindeer
    {
        [Constructable]
        public SantasReindeer3()
            : this( 0 )
        {
        }

        [Constructable]
        public SantasReindeer3( int hue )
            : base( 0x3A6F, hue, NameList.RandomName( "reindeer" ) )
        {
        }

        public SantasReindeer3( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version 
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
            if( String.IsNullOrEmpty( Name ) )
                Name = NameList.RandomName( "reindeer" );

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }
}