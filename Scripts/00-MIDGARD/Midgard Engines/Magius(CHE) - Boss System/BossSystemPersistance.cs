using System;
using Server;

namespace Midgard.Engines.BossSystem
{
    internal class BossSystemPersistence : Item
    {
        private static BossSystemPersistence m_Singleton;

        public BossSystemPersistence()
        {
            m_Singleton = this;
        }

        public static BossSystemPersistence Singleton
        {
            get { return m_Singleton; }
        }

        public long SyncKey { get; private set; }

        public static void EnsureExistence()
        {
            if( m_Singleton == null )
                new BossSystemPersistence();
        }

        private long CreateNewSyncroKey()
        {
            SyncKey = DateTime.Now.ToBinary();
            return SyncKey;
        }

        #region serialization
        public BossSystemPersistence( Serial serial )
            : base( serial )
        {
            m_Singleton = this;
        }

        public override void Serialize( GenericWriter writer )
        {
            CreateNewSyncroKey();

            base.Serialize( writer );

            writer.Write( SyncKey ); // SyncKey
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            SyncKey = reader.ReadLong();
        }
        #endregion
    }
}