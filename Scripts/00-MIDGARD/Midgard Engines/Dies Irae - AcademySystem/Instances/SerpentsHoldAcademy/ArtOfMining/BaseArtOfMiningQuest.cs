/***************************************************************************
 *                               BaseArtOfMiningQuest.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.Academies
{
    public class BaseArtOfMiningQuest : BaseAcademyQuest
    {
        public override AcademySystem Academy
        {
            get { return AcademySystem.SerpentsHoldAcademy; }
        }

        public override Disciplines Discipline
        {
            get { return Disciplines.ArtOfMining; }
        }

        #region serialization
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