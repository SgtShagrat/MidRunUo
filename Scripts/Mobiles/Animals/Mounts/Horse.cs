using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a horse corpse" )]
	[TypeAlias( "Server.Mobiles.BrownHorse", "Server.Mobiles.DirtyHorse", "Server.Mobiles.GrayHorse", "Server.Mobiles.TanHorse" )]
	public class Horse : BaseMount
	{
        #region Genetics by Faxx
        public override bool IsGeneticCreature { get { return true; } }

        public override Type DNAType { get { return typeof( HorseDNA ); } }

        public void UpdateItemid()
        {   
            // we don't need an update because InternalItem == null
            // this is the case of Horse not mounted.
            if( ItemID == 0 )
                return;

            int newItemID = -1;

            switch( Body.BodyID )
            {
                case 0xC8:
                    newItemID = 0x3E9F;
                    break;
                case 0xE2:
                    newItemID = 0x3EA0;
                    break;
                case 0xE4:
                    newItemID = 0x3EA1;
                    break;
                case 0xCC:
                    newItemID = 0x3EA2;
                    break;
                default:
                    break;
            }

            if( newItemID != ItemID )
            {
                if( Core.Debug )
                    Console.WriteLine( "Notice: Horse.UpdateItemid -> B: 0x{0:X2}, O: 0x{1:X3}, N: 0x{2:X3} (type {3} - serial {4})", Body.BodyID, ItemID, newItemID, GetType().Name, Serial.ToString() );
                ItemID = newItemID;
            }
        }
        #endregion

		private static int[] m_IDs = new int[]
			{
				0xC8, 0x3E9F,
				0xE2, 0x3EA0,
				0xE4, 0x3EA1,
				0xCC, 0x3EA2
			};

		[Constructable]
		public Horse() : this( "a horse" )
		{
		}

		[Constructable]
		public Horse( string name ) : base( name, 0xE2, 0x3EA0, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
            if( !IsGeneticCreature )
            {
                int random = Utility.Random( 4 );

                Body = m_IDs[ random * 2 ];
                ItemID = m_IDs[ random * 2 + 1 ];
            }

			BaseSoundID = 0xA8;

			SetStr( 22, 98 );
			SetDex( 56, 75 );
			SetInt( 6, 10 );

			SetHits( 28, 45 );
			SetMana( 0 );

			SetDamage( 3, 4 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 20 );

			SetSkill( SkillName.MagicResist, 25.1, 30.0 );
			SetSkill( SkillName.Tactics, 29.3, 44.0 );
			SetSkill( SkillName.Wrestling, 29.3, 44.0 );

			Fame = 300;
			Karma = 300;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 29.1;
		}

		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Horse( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}