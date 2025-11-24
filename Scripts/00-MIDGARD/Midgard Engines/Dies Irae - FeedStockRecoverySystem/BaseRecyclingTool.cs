/***************************************************************************
 *                               BaseRecyclingTool.cs
 *
 *   begin                : 23 aprile 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Reflection;

using Midgard.Engines.OldCraftSystem;
using Midgard.Gumps;

using Server;
using Server.Engines.Craft;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.FeedStockRecoverySystem
{
    public abstract class BaseRecyclingTool : Item
    {
        private int m_CurCharges;
        private int m_MaxCharges;

        public BaseRecyclingTool( int itemID, int charges )
            : base( itemID )
        {
            m_MaxCharges = charges < DefaultMaxCharges ? charges : DefaultMaxCharges;
            m_CurCharges = m_MaxCharges;
        }

        public abstract double DifficultyBonus { get; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool Debug { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int CurCharges
        {
            get { return m_CurCharges; }
            set
            {
                int oldValue = m_CurCharges;
                if( oldValue != value )
                {
                    if( value < 0 )
                        m_CurCharges = 0;
                    else
                        m_CurCharges = value > MaxCharges ? MaxCharges : value;

                    OnCurChargesChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int MaxCharges
        {
            get { return m_MaxCharges; }
            set
            {
                int oldValue = m_MaxCharges;
                if( oldValue != value )
                {
                    if( value < 0 )
                        m_MaxCharges = 0;
                    else
                        m_MaxCharges = value > DefaultMaxCharges ? DefaultMaxCharges : value;

                    OnMaxChargesChanged( oldValue );
                }
            }
        }

        public int DefaultMaxCharges
        {
            get { return 50; }
        }

        /// <summary>
        /// Check if from can use this tool.
        /// </summary>
        /// <param name = "from">mobile who would use our tool</param>
        /// <param name = "sendFailureMessage">true if a message should be sent to from</param>
        /// <returns>true if item is accessible</returns>
        public bool CheckAccess( Mobile from, bool sendFailureMessage )
        {
            if( from == null || from.Deleted || Deleted )
                return false;

            if( !IsChildOf( from.Backpack ) )
            {
                from.SendMessage( "Recycling tools must be in your backpack to be used!" );
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if item can be recycled from this tool.
        /// </summary>
        /// <param name = "item">item to recycle</param>
        /// <returns>true if item is recyclable with our tool</returns>
        public bool IsRecyclable( Item item )
        {
            if( item == null )
                return false;

            if( IsNotRecyclable( item.GetType() ) )
                return false;

            return ( item is IRecyclable ) || IsSpecialRecyclable( item );
        }

        /// <summary>
        /// Return true if item is part of a craft system but does not implement the recyclable interface
        /// </summary>
        public bool IsSpecialRecyclable( Item item )
        {
            CraftItem craftItem;

            CraftSystem system = Core.Find( item, out craftItem );
            if( system == null )
            {
                DebugMessage( "Oh no! I cant find any valid craft system!" );
                return false;
            }

            CraftRes craftResource = craftItem.Resources.GetAt( 0 );
            if( craftResource.Amount < 2 )
            {
                DebugMessage( "Mmm! The resouces to craft this item are too low!" );
                return false;
            }

            return true;
        }

        private static readonly Type[] m_NotRecyclable = new Type[]
			{
				typeof( KeyRing ), typeof( Key )
			};

        public bool IsNotRecyclable( Type type )
        {
            bool contains = false;

            for( int i = 0; !contains && i < m_NotRecyclable.Length; ++i )
                contains = ( m_NotRecyclable[ i ] == type );

            return contains;
        }

        /// <summary>
        /// Event invoked when current charges of our tool changes
        /// </summary>
        public virtual void OnCurChargesChanged( int oldValue )
        {
            if( m_CurCharges < 1 )
                Delete();
        }

        /// <summary>
        /// Event invoked when max charges of our tool changes
        /// </summary>
        public virtual void OnMaxChargesChanged( int oldValue )
        {
            if( m_MaxCharges < 1 )
                Delete();
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "(charges: {0})", m_CurCharges );
        }

        public override void OnDoubleClick( Mobile from )
        {
            base.OnDoubleClick( from );

            if( !CheckAccess( from, true ) )
                return;

            if( m_CurCharges > 0 )
            {
                from.SendMessage( "Choose an item in your backpack you would like to recycle." );
                from.Target = new RecycleTarget( this );
            }
            else
            {
                from.SendMessage( "This tool is out of charges." );
                Delete();
            }
        }

        private void EndRecycle( Mobile from, Item itemToRecycle )
        {
            if( !CheckAccess( from, true ) )
                return;

            CraftItem craftItem;
            CraftSystem system = Core.Find( itemToRecycle, out craftItem );

            if( craftItem == null )
            {
                DebugMessage( "Oh no. I cant find a valid craft item for this." );
                return;
            }

            double chanceToRecycle = Math.Max( 0.0, GetRecycleChance( from, itemToRecycle, craftItem ) + DifficultyBonus );
            DebugMessage( "Hey, this item has a final chance of {0:F1} of being recycled", chanceToRecycle );

            if( Config.FullChanceToRestock )
                chanceToRecycle = 1.0;

            double roll = Utility.RandomDouble();
            bool success = chanceToRecycle >= roll;

            DebugMessage( "Chance: {0} - Roll: {1} - Success: {2}", chanceToRecycle, roll, success );

            system.PlayCraftEffect( from );

            if( success && TryRecycle( from, itemToRecycle, system ) )
            {
                // from.PlaySound( 64 );
            }
            else
            {
                from.SendMessage( "You failed to recycle this item." );
                itemToRecycle.Delete();

                // from.PlaySound( 63 );
            }

            CurCharges--;
        }

        /// <summary>
        /// Evaluate the chance for an item of being recycled. From 0 to 1.
        /// </summary>
        private double GetRecycleChance( Mobile from, Item itemToRecycle, CraftItem craftItem )
        {
            CraftSkill craftSkill = craftItem.Skills.GetAt( 0 );

            // this is the minimum skill required to craft our item
            double chance = craftSkill.MinSkill;

            // apply here any material difficulty
            if( itemToRecycle is IRecyclable )
                chance /= ( (IRecyclable)itemToRecycle ).GetDifficulty( from );

            DebugMessage( "Hey, this item has a difficulty of {0:F1}% of being recycled", chance );

            return chance / 100.0;
        }

        private bool TryRecycle( Mobile from, Item itemToRecycle, CraftSystem system )
        {
            if( itemToRecycle is IRecyclable )
                return ( (IRecyclable)itemToRecycle ).Recycle( from, this );

            CraftItem craftItem = system.CraftItems.SearchFor( itemToRecycle.GetType() );
            if( craftItem == null )
            {
                from.SendMessage( "That item seems not to be crafted and so cannot be recycled anyway." );
                DebugMessage( "Bad: craft item not found." );
                return false;
            }

            if( craftItem.Resources == null || craftItem.Resources.GetAt( 0 ).Amount < 2 )
            {
                from.SendMessage( "That item seems not to being crafted with valid materials and so cannot be recycled anyway." );
                DebugMessage( "Bad: resouces is null or amount is less then 2." );
                return false;
            }

            CraftRes craftRes = craftItem.Resources.GetAt( 0 );
            if( craftRes == null )
            {
                from.SendMessage( "That item seems not to being crafted with valid resource and so cannot be recycled anyway." );
                DebugMessage( "Bad: main resouce is null or amount is less then 2." );
                return false;
            }

            CraftResource resource;
            if ( itemToRecycle is IResourceItem )
                resource = ( (IResourceItem)itemToRecycle ).Resource;
            else
                resource = CraftResources.GetFromType( craftRes.ItemType );
            
            if( resource == CraftResource.None )
            {
                from.SendMessage( "That item seems not to being crafted with valid resouce and so cannot be recycled anyway." );
                DebugMessage( "Bad: main resouce is none." );
                return false;
            }

            CraftResourceInfo info = CraftResources.GetInfo( resource );
            if( info == null )
            {
                from.SendMessage( "That item seems not to being crafted with valid resource and so cannot be recycled anyway." );
                DebugMessage( "Bad: main resouce craft info is null." );
                return false;
            }

            Type resourceType = null;

            if( info.ResourceTypes.Length > 0 )
                resourceType = info.ResourceTypes[ 0 ];

            if( resourceType == null )
                resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

            if( resourceType != null )
                resourceType = CraftItem.GetMutatedTypeFromBase( resourceType );

            try
            {
                if( resourceType != null )
                {
                    Item res = (Item)Activator.CreateInstance( resourceType );
                    res.LootType = LootType.Regular;

                    bool moreResources = PlayerConstructed( itemToRecycle ) || !CraftResources.IsStandard( resource );

                    itemToRecycle.ScissorHelper( from, res, moreResources ? ( craftItem.Resources.GetAt( 0 ).Amount / 2 ) : 1, false );

                    LogRecycle( from, itemToRecycle, resourceType, res.Amount );

                    return true;
                }
                else
                    return false;
            }
            catch( Exception e )
            {
                Config.Pkg.LogError( e );
                return false;
            }
        }

        private static bool PlayerConstructed( Item item )
        {

            PropertyInfo info = item.GetType().GetProperty( "PlayerConstructed", BindingFlags.Public | BindingFlags.Instance );
            return info != null && (bool)info.GetValue( item, null );
        }

        /*
        private object InvokePublicProperty( object source, string propertyName )
        {
            PropertyInfo info = source.GetType().GetProperty( propertyName, BindingFlags.Public | BindingFlags.Instance );
            if( info != null )
                return info.GetValue( source, null );
            else
            {
                DebugMessage( "Couldn't find a property named '{0}' for type {1}", propertyName, source.GetType().Name );
                return null;
            }
        }
        */

        #region [debug]
        private static void LogRecycle( Mobile from, Item itemToRecycle, Type resourceType, int amount )
        {
            Utility.Log( Config.LogFileName, "{0} - name: {1} - item: {2} - res: {3} - amount {4}",
                DateTime.Now, from.Name, itemToRecycle.GetType().Name, resourceType.Name, amount );
        }

        private void DebugMessage( string format, params object[] args )
        {
            DebugMessage( String.Format( format, args ) );
        }

        private void DebugMessage( string message )
        {
            if( Debug )
            {
                Config.Pkg.LogInfoLine( message );
                PublicOverheadMessage( MessageType.Regular, 0x3B2, true, message );
            }
        }
        #endregion

        #region serial-deserial
        public BaseRecyclingTool( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            writer.WriteEncodedInt( m_CurCharges );
            writer.WriteEncodedInt( m_MaxCharges );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    {
                        m_CurCharges = reader.ReadEncodedInt();
                        m_MaxCharges = reader.ReadEncodedInt();
                        break;
                    }
            }
        }
        #endregion

        private class RecycleTarget : Target
        {
            private readonly BaseRecyclingTool m_Tool;

            public RecycleTarget( BaseRecyclingTool tool )
                : base( 2, true, TargetFlags.None )
            {
                m_Tool = tool;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Tool == null || m_Tool.Deleted )
                    return;

                if( targeted is Item )
                {
                    Item item = (Item)targeted;

                    if( item.IsChildOf( from.Backpack ) )
                    {
                        if( item.IsAccessibleTo( from ) )
                        {
                            if( item is ISmeltable )
                            {
                                CraftItem craftItem;
                                CraftSystem system = Core.Find( item, out craftItem );
                                if( system != null && system == DefBlacksmithy.CraftSystem )
                                {
                                    from.SendMessage( "That item is not recyclable. Try to smelt it instead." );
                                    return;
                                }
                            }

                            if( item.LootType != LootType.Regular )
                            {
                                from.SendMessage( "That item is not recyclable. You can recycle only regular items (not blessed or newbied)." );
                                return;
                            }

                            if( item.Items != null && item.Items.Count > 0 )
                            {
                                from.SendMessage( "That item is not recyclable. You can recycle only one item only." );
                                return;
                            }

                            if( m_Tool.IsRecyclable( item ) )
                            {
                                if( Config.ResmeltWithoutConfirm )
                                {
                                    m_Tool.EndRecycle( from, item );
                                }
                                else
                                {
                                    from.CloseGump( typeof( ConfirmRecycleGump ) );
                                    from.SendGump( new ConfirmRecycleGump( m_Tool, item ) );
                                }
                            }
                            else
                                from.SendMessage( "The selected item cannot be recycled by this tool." );
                        }
                        else
                            from.SendMessage( "The selected item is not accessible." );
                    }
                    else
                        from.SendMessage( "Item you would to recycle have to be in your backpack." );
                }
                else
                    from.SendMessage( "The selected item cannot be recycled by this tool." );
            }

            protected override void OnTargetOutOfRange( Mobile from, object targeted )
            {
                from.SendMessage( "The selected item is not accessible." );
            }
        }

        private class ConfirmRecycleGump : SmallConfirmGump
        {
            public ConfirmRecycleGump( BaseRecyclingTool tool, Item itemToRecycle )
                : base( Config.RecycleString, ConfirmJoin_Callback, new object[] { tool, itemToRecycle } )
            {
            }

            private static void ConfirmJoin_Callback( Mobile from, bool okay, object state )
            {
                if( okay )
                {
                    object[] states = state as object[];
                    if( states == null )
                        return;

                    BaseRecyclingTool tool = states[ 0 ] as BaseRecyclingTool;
                    if( tool == null || tool.Deleted )
                        return;

                    Item itemToRecycle = states[ 1 ] as Item;
                    if( itemToRecycle == null )
                        return;

                    tool.EndRecycle( from, itemToRecycle );
                }
            }
        }
    }
}