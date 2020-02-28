using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LMS.Data.EF
{
    #region Course Metadata

    [MetadataType(typeof(CourseMetadata))]
    public partial class Course { }
    public class CourseMetadata
    {
        [Key]
        public int CourseID { get; set; }

        [Required(ErrorMessage = "Required***")]
        [StringLength(200, ErrorMessage = "Course Name must be less than 200 characters***")]
       public string CourseName { get; set; }

        [StringLength(500, ErrorMessage = "Description must be less than 500 characters***")]
        [DisplayFormat(NullDisplayText = "--N/A--")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Required***")]
        public bool IsActive { get; set; }

    }
    #endregion

    #region Course Completion Metadata

    [MetadataType(typeof(CourseCompletionMetadata))]
    public partial class CourseCompletion { }
    public class CourseCompletionMetadata
    {
        [Key]
        public int CourseCompletionID { get; set; }

        [Required(ErrorMessage = "Required***")]
        //[Range(128,128, ErrorMessage = "Please enter a valid User ID***")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Required***")]
        public int CourseID { get; set; }

        [Required(ErrorMessage = "Required***")]
        [DataType(DataType.Date)]
       // [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime DateCompleted { get; set; }

    }

    #endregion

    #region Employee Metadata
    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee
    {
        public string FullName { get { return FirstName + " " + LastName;  } }
    }
    public class EmployeeMetadata
    {
        [Key]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Required***")]
        [StringLength(30,ErrorMessage = "First Name must be less than 30 characters***")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Required***")]
        [StringLength(50, ErrorMessage = "Last Name must be less than 50 characters***")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Required***")]
        public bool IsManager { get; set; }

        [Required(ErrorMessage = "Required***")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email address***")]
        [StringLength(50, ErrorMessage = "Email must be less than 50 characters***")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required***")]
        public int JobID { get; set; }

        public int ReportsToID { get; set; }


    }

    #endregion

    #region Manager Metadata
    [MetadataType(typeof(ManagerMetadata))]
    public partial class Manager { }
    public class ManagerMetadata
    {

        
        public string UserID { get; set; }

        [Required(ErrorMessage = "Required***")]
        [StringLength(30, ErrorMessage = "First Name must be less than 30 characters***")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Required***")]
        [StringLength(50, ErrorMessage = "Last Name must be less than 50 characters***")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Required***")]
        public int JobID { get; set; }

        [Key]
        public int ManagerID { get; set; }

        [Required(ErrorMessage = "Required***")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email address***")]
        [StringLength(50, ErrorMessage = "Email must be less than 50 characters***")]
        public string Email { get; set; }


    }
    #endregion

    #region Job Metadata
    [MetadataType(typeof(JobMetadata))]
    public partial class Job { }
    public class JobMetadata
    {
        [Key]
        public int JobID { get; set; }

        [Required(ErrorMessage = "Required***")]
        [StringLength(150, ErrorMessage = "Job Name must be less than 150 characters***")]
        public string JobName { get; set; }

    }

    #endregion
    #region Lesson Metadata
    [MetadataType(typeof(LessonMetadata))]
    public partial class Lesson { }
    public class LessonMetadata
    {
        [Key]
        public int LessonID { get; set; }

        [Required(ErrorMessage = "Required***")]
        [StringLength(200, ErrorMessage = "Lesson Title must be less than 200 characters***")]
        public string LessonTitle { get; set; }

        [Required(ErrorMessage = "Required***")]
        public int CourseID { get; set; }

        [DisplayFormat(NullDisplayText = "--N/A--")]
        [StringLength(300, ErrorMessage = "Introduction must be less than 300 characters***")]
        [UIHint("MultilineText")]
        public string Introduction { get; set; }


        [DisplayFormat(NullDisplayText = "--N/A--")]
        [StringLength(250, ErrorMessage = "The video URL must be less than 250 characters***")]
        public string VideoUrl { get; set; }

        [DisplayFormat(NullDisplayText = "--N/A--")]
        [StringLength(100, ErrorMessage = "Filename must be less than 100 characters***")]
        public string PdfFileName { get; set; }

        [Required(ErrorMessage = "Required***")]
        public bool IsActive { get; set; }

    }

    #endregion

    #region Lesson View Metadata
    [MetadataType(typeof(LessionViewMetadata))]
    public partial class LessionView { }
    public class LessionViewMetadata
    {
        [Key]
        public int LessonViewID { get; set; }

        [Required(ErrorMessage = "Required***")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Required***")]
        public int LessonID { get; set; }

        [Required(ErrorMessage = "Required***")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime DateViewed { get; set; }

    }

    #endregion


}//end namespace LMS.Data.EF
