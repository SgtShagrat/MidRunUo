using System;
using System.Collections;
using System.Collections.Generic;

using Midgard.Engines.NotoSystem;

using Server;
using Server.Items;
using Server.Guilds;
using Server.Multis;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Factions;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells;

using Server.SkillHandlers;
using Server.Engines.XmlPoints;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.SpellSystem;
using Midgard.Misc;
using Midgard.Engines.AdvancedDisguise;
using MidgardTownSystem = Midgard.Engines.MidgardTownSystem;

namespace Server.Misc
{
	public class NotorietyHandlers
	{
		public static void Initialize()
		{
			Notoriety.Hues[Notoriety.Innocent]		= 0x59;
			Notoriety.Hues[Notoriety.Ally]			= 0x3F;
			Notoriety.Hues[Notoriety.CanBeAttacked]	= 0x3B6; // 0x3B2;
			Notoriety.Hues[Notoriety.Criminal]		= 0x3B2;
			Notoriety.Hues[Notoriety.Enemy]			= 0x90;
			Notoriety.Hues[Notoriety.Murderer]		= 0x25; // 0x22;
			Notoriety.Hues[Notoriety.Invulnerable]	= 0x35;

			Notoriety.Handler = new NotorietyHandler( MobileNotoriety );

			Mobile.AllowBeneficialHandler = new AllowBeneficialHandler( Mobile_AllowBeneficial );
			Mobile.AllowHarmfulHandler = new AllowHarmfulHandler( Mobile_AllowHarmful );
		}

		private enum GuildStatus { None, Peaceful, Waring }

		private static GuildStatus GetGuildStatus( Mobile m )
		{
			if( m.Guild == null )
				return GuildStatus.None;
			else if( ( (Guild)m.Guild ).Enemies.Count == 0 && m.Guild.Type == GuildType.Regular )
				return GuildStatus.Peaceful;

			return GuildStatus.Waring;
		}

		private static bool CheckBeneficialStatus( GuildStatus from, GuildStatus target )
		{
			if( from == GuildStatus.Waring || target == GuildStatus.Waring )
				return false;

			return true;
		}

		/*private static bool CheckHarmfulStatus( GuildStatus from, GuildStatus target )
		{
			if ( from == GuildStatus.Waring && target == GuildStatus.Waring )
				return true;

			return false;
		}*/

		public static bool Mobile_AllowBeneficial( Mobile from, Mobile target )
		{
			if( from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player )
				return true;

			Map map = from.Map;
			
			#region Factions
			Faction targetFaction = Faction.Find( target, true );

			if( (!Core.ML || map == Faction.Facet) && targetFaction != null )
			{
				if( Faction.Find( from, true ) != targetFaction )
					return false;
			}
			#endregion

			#region modifica by Dies Irae per le TS
			if( Midgard.Engines.MidgardTownSystem.Notoriety.AreOpposedCitizens( from, target ) )
				return false; // M2pm non della stessa cittadinanza non possono curarsi.

			if( RessSystem.IsImmune( from ) || RessSystem.IsImmune( target ) )
				return false;
			#endregion

			#region Mondain's Legacy
			if ( target is Gregorio )
				return false;
			#endregion

			if( map != null && ( map.Rules & MapRules.BeneficialRestrictions ) == 0 )
				return true; // In felucca, anything goes

			if( !from.Player )
				return true; // NPCs have no restrictions

			if( target is BaseCreature && !( (BaseCreature)target ).Controlled )
				return false; // Players cannot heal uncontrolled mobiles

			if( from is PlayerMobile && ( (PlayerMobile)from ).Young && ( !( target is PlayerMobile ) || !( (PlayerMobile)target ).Young ) )
				return false; // Young players cannot perform beneficial actions towards older players

			#region modifica by Dies Irae
			if( XmlPointsAttach.AreInAnyGame( target ) )
				return XmlPointsAttach.AreTeamMembers( from, target ); // to allow beneficial acts between team members
			#endregion

			Guild fromGuild = from.Guild as Guild;
			Guild targetGuild = target.Guild as Guild;

			if( fromGuild != null && targetGuild != null && ( targetGuild == fromGuild || fromGuild.IsAlly( targetGuild ) ) )
				return true; // Guild members can be beneficial

			return CheckBeneficialStatus( GetGuildStatus( from ), GetGuildStatus( target ) );
		}

