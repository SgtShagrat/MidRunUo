using System;

using Midgard.Engines.BountySystem;
using Midgard.Mobiles;

using Server;
using Server.Items;
using Server.Mobiles;

using Core = Midgard.Engines.BountySystem.Core;

namespace Midgard.Engines.MidgardTownSystem
{
    public abstract class BaseTownSoldier : BaseHuman
    {
        public override SpeechFragment PersonalFragmentObj
        {
            get { return PersonalFragment.Guard; }
        }

        public MidgardTowns Town { get; set; }

        public override bool GuardImmune
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override bool IsScaryToPets
        {
            get { return true; }
        }

        public override bool CanRummageCorpses
        {
            get { return false; }
        }

        public TownSystem System
        {
            get { return TownSystem.Find( Town ); }
        }

        protected BaseTownSoldier( bool isFemale, bool mounted )
            : base( isFemale, mounted )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( System != null )
            {
                list.Add( "Town Guard of {0}", System.Definition.TownName );
            }
        }

        public override bool OnDragDrop( Mobile from, Item item )
        {
            bool isHandled = false;

            // Check if the item being dropped is a head
            if( item is Head )
            {
                Head head = (Head)item;

                Say( 500670 ); //  Ah, a head!  Let me check to see if there is a bounty on this.

                isHandled = HandleNPCBountyHead( head, from ); // Check if the head was from a Murderer (NPC)

                if( !isHandled )
                {
                    isHandled = HandlePlayerBountyHead( head, from ); // Check if the head was from a Player Murderer
                }
            }
            else
            {
                Say( 500200 ); // I have no need for that.
            }

            return isHandled;
        }

        private bool HandleNPCBountyHead( Head head, Mobile from )
        {
            bool isHandled = false;

            if( head.Owner is Murderer )
            {
                // Get the bounty amount
                Murderer murderer = (Murderer)head.Owner;

                // Check that the player does not have negative karma or is a criminal
                if( from.Karma >= -25 && from.Kills < 5 && !from.Criminal )
                {
                    // Check if there is a bounty on the head at all
                    if( murderer.Bounty > 0 )
                    {
                        isHandled = true;
                        Say( 1042855, String.Format( "{0}\t{1}\t", murderer.Name, murderer.Bounty ) ); // The bounty on ~1_PLAYER_NAME~ was ~2_AMOUNT~ gold, and has been credited to your account.
                        from.BankBox.DropItem( new Gold( murderer.Bounty ) );

                        // Give the karm they lost back + an amount equal to the bounty
                        from.Karma += murderer.Bounty;
                        murderer.Bounty = 0;

                        // Remove the quest if any
                        from.SendLocalizedMessage( 1019060 ); // You have gained some karma. 
                    }
                    else
                    {
                        Say( 1042854, murderer.Name ); // There was no bounty on ~1_PLAYER_NAME~.
                    }
                }
                else
                {
                    Say( 500542 ); // We only accept bounty hunting from honorable folk! Away with thee!
                }
            }
            /*
            else
            {
                Say( 500660 ); // If this were the head of a murderer, I would check for a bounty.
            }
            */

            return isHandled;
        }

