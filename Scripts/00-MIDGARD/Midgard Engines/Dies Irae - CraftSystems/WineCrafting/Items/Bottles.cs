namespace Midgard.Engines.WineCrafting
{
    /*
    public abstract class BaseCraftWine : Item, ICraftable
    {
        private Mobile m_Poisoner;
        private Poison m_Poison;
        private int m_FillFactor;
        private Mobile m_Crafter;
        private WineQuality m_Quality;
        private GrapeVariety m_Variety;

        public virtual Item EmptyItem
        {
            get { return null; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Poisoner
        {
            get { return m_Poisoner; }
            set { m_Poisoner = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Poison Poison
        {
            get { return m_Poison; }
            set { m_Poison = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int FillFactor
        {
            get { return m_FillFactor; }
            set { m_FillFactor = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public GrapeVariety Variety
        {
            get
            {
                return m_Variety;
            }
            set
            {
                if( m_Variety != value )
                {
                    m_Variety = value;

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; InvalidateProperties(); }
        }


        [CommandProperty( AccessLevel.GameMaster )]
        public WineQuality Quality
        {
            get { return m_Quality; }
            set { m_Quality = value; InvalidateProperties(); }
        }

        #region serialization
        public BaseCraftWine( Serial serial )
            : base( serial )
        {
        }

        private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
        {
            if( setIf )
                flags |= toSet;
        }

        private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
        {
            return ( ( flags & toGet ) != 0 );
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            Crafter = 0x00000001,
            Quality = 0x00000002,
            Variety = 0x00000004
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)2 ); // version

            //Version 2
            SaveFlag flags = SaveFlag.None;
            SetSaveFlag( ref flags, SaveFlag.Crafter, m_Crafter != null );
            SetSaveFlag( ref flags, SaveFlag.Quality, m_Quality != WineQuality.Regular );
            SetSaveFlag( ref flags, SaveFlag.Variety, m_Variety != DefaultVariety );

            writer.WriteEncodedInt( (int)flags );

            if( GetSaveFlag( flags, SaveFlag.Crafter ) )
                writer.Write( (Mobile)m_Crafter );
            if( GetSaveFlag( flags, SaveFlag.Quality ) )
                writer.WriteEncodedInt( (int)m_Quality );
            if( GetSaveFlag( flags, SaveFlag.Variety ) )
                writer.WriteEncodedInt( (int)m_Variety );

            //Version 1
            writer.Write( m_Poisoner );

            Poison.Serialize( m_Poison, writer );
            writer.Write( m_FillFactor );

        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 2:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        if( GetSaveFlag( flags, SaveFlag.Crafter ) )
                            m_Crafter = reader.ReadMobile();

                        if( GetSaveFlag( flags, SaveFlag.Quality ) )
                            m_Quality = (WineQuality)reader.ReadEncodedInt();
                        else
                            m_Quality = WineQuality.Regular;
                        if( m_Quality == WineQuality.Low )
                            m_Quality = WineQuality.Regular;

                        if( GetSaveFlag( flags, SaveFlag.Variety ) )
                            m_Variety = (GrapeVariety)reader.ReadEncodedInt();
                        else
                            m_Variety = DefaultVariety;

                        if( m_Variety == GrapeVariety.None )
                            m_Variety = DefaultVariety;

                        //break;
                        goto case 1;
                    }
                case 1:
                    {
                        m_Poisoner = reader.ReadMobile();

                        goto case 0;
                    }
                case 0:
                    {
                        m_Poison = Poison.Deserialize( reader );
                        m_FillFactor = reader.ReadInt();
                        break;
                    }
            }
        }
        #endregion

        public virtual GrapeVariety DefaultVariety { get { return GrapeVariety.CabernetSauvignon; } }

        public BaseCraftWine( int itemID )
            : base( itemID )
        {
            m_Quality = WineQuality.Regular;
            m_Crafter = null;

            m_Variety = DefaultVariety;

            FillFactor = 4;
        }

        public void Drink( Mobile from )
        {
            if( Thirsty( from, m_FillFactor ) )
            {
                // Play a random drinking sound
                from.PlaySound( Utility.Random( 0x30, 2 ) );

                if( from.Body.IsHuman && !from.Mounted )
                    from.Animate( 34, 5, 1, true, false, 0 );

                if( m_Poison != null )
                    from.ApplyPoison( m_Poisoner, m_Poison );

                int bac = 5;
                from.BAC += bac;
                if( from.BAC > 60 )
                    from.BAC = 60;

                BaseBeverage.CheckHeaveTimer( from );

                Consume();

                Item item = EmptyItem;

                if( item != null )
                    from.AddToBackpack( item );
            }
        }

        public static bool Thirsty( Mobile from, int fillFactor )
        {
            if( from.Thirst >= 20 )
            {
                from.SendMessage( "You are simply too full to drink any more!" );
                return false;
            }

            int iThirst = from.Thirst + fillFactor;
            if( iThirst >= 20 )
            {
                from.Thirst = 20;
                from.SendMessage( "You manage to drink the beverage, but you are full!" );
            }
            else
            {
                from.Thirst = iThirst;

                if( iThirst < 5 )
                    from.SendMessage( "You drink the beverage, but are still extremely thirsty." );
                else if( iThirst < 10 )
                    from.SendMessage( "You drink the beverage, and begin to feel more satiated." );
                else if( iThirst < 15 )
                    from.SendMessage( "After drinking the beverage, you feel much less thirsty." );
                else
                    from.SendMessage( "You feel quite full after consuming the beverage." );
            }

            return true;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            if( from.InRange( GetWorldLocation(), 1 ) )
                Drink( from );
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( m_Crafter != null )
                    list.Add( m_Crafter.Name + " Vineyards" );
                else
                    list.Add( "Cheap Table Wine" );
            }
            else
            {
                list.Add( Name );
            }
        }

        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            string wineType;
            wineType = WinemakingResourceInfo.GetName( m_Variety );

            if( m_Quality == WineQuality.Exceptional )
            {
                list.Add( 1060847, "Special Reserve\t{0}", wineType );
            }
            else
            {
                list.Add( 1060847, "\t{0}", wineType );
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            string wineType;

            if( Name == null )
            {
                if( m_Crafter != null )
                    LabelTo( from, "{0} Vinyards", m_Crafter.Name );
                else
                    LabelTo( from, "Cheap Table Wine" );
            }
            else
            {
                LabelTo( from, "{0}", Name );
            }

            wineType = WinemakingResourceInfo.GetName( m_Variety );

            if( m_Quality == WineQuality.Exceptional )
            {
                LabelTo( from, "Special Reserve {0}", wineType );
            }
            else
            {
                LabelTo( from, "{0}", wineType );
            }
        }

        #region ICraftable Members
        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
        {
            Quality = (WineQuality)quality;

            if( makersMark )
                Crafter = from;

            Item[] items = from.Backpack.FindItemsByType( typeof( VinyardLabelMaker ) );

            if( items.Length != 0 )
            {
                foreach( VinyardLabelMaker lm in items )
                {
                    if( lm.VinyardName != null )
                    {
                        Name = lm.VinyardName;
                        break;
                    }
                }
            }

            Type resourceType = typeRes;

            if( resourceType == null )
                resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

            Variety = WinemakingResourceInfo.GetFromType( resourceType );
            Hue = 0;

            return quality;
        }
        #endregion
    }
    */

    /*
    public class EmptyWineBottle : Item
    {
        [Constructable]
        public EmptyWineBottle()
            : this( 1 )
        {
        }

        [Constructable]
        public EmptyWineBottle( int amount )
            : base( 0x99B )
        {
            Stackable = true;
            Weight = 1.0;
            Name = "Empty Wine Bottle";
            Amount = amount;
        }

        public EmptyWineBottle( Serial serial )
            : base( serial )
        {
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
        }
    }
    */

    /*
    public class WineKeg : Item, ICraftable
    {
        public static readonly TimeSpan CheckDelay = TimeSpan.FromDays( 7.0 );

        private int m_Held;
        private Mobile m_Crafter;
        private WineQuality m_Quality;
        private GrapeVariety m_Variety;
        private DateTime m_Start;
        private double m_BottleDuration;
        private bool m_AllowBottling;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Held
        {
            get
            {
                return m_Held;
            }
            set
            {
                if( m_Held != value )
                {
                    Weight += ( value - m_Held ) * 0.8;

                    m_Held = value;
                    InvalidateProperties();
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public GrapeVariety Variety
        {
            get
            {
                return m_Variety;
            }
            set
            {
                if( m_Variety != value )
                {
                    m_Variety = value;

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public WineQuality Quality
        {
            get { return m_Quality; }
            set { m_Quality = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowBottling
        {
            get
            {
                if( !m_AllowBottling )
                    m_AllowBottling = ( 0 >= TimeSpan.Compare( TimeSpan.FromDays( m_BottleDuration ),
                        DateTime.Now.Subtract( m_Start ) ) );
                return m_AllowBottling;
            }
            set { m_AllowBottling = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double BottleDuration
        {
            get { return m_BottleDuration; }
            set { m_BottleDuration = value; }
        }

        public WineKeg( Serial serial )
            : base( serial )
        {
        }

        private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
        {
            if( setIf )
                flags |= toSet;
        }

        private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
        {
            return ( ( flags & toGet ) != 0 );
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            Held = 0x00000001,
            Crafter = 0x00000002,
            Quality = 0x00000004,
            Variety = 0x00000008
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)2 ); // version

            //version 2
            writer.Write( (DateTime)m_Start );
            writer.Write( (double)m_BottleDuration );
            writer.Write( (bool)m_AllowBottling );

            //version 1
            SaveFlag flags = SaveFlag.None;

            SetSaveFlag( ref flags, SaveFlag.Held, m_Held != 0 );
            SetSaveFlag( ref flags, SaveFlag.Crafter, m_Crafter != null );
            SetSaveFlag( ref flags, SaveFlag.Quality, m_Quality != WineQuality.Regular );
            SetSaveFlag( ref flags, SaveFlag.Variety, m_Variety != DefaultVariety );

            writer.WriteEncodedInt( (int)flags );

            if( GetSaveFlag( flags, SaveFlag.Held ) )
                writer.Write( (int)m_Held );
            if( GetSaveFlag( flags, SaveFlag.Crafter ) )
                writer.Write( (Mobile)m_Crafter );
            if( GetSaveFlag( flags, SaveFlag.Quality ) )
                writer.WriteEncodedInt( (int)m_Quality );
            if( GetSaveFlag( flags, SaveFlag.Variety ) )
                writer.WriteEncodedInt( (int)m_Variety );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 2:
                    {
                        m_Start = reader.ReadDateTime();
                        m_BottleDuration = reader.ReadDouble();
                        m_AllowBottling = reader.ReadBool();

                        goto case 1;
                    }
                case 1:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        if( GetSaveFlag( flags, SaveFlag.Held ) )
                            m_Held = reader.ReadInt();

                        if( GetSaveFlag( flags, SaveFlag.Crafter ) )
                            m_Crafter = reader.ReadMobile();

                        if( GetSaveFlag( flags, SaveFlag.Quality ) )
                            m_Quality = (WineQuality)reader.ReadEncodedInt();
                        else
                            m_Quality = WineQuality.Regular;

                        if( m_Quality == WineQuality.Low )
                            m_Quality = WineQuality.Regular;

                        if( GetSaveFlag( flags, SaveFlag.Variety ) )
                            m_Variety = (GrapeVariety)reader.ReadEncodedInt();
                        else
                            m_Variety = DefaultVariety;

                        if( m_Variety == GrapeVariety.None )
                            m_Variety = DefaultVariety;

                        break;
                    }
            }
        }

        public virtual GrapeVariety DefaultVariety { get { return GrapeVariety.CabernetSauvignon; } }


        [Constructable]
        public WineKeg()
            : base( 0x1940 )
        {
            Weight = 1.0;

            m_Held = 75;
            m_Quality = WineQuality.Regular;
            m_Crafter = null;
            m_Variety = DefaultVariety;
            m_BottleDuration = 7.0;
            m_AllowBottling = false;
            m_Start = DateTime.Now;
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( m_Crafter != null )
                    list.Add( m_Crafter.Name + " Vineyards" );
                else
                    list.Add( "Cheap Table Wine" );
            }
            else
            {
                list.Add( Name );
            }
        }

        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            string wineType;
            wineType = WinemakingResourceInfo.GetName( m_Variety );

            if( m_Quality == WineQuality.Exceptional )
            {
                list.Add( 1060847, "Special Reserve\t{0}", wineType );
            }
            else
            {
                list.Add( 1060847, "\t{0}", wineType );
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            int number;

            if( m_Held <= 0 )
                number = 502246; // The keg is empty.
            else if( m_Held < 5 )
                number = 502248; // The keg is nearly empty.
            else if( m_Held < 10 )
                number = 502249; // The keg is not very full.
            else if( m_Held < 18 )
                number = 502250; // The keg is about one quarter full.
            else if( m_Held < 25 )
                number = 502251; // The keg is about one third full.
            else if( m_Held < 32 )
                number = 502252; // The keg is almost half full.
            else if( m_Held < 38 )
                number = 502254; // The keg is approximately half full.
            else if( m_Held < 45 )
                number = 502253; // The keg is more than half full.
            else if( m_Held < 56 )
                number = 502255; // The keg is about three quarters full.
            else if( m_Held < 64 )
                number = 502256; // The keg is very full.
            else if( m_Held < 75 )
                number = 502257; // The liquid is almost to the top of the keg.
            else
                number = 502258; // The keg is completely full.

            list.Add( number );
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            int number;
            string wineType;

            if( Name == null )
            {
                if( m_Crafter != null )
                    LabelTo( from, "{0} Vinyards", m_Crafter.Name );
                else
                    LabelTo( from, "Cheap Table Wine" );
            }
            else
            {
                LabelTo( from, "{0}", Name );
            }

            wineType = WinemakingResourceInfo.GetName( m_Variety );

            if( m_Quality == WineQuality.Exceptional )
            {
                LabelTo( from, "Special Reserve {0}", wineType );
            }
            else
            {
                LabelTo( from, "{0}", wineType );
            }

            if( m_Held <= 0 )
                number = 502246; // The keg is empty.
            else if( m_Held < 5 )
                number = 502248; // The keg is nearly empty.
            else if( m_Held < 10 )
                number = 502249; // The keg is not very full.
            else if( m_Held < 18 )
                number = 502250; // The keg is about one quarter full.
            else if( m_Held < 25 )
                number = 502251; // The keg is about one third full.
            else if( m_Held < 32 )
                number = 502252; // The keg is almost half full.
            else if( m_Held < 38 )
                number = 502254; // The keg is approximately half full.
            else if( m_Held < 45 )
                number = 502253; // The keg is more than half full.
            else if( m_Held < 56 )
                number = 502255; // The keg is about three quarters full.
            else if( m_Held < 64 )
                number = 502256; // The keg is very full.
            else if( m_Held < 75 )
                number = 502257; // The liquid is almost to the top of the keg.
            else
                number = 502258; // The keg is completely full.

            LabelTo( from, number );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( AllowBottling )
            {
                if( from.InRange( GetWorldLocation(), 2 ) )
                {
                    if( m_Held > 0 )
                    {
                        Container pack = from.Backpack;

                        if( pack != null && pack.ConsumeTotal( typeof( EmptyWineBottle ), 1 ) )
                        {
                            from.SendLocalizedMessage( 502242 ); // You pour some of the keg's contents into an empty bottle...

                            BaseCraftWine wine = FillBottle();
                            wine.Crafter = m_Crafter;
                            wine.Quality = m_Quality;
                            wine.Variety = m_Variety;
                            if( Name != null )
                                wine.Name = Name;

                            if( pack.TryDropItem( from, wine, false ) )
                            {
                                from.SendLocalizedMessage( 502243 ); // ...and place it into your backpack.
                                from.PlaySound( 0x240 );

                                if( --Held == 0 )
                                {
                                    Delete();

                                    if( GiveKeg( from ) )
                                    {
                                        from.SendMessage( "The Keg is empty and you clean it for reuse" );
                                    }
                                    else
                                    {
                                        from.SendMessage( "The Keg is now empty and cannot be reused." );
                                    }
                                }
                            }
                            else
                            {
                                from.SendLocalizedMessage( 502244 ); // ...but there is no room for the bottle in your backpack.
                                wine.Delete();
                            }
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage( 502246 ); // The keg is empty.
                    }
                }
                else
                {
                    from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
                }
            }
            else
            {
                from.SendMessage( "This keg is not ready to bottle yet, the fermentation process is not yet complete." );
            }
        }

        public bool GiveKeg( Mobile m )
        {
            Container pack = m.Backpack;

            Keg keg = new Keg();

            if( pack == null || !pack.TryDropItem( m, keg, false ) )
            {
                keg.Delete();
                return false;
            }

            return true;
        }

        public BaseCraftWine FillBottle()
        {
            switch( m_Variety )
            {
                default: return new BottleOfWine();
            }
        }

        public static void Initialize()
        {
            TileData.ItemTable[ 0x1940 ].Height = 4;
        }

        #region ICraftable Members
        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
        {
            Held = 75;

            Quality = (WineQuality)quality;

            if( makersMark )
                Crafter = from;

            Item[] items = from.Backpack.FindItemsByType( typeof( VinyardLabelMaker ) );

            if( items.Length != 0 )
            {
                foreach( VinyardLabelMaker lm in items )
                {
                    if( lm.VinyardName != null )
                    {
                        Name = lm.VinyardName;
                        break;
                    }
                }
            }

            Type resourceType = typeRes;

            if( resourceType == null )
                resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

            Variety = WinemakingResourceInfo.GetFromType( resourceType );

            CraftContext context = craftSystem.GetContext( from );

            Hue = 0;

            BottleDuration = 7.0;
            AllowBottling = false;
            m_Start = DateTime.Now;

            return quality;
        }
        #endregion
    }
    */

    /*
    public class BottleOfWine : BaseCraftWine
    {
        public override Item EmptyItem { get { return new EmptyWineBottle(); } }

        [Constructable]
        public BottleOfWine()
            : base( 0x9C7 )
        {
            Weight = 0.2;
            FillFactor = 4;
        }

        public BottleOfWine( Serial serial )
            : base( serial )
        {
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
        }
    }
     */
}