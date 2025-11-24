using System;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public class XmlGeneralEnemyMastery : XmlAttachment
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public int Chance { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PercentIncrease { get; set; }

        #region costruttori
        [Attachable]
        public XmlGeneralEnemyMastery()
        {
            PercentIncrease = 50;
            Chance = 20;
        }

        [Attachable]
        public XmlGeneralEnemyMastery( int increase )
        {
            Chance = 20;
            PercentIncrease = increase;
        }

        [Attachable]
        public XmlGeneralEnemyMastery( int chance, int increase )
        {
            Chance = chance;
            PercentIncrease = increase;
        }

        [Attachable]
        public XmlGeneralEnemyMastery( int chance, int increase, double expiresin )
        {
            Chance = chance;
            PercentIncrease = increase;
            Expiration = TimeSpan.FromMinutes( expiresin );
        }

        public XmlGeneralEnemyMastery( ASerial serial )
            : base( serial )
        {
            PercentIncrease = 50;
            Chance = 20;
        }

        #endregion

        public override void OnAttach()
        {
            base.OnAttach();

            if( AttachedTo is Mobile )
            {
                Mobile m = AttachedTo as Mobile;
                Effects.PlaySound( m, m.Map, 516 );
                m.SendMessage( "You gain the power of Enemy Mastery!" );
            }
        }

        public override void OnWeaponHit( Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven )
        {
            if( Chance <= 0 || Utility.Random( 100 ) > Chance )
                return;

            if( defender != null && attacker != null )
            {
                if( !( defender.Player ) )
                {
                    defender.Damage( damageGiven * PercentIncrease / 100, attacker );
                }
            }
        }

        public override string OnIdentify( Mobile from )
        {
            string msg;

            if( Expiration > TimeSpan.Zero )
            {
                msg = String.Format( "Enemy Mastery : +{2}% damage , {0}% hitchance expires in {1} mins", Chance, Expiration.TotalMinutes, PercentIncrease );
            }
            else
            {
                msg = String.Format( "Enemy Mastery : +{1}% damage, {0}% hitchance", Chance, PercentIncrease );
            }

            return msg;
        }

        #region serial deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( PercentIncrease );
            writer.Write( Chance );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            PercentIncrease = reader.ReadInt();
            Chance = reader.ReadInt();
        }
        #endregion
    }
}