﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalBallSystem.BLL;
using CrystalBallSystem.DAL.Entities;
using CrystalBallSystem.DAL.POCOs;

public partial class Admin_Equivalencies : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    protected void AddNew_Click(object sender, EventArgs e)
    {
        //equivalencyInformation.Visible = false;
        addNewEquivalency.Visible = true;
    }
    protected void CheckIDs_Click(object sender, EventArgs e)
    {
        AshleyTestController sysmgr = new AshleyTestController();
        string courseCode = EmptyCurrentTextBox.Text;
        NAITCourse courseInfo = sysmgr.GetCourseName(courseCode);
        CurrentCourseName.Text = courseInfo.CourseName;
        CurrentCourseID.Text = courseInfo.CourseID.ToString();

        courseCode = EmptyEquivalentTextBox.Text;
        courseInfo = sysmgr.GetCourseName(courseCode);
        EquivalentCourseName.Text = courseInfo.CourseName;
        EquivalentCourseID.Text = courseInfo.CourseID.ToString();
    }


    protected void Enter_Click(object sender, EventArgs e)
    {
        AshleyTestController sysmgr = new AshleyTestController();
        int programID = int.Parse(ProgramDropdownList.SelectedValue); 
        int courseID = int.Parse(CurrentCourseID.Text);
        int destinationCourseID = int.Parse(EquivalentCourseID.Text);
        sysmgr.AddEquivalency(programID, courseID, destinationCourseID);
    }
}