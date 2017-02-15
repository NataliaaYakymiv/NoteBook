using System;

namespace WebService.Models
{
    public class NotesItem
    {

        public string Id { get; set; }

        public string CreatorId { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        //public bool Done { get; set; }

    }
}