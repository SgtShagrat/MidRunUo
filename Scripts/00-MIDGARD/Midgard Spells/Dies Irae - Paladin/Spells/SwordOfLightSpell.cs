/***************************************************************************
 *                               SwordOfLightSpell.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Network;
using Server.Spells;
using Core = Midgard.Engines.Races.Core;

namespace Midgard.Engines.SpellSystem
{
    public class SwordOfLightSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Sword Of Light", "Gladius Fulgens",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( SwordOfLightSpell ),
            m_Info.Name,
            "This Miracle summons the Holy Paladin Sword, powerful Virtue artifact.",
            "Questo Miracolo crea la Spada del Paladino, potente artefatto delle Virtu'.",
            "Crea la spada di luce per PowerValueScaled secondi. La spada e' blessata e quando colpisce ha (faith / 200)% di usare un potere magico a scelta tra" +
            "Cure, FillHunger e Bless sul paladino.",
            0x5102
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Modificato il funzionamento. Ora l'arma viene blessata direttamente. La durata e' PowerValueScaled / 60.0 minuti. Disabilitato il potere di luce.<br>";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Second; }
        }

        public SwordOfLightSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            BaseWeapon weapon = FindValidWeapon();
            if( weapon == null )
            {
                Caster.SendLangMessage( 10001101 ); // "You must wield a valid weapon su summon this power." );
                FinishSequence();
                return;
            }

            if( Caster.CanBeginAction( typeof( SwordOfLightSpell ) ) )
            {
                if( CheckSequence() )
                {
                    if( ToggleWeaponStatus( weapon, PowerValueScaled ) )
                    {
                        Caster.CheckLightLevels( false );
                        Caster.BeginAction( typeof( SwordOfLightSpell ) );

                        Caster.PlaySound( 0x5d9 ); // 1497
                        Caster.FixedParticles( 0x3779, 1, 30, 9964, 3, 3, EffectLayer.Waist );

                        IEntity from = new Entity( Serial.Zero, new Point3D( Caster.X, Caster.Y, Caster.Z ), Caster.Map );
                        IEntity to = new Entity( Serial.Zero, new Point3D( Caster.X, Caster.Y, Caster.Z + 50 ), Caster.Map );
                        Effects.SendMovingParticles( from, to, 0xF5F, 1, 0, false, false, 33, 3, 9501, 1, 0, EffectLayer.Head, 0x100 );

                        string message = TextHelper.Text( 10001102, Caster.TrueLanguage ); // "* Your sword will be your arm *"
                        Caster.PublicOverheadMessage( MessageType.Regular, 37, true, message );

                        Timer.DelayCall( TimeSpan.FromSeconds( PowerValueScaled ) + GetDelayOfReuseInSeconds(), new TimerStateCallback( ReleaseSwordOfLightLock ), Caster );
                    }
                }
            }
            else
                Caster.SendLangMessage( 10001103 ); // "You cannot summon another sword yet." );

            FinishSequence();
        }

        private BaseWeapon FindValidWeapon()
        {
            Item weapon = Caster.FindItemOnLayer( Layer.OneHanded );

            // fists are not movable
            if( weapon == null || !weapon.Movable )
                weapon = Caster.FindItemOnLayer( Layer.TwoHanded );

            if( weapon == null || !( weapon is BaseWeapon ) || weapon is BaseRanged )
                return null;

            return weapon as BaseWeapon;
        }

        private bool ToggleWeaponStatus( BaseWeapon weapon, double durationInSeconds )
        {
            int hue = RPGSpellsSystem.IsAtCap( Caster, SkillName.Chivalry ) ? HolyHueAtCap : HolyHue;
            if( Core.IsElfRace( Caster.Race ) )
                hue = HolyHueForElves; // green hue

            XmlHolyItemAttach attach = new XmlHolyItemAttach( durationInSeconds / 60.0, hue, Caster, true );
            return XmlAttach.AttachTo( weapon, attach );
        }

        private static void ReleaseSwordOfLightLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( SwordOfLightSpell ) );
            ( (Mobile)state ).SendLangMessage( 10001104 ); // "You can summon a new holy sword now." );
        }

        public static bool OnHitOld( Mobile paladin, Mobile defender )
        {
            // attacker è il mobile che tiene l'arma in mano
            // defender è colui che viene colpito (e che quindi ha l'armor)

            defender.BoltEffect( 0x480 );
            defender.PlaySound( 0x1E0 );
            
            if( Utility.RandomDouble() <= paladin.Skills[ SkillName.Chivalry ].Value / 200.0 )
            {
                bool success = false;
                switch( Utility.Random( 3 ) )
                {
                    case 0:
                        if( DoHolyCure( paladin ) )
                            success = true;
                        break;
                    case 1:
                        if( DoFillHunger( paladin ) )
                            success = true;
                        break;
                    case 2:
                        if( DoHolyBless( paladin ) )
                            success = true;
                        break;
                }

                if( success )
                {
                    paladin.PublicOverheadMessage( MessageType.Regular, 1154, true, "* the power of light will prevail *" );
                    return true;
                }
            }

            return false;
        }

        #region [effects]
        private static bool DoFillHunger( Mobile paladin )
        {
            if( paladin == null )
                return false;

            if( paladin.Hunger < 20 )
            {
                paladin.Hunger++;
                paladin.SendLangMessage( 10001105 ); // "Your holy weapon filled your hunger!" );
                paladin.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
                paladin.PlaySound( 0x1E0 );
                return true;
            }
            else
                return DoHolyCure( paladin );
        }

        private static bool DoHolyCure( Mobile paladin )
        {
            if( paladin == null )
                return false;

            if( paladin.CurePoison( paladin ) )
            {
                paladin.SendLangMessage( 10001106 ); // "Your holy weapon cured you!" );
                paladin.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
                paladin.PlaySound( 0x1E0 );

                return true;
            }
            else
                return DoHolyBless( paladin );
        }

        /*
        private static bool DoHolyLight( Mobile attacker, Mobile defender )
        {
            if( defender == null || attacker == null )
                return false;

            if( attacker.BeginAction( typeof( LightCycle ) ) )
            {
                new LightCycle.NightSightTimer( attacker ).Start();
                attacker.LightLevel = LightCycle.DayLevel;

                attacker.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
                attacker.PlaySound( 0x1EA );

                attacker.SendMessage( "Your holy weapon enlightned you!" );
                return true;
            }

            return false;
        }
        */

        private static bool DoHolyBless( Mobile paladin )
        {
            if( paladin == null )
                return false;

            SpellHelper.DisableSkillCheck = true;

            TimeSpan duration = GetDuration( paladin );

            bool success = ( SpellHelper.AddStatBonus( paladin, paladin, StatType.Str, GetOffset( paladin, StatType.Str, false ), duration ) ||
                             SpellHelper.AddStatBonus( paladin, paladin, StatType.Dex, GetOffset( paladin, StatType.Str, false ), duration ) ||
                             SpellHelper.AddStatBonus( paladin, paladin, StatType.Int, GetOffset( paladin, StatType.Str, false ), duration ) );

            SpellHelper.DisableSkillCheck = false;

            if( success )
            {
                paladin.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
                paladin.PlaySound( 0x1EA );

                paladin.SendLangMessage( 10001107 ); // "Your holy weapon blessed you!" );
                return true;
            }
            else
                paladin.SendLangMessage( 10001108 ); // "You are already blessed by your weapon!" );

            return false;
        }

        public static int GetOffset( Mobile paladin, StatType type, bool curse )
        {
            return 1 + (int)( paladin.Skills[ SkillName.Chivalry ].Value * 0.1 );
        }

        public static TimeSpan GetDuration( Mobile paladin )
        {
            return TimeSpan.FromSeconds( paladin.Skills[ SkillName.Chivalry ].Value * 1.2 );
        }
        #endregion
    }
}