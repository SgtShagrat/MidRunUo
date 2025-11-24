/***************************************************************************
 *                               TrainingClass.cs
 *
 *   begin                : 07 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Midgard.Engines.Academies
{
    public class TrainingClass
    {
        public TrainingClass( Disciplines discipline, string name )
        {
            Discipline = discipline;
            Name = name;

            CreationTime = DateTime.Now;
        }

        public Disciplines Discipline { get; private set; }
        public string Name { get; set; }

        public List<AcademyPlayerState> Trainers { get; private set; }

        public List<AcademyPlayerState> Teachers { get; private set; }

        public DateTime CreationTime { get; private set; }


        public List<AcademyPlayerState> Members
        {
            get
            {
                List<AcademyPlayerState> list = new List<AcademyPlayerState>();

                if( Teachers != null )
                    list.AddRange( Teachers );

                if( Trainers != null )
                    list.AddRange( Trainers );

                return list;
            }
        }

        public bool AddTeacher( AcademyPlayerState state )
        {
            if( Teachers == null )
                Teachers = new List<AcademyPlayerState>();

            if( !Teachers.Contains( state ) )
            {
                Teachers.Add( state );

                state.BeginTeaching( Discipline );

                return true;
            }
            else
                return false;
        }

        public bool IsTeacher( AcademyPlayerState from )
        {
            return Teachers != null && Teachers.Contains( from );
        }

        public bool IsTrainer( AcademyPlayerState from )
        {
            return Trainers != null && Trainers.Contains( from );
        }

        public bool AddTrainer( AcademyPlayerState state )
        {
            if( Trainers == null )
                Trainers = new List<AcademyPlayerState>();

            if( !Trainers.Contains( state ) )
            {
                Trainers.Add( state );

                state.BeginLearning( Discipline );

                return true;
            }
            else
                return false;
        }

        public void RemoveTeacher( AcademyPlayerState state )
        {
            if( Teachers == null )
                return;

            if( Teachers.Contains( state ) )
                Teachers.Remove( state );

            state.EndTeaching( Discipline );
        }

        public void RemoveTrainer( AcademyPlayerState state )
        {
            if( Trainers == null )
                return;

            if( Trainers.Contains( state ) )
                Trainers.Remove( state );

            state.EndLearning( Discipline );
        }

        public override string ToString()
        {
            return Name;
        }

        public void RemoveMember( AcademyPlayerState state )
        {
            RemoveTeacher( state );
            RemoveTrainer( state );
        }
    }
}