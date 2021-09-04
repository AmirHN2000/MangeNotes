using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageNotes.Data;
using Microsoft.EntityFrameworkCore;

namespace ManageNotes.Services
{
    public class NoteServices
    {
        private ApplicationContext _applicationcontext;

        public NoteServices(ApplicationContext applicationcontext)
        {
            _applicationcontext = applicationcontext;
        }

        public async Task<bool> IsExistNoteAsync(int? userId, String title)
        {
            return await _applicationcontext
                .Notes
                .Where(x => x.User.Id == userId)
                .AnyAsync(x => x.Title == title);
        }

        public async Task AddNoteAsync(Note note)
        {
            await _applicationcontext.Notes.AddAsync(note);
        }

        public async Task<List<Note>> GetNotesByUserIdsAsync(int? id)
        {
            /*return await _applicationcontext.Notes
                .AsNoTracking()
                .Where(x => x.UserId == id)
                .ToListAsync();*/
            var list= await _applicationcontext
                .Notes
                .Join(_applicationcontext.Users,
                    x => x.UserId, y => y.Id
                    , (x, y) => x)
                .Where(u => u.UserId == id)
                .ToListAsync();
            return list;
        }

        public async Task<Note> FindAsync(int? id)
        {
            return await _applicationcontext
                .Notes
                .FindAsync(id);
        }

        public async Task<List<Note>> GetAllNotesAsync(int id)
        {
            var list=await _applicationcontext
                            .Notes
                            .AsNoTracking()
                            .Where(x => x.User.Id == id)
                            .OrderByDescending(x => x.CreatedDate)
                            .ToListAsync();
            return list;
        }
    }
}