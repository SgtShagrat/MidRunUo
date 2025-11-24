using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfItemTestingClothing : Bag
    {
        [Constructable]
        public BagOfItemTestingClothing()
        {
            DropItem( new FishnetStockings() );
            DropItem( new HoodedCloak() );
            DropItem( new HoodedRobe() );
            DropItem( new EveningGown() );
            DropItem( new LaceDress() );
            DropItem( new SexyDress() );
            DropItem( new ServantShirt() );
            DropItem( new Gilet() );
            DropItem( new AdventurerTunic() );
            DropItem( new Quiver() );
            DropItem( new Sheath() );
            DropItem( new ShoulderBag() );
            DropItem( new PocketBag() );
            DropItem( new SuperiorKilt() );
            DropItem( new ReinforcedBoots() );
            DropItem( new TearedRobe() );
            DropItem( new Blouse() );
            DropItem( new FurCoat() );
            DropItem( new TwoColouredPants() );
            DropItem( new SharpedShoes() );
            DropItem( new BeltWithBag() );
            DropItem( new Belt() );
            DropItem( new Weistband() );
            DropItem( new AlchemistBag() );
            DropItem( new KiltBag() );
            DropItem( new Monocle() );
            DropItem( new TraditionalGlasses() );

            DropItem( new BandanaWithPearls() );
            DropItem( new TearedShirt() );
            DropItem( new TearedPants() );
            DropItem( new NobleShirt() );
            DropItem( new PiratesJacket() );
            DropItem( new BonesNecklace() );
            DropItem( new PiratesHat() );

            DropItem( new Crown() );
            DropItem( new TraditionalFurCape() );
        }

        #region serialization
        public BagOfItemTestingClothing( Serial serial )
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
}