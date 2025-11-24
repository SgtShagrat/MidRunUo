using System;
using System.IO;
using System.Text;

using Midgard.Items;

using Server;
using Server.Items;

namespace Midgard.Misc
{
    public class WeaponDocs
    {
        public static void Initialize()
        {
            if( Core.Debug )
                GenerateDoc();
        }

        private static readonly Type[] m_WeaponTypes = new Type[]
			{
				typeof( Axe ),					typeof( BattleAxe ),			typeof( DoubleAxe ),
				typeof( ExecutionersAxe ),		typeof( Hatchet ),				typeof( LargeBattleAxe ),
				typeof( TwoHandedAxe ),			typeof( WarAxe ),				typeof( Club ),
				typeof( Mace ),					typeof( Maul ),					typeof( WarHammer ),
				typeof( WarMace ),				typeof( Bardiche ),				typeof( Halberd ),
				typeof( Spear ),				typeof( ShortSpear ),			/* typeof( Pitchfork ), */
				typeof( WarFork ),				typeof( BlackStaff ),			typeof( GnarledStaff ),
				typeof( QuarterStaff ),			typeof( Broadsword ),			typeof( Cutlass ),
				typeof( Katana ),				typeof( Kryss ),				typeof( Longsword ),
				typeof( Scimitar ),				typeof( VikingSword ),			typeof( Pickaxe ),
				typeof( HammerPick ),			typeof( ButcherKnife ),			typeof( Cleaver ),
				typeof( Dagger ),				typeof( SkinningKnife ),		typeof( ShepherdsCrook ),
                typeof( MageStaff )
			};

        /*
        weapon 0x13b3
        {
	        name			club
	        speed			40
	        damage			4d5+4
	        skillid			mace
	        maxhp			30
	        hitsound		0x13c
	        misssound		0x234
	        staminaloss		1d4
	        armorloss		1
	        blockcircle		1
	        anim			0x000b
	        destroyscript		::itemdestruction
	        unequipscript		::skilladvancerunequip
	        equipscript		::skilladvancerequip
	        hitscript		mainhitscript
	        controlscript		::makehitscript
	        vendorsellsfor		119
	        vendorbuysfor		60
	        weight			14
        }
        */

        private const string LogFile = "Logs/weapon-docs.log";

        private static void GenerateDoc()
        {
            using( StreamWriter op = new StreamWriter( LogFile, true ) )
            {
                foreach( Type weaponType in m_WeaponTypes )
                    op.WriteLine( DocumentWeapon( weaponType ) );
            }
        }

        private static string DocumentWeapon( Type t )
        {
            BaseWeapon w = Loot.Construct( t ) as BaseWeapon;
            if( w == null )
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine( string.Format( "Name: {0}", GetFriendlyClassName( t.Name ) ) );
            sb.AppendLine( string.Format( "ItemID: 0x{0:X}", w.ItemID ) );

            sb.AppendLine( string.Format( "Speed: {0}", w.OldSpeed ) );
            sb.AppendLine( string.Format( "Damage: {0}", w.Dice ) );
            sb.AppendLine( string.Format( "Skill: {0}", w.DefSkill ) );
            sb.AppendLine( string.Format( "Max Hits: {0}", w.InitMaxHits ) );
            sb.AppendLine( string.Format( "Hit Sound: {0}", w.DefHitSound ) );
            sb.AppendLine( string.Format( "Miss Sound: {0}", w.DefMissSound ) );
            sb.AppendLine( string.Format( "Strength Req: {0}", w.OldStrengthReq ) );
            sb.AppendLine( string.Format( "Hit Rate Bonus: {0}", w.HitRateBonus ) );
            sb.AppendLine( string.Format( "Evasion Bonus: {0}", w.EvasionBonus ) );

            return sb.ToString();
        }

        private static string GetFriendlyClassName( string typeName )
        {
            string temp = typeName;

            for( int index = 1; index < temp.Length; index++ )
            {
                if( char.IsUpper( temp, index ) )
                    temp = temp.Insert( index++, " " );
            }

            return temp;
        }
    }
}