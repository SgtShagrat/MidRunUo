/***************************************************************************
 *							   SkitteringHopperFamiliar.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.SpellSystem
{
	[CorpseName( "a skittering hopper corpse" )]
	public class SkitteringHopperFamiliar : BaseFamiliar
	{
		public SkitteringHopperFamiliar()
		{
			Name = "a skittering hopper";
			Body = 302;
			BaseSoundID = 959;

			SetStr( 41, 65 );
			SetDex( 91, 115 );
			SetInt( 26, 50 );

			SetHits( 310, 450 );

			SetDamage( 3, 5 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 5, 10 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Energy, 5, 10 );

			SetSkill( SkillName.MagicResist, 30.1, 45.0 );
			SetSkill( SkillName.Tactics, 45.1, 70.0 );
			SetSkill( SkillName.Wrestling, 40.1, 60.0 );

			Fame = 300;
			Karma = 0;

			ControlSlots = 1;

			Container pack = Backpack;

			if( pack != null )
				pack.Delete();

			pack = new StrongBackpack();
			pack.Movable = false;

			AddItem( pack );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool IsDispellable { get { return false; } }
		private DateTime m_NextPickup;

		public virtual bool RummageCorpse()
		{
			Corpse toRummage = null;

			foreach ( Item item in this.GetItemsInRange( 3 ) )
			{
				if ( item is Corpse && item.Items.Count > 0 )
				{
					toRummage = (Corpse)item;
					break;
				}
			}

			if ( toRummage == null )
				return false;

			Container pack = this.Backpack;

			if ( pack == null )
			{
				Say("Error, Contact Scripter!");
				return false;
			}

			List<Item> items = toRummage.Items;

			if ( items.Count > 0 )
			{
				//for ( int i = 0; i < items.Count; ++i )
				//{
					Item item = items[Utility.Random( items.Count )];
					Mobile master = this.GetMaster();
					if ( master != null && toRummage.Killer == master )
						AddToBackpack( item );

					return true;
				//}
			}

			if( Utility.Random( 30 ) < 3 )
			{
				Say( "*sniff sniff*" );
			}

			return false;
		}

		public override void OnThink()
		{
			base.OnThink();

			if( DateTime.Now < m_NextPickup )
				return;

			RummageCorpse();

			m_NextPickup = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 3, 6 ) );

			Container pack = Backpack;

			if( pack == null )
				return;

			ArrayList list = new ArrayList();

			foreach( Item item in GetItemsInRange( 2 ) )
			{
				if( item.Movable && item.Stackable )
					list.Add( item );
			}

			int pickedUp = 0;

			for( int i = 0; i < list.Count; ++i )
			{
				Item item = (Item)list[ i ];

				if( !pack.CheckHold( this, item, false, true ) )
					return;

				bool rejected;
				LRReason reject;

				NextActionTime = DateTime.Now;

				Lift( item, item.Amount, out rejected, out reject );

				if( rejected )
					continue;

				Drop( this, Point3D.Zero );

				if( ++pickedUp == 3 )
					break;
			}
		}

		private void ConfirmRelease_Callback( Mobile from, bool okay, object state )
		{
			if( okay )
				EndRelease( from );
		}

		public override void BeginRelease( Mobile from )
		{
			Container pack = Backpack;

			if( pack != null && pack.Items.Count > 0 )
				from.SendGump( new WarningGump( 1060635, 30720, 1061672, 32512, 420, 280, new WarningGumpCallback( ConfirmRelease_Callback ), null ) );
			else
				EndRelease( from );
		}

		#region Pack Animal Methods
		public override bool OnBeforeDeath()
		{
			if( !base.OnBeforeDeath() )
				return false;

			//PackAnimal.CombineBackpacks( this );

			Container pack = this.Backpack;
			Map map = this.Map;

			if ( pack != null && map != null )
			{
				Container newPack = new Backpack();

				for ( int i = pack.Items.Count - 1; i >= 0; --i )
				{
					if ( i >= pack.Items.Count )
						continue;

					newPack.DropItem( pack.Items[i] );
				}

				if ( map != Map.Internal )
					newPack.MoveToWorld( Location, map );
			}

			return true;
		}

		public override DeathMoveResult GetInventoryMoveResultFor( Item item )
		{
			return DeathMoveResult.MoveToCorpse;
		}

		public override bool IsSnoop( Mobile from )
		{
			if( PackAnimal.CheckAccess( this, from ) )
				return false;

			return base.IsSnoop( from );
		}

		public override bool OnDragDrop( Mobile from, Item item )
		{
			if( CheckFeed( from, item ) )
				return true;

			if( PackAnimal.CheckAccess( this, from ) )
			{
				AddToBackpack( item );
				return true;
			}

			return base.OnDragDrop( from, item );
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			PackAnimal.TryPackOpen( this, from );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			PackAnimal.GetContextMenuEntries( this, from, list );
		}
		#endregion

		#region serialization
		public SkitteringHopperFamiliar( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		#endregion
	}
}