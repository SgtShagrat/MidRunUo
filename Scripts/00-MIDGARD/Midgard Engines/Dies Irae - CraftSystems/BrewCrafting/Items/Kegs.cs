using System;
using Server;

namespace Midgard.Engines.BrewCrafing
{
    /*** birra ***/
    public abstract class AleKeg : BaseBrewContainerKeg
    {
        public override Type EmptyBottle
        {
            get { return typeof( EmptyAleBottle ); }
        }

        public AleKeg()
        {
        }

        public override BaseBrewContainer FillBottle()
        {
            return new BottleOfAle();
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( Crafter != null )
                    list.Add( Crafter.Name + " Brewery" );
                else
                    list.Add( "Cheap Ale" );
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

            if( !String.IsNullOrEmpty( aleType ) )
            {
                if( Quality == BrewQuality.Exceptional )
                {
                    list.Add( 1060658, String.Format( "{0}\t{1}", "Black Label", aleType ) );
                }
                else
                {
                    list.Add( 1042971, aleType );
                }
            }
        }

        #region serialization

        public AleKeg( Serial serial )
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

    /* CiderKeg... da implementare
    public class CiderKeg : BaseBrewContainerKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.???; }
        }

        public override Type EmptyBottle
        {
            get { return typeof( EmptyJug ); }
        }

        [Constructable]
        public CiderKeg()
        {
        }

        public override BaseBrewContainer FillBottle()
        {
            return new JugOfCider();
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( Crafter != null )
                    list.Add( Crafter.Name + " Brewery" );
                else
                    list.Add( "Hard Cider" );
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

        public CiderKeg( Serial serial )
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

    /*** Idromele... da implementare
    /*
    public class MeadKeg : BaseBrewContainerKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.BitterHops; }
        }

        public override Type EmptyBottle
        {
            get { return typeof( EmptyMeadBottle ); }
        }

        [Constructable]
        public MeadKeg()
        {
        }

        public override BaseBrewContainer FillBottle()
        {
            return new BottleOfMead();
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( Crafter != null )
                    list.Add( Crafter.Name + " Brewery" );
                else
                    list.Add( "Hard Cider" );
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
                list.Add( "Reserve Dark Mead" );
            }
            else
            {
                list.Add( "Hard Mead" );
            }
        }

        #region serialization

        public MeadKeg( Serial serial )
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

    public abstract class WhiskeyKeg : BaseBrewContainerKeg
    {
        public override Type EmptyBottle
        {
            get { return typeof( EmptyWhiskeyBottle ); }
        }

        public WhiskeyKeg()
        {
        }

        public override BaseBrewContainer FillBottle()
        {
            return new BottleOfWhiskey();
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Name == null )
            {
                if( Crafter != null )
                    list.Add( Crafter.Name + " Brewery" );
                else
                    list.Add( "Cheap Whiskey" );
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

            if( !String.IsNullOrEmpty( whiskeyType ) )
            {
                if( Quality == BrewQuality.Exceptional )
                {
                    list.Add( 1060658, String.Format( "{0}\t{1}", "Black Label", whiskeyType ) );
                }
                else
                {
                    list.Add( 1042971, whiskeyType );
                }
            }
        }

        #region serialization

        public WhiskeyKeg( Serial serial )
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

    public abstract class WineKeg : BaseBrewContainerKeg
    {
        public override Type EmptyBottle
        {
            get { return typeof( EmptyWineBottle ); }
        }

        [Constructable]
        public WineKeg()
        {
        }

        public override BaseBrewContainer FillBottle()
        {
            return new BottleOfWine();
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
            base.GetProperties( list ); ;

            string wineType = BrewingResources.GetName( Variety );

            if( !String.IsNullOrEmpty( wineType ) )
            {
                if( Quality == BrewQuality.Exceptional )
                {
                    list.Add( 1060658, String.Format( "{0}\t{1}", "Special Reserve", wineType ) );
                }
                else
                {
                    list.Add( 1042971, wineType );
                }
            }
        }

        #region serialization

        public WineKeg( Serial serial )
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

    #region alekegs

    public class RawAleKeg : Item
    {
        [Constructable]
        public RawAleKeg()
            : base( 0x1940 )
        {
        }

        #region serialization

        public RawAleKeg( Serial serial )
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

    public class StoutAleKeg : AleKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.StoutAle; }
        }

        [Constructable]
        public StoutAleKeg()
        {
            BottleDuration = 3.0;
        }

        #region serialization

        public StoutAleKeg( Serial serial )
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

    public class PorterAleKeg : AleKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.PorterAle; }
        }

        [Constructable]
        public PorterAleKeg()
        {
            BottleDuration = 4.0;
        }

        #region serialization

        public PorterAleKeg( Serial serial )
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

    public class MildAleKeg : AleKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.MildAle; }
        }

        [Constructable]
        public MildAleKeg()
        {
            BottleDuration = 7.0;
        }

        #region serialization

        public MildAleKeg( Serial serial )
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

    public class WinterAleKeg : AleKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.WinterAle; }
        }

        [Constructable]
        public WinterAleKeg()
        {
            BottleDuration = 8.0;
        }

        #region serialization

        public WinterAleKeg( Serial serial )
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

    public class BlondAleKeg : AleKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.BlondAle; }
        }

        [Constructable]
        public BlondAleKeg()
        {
            BottleDuration = 5.0;
        }

        #region serialization

        public BlondAleKeg( Serial serial )
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

    public class TrappisteDoubelAleKeg : AleKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.TrappisteDoubelAle; }
        }

        [Constructable]
        public TrappisteDoubelAleKeg()
        {
            BottleDuration = 9.0;
        }

        #region serialization

        public TrappisteDoubelAleKeg( Serial serial )
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

    public class TrappisteTripelAleKeg : AleKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.TrappisteTripelAle; }
        }

        [Constructable]
        public TrappisteTripelAleKeg()
        {
            BottleDuration = 9.0;
        }

        #region serialization

        public TrappisteTripelAleKeg( Serial serial )
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

    public class PilsenerKeg : AleKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Pilsener; }
        }

        [Constructable]
        public PilsenerKeg()
        {
            BottleDuration = 9.0;
        }

        #region serialization

        public PilsenerKeg( Serial serial )
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

    public class AltBierKeg : AleKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.AltBier; }
        }

        [Constructable]
        public AltBierKeg()
        {
            BottleDuration = 5.0;
        }

        #region serialization

        public AltBierKeg( Serial serial )
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

    public class KarakIdrilKeg : AleKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.KarakIdril; }
        }

        [Constructable]
        public KarakIdrilKeg()
        {
            BottleDuration = 10.0;
        }

        #region serialization

        public KarakIdrilKeg( Serial serial )
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

    #endregion

    #region winekegs

    public class CabernetSauvignonWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.CabernetSauvignon; }
        }

        [Constructable]
        public CabernetSauvignonWineKeg()
        {
        }

        #region serialization

        public CabernetSauvignonWineKeg( Serial serial )
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

    public class ChardonnayWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Chardonnay; }
        }

        [Constructable]
        public ChardonnayWineKeg()
        {
        }

        #region serialization

        public ChardonnayWineKeg( Serial serial )
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

    public class CheninBlancWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.CheninBlanc; }
        }

        [Constructable]
        public CheninBlancWineKeg()
        {
        }

        #region serialization

        public CheninBlancWineKeg( Serial serial )
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

    public class MerlotWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Merlot; }
        }

        [Constructable]
        public MerlotWineKeg()
        {
        }

        #region serialization

        public MerlotWineKeg( Serial serial )
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

    public class PinotNoirWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.PinotNoir; }
        }

        [Constructable]
        public PinotNoirWineKeg()
        {
        }

        #region serialization

        public PinotNoirWineKeg( Serial serial )
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

    public class RieslingWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Riesling; }
        }

        [Constructable]
        public RieslingWineKeg()
        {
        }

        #region serialization

        public RieslingWineKeg( Serial serial )
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

    public class SangioveseWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Sangiovese; }
        }

        [Constructable]
        public SangioveseWineKeg()
        {
        }

        #region serialization

        public SangioveseWineKeg( Serial serial )
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

    public class SauvignonBlancWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.SauvignonBlanc; }
        }

        [Constructable]
        public SauvignonBlancWineKeg()
        {
        }

        #region serialization

        public SauvignonBlancWineKeg( Serial serial )
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

    public class ShirazWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Shiraz; }
        }

        [Constructable]
        public ShirazWineKeg()
        {
        }

        #region serialization

        public ShirazWineKeg( Serial serial )
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

    public class ViognierWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Viognier; }
        }

        [Constructable]
        public ViognierWineKeg()
        {
        }

        #region serialization

        public ViognierWineKeg( Serial serial )
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

    public class ZinfandelWineKeg : WineKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Zinfandel; }
        }

        [Constructable]
        public ZinfandelWineKeg()
        {
        }

        #region serialization

        public ZinfandelWineKeg( Serial serial )
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

    #endregion

    #region whiskeys

    public class BurbonKeg : WhiskeyKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Burbon; }
        }

        [Constructable]
        public BurbonKeg()
        {
            BottleDuration = 10.0;
        }

        #region serialization

        public BurbonKeg( Serial serial )
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

    public class RyeWhiskeyKeg : WhiskeyKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.RyeWhiskey; }
        }

        [Constructable]
        public RyeWhiskeyKeg()
        {
            BottleDuration = 10.0;
        }

        #region serialization

        public RyeWhiskeyKeg( Serial serial )
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

    public class IrishWhiskeyKeg : WhiskeyKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.IrishWhiskey; }
        }

        [Constructable]
        public IrishWhiskeyKeg()
        {
            BottleDuration = 10.0;
        }

        #region serialization

        public IrishWhiskeyKeg( Serial serial )
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

    public class BrandyKeg : WhiskeyKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Brandy; }
        }

        [Constructable]
        public BrandyKeg()
        {
            BottleDuration = 10.0;
        }

        #region serialization

        public BrandyKeg( Serial serial )
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

    public class VodkaKeg : WhiskeyKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Vodka; }
        }

        [Constructable]
        public VodkaKeg()
        {
            BottleDuration = 10.0;
        }

        #region serialization

        public VodkaKeg( Serial serial )
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

    public class GinKeg : WhiskeyKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Gin; }
        }

        [Constructable]
        public GinKeg()
        {
            BottleDuration = 10.0;
        }

        #region serialization

        public GinKeg( Serial serial )
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

    public class RumKeg : WhiskeyKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Rum; }
        }

        [Constructable]
        public RumKeg()
        {
            BottleDuration = 10.0;
        }

        #region serialization

        public RumKeg( Serial serial )
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

    public class ScotchKeg : WhiskeyKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Scotch; }
        }

        [Constructable]
        public ScotchKeg()
        {
            BottleDuration = 10.0;
        }

        #region serialization

        public ScotchKeg( Serial serial )
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

    public class TequilaKeg : WhiskeyKeg
    {
        public override BrewVariety DefaultVariety
        {
            get { return BrewVariety.Tequila; }
        }

        [Constructable]
        public TequilaKeg()
        {
            BottleDuration = 10.0;
        }

        #region serialization

        public TequilaKeg( Serial serial )
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
    #endregion
}