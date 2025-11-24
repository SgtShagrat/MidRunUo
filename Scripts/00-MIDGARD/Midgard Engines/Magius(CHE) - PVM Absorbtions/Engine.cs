using System;
using System.Reflection;
using System.Text;

using Midgard.Engines.Packager;
using System.IO;
using Server.Mobiles;
using Server.Items;
using Server;
using System.Collections.Generic;

using Server.Network;

namespace Midgard.Engines.PVMAbsorbtions
{
    public enum DamageTypes
    {
        Physical,
        Magical,
    }

    public class Core
    {
        public static object[] Package_Info =
        {
            "Script Title", "PVM Absorbtions Engine",
            "Enabled by Default", true,
            "Script Version", new Version( 1, 0, 0, 0 ),
            "Author name", "Magius(CHE)",
            "Creation Date", new DateTime( 2011, 4, 28 ),
            "Author mail-contact", "cheghe@tiscali.it",
            "Author home site", "http://www.magius.it",
            //"Author notes",           null,
            "Script Copyrights", "(C) Midgard Shard - Magius(CHE",
            "Provided packages", new string[] { "Midgard.Engines.PVMAbsorbtions" },
            /*"Required packages",       new string[]{"Midgard.Engines.SkillSystem"},*/
            //"Conflicts with packages",new string[0],
            "Research tags", new string[] { "PVMAbsorbtions", "PVM", "Combat"}
        };

        #region [ Initialization ]
        private const double MaximumArmourAbsorbtion_Phisical = 0.5;
        private const double MaximumArmourAbsorbtion_Magical = 0.7;

