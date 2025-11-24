/***************************************************************************
 *                                     BaseFungicidePotion.cs
 *                            		----------------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Base class for plant fungicide potions.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public abstract class BaseFungicidePotion : BasePlantPotion
    {
        public override int LabelNumber { get { return 1065762; } } // plant fungicide

        public BaseFungicidePotion( PotionEffect effect, int amount )
            : base( effect, amount )
        {
        }

        public BaseFungicidePotion( Serial serial )
            : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( Level != PlantPotionLevel.None )
                list.Add( 1065763, Enum.GetName( typeof( PlantPotionLevel ), Level ) ); // venom level: ~1_LEVEL~
        }

        public override void PotionAreaEffect( Point3D p, Map map )
        {
            if( map.CanFit( p, 12, true, false ) )
            {
                int renderMode = Utility.RandomList( 0, 2, 3, 4, 5, 7 );
                Effects.SendLocationEffect( p, map, 0x3728, 16, Utility.RandomGreenHue(), renderMode );
            }
        }

        public override void DoPlantEffect( BasePlant plant )
        {
            plant.Fungicide();
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