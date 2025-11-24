//using Server.Factions;

//namespace Server.Items
//{
//    public class EvilRobe : BaseOuterTorso
//    {
//        #region campi
//        #endregion
		
//        #region proprietà
//        public override bool AllowFemaleWearer{ get{ return false; } }
//        public override int LabelNumber{ get{ return 1064330; } }
//        #endregion
		
//        #region costruttori
//        [Constructable]
//        public EvilRobe() : this( 0 )
//        {
//        }

//        [Constructable]
//        public EvilRobe( int hue ) : base( 0x2669, hue )
//        {
//            Weight = 2.0;
//        }

//        public EvilRobe( Serial serial ) : base( serial )
//        {
//        }
//        #endregion
		
//        #region metodi
//        public override bool OnEquip( Mobile from )
//        {
//            return Validate( from ) && base.OnEquip( from );
//        }
			
//        public bool Validate( Mobile m )
//        {
//            if ( !Core.AOS || m == null || !m.Player)			
//                return true;
			
//            Faction f = Factions.Faction.Find( m );
//            if( f is CrowsOfAgony )
//            {
//                return true;
//            }
//            else
//            {
//                m.SendLocalizedMessage( 1064335 ); // Only an evil Crow can wear this!
//                return false;
//            }		
//        }
//        #endregion
		
//        #region serial-deserial
//        public override void Serialize( GenericWriter writer )
//        {
//            base.Serialize( writer );

//            writer.WriteEncodedInt( 0 ); // version
//        }

//        public override void Deserialize( GenericReader reader )
//        {
//            base.Deserialize( reader );

//            int version = reader.ReadEncodedInt();
//        }
//        #endregion
//    }
	
//    public class EvilFemaleRobe : BaseOuterTorso
//    {
//        #region campi		
//        #endregion
		
//        #region proprietà
//        public override bool AllowMaleWearer{ get{ return false; } }
//        public override int LabelNumber{ get{ return 1064331; } }
//        #endregion
		
//        #region costruttori
//        [Constructable]
//        public EvilFemaleRobe() : this( 0 )
//        {
//        }

//        [Constructable]
//        public EvilFemaleRobe( int hue ) : base( 0x266A, hue )
//        {
//            Weight = 2.0;
//        }

//        public EvilFemaleRobe( Serial serial ) : base( serial )
//        {
//        }
//        #endregion
		
//        #region metodi
//        public override bool OnEquip( Mobile from )
//        {
//            return Validate( from ) && base.OnEquip( from );
//        }
			
//        public bool Validate( Mobile m )
//        {
//            if ( !Core.AOS || m == null || !m.Player)			
//                return true;
			
//            Faction f = Factions.Faction.Find( m );
//            if( f is CrowsOfAgony )
//            {
//                return true;
//            }
//            else
//            {
//                m.SendLocalizedMessage( 1064335 ); // Only an evil Crow can wear this!
//                return false;
//            }		
//        }
//        #endregion
		
//        #region serial-deserial
//        public override void Serialize( GenericWriter writer )
//        {
//            base.Serialize( writer );

//            writer.WriteEncodedInt( 0 ); // version
//        }

//        public override void Deserialize( GenericReader reader )
//        {
//            base.Deserialize( reader );

//            int version = reader.ReadEncodedInt();
//        }
//        #endregion		
//    }
	
//    public class GoodRobe : BaseOuterTorso
//    {
//        #region campi
//        #endregion
		
//        #region proprietà
//        public override bool AllowFemaleWearer{ get{ return false; } }
//        public override int LabelNumber{ get{ return 1064332; } }
//        #endregion
		
//        #region costruttori
//        [Constructable]
//        public GoodRobe() : this( 0 )
//        {
//        }

//        [Constructable]
//        public GoodRobe( int hue ) : base( 0x266F, hue )
//        {
//            Weight = 2.0;
//        }

//        public GoodRobe( Serial serial ) : base( serial )
//        {
//        }
//        #endregion
		
//        #region metodi
//        public override bool OnEquip( Mobile from )
//        {
//            return Validate( from ) && base.OnEquip( from );
//        }
			
//        public bool Validate( Mobile m )
//        {
//            if ( !Core.AOS || m == null || !m.Player)			
//                return true;
			
//            Faction f = Factions.Faction.Find( m );
//            if( f is LegioImperialis )
//            {
//                return true;
//            }
//            else
//            {
//                m.SendLocalizedMessage( 1064336 ); // Only an good Legionary can wear this!
//                return false;
//            }		
//        }
//        #endregion
		
//        #region serial-deserial
//        public override void Serialize( GenericWriter writer )
//        {
//            base.Serialize( writer );

//            writer.WriteEncodedInt( 0 ); // version
//        }

//        public override void Deserialize( GenericReader reader )
//        {
//            base.Deserialize( reader );

//            int version = reader.ReadEncodedInt();
//        }
//        #endregion		
//    }
	
//    public class GoodFemaleRobe : BaseOuterTorso
//    {
//        #region campi
//        #endregion
		
//        #region proprietà
//        public override bool AllowMaleWearer{ get{ return false; } }
//        public override int LabelNumber{ get{ return 1064333; } }
//        #endregion
		