        private bool HandlePlayerBountyHead( Head head, Mobile from )
        {
            bool isHandled = false;

            if( !BountySystem.Config.Enabled )
            {
                return false;
            }

            BountyBoardEntry entry;
            Mobile murderer = head.Owner;

            // Check if the head is a valid head
            if( murderer != null && head.Killer != null )
            {
                // check if there is a bounty and if the murderer can claim it
                bool canClaim;
                if( Core.HasBounty( from, murderer, out entry, out canClaim ) )
                {
                    // check if the claimer killed the murderer
                    if( head.Killer == from )
                    {
                        //check age of head
                        if( head.CreationTime >= ( entry.ExpireTime - BountySystem.Config.DefaultDecayRate ) )
                        {
                            // Check that the player does not have negative karma or is a criminal
                            if( from.Karma >= -25 && from.Kills < 5 && !from.Criminal )
                            {
                                if( from is PlayerMobile && ( (PlayerMobile)from ).NpcGuild != NpcGuild.ThievesGuild )
                                {
                                    if( canClaim )
                                    {
                                        if( !entry.Expired )
                                        {
                                            isHandled = true;
                                            Say( 1042855, String.Format( "{0}\t{1}\t", murderer.Name, entry.Price ) );

                                            // The bounty on ~1_PLAYER_NAME~ was ~2_AMOUNT~ gold, and has been credited to your account.
                                            from.BankBox.DropItem( new Gold( entry.Price ) );

                                            // Give the karma they lost back + an amount equal to the bounty
                                            from.Karma += entry.Price;
                                            BountySystemLog.WriteInfo( entry, BountySystemLog.LogType.HeadGiven, from );

                                            Core.RemoveEntry( entry, false );
                                            head.Delete();
                                        }
                                        else
                                        {
                                            Say( "The bounty on this murderer has expired." );
                                            Core.RemoveEntry( entry, false );
                                        }
                                    }
                                    else
                                    {
                                        Say( "The bounty owner did not approve a reward to you!" );
                                    }
                                }
                                else
                                {
                                    Say( "You are a thief and i'm not a stupid. Go away scum!" );
                                    from.CriminalAction( false );
                                }
                            }
                            else
                            {
                                Say( 500542 ); // We only accept bounty hunting from honorable folk! Away with thee!
                            }
                        }
                        else
                        {
                            Say( "Their is a bounty on this murderer but this head is from long ago!" );
                        }
                    }
                    else
                    {
                        Say( 500543 ); // I had heard this scum was taken care of...but not by you
                    }
                }
                else
                {
                    Say( 1042854, murderer.Name ); // There was no bounty on ~1_PLAYER_NAME~.						
                }
            }
            else
            {
                Say( 500660 ); // If this were the head of a murderer, I would check for a bounty.
            }

            return isHandled;
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            // Word 'guard' OR 'guards' was said
            // Turn to face the player who called out
            if( e.HasKeyword( 0x0007 ) )
            {
                Direction = GetDirectionTo( e.Mobile.Location );
            }

            base.OnSpeech( e );
        }

        private static readonly string[] m_Warns = new string[]
                                          {
                                              "Don't do anything foolish.",
                                              "We'll be keeping an eye on thee.",
                                              "I know thy type. Break not the laws, or else suffer punishment!",
                                              "Thou hast a suspicious look about thee.",
                                              "I trust thee not, nor should anyone. Thou art a ruffian.",

                                              "Faugh, another scoundrel to keep an eye on!",
                                              "I have heard of thee--keep thee to the straight and narrow whilst here!",
                                              "If I catch thee breaking laws, thy neck will be on the line.",
                                              "Thou hast an unsavory reputation--best that thou dost not prove that thou earnst it.",
                                              "Thou art known as a criminal. Do not draw the attention of the guards with thy deeds, or risk death!",

                                              "Mine eye is on thee, scoundrel. Break not the law.", "Thou art a scofflaw, but while here thou SHALT obey the laws, or suffer.", 
                                              "Look thee who crawled out from under a rock! Well, thou shalt not do anything illegal whilst I am watching thee!", 
                                              "The guards well know thee and thy nefarious ways! Be sure to behave thyself!", 
                                              "Scum! Stay not overlong here, for thou art not welcome!",
                                              "I hope that thou art merely visiting, for we mislike knaves mingling with our citizens.",

                                              "Thou shouldst leave this city before any decide to lynch thee.",
                                              "I should arrest thee now, before thou dost something terrible. Thou art well known as a rogue of the worst kind."
                                          };

        public int AutoSpeakRange { get { return 10; } }
        public TimeSpan SpeakDelay { get { return TimeSpan.FromSeconds( 10 ); } }

        private DateTime m_Spoken;

        public virtual void Warn( Mobile to )
        {
            int level = Server.Misc.Titles.GetKarmaLevel( to );
            if( level <= 4 )
            {
                int index = Utility.RandomMinMax( 0x00, 0x0C ) + level;
                if( index > -1 && index < m_Warns.Length )
                    SayTo( to, true, m_Warns[ index ] );
            }
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            if( m.Alive && !m.Hidden && m.Player )
            {
                int range = AutoSpeakRange;

                if( range >= 0 && InRange( m, range ) && !InRange( oldLocation, range ) && DateTime.Now >= m_Spoken + SpeakDelay )
                {
                    if( m.Karma < -100 )
                        Warn( m );

                    m_Spoken = DateTime.Now;
                }
            }
        }

        public BaseTownSoldier( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( (int)Town );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Town = (MidgardTowns)( reader.ReadInt() );
        }
        #endregion
    }
}