using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.PetSystem
{
    public class PetStatGump : Gump
    {
        private enum Buttons
        {
            Close,

            Points,
            Mate
        }

        #region campi
        private BaseCreature m_Pet;
        private Mobile m_From;
        #endregion

        #region gump design
        private int LabelOffset = 22;
        private int ValueOffset = 140;

        private int RawsOffset = 20;
        private int LabelHue = 1149;
        private int ValueHue = 64;
        #endregion

        #region costruttori
        public PetStatGump( BaseCreature pet, Mobile from )
            : base( 25, 25 )
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_Pet = pet;
            m_From = from;

            from.CloseGump( typeof( PetStatGump ) );
            from.CloseGump( typeof( PetLevelGump ) );

            Mobile master = m_Pet.ControlMaster;

            int nextLevel = m_Pet.NextLevel * m_Pet.Level;
            bool YoungPet = ( DateTime.Now - m_Pet.CreationTime ) < TimeSpan.FromDays( PetUtility.GetParMax( PetUtility.PetPar.PetYoungTime, pet ) );
            bool canMate = ( m_From == master && m_Pet.AllowMating && !YoungPet && !PetUtility.CannotBeBreeded( m_Pet ) ) || m_From.AccessLevel >= AccessLevel.Administrator;

            int daysToBond = 0;
            if( !m_Pet.IsBonded && m_Pet.BondingBegin != DateTime.MinValue )
                daysToBond = (int)( m_Pet.BondingBegin + m_Pet.BondingDelay - DateTime.Now ).TotalDays;

            int expPercentage = (int)( ( m_Pet.Exp / (double)nextLevel ) * 100 );
            if( expPercentage > 100 )
                expPercentage = 100;
            if( expPercentage < 0 )
                expPercentage = 0;

            int ageInDays = (int)( DateTime.Now - m_Pet.CreationTime ).TotalDays;

            AddPage( 0 );

            AddBackground( 0, 0, 270, 19 * RawsOffset + 12, 2620 );
            AddImageTiled( 5, 6, 260, 19 * RawsOffset, 9274 );
            AddAlphaRegion( 4, 6, 262, 19 * RawsOffset );

            AddInfo( "Name: ", m_Pet.Name, 1, true );
            AddInfo( "Owner: ", master == null ? "unowned" : master.Name, 2 );
            AddInfo( "Type: ", GetFriendlyClassName( m_Pet.GetType().Name ), 3 );
            AddInfo( "Gender: ", m_Pet.Female ? "Female" : "Male", 4 );
            AddInfo( "Is Bonded: ", m_Pet.IsBonded ? "Yes" : string.Format( "No ({0} d. left)", daysToBond ), 5 );
            AddInfo( "Generation: ", m_Pet.Generation.ToString(), 6 );
            AddInfo( "Current Level: ", m_Pet.Level.ToString(), 7 );
            AddInfo( "Maximum Level: ", m_Pet.MaxLevel.ToString(), 8 );
            AddInfo( "Age: ", string.Format( "{0} days", ageInDays ), 9 );
            AddInfo( "Can Mate: ", canMate ? "Yes" : "No", 10 );
            AddInfo( "Ability Points: ", m_Pet.AbilityPoints.ToString(), 11 );

            AddInfo( "Experience: ", m_Pet.Exp.ToString(), 16 );
            AddInfo( "Exp Till Next Lev: ", nextLevel.ToString(), 17 );

            AddLabel( LabelOffset, RawsOffset * 12, LabelHue, "Experience Status: " );
            AddImage( 140, RawsOffset * 12 + 4, 2053 );
            AddImageTiled( 140, RawsOffset * 12 + 4, (int)( ( expPercentage / 100.0 ) * 109 ), 11, 2054 );

            AddLabel( 60, RawsOffset * 14, LabelHue, canMate ? "Mate this pet with another" : "This pet cannot mate" );
            if( canMate )
                AddButton( 24, RawsOffset * 14, 4005, 4006, (int)Buttons.Mate, GumpButtonType.Reply, 0 );

            if( ( m_Pet.AbilityPoints > 0 && m_From == master ) || m_From.AccessLevel >= AccessLevel.GameMaster )
            {
                AddLabel( 60, RawsOffset * 15, LabelHue, "Assign Points " );
                AddButton( 24, RawsOffset * 15, 4005, 4006, (int)Buttons.Points, GumpButtonType.Reply, 0 );
            }
        }
        #endregion

        #region metodi
        private void AddInfo( string label, string value, int index )
        {
            AddInfo( label, value, index, false );
        }

        private void AddInfo( string label, string value, int index, bool cropped )
        {
            AddLabel( LabelOffset, RawsOffset * index, LabelHue, label );
            if( cropped )
                AddLabelCropped( ValueOffset, RawsOffset * index, 120, 20, ValueHue, value );
            else
                AddLabel( ValueOffset, RawsOffset * index, ValueHue, value );
        }

        /// <summary>
        /// Puts spaces before type name inner-caps
        /// </summary>
        /// <param name="typeName">type name of our item</param>
        /// <returns>string with friendly name or <c>typeName</c></returns>
        public static string GetFriendlyClassName( string typeName )
        {
            for( int index = 1; index < typeName.Length; index++ )
            {
                if( char.IsUpper( typeName, index ) )
                    typeName.Insert( index++, " " );
            }

            return typeName;
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;

            if( from == null )
                return;

            if( info.ButtonID == (int)Buttons.Close )
                return;

            if( info.ButtonID == (int)Buttons.Points )
                from.SendGump( new PetLevelGump( m_Pet ) );
            else if( info.ButtonID == (int)Buttons.Mate )
            {
                bool isNearBreeder = false;

                foreach( Mobile m in from.GetMobilesInRange( 5 ) )
                {
                    if( m is AnimalBreeder )
                        isNearBreeder = true;
                }

                if( isNearBreeder )
                {
                    from.SendMessage( "What creature would you like your pet to breed with?" );
                    from.Target = new BeginMatingTarget( m_Pet );
                }
                else
                    from.SendMessage( "You must be near an animal breeder in order to breed your pet." );
            }
        }
        #endregion

        #region target
        internal class BeginMatingTarget : Target
        {
            #region campi
            private BaseCreature m_Pet;
            #endregion

            #region costruttori
            public BeginMatingTarget( BaseCreature pet )
                : base( 10, false, TargetFlags.None )
            {
                m_Pet = pet;
            }
            #endregion

            #region metodi
            protected override void OnTarget( Mobile from, object target )
            {
                if( target is PlayerMobile )
                {
                    from.SendMessage( "Huh? But the childern would be so ugly!" );
                }
                else if( target is BaseCreature )
                {
                    BaseCreature bc = (BaseCreature)target;

                    bool YoungPet = ( DateTime.Now - bc.CreationTime ) < TimeSpan.FromDays( PetUtility.GetParMax( PetUtility.PetPar.PetYoungTime, bc ) );

                    Type targettype = bc.GetType();
                    Type pettype = m_Pet.GetType();
                    Mobile breeder = null;
                    Mobile owner = null;

                    foreach( Mobile m in bc.GetMobilesInRange( 5 ) )
                    {
                        if( m is AnimalBreeder )
                            breeder = m;

                        if( m == bc.ControlMaster )
                            owner = m;
                    }

                    if( bc.Controlled != true )
                    {
                        from.SendMessage( "That creature is not tamed." );
                    }
                    else if( bc.ControlMaster == null )
                    {
                        from.SendMessage( "That creature has no master." );
                    }
                    else if( bc.MatingDelay >= DateTime.Now )
                    {
                        from.SendMessage( "That creature has mating in that last six days, It cannot mate again so soon." );
                    }
                    else if( bc.ControlMaster == from )
                    {
                        from.SendMessage( "You cannot breed two of your own pets together, You must find another player who has the same type of pet as your in order to breed." );
                    }
                    else if( breeder == null )
                    {
                        from.SendMessage( "You must be near an animal breeder in order to breed your pet." );
                    }
                    else if( owner == null )
                    {
                        from.SendMessage( "The owner of that pet is not near by to confirm mating between the two pets." );
                    }
                    else if( targettype != pettype )
                    {
                        from.SendMessage( "You cannot crossbreed to different species together." );
                    }
                    else if( bc.AllowMating != true )
                    {
                        from.SendMessage( "This creature is not at the correct level to breed yet." );
                    }
                    else if( bc.Female == m_Pet.Female )
                    {
                        from.SendMessage( "You cannot breed two pets of the same gender together." );
                    }
                    else if( YoungPet && from.AccessLevel < AccessLevel.GameMaster )
                    {
                        from.SendMessage( "You cannot breed with a pet so young!" );
                    }
                    else
                    {
                        from.SendGump( new AwaitingConfirmationGump( m_Pet, bc ) );
                        owner.SendGump( new BreedingAcceptGump( m_Pet, bc ) );
                    }
                }
                else
                {
                    from.SendMessage( "Your pet cannot breed with that." );
                }
            }
            #endregion
        }
        #endregion
    }
}