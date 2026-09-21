using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnrollmentManager.API.Models
{
    public class StudentStudyFormat
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int FormatId { get; set; }
        public StudyFormat Format { get; set; } = null!;
    }
}