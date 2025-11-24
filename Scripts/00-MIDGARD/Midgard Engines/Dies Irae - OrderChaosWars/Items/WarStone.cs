/***************************************************************************
 *                               WarStone.cs
 *                            -----------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Network;

namespace Midgard.Engines.OrderChaosWars
{
    public class WarStone : Item
    {
        [Constructable]
        public WarStone()
            : base( 0xEDE )
        {
            Movable = false;

            if( Core.Instance.CurrentBattle != null )
                WarType = Core.Instance.CurrentBattle.Definition.Wartype;
            else
                WarType = BattleType.None;
        }

        public override string DefaultName
        {
            get { return "war stone"; }
        }

        [CommandProperty( AccessLevel.Counselor, AccessLevel.Administrator )]
        public BattleType WarType { get; set; }

        [CommandProperty( AccessLevel.Administrator )]
        public WarPhase WarPhase
        {
            get { return Core.Instance.CurrentPhase; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public string CurrentWar
        {
            get
            {
                if( Core.Instance.CurrentBattle != null )
                    return Core.Instance.CurrentBattle.Definition.WarName;
                else
                    return "no war pending";
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public bool HasTimer
        {
            get { return Core.Instance.HasTimer; }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( WarType == BattleType.None )
                return;

            bool canAccess = from.AccessLevel > AccessLevel.Player || ( Core.Find( from ) != Virtue.None );

            if( !canAccess )
                return;

            if( !from.InRange( GetWorldLocation(), 2 ) )
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
            else
                from.SendGump( new WarGump( from ) );
        }

        #region serialization
        public WarStone( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (int)WarType );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        WarType = (BattleType)reader.ReadInt();
                        break;
                    }
            }
        }
        #endregion
    }
}