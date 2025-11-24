using Midgard.Items;

using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Midgard.Engines.Classes
{
    public class AncientScout : BaseClassGiver
    {
        public override string Greetings
        {
            get { return "Hail to you! How can i help you?"; }
        }

        public override Item Book
        {
            get { return new BlueBook( true ); }
        }

         public override ClassSystem System
        {
            get { return ClassSystem.Scout; }
        }

        [Constructable]
        public AncientScout()
            : base( "the ancient scout" )
        {
            GenerateBody( false, false );

            HairItemID = 0x203b;
            HairHue = 1109;

            SetStr( 151, 175 );
            SetDex( 61, 85 );
            SetInt( 81, 95 );

            SetSkill( SkillName.Camping, 55.0, 78.0 );
            SetSkill( SkillName.DetectHidden, 65.0, 88.0 );
            SetSkill( SkillName.Hiding, 45.0, 68.0 );
            SetSkill( SkillName.Archery, 65.0, 88.0 );
            SetSkill( SkillName.Tracking, 65.0, 88.0 );
            SetSkill( SkillName.Veterinary, 60.0, 83.0 );

            AddItem( Immovable( Rehued( new AdventurerTunic(), 0x05A3 ) ) );
            AddItem( Immovable( Rehued( new Quiver(), 0x070D ) ) );
            AddItem( Immovable( Rehued( new ScoutGloves(), 0x05A5 ) ) );
            AddItem( Immovable( Rehued( new ScoutGorget(), 0x05A5 ) ) );
            AddItem( Immovable( Rehued( new ScoutLegs(), 0x05A5 ) ) );
            AddItem( Immovable( Rehued( new ScoutSleeves(), 0x05A5 ) ) );
            AddItem( Immovable( Rehued( new TraditionalFurCape(), 0x0000 ) ) );
            AddItem( Immovable( Rehued( new Boots(), 0x070D ) ) );
            AddItem( Immovable( Rehued( new Bow(), 0x08F7 ) ) );
        }

        public override void EndJoin( Mobile joiner, bool join )
        {
            base.EndJoin( joiner, join );

            if( join )
            {
                joiner.FixedEffect( 0x373A, 10, 30 );
                joiner.PlaySound( 0x209 );
                joiner.PublicOverheadMessage( MessageType.Regular, Utility.RandomMinMax( 90, 95 ), true, "* You are now a scout candidate *" );
            }
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            Mobile from = e.Mobile;

            if( !e.Handled && from is PlayerMobile && from.InRange( Location, 2 ) )
            {
                bool isScout = ClassSystem.IsScout( from );

                if( Insensitive.Contains( e.Speech, "paint" ) )
                {
                    if( isScout )
                        SayTo( from, true, "That useful item costs only 250 gold pieces, for thy, my friend." );
                    else
                        SayTo( from, 501838 ); // I don't know what you're talking about.

                    e.Handled = true;
                }
            }

            base.OnSpeech( e );
        }

        public override bool OnGoldGiven( Mobile from, Gold dropped )
        {
            if( from is PlayerMobile && ClassSystem.IsScout( from ) && dropped.Amount == 250 )
            {
                from.AddToBackpack( new ScoutMimeticPaint() );
                dropped.Delete();
                return true;
            }

            return base.OnGoldGiven( from, dropped );
        }

        #region serialization
        public AncientScout( Serial serial )
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
        #endregion
    }
}