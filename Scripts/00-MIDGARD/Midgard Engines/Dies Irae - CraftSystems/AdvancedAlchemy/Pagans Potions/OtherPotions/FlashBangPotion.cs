using System;
using System.Collections.Generic;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class FlashBangPotion : BasePaganPotion
    {
        public const int Level = 8;
        public const double PercProperFun = 0.75;
        public const int Range = 8;
        public const int Duration = 10;
        public static readonly bool StaticFlash = true; // se true la fiamma è solo un frame

        public override int DelayUse
        {
            get { return 14; }
        }

        [Constructable]
        public FlashBangPotion( int amount )
            : base( PotionEffect.FlashBang, amount )
        {
            // Name = "Morderkainen's FlashBang Potion";
            Hue = 2548;
        }

        [Constructable]
        public FlashBangPotion()
            : this( 1 )
        {
        }

        public override int Strength
        {
            get { return 0; }
        }

        public override bool DoPaganEffect( Mobile from )
        {
            // Se funziona correttamente hidda il pg
            from.SendMessage( "La pozione FlashBang ha funzionato correttamente." );

            // La durata finale e' la base scalata con EP
            int dur = Scale( from, Duration );

            // Il raggio finale e' scalato con EP
            int ran = Scale( from, Range );

            // Crea l'area che conterrà i flash
            List<Static> colonne = new List<Static>();

            // Crea il confine dell'area d'effetto
            Rectangle2D areaFlashBang = new Rectangle2D( from.X - ran, from.Y - ran, ran * 2 + 1, ran * 2 + 1 );

            // Per ogni punto 3d nell'area addalo alla lista di possibili target del flash
            for( int x = areaFlashBang.Start.X; x < areaFlashBang.End.X; x++ )
            {
                for( int y = areaFlashBang.Start.Y; y < areaFlashBang.End.Y; y++ )
                {
                    Static s;
                    if( StaticFlash )
                    {
                        s = new Static( 14108 );
                    }
                    else
                    {
                        s = new Static( 14089 );
                    }
                    s.Hue = 1154;
                    s.MoveToWorld( new Point3D( x, y, from.Z ), from.Map );
                    colonne.Add( s );
                }
            }

            // Frozza i pg nell'area per 3 secondi
            List<Mobile> banged = new List<Mobile>();
            foreach( Mobile m in from.GetMobilesInRange( ran ) )
            {
                if( from != m )
                {
                    banged.Add( m );
                    m.SendMessage( "Sei accecato e stordito dall'esplosione!" );
                    m.Paralyze( TimeSpan.FromSeconds( 3 ) );
                }
            }

            // Fa partire il timer per la durata della FlashBang
            Timer t = new InternalTimer( from, TimeSpan.FromSeconds( dur ), colonne );
            t.Start();

            return true;
        }

        #region serialization
        public FlashBangPotion( Serial serial )
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

        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;
            private readonly List<Static> m_Colonne;

            public InternalTimer( Mobile from, TimeSpan duration, List<Static> colonne )
                : base( duration )
            {
                Priority = TimerPriority.OneSecond;

                m_From = from;
                m_Colonne = colonne;
            }

            protected override void OnTick()
            {
                if( m_From != null )
                {
                    m_From.SendMessage( "La pozione FlahBang si esaurisce!" );
                    foreach( Static s in m_Colonne )
                    {
                        s.Delete();
                    }

                    Stop();
                }
            }
        }
    }
}