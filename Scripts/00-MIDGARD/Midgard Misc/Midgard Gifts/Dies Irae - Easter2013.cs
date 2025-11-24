/***************************************************************************
 *                                  Easter2008.cs
 *                            		-------------
 *  begin                	: Marzo, 2013
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;

using Midgard.Items;
using Midgard.Misc;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

namespace Midgard.Misc
{
    public class Easter2013 : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register( new Easter2013() );
        }

        public const string Greeting = "Buona Pasqua dallo Staff di Midgard Third Crown!";

        public override TimeSpan MinimumAge
        {
            get { return TimeSpan.Zero; }
        }

        public override DateTime Start
        {
            get { return new DateTime( 2013, 3, 31, 13, 00, 0 ); }
        }

        public override DateTime Finish
        {
            get { return new DateTime( 2013, 4, 1, 23, 59, 0 ); }
        }

        public override void GiveGift( Mobile mob )
        {
            EasterBasket2013 box = new EasterBasket2013( mob );
            box.DropItem( new EasterCard2013( string.Format( "Buona Pasqua da {0}!", mob.Name ) ) );

            switch( GiveGift( mob, box ) )
            {
                case GiftResult.Backpack:
                    mob.SendMessage( 0x482, "Buona Pasqua dallo staff di Midgard! Cerca un piccolo regalo nel tuo zaino." );
                    break;
                case GiftResult.BankBox:
                    mob.SendMessage( 0x482, "Buona Pasqua dallo staff di Midgard! Cerca un piccolo regalo nella tua banca." );
                    break;
            }
        }
    }
}

namespace Midgard.Items
{
    public sealed class EasterBasket2013 : Container
    {
        public override bool DisplaysContent
        {
            get { return false; }
        }

        public override string DefaultName
        {
            get { return "Pasqua 2013"; }
        }

        [Constructable]
        public EasterBasket2013()
            : this( null )
        {
        }

        [Constructable]
        public EasterBasket2013( Mobile m )
            : base( 0x990 )
        {
            Hue = Utility.RandomBlueHue();
            Weight = 1.0;

            bool gm = m != null && m.AccessLevel >= AccessLevel.Player;

            DropItem( Utility.RandomBool() || gm ? (Item)new EasterCarrot2013() : new EasterEggs2013() );

            DropItem( Utility.RandomBool() || gm ? (Item)new ChocolateEasterBunny2013() : new DeadRabbit2013() );

            if( Utility.RandomDouble() < 0.1 || gm )
                DropItem( new BunnyIdol2013() );

            DropItem( new EasterRum2013() );
        }

        public EasterBasket2013( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, Easter2013.Greeting );
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
    }

    public class EasterCarrot2013 : Food
    {
        public override string DefaultName
        {
            get { return "carota del coniglietto"; }
        }

        [Constructable]
        public EasterCarrot2013()
            : base( 0xC78 )
        {
            Weight = 1.0;
        }

        public EasterCarrot2013( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, Easter2013.Greeting );
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
    }

    public class EasterEggs2013 : EasterEggs
    {
        public override string DefaultName
        {
            get { return "uova del coniglietto"; }
        }

        [Constructable]
        public EasterEggs2013()
        {
        }

        public EasterEggs2013( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, Easter2013.Greeting );
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

        public override Food Cook()
        {
            return new FriedEggs();
        }
    }

    public class ChocolateEasterBunny2013 : Food
    {
        public override string DefaultName
        {
            get { return "coniglietto di cioccolata fondente"; }
        }

        [Constructable]
        public ChocolateEasterBunny2013()
            : base( 0x2125 )
        {
            Hue = Utility.RandomList( 0x156, 0x21E, 0x71A );
            Weight = 1.0;
        }

        public ChocolateEasterBunny2013( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, Easter2013.Greeting );
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
    }

    public class DeadRabbit2013 : Food
    {
        public override string DefaultName
        {
            get { return "coniglietto pasquale"; }
        }

        [Constructable]
        public DeadRabbit2013()
            : base( 0x3d6b )
        {
            Weight = 1.0;
        }

        public DeadRabbit2013( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, Easter2013.Greeting );
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
    }

    public class BunnyIdol2013 : Item
    {
        public override string DefaultName
        {
            get { return "idolo di un coniglietto pasquale"; }
        }

        [Constructable]
        public BunnyIdol2013()
            : base( 8485 )
        {
            Hue = Utility.RandomGreenHue();
            Weight = 1.0;
        }

        public BunnyIdol2013( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, Easter2013.Greeting );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !from.InRange( GetWorldLocation(), 2 ) )
                from.SendLocalizedMessage( 500446 ); // That is too far away.
            else
                Polymorph( from );
        }

        public void Polymorph( Mobile m )
        {
            if( !m.CanBeginAction( typeof( PolymorphSpell ) ) || !m.CanBeginAction( typeof( IncognitoSpell ) ) || m.IsBodyMod )
            {
                m.SendMessage( "Non puoi usare ancora il coniglietto." );
                return;
            }

            IMount mount = m.Mount;

            if( mount != null )
                mount.Rider = null;

            if( m.Mounted )
                return;

            if( m.BeginAction( typeof( PolymorphSpell ) ) )
            {
                Item disarm = m.FindItemOnLayer( Layer.OneHanded );

                if( disarm != null && disarm.Movable )
                    m.AddToBackpack( disarm );

                disarm = m.FindItemOnLayer( Layer.TwoHanded );

                if( disarm != null && disarm.Movable )
                    m.AddToBackpack( disarm );

                m.BodyMod = 205;
                m.HueMod = 0;
                m.PlaySound( 0x247 );

                Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 );
                Effects.SendLocationParticles( EffectItem.Create( new Point3D( m.X, m.Y, m.Z - 7 ), m.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 );

                new ExpirePolymorphTimer( m ).Start();
            }
        }

        private class ExpirePolymorphTimer : Timer
        {
            private readonly Mobile m_Owner;

            public ExpirePolymorphTimer( Mobile owner )
                : base( TimeSpan.FromMinutes( 3.0 ) )
            {
                m_Owner = owner;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if( !m_Owner.CanBeginAction( typeof( PolymorphSpell ) ) )
                {
                    m_Owner.BodyMod = 0;
                    m_Owner.HueMod = -1;
                    m_Owner.EndAction( typeof( PolymorphSpell ) );

                    m_Owner.PlaySound( 0x247 );
                    Effects.SendLocationParticles( EffectItem.Create( m_Owner.Location, m_Owner.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 );
                    Effects.SendLocationParticles( EffectItem.Create( new Point3D( m_Owner.X, m_Owner.Y, m_Owner.Z - 7 ), m_Owner.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 );
                }
            }
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
    }

    public class EasterCard2013 : Item
    {
        [Constructable]
        public EasterCard2013( string name )
            : base( 0x14EF )
        {
            Name = name;
            Hue = Utility.RandomNeutralHue();
            Weight = 1.0;
        }

        public EasterCard2013( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, Easter2013.Greeting );
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
    }

    public class EasterRum2013 : BeverageBottle
    {
        public override string DefaultName
        {
            get { return "uno speziato liquore pasquale"; }
        }

        [Constructable]
        public EasterRum2013()
            : base( BeverageType.Liquor )
        {
            Hue = 0x66C;
        }

        public EasterRum2013( Serial serial )
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
        }
    }
}