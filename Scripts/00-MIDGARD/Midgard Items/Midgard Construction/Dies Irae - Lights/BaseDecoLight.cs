using System;
using System.Text;

using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseDecoLight : BaseLight, ICraftable, IResourceItem
    {
        private Mobile m_Crafter;
        private CraftResource m_Resource;

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                if( m_Resource != value )
                {
                    m_Resource = value;
                    Hue = CraftResources.GetHue( m_Resource );

                    InvalidateSecondAgeName();
                }
            }
        }

        public BaseDecoLight( int itemID )
            : base( itemID )
        {
        }

        public BaseDecoLight( Serial serial )
            : base( serial )
        {
        }

        private string m_SecondAgeFullName;

        public void InvalidateSecondAgeName()
        {
            StringBuilder info = new StringBuilder();

            CraftResourceInfo materialName = CraftResources.IsStandard( m_Resource ) ? null : CraftResources.GetInfo( m_Resource );

            // 'exceptional '
            string customQualName = QualityName;
            if( customQualName.Length > 0 )
                info.AppendFormat( "{0} ", customQualName );

            if( materialName != null && materialName.Name != null )
                info.AppendFormat( "{0} ", materialName.Name ); // 'bronze '

            info.Append( string.IsNullOrEmpty( Name ) ? StringList.Localization[ LabelNumber ] : Name ); // 'scarecrow'

            if( Crafter != null && !String.IsNullOrEmpty( Crafter.Name ) )
                info.AppendFormat( " crafted by {0}", Crafter.Name ); // ' crafted by Dies Irae'


            m_SecondAgeFullName = info.ToString();
        }

        public override void OnSingleClick( Mobile from )
        {
            if( m_SecondAgeFullName == null )
                InvalidateSecondAgeName();

            LabelTo( from, m_SecondAgeFullName );
        }

        #region ICraftable Members
        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
        {
            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            Type resourceType = typeRes ?? craftItem.Resources.GetAt( 0 ).ItemType;

            Resource = CraftResources.GetFromType( resourceType );

            CraftContext context = craftSystem.GetContext( from );

            if( context != null && context.DoNotColor )
                Hue = 0;

            return quality;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set
            {
                m_Crafter = value;
                InvalidateSecondAgeName();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_Crafter );
            writer.Write( (int)m_Resource );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Crafter = reader.ReadMobile();
            m_Resource = (CraftResource)reader.ReadInt();
        }
        #endregion
    }
}