﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#region Additional namespace

using CrystalBallSystem.DAL.Entities;
using CrystalBallSystem.DAL;
using System.ComponentModel;
using CrystalBallSystem.DAL.POCOs;

#endregion


namespace CrystalBallSystem.BLL
{
    [DataObject]
    public class StudentController
    {
        #region add nait course
        public void AddCourse(NaitCours item)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                // TODO: Validation rules...
                var added = context.NaitCourses.Add(item);
                context.SaveChanges();
            }
        }
        #endregion

        //select method that will populate the drop down list allowing a user to select courses.
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<GetHSCourses> GetCourseList()
        {
            using (var context = new CrystalBallContext())
            {
                var results = from course in context.HighSchoolCourses
                              orderby course.HighSchoolCourseName
                              select new GetHSCourses
                              {
                                  HighSchoolCourseID = course.HighSchoolCourseID,
                                  HighSchoolCourseDescription = course.HighSchoolCourseName
                              };
                return results.ToList();
            }
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public int GetEntranceList(int courseID, int mark)
        {
            using (var context = new CrystalBallContext())
            {
                var results = (from entrance in context.EntranceRequirements
                               orderby entrance.EntranceRequirementID
                               where entrance.HighSchoolCourseID == courseID && mark >= entrance.RequiredMark
                               select entrance.EntranceRequirementID).FirstOrDefault();
                return results;
            }
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public int[] GetPrograms(int[] entranceID)
        {
            List<int> returnArray = new List<int>();
            using (var context = new CrystalBallContext())
            {
                //create a list inside a list that can be cast as an array down the line and will contain
                //the program id and relevant return variables including program name etc etc
                for (int i = 0; i < entranceID.Length; i++)
                {
                    var results = (from program in context.EntranceRequirements
                                  where program.EntranceRequirementID == entranceID[i] && program.Program.Active == true
                                  select program.ProgramID).First();
                    returnArray.Add(results);
                }
            }
            int[] newArray = returnArray.ToArray();
            return newArray;
        }

        #region preference questions
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<PreferenceQuestion> GetQuestions()
        {
            using (var context = new CrystalBallContext())
            {
                var results = from row in context.PreferenceQuestions
                              orderby row.QuestionID
                              select row;
                return results.ToList();
            }
        }




        #endregion

        
    
    }
}