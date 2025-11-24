/***************************************************************************
 *                               MobileTemplate.cs
 *                            -----------------------
 *   begin                : 21 agosto, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Targeting;

namespace Midgard.Commands
{
    public class MobileTemplate
    {
        public static void Initialize()
        {
            CommandSystem.Register( "MobileTemplate", AccessLevel.Counselor, new CommandEventHandler( MobileTemplate_OnCommand ) );
        }

        [Usage( "MobileTemplate <templateName>" )]
        [Description( "List all items in a given mobile" )]
        public static void MobileTemplate_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 1 )
            {
                from.SendMessage( "Choose the mobile." );
                from.Target = new InternalTarget( e.GetString( 0 ) );
            }
            else
            {
                from.SendMessage( "Command use: MobileTemplate <templateName>" );
            }
        }

        private static string ToSerial( int num )
        {
            return string.Format( "0x{0}", num.ToString( "X4" ) );
        }

        private class InternalTarget : Target
        {
            private string m_TemplateName;

            public InternalTarget( string templateName )
                : base( 16, false, TargetFlags.None )
            {
                m_TemplateName = templateName;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( !( targeted is Mobile ) )
                    return;

                Mobile mob = (Mobile)targeted;
                List<Item> items = new List<Item>( ( (Mobile)targeted ).Items );
                items.Sort();

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat( "Template for \"{0}\" ({1}-{2})<br>", m_TemplateName, mob.GetType().Name, ToSerial( mob.Serial.Value ) );

                stringBuilder.AppendFormat( "<br><br>" );

                stringBuilder.AppendFormat( "Hair: ID: {0} - Hue {1}<br>", ToSerial( mob.HairItemID ), ToSerial( mob.HairHue ) );
                stringBuilder.AppendFormat( "FacialHair: ID: {0} - Hue {1}<br>", ToSerial( mob.FacialHairItemID ), ToSerial( mob.FacialHairHue ) );

                stringBuilder.AppendFormat( "<br>Equip:<br>" );

                foreach( Item item in items )
                {
                    if( item is Backpack || item is BankBox )
                        continue;

                    stringBuilder.AppendFormat( "Type: {0} - ID {1} - Hue {2}<br>", item.GetType().Name,
                                                ToSerial( item.ItemID ), ToSerial( item.Hue ) );
                }

                stringBuilder.AppendFormat( "<br>" );

                foreach( Item item in items )
                {
                    if( item is Backpack || item is BankBox )
                        continue;

                    if( item.GetType() == typeof( Item ) && !item.GetType().IsSubclassOf( typeof( Item ) ) )
                    {
                        if( item.Hue != 0 )
                            stringBuilder.AppendFormat( "AddItem( Immovable( Rehued( new Item( {0} ), {1} ) ) );<br>", ToSerial( item.ItemID ), ToSerial( item.Hue ) );
                        else
                            stringBuilder.AppendFormat( "AddItem( Immovable( new Item( {0} ) ) );<br>", ToSerial( item.ItemID ) );
                    }
                    else if( item.Hue != 0 )
                        stringBuilder.AppendFormat( "AddItem( Immovable( Rehued( new {0}(), {1} ) ) );<br>", item.GetType().Name, ToSerial( item.Hue ) );
                    else
                        stringBuilder.AppendFormat( "AddItem( Immovable( new {0}() ) );<br>", item.GetType().Name );
                }

                from.SendGump( new WarningGump( 1060635, 30720, stringBuilder.ToString(), 0xFFC000, 400, 300, new WarningGumpCallback( ConfirmExport ), stringBuilder, true ) );
            }

            private void ConfirmExport( Mobile from, bool okay, object state )
            {
                if( state == null )
                    return;

                if( !okay )
                    return;

                ScriptCompiler.EnsureDirectory( "MobileTemplates" );

                StringBuilder sb = (StringBuilder)state;
                sb.Replace( "<br>", System.Environment.NewLine );

                string path = Path.Combine( Core.BaseDirectory, "MobileTemplates" );
                string fileName = Path.Combine( path, string.Format( "{0}.txt", m_TemplateName ) );

                using( StreamWriter tw = new StreamWriter( fileName, true ) )
                    tw.WriteLine( sb );

                from.SendMessage( "Template exported to: {0}.", fileName );
            }
        }
    }
}