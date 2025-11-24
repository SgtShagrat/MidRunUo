using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.BountySystem
{
    public class Murderer : BaseCreature
    {
        private int m_Bounty;
        private bool m_PostBounty;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Bounty
        {
            get { return m_Bounty; }
            set { m_Bounty = value; }
        }

        [Constructable]
        public Murderer()
            : this( true )
        {
        }

        public Murderer( bool postBounty )
            : base( AIType.AI_BossMelee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            m_PostBounty = postBounty;
            InitializeMurderer();
        }

        private void InitializeMurderer()
        {
            Title = "the murderer";
            SpeechHue = Utility.RandomDyedHue();

            if( Female = Server.Utility.RandomBool() )
            {
                Body = 0x191;
                Name = NameList.RandomName( "female" );
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );
            }

            // Add the hair
            HairItemID = Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A );
            HairHue = Utility.RandomNondyedHue();

            // Set the skills
            SetSkill( SkillName.Swords, 30, 75 );
            SetSkill( SkillName.MagicResist, 25, 48 );
            SetSkill( SkillName.Tactics, 30, 75 );

            // Pick a random weapon
            switch( Utility.Random( 3 ) )
            {
                case 0: AddItem( new Scimitar() ); break;
                case 1: AddItem( new Broadsword() ); break;
                case 2: AddItem( new VikingSword() ); break;
            }

            // Add the outfit
            AddItem( new FancyShirt( Utility.RandomNeutralHue() ) );
            AddItem( new LongPants( Utility.RandomNeutralHue() ) );
            AddItem( new Shoes( Utility.RandomNeutralHue() ) );

            Container pack = new Backpack();
            pack.Movable = false;
            AddItem( pack );

            AddToBackpack( new Gold( 250, 300 ) );

            Karma = Utility.RandomMinMax( 0, -1250 ) - 2500;

            // Set the number of kills based on the karma
            if( Karma <= -2900 )
                Kills = Utility.RandomMinMax( 5, 15 );
            else if( Karma <= -3300 )
                Kills = Utility.RandomMinMax( 16, 25 );
            else
                Kills = Utility.RandomMinMax( 26, 35 );

            // Set the murderers stats based on the number of kills
            // since they should be tougher to kill if they have killed more people
            InitStats( Kills * Utility.RandomMinMax( 4, 8 ),
                Kills * Utility.RandomMinMax( 4, 8 ),
                Kills * Utility.RandomMinMax( 2, 4 ) );

            // Set the fame based on the number of kills
            Fame = Kills * ( Utility.RandomMinMax( 50, 100 ) ) + 2000;

            // Set the bounty amount based on the kills
            Bounty = Kills * ( Utility.RandomMinMax( 100, 200 ) );

            HitsRegenBonus += 10;
            StaminaRegenBonus += 10;
            ManaRegenBonus += 10;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
            AddLoot( LootPack.Average );
            AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
        }

        public Murderer( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version 

            writer.Write( m_Bounty );
            writer.Write( m_PostBounty );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    m_Bounty = reader.ReadInt();
                    m_PostBounty = reader.ReadBool();
                    break;
            }
        }

        protected override void OnLocationChange( Point3D oldLocation )
        {
            base.OnLocationChange( oldLocation );

            // Check if the old location is zero (if so, then the mobile is being placed in the world)
            if( m_PostBounty && ( oldLocation == Point3D.Zero ) && ( Map != Map.Internal ) )
                AddQuestPost();
        }

        protected override void OnMapChange( Map oldMap )
        {
            // Call the base class OnMapChange
            base.OnMapChange( oldMap );

            // Check if the mobiles has been placed in a valid location
            if( m_PostBounty && ( oldMap == Map.Internal ) && ( Location != Point3D.Zero ) )
                AddQuestPost();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            // Remove the quests posted on the message board
            foreach( Item i in Items )
            {
                if( i is BulletinMessage )
                    i.Delete();
            }
        }

        private void AddQuestPost()
        {
            // Post a new bounty message with the bounty amount 
            int RangeToPost = 500;
            string Subject = "Bounty";
            string[] Message = new string[]{ 
                                               Name, 
                                               "has killed " + Kills + " people!", 
                                               "The lords of the land have", 
                                               "placed a price on this vile", 
                                               "murderers head of", 
                                               Bounty + " gold.", 
                                               "Return the severed head of", 
                                               Name + " to any", 
                                               "town guard to collect your", 
                                               "reward." 
                                           };

            BulletinMessage[] posts = BaseBulletinBoard.PostQuest( this, RangeToPost, Subject, Message );

            // If the quest was posted to any bulletin boards, store them
            if( posts.Length > 0 )
                Items.InsertRange( 0, posts );
        }
    }
}