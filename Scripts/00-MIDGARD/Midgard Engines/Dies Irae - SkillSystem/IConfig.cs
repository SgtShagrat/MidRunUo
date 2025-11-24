using System;
using Server;

namespace Midgard.Engines.SkillSystem
{
	public interface IConfig
	{
		bool IsEnabled
        {
            get;
            set;
        }
	}
}

