/*
<enemyMasteryDefinition Normal = "1" Strong = "10" Elite = "0" Defiant = "0" Boss = "0" />
*/

using System;

using Server;
using Server.Mobiles;
using Server.Regions;

namespace Midgard.Engines.MonsterMasterySystem
{
    public enum MasteryLevel
    {
        Normal = 0,
        Strong,
        Elite,
        Defiant,
        Boss
    }

    public class Core
    {
        public static bool Enabled { get { return Packager.Core.Singleton[ typeof( Core ) ].Enabled; } set { Packager.Core.Singleton[ typeof( Core ) ].Enabled = value; } }

        public static bool Debug = false;

        /// <summary>
        /// If false hue for definition is not applied
        /// </summary>
        public static bool ApplyHueForDefinition = false;

        public static object[] Package_Info = {
							  "Script Title", "Monster Mastery System",
							  "Enabled by Default", true,
							  "Script Version", new Version( 1, 0, 0, 0 ),
							  "Author name", "Dies Irae",
							  "Creation Date", new DateTime( 2009, 08, 07 ),
							  "Author mail-contact", "tocasia@alice.it",
							  "Author home site", "http://www.midgardshard.it",
							  //"Author notes",		   null,
							  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
							  "Provided packages", new string[] { "Midgard.Engines.MonsterMasterySystem" },
							  //"Required packages",	  new string[0],
							  //"Conflicts with packages",new string[0],
							  "Research tags", new string[] { "Monster Mastery System" }
							  };

        public static void Package_Initialize()
        {
        }

        //                                                                                      hue   hits  str   dex   int   speed skill fame  karma dmg label
        public static MasteryValuesDefinition NormalMastery     = new MasteryValuesDefinition( 0x000, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 00, "" );
        public static MasteryValuesDefinition StrongMastery     = new MasteryValuesDefinition( 0x482, 1.50, 1.05, 1.05, 1.05, 1.05, 1.10, 2.00, 1.20, 02, "[Strong]" );
        public static MasteryValuesDefinition EliteMastery      = new MasteryValuesDefinition( 0x483, 2.00, 1.10, 1.10, 1.10, 1.10, 1.20, 3.00, 1.40, 05, "[Elite]" );
        public static MasteryValuesDefinition DefiantMastery    = new MasteryValuesDefinition( 0x484, 2.50, 1.15, 1.15, 1.15, 1.15, 1.30, 4.00, 1.60, 07, "[Defiant]" );
        public static MasteryValuesDefinition BossMastery       = new MasteryValuesDefinition( 0x485, 3.00, 1.20, 1.20, 1.20, 1.20, 1.40, 5.00, 1.80, 10, "[Boss]" );

        public static readonly MasteryValuesDefinition[] Defaults = new MasteryValuesDefinition[]
		{
			NormalMastery,
			StrongMastery,
			EliteMastery,
			DefiantMastery,
			BossMastery
		};

        public static MasteryValuesDefinition GetValuesDefinition( MasteryLevel level )
        {
            return Defaults[ (int)level ];
        }

        public static string GetNameLabel( BaseCreature bc )
        {
            if( bc.Mastery == MasteryLevel.Normal )
                return "";

            MasteryValuesDefinition def = GetValuesDefinition( bc.Mastery );
            return def.Label;
        }

        public static void HandleOnAfterSpawn( BaseCreature bc )
        {
            if( !Enabled )
                return;

            if( Debug )
                Console.WriteLine( "MonsterMasterySystem.Core.HandleOnAfterSpawn" );

            if( CheckConvert( bc, bc.Location, bc.Map ) )
            {
                DungeonRegion dungeon = bc.Region.GetRegion( typeof( DungeonRegion ) ) as DungeonRegion;
                if( dungeon != null )
                {
                    EnemyMasteryDefinition def = dungeon.EnMastDefinition;
                    if( def != null )
                    {
                        MasteryLevel level = def.GetLevel();
                        if( level != MasteryLevel.Normal )
                        {
                            if( Debug )
                                Console.WriteLine( "\tHandleOnAfterSpawn: " + level );

                            if( bc.Map == Map.Ilshenar && level < MasteryLevel.Boss )
                                level++;

                            bc.Mastery = level;
                        }
                    }
                }
            }
        }

