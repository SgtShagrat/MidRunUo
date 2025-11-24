/***************************************************************************
 *                               QuagmireFamiliar.cs
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
    [CorpseName( "a quagmire corpse" )]
    public class QuagmireFamiliar : BaseFamiliar
    {
        public override double DispelDifficulty
        {
            get { return 50.0; }
        }

        public QuagmireFamiliar()
        {
            Name = "a quagmire";
            Body = 789;
            BaseSoundID = 352;

            SetStr( 120 );
            SetDex( 120 );
            SetInt( 100 );

            SetHits( 900 );
            SetStam( 120 );
            SetMana( 0 );

            SetDamage( 5, 10 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 10, 15 );
            SetResistance( ResistanceType.Fire, 10, 15 );
            SetResistance( ResistanceType.Cold, 10, 15 );
            SetResistance( ResistanceType.Poison, 10, 15 );
            SetResistance( ResistanceType.Energy, 10, 15 );

            SetSkill( SkillName.Wrestling, 95.1, 100.0 );
            SetSkill( SkillName.Tactics, 50.0 );

            ControlSlots = 1;
        }

        #region serialization
        public QuagmireFamiliar( Serial serial )
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