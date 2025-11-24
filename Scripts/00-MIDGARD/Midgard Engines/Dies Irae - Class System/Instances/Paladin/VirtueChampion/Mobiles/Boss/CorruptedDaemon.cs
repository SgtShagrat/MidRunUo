/***************************************************************************
 *                               CorruptedDaemon.cs
 *
 *   begin                : 13 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class CorruptedDaemon : BaseVirtueChampionBoss
    {
        public AntiVirtues AntiVirtue { get; set; }

        [Constructable]
        public CorruptedDaemon( AntiVirtues antiVirtue )
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            AntiVirtue = antiVirtue;
            Body = 785;
            Name = string.Format( "Soulshatter of {0}", Core.GetAntiVirtueName( antiVirtue ) );
            BaseSoundID = 357;

            SetStr( 1500, 1800 );
            SetDex( 187, 238 );
            SetInt( 151, 247 );
            SetSkill( SkillName.Wrestling, 91, 99 );
            SetSkill( SkillName.Tactics, 90, 99 );
            SetSkill( SkillName.MagicResist, 116, 122 );
            SetSkill( SkillName.Magery, 96, 99 );
            SetSkill( SkillName.EvalInt, 80, 93 );
            SetSkill( SkillName.Meditation, 27, 50 );
            SetSkill( SkillName.Anatomy, 25, 35 );

            VirtualArmor = Utility.RandomMinMax( 74, 90 );
            SetFameLevel( 5 );
            SetKarmaLevel( 5 );
            PackItem( new Longsword() );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.FilthyRich );
            AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
        }

        public override int Meat
        {
            get { return 1; }
        }

        #region serialization
        public CorruptedDaemon( Serial serial )
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