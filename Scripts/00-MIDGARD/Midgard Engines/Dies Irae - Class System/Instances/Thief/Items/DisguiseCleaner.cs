/***************************************************************************
 *                                  DisguiseCleaner.cs
 *                            		------------------
 *  begin                	: Febbraio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Tool per rimuovere l'effetto del disguise kit.
 * 
 ***************************************************************************/

using System;

using Midgard.Engines.AdvancedDisguise;
using Midgard.Engines.Classes;

using Server.Mobiles;

namespace Server.Items
{
    public class DisguiseCleaner : Item
    {
        public override string DefaultName
        {
            get { return "a Disguise Cleaner"; }
        }

        [Constructable]
        public DisguiseCleaner()
            : base( 0x2FD6 )
        {
            Weight = 1.0;
            Hue = 0x482;
        }

        public DisguiseCleaner( Serial serial )
            : base( serial )
        {
        }

        public bool ValidateUse( Mobile from )
        {
            PlayerMobile pm = from as PlayerMobile;
            if( pm == null )
                return false;

            if( !IsChildOf( pm.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
            else if( !DisguiseTimers.IsDisguised( from ) && !SketchGump.IsCamuflated( from ) )
            {
                from.SendMessage( "Your face is so clear, isn't it?" );
            }
            else
            {
                return true;
            }

            return false;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( ValidateUse( from ) )
            {
                from.SendMessage( "You clean away your alias and return a normal... thief!" );
                DisguiseTimers.RemoveTimer( from );
                Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( SketchGump.OnDisguiseExpire ), from );
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
}