        public static void Convert( BaseCreature bc, MasteryLevel oldLevel )
        {
            if( bc == null || bc.IsParagon )
                return;

            if( !Enabled )
                return;

            if( Debug )
                Console.WriteLine( "MonsterMasterySystem.Core.Convert" );

            Logger.WriteConversion( bc );

            if( oldLevel != MasteryLevel.Normal )
                UnConvert( bc, oldLevel );

            MasteryValuesDefinition def = GetValuesDefinition( bc.Mastery );

            if( ApplyHueForDefinition )
                bc.Hue = def.Hue;

            if( bc.HitsMaxSeed >= 0 )
                bc.HitsMaxSeed = (int)( bc.HitsMaxSeed * def.HitsBuff );

            bc.RawStr = (int)( bc.RawStr * def.StrBuff );
            bc.RawInt = (int)( bc.RawInt * def.IntBuff );
            bc.RawDex = (int)( bc.RawDex * def.DexBuff );

            bc.Hits = bc.HitsMax;
            bc.Mana = bc.ManaMax;
            bc.Stam = bc.StamMax;

            for( int i = 0; i < bc.Skills.Length; i++ )
            {
                Skill skill = bc.Skills[ i ];

                if( skill.Base > 0.0 )
                    skill.Base *= def.SkillsBuff;
            }

            bc.PassiveSpeed /= def.SpeedBuff;
            bc.ActiveSpeed /= def.SpeedBuff;

            bc.DamageMin += def.DamageBuff;
            bc.DamageMax += def.DamageBuff;

            if( bc.Fame > 0 )
                bc.Fame = (int)( bc.Fame * def.FameBuff );

            if( bc.Fame > 32000 )
                bc.Fame = 32000;

            // TODO: Mana regeneration rate = Sqrt( buffedFame ) / 4

            if( bc.Karma != 0 )
            {
                bc.Karma = (int)( bc.Karma * def.KarmaBuff );

                if( Math.Abs( bc.Karma ) > 32000 )
                    bc.Karma = 32000 * Math.Sign( bc.Karma );
            }
        }

        public static void UnConvert( BaseCreature bc, MasteryLevel oldLevel )
        {
            if( bc == null )
                return;

            if( !Enabled )
                return;

            if( Debug )
                Console.WriteLine( "MonsterMasterySystem.Core.UnConvert" );

            MasteryValuesDefinition def = GetValuesDefinition( oldLevel );

            if( ApplyHueForDefinition )
                bc.Hue = 0;

            if( bc.HitsMaxSeed >= 0 )
                bc.HitsMaxSeed = (int)( bc.HitsMaxSeed / def.HitsBuff );

            bc.RawStr = (int)( bc.RawStr / def.StrBuff );
            bc.RawInt = (int)( bc.RawInt / def.IntBuff );
            bc.RawDex = (int)( bc.RawDex / def.DexBuff );

            bc.Hits = bc.HitsMax;
            bc.Mana = bc.ManaMax;
            bc.Stam = bc.StamMax;

            for( int i = 0; i < bc.Skills.Length; i++ )
            {
                Skill skill = bc.Skills[ i ];

                if( skill.Base > 0.0 )
                    skill.Base /= def.SkillsBuff;
            }

            bc.PassiveSpeed *= def.SpeedBuff;
            bc.ActiveSpeed *= def.SpeedBuff;

            bc.DamageMin -= def.DamageBuff;
            bc.DamageMax -= def.DamageBuff;

            if( bc.Fame > 0 )
                bc.Fame = (int)( bc.Fame / def.FameBuff );
            if( bc.Karma != 0 )
                bc.Karma = (int)( bc.Karma / def.KarmaBuff );
        }

        private static readonly Type[] m_NotConvertibleTypes = new Type[]
		{
			typeof( Cow )
		};

        public static bool IsNotConvertible( BaseCreature toConvert )
        {
            Type type = toConvert.GetType();
            bool contains = false;

            for( int i = 0; !contains && i < m_NotConvertibleTypes.Length; ++i )
            {
                contains = ( m_NotConvertibleTypes[ i ] == type );
            }

            return contains;
        }

        public static bool CheckConvert( BaseCreature bc, Point3D location, Map m )
        {
            if( !Enabled || bc == null )
                return false;

            if( bc is BaseChampion || bc is Harrower || bc is BaseVendor || bc is BaseEscortable || bc is Clone )
                return false;

            if( IsNotConvertible( bc ) )
                return false;

            return true;
        }

        public static string HandleNameSuffix( BaseCreature bc, string suffix )
        {
            if( bc.Mastery != MasteryLevel.Normal && bc.ShowTag )
            {
                if( suffix.Length == 0 )
                    suffix = GetNameLabel( bc );
                else
                    suffix = String.Concat( suffix, " " + GetNameLabel( bc ) );
            }

            return suffix;
        }

        public static void GenerateLoot( BaseCreature bc )
        {
            if( bc.Mastery == MasteryLevel.Normal )
                return;

            if( bc.Fame < 1250 )
                bc.AddLoot( LootPack.Meager, 2 );
            else if( bc.Fame < 2500 )
                bc.AddLoot( LootPack.Average, 2 );
            else if( bc.Fame < 5000 )
                bc.AddLoot( LootPack.Rich );
            else if( bc.Fame < 10000 )
                bc.AddLoot( LootPack.FilthyRich );
            else
                bc.AddLoot( LootPack.UltraRich );
        }
    }
}