		public static bool Mobile_AllowHarmful( Mobile from, Mobile target )
		{
			if( from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player )
				return true;

			Map map = from.Map;

			#region Mondain's Legacy
			if ( target is Gregorio )
			{
				if ( Gregorio.IsMurderer( from ) )
					return true;
				
				from.SendLocalizedMessage( 1075456 ); // You are not allowed to damage this NPC unless your on the Guilty Quest
				return false;
			}
			#endregion

			#region modifica by Dies Irae per gli Young
			if( !CheckAggressor( from.Aggressors, target ) && !CheckAggressed( from.Aggressed, target ) && target is PlayerMobile && ( (PlayerMobile)target ).CheckYoungProtection( from ) )
				return false; // I pg young che superano il check CheckYoungProtection non possono essere attaccati a meno che nn attacchino loro per primi.

			if( RessSystem.IsImmune( from ) || RessSystem.IsImmune( target ) )
				return false;
			#endregion

			if( map != null && ( map.Rules & MapRules.HarmfulRestrictions ) == 0 )
				return true; // In felucca, anything goes

			BaseCreature bc = from as BaseCreature;

			if( !from.Player && !( bc != null && bc.GetMaster() != null && bc.GetMaster().AccessLevel == AccessLevel.Player ) )
			{
				if( !CheckAggressor( from.Aggressors, target ) && !CheckAggressed( from.Aggressed, target ) && target is PlayerMobile && ( (PlayerMobile)target ).CheckYoungProtection( from ) )
					return false;

				return true; // Uncontrolled NPCs are only restricted by the young system
			}

			#region modifica by Dies Irae
			if( XmlPointsAttach.AreChallengers( from, target ) )
				return true;
			#endregion

			Guild fromGuild = GetGuildFor( from.Guild as Guild, from );
			Guild targetGuild = GetGuildFor( target.Guild as Guild, target );

			if( fromGuild != null && targetGuild != null && ( fromGuild == targetGuild || fromGuild.IsAlly( targetGuild ) || fromGuild.IsEnemy( targetGuild ) ) )
				return true; // Guild allies or enemies can be harmful

			if( target is BaseCreature && ( ( (BaseCreature)target ).Controlled || ( ( (BaseCreature)target ).Summoned && from != ( (BaseCreature)target ).SummonMaster ) ) )
				return false; // Cannot harm other controlled mobiles

			if( target.Player )
				return false; // Cannot harm other players

			if( !( target is BaseCreature && ( (BaseCreature)target ).InitialInnocent ) )
			{
				if( Notoriety.Compute( from, target ) == Notoriety.Innocent )
					return false; // Cannot harm innocent mobiles
			}

			return true;
		}

		public static Guild GetGuildFor( Guild def, Mobile m )
		{
			Guild g = def;

			BaseCreature c = m as BaseCreature;

			if( c != null && c.Controlled && c.ControlMaster != null )
			{
				c.DisplayGuildTitle = false;

				if( c.Map != Map.Internal && ( Core.AOS || Guild.NewGuildSystem || c.ControlOrder == OrderType.Attack || c.ControlOrder == OrderType.Guard ) )
					g = (Guild)( c.Guild = c.ControlMaster.Guild );
				else if( c.Map == Map.Internal || c.ControlMaster.Guild == null )
					g = (Guild)( c.Guild = null );
			}

			return g;
		}

