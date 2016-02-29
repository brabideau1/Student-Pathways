﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#region Additional namespace
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using CrystalBallSystem.DAL.Entities;
using CrystalBallSystem.DAL.POCOs;
using CrystalBallSystem.DAL;
using System.ComponentModel;
#endregion

namespace CrystalBallSystem.BLL
{
    public class AdminController
    {
        #region Admin management

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<Category> Category_List()
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                return context.Categories.OrderBy(x => x.CategoryID).ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<HighSchoolCours> HighSchoolCourse_List()
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                return context.HighSchoolCourses.OrderBy(x => x.HighSchoolCourseID).ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CredentialType> CredentialType_List()
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                return context.CredentialTypes.OrderBy(x => x.CredentialTypeID).ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public void AddCategory(Category item)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                Category added = null;
                added = context.Categories.Add(item);
                context.SaveChanges();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public void UpdateCategory(Category item)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                context.Entry<Category>(context.Categories.Attach(item)).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<Program> GetProgramByCategory(int categoryID)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                //var results = from row in context.Programs.Where(s => s.Categories.Any(c => c.CategoryID == categoryID))
                //from s in context.Programs
                //from c in s.Categories
                //where c.CategoryID == categoryID
                return context.Programs.Where(p => p.Categories.Any(c => c.CategoryID == categoryID)).ToList();

                //var results = from row in context.Categories
                //              where row.CategoryID == categoryID
                //              select new
                //              {

                //                  Programs = from info in row.Programs
                //                              where info.Active == true
                //                              select new ProgramSummary()
                //                              {
                //                                  ProgramName = info.ProgramName,
                //                                  EntranceSummary = info.ProgramDescription,
                //                                  CompetitiveEntrance = info.CompetitiveAdvantage,
                //                                  TotalCredits = info.TotalCredits,
                //                                  ProgramLink = info.ProgramLink

                //                              }

                //              };

                //return results.Cast<ProgramSummary>().ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public void Program_Update(Program item)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                Program data = new Program()
                {
                    ProgramID = item.ProgramID,
                    ProgramName = item.ProgramName,
                    ProgramDescription = item.ProgramDescription,
                    ProgramLength = item.ProgramLength,
                    ProgramLink = item.ProgramLink,
                    Active = item.Active,
                    CompetitiveAdvantage = item.CompetitiveAdvantage,
                    TotalCredits = item.TotalCredits,
                    CredentialTypeID = item.CredentialTypeID
                };

                context.Entry<Program>(context.Programs.Attach(data)).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public void AddProgram(List<Program> program, int categoryid)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                Program added = null;

                var newProgram = new Program();
                int newProgramID = newProgram.ProgramID;

                foreach (var item in program)
                {
                    added = context.Programs.Add(new Program()
                    {
                        ProgramID = newProgramID,
                        CredentialTypeID = item.CredentialTypeID,
                        ProgramName = item.ProgramName,
                        ProgramDescription = item.ProgramDescription,
                        TotalCredits = item.TotalCredits,
                        ProgramLength = item.ProgramLength,
                        CompetitiveAdvantage = item.CompetitiveAdvantage,
                        Active = item.Active,
                        ProgramLink = item.ProgramLink
                    });
                }

                Category newCategory = new Category { CategoryID = categoryid };
                context.Categories.Add(newCategory);
                context.Categories.Attach(newCategory);
                added.Categories.Add(newCategory);

                context.SaveChanges();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<NaitCours> GetCoursesByProgram(int programID)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {

                return context.NaitCourses.Where(c => c.Programs.Any(p => p.ProgramID == programID)).ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public void AddNaitCourse(List<NaitCours> courses, int programid)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                NaitCours added = null;

                var newCourse = new NaitCours();
                int newCourseID = newCourse.CourseID;

                //added = context.NaitCourses.Add(item);
                foreach (var item in courses)
                {
                    added = context.NaitCourses.Add(new NaitCours()
                    {
                        CourseID = newCourseID,
                        CourseCode = item.CourseCode,
                        CourseName = item.CourseName,
                        CourseCredits = item.CourseCredits,
                        Active = item.Active
                    });


                }

                Program newProgram = new Program { ProgramID = programid };
                context.Programs.Add(newProgram);
                context.Programs.Attach(newProgram);
                added.Programs.Add(newProgram);

                context.SaveChanges();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public void AddCourse(NaitCours item, int programID)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                NaitCours added = null;

                added = context.NaitCourses.Add(item);

                Program newProgram = new Program { ProgramID = programID };
                context.Programs.Add(newProgram);
                context.Programs.Attach(newProgram);
                added.Programs.Add(newProgram);

                context.SaveChanges();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public void UpdateNaitCourse(NaitCours item)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                NaitCours data = new NaitCours()
                {
                    CourseID = item.CourseID,
                    CourseCode = item.CourseCode,
                    CourseName = item.CourseName,
                    CourseCredits = item.CourseCredits,
                    Active = item.Active,

                };

                context.Entry<NaitCours>(context.NaitCourses.Attach(data)).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public void HighSchoolCourse_Update(HighSchoolCours item)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                HighSchoolCours data = new HighSchoolCours()
                {
                    HighSchoolCourseID = item.HighSchoolCourseID,
                    HighSchoolCourseName = item.HighSchoolCourseName
                };

                context.Entry<HighSchoolCours>(context.HighSchoolCourses.Attach(data)).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public void AddHighSchoolCourse(HighSchoolCours item)
        {
            using (CrystalBallContext context = new CrystalBallContext())
            {
                // TODO: Validation rules...
                var added = context.HighSchoolCourses.Add(item);
                context.SaveChanges();
            }
        }

        #endregion

        #region requirement

        #endregion
        #region report

        #endregion

    }
}
