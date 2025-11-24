/***************************************************************************
 *                                  BaseCraftableDoorTrapDeed.cs
 *                            		----------------------------
 *  begin                	: Ottobre, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Engines.Craft;
using Server.Items;
using Server.Misc;
using Server.Targeting;

namespace Midgard.Items
{
    public abstract class BaseCraftableDoorTrapDeed : Item, ICraftable
    {
        protected BaseCraftableDoorTrapDeed()
            : base( 0x14F0 )
        {
            Weight = 1.0;

            TrapPower = 0;
            Level = 0;
            TrapCharges = 0;

            if( TestCenter.Enabled )
            {
                int trapLevel = Utility.Dice( 1, 5, 5 ); // 1d5+5

                TrapPower = trapLevel * 9;
                Level = trapLevel;
                TrapCharges = trapLevel + 2;
            }
        }

        public abstract TrapType DeedTrapType { get; }

        [CommandProperty( AccessLevel.GameMaster )]
        public TownSystem System { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int TrapPower { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Level { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int TrapCharges { get; set; }

        #region ICraftable Members
        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
        {
            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            ItemID = 0x14F0;
            System = TownSystem.Find( from );

            int trapLevel = (int)( from.Skills.Tinkering.Value / 10 );

            TrapPower = trapLevel * 9;
            Level = trapLevel;
            TrapCharges = trapLevel + 2;

            return 1;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }
        #endregion

        #region serialization
        public BaseCraftableDoorTrapDeed( Serial serial )
            : base( serial )
        {
        }

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

            if( TestCenter.Enabled )
            {
                int trapLevel = Utility.Dice( 1, 5, 5 ); // 1d5+5

                TrapPower = trapLevel * 9;
                Level = trapLevel;
                TrapCharges = trapLevel + 2;
            }
        }
        #endregion

        public virtual void Place( Mobile from, BaseDoor door )
        {
            if( door != null )
            {
                door.TrapType = DeedTrapType;
                door.TrapPower = TrapPower;
                door.TrapLevel = Level;
                door.TrapCharges = TrapCharges;

                from.SendMessage( "You carefully placed the trap!" );
            }
            else
                from.SendMessage( "You failed to place this trap and it's destroyed!" );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( System == null )
                System = TownSystem.SerpentsHold;

            TownSystem system = TownSystem.Find( from );

            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
            else if( System != null && system != System )
            {
                from.SendMessage( "Only citizens of {0} can use this deed!", System.Definition.TownName );
            }
            else if( !CheckPlaceTrap( from, Level ) )
            {
                from.SendMessage( "You failed to place this trap and it's destroyed!" );
                Delete();
            }
            else
            {
                from.SendMessage( "Choose a door you would to trap." );
                from.Target = new InternalTarget( this );
            }
        }

        private static bool CheckPlaceTrap( Mobile m, int level )
        {
            double chanceBase = m.Skills[ SkillName.RemoveTrap ].Value / 100.0;

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

            double bonusLevel = BaseDoor.GetDifficultyToPlace( level );

            double chance = chanceBase * bonusDex * bonusInt * bonusLevel;

            m.SendMessage( "Debug CheckPlaceTrap: chanceBase {0}, bonusDex {1}, bonusInt {2}, bonusLevel {3}, chance {4}.",
                           chanceBase.ToString( "F2" ), bonusDex.ToString( "F2" ), bonusInt.ToString( "F2" ), bonusLevel.ToString( "F2" ), chance.ToString( "F2" ) );

            return m.CheckSkill( SkillName.RemoveTrap, chance );
        }

        internal class InternalTarget : Target
        {
            private readonly BaseCraftableDoorTrapDeed m_Deed;

            public InternalTarget( BaseCraftableDoorTrapDeed deed )
                : base( 1, true, TargetFlags.None )
            {
                m_Deed = deed;
            }

            protected override void OnTarget( Mobile from, object target )
            {
                if( m_Deed.Deleted || m_Deed.RootParent != from )
                    return;

                if( target is BaseDoor )
                {
                    BaseDoor door = (BaseDoor)target;

                    if( door.CanBeTrapped && door.CanPlaceTrap( from ) )
                    {
                        if( door.TrapType == TrapType.None )
                        {
                            if( from.InRange( door, 1 ) )
                            {
                                m_Deed.Place( from, door );
                                m_Deed.Delete();
                            }
                            else
                            {
                                from.SendMessage( "That is too far away" );
                            }
                        }
                        else
                        {
                            from.SendMessage( "You can only place one trap on an object at a time." );
                        }
                    }
                    else
                    {
                        from.SendMessage( "That door cannot be trapped." );
                    }
                }
                else
                {
                    from.SendMessage( "You must target a door to place this trap." );
                }
            }
        }
    }

    public class ExplosionTrapDeed : BaseCraftableDoorTrapDeed
    {
        [Constructable]
        public ExplosionTrapDeed()
        {
        }

        #region serialization
        public ExplosionTrapDeed( Serial serial )
            : base( serial )
        {
        }

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

        public override TrapType DeedTrapType
        {
            get { return TrapType.ExplosionTrap; }
        }

        public override string DefaultName
        {
            get { return "an Explosion Trap Deed [only for doors]"; }
        }
    }

    public class DartTrapDeed : BaseCraftableDoorTrapDeed
    {
        [Constructable]
        public DartTrapDeed()
        {
        }

        #region serialization
        public DartTrapDeed( Serial serial )
            : base( serial )
        {
        }

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

        public override TrapType DeedTrapType
        {
            get { return TrapType.DartTrap; }
        }

        public override string DefaultName
        {
            get { return "a Dart Trap Deed [only for doors]"; }
        }
    }

    public class PoisonTrapDeed : BaseCraftableDoorTrapDeed
    {
        [Constructable]
        public PoisonTrapDeed()
        {
        }

        #region serialization
        public PoisonTrapDeed( Serial serial )
            : base( serial )
        {
        }

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

        public override TrapType DeedTrapType
        {
            get { return TrapType.PoisonTrap; }
        }

        public override string DefaultName
        {
            get { return "a Poison Trap Deed [only for doors]"; }
        }
    }
}