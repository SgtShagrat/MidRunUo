/***************************************************************************
 *                                  OldShrinkItem.cs
 *                            		-------------------
 *  begin                	: Agosto, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  L'oggetto permette lo Shrink e l'Un-Shrink dei pet di Midgard.
 *  
 ***************************************************************************/

using System;
using System.IO;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class OldShrinkItem : Item
    {
        #region Campi
        private bool m_Filled;
        private bool m_PetControlled;
        private string m_PetControlMasterName;
        private int m_PetHue;
        private bool m_PetIsBonded;
        private string m_PetName;
        private string m_PetTypeString;
        #endregion

        #region Proprietà
        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public bool Filled
        {
            get { return m_Filled; }
            set
            {
                m_Filled = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public int PetHue
        {
            get { return m_PetHue; }
            set
            {
                m_PetHue = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public string PetName
        {
            get { return m_PetName; }
            set
            {
                m_PetName = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public bool PetControlled
        {
            get { return m_PetControlled; }
            set
            {
                m_PetControlled = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public string PetControlMasterName
        {
            get { return m_PetControlMasterName; }
            set
            {
                m_PetControlMasterName = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public bool PetBonded
        {
            get { return m_PetIsBonded; }
            set
            {
                m_PetIsBonded = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public string PetTypeString
        {
            get { return m_PetTypeString; }
            set
            {
                m_PetTypeString = value;
                InvalidateProperties();
            }
        }
        #endregion

        #region Cotruttori
        [Constructable]
        public OldShrinkItem()
            : base()
        {
            m_Filled = false;
            m_PetControlled = false;
            m_PetControlMasterName = string.Empty;
            m_PetHue = 0;
            m_PetIsBonded = false;
            m_PetName = string.Empty;
            m_PetTypeString = string.Empty;

            ItemID = 0x1870;
            Name = "a pet gem";
            Movable = true;
            Hue = 1152;
            LootType = LootType.Blessed;
            Weight = 25.0;
        }

        public OldShrinkItem( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region Metodi
        public override void OnDoubleClick( Mobile from )
        {
            PlayerMobile From = from as PlayerMobile;

            if( From == null )
                return;

            if( !IsChildOf( From.Backpack ) )
            {
                From.SendMessage( "La gemma deve essere nel tuo zaino per essere usata." );
                return;
            }

            if( m_Filled )
            {
                from.SendMessage( "Stai tentando di evocare l'animale nella gemma." );

                if( m_PetControlMasterName == from.Name )
                {
                    from.SendMessage( "L'animale nella gemma ti riconosce come suo Padrone." );

                    if( m_PetTypeString != string.Empty )
                    {
                        object o = Activator.CreateInstance( SpawnerType.GetType( m_PetTypeString ) );

                        BaseCreature creature = o as BaseCreature;

                        if( creature != null )
                        {
                            creature.Hue = m_PetHue;
                            creature.Name = m_PetName;
                            creature.Controlled = m_PetControlled;
                            creature.ControlMaster = from;
                            creature.IsBonded = m_PetIsBonded;

                            creature.MoveToWorld( from.Location, from.Map );
                        }

                        Delete();
                    }
                }
                else
                    from.SendMessage( "L'animale nella gemma si rifiuta di uscire perchè non ti appartiene." );
            }
            else
            {
                From.BeginTarget( 2, false, TargetFlags.None, new TargetCallback( OnTarget ) );
                From.SendMessage( "Seleziona l'animale che vuoi far entrare nella gemma." );
            }
        }

        public void OnTarget( Mobile from, object obj )
        {
            if( Deleted )
                return;

            BaseCreature pet = obj as BaseCreature;
            if( pet == null )
                return;

            if( from != pet.ControlMaster || !pet.Controlled )
            {
                from.SendMessage( "La creatura si rifiuta di entrare nella gemma perchè non " +
                                  "sei il suo Padrone." );
            }
            else
            {
                // Setta le props della statuetta
                m_Filled = true;
                m_PetHue = pet.Hue;
                m_PetName = pet.Name;
                m_PetControlled = pet.Controlled;
                m_PetControlMasterName = from.Name;
                m_PetIsBonded = pet.IsBonded;
                m_PetTypeString = pet.GetType().Name;

                string FileName = "LogUsiShrinkItem.txt";
                string FilePath = Path.Combine( "Porting", FileName );

                TextWriter tw = File.AppendText( FilePath );
                try
                {
                    tw.WriteLine( "L'utente: {0} (Account: {1} ) ha usato uno ShrinkItem alle ore: {2} del {3} per shrinkare un {4} di nome -{5}- e di colore -{6}-.",
                        from.Name, from.Account, DateTime.Now.ToShortTimeString(),
                        DateTime.Now.Date.ToShortDateString(), m_PetTypeString,
                        m_PetName, m_PetHue );
                }
                finally
                {
                    tw.Close();
                }

                pet.Delete();

                InvalidateProperties();
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( m_Filled )
            {
                list.Add( 1060747 );
                if( m_PetName != string.Empty )
                    list.Add( 1060658, "Pet Name\t{0}", m_PetName );
                if( m_PetTypeString != string.Empty )
                    list.Add( 1060659, "Pet Type\t{0}", m_PetTypeString );
                if( m_PetHue != 0 )
                    list.Add( 1060660, "Pet Hue\t{0}", m_PetHue.ToString() );
            }
            else
                list.Add( 1074036 );
        }
        #endregion

        #region Serial-Deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( (bool)m_Filled );
            writer.Write( (int)m_PetHue );
            writer.Write( (string)m_PetName );
            writer.Write( (bool)m_PetControlled );
            writer.Write( (string)m_PetControlMasterName );
            writer.Write( (bool)m_PetIsBonded );
            writer.Write( (string)m_PetTypeString );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Filled = (bool)reader.ReadBool();
            m_PetHue = (int)reader.ReadInt();
            m_PetName = (string)reader.ReadString();
            m_PetControlled = (bool)reader.ReadBool();
            m_PetControlMasterName = (string)reader.ReadString();
            m_PetIsBonded = (bool)reader.ReadBool();
            m_PetTypeString = (string)reader.ReadString();
        }
        #endregion
    }
}