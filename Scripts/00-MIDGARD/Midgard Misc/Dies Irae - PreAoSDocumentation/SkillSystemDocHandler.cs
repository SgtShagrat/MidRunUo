using System.Collections.Generic;
using System.IO;
using System.Web.UI;

using Server;

namespace Midgard.Misc
{
    public class SkillSystemDocHandler : DocumentationHandler
    {
        public SkillSystemDocHandler()
        {
            Enabled = false;
        }

        public override void GenerateDocumentation()
        {
            PreAoSDocHelper.EnsureDirectory( SkillStatDocDir );

            List<string> headers = new List<string>();
            headers.Add( "Skill Name" );
            headers.Add( "Primary Stat" );
            headers.Add( "Secondary Stat" );
            headers.Add( "Title" );

            using( StreamWriter op = new StreamWriter( Path.Combine( SkillStatDocDir, "skills.html" ) ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Documentation - {0}\n", "Skill System" );
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

                    List<SkillInfo> list = new List<SkillInfo>( SkillInfo.Table );
                    list.Sort( new SkillInfoSorter() );

                    foreach( SkillInfo info in list )
                    {
                        if( info.SkillID == (int)SkillName.Focus || info.SkillID == (int)SkillName.Bushido || info.SkillID == (int)SkillName.Ninjitsu )
                            continue;

                        List<string> contentLine = new List<string>();

                        contentLine.Add( info.Name );
                        contentLine.Add( GetStatName( GetPrimaryStat( info ) ) );
                        contentLine.Add( GetStatName( GetSecondaryStat( info ) ) );
                        contentLine.Add( info.Title );

                        contentMatrix.Add( contentLine );
                    }

                    PreAoSDocHelper.AppendTable( html, "Midgard Skill System", headers, contentMatrix, true, false, "" );
                    html.Write( "<br><br>" );

                    html.RenderEndTag(); // Body

                    html.RenderEndTag(); // Html
                }
            }
        }

        private static StatType GetPrimaryStat( SkillInfo info )
        {
            if( info.StrGain == 0.75 )
                return StatType.Str;
            else if( info.DexGain == 0.75 )
                return StatType.Dex;
            else if( info.IntGain == 0.75 )
                return StatType.Int;
            else
                return StatType.All;
        }

        private static StatType GetSecondaryStat( SkillInfo info )
        {
            if( info.StrGain == 0.25 )
                return StatType.Str;
            else if( info.DexGain == 0.25 )
                return StatType.Dex;
            else if( info.IntGain == 0.25 )
                return StatType.Int;
            else
                return StatType.All;
        }

        private static string GetStatName( StatType stat )
        {
            switch( stat )
            {
                case StatType.Dex:
                    return "Dexterity";
                case StatType.Str:
                    return "Strength";
                case StatType.Int:
                    return "Intelligence";
                default:
                    return "";
            }
        }

        private static readonly string SkillStatDocDir = Path.Combine( PreAoSDocHelper.DocDir, "skills" );

        private class SkillInfoSorter : IComparer<SkillInfo>
        {
            public int Compare( SkillInfo x, SkillInfo y )
            {
                return Insensitive.Compare( x.Name, y.Name );
            }
        }
    }
}