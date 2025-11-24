using Server.Items;
using Server;

namespace Midgard.Items
{
    /*
     * New players start with practice weapons (newbied weapons that swing 2x faster 
     * and are 2x weaker than regular weapons) depending on what skills they select.
     * First, the arms lore skill could yield you a random practice weapon from the list 
     * of available practice weapons (Shepard's crook, Bow, Gnarled Staff, Spear, Hatchet, 
     * Mace, Longsword, or a Skinning knife), but it was also capable of giving you a 
     * battle axe (practice weapon). It turns out, that arms lore was the only way to 
     * receive a practice battle axe at all, which made the weapon extremely rare once 
     * practice weapons were done away with in early UOR.
     */

    public interface IPracticeWeapon
    {
    }

    [FlipableAttribute( 0xE81, 0xE82 )]
    public class PracticeShepherdsCrook : ShepherdsCrook, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden shepherds crook (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeShepherdsCrook()
        {
            Hue = 0x21E;
            Weight = 7.0;
        }

        #region serialization
        public PracticeShepherdsCrook( Serial serial )
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

    [FlipableAttribute( 0x13F8, 0x13F9 )]
    public class PracticeGnarledStaff : GnarledStaff, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden gnarled staff (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeGnarledStaff()
        {
            Hue = 0x21E;
            Weight = 7.0;
        }

        #region serialization
        public PracticeGnarledStaff( Serial serial )
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

    [FlipableAttribute( 0xF62, 0xF63 )]
    public class PracticeWoodenSpear : Spear, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden spear (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeWoodenSpear()
        {
            Weight = 7.0;
            Hue = 0x21E;
        }

        #region serialization
        public PracticeWoodenSpear( Serial serial )
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

    public class PracticeWoodenLongSword : Longsword, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden longsword (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeWoodenLongSword()
        {
            Weight = 7.0;
            Hue = 0x21E;
        }

        #region serialization
        public PracticeWoodenLongSword( Serial serial )
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

    public class PracticeSkinningKnife : SkinningKnife, IPracticeWeapon
    {
        public override string DefaultName { get { return "a skinning knife (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeSkinningKnife()
        {
            Weight = 7.0;
        }

        #region serialization
        public PracticeSkinningKnife( Serial serial )
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

    [FlipableAttribute( 0x13B2, 0x13B1 )]
    public class PracticeBow : Bow, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden bow (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeBow()
        {
            Weight = 7.0;
            Hue = 0x21E;
            Layer = Layer.TwoHanded;
        }

        #region serialization
        public PracticeBow( Serial serial )
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

    [FlipableAttribute( 0xF43, 0xF44 )]
    public class PracticeWoodenHatchet : Hatchet, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden hatchet (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeWoodenHatchet()
        {
            Weight = 7.0;
            Hue = 0x21E;
        }

        #region serialization
        public PracticeWoodenHatchet( Serial serial )
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

    [FlipableAttribute( 0xF5C, 0xF5D )]
    public class PracticeWoodenMace : Mace, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden mace (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeWoodenMace()
        {
            Weight = 7.0;
            Hue = 0x21E;
        }

        #region serialization
        public PracticeWoodenMace( Serial serial )
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

    // [Flipable( 0xF61, 0xF60 )]
    public class PacticeWoodenLongsword : Longsword, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden longsword (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PacticeWoodenLongsword()
        {
            Hue = 0x21E;
            Weight = 7.0;
        }

        #region serialization
        public PacticeWoodenLongsword( Serial serial )
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

    [FlipableAttribute( 0xEC4, 0xEC5 )]
    public class PacticeSkinningKnife : SkinningKnife, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden skinning knife (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PacticeSkinningKnife()
        {
            Hue = 0x21E;
            Weight = 7.0;
        }

        #region serialization
        public PacticeSkinningKnife( Serial serial )
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

    [FlipableAttribute( 0x1401, 0x1400 )]
    public class PracticeWoodenKryss : Kryss, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden kryss (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeWoodenKryss()
        {
            Weight = 7.0;
            Hue = 0x21E;
        }

        #region serialization
        public PracticeWoodenKryss( Serial serial )
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

        public class PracticeWoodenDagger : Dagger, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden dagger (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeWoodenDagger()
        {
            Hue = 0x21E;
        }

        #region serialization
        public PracticeWoodenDagger( Serial serial )
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

    [FlipableAttribute( 0xF47, 0xF48 )]
    public class PracticeBattleAxe : BattleAxe, IPracticeWeapon
    {
        public override string DefaultName { get { return "a wooden battle axe (practice weapon)"; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeBattleAxe()
        {
            Weight = 7.0;
            Hue = 0x21E;
        }

        #region serialization
        public PracticeBattleAxe( Serial serial )
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

    [FlipableAttribute( 0x13B4, 0x13B3 )]
    public class PracticeClub : Club, IPracticeWeapon
    {
        public override int OldStrengthReq { get { return 1; } }
        public override int OldSpeed { get { return base.OldSpeed * 2; } }

        public override int InitMinHits { get { return (int)( base.InitMinHits * 0.5 ); } }
        public override int InitMaxHits { get { return (int)( base.InitMaxHits * 0.5 ); } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 4; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public PracticeClub()
        {
            Weight = 7.0;
        }

        #region serialization
        public PracticeClub( Serial serial )
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