﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SelectNaitCourses.aspx.cs" Inherits="User_SelectNaitCourses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <h1>Select NAIT Course</h1>
    <div>
        <div class="search-bar" >
            <label>Please select a program</label>
            <br />
            <asp:DropDownList runat="server" ID="ProgramDropDownList" 
                              DataSourceID="SelectProgramODB" 
                              DataTextField="ProgramName" 
                              DataValueField="ProgramID"
                              AppendDataBoundItems="True" >
                <asp:ListItem  Value=0 Text="[Select All]" />

            </asp:DropDownList>
        </div>
        <div class="search-bar">

            <label >Please search the NAIT course you want.</label>
            <br />
            <asp:TextBox ID="SearchTextBox" runat="server" Width="200px"></asp:TextBox><asp:Button ID="Search" runat="server" Text="Search" />
        </div>
        <div style="clear:both"></div>
    </div>
    <br />
    <div>
        <asp:GridView ID="CourseGridView" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataSourceID="NaitCourseODB"
            CssClass="CGV" DataKeyNames="CourseCode" OnSelectedIndexChanging="SelectCourses">
            <Columns>
                <asp:TemplateField HeaderText="CourseID" Visible ="false">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="CourseID" Text='<%# Eval("CourseID") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="CourseCode">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="CourseCode" Text='<%# Eval("CourseCode") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="CourseName" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="CourseName" Text='<%# Eval("CourseName") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="CourseCredits" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="CourseCredits" Text='<%# Eval("CourseCredits") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>

                <%--<asp:BoundField DataField="CourseID" HeaderText="CourseID" SortExpression="CourseID" Visible="false"/>
                <asp:BoundField DataField="CourseCode" HeaderText="CourseCode" SortExpression="CourseCode" />
                <asp:BoundField DataField="CourseName" HeaderText="CourseName" SortExpression="CourseName" />
                <asp:BoundField DataField="CourseCredits" HeaderText="CourseCredits" SortExpression="CourseCredits" />--%>
                <asp:CommandField ShowSelectButton="True" />
            </Columns>
            <EmptyDataTemplate>
                No data found.

            </EmptyDataTemplate>
        </asp:GridView>
    </div>
    <div class ="rpt_div clearfix">
        <asp:Repeater ID="rptCourse" runat="server" >
        <ItemTemplate>    
            <div class="inner-rpt-div">
                <span><b>Course code: </b><%# Eval("CourseCode") %></span>
                <span><b>Course credit: </b><%# Eval("CourseCredits") %></span>
                <span><asp:Button ID="Delete" runat="server" Text="Delete" /> </span>
            </div>      
        </ItemTemplate>
    </asp:Repeater>
    </div>
    <br />
    <br />
     
        <hr />
    <div>
        <asp:Button ID="Next" runat="server" Text="Next" OnClick="Next_Click" />
    </div>    
    <div>
        <asp:ObjectDataSource ID="SelectProgramODB" runat="server" SelectMethod="GetProgram" TypeName="CrystalBallSystem.BLL.SelectNaitCourseController" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
        <asp:ObjectDataSource ID="NaitCourseODB" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="SearchNaitCourses" TypeName="CrystalBallSystem.BLL.SelectNaitCourseController">
            <SelectParameters>
                <asp:ControlParameter ControlID="SearchTextBox" Name="SearchInfo" PropertyName="Text" Type="String" />
                <asp:ControlParameter ControlID="ProgramDropDownList" Name="programID" PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="SelectedCourseODB" runat="server" SelectMethod="SelectedNaitCourses" TypeName="CrystalBallSystem.BLL.SelectNaitCourseController" OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:ControlParameter ControlID="CourseGridView" Name="courseID" PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
</asp:Content>