        private const double MaximumShieldAbsorbtion_Phisical = 0.6;
        private const double MaximumShieldAbsorbtion_Magical = 0.6;

#if DEBUG
        public static bool Debug = true;
#else
		public static bool Debug = false;
#endif
        #endregion

        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Pkg.Enabled; }
            set { Pkg.Enabled = value; }
        }

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Core ) ];
        }

        public static void Package_Initialize()
        {
            if( Debug )
                ExtractItems();
        }

        private static void ExtractItems()
        {
            string output = "Logs/shields.txt";
            using( var writer = new StreamWriter( output, false, Encoding.ASCII ) )
            {
                var items = new List<Type>();
                foreach( Assembly asse in ScriptCompiler.Assemblies )
                {
                    foreach( Type type in asse.GetTypes() )
                    {
                        if( type.IsSubclassOf( typeof( BaseShield ) ) && !items.Contains( type ) )
                            items.Add( type );
                    }
                }
                items.Sort( ( a, b ) => a.Name.CompareTo( b.Name ) );
                writer.WriteLine( "Type\tNome\tAR\tRDmg\tRDmgM" );
                foreach( Type type in items )
                {
                    var tmp = (BaseShield)Loot.Construct( type );
                    if( tmp != null )
                        writer.WriteLine( "{0}\t{1}\t{2}\t{3}\t{4}", type.FullName, tmp.Name, tmp.ArmorBase, AlterDamage( tmp, 100, DamageTypes.Physical ), AlterDamage( tmp, 100, DamageTypes.Magical ) );
                }
            }

            output = "Logs/platearmours.txt";

            var plates = new List<Type>();
            plates.AddRange( new Type[]
                                 {
                                     typeof(PlateChest), typeof(PlateLegs), typeof(CloseHelm), typeof(PlateGorget), typeof(PlateArms), typeof(PlateGloves)
                                 } );
            using( var writer = new StreamWriter( output, false, Encoding.ASCII ) )
            {
                var items = new List<Type>();
                foreach( Assembly asse in ScriptCompiler.Assemblies )
                {
                    foreach( Type type in asse.GetTypes() )
                    {
                        if( !items.Contains( type ) )
                        {
                            if( type.IsSubclassOf( typeof( BaseArmor ) ) )
                            /*foreach (var platetype in plates)
                            if (type.IsSubclassOf(platetype))*/
                            {
                                items.Add( type );
                                //break;
                            }
                        }
                    }
                }
                items.Sort( ( a, b ) => a.Name.CompareTo( b.Name ) );
                writer.WriteLine( "Type\tNome\tAR\tRDmg\tRDmgM" );
                foreach( Type type in items )
                {
                    var tmp = (BaseArmor)Loot.Construct( type );
                    if( tmp != null )
                        writer.WriteLine( "{0}\t{1}\t{2}\t{3}\t{4}", type.FullName, tmp.Name, tmp.ArmorBase, AlterDamage( tmp, 100, DamageTypes.Physical ), AlterDamage( tmp, 100, DamageTypes.Magical ) );
                }
            }
        }

        public static BaseArmor GetRandomHittedArmour( Mobile defender )
        {
            double chance = Utility.RandomDouble();

            Item armorItem;

            if( chance < 0.07 )
                armorItem = defender.NeckArmor;
            else if( chance < 0.14 )
                armorItem = defender.HandArmor;
            else if( chance < 0.28 )
                armorItem = defender.ArmsArmor;
            else if( chance < 0.42 )
                armorItem = defender.HeadArmor;
            else if( chance < 0.66 )
                armorItem = defender.LegsArmor;
            else
                armorItem = defender.ChestArmor;

            return armorItem as BaseArmor;
        }

        private class PreventInfoClass
        {
            public Mobile Attacker;
            public Mobile Defender;
        }

        private static readonly List<PreventInfoClass> prevented = new List<PreventInfoClass>();

        public static void PreventAbsorbtionFrom( Mobile attacker, Mobile defender, bool prevent )
        {
            lock( prevented )
            {
                if( prevent )
                {
                    foreach( PreventInfoClass elem in prevented )
                    {
                        if( elem.Attacker == attacker && elem.Defender == defender )
                            return;
                    }
                    prevented.Add( new PreventInfoClass
                                       {
                                           Attacker = attacker,
                                           Defender = defender
                                       } );
                }
                else
                {
                    PreventInfoClass torem = null;
                    foreach( PreventInfoClass elem in prevented )
                    {
                        if( elem.Attacker == attacker && elem.Defender == defender )
                        {
                            torem = elem;
                            break;
                        }
                    }
                    if( torem != null )
                        prevented.Remove( torem );
                }
            }
        }

        public static bool CanAlterDamage( Mobile attacker, Mobile defender, DamageTypes dtype )
        {
            if( attacker is PlayerMobile )
                return false; //no modification in PVP

            if( defender is BaseCreature )
                return false; //no modification if defender is MOB

            var mid = defender as Midgard2PlayerMobile;
            if( mid == null )
                return false;

            /*if(!mid.PVMTankBonus)
				return false;*/

            lock( prevented )
                foreach( PreventInfoClass elem in prevented )
                {
                    if( elem.Attacker == attacker && elem.Defender == defender )
                        return false;
                }

            return true;
        }

        public static int OnDealDamage( Mobile attacker, Mobile defender, int damage, DamageTypes dtype )
        {
            try
            {
                if( !CanAlterDamage( attacker, defender, dtype ) )
                    return damage;

                if( dtype == DamageTypes.Physical )
                    return damage; //already calculated (OnHitArmour, OnHitShield)

                BaseArmor armor = GetRandomHittedArmour( defender );
                var shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;
                // IWeapon weapon = attacker.Weapon;

                int newdamage = damage;

                if( armor != null )
                    newdamage = AlterDamage( armor, damage, DamageTypes.Magical );

                if( shield != null )
                {
                    if( Parryed( defender as PlayerMobile, shield, true, "Your shield blocks a magical attack!" ) )
                        newdamage = AlterDamage( shield, newdamage, DamageTypes.Magical );
                }

                if( armor == null && shield == null )
                    return newdamage;

                newdamage = Math.Max( 1, ScaleDamgeVsMobSummoning( attacker, newdamage ) );

                TracePlayer( defender as PlayerMobile, null, damage, newdamage, dtype );
                return newdamage;
            }
            catch( Exception ex )
            {
                Pkg.LogError( ex );
                PlayerDebugError( defender as PlayerMobile, "PVMTankEngine OnDealDamage Error: {0}", ex.GetType() + "::" + ex.Message );
                return damage;
            }
        }

        public static bool Parryed( PlayerMobile defender, BaseShield shield, bool parryeffects, string parryedstring )
        {
            double ar = shield.ArmorRating;
            double chance = ( defender.Skills[ SkillName.Parry ].Value - ( ar * 2.0 ) ) / 100.0;
            if( chance < 0.01 )
                chance = 0.01;

            bool ret = ( Utility.RandomDouble() <= chance );

            if( ret && parryeffects )
            {
                if( defender.Player && !string.IsNullOrEmpty( parryedstring ) )
                    defender.SendMessage( parryedstring );

                defender.FixedEffect( 0x37B9, 10, 16 );

                defender.PlaySound( 0x139 ); // mod by Dies Irae
            }
            return ret;
        }

        public static int OnHitArmour( BaseArmor armor, BaseWeapon weapon, int damage )
        {
            Mobile defender = null;
            Mobile attacker = null;
            try
            {
                defender = armor.Parent as Mobile;
                if( defender != null )
                {
                    attacker = defender.LastAttacker;

                    if( !CanAlterDamage( attacker, defender, DamageTypes.Physical ) )
                        return damage;

                    int newdamage = ScaleDamgeVsMobSummoning( attacker, damage );

                    newdamage = AlterDamage( armor, newdamage, DamageTypes.Physical );
                    TracePlayer( defender as PlayerMobile, armor, damage, newdamage, DamageTypes.Physical );
                    return newdamage;
                }
                else
                    return damage;
            }
            catch( Exception ex )
            {
                Pkg.LogError( ex );
                Pkg.LogError("Armor: {0}", armor);
                Pkg.LogError("Weapon: {0}", weapon);
                Pkg.LogError("Defender: {0}", defender);
                Pkg.LogError("Attacker: {0}", attacker);
                PlayerDebugError( defender as PlayerMobile, "PVMTankEngine OnHitArmour Error: {0}", ex.GetType() + "::" + ex.Message );
                return damage;
            }
        }

        public static int OnHitShield( BaseShield shield, BaseWeapon weapon, int damage )
        {
            Mobile defender = null;
            Mobile attacker = null;
            try
            {
                defender = shield.Parent as Mobile;
                if( defender != null )
                {
                    attacker = defender.LastAttacker;

                    if( !CanAlterDamage( attacker, defender, DamageTypes.Physical ) )
                        return damage;

                    // is this correct? By Dies :)
                    int newdamage = ScaleDamgeVsMobSummoning( attacker, damage );

                    newdamage = AlterDamage( shield, damage, DamageTypes.Physical );
                    TracePlayer( defender as PlayerMobile, shield, damage, newdamage, DamageTypes.Physical );
                    return newdamage;
                }
                return damage;
            }
            catch( Exception ex )
            {
                Pkg.LogError( ex );
                Pkg.LogError("Shield: {0}", shield);
                Pkg.LogError("Weapon: {0}", weapon);
                Pkg.LogError("Defender: {0}", defender);
                Pkg.LogError("Attacker: {0}", attacker);
                PlayerDebugError( defender as PlayerMobile, "PVMTankEngine OnHitShield Error: {0}", ex.GetType() + "::" + ex.Message );
                return damage;
            }
        }

        public static int OnHitSkin( Mobile defender, BaseWeapon weapon, int damage, DamageTypes dtype )
        {
            try
            {
                var attacker = weapon.Parent as Mobile;

                if( !CanAlterDamage( attacker, defender, dtype ) )
                    return damage;

                /*for now, no armour or shields means no absobrtion */

                return damage;
            }
            catch( Exception ex )
            {
                Pkg.LogError( ex );
                PlayerDebugError( defender as PlayerMobile, "PVMTankEngine OnHitSkin Error: {0}", ex.GetType() + "::" + ex.Message );
                return damage;
            }
        }

        private static void PlayerDebugError( PlayerMobile defender, string format, params object[] args )
        {
            try
            {
                defender.SendMessage( 32, string.Format( format, args ) );
                defender.PrivateOverheadMessage( MessageType.System, 32, false, "* ERROR *", defender.NetState );
            }
            catch( Exception ex )
            {
                Pkg.LogError( ex );
            }
        }

        private static void TracePlayer( PlayerMobile defender, BaseArmor armor, int damage, int modifieddamage, DamageTypes damagetype )
        {
            if( damage == modifieddamage )
                return;

            var midPlayerMobile = defender as Midgard2PlayerMobile;
            if( midPlayerMobile == null || !midPlayerMobile.NotificationPVMTankEnabled )
                return;

            int abs = damage - modifieddamage;

            string str = "Tank abs";
            string iconstr = "Tank mode: ";
            str += " " + abs + "/" + damage;
            str += " dmg";
            iconstr += string.Format( "{0:p0}", abs / (double)damage );
            switch( damagetype )
            {
                case DamageTypes.Magical:
                    str += " (m)";
                    break;
            }

            if( armor is BaseShield )
                str += " (shield)";
            /*else if (armor!=null)
				str+=" (armor)";*/

            /*
            if( abs == -1 )
            {
                int a = 1;
            }
            */

            new TankNotifyIcon( defender, iconstr );
            if(Core.Debug)
				defender.PrivateOverheadMessage( MessageType.Regular, 65, false, str, defender.NetState );
        }

        /// <summary>
        /// Assume 46 as maximum Armour AR based on Plate + Adamantium bonus => 30 + 16
        /// </summary>
        public const double MaximumAROnArmor = 46;

        public const double PhysicalDamageScaleOnMaximum = 0.4;
        public const double MagicalDamageScaleOnMaximum = 0.4;

        public const double MagicalDamageScaleVsMobSummon = 0.5;

        public const double UnknownMaterialScale = 0.1;
        public const double ShiledScale = 2.8;

        public static int AlterDamage( BaseArmor armor, int damage, DamageTypes type )
        {
            // var defender = armor.Parent as Mobile;
            if( armor == null ) /*do not absorb if no armour or shield*/
                return damage;

            double absVal = armor.ScaleArmorByDurability( armor.ArmorBase );
            double absPer = absVal / MaximumAROnArmor;

            absPer *= type == DamageTypes.Physical ? PhysicalDamageScaleOnMaximum : MagicalDamageScaleOnMaximum;

            var shield = armor as BaseShield;

            switch( armor.MaterialType )
            {
                case ArmorMaterialType.Plate:
                case ArmorMaterialType.Dragon:
                    absPer *= 1.0;
                    break;
                case ArmorMaterialType.Chainmail:
                case ArmorMaterialType.Bone:
                    absPer *= 0.7;
                    break;
                case ArmorMaterialType.Ringmail:
                case ArmorMaterialType.Glass:
                    absPer *= 0.4;
                    break;
                case ArmorMaterialType.Studded:
                    absPer *= 0.3;
                    break;
                case ArmorMaterialType.Leather:
                    absPer *= 0.2;
                    break;
                default:
                    absPer *= UnknownMaterialScale;
                    break;
            }
            if( shield != null )
                absPer *= ShiledScale;

            return Math.Max( 1, (int)Math.Floor( damage * ( 1.0 - absPer ) ) );
        }

        public static int ScaleDamgeVsMobSummoning( Mobile attacker, int damage )
        {
            var summon = attacker as BaseCreature;
            if( summon == null )
                return damage;

            if( !summon.Summoned )
                return damage;

            var monster = summon.GetMaster() as BaseCreature;
            if( monster == null )
                return damage;

            return (int)Math.Floor( damage * MagicalDamageScaleVsMobSummon );
        }
    }
}