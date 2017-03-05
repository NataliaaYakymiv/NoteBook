using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Diagnostics;

namespace WebService.Models
{
    public class NoteModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string NoteId { get; set; }

        [Required]
        [Display(Name = "Note name")]
        public string NoteName { get; set; }

        [Required]
        [Display(Name = "Note text")]
        public string NoteText { get; set; }

        

        public int UserId { get; set; }
        public AccountModels.UserProfile User { get; set; }

        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
        public DateTime? Delete { get; set; }
        public string Image { get; set; }
    }

    public class SyncModel
    {
        public string LastModify { set; get; }
        public List<NoteModel> NoteModels { set; get; }
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