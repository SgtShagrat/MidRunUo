using Midgard.Engines.AdvancedCooking;
using Server.Engines.Craft;

namespace Server.Items
{
    public class BakersBoard : BaseTool
    {
        public override CraftSystem CraftSystem
        {
            get { return DefBaking.CraftSystem; }
        }

        [Constructable]
        public BakersBoard()
            : base( 0x14EA )
        {
            Name = "Baker's Board";
            Weight = 4.0;
        }

        [Constructable]
        public BakersBoard( int uses )
            : base( uses, 0x14EA )
        {
            Name = "Baker's Board";
            Weight = 4.0;
        }

        public BakersBoard( Serial serial )
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

    public class CooksCauldron : BaseTool
    {
        public override CraftSystem CraftSystem
        {
            get { return DefBoiling.CraftSystem; }
        }

        [Constructable]
        public CooksCauldron()
            : base( 0x9ED )
        {
            Name = "Cook's Cauldron";
            Weight = 4.0;
        }

        [Constructable]
        public CooksCauldron( int uses )
            : base( uses, 0x9ED )
        {
            Name = "Cook's Cauldron";
            Weight = 4.0;
        }

        public CooksCauldron( Serial serial )
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

    public class FryingPan : BaseTool
    {
        public override CraftSystem CraftSystem
        {
            get { return DefGrilling.CraftSystem; }
        }

        [Constructable]
        public FryingPan()
            : base( 0x9E2 )
        {
            Name = "Frying Pan";
            Weight = 3.0;
        }

        [Constructable]
        public FryingPan( int uses )
            : base( uses, 0x9E2 )
        {
            Name = "Frying Pan";
            Weight = 3.0;
        }

        public FryingPan( Serial serial )
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