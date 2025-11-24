using System;

using Midgard.Items;

using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

namespace Midgard.Misc
{
    public class Easter2010 : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register( new Easter2010() );
        }

        public override TimeSpan MinimumAge
        {
            get { return TimeSpan.Zero; }
        }

        public override DateTime Start
        {
            get { return new DateTime( 2010, 4, 4, 9, 15, 0 ); }
        }

        public override DateTime Finish
        {
            get { return new DateTime( 2010, 4, 5, 0, 0, 0 ); }
        }

        public override void GiveGift( Mobile mob )
        {
            EasterBasket2010 box = new EasterBasket2010();
            box.DropItem( new EasterCard2010( string.Format( "Buona Pasqua da {0}", mob.Name ) ) );

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
    public class EasterBasket2010 : Container
    {
        public override bool DisplaysContent
        {
            get { return false; }
        }

        public override string DefaultName
        {
            get { return "Cesta Pasquale 2010"; }
        }

        [Constructable]
        public EasterBasket2010()
            : base( 0x990 )
        {
            Hue = Utility.RandomList( 0x36, 0x17, 0x120 );
            Weight = 1.0;

            if( Utility.RandomBool() )
            {
                DropItem( new EasterCarrot2010() );
            }
            else
            {
                DropItem( new EasterEggs2010() );
            }

            if( Utility.RandomBool() )
            {
                DropItem( new ChocolateEasterBunny2010() );
            }
            else
            {
                DropItem( new DeadRabbit2010() );
            }

            if( Utility.RandomDouble() < 0.1 )
            {
                DropItem( new BunnyIdol2010() );
            }
        }

        public EasterBasket2010( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua 2010 dallo Staff di Midgard!" );
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

    public class EasterCarrot2010 : Food
    {
        public override string DefaultName
        {
            get { return "Carotina del coniglio pasquale"; }
        }

        [Constructable]
        public EasterCarrot2010()
            : base( 0xC78 )
        {
            Weight = 1.0;
        }

        public EasterCarrot2010( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua 2010 dallo Staff di Midgard!" );
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

    public class EasterEggs2010 : EasterEggs
    {
        public override string DefaultName
        {
            get { return "Uova pasquali"; }
        }

        [Constructable]
        public EasterEggs2010()
        {
        }

        public EasterEggs2010( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua 2010 dallo Staff di Midgard!" );
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

        public override Food Cook()
        {
            return new FriedEggs();
        }
    }

    public class ChocolateEasterBunny2010 : Food
    {
        public override string DefaultName
        {
            get { return "Coniglietto cioccolatoso"; }
        }

        [Constructable]
        public ChocolateEasterBunny2010()
            : base( 0x2125 )
        {
            Hue = Utility.RandomList( 0x156, 0x21E, 0x71A );
            Weight = 1.0;
        }

        public ChocolateEasterBunny2010( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua 2010 dallo Staff di Midgard!" );
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

    public class DeadRabbit2010 : Food
    {
        public override string DefaultName
        {
            get { return "Coniglietto di Pasqua"; }
        }

        [Constructable]
        public DeadRabbit2010()
            : base( 15723 )
        {
            Weight = 1.0;
        }

        public DeadRabbit2010( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua 2010 dallo Staff di Midgard!" );
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

    public class BunnyIdol2010 : Item
    {
        public override string DefaultName
        {
            get { return "Idolo di un coniglietto pasquale"; }
        }

        [Constructable]
        public BunnyIdol2010()
            : base( 8485 )
        {
            Hue = Utility.RandomGreenHue();
            Weight = 1.0;
        }

        public BunnyIdol2010( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua 2010 dallo Staff di Midgard!" );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !from.InRange( GetWorldLocation(), 2 ) )
            {
                from.SendLocalizedMessage( 500446 ); // That is too far away.
            }
            else
            {
                Polymorph( from );
            }
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
            {
                mount.Rider = null;
            }

            if( m.Mounted )
            {
                return;
            }

            if( m.BeginAction( typeof( PolymorphSpell ) ) )
            {
                Item disarm = m.FindItemOnLayer( Layer.OneHanded );

                if( disarm != null && disarm.Movable )
                {
                    m.AddToBackpack( disarm );
                }

                disarm = m.FindItemOnLayer( Layer.TwoHanded );

                if( disarm != null && disarm.Movable )
                {
                    m.AddToBackpack( disarm );
                }

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

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class EasterCard2010 : Item
    {
        [Constructable]
        public EasterCard2010( string name )
            : base( 0x14EF )
        {
            Name = name;
            Hue = Utility.RandomList( 0x36, 0x17, 0x120 );
            Weight = 1.0;
        }

        public EasterCard2010( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua 2010 dallo Staff di Midgard!" );
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
}