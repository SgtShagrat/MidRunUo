using System;
using Server;
using Server.Engines.Craft;
using Server.Items;
using Poison = Server.Poison;
using System.Text;

namespace Midgard.Engines.BrewCrafing
{
    public abstract class BaseBrewContainer : Item, ICraftable
    {
        private Mobile m_Poisoner;
        private Poison m_Poison;
        private int m_FillFactor;
        private Mobile m_Crafter;
        private BrewQuality m_Quality;
        private BrewVariety m_Variety;

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
        public BrewVariety Variety
        {
            get { return m_Variety; }
            set
            {
                if( m_Variety != value )
                {
                    m_Variety = value;

                    InvalidateProperties();
                }
            }
        }

        /*
        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }
        */

        [CommandProperty( AccessLevel.GameMaster )]
        public BrewQuality Quality
        {
            get { return m_Quality; }
            set
            {
                m_Quality = value;
                InvalidateProperties();
            }
        }

        public BaseBrewContainer( Serial serial )
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

            writer.Write( 0 ); // version

            //Version 0
            SaveFlag flags = SaveFlag.None;
            SetSaveFlag( ref flags, SaveFlag.Crafter, m_Crafter != null );
            SetSaveFlag( ref flags, SaveFlag.Quality, m_Quality != BrewQuality.Regular );
            SetSaveFlag( ref flags, SaveFlag.Variety, m_Variety != DefaultVariety );

            writer.WriteEncodedInt( (int)flags );

            if( GetSaveFlag( flags, SaveFlag.Crafter ) )
                writer.Write( m_Crafter );
            if( GetSaveFlag( flags, SaveFlag.Quality ) )
                writer.WriteEncodedInt( (int)m_Quality );
            if( GetSaveFlag( flags, SaveFlag.Variety ) )
                writer.WriteEncodedInt( (int)m_Variety );

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
                case 0:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        if( GetSaveFlag( flags, SaveFlag.Crafter ) )
                            m_Crafter = reader.ReadMobile();

                        if( GetSaveFlag( flags, SaveFlag.Quality ) )
                            m_Quality = (BrewQuality)reader.ReadEncodedInt();
                        else
                            m_Quality = BrewQuality.Regular;
                        if( m_Quality == BrewQuality.Low )
                            m_Quality = BrewQuality.Regular;

                        if( GetSaveFlag( flags, SaveFlag.Variety ) )
                            m_Variety = (BrewVariety)reader.ReadEncodedInt();
                        else
                            m_Variety = DefaultVariety;

                        if( m_Variety == BrewVariety.None )
                            m_Variety = DefaultVariety;

                        m_Poisoner = reader.ReadMobile();

