using System;
using Server;

namespace Server.Engines.Quests
{
	public enum QuestChain
	{
		None,
		Aemaeth,
		AncientWorld,
		BlightedGrove,
		CovetousGhost,
		GemkeeperWarriors,
		HonestBeggar,
		LibraryFriends,
		Marauders,
		MiniBoss,
		SummonFey,
		SummonFiend,
		TuitionReimbursement,
		Spellweaving,
		SpellweavingS,
		UnfadingMemories,
		DraconomiconRevised, // mod by Dies Irae
		PrincessWedding, // mod by Dies Irae
        ArcaioloIncontentabile, // mod by Dies Irae

        #region virtue champion quests
        VirtueChampionCompassion,
        VirtueChampionHonesty,
        VirtueChampionValor,
        VirtueChampionJustice,
        VirtueChampionSacrifice,
        VirtueChampionHonor,
        VirtueChampionSpirituality,
        VirtueChampionHumility
        #endregion
    }
	
	public class BaseChain
	{
		private Type m_CurrentQuest;
		private Type m_Quester;
		
		public Type CurrentQuest
		{
			get{ return m_CurrentQuest; }
			set{ m_CurrentQuest = value; }
		}
		
		public Type Quester
		{
			get{ return m_Quester; }
			set{ m_Quester = value; }
		}
	
		public BaseChain( Type currentQuest, Type quester )
		{
			m_CurrentQuest = currentQuest;
			m_Quester = quester;
		}
	}
}