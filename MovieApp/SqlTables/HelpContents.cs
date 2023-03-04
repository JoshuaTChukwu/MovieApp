using System.ComponentModel.DataAnnotations;

namespace GOSBackend.SqlTables
{
    public class HelpContents
    {
        public HelpContents()
        {
            HelpContentTasks = new HashSet<HelpContentTasks>();
        }
        [Key]
        public int HelpContentsId { get; set; }
        public int Application { get; set; }
        public string Module { get; set; } = string.Empty;
        public int Type { get; set; }
        public string ProcessTitle { get; set; } = string.Empty;
        public string ProcessDescription { get; set; } = string.Empty;
        public virtual ICollection<HelpContentTasks> HelpContentTasks { get; set; }

    }
}
