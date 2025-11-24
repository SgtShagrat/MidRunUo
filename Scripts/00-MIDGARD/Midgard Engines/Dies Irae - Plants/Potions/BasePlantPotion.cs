/***************************************************************************
 *                                     BasePlantPotion.cs
 *                            		------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Base class for plant potions.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public abstract class BasePlantPotion : BasePotion
    {
        public BasePlantPotion( PotionEffect effect, int amount )
            : base( 0xF0E, effect, amount )
        {
        }

        public BasePlantPotion( Serial serial )
            : base( serial )
        {
        }

        public abstract double MinUseSkill { get; }
        public abstract override int LabelNumber { get; }
        public abstract PlantPotionLevel Level { get; }

        public override void OnDoubleClick( Mobile from )
        {
            if( from == null || from.Deleted || from.Map == Map.Internal )
                return;

            double skillToCheck = from.Skills.Camping.Value;

            if( skillToCheck < MinUseSkill )
                from.SendLocalizedMessage( 1065767 ); // You don't know how to use this plant potion.
            else if( from.Mounted )
                from.SendLocalizedMessage( 1065768 ); // You can't use this plant potion while mounted.
            else
                base.OnDoubleClick( from );
        }

        public override void Drink( Mobile from )
        {
            if( from == null || from.Deleted || from.Map == Map.Internal )
                return;

            if( Level > 0 )
            {
                for( int i = 0; i < (int)Level; i++ )
                    EffectCircle( from.Location, from.Map, i );
            }

            List<BasePlant> plantsNearby = PlantHelper.GetPlantsInRange( from.Location, from.Map, (int)Level );

            foreach( BasePlant p in plantsNearby )
            {
                if( p == null || p.Deleted )
                    continue;

                if( BasePlant.CheckAccess( from, p ) )
                    DoPlantEffect( p );
            }

            from.SendLocalizedMessage( 1065766 ); // You scatter some liquid from this potion on the plants around.
            Consume();

            from.AddToBackpack( new Bottle() );
        }

        public void EffectCircle( Point3D center, Map map, int radius )
        {
            Point3D current = new Point3D( center.X + radius, center.Y, center.Z );

            for( int i = 0; i <= 360; i++ )
            {
                Point3D next = new Point3D( (int)Math.Round( Math.Cos( i ) * radius ) + center.X, (int)Math.Round( Math.Sin( i ) * radius ) + center.Y, current.Z );

                PotionAreaEffect( next, map );
            }
        }

        public abstract void PotionAreaEffect( Point3D p, Map map );

        public abstract void DoPlantEffect( BasePlant plant );

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