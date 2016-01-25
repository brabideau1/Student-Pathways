﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace CrystalBallSystem.DAL.Entities
{
    [Table("SubjectRequirement")]
    public class SubjectRequirement
    {
        [Key]
        public int SubjectRequirementID { get; set; }
        [Required]
        public int ProgramID { get; set; }
        [Required]
        [StringLength(30)]
        public string SubjectDescription { get; set; }

        public virtual ICollection<EntranceRequirement> EntranceRequirements { get; set; }

        public virtual Program Program { get; set; }
    }
}
