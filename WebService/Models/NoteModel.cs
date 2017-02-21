using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WebService.Models
{
    public class NoteModel
    {
        [Key]
        [Required]
        public int NoteId { get; set; }

        // public string CreatorId { get; set; }
        [Required]
        [Display(Name = "Note name")]
        public string NoteName { get; set; }

        [Required]
        [Display(Name = "Note text")]
        public string NoteText { get; set; }

        // public DateTime Date { get; set; }

        //public bool Done { get; set; }

    }
    public class NotesContext : DbContext
    {
        public NotesContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<NoteModel> NoteModels { get; set; }
    }
}