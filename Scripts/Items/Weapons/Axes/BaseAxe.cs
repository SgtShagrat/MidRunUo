using System;
using System.Collections;
using System.Collections.Generic;

using Midgard.Items;

using Server;
using Server.Engines.Craft;
using Server.Items;
using Server.Engines.Harvest;
using Server.ContextMenus;

namespace Server.Items
{
	public interface IAxe
	{
		bool Axe( Mobile from, BaseWeapon axe );
	}

	public abstract class BaseAxe : BaseMeleeWeapon, IUsesRemaining
	{
		public override int DefHitSound{ get{ return 0x232; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override SkillName DefSkill{ get{ return SkillName.Swords; } }
		public override WeaponType DefType{ get{ return WeaponType.Axe; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Slash2H; } }

		public virtual HarvestSystem HarvestSystem{ get{ return Lumberjacking.System; } }

		private int m_UsesRemaining;
		private bool m_ShowUsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool ShowUsesRemaining
		{
			get { return m_ShowUsesRemaining; }
			set { m_ShowUsesRemaining = value; InvalidateProperties(); }
		}

		public virtual int GetUsesScalar()
		{
			if ( Quality == WeaponQuality.Exceptional )
				return 200;

			return 100;
		}

		public override void UnscaleDurability()
		{
			base.UnscaleDurability();

			int scale = GetUsesScalar();

			m_UsesRemaining = ((m_UsesRemaining * 100) + (scale - 1)) / scale;
			InvalidateProperties();
		}

		public override void ScaleDurability()
		{
			base.ScaleDurability();

			int scale = GetUsesScalar();

			m_UsesRemaining = ((m_UsesRemaining * scale) + 99) / 100;
			InvalidateProperties();
		}

		public BaseAxe( int itemID ) : base( itemID )
		{
			m_UsesRemaining = 150 * 2; // modifica by Dies Irae
		}

		public BaseAxe( Serial serial ) : base( serial )
		{
		}
		
		#region Mod by Magius(CHE)
		protected override void SetupHitRateAndEvasionBonus (out double hitrate, out double evasion)
		{
			hitrate = +24.0;
			evasion = -6.0;
		}
		#endregion
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( HarvestSystem == null || Deleted )
				return;

            Point3D loc = this.GetWorldLocation();

			if ( !from.InLOS( loc ) || !from.InRange( loc, 2 ) )
			{
				from.LocalOverheadMessage( Server.Network.MessageType.Regular, 0x3E9, 1019045 ); // I can't reach that
				return;
			}
			else if ( !this.IsAccessibleTo( from ) )
			{
				this.PublicOverheadMessage( Server.Network.MessageType.Regular, 0x3E9, 1061637 ); // You are not allowed to access this.
				return;
			}

            #region mod by Dies Irae
            if( !HarvestSystem.CheckHarvest( from, this ) )
                return;

            if( CraftSystem.IsCrafting( from ) )
            {
                from.SendMessage( "Thou are busy at this time." );
                return;
            }
            #endregion

			if ( !(this.HarvestSystem is Mining) )
				from.SendLocalizedMessage( 1010018 ); // What do you want to use this item on?
			
			HarvestSystem.BeginHarvesting( from, this );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( HarvestSystem != null )
				BaseHarvestTool.AddContextMenuEntries( from, this, list, HarvestSystem );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			writer.Write( (bool) m_ShowUsesRemaining );

			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				{
					m_ShowUsesRemaining = reader.ReadBool();
					goto case 1;
				}
				case 1:
				{
					m_UsesRemaining = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					if ( m_UsesRemaining < 1 )
						m_UsesRemaining = 150;

					break;
				}
			}
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );

            #region mod by Dies Irae
            if( this is IDoubleWeapon )
                return;
            #endregion

			if ( AxesSpecialMoveEnabled && !Core.AOS && (attacker.Player || attacker.Body.IsHuman) && Layer == Layer.TwoHanded && (attacker.Skills[SkillName.Anatomy].Value / 800.0) >= Utility.RandomDouble() )
			{
				StatMod mod = defender.GetStatMod( "Concussion" );

				if ( mod == null )
				{
					defender.SendMessage( "You receive a concussion blow!" );
					defender.AddStatMod( new StatMod( StatType.Int, "Concussion", -(defender.RawInt / 4), TimeSpan.FromSeconds( 30.0 ) ) );

					attacker.SendMessage( "You deliver a concussion blow!" );
					attacker.PlaySound( 0x308 );
				}
			}
		}
	}
}