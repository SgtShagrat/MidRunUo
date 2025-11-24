/***************************************************************************
 *                                  HardLabourTools.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Engines.Harvest;

namespace Midgard.Engines.HardLabour
{
    public class KerberosWhip : BaseAxe
    {
        public override string DefaultName { get { return "Kerberos Whip"; } }

        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.Disarm; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.ParalyzingBlow; } }

        public override int AosStrengthReq { get { return 190; } }
        public override int AosMinDamage { get { return 10; } }
        public override int AosMaxDamage { get { return 30; } }
        public override int AosSpeed { get { return 100; } }

        public override int OldStrengthReq { get { return 190; } }
        public override int OldMinDamage { get { return 10; } }
        public override int OldMaxDamage { get { return 30; } }
        public override int OldSpeed { get { return 100; } }

        public override int DefHitSound { get { return 0x141; } }
        public override int DefMissSound { get { return -1; } }

        public override SkillName DefSkill { get { return SkillName.Wrestling; } }
        public override WeaponType DefType { get { return WeaponType.Fists; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.ShootBow; } }

        [Constructable]
        public KerberosWhip()
            : base( 0x313 )
        {
            Layer = Layer.TwoHanded;
            Weight = 5.0;
        }

        #region serial-deserial
        public KerberosWhip( Serial serial )
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

    [FlipableAttribute( 0xE86, 0xE85 )]
    public class SlavePickaxe : BaseAxe
    {
        #region proprietà
        public override HarvestSystem HarvestSystem { get { return Mining.System; } }

        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.DoubleStrike; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.Disarm; } }

        public override int AosStrengthReq { get { return 10; } }
        public override int AosMinDamage { get { return 13; } }
        public override int AosMaxDamage { get { return 15; } }
        public override int AosSpeed { get { return 35; } }

        public override int OldStrengthReq { get { return 25; } }
        public override int OldMinDamage { get { return 1; } }
        public override int OldMaxDamage { get { return 15; } }
        public override int OldSpeed { get { return 35; } }

        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 60; } }

        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Slash1H; } }

	    public override int NumDice { get { return 1; } }
	    public override int NumSides { get { return 15; } }
	    public override int DiceBonus { get { return 0; } }

        public override SkillName OldSkill{ get{ return SkillName.Mining; } }

        public override int BlockCircle { get { return -1; } }

        public override bool CheckForAttackSkillOnSwing { get { return false; } }
        #endregion

        private const int m_Bonus = 20;
        private SkillMod m_SkillMod;

        [Constructable]
        public SlavePickaxe()
            : base( 0xE86 )
        {
            Weight = 11.0;
            UsesRemaining = 50;
            ShowUsesRemaining = true;
            Hue = 0x973;
        }

        public override void OnAdded( object parent )
        {
            base.OnAdded( parent );

            if( m_Bonus != 0 && parent is Mobile )
            {
                if( m_SkillMod != null )
                    m_SkillMod.Remove();

                m_SkillMod = new DefaultSkillMod( SkillName.Mining, true, m_Bonus );
                ( (Mobile)parent ).AddSkillMod( m_SkillMod );
            }
        }

        public override void OnRemoved( object parent )
        {
            base.OnRemoved( parent );

            if( m_SkillMod != null )
                m_SkillMod.Remove();

            m_SkillMod = null;
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1062005, m_Bonus.ToString() ); // mining bonus +~1_val~
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, 1062005, m_Bonus.ToString() ); // mining bonus +~1_val~
        }

        #region serial-deserial
        public SlavePickaxe( Serial serial )
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

            if( m_Bonus != 0 && Parent is Mobile )
            {
                if( m_SkillMod != null )
                    m_SkillMod.Remove();

                m_SkillMod = new DefaultSkillMod( SkillName.Mining, true, m_Bonus );
                ( (Mobile)Parent ).AddSkillMod( m_SkillMod );
            }
        }
        #endregion
    }
}