                        m_Poison = Poison.Deserialize( reader );
                        m_FillFactor = reader.ReadInt();
                        break;
                    }
            }
        }

        public abstract BrewVariety DefaultVariety { get; }

        public BaseBrewContainer( int itemID )
            : base( itemID )
        {
            m_Quality = BrewQuality.Regular;
            m_Crafter = null;

            m_Variety = DefaultVariety;

            FillFactor = 4;
        }

        public virtual void Drink( Mobile from )
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

        #region ICraftable Members

        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes,
                           BaseTool tool, CraftItem craftItem, int resHue )
        {
            Quality = (BrewQuality)quality;

            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            Item[] items = from.Backpack.FindItemsByType( typeof( BreweryLabelMaker ) );

            if( items.Length != 0 )
            {
                foreach( BreweryLabelMaker lm in items )
                {
                    if( lm.BreweryName != null )
                    {
                        Name = lm.BreweryName;
                        break;
                    }
                }
            }

            Type resourceType = typeRes;

            if( resourceType == null )
                resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

            Variety = BrewingResources.GetFromType( resourceType );

            CraftContext context = craftSystem.GetContext( from );

            if( context != null && context.DoNotColor )
                Hue = 0;

            return quality;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }

        #endregion
    }

    public abstract class BaseCraftAle : BaseBrewContainer
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.StoutAle; }
        }

        public BaseCraftAle( int itemID )
            : base( itemID )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( Crafter != null )
                    list.Add( Crafter.Name + " Brewery" );
                else
                    list.Add( "Ale" );
            }
            else
            {
                list.Add( Name );
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            string aleType = BrewingResources.GetName( Variety );

            if( Quality == BrewQuality.Exceptional )
            {
                list.Add( 1060847, "Black Label\t{0}", aleType );
            }
            else
            {
                list.Add( 1060847, "\t{0}", aleType );
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            StringBuilder info = new StringBuilder();

            if( String.IsNullOrEmpty( Name ) )
            {
                if( Crafter != null )
                    info.Append( Crafter.Name + " Brewery" );
                else
                    info.Append( "Ale" );
            }
            else
                info.Append( Name );

            LabelTo( from, info.ToString() );

            string aleType = BrewingResources.GetName( Variety );

            if( Quality == BrewQuality.Exceptional )
                LabelTo( from, "Black Label: {0}", aleType );
            else
                LabelTo( from, aleType );

            base.OnSingleClick( from );
        }

        #region serialization

        public BaseCraftAle( Serial serial )
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

    /*
    public abstract class BaseCraftCider : BaseBrewContainer
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.BitterHops; }
        }

        public BaseCraftCider( int itemID )
            : base( itemID )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( Crafter != null )
                    list.Add( Crafter.Name + " Brewery" );
                else
                    list.Add( "Keg of Cider" );
            }
            else
            {
                list.Add( Name );
            }
        }

        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            if( Quality == BrewQuality.Exceptional )
            {
                list.Add( "Reserve Dark Cider" );
            }
            else
            {
                list.Add( "Hard Cider" );
            }
        }

        #region serialization

        public BaseCraftCider( Serial serial )
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
    */

    /*
    public abstract class BaseCraftCocktail : BaseBrewContainer
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Yeast; }
        }

        public BaseCraftCocktail( int itemID )
            : base( itemID )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( Crafter != null )
                    list.Add( Crafter.Name + " Brewery" );
                else
                    list.Add( "Cocktail" );
            }
            else
            {
                list.Add( Name );
            }
        }

        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            string cocktailType = BrewingResources.GetName( Variety );

            if( Quality == BrewQuality.Exceptional )
            {
                list.Add( 1060847, "Black Label\t{0} Cocktail", cocktailType );
            }
            else
            {
                list.Add( 1060847, "\t{0}", cocktailType );
            }
        }

        #region serialization

        public BaseCraftCocktail( Serial serial )
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
    */

    /*
    public abstract class BaseCraftMead : BaseBrewContainer
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.BitterHops; }
        }

        public BaseCraftMead( int itemID )
            : base( itemID )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( Crafter != null )
                    list.Add( Crafter.Name + " Brewery" );
                else
                    list.Add( "Mead" );
            }
            else
            {
                list.Add( Name );
            }
        }

        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            string meadType = BrewingResources.GetName( Variety );

            if( Quality == BrewQuality.Exceptional )
            {
                list.Add( 1060847, "Black Label\t{0} Mead", meadType );
            }
            else
            {
                list.Add( 1060847, "\t{0}", meadType );
            }
        }

        #region serialization

        public BaseCraftMead( Serial serial )
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
    */

    public abstract class BaseCraftWhiskey : BaseBrewContainer
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Burbon; }
        }

        public BaseCraftWhiskey( int itemID )
            : base( itemID )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( Crafter != null )
                    list.Add( Crafter.Name + " Brewery" );
                else
                    list.Add( "Whiskey" );
            }
            else
            {
                list.Add( Name );
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            string whiskeyType = BrewingResources.GetName( Variety );

            if( Quality == BrewQuality.Exceptional )
            {
                list.Add( 1060847, "Black Label\t{0}", whiskeyType );
            }
            else
            {
                list.Add( 1060847, "\t{0}", whiskeyType );
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            StringBuilder info = new StringBuilder();

            if( String.IsNullOrEmpty( Name ) )
            {
                if( Crafter != null )
                    info.Append( Crafter.Name + " Brewery" );
                else
                    info.Append( "Whiskey" );
            }
            else
                info.Append( Name );

            LabelTo( from, info.ToString() );

            string aleType = BrewingResources.GetName( Variety );

            if( Quality == BrewQuality.Exceptional )
                LabelTo( from, "Black Label: {0}", aleType );
            else
                LabelTo( from, aleType );

            base.OnSingleClick( from );
        }

        #region serialization

        public BaseCraftWhiskey( Serial serial )
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

    public abstract class BaseCraftWine : BaseBrewContainer
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.CabernetSauvignon; }
        }

        public BaseCraftWine( int itemID )
            : base( itemID )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( Crafter != null )
                    list.Add( Crafter.Name + " Vineyards" );
                else
                    list.Add( "Cheap Table Wine" );
            }
            else
            {
                list.Add( Name );
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            string wineType = BrewingResources.GetName( Variety );

            if( Quality == BrewQuality.Exceptional )
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
            StringBuilder info = new StringBuilder();

            if( String.IsNullOrEmpty( Name ) )
            {
                if( Crafter != null )
                    info.Append( Crafter.Name + " Brewery" );
                else
                    info.Append( "Whiskey" );
            }
            else
                info.Append( Name );

            LabelTo( from, info.ToString() );

            string aleType = BrewingResources.GetName( Variety );

            if( Quality == BrewQuality.Exceptional )
                LabelTo( from, "Special Reserve: {0}", aleType );
            else
                LabelTo( from, aleType );

            base.OnSingleClick( from );
        }

        #region serialization

        public BaseCraftWine( Serial serial )
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