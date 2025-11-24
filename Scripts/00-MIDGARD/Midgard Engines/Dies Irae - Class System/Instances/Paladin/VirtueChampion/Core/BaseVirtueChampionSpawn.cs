/***************************************************************************
 *                               BaseVirtueChampionSpawn.cs
 *
 *   begin                : 18 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.MiniChampionSystem;

using Server;
using Server.Mobiles;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public abstract class BaseVirtueChampionSpawn : MiniChampionSpawn
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public string Key { get; set; }

        public BaseVirtueChampionSpawn()
        {
            ItemID = Config.ChampionSpawnID;
            Key = "undefined key";
        }

        public abstract bool HasRightQuest( Mobile to );

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( !e.Handled && !Active )
            {
                Mobile m = e.Mobile;

                if( !m.Player || !m.InRange( GetWorldLocation(), Config.SpawnerSpeechRange ) )
                    return;

                if( e.Mobile is PlayerMobile && HasRightQuest( e.Mobile ) )
                {
                    if( Insensitive.Compare( e.Speech, Key ) != 0 )
                        return;
                }

                e.Handled = true;

                StartChampion( e.Mobile );
            }
        }

        public virtual void StartChampion( Mobile mobile )
        {
            mobile.SendMessage( "Good luck warrior. May the wisdom be you sword!" );
            Active = true;
        }

        public static Item Construct( Type type, params object[] pars )
        {
            try
            {
                return Activator.CreateInstance( type, pars ) as Item;
            }
            catch
            {
                return null;
            }
        }

        #region serialization
        public BaseVirtueChampionSpawn( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}