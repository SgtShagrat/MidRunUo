using System;

namespace Server.Engines.Craft
{
	public class CraftSubRes
	{
		private Type m_Type;
		private double m_ReqSkill;
		private string m_NameString;
		private int m_NameNumber;
		private int m_GenericNameNumber;
		private object m_Message;
        private double m_MaterialDifficulty; // mod by Dies Irae

		public CraftSubRes( Type type, TextDefinition name, double reqSkill, object message ) : this( type, name, reqSkill, 0, message, 1.0 )
		{
		}

		public CraftSubRes( Type type, TextDefinition name, double reqSkill, int genericNameNumber, object message, double difficulty )
		{
			m_Type = type;
			m_NameNumber = name;
			m_NameString = name;
			m_ReqSkill = reqSkill;
			m_GenericNameNumber = genericNameNumber;
			m_Message = message;
            m_MaterialDifficulty = difficulty; // mod by Dies Irae
		}

		public Type ItemType
		{
			get { return m_Type; }
		}

		public string NameString
		{
			get { return m_NameString; }
		}

		public int NameNumber
		{
			get { return m_NameNumber; }
		}

		public int GenericNameNumber
		{
			get { return m_GenericNameNumber; }
		}

		public object Message
		{
			get { return m_Message; }
		}

		public double RequiredSkill
		{
			get { return m_ReqSkill; }
		}

        #region mod by Dies Irae
        public double MaterialDifficulty
        {
            get { return m_MaterialDifficulty; }
        }
        #endregion
    }
}