using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    public abstract class BaseServantOfTocasia : BaseCreature
    {
        public BaseServantOfTocasia()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
        }

        #region serialization
        public BaseServantOfTocasia( Serial serial )
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
        #endregion

        public override bool CanRummageCorpses
        {
            get { return true; }
        }

        public override bool CanMoveOverObstacles
        {
            get { return true; }
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }

        public override void AlterMeleeDamageTo( Mobile to, ref int damage )
        {
            if( !to.Player )
                damage = damage * 10;

            base.AlterMeleeDamageTo( to, ref damage );
        }
    }

    public class LesserServantOfTocasiaA : BaseServantOfTocasia
    {
        [Constructable]
        public LesserServantOfTocasiaA()
        {
            Name = "a devoured soul of " + NameList.RandomName( "male" );
            Body = 3;
            BaseSoundID = 224;
            Hue = Utility.RandomList( 1401, 1454 );

            SetStr( 161, 185 );
            SetDex( 41, 65 );
            SetInt( 46, 70 );

            SetHits( 97, 111 );

            SetDamage( 15, 21 );

            SetDamageType( ResistanceType.Physical, 85 );
            SetDamageType( ResistanceType.Poison, 15 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 30, 40 );

            SetSkill( SkillName.MagicResist, 40.1, 55.0 );
            SetSkill( SkillName.Tactics, 45.1, 70.0 );
            SetSkill( SkillName.Wrestling, 50.1, 70.0 );

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 24;
        }

        #region serialization
        public LesserServantOfTocasiaA( Serial serial )
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
        #endregion
    }

    public class LesserServantOfTocasiaB : BaseServantOfTocasia
    {
        [Constructable]
        public LesserServantOfTocasiaB()
        {
            Name = string.Format( "a zombie in {0} status of decomposition", Utility.RandomStringList( "medium", "advanced", "critical" ) );
            Body = 3;
            BaseSoundID = 224;
            Hue = Utility.RandomList( 1401, 1454 );

            SetStr( 126, 150 );
            SetDex( 76, 100 );
            SetInt( 86, 110 );

            SetHits( 76, 90 );

            SetDamage( 10, 14 );

            SetDamageType( ResistanceType.Physical, 20 );
            SetDamageType( ResistanceType.Cold, 60 );
            SetDamageType( ResistanceType.Poison, 20 );

            SetResistance( ResistanceType.Physical, 50, 60 );
            SetResistance( ResistanceType.Fire, 25, 30 );
            SetResistance( ResistanceType.Cold, 70, 80 );
            SetResistance( ResistanceType.Poison, 30, 40 );
            SetResistance( ResistanceType.Energy, 40, 50 );

            SetSkill( SkillName.MagicResist, 70.1, 95.0 );
            SetSkill( SkillName.Tactics, 45.1, 70.0 );
            SetSkill( SkillName.Wrestling, 50.1, 70.0 );

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 19;
        }

        #region serialization
        public LesserServantOfTocasiaB( Serial serial )
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
        #endregion
    }

    public class MediumServantOfTocasiaA : BaseServantOfTocasia
    {
        [Constructable]
        public MediumServantOfTocasiaA()
        {
            Name = "a skeletal body of a lady named " + NameList.RandomName( "female" );
            Body = 147;
            BaseSoundID = 451;

            SetStr( 196, 250 );
            SetDex( 76, 95 );
            SetInt( 36, 60 );

            SetHits( 118, 150 );

            SetDamage( 8, 18 );

            SetDamageType( ResistanceType.Physical, 40 );
            SetDamageType( ResistanceType.Cold, 60 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 20, 30 );
            SetResistance( ResistanceType.Cold, 50, 60 );
            SetResistance( ResistanceType.Poison, 20, 30 );
            SetResistance( ResistanceType.Energy, 30, 40 );

            SetSkill( SkillName.MagicResist, 65.1, 80.0 );
            SetSkill( SkillName.Tactics, 85.1, 100.0 );
            SetSkill( SkillName.Wrestling, 85.1, 95.0 );

            Fame = 3000;
            Karma = -3000;

            VirtualArmor = 40;
        }

        #region serialization
        public MediumServantOfTocasiaA( Serial serial )
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
        #endregion
    }

    public class MediumServantOfTocasiaB : BaseServantOfTocasia
    {
        [Constructable]
        public MediumServantOfTocasiaB()
        {
            Name = "a ghost without peace";
            Body = 26;
            Hue = 0x4001;
            BaseSoundID = 0x482;

            SetStr( 196, 250 );
            SetDex( 76, 95 );
            SetInt( 36, 60 );

            SetHits( 200, 250 );

            SetDamage( 15, 30 );

            SetDamageType( ResistanceType.Physical, 20 );
            SetDamageType( ResistanceType.Cold, 80 );

            SetResistance( ResistanceType.Physical, 55, 65 );
            SetResistance( ResistanceType.Fire, 50, 70 );
            SetResistance( ResistanceType.Cold, 50, 60 );
            SetResistance( ResistanceType.Poison, 20, 30 );
            SetResistance( ResistanceType.Energy, 30, 40 );

            SetSkill( SkillName.MagicResist, 65.1, 80.0 );
            SetSkill( SkillName.Tactics, 85.1, 100.0 );
            SetSkill( SkillName.Wrestling, 85.1, 95.0 );

            Fame = 3000;
            Karma = -3000;

            VirtualArmor = 40;

            PackItem( new Scimitar() );
            PackItem( new Arrow( 10 ) );
            PackItem( new WoodenShield() );
        }

        #region serialization
        public MediumServantOfTocasiaB( Serial serial )
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
        #endregion

        public override Poison HitPoison
        {
            get { return ( 0.8 >= Utility.RandomDouble() ? Poison.Deadly : Poison.Lethal ); }
        }
    }

    public class HardServantOfTocasiaA : BaseServantOfTocasia
    {
        [Constructable]
        public HardServantOfTocasiaA()
        {
            AI = AIType.AI_Necromage;

            Name = "an apprentice servant of Tocasia";
            Body = 24;
            BaseSoundID = 0x3E9;
            Hue = Utility.RandomRedHue();

            SetStr( 171, 200 );
            SetDex( 126, 145 );
            SetInt( 276, 305 );

            SetHits( 103, 120 );

            SetDamage( 24, 26 );

            SetDamageType( ResistanceType.Physical, 10 );
            SetDamageType( ResistanceType.Cold, 40 );
            SetDamageType( ResistanceType.Energy, 50 );

            SetResistance( ResistanceType.Physical, 40, 60 );
            SetResistance( ResistanceType.Fire, 20, 30 );
            SetResistance( ResistanceType.Cold, 50, 60 );
            SetResistance( ResistanceType.Poison, 55, 65 );
            SetResistance( ResistanceType.Energy, 40, 50 );

            SetSkill( SkillName.EvalInt, 100.0 );
            SetSkill( SkillName.Magery, 70.1, 80.0 );
            SetSkill( SkillName.Meditation, 85.1, 95.0 );
            SetSkill( SkillName.MagicResist, 80.1, 100.0 );
            SetSkill( SkillName.Tactics, 70.1, 90.0 );

            SetSkill( SkillName.Necromancy, 150.0 );
            SetSkill( SkillName.SpiritSpeak, 150.0 );

            Fame = 8000;
            Karma = -8000;

            VirtualArmor = 50;
            PackItem( new GnarledStaff() );
            PackNecroReg( 17, 24 );
        }

        #region serialization
        public HardServantOfTocasiaA( Serial serial )
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
        #endregion

        public override bool CanAreaDamage
        {
            get { return true; }
        }
    }

    public class HardServantOfTocasiaB : BaseServantOfTocasia
    {
        [Constructable]
        public HardServantOfTocasiaB()
        {
            Name = "an wrong experiment of Tocasia";
            Body = 155;
            BaseSoundID = 471;

            SetStr( 301, 350 );
            SetDex( 75 );
            SetInt( 151, 200 );

            SetHits( 1200 );
            SetStam( 150 );
            SetMana( 0 );

            SetDamage( 8, 10 );

            SetDamageType( ResistanceType.Physical, 0 );
            SetDamageType( ResistanceType.Cold, 50 );
            SetDamageType( ResistanceType.Poison, 50 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 20, 30 );
            SetResistance( ResistanceType.Cold, 50, 70 );
            SetResistance( ResistanceType.Poison, 40, 50 );
            SetResistance( ResistanceType.Energy, 20, 30 );

            SetSkill( SkillName.Poisoning, 120.0 );
            SetSkill( SkillName.MagicResist, 250.0 );
            SetSkill( SkillName.Tactics, 100.0 );
            SetSkill( SkillName.Wrestling, 90.1, 100.0 );

            Fame = 6000;
            Karma = -6000;

            VirtualArmor = 40;
        }

        #region serialization
        public HardServantOfTocasiaB( Serial serial )
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
        #endregion

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override Poison HitPoison
        {
            get { return Poison.Lethal; }
        }

        public override bool CanAreaPoison
        {
            get { return true; }
        }
    }

    public class DefiantServantOfTocasia : BaseServantOfTocasia
    {
        [Constructable]
        public DefiantServantOfTocasia()
        {
            AI = AIType.AI_Necromage;

            Name = "a crazy mage fallen under Tocasia";
            Body = 770;
            BaseSoundID = 224;
            Hue = Utility.RandomOrangeHue();

			SetStr( 216, 305 );
			SetDex( 96, 115 );
			SetInt( 966, 1045 );

			SetHits( 560, 595 );

			SetDamage( 15, 27 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 40 );
			SetDamageType( ResistanceType.Energy, 40 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 25, 30 );

			SetSkill( SkillName.EvalInt, 120.1, 130.0 );
			SetSkill( SkillName.Magery, 120.1, 130.0 );
			SetSkill( SkillName.Meditation, 100.1, 101.0 );
			SetSkill( SkillName.Poisoning, 100.1, 101.0 );
			SetSkill( SkillName.MagicResist, 175.2, 200.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 75.1, 100.0 );

            SetSkill( SkillName.Necromancy, 150.0 );
            SetSkill( SkillName.SpiritSpeak, 150.0 );

            Fame = 23000;
			Karma = -23000;

			VirtualArmor = 60;
			PackNecroReg( 30, 275 );
        }

        #region serialization
        public DefiantServantOfTocasia( Serial serial )
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
        #endregion
    }
}