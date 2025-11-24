using System;

using Server.Items;

namespace Server.Mobiles
{
    /// <summary>
    /// L'incredibile uomo delle nevi e' un npc dotato di grande forza e velocita'.
    /// Immune al gelo, al veleno e all'energia ma molto sensibile al fuoco e in corpo a corpo. 
    /// Combatte a pugni in corpo a corpo oppure lancia palle di Neve. 
    /// Naturalente va spawnato nelle zone ghiacciate.
    /// </summary>
    [CorpseName( "a Yeti corpse" )]
    public class Yeti : BaseCreature
    {
        [Constructable]
        public Yeti()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "a Yeti";
            Body = 161;
            BaseSoundID = 268;
            Hue = 1917;
            ActiveSpeed = 0.175;

			SetStr( 351, 400 );
			SetDex( 151, 165 );
			SetInt( 76, 100 );

            SetHits( 500, 550 );

            SetDamage( 15, 17 );

            SetDamageType( ResistanceType.Physical, 20 );
            SetDamageType( ResistanceType.Cold, 80 );

            SetResistance( ResistanceType.Physical, 45, 55 );
            SetResistance( ResistanceType.Fire, 10, 15 );
            SetResistance( ResistanceType.Cold, 700, 800 );
            SetResistance( ResistanceType.Poison, 70, 80 );
            SetResistance( ResistanceType.Energy, 50, 60 );

            SetSkill( SkillName.MagicResist, 165.0 );
            SetSkill( SkillName.Tactics, 100.0 );
            SetSkill( SkillName.Wrestling, 100.0 );
            SetSkill( SkillName.Anatomy, 100.0 );

            Fame = 5000;
            Karma = -5000;

            VirtualArmor = 50;

            if( 0.02 >= Utility.RandomDouble() )
                PackItem( new SnowPile() );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich );
            AddLoot( LootPack.Gems );
        }

        public override bool BardImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool HasBreath { get { return true; } } // palla di neve enabled
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathColdDamage { get { return 100; } }
        public override int BreathEffectHue { get { return 0x480; } }
        public override int BreathEffectSound { get { return 0x233; } }
        public override int BreathEffectSpeed { get { return 10; } }
        public override bool BreathEffectExplodes { get { return true; } }

        #region area damage
        public override bool CanAreaDamage { get { return true; } }
        public override TimeSpan AreaDamageDelay { get { return TimeSpan.FromSeconds( 15 ); } }
        public override int AreaColdDamage { get { return 100; } }
        #endregion

        public override void OnThink()
        {
            if( Combatant != null && 0.01 >= Utility.RandomDouble() )
            {
                Mobile combatant = Combatant;

                int ability = Utility.Random( 4 );

                switch( ability )
                {
                    case 0: Say( string.Format( "Roarr {0}!! So, You are my food Today!", combatant.Name ) ); break;
                    case 1: Say( string.Format( "do you feel freeze {0}?", combatant.Name ) ); break;
                    case 2: Say( string.Format( "GROAaaarrr {0}!! ", combatant.Name ) ); break;
                    case 3: Say( string.Format( "Come Here {0}, I want to meet you! BRahhhuaaaahhhhhrghh", combatant.Name ) ); break;

                }
            }

            base.OnThink();
        }

        public Yeti( Serial serial )
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
    }
}
