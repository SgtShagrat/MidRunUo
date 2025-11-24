/***************************************************************************
 *                                  ElvenHorse.cs
 *                            		-------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info: 
 * 			Cavallo cavalcabile solo da elfi.
 * 
 ***************************************************************************/

using Midgard.Engines.PetSystem;
using Server;
using Server.Mobiles;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Mobiles
{
    [CorpseName( "an elven horse corpse" )]
    public class ElvenHorse : BaseMount
    {
        public override int Meat { get { return 2; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }
        public override Race RequiredRace { get { return Core.HighElf; } }

        [Constructable]
        public ElvenHorse()
            : base( "an elven horse", 0x74, 0x3EA7, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            Hue = 0x875;
            BaseSoundID = 0xA8;

            SetStr( 80, 90 );
            SetDex( 80, 90 );
            SetInt( 10, 15 );

            SetHits( 80, 90 );
            SetMana( 0 );

            SetDamage( 14, 20 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 70, 85 );
            SetResistance( ResistanceType.Fire, 15, 20 );
            SetResistance( ResistanceType.Poison, 25, 30 );
            SetResistance( ResistanceType.Energy, 25, 30 );

            SetSkill( SkillName.MagicResist, 80.1, 85.0 );
            SetSkill( SkillName.Tactics, 85.3, 100.0 );
            SetSkill( SkillName.Wrestling, 85.3, 100.0 );

            Fame = 2000;
            Karma = -2000;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 29.1;

            PetUtility.ScalePet( this );
            PetUtility.PetNormalize( this );

            MaxLevel = 10;
        }

        public ElvenHorse( Serial serial )
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