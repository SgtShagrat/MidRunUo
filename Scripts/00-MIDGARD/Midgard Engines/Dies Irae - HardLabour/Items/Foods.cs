/***************************************************************************
 *                                         Foods.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Engines.HardLabour
{
    public class StaleBreadLoaf : BreadLoaf
    {
        public static double PoisonChance = 0.15;

        public override string DefaultName
        {
            get
            {
                return "A Stale Bread";
            }
        }

        [Constructable]
        public StaleBreadLoaf()
            : this( 1 )
        {
        }

        [Constructable]
        public StaleBreadLoaf( int amount )
            : base( amount )
        {
            Hue = 2952;

            Stackable = true;
        }

        public override bool Eat( Mobile from )
        {
            if( PoisonChance > Utility.RandomDouble() )
            {
                Poison = Poison.Lesser;
            }

            return base.Eat( from );
        }

        #region serial-deserial
        public StaleBreadLoaf( Serial serial )
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

    public class SpoiledMeat : Ribs
    {
        public static double PoisonChance = 0.15;

        public override string DefaultName
        {
            get
            {
                return "A Spoilde Meat";
            }
        }

        [Constructable]
        public SpoiledMeat()
            : this( 1 )
        {
        }

        [Constructable]
        public SpoiledMeat( int amount )
            : base( amount )
        {
            Hue = 1157;

            Stackable = true;
        }

        public override bool Eat( Mobile from )
        {
            if( PoisonChance > Utility.RandomDouble() )
            {
                Poison = Poison.Lesser;
            }

            return base.Eat( from );
        }

        #region serial deserial
        public SpoiledMeat( Serial serial )
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