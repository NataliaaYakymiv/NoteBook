using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService
{
    public enum ErrorCode
    {
        NotesItemNameAndNotesRequired,
        NotesItemIDInUse,
        RecordNotFound,
        CouldNotCreateItem,
        CouldNotUpdateItem,
        CouldNotDeleteItem
    }
}
