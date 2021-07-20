using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SimpleSudoku.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Set field value and invoke property changed event for caller property
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="target">/param>
        /// <param name="value"></param>
        /// <param name="propertyName">Specify propertyname if it differs from caller</param>
        /// <returns>True if value has changed</returns>
        protected virtual bool SetValue<TValue>(ref TValue target, TValue value, [CallerMemberName] string propertyName = "")
        {
            return SetValue(ref target, value, (t, v) => t.Equals(v), propertyName);
        }


        /// <summary>
        /// Set field value and invoke property changed event for caller property
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="equalityComparer">Func for comparing equal values. Used for determining wether a value hast changed.</param>
        /// <param name="propertyName">Specify propertyname if it differs from caller</param>
        /// <returns>True if value has changed</returns>
        protected virtual bool SetValue<TValue>(ref TValue target, TValue value, Func<TValue, TValue, bool> equalityComparer, [CallerMemberName] string propertyName = "")
        {
            if (target == null)
            {
                if (value != null)
                {
                    target = value;
                    OnPropertyChanged(propertyName);
                    return true;
                }
                return false;
            }

            if (!equalityComparer(target, value))
            {
                target = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
    }
}
