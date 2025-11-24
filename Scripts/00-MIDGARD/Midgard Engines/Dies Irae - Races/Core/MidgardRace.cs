using System;
using System.Collections.Generic;
using System.Text;

using Server;

namespace Midgard.Engines.Races
{
    public abstract class MidgardRace : Race
    {
        protected MidgardRace( int raceID, int raceIndex, string name, string pluralName, int maleBody, int femaleBody, int maleGhostBody, int femaleGhostBody )
            : base( raceID, raceIndex, name, pluralName, maleBody, femaleBody, maleGhostBody, femaleGhostBody, Expansion.None )
        {
            m_Candidates = new List<Mobile>();
            m_Players = new List<Mobile>();
        }

        public virtual MorphEntry[] GetMorphList()
        {
            return Morph.EmptyList;
        }

        public virtual bool IsEvilAlignedRace { get { return false; } }

        public virtual double GetGainFactorBonuses( Skill skill )
        {
            return 0.0;
        }

        public virtual int GetResistanceBonus( ResistanceType type )
        {
            return 0;
        }

        protected List<Mobile> m_Players;

        public List<Mobile> Players { get { return m_Players; } }

        protected List<Mobile> m_Candidates;

        public List<Mobile> Candidates { get { return m_Candidates; } }

        public bool IsCandidate( Mobile m )
        {
            return m_Candidates != null && m_Candidates.Contains( m );
        }

        public void AddCandidate( Mobile m )
        {
            if( m_Candidates != null && m_Candidates.Contains( m ) )
                return;

            if( m_Candidates == null )
                m_Candidates = new List<Mobile>();

            m_Candidates.Add( m );
        }

        public virtual int InfravisionLevel { get { return -1; } }
        public virtual int InfravisionDuration { get { return -1; } }

        public virtual bool SupportsFireworks { get { return false; } }
        public virtual bool SupportsBless { get { return false; } }
        public virtual bool SupportsBite { get { return false; } }

        public virtual bool IsAllowedSkill( SkillName skillName )
        {
            return true;
        }

        public virtual RaceSkillMod[] GetSkillBonuses()
        {
            return SkillBonuses.EmptyList;
        }

        public virtual bool SupportsMagerySpells { get { return true; } }

        public virtual bool SameHairSameSkinHues { get { return false; } }
        public virtual bool SupportCustomBody { get { return false; } }

        public virtual void DressCustomBody( Mobile from )
        {
        }

        public virtual void UnDressCustomBody( Mobile from )
        {
        }

        public virtual bool SameHairSameFacialHues { get { return false; } }
        public virtual bool UnhuedFacialhair { get { return false; } }
        public abstract RaceLanguageFlag LanguageFlag { get; }

        public static bool CheckEquip( Race requiredRace, Mobile from, bool message )
        {
            if( requiredRace == null )
                return true;

            Race toCheck = from.Race;
            bool allowed = true;

            if( requiredRace == Elf )
                allowed = Core.IsElfRace( toCheck );
            else
                allowed = from.Race == toCheck;

            if( !allowed && message )
                from.SendMessage( "Only {0} can wear this", requiredRace.PluralName );

            return allowed;
        }

        public virtual string Log()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine( string.Format( "Race: {0}", Name ) );

            MorphEntry[] morphList = GetMorphList();
            if( morphList != null && morphList.Length > 0 )
                foreach( MorphEntry entry in morphList )
                {
                    b.AppendLine( entry.ToString() );
                }

            Mobile m = new Mobile();
            for( int s = 0; s < m.Skills.Length; s++ )
            {
                Skill skill = m.Skills[ s ];
                double bonus = GetGainFactorBonuses( skill );
                if( bonus > 0.0 )
                    b.AppendLine( string.Format( "Skill gain factor bonus: {0} {1}", skill.Name, bonus.ToString( "F2" ) ) );
            }

            foreach( int i in Enum.GetValues( typeof( ResistanceType ) ) )
            {
                int bonus = GetResistanceBonus( (ResistanceType)i );
                if( bonus > 0 )
                    b.AppendLine( string.Format( "Elemental Resistance bonus: {0} {1}", ( (ResistanceType)i ), bonus ) );
            }

            if( InfravisionLevel > 0 )
                b.AppendLine( string.Format( "Infravision bonus: level {0} duration {1}", InfravisionDuration, InfravisionLevel ) );

            if( SupportsFireworks )
                b.AppendLine( string.Format( "Support [Fuochi command" ) );

            if( SupportsBless )
                b.AppendLine( string.Format( "Support [Urlo command" ) );

            if( SupportsBite )
                b.AppendLine( string.Format( "Support [Morso command" ) );

            RaceSkillMod[] list = GetSkillBonuses();
            if( list != null )
                foreach( RaceSkillMod rsm in list )
                {
                    b.AppendLine( string.Format( "Skill bonus: skill {0} bonus {1}", rsm.Skill, rsm.Value.ToString( "F2" ) ) );
                }

            if( SameHairSameSkinHues )
                b.AppendLine( string.Format( "Has same hue for hair and skin." ) );

            if( SameHairSameFacialHues )
                b.AppendLine( string.Format( "Has same hue for hair and facialhair." ) );

            if( UnhuedFacialhair )
                b.AppendLine( string.Format( "Has hue 0 for facial hue." ) );

            return b.ToString();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            if( m_Candidates == null )
                m_Candidates = new List<Mobile>();

            writer.Write( m_Candidates, true );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            if( m_Candidates == null )
                m_Candidates = new List<Mobile>();

            m_Candidates = reader.ReadStrongMobileList();
        }

        public static void WriteReference( GenericWriter writer, Race sys )
        {
            int idx = AllRaces.IndexOf( sys );

            writer.WriteEncodedInt( idx + 1 );
        }

        public static Race ReadReference( GenericReader reader )
        {
            int idx = reader.ReadEncodedInt() - 1;

            if( idx >= 0 && idx < AllRaces.Count )
                return AllRaces[ idx ];

            return null;
        }
    }
}