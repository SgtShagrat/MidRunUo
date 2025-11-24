using Server.Items;

namespace Server.Mobiles
{
    public abstract class FlyingCreature : BaseCreature
    {
        public virtual double FlySpeed { get { return 0.2; } }

        #region props
        private bool m_CanFlying = false;
        private bool m_IsFlying = false;
        private bool m_IsTakingOff = false;
        private bool m_IsLanding = false;
        private bool m_FlyingUp = false;
        private bool m_FlyingDown = false;
        private int i_Ceiling = 100;
        private int i_Ground;
        private int i_LeftSide;
        private int i_RightSide;
        private int i_TopSide;
        private int i_BottomSide;
        private int i_Direction;
        private int i_FlyStam;
        private int i_FlyStamMax;
        private int i_FlyAnim;
        private int i_FlyCnt;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanFly
        {
            get { return m_CanFlying; }
            set { m_CanFlying = value; }
        }

        public bool IsFlying
        {
            get { return m_IsFlying; }
            set { m_IsFlying = value; }
        }

        public bool IsTakingOff
        {
            get { return m_IsTakingOff; }
            set { m_IsTakingOff = value; }
        }

        public bool IsLanding
        {
            get { return m_IsLanding; }
            set { m_IsLanding = value; }
        }

        public bool FlyingUp
        {
            get { return m_FlyingUp; }
            set { m_FlyingUp = value; }
        }

        public bool FlyingDown
        {
            get { return m_FlyingDown; }
            set { m_FlyingDown = value; }
        }

        public int Ceiling
        {
            get { return i_Ceiling; }
            set { i_Ceiling = value; }
        }

        public int Ground
        {
            get { return i_Ground; }
            set { i_Ground = value; }
        }

        public int LeftSide
        {
            get { return i_LeftSide; }
            set { i_LeftSide = value; }
        }

        public int RightSide
        {
            get { return i_RightSide; }
            set { i_RightSide = value; }
        }

        public int TopSide
        {
            get { return i_TopSide; }
            set { i_TopSide = value; }
        }

        public int BottomSide
        {
            get { return i_BottomSide; }
            set { i_BottomSide = value; }
        }

        public int CDirection
        {
            get { return i_Direction; }
            set { i_Direction = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int FlyStam
        {
            get { return i_FlyStam; }
            set { i_FlyStam = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int FlyStamMax
        {
            get { return i_FlyStamMax; }
            set { i_FlyStamMax = value; }
        }

        public int FlyAnim
        {
            get { return i_FlyAnim; }
            set { i_FlyAnim = value; }
        }

        public int FlyCnt
        {
            get { return i_FlyCnt; }
            set { i_FlyCnt = value; }
        }
        #endregion

        public FlyingCreature( AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed )
            : base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
        {
        }

        public override bool OnBeforeDeath()
        {
            if( IsFlying )
            {
                IsFlying = false;
                Z = ( Ground + 1 );
                PlaySound( 0x525 );
            }

            return base.OnBeforeDeath();
        }

        public override void OnThink()
        {
            if( CanFly && !IsFlying )
            {
                if( FlyStam >= FlyStamMax && !Controlled )
                {
                    IsFlying = true;
                    IsTakingOff = true;

                    FlyingAI.Flying( this );
                }
                if( FlyStam < FlyStamMax )
                    FlyStam++;
            }

            base.OnThink();
        }

        public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
        {
            if( from.Weapon != null )
            {
                if( from.Weapon is BaseRanged )
                    return;
                else
                {
                    if( ( from.Z > Z + 10 ) || ( from.Z < Z - 10 ) )
                    {
                        from.SendMessage( "You have to be closer to attack this!" );
                        damage = 0;
                    }
                }
            }
        }

        public FlyingCreature( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( (bool)m_CanFlying );
            writer.Write( (bool)m_IsFlying );
            writer.Write( (bool)m_IsTakingOff );
            writer.Write( (bool)m_IsLanding );
            writer.Write( (bool)m_FlyingUp );
            writer.Write( (bool)m_FlyingDown );
            writer.Write( (int)i_Ceiling );
            writer.Write( (int)i_Ground );
            writer.Write( (int)i_LeftSide );
            writer.Write( (int)i_RightSide );
            writer.Write( (int)i_TopSide );
            writer.Write( (int)i_BottomSide );
            writer.Write( (int)i_Direction );
            writer.Write( (int)i_FlyStam );
            writer.Write( (int)i_FlyStamMax );
            writer.Write( (int)i_FlyAnim );
            writer.Write( (int)i_FlyCnt );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_CanFlying = (bool)reader.ReadBool();
            m_IsFlying = (bool)reader.ReadBool();
            m_IsTakingOff = (bool)reader.ReadBool();
            m_IsLanding = (bool)reader.ReadBool();
            m_FlyingUp = (bool)reader.ReadBool();
            m_FlyingDown = (bool)reader.ReadBool();
            i_Ceiling = (int)reader.ReadInt();
            i_Ground = (int)reader.ReadInt();
            i_LeftSide = (int)reader.ReadInt();
            i_RightSide = (int)reader.ReadInt();
            i_TopSide = (int)reader.ReadInt();
            i_BottomSide = (int)reader.ReadInt();
            i_Direction = (int)reader.ReadInt();
            i_FlyStam = (int)reader.ReadInt();
            i_FlyStamMax = (int)reader.ReadInt();
            i_FlyAnim = (int)reader.ReadInt();
            i_FlyCnt = (int)reader.ReadInt();
        }
    }
}
