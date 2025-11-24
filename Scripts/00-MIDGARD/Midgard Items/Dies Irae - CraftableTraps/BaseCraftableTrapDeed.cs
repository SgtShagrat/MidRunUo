/***************************************************************************
 *                                  BaseCraftableTrapDeed.cs
 *                            		------------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Classes;
using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseCraftableTrapDeed : Item, ICraftable
    {
        public abstract Type TrapType { get; }

        [CommandProperty( AccessLevel.GameMaster )]
        public TownSystem System { get; set; }

        public BaseCraftableTrapDeed()
            : base( 0x14F0 )
        {
            Weight = 1.0;
        }

        public BaseCraftableTrapDeed( Serial serial )
            : base( serial )
        {
        }

        public virtual BaseCraftableTrap Construct( Mobile from )
        {
            try
            {
                return Activator.CreateInstance( TrapType, new object[] { System, from } ) as BaseCraftableTrap;
            }
            catch
            {
                return null;
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendMessage( "That deed must be in your pack to be used." );
                return;
            }

            if( System != null && !CheckTownRequisites( from ) )
            {
                return;
            }
            else
            {
                BaseCraftableTrap trap = Construct( from );
                if( trap == null )
                    return;

                bool isScout = ClassSystem.IsScout( from );
                bool isScoutOrThief = isScout || ClassSystem.IsThief( from );

                if( !isScoutOrThief && trap.Level > TrapLevel.Medium )
                {
                    from.SendMessage( "You cannot place heavy traps." );

                    trap.Delete();
                    return;
                }

                if( isScout && !ScoutSystem.IsInForest( from ) )
                {
                    from.SendMessage( "This trap can only be placed in a forest." );

                    trap.Delete();
                    return;
                }

                string message = trap.IsValidLocation( from.Location, from.Map );

                if( !String.IsNullOrEmpty( message ) )
                {
                    from.SendMessage( message );

                    trap.Delete();
                }
                else if( !CheckPlaceTrap( from, trap ) )
                {
                    from.SendMessage( "You failed to place the trap and the deed is destroyed." );
                    trap.Delete();

                    Delete();
                }
                else
                {
                    from.SendLocalizedMessage( 1010360 ); // You arm the trap and carefully hide it from view
                    trap.MoveToWorld( from.Location, from.Map );

                    if( System != null )
                        System.TownTraps.Add( trap );

                    Delete();
                }
            }
        }

        private bool CheckTownRequisites( Mobile from )
        {
            TownSystem system = TownSystem.Find( from );

            if( System != null && system == null )
            {
                from.SendMessage( "Only citizens can use this deed!" );
            }
            else if( System != null && system != System )
            {
                from.SendMessage( "Only citizens of {0} can use this deed!", System.Definition.TownName );
            }
            else if( System != null && system.TownTraps.Count >= system.MaximumTraps )
            {
                from.SendMessage( "Your town already has the maximum number of traps placed." );
            }
            else
            {
                return true;
            }

            return false;
        }

        private static bool CheckPlaceTrap( Mobile m, BaseCraftableTrap trap )
        {
            bool isScout = ClassSystem.IsScout( m );

            double chanceBase = m.Skills[ isScout ? SkillName.Camping : SkillName.RemoveTrap ].Value / 100.0;

            double bonusDex = m.Dex / 100.0;
            if( bonusDex > 1.2 )
                bonusDex = 1.2;
            else if( bonusDex < 0.7 )
                bonusDex = 0.7;

            double bonusInt = m.Int / 100.0;
            if( bonusInt > 1.2 )
                bonusInt = 1.2;
            else if( bonusInt < 0.7 )
                bonusInt = 0.7;

            double bonusLevel = BaseCraftableTrap.GetDifficultyToPlaceScalar( trap.Level );

            double chance = chanceBase * bonusDex * bonusInt * bonusLevel;

            if( m.PlayerDebug )
            {
                m.SendMessage( "Debug CheckPlaceTrap: chanceBase {0}, bonusDex {1}, bonusInt {2}, bonusLevel {3}, chance {4}.",
                          chanceBase.ToString( "F2" ), bonusDex.ToString( "F2" ), bonusInt.ToString( "F2" ), bonusLevel.ToString( "F2" ), chance.ToString( "F2" ) );
            }

            return m.CheckSkill( isScout ? SkillName.Camping : SkillName.RemoveTrap, chance );
        }

        #region ICraftable Members
        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
        {
            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            ItemID = 0x14F0;
            System = TownSystem.Find( from );

            return 1;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }
        #endregion

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            TownSystem.WriteReference( writer, System );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            System = TownSystem.ReadReference( reader );
        }
    }
}