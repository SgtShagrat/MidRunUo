/*
using Server;

namespace Midgard.Engines.RazorRpvRecorder
{
    using System.Collections;

    public class World
    {
        private static string m_AccountName;
        private static Hashtable m_Items = Hashtable.Synchronized(new Hashtable());
        private static Hashtable m_Mobiles = Hashtable.Synchronized(new Hashtable());
        private static PlayerData m_Player;
        private static string m_PlayerName;
        private static Hashtable m_Servers = new Hashtable();
        private static string m_ShardName = "[None]";

        internal static void AddItem(Item item)
        {
            m_Items[item.Serial] = item;
        }

        internal static void AddMobile(Mobile mob)
        {
            m_Mobiles[mob.Serial] = mob;
        }

        internal static Item FindItem(Serial serial)
        {
            return (m_Items[serial] as Item);
        }

        internal static Mobile FindMobile(Serial serial)
        {
            return (m_Mobiles[serial] as Mobile);
        }

        internal static ArrayList MobilesInRange()
        {
            if (Player == null)
            {
                return MobilesInRange(0x12);
            }
            return MobilesInRange(Player.VisRange);
        }

        internal static ArrayList MobilesInRange(int range)
        {
            ArrayList list = new ArrayList();
            if (Player != null)
            {
                foreach (Mobile mobile in Mobiles.Values)
                {
                    //if (Utility.InRange(Player.Position, mobile.Position, Player.VisRange))
                    //{
                    //    list.Add(mobile);
                    //}
                }
            }
            return list;
        }

        internal static void RemoveItem(Item item)
        {
            m_Items.Remove(item.Serial);
        }

        internal static void RemoveMobile(Mobile mob)
        {
            m_Mobiles.Remove(mob.Serial);
        }

        internal static string AccountName
        {
            get
            {
                return m_AccountName;
            }
            set
            {
                m_AccountName = value;
            }
        }

        internal static Hashtable Items
        {
            get
            {
                return m_Items;
            }
        }

        internal static Hashtable Mobiles
        {
            get
            {
                return m_Mobiles;
            }
        }

        internal static string OrigPlayerName
        {
            get
            {
                return m_PlayerName;
            }
            set
            {
                m_PlayerName = value;
            }
        }

        internal static PlayerData Player
        {
            get
            {
                return m_Player;
            }
            set
            {
                m_Player = value;
            }
        }

        internal static Hashtable Servers
        {
            get
            {
                return m_Servers;
            }
        }

        internal static string ShardName
        {
            get
            {
                return m_ShardName;
            }
            set
            {
                m_ShardName = value;
            }
        }
    }
}
*/