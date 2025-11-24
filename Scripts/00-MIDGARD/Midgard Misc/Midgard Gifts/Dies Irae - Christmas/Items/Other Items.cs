/***************************************************************************
 *                               Christmas2010
 *                            -------------------
 *   begin                : 24 dicembre, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.Events.Items
{
    public class SnowGlobe : Item
    {
        private ChristmasHelper.SnowScenes m_Type;

        [CommandProperty( AccessLevel.GameMaster )]
        public ChristmasHelper.SnowScenes Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        [Constructable]
        public SnowGlobe()
            : base( 0xE2D )
        {
            m_Type = (ChristmasHelper.SnowScenes)Utility.Random( Enum.GetNames( typeof( ChristmasHelper.SnowScenes ) ).Length );
            LootType = LootType.Blessed;
            Name = "A Midgard Snow Globe";
        }

        public SnowGlobe( Serial serial )
            : base( serial )
        {
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            if( Utility.InsensitiveCompare( Name, "A Midgard Snow Globe" ) == 0 )
            {
                string scene = ChristmasHelper.GetSceneName( m_Type );

                if( !String.IsNullOrEmpty( scene ) )
                {
                    from.SendMessage( "Hey, you notice some words on the globe! " );
                    from.PlaySound( 0x1FA );
                    Effects.SendLocationEffect( this, Map, 14201, 16 );
                    Name = String.Format( "a snowy scene of {0}", scene );
                }
                return;
            }

            PublicOverheadMessage( MessageType.Regular, Utility.RandomRedHue(), true, ChristmasHelper.GetMidgardGreetings() );

            base.OnDoubleClick( from );
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.WriteEncodedInt( (int)m_Type );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Type = (ChristmasHelper.SnowScenes)reader.ReadEncodedInt();

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    [Flipable( 0x2328, 0x2329 )]
    public class Snowman : Item, IDyable
    {
        private static readonly TimeSpan MessageDelay = TimeSpan.FromSeconds( 5.0 );

        private DateTime m_NextMessage;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public ChristmasHelper.MidgardStaff Type { get; set; }

        [Constructable]
        public Snowman()
            : this( Utility.RandomDyedHue(), ChristmasHelper.GetRandomStaffMember() )
        {
        }

        [Constructable]
        public Snowman( int hue )
            : this( hue, ChristmasHelper.GetRandomStaffMember() )
        {
        }

        [Constructable]
        public Snowman( ChristmasHelper.MidgardStaff type )
            : this( Utility.RandomDyedHue(), type )
        {
        }

        [Constructable]
        public Snowman( int hue, ChristmasHelper.MidgardStaff type )
            : base( Utility.Random( 0x2328, 2 ) )
        {
            Weight = 10.0;
            LootType = LootType.Blessed;

            Hue = hue;
            Type = type;

            Name = "a midgard snowman";
        }

        public Snowman( Serial serial )
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

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( ChristmasHelper.StaticMidgardGreetings );
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        private static readonly string[] m_Shouts = new string[]
                                                   {
                                                   "Ho Ho Ho! Sto congelando!",
                                                   "Hey! Tu! Ma non hai freddo?",
                                                   "Brrrrrrrrrrr",
                                                   "Buon Natale su Midgard!",
                                                   "Ma cosa mi avete messo al posto del naso!?",
                                                   "Via con quella lampada, mi sciolgo!",
                                                   "Niente scherzi eh, non fare flame!"
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
            if( Utility.InsensitiveCompare( Name, "a midgard snowman" ) == 0 )
            {
                string name = ChristmasHelper.GetStaffMember( Type );

                if( !String.IsNullOrEmpty( name ) )
                {
                    from.SendMessage( "The snowman seems to be someone you know..." );
                    from.PlaySound( 0x1FA );
                    Effects.SendLocationEffect( this, Map, 14201, 16 );
                    Name = String.Format( "{0}, the Snowman", name );
                    InvalidateProperties();
                }
                return;
            }

            PublicOverheadMessage( MessageType.Regular, 37, true, m_Shouts[ Utility.Random( m_Shouts.Length ) ] );

            base.OnDoubleClick( from );
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
            writer.WriteEncodedInt( (int)Type );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
            Type = (ChristmasHelper.MidgardStaff)reader.ReadEncodedInt();
            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    public class SnowPile : Item
    {
        [Constructable]
        public SnowPile()
            : base( 0x913 )
        {
            Hue = 0x481;
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public override int LabelNumber { get { return 1005578; } } // a pile of snow

        public SnowPile( Serial serial )
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

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
                from.SendLocalizedMessage( 1042010 ); // You must have the object in your backpack to use it.
            else if( from.CanBeginAction( typeof( SnowPile ) ) )
            {
                from.SendLocalizedMessage( 1005575 ); // You carefully pack the snow into a ball...
                from.Target = new SnowTarget();
            }
            else
                from.SendLocalizedMessage( 1005574 ); // The snow is not ready to be packed yet.  Keep trying.
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;

            public InternalTimer( Mobile from )
                : base( TimeSpan.FromSeconds( 5.0 ) )
            {
                m_From = from;
            }

            protected override void OnTick()
            {
                m_From.EndAction( typeof( SnowPile ) );
            }
        }

        private class SnowTarget : Target
        {
            public SnowTarget()
                : base( 10, false, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object target )
            {
                if( target == from )
                {
                    from.SendLocalizedMessage( 1005576 ); // You can't throw this at yourself.
                }
                else if( target is Mobile )
                {
                    Mobile targ = (Mobile)target;
                    Container pack = targ.Backpack;

                    if( pack != null && pack.FindItemByType( new Type[] { typeof( SnowPile ), typeof( ChristmasPileOfGlacialSnow ) } ) != null )
                    {
                        if( from.BeginAction( typeof( SnowPile ) ) )
                        {
                            new InternalTimer( from ).Start();

                            from.PlaySound( 0x145 );

                            from.Animate( 9, 1, 1, true, false, 0 );

                            targ.SendLocalizedMessage( 1010572 ); // You have just been hit by a snowball!
                            from.SendLocalizedMessage( 1010573 ); // You throw the snowball and hit the target!

                            Effects.SendMovingEffect( from, targ, 0x36E4, 7, 0, false, true, 0x480, 0 );
                        }
                        else
                        {
                            from.SendLocalizedMessage( 1005574 ); // The snow is not ready to be packed yet.  Keep trying.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage( 1005577 ); // You can only throw a snowball at something that can throw one back.
                    }
                }
                else
                {
                    from.SendLocalizedMessage( 1005577 ); // You can only throw a snowball at something that can throw one back.
                }
            }
        }
    }

    [FlipableAttribute( 0x236E, 0x2371 )]
    public class LightOfTheWinterSolstice : Item
    {
        private ChristmasHelper.MidgardStaff m_Type;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public ChristmasHelper.MidgardStaff Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        [Constructable]
        public LightOfTheWinterSolstice()
            : base( 0x236E )
        {
            m_Type = (ChristmasHelper.MidgardStaff)Utility.Random( Enum.GetNames( typeof( ChristmasHelper.MidgardStaff ) ).Length );

            Weight = 1.0;
            Hue = 1154;
            Light = LightType.Circle300;
            LootType = LootType.Blessed;

            Name = "light of the 2010 winter solstice";
        }

        public LightOfTheWinterSolstice( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            string name = ChristmasHelper.GetStaffMember( m_Type );

            if( !String.IsNullOrEmpty( name ) )
                LabelTo( from, 1070881, name ); // Hand Dipped by ~1_name~

            LabelTo( from, "Christmas 2010" );
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            if( Utility.InsensitiveCompare( Name, "light of the 2010 winter solstice" ) == 0 )
            {
                string name = ChristmasHelper.GetStaffMember( m_Type );

                if( !String.IsNullOrEmpty( name ) )
                {
                    from.SendMessage( "You see some words on the lightsource..." );
                    from.PlaySound( 0x1FA );
                    Effects.SendLocationEffect( this, Map, 14201, 16 );
                    Name = String.Format( "light of the winter solstice, gifted by {0}", name );
                    Hue = Utility.RandomDyedHue();
                    InvalidateProperties();
                }
                return;
            }

            PublicOverheadMessage( MessageType.Regular, Utility.RandomRedHue(), true, ChristmasHelper.GetMidgardGreetings() );

            base.OnDoubleClick( from );
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.WriteEncodedInt( (int)m_Type );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Type = (ChristmasHelper.MidgardStaff)reader.ReadEncodedInt();

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    [Furniture]
    [Flipable( 0x2BDB, 0x2BDC )]
    public class RedStockin : BaseContainer
    {
        public override int MaxWeight { get { return 0; } }
        public override int DefaultDropSound { get { return 0x42; } }
        public override int DefaultGumpID { get { return 0x103; } }

        [Constructable]
        public RedStockin()
            : base( Utility.Random( 0x2BDB, 2 ) )
        {
            Weight = 1.0;
        }

        public RedStockin( Serial serial )
            : base( serial )
        {
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 37, true, ChristmasHelper.GetMidgardGreetings() );

            base.OnDoubleClick( from );
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

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    public class GreenStockin : Bag
    {
        public override int MaxWeight { get { return 0; } }
        public override int DefaultDropSound { get { return 0x42; } }

        [Constructable]
        public GreenStockin()
        {
            Movable = true;
            GumpID = 259;
            Weight = 1.0;
            ItemID = Utility.RandomList( 0x2BD9, 0x2BDA ); //Green Stockin
        }

        public GreenStockin( Serial serial )
            : base( serial )
        {
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }
        public override bool DisplaysContent { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 37, true, ChristmasHelper.GetMidgardGreetings() );

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

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    public class GingerbreadHouseAddonDeed : MiniHouseDeed
    {
        [Constructable]
        public GingerbreadHouseAddonDeed()
            : base( MiniHouseType.GingerbreadHouse )
        {
            LootType = LootType.Blessed;
        }

        public GingerbreadHouseAddonDeed( Serial serial )
            : base( serial )
        {
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

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

    #region clothing
    public class ReggisenoNatalizio2010 : BaseShirt
    {
        public override bool AllowMaleWearer { get { return false; } }

        [Constructable]
        public ReggisenoNatalizio2010()
            : this( 0 )
        {
        }

        [Constructable]
        public ReggisenoNatalizio2010( int hue )
            : base( 5150, hue )
        {
            Name = "Holiday Brassiere";
            LootType = LootType.Blessed;
            Weight = 2.0;
        }

        public ReggisenoNatalizio2010( Serial serial )
            : base( serial )
        {
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

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
    }

    public class CappellinoNatalizio2010 : BaseHat
    {
        public override int InitMinHits { get { return 19; } }
        public override int InitMaxHits { get { return 38; } }

        [Constructable]
        public CappellinoNatalizio2010()
            : this( 0 )
        {
        }

        [Constructable]
        public CappellinoNatalizio2010( int hue )
            : base( 5149, hue )
        {
            Name = "Holiday Cap";
            LootType = LootType.Blessed;
            Weight = 2.0;
        }

        public CappellinoNatalizio2010( Serial serial )
            : base( serial )
        {
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public bool Validate( Mobile m )
        {
            return true;
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

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
    }

    public class GonnellinoNatalizio2010 : BaseOuterLegs
    {
        public override bool AllowMaleWearer { get { return false; } }

        [Constructable]
        public GonnellinoNatalizio2010()
            : this( 0 )
        {
        }

        [Constructable]
        public GonnellinoNatalizio2010( int hue )
            : base( 5151, hue )
        {
            Name = "Holiday Skirt";
            LootType = LootType.Blessed;
            Weight = 2.0;
        }

        public GonnellinoNatalizio2010( Serial serial )
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

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public bool Validate( Mobile m )
        {
            if( m == null || !m.Player || !m.Female )
                return true;
            return true;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
    }
    #endregion

    #region snowflakes
    public class BlueSnowflake2010 : Item
    {
        [Constructable]
        public BlueSnowflake2010()
            : base( 0x232E )
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public BlueSnowflake2010( Serial serial )
            : base( serial )
        {
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 37, true, ChristmasHelper.GetMidgardGreetings() );

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

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    public class WhiteSnowflake2010 : Item
    {
        [Constructable]
        public WhiteSnowflake2010()
            : base( 0x232F )
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public WhiteSnowflake2010( Serial serial )
            : base( serial )
        {
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 37, true, ChristmasHelper.GetMidgardGreetings() );

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

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }
    #endregion

    #region poinsettias
    public class RedPoinsettia2010 : Item
    {
        [Constructable]
        public RedPoinsettia2010()
            : base( 0x2330 )
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public RedPoinsettia2010( Serial serial )
            : base( serial )
        {
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 37, true, ChristmasHelper.GetMidgardGreetings() );

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

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }

    public class WhitePoinsettia2010 : Item
    {
        [Constructable]
        public WhitePoinsettia2010()
            : base( 0x2331 )
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public WhitePoinsettia2010( Serial serial )
            : base( serial )
        {
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, 37, true, ChristmasHelper.GetMidgardGreetings() );

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

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }
    #endregion
}