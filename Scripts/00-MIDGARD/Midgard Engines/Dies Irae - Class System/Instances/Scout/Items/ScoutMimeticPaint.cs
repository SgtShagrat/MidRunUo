/***************************************************************************
 *                               ScoutMimeticPaint.cs
 *
 *   begin                : 08 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.Classes;
using Midgard.Engines.SpellSystem;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

namespace Midgard.Items
{
    public class ScoutMimeticPaint : Item
    {
        public override string DefaultName
        {
            get { return "scout mimetic paints"; }
        }

        [Constructable]
        public ScoutMimeticPaint()
            : base( 0x9EC )
        {
            Hue = Utility.RandomNeutralHue();
            Weight = 2.0;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsChildOf( from.Backpack ) )
            {
                if( !ClassSystem.IsScout( from ) )
                {
                    from.SendMessage( "Only scouts can use this item." );
                }
                else if( !from.CanBeginAction( typeof( IncognitoSpell ) ) )
                {
                    from.SendMessage( "You cannot use this paints while incognitoed." );
                }
                else if( !from.CanBeginAction( typeof( PolymorphSpell ) ) )
                {
                    from.SendMessage( "You cannot use this paints while polymorphed." );
                }
                else if( TransformationSpellHelper.UnderTransformation( from ) )
                {
                    from.SendMessage( "You cannot use this paints while polymorphed." );
                }
                else if( AnimalFormSpell.UnderTransformation( from ) )
                {
                    from.SendMessage( "You cannot use this paints while in that form." );
                }
                else if( from.IsBodyMod || from.FindItemOnLayer( Layer.Helm ) is OrcishKinMask )
                {
                    from.SendMessage( "You cannot use this paints now." );
                }
                else
                {
                    from.HueMod = Hue;

                    if( from is PlayerMobile )
                        ( (PlayerMobile)from ).SavagePaintExpiration = TimeSpan.FromDays( 7.0 );

                    from.SendMessage( "You mimetic session starts... now!" );

                    ExpireTimer timer = m_Dictionary[ from ];
                    if( timer != null )
                        RemovePaint( from, false );

                    timer = new ExpireTimer( from, DateTime.Now + TimeSpan.FromHours( 1.0 ) );
                    timer.Start();

                    m_Dictionary[ from ] = timer;

                    Consume();
                }
            }
            else
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
        }

        private static readonly Dictionary<Mobile, ExpireTimer> m_Dictionary = new Dictionary<Mobile, ExpireTimer>();

        public static void RemovePaint( Mobile from, bool message )
        {
            if( IsUnderMimetism( from ) )
            {
                m_Dictionary.Remove( from );

                if( message )
                    from.SendMessage( "Your mimetism effect vanishes."  );

                from.HueMod = -1;
            }
        }

        public static bool IsUnderMimetism( Mobile from )
        {
            return m_Dictionary != null && m_Dictionary.ContainsKey( from );
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Owner;
            private readonly DateTime m_End;

            public ExpireTimer( Mobile owner, DateTime end )
                : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
            {
                m_Owner = owner;
                m_End = end;

                Priority = TimerPriority.OneSecond;
            }

            public DateTime End
            {
                get { return m_End; }
            }

            protected override void OnTick()
            {
                if( m_Owner.Deleted || !m_Owner.Alive || DateTime.Now >= End )
                    RemovePaint( m_Owner, !m_Owner.Deleted );
            }
        }

        #region serialization
        public ScoutMimeticPaint( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( m_Dictionary.Keys.Count );
            foreach( KeyValuePair<Mobile, ExpireTimer> keyValuePair in m_Dictionary )
            {
                if( keyValuePair.Key == null )
                {
                    writer.Write( Serial.MinusOne );
                    writer.Write( DateTime.MinValue );
                }
                else
                {
                    writer.Write( keyValuePair.Key.Serial );

                    if( keyValuePair.Value != null )
                        writer.Write( keyValuePair.Value.End );
                    else
                        writer.Write( DateTime.MinValue );
                }
            }
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            int count = reader.ReadInt();
            for( int i = 0; i < count; i++ )
            {
                Mobile owner = null;
                Serial serial = reader.ReadInt();
                if( serial != Serial.MinusOne )
                    owner = World.FindMobile( serial );

                DateTime end = reader.ReadDateTime();
                if( owner != null && DateTime.Now < end )
                {
                    ExpireTimer timer = new ExpireTimer( owner, end );
                    timer.Start();

                    m_Dictionary[ owner ] = timer;
                }
            }
        }
        #endregion
    }
}