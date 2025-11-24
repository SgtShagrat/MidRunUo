using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public abstract class BasePetHealthPotion : BasePotion
    {
        public abstract double PercProperFun { get; }

        public BasePetHealthPotion( PotionEffect effect, int amount )
            : base( 0xF0E, effect, amount )
        {
        }

        public BasePetHealthPotion()
            : this( 1 )
        {
        }

        public BasePetHealthPotion( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber { get { return 1064000 + (int)PotionEffect; } } // da 1064165 a 1064169

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
                return;
            }

            base.OnDoubleClick( from );
        }

        public abstract void DoPetHealthEffect( Mobile user, BaseCreature pet );

        public override void Drink( Mobile from )
        {
            if( from.BeginAction( typeof( BasePetHealthPotion ) ) )
            {
                // La percentuale che la pozione funzioni è scalabile con EP
                double chance = Scale( from, PercProperFun );

                // In ogni caso fai partire il timer dell' UseDelay
                Timer.DelayCall( TimeSpan.FromSeconds( DelayUse ), new TimerStateCallback( ReleasePetHealthLock ), from );

                if( chance > Utility.RandomDouble() )
                {
                    from.SendMessage( "Seleziona l'animale (TUO) sul quale vuoi usare la pozione." );
                    from.Target = new PetHealthTarget( this );
                }
                else
                {
                    // Se non funziona spreca la pozione fa partire il delaytimer per l'uso successivo e non colora il pet
                    from.SendMessage( "La pozione per animali non funziona e viene distrutta." );
                    Consume();
                }
            }
            else
            {
                // Se il lock all'uso non e' disabilitato...
                from.SendMessage( "Non puoi ancora usare un'altra pozione per animali!" );
            }
        }

        private static void ReleasePetHealthLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( BasePetHealthPotion ) );
        }

        #region serial deserial
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

        private class PetHealthTarget : Target
        {
            private BasePetHealthPotion m_Bphp;

            public PetHealthTarget( BasePetHealthPotion bphp )
                : base( 5, false, TargetFlags.None )
            {
                m_Bphp = bphp;
            }

            protected override void OnTarget( Mobile from, object target )
            {
                if( target == from )
                {
                    from.SendMessage( "Non puoi usare questa pozione per animali su te stesso." );
                    return;
                }
                else if( target is PlayerMobile )
                {
                    from.SendMessage( "Non puoi usare questa pozione per animali su altre persone." );
                }
                else if( target is Item )
                {
                    from.SendMessage( "Non puoi usare questa pozione per animali su oggetti." );
                }
                else if( target is BaseCreature )
                {
                    BaseCreature bc = (BaseCreature)target;
                    if( bc.ControlMaster != from && bc.Controlled == false )
                    {
                        from.SendMessage( "Non puoi usare questa pozione per animali su un animale non tuo." );
                    }
                    else if( bc.Controlled && bc.ControlMaster == from )
                    {
                        m_Bphp.DoPetHealthEffect( from, bc );
                    }
                }
            }
        }
    }
}