using System;
using Server.Gumps;
using Server.Mobiles;
using Server;
using System.Collections.Generic;
namespace Midgard.Engines.Notification
{
	public class NotifyIcon : Gump
	{
		public TimeSpan Duration{get; set;}
		public int GumpIcon{get; set;}
		public int ArtIcon{get; set;}
		public int ArtIconHue{get; set;}
		public string Label{get; set;}
		public int LabelHue{get; set;}
		public int IconWidth {get; set;}
		public int LabelWidth {get; set;}
		public int IconHeight {get; set;}
		public int AppearSound {get; set;}
		public int UpdateSound {get; set;}
		public readonly PlayerMobile Player;
		
		/// <summary>
		/// use gumpicon -1 to add an art icon instead.
		/// </summary>
		/// <param name="gumpicon">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="articon">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="label">
		/// A <see cref="System.String"/>
		/// </param>
		public NotifyIcon(PlayerMobile player) : base(3,30)
		{					
			Duration = TimeSpan.FromSeconds(10);
			UpdateSound= -1;
			AppearSound = -1;
			GumpIcon = -1;
			ArtIcon = -1;
			Player = player;
			Dragable = true;
			Disposable = false;
			Resizable = false;			
		}
		private static List<InternalTimer> ActiveTimers=new List<InternalTimer>();
		private InternalTimer timer = null;
		protected void SendTimered()
		{
			Design();
			
			if (Player.HasGump(GetType()))
				Player.CloseGump(GetType());
			else
			{
				Player.SendSound( AppearSound );
			}
			
			Player.SendGump(this);
			
			StopExistingTimer ( );
			if (Duration != TimeSpan.Zero)
			{
				timer = new InternalTimer(this);
				timer.Start();
			}
		}
		
		private void StopExistingTimer( )
		{
			var activeTimers = ActiveTimers.ToArray();
			foreach(var exists in activeTimers)
			{
				if (exists.Notify.GetType() == GetType() && exists.Notify.Player == Player)
				{
					exists.Stop();
					ActiveTimers.Remove(exists);
				}
			}
			
		}
		
		protected virtual void Design()
		{
			var Separator = 10;
			var textsize = 20;
			AddAlphaRegion(0,0,IconWidth + LabelWidth + Separator,IconHeight);
			if (GumpIcon>-1)
				AddImage(0,0,GumpIcon);
			else
				AddItem(0,0,ArtIcon,ArtIconHue);
			int y =  (int)(((double)IconWidth - (double)textsize) / 2.0);
			AddLabelCropped(IconWidth + Separator, y, LabelWidth, textsize , LabelHue, Label);			
		}
		public void Close()
		{
			if (timer!=null)
				timer.Stop();
			timer = null;
			
			StopExistingTimer();
			
			if (Player.HasGump(GetType()))
				Player.CloseGump(GetType());
		}
		private class InternalTimer : Timer
		{
			public readonly NotifyIcon Notify;
			public InternalTimer(NotifyIcon notify) : base( notify.Duration )
			{
				Notify = notify;
				ActiveTimers.Add(this);
			}
			protected override void OnTick ()
			{
				Stop();
				Notify.Close();
			}
		}
	}
}

