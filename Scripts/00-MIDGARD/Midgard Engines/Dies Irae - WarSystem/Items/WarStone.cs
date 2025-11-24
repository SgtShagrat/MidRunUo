/***************************************************************************
 *                               WarStone.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Network;

namespace Midgard.Engines.WarSystem
{
    public class WarStone : Item
    {
        public override string DefaultName
        {
            get { return "war stone"; }
        }

        [CommandProperty( AccessLevel.Counselor, AccessLevel.Administrator )]
        public BattleType WarType
        {
            get { return Core.Instance.CurrentBattle.Definition.WarNameEnum; }
        }

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

        [CommandProperty( AccessLevel.Administrator )]
        public WarTeam OwnerTeam { get; set; }

        [Constructable]
        public WarStone()
            : this( null )
        {
        }

        [Constructable]
        public WarStone( WarTeam team )
            : base( 0xEDE )
        {
            OwnerTeam = team;
            Movable = false;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( WarType == BattleType.None )
                return;

            if( !from.InRange( GetWorldLocation(), 2 ) )
            {
                SendMessage( from, "I can't reach that." );
            }
            else if( !from.Alive )
            {
                SendMessage( from, "Thou cannot do that while dead!" );
            }
            else
            {
                WarTeam playerTeam = Utility.Find( from );
                bool isGM = from.AccessLevel > AccessLevel.Player;

                // Criteria to access this stone:
                // GMs can always access the stone
                // if the stone has a valid owner team only not-waring players or members can access
                bool canAccess = isGM || ( OwnerTeam != null && ( playerTeam == OwnerTeam || playerTeam == null ) );

                if( !canAccess )
                {
                    SendMessage( from, "Thou cannot use this powerful stone." );
                    return;
                }

                if( !isGM && playerTeam == null )
                {
                    switch( WarPhase )
                    {
                        case WarPhase.Idle:
                            SendMessage( from, "There is no war in progress." );
                            break;
                        case WarPhase.BattlePending:
                            SendMessage( from, "Thou cannot sign in while a war is in progress." );
                            break;
                        case WarPhase.PostBattle:
                            SendMessage( from, "Thou cannot sign in. The war is over." );
                            break;
                        case WarPhase.PreBattle:
                            from.SendGump( new ConfirmJoinGump( from, this ) );
                            break;
                    }
                }
                else
                    from.SendGump( new WarGump( from ) );
            }
        }

        private static void SendMessage( Mobile from, string message )
        {
            from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, message );
        }

        public void EndJoin( Mobile from, bool okay )
        {
            if( okay )
            {
                OwnerTeam.AddMember( from, true );
                from.SendMessage( "You joined this war. Now you are a member or {0} team.", OwnerTeam.Name );
            }
        }

        #region serialization
        public WarStone( Serial serial )
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