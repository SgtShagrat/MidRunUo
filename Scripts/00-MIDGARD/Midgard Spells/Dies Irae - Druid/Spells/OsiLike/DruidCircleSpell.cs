/***************************************************************************
 *							   DruidCircleSpell.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public class DruidCircleSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Druid Circle", "En Ess Ohm",
			224,
			9011,
			Reagent.Kindling,
			Reagent.DestroyingAngel,
			Reagent.FertileDirt,
			Reagent.SpringWater,
			Reagent.PetrifiedWood,
			Reagent.MandrakeRoot
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
		(
			typeof( DruidCircleSpell ),
			"Creates a Druid Focus crystal which enhances other spellweaving spells.",
			"Permette di creare il Druid Focus, uno smeraldo in grado di potenziare tutte le magie del druido."+
			"Durata (SK/24 + Druidi) Ore.",
			0x59d8
		);

		public override ExtendedSpellInfo ExtendedInfo { get { return m_ExtendedInfo; } }

		public override SpellCircle Circle
		{
			get { return SpellCircle.Fourth; }
		}

		public DruidCircleSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if( !IsValidLocation( Caster.Location, Caster.Map ) )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Devi stare dentro un cerchio druidico per utilizzare questo incantesimo." : "You must be standing inside an druid circle to use this spell.") );
				return false;
			}

			if( GetDruids().Count < 2 )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Non ci sono abbastanza druidi per formare un circolo." : "There are not enough druids present to create a Druid Focus.") );
				return false;
			}

			return base.CheckCast();
		}

		public override void OnCast()
		{
			if( CheckSequence() )
			{
				Caster.FixedParticles( 0x3779, 10, 20, 0x0, EffectLayer.Waist );
				Caster.PlaySound( 0x5C0 );

				List<Mobile> arcanists = GetDruids();

				TimeSpan duration = TimeSpan.FromHours( Math.Max( 1, (int)( Caster.Skills.Spellweaving.Value / 24 ) ) );

				duration += TimeSpan.FromHours( Math.Min( 5, arcanists.Count ) );

				// int strengthBonus = Math.Min( arcanists.Count, 5 );
				//int strengthBonus = (int)( Caster.Skills[ DamageSkill ].Value / ( 10 - GetPowerLevel() ) );
				int strengthBonus = Math.Max( 1, arcanists.Count );//Math.Min( 5,  (int)( Caster.Skills[ DamageSkill ].Base * (double)arcanists.Count ) ) ) ; //mod by Magius(CHE): midgard balance

				for( int i = 0; i < arcanists.Count; i++ )
					GiveDruidFocus( arcanists[ i ], duration, strengthBonus );
			}

			FinishSequence();
		}

		private static bool IsValidLocation( Point3D location, Map map )
		{
			Tile lt = map.Tiles.GetLandTile( location.X, location.Y ); // Land   Tiles			

			if( IsValidTile( lt.ID ) && lt.Z == location.Z )
				return true;

			Tile[] tiles = map.Tiles.GetStaticTiles( location.X, location.Y ); // Static Tiles

			for( int i = 0; i < tiles.Length; ++i )
			{
				Tile t = tiles[ i ];
				// ItemData id = TileData.ItemTable[t.ID & 0x3FFF];

				int tand = t.ID & 0x3FFF;

				if( t.Z != location.Z )
					continue;
				else if( IsValidTile( tand ) )
					return true;
			}

			IPooledEnumerable eable = map.GetItemsInRange( location, 0 ); // Added  Tiles

			foreach( Item item in eable )
			{
				if( item == null || item.Z != location.Z )
					continue;
				else if( IsValidTile( item.ItemID ) )
				{
					eable.Free();
					return true;
				}
			}

			eable.Free();
			return false;
		}

		public static bool IsValidTile( int itemID )
		{
			return true;
		}

		private List<Mobile> GetDruids()
		{
			List<Mobile> weavers = new List<Mobile>();

			weavers.Add( Caster );

			foreach( Mobile m in Caster.GetMobilesInRange( 20 ) )
			{
				if( m != Caster && Caster.CanBeBeneficial( m, false ) && !( m is Clone ) && (m.Skills.Spellweaving.Value > 0) )//Edit by Arlas, was: && Math.Abs( Caster.Skills.Spellweaving.Value - m.Skills.Spellweaving.Value ) <= 20
				{
					weavers.Add( m );
				}
			}

			return weavers;
		}

		private static void GiveDruidFocus( Mobile to, TimeSpan duration, int strengthBonus )
		{
			if( to == null )
				return;

			DruidFocus focus = FindDruidFocus( to );

			if( focus == null )
			{
				DruidFocus f = new DruidFocus( duration, strengthBonus );
				if( to.PlaceInBackpack( f ) )
				{
					to.AddStatMod( new StatMod( StatType.Str, "[StrDruidFocus]", strengthBonus, duration ) );
					to.AddStatMod( new StatMod( StatType.Dex, "[DexDruidFocus]", strengthBonus, duration ) );
					to.AddStatMod( new StatMod( StatType.Int, "[IntDruidFocus]", strengthBonus, duration ) );

					f.SendTimeRemainingMessage( to );
					to.SendMessage( (to.Language == "ITA" ? "Un potenziamento druidico appare nel tuo zaino." : "A druid focus appears in your backpack.") );
				}
				else
				{
					f.Delete();
				}
			}
			else
			{
				to.SendMessage( (to.Language == "ITA" ? "Il tuo potenziamento è rinnovato." : "Your druid focus is renewed.") );
				focus.LifeSpan = duration;
				focus.CreationTime = DateTime.Now;
				focus.StrengthBonus = strengthBonus;
				focus.InvalidateProperties();
				focus.SendTimeRemainingMessage( to );

				to.AddStatMod( new StatMod( StatType.Str, "[StrDruidFocus]", strengthBonus, duration ) );
				to.AddStatMod( new StatMod( StatType.Dex, "[DexDruidFocus]", strengthBonus, duration ) );
				to.AddStatMod( new StatMod( StatType.Int, "[IntDruidFocus]", strengthBonus, duration ) );
			}
		}
	}
}