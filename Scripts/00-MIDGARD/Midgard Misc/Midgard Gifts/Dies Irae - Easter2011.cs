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
    public class Easter2011 : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register( new Easter2011() );
        }

        public override TimeSpan MinimumAge
        {
            get { return TimeSpan.Zero; }
        }

        public override DateTime Start
        {
            get { return new DateTime( 2011, 4, 24, 13, 30, 0 ); }
        }

        public override DateTime Finish
        {
            get { return new DateTime( 2011, 4, 26, 0, 1, 0 ); }
        }

        public override void GiveGift( Mobile mob )
        {
            EasterBasket2011 box = new EasterBasket2011();
            box.DropItem( new EasterCard2011( string.Format( "Buona Pasqua da {0}", mob.Name ) ) );

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
    public class EasterBasket2011 : Container
    {
        public override bool DisplaysContent
        {
            get { return false; }
        }

        public override string DefaultName
        {
            get { return "Pasqua 2011"; }
        }

        [Constructable]
        public EasterBasket2011()
            : base( 0x990 )
        {
            Hue = Utility.RandomPinkHue();
            Weight = 1.0;

            if( Utility.RandomBool() )
            {
                DropItem( new EasterCarrot2011() );
            }
            else
            {
                DropItem( new EasterEggs2011() );
            }

            if( Utility.RandomBool() )
            {
                DropItem( new ChocolateEasterBunny2011() );
            }
            else
            {
                DropItem( new DeadRabbit2011() );
            }

            if( Utility.RandomDouble() < 0.1 )
            {
                DropItem( new BunnyIdol2011() );
            }
        }

        public EasterBasket2011( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua dallo Staff di Midgard!" );
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

    public class EasterCarrot2011 : Food
    {
        public override string DefaultName
        {
            get { return "carotina golosa"; }
        }

        [Constructable]
        public EasterCarrot2011()
            : base( 0xC78 )
        {
            Weight = 1.0;
        }

        public EasterCarrot2011( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua dallo Staff di Midgard!" );
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

    public class EasterEggs2011 : EasterEggs
    {
        public override string DefaultName
        {
            get { return "uova colorate"; }
        }

        [Constructable]
        public EasterEggs2011()
        {
        }

        public EasterEggs2011( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua dallo Staff di Midgard!" );
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

    public class ChocolateEasterBunny2011 : Food
    {
        public override string DefaultName
        {
            get { return "coniglietto di cioccolata"; }
        }

        [Constructable]
        public ChocolateEasterBunny2011()
            : base( 0x2125 )
        {
            Hue = Utility.RandomList( 0x156, 0x21E, 0x71A );
            Weight = 1.0;
        }

        public ChocolateEasterBunny2011( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua dallo Staff di Midgard!" );
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

    public class DeadRabbit2011 : Food
    {
        public override string DefaultName
        {
            get { return "coniglietto pasquale"; }
        }

        [Constructable]
        public DeadRabbit2011()
            : base( 15723 )
        {
            Weight = 1.0;
        }

        public DeadRabbit2011( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua dallo Staff di Midgard!" );
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

    public class BunnyIdol2011 : Item
    {
        public override string DefaultName
        {
            get { return "idolo di un coniglietto goloso"; }
        }

        [Constructable]
        public BunnyIdol2011()
            : base( 8485 )
        {
            Hue = Utility.RandomBlueHue();
            Weight = 1.0;
        }

        public BunnyIdol2011( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua dallo Staff di Midgard!" );
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

    public class EasterCard2011 : Item
    {
        [Constructable]
        public EasterCard2011( string name )
            : base( 0x14EF )
        {
            Name = name;
            Hue = Utility.RandomMetalHue();
            Weight = 1.0;
        }

        public EasterCard2011( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Buona Pasqua dallo Staff di Midgard!" );
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