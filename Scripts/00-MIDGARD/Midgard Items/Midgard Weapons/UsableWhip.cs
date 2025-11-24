/***************************************************************************
 *                               UsableWhip.cs
 *
 *   begin                : 12 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    public class UsableWhip : BaseRanged
    {
        public override int OldStrengthReq { get { return 1; } }

        public override int OldMinDamage { get { return 3; } }

        public override int OldMaxDamage { get { return 15; } }

        public override int OldSpeed { get { return 55; } }

        public override int InitMinHits { get { return 31; } }

        public override int InitMaxHits { get { return 50; } }

        public override int EffectID { get { return 0; } }

        public override Type AmmoType { get { return null; } }

        public override Item Ammo { get { return null; } }

        public override SkillName DefSkill { get { return SkillName.Wrestling; } }

        public override WeaponType DefType { get { return WeaponType.Ranged; } }

        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.ShootXBow; } }

        [Constructable]
        public UsableWhip()
            : base( 0x13f5 )
        {
            Weight = 4.0;
        }

	    public override void OnDoubleClick( Mobile from )
        {
            // do not display ammo gump
        }

        public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
        {
            defender.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "* slap! *" );

            base.OnHit( attacker, defender, damageBonus );
        }

        public override bool OnFired( Mobile attacker, Mobile defender, out Item freccia )
        {
		freccia = null;
            attacker.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "* schiocca la frusta *" );
            return true;
        }

        #region serialization
        public UsableWhip( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}