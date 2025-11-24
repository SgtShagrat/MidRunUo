/***************************************************************************
 *                                  Easter2008.cs
 *                            		-------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Pacco dono per Pasqua 2008.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using Midgard.Items;

namespace Midgard.Misc
{
    public class Easter2008 : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register( new Easter2008() );
        }

        public override DateTime Start{ get{ return new DateTime( 2008, 3, 23 ); } }
        public override DateTime Finish{ get{ return new DateTime( 2008, 3, 24 ); } }

        public override void GiveGift( Mobile mob )
        {
            EasterBasket2008 box = new EasterBasket2008();

            box.Name = "Pasqua 2008";
            box.Hue = Utility.RandomBlueHue();

            if( Utility.RandomBool() )
                box.DropItem( new EasterCarrot2008() );
            else
                box.DropItem( new EasterEggs2008() );

            if( Utility.RandomBool() )
                box.DropItem( new ChocolateEasterBunny2008() );
            else
                box.DropItem( new DeadRabbit2008() );

            if( Utility.RandomDouble() < 0.1 )
                box.DropItem( new BunnyIdol2008() );
            
            box.DropItem( new EasterCard2008( "Buona Pasqua da " + mob.Name ) );

            switch ( GiveGift( mob, box ) )
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
    public class EasterBasket2008 : Container
    {
        [Constructable]
        public EasterBasket2008() : base( 0x990 )
        {
            Name = "Easter Basket";
            Hue = Utility.RandomList( 0x36, 0x17, 0x120 );
            Weight = 1.0;
        }

        public EasterBasket2008( Serial serial ) : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Buona Pasqua 2008 dallo Staff di Midgard!" );
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

    public class EasterCarrot2008 : Food
    {
        [Constructable]
        public EasterCarrot2008() : base( 0xC78 )
        {
            Name = "Carotina del coniglio pasquale";
            Weight = 1.0;
        }

        public EasterCarrot2008( Serial serial ) : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Buona Pasqua 2008 dallo Staff di Midgard!" );
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

    public class EasterEggs2008 : EasterEggs
    {
        [Constructable]
        public EasterEggs2008()
        {
            Name = "Uova pasquali";
            Weight = 1.0;
        }

        public EasterEggs2008( Serial serial ) : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Buona Pasqua 2008 dallo Staff di Midgard!" );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
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

    public class ChocolateEasterBunny2008 : Food
    {
        [Constructable]
        public ChocolateEasterBunny2008() : base( 0x2125 )
        {
            Name = "Coniglietto cioccolatoso";
            Hue = Utility.RandomList( 0x156, 0x21E, 0x71A );
            Weight = 1.0;
        }

        public ChocolateEasterBunny2008( Serial serial ) : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Buona Pasqua 2008 dallo Staff di Midgard!" );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class DeadRabbit2008 : Food
    {
        [Constructable]
        public DeadRabbit2008() : base( 15723 )
        {
            Name = "Coniglietto di Pasqua";
            Weight = 1.0;
        }

        public DeadRabbit2008( Serial serial ) : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Buona Pasqua 2008 dallo Staff di Midgard!" );
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
    
    public class BunnyIdol2008 : Item
    {
        [Constructable]
        public BunnyIdol2008() : base( 8485 )
        {
            Name = "Idolo di un coniglietto pasquale";
            Hue = Utility.RandomGreenHue();
            Weight = 1.0;
        }

        public BunnyIdol2008( Serial serial ) : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Buona Pasqua 2008 dallo Staff di Midgard!" );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if ( !from.InRange( GetWorldLocation(), 2 ) )
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

                if ( disarm != null && disarm.Movable )
                    m.AddToBackpack( disarm );

                disarm = m.FindItemOnLayer( Layer.TwoHanded );

                if ( disarm != null && disarm.Movable )
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
            private Mobile m_Owner;

            public ExpirePolymorphTimer( Mobile owner ) : base( TimeSpan.FromMinutes( 3.0 ) )
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

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class EasterCard2008 : Item
    {
        [Constructable]
        public EasterCard2008( string name ) : base( 0x14EF )
        {
            Name = name;
            Hue = Utility.RandomList( 0x36, 0x17, 0x120 );
            Weight = 1.0;
        }

        public EasterCard2008( Serial serial ) : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Buona Pasqua 2008 dallo Staff di Midgard!" );
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