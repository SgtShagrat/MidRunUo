/***************************************************************************
 *                                  PlantInfoGump.cs
 *                            		----------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasiaalice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.PlantSystem
{
    public class PlantInfoGump : Gump
    {
        #region campi

        private BasePlant m_Plant;
        private Mobile m_Owner;

        #endregion

        #region enums

        private enum MessageHues
        {
            Good = 62,
            Normal = 252,
            Bad = 237,
            Label = 955
        }

        #endregion

        #region costruttori

        public PlantInfoGump( BasePlant plant, Mobile owner )
            : base( 50, 50 )
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_Plant = plant;
            m_Owner = owner;

            if( m_Plant == null || m_Owner == null )
                return;

            m_Owner.CloseGump( typeof( PlantInfoGump ) );

            AddPage( 0 );
            AddBackground( 0, 0, 400, 380, 9350 );
            AddBackground( 10, 40, 380, 260, 2620 );
            AddBlackAlpha( 20, 50, 360, 240 );

            // Title of the Gump
            AddLabel( 119, 10, 662, "Midgard - Plant System" );

            // Labels in upper part
            AddLabel( 40, 60, (int)MessageHues.Label, "Plant Type:" );
            AddLabel( 40, 80, (int)MessageHues.Label, "Age:" );
            AddLabel( 40, 100, (int)MessageHues.Label, "Is Growing:" );
            AddLabel( 40, 120, (int)MessageHues.Label, "Can Produce:" );
            AddLabel( 40, 140, (int)MessageHues.Label, "Dry Hours:" );

            AddLabel( 220, 80, (int)MessageHues.Label, "Fruits-Seeds:" );
            AddLabel( 220, 100, (int)MessageHues.Label, "Is Public:" );
            AddLabel( 220, 120, (int)MessageHues.Label, "Level of Care:" );

            AddLabel( 40, 180, (int)MessageHues.Label, "General Infoes:" );
            AddLabel( 40, 240, (int)MessageHues.Label, "Desease Infoes:" );

            // Other labels
            AddLabel( 50, 345, (int)MessageHues.Label, "Uproot this plant" );
            AddLabel( 50, 315, (int)MessageHues.Label,
                     ( m_Plant.IsPublic ? "Make this plant private" : "Make this plant public" ) );
            AddLabel( 320, 315, (int)MessageHues.Label, "Close" );
            AddLabel( 290, 345, (int)MessageHues.Label, "Open guide" );

            // Messages
            string plantName = m_Plant.PhaseName[ m_Plant.CurrentPhase ];
            if( m_Plant.CurrentPhase == 0 )
                plantName = m_Plant.CropName + " (just planted)";
            AddLabel( 140, 60, (int)MessageHues.Good, plantName );

            string ageInDays = string.Concat( m_Plant.AgeInDays, " days" );
            AddLabel( 140, 80, (int)MessageHues.Good, ageInDays );

            int growingHue = m_Plant.IsGrowing ? (int)MessageHues.Good : (int)MessageHues.Bad;
            AddLabel( 140, 100, growingHue, m_Plant.IsGrowing.ToString() );

            int canProduceHue = m_Plant.CanProduce ? (int)MessageHues.Good : (int)MessageHues.Bad;
            AddLabel( 140, 120, canProduceHue, m_Plant.CanProduce.ToString() );

            int dryHours = (int)( ( DateTime.Now - m_Plant.LastWatered ) ).TotalHours;
            int dryHoursHue = dryHours < PlantHelper.ProperWaterInterval.TotalHours
                                  ? (int)MessageHues.Good
                                  : (int)MessageHues.Bad;
            AddLabel( 140, 140, dryHoursHue, dryHours.ToString() );

            int hasFruitsHue = ( m_Plant.Yield + m_Plant.Seeds > 0 ) ? (int)MessageHues.Good : (int)MessageHues.Normal;
            AddLabel( 330, 80, hasFruitsHue, String.Format( "{0}-{1}", m_Plant.Yield, m_Plant.Seeds ) );

            int isPublicHue = m_Plant.IsPublic ? (int)MessageHues.Good : (int)MessageHues.Normal;
            AddLabel( 330, 100, isPublicHue, m_Plant.IsPublic.ToString() );

            int levelOfCareHue;
            if( m_Plant.CareLevel < PlantHelper.CareLevelBad )
                levelOfCareHue = (int)MessageHues.Bad;
            else if( m_Plant.CareLevel < PlantHelper.CareLevelGood )
                levelOfCareHue = (int)MessageHues.Normal;
            else
                levelOfCareHue = (int)MessageHues.Good;
            AddLabel( 330, 120, levelOfCareHue, Enum.GetName( typeof( MessageHues ), levelOfCareHue ) );

            string generalMessage;
            int generalMessageHue;
            if( !m_Plant.IsGrowing && m_Plant.CurrentPhase == 0 )
            {
                generalMessage = "the seed is not sprouting due to lack of water.";
                generalMessageHue = (int)MessageHues.Bad;
            }
            else if( m_Plant.CurrentPhase == 0 )
            {
                generalMessage = "wait until the seed sprout...";
                generalMessageHue = (int)MessageHues.Bad;
            }
            else if( !m_Plant.IsGrowing )
            {
                generalMessage = "this plant is withering due to lack of proper care.";
                generalMessageHue = (int)MessageHues.Bad;
            }
            else
            {
                generalMessage = "this plant is growing";
                generalMessageHue = (int)MessageHues.Good;
            }
            AddLabel( 50, 200, generalMessageHue, generalMessage );

            if( !m_Plant.HasAnyDesease )
                AddLabel( 50, 260, (int)MessageHues.Good, "this plant has no desease" );
            else if( m_Plant.GotFungus && !m_Plant.GotPest )
                AddLabel( 50, 260, (int)MessageHues.Bad, "you saw some leaves covered in white powder!" );
            else if( !m_Plant.GotFungus && m_Plant.GotPest )
                AddLabel( 50, 260, (int)MessageHues.Bad, "you saw some aphids sucking the leaves! Yuck!" );
            else
            {
                AddLabel( 50, 260, (int)MessageHues.Bad, "you saw some leaves covered in white powder!" );
                AddLabel( 50, 280, (int)MessageHues.Bad, "you saw some aphids sucking the leaves! Yuck!" );
            }

            // Buttons
            AddButton( 10, 310, 2640, 2641, 1, GumpButtonType.Reply, 0 ); //
            AddButton( 360, 310, 2640, 2641, 0, GumpButtonType.Reply, 0 ); // 
            AddButton( 10, 340, 2640, 2641, 2, GumpButtonType.Reply, 0 ); //
            AddButton( 360, 340, 2640, 2641, 3, GumpButtonType.Reply, 0 ); //
        }

        #endregion

        #region metodi

        private void AddBlackAlpha( int x, int y, int width, int height )
        {
            AddImageTiled( x, y, width, height, 2624 );
            AddAlphaRegion( x, y, width, height );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( m_Plant == null )
                return;

            Mobile from = sender.Mobile;
            if( from == null )
                return;

            if( info.ButtonID == 1 )
            {
                if( !m_Plant.IsPublic )
                {
                    from.SendGump( new WarningGump( 1060635, 30720, "You are going to make this plant public.<br>" +
                                                                  "Every one will have access and could harvest or even destroy that plant.<br>" +
                                                                  "Are you sure you want to make this plant public?",
                                                  0xFFC000, 420, 280, new WarningGumpCallback( ConfirmMakePublicCallback ),
                                                  new object[] { m_Plant }, true ) );
                }
                else
                {
                    from.SendGump( new WarningGump( 1060635, 30720, "You are going to make this plant private.<br>" +
                                                                  "Only you, the plant owner, will have access and could harvest or even destroy that plant.<br>" +
                                                                  "Are you sure you want to make this plant private?",
                                                  0xFFC000, 420, 280,
                                                  new WarningGumpCallback( ConfirmMakePrivateCallback ),
                                                  new object[] { m_Plant }, true ) );
                }
            }
            else if( info.ButtonID == 2 )
            {
                if( m_Plant.IsDestroyable )
                {
                    from.SendGump( new WarningGump( 1060635, 30720, "You are going to uproot this plant.<br>" +
                                                                  "That action is irreversible and will cause plat death and removement.<br>" +
                                                                  "Are you sure you want to uproot this plant?",
                                                  0xFFC000, 420, 280, new WarningGumpCallback( ConfirmDestroyCallback ),
                                                  new object[] { m_Plant }, true ) );
                }
            }
            else if( info.ButtonID == 3 )
            {
                from.LaunchBrowser( PlantHelper.HelpPlantSystemURL );
            }
            else
            {
                from.SendMessage( "You closed that plant info menu." );
            }
        }

        private static void ConfirmDestroyCallback( Mobile from, bool okay, object state )
        {
            object[] states = (object[])state;
            BasePlant plant = (BasePlant)states[ 0 ];

            if( okay )
            {
                from.SendMessage( "You have decided to proceede." );
                plant.Die();
            }
        }

        private static void ConfirmMakePublicCallback( Mobile from, bool okay, object state )
        {
            object[] states = (object[])state;
            BasePlant plant = (BasePlant)states[ 0 ];

            if( okay )
            {
                from.SendMessage( "You have decided to proceede." );
                plant.IsPublic = true;
            }
        }

        private static void ConfirmMakePrivateCallback( Mobile from, bool okay, object state )
        {
            object[] states = (object[])state;
            BasePlant plant = (BasePlant)states[ 0 ];

            if( okay )
            {
                from.SendMessage( "You have decided to proceede." );
                plant.IsPublic = false;
            }
        }

        #endregion
    }
}