using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlAntiBard : XmlAttachment
    {
        [CommandProperty( AccessLevel.Seer )]
        public int Difficulty { get; set; }

        [CommandProperty( AccessLevel.Seer )]
        public bool BardInvulnerable { get; set; }

        public XmlAntiBard( ASerial serial )
            : base( serial )
        {
            BardInvulnerable = true;
            Difficulty = 70;
        }

        [Attachable]
        public XmlAntiBard()
            : this( true, 0 )
        {
        }

        [Attachable]
        public XmlAntiBard( bool invulnerable, int difficulty )
        {
            BardInvulnerable = invulnerable;
            Difficulty = difficulty;
        }

        [Attachable]
        public XmlAntiBard( bool invulnerable, int difficulty, double expiresin )
        {
            BardInvulnerable = invulnerable;
            Difficulty = difficulty;
            Expiration = TimeSpan.FromMinutes( expiresin );
        }

        public override string OnIdentify( Mobile from )
        {
            if( from == null || from.AccessLevel == AccessLevel.Player )
                return null;

            if( Expiration > TimeSpan.Zero )
            {
                return String.Format( "The Bard Immune ability expires in {0} mins", Expiration.TotalMinutes );
            }
            else
            {
                string tmp = BardInvulnerable ? "Enabled" : "disabled";
                return String.Format( "The Bard Immune Ability is {0}", tmp );
            }
        }

        #region serial deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( Difficulty );
            writer.Write( BardInvulnerable );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Difficulty = reader.ReadInt();
            BardInvulnerable = reader.ReadBool();
        }
        #endregion
    }
}