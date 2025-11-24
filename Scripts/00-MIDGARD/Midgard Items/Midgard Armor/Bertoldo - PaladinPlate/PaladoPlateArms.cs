namespace Server.Items
{
    public class PaladinPlateArms : BaseArmor
    {
        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 2; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 2; } }

        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 65; } }

        public override int AosStrReq { get { return 80; } }
        public override int OldStrReq { get { return 40; } }

        public override int OldDexBonus { get { return -2; } }

        public override int ArmorBase { get { return 40; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }
        private Mobile m_Owner;

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }

        [Constructable]
        public PaladinPlateArms()
            : base( 0x1410 )
        {
            Weight = 5.0;
            Hue = 2023;
            Name = "Paladin Plate Arms";
        }

        public PaladinPlateArms( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
            writer.Write( m_Owner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            if( Weight == 1.0 )
                Weight = 5.0;
            m_Owner = reader.ReadMobile();
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public bool Validate( Mobile m )
        {
            #region modifica by dies irae
            m.SendMessage( 37, "Staff Message: this armor is no longer wearable. Give this armor to a Junk Dealer to obtain something valuable." );
            return false;
            #endregion

            //			if ( !Core.AOS || m == null || !m.Player)			
            //				return true;
            //			try{
            //				if (m.Skills[SkillName.Chivalry].Value<60)
            //					return false;
            //				else				
            //					if (Owner==m)
            //						return true;
            //				return false;
            //			}
            //			catch
            //			{
            //				return false;
            //			}			
        }
    }
}
