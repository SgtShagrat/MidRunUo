namespace Server.Items
{
    [FlipableAttribute( 0x170B, 0x170C )]
    public class BootsOfVeterinary : BaseShoesOfVeterinary
    {
        public override string DefaultName
        {
            get
            {
                return "tamers boots of veterinary";
            }
        }

        [Constructable]
        public BootsOfVeterinary( int bonus )
            : base( bonus, 0x170B )
        {
            Weight = 1.0;
        }

        public BootsOfVeterinary( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( Name == "tamers boots of veterinary" )
                Name = null;
        }
    }

    [FlipableAttribute( 0x170D, 0x170E )]
    public class SandalsOfVeterinary : BaseShoesOfVeterinary
    {
        public override string DefaultName
        {
            get
            {
                return "tamers sandals of veterinary";
            }
        }

        [Constructable]
        public SandalsOfVeterinary( int bonus )
            : base( bonus, 0x170D )
        {
            Weight = 2.0;
        }

        public SandalsOfVeterinary( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( Name == "tamers sandals of veterinary" )
                Name = null;
        }
    }

    [FlipableAttribute( 0x170F, 0x1710 )]
    public class ShoesOfVeterinary : BaseShoesOfVeterinary
    {
        public override string DefaultName
        {
            get
            {
                return "tamers shoes of veterinary";
            }
        }

        [Constructable]
        public ShoesOfVeterinary( int bonus )
            : base( bonus, 0x170F )
        {
            Weight = 1.0;
        }

        public ShoesOfVeterinary( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( Name == "tamers shoes of veterinary" )
                Name = null;
        }
    }

    public abstract class BaseShoesOfVeterinary : BaseShoes
    {
        private int m_Bonus;
        private SkillMod m_SkillMod;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Bonus
        {
            get
            {
                return m_Bonus;
            }
            set
            {
                m_Bonus = value;
                InvalidateProperties();

                if( m_Bonus == 0 )
                {
                    if( m_SkillMod != null )
                        m_SkillMod.Remove();

                    m_SkillMod = null;
                }
                else if( m_SkillMod == null && Parent is Mobile )
                {
                    m_SkillMod = new DefaultSkillMod( SkillName.Veterinary, true, m_Bonus );
                    ( (Mobile)Parent ).AddSkillMod( m_SkillMod );
                }
                else if( m_SkillMod != null )
                {
                    m_SkillMod.Value = m_Bonus;
                }
            }
        }

        public override void OnAdded( object parent )
        {
            base.OnAdded( parent );

            if( m_Bonus != 0 && parent is Mobile )
            {
                if( m_SkillMod != null )
                    m_SkillMod.Remove();

                m_SkillMod = new DefaultSkillMod( SkillName.Veterinary, true, m_Bonus );
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

        public BaseShoesOfVeterinary( int bonus, int itemID )
            : base( itemID )
        {
            m_Bonus = bonus;

            Hue = Utility.RandomMinMax( 1150, 1175 );
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( m_Bonus != 0 )
                list.Add( 1060658, "veterinary bonus\t+{0}", m_Bonus.ToString() );
        }

        public BaseShoesOfVeterinary( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (int)m_Bonus );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_Bonus = reader.ReadInt();
                        break;
                    }
            }

            if( m_Bonus != 0 && Parent is Mobile )
            {
                if( m_SkillMod != null )
                    m_SkillMod.Remove();

                m_SkillMod = new DefaultSkillMod( SkillName.Veterinary, true, m_Bonus );
                ( (Mobile)Parent ).AddSkillMod( m_SkillMod );
            }
        }
    }
}