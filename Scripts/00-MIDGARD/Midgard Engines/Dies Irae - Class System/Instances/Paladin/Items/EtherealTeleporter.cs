using System;

using Midgard.Engines.Classes;
using Midgard.Engines.SpellSystem;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Items
{
    public class EtherealTeleporter : Teleporter
    {
        [Constructable]
        public EtherealTeleporter()
        {
            SpellLevelToPass = 0;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int SpellLevelToPass { get; private set; }

        public override string DefaultName
        {
            get { return "Midgard Ethereal Teleporter"; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        private void EndMessageLock( object state )
        {
            ( (Mobile)state ).EndAction( this );
        }

        public override bool OnMoveOver( Mobile m )
        {
            if( Active )
            {
                if( !Creatures && !m.Player )
                    return true;

                bool canPass = ClassSystem.IsPaladine( m );

                int level = RPGPaladinSpell.GetPowerLevelByType( m, typeof( PathToHeavenSpell ) );
                if( level < SpellLevelToPass )
                    canPass = false;

                Midgard2PlayerMobile player = m as Midgard2PlayerMobile;
                bool shouldPunish = m.Karma < -5000 || ( player != null && ( player.PermaRed || player.Town == MidgardTowns.BuccaneersDen || player.Kills > 5 ) );

                if( !canPass )
                {
                    if( m.BeginAction( this ) )
                    {
                        m.SendMessage( 0x22, "Thou may not pass." );

                        Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), m );

                        if( shouldPunish )
                        {
                            m.BoltEffect( 0 );
                            m.SendMessage( 0x22, "Thou have been punished for that insolence!" );
                            m.Damage( DiceRoll.OneDiceHundred.Roll() );
                        }
                    }

                    return false;
                }

                StartTeleport( m );
                return false;
            }

            return true;
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            if( from.AccessLevel > AccessLevel.Player )
                LabelTo( from, string.Format( from.Language == "ITA" ? "Livello Richiesto: {0}" : "Level Required: {0}", SpellLevelToPass ) );
        }

        #region serialization
        public EtherealTeleporter( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.WriteEncodedInt( SpellLevelToPass );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        SpellLevelToPass = reader.ReadEncodedInt();
                        break;
                    }
            }
        }
        #endregion
    }
}