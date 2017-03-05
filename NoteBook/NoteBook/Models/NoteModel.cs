using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;

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
        public string Image { get; set; }

        [Ignore]
        [JsonIgnore]
        public MediaFile MediaFile { get; set; }

        [JsonIgnore]
        public byte[] ImageInBytes { get; set; }
    }

    public class SyncModel
    {
        public string LastModify { set; get; }
        public List<NoteModel> NoteModels { set; get; }
    }
}
