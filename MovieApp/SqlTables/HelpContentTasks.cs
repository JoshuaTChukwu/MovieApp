using System.ComponentModel.DataAnnotations;

namespace GOSBackend.SqlTables
{
    public class HelpContentTasks
    {
        public HelpContentTasks()
        {
            HelpContents = new HelpContents();
        } 
        [Key]
       public int TaskId { get; set; }
        public int HelpContentsId { get; set; }
        public virtual HelpContents HelpContents { get; set; }
        public string Description { get; set; }= string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public bool HasImage { get; set; }
    }
}
