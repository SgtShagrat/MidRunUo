/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 13 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class Core
    {
        public static VirtueDefinition FindDefinitionByVirtue( Virtues virtue )
        {
            if( Config.Virtues != null )
            {
                foreach( BaseVirtue v in Config.Virtues )
                {
                    if( v.Definition.Virtue == virtue )
                        return v.Definition;
                }
            }

            return null;
        }

        public static AntiVirtueDefinition FindDefinitionByAntiVirtue( AntiVirtues antiVirtue )
        {
            foreach( BaseAntiVirtue v in Config.AntiVirtues )
            {
                if( v.Definition.AntiVirtue == antiVirtue )
                    return v.Definition;
            }

            return null;
        }

        public static string GetVirtueName( Virtues virtue )
        {
            return Enum.GetName( typeof( Virtues ), virtue );
        }

        public static string GetAntiVirtueName( AntiVirtues antiVirtue )
        {
            return Enum.GetName( typeof( AntiVirtues ), antiVirtue );
        }

        public static bool FindItemByItemID( int x, int y, int z, Map map, int itemID )
        {
            return FindItemByItemID( new Point3D( x, y, z ), map, itemID );
        }

        public static bool FindItemByItemID( Point3D p, Map map, int itemID )
        {
            IPooledEnumerable eable = map.GetItemsInRange( p );

            foreach( Item item in eable )
            {
                if( item.Z == p.Z && item.ItemID == itemID )
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        public static Item FindItemByType( Point3D p, Map map, Type type )
        {
            IPooledEnumerable eable = map.GetItemsInRange( p );

            foreach( Item item in eable )
            {
                if( item.Z == p.Z && item.GetType() == type )
                {
                    eable.Free();
                    return item;
                }
            }

            eable.Free();
            return null;
        }

        public static Type FindQuestByVirtue( Virtues virtue, int stage )
        {
            VirtueDefinition def = FindDefinitionByVirtue( virtue );
            if( def != null )
            {
                switch( stage )
                {
                    case 1: return def.QuestStageOneType;
                    case 2: return def.QuestStageTwoType;
                    case 3: return def.QuestStageThreeType;
                    default: return def.QuestStageOneType;
                }
            }

            return null;
        }

        public static Type FindQuestByVirtue( AntiVirtues virtue, int stage )
        {
            AntiVirtueDefinition def = FindDefinitionByAntiVirtue( virtue );
            if( def != null )
            {
                switch( stage )
                {
                    case 1: return def.QuestStageOneType;
                    case 2: return def.QuestStageTwoType;
                    case 3: return def.QuestStageThreeType;
                    default: return def.QuestStageOneType;
                }
            }

            return null;
        }
    }
}
