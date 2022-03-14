// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSE681.Support
{
    /// <summary>
    /// This class is used to make the child class observable. This allows for properties to be
    /// updated and notify all other classes that are interested in changes that the property in
    /// question has changed.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>This method is used to notify observers that the property has changed.</summary>
        /// <param name="propertyName">
        /// A property name may be provided, but if left off from the call, the method will use the
        /// method that it was called from as the property name.
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// This method will set the property value is there is a change in the value. Otherwise it
        /// will not update the value or notify observers of the change.
        /// </summary>
        /// <typeparam name="T">
        /// This type parameter can be set to the type that this method will use.
        /// </typeparam>
        /// <param name="field">
        /// This is a reference to the field that needs to have its value updated to the newValue
        /// </param>
        /// <param name="newValue">This is the value that should be used to update the field value.</param>
        /// <param name="propertyName">
        /// A property name may be provided, but if left off from the call, the method will use the
        /// method that it was called from as the property name.
        /// </param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
    }
}