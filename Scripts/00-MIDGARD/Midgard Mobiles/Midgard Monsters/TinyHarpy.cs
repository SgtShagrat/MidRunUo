/***************************************************************************
 *                               TinyHarpy.cs
 *
 *   begin                : 26 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a tiny harpy corpse" )]
    public class TinyHarpy : BaseCreature
    {
        [Constructable]
        public TinyHarpy()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "a tiny harpy";
            Body = 30;
            Hue = Utility.RandomNeutralHue();

            BaseSoundID = 402;

            SetStr( 60 );
            SetDex( 55 );
            SetInt( 60 );

            SetHits( 58, 72 );

            SetDamage( 5 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.MagicResist, 50.1 );
            SetSkill( SkillName.Tactics, 70.1 );
            SetSkill( SkillName.Wrestling, 60.1 );

            Fame = 1250;
            Karma = -1250;

            VirtualArmor = 14;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager, 1 );
        }

        public override int GetAttackSound()
        {
            return 916;
        }

        public override int GetAngerSound()
        {
            return 916;
        }

        public override int GetDeathSound()
        {
            return 917;
        }

        public override int GetHurtSound()
        {
            return 919;
        }

        public override int GetIdleSound()
        {
            return 918;
        }

        public override bool CanRummageCorpses
        {
            get { return true; }
        }

        public override int Meat
        {
            get { return 4; }
        }

        public override MeatType MeatType
        {
            get { return MeatType.Bird; }
        }

        public override int Feathers
        {
            get { return 50; }
        }

        #region serialization
        public TinyHarpy( Serial serial )
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