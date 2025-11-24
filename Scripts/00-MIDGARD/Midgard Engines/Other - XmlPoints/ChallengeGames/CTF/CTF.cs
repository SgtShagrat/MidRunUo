using System;

/*
** CTFGauntlet
** ArteGordon
** updated 12/05/04
**
** used to set up a capture the flag pvp challenge game through the XmlPoints system.
*/

namespace Server.Engines.XmlPoints
{
    public class CTFBase : Item
    {
        private CTFFlag m_Flag;
        private CTFGauntlet m_Gauntlet;
        private bool m_HasFlag;
        private int m_ProximityRange = 1;

        public CTFBase( CTFGauntlet gauntlet, int team )
            : base( 0x1183 )
        {
            Movable = false;
            Hue = BaseChallengeGame.TeamColor( team );
            Team = team;
            Name = String.Format( "Team {0} Base", team );
            m_Gauntlet = gauntlet;

            // add the flag

            Flag = new CTFFlag( this, team );
            Flag.HomeBase = this;
            HasFlag = true;
        }

        #region serialization

        public CTFBase( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( Team );
            writer.Write( m_ProximityRange );
            writer.Write( m_Flag );
            writer.Write( m_Gauntlet );
            writer.Write( m_HasFlag );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Team = reader.ReadInt();
            ProximityRange = reader.ReadInt();
            Flag = reader.ReadItem() as CTFFlag;
            m_Gauntlet = reader.ReadItem() as CTFGauntlet;
            m_HasFlag = reader.ReadBool();
        }

        #endregion

        public int Team { get; set; }

        public CTFFlag Flag
        {
            get { return m_Flag; }
            set { m_Flag = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ProximityRange
        {
            get { return m_ProximityRange; }
            set { m_ProximityRange = value; }
        }

        public bool HasFlag
        {
            get { return m_HasFlag; }
            set { m_HasFlag = value; }
        }

        public override bool HandlesOnMovement
        {
            get { return m_Gauntlet != null; }
        }

        public override void OnDelete()
        {
            // delete any flag associated with the base
            if( m_Flag != null )
                m_Flag.Delete();

            base.OnDelete();
        }

        public override void OnLocationChange( Point3D oldLocation )
        {
            // set the flag location
            PlaceFlagAtBase();
        }

        public void PlaceFlagAtBase()
        {
            if( m_Flag != null )
            {
                m_Flag.MoveToWorld( new Point3D( Location.X + 1, Location.Y, Location.Z + 4 ), Map );
            }
        }

        public override void OnMapChange()
        {
            // set the flag location
            PlaceFlagAtBase();
        }

        public void ReturnFlag()
        {
            ReturnFlag( true );
        }

        public void ReturnFlag( bool verbose )
        {
            if( Flag == null )
                return;

            PlaceFlagAtBase();
            HasFlag = true;
            if( m_Gauntlet != null && verbose )
            {
                m_Gauntlet.GameBroadcast( 100419, Team ); // "Team {0} flag has been returned to base"
            }
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            if( m == null || m_Gauntlet == null )
                return;

            if( m.AccessLevel > AccessLevel.Player )
                return;

            // look for players within range of the base
            // check to see if player is within range of the spawner
            if( ( Parent == null ) && Utility.InRange( m.Location, Location, m_ProximityRange ) )
            {
                var entry = m_Gauntlet.GetParticipant( m ) as CTFGauntlet.ChallengeEntry;

                if( entry == null )
                    return;

                bool carryingflag = false;
                // is the player carrying a flag?
                foreach( CTFBase b in m_Gauntlet.HomeBases )
                {
                    if( b != null && !b.Deleted && b.Flag != null && b.Flag.RootParent == m )
                    {
                        carryingflag = true;
                        break;
                    }
                }

                // if the player is on an opposing team and the flag is at the base and the player doesnt already
                // have a flag then give them the flag
                if( entry.Team != Team && HasFlag && !carryingflag && Flag != null && m.InLOS( Flag ) )
                {
                    m.AddToBackpack( m_Flag );
                    HasFlag = false;
                    m_Gauntlet.GameBroadcast( 100420, entry.Team, Team ); // "Team {0} has the Team {1} flag"
                    m_Gauntlet.GameBroadcastSound( 513 );
                }
                else if( entry.Team == Team && HasFlag )
                {
                    // if the player has an opposing teams flag then give them a point and return the flag
                    foreach( CTFBase b in m_Gauntlet.HomeBases )
                    {
                        if( b != null && !b.Deleted && b.Flag != null && b.Flag.RootParent == m && b.Team != entry.Team )
                        {
                            m_Gauntlet.GameBroadcast( 100421, entry.Team ); // "Team {0} has scored"
                            m_Gauntlet.AddScore( entry );

                            Effects.SendTargetParticles( entry.Participant, 0x375A, 35, 20,
                                                        BaseChallengeGame.TeamColor( entry.Team ), 0x00, 9502,
                                                        (EffectLayer)255, 0x100 );
                            // play the score sound
                            m_Gauntlet.ScoreSound( entry.Team );

                            b.ReturnFlag( false );
                            break;
                        }
                    }
                }
            }
        }
    }
}