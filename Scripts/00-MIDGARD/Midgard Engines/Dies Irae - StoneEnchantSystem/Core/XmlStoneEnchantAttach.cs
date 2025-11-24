/***************************************************************************
 *                                  XmlStoneEnchantAttach.cs
 *                            		------------------------
 *  begin                	: Ottobre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Engines.XmlSpawner2;
using Server.Items;
using System.Collections;

namespace Midgard.Engines.StoneEnchantSystem
{
    public class XmlStoneEnchantAttach : XmlAttachment
    {
        #region fields
        private StoneTypes m_StoneType;
        private int m_Level;
        private int m_Originalhue;

        public StoneTypes StoneType
        {
            get { return m_StoneType; }
        }

        public int Level
        {
            get { return m_Level; }
        }

        public int Originalhue
        {
            get { return m_Originalhue; }
        }
        #endregion

        #region constructors
        [Attachable]
        public XmlStoneEnchantAttach( StoneTypes stoneType, int level )
        {
            m_StoneType = stoneType;
            m_Level = level;
        }

        public XmlStoneEnchantAttach( ASerial serial )
            : base( serial )
        {
        }
        #endregion

        #region members
        public virtual void AddStoneProperties( ObjectPropertyList list )
        {
            list.Add( "Infused With {0} {1} Power", StoneEnchantHelper.GetStringLevel( m_Level ), m_StoneType.ToString() );
        }

        public override void OnAttach()
        {
            base.OnAttach();

            if( AttachedTo is Item )
            {
                Item i = AttachedTo as Item;
                if( i != null )
                {
                    m_Originalhue = i.Hue;
                    i.Hue = StoneEnchantHelper.GetHueFromStoneType( m_StoneType );
                }
            }
            else
                Delete();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if( AttachedTo is Item )
                ( (Item)AttachedTo ).Hue = m_Originalhue;
        }

        public void DoArmorSpecialEffect( Mobile attacker, Mobile defender )
        {
            if( !( AttachedTo is BaseArmor ) )
                return;

            BaseArmor armor = (BaseArmor)AttachedTo;
            if( armor == null )
                return;

            switch( m_StoneType )
            {
                /*
                case StoneTypes.Explosion: armor.DoAreaAttack( attacker, defender, 0x10E,   50, 100, 0, 0, 0, 0 );
                case StoneTypes.Explosion: armor.DoAreaAttack( attacker, defender, 0x11D, 1160, 0, 100, 0, 0, 0 );
                case StoneTypes.Explosion: armor.DoAreaAttack( attacker, defender, 0x0FC, 2100, 0, 0, 100, 0, 0 );
                case StoneTypes.Explosion: armor.DoAreaAttack( attacker, defender, 0x205, 1166, 0, 0, 0, 100, 0 );
                case StoneTypes.Explosion: armor.DoAreaAttack( attacker, defender, 0x1F1,  120, 0, 0, 0, 0, 100 );	
                */
                case StoneTypes.Phoenix: armor.DoAreaAttack( attacker, defender, 0x11D, 1160, 0, 100, 0, 0, 0 ); break;
                case StoneTypes.Flamming:
                case StoneTypes.Electrical:
                case StoneTypes.Cloud:
                case StoneTypes.Explosion:
                case StoneTypes.Lockmind:
                case StoneTypes.Spectral:
                case StoneTypes.Serpent:
                case StoneTypes.Perforate:
                case StoneTypes.Antimagical:
                case StoneTypes.Vampirical:
                case StoneTypes.Mammoth:
                case StoneTypes.Sonical:
                case StoneTypes.Paralyze:
                case StoneTypes.Venom:
                default: break;
            }
        }

        public void DoWeaponSpecialEffect( Mobile attacker, Mobile defender )
        {
            if( !( AttachedTo is BaseWeapon ) )
                return;

            BaseWeapon weapon = (BaseWeapon)AttachedTo;
            if( weapon == null )
                return;

            switch( m_StoneType )
            {
                case StoneTypes.Phoenix:
                case StoneTypes.Flamming:
                case StoneTypes.Electrical:
                case StoneTypes.Cloud:
                case StoneTypes.Explosion:
                case StoneTypes.Lockmind:
                case StoneTypes.Spectral:
                case StoneTypes.Serpent:
                case StoneTypes.Perforate:
                case StoneTypes.Antimagical:
                case StoneTypes.Vampirical:
                case StoneTypes.Mammoth:
                case StoneTypes.Sonical:
                case StoneTypes.Paralyze:
                case StoneTypes.Venom:
                default: break;
            }
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( (int)m_StoneType );
            writer.Write( m_Level );

            writer.Write( m_Originalhue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_StoneType = (StoneTypes)reader.ReadInt();
            m_Level = reader.ReadInt();

            m_Originalhue = reader.ReadInt();
        }
        #endregion
    }
}
