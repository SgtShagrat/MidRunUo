using System;
using Server.Gumps;
using Server.Mobiles;
using Server;
using System.Collections.Generic;
using Midgard.Engines.Notification;
namespace Midgard.Engines.PVMAbsorbtions
{
	public class TankNotifyIcon : NotifyIcon
	{
		public TankNotifyIcon (PlayerMobile player,string tankmsg) : base(player)
		{
			Duration = TimeSpan.FromSeconds(30);
			AppearSound = 0x2E8;
			GumpIcon = 140;
			Label = tankmsg;
			IconWidth = 40;
			LabelHue = 65;
			IconHeight = 40;
			LabelWidth = 130;
			X=3;
			Y=30;
			SendTimered();			
		}
	}
}

