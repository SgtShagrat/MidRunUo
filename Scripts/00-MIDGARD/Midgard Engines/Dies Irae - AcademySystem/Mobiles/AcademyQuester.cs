/***************************************************************************
 *                               AcademyQuester.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Engines.Quests;
using Server.Mobiles;

namespace Midgard.Engines.Academies
{
    public abstract class AcademyQuester : BaseVendor
    {
        /// <summary>
        /// The main Academy this vendor works for
        /// </summary>
        public abstract AcademySystem Academy { get; }

        /// <summary>
        /// The training discipline of this quester
        /// </summary>
        public abstract Disciplines Discipline { get; }

		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public override void InitSBInfo()
		{		
		}

        public AcademyQuester()
            : base( null )
        {
            SpeechHue = 0x3B2;
        }

        public AcademyQuester( string name )
            : this( name, null )
        {
        }

        public AcademyQuester( string name, string title )
            : base( title )
        {
            Name = name;
            SpeechHue = 0x3B2;
        }

        public override void OnDoubleClick( Mobile m )
        {
            if( m.Alive && m is PlayerMobile )
                OnTalk( (PlayerMobile)m );
        }

        public override bool CanOfferQuestTo( Mobile m )
        {
            return ( m.Alive && m is PlayerMobile && Academy.IsMember( m ) );
        }

        public override void OnTalk( PlayerMobile player )
        {
            if( QuestHelper.DeliveryArrived( player, this ) )
                return;

            if( QuestHelper.InProgress( player, this ) )
                return;

            if( QuestHelper.QuestLimitReached( player ) )
                return;

            // check if this quester can offer any quest chain (already started)
            foreach( KeyValuePair<QuestChain, BaseChain> pair in player.Chains )
            {
                BaseChain chain = pair.Value;

                if( chain != null && chain.Quester != null && chain.Quester == GetType() )
                {
                    BaseQuest quest = QuestHelper.RandomQuest( player, new Type[] { chain.CurrentQuest }, this );

                    if( quest != null )
                    {
                        player.CloseGump( typeof( MondainQuestGump ) );
                        player.SendGump( new MondainQuestGump( quest ) );
                        return;
                    }
                }
            }

            BaseQuest questt = QuestHelper.RandomQuest( player, GetValidQuestFor( player ), this );

            if( questt != null )
            {
                player.CloseGump( typeof( MondainQuestGump ) );
                player.SendGump( new MondainQuestGump( questt ) );
            }
        }

        private Type[] GetValidQuestFor( PlayerMobile player )
        {
            return Quests;
        }

        #region serialization
        public AcademyQuester( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}