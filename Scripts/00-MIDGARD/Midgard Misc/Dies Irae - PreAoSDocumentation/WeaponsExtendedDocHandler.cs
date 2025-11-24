using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using Midgard.Items;
using Server;
using Server.Items;

namespace Midgard.Misc
{
    public class WeaponsExtendedDocHandler : DocumentationHandler
    {
        public static void Initialize()
        {
            PreAoSDocHelper.Register( new WeaponsExtendedDocHandler() );
        }

        public WeaponsExtendedDocHandler()
        {
            Enabled = true;
        }

        private static readonly Type[] m_WeaponTypes = new Type[]
                                                           {
                                                               typeof( Axe ),					typeof( BattleAxe ),			typeof( DoubleAxe ),
                                                               typeof( ExecutionersAxe ),		typeof( Hatchet ),				typeof( LargeBattleAxe ),
                                                               typeof( TwoHandedAxe ),			typeof( WarAxe ),				typeof( Club ),
                                                               typeof( Mace ),					typeof( Maul ),					typeof( WarHammer ),
                                                               typeof( WarMace ),				typeof( Bardiche ),				typeof( Halberd ),
                                                               typeof( Spear ),				    typeof( ShortSpear ),			
                                                               typeof( WarFork ),				typeof( BlackStaff ),			typeof( GnarledStaff ),
                                                               typeof( QuarterStaff ),			typeof( Broadsword ),			typeof( Cutlass ),
                                                               typeof( Katana ),				typeof( Kryss ),				typeof( Longsword ),
                                                               typeof( Scimitar ),				typeof( VikingSword ),			typeof( Pickaxe ),
                                                               typeof( HammerPick ),			typeof( ButcherKnife ),			typeof( Cleaver ),
                                                               typeof( Dagger ),				typeof( SkinningKnife ),		typeof( ShepherdsCrook ),
                                                               typeof( MageStaff ),             typeof( LightStaff ),           typeof( DarkenStaff ),
                                                           };
        private class UsedWeaponInfo
        {
            public int Count;
            public string SortingKey;
        }

