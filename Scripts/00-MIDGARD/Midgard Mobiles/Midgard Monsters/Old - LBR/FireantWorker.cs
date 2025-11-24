namespace Server.Mobiles
{
    [CorpseName( "an fire ant workers corpse" )]
    public class FireantWorker : BaseCreature
    {
        [Constructable]
        public FireantWorker()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Body = 781;
            Name = "a fireant worker";

            Kills = 20;
            ControlSlots = 5;

            SetStr( 60, 90 );
            SetDex( 50, 70 );
            SetInt( 55, 70 );
            SetSkill( SkillName.Wrestling, 60, 70 );
            SetSkill( SkillName.Tactics, 50, 70 );
            SetSkill( SkillName.MagicResist, 46, 590 );

            VirtualArmor = Utility.RandomMinMax( 52, 58 );
            SetFameLevel( 2 );
            SetKarmaLevel( 2 );


            //PackItem( new Gold( 100, 300 ));
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Poor );
        }

        public override int TreasureMapLevel
        {
            get { return 4; }
        }

        public override int Meat
        {
            get { return 1; }
        }

        public FireantWorker( Serial serial )
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