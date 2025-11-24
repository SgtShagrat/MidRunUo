/*
 * [Author]: Mideon Quo
 * July,26/08
 * RunUO Scripting Contenst "Warm-Up" Submission
 * 
 * [Type]: Command
 * [Usage]: .decocorpse
 * - and then target a non-player mobile to generate a non-decaying corpse of that mobile.
 * 
 * 
 * [Description]:
 * This is a rather simple command which allows a GameMaster (or higher) to target a mobile (non-player)
 * and generate a corpse of this mobile. The target mobile is removed after the command is executed and
 * all items are dropped to the new corpse.
 * 
 * This corpse is different from a normal corpse in one special way, it does not decay. This command is
 * useful for setting up long-term corpse decorations. RP servers will find this useful, as well as tank
 * servers that like to setup interesting decoration or even run some storyline geared events. There are
 * plenty of improvements that could be made, but in the interest of keeping it simple and supporting
 * RunUO's new scripting competition, I am going to leave it as is. Thank you for checking out my script!
 *  
 */

using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Targeting;

namespace Server.Items
{
    public class DecoCorpse : Corpse
    {
        public new static void Initialize()
        {
            CommandSystem.Register( "DecoCorpse", AccessLevel.GameMaster, new CommandEventHandler( DecoCorpse_OnCommand ) );
        }

        [Usage( "DecoCorpse" )]
        [Description( "Drops a DecoCorpse (undecayable except by deletion) of targeted mobile." )]
        public static void DecoCorpse_OnCommand( CommandEventArgs arg )
        {
            arg.Mobile.SendMessage( "Select target for deco-corpsification." );
            arg.Mobile.Target = new DecoCorpseTarget();
        }

        /// <summary>
        /// This class handles the targeting of a non-player mobile to create a decocorpse from.
        /// </summary>
        private class DecoCorpseTarget : Target
        {
            public DecoCorpseTarget()
                : base( 10, false, TargetFlags.None ) //No Flags for targeting, cannot target ground
            {
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                base.OnTarget( from, targeted );

                if( targeted is Mobile )
                {
                    Mobile mob = targeted as Mobile;

                    if( !mob.Player )
                    {
                        PlaceDecoCorpse( mob );
                        from.SendMessage( "Corpse created, be mindful you need to remove it. It does not decay!" );
                    }
                    else
                        from.SendMessage( "Only NPC Mobile targets are valid;" );
                }
                else
                {
                    from.SendMessage( "Only NPC Mobile targets are valid;" );
                }
            }
        }

        public DecoCorpse( Mobile owner, List<Item> items )
            : base( owner, items )
        {
        }

        /// <summary>
        /// Hides the Parent class Corpse's BeginDecay, to eliminate Decay functionality suitable for decoration.
        /// </summary>
        public override void BeginDecay( TimeSpan delay )
        {
            return;
        }

        public override bool Decays
        {
            get { return false; }
        }

        #region serialization
        public DecoCorpse( Serial s )
            : base( s )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); //version;
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        /// <summary>
        /// This method creates a DecoCorpse from the selected Mobile mob (non-player mobile)
        /// and deletes mob once the corpse is placed. All suitable items are transferred to 
        /// the DecoCorpse.
        /// </summary>
        public static void PlaceDecoCorpse( Mobile mob )
        {
            List<Item> itemsCopy = new List<Item>( mob.Items );
            List<Item> content = new List<Item>();
            List<Item> equip = new List<Item>();
            List<Item> moveToPack = new List<Item>();
            Container pack = mob.Backpack;

            //Sorts through each of the mobiles items and adds to the content and equipment of the corpse
            foreach( Item item in itemsCopy )
            {
                //Do not move the backpack over to the corpse
                if( item == pack )
                    continue;

                //Check if Item should move to corpse or stay on mobile
                DeathMoveResult res = mob.GetParentMoveResultFor( item );

                switch( res )
                {
                    //if item should move to corpse
                    case DeathMoveResult.MoveToCorpse:
                        {
                            content.Add( item );
                            equip.Add( item );
                            break;
                        }
                    //if item should remain with mobile
                    case DeathMoveResult.MoveToBackpack:
                        {
                            moveToPack.Add( item );
                            break;
                        }
                }
            }

            //Sorts through the backpack and drops it's contents to the corpse
            if( pack != null )
            {
                List<Item> packCopy = new List<Item>( pack.Items );

                foreach( Item item in packCopy )
                {
                    DeathMoveResult res = mob.GetInventoryMoveResultFor( item );

                    if( res == DeathMoveResult.MoveToCorpse )
                        content.Add( item );
                    else
                        moveToPack.Add( item );
                }

                foreach( Item item in moveToPack )
                {
                    if( mob.RetainPackLocsOnDeath && item.Parent == pack )
                        continue;

                    pack.DropItem( item );
                }
            }

            Corpse c = new DecoCorpse( mob, equip );

            foreach( Item item in content )
            {
                c.DropItem( item );
                item.Movable = false;
            }

            c.MoveToWorld( mob.Location, mob.Map );
            mob.Delete();
        }
    }
}