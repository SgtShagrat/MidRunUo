/***************************************************************************
 *                               OscarWaltz.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;

namespace Midgard.Engines.Academies
{
    public class OscarWaltz : AcademyQuester
    {
        public override AcademySystem Academy
        {
            get { return AcademySystem.SerpentsHoldAcademy; }
        }

        public override Disciplines Discipline
        {
            get { return Disciplines.ArtOfMining; }
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                        { 
                            typeof( TrainingMiningOneQuest )
                        };
            }
        }

        [Constructable]
        public OscarWaltz()
            : base( "Oscar Waltz", "The Miner Instructor" )
        {
            SetSkill( SkillName.ArmsLore, 120.0, 120.0 );
            SetSkill( SkillName.Blacksmith, 120.0, 120.0 );
            SetSkill( SkillName.Magery, 120.0, 120.0 );
            SetSkill( SkillName.Tactics, 120.0, 120.0 );
            SetSkill( SkillName.Tinkering, 120.0, 120.0 );
            SetSkill( SkillName.Swords, 120.0, 120.0 );
            SetSkill( SkillName.Mining, 120.0, 120.0 );
        }

        public override bool CanOfferQuestTo( Mobile m )
        {
            AcademyPlayerState state = AcademyPlayerState.Find( m );
            if( state == null )
                return false;

            if( state.Academy != Academy )
                return false;

            return state.IsLearning( Discipline );
        }

        public override void Advertise()
        {
            Say( 1078124 ); // You there! I can use some help mining these rocks!
        }

        public override void OnOfferFailed()
        {
            Say( 1077772 ); // I cannot teach you, for you know all I can teach!
        }

        public override void InitBody()
        {
            Female = false;
            CantWalk = true;

            Race = Race.Human;

            base.InitBody();
        }

        public override void InitOutfit()
        {
            AddItem( new Backpack() );
            AddItem( new Pickaxe() );
            AddItem( new Boots() );
            AddItem( new ShortPants( 0x370 ) );
            AddItem( new Shirt( 0x966 ) );
            AddItem( new WideBrimHat( 0x966 ) );
            AddItem( new HalfApron( 0x1BB ) );
        }

        #region serialization
        public OscarWaltz( Serial serial )
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