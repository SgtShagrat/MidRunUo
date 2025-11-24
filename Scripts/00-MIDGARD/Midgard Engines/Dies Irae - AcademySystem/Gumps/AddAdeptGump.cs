/***************************************************************************
 *                               AddAdeptGump.cs
 *
 *   begin                : 07 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.Academies
{
    public class AddAdeptGump : PlayerStateSelectionGump
    {
        public override string Title { get { return "Adept selection:"; } }

        public AddAdeptGump( AcademySystem academy, Mobile owner, TrainingClass trainingClass, AcademyAccessFlags flag )
            : base( academy, owner, null, flag, AddAdept_Callback, trainingClass )
        {
        }

        private static void AddAdept_Callback( Mobile from, AcademyPlayerState playerState, object state )
        {
            TrainingClass trainingClass = state as TrainingClass;
            if( trainingClass != null )
                trainingClass.AddTrainer( playerState );
        }
    }
}