//        #region costruttori
//        [Constructable]
//        public GoodFemaleRobe() : this( 0 )
//        {
//        }

//        [Constructable]
//        public GoodFemaleRobe( int hue ) : base( 0x2670, hue )
//        {
//            Weight = 2.0;
//        }

//        public GoodFemaleRobe( Serial serial ) : base( serial )
//        {
//        }
//        #endregion
		
//        #region metodi
//        public override bool OnEquip( Mobile from )
//        {
//            return Validate( from ) && base.OnEquip( from );
//        }
			
//        public bool Validate( Mobile m )
//        {
//            if ( !Core.AOS || m == null || !m.Player)			
//                return true;
			
//            Faction f = Factions.Faction.Find( m );
//            if( f is LegioImperialis )
//            {
//                return true;
//            }
//            else
//            {
//                m.SendLocalizedMessage( 1064336 ); // Only an good Legionary can wear this!
//                return false;
//            }		
//        }
//        #endregion
		
//        #region serial-deserial
//        public override void Serialize( GenericWriter writer )
//        {
//            base.Serialize( writer );

//            writer.WriteEncodedInt( 0 ); // version
//        }

//        public override void Deserialize( GenericReader reader )
//        {
//            base.Deserialize( reader );

//            int version = reader.ReadEncodedInt();
//        }
//        #endregion		
//    }
	
//    [FlipableAttribute( 0x382A, 0x382B )]
//    public class FactionBannerLargeEvil : Item
//    {
//        #region campi
//        #endregion
		
//        #region proprietà}
//        public override int LabelNumber{ get{ return 0; } }
//        #endregion
		
//        #region costruttori
//        [Constructable]
//        public FactionBannerLargeEvil( ) : base( 0x382A )
//        {
//        }

//        public FactionBannerLargeEvil( Serial serial ) : base( serial )
//        {
//        }
//        #endregion
		
//        #region metodi
//        #endregion
		
//        #region serial-deserial
//        public override void Serialize( GenericWriter writer )
//        {
//            base.Serialize( writer );

//            writer.WriteEncodedInt( 0 ); // version
//        }

//        public override void Deserialize( GenericReader reader )
//        {
//            base.Deserialize( reader );

//            int version = reader.ReadEncodedInt();
//        }
//        #endregion		
//    }

//    [FlipableAttribute( 0x3828, 0x3829 )]
//    public class FactionBannerLargeGood : Item
//    {
//        #region campi
//        #endregion
		
//        #region proprietà}
//        public override int LabelNumber{ get{ return 0; } }
//        #endregion
		
//        #region costruttori
//        [Constructable]
//        public FactionBannerLargeGood( ) : base( 0x3828 )
//        {
//        }

//        public FactionBannerLargeGood( Serial serial ) : base( serial )
//        {
//        }
//        #endregion
		
//        #region metodi
//        #endregion
		
//        #region serial-deserial
//        public override void Serialize( GenericWriter writer )
//        {
//            base.Serialize( writer );

//            writer.WriteEncodedInt( 0 ); // version
//        }

//        public override void Deserialize( GenericReader reader )
//        {
//            base.Deserialize( reader );

//            int version = reader.ReadEncodedInt();
//        }
//        #endregion		
//    }
	
//    [FlipableAttribute( 0x26A0, 0x26A5 )]
//    public class FactionBannerEvil : Item
//    {
//        #region campi
//        #endregion
		
//        #region proprietà}
//        public override int LabelNumber{ get{ return 0; } }
//        #endregion
		
//        #region costruttori
//        [Constructable]
//        public FactionBannerEvil( ) : base( 0x26A0 )
//        {
//        }

//        public FactionBannerEvil( Serial serial ) : base( serial )
//        {
//        }
//        #endregion
		
//        #region metodi
//        #endregion
		
//        #region serial-deserial
//        public override void Serialize( GenericWriter writer )
//        {
//            base.Serialize( writer );

//            writer.WriteEncodedInt( 0 ); // version
//        }

//        public override void Deserialize( GenericReader reader )
//        {
//            base.Deserialize( reader );

//            int version = reader.ReadEncodedInt();
//        }
//        #endregion		
//    }

//    [FlipableAttribute( 0x269A, 0x269B )]
//    public class FactionBannerGood : Item
//    {
//        #region campi
//        #endregion
		
//        #region proprietà}
//        public override int LabelNumber{ get{ return 0; } }
//        #endregion
		
//        #region costruttori
//        [Constructable]
//        public FactionBannerGood( ) : base( 0x269A )
//        {
//        }

//        public FactionBannerGood( Serial serial ) : base( serial )
//        {
//        }
//        #endregion
		
//        #region metodi
//        #endregion
		
//        #region serial-deserial
//        public override void Serialize( GenericWriter writer )
//        {
//            base.Serialize( writer );

//            writer.WriteEncodedInt( 0 ); // version
//        }

//        public override void Deserialize( GenericReader reader )
//        {
//            base.Deserialize( reader );

//            int version = reader.ReadEncodedInt();
//        }
//        #endregion		
//    }
//}
