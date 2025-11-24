/***************************************************************************
 *                                    BaseSeed.cs
 *                            		--------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Base class for seeds. They do not inherit from base plant!
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;

namespace Midgard.Engines.PlantSystem
{
    public abstract class BaseSeed : Item, ISowable
    {
        #region proprietà astratte
        public abstract Type[] PlantTypes { get; } 						// The plant type the seed will grow into.
        public abstract string PlantName { get; }						// The name of the plant type the seed will grow into
        #endregion

        public virtual int RootRadius { get { return 0; } }             // Is the radius in with no other seed can be planted.

        public virtual SkillName RequiredSkillNameToPlant { get { return SkillName.Camping; } }

        public override string DefaultName
        {
            get
            {
                string rawName = StringList.Localization.SplitFormat( 1065750, PlantName );
                return StringUtility.ConvertItemName( rawName );
            }
        }

        #region proprietà di ISowable
        public virtual bool CanGrowFarm { get { return false; } }		// If seed can grow on farm tiles, etc.
        public virtual bool CanGrowDirt { get { return false; } }
        public virtual bool CanGrowGround { get { return false; } }
        public virtual bool CanGrowSwamp { get { return false; } }
        public virtual bool CanGrowSand { get { return false; } }
        public virtual bool CanGrowGarden { get { return false; } }

        public virtual double RequiredSkillToPlant { get { return 0.0; } }
        #endregion

        #region costruttori
        protected BaseSeed()
            : this( 1 )
        {
        }

        protected BaseSeed( int amount )
            : base( 0xF27 )
        {
            Stackable = true;
            Weight = 1.0;
            Hue = 0x5E2;
            Amount = amount;
        }

        public BaseSeed( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Amount == 1 )
                list.Add( 1065750, PlantName ); // ~1_PLANT~ seed
            else
                list.Add( 1065751, PlantName ); // ~1_PLANT~ seeds
        }

        public override void OnSingleClick( Mobile from )
        {
            bool one = Amount == 1;

            LabelTo( from, String.Format( "{0} {1} seed{2}", one ? "" : Amount.ToString(), PlantName, one ? "" : "s" ) );
        }

        public override void OnDoubleClick( Mobile from )
        {
            Point3D location = from.Location;
            Map map = from.Map;

            if( !from.Alive )
            {
                from.SendLocalizedMessage( 1065752 ); // Gardening is not a dead work. 
            }
            else if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1065753 ); // You must have the seed in your backpack to use it. 
            }
            else if( !( PlantHelper.ValidatePlacement( this, map, location.X, location.Y, from ) ) )
            {
                from.SendLocalizedMessage( 1065754 ); // This seed will not grow here.
            }
            else if( from.Mounted )
            {
                from.SendLocalizedMessage( 1065755 ); // You cannot plant a seed while mounted.
            }
            else if( from.Skills[ RequiredSkillNameToPlant ].Value < RequiredSkillToPlant )
            {
                from.SendMessage( "You don't know how to plant this seed." );
            }
            else if( BasePlant.GetPlantForMobile( from ) > PlantHelper.GetMaxPlantsForPlayer( from ) )
            {
                from.SendMessage( "You have already reached maximum quantity of plants." );
            }
            else
            {
                if( CheckNearPlants( from, location, map, 1, RootRadius ) )
                {
                    bool gardenPlot = PlantHelper.ValidateGardenPlot( map, location.X, location.Y );
                    if( gardenPlot )
                        ++location.Z;

                    from.Animate( 32, 5, 1, true, false, 0 );

                    from.SendLocalizedMessage( 1065756 ); // You carefully planted the seed.
                    Consume();

                    object[] args = { from };
                    int typeIndex = Utility.Random( PlantTypes.Length );

                    try
                    {
                        Item item = Activator.CreateInstance( PlantTypes[ typeIndex ], args ) as Item;
                        if( item != null )
                        {
                            item.MoveToWorld( location, map );
                            if( gardenPlot )
                                ( (BasePlant)item ).IsInGarden = true;

                            from.CheckSkill( RequiredSkillNameToPlant, RequiredSkillToPlant, GetMaxSkillDifficulty() );
                        }
                    }
                    catch( Exception e )
                    {
                        Console.WriteLine( "Warning: failed seed plotting." );
                        Console.WriteLine( e.ToString() );
                    }
                }
                else
                    from.SendMessage( "You cannot plant that seed there." );
            }
        }

        public virtual double GetMaxSkillDifficulty()
        {
            return RequiredSkillToPlant + 50.0;
        }

        public virtual bool CheckNearPlants( Mobile farmer, Point3D location, Map map, int limit, int range )
        {
            return CheckNearPlants( farmer, location, map, limit, range, false );
        }

        public virtual bool CheckNearPlants( Mobile farmer, Point3D location, Map map, int limit, int range, bool ignoreAccessLevel )
        {
            if( map == null )
                return false;

            if( !ignoreAccessLevel && farmer.AccessLevel > AccessLevel.Player )
                return true;

            List<BasePlant> plantOnLoc = PlantHelper.GetPlantsInRange( location, map, 0 );
            if( plantOnLoc.Count > 0 )
            {
                farmer.SendMessage( "There is already a plant growing here." );
                return false;
            }

            List<BasePlant> plantNearLoc = PlantHelper.GetPlantsInRange( location, map, range );
            if( plantNearLoc.Count >= limit )
            {
                farmer.SendMessage( "There are too much plants around here." );
                return false;
            }

            return true;
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            if( Weight != 1.0 )
                Weight = 1.0;
        }
        #endregion
    }
}