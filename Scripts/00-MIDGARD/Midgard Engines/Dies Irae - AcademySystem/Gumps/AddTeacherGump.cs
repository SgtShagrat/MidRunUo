/***************************************************************************
 *                               AddTeacherGump.cs
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
    public class AddTeacherGump : PlayerStateSelectionGump
    {
        public override string Title { get { return "Teacher selection:"; } }

        public AddTeacherGump( AcademySystem academy, Mobile owner, TrainingClass trainingClass, AcademyAccessFlags flag )
            : base( academy, owner, null, flag, AddTeacher_Callback, trainingClass )
        {
        }

        private static void AddTeacher_Callback( Mobile from, AcademyPlayerState playerState, object state )
        {
            TrainingClass trainingClass = state as TrainingClass;
            if( trainingClass != null )
                trainingClass.AddTeacher( playerState );
        }
    }
}