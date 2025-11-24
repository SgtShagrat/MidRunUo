/*
namespace Assistant
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Windows.Forms;

    public class PacketPlayer
    {
        private static Button btnClose;
        private static Button btnPlay;
        private static Button btnRec;
        private static Button btnStop;
        private static TimeSpan FadeDelay = TimeSpan.FromSeconds(3.0);
        private static Label lblPlay;
        private static Label lblTime;
        private static Assistant.TimerCallback m_BeginPlay = new Assistant.TimerCallback(PacketPlayer.BeginPlayback);
        private static TimeSpan m_CurLength = TimeSpan.Zero;
        private static TimeSpan m_Elapsed;
        private static Assistant.TimerCallback m_EndPlay = new Assistant.TimerCallback(PacketPlayer.EndPlayback);
        private static GZBlockIn m_GZIn;
        private static GZBlockOut m_GZOut;
        private static Hashtable m_HouseDataWritten = new Hashtable();
        private static DateTime m_LastTime;
        private static bool m_Playing = false;
        private static int m_PlaySpeed = 0;
        private static Assistant.Timer m_PlayTimer;
        private static bool m_Recording = false;
        private static string m_RPVInfo;
        private static Assistant.Timer m_ScrollTimer;
        private static Assistant.TimerCallback m_SendNext = new Assistant.TimerCallback(PacketPlayer.SendNextPacket);
        private static int m_StartPos;
        private static DateTime m_StartTime;
        private static BinaryWriter m_TempWriter;
        private static byte m_Version;
        private const byte PlayerVersion = 4;
        private static TrackBar tbPos;

        private static void BeginPlayback()
        {
            DoLogin(World.Player);
            ClientCommunication.SetDeathMsg("You are dead.");
            ClientCommunication.BringToFront(ClientCommunication.FindUOWindow());
            TimeSpan delay = TimeSpan.FromMilliseconds((double) m_GZIn.Compressed.ReadInt32());
            m_PlayTimer = Assistant.Timer.DelayedCallback(delay, m_SendNext);
            m_PlayTimer.Start();
            if (m_ScrollTimer == null)
            {
                m_ScrollTimer = new ScrollTimer();
            }
            m_ScrollTimer.Start();
            m_StartTime = DateTime.Now;
            m_Elapsed = delay;
            UpdateTimeText();
            btnPlay.Enabled = btnStop.Enabled = true;
        }

        public static bool ClientPacket(Packet p)
        {
            Serial serial2;
            Mobile mobile2;
            int num2;
            if (!Playing)
            {
                if (m_Recording)
                {
                    if (p == null)
                    {
                        return true;
                    }
                    if (p.PacketID == 0xb1)
                    {
                        p.ReadInt32();
                        WritePacket(new CloseGump(p.ReadUInt32()));
                    }
                }
                return true;
            }
            if ((p == null) || (World.Player == null))
            {
                return false;
            }
            switch (p.PacketID)
            {
                case 0x6c:
                {
                    p.ReadByte();
                    uint num4 = p.ReadUInt32();
                    p.ReadByte();
                    Serial serial3 = p.ReadUInt32();
                    if (num4 == 0x7fffffff)
                    {
                        ClientCommunication.ForceSendToClient(new UnicodeMessage(-1, -1, MessageType.Regular, 0x25, 3, Language.CliLocName, "System", string.Format("Serial Number is {0}", serial3)));
                    }
                    goto Label_0446;
                }
                case 0xad:
                    p.MoveToData();
                    Assistant.Command.OnSpeech(p, new PacketHandlerEventArgs());
                    goto Label_0446;

                case 0xbf:
                    if (p.ReadUInt16() == 30)
                    {
                        Item item2 = World.FindItem(p.ReadUInt32());
                        if ((item2 != null) && (item2.HousePacket != null))
                        {
                            ClientCommunication.ForceSendToClient(new Packet(item2.HousePacket, item2.HousePacket.Length, true));
                        }
                    }
                    goto Label_0446;

                case 6:
                {
                    Mobile player;
                    Serial serial = p.ReadUInt32();
                    if (((serial.Value & 0x80000000) != 0) || (serial == World.Player.Serial))
                    {
                        player = World.Player;
                    }
                    else
                    {
                        player = World.FindMobile(serial);
                    }
                    if (player != null)
                    {
                        string name = player.Name;
                        switch (name)
                        {
                            case null:
                            case "":
                                name = "<No Data>";
                                break;
                        }
                        ClientCommunication.ForceSendToClient(new DisplayPaperdoll(player, name));
                    }
                    goto Label_0446;
                }
                case 7:
                    ClientCommunication.ForceSendToClient(new LiftRej(5));
                    goto Label_0446;

                case 8:
                    goto Label_0446;

                case 9:
                    serial2 = p.ReadUInt32();
                    if (!serial2.IsMobile)
                    {
                        if (serial2.IsItem)
                        {
                            Item item = World.FindItem(serial2);
                            if (((item != null) && (item.Name != null)) && (item.Name != ""))
                            {
                                ClientCommunication.ForceSendToClient(new UnicodeMessage(serial2, (int) item.ItemID, MessageType.Label, 0x3b2, 3, "ENU", "", item.Name));
                            }
                        }
                        goto Label_0446;
                    }
                    mobile2 = World.FindMobile(serial2);
                    if (((mobile2 == null) || (mobile2.Name == null)) || !(mobile2.Name != ""))
                    {
                        goto Label_0446;
                    }
                    switch (mobile2.Notoriety)
                    {
                        case 1:
                            num2 = 0x59;
                            goto Label_01CF;

                        case 2:
                            num2 = 0x3f;
                            goto Label_01CF;

                        case 3:
                        case 4:
                            num2 = 0x3b2;
                            goto Label_01CF;

                        case 5:
                            num2 = 0x90;
                            goto Label_01CF;

                        case 6:
                            num2 = 0x22;
                            goto Label_01CF;

                        case 7:
                            num2 = 0x35;
                            goto Label_01CF;
                    }
                    num2 = 0x481;
                    goto Label_01CF;

                case 0x34:
                {
                    p.ReadInt32();
                    int num3 = p.ReadByte();
                    Mobile m = World.FindMobile(p.ReadUInt32());
                    if (m != null)
                    {
                        switch (num3)
                        {
                            case 4:
                                if (m != World.Player)
                                {
                                    if ((m.Hits == 0) && ((m.HitsMax == 0) || (m.HitsMax == 1)))
                                    {
                                        m.HitsMax = 1;
                                        if ((m.Name == null) || (m.Name == ""))
                                        {
                                            m.Name = "<No Data>";
                                        }
                                    }
                                    else if (((m.Name == null) || (m.Name == "")) || (m.Name == "<No Data>"))
                                    {
                                        m.Name = "<No Name>";
                                    }
                                    ClientCommunication.ForceSendToClient(new MobileStatusCompact(m));
                                }
                                else
                                {
                                    ClientCommunication.ForceSendToClient(new MobileStatusExtended(World.Player));
                                    ClientCommunication.ForceSendToClient(new StatLockInfo(World.Player));
                                }
                                goto Label_0446;

                            case 5:
                                ClientCommunication.ForceSendToClient(new SkillsList());
                                goto Label_0446;
                        }
                    }
                    goto Label_0446;
                }
                case 2:
                    p.ReadByte();
                    ClientCommunication.ForceSendToClient(new MoveReject(p.ReadByte(), World.Player));
                    World.Player.Resync();
                    goto Label_0446;
            }
            goto Label_0446;
        Label_01CF:
            ClientCommunication.ForceSendToClient(new UnicodeMessage(serial2, mobile2.Body, MessageType.Label, num2, 3, "ENU", "", mobile2.Name));
        Label_0446:
            return false;
        }

        public static void Close()
        {
            if (!Playing)
            {
                btnClose.Enabled = btnPlay.Enabled = btnStop.Enabled = false;
                if (m_GZIn != null)
                {
                    m_GZIn.Close();
                }
                m_GZIn = null;
                tbPos.Value = tbPos.Minimum;
                lblPlay.Text = "";
                m_RPVInfo = null;
                m_Elapsed = m_CurLength = TimeSpan.Zero;
                UpdateTimeText();
            }
        }

        private static void DoLogin(PlayerData player)
        {
            PlayerData.ExternalZ = false;
            ClientCommunication.ForceSendToClient(new LoginConfirm(player));
            ClientCommunication.ForceSendToClient(new MapChange(player.Map));
            ClientCommunication.ForceSendToClient(new MapPatches(player.MapPatches));
            ClientCommunication.ForceSendToClient(new SeasonChange(player.Season, true));
            ClientCommunication.ForceSendToClient(new SupportedFeatures(player.Features));
            ClientCommunication.ForceSendToClient(new MobileUpdate(player));
            ClientCommunication.ForceSendToClient(new MobileUpdate(player));
            ClientCommunication.ForceSendToClient(new GlobalLightLevel(player.GlobalLightLevel));
            ClientCommunication.ForceSendToClient(new PersonalLightLevel(player));
            ClientCommunication.ForceSendToClient(new MobileUpdate(player));
            ClientCommunication.ForceSendToClient(new MobileIncoming(player));
            ClientCommunication.ForceSendToClient(new MobileAttributes(player));
            ClientCommunication.ForceSendToClient(new SetWarMode(player.Warmode));
            foreach (Item item in World.Items.Values)
            {
                if (item.Container == null)
                {
                    ClientCommunication.ForceSendToClient(new WorldItem(item));
                    if (item.HouseRevision != 0)
                    {
                        ClientCommunication.ForceSendToClient(new DesignStateGeneral(item));
                    }
                }
            }
            foreach (Mobile mobile in World.Mobiles.Values)
            {
                ClientCommunication.ForceSendToClient(new MobileIncoming(mobile));
            }
            ClientCommunication.ForceSendToClient(new SupportedFeatures(player.Features));
            ClientCommunication.ForceSendToClient(new MobileUpdate(player));
            ClientCommunication.ForceSendToClient(new MobileIncoming(player));
            ClientCommunication.ForceSendToClient(new MobileAttributes(player));
            ClientCommunication.ForceSendToClient(new SetWarMode(player.Warmode));
            ClientCommunication.ForceSendToClient(new MobileIncoming(player));
            ClientCommunication.ForceSendToClient(new LoginComplete());
            ClientCommunication.ForceSendToClient(new CurrentTime());
            ClientCommunication.ForceSendToClient(new SeasonChange(player.Season, true));
            ClientCommunication.ForceSendToClient(new MapChange(player.Map));
            ClientCommunication.ForceSendToClient(new MobileUpdate(player));
            ClientCommunication.ForceSendToClient(new MobileIncoming(player));
            PacketHandlers.PlayCharTime = DateTime.Now;
            ClientCommunication.BeginCalibratePosition();
        }

        private static void EndPlayback()
        {
            PlayerData data;
            m_PlayTimer = null;
            m_Playing = false;
            ClientCommunication.SetAllowDisconn(true);
            ClientCommunication.SetDeathMsg("You are dead.");
            using (BinaryReader reader = new BinaryReader(m_TempWriter.BaseStream))
            {
                reader.BaseStream.Seek(0L, SeekOrigin.Begin);
                data = World.Player = new PlayerData(reader, 4);
            }
            m_TempWriter.Close();
            data.Contains.Clear();
            World.AddMobile(data);
            DoLogin(data);
            tbPos.Enabled = btnClose.Enabled = btnPlay.Enabled = btnStop.Enabled = btnRec.Enabled = true;
            tbPos.Value = tbPos.Minimum;
            m_Elapsed = TimeSpan.Zero;
            UpdateTimeText();
            ClientCommunication.SendToClient(new MoveReject(World.Player.WalkSequence, World.Player));
            ClientCommunication.SendToServer(new ResyncReq());
            World.Player.Resync();
            ClientCommunication.RequestTitlebarUpdate();
            if (ClientCommunication.AllowBit(FeatureBit.LightFilter) && (World.Player != null))
            {
                World.Player.LocalLightLevel = 0;
                ClientCommunication.SendToClient(new GlobalLightLevel(0));
                ClientCommunication.SendToClient(new PersonalLightLevel(World.Player));
            }
        }

        private static void LoadWorldState()
        {
            int num = m_GZIn.Compressed.ReadInt32() + ((int) m_GZIn.Position);
            try
            {
                PlayerData data;
                World.Player = data = new PlayerData(m_GZIn.Compressed, m_Version);
                World.AddMobile(data);
                while (m_GZIn.Position < num)
                {
                    switch (m_GZIn.Compressed.ReadByte())
                    {
                        case 1:
                        {
                            World.AddMobile(new Mobile(m_GZIn.Compressed, m_Version));
                            continue;
                        }
                        case 0:
                            World.AddItem(new Item(m_GZIn.Compressed, m_Version));
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                new MessageDialog("Error Reading PacketVideo", true, exception.ToString()).ShowDialog(Engine.ActiveWindow);
            }
            foreach (Mobile mobile in World.Mobiles.Values)
            {
                mobile.AfterLoad();
            }
            foreach (Item item in World.Items.Values)
            {
                item.AfterLoad();
            }
        }

        public static void OnScroll()
        {
            bool running = m_PlayTimer.Running;
            TimeSpan zero = TimeSpan.Zero;
            TimeSpan span2 = TimeSpan.FromSeconds((double) tbPos.Value);
            try
            {
                if (!Playing)
                {
                    tbPos.Value = tbPos.Minimum;
                }
                else if (span2 > m_Elapsed)
                {
                    if (running)
                    {
                        m_PlayTimer.Stop();
                    }
                    PlayerData.ExternalZ = false;
                    int num = 0;
                    while ((m_Elapsed < span2) && !m_GZIn.EndOfFile)
                    {
                        if (m_GZIn.Compressed.ReadByte() == 0xff)
                        {
                            break;
                        }
                        m_GZIn.Seek(-1L, SeekOrigin.Current);
                        ClientCommunication.ProcessPlaybackData(m_GZIn.Compressed);
                        if (!m_GZIn.EndOfFile)
                        {
                            zero = TimeSpan.FromMilliseconds((double) m_GZIn.Compressed.ReadInt32());
                            m_Elapsed += zero;
                            if ((++num % 5) == 0)
                            {
                                tbPos.Value = (int) m_Elapsed.TotalSeconds;
                                Application.DoEvents();
                                Thread.Sleep(TimeSpan.FromMilliseconds(1.0));
                            }
                        }
                    }
                    try
                    {
                        tbPos.Value = (int) m_Elapsed.TotalSeconds;
                    }
                    catch
                    {
                    }
                    UpdateTimeText();
                    ClientCommunication.ForceSendToClient(new MobileUpdate(World.Player));
                    ClientCommunication.ForceSendToClient(new MobileIncoming(World.Player));
                    if (running)
                    {
                        if (!m_GZIn.EndOfFile)
                        {
                            m_PlayTimer = Assistant.Timer.DelayedCallback(zero, m_SendNext);
                            m_PlayTimer.Start();
                        }
                        else
                        {
                            Stop();
                        }
                    }
                    ClientCommunication.BeginCalibratePosition();
                }
                else
                {
                    tbPos.Value = (int) m_Elapsed.TotalSeconds;
                }
            }
            catch
            {
            }
        }

        public static void Open(string filename)
        {
            if (!Playing)
            {
                btnPlay.Enabled = btnStop.Enabled = false;
                if (m_GZIn != null)
                {
                    m_GZIn.Close();
                }
                try
                {
                    m_GZIn = new GZBlockIn(filename);
                    m_Version = m_GZIn.Raw.ReadByte();
                    if (m_Version > 4)
                    {
                        m_GZIn.Close();
                        m_GZIn = null;
                        MessageBox.Show(Engine.MainWindow, Language.GetString(LocString.WrongVer), "Version Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    else
                    {
                        m_GZIn.IsCompressed = m_Version > 1;
                        byte[] buffer = m_GZIn.Raw.ReadBytes(0x10);
                        DateTime time = DateTime.FromFileTime(m_GZIn.Raw.ReadInt64());
                        TimeSpan span = TimeSpan.FromMilliseconds((double) m_GZIn.Raw.ReadInt32());
                        string str = m_GZIn.Compressed.ReadString();
                        string str2 = m_GZIn.Compressed.ReadString();
                        IPAddress any = IPAddress.Any;
                        try
                        {
                            if (m_Version > 1)
                            {
                                any = new IPAddress((long) m_GZIn.Compressed.ReadUInt32());
                            }
                        }
                        catch
                        {
                        }
                        m_StartPos = (int) m_GZIn.Position;
                        long position = m_GZIn.RawStream.Position;
                        m_GZIn.RawStream.Seek(0x11L, SeekOrigin.Begin);
                        using (MD5 md = MD5.Create())
                        {
                            byte[] buffer2 = md.ComputeHash(m_GZIn.RawStream);
                            for (int i = 0; i < buffer2.Length; i++)
                            {
                                if (buffer2[i] != buffer[i])
                                {
                                    m_GZIn.Close();
                                    m_GZIn = null;
                                    MessageBox.Show(Engine.MainWindow, Language.GetString(LocString.VideoCorrupt), "Damaged File", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                    return;
                                }
                            }
                        }
                        m_GZIn.RawStream.Seek(position, SeekOrigin.Begin);
                        m_CurLength = span;
                        m_Elapsed = TimeSpan.Zero;
                        UpdateTimeText();
                        m_RPVInfo = lblPlay.Text = string.Format("File: {0}\nLength: {1} ({2})\nDate: {3}\nRecorded by \"{4}\" on \"{5}\" ({6})\n", new object[] { Path.GetFileName(filename), Utility.FormatTime((int) span.TotalSeconds), Utility.FormatSize(m_GZIn.RawStream.Length), time.ToString("M-dd-yy @ h:mmtt"), str, str2, any });
                        btnClose.Enabled = btnPlay.Enabled = btnStop.Enabled = true;
                        tbPos.Maximum = (int) span.TotalSeconds;
                        tbPos.Minimum = 0;
                    }
                }
                catch (Exception exception)
                {
                    if (exception is FileNotFoundException)
                    {
                        MessageBox.Show(Engine.MainWindow, exception.Message, "File not found.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    else
                    {
                        Engine.LogCrash(exception);
                        MessageBox.Show(Engine.MainWindow, Language.GetString(LocString.ReadError), "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    m_GZIn.Close();
                    m_GZIn = null;
                }
            }
        }

        public static void Pause()
        {
            if (Playing)
            {
                if (!m_PlayTimer.Running)
                {
                    SendNextPacket();
                    btnPlay.Text = "Pause";
                }
                else
                {
                    m_PlayTimer.Stop();
                    btnPlay.Text = "Play";
                }
            }
        }

        public static void Play()
        {
            if ((!m_Recording && !Playing) && (m_GZIn != null))
            {
                if (World.Player == null)
                {
                    MessageBox.Show(Engine.MainWindow, "You must be logged in to ANY shard to play a packet video.", "Must Log in", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    btnPlay.Enabled = btnStop.Enabled = btnClose.Enabled = btnRec.Enabled = false;
                    btnPlay.Text = "Pause";
                    m_TempWriter = new BinaryWriter(new MemoryStream());
                    World.Player.SaveState(m_TempWriter);
                    m_Playing = true;
                    ClientCommunication.SetAllowDisconn(false);
                    ClientCommunication.BringToFront(ClientCommunication.FindUOWindow());
                    ClientCommunication.SetDeathMsg("Playing...");
                    ClientCommunication.ForceSendToClient(new DeathStatus(true));
                    RemoveAll();
                    m_GZIn.Seek((long) m_StartPos, SeekOrigin.Begin);
                    LoadWorldState();
                    m_PlayTimer = Assistant.Timer.DelayedCallback(FadeDelay, m_BeginPlay);
                    m_PlayTimer.Start();
                    tbPos.Value = tbPos.Minimum;
                    m_Elapsed = TimeSpan.Zero;
                    UpdateTimeText();
                    ClientCommunication.RequestTitlebarUpdate();
                }
            }
        }

        public static void Record()
        {
            if ((!m_Recording && !Playing) && (World.Player != null))
            {
                string str;
                btnRec.Text = "Stop Recording (PV)";
                btnPlay.Enabled = btnStop.Enabled = false;
                m_HouseDataWritten.Clear();
                string name = "Unknown";
                string dir = Config.GetString("RecFolder");
                if (World.Player != null)
                {
                    name = World.Player.Name;
                }
                if (((name == null) || (name.Trim() == "")) || (name.IndexOfAny(Path.InvalidPathChars) != -1))
                {
                    name = "Unknown";
                }
                name = string.Format("{0}_{1}", name, DateTime.Now.ToString("M-d_HH.mm"));
                try
                {
                    Engine.EnsureDirectory(dir);
                }
                catch
                {
                    try
                    {
                        dir = Engine.GetDirectory("Videos");
                        Config.SetProperty("RecFolder", dir);
                    }
                    catch
                    {
                        dir = "";
                    }
                }
                int num = 0;
                do
                {
                    str = Path.Combine(dir, string.Format("{0}{1}.rpv", name, (num != 0) ? num.ToString() : ""));
                    num--;
                }
                while (System.IO.File.Exists(str));
                m_Recording = true;
                m_StartTime = m_LastTime = DateTime.Now;
                try
                {
                    byte[] addressBytes;
                    m_GZOut = new GZBlockOut(str, 0x800);
                    m_GZOut.Raw.Write((byte) 4);
                    m_GZOut.Raw.Seek(0x10, SeekOrigin.Current);
                    m_GZOut.Raw.Write(m_StartTime.ToFileTime());
                    m_GZOut.Raw.Write(0);
                    m_GZOut.BufferAll = true;
                    m_GZOut.Compressed.Write(World.Player.Name);
                    m_GZOut.Compressed.Write(World.ShardName);
                    try
                    {
                        addressBytes = ClientCommunication.LastConnection.GetAddressBytes();
                    }
                    catch
                    {
                        addressBytes = new byte[4];
                    }
                    m_GZOut.Compressed.Write(addressBytes, 0, 4);
                    SaveWorldState();
                    m_GZOut.BufferAll = false;
                    m_GZOut.Flush();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(Engine.MainWindow, Language.GetString(LocString.RecError), "Rec Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    Engine.LogCrash(exception);
                }
            }
        }

        private static void RemoveAll()
        {
            World.Mobiles.Clear();
            World.Items.Clear();
            ClientCommunication.OnLogout();
        }

        private static void SaveWorldState()
        {
            long position = m_GZOut.Position;
            m_GZOut.Compressed.Write(0);
            World.Player.SaveState(m_GZOut.Compressed);
            foreach (Mobile mobile in World.Mobiles.Values)
            {
                if (mobile != World.Player)
                {
                    m_GZOut.Compressed.Write((byte) 1);
                    mobile.SaveState(m_GZOut.Compressed);
                }
            }
            foreach (Item item in World.Items.Values)
            {
                if (!(item.Container is Item))
                {
                    m_GZOut.Compressed.Write((byte) 0);
                    item.SaveState(m_GZOut.Compressed);
                    m_HouseDataWritten[item.Serial] = true;
                }
            }
            long num2 = m_GZOut.Position;
            m_GZOut.Seek((long) ((int) position), SeekOrigin.Begin);
            m_GZOut.Compressed.Write((int) (num2 - (position + 4L)));
            m_GZOut.Seek(0L, SeekOrigin.End);
        }

        private static void SendNextPacket()
        {
            if (Playing)
            {
                if (World.Player == null)
                {
                    btnPlay.Text = "Play";
                    tbPos.Enabled = btnClose.Enabled = btnPlay.Enabled = btnStop.Enabled = btnRec.Enabled = true;
                    tbPos.Value = tbPos.Minimum;
                    if ((m_PlayTimer != null) && m_PlayTimer.Running)
                    {
                        m_PlayTimer.Stop();
                    }
                    if ((m_ScrollTimer != null) && m_ScrollTimer.Running)
                    {
                        m_ScrollTimer.Stop();
                    }
                }
                else
                {
                    int num = 0;
                    int num2 = 0;
                    do
                    {
                        if (m_GZIn.Compressed.ReadByte() == 0xff)
                        {
                            break;
                        }
                        m_GZIn.Seek(-1L, SeekOrigin.Current);
                        ClientCommunication.ProcessPlaybackData(m_GZIn.Compressed);
                        if (!m_GZIn.EndOfFile)
                        {
                            num2 += num = m_GZIn.Compressed.ReadInt32();
                        }
                    }
                    while ((((num2 * SpeedScalar()) < 2.0) || ((num * SpeedScalar()) < 2.0)) && !m_GZIn.EndOfFile);
                    m_Elapsed += TimeSpan.FromMilliseconds((double) num2);
                    UpdateTimeText();
                    if (!m_GZIn.EndOfFile)
                    {
                        m_PlayTimer = Assistant.Timer.DelayedCallback(TimeSpan.FromMilliseconds((num * SpeedScalar()) * 0.75), m_SendNext);
                        m_PlayTimer.Start();
                    }
                    else
                    {
                        Stop();
                    }
                }
            }
        }

        public static bool ServerPacket(Packet p)
        {
            if (Playing)
            {
                return false;
            }
            if (m_Recording && (p != null))
            {
                if (World.Player == null)
                {
                    Stop();
                    return true;
                }
                switch (p.PacketID)
                {
                    case 0x6c:
                    case 0x7c:
                    case 0x21:
                    case 0x27:
                    case 0x88:
                    case 0xb2:
                    case 0xba:
                        return true;

                    case 0x22:
                    {
                        byte seq = p.ReadByte();
                        if (World.Player.HasWalkEntry(seq))
                        {
                            WritePacket(new ForceWalk(World.Player.GetMoveEntry(seq).Dir & Direction.Up));
                        }
                        return true;
                    }
                    case 0xbf:
                        switch (p.ReadInt16())
                        {
                            case 6:
                                switch (p.ReadByte())
                                {
                                    case 3:
                                    case 4:
                                    {
                                        Mobile mobile = World.FindMobile(p.ReadUInt32());
                                        string str = p.ReadUnicodeStringSafe();
                                        string text = string.Format("[{0}]: {1}", (((mobile != null) && (mobile.Name != null)) && (mobile.Name.Length > 0)) ? mobile.Name : "Party", str);
                                        WritePacket(new UnicodeMessage(Serial.MinusOne, 0, MessageType.System, 0x3b2, 3, "ENU", "Party", text));
                                        break;
                                    }
                                }
                                return true;

                            case 0x1d:
                            {
                                Item item = World.FindItem(p.ReadUInt32());
                                if (item != null)
                                {
                                    item.HouseRevision = p.ReadInt32();
                                    if (m_HouseDataWritten[item.Serial] == null)
                                    {
                                        if (item.HousePacket == null)
                                        {
                                            item.MakeHousePacket();
                                        }
                                        if (item.HousePacket != null)
                                        {
                                            m_HouseDataWritten[item.Serial] = true;
                                            WritePacket(new Packet(item.HousePacket, item.HousePacket.Length, true));
                                            return true;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        break;

                    case 0xd8:
                    {
                        p.ReadByte();
                        p.ReadByte();
                        Serial serial = p.ReadUInt32();
                        m_HouseDataWritten[serial] = true;
                        break;
                    }
                }
                WritePacket(p);
            }
            return true;
        }

        public static void SetControls(Label play, Button bRec, Button bPlay, Button stop, Button close, TrackBar pos, Label time)
        {
            lblPlay = play;
            lblTime = time;
            btnRec = bRec;
            btnPlay = bPlay;
            btnStop = stop;
            btnClose = close;
            tbPos = pos;
        }

        public static void SetSpeed(int speed)
        {
            m_PlaySpeed = speed;
        }

        public static double SpeedScalar()
        {
            switch (m_PlaySpeed)
            {
                case -2:
                    return 4.0;

                case -1:
                    return 2.0;

                case 1:
                    return 0.5;

                case 2:
                    return 0.25;
            }
            return 1.0;
        }

        public static void Stop()
        {
            if (m_Recording)
            {
                byte[] buffer;
                TimeSpan span = (TimeSpan) (DateTime.Now - m_LastTime);
                m_GZOut.Compressed.Write((int) span.TotalMilliseconds);
                m_GZOut.Compressed.Write((byte) 0xff);
                m_GZOut.ForceFlush();
                m_GZOut.BufferAll = true;
                m_GZOut.RawStream.Seek(0x19L, SeekOrigin.Begin);
                span = (TimeSpan) (DateTime.Now - m_StartTime);
                m_GZOut.Raw.Write((int) span.TotalMilliseconds);
                m_GZOut.RawStream.Seek(0x11L, SeekOrigin.Begin);
                using (MD5 md = MD5.Create())
                {
                    buffer = md.ComputeHash(m_GZOut.RawStream);
                }
                m_GZOut.RawStream.Seek(1L, SeekOrigin.Begin);
                m_GZOut.Raw.Write(buffer);
                m_GZOut.RawStream.Flush();
                m_GZOut.Close();
                m_GZOut = null;
                m_Recording = false;
                btnRec.Text = "Record PacketVideo";
                btnPlay.Enabled = btnStop.Enabled = true;
            }
            else if (Playing)
            {
                ClientCommunication.SetDeathMsg(Language.GetString((LocString) (0x546 + Utility.Random(10))));
                ClientCommunication.ForceSendToClient(new DeathStatus(true));
                RemoveAll();
                if ((m_PlayTimer != null) && m_PlayTimer.Running)
                {
                    m_PlayTimer.Stop();
                }
                if (m_ScrollTimer != null)
                {
                    m_ScrollTimer.Stop();
                }
                m_PlayTimer = Assistant.Timer.DelayedCallback(FadeDelay, m_EndPlay);
                m_PlayTimer.Start();
                btnPlay.Text = "Play";
                btnClose.Enabled = tbPos.Enabled = btnPlay.Enabled = btnStop.Enabled = false;
            }
        }

        private static void UpdateTimeText()
        {
            lblTime.Text = ElapsedString;
            ClientCommunication.RequestTitlebarUpdate();
        }

        private static void WritePacket(Packet p)
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - m_LastTime);
            int totalMilliseconds = (int) span.TotalMilliseconds;
            m_LastTime = DateTime.Now;
            m_GZOut.Compressed.Write(totalMilliseconds);
            m_GZOut.Compressed.Write(p.Compile());
        }

        public static string CurrentOpenedInfo
        {
            get
            {
                return m_RPVInfo;
            }
        }

        public static string ElapsedString
        {
            get
            {
                return string.Format("{0:00}:{1:00}/{2:00}:{3:00}", new object[] { (int) m_Elapsed.TotalMinutes, ((int) m_Elapsed.TotalSeconds) % 60, (int) m_CurLength.TotalMinutes, ((int) m_CurLength.TotalSeconds) % 60 });
            }
        }

        public static bool Playing
        {
            get
            {
                return m_Playing;
            }
        }

        public static bool Recording
        {
            get
            {
                return m_Recording;
            }
        }

        private class ScrollTimer : Assistant.Timer
        {
            private DateTime m_LastPing;

            public ScrollTimer() : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
            {
            }

            protected override void OnTick()
            {
                if (PacketPlayer.Playing)
                {
                    TimeSpan span = PacketPlayer.m_PlayTimer.Delay - PacketPlayer.m_PlayTimer.TimeUntilTick;
                    int maximum = ((int) PacketPlayer.m_Elapsed.TotalSeconds) + ((int) span.TotalSeconds);
                    if (maximum > PacketPlayer.tbPos.Maximum)
                    {
                        maximum = PacketPlayer.tbPos.Maximum;
                    }
                    else if (maximum < PacketPlayer.tbPos.Minimum)
                    {
                        maximum = PacketPlayer.tbPos.Minimum;
                    }
                    PacketPlayer.UpdateTimeText();
                    PacketPlayer.tbPos.Value = maximum;
                    if ((DateTime.Now - this.m_LastPing) >= TimeSpan.FromMinutes(1.0))
                    {
                        ClientCommunication.ForceSendToServer(new PingPacket(0));
                        this.m_LastPing = DateTime.Now;
                    }
                }
                else
                {
                    base.Stop();
                }
            }
        }
    }
}
*/
