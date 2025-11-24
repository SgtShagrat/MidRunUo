/***************************************************************************
 *                               PatrolGuard.cs
 *
 *   begin                : 20 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    public class PatrolGuard : BaseTownSoldier
    {
        public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.Guard; } }

        private bool m_Archer;

        [Constructable]
        public PatrolGuard()
            : this( MidgardTowns.None )
        {
        }

        [Constructable]
        public PatrolGuard( MidgardTowns town )
            : this( town, false, false, Utility.RandomBool() )
        {
        }

        [Constructable]
        public PatrolGuard( MidgardTowns town, bool mounted, bool archer, bool isFemale )
            : base( isFemale, mounted )
        {
            Town = town;
            m_Archer = archer;

            DeleteOnDeath = true;
            HumanMurderer = false;
            HumanInnocent = true;
            HumanBardImmune = false;

            if( System != null )
                System.DressTownGuard( this );
            else
                Console.WriteLine( "Warning: patrol guard with no TownSystem." );
        }

        public override void OnAfterSpawn()
        {
            Console.WriteLine( "Calling OnAfterSpawn for patrol guard of: " + Town );

            base.OnAfterSpawn();

            if( System != null )
                System.DressTownGuard( this );
            else
            {
                SetStr( 151, 175 );
                SetDex( 61, 85 );
                SetInt( 81, 95 );

                SetResistance( ResistanceType.Physical, 40, 60 );
                SetResistance( ResistanceType.Fire, 40, 60 );
                SetResistance( ResistanceType.Cold, 40, 60 );
                SetResistance( ResistanceType.Energy, 40, 60 );
                SetResistance( ResistanceType.Poison, 40, 60 );

                VirtualArmor = 32;

                SetSkill( SkillName.Archery, 110.0, 120.0 );
                SetSkill( SkillName.Swords, 110.0, 120.0 );
                SetSkill( SkillName.Wrestling, 110.0, 120.0 );
                SetSkill( SkillName.Tactics, 110.0, 120.0 );
                SetSkill( SkillName.MagicResist, 110.0, 120.0 );
                SetSkill( SkillName.Healing, 110.0, 120.0 );
                SetSkill( SkillName.Anatomy, 110.0, 120.0 );

                if( m_Archer )
                {
                    AddItem( Immovable( Rehued( new ChainChest(), 0x71B ) ) );

                    AddItem( Immovable( Rehued( new ChainLegs(), 0x71B ) ) );
                    AddItem( Immovable( Rehued( new ChainCoif(), 0x71B ) ) );
                    AddItem( Immovable( Rehued( new PlateGorget(), 0x71B ) ) );
                    AddItem( Immovable( Rehued( new RingmailArms(), 0x71B ) ) );
                    AddItem( Immovable( Rehued( new RingmailGloves(), 0x71B ) ) );

                    AddItem( Newbied( new Bow() ) );
                    AddItem( Newbied( Rehued( new Boots(), 0x93C ) ) );
                }
                else
                {
                    if( !Female )
                        AddItem( Immovable( Rehued( new PlateChest(), 0x71B ) ) );
                    else
                        AddItem( Immovable( Rehued( new FemalePlateChest(), 0x71B ) ) );

                    AddItem( Immovable( Rehued( new PlateLegs(), 0x71B ) ) );
                    AddItem( Immovable( Rehued( new CloseHelm(), 0x71B ) ) );
                    AddItem( Immovable( Rehued( new PlateGorget(), 0x71B ) ) );
                    AddItem( Immovable( Rehued( new PlateArms(), 0x71B ) ) );
                    AddItem( Immovable( Rehued( new PlateGloves(), 0x71B ) ) );

                    AddItem( Newbied( new Halberd() ) );
                }

                AddItem( Newbied( Rehued( new Cloak(), 0x93C ) ) );

                if( Backpack == null )
                    AddItem( Immovable( new Backpack() ) );
            }
        }

        #region serial-deserial
        public PatrolGuard( Serial serial )
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