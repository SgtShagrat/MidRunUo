using System;

using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;

namespace Midgard.Items
{
    public abstract class BaseHomericMightPotion : BasePaganPotion
    {
        public override int DelayUse
        {
            get { return 14; }
        }

        public override int BonusOnDelayAtHundred
        {
            get { return 6; }
        }

        public override int CustomSound
        {
            get { return 0x01eb; }
        }

        public override int CustomAnim
        {
            get { return 0x0022; }
        }

        public override int CustomEffects
        {
            get { return 0x373a; }
        }

        protected BaseHomericMightPotion( PotionEffect effect, int amount )
            : base( 0xF0E, effect, amount )
        {
        }

        public override bool DoPaganEffect( Mobile from )
        {
            /*
            function DoBlessEffect(me,strength)

                var mod_amount := RandomDiceRoll(cstr(strength)+"d2");
                var duration := cint(strength)*60;

                if (CanMod(me, "poly")=0)
                    SendSysMessage(me,"You are cannot use while polmorphed.");
                    return;
                endif

                if (CanMod(me, "bstr")=0)
                    SendSysMessage(me,"You are already under influence.");
                else
                    DoTempMod(me, "bstr", mod_amount, duration);
                endif
                if (CanMod(me, "bint")=0)
                    SendSysMessage(me,"You are already under influence.");
                else
                    DoTempMod(me, "bint", mod_amount, duration);
                endif
                if (CanMod(me, "bdex")=0)
                    SendSysMessage(me,"You are already under influence.");
                else
                    DoTempMod(me, "bdex", mod_amount, duration);
                endif

            endfunction
            */

            if( !from.CanBeginAction( typeof( PolymorphSpell ) ) )
            {
                from.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
                return false;
            }

            if( SpellHelper.AddStatOffset( from, StatType.All, Utility.Dice( Strength, 2, 0 ), TimeSpan.FromSeconds( Strength * 15 ) ) )
            {
                LockBasePotionUse( from );
                return true;
            }
            else
            {
                from.SendMessage( "You are already under influence." );
                return false;
            }
        }

        #region serial deserial
        public BaseHomericMightPotion( Serial serial )
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