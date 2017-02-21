using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;

namespace WebService.Models
{
    public class NoteModel
    {
        [Key]
        [Required]
        public int NoteId { get; set; }

        [Required]
        [Display(Name = "Note name")]
        public string NoteName { get; set; }

        [Required]
        [Display(Name = "Note text")]
        public string NoteText { get; set; }

        public int? UserId { get; set; }
        public AccountModels.UserProfile User { get; set; }

        // public DateTime Date { get; set; }

        //public bool Done { get; set; }

    }
    public class NotesContext : DbContext
    {
        public NotesContext()
            : base("DefaultConnection")
        {
            //Database.SetInitializer<NotesContext>(new CreateDatabaseIfNotExists<NotesContext>());
        }

        public DbSet<NoteModel> NoteModels { get; set; }
    }
}