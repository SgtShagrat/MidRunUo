using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;

using Server.Engines.BulkOrders;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Misc;

namespace Server.Mobiles
{
    public class AnimalTrainer : BaseVendor
    {
        public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.AnimalTrainer; } } // mod by Dies Irae

        public static readonly bool RegionalStabled = true;

 		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

        [Constructable]
		public AnimalTrainer() : base( "the animal trainer" )
        {
            SetSkill( SkillName.AnimalLore, 64.0, 100.0 );
            SetSkill( SkillName.AnimalTaming, 90.0, 100.0 );
            SetSkill( SkillName.Veterinary, 65.0, 88.0 );
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add( new SBAnimalTrainer() );
        }

        public override VendorShoeType ShoeType
        {
            get { return Female ? VendorShoeType.ThighBoots : VendorShoeType.Boots; }
        }

        #region modifica by Dies Irae
        public override Item CreateBulkOrder( Mobile from, bool fromContextMenu )
        {
            Midgard2PlayerMobile m2Pm = from as Midgard2PlayerMobile;

            if( m2Pm != null && m2Pm.NextTamingBulkOrder == TimeSpan.Zero && ( fromContextMenu || 0.2 > Utility.RandomDouble() ) )
            {
                double theirSkill = m2Pm.Skills[ SkillName.AnimalTaming ].Base;

                if( theirSkill >= 70.1 )
                    m2Pm.NextTamingBulkOrder = TimeSpan.FromHours( 2.0 );
                else if( theirSkill >= 50.1 )
                    m2Pm.NextTamingBulkOrder = TimeSpan.FromHours( 2.0 );
                else
                    m2Pm.NextTamingBulkOrder = TimeSpan.FromMinutes( 30.0 );

                if( theirSkill >= 70.1 && ( ( theirSkill - 40.0 ) / 300.0 ) > Utility.RandomDouble() )
                    return new LargeTamingBOD();

                return SmallTamingBOD.CreateRandomFor( from );
            }

            return null;
        }

        public override bool IsValidBulkOrder( Item item )
        {
            return ( item is SmallTamingBOD || item is LargeTamingBOD );
        }

        public override bool SupportsBulkOrders( Mobile from )
        {
            return ( Midgard2Persistance.TamingBulksEnabled && from is PlayerMobile && from.Skills[ SkillName.AnimalTaming ].Base > 0 );
        }

        public override TimeSpan GetNextBulkOrder( Mobile from )
        {
            if( from is Midgard2PlayerMobile )
                return ( (Midgard2PlayerMobile)from ).NextTamingBulkOrder;

            return TimeSpan.Zero;
        }

		public override void OnSuccessfulBulkOrderReceive( Mobile from )
		{
            if( from is Midgard2PlayerMobile )
                ( (Midgard2PlayerMobile)from ).NextTamingBulkOrder = TimeSpan.Zero;
		}
        #endregion

        public override int GetShoeHue()
        {
            return 0;
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem( Utility.RandomBool() ? (Item)new QuarterStaff() : (Item)new ShepherdsCrook() );
        }

        private class StableEntry : ContextMenuEntry
        {
            private AnimalTrainer m_Trainer;
            private Mobile m_From;

			public StableEntry( AnimalTrainer trainer, Mobile from ) : base( 6126, 12 )
            {
                m_Trainer = trainer;
                m_From = from;
            }

            public override void OnClick()
            {
                m_Trainer.BeginStable( m_From );
            }
        }

        private class ClaimListGump : Gump
        {
            private AnimalTrainer m_Trainer;
            private Mobile m_From;
            private List<BaseCreature> m_List;

			public ClaimListGump( AnimalTrainer trainer, Mobile from, List<BaseCreature> list ) : base( 50, 50 )
            {
                m_Trainer = trainer;
                m_From = from;
                m_List = list;

                from.CloseGump( typeof( ClaimListGump ) );

                AddPage( 0 );

                AddBackground( 0, 0, 325, 50 + ( list.Count * 20 ), 9250 );
                AddAlphaRegion( 5, 5, 315, 40 + ( list.Count * 20 ) );

                AddHtml( 15, 15, 275, 20, "<BASEFONT COLOR=#FFFFFF>Select a pet to retrieve from the stables:</BASEFONT>", false, false );

                for( int i = 0; i < list.Count; ++i )
                {
                    BaseCreature pet = list[ i ];

                    if( pet == null || pet.Deleted )
                        continue;

                    AddButton( 15, 39 + ( i * 20 ), 10006, 10006, i + 1, GumpButtonType.Reply, 0 );
                    AddHtml( 32, 35 + ( i * 20 ), 275, 18, String.Format( "<BASEFONT COLOR=#C0C0EE>{0}</BASEFONT>", pet.Name ), false, false );
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                int index = info.ButtonID - 1;

                if( index >= 0 && index < m_List.Count )
                    m_Trainer.EndClaimList( m_From, m_List[ index ] );
            }
        }

        private class ClaimAllEntry : ContextMenuEntry
        {
            private AnimalTrainer m_Trainer;
            private Mobile m_From;

			public ClaimAllEntry( AnimalTrainer trainer, Mobile from ) : base( 6127, 12 )
            {
                m_Trainer = trainer;
                m_From = from;
            }

            public override void OnClick()
            {
                m_Trainer.Claim( m_From );
            }
        }

        public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
        {
            #region mod by Dies Irae
            if( !Core.AOS )
                return;
            #endregion

            if( from.Alive )
            {
                list.Add( new StableEntry( this, from ) );

                if( from.Stabled.Count > 0 )
                    list.Add( new ClaimAllEntry( this, from ) );
            }

            base.AddCustomContextEntries( from, list );
        }

        public static int GetMaxStabled( Mobile from )
        {
            double taming = from.Skills[ SkillName.AnimalTaming ].Value;
            double anlore = from.Skills[ SkillName.AnimalLore ].Value;
            double vetern = from.Skills[ SkillName.Veterinary ].Value;
            double sklsum = taming + anlore + vetern;

            int max;

            if( sklsum >= 240.0 )
                max = 5;
            else if( sklsum >= 200.0 )
                max = 4;
            else if( sklsum >= 160.0 )
                max = 3;
            else
                max = 2;

            if( taming >= 100.0 )
                max += (int)( ( taming - 90.0 ) / 10 );

            if( anlore >= 100.0 )
                max += (int)( ( anlore - 90.0 ) / 10 );

            if( vetern >= 100.0 )
                max += (int)( ( vetern - 90.0 ) / 10 );

            #region Modifica by Dies Irae per il raddoppio dei pet in stalla
            if( RegionalStabled )
                return max;

            from.SendMessage( "Your maximum stable capacity is {0}.", ( max * 2 ).ToString() );
            return ( max * 2 );
            #endregion
        }

        #region mod by Dies Irae
        public int GetStabledInRegion( Mobile from, Region region )
        {
            DebugSay( "I'm searching reagional stabled mobs." );

            int count = 0;

            for( int i = 0; i < from.Stabled.Count; ++i )
            {
                BaseCreature pet = from.Stabled[ i ] as BaseCreature;

                if( pet == null || pet.Deleted )
                {
                    if( pet != null )
                        pet.IsStabled = false;
                    from.Stabled.RemoveAt( i );
                    --i;
                    continue;
                }

                Region r = Region.Find( pet.Location, from.Map );

                DebugSay( "I've found {0} thas is a {1}", r.Name, pet.GetType().Name );

                if( r == from.Region )
                    count++;
            }

            return count;
        }
        #endregion
        
        private class StableTarget : Target
        {
            private AnimalTrainer m_Trainer;

			public StableTarget( AnimalTrainer trainer ) : base( 12, false, TargetFlags.None )
            {
                m_Trainer = trainer;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted is BaseCreature )
                    m_Trainer.EndStable( from, (BaseCreature)targeted );
                else if( targeted == from )
                    m_Trainer.SayTo( from, 502672 ); // HA HA HA! Sorry, I am not an inn.
                else
                    m_Trainer.SayTo( from, 1048053 ); // You can't stable that!
            }
        }

        public void BeginClaimList( Mobile from )
        {
            if( Deleted || !from.CheckAlive() )
                return;

            List<BaseCreature> list = new List<BaseCreature>();

            for( int i = 0; i < from.Stabled.Count; ++i )
            {
                BaseCreature pet = from.Stabled[ i ] as BaseCreature;

                if( pet == null || pet.Deleted )
                {
                    if( pet != null )
                        pet.IsStabled = false;
                    from.Stabled.RemoveAt( i );
                    --i;
                    continue;
                }

                #region mod by Dies Irae
                if( RegionalStabled )
                {
                    Region r = Region.Find( pet.Location, Map );

                    if( from.PlayerDebug )
                        Console.WriteLine( "Debug regional stables: {0} - {1}", r.Name, pet.GetType().Name );

                    if( r != from.Region )
                        continue;
                }
                #endregion

                list.Add( pet );
            }

            if( list.Count > 0 )
                from.SendGump( new ClaimListGump( this, from, list ) );
            else
                SayTo( from, 502671 ); // But I have no animals stabled with me at the moment!
        }

        public void EndClaimList( Mobile from, BaseCreature pet )
        {
			if ( pet == null || pet.Deleted || from.Map != this.Map || !from.InRange( this, 14 ) || !from.Stabled.Contains( pet ) || !from.CheckAlive() )
                return;

            #region mod by Dies Irae
            if( !from.InRange( Location, 5 ) )
            {
                from.SendMessage( "Thou are too far away from that animal trainer." );
                return;
            }
            #endregion

			if ( CanClaim( from, pet ) )
            {
                DoClaim( from, pet );

                from.Stabled.Remove( pet );

                SayTo( from, 1042559 ); // Here you go... and good day to you!
            }
            else
            {
                SayTo( from, 1049612, pet.Name ); // ~1_NAME~ remained in the stables because you have too many followers.
            }
        }

        public void BeginStable( Mobile from )
        {
            if( Deleted || !from.CheckAlive() )
                return;

            if( RegionalStabled && GetStabledInRegion( from, Region ) >= GetMaxStabled( from ) )
            {
                SayTo( from, 1042565 ); // You have too many pets in the stables!
            }
            if( !RegionalStabled && from.Stabled.Count >= GetMaxStabled( from ) )
            {
                SayTo( from, 1042565 ); // You have too many pets in the stables!
            }
            else
            {
				from.SendLocalizedMessage( 1042558 ); /* I charge 30 gold per pet for a real week's stable time.
										 * I will withdraw it from thy bank account.
										 * Which animal wouldst thou like to stable here?
										 */

                from.Target = new StableTarget( this );
            }
        }

        public bool CanStable( Mobile from )
        {
            return !Deleted && from.CheckAlive() && ( !RegionalStabled || GetStabledInRegion( from, Region ) < GetMaxStabled( from ) );
        }

        public void EndStable( Mobile from, BaseCreature pet )
        {
            if( Deleted || !from.CheckAlive() )
                return;

            #region mod by Dies Irae
            if( !from.InRange( Location, 5 ) )
            {
                from.SendMessage( "Thou are too far away from that animal trainer." );
                return;
            }
            #endregion

            if( !pet.Controlled || pet.ControlMaster != from )
            {
                SayTo( from, 1042562 ); // You do not own that pet!
            }
            else if( pet.IsDeadPet )
            {
                SayTo( from, 1049668 ); // Living pets only, please.
            }
            else if( pet.Summoned )
            {
                SayTo( from, 502673 ); // I can not stable summoned creatures.
            }
			#region Mondain's Legacy
			else if ( pet.Allured )
			{
				SayTo( from, 1048053 ); // You can't stable that!
			}
			#endregion
            else if( pet.Body.IsHuman )
            {
                SayTo( from, 502672 ); // HA HA HA! Sorry, I am not an inn.
            }
            else if( ( pet is PackLlama || pet is PackHorse || pet is Beetle ) && ( pet.Backpack != null && pet.Backpack.Items.Count > 0 ) )
            {
                SayTo( from, 1042563 ); // You need to unload your pet.
            }
            else if( pet.Combatant != null && pet.InRange( pet.Combatant, 12 ) && pet.Map == pet.Combatant.Map )
            {
                SayTo( from, 1042564 ); // I'm sorry.  Your pet seems to be busy.
            }
            #region mod by Dies Irae
            else if( RegionalStabled && GetStabledInRegion( from, Region ) >= GetMaxStabled( from ) )
            {
                SayTo( from, 1042565 ); // You have too many pets in the stables!
            }
            #endregion
            else if( !RegionalStabled && from.Stabled.Count >= GetMaxStabled( from ) )
            {
                SayTo( from, 1042565 ); // You have too many pets in the stables!
            }
            else
            {
                Container bank = from.FindBankNoCreate();

				if ( ( from.Backpack != null && from.Backpack.ConsumeTotal( typeof( Gold ), 30 ) ) || ( bank != null && bank.ConsumeTotal( typeof( Gold ), 30 ) ) )
                {
                    pet.ControlTarget = null;
                    pet.ControlOrder = OrderType.Stay;
                    pet.Internalize();

                    pet.SetControlMaster( null );
                    pet.SummonMaster = null;

                    pet.IsStabled = true;

                    // if( Core.SE )
 						pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

                    from.Stabled.Add( pet );

                    from.AddToBackpack( new PetTicket( from, pet, Region.Name ) ); // mod by Dies Irae

					SayTo( from, /*Core.AOS ? */ 1049677 /*: 502679*/ ); // [AOS: Your pet has been stabled.] Very well, thy pet is stabled. Thou mayst recover it by saying 'claim' to me. In one real world week, I shall sell it off if it is not claimed!
                }
                else
                {
                    SayTo( from, 502677 ); // But thou hast not the funds in thy bank account!
                }
            }
        }

        public void Claim( Mobile from )
        {
            Claim( from, null );
        }

		public void Claim( Mobile from, string petName )
		{
            if( Deleted || !from.CheckAlive() )
                return;

            bool claimed = false;
            int stabled = 0;

            for( int i = 0; i < from.Stabled.Count; ++i )
            {
                BaseCreature pet = from.Stabled[ i ] as BaseCreature;

                #region mod by Dies Irae
                if( RegionalStabled && pet != null && Region.Find( pet.Location, Map ) != from.Region )
                    continue;
                #endregion

                if( pet == null || pet.Deleted )
                {
                    if( pet != null )
                        pet.IsStabled = false;
                    from.Stabled.RemoveAt( i );
                    --i;
                    continue;
                }

                ++stabled;

				if ( CanClaim( from, pet ) )
                {
                    DoClaim( from, pet );

                    from.Stabled.RemoveAt( i );
                    --i;

                    claimed = true;
                }
                else
                {
                    SayTo( from, 1049612, pet.Name ); // ~1_NAME~ remained in the stables because you have too many followers.
                }
            }

            if( claimed )
                SayTo( from, 1042559 ); // Here you go... and good day to you!
            else if( stabled == 0 )
                SayTo( from, 502671 ); // But I have no animals stabled with me at the moment!
        }

        #region modifica by Berto e Dies
        public static int GetFollowersCost = 2000;

        public static void GetFollowers_OnCommand( Mobile from, Mobile trainer )
        {
            if( Banker.GetBalance( from ) >= GetFollowersCost )
            {
                List<BaseCreature> pets = new List<BaseCreature>();
                try
                {
                    foreach( Mobile m in World.Mobiles.Values )
                    {
                        if( m is BaseCreature )
                        {
                            BaseCreature bc = (BaseCreature)m;

                            if( bc.IsHitched || bc.IsPregnant )
                                continue;

                            if( ( bc.Controlled && bc.ControlMaster == from ) || ( bc.Summoned && bc.SummonMaster == from ) )
                                pets.Add( bc );
                        }
                    }
                }
                catch { }

                if( pets.Count > 0 )
                {
                    trainer.Say( "I have found your pets. Thank you for {0} gold!", GetFollowersCost );
                    Banker.Withdraw( from, GetFollowersCost );

                    if( trainer is BaseVendor )
                    {
                        TownSystem system = TownSystem.Find( ( (BaseVendor)trainer ).MidgardTown );
                        if( system != null )
                            system.RegisterTransaction( GetFollowersCost );
                    }

                    foreach( BaseCreature pet in pets )
                    {
                        if( pet is IMount )
                            ( (IMount)pet ).Rider = null; // make sure it's dismounted

                        pet.MoveToWorld( trainer.Location, trainer.Map );

                        PetTicket.DeleteTicketsFor( pet );
                    }
                }
                else
                {
                    trainer.Say( "You do not have any follower on Sosaria." );
                }
            }
            else
            {
                trainer.Say( "I'm sorry {0}. You lack funds to access my services. Getting your followers cost {1} gold.", from.Name, GetFollowersCost );
            }
        }
        #endregion

        public bool CanClaim( Mobile from, BaseCreature pet )
        {
            if( pet.IsPregnant && DateTime.Now < DeliveryTime )
            {
                SayTo( from, true, "Return here in {0} days and I will give mother and youngling.", pet.DeliveryTime );
                return false;
            }

            return ((from.Followers + pet.ControlSlots) <= from.FollowersMax);
        }

        private void DoClaim( Mobile from, BaseCreature pet )
        {
            pet.SetControlMaster( from );

            if ( pet.Summoned )
                pet.SummonMaster = from;

            pet.ControlTarget = from;
            pet.ControlOrder = OrderType.Follow;

            pet.MoveToWorld( from.Location, from.Map );

            pet.IsStabled = false;

            #region mod by Dies Irae
            if( pet.IsPregnant && pet.Deliver() )
                SayTo( from, true, "Wow! A new baby was born recently." );

            PetTicket.DeleteTicketsFor( pet );
            #endregion

            if ( Core.SE )
                pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy
        }

        public override bool HandlesOnSpeech( Mobile from )
        {
            return true;
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( !e.Handled && e.HasKeyword( 0x0008 ) )
            {
                e.Handled = true;

                #region modifica by Dies per i Vendors YoungDealers
                if( !YoungDeal( this, e.Mobile ) )
                    return;
                else if( !CitizenDeal( this, e.Mobile, false, true ) )
                    return;
                #endregion

                BeginStable( e.Mobile );
            }
            else if( !e.Handled && e.HasKeyword( 0x0009 ) )
            {
                e.Handled = true;

                #region modifica by Dies per i Vendors YoungDealers
                if( !YoungDeal( this, e.Mobile ) )
                    return;
                else if( !CitizenDeal( this, e.Mobile, false, true ) )
                    return;
                #endregion

                #region modifica by Dies Irae
                if( Insensitive.Equals( e.Speech, "claim" ) )
                    BeginClaimList( e.Mobile );
                else if( Insensitive.Equals( e.Speech, "claim all" ) )
                    Claim( e.Mobile );
                else
                    Say( true, "I cannot understand that order, Sir" );

                //				if ( !Insensitive.Equals( e.Speech, "claim" ) )
                //					BeginClaimList( e.Mobile );
                //				else
                //					Claim( e.Mobile );
                #endregion
            }
            else if( e.Mobile.InRange( this, 2 ) && Insensitive.Equals( e.Speech, ( "get followers" ) ) )
            {
                #region modifica by Dies per i Vendors YoungDealers
                if( !YoungDeal( this, e.Mobile ) )
                    return;
                else if( !CitizenDeal( this, e.Mobile, false, true ) )
                    return;
                #endregion

                GetFollowers_OnCommand( e.Mobile, this );
            }
/*
			else if ( !e.Handled && e.HasKeyword( 0x0009 ) )
			{
				e.Handled = true;

                if (!Insensitive.Equals( e.Speech, "claim" ))
                {
                    bool showList = true;

                    if ( e.Speech.Length > 6 ) //sanity    
                    {
                        string name = e.Speech.Substring( 6 ).Trim();

                        for ( int i = 0; i < e.Mobile.Stabled.Count; ++i )
                        {
                            if ( Insensitive.Equals( e.Mobile.Stabled[i].Name, name ) ) //Similar names?
                            {
                                showList = false;
                                break;
                            }
                        }

                        //What about pets with the same name, or 'similar' names, ie Fluffy 1, Fluffy 2?? - Similar names don't work.. what about identicals?
                        //What if you try and claim a pet that doesn't exist? -- GUMP
                    }

                    if ( showList )
                        BeginClaimList( e.Mobile );
                }
                else
                {
                    Claim( e.Mobile );
                }
			}
*/
            else
            {
                base.OnSpeech( e );
            }
        }

        #region mod by Dies Irae
        public int GetSellPriceFor( BaseCreature bc )
        {
            if( bc.Summoned || bc.IsAnimatedDead || !bc.Alive )
                return 0;

            IShopSellInfo[] info = GetSellInfo();

            foreach( IShopSellInfo ssi in info )
            {
                AnimalSellInfo animalSellInfo = ssi as AnimalSellInfo;
                if( animalSellInfo != null )
                    return animalSellInfo.GetSellPriceFor( bc );
            }

            return 0;
        }

        public virtual void SellPet( Mobile from, BaseCreature bc )
        {
            int price = GetSellPriceFor( bc );

            if( price > 0 )
            {
                int sound;
                if( price < 10 )
                    sound = 53;
                else if( price < 100 )
                    sound = 54;
                else
                    sound = 55;

                SayTo( from, true, "Thank you! I'll take care of this creature. Here is your {0}gp.", price );

                while( price > 60000 )
                {
                    from.AddToBackpack( new Gold( 60000 ) );
                    price -= 60000;
                }

                from.AddToBackpack( new Gold( price ) );
                from.PlaySound( sound );

                bc.Delete();
            }
            else
                SayTo( from, true, "You have nothing I would be interested in." );
        }
        #endregion

        public AnimalTrainer( Serial serial ) : base( serial )
		{
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

        private class PetTicket : Item
        {
            private static List<PetTicket> m_Tickets = new List<PetTicket>();

            private void RegisterTicket()
            {
                if( m_Tickets == null )
                    m_Tickets = new List<PetTicket>();

                if( !m_Tickets.Contains( this ) )
                    m_Tickets.Add( this );
            }

            public static void DeleteTicketsFor( BaseCreature bc )
            {
                for( int i = 0; i < m_Tickets.Count; i++ )
                {
                    PetTicket petTicket = m_Tickets[ i ];
                    if( petTicket.m_Creature == bc )
                        petTicket.Delete();
                }
            }

            private Mobile m_Owner;
            private BaseCreature m_Creature;
            private string m_Region;

            public PetTicket( Mobile owner, BaseCreature creature, string region )
                : base( 0x14f0 )
            {
                m_Owner = owner;
                m_Creature = creature;
                m_Region = region;

                LootType = LootType.Blessed;

                RegisterTicket();
            }

            public override void OnSingleClick( Mobile from )
            {
                if( m_Creature != null && m_Owner != null )
                    LabelTo( from, string.Format( "Pet claim ticket - Name: {0} - Owner: {1}", m_Creature.Name, m_Owner.Name ) );
                else
                    LabelTo( from, string.Format( "a pet claim ticket" ) );

                if( !string.IsNullOrEmpty( m_Region ) )
                    LabelTo( from, "stabled in the region of {0}", m_Region );
            }

            public PetTicket( Serial serial )
                : base( serial )
            {
                RegisterTicket();
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( (int)0 ); // version

                writer.Write( m_Owner );
                writer.Write( m_Creature );
                writer.Write( m_Region );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();

                m_Owner = reader.ReadMobile();
                m_Creature = reader.ReadMobile() as BaseCreature;
                m_Region = reader.ReadString();
            }
        }
	}
}