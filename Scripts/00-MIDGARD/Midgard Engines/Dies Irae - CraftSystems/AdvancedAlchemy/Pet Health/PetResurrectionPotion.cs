/***************************************************************************
 *                               PetResurrectionPotion.cs
 *
 *   begin                : 28 luglio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Midgard.Menus;

using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class PetResurrectionPotion : BasePetHealthPotion
    {
        public override double PercProperFun { get { return 0.75; } }
        public override int DelayUse { get { return 10; } }

        [Constructable]
        public PetResurrectionPotion( int amount )
            : base( PotionEffect.PetResurrection, amount )
        {
            // Name = "Pet Resurrection Potion";
            Hue = 1943;
        }

        [Constructable]
        public PetResurrectionPotion()
            : this( 1 )
        {
        }

        public override void DoPetHealthEffect( Mobile user, BaseCreature pet )
        {
            if( pet.IsDeadPet )
            {
                if( Core.AOS )
                    user.SendGump( new PetResurrectGump( user, pet ) );
                else
                    user.SendMenu( new PetResurrectionMenu( pet, 0.10, this ) );
            }
            else
            {
                user.SendMessage( "Il tuo animale non è morto!" );
            }
        }

        #region serial deserial
        public PetResurrectionPotion( Serial serial )
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