		public static int CorpseNotoriety( Mobile source, Corpse target )
		{
			if( target.AccessLevel > AccessLevel.Player )
				return Notoriety.CanBeAttacked;

			Body body = (Body)target.Amount;

			BaseCreature cretOwner = target.Owner as BaseCreature;

			if( cretOwner != null )
			{
				Guild sourceGuild = GetGuildFor( source.Guild as Guild, source );
				Guild targetGuild = GetGuildFor( target.Guild as Guild, target.Owner );

				if( sourceGuild != null && targetGuild != null )
				{
					if( sourceGuild == targetGuild || sourceGuild.IsAlly( targetGuild ) )
						return Notoriety.Ally;
					else if( sourceGuild.IsEnemy( targetGuild ) || sourceGuild.IsTempEnemy( target.Owner ) )
						return Notoriety.Enemy;
				}

				Faction srcFaction = Faction.Find( source, true, true );
				Faction trgFaction = Faction.Find( target.Owner, true, true );

				if( srcFaction != null && trgFaction != null && srcFaction != trgFaction && source.Map == Faction.Facet )
					return Notoriety.Enemy;

				#region modifica by Dies Irae per le TS
				int corpseNoto;
				if( Midgard.Engines.MidgardTownSystem.Notoriety.HandleTownCorpseNotoriety( source, target, out corpseNoto ) )
					return corpseNoto;
				#endregion

				if( CheckHouseFlag( source, target.Owner, target.Location, target.Map ) )
					return Notoriety.CanBeAttacked;

				int actual = Notoriety.CanBeAttacked;

				if( target.Kills >= 5 || ( body.IsMonster && IsSummoned( target.Owner as BaseCreature ) ) || ( target.Owner is BaseCreature && ( ( (BaseCreature)target.Owner ).AlwaysMurderer || ( (BaseCreature)target.Owner ).IsAnimatedDead ) ) )
					actual = Notoriety.Murderer;

				#region mod by Dies Irae [PermaRed]
				if( target.Owner is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)target.Owner ).PermaRed )
					return Notoriety.Murderer;
				#endregion

				if( DateTime.Now >= ( target.TimeOfDeath + Corpse.MonsterLootRightSacrifice ) )
					return actual;

				Party sourceParty = Party.Get( source );

				List<Mobile> list = target.Aggressors;

				for( int i = 0; i < list.Count; ++i )
				{
					if( list[ i ] == source || ( sourceParty != null && Party.Get( list[ i ] ) == sourceParty ) )
						return actual;
				}

