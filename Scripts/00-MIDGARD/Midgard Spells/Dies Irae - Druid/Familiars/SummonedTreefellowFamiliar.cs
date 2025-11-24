/***************************************************************************
 *                               SummonedTreefellowFamiliar.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Engines.SpellSystem
{
    [CorpseName( "a treefellow corpse" )]
    public class SummonedTreefellow : BaseCreature
    {
        public override double DispelDifficulty
        {
            get { return 70.0; }
        }

        [Constructable]
        public SummonedTreefellow()
            : base( AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4 )
        {
            Name = "a treefellow";
            Body = 301;
            BaseSoundID = 442;

            SetStr( 196, 220 );
            SetDex( 31, 55 );
            SetInt( 66, 90 );

            SetHits( 1180, 1320 );

            SetDamage( 12, 16 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 20, 25 );
            SetResistance( ResistanceType.Cold, 50, 60 );
            SetResistance( ResistanceType.Poison, 30, 35 );
            SetResistance( ResistanceType.Energy, 20, 30 );

            SetSkill( SkillName.MagicResist, 65.0 );
            SetSkill( SkillName.Tactics, 100.0 );
            SetSkill( SkillName.Wrestling, 90.0 );

            VirtualArmor = 34;
            ControlSlots = 2;
        }

        #region serialization
        public SummonedTreefellow( Serial serial )
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