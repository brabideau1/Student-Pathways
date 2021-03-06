﻿using CrystalBallSystem.BLL;
using CrystalBallSystem.DAL.Entities;
using CrystalBallSystem.DAL.POCOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Student_StudentPreferences : System.Web.UI.Page
{
    DataTable CoursesSelected;
    GetDegEntReqs degree = new GetDegEntReqs();

    /*
     * On page load create the session, view, and repeater that will house the select NAIT courses view
     * This page will allow the user to add courses to a 'basket' and automatically calculate the number of
     * courses added. Allows for add/delete functionality in addition to searching programs.
     */
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataColumn CourseID;
            DataColumn CourseCode;
            DataColumn CourseName;
            DataColumn CourseCredits;
            CoursesSelected = new DataTable();
                CourseID = new DataColumn();
                CourseID.DataType = System.Type.GetType("System.Int32");
                CourseID.ColumnName = "CourseID";
                CourseID.Caption = "CourseID";
                CoursesSelected.Columns.Add(CourseID);

                CourseCode = new DataColumn();
                CourseCode.DataType = System.Type.GetType("System.String");
                CourseCode.ColumnName = "CourseCode";
                CourseCode.Caption = "CourseCode";
                CoursesSelected.Columns.Add(CourseCode);

                CourseName = new DataColumn();
                CourseName.DataType = System.Type.GetType("System.String");
                CourseName.ColumnName = "CourseName";
                CourseName.Caption = "CourseName";
                CoursesSelected.Columns.Add(CourseName);

                CourseCredits = new DataColumn();
                CourseCredits.DataType = System.Type.GetType("System.Double");
                CourseCredits.ColumnName = "CourseCredits";
                CourseCredits.Caption = "CourseCredits";
                CoursesSelected.Columns.Add(CourseCredits);

                DataColumn[] pCol = new DataColumn[1];
                pCol[0] = CourseID;
                CoursesSelected.PrimaryKey = pCol;
                ViewState["CoursesSelected"] = CoursesSelected;

                ViewState["BackupTable"] = CoursesSelected;

            int count = 0;
            foreach (DataRow row1 in CoursesSelected.Rows)
            {
                count++;
            }
            TotalCourseLabel.Text = "Total courses : " + count;
            rptCourse.DataSource = CoursesSelected;
            rptCourse.DataBind();
            CourseGridView.Visible = false;
        }
    }

    /*
     * When user clicks the submit button the program will log all of the options selected and send them to the database step by step to determine the final results that should be returned.
     * Once those results are gathered the final view will be displayed with the returned values from the databased bound to a gridview.
     */
    protected void Submit_Click(object sender, EventArgs e)
    {
        //step 1 - Gather program information from the student. This is primarily used in metrics data to see what programs see students looking to transfer.
        int? programCategoryID, programID, semester;
        bool? programChange;
        StudentController sysmgr = new StudentController();
        int tempInt;

        if (RBL_NAIT_Student.SelectedValue == "1" && int.TryParse((CategoryDropDown.SelectedValue), out tempInt) && int.TryParse(ProgramDropDown.SelectedValue, out tempInt) && int.TryParse(SemesterDropDown.SelectedValue, out tempInt))
        {
            programCategoryID = Convert.ToInt32(CategoryDropDown.SelectedValue);
            programID = Convert.ToInt32(ProgramDropDown.SelectedValue);
            programChange = Convert.ToBoolean(RBL_SwapPrograms.SelectedValue);
            semester = Convert.ToInt32(SemesterDropDown.SelectedValue);
        }
        else
        {
            programCategoryID = null;
            programID = null;
            programChange = null;
            semester = null;
        }

        try
        {
            //step 2 - Gather the answers to the student preference questions
            List<StudentPreference> myPreferences = new List<StudentPreference>();
            foreach (GridViewRow row in prefGridView.Rows)
            {
                RadioButtonList rlist = row.FindControl("prefSelection") as RadioButtonList;
                int prefchoice = Convert.ToInt32(rlist.SelectedValue);

                myPreferences.Add(new StudentPreference(
                                Convert.ToInt32((row.FindControl("QuestionID") as Label).Text),

                                prefchoice
                ));
            }

            //prior to first step towards getting results log the student data for metrics gathering
            //including their current program, semester, and desire to change programs.
            ReportController report = new ReportController();
            int currentProgID, currentSemester;
            bool changeProgram = true;
            if (RBL_NAIT_Student.SelectedValue == "1")
            {
                currentProgID = Convert.ToInt32(ProgramDropDown.SelectedValue);
                currentSemester = Convert.ToInt32(SemesterDropDown.SelectedValue);
                if (RBL_SwapPrograms.SelectedValue == "false")
                {
                    changeProgram = false;
                }

                report.InsertCurrentStudentMetrics(myPreferences, currentProgID, currentSemester, changeProgram);
            }
            else
            {
                report.InsertNewStudentMetrics(myPreferences);
            }

            //step 3 - Gather the selected courses provided by the student. If a selected course has a higher
            //level than others in the same course group it will add that course in addition to the one selected

            //search for all checked items in the high school course list
            List<int> demoCourses = new List<int>();
            foreach (ListItem item in CB_CourseList.Items)
            {
                if (item.Selected)
                    demoCourses.Add(Convert.ToInt32(item.Value));
            }

            DataTable CoursesSelected = (DataTable)ViewState["CoursesSelected"];
            List<int> naitcourseids = new List<int>();
            foreach (DataRow x in CoursesSelected.Rows)
            {
                naitcourseids.Add(Convert.ToInt32(x["CourseID"]));
            }
            List<ProgramResult> finalProgramResults = StudentController.EntranceReq_Pref_Match(myPreferences, naitcourseids, demoCourses, degree);
            Session["finalProgramResults"] = finalProgramResults;

            ResultsView.DataSource = finalProgramResults;
            ResultsView.DataBind();

            Show_Results(sender, e);
        }
        catch (Exception error)
        {
            MessageUserControl.ShowInfo(error.Message);
        }
    }
    //changes student information gathering page to invisible if the user is not a student
    protected void CurrentStudent_CheckedChanged(object sender, EventArgs e)
    {
        if (RBL_NAIT_Student.SelectedValue == "1")
        {
            chooseProgram.Visible = true;
        }
        else
        {
            chooseProgram.Visible = false;
        }
    }

    //populates the program drop down list on the student information gathering page
    protected void Populate_Program(object sender, EventArgs e)
    {
        AdminController sysmgr = new AdminController();
        int category = Convert.ToInt32(CategoryDropDown.SelectedValue);
        ProgramDropDown.DataSource = sysmgr.GetProgramByCategory(category);
        ProgramDropDown.DataBind();
    }
    protected void Goto_Metrics(object sender, EventArgs e)
    {
        double gpa;
        if (RBL_GraduatedPostSecondary.SelectedValue == "true") 
        {
            if (TB_GPA.Text != "")
            {
                if (!double.TryParse(TB_GPA.Text, out gpa))
                {
                    MessageUserControl.ShowInfo("GPA must be a decimal value");
                }
                else
                {
                    if (gpa < 0)
                    {
                        MessageUserControl.ShowInfo("GPA must be greater than 0.");
                    }
                    else if (gpa > 4)
                    {
                        MessageUserControl.ShowInfo("GPA cannot be greater than 4.0");
                    }
                    else
                    {
                        degree.CategoryID = Convert.ToInt32(DDL_ProgramCategory.SelectedValue);
                        degree.CredentialTypeID = Convert.ToInt32(DDL_CredentialType.SelectedValue);
                        degree.GPA = decimal.Parse(TB_GPA.Text);
                        Show_Metrics(sender, e);
                    }
                }
            }
            else
            {
                MessageUserControl.ShowInfo("You must enter your GPA.");
            }
        }
        else
        {
            //empty degree variable
            degree = null;
            Show_Metrics(sender, e);
        }
            
    }
    protected void Prefs_To_NAITCourse(object sender, EventArgs e)
    {
        if (RBL_NAIT_Student.SelectedValue == "0")
            //skip the nait course selection page
            Goto_Metrics(sender, e);
        else
            Goto_NAITCourse(sender, e);
    }
    protected void Goto_HSCourse(object sender, EventArgs e)
    {
        StudentPrefs.Visible = false;
        HighSchoolCourses.Visible = true;
    }

    //Populates the data for the gridview and repeater based on the program selected by the user
    //will not proceed to step four if the user selects no program
    //will skip step four completely if the user is not a NAIT student
    protected void Goto_NAITCourse(object sender, EventArgs e)
    {
        DataTable CoursesSelected;
        if (RBL_NAIT_Student.SelectedValue == "0")
        {
            //skip the nait course selection page
            Show_StudentPrefs(sender, e);
        }
        else if (RBL_NAIT_Student.SelectedValue == "1" && CategoryDropDown.SelectedValue == "0")
        {
            MessageUserControl.ShowInfo("If you are a current student you must select a program.");
        }
        else
        {
            ProgramMetrics.Visible = false;
            StudentController course = new StudentController();
            int programID, semester;
            List<NAITCourse> courses = new List<NAITCourse>();
            //run the search for for the program automatically and fill the basket based on prefill results
            if (RBL_NAIT_Student.SelectedValue == "1")
            {
                programID = Convert.ToInt32(ProgramDropDown.SelectedValue);
                semester = Convert.ToInt32(SemesterDropDown.SelectedValue);
                courses = StudentController.Prefill_Courses(programID, semester);
                CoursesSelected = (DataTable)ViewState["CoursesSelected"];
                foreach (var item in courses)
                {
                    //add items to the basket
                    //step 1 add a new datatable row
                    int id = item.CourseID;
                    double CCredits = item.CourseCredits;
                    string CCode = item.CourseCode, CName = item.CourseName;
                    DataRow dr;
                    
                    dr = CoursesSelected.NewRow();
                    dr["CourseID"] = id;
                    dr["CourseCode"] = CCode;
                    dr["CourseName"] = CName;
                    dr["CourseCredits"] = CCredits;

                    //find duplicates and add if there are none
                    DataRow findRow = CoursesSelected.Rows.Find(id);
                    if (findRow == null)
                    {
                        CoursesSelected.Rows.Add(dr);
                    }
                    //ViewState["CoursesSelected"] = CoursesSelected;
                    ViewState["CoursesSelected"] = CoursesSelected;

                    

                }
                rptCourse.DataSource = CoursesSelected;
                rptCourse.DataBind();
                //set drop down list to programid
                //filter search results based on programid
                ProgramDropDownList.DataBind();
                ProgramDropDownList.SelectedValue = programID.ToString();

                bool active = true;
                if (ActiveCheckBox.Checked)
                {
                    active = true;
                }
                else
                {
                    active = false;
                }
                CourseGridView.DataSource = course.SearchNaitCourses(null, programID, active);
                CourseGridView.DataBind();
                CourseGridView.Visible = true;
                int count = 0;
                foreach (DataRow row1 in CoursesSelected.Rows)
                {
                    count++;
                }
                TotalCourseLabel.Text = "Total courses : " + count;

                for (int i = 0; i < CourseGridView.Rows.Count; i++)
                {
                    CourseGridView.Rows[i].Font.Bold = false;
                    for (int j = 0; j < CoursesSelected.Rows.Count; j++)
                    {
                        if (CourseGridView.DataKeys[i]["CourseID"].ToString() == CoursesSelected.Rows[j]["CourseID"].ToString())
                        {
                            CourseGridView.Rows[i].BackColor = System.Drawing.Color.FromName("#D1DDF1");
                            CourseGridView.Rows[i].Font.Bold = true;
                            CourseGridView.Rows[i].ForeColor = System.Drawing.Color.FromName("#333333");
                        }
                    }
                }

            }

            Show_NaitCourses(sender, e);
        }
    }
    //Goes to metrics page and clear repeater data
    protected void Goto_Metrics_ClearData(object sender, EventArgs e)
    {
        //clear the repeater
        DataTable BackupTable = (DataTable)ViewState["BackupTable"];
        ViewState["CoursesSelected"] = BackupTable;

        CoursesSelected = null;
        CourseGridView.DataSource = null;
        CourseGridView.DataBind();
        rptCourse.DataBind();
        TotalCourseLabel.Text = "Total courses : 0";
        Show_Metrics(sender, e);
    }
    //Returns to step one, saving the previous user inputs
    //Will clear the repeater to avoid duplicate data
    protected void searchAgain_Click(object sender, EventArgs e)
    {
        //clear the repeater
        DataTable BackupTable = (DataTable)ViewState["BackupTable"];
        ViewState["CoursesSelected"] = BackupTable;

        CoursesSelected = null;
        CourseGridView.DataSource = null;
        CourseGridView.DataBind();
        rptCourse.DataBind();
        TotalCourseLabel.Text = "Total courses : 0";
        //Show first page
        Show_HSCourses(sender, e);
    }
    //Populates the select NAIT course page and sets the session view
    protected void SelectCourses(object sender, GridViewSelectEventArgs e)
    {

        GridViewRow row = CourseGridView.Rows[e.NewSelectedIndex];
        string CCode = (row.FindControl("CourseCode") as Label).Text;
        string CName = (row.FindControl("CourseName") as Label).Text;
        int id = int.Parse((row.FindControl("CourseID") as Label).Text);
        double CCredits = double.Parse((row.FindControl("CourseCredits") as Label).Text);
        DataRow dtrow;
        DataTable CoursesSelected = (DataTable)ViewState["CoursesSelected"];
        dtrow = CoursesSelected.NewRow();
        dtrow["CourseID"] = id;
        dtrow["CourseCode"] = CCode;
        dtrow["CourseName"] = CName;
        dtrow["CourseCredits"] = CCredits;

        DataRow findRow = CoursesSelected.Rows.Find(id);
        if (findRow == null)
        {
            CoursesSelected.Rows.Add(dtrow);
        }
        int count = 0;
        foreach (DataRow row1 in CoursesSelected.Rows)
        {
            count++;
        }
        ViewState["CoursesSelected"] = CoursesSelected;

        rptCourse.DataSource = CoursesSelected;
        rptCourse.DataBind();

        TotalCourseLabel.Text = "Total courses : " + count;
        for (int i = 0; i < CourseGridView.Rows.Count; i++)
        {
            CourseGridView.Rows[i].Font.Bold = false;
            for (int j = 0; j < CoursesSelected.Rows.Count; j++)
            {
                if (CourseGridView.DataKeys[i]["CourseID"].ToString() == CoursesSelected.Rows[j]["CourseID"].ToString())
                {
                    CourseGridView.Rows[i].BackColor = System.Drawing.Color.FromName("#D1DDF1");
                    CourseGridView.Rows[i].Font.Bold = true;
                    CourseGridView.Rows[i].ForeColor = System.Drawing.Color.FromName("#333333");
                }
            }
        }

    }
    protected void Next_Click(object sender, EventArgs e)
    {
        DataTable CoursesSelected = (DataTable)ViewState["CoursesSelected"];
        Session["CoursesSelected"] = CoursesSelected;
        Response.Redirect("../Student/testpage.aspx");
    }

    //Functionality for deleting items from the repeater
    protected void rptCourse_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int courserId = Convert.ToInt32(e.CommandArgument);
        DataTable CoursesSelected = (DataTable)ViewState["CoursesSelected"];
        if (e.CommandName == "Delete" && e.CommandArgument.ToString() != "")
        {
            CoursesSelected.Rows.Find(courserId).Delete();
            rptCourse.DataSource = CoursesSelected;
            rptCourse.DataBind();
        }
        StudentController course = new StudentController();
        List<NAITCourse> courses = new List<NAITCourse>();
        bool active = true;
        if(ActiveCheckBox.Checked)
        {
            active = true;
        }
        else
        {
            active = false;
        }
        CourseGridView.DataSource = course.SearchNaitCourses(SearchTextBox.Text, int.Parse(ProgramDropDownList.SelectedValue), active);
        CourseGridView.DataBind();
        int count = 0;
        foreach (DataRow row1 in CoursesSelected.Rows)
        {
            count++;
        }
        TotalCourseLabel.Text = "Total courses : " + count;
        for (int i = 0; i < CourseGridView.Rows.Count; i++)
        {
            CourseGridView.Rows[i].Font.Bold = false;
            for (int j = 0; j < CoursesSelected.Rows.Count; j++)
            {
                if (CourseGridView.DataKeys[i]["CourseID"].ToString() == CoursesSelected.Rows[j]["CourseID"].ToString())
                {
                    CourseGridView.Rows[i].BackColor = System.Drawing.Color.FromName("#D1DDF1");
                    CourseGridView.Rows[i].Font.Bold = true;
                    CourseGridView.Rows[i].ForeColor = System.Drawing.Color.FromName("#333333");
                }
            }
        }
    }
    //Will search for courses and populate the gridview based on what the user inputs into the textbox
    protected void Search_Click(object sender, EventArgs e)
    {
        StudentController course = new StudentController();
        List<NAITCourse> courses = new List<NAITCourse>();
        bool active = true;
        if (ActiveCheckBox.Checked)
        {
            active = true;
        }
        else
        {
            active = false;
        }
        try
        {
            CourseGridView.DataSource = course.SearchNaitCourses(SearchTextBox.Text, int.Parse(ProgramDropDownList.SelectedValue), active);
            CourseGridView.DataBind();
            CourseGridView.Visible = true;
            SearchTextBox.Text = null;
        }
        catch (Exception error)
        {
            MessageUserControl.ShowInfo(error.Message);
        }
        

    }
    //Clears the repeater and search options
    protected void reset_Click(object sender, EventArgs e)
    {
        DataTable BackupTable = (DataTable)ViewState["BackupTable"];
        ViewState["CoursesSelected"] = BackupTable;

        CoursesSelected = null;
        StudentController course = new StudentController();
        List<NAITCourse> courses = new List<NAITCourse>();
        bool active = true;
        if (ActiveCheckBox.Checked)
        {
            active = true;
        }
        else
        {
            active = false;
        }
        CourseGridView.DataSource = course.SearchNaitCourses(SearchTextBox.Text, int.Parse(ProgramDropDownList.SelectedValue), active);
        CourseGridView.DataBind();
        CourseGridView.Visible = true;
        SearchTextBox.Text = null;
        rptCourse.DataBind();
        TotalCourseLabel.Text = "Total courses : 0";

    }
    //Automatically searches for courses based on what the user selects in the drop down list
    protected void List_Change(object sender, EventArgs e)
    {
        StudentController sysmgr = new StudentController();
        List<NAITCourse> courses = new List<NAITCourse>();
        bool active = true;
        if (ActiveCheckBox.Checked)
        {
            active = true;
        }
        else
        {
            active = false;
        }
        CourseGridView.DataSource = sysmgr.SearchNaitCourses(SearchTextBox.Text, int.Parse(ProgramDropDownList.SelectedValue), active);
        CourseGridView.DataBind();
    }
    //Hides/shows the graduated post secondary section of the high school courses page
    protected void RBL_GraduatedPostSecondary_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RBL_GraduatedPostSecondary.SelectedValue == "true")
            graduated.Visible = true;
        else
            graduated.Visible = false;
    }
    //Adds pagination to the results page
    protected void ResultsView_PagePropertiesChanged(object sender, PagePropertiesChangingEventArgs e)
    {
        List<ProgramResult> finalProgramResults = (List<ProgramResult>)Session["finalProgramResults"];

        (ResultsView.FindControl("DataPager") as DataPager).SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

        ResultsView.DataSource = finalProgramResults;
        ResultsView.DataBind();


    }
    protected void Show_HSCourses(object sender, EventArgs e)
    {
        HighSchoolCourses.Visible = true;
        ProgramMetrics.Visible = false;
        NaitCourses.Visible = false;
        StudentPrefs.Visible = false;
        ResultsList.Visible = false;
    }
    protected void Show_Metrics(object sender, EventArgs e)
    {
        HighSchoolCourses.Visible = false;
        ProgramMetrics.Visible = true;
        NaitCourses.Visible = false;
        StudentPrefs.Visible = false;
    }
    protected void Show_NaitCourses(object sender, EventArgs e)
    {
        HighSchoolCourses.Visible = false;
        ProgramMetrics.Visible = false;
        NaitCourses.Visible = true;
        StudentPrefs.Visible = false;
    }
    protected void Show_StudentPrefs(object sender, EventArgs e)
    {
        HighSchoolCourses.Visible = false;
        ProgramMetrics.Visible = false;
        NaitCourses.Visible = false;
        StudentPrefs.Visible = true;
    }
    protected void Show_Results(object sender, EventArgs e)
    {
        HighSchoolCourses.Visible = false;
        ProgramMetrics.Visible = false;
        NaitCourses.Visible = false;
        StudentPrefs.Visible = false;
        ResultsList.Visible = true;
    }
}