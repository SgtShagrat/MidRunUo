using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Midgard.Items;
using Midgard.Engines.Classes;
using Midgard.Engines.AdvancedDisguise;

namespace Server.Mobiles
{
    public class Thief : BaseVendor
    {
        public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.Thief; } } // mod by Dies Irae

		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

        [Constructable]
		public Thief() : base( "the thief" )
        {
            SetSkill( SkillName.Camping, 55.0, 78.0 );
            SetSkill( SkillName.DetectHidden, 65.0, 88.0 );
            SetSkill( SkillName.Hiding, 45.0, 68.0 );
            SetSkill( SkillName.Archery, 65.0, 88.0 );
            SetSkill( SkillName.Tracking, 65.0, 88.0 );
            SetSkill( SkillName.Veterinary, 60.0, 83.0 );

            #region mod by Dies Irae
            if( !Core.AOS )
                SetSkill( SkillName.Poisoning, 60.0, 83.0 );
            #endregion
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add( new SBThief() );
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            if( FindItemOnLayer( Layer.Shirt ) == null ) // mod by Dies Irae
                AddItem( new Server.Items.Shirt( Utility.RandomNeutralHue() ) );
            if( FindItemOnLayer( Layer.Pants ) == null ) // mod by Dies Irae
                AddItem( new Server.Items.LongPants( Utility.RandomNeutralHue() ) );
            AddItem( new Server.Items.Dagger() );
            if( FindItemOnLayer( Layer.Shoes ) == null ) // mod by Dies Irae
                AddItem( new Server.Items.ThighBoots( Utility.RandomNeutralHue() ) );
        }

        #region mod by Dies Irae
        public override bool HandlesOnSpeech( Mobile from )
        {
            if( from.InRange( Location, 2 ) )
                return true;

            return base.HandlesOnSpeech( from );
        }

        /*
        public override bool CheckVendorAccess( Mobile from )
        {
            if( !ClassSystem.IsThief( from ) )
            {
                SayTo( from, 501838 ); // I don't know what you're talking about.
                return false;
            }

            return base.CheckVendorAccess( from );
        }
        */

        public override void OnSpeech( SpeechEventArgs e )
        {
            Mobile from = e.Mobile;

            if( !e.Handled && from is PlayerMobile && from.InRange( Location, 2 ) )
            {
                bool isThief = ClassSystem.IsThief( from );

                if( Insensitive.Contains( e.Speech, "cleaner" ) )
                {
                    if( isThief )
                        SayTo( from, true, "That particular item costs only 1000 gold pieces, for thy, my friend." );
                    else
                        SayTo( from, 501838 ); // I don't know what you're talking about.

                    e.Handled = true;
                }
                else if( Insensitive.Contains( e.Speech, "narcotic" ) )
                {
                    if( isThief )
                        SayTo( from, true, "That particular item costs ... UHM ... 500 gold pieces." );
                    else
                        SayTo( from, 501838 ); // I don't know what you're talking about.

                    e.Handled = true;
                }
                else if( Insensitive.Contains( e.Speech, "stabbing" ) )
                {
                    if( isThief )
                        SayTo( from, true, "That particular item costs ... UHM ... 750 gold pieces." );
                    else
                        SayTo( from, 501838 ); // I don't know what you're talking about.

                    e.Handled = true;
                }
                else if( Insensitive.Contains( e.Speech, "sketch" ) )
                {
                    if( isThief )
                        SayTo( from, true, "That particular item costs ... UHM ... 2000 gold pieces, for thy, my friend." );
                    else
                        SayTo( from, 501838 ); // I don't know what you're talking about.

                    e.Handled = true;
                }
                else if( e.HasKeyword( 0x1F ) ) // *disguise*
                {
                    if( isThief )
                        SayTo( from, 501839 ); // That particular item costs 700 gold pieces.
                    else
                        SayTo( from, 501838 ); // I don't know what you're talking about.

                    e.Handled = true;
                }
            }

            base.OnSpeech( e );
        }

        public override bool OnGoldGiven( Mobile from, Gold dropped )
        {
            if( from is PlayerMobile )
            {
                if( ClassSystem.IsThief( from ) )
                {
                    if( dropped.Amount == 500 )
                    {
                        from.AddToBackpack( new NarcoticBandage() );
                        dropped.Delete();
                        return true;
                    }
                    else if( dropped.Amount == 1000 )
                    {
                        from.AddToBackpack( new DisguiseCleaner() );
                        dropped.Delete();
                        return true;
                    }
                    else if( dropped.Amount == 750 )
                    {
                        from.AddToBackpack( new BackstabbingKnife() );
                        dropped.Delete();
                        return true;
                    }
                    else if( dropped.Amount == 2000 )
                    {
                        from.AddToBackpack( new SketchBook() );
                        dropped.Delete();
                        return true;
                    }
                    else if( dropped.Amount == 700 )
                    {
                        from.AddToBackpack( new DisguiseKit() );
                        dropped.Delete();
                        return true;
                    }
                }
            }

            return base.OnGoldGiven( from, dropped );
        }
        #endregion

        public Thief( Serial serial )
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
    }
}