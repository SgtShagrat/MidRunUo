/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 23 aprile 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Engines.Craft;

namespace Midgard.Engines.FeedStockRecoverySystem
{
    public class Core
    {
        private static List<CraftSystem> m_Systems = new List<CraftSystem>();

        public static CraftSystem Find( Item item, out CraftItem craftItem )
        {
            if( m_Systems == null || m_Systems.Count == 0 )
            {
                m_Systems = new List<CraftSystem>
                { 
                    DefAlchemy.CraftSystem,
                    DefBlacksmithy.CraftSystem,
                    DefBowFletching.CraftSystem,
                    DefCarpentry.CraftSystem,
                    DefCartography.CraftSystem,
                    DefCooking.CraftSystem,
                    DefGlassblowing.CraftSystem,
                    DefInscription.CraftSystem,
                    DefMasonry.CraftSystem,
                    DefTailoring.CraftSystem,
                    DefTinkering.CraftSystem
                };
            }

            Type t = item.GetType();

            foreach( CraftSystem system in m_Systems )
            {
                craftItem = system.CraftItems.SearchFor( t );
                if( craftItem != null )
                    return system;
            }

            craftItem = null;
            return null;
        }
    }
}