				return Notoriety.Innocent;
			}
			else
			{
				if( target.Kills >= 5 || ( body.IsMonster && IsSummoned( target.Owner as BaseCreature ) ) || ( target.Owner is BaseCreature && ( ( (BaseCreature)target.Owner ).AlwaysMurderer || ( (BaseCreature)target.Owner ).IsAnimatedDead ) ) )
					return Notoriety.Murderer;

				if( target.Criminal )
					return Notoriety.Criminal;

				Guild sourceGuild = GetGuildFor( source.Guild as Guild, source );
				Guild targetGuild = GetGuildFor( target.Guild as Guild, target.Owner );

				if( sourceGuild != null && targetGuild != null )
				{
					if( sourceGuild == targetGuild || sourceGuild.IsAlly( targetGuild ) )
						return Notoriety.Ally;
					else if( sourceGuild.IsEnemy( targetGuild ) || sourceGuild.IsTempEnemy( target.Owner ) )
						return Notoriety.Enemy;
				}

				Faction srcFaction = Faction.Find( source, true, true );
				Faction trgFaction = Faction.Find( target.Owner, true, true );

				if( srcFaction != null && trgFaction != null && srcFaction != trgFaction && source.Map == Faction.Facet )
				{
					List<Mobile> secondList = target.Aggressors;

					for( int i = 0; i < secondList.Count; ++i )
					{
						if( secondList[ i ] == source || secondList[ i ] is BaseFactionGuard )
							return Notoriety.Enemy;
					}
				}

				#region mod by Dies Irae
				if( target.Owner != null )
				{
					PlayerMobile owner = target.Owner as PlayerMobile;
					if( owner != null && owner.PermaFlags.Contains( source ) )
						return Notoriety.CanBeAttacked;
				}
				#endregion

				#region modifica by Dies Irae per le TS
				int corpseNoto;
				if( Midgard.Engines.MidgardTownSystem.Notoriety.HandleTownCorpseNotoriety( source, target, out corpseNoto ) )
					return corpseNoto;
				#endregion

				if( target.Owner != null && target.Owner is BaseCreature && ( (BaseCreature)target.Owner ).AlwaysAttackable )
					return Notoriety.CanBeAttacked;

				if( CheckHouseFlag( source, target.Owner, target.Location, target.Map ) )
					return Notoriety.CanBeAttacked;

				if( !( target.Owner is PlayerMobile ) && !IsPet( target.Owner as BaseCreature ) )
					return Notoriety.CanBeAttacked;

				List<Mobile> list = target.Aggressors;

				for( int i = 0; i < list.Count; ++i )
				{
					if( list[ i ] == source )
						return Notoriety.CanBeAttacked;
				}

				return Notoriety.Innocent;
			}
		}

		public static int MobileNotoriety( Mobile source, Mobile target )
		{
			if( Core.AOS && ( target.Blessed || ( target is BaseVendor && ( (BaseVendor)target ).IsInvulnerable ) || target is PlayerVendor || target is TownCrier ) )
				return Notoriety.Invulnerable;

			if( target.AccessLevel > AccessLevel.Player )
				return Notoriety.CanBeAttacked;

			#region mod by Dies Irae [SketchGump]
			int sketchNoto;
			if( SketchGump.HandleMobileNotoriety( source, target, out sketchNoto ) )
				return sketchNoto;
			#endregion

			#region mod by Dies Irae [custom noto]
			int forcedNoto;
			if( MidgardNotoHelper.HandleMobileNotoriety( source, target, out forcedNoto ) )
				return forcedNoto;
			#endregion

			if( source.Player && !target.Player && source is PlayerMobile && target is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)target;

				Mobile master = bc.GetMaster();

				if( master != null && master.AccessLevel > AccessLevel.Player )
					return Notoriety.CanBeAttacked;

				master = bc.ControlMaster;

				if ( /* Core.ML && */ master != null )
				{
					//edit by Arlas: cavalli dei murderer grigi e non rossi
					if( target is BaseMount && master is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)master ).PermaRed )
						return Notoriety.Criminal;

					if( source == master && CheckAggressor( target.Aggressors, source ) )
						return Notoriety.CanBeAttacked;
					else
						return MobileNotoriety( source, master );
				}

				if( !bc.Summoned && !bc.Controlled && ( (PlayerMobile)source ).EnemyOfOneType == target.GetType() )
					return Notoriety.Enemy;
			}

			//fix problema creature in città
			if (!source.Player && !target.Player && target is BaseCreature && IsSummoned( target as BaseCreature ) )
			{
				BaseCreature bc = (BaseCreature)target;
				if ( target.Kills >= 5 || bc.AlwaysMurderer )
					return Notoriety.Murderer;

				Mobile master = bc.GetMaster();

				if ( master != null )
				{
					if( source == master && CheckAggressor( target.Aggressors, source ) )
						return Notoriety.CanBeAttacked;
					else
						return MobileNotoriety( source, master );
				}
			}

			if( target.Kills >= 5 || ( target.Body.IsMonster && IsSummoned( target as BaseCreature ) && !( target is BaseFamiliar ) && !( target is ArcaneFey ) && !( target is Golem ) ) || ( target is BaseCreature && ( ( (BaseCreature)target ).AlwaysMurderer || ( (BaseCreature)target ).IsAnimatedDead ) ) )
				return Notoriety.Murderer;

			#region mod by Dies Irae [PermaRed]
			if( target is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)target ).PermaRed )
				return Notoriety.Murderer;
			#endregion

			#region Mondain's Legacy
			if ( target is Gregorio )
			{
				Gregorio gregorio = (Gregorio) target;
				
				if ( Gregorio.IsMurderer( source ) )
					return Notoriety.Murderer;
				
				return Notoriety.Innocent;
			}
			else if( source.Player && target is Engines.Quests.BaseEscort )
				return Notoriety.Innocent;
			#endregion

			if( target.Criminal )
				return Notoriety.Criminal;

			//druidi
			if ( TransformationSpellHelper.UnderTransformation( target, typeof( EtherealVoyageSpell ) ) )
				return Notoriety.Invulnerable;

			#region modifica by Dies Irae [RessSystem - XmlPoints - IsTownBanned - IsTownMurderer - RPGSpellsSystem]
			if( RessSystem.IsImmune( target ) )
				return Notoriety.Invulnerable; // i player vedono l'immune invulnerabile
			else if( RessSystem.IsImmune( source ) )
				return Notoriety.Innocent; // mentre lui vede gli altri innocenti

			if( XmlPointsAttach.AreTeamMembers( source, target ) )
				return Notoriety.Ally;
			else if( XmlPointsAttach.AreChallengers( source, target ) )
				return Notoriety.Enemy;

			//edit by arlas: i bannati cittadini arancioni e non rossi:
			// Indipendentemente da gilde e citta', se un pg e' bannato cittadino
			// ed e' in quella citta' allora e' Arancio.
			// Inoltre, se un player e' cittadino di una citta' ROSSA e' sicuramente ROSSO 
			if( TownHelper.IsTownPermaBanned( target ) )
				return Notoriety.Murderer;

			if( TownHelper.IsTownBanned( target ) )
				return Notoriety.Enemy;

			if ( TownHelper.IsTownMurderer( target ) )
				return Notoriety.Murderer;

			int classNoto;
			if( RPGSpellsSystem.HandleClassMobileNotoriety( source, target, out classNoto ) )
				return classNoto;
			#endregion

			Guild sourceGuild = GetGuildFor( source.Guild as Guild, source );
			Guild targetGuild = GetGuildFor( target.Guild as Guild, target );

			/*
			* source e target sono Gildati, le regole sono
			* quelle di gilda tranne se entrambe le gilde
			* sono associate ad una townstone, in questo
			* caso valgono le regole di ingaggio delle TS
			* quando il target è nella sua città
			*/
			if( sourceGuild != null && targetGuild != null )
			{
				//Se il target è nella sua città e sono entrambi in GS associate a TS
				//non valgono le regole di gilda ma quelle di città
				if( sourceGuild.Town != MidgardTowns.None && targetGuild.Town != MidgardTowns.None && TownHelper.IsInHisOwnCity( target ) )//!TownHelper.IsInHisOwnCity( target ) && 
				{
					//int townNoto;
					//if( MidgardTownSystem.Notoriety.HandleTownMobileNotoriety( source, target, out townNoto ) )
					//	return townNoto;

					//if( MidgardTownSystem.Notoriety.AreSystemsAtWar( TownSystem.Find( sourceGuild ), TownSystem.Find( target ) ) && TownHelper.IsCitizenCriminal( target ) )
					//	return Notoriety.Enemy;

					if( target is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)target ).TownEnemy && sourceGuild.IsEnemy( targetGuild ) && !GuildsHelper.IsGuildAdept( target ) )
						return Notoriety.Enemy;
				}
				else
				{
					if( sourceGuild == targetGuild || sourceGuild.IsAlly( targetGuild ) )
						return Notoriety.Ally;
					else if( sourceGuild.IsEnemy( targetGuild ) && !GuildsHelper.IsGuildAdept( target ) ) // mod by Dies Irae
						return Notoriety.Enemy;
				}
			}

			#region modifica by Dies Irae per le TS
			/*
			 * Si vuole verificare se 'source' vede 'target' arancio 
			 * anche se semplicemente cittadino a causa del CitizenCriminal.
			 * Non serve verificare la gilda nemica poiché se 'sourceGuild' è
			 * non-nulla e 'targetGuild' lo è, evidentemente basta verificare
			 * che le cittadinanze (della gilda e del target) siano in guerra
			 * e che il 'target' sia effettivamente CitizenCriminal.
			 */
			if( sourceGuild != null && targetGuild == null )
			{
				if( MidgardTownSystem.Notoriety.AreSystemsAtWar( TownSystem.Find( sourceGuild ), TownSystem.Find( target ) ) && TownHelper.IsCitizenCriminal( target ) )
					return Notoriety.Enemy;
			}
			#endregion

			/*
			Faction srcFaction = Faction.Find( source, true, true );
			Faction trgFaction = Faction.Find( target, true, true );
			
			if( srcFaction != null && trgFaction != null && srcFaction != trgFaction && source.Map == Faction.Facet )
				return Notoriety.Enemy;
			*/

			if( Stealing.ClassicMode && target is PlayerMobile && ( (PlayerMobile)target ).PermaFlags.Contains( source ) )
				return Notoriety.CanBeAttacked;

			if( target is BaseCreature && ( (BaseCreature)target ).AlwaysAttackable )
				return Notoriety.CanBeAttacked;

			if( CheckHouseFlag( source, target, target.Location, target.Map ) )
				return Notoriety.CanBeAttacked;

			if( !(target is BaseCreature && ((BaseCreature)target).InitialInnocent) )   //If Target is NOT A baseCreature, OR it's a BC and the BC is initial innocent...
			{
				if( !target.Body.IsHuman && !target.Body.IsGhost && !IsPet( target as BaseCreature ) && !(target is PlayerMobile) || !Core.ML && !target.CanBeginAction( typeof( Spells.Seventh.PolymorphSpell ) ) )
					return Notoriety.CanBeAttacked;
			}

			if( CheckAggressor( source.Aggressors, target ) )
				return Notoriety.CanBeAttacked;

			if( CheckAggressed( source.Aggressed, target ) )
				return Notoriety.CanBeAttacked;

			if( target is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)target;

				if( bc.Controlled && bc.ControlOrder == OrderType.Guard && bc.ControlTarget == source )
					return Notoriety.CanBeAttacked;

				#region modifica by Dies Irae per rendere CanBeAttacked le creature di un Aggredito/Aggressore.
				Mobile master = bc.GetMaster();
				if( master != null )
				{
					if( CheckAggressor( master.Aggressors, target ) )
						return Notoriety.CanBeAttacked;
				}

				//if( master != null && Notoriety.Compute( source, master ) == Notoriety.CanBeAttacked )
				//	return Notoriety.CanBeAttacked;
				#endregion
			}

			if( source is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)source;

				Mobile master = bc.GetMaster();
				if( master != null && CheckAggressor( master.Aggressors, target ) )
					return Notoriety.CanBeAttacked;
			}

			#region mod by Dies Irae : bounty system
			if ( source.Player && Midgard.Engines.BountySystem.Core.Attackable( source, target ) )
				return Notoriety.CanBeAttacked;
			#endregion

			#region modifica by Dies Irae per il TownSystem
			int townNoto;
			if( MidgardTownSystem.Notoriety.HandleTownMobileNotoriety( source, target, out townNoto ) )
				return townNoto;
			#endregion

			return Notoriety.Innocent;
		}

		public static bool CheckHouseFlag( Mobile from, Mobile m, Point3D p, Map map )
		{
			BaseHouse house = BaseHouse.FindHouseAt( p, map, 16 );

			if( house == null || house.Public || !house.IsFriend( from ) )
				return false;

			if( m != null && house.IsFriend( m ) )
				return false;

			BaseCreature c = m as BaseCreature;

			if( c != null && !c.Deleted && c.Controlled && c.ControlMaster != null )
				return !house.IsFriend( c.ControlMaster );

			return true;
		}

		public static bool IsPet( BaseCreature c )
		{
			return ( c != null && c.Controlled );
		}

		public static bool IsSummoned( BaseCreature c )
		{
			return ( c != null && /*c.Controlled &&*/ c.Summoned );
		}

		public static bool CheckAggressor( List<AggressorInfo> list, Mobile target )
		{
			for( int i = 0; i < list.Count; ++i )
				if( list[ i ].Attacker == target )
					return true;

			return false;
		}

		public static bool CheckAggressed( List<AggressorInfo> list, Mobile target )
		{
			for( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo info = list[ i ];

				if( !info.CriminalAggression && info.Defender == target )
					return true;
			}

			return false;
		}

		#region mod by Dies Irae
		public static void ClearAggressors( Mobile m )
		{
			for( int i = 0; i < m.Aggressors.Count; i++ )
			{
				AggressorInfo c = m.Aggressors[ i ];
				m.RemoveAggressor( c.Attacker );
			}
		}

		public static void ClearAggressed( Mobile m )
		{
			for( int i = 0; i < m.Aggressed.Count; i++ )
			{
				AggressorInfo c = m.Aggressed[ i ];
				m.RemoveAggressed( c.Defender );
			}
		}
		#endregion
	}
}
