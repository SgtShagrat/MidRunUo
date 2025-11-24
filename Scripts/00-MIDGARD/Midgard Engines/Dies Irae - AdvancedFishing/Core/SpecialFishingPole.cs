/***************************************************************************
 *                               SpecialFishingPole.cs
 *
 *   begin                : 20 settembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.AdvancedFishing
{
    public class SpecialFishingPole : Item, IUsesRemaining
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public Actions FisherAction { get; set; }

        public FishingAITimer AITimer { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastAction { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastFishAction { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int UsesRemaining { get; set; }

        public override string DefaultName
        {
            get { return "a special fishing rod"; }
        }

        public bool ShowUsesRemaining { get { return true; } set { } }

        public void Consume( Mobile from )
        {
            --UsesRemaining;

            if( UsesRemaining <= 0 )
                Delete();
        }

        public override bool CanEquip( Mobile from )
        {
            if( from.Dex < Config.MinDexRequired )
            {
                from.SendLangMessage( 10020014 ); // "You are not nimble enough to equip that."
                return false;
            }
            else if( from.Str < Config.MinStrRequired )
            {
                from.SendLangMessage( 10020015 ); // "You are not strong enough to equip that."
                return false;
            }
            else if( from.Skills[ SkillName.Fishing ].Value < Config.MinFishingRequired )
            {
                from.SendLangMessage( 10020016, Config.MinFishingRequired.ToString( "F1" ) );
                return false;
            }
            else if( !from.CanBeginAction( typeof( SpecialFishingPole ) ) ) // "Thou must have at least {0} fishing skill to use this pole."
            {
                return false;
            }
            else
            {
                return base.CanEquip( from );
            }
        }

        public override void OnRemoved( object parent )
        {
            base.OnRemoved( parent );

            if( parent is PlayerMobile )
                Reset( (PlayerMobile)parent );
        }

        public override DeathMoveResult OnParentDeath( Mobile parent )
        {
            Reset( parent );

            return base.OnParentDeath( parent );
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, from.Language == "ITA" ? "(cariche: {0})" : "(charges: {0})", UsesRemaining );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( AITimer != null )
            {
                from.SendLangMessage( 10020017 ); // "You decided to give up..."
                Reset( from );
            }
            else if( from.Mounted )
                from.SendLangMessage( 10020018 ); // You can't fish while riding!
            else
            {
                from.BeginTarget( (int)Config.MaxRange, true, TargetFlags.None, new TargetCallback( Core.SpecialFishing_OnTarget ) );
                from.SendLangMessage( 10020019 ); // "Choose a valid water target to start your adventure!"
            }
        }

        public void StartTimer( Mobile from, Point3D p, int tileID )
        {
            LastAction = DateTime.Now;
            LastFishAction = DateTime.Now;

            AITimer = new FishingAITimer( from, this, p, tileID );
            AITimer.Start();
        }

        public void Reset( Mobile from )
        {
            FisherAction = 0;
            from.CloseGump( typeof( FishingGump ) );

            if( AITimer != null && AITimer.Running )
            {
                AITimer.Stop();
                AITimer = null;
            }
        }

        [Constructable]
        public SpecialFishingPole()
            : base( 0x0DC0 )
        {
            Layer = Layer.OneHanded;
            Weight = 8.0;

            UsesRemaining = 50;
        }

        #region serialization
        public SpecialFishingPole( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( UsesRemaining );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            UsesRemaining = reader.ReadInt();
        }
        #endregion
    }
}