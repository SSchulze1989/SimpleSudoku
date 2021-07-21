using SimpleSudoku.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SimpleSudoku.ViewModels
{
    public class CellNoteViewModel : ViewModelBase
    {
        private CellNote NoteModel { get; set; }

        public IReadOnlyCollection<int> Values => NoteModel.Values;

        public CellNoteViewModel()
        {
        }

        public void SetNoteModel(CellNote note)
        {
            NoteModel = note;
            OnPropertyChanged(null);
        }

        public void Add(int value)
        {
            NoteModel.Add(value);
            OnPropertyChanged(nameof(Values));
        }

        public void SetValues(IEnumerable<int> values)
        {
            NoteModel.Clear();
            foreach(var value in values)
            {
                NoteModel.Add(value);
            }
            OnPropertyChanged(nameof(values));
        }

        public void Remove(int value)
        {
            NoteModel.Remove(value);
            OnPropertyChanged(nameof(Values));
        }
    }
}
