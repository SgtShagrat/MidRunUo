using System;
using Midgard.Engines.Races;
using Midgard.Items;
using Server;

namespace Midgard.Engines.MidgardTownSystem
{
    [RaceAllowance( typeof( MountainDwarf ) )]
    public class JailHandcuffs : Handcuffs
    {
        public override string DefaultName { get { return "jail handcuffs"; } }

        [Constructable]
        public JailHandcuffs()
        {
        }

        #region serialization
        public JailHandcuffs( Serial serial )
            : base( serial )
        {
        }

        public override DeathMoveResult OnParentDeath( Mobile parent )
        {
            return DeathMoveResult.RemainEquiped;
        }

        public override void OnDoubleClick( Mobile from )
        {
            Condemn condemn = TownJailSystem.Instance.FindCondemnForPrisoner( from );
            if( condemn != null )
                condemn.Profile.SendCondemnMessage();
            else
                CheckDelete();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( CheckDelete ) );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        internal void CheckDelete()
        {
            if( !Deleted )
            {
                Mobile owner = Parent as Mobile;
                if( Parent != null && !TownJailSystem.Instance.IsActuallyCondemned( owner ) )
                    Delete();
                else if( Parent == null )
                    Delete();
            }
        }
        #endregion
    }
}