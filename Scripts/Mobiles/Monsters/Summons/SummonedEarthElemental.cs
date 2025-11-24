using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an earth elemental corpse" )]
	public class SummonedEarthElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 35.0; } }

		[Constructable]
		public SummonedEarthElemental() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            #region mod by Dies Irae
            if( !Core.AOS )
            {
                SetOldTemplate();
                return;
            }
            #endregion

			Name = "an earth elemental";
			Body = 14;
			BaseSoundID = 268;

			SetStr( 200 );
			SetDex( 70 );
			SetInt( 70 );

			SetHits( 180 );

			SetDamage( 14, 21 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.MagicResist, 65.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 90.0 );

			VirtualArmor = 34;
			ControlSlots = 2;
		}

        private void SetOldTemplate()
        {
            /*
            npctemplate                  earthelemental
            {
                name                     an earth elemental
                objtype                  0x0e
                color                    33784
                str                      400
                int                      90
                dex                      50
                hits                     400
                mana                     90
                stam                     50
                parry                    45
                magicresistance          90
                tactics                  90
                wrestling                160
                attackspeed              25
                attackdamage             3d10
                ar                       29
            }
            */

			Name = "an earth elemental";
			Body = 0xE;
			BaseSoundID = 0x10C;
            
            SetStr( 400 );
            SetDex( 50 );
            SetInt( 90 );
                
            SetHits( 400 );
            SetMana( 90 );
            SetStam( 50 );

			SetDamage( "3d10" );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Parry, 45.0 );
			SetSkill( SkillName.MagicResist, 90.0 );
			SetSkill( SkillName.Tactics, 90.0 );
			SetSkill( SkillName.Wrestling, 160.0 );

			VirtualArmor = 29;
			ControlSlots = 2;
        }

        public override int CustomWeaponSpeed { get { return 25; } } // mod by Dies Irae
        
	    public SummonedEarthElemental( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}