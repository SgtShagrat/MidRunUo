/***************************************************************************
 *                               MiniChampionSpawner.cs
 *
 *   begin                : 14 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.MiniChampionSystem
{
    public class MiniChampionSpawner : Item
    {
        protected MiniChampionSpawn Spawn;

        public MiniChampionSpawner( MiniChampionSpawn spawn )
        {
            Spawn = spawn;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            
            if( Spawn != null )
                Spawn.Delete();
        }

        #region serialization
        public MiniChampionSpawner( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( Spawn );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        Spawn = reader.ReadItem() as MiniChampionSpawn;

                        if( Spawn == null )
                            Delete();

                        break;
                    }
            }
        }
        #endregion
    }
}