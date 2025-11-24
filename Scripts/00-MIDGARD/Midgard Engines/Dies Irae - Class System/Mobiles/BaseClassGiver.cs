/***************************************************************************
 *                               BaseClassGiver.cs
 *
 *   revision             : 03 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public abstract class BaseClassGiver : BaseCreature, IMidgardQuester
    {
        private const int ListenRange = 12;

        protected BaseClassGiver( string title )
            : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4 )
        {
            Title = title;
        }

        public BaseClassGiver( Serial serial )
            : base( serial )
        {
        }

        public virtual string Greetings
        {
            get { return "Greetings. Do you want to join my group?"; }
        }

        public virtual string JoinQuestion
        {
            get { return "Do you want to join?"; }
        }

        public abstract Item Book { get; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public abstract ClassSystem System { get; }

        public override bool DisallowAllMoves
        {
            get { return true; }
        }

        public override bool ClickTitle
        {
            get { return false; }
        }

        public override bool CanTeach
        {
            get { return false; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override bool HandlesOnSpeech( Mobile from )
        {
            if( InRange( from, ListenRange ) )
                return true;

            return base.HandlesOnSpeech( from );
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            Mobile from = e.Mobile;

            if( !e.Handled && from is Midgard2PlayerMobile && from.InRange( Location, 2 ) )
            {
                if( WasNamed( e.Speech ) )
                {
                    SpellHelper.Turn( this, from );

                    if( e.HasKeyword( 0x0004 ) ) // *join* | *member*
                    {
                        ClassPlayerState state = ClassPlayerState.Find( from );
                        if( state != null )
                        {
                            if( state.ClassSystem == System )
                                SayTo( from, ( from.Language == "ITA" ? "Sei già un membro di questa congrega!" : "Thou art already a member of this congregation!" ) );
                            else
                                SayTo( from, ( from.Language == "ITA" ? "Devi lasciare la tua precedente classe." : "Thou must resign from thy other congregation first." ) );
                            return;
                        }

                        if( System != null && System.IsCandidate( from ) )
                            SayTo( from, "Thou art already a candidate for my congregation!" );
                        else if( System != null && System.IsEligible( from ) )
                        {
                            SayTo( from, Greetings );
                            from.SendGump( new ConfirmJoinGump( from, System, this ) );
                        }
                        else
                        {
                            if( System != null )
                                SayTo( from, System.IsEligibleString( @from ) );
                        }

                        e.Handled = true;
                    }
                    else if( e.HasKeyword( 0x0005 ) ) // *resign* | *quit*
                    {
                        ClassPlayerState state = ClassPlayerState.Find( from );
                        if( state == null || state.ClassSystem != System )
                        {
                            SayTo( from, ( from.Language == "ITA" ? "Non appartieni alla mia congrega!" : "Thou dost not belong to my congregation!" ) );
                        }
                        else
                        {
                            SayTo( from, 501054 ); // I accept thy resignation.
                            ClassSystemCommands.DoPlayerReset( from );
                        }

                        e.Handled = true;
                    }
                    else if( e.Speech.Equals( "Please hide my title." ) )
                    {
                        ClassPlayerState state = ClassPlayerState.Find( from );
                        if( state == null || state.ClassSystem != System )
                            SayTo( from, "Thou dost not belong to my congregation!" );
                        else
                        {
                            SayTo( from, "I will hide your title..." );
                            state.DisplayClassStatus = false;
                        }

                        e.Handled = true;
                    }
                    else if( e.Speech.Equals( "Please give me my title." ) )
                    {
                        ClassPlayerState state = ClassPlayerState.Find( from );
                        if( state == null || state.ClassSystem != System )
                            SayTo( from, "Thou dost not belong to my congregation!" );
                        else
                        {
                            SayTo( from, "Your title is know to other people." );
                            state.DisplayClassStatus = true;
                        }

                        e.Handled = true;
                    }
                }
                // The ritual books are public, not movable and are no longer given to players
                /*else if( Insensitive.Compare( e.Speech, "Please, give me a ritual book" ) == 0 )
                            {
                                ClassPlayerState state = pm.ClassState;

                                if( state == null || state.ClassSystem.Definition.Class != Class )
                                {
                                    SayTo( from, "Thou dost not belong to my congregation!" );
                                    return;
                                }

                                if( BaseClassPowersBook.HasClassBook( from ) )
                                {
                                    Item oldBook = BaseClassPowersBook.FindClassBook( from );
                                    if( oldBook != null && !oldBook.Deleted )
                                        oldBook.Delete();

                                    SayTo( from, "Your old book is now dust!" );
                                }

                                Item book = Book;

                                if( from.PlaceInBackpack( book ) )
                                {
                                    Say( "I see you are in need of a precious book. Here you go." );
                                    from.AddToBackpack( book );

                                    if( book is BaseClassPowersBook )
                                    {
                                        ( (BaseClassPowersBook)book ).Owner = from;
                                        ( (BaseClassPowersBook)book ).RegisterBook();
                                    }
                                }
                                else
                                {
                                    from.SendLocalizedMessage( 502868 ); // Your backpack is too full.
                                    book.Delete();
                                }

                                e.Handled = true;
                            }
                             */
            }

            base.OnSpeech( e );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.AccessLevel > AccessLevel.GameMaster )
                from.SendGump( new ClassCandidateApprovalGump( this, System, from ) );

            base.OnDoubleClick( from );
        }

        public virtual void EndJoin( Mobile joiner, bool join )
        {
            if( join )
                System.AddCandidate( joiner );
        }

        public virtual int GetRandomHue()
        {
            switch( Utility.Random( 5 ) )
            {
                default:
                    return Utility.RandomBlueHue();
                case 1:
                    return Utility.RandomGreenHue();
                case 2:
                    return Utility.RandomRedHue();
                case 3:
                    return Utility.RandomYellowHue();
                case 4:
                    return Utility.RandomNeutralHue();
            }
        }

        public virtual int GetShoeHue()
        {
            if( 0.1 > Utility.RandomDouble() )
                return 0;

            return Utility.RandomNeutralHue();
        }

        public virtual void InitOutfit()
        {
            switch( Utility.Random( 3 ) )
            {
                case 0:
                    AddItem( new FancyShirt( GetRandomHue() ) );
                    break;
                case 1:
                    AddItem( new Doublet( GetRandomHue() ) );
                    break;
                case 2:
                    AddItem( new Shirt( GetRandomHue() ) );
                    break;
            }

            switch( Utility.Random( 4 ) )
            {
                case 0:
                    AddItem( new Shoes( GetShoeHue() ) );
                    break;
                case 1:
                    AddItem( new Boots( GetShoeHue() ) );
                    break;
                case 2:
                    AddItem( new Sandals( GetShoeHue() ) );
                    break;
                case 3:
                    AddItem( new ThighBoots( GetShoeHue() ) );
                    break;
            }

            GenerateRandomHair();

            if( Female )
            {
                switch( Utility.Random( 6 ) )
                {
                    case 0:
                        AddItem( new ShortPants( GetRandomHue() ) );
                        break;
                    case 1:
                    case 2:
                        AddItem( new Kilt( GetRandomHue() ) );
                        break;
                    case 3:
                    case 4:
                    case 5:
                        AddItem( new Skirt( GetRandomHue() ) );
                        break;
                }
            }
            else
            {
                switch( Utility.Random( 2 ) )
                {
                    case 0:
                        AddItem( new LongPants( GetRandomHue() ) );
                        break;
                    case 1:
                        AddItem( new ShortPants( GetRandomHue() ) );
                        break;
                }
            }

            PackGold( 100, 200 );
        }

        public virtual void GenerateRandomHair()
        {
            Utility.AssignRandomHair( this );
            Utility.AssignRandomFacialHair( this, HairHue );
        }

        public Item Immovable( Item item )
        {
            item.Movable = false;
            return item;
        }

        public Item Newbied( Item item )
        {
            item.LootType = LootType.Newbied;
            return item;
        }

        public Item Rehued( Item item, int hue )
        {
            item.Hue = hue;
            return item;
        }

        public Item Layered( Item item, Layer layer )
        {
            item.Layer = layer;
            return item;
        }

        public Item Resourced( BaseWeapon weapon, CraftResource resource )
        {
            weapon.Resource = resource;
            return weapon;
        }

        public Item Resourced( BaseArmor armor, CraftResource resource )
        {
            armor.Resource = resource;
            return armor;
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            c.Delete();
        }

        public virtual void GenerateBody( bool isFemale, bool randomHair )
        {
            Hue = Utility.RandomSkinHue();

            if( isFemale )
            {
                Female = true;
                Body = 401;
                Name = NameList.RandomName( "female" );
            }
            else
            {
                Female = false;
                Body = 400;
                Name = NameList.RandomName( "male" );
            }

            if( randomHair )
                GenerateRandomHair();
        }

        #region Quests by Dies Irae
        public override bool GuardImmune { get { return true; } }

        public virtual int AutoTalkRange { get { return -1; } }
        public virtual int AutoSpeakRange { get { return 10; } }
        public virtual TimeSpan SpeakDelay { get { return TimeSpan.FromMinutes( 1 ); } }

        public virtual Type[] Quests { get { return null; } }

        private DateTime m_Spoken;

        public virtual void OnTalk( PlayerMobile player )
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

            BaseQuest questt = QuestHelper.RandomQuest( player, Quests, this );

            if( questt != null )
            {
                player.CloseGump( typeof( MondainQuestGump ) );
                player.SendGump( new MondainQuestGump( questt ) );
            }
        }

        public virtual void OnOfferFailed()
        {
            Say( 1075575 ); // I'm sorry, but I don't have anything else for you right now. Could you check back with me in a few minutes?
        }

        /*
        public virtual void Advertise()
        {
            Say( Utility.RandomMinMax( 1074183, 1074223 ) );
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            if( Quests == null || Quests.Length == 0 || !CanOfferQuestTo( m ) )
                return;

            if( m.Alive && !m.Hidden && m is PlayerMobile )
            {
                PlayerMobile pm = (PlayerMobile)m;

                int range = AutoTalkRange;

                if( range >= 0 && InRange( m, range ) && !InRange( oldLocation, range ) )
                    OnTalk( pm );

                range = AutoSpeakRange;

                if( range >= 0 && InRange( m, range ) && !InRange( oldLocation, range ) && DateTime.Now >= m_Spoken + SpeakDelay )
                {
                    if( Utility.Random( 100 ) < 50 )
                        Advertise();

                    m_Spoken = DateTime.Now;
                }
            }
        }
        */

        public virtual bool CanOfferQuestTo( Mobile m )
        {
            return ClassSystem.Find( m ) == System;
        }

        public void FocusTo( Mobile to )
        {
            QuestSystem.FocusTo( this, to );
        }
        #endregion

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}