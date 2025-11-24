/***************************************************************************
 *                               BasePhandelsIntellectPotion
 *                            ---------------------------------
 *   begin                : 24 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;

namespace Midgard.Items
{
    public abstract class BasePhandelsIntellectPotion : BasePaganPotion
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
            get { return 0x01ec; }
        }

        public override int CustomAnim
        {
            get { return 0x0022; }
        }

        public override int CustomEffects
        {
            get { return 0x373a; }
        }

        public BasePhandelsIntellectPotion( PotionEffect effect, int amount )
            : base( 0xF0E, effect, amount )
        {
        }

        public override bool DoPaganEffect( Mobile from )
        {
            /*
            function DoCunningEffect(me,strength)

                if (CanMod(me, "bint")=0)
                    SendSysMessage(me,"You are already under influence.");
                    return;
                endif
                if (CanMod(me, "poly")=0)
                    SendSysMessage(me,"You are cannot use while polmorphed.");
                    return;
                endif
                var mod_amount := RandomDiceRoll(cstr(strength)+"d2");
                var duration := cint(strength)*60;
                DoTempMod(me, "bint", mod_amount, duration);

            endfunction
            */

            if( !from.CanBeginAction( typeof( PolymorphSpell ) ) )
            {
                from.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
                return false;
            }

            int intOffset = Utility.Dice( Strength, 2, 0 );
            int duration = Strength * 60;

            if( SpellHelper.AddStatOffset( from, StatType.Int, intOffset, TimeSpan.FromSeconds( duration ) ) )
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
        public BasePhandelsIntellectPotion( Serial serial )
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