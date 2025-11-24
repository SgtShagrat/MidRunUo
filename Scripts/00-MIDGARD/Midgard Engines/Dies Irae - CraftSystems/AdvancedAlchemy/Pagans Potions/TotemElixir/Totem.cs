/***************************************************************************
 *                               Dies Irae - Totem.cs
 *                            --------------------------
 *   begin                : 24 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;
using Server.Mobiles;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class Totem : BasePaganPotion
    {
        public override int Strength
        {
            get { return 1; }
        }

        public override int DelayUse
        {
            get { return 14; }
        }

        public override int BonusOnDelayAtHundred
        {
            get { return 6; }
        }

        public override double AlchemyRequiredToDrink
        {
            get { return 100.0; }
        }

        public override int CustomSound
        {
            get { return 0x1fe; }
        }

        public override int CustomAnim
        {
            get { return 0x0014; }
        }

        public override int CustomEffects
        {
            get { return 0x3728; }
        }

        public static Hashtable Table { get { return m_Table; } }

        private static readonly Hashtable m_Table = new Hashtable();

        [Constructable]
        public Totem( int amount )
            : base( PotionEffect.Totem, amount )
        {
            Hue = 2800;
        }

        [Constructable]
        public Totem()
            : this( 1 )
        {
        }

        #region metodi
        public override bool DoPaganEffect( Mobile from )
        {
            if( !CheckTotemExist( from ) )
            {
                from.SendMessage( "A totem is already walking on this land." );
            }
            else
            {
                Container pack = from.Backpack;
                if( pack == null )
                {
                    return false;
                }

                Elixir el = pack.FindItemByType( typeof( Elixir ) ) as Elixir;
                if( el != null )
                {
                    el.Consume();

                    try
                    {
                        BaseCreature bc = (BaseCreature)Activator.CreateInstance( typeof( TotemCreature ) );

                        if( BaseCreature.Summon( bc, from, from.Location, -1, TimeSpan.FromDays( 1.0 ) ) )
                        {
                            // Copia le props del suo master
                            MasterEmulation( bc );

                            from.SendMessage( "Your friend is now walking on Sosaria." );
                            bc.PlaySound( bc.GetIdleSound() );

                            m_Table[ from ] = bc;

                            LockBasePotionUse( from );
                        }
                    }
                    catch
                    {
                        from.SendMessage( "Something wrong..." );
                    }

                    return true;
                }
                else
                {
                    from.SendMessage( "You must have an elixir potion to summon your totem." );
                }
            }

            return false;
        }

        private static void MasterEmulation( Mobile m )
        {
            TotemCreature totem = m as TotemCreature;
            if (totem == null)
                return;

            // Controlla e setta il ControlMaster del Totem
            Mobile master = totem.ControlMaster;
            if (master == null)
                return;

            // Copia il Nome
            totem.Name = master.Name ?? "a totem creature";
            totem.RawStr += master.RawStr;
            totem.RawInt += master.RawInt;
            totem.RawDex += master.RawDex;

            // Aggiorna Hp, stamina e mana
            totem.Hits = master.Hits;
            totem.Stam = master.Stam;
            totem.Mana = master.Mana;

            // Armatura virtuale
            totem.VirtualArmor = master.VirtualArmor + 20;

            // Copia le skills
            for( int s = 0; s < master.Skills.Length; s++ )
            {
                try
                {
                    if( totem.Skills[ s ].Base < master.Skills[ s ].Base )
                        totem.Skills[ s ].Base = master.Skills[ s ].Base;
                    if( totem.Skills[ s ].Cap < master.Skills[ s ].Cap )
                        totem.Skills[ s ].Cap = master.Skills[ s ].Cap;
                }
                catch
                {
                }
            }
        }

        public bool CheckTotemExist( Mobile from )
        {
            BaseCreature totem = (BaseCreature)m_Table[ from ];

            return totem == null || totem.Deleted;
        }
        #endregion

        #region serial deserial
        public Totem( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}