using Midgard.Engines.SpellSystem;
using Midgard.Items;

using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.Classes
{
    public class AncientNecromancer : BaseClassGiver
    {
        public override string Greetings
        {
            get { return "Hail to you! How can i help you?"; }
        }

        public override Item Book
        {
            get { return new NecromancerTome(); }
        }

        public override ClassSystem System
        {
            get { return ClassSystem.Necromancer; }
        }

        [Constructable]
        public AncientNecromancer()
            : base( "the ancient necromancer" )
        {
            GenerateBody( false, false );

            FacialHairItemID = 0;

            HairItemID = 0x203B;
            HairHue = 0;

            SetStr( 151, 175 );
            SetDex( 61, 85 );
            SetInt( 81, 95 );

            SetResistance( ResistanceType.Physical, 40, 60 );
            SetResistance( ResistanceType.Fire, 40, 60 );
            SetResistance( ResistanceType.Cold, 40, 60 );
            SetResistance( ResistanceType.Energy, 40, 60 );
            SetResistance( ResistanceType.Poison, 40, 60 );

            VirtualArmor = 32;

            SetSkill( SkillName.Swords, 110.0, 120.0 );
            SetSkill( SkillName.Wrestling, 110.0, 120.0 );
            SetSkill( SkillName.Tactics, 110.0, 120.0 );
            SetSkill( SkillName.MagicResist, 110.0, 120.0 );
            SetSkill( SkillName.Healing, 110.0, 120.0 );
            SetSkill( SkillName.Anatomy, 110.0, 120.0 );

            AddItem( Immovable( Rehued( new Monocle(), 0 ) ) );
            AddItem( Immovable( Rehued( new BonesNecklace(), 0x0763 ) ) );
            AddItem( Immovable( Rehued( Layered( new Item( 0x2253 ), Layer.OneHanded ), 0 ) ) );
            AddItem( Immovable( Rehued( new Sandals(), 0 ) ) );
            AddItem( Immovable( Rehued( new HoodedShroudOfShadows(), 0x0759 ) ) );
        }

        public override void EndJoin( Mobile joiner, bool join )
        {
            base.EndJoin( joiner, join );

            if( join )
            {
                joiner.FixedEffect( 0x373A, 10, 30 );
                joiner.PlaySound( 0x209 );
                joiner.PublicOverheadMessage( MessageType.Regular, Utility.RandomMinMax( 90, 95 ), true, "* You are now a necromancer candidate *" );

                // joiner.AddToBackpack( new NecromancerTome() );
            }
        }

        #region serialization
        public AncientNecromancer( Serial serial )
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