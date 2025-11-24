using System;
using Midgard.Engines.AdvancedCooking;
using Server;
using Server.Network;

namespace Midgard.Engines.CheeseCrafting
{
    public class CheeseForm : Item
    {
        private MilkTypes m_MilkType;
        private int m_CheeseQuality;
        private int m_FermentationState;
        private bool m_HasMilk;
        private bool m_HasCheese;
        public int m_FromBonusSkill;
        public Timer m_FermentationTimer;

        [CommandProperty( AccessLevel.GameMaster )]
        public MilkTypes MilkType
        {
            get { return m_MilkType; }
            set { m_MilkType = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int CheeseQuality
        {
            get { return m_CheeseQuality; }
            set { m_CheeseQuality = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int FermentationState
        {
            get { return m_FermentationState; }
            set { m_FermentationState = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasMilk
        {
            get { return m_HasMilk; }
            set { m_HasMilk = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsFermenting
        {
            get { return m_FermentationTimer != null && m_FermentationTimer.Running; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasCheese
        {
            get { return m_HasCheese; }
            set { m_HasCheese = value; }
        }

        [Constructable]
        public CheeseForm()
            : base( 0x0E78 )
        {
            Weight = 10.0;
            Name = "Cheese Form";
            Hue = 0x481;
        }

        public virtual void OnFermentationStarted()
        {
            if( m_FermentationTimer != null )
                return;

            m_FermentationTimer = new InternalTimer( this );
            m_FermentationTimer.Start();

            m_FermentationState = 0;

            m_HasCheese = false;
        }

        public virtual void OnFermentationEnded()
        {
            if( m_FermentationTimer != null )
                m_FermentationTimer = null;

            m_FermentationState = 0;

            m_HasCheese = true;
            m_HasMilk = false;

            m_CheeseQuality = Utility.Random( 1, 100 );
        }

        public virtual void DoCheese( Mobile to, bool magical )
        {
            Item cheese = null;

            switch( m_MilkType )
            {
                case MilkTypes.Cow:
                    {
                        if( magical )
                            cheese = new MagicalCowCheese();
                        else
                            cheese = new CowRawCheeseForm();
                        break;
                    }
                case MilkTypes.Goat:
                    {
                        if( magical )
                            cheese = new MagicalGoatCheese();
                        else
                            cheese = new GoatRawCheeseForm();
                        break;
                    }
                case MilkTypes.Sheep:
                    {
                        if( magical )
                            cheese = new MagicalSheepCheese();
                        else
                            cheese = new SheepRawCheeseForm();
                        break;
                    }
                default: break;
            }

            if( cheese != null )
            {
                if( magical )
                    to.SendMessage( 0x84C, "You obtain a wonderful magical cheese from the form." );
                else
                    to.SendMessage( 0x84C, "You obtain some cheese from the form. Now it is empty." );

                FoodHelper.GiveFood( to, cheese );
            }

            if( m_FermentationTimer != null )
                m_FermentationTimer = null;

            m_FermentationState = 0;

            m_HasCheese = false;
            m_HasMilk = false;
            m_MilkType = MilkTypes.None;

            m_CheeseQuality = 0;
        }

        public override void OnDoubleClick( Mobile from )
        {
            m_FromBonusSkill = ( CheeseQuality + ( (int)( from.Skills[SkillName.Cooking].Value ) / 5 ) );

            if( !from.InRange( GetWorldLocation(), 2 ) )
            {
                from.LocalOverheadMessage( MessageType.Regular, 906, 1019045 );
            }
            else
            {
                if( !m_HasCheese )
                {
                    if( !IsFermenting )
                    {
                        if( !m_HasMilk )
                        {
                            from.SendMessage( 0x84C, "Fill the form with milk before fermentation start." );
                        }
                        else
                        {
                            OnFermentationStarted();
                            from.SendMessage( "You start the fermentation process." );
                            m_FermentationState += from.CheckSkill( SkillName.Cooking, 0, 100 ) ? 5 : 0;
                        }
                    }
                    else
                    {
                        PublicOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( "Fermentation process: {0} %", m_FermentationState ) );
                    }
                }
                else
                {
                    if( m_FromBonusSkill < 10 )
                        PublicOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( "Fermentation failed, the milk is lost." ) );
                    else
                        DoCheese( from, ( m_FromBonusSkill > 95 ) && Utility.RandomBool() );
                }
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            string milkName = CheeseCrafingHelper.GetMilkName( m_MilkType );

            if( m_MilkType != MilkTypes.None )
            {
                if( m_HasMilk )
                    list.Add( 1060658, "Type\t{0}", milkName ); // ~1_val~ ~2_val~

                if( IsFermenting )
                    list.Add( "Fermenting" );
            }
        }

        #region serialization
        public CheeseForm( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
            writer.Write( (int)m_MilkType );
            writer.Write( m_CheeseQuality );
            writer.Write( m_FermentationState );
            writer.Write( m_HasMilk );
            writer.Write( m_HasCheese );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
            switch( version )
            {
                case 0:
                    {
                        m_MilkType = (MilkTypes)reader.ReadInt();
                        m_CheeseQuality = reader.ReadInt();
                        m_FermentationState = reader.ReadInt();
                        m_HasMilk = reader.ReadBool();
                        m_HasCheese = reader.ReadBool();

                        if( m_MilkType > MilkTypes.None && !m_HasCheese )
                        {
                            m_FermentationTimer = new InternalTimer( this );
                            m_FermentationTimer.Start();
                        }

                        break;
                    }
            }
        }
        #endregion

        public class InternalTimer : Timer
        {
            private CheeseForm m_CheeseForm;

            public InternalTimer( CheeseForm form )
                : base( TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 72 ), 100 )
            {
                Priority = TimerPriority.FiftyMS;

                m_CheeseForm = form;
            }

            protected override void OnTick()
            {
                if( m_CheeseForm.FermentationState <= 99 )
                {
                    ++m_CheeseForm.FermentationState;
                }
                else
                {
                    if( m_CheeseForm != null )
                    {
                        m_CheeseForm.OnFermentationEnded();
                    }

                    if( Running )
                        Stop();
                }
            }
        }
    }
}