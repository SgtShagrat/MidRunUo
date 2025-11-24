using System;
using Midgard.Engines.AdvancedSmelting;
using Midgard.Engines.OldCraftSystem;
using Server;
using Server.Engines.Craft;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	[FlipableAttribute( 0xfbb, 0xfbc )]
	public class Tongs : BaseTool
	{
		public override CraftSystem CraftSystem{ get{ return Core.AOS ? DefBlacksmithy.CraftSystem : null; } }

		[Constructable]
		public Tongs() : base( 0xFBB )
		{
			Weight = 2.0;
		}

		[Constructable]
		public Tongs( int uses ) : base( uses, 0xFBB )
		{
			Weight = 2.0;
		}

		public Tongs( Serial serial ) : base( serial )
		{
        }

        #region mod by Dies Irae
        public override void OnDoubleClick( Mobile from )
        {
            if( Core.AOS )
                return;

            if( IsChildOf( from.Backpack ) )
            {
                from.SendMessage( "What should I use these tongs on?" );
                from.Target = new TongsTarget( this );
            }
            else
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
        }

	    private class TongsTarget : Target
        {
            private readonly Tongs m_Tongs;

            public TongsTarget( Tongs tongs )
                : base( 2, false, TargetFlags.None )
            {
                m_Tongs = tongs;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Tongs.Deleted || m_Tongs.UsesRemaining <= 0 )
                {
                    from.SendMessage( "You have used up this tool." );
                }
                else if( targeted is Item && !( (Item)targeted ).Movable )
                {
                    from.SendMessage( "You cannot use this tongs on that item." );
                }
                if( targeted is Item && !( (Item)targeted ).IsChildOf( from.Backpack ) )
                {
                    from.SendMessage( "That item must be in your backpack." );
                }
                else if( targeted is BaseOre )
                {
                    BaseOre ore = (BaseOre)targeted;

                    if( !ore.Deleted )
                    {
                        from.SendMessage( "Select the forge you want to add this ore to." );
                        from.Target = new AddOreTarget( m_Tongs, ore );
                    }
                    else
                        from.SendMessage( "That ore is no longer accessible." );
                }
                else if( targeted is BaseIngot )
                {
                    BaseIngot ingot = (BaseIngot)targeted;

                    if( !ingot.Deleted )
                    {
                        from.SendMessage( "Select the forge you want to add this ingot to." );
                        from.Target = new AddIngotTarget( m_Tongs, ingot );
                    }
                    else
                        from.SendMessage( "That ore is no longer accessible." );
                }
                else if( targeted is ISmeltable )
                {
                    bool anvil, forge;
                    DefBlacksmithy.CheckAnvilAndForge( from, 2, out anvil, out forge );

                    if( !forge )
                    {
                        from.SendMessage( "You must be near a forge to smelt items." );
                        return;
                    }

                    if( !( (ISmeltable)targeted ).Resmelt( from ) )
                        from.SendMessage( "You can't melt that down into ingots." );
                    else
                        m_Tongs.UsesRemaining--;

                    if( m_Tongs.UsesRemaining <= 0 )
                    {
                        from.SendMessage( "You have used up this tool." );
                        m_Tongs.Delete();
                    }
                }
                else
                {
                    from.SendMessage( "You cannot use this tongs on that item." );
                }
            }
        }

        private class AddOreTarget : Target
        {
            private readonly Tongs m_Tongs;
            private readonly BaseOre m_Ore;

            public AddOreTarget( Tongs tongs, BaseOre ore )
                : base( 2, false, TargetFlags.None )
            {
                m_Tongs = tongs;
                m_Ore = ore;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Tongs.Deleted || m_Tongs.UsesRemaining <= 0 )
                {
                    from.SendMessage( "You have used up this tool." );
                }
                else if( m_Ore == null || m_Ore.Deleted || !m_Ore.IsChildOf( from.Backpack ) )
                {
                    from.SendMessage( "That ore is no longer accessible." );
                }
                else
                {
                   if( targeted is AdvancedForge )
                    {
                        AdvancedForge forge = (AdvancedForge)targeted;

                        if( forge.Movable )
                            from.SendMessage( "That forge is not a valid one." );
                        else if( !from.InRange( forge.GetWorldLocation(), 2 ) )
                            from.SendMessage( "That is too far away." );
                        else if( forge.IsSmelting )
                            from.SendMessage( "You cannot add ores to a forge during a smelting in progress." );
                        else
                            forge.CheckAddResource( m_Ore );

                        m_Tongs.UsesRemaining--;

                        if( m_Tongs.UsesRemaining <= 0 )
                        {
                            from.SendMessage( "You have used up this tool." );
                            m_Tongs.Delete();
                        }
                    }
                    else
                    {
                        from.SendMessage( "You cannot use this tongs on that item." );
                    }
                }
            }
        }

        private class AddIngotTarget : Target
        {
            private readonly Tongs m_Tongs;
            private readonly BaseIngot m_Ingot;

            public AddIngotTarget( Tongs tongs, BaseIngot ingot )
                : base( 2, false, TargetFlags.None )
            {
                m_Tongs = tongs;
                m_Ingot = ingot;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Tongs.Deleted || m_Tongs.UsesRemaining <= 0 )
                {
                    from.SendMessage( "You have used up this tool." );
                }
                else if( m_Ingot == null || m_Ingot.Deleted || !m_Ingot.IsChildOf( from.Backpack ) )
                {
                    from.SendMessage( "That ore is no longer accessible." );
                }
                else
                {
                    if( targeted is AdvancedForge )
                    {
                        AdvancedForge forge = (AdvancedForge)targeted;

                        if( forge.Movable )
                            from.SendMessage( "That forge is not a valid one." );
                        else if( !from.InRange( forge.GetWorldLocation(), 2 ) )
                            from.SendMessage( "That is too far away." );
                        else if( forge.IsSmelting )
                            from.SendMessage( "You cannot add ores to a forge during a smelting in progress." );
                        else
                            forge.CheckAddResource( m_Ingot );

                        m_Tongs.UsesRemaining--;

                        if( m_Tongs.UsesRemaining <= 0 )
                        {
                            from.SendMessage( "You have used up this tool." );
                            m_Tongs.Delete();
                        }
                    }
                    else
                    {
                        from.SendMessage( "You cannot use this tongs on that item." );
                    }
                }
            }
        }

        public override void OnDelete()
        {
            Mobile parent = RootParent as Mobile;

            if( parent != null )
                parent.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "*Houch!! My hands!*" );

            base.OnDelete();
        }
	    #endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}