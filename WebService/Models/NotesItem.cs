using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebService.Models
{
    public class NoteModel
    {
        [Required]
        [Display(Name = "Note id")]
        public string NoteId { get; set; }

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
}