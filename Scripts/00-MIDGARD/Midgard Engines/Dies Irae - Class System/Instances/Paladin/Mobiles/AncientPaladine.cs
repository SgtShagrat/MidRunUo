using System;
using System.Collections.Generic;

using Midgard.Engines.Classes.VirtueChampion;
using Midgard.Engines.SpellSystem;

using Server;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.Classes
{
    public class AncientPaladine : BaseClassGiver
    {
        public override string Greetings
        {
            get { return "Hail to you! How can i help you?"; }
        }

        public override Item Book
        {
            get { return new PaladinTome(); }
        }

        public override ClassSystem System
        {
            get { return ClassSystem.Paladin; }
        }

        [Constructable]
        public AncientPaladine()
            : base( "the ancient paladine" )
        {
            /*
            Equip	0x1408 1170 // Plate parts...
            Equip	0x1413 1170
            Equip	0x1414 1170
            Equip	0x1410 1170
            Equip	0x1411 1170
            Equip	0x1415 1170
            Equip	0x13B9 1170 // VikingSword
            Equip	0x1BC4 1170 // Order Shield
            Equip	0x1515 1170 // Cloak
            Equip	0x203b 	// short hair
            Equip	0x203f 	// short beard
             */

            GenerateBody( false, false );
            FacialHairItemID = 0x203f;
            HairItemID = 0x203b;

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

            AddItem( Immovable( Rehued( new Cloak(), 0x683 ) ) );
            AddItem( Immovable( Rehued( new PlateChest(), 0x683 ) ) );
            AddItem( Immovable( Rehued( new PlateLegs(), 0x683 ) ) );
            AddItem( Immovable( Rehued( new CloseHelm(), 0x683 ) ) );
            AddItem( Immovable( Rehued( new PlateGorget(), 0x683 ) ) );
            AddItem( Immovable( Rehued( new PlateArms(), 0x683 ) ) );
            AddItem( Immovable( Rehued( new PlateGloves(), 0x683 ) ) );

            AddItem( Newbied( Rehued( new VikingSword(), 0x683 ) ) );
            AddItem( Newbied( Rehued( new OrderShield(), 0x683 ) ) );
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            PlayerMobile from = e.Mobile as PlayerMobile;

            if( !e.Handled && from != null && from.InRange( Location, 2 ) )
            {
                BaseVirtue match = null;
                if( VirtueChampion.Config.Virtues != null )
                {
                    foreach( BaseVirtue d in VirtueChampion.Config.Virtues )
                    {
                        if( !e.Speech.Contains( d.Definition.Name ) )
                            continue;

                        match = d;
                    }
                }
                else
                    Say( "Warning: VirtueChampion.Config.VirtueDefinitions is null." );

                if( match != null )
                {
                    if( match.Definition.ChainID != QuestChain.None && from.Chains.ContainsKey( match.Definition.ChainID ) )
                        Say( "You already in progress with that mission." );
                    else
                        TryOffer( from, match.Definition.QuestStageOneType );

                    e.Handled = true;
                }
            }

            base.OnSpeech( e );
        }

        private void TryOffer( PlayerMobile player, Type questType )
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
            
            BaseQuest q = QuestHelper.Construct( questType ) as BaseQuest;
            if( q != null )
            {
                player.CloseGump( typeof( MondainQuestGump ) );
                player.SendGump( new MondainQuestGump( q ) );
            }
        }

        public override void EndJoin( Mobile joiner, bool join )
        {
            base.EndJoin( joiner, join );

            if( join )
            {
                joiner.FixedEffect( 0x373A, 10, 30 );
                joiner.PlaySound( 0x209 );
                joiner.PublicOverheadMessage( MessageType.Regular, Utility.RandomMinMax( 90, 95 ), true, "* You are now a paladin candidate *" );

                // joiner.AddToBackpack( new PaladinTome() );
            }
        }

        #region serialization
        public AncientPaladine( Serial serial )
            : base( serial )
        {
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
        #endregion
    }
}