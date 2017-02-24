using System;
using System.Collections.Generic;
using SQLite;

namespace NoteBook.Models
{
    [Table("Notes")]
    public class NoteModel
    {
        [PrimaryKey]
        public string NoteId { get; set; }
        public string NoteName { get; set; }
        public string NoteText { get; set; }

        public string Create { get; set; }
        public string Update { get; set; }
        public string Delete { get; set; }
    }

    public class SyncModel
    {
        public DateTime LastModify { set; get; }
        public List<NoteModel> NoteModels { set; get; }
    }
}
