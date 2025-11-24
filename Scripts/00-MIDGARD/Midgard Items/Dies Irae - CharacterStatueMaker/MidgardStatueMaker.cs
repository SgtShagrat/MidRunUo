/***************************************************************************
 *                               MidgardStatueMaker.cs
 *
 *   begin                : 07 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Items.StatueSystem
{
    public class MidgardStatueMaker : Item, ICraftable, IResourceItem
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsStaffGranted { get; set; }

        private CraftResource m_Resource;

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                m_Resource = value;
                Hue = CraftResources.GetHue( m_Resource );
            }
        }

        [Constructable]
        public MidgardStatueMaker()
            : this( CraftResource.OldBronze )
        {
        }

        [Constructable]
        public MidgardStatueMaker( CraftResource resource )
            : base( 0x32F0 )
        {
            Resource = resource;
            IsStaffGranted = false;

            LootType = LootType.Blessed;
            Weight = 30.0;
        }

        public override string DefaultName
        {
            get { return "a midgard statue maker"; }
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, "a {0} midgard statue maker", GetMaterialName( Resource ).ToLower() );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsChildOf( from.Backpack ) )
            {
                if( !from.IsBodyMod )
                {
                    from.SendLocalizedMessage( 1076194 ); // Select a place where you would like to put your statue.
                    from.Target = new MidgardStatueTarget( m_Resource, this );
                }
                else
                    from.SendLocalizedMessage( 1073648 ); // You may only proceed while in your original state...
            }
            else
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
        }

        private static string GetMaterialName( CraftResource material )
        {
            CraftResourceInfo info = CraftResources.GetInfo( material );
            return info != null ? info.Name : "";
        }

        #region serialization
        public MidgardStatueMaker( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            writer.Write( (int)m_Resource );
            writer.Write( IsStaffGranted );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            m_Resource = (CraftResource)reader.ReadInt();
            IsStaffGranted = reader.ReadBool();
        }
        #endregion

        #region ICraftable Members
        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes,
                            BaseTool tool, CraftItem craftItem, int resHue )
        {
            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            Type resourceType = typeRes;

            if( resourceType == null )
                resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

            Resource = CraftResources.GetFromType( resourceType );

            CraftContext context = craftSystem.GetContext( from );

            if( context != null && context.DoNotColor )
                Hue = 0;

            return quality;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }
        #endregion
    }
}