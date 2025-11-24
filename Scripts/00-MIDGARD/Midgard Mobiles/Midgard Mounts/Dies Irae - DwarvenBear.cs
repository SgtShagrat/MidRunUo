/***************************************************************************
 *                                  DwarvenRidableBear.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info: 
 * 			Orso cavalcabile solo da nani.
 * 
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;

using Core = Midgard.Engines.Races.Core;

namespace Midgard.Mobiles
{
    [CorpseName( "an dwarven bear corpse" )]
    public class DwarvenRidableBear : BaseMount
    {
        public override int Meat { get { return 2; } }
        public override FoodType FavoriteFood { get { return FoodType.Fish | FoodType.FruitsAndVegies | FoodType.Meat; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Bear; } }
        public override Race RequiredRace { get { return Core.MountainDwarf; } }

        [Constructable]
        public DwarvenRidableBear()
            : base( "a dwarven bear", 213, 16143, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            Hue = 0x73E;
            BaseSoundID = 0xA3;

			SetStr( 76, 100 );
			SetDex( 56, 75 );
			SetInt( 11, 14 );

			SetHits( 46, 60 );
			SetMana( 0 );

			SetDamage( 4, 10 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.MagicResist, 80.1, 85.0 );
            SetSkill( SkillName.Tactics, 60.1, 70.0 );
            SetSkill( SkillName.Wrestling, 45.1, 60.0 );

            Fame = 1500;
            Karma = 0;

            VirtualArmor = 24;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 35.1;

            MaxLevel = 10;

            Container pack = Backpack;
            if( pack != null )
                pack.Delete();

            pack = new StrongBackpack();
            pack.Movable = false;
            AddItem( pack );
        }

        public DwarvenRidableBear( Serial serial )
            : base( serial )
        {
        }

        public override double GetControlChance( Mobile m, bool useBaseSkill )
        {
            return m.Race == RequiredRace ? 1.0 : 0.0;
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}