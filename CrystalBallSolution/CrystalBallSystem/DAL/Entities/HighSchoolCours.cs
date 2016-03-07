using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace CrystalBallSystem.DAL.Entities
{
   
    [Table("HighSchoolCourses")]
    public partial class HighSchoolCours
    {
     
        public HighSchoolCours()
        {
            EntranceRequirements = new HashSet<EntranceRequirement>();
        }

        [Key]
        public int HighSchoolCourseID { get; set; }

        [Required]
        [StringLength(30)]
        public string HighSchoolCourseName { get; set; }
        public string CourseGroup { get; set; }
        public bool Highest { get; set; }

        public virtual ICollection<EntranceRequirement> EntranceRequirements { get; set; }
    }
}
