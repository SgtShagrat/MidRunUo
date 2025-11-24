using System;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Engines.XmlPoints
{
    public class PointsGump : Gump
    {
        private readonly XmlPointsAttach m_Attachment;
        private readonly Mobile m_Target;
        private readonly string m_Text;

        public PointsGump(XmlPointsAttach a, Mobile from, Mobile target, string text)
            : base(0, 0)
        {
            if (target == null || a == null)
                return;

            m_Attachment = a;
            m_Target = target;
            m_Text = text;

            // prepare the page
            AddPage(0);

            if (from == target)
            {
                AddBackground(0, 0, 440, 295, 5054);
                AddAlphaRegion(0, 0, 440, 295);
            }
            else
            {
                AddBackground(0, 0, 440, 190, 5054);
                AddAlphaRegion(0, 0, 440, 190);
            }
            AddLabel(20, 2, 55, String.Format(XmlPointsAttach.GetText(from, 200224), target.Name));
                // "Points Standing for {0}"

            // 1 on 1 duel status
            if (a.Challenger != null)
            {
                int challengehue = 68;

                if (a.MCancelTimer != null && a.MCancelTimer.Running)
                    challengehue = 33;
                // also check the challenger timer to see if he is cancelling
                var ca = (XmlPointsAttach) XmlAttach.FindAttachment(a.Challenger, typeof (XmlPointsAttach));
                if (ca != null && !ca.Deleted)
                {
                    if ((ca.MCancelTimer != null && ca.MCancelTimer.Running) ||
                        (ca.ChallengeGame != null && ca.ChallengeGame.ChallengeBeingCancelled))
                        challengehue = 33;
                }


                AddLabel(20, 143, challengehue, String.Format(XmlPointsAttach.GetText(from, 200225), a.Challenger.Name));
                // "Currently challenging {0}"
            }
            else
                // challenge game status
                if (a.ChallengeGame != null && !a.ChallengeGame.Deleted)
                {
                    AddLabel(50, 143, 68, String.Format("{0}", a.ChallengeGame.ChallengeName));
                    // add the info button that will open the game gump
                    AddButton(23, 143, 0x5689, 0x568A, 310, GumpButtonType.Reply, 0);
                }

            AddHtml(20, 20, 400, 120, text, true, true);

            int x1 = 20;
            int x2 = 150;
            int x3 = 290;

            if (from == target)
            {
                // add the see kills checkbox
                AddLabel(x1 + 30, 165, 55, a.Text(200226)); // "See kills"
                AddButton(x1, 165, (a.ReceiveBroadcasts ? 0xD3 : 0xD2), (a.ReceiveBroadcasts ? 0xD2 : 0xD3), 100,
                          GumpButtonType.Reply, 0);

                // add the broadcast kills checkbox
                AddLabel(x2 + 30, 165, 55, a.Text(200227)); // "Broadcast kills"
                AddButton(x2, 165, (a.Broadcast ? 0xD3 : 0xD2), (a.Broadcast ? 0xD2 : 0xD3), 200, GumpButtonType.Reply,
                          0);

                // add the topplayers button
                AddLabel(x3 + 30, 165, 55, a.Text(200228)); // "Top players"
                AddButton(x3, 165, 0xFAB, 0xFAD, 300, GumpButtonType.Reply, 0);

                // add the challenge button
                AddLabel(x1 + 30, 190, 55, a.Text(200229)); // "Challenge"
                AddButton(x1, 190, 0xFAB, 0xFAD, 400, GumpButtonType.Reply, 0);

                // add the last man standing challenge button
                AddLabel(x2 + 30, 190, 55, a.Text(200230)); // "LMS"
                AddButton(x2, 190, 0xFAB, 0xFAD, 401, GumpButtonType.Reply, 0);

                // add the deathmatch challenge button
                AddLabel(x3 + 30, 190, 55, a.Text(200231)); // "Deathmatch"
                AddButton(x3, 190, 0xFAB, 0xFAD, 403, GumpButtonType.Reply, 0);

                // add the kingofthehill challenge button
                AddLabel(x1 + 30, 215, 55, a.Text(200232)); // "KotH"
                AddButton(x1, 215, 0xFAB, 0xFAD, 404, GumpButtonType.Reply, 0);

                // add the deathball challenge button
                AddLabel(x2 + 30, 215, 55, a.Text(200233)); // "DeathBall"
                AddButton(x2, 215, 0xFAB, 0xFAD, 405, GumpButtonType.Reply, 0);

                // add the teamlms challenge button
                AddLabel(x3 + 30, 215, 55, a.Text(200234)); // "Team LMS"
                AddButton(x3, 215, 0xFAB, 0xFAD, 406, GumpButtonType.Reply, 0);

                // add the team deathmatch challenge button
                AddLabel(x1 + 30, 240, 55, a.Text(200235)); // "Team DMatch"
                AddButton(x1, 240, 0xFAB, 0xFAD, 407, GumpButtonType.Reply, 0);

                // add the team deathball challenge button
                AddLabel(x2 + 30, 240, 55, a.Text(200236)); // "Team DBall"
                AddButton(x2, 240, 0xFAB, 0xFAD, 408, GumpButtonType.Reply, 0);

                // add the team KotH challenge button
                AddLabel(x3 + 30, 240, 55, a.Text(200237)); // "Team KotH"
                AddButton(x3, 240, 0xFAB, 0xFAD, 409, GumpButtonType.Reply, 0);

                // add the CTF challenge button
                AddLabel(x1 + 30, 265, 55, a.Text(200238)); // "CTF"
                AddButton(x1, 265, 0xFAB, 0xFAD, 410, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Attachment == null || state == null || state.Mobile == null || info == null)
                return;

            switch (info.ButtonID)
            {
                case 100:
                    // toggle see kills
                    m_Attachment.ReceiveBroadcasts = !m_Attachment.ReceiveBroadcasts;

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 200:
                    // toggle broadcast my kills
                    m_Attachment.Broadcast = !m_Attachment.Broadcast;

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 300:
                    // top players
                    state.Mobile.CloseGump(typeof (TopPlayersGump));
                    state.Mobile.SendGump(new TopPlayersGump(m_Attachment));

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 310:
                    // Challenge game info
                    if (m_Attachment.ChallengeGame != null && !m_Attachment.ChallengeGame.Deleted)
                        m_Attachment.ChallengeGame.OnDoubleClick(state.Mobile);

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 400:
                    // 1 on 1 challenge duel
                    state.Mobile.Target = new ChallengeTarget(state.Mobile);

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 401:
                    // last man standing
                    BaseChallengeGame.DoSetupChallenge(state.Mobile, 100302, typeof (LastManStandingGauntlet));

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 403:
                    // deathmatch challenge
                    BaseChallengeGame.DoSetupChallenge(state.Mobile, 100400, typeof (DeathmatchGauntlet));

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 404:
                    // kingofthehill challenge
                    BaseChallengeGame.DoSetupChallenge(state.Mobile, 100410, typeof (KingOfTheHillGauntlet));

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 405:
                    // deathball challenge
                    BaseChallengeGame.DoSetupChallenge(state.Mobile, 100411, typeof (DeathBallGauntlet));

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 406:
                    // team lms challenge
                    BaseChallengeGame.DoSetupChallenge(state.Mobile, 100413, typeof (TeamLMSGauntlet));

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 407:
                    // team deathmatch challenge
                    BaseChallengeGame.DoSetupChallenge(state.Mobile, 100415, typeof (TeamDeathmatchGauntlet));

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 408:
                    // team deathball challenge
                    BaseChallengeGame.DoSetupChallenge(state.Mobile, 100416, typeof (TeamDeathballGauntlet));

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 409:
                    // team KotH challenge
                    BaseChallengeGame.DoSetupChallenge(state.Mobile, 100417, typeof (TeamKotHGauntlet));

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
                case 410:
                    // CTF challenge
                    BaseChallengeGame.DoSetupChallenge(state.Mobile, 100418, typeof (CTFGauntlet));

                    state.Mobile.SendGump(new PointsGump(m_Attachment, state.Mobile, m_Target, m_Text));
                    break;
            }
        }
    }
}