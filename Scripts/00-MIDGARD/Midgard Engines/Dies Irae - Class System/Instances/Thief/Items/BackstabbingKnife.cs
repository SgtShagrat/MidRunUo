/***************************************************************************
 *                               BackstabbingKnife
 *                            -----------------------
 *   begin                : 12 gennaio, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Classes;

using Server;
using Server.Commands;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Midgard.Items
{
    [Flipable( 0xF52, 0xF51 )]
    public class BackstabbingKnife : BaseKnife
    {
        public override int OldStrengthReq { get { return 5; } }
        public override int OldSpeed { get { return 53; } }

        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 40; } }

        public override int NumDice { get { return 1; } }
        public override int NumSides { get { return 10; } }
        public override int DiceBonus { get { return 0; } }

        public override SkillName DefSkill { get { return SkillName.Fencing; } }

        #region costanti
        private static readonly double StealingToUse = 00.0;
        private static readonly double HidingToUse = 00.0;
        private static readonly double StealthToUse = 00.0;
        private static readonly double AnatomyToUse = 00.0;
        private static readonly double FencingToUse = 50.0;

        private static readonly double DelayOnFail = 10.0; // in secondi
        private static readonly double DelayOnSuccess = 20.0; // in secondi
        private static readonly double NonPlayerScalar = 1.5; // 150%
        #endregion

        [Constructable]
        public BackstabbingKnife()
            : base( 0xF52 )
        {
            Weight = 1.0;
        }

        public override void OnSingleClick( Mobile from )
        {
            // if( Parent == from || ClassSystem.IsThief( from ) )
			if( Poison != null && PoisonCharges > 0 )
			{
				string veleno = (from.Language == "ITA" ? "veleno" : "poison");

				if ( Poison.Level >= 39 )//Lentezza
					veleno = (from.Language == "ITA" ? "vel. lentezza" : "slow poison");
				else if ( Poison.Level >= 34 )//Blocco
					veleno = (from.Language == "ITA" ? "vel. blocco" : "block poison");
				else if ( Poison.Level >= 29 )//Paralisi
					veleno = (from.Language == "ITA" ? "vel. paralisi" : "paralysis poison");
				else if ( Poison.Level >= 24 )//Stanchezza
					veleno = (from.Language == "ITA" ? "vel. stanchezza" : "fatigue poison");
				else if ( Poison.Level >= 19 )//Magia
					veleno = (from.Language == "ITA" ? "vel. magia" : "magic poison");
				else
					veleno = (from.Language == "ITA" ? "veleno" : "poison");

				switch( Poison.RealLevel )
				{
					case 0: veleno = (from.Language == "ITA" ? "["+veleno+" minore: {0}]" : "[lesser "+veleno+": {0}]"); break;// Lesser
					case 1: veleno = (from.Language == "ITA" ? "["+veleno+": {0}]" : "["+veleno+": {0}]"); break;// Regular
					case 2: veleno = (from.Language == "ITA" ? "["+veleno+" maggiore: {0}]" : "[great "+veleno+": {0}]"); break;// Great
					case 3: veleno = (from.Language == "ITA" ? "["+veleno+" mortale: {0}]" : "[deadly "+veleno+": {0}]"); break;// Deadly
					case 4: veleno = (from.Language == "ITA" ? "["+veleno+" letale: {0}]" : "[lethal "+veleno+": {0}]"); break;// Lethal
					default: veleno = "["+veleno+" : {0}]"; break;
				}
				LabelTo( from, string.Format(veleno, PoisonCharges));//string.Format( from.Language == "ITA" ? "[avvelenato]" : "[Poisoned]" ) );
			}
			//if( Poison != null && PoisonCharges > 0 )
			//	LabelTo( from, string.Format( from.Language == "ITA" ? "[Veleno : {0}]" : "[Poisoned : {0}]", PoisonCharges ) );

			LabelTo( from, from.Language == "ITA" ? "coltello per pugnalare" : "backstabbing knife" );
            // else
            //  LabelTo( from, "a dagger" );
        }

        public override void OnDoubleClick( Mobile from )
        {
            Midgard2PlayerMobile thief = from as Midgard2PlayerMobile;

            if( thief == null )
                return;

            if( !thief.CanBeginAction( typeof( BackstabbingKnife ) ) )
            {
                thief.SendMessage( thief.Language == "ITA" ? "Non puoi ancora usare questo coltello." : "You cannot use a backstabbing knife yet." );
            }
            if( !CheckKnifeUse( thief, true ) )
            {
                return;
            }
            else if( !CheckThiefSkills( from ) )
            {
                BadAction( from, null, false );
            }
            else if( thief.BeginAction( typeof( BackstabbingKnife ) ) )
            {
                thief.SendMessage( thief.Language == "ITA" ? "Seleziona il bersaglio da pugnalare... 10 secondi rimasti." : "Target the guy you want to stab... 10 seconds remaining." );
                thief.Target = new InternalTarget( this );
                thief.Target.BeginTimeout( thief, TimeSpan.FromSeconds( 10.0 ) );

                // sanity
                Timer.DelayCall( TimeSpan.FromSeconds( DelayOnSuccess + 10.0 ), new TimerStateCallback( ReleaseBackstabbingKnifeLock ), thief );
            }
            else
            {
                thief.SendMessage( thief.Language == "ITA" ? "Non puoi ancora usare questo coltello." : "You cannot use a backstabbing knife yet." );
            }
        }

        public bool CheckKnifeUse( Mobile thief, bool message )
        {
            if( thief == null )
                return false;
            else if( thief.FindItemOnLayer( Layer.OneHanded ) != this )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "Devi avere il coltello in mano per usarlo." : "You must equip the knife in order to use it." );
            }
            //else if( thief.Mounted )
            //{
            //    if( message )
            //        thief.SendMessage( "You cannot use a backstabbing knife while mounted." );
            //}
            else if( !thief.Body.IsHuman )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "Non puoi accoltellare nessuno in questa forma." : "You cannot use a backstabbing knife while in a such form." );
            }
            else if( thief.Skills[ SkillName.Fencing ].Base < 50.0 )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "Con la tua abilità attuale non pugnaleresti un formaggio con un grissino." : "You must train in knife tecniques!" );
            }
            else if( !thief.Warmode )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "Devi essere in war mode!" : "You must be in war mode!" );
            }
            else if( !thief.Hidden )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "Sei troppo visibile!" : "You aren't hidden!" );
            }
            else if( thief.AllowedStealthSteps <= 0 )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "Non ti stai muovendo nelle ombre!" : "You aren't moving stealthily!" );
            }
            else if( thief.Region.IsPartOf( typeof( Server.Regions.Jail ) ) )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "Hey, questo non è carino da fare in prigione." : "Hey, this actions are prohibited in Jail." );
            }
            else
            {
                return true;
            }

            return false;
        }

        private static bool CheckThiefSkills( Mobile thiev )
        {
            if( thiev == null || thiev.Deleted )
            {
                return false;
            }
            if( thiev.Skills[ SkillName.Stealing ].Value < StealingToUse )
            {
                SendSkillMessage( thiev, StealingToUse, SkillName.Stealing );
            }
            else if( thiev.Skills[ SkillName.Hiding ].Value < HidingToUse )
            {
                SendSkillMessage( thiev, HidingToUse, SkillName.Hiding );
            }
            else if( thiev.Skills[ SkillName.Stealth ].Value < StealthToUse )
            {
                SendSkillMessage( thiev, StealthToUse, SkillName.Stealth );
            }
            else if( thiev.Skills[ SkillName.Anatomy ].Value < AnatomyToUse )
            {
                SendSkillMessage( thiev, AnatomyToUse, SkillName.Anatomy );
            }
            else if( thiev.Skills[ SkillName.Fencing ].Value < FencingToUse )
            {
                SendSkillMessage( thiev, FencingToUse, SkillName.Fencing );
            }
            else
            {
                return true;
            }

            return false;
        }

        private static void SendSkillMessage( Mobile thief, double value, SkillName skill )
        {
            thief.SendMessage( thief.Language == "ITA" ? "Devi avere {0} in {1} per usare questo coltello." : "You must have {0} in {1} to use this knife.", value.ToString( "F1" ), skill );
        }

        private static bool CheckResisted( Mobile thiev, Mobile victim )
        {
            int thievPars = ( thiev.Str * 10 ) + ( thiev.Dex * 10 ) + thiev.Skills[ SkillName.Anatomy ].Fixed;
            int victimPars = ( victim.Str * 10 ) + ( victim.Dex * 10 ) + thiev.Skills[ SkillName.DetectHidden ].Fixed;

            int chance = victimPars > 0 ? (int)( ( ( 50 * thievPars ) / (double)victimPars ) ) : 100;

            return chance > Utility.Random( 100 );
        }

        private static void BadAction( Mobile thief, object message, bool isCriminalAction )
        {
            if( message != null )
            {
                if( message is string )
                    thief.SendMessage( (string)message );
                else
                    thief.SendLocalizedMessage( (int)message );
            }

            if( isCriminalAction )
                thief.CriminalAction( false );

            double skillToCheck = ( thief.Skills[ SkillName.Hiding ].Value + thief.Skills[ SkillName.Stealth ].Value ) / 240.0;

            if( Utility.RandomDouble() > skillToCheck )
                thief.RevealingAction();

            Timer.DelayCall( TimeSpan.FromSeconds( DelayOnFail ), new TimerStateCallback( ReleaseBackstabbingKnifeLock ), thief );
        }

        private static bool IsBehind( Mobile thief, Mobile victim, bool message )
        {
            Point3D thiefLoc = thief.Location;
            Direction thiefDir = thief.Direction;
            Point3D victimLoc = victim.Location;
            Direction victimDir = victim.Direction;

            if( thiefLoc == victimLoc )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "{0} è troppo vicino!" : "{0} is too close!", victim.Name );
            }
            else if( !Utility.InRange( thiefLoc, victimLoc, 1 ) )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "{0} è troppo lontano!" : "{0} is too far!", victim.Name );
            }
            else if( !Offset( victimDir & Direction.Mask, thiefLoc, victimLoc ) )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "Non sei dietro la tua vittima." : "You are not behind your victim." );
            }
            /*else if( ( ( thiefDir & Direction.Mask ) != Utility.GetDirection( thiefLoc, victimLoc ) ) )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "La vittima deve stare davanti a te!" : "But you are not facing your victim, fool!", victim.Name );
            }*/
            /*
            else if( ( ( victimDir & Direction.Mask ) != Utility.GetDirection( thiefLoc, victimLoc ) ) )
            {
                if( message )
                    thief.SendMessage( thief.Language == "ITA" ? "{0} non è di spalle!" : "{0} isn't turned back!", victim.Name );
            }
            */
            else
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  W   U   N
        ///  L       R
        ///  S   D   E
        /// </summary>
        public static bool Offset( Direction d, Point3D t, Point3D v )
        {
            switch( d & Direction.Mask )
            {
                case Direction.North:
                    return Match( v, t, -1, +1 ) || Match( v, t, 0, +1 ) || Match( v, t, +1, +1 );
                case Direction.South:
                    return Match( v, t, -1, -1 ) || Match( v, t, 0, -1 ) || Match( v, t, +1, -1 );
                case Direction.West:
                    return Match( v, t, +1, -1 ) || Match( v, t, +1, 0 ) || Match( v, t, +1, -1 );
                case Direction.East:
                    return Match( v, t, -1, -1 ) || Match( v, t, -1, 0 ) || Match( v, t, -1, +1 );
                case Direction.Right:
                    return Match( v, t, -1, 0 ) || Match( v, t, -1, +1 ) || Match( v, t, 0, +1 );
                case Direction.Left:
                    return Match( v, t, 0, -1 ) || Match( v, t, +1, -1 ) || Match( v, t, +1, 0 );
                case Direction.Down:
                    return Match( v, t, -1, 0 ) || Match( v, t, -1, -1 ) || Match( v, t, 0, -1 );
                case Direction.Up:
                    return Match( v, t, +1, 0 ) || Match( v, t, +1, +1 ) || Match( v, t, 0, +1 );
            }

            return false;
        }

        /// <summary>
        /// Compare two points applying an offset to their coordinates.
        /// </summary>
        private static bool Match( Point3D first, Point3D second, int xOffset, int yOffset )
        {
            return ( first.X + xOffset == second.X ) && ( first.Y + yOffset == second.Y );
        }

        protected void OnTarget( Mobile thief, object target )
        {
            if( Deleted )
                return;

            Mobile victim = target as Mobile;

            if( victim == null )
            {
                BadAction( thief, thief.Language == "ITA" ? "Hai scelto la persona sbagliata..." : "Bad target guy...", false );
            }
            else if( !CheckKnifeUse( thief, true ) )
            {
                BadAction( thief, null, false );
            }
            else if( !CheckThiefSkills( thief ) )
            {
                BadAction( thief, null, false );
            }
            else if( !thief.CanSee( victim ) )
            {
                BadAction( thief, thief.Language == "ITA" ? "Bersaglio non in vista." : "Target can not be seen.", false );
            }
            else if( !thief.CanBeHarmful( victim ) )
            {
                BadAction( thief, thief.Language == "ITA" ? "Hai scelto la persona sbagliata..." : "Bad target guy...", false );
            }
            //else if( m_Victim.Mounted && !thief.Mounted )
            //{
            //    BadAction( thief, "You cannot use a backstabbing knife on a mounted guy without riding a mount!", false );
            //}
            //else if( !m_Victim.Mounted && thief.Mounted )
            //{
            //    BadAction( thief, "You cannot use a backstabbing knife on a walking guy while riding a mount!", false );
            //}
            else if( !IsBehind( thief, victim, true ) )
            {
                BadAction( thief, thief.Language == "ITA" ? "Devi stare dietro la vittima: stessa direzione e esattamente un passo di distanza." : "You must be behind your victim: same direction and exactly one step distance.", false );
            }
            else
            {
                thief.PrivateOverheadMessage( MessageType.Regular, 37, true, "*backstabbing*", thief.NetState );
                Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( EndStab ), new object[] { thief, victim } );
            }
        }

        private static void EndStab( object state )
        {
            object[] states = (object[])state;

            Mobile thief = (Mobile)states[ 0 ];
            Mobile victim = (Mobile)states[ 1 ];

            if( !CheckResisted( thief, victim ) || !IsBehind( thief, victim, false ) )
            {
                thief.PrivateOverheadMessage( MessageType.Regular, 37, true, thief.Language == "ITA" ? "*mancato*" : "*failed*", thief.NetState );
                victim.SendMessage( victim.Language == "ITA" ? "Qualcuno ha cercato di accoltellarti." : "Somebody tryed to stab you." );

                if( thief.IsHarmfulCriminal( victim ) )
                    thief.CriminalAction( false );

                thief.RevealingAction();

                Titles.AwardKarma( thief, -25, false );

                string message = String.Format( "You notice {0} trying to agress {1}.", thief.Name, victim.Name );
                string messageita = String.Format( "Noti che {0} ha tentato di aggredire {1}.", thief.Name, victim.Name );

                foreach( NetState ns in thief.GetClientsInRange( 8 ) )
                {
                    if( ns != thief.NetState )
                        ns.Mobile.SendMessage( (ns.Mobile).Language == "ITA" ? messageita : message );
                }

                double delay = victim.Player ? DelayOnFail : DelayOnFail * NonPlayerScalar;
                thief.SendMessage( thief.Language == "ITA" ? "Potrai pugnalare di nuovo tra {0} secondi." : "Thou could stab again in {0} seconds.", delay.ToString( "F0" ) );

                Timer.DelayCall( TimeSpan.FromSeconds( delay ), new TimerStateCallback( ReleaseBackstabbingKnifeLock ), thief );
            }
            else
            {
                thief.PrivateOverheadMessage( MessageType.Regular, 37, true, thief.Language == "ITA" ? "*successo*" : "* successful *", thief.NetState );
                victim.SendMessage( victim.Language == "ITA" ? "Sei stato pugnalato alla schiena!" : "You have been stab in the back!" );

                int noto = Notoriety.Compute( thief, victim );

                if( thief.HarmfulCheck( victim ) && ( noto == Notoriety.Innocent ) )
                    thief.CriminalAction( false );

                Titles.AwardKarma( thief, -50, false );

                int anatomy = (int)( thief.Skills[ SkillName.Anatomy ].Value );
                int damage = ( anatomy / 3 ) + Utility.Random( 17 ) + 1;
                int stealthBonus = (int)( ( damage / 10.0 ) * thief.AllowedStealthSteps );

                int finalDamage = (int)( ( ( damage + stealthBonus ) * Math.Min( victim.HitsMax, 100 ) ) / 100.0 );

                if( thief.PlayerDebug )
                {
                    thief.SendMessage( "Debug backstab: Steps: {0}", thief.AllowedStealthSteps );
                    thief.SendMessage( "Debug backstab: Ana: {0} - Dam: {1} - Ste: {2} - Tot: {3}", anatomy, damage, stealthBonus, finalDamage );
                }

                //if( victim.Player )
                //    finalDamage = Math.Min( finalDamage, 30 );

                thief.RevealingAction();
                victim.Damage( finalDamage, thief );

                double delay = victim.Player ? DelayOnSuccess : DelayOnSuccess * NonPlayerScalar;
                thief.SendMessage( thief.Language == "ITA" ? "Potrai pugnalare di nuovo tra {0} secondi." : "Thou could stab again in {0} seconds.", delay.ToString( "F0" ) );

                Timer.DelayCall( TimeSpan.FromSeconds( delay ), new TimerStateCallback( ReleaseBackstabbingKnifeLock ), thief );
            }
        }

        private static void ReleaseBackstabbingKnifeLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( BackstabbingKnife ) );
        }

        private class InternalTarget : Target
        {
            private readonly BackstabbingKnife m_Owner;

            public InternalTarget( BackstabbingKnife knife )
                : base( 1, false, TargetFlags.None )
            {
                m_Owner = knife;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                m_Owner.OnTarget( from, o );
            }

            protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
            {
                switch( cancelType )
                {
                    case TargetCancelType.Canceled:
                        from.SendMessage( from.Language == "ITA" ? "Ben fatto. Il tuo coltello sarà subito pronto a colpire." : "Well done. Your knife will be ready again in a moment." );
                        break;
                    case TargetCancelType.Timeout:
                        from.SendMessage( from.Language == "ITA" ? "Tempo scaduto." : "Time has gone." );
                        break;
                    default:
                        break;
                }

                Timer.DelayCall( TimeSpan.FromSeconds( DelayOnFail ), new TimerStateCallback( ReleaseBackstabbingKnifeLock ), from );
                base.OnTargetCancel( from, cancelType );
            }
        }

        #region serialization
        public BackstabbingKnife( Serial serial )
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

        internal class CheckBehindCommand
        {
            public static void Initialize()
            {
                CommandSystem.Register( "CheckBehind", AccessLevel.Counselor, new CommandEventHandler( CheckBehind_OnCommand ) );
            }

            [Usage( "CheckBehind" )]
            [Description( "Verify if command user is behind target." )]
            private static void CheckBehind_OnCommand( CommandEventArgs e )
            {
                e.Mobile.SendMessage( (e.Mobile).Language == "ITA" ? "Seleziona l'essere vivente da controllare." : "Target the mobile you want to check." );
                e.Mobile.Target = new CheckBehindTarget();
            }

            private class CheckBehindTarget : Target
            {
                public CheckBehindTarget()
                    : base( 15, false, TargetFlags.None )
                {
                }

                protected override void OnTarget( Mobile from, object targ )
                {
                    if( !( targ is Mobile ) )
                    {
                        from.SendMessage( from.Language == "ITA" ? "Seleziona un bersaglio valido per il check." : "Target a valid mobile to check if you are behind." );
                        return;
                    }

                    bool isBehind = Offset( ( (Mobile)targ ).Direction & Direction.Mask, from.Location, ( (Mobile)targ ).Location );
                    bool isFacing = ( from.Direction & Direction.Mask ) == Utility.GetDirection( from.Location, ( (Mobile)targ ).Location );
					bool isBehind2 = IsBehind ( from, (Mobile)targ, false);
					if ( isBehind2 != isBehind)
					{
						from.SendMessage( "Critical Allucination!!!! behind1={0} behind2={1}. IT IS IMPOSSIBILE! SCRIPT ERROR!", isBehind,isBehind2);
					}
					else
					{
						if (from.Language == "ITA")
						{
							from.SendMessage( "{0}ei dietro il bersaglio.", !isBehind ? "Non s" : "S" );
							from.SendMessage( "{0}tai guardando il tuo bersaglio.", !isFacing ? "Non s" : "S" );
						}
						else
						{
							from.SendMessage( "You are {0}behind your target.", !isBehind ? "not " : "" );
							from.SendMessage( "You are {0}facing your target.", !isFacing ? "not " : "" );
						}
					}
				}
			}
		}
	}
}