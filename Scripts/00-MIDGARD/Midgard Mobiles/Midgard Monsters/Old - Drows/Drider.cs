/***************************************************************************
 *                               Drider.cs
 *
 *   begin                : 03 luglio 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a drider corpse" )]
    public class Drider : BaseDrow
    {
        [Constructable]
        public Drider()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Title = ", the drider";

            BaseSoundID = 634;

            SetStr( 300, 350 );
            SetDex( 106, 145 );
            SetInt( 41, 65 );

            SetHits( 200, 265 );
            SetMana( 0 );

            SetDamage( 14, 25 );

            SetDamageType( ResistanceType.Physical, 70 );
            SetDamageType( ResistanceType.Poison, 30 );

            SetResistance( ResistanceType.Physical, 45, 60 );
            SetResistance( ResistanceType.Fire, 45, 55 );
            SetResistance( ResistanceType.Cold, 45, 55 );
            SetResistance( ResistanceType.Poison, 55, 65 );
            SetResistance( ResistanceType.Energy, 40, 55 );

            SetSkill( SkillName.Poisoning, 90.1, 120.0 );
            SetSkill( SkillName.MagicResist, 85.1, 105.0 );
            SetSkill( SkillName.Tactics, 90.1, 110.0 );
            SetSkill( SkillName.Wrestling, 90.1, 100.0 );
            SetSkill( SkillName.Anatomy, 85.6, 100.3 );

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 45;

            PackGold( 360 / 2, 490 / 2 );
            PackItem( new Emerald( Utility.RandomMinMax( 5, 15 ) ) );
        }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override double HitPoisonChance { get { return 100.0; } }
        public override int Meat { get { return 4; } }

        #region serialization
        public Drider( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }
}