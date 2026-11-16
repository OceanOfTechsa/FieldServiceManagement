using System.Security.Cryptography;
using System.Text;
using FieldServiceManagement.Data.DataModels.Note;

namespace FieldServiceManagement.Business.Imports.Readers
{
    // The Notes file of any module: reads and checks a row, and makes sure the same note is not added twice to the same record.
    public class ImportNotes
    {
        private const int MaxTitleLength = 200;

        private readonly int _entityTypeId;
        private readonly string _parentLabel;
        private readonly HashSet<string> _keys = new();

        // existing:      the notes the records of this type already have
        // entityTypeId:  the type of record the notes attach to (Contact = 6, Company = 2, ...)
        // parentLabel:   what the record is called in messages ("contact", "company")
        public ImportNotes(IEnumerable<Note> existing, int entityTypeId, string parentLabel)
        {
            _entityTypeId = entityTypeId;
            _parentLabel = parentLabel;

            foreach (var n in existing)
                _keys.Add(Key(n.EntityId, n.Title, n.Content));
        }

        public string DuplicateMessage => $"This {_parentLabel} already has a note with the same title and content.";

        // The row's own fields. Parent and Content are mandatory (after "Replace Empty Value"), and the title column is 200 characters.
        public (string? Error, string ParentValue, string Title, string Content) Read(ImportRow r)
        {
            string parent = r.Get("parent"), title = r.Get("title"), content = r.Get("content");

            var blank = new List<string>();
            if (parent == "") blank.Add("Parent");
            if (content == "") blank.Add("Content");
            if (blank.Count > 0)
                return ($"Mandatory field(s) empty: {string.Join(", ", blank)}.", parent, title, content);

            if (title.Length > MaxTitleLength)
                return ($"Title must be {MaxTitleLength} characters or fewer.", parent, title, content);

            return (null, parent, title, content);
        }

        // The new note, or null when this record already has the same note.
        // The note is remembered, so a repeat later in the same file is caught too. The caller adds it to the batch.
        public Note? TryCreate(Guid entityId, string title, string content, Guid organisationId, Guid userId, DateTime now)
        {
            if (!_keys.Add(Key(entityId, title, content)))
                return null;

            return new Note
            {
                Id = Guid.NewGuid(),
                OrganisationId = organisationId,
                EntityTypeId = _entityTypeId,
                EntityId = entityId,
                Title = title == "" ? null : title,
                Content = content,
                IsSharedToClient = false,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = now,
                CreatedBy = userId
            };
        }

        // Call when a batch could not be saved, so an identical later row is not skipped because of notes that never made it in
        public void Forget(IEnumerable<Note> notes)
        {
            foreach (var n in notes)
                _keys.Remove(Key(n.EntityId, n.Title, n.Content));
        }

        // The same note on the same record (ignoring case and extra spaces) has the same key
        private static string Key(Guid entityId, string? title, string? content)
        {
            var text = ImportAddressBook.Norm(title) + "\n" + ImportAddressBook.Norm(content);
            return entityId + "|" + Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
        }
    }
}