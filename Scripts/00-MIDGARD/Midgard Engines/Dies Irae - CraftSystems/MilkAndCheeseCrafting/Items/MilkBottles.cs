using System;
using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Engines.CheeseCrafting
{
    public abstract class MilkBottle : Item
    {
        public virtual MilkTypes DefaultMilkType
        {
            get { return MilkTypes.Cow; }
        }

        private Mobile m_Poisoner;
        private Poison m_Poison;
        private int m_FillFactor;
        private Mobile m_Crafter;
        private CheeseQuality m_Quality;
        private MilkTypes m_MilkType;

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
        public MilkTypes MilkType
        {
            get { return m_MilkType; }
            set
            {
                if( m_MilkType != value )
                {
                    m_MilkType = value;

                    InvalidateProperties();
                }
            }
        }

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

        [CommandProperty( AccessLevel.GameMaster )]
        public CheeseQuality Quality
        {
            get { return m_Quality; }
            set
            {
                m_Quality = value;
                InvalidateProperties();
            }
        }

        public MilkBottle( int itemID, Mobile crafter, CheeseQuality quality )
            : base( itemID )
        {
            m_Quality = quality;
            m_Crafter = crafter;
            m_MilkType = DefaultMilkType;

            FillFactor = 4;
        }

        public MilkBottle( int itemID )
            : this( itemID, null, CheeseQuality.Regular )
        {
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

        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            if( m_Quality == CheeseQuality.Exceptional )
            {
                if( m_Crafter != null )
                    list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

                list.Add( 1060847, "Quality\tExceptional" );
            }
        }

        #region serialization

        public MilkBottle( Serial serial )
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
            MilkType = 0x00000004
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            SaveFlag flags = SaveFlag.None;
            SetSaveFlag( ref flags, SaveFlag.Crafter, m_Crafter != null );
            SetSaveFlag( ref flags, SaveFlag.Quality, m_Quality != CheeseQuality.Regular );
            SetSaveFlag( ref flags, SaveFlag.MilkType, m_MilkType != DefaultMilkType );

            writer.WriteEncodedInt( (int)flags );

            if( GetSaveFlag( flags, SaveFlag.Crafter ) )
                writer.Write( m_Crafter );
            if( GetSaveFlag( flags, SaveFlag.Quality ) )
                writer.WriteEncodedInt( (int)m_Quality );
            if( GetSaveFlag( flags, SaveFlag.MilkType ) )
                writer.WriteEncodedInt( (int)m_MilkType );

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
                case 0:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        if( GetSaveFlag( flags, SaveFlag.Crafter ) )
                            m_Crafter = reader.ReadMobile();

                        if( GetSaveFlag( flags, SaveFlag.Quality ) )
                            m_Quality = (CheeseQuality)reader.ReadEncodedInt();
                        else
                            m_Quality = CheeseQuality.Regular;

                        if( m_Quality == CheeseQuality.Low )
                            m_Quality = CheeseQuality.Regular;

                        if( GetSaveFlag( flags, SaveFlag.MilkType ) )
                            m_MilkType = (MilkTypes)reader.ReadEncodedInt();
                        else
                            m_MilkType = DefaultMilkType;

                        if( m_MilkType == MilkTypes.None )
                            m_MilkType = DefaultMilkType;

                        break;
                    }
            }

            m_Poisoner = reader.ReadMobile();
            m_Poison = Poison.Deserialize( reader );
            m_FillFactor = reader.ReadInt();
        }

        #endregion

        #region ICraftable Members

        /*
        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes,
                           BaseTool tool, CraftItem craftItem, int resHue )
        {
            m_Quality = (CheeseQuality)quality;

            if( makersMark )
                m_Crafter = from;

            Type resourceType = typeRes;

            if( resourceType == null )
                resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

            // TODO: implementare un CheesemakingResourceInfo
            //m_MilkType = WinemakingResourceInfo.GetFromType( resourceType );
            Hue = 0;

            return quality;
        }
        */
        #endregion
    }

    public class CowMilkBottle : MilkBottle
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        public CowMilkBottle( Mobile crafter, CheeseQuality quality )
            : base( 0x0f09, crafter, quality )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of cow milk";

            MilkType = MilkTypes.Cow;
        }

        [Constructable]
        public CowMilkBottle()
            : this( null, CheeseQuality.Regular )
        {
        }

        public CowMilkBottle( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class GoatMilkBottle : MilkBottle
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public GoatMilkBottle( Mobile crafter, CheeseQuality quality )
            : base( 0x0f09, crafter, quality )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of goat milk";

            MilkType = MilkTypes.Goat;
        }

        [Constructable]
        public GoatMilkBottle()
            : this( null, CheeseQuality.Regular )
        {
        }

        public GoatMilkBottle( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class SheepMilkBottle : MilkBottle
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public SheepMilkBottle( Mobile crafter, CheeseQuality quality )
            : base( 0x0f09, crafter, quality )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of sheep milk";

            MilkType = MilkTypes.Sheep;
        }

        [Constructable]
        public SheepMilkBottle()
            : this( null, CheeseQuality.Regular )
        {
        }

        public SheepMilkBottle( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}