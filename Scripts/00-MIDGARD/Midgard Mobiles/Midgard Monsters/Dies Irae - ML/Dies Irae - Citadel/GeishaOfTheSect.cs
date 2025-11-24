/***************************************************************************
 *                                  GeishaOfTheSect.cs
 *                            		-------------------
 *  begin                	: Febbraio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a fan dancer corpse" )]
    public class GeishaOfTheSect : FanDancer
    {
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

        [Constructable]
        public GeishaOfTheSect()
        {
            Name = NameList.RandomName( "tokuno female" );
            Title = "geisha of the dragon's flame sect";
        }

        public GeishaOfTheSect( Serial serial )
            : base( serial )
        {
        }

        public override void OnThink()
        {
            DoSpeach( this );
        }

        public static readonly double ChanceToSpeak = 0.01;

        public static void DoSpeach( BaseCreature bc )
        {
            if( bc.Combatant != null && ChanceToSpeak > Utility.RandomDouble() )
            {
                Mobile combatant = bc.Combatant;

                switch( Utility.Random( 4 ) )
                {
                    case 0: bc.Say( true, "* geisha starts singing *" ); break;
                    case 1: bc.Say( true, String.Format( "Hey {0}! Let me entertain you!", combatant.Name ) ); break;
                    case 2: bc.Say( true, "* geisha bows and moves her tessen *" ); break;
                    case 3: bc.Say( true, String.Format( "Do you like Citadel {0}", combatant.Name ) ); break;
                    case 4: bc.Say( true, "Come here and sing with me!" ); break;
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
}
