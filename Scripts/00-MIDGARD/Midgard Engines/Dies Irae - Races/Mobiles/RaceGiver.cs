using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.Races
{
    public class RaceGiver : BaseCreature
    {
        private const int ListenRange = 12;

        public virtual string Greetings { get { return "Greetings."; } }

        public virtual string JoinQuestion { get { return "Do you want a new race?"; } }

        public MidgardRace MidRace
        {
            get { return Race != null && Race is MidgardRace ? (MidgardRace) Race : null; }
        }

        public override bool DisallowAllMoves { get { return true; } }
        public override bool ClickTitle { get { return false; } }
        public override bool CanTeach { get { return false; } }
        public override bool BardImmune { get { return true; } }

        [Constructable]
        public RaceGiver( string title )
            : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4 )
        {
            Title = title;

            GenerateBody( false, true );

            SetStr( 151, 175 );
            SetDex( 61, 85 );
            SetInt( 81, 95 );
        }

        public RaceGiver( Serial serial )
            : base( serial )
        {
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

            if( !e.Handled && from.Player && from.InRange( Location, 2 ) )
            {
                if( WasNamed( e.Speech ) && e.HasKeyword( 0x0004 ) ) // *join* | *member*
                {
                    if( from.Race != Race.Human )
                        SayTo( from, "Thou art already a {0}", Race.Name );
                    else if( MidRace != null )
                    {
                        if( MidRace.IsCandidate( from ) )
                            SayTo( from, "Thou art already a {0} candidate", Race.Name );
                        else
                        {
                            SayTo( from, Greetings );
                            from.SendGump( new ConfirmRaceJoinGump( from, MidRace, this ) );
                        }
                    }

                    e.Handled = true;
                }
            }

            base.OnSpeech( e );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.AccessLevel > AccessLevel.GameMaster && MidRace != null )
                from.SendGump( new RaceCandidateApprovalGump( MidRace, from ) );

            base.OnDoubleClick( from );
        }

        public virtual void EndJoin( Mobile joiner, bool join )
        {
            if( join && MidRace != null )
                MidRace.AddCandidate( joiner );
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
    }
}