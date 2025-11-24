/***************************************************************************
 *                                   SalvageBag.cs
 *                            		--------------
 *  begin                	: April, 2008
 *  version					: 2.0 **VERSION FOR RUNUO 2.0**
 *  copyright            	: Matteo Visintin
 *  email                	: tocasia@alice.it
 *  msn						: Matteo_Visintin@hotmail.com
 *  
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

/***************************************************************************
 *  Item for advanced re-smelting of clothing and metal items.
 *  
 *  Reference: http://www.uoguide.com/Salvage_Bag
 *  
 *  The Salvage Bag was added during Publish 48 on November 27, 2007. 
 *  It is a comprehensive recyclying system that will help players turn 
 *  finished products back into raw materials such as Ingots, Cloth and Leather. 
 *  The bag can be purchased from a Provisioner for around 1,000 gold.
 *  
 *  Place a Salvage bag in your backpack and fill it with all the items 
 *  you wish to recycle. Once full, open a Contextual Menu on the bag 
 *  itself and select the appropriate option (Ingots, Cloth, or All). 
 *  The items will be turned back into raw materials with a system message 
 *  informing you how many items were recycled.
 *  
 *  Notes:
 *  The "Salvage Cloth" option recycles leather as well;
 *  You must have the appropriate tool in the main level of your backpack 
 *  to recycle;
 *  Mining skill determines how many ingots you will be able to recycle; (*)
 *  You must stand near a Forge to recycle metal items;
 *  
 *  (*) not implemented
 *  
 ***************************************************************************/

/***************************************************************************
 * Clilocs used:
 * 
 * 1079822 - You need a blacksmithing tool in order to salvage ingots.
 * 1079823 - You need scissors in order to salvage cloth. 
 * 1079931 - Salvage Bag 
 * 1079973 - Salvaged: ~1_COUNT~/~2_NUM~ blacksmithed items 
 * 1079974 - Salvaged: ~1_COUNT~/~2_NUM~ tailored items 
 * 1079975 - You failed to smelt some metal for lack of skill. 
 * 3006276 - Salvage All 
 * 3006277 - Salvage Ingots 
 * 3006278 - Salvage Cloth
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Network;

namespace Server.Items
{
    public class SalvageBag : Bag
    {
        public override int LabelNumber { get { return 1079931; } } // Salvage Bag

        [Constructable]
        public SalvageBag()
            : this( Utility.RandomBlueHue() )
        {
        }

        [Constructable]
        public SalvageBag( int hue )
        {
            Weight = 2.0;
            Hue = hue;
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries( from, list );

            if( from.Alive )
            {
                list.Add( new SalvageIngotsEntry( this, IsChildOf( from.Backpack ) && HasIronItems() ) );
                list.Add( new SalvageClothEntry( this, IsChildOf( from.Backpack ) && HasClothItems() ) );
                list.Add( new SalvageAllEntry( this, IsChildOf( from.Backpack ) && HasIronItems() && HasClothItems() ) );
            }
        }

        private bool HasIronItems()
        {
            foreach( Item i in Items )
            {
                if( i != null && !i.Deleted )
                {
                    if( i is BaseArmor && CraftResources.GetType( ( (BaseArmor)i ).Resource ) == CraftResourceType.Metal )
                        return true;

                    if( i is BaseWeapon && CraftResources.GetType( ( (BaseWeapon)i ).Resource ) == CraftResourceType.Metal )
                        return true;
                }
            }

            return false;
        }

        private bool HasClothItems()
        {
            foreach( Item i in Items )
            {
                if( i != null && !i.Deleted )
                {
                    if( i is IScissorable )
                        return true;
                }
            }

            return false;
        }

        private bool Resmelt( Mobile from, Item item, CraftResource resource )
        {
            try
            {
                if( CraftResources.GetType( resource ) != CraftResourceType.Metal )
                    return false;

                CraftResourceInfo info = CraftResources.GetInfo( resource );

                if( info == null || info.ResourceTypes.Length == 0 )
                    return false;

                CraftItem craftItem = DefBlacksmithy.CraftSystem.CraftItems.SearchFor( item.GetType() );

                if( craftItem == null || craftItem.Resources.Count == 0 )
                    return false;

                CraftRes craftResource = craftItem.Resources.GetAt( 0 );

                if( craftResource.Amount < 2 )
                    return false; // Not enough metal to resmelt

                Type resourceType = info.ResourceTypes[ 0 ];
                Item ingot = (Item)Activator.CreateInstance( resourceType );

                if( item is DragonBardingDeed || ( item is BaseArmor && ( (BaseArmor)item ).PlayerConstructed ) ||
                    ( item is BaseWeapon && ( (BaseWeapon)item ).PlayerConstructed ) ||
                    ( item is BaseClothing && ( (BaseClothing)item ).PlayerConstructed ) )
                    ingot.Amount = craftResource.Amount / 2;
                else
                    ingot.Amount = 1;

                item.Delete();

                if( !TryDropItem( from, item, false ) )
                    from.AddToBackpack( ingot );

                from.PlaySound( 0x2A );
                from.PlaySound( 0x240 );

                return true;
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }

            return false;
        }

        private void SmeltIngots( Mobile from )
        {
            Item[] tools = from.Backpack.FindItemsByType( typeof( BaseTool ) );

            bool hasTool = false;
            foreach( Item tool in tools )
            {
                if( tool is BaseTool && ( (BaseTool)tool ).CraftSystem == DefBlacksmithy.CraftSystem )
                    hasTool = true;
            }

            if( !hasTool )
            {
                from.SendLocalizedMessage( 1079822 ); // You need a blacksmithing tool in order to salvage ingots.
                return;
            }

            bool anvil, forge;
            DefBlacksmithy.CheckAnvilAndForge( from, 2, out anvil, out forge );

            if( !forge )
            {
                from.SendLocalizedMessage( 1044265 ); // You must be near a forge.
                return;
            }

            int salvaged = 0;
            int notSalvaged = 0;

            for( int i = 0; i < Items.Count; i++ )
            {
                Item item = Items[ i ];

                if( item != null && !item.Deleted )
                {
                    if( item is BaseArmor )
                    {
                        if( Resmelt( from, item, ( (BaseArmor)item ).Resource ) )
                            salvaged++;
                        else
                            notSalvaged++;
                    }
                    else if( item is BaseWeapon )
                    {
                        if( Resmelt( from, item, ( (BaseWeapon)item ).Resource ) )
                            salvaged++;
                        else
                            notSalvaged++;
                    }
                    else if( item is DragonBardingDeed )
                    {
                        if( Resmelt( from, item, ( (DragonBardingDeed)item ).Resource ) )
                            salvaged++;
                        else
                            notSalvaged++;
                    }
                }
            }

            from.SendLocalizedMessage( 1079973, String.Format( "{0}\t{1}", salvaged, salvaged + notSalvaged ) ); // Salvaged: ~1_COUNT~/~2_NUM~ blacksmithed items
        }

        private void SmeltCloth( Mobile from )
        {
            Scissors scissors = from.Backpack.FindItemByType( typeof( Scissors ) ) as Scissors;
            if( scissors == null )
            {
                from.SendLocalizedMessage( 1079823 ); // You need scissors in order to salvage cloth.
                return;
            }

            int salvaged = 0;
            int notSalvaged = 0;

            for( int i = 0; i < Items.Count; i++ )
            {
                Item item = Items[ i ];

                if( item != null && !item.Deleted )
                {
                    if( item is IScissorable )
                    {
                        if( ( (IScissorable)item ).Scissor( from, scissors ) )
                            salvaged++;
                        else
                            notSalvaged++;
                    }
                }
            }

            from.SendLocalizedMessage( 1079974, String.Format( "{0}\t{1}", salvaged, salvaged + notSalvaged ) ); // Salvaged: ~1_COUNT~/~2_NUM~ tailored items
        }

        private void SmeltAll( Mobile from )
        {
            SmeltIngots( from );

            SmeltCloth( from );
        }

        #region entries
        private class SalvageAllEntry : ContextMenuEntry
        {
            private SalvageBag m_Bag;

            public SalvageAllEntry( SalvageBag bag, bool enabled )
                : base( 6276 )
            {
                m_Bag = bag;

                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if( m_Bag.Deleted )
                    return;

                Mobile from = Owner.From;

                if( from.CheckAlive() )
                    m_Bag.SmeltAll( from );
            }
        }

        private class SalvageIngotsEntry : ContextMenuEntry
        {
            private SalvageBag m_Bag;

            public SalvageIngotsEntry( SalvageBag bag, bool enabled )
                : base( 6277 )
            {
                m_Bag = bag;

                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if( m_Bag.Deleted )
                    return;

                Mobile from = Owner.From;

                if( from.CheckAlive() )
                    m_Bag.SmeltIngots( from );
            }
        }

        private class SalvageClothEntry : ContextMenuEntry
        {
            private SalvageBag m_Bag;

            public SalvageClothEntry( SalvageBag bag, bool enabled )
                : base( 6278 )
            {
                m_Bag = bag;

                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if( m_Bag.Deleted )
                    return;

                Mobile from = Owner.From;

                if( from.CheckAlive() )
                    m_Bag.SmeltCloth( from );
            }
        }
        #endregion

        #region serialization
        public SalvageBag( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
        #endregion
    }
}