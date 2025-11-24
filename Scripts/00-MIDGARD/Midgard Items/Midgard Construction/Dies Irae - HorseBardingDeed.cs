/***************************************************************************
 *                                  HorseBardingDeed.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Deed per bardare un war horse
 * 
 ***************************************************************************/

using System;

using Midgard.Engines.OldCraftSystem;
using Midgard.Mobiles;

using Server;
using Server.Engines.Craft;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Items
{
    public class HorseBardingDeed : Item, ICraftable, IDurability, IRepairable, IResourceItem
    {
        private CraftResource m_Resource;
        private bool m_Exceptional;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public bool Exceptional
        {
            get { return m_Exceptional; }
            set
            {
                UnscaleDurability();

                m_Exceptional = value;

                ScaleDurability();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                if( m_Resource == value )
                    return;

                UnscaleDurability();

                m_Resource = value;

                Hue = CraftResources.GetHue( m_Resource );

                ScaleDurability();
            }
        }

        public override string DefaultName
        {
            get { return "a horse barding deed"; }
        }

        [Constructable]
        public HorseBardingDeed()
            : base( 0x14F0 )
        {
            Weight = 1.0;

            HitPoints = InitMaxHits;
            MaxHitPoints = InitMaxHits;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return base.CanBeCraftedBy( from ) && IsOrderOrChaosMobile( from );
        }

        public static bool IsOrderOrChaosMobile( Mobile mob )
        {
            return mob is Midgard2PlayerMobile && ( ( (Midgard2PlayerMobile)mob ).IsOrder || ( (Midgard2PlayerMobile)mob ).IsChaos );
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                LabelTo( from, string.Format( "crafted by {0}", Crafter.Name ) );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsChildOf( from.Backpack ) )
            {
                if( IsOrderOrChaosMobile( from ) || from.AccessLevel > AccessLevel.Counselor )
                {
                    from.BeginTarget( 6, false, TargetFlags.None, new TargetCallback( OnTarget ) );
                    from.SendMessage( "Select the war horse you wish to place the barding on." );
                }
                else
                    from.SendMessage( "You cannot use that item." );
            }
            else
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
        }

        public virtual void OnTarget( Mobile from, object obj )
        {
            if( Deleted )
                return;

            MidgardWarHorse pet = obj as MidgardWarHorse;

            if( pet == null || pet.HasBarding )
                from.SendMessage( "That is not an unarmored war horse." );
            else if( !pet.Controlled || pet.ControlMaster != from )
                from.SendMessage( "You can only put barding on a tamed war horse that you own." );
            else if( !IsChildOf( from.Backpack ) )
                from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
            else
            {
                pet.BardingExceptional = Exceptional;
                pet.BardingCrafter = Crafter;
                pet.BardingHP = HitPoints;
                pet.BardingMaxHP = MaxHitPoints;
                pet.BardingResource = Resource;
                pet.HasBarding = true;
                pet.Hue = Hue;

                Delete();

                from.SendMessage( "You place the barding on your war horse.  Use a bladed item on your war horse to remove the armor." );
            }
        }

        #region ICraftable Members

        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes,
                            BaseTool tool, CraftItem craftItem, int resHue )
        {
            Exceptional = ( quality >= 2 );

            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            Type resourceType = typeRes;

            if( resourceType == null )
                resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

            Resource = CraftResources.GetFromType( resourceType );

            CraftContext context = craftSystem.GetContext( from );

            if( context != null && context.DoNotColor )
                Hue = 0;

            return quality;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }

        #endregion

        #region serialization
        public HorseBardingDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version

            writer.Write( HitPoints );
            writer.Write( MaxHitPoints );

            writer.Write( Exceptional );
            writer.Write( Crafter );
            writer.Write( (int)m_Resource );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        HitPoints = reader.ReadInt();
                        MaxHitPoints = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        Exceptional = reader.ReadBool();
                        Crafter = reader.ReadMobile();
                        m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }
        }
        #endregion

        public static void ReturnBardingToOwner( MidgardWarHorse pet, Mobile from )
        {
            HorseBardingDeed deed = new HorseBardingDeed();

            deed.Exceptional = pet.BardingExceptional;
            deed.Crafter = pet.BardingCrafter;
            deed.Resource = pet.BardingResource;
            deed.HitPoints = pet.BardingHP;
            deed.MaxHitPoints = pet.BardingMaxHP;

            from.AddToBackpack( deed );
        }

        #region Implementation of IDurability
        public bool CanFortify
        {
            get { return false; }
        }

        public int InitMinHits { get { return 1000; } }

        public int InitMaxHits { get { return 1000; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int HitPoints { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxHitPoints { get; set; }

        public void UnscaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            HitPoints = ( ( HitPoints * 100 ) + ( scale - 1 ) ) / scale;
            MaxHitPoints = ( ( MaxHitPoints * 100 ) + ( scale - 1 ) ) / scale;
        }

        public void ScaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            HitPoints = ( ( HitPoints * scale ) + 99 ) / 100;
            MaxHitPoints = ( ( MaxHitPoints * scale ) + 99 ) / 100;
        }

        public int GetDurabilityBonus()
        {
            int bonus = Exceptional ? 150 : 100;

            double diff = DefBlacksmithy.CraftSystem.GetMaterialDifficulty( Resource );
            if( diff > 1.0 )
                bonus = (int)( bonus * diff );

            return bonus;
        }
        #endregion

        #region Implementation of IRepairable
        public bool Repair( Mobile from, BaseTool tool )
        {
            CraftSystem system = tool.CraftSystem;
            if( system != DefBlacksmithy.CraftSystem )
            {
                from.SendMessage( "Thou cannot repair that armor using that tool." );
                return false;
            }

            int number;

            SkillName skill = system.MainSkill;
            int toWeaken;

            double skillLevel = from.Skills[ skill ].Base;

            if( skillLevel >= 90.0 )
                toWeaken = 1;
            else if( skillLevel >= 70.0 )
                toWeaken = 2;
            else
                toWeaken = 3;

            if( !IsChildOf( from.Backpack ) )
            {
                number = 1044275; // The item must be in your backpack to repair it.
            }
            else if( MaxHitPoints <= 0 || HitPoints == MaxHitPoints )
            {
                number = 1044281; // That item is in full repair
            }
            else if( MaxHitPoints <= toWeaken )
            {
                number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
            }
            else
            {
                if( RepairHelper.CheckWeaken( from, skill, HitPoints, MaxHitPoints ) )
                {
                    MaxHitPoints -= toWeaken;
                    HitPoints = Math.Max( 0, HitPoints - toWeaken );
                }

                if( RepairHelper.CheckRepairDifficulty( from, skill, HitPoints, MaxHitPoints ) )
                {
                    number = 1044279; // You repair the item.
                    system.PlayCraftEffect( from );
                    HitPoints = MaxHitPoints;
                }
                else
                {
                    number = 1044280; // You fail to repair the item.
                    system.PlayCraftEffect( from );
                }

            }

            if( number > 0 )
                from.SendLocalizedMessage( number );

            return ( number == 1044279 );
        }
        #endregion
    }
}