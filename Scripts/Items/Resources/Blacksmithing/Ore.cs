using System;
using Midgard.Engines.AdvancedSmelting;
using Midgard.Items;
using Server.Engines.Craft;
using Server.Targeting;

namespace Server.Items
{
    public abstract class BaseOre : Item, ICommodity
    {
        #region mod by Dies Irae
        public static readonly bool POLWeight = true;

        public override double DefaultWeight
        {
            get { return POLWeight ? 4.0 : 12; }
        }
        #endregion

        private CraftResource m_Resource;

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set { m_Resource = value; InvalidateProperties(); }
        }

        string ICommodity.Description
        {
            get
            {
                return String.Format( "{0} {1} ore", Amount, CraftResources.GetName( m_Resource ).ToLower() );
            }
        }

        int ICommodity.DescriptionNumber { get { return LabelNumber; } }

        public abstract BaseIngot GetIngot();

        #region mod by Dies Irae : pre-aos stuff
        public override void OnSingleClick( Mobile from )
        {
            string format = ( Amount == 1 ? "{0} {1} ore pile" : "{0} {1} ore piles" );

            LabelTo( from, format, Amount, CraftResources.GetName( m_Resource ).ToLower() );
        }
        #endregion

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)2 ); // version

            writer.Write( (int)m_Resource );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 2:
                    if( version < 2 )
                        Weight = DefaultWeight;
                    goto case 1;
                case 1:
                    {
                        m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
                case 0:
                    {
                        OreInfo info;

                        switch( reader.ReadInt() )
                        {
                            case 0:
                                info = OreInfo.Iron;
                                break;
                            case 1:
                                info = OreInfo.DullCopper;
                                break;
                            case 2:
                                info = OreInfo.ShadowIron;
                                break;
                            case 3:
                                info = OreInfo.Copper;
                                break;
                            case 4:
                                info = OreInfo.Bronze;
                                break;
                            case 5:
                                info = OreInfo.Gold;
                                break;
                            case 6:
                                info = OreInfo.Agapite;
                                break;
                            case 7:
                                info = OreInfo.Verite;
                                break;
                            case 8:
                                info = OreInfo.Valorite;
                                break;

                            default:
                                info = null;
                                break;
                        }

                        m_Resource = CraftResources.GetFromOreInfo( info );
                        break;
                    }
            }

            #region mod by Dies Irae
            if( Hue != CraftResources.GetHue( m_Resource ) )
                Hue = CraftResources.GetHue( m_Resource );
            #endregion
        }

        public BaseOre( CraftResource resource )
            : this( resource, 1 )
        {
        }

        public BaseOre( CraftResource resource, int amount )
            : base( 0x19B9 )
        {
            Stackable = true;
            Weight = 12.0;
            Amount = amount;
            Hue = CraftResources.GetHue( resource );

            m_Resource = resource;
        }

        public BaseOre( Serial serial )
            : base( serial )
        {
        }

        /*
        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Amount > 1 )
                list.Add( 1050039, "{0}\t#{1}", Amount, 1026583 ); // ~1_NUMBER~ ~2_ITEMNAME~
            else
                list.Add( 1026583 ); // ore
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( !CraftResources.IsStandard( m_Resource ) )
            {
                int num = CraftResources.GetLocalizationNumber( m_Resource );

                if( num > 0 )
                    list.Add( num );
                else
                    list.Add( CraftResources.GetName( m_Resource ) );
            }
        }
        */

        public override int LabelNumber
        {
            get
            {
                #region mod by Dies Irae
                // if ( m_Resource >= CraftResource.DullCopper && m_Resource <= CraftResource.Valorite )
                //	return 1042845 + (int)(m_Resource - CraftResource.DullCopper);

                if( m_Resource >= CraftResource.DullCopper && m_Resource <= CraftResource.Aqua )
                    return 1042845 + (int)( m_Resource - CraftResource.Aqua );
                #endregion

                return 1042853; // iron ore;
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            if( from.InRange( this.GetWorldLocation(), 2 ) )
            {
                from.SendLocalizedMessage( 501971 ); // Select the forge on which to smelt the ore, or another pile of ore with which to combine it.
                from.Target = new InternalTarget( this );
            }
            else
            {
                from.SendLocalizedMessage( 501976 ); // The ore is too far away.
            }
        }

        private class InternalTarget : Target
        {
            private BaseOre m_Ore;

            public InternalTarget( BaseOre ore )
                : base( 2, false, TargetFlags.None )
            {
                m_Ore = ore;
            }

            private bool IsForge( object obj )
            {
                if ( Core.ML && obj is Mobile && ( (Mobile)obj ).IsDeadBondedPet )
                    return false;

                #region mod by Dies Irae
                if( obj is AdvancedForge )
                    return true;
                #endregion

                int itemID = 0;

                if( obj is Item )
                    itemID = ( (Item)obj ).ItemID;
                else if( obj is StaticTarget )
                    itemID = ( (StaticTarget)obj ).ItemID & 0x3FFF;

                return ( itemID == 4017 || ( itemID >= 6522 && itemID <= 6569 ) );
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Ore.Deleted )
                    return;

                if( !from.InRange( m_Ore.GetWorldLocation(), 2 ) )
                {
                    from.SendLocalizedMessage( 501976 ); // The ore is too far away.
                    return;
                }

                if( IsForge( targeted ) )
                {
                    double difficulty = 50.0;
                    CraftResourceInfo info = CraftResources.GetInfo( m_Ore.Resource );
                    if( info != null )
                        difficulty = info.AttributeInfo.OldSmeltingRequiredSkill;

                    /*
                    switch( m_Ore.Resource )
                    {
                        default:
                            difficulty = 50.0;
                            break;
					    case CraftResource.DullCopper: difficulty = 65.0; break;
					    case CraftResource.ShadowIron: difficulty = 70.0; break;
					    case CraftResource.Copper: difficulty = 75.0; break;
					    case CraftResource.Bronze: difficulty = 80.0; break;
					    case CraftResource.Gold: difficulty = 85.0; break;
					    case CraftResource.Agapite: difficulty = 90.0; break;
					    case CraftResource.Verite: difficulty = 95.0; break;
					    case CraftResource.Valorite: difficulty = 99.0; break;
                    }
                    */

                    double minSkill = difficulty - 25.0;
                    double maxSkill = difficulty + 25.0;

                    if( difficulty > 50.0 && difficulty > from.Skills[ SkillName.Mining ].Value )
                    {
                        from.SendLocalizedMessage( 501986 ); // You have no idea how to smelt this strange ore!
                        return;
                    }

                    if( from.CheckTargetSkill( SkillName.Mining, targeted, minSkill, maxSkill ) )
                    {
                        int toConsume = m_Ore.Amount;

                        if( toConsume <= 0 )
                        {
                            from.SendLocalizedMessage( 501987 ); // There is not enough metal-bearing ore in this pile to make an ingot.
                        }
                        else
                        {
                            if( toConsume > 30000 )
                                toConsume = 30000;

                            BaseIngot ingot = m_Ore.GetIngot();
                            ingot.Amount = toConsume * 2;

                            m_Ore.Consume( toConsume );
                            
                            if( from.Body.Type == BodyType.Human && !from.Mounted )
                                from.Animate( Utility.RandomList( 10, 14 ), 5, 1, true, false, 0 );

                            Timer.DelayCall( TimeSpan.FromSeconds( 0.7 ), delegate
                                                                           {
                                                                               from.PlaySound( 0x2B );
                                                                               from.AddToBackpack( ingot );
                                                                           } );

                            from.SendLocalizedMessage( 501988 ); // You smelt the ore removing the impurities and put the metal in your backpack.
                        }
                    }
                    else if( m_Ore.Amount < 2 )
                    {
                        from.SendLocalizedMessage( 501989 ); // You burn away the impurities but are left with no useable metal.
                        m_Ore.Delete();
                    }
                    else
                    {
                        from.SendLocalizedMessage( 501990 ); // You burn away the impurities but are left with less useable metal.
                        m_Ore.Amount /= 2;
                    }
                }
            }


        }
    }

    public class IronOre : BaseOre
    {
        [Constructable]
        public IronOre()
            : this( 1 )
        {
        }

        [Constructable]
        public IronOre( int amount )
            : base( CraftResource.Iron, amount )
        {
        }

        public IronOre( Serial serial )
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



        public override BaseIngot GetIngot()
        {
            return new IronIngot();
        }
    }

    public class DullCopperOre : BaseOre
    {
        [Constructable]
        public DullCopperOre()
            : this( 1 )
        {
        }

        [Constructable]
        public DullCopperOre( int amount )
            : base( Core.AOS ? CraftResource.DullCopper : CraftResource.OldDullCopper, amount )
        {
        }

        public DullCopperOre( Serial serial )
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



        public override BaseIngot GetIngot()
        {
            return new DullCopperIngot();
        }
    }

    public class ShadowIronOre : BaseOre
    {
        [Constructable]
        public ShadowIronOre()
            : this( 1 )
        {
        }

        [Constructable]
        public ShadowIronOre( int amount )
            : base( Core.AOS ? CraftResource.ShadowIron : CraftResource.OldShadowIron, amount )
        {
        }

        public ShadowIronOre( Serial serial )
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



        public override BaseIngot GetIngot()
        {
            return new ShadowIronIngot();
        }
    }

    public class CopperOre : BaseOre
    {
        [Constructable]
        public CopperOre()
            : this( 1 )
        {
        }

        [Constructable]
        public CopperOre( int amount )
            : base( Core.AOS ? CraftResource.Copper : CraftResource.OldCopper, amount )
        {
        }

        public CopperOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new CopperIngot();
        }
    }

    public class BronzeOre : BaseOre
    {
        [Constructable]
        public BronzeOre()
            : this( 1 )
        {
        }

        [Constructable]
        public BronzeOre( int amount )
            : base( Core.AOS ? CraftResource.Bronze : CraftResource.OldBronze, amount )
        {
        }

        public BronzeOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new BronzeIngot();
        }
    }

    public class GoldOre : BaseOre
    {
        [Constructable]
        public GoldOre()
            : this( 1 )
        {
        }

        [Constructable]
        public GoldOre( int amount )
            : base( Core.AOS ? CraftResource.Gold : CraftResource.OldGold, amount )
        {
        }

        public GoldOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new GoldIngot();
        }
    }

    public class AgapiteOre : BaseOre
    {
        [Constructable]
        public AgapiteOre()
            : this( 1 )
        {
        }

        [Constructable]
        public AgapiteOre( int amount )
            : base( Core.AOS ? CraftResource.Agapite : CraftResource.OldAgapite, amount )
        {
        }

        public AgapiteOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new AgapiteIngot();
        }
    }

    public class VeriteOre : BaseOre
    {
        [Constructable]
        public VeriteOre()
            : this( 1 )
        {
        }

        [Constructable]
        public VeriteOre( int amount )
            : base( Core.AOS ? CraftResource.Verite : CraftResource.OldVerite, amount )
        {
        }

        public VeriteOre( Serial serial )
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



        public override BaseIngot GetIngot()
        {
            return new VeriteIngot();
        }
    }

    public class ValoriteOre : BaseOre
    {
        [Constructable]
        public ValoriteOre()
            : this( 1 )
        {
        }

        [Constructable]
        public ValoriteOre( int amount )
            : base( Core.AOS ? CraftResource.Valorite : CraftResource.OldValorite, amount )
        {
        }

        public ValoriteOre( Serial serial )
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



        public override BaseIngot GetIngot()
        {
            return new ValoriteIngot();
        }
    }

    #region mod by Dies Irae
    public class PlatinumOre : BaseOre
    {
        [Constructable]
        public PlatinumOre()
            : this( 1 )
        {
        }

        [Constructable]
        public PlatinumOre( int amount )
            : base( Core.AOS ? CraftResource.Platinum : CraftResource.OldPlatinum, amount )
        {
        }

        public PlatinumOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new PlatinumIngot();
        }
    }

    public class TitaniumOre : BaseOre
    {
        [Constructable]
        public TitaniumOre()
            : this( 1 )
        {
        }

        [Constructable]
        public TitaniumOre( int amount )
            : base( Core.AOS ? CraftResource.Titanium : CraftResource.OldTitanium, amount )
        {
        }

        public TitaniumOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new TitaniumIngot();
        }
    }

    public class ObsidianOre : BaseOre
    {
        [Constructable]
        public ObsidianOre()
            : this( 1 )
        {
        }

        [Constructable]
        public ObsidianOre( int amount )
            : base( Core.AOS ? CraftResource.Obsidian : CraftResource.OldObsidian, amount )
        {
        }

        public ObsidianOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new ObsidianIngot();
        }
    }

    public class DarkRubyOre : BaseOre
    {
        [Constructable]
        public DarkRubyOre()
            : this( 1 )
        {
        }

        [Constructable]
        public DarkRubyOre( int amount )
            : base( Core.AOS ? CraftResource.DarkRuby : CraftResource.OldDarkRuby, amount )
        {
        }

        public DarkRubyOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new DarkRubyIngot();
        }
    }

    public class EbonSapphireOre : BaseOre
    {
        [Constructable]
        public EbonSapphireOre()
            : this( 1 )
        {
        }

        [Constructable]
        public EbonSapphireOre( int amount )
            : base( Core.AOS ? CraftResource.EbonSapphire : CraftResource.OldEbonSapphire, amount )
        {
        }

        public EbonSapphireOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new EbonSapphireIngot();
        }
    }

    public class RadiantDiamondOre : BaseOre
    {
        [Constructable]
        public RadiantDiamondOre()
            : this( 1 )
        {
        }

        [Constructable]
        public RadiantDiamondOre( int amount )
            : base( Core.AOS ? CraftResource.RadiantDiamond : CraftResource.OldRadiantDiamond, amount )
        {
        }

        public RadiantDiamondOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new RadiantDiamondIngot();
        }
    }

    public class EldarOre : BaseOre
    {
        public EldarOre()
            : this( 1 )
        {
        }

        public EldarOre( int amount )
            : base( CraftResource.Eldar, amount )
        {
        }

        public EldarOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new EldarIngot();
        }
    }

    public class CrystalineOre : BaseOre
    {
        public CrystalineOre()
            : this( 1 )
        {
        }

        public CrystalineOre( int amount )
            : base( CraftResource.Crystaline, amount )
        {
        }

        public CrystalineOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new CrystalineIngot();
        }
    }

    public class VulcanOre : BaseOre
    {
        public VulcanOre()
            : this( 1 )
        {
        }

        public VulcanOre( int amount )
            : base( CraftResource.Vulcan, amount )
        {
        }

        public VulcanOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new VulcanIngot();
        }
    }

    public class AquaOre : BaseOre
    {
        public AquaOre()
            : this( 1 )
        {
        }

        public AquaOre( int amount )
            : base( CraftResource.Aqua, amount )
        {
        }

        public AquaOre( Serial serial )
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

        public override BaseIngot GetIngot()
        {
            return new AquaIngot();
        }
    }
    #endregion

    #region mod by Dies Irae : pre-Aos stuff
    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class GraphiteOre : BaseOre
    {
        [Constructable]
        public GraphiteOre()
            : this( 1 )
        {
        }

        [Constructable]
        public GraphiteOre( int amount )
            : base( CraftResource.OldGraphite, amount )
        {
        }

        public override BaseIngot GetIngot()
        {
            return new GraphiteIngot();
        }

        #region serialization
        public GraphiteOre( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class PyriteOre : BaseOre
    {
        [Constructable]
        public PyriteOre()
            : this( 1 )
        {
        }

        [Constructable]
        public PyriteOre( int amount )
            : base( CraftResource.OldPyrite, amount )
        {
        }

        #region serialization
        public PyriteOre( Serial serial )
            : base( serial )
        {
        }

        public override BaseIngot GetIngot()
        {
            return new PyriteIngot();
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class AzuriteOre : BaseOre
    {
        [Constructable]
        public AzuriteOre()
            : this( 1 )
        {
        }

        [Constructable]
        public AzuriteOre( int amount )
            : base( CraftResource.OldAzurite, amount )
        {
        }

        public override BaseIngot GetIngot()
        {
            return new AzuriteIngot();
        }

        #region serialization
        public AzuriteOre( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class VanadiumOre : BaseOre
    {
        [Constructable]
        public VanadiumOre()
            : this( 1 )
        {
        }

        [Constructable]
        public VanadiumOre( int amount )
            : base( CraftResource.OldVanadium, amount )
        {
        }

        public override BaseIngot GetIngot()
        {
            return new VanadiumIngot();
        }

        #region serialization
        public VanadiumOre( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class SilverOre : BaseOre
    {
        [Constructable]
        public SilverOre()
            : this( 1 )
        {
        }

        [Constructable]
        public SilverOre( int amount )
            : base( CraftResource.OldSilver, amount )
        {
        }

        public override BaseIngot GetIngot()
        {
            return new SilverIngot();
        }

        #region serialization
        public SilverOre( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class AmethystOre : BaseOre
    {
        [Constructable]
        public AmethystOre()
            : this( 1 )
        {
        }

        [Constructable]
        public AmethystOre( int amount )
            : base( CraftResource.OldAmethyst, amount )
        {
        }

        public override BaseIngot GetIngot()
        {
            return new AmethystIngot();
        }

        #region serialization
        public AmethystOre( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class XenianOre : BaseOre
    {
        [Constructable]
        public XenianOre()
            : this( 1 )
        {
        }

        [Constructable]
        public XenianOre( int amount )
            : base( CraftResource.OldXenian, amount )
        {
        }

        public override BaseIngot GetIngot()
        {
            return new XenianIngot();
        }

        #region serialization
        public XenianOre( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class RubidianOre : BaseOre
    {
        [Constructable]
        public RubidianOre()
            : this( 1 )
        {
        }

        [Constructable]
        public RubidianOre( int amount )
            : base( CraftResource.OldRubidian, amount )
        {
        }

        public override BaseIngot GetIngot()
        {
            return new RubidianIngot();
        }

        #region serialization
        public RubidianOre( Serial serial )
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
        #endregion
    }
    #endregion
}