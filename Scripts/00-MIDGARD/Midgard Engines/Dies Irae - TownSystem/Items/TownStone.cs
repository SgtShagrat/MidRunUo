/***************************************************************************
 *                                  MidgardTownStone.cs
 *                            		------------------------
 *  begin                	: Gennaio, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class MidgardTownStone : Item
    {
        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public TownSystem System { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public static bool TownAccessEnabled
        {
            get { return TownSystem.TownAccessEnabled; }
            set { TownSystem.TownAccessEnabled = value; } //Magius(CHE): Crash fix
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual bool AcceptCitizens
        {
            get { return System.AcceptCitizens; }
            set { System.AcceptCitizens = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public int TownTreasure
        {
            get { return System.TownTreasure; }
            set { System.RawTownTreasure = value; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual int AccessCost
        {
            get { return System.AccessCost; }
            set { System.AccessCost = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual double ScalarCost
        {
            get { return System.ScalarCost; }
            set { System.ScalarCost = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual bool VendorBuyAllowed
        {
            get { return System.VendorBuyAllowed; }
            set { System.VendorBuyAllowed = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual bool VendorSellAllowed
        {
            get { return System.VendorSellAllowed; }
            set { System.VendorSellAllowed = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual int PercMercTaxes
        {
            get { return System.PercMercTaxes; }
            set { System.PercMercTaxes = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual int PercPlayerVendorTaxes
        {
            get { return System.PercPlayerVendorTaxes; }
            set { System.PercPlayerVendorTaxes = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual int LandCost
        {
            get { return System.LandCost; }
            set { System.LandCost = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual int ServiceAccessCost
        {
            get { return System.ServiceAccessCost; }
            set { System.ServiceAccessCost = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public Mobile WarLord
        {
            get { return System.WarLord; }
            set { System.WarLord = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public bool IsEmptyNews
        {
            get { return System.IsEmptyNews; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public int NumTownGuilds
        {
            get { return System.NumTownGuilds; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual bool IsMurdererTown
        {
            get { return System.IsMurdererTown; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual bool IsEvilAlignedTown
        {
            get { return System.IsEvilAlignedTown; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual bool IsGoodAlignedTown
        {
            get { return System.IsGoodAlignedTown; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public virtual VirtueType Virtue
        {
            get { return System.Type; }
            set { System.Type = value; }
        }

        [Constructable]
        public MidgardTownStone( MidgardTowns town )
            : base( 0xEDE )
        {
            Movable = false;

            System = TownSystem.Find( town );
            if( System != null )
                Name = string.Format( "town stone of {0}", System.Definition.TownName );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( System == null )
                return;

            if( from.AccessLevel > AccessLevel.GameMaster )
            {
                from.SendGump( new PropertiesGump( from, this ) );
                from.SendGump( new TownSystemInfoGump( System, from ) );
                return;
            }

            if( from.InRange( GetWorldLocation(), 2 ) )
            {
                if( !TownSystem.TownAccessEnabled && from.AccessLevel < AccessLevel.Administrator )
                {
                    from.SendMessage( from.Language == "ITA" ? "L'accesso è stato temporaneamente disabilitato, prova più tardi..." : "Town access has been temporary disabled. Try later..." );
                }
                else if( !System.AcceptCitizens )
                {
                    from.SendMessage( from.Language == "ITA" ? "Questa città non accetta cittadini." : "This town does not accept any citizen." );
                }
                else if( from is Midgard2PlayerMobile && (TownHelper.IsTownBanned( (Midgard2PlayerMobile)from, System ) || TownHelper.IsTownPermaBanned( (Midgard2PlayerMobile)from, System )) )
                {
                    from.SendMessage( from.Language == "ITA" ? "Sei esiliato da {0}. Non potrai mai accedere a questa Pietra Cittadina." :"You are exiled from {0}. You can not access this Town Stone forever more.", System.Definition.TownName );
                }
                else
                {
                    // TownSystem acctSystem = TownHelper.FindTownSystemFromAccountTag( from );

                    if( TownSystem.Find( from ) == null )
                    {
                        //if( acctSystem != null && acctSystem == System )
                        //    from.SendMessage( "Your account is already bonded to {0}. Now you are a citizen of {0}", System.Definition.TownName );
                        //else
                        from.SendGump( new TownJoinGump( System, from ) );
                    }
                    else
                    {
                        TownSystem fromSystem = TownSystem.Find( from );

                        if( ( fromSystem != null && fromSystem == System ) || from.AccessLevel > AccessLevel.Counselor )
                        {
                            from.SendLocalizedMessage( 1064669, System.Definition.TownName ); // This account is already bound to ~1_CITY~
                            from.SendGump( new TownSystemInfoGump( System, from ) );
                        }
                        else
                            from.SendMessage( from.Language == "ITA" ? "Non ti è concesso l'accesso a questa Pietra Cittadina." : "You are not allowed to access this Town Stone." );
                    }
                }
            }
            else
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
        }

        #region serial-deserial
        public MidgardTownStone( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( (int)System.Definition.Town );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            System = TownSystem.Find( (MidgardTowns)reader.ReadInt() );
        }
        #endregion
    }
}