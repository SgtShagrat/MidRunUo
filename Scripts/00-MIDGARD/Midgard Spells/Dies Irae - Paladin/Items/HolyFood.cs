/***************************************************************************
 *                               HolyFood.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.SpellSystem
{
    public abstract class HolyFood : BaseMagicalFood
    {
        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromMinutes( 2 ); }
        }

        public TimeSpan ShortCooldown
        {
            get { return TimeSpan.FromSeconds( 30 ); }
        }

        public override TimeSpan GetDuration( Mobile from )
        {
            return TimeSpan.FromSeconds( RPGPaladinSpell.GetPowerValueScaledByType( from, typeof( SacredFeastSpell ) ) / 2.0 );
        }

        public abstract bool DoInstantEffect( Mobile from, int level );
        public abstract bool DoLastingEffect( Mobile from, TimeSpan duration, int level );

        public HolyFood( int itemID )
            : base( itemID )
        {
            Weight = 1.0;
            Hue = 0x482;
            FillFactor = 3;
        }

        public HolyFood( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from == null || from.Deleted )
                return;

            if( RPGPaladinSpell.IsSuperVulnerable( from ) )
            {
                from.PublicOverheadMessage( MessageType.Regular, 17, true, "* Your evil soul burns! *" );
                AOS.Damage( from, Utility.RandomMinMax( 40, 120 ), true, 0, 0, 0, 0, 100 );
                from.PlaySound( 0x1E0 );
                from.BoltEffect( 0x480 );
                return;
            }

            base.OnDoubleClick( from );
        }

        public override bool Eat( Mobile from )
        {
            if( !CoolingDown( from, FoodID ) )
            {
                from.SendLocalizedMessage( 1070772 ); // You must wait a few seconds before you can use that item.
                return false;
            }

            bool consume = false;
            bool startCoolDown = false;

            int level = RPGSpellsSystem.GetPowerLevel( from, typeof( SacredFeastSpell ) );

            if( DoInstantEffect( from, level ) )
            {
                consume = true;
                startCoolDown = true;
            }

            if( level >= 3 )
            {
                if( DoLastingEffect( from, GetDuration( from ), level ) )
                {
                    consume = true;
                    startCoolDown = false; // StartInfluence will call Cooldown itself

                    from.SendMessage( "Holy food effect will last for {0} seconds.", GetDuration( from ).TotalSeconds );

                    StartInfluence( from, FoodID, GetDuration( from ), Cooldown );
                }
                else
                    from.SendMessage( "You are already under holy food influence." );
            }

            // this cooldown is for istant use of magical food
            if( startCoolDown )
            {
                Timer.DelayCall( ShortCooldown, new TimerStateCallback( EndCooldown ), new object[] { from, FoodID } );
            }

            if( FillHunger( from, FillFactor ) || consume )
            {
                Consume();
                return true;
            }
            else
                return false;
        }

        #region serial-deserial
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

    public class HolyApple : HolyFood
    {
        public override MagicalFood FoodID
        {
            get { return MagicalFood.HolyApple; }
        }

        public override object EatMessage
        {
            get { return "This apple has enhanched your dexterity!"; }
        }

        public override bool DoInstantEffect( Mobile from, int level )
        {
            if( from.Stam < from.StamMax )
            {
                int restore = DiceRoll.Roll( level + "d10+5" );
                from.Stam = ( from.Mana + restore > from.StamMax ? from.StamMax : from.Stam + restore );
                from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
                from.PlaySound( 0x202 );
                from.SendMessage( "Some stamina has been restored." );
                return true;
            }
            else
                return false;
        }

        public override bool DoLastingEffect( Mobile from, TimeSpan duration, int level )
        {
            if( from.GetStatMod( "[Holy Apple]" ) != null )
                return false;

            from.AddStatMod( new StatMod( StatType.Dex, "[Holy Apple]", (int)( from.RawDex * 0.08 ), GetDuration( from ) ) );
            from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
            from.PlaySound( 0x202 );
            from.SendMessage( "The holy food enhance your dexterity for a limited amount of time." );

            return true;
        }

        public override string DefaultName
        {
            get { return "holy apple"; }
        }

        [Constructable]
        public HolyApple()
            : base( 0x9D0 )
        {
            Weight = 1.0;
            FillFactor = 3;
        }

        public HolyApple( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    public class HolyBread : HolyFood
    {
        public override MagicalFood FoodID
        {
            get { return MagicalFood.HolyBread; }
        }

        public override object EatMessage
        {
            get { return "This holy bread has raised your strength!"; }
        }

        public override bool DoInstantEffect( Mobile from, int level )
        {
            if( from.Hits < from.HitsMax )
            {
                from.Heal( DiceRoll.Roll( level + "d10+5" ) );
                from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
                from.PlaySound( 0x202 );
                from.SendMessage( "Some damage has been healed." );

                return true;
            }
            else
                return false;
        }

        public override bool DoLastingEffect( Mobile from, TimeSpan duration, int level )
        {
            if( from.GetStatMod( "[Holy Bread]" ) != null )
                return false;

            from.AddStatMod( new StatMod( StatType.Str, "[Holy Bread]", (int)( from.RawStr * 0.08 ), GetDuration( from ) ) );
            from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
            from.PlaySound( 0x202 );
            from.SendMessage( "The holy food enhance your strength for a limited amount of time." );

            return true;
        }

        public override string DefaultName
        {
            get { return "holy bread"; }
        }

        [Constructable]
        public HolyBread()
            : base( 0x103B )
        {
            FillFactor = 3;
        }

        public HolyBread( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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

    public class HolyGrapes : HolyFood
    {
        public override MagicalFood FoodID
        {
            get { return MagicalFood.HolyGrapes; }
        }

        public override object EatMessage
        {
            get { return "This grape has enhanched your intelligence!"; }
        }

        public override bool DoInstantEffect( Mobile from, int level )
        {
            if( from.Mana < from.ManaMax )
            {
                int restore = DiceRoll.Roll( level + "d10+5" );
                from.Mana = ( from.Mana + restore > from.ManaMax ? from.ManaMax : from.Mana + restore );
                from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
                from.PlaySound( 0x202 );
                from.SendMessage( "Some mana has been refilled." );

                return true;
            }
            else
                return false;
        }

        public override bool DoLastingEffect( Mobile from, TimeSpan duration, int level )
        {
            if( from.GetStatMod( "[Holy Grapes]" ) != null )
                return false;

            from.AddStatMod( new StatMod( StatType.Int, "[Holy Grapes]", (int)( from.RawInt * 0.08 ), GetDuration( from ) ) );
            from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
            from.PlaySound( 0x202 );
            from.SendMessage( "The holy food enhance your intelligence for a limited amount of time." );

            return true;
        }

        public override string DefaultName
        {
            get { return "holy grapes"; }
        }

        [Constructable]
        public HolyGrapes()
            : base( 0x9D1 )
        {
            FillFactor = 3;
        }

        public HolyGrapes( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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