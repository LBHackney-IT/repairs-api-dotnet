using System;

namespace RepairsApi.V2.Boundary.Response
{
    public class NoteListItem
    {
        public string Note { get; set; }
        public DateTime Time { get; set; }
        public string User { get; set; }
    }
}
