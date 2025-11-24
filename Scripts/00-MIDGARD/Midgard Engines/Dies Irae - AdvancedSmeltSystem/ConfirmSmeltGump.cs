/***************************************************************************
 *                               ConfirmSmeltGump.cs
 *
 *   begin                : 06 agosto, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.AdvancedSmelting
{
    public class ConfirmSmeltGump : Gump
    {
        private class HuedLine
        {
            public HuedLine( string line, int hue )
            {
                Line = line;
                Hue = hue;
                Line = line;
                Hue = hue;
            }

            public string Line { get; private set; }

            public int Hue { get; private set; }
        }

        private const int m_FieldsDist = 25;

        private readonly Mobile m_Owner;
        private readonly AdvancedForge m_Forge;

        public ConfirmSmeltGump( Mobile owner, AdvancedForge forge )
            : base( 100, 100 )
        {
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_Owner = owner;
            if( m_Owner == null )
                return;

            m_Forge = forge;
            if( m_Forge == null || m_Forge.Deleted || !m_Owner.InRange( m_Forge.GetWorldLocation(), 2 ) )
                return;

            Design();
        }

        private void Design()
        {
            AddPage( 0 );

            AddBackground( 0, 0, 275, 325, 9200 );

            AddImageTiled( 10, 10, 255, 25, 2624 );
            AddImageTiled( 10, 45, 255, 240, 2624 );
            AddImageTiled( 40, 295, 225, 20, 2624 );

            AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
            AddHtmlLocalized( 45, 295, 75, 20, 1011012, 32767, false, false ); // CANCEL

            AddAlphaRegion( 10, 10, 255, 285 );
            AddAlphaRegion( 40, 295, 225, 20 );

            AddOldHtmlHued( 14, 12, 255, 25, "Smelting process:", Colors.GreenYellow );

            List<HuedLine> linesList = new List<HuedLine>();

            if( m_Forge.TotalAmount + m_Forge.ForgeCoilAmount > 0 )
            {
                linesList.Add( new HuedLine( "This forge contains:", Colors.Gold ) );

                if( m_Forge.ForgeResourceAmount1 > 0 )
                    linesList.Add( new HuedLine( string.Format( "{0} ({1} parts)", GetMaterialName( m_Forge.ForgeInfo1 ), m_Forge.ForgeResourceAmount1 ), Colors.RoyalBlue ) );

                if( m_Forge.ForgeResourceAmount2 > 0 )
                    linesList.Add( new HuedLine( string.Format( "{0} ({1} parts)", GetMaterialName( m_Forge.ForgeInfo2 ), m_Forge.ForgeResourceAmount2 ), Colors.RoyalBlue ) );

                if( m_Forge.ForgeCoilAmount > 0 )
                    linesList.Add( new HuedLine( string.Format( "Coil ({0} parts)", m_Forge.ForgeCoilAmount ), Colors.LightSlateGray ) );

                if( m_Forge.Temperature > Temperature.Cold )
                    linesList.Add( new HuedLine( string.Format( "Temperature {0}", m_Forge.Temperature ), Colors.LightSlateGray ) );

                linesList.Add( new HuedLine( "", 0 ) );
            }
            else
                linesList.Add( new HuedLine( "This forge is empty", Colors.RoyalBlue ) );

            int amount;
            SmeltInfo info = SmeltInfo.GetRecipe( m_Forge, out amount );

            if( info != null )
            {
                linesList.Add( new HuedLine( "The ore mixture will produce:", Colors.Tomato ) );
                linesList.Add( new HuedLine( string.Format( "{0} ingots of {1}", amount, GetMaterialName( info.ResultRes ) ), Colors.Tomato ) );

                if( info.Temperature > m_Forge.Temperature )
                    linesList.Add( new HuedLine( "The forge is not enought hot.", Colors.Tomato ) );
                else
                {
                    linesList.Add( new HuedLine( "Do you want to proceede?", Colors.Tomato ) );
                    AddButton( 235, 295, 4017, 4018, 1, GumpButtonType.Reply, 0 );
                    AddHtmlLocalized( 180, 295, 60, 20, 1011036, 32767, false, false ); // OKAY
                }
            }

            for( int i = 0; i < linesList.Count; i++ )
                AddOldHtmlHued( 20, 52 + i * m_FieldsDist, 225, 20, linesList[ i ].Line, linesList[ i ].Hue );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( info.ButtonID != 1 )
                return;

            if( m_Forge != null && !m_Forge.Deleted && m_Owner.InRange( m_Forge.GetWorldLocation(), 2 ) )
                m_Forge.BeginSmelt( m_Owner );
        }

        private static string GetMaterialName( CraftResource resource )
        {
            return CraftResources.GetName( resource );
        }
    }
}