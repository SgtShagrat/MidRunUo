using System;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Midgard.Commands
{
    public class PlayerTemplate
    {
        public static void Initialize()
        {
            CommandSystem.Register( "PlayerTemplate", AccessLevel.GameMaster, new CommandEventHandler( PlayerTemplate_OnCommand ) );
        }

        [Usage( "PlayerTemplate" )]
        [Description( "Set a special temporary template to a given mobile" )]
        private static void PlayerTemplate_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new TemplateSelectionGump( e.Mobile ) );
            e.Mobile.SendMessage( "Select the template thou want to apply." );
        }

        private abstract class TemplateDefinition
        {
            public abstract string Name { get; }

            public abstract void Apply( Mobile m );

            protected static void TopSkill( Mobile m, SkillName skill )
            {
                m.Skills[ skill ].Base = 100.0;
            }

            protected static void ResetSkills( Mobile m )
            {
                foreach( int i in Enum.GetValues( typeof( SkillName ) ) )
                    m.Skills[ (SkillName)i ].Base = 0.0;
            }

            protected static void SetStats( Mobile m, int strength, int intelligence, int dexterity )
            {
                m.RawStr = strength;
                m.RawDex = dexterity;
                m.RawInt = intelligence;
            }
        }

        private class TemplateSelectionGump : Gump
        {
            private const int Fields = 9;
            private const int HueTit = 662;
            private const int DeltaBut = 2;
            private const int FieldsDist = 25;
            private const int HuePrim = 92;

            private int m_Page;
            private readonly Mobile m_From;

            public TemplateSelectionGump( Mobile from )
                : this( from, 1 )
            {
            }

            private TemplateSelectionGump( Mobile from, int page )
                : base( 50, 50 )
            {
                Closable = false;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_From = from;
                m_Page = page;

                Design();
            }

            private enum Buttons
            {
                Close = 0,

                PrevPage = 200,
                NextPage = 300
            }

            private void Design()
            {
                AddPage( 0 );

                AddBackground( 0, 0, 275, 325, 9200 );

                AddImageTiled( 10, 10, 255, 25, 2624 );
                AddImageTiled( 10, 45, 255, 240, 2624 );
                AddImageTiled( 40, 295, 225, 20, 2624 );

                AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 45, 295, 75, 20, 1011012, HueTit, false, false ); // CANCEL

                AddAlphaRegion( 10, 10, 255, 285 );
                AddAlphaRegion( 40, 295, 225, 20 );

                AddLabelCropped( 14, 12, 255, 25, HueTit, "Make your choice:" );

                if( m_Page > 1 )
                    AddButton( 225, 297, 5603, 5607, (int)Buttons.PrevPage, GumpButtonType.Reply, 0 );

                if( m_Page < Math.Ceiling( m_Table.Length / (double)Fields ) )
                    AddButton( 245, 297, 5601, 5605, (int)Buttons.NextPage, GumpButtonType.Reply, 0 );

                int indMax = ( m_Page * Fields ) - 1;
                int indMin = ( m_Page * Fields ) - Fields;
                int indTemp = 0;

                for( int i = 0; i < m_Table.Length; i++ )
                {
                    if( i >= indMin && i <= indMax )
                    {
                        AddLabelCropped( 35, 52 + ( indTemp * FieldsDist ), 225, 20, HuePrim, m_Table[ i ].Name );
                        AddButton( 15, 52 + DeltaBut + ( indTemp * FieldsDist ), 1209, 1210, i + 1, GumpButtonType.Reply, 0 );
                        indTemp++;
                    }
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == (int)Buttons.Close )
                    return;
                else if( info.ButtonID == (int)Buttons.PrevPage )
                {
                    m_Page--;
                    from.SendGump( new TemplateSelectionGump( m_From, m_Page ) );
                }
                else if( info.ButtonID == (int)Buttons.NextPage )
                {
                    m_Page++;
                    from.SendGump( new TemplateSelectionGump( m_From, m_Page ) );
                }
                else
                {
                    int index = info.ButtonID - 1;
                    if( index > -1 && index < m_Table.Length )
                    {
                        TemplateDefinition t = m_Table[ index ];
                        from.SendMessage( "Choose the mobile." );
                        from.Target = new Templatetarget( t );
                    }
                }
            }
        }

        /// <summary>
        /// porta le seguenti skill : eval int, magery, res.spell, alchemy, inscription, 
        /// wrestling, meditation al 100% mentre azzera le restanti. 
        /// Stats : 100 Int , 100 Str, 25 Dex
        /// </summary>
        private class MageTemplate : TemplateDefinition
        {
            public override string Name { get { return "Mage"; } }

            public override void Apply( Mobile m )
            {
                ResetSkills( m );
                TopSkill( m, SkillName.Magery );
                TopSkill( m, SkillName.EvalInt );
                TopSkill( m, SkillName.MagicResist );
                TopSkill( m, SkillName.Alchemy );
                TopSkill( m, SkillName.Inscribe );
                TopSkill( m, SkillName.Wrestling );
                TopSkill( m, SkillName.Meditation );

                SetStats( m, 100, 100, 25 );
            }
        }

        /// <summary>
        /// porta le seguenti skill: swordmanship, parrying, anatomy, tactics, tasteid, 
        /// healing, resspell al 100% mentre azzera le restanti. 
        /// Stats : 100 Str , 100 Dex, 25 Int
        /// </summary>
        private class WarriorTemplate : TemplateDefinition
        {
            public override string Name { get { return "Warrior"; } }

            public override void Apply( Mobile m )
            {
                ResetSkills( m );

                TopSkill( m, SkillName.Swords );
                TopSkill( m, SkillName.Parry );
                TopSkill( m, SkillName.Anatomy );
                TopSkill( m, SkillName.Tactics );
                TopSkill( m, SkillName.TasteID );
                TopSkill( m, SkillName.Healing );
                TopSkill( m, SkillName.MagicResist );

                SetStats( m, 100, 25, 100 );
            }
        }

        /// <summary>
        /// porta le seguenti skill: archery, anatomy, tactics, tasteid, 
        /// healing, resspell, bowcrafting al 100% mentre azzera le restanti. 
        /// Stats : 100 Dex , 100 Str, 25 Int
        /// </summary>
        private class ArcherTemplate : TemplateDefinition
        {
            public override string Name { get { return "Archer"; } }

            public override void Apply( Mobile m )
            {
                ResetSkills( m );

                TopSkill( m, SkillName.Archery );
                TopSkill( m, SkillName.Anatomy );
                TopSkill( m, SkillName.Tactics );
                TopSkill( m, SkillName.TasteID );
                TopSkill( m, SkillName.Healing );
                TopSkill( m, SkillName.MagicResist );
                TopSkill( m, SkillName.Fletching );

                SetStats( m, 100, 100, 25 );
            }
        }

        /// <summary>
        /// porta le seguenti skill: snooping, stealing, lockpicking, remove traps,
        /// tinkering, fencing, anatomy al 100% mentre azzera le restanti.
        /// Stats : 25 Int , 100 Str, 100 Dex
        /// </summary>
        private class ThiefTemplate : TemplateDefinition
        {
            public override string Name { get { return "Thief"; } }

            public override void Apply( Mobile m )
            {
                ResetSkills( m );

                TopSkill( m, SkillName.Snooping );
                TopSkill( m, SkillName.Stealing );
                TopSkill( m, SkillName.Lockpicking );
                TopSkill( m, SkillName.RemoveTrap );
                TopSkill( m, SkillName.Tinkering );
                TopSkill( m, SkillName.Fencing );
                TopSkill( m, SkillName.Anatomy );

                SetStats( m, 100, 100, 25 );
            }
        }

        private class BlacksmithTemplate : TemplateDefinition
        {
            public override string Name { get { return "Blacksmith"; } }

            public override void Apply( Mobile m )
            {
                ResetSkills( m );

                TopSkill( m, SkillName.Blacksmith );
                TopSkill( m, SkillName.Mining );
                TopSkill( m, SkillName.ItemID );
                TopSkill( m, SkillName.ArmsLore );
                TopSkill( m, SkillName.Tinkering );

                SetStats( m, 100, 25, 100 );
            }
        }

        private class CarpenterTemplate : TemplateDefinition
        {
            public override string Name { get { return "Carpenter"; } }

            public override void Apply( Mobile m )
            {
                ResetSkills( m );

                TopSkill( m, SkillName.Carpentry );
                TopSkill( m, SkillName.ItemID );
                TopSkill( m, SkillName.ArmsLore );
                TopSkill( m, SkillName.Tinkering );

                SetStats( m, 100, 25, 100 );
            }
        }

        private class TailorTemplate : TemplateDefinition
        {
            public override string Name { get { return "Tailor"; } }

            public override void Apply( Mobile m )
            {
                ResetSkills( m );

                TopSkill( m, SkillName.Tailoring );
                TopSkill( m, SkillName.ItemID );
                TopSkill( m, SkillName.ArmsLore );
                TopSkill( m, SkillName.Tinkering );

                SetStats( m, 100, 25, 100 );
            }
        }

        private class EnchanterTemplate : TemplateDefinition
        {
            public override string Name { get { return "Tailor"; } }

            public override void Apply( Mobile m )
            {
                ResetSkills( m );

                TopSkill( m, SkillName.Blacksmith );
                TopSkill( m, SkillName.Tailoring );
                TopSkill( m, SkillName.ArmsLore );
                TopSkill( m, SkillName.ItemID );
                TopSkill( m, SkillName.Magery );
                TopSkill( m, SkillName.Tinkering );

                SetStats( m, 100, 100, 25 );
            }
        }

        private static readonly TemplateDefinition[] m_Table = new TemplateDefinition[]
                                                               {
                                                                   new MageTemplate(),
                                                                   new WarriorTemplate(),
                                                                   new ArcherTemplate(),
                                                                   new ThiefTemplate(),
                                                                   new BlacksmithTemplate(),
                                                                   new TailorTemplate(),
                                                                   new CarpenterTemplate(),
                                                                   new EnchanterTemplate()
                                                               };

        private class Templatetarget : Target
        {
            private readonly TemplateDefinition m_Definition;

            public Templatetarget( TemplateDefinition definition )
                : base( 16, false, TargetFlags.None )
            {
                m_Definition = definition;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( !( targeted is Mobile ) )
                    return;

                m_Definition.Apply( (Mobile)targeted );

                from.SendMessage( "Thou applied the {0} template.", m_Definition.Name );
            }
        }
    }
}