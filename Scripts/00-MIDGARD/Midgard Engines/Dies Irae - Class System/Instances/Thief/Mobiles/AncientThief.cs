using Midgard.Items;

using Server;
using Server.Items;
using Server.Network;
using Midgard.Engines.AdvancedDisguise;

namespace Midgard.Engines.Classes
{
    public class AncientThief : BaseClassGiver
    {
        public override string Greetings
        {
            get { return "Hail to you! How can i help you?"; }
        }

        public override Item Book
        {
            get { return new SketchBook(); }
        }

        public override ClassSystem System
        {
            get { return ClassSystem.Thief; }
        }

        [Constructable]
        public AncientThief()
            : base( "the ancient thief" )
        {
            GenerateBody( false, false );

            HairItemID = 0x203b;
            HairHue = 51;

            SetStr( 151, 175 );
            SetDex( 61, 85 );
            SetInt( 81, 95 );

            SetSkill( SkillName.Camping, 55.0, 78.0 );
            SetSkill( SkillName.DetectHidden, 65.0, 88.0 );
            SetSkill( SkillName.Hiding, 45.0, 68.0 );
            SetSkill( SkillName.Archery, 65.0, 88.0 );
            SetSkill( SkillName.Tracking, 65.0, 88.0 );
            SetSkill( SkillName.Veterinary, 60.0, 83.0 );

            AddItem( Immovable( Rehued( new ReinforcedBoots(), 0x070D ) ) );
            AddItem( Immovable( Rehued( new KiltBag(), 0x0000 ) ) );
            AddItem( Immovable( Rehued( new TearedShirt(), 0x070D ) ) );
            AddItem( Immovable( Rehued( new TraditionalFurCape(), 0x0000 ) ) );
            AddItem( Immovable( Rehued( new LeatherGloves(), 0x0000 ) ) );
            AddItem( Immovable( Rehued( new ShortLeatherSkirt(), 0x070D ) ) );
            AddItem( Immovable( Rehued( new FishnetStockings(), 0x0000 ) ) );
            AddItem( Immovable( Rehued( new Dagger(), 0x0000 ) ) );
        }

        public override void EndJoin( Mobile joiner, bool join )
        {
            base.EndJoin( joiner, join );

            if( join )
            {
                joiner.FixedEffect( 0x373A, 10, 30 );
                joiner.PlaySound( 0x209 );
                joiner.PublicOverheadMessage( MessageType.Regular, Utility.RandomMinMax( 90, 95 ), true, "* You are now a thief candidate *" );
            }
        }

        #region serialization
        public AncientThief( Serial serial )
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