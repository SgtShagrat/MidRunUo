/***************************************************************************
 *                                  IBurningCreature.cs
 *                            		-------------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.CreatureBurningSystem
{
	public enum LightEffect
	{
		Light,
		Dark
	}

	/// <summary>
	/// Interface giving properties to burn a creature
	/// </summary>
	public interface IBurningCreature
	{
		LightEffect iEffect { get; }
		int iHue { get; }
		int iLevel { get; }
	}

	public static class CreatureBurningSystem
	{
		public static void AddLight( BaseCreature bc )
		{
			if( bc == null || bc.Deleted || !( bc is IBurningCreature ) )
				return;

			IBurningCreature iCreature = (IBurningCreature)bc;
			
			switch( iCreature.iEffect )
			{
				case LightEffect.Light:	bc.AddItem( new LightSource() ); 	break;
				case LightEffect.Dark:	bc.AddItem( new DarkSource() ); 	break;
				default: 													break;
			}			
		}

		public static void HandleHue( BaseCreature bc )
		{
			if( bc == null || bc.Deleted || !( bc is IBurningCreature ) )
				return;

			IBurningCreature iCreature = (IBurningCreature)bc;

			bc.Hue = iCreature.iHue;
		}

		public static string HandleNameSuffix( BaseCreature bc, string suffix )
		{
			if( bc == null || bc.Deleted || !( bc is IBurningCreature ) )
				return suffix;

		    if( suffix.Length == 0 )
				suffix = "(Burning)";
			else
				suffix = String.Concat( suffix, " (Burning)" );

			return suffix;
		}

		public static double HitsBuff   = 5.0;
		public static double StrBuff    = 1.05;
		public static double IntBuff    = 1.20;
		public static double DexBuff    = 1.20;
		public static double SkillsBuff = 1.20;
		public static double SpeedBuff  = 1.20;
		public static double FameBuff   = 1.40;
		public static double KarmaBuff  = 1.40;
		public static int    DamageBuff = 5;

		public static void DoFireMorph( BaseCreature bc )
		{
			if( !(bc is IBurningCreature) )
				return;

			if ( bc.HitsMaxSeed >= 0 )
				bc.HitsMaxSeed = (int)( bc.HitsMaxSeed * HitsBuff );
			
			bc.RawStr = (int)( bc.RawStr * StrBuff );
			bc.RawInt = (int)( bc.RawInt * IntBuff );
			bc.RawDex = (int)( bc.RawDex * DexBuff );

			bc.Hits = bc.HitsMax;
			bc.Mana = bc.ManaMax;
			bc.Stam = bc.StamMax;

			for( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = bc.Skills[i];

				if ( skill.Base > 0.0 )
					skill.Base *= SkillsBuff;
			}

			bc.PassiveSpeed /= SpeedBuff;
			bc.ActiveSpeed /= SpeedBuff;

			bc.DamageMin += DamageBuff;
			bc.DamageMax += DamageBuff;

			if ( bc.Fame > 0 )
				bc.Fame = (int)( bc.Fame * FameBuff );

			if ( bc.Fame > 32000 )
				bc.Fame = 32000;

			if ( bc.Karma != 0 )
			{
				bc.Karma = (int)( bc.Karma * KarmaBuff );

				if( Math.Abs( bc.Karma ) > 32000 )
					bc.Karma = 32000 * Math.Sign( bc.Karma );
			}			
		}

		public static readonly double ChanceToSpeak = 0.01;

		public static void FireThoughts( BaseCreature bc )
		{
			if( !(bc is IBurningCreature) )
				return;

			if( bc.Combatant != null && ChanceToSpeak > Utility.RandomDouble() )
			{
				Mobile combatant = bc.Combatant;

				switch ( Utility.Random( 4 ) )
				{
					case 0: bc.Say( true, "* a fire lights in the dark *" );  break;
					case 1: bc.Say( true, String.Format( "Come here {0}! Let me ignite your soul!", combatant.Name ) );  break;
					case 2: bc.Say( true, "* the creature smokes and burns *" ); break;
					case 3: bc.Say( true, String.Format("Do you like being cooked damned {0}", combatant.Name ) ); break;
				}
			}
		}

		public static readonly int ExplosionRange = 5;

		public static void Explode( BaseCreature bc )
		{
			if( !(bc is IBurningCreature) )
				return;

			List<Mobile> targets = new List<Mobile>();

			if( bc.Map != null )
			{
				foreach( Mobile m in bc.GetMobilesInRange( ExplosionRange ) )
				{
					if( bc != m && SpellHelper.ValidIndirectTarget( bc, m ) && bc.CanBeHarmful( m, false ) && ( bc.InLOS( m ) ) )
					{
						if( m is BaseCreature && ((BaseCreature)m).Controlled )
							targets.Add( m );
						else if( m.Player )
							targets.Add( m );
					}
				}
			}

			int min = ((IBurningCreature)bc).iLevel;
			int max = ((IBurningCreature)bc).iLevel * 5;

			for( int i = 0; i < targets.Count; ++i )
			{
				Mobile m = targets[ i ];

				int damage = Utility.RandomMinMax( min, max );

				if( !m.Player )
					damage *= 5;

				bc.DoHarmful( m );
				
				m.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot ); // flamestrike
				m.PlaySound( 0x208 );

                SpellHelper.Damage( TimeSpan.Zero, m, damage, 0, 100, 0, 0, 0 );
			}
		}
		
		public static void GenerateFireLoot( BaseCreature bc )
		{
			if( !(bc is IBurningCreature) )
				return;

			switch( ((IBurningCreature)bc).iLevel )
			{
				case 1:
					bc.AddLoot( LootPack.Meager, 3 ); break;
				case 2:
					bc.AddLoot( LootPack.Average, 3 ); break;
				case 3:	
					bc.AddLoot( LootPack.Rich, 3 ); break;
				case 4:
					bc.AddLoot( LootPack.FilthyRich, 3 ); break;
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
					bc.AddLoot( LootPack.UltraRich, 3 ); break;
				case 10:
					bc.AddLoot( LootPack.SuperBoss ); break;
				default: break;
			}
		}
	}
}
