using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.Races;

using Server;
using Server.Items;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Items
{
    /// <summary>
    /// 0x266A Handcuffs trad. Manette - ( craftabile esclusivamente dal pg fabbro, loot di orchi e scheletri,)
    /// </summary>
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
    public class Handcuffs : GoldBracelet
    {
        public override string DefaultName { get { return "handcuffs"; } }

        [Constructable]
        public Handcuffs()
        {
            ItemID = 0x266A;
        }

        public override void OnDoubleClick( Mobile from )
        {
            TownSystem sys = TownSystem.Find( from );
            if( sys == null )
                return;

            if( TownHelper.IsInHisOwnCity( from ) && sys.CanCommandGuards( from ) )
            {
                from.SendMessage( "Choose the person thou would to arrest!" );
                from.Target = new InternalTarget( this );
            }
            else
                base.OnDoubleClick( from );
        }

        #region serialization
        public Handcuffs( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        private class InternalTarget : Target
        {
            private Handcuffs m_Owner;

            public InternalTarget( Handcuffs owner )
                : base( 15, false, TargetFlags.None )
            {
                m_Owner = owner;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is Mobile )
                {
                    m_Owner.Target( from, (Mobile)o );
                }
            }
        }

        public void Target( Mobile from, Mobile target )
        {
            if( !from.Alive || !target.Alive )
                return;

            SpellHelper.Turn( from, target );

            TownSystem sys = TownSystem.Find( from );
            if( sys == null )
                return;

            if( TownHelper.IsInHisOwnCity( from ) && sys.IsInTown( target ) )
            {
                sys.RegisterCriminal( target, (CrimeType)Utility.RandomMinMax( 0, 1 ) );
                sys.TryArrest( from, target, true );
            }
        }
    }
}