        public override void GenerateDocumentation()
        {
            PreAoSDocHelper.EnsureDirectory( WeaponsExtendedDocDir );

            List<string> headers = new List<string>();

            headers.Add( "Name" );
            headers.Add( "STR Req." );
            headers.Add( "Damage" );
            headers.Add( "Speed" );
            headers.Add( "Max Hits" );

            headers.Add( "ItemID" );
            headers.Add( "Skill" );
            headers.Add( "Hit Sound" );
            headers.Add( "Miss Sound" );
            headers.Add( "Hit Rate Bonus" );
            headers.Add( "Evasion Bonus" );
            headers.Add( "Stamina Loss" );
            headers.Add( "B.Lumber" );
            headers.Add( "B.Swords" );
            headers.Add( "B.Mining" );
            headers.Add( "B.BCraft" );
            headers.Add( "W.Count" );

            using( StreamWriter op = new StreamWriter( Path.Combine( WeaponsExtendedDocDir, "weapons.html" ) ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Documentation - {0}\n", "Weapons Extended" );
                    html.RenderEndTag(); // Title

                    html.RenderEndTag(); // Head

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    List<List<string>> contentMatrix = new List<List<string>>();

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    html.AddAttribute( HtmlTextWriterAttribute.Border, "1" );
                    html.AddAttribute( HtmlTextWriterAttribute.Cellpadding, "1" );
                    html.AddAttribute( HtmlTextWriterAttribute.Cellspacing, "0" );
                    html.RenderBeginTag( HtmlTextWriterTag.Table );

                    contentMatrix.Clear();

                    Dictionary<Type, UsedWeaponInfo> list = new Dictionary<Type, UsedWeaponInfo>();

                    // Aggiungo prima i types preferiti con sorting A in modo che alfabeticamente siano i primi ad essere mostrati.
                    foreach( Type type in m_WeaponTypes )
                        list.Add( type, new UsedWeaponInfo { Count = 0, SortingKey = "A" } );

                    // Aggiungo poi tutti i type usati nel mondo attuale con sorting B così saranno il secondo gruppo
                    foreach( Item item in World.Items.Values )
                    {
                        if( item is BaseWeapon )
                        {
                            if( !list.ContainsKey( item.GetType() ) )
                                list.Add( item.GetType(), new UsedWeaponInfo { Count = 1, SortingKey = "B" } );
                            else
                                list[ item.GetType() ].Count++;
                        }
                    }

                    // Aggiungo poi tutti i type esistenti con sorting C così saranno il terzo gruppo
                    foreach( var itemtype in World.ItemTypes )
                    {
                        if( itemtype.IsSubclassOf( typeof( BaseWeapon ) ) )
                        {
                            if( !list.ContainsKey( itemtype ) )
                                list.Add( itemtype, new UsedWeaponInfo { Count = 0, SortingKey = "C" } );
                        }
                    }

                    List<KeyValuePair<Type, UsedWeaponInfo>> newList = new List<KeyValuePair<Type, UsedWeaponInfo>>( list );

                    newList.Sort( ( a, b ) => ( a.Value.SortingKey + a.Key.Name ).CompareTo( b.Value.SortingKey + b.Key.Name ) );

                    var alertstyle = "{#STYLE:background-color:red;color:white;}";

                    foreach( var elem in newList )
                    {
                        BaseWeapon w = Loot.Construct( elem.Key ) as BaseWeapon;
                        if( w == null )
                            continue;

                        List<string> contentLine = new List<string>();

                        switch( elem.Value.SortingKey )
                        {
                            case "B":
                                contentLine.Add( "{#STYLE:background-color:#EEE;}" );
                                break;
                            case "C":
                                contentLine.Add( "{#STYLE:background-color:#E9E9E9;}" );
                                break;
                            default:
                                contentLine.Add( "{#STYLE:background-color:#FFF;}" );
                                break;
                        }

                        if( !( w.ElegibleForLumberBonus || w.ElegibleForSwordsBonus || w.ElegibleForMiningBonus || w.ElegibleForBowcraftBonus ) )
                            contentLine.Add( alertstyle );

                        contentLine.Add( GetFriendlyClassName( elem.Key.Name ) );
                        contentLine.Add( w.OldStrengthReq.ToString() );
                        contentLine.Add( w.Dice );
                        contentLine.Add( w.OldSpeed.ToString() );
                        contentLine.Add( w.InitMaxHits.ToString() );

                        contentLine.Add( w.ItemID.ToString( "{0:X}" ) );
                        contentLine.Add( w.DefSkill.ToString() );
                        contentLine.Add( w.DefHitSound.ToString() );
                        contentLine.Add( w.DefMissSound.ToString() );
                        contentLine.Add( w.HitRateBonus.ToString() );
                        contentLine.Add( w.EvasionBonus.ToString() );
                        contentLine.Add( w.StaminaLossOnHit ? "1d3+2" : "" );

                        if( !( w.ElegibleForLumberBonus || w.ElegibleForSwordsBonus || w.ElegibleForMiningBonus || w.ElegibleForBowcraftBonus ) )
                            contentLine.Add( alertstyle );
                        contentLine.Add( w.ElegibleForLumberBonus ? "X" : "" );

                        if( !( w.ElegibleForLumberBonus || w.ElegibleForSwordsBonus || w.ElegibleForMiningBonus || w.ElegibleForBowcraftBonus ) )
                            contentLine.Add( alertstyle );
                        contentLine.Add( w.ElegibleForSwordsBonus ? "X" : "" );

                        if( !( w.ElegibleForLumberBonus || w.ElegibleForSwordsBonus || w.ElegibleForMiningBonus || w.ElegibleForBowcraftBonus ) )
                            contentLine.Add( alertstyle );
                        contentLine.Add( w.ElegibleForMiningBonus ? "X" : "" );

                        if( !( w.ElegibleForLumberBonus || w.ElegibleForSwordsBonus || w.ElegibleForMiningBonus || w.ElegibleForBowcraftBonus ) )
                            contentLine.Add( alertstyle );
                        contentLine.Add( w.ElegibleForBowcraftBonus ? "X" : "" );

                        contentLine.Add( ( elem.Value.Count > 0 ) ? elem.Value.Count.ToString() : "" );

                        contentMatrix.Add( contentLine );
                    }

                    PreAoSDocHelper.AppendTable( html, "Midgard Weapons", headers, contentMatrix, true, false, "" );
                    html.Write( "<br/><br/>" );

                    html.RenderEndTag(); // Body

                    html.RenderEndTag(); // Html
                }
            }
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

        private static readonly string WeaponsExtendedDocDir = Path.Combine( PreAoSDocHelper.DocDir, "weapons" );
    }
}