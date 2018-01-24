using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// Abstract base class of a Model of Data
    /// </summary>
    /// <typeparam name="D">Generic Data Type</typeparam>
    public abstract class DataModel<D> : CheckedListItem, INotifyPropertyChanged where D : class
    {
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public DataModel() : this((D)null)
        {
        }

        /// <summary>
        /// Data Constructor
        /// </summary>
        /// <param name="data">The source data</param>
        public DataModel(D data)
        {
            Data = data;
        }

        /// <summary>
        /// The Category of this Data if any
        /// </summary>
        public abstract String Category
        {
            get;
        }

        /// <summary>
        /// The underlying data.
        /// </summary>
        public D Data
        {
            get;
            set;
        }

        /// <summary>
        /// The underlying original data Data if any.
        /// The original data is the one from which the edited Data was cloned.
        /// </summary>
        public D DataOrigin
        {
            get;
            protected set;
        }

        /// <summary>
        /// Is this a new Data's model ?
        /// </summary>
        public bool IsNew
        {
            get;
            set;
        }

        public const String DESCRIPTION_PROPERTY = "Description";
        /// <summary>
        /// The Description of this Data
        /// </summary>
        public virtual String Description
        {
            get;
            set;
        }

        public const String IS_MODIFIED_PROPERTY = "IsModified";
        /// <summary>
        /// Determine if the data is modified or not.
        /// </summary>
        public abstract bool IsModified
        {
            get;
        }

        bool m_NeedToBeSaved;
        public const String NEEDTOBESAVE_PROPERTY = "NeedToBeSave";
        /// <summary>
        /// Is this property need to be saved ?
        /// </summary>
        public bool NeedToBeSaved
        {
            get
            {
                return m_NeedToBeSaved;
            }
            protected set
            {
                if (value != m_NeedToBeSaved)
                {
                    m_NeedToBeSaved = value;
                    OnPropertyChanged(NEEDTOBESAVE_PROPERTY);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int ValidationFlags
        {
            get;
            set;
        }

        /// <summary>
        /// Is This Model Valid ?
        /// </summary>
        /// <param name="Reason">Any Reason why this Model is not valid</param>
        /// <returns>true if OK, false otherwise</returns>
        public virtual bool IsValid(ref String Reason)
        {
            //The Name cannot be Empty
            if (Name.Trim().Length == 0)
            {
                Reason = Properties.Resources.EmptyDataName;
                return false;
            }
            return true;
        }

        public const String IsDataValidProperty = "IsDataValid";
        bool m_IsDataValid;
        /// <summary>
        /// Property to check if an Data is Valid by Itself, for instance to check
        /// its validadity in the underlying database.
        /// </summary>
        public virtual bool IsDataValid
        {
            get
            {
                return m_IsDataValid;
            }
            set
            {
                if (m_IsDataValid != value)
                {
                    m_IsDataValid = value;
                    OnPropertyChanged(IsDataValidProperty);
                }
            }
        }
        /// <summary>
        /// Apply modifications, but not save in the database.
        /// </summary>
        /// <returns>true if OK, false otherwise</returns>
        public abstract bool Apply();

        /// <summary>
        /// Save modifications into the database.
        /// <summary>
        /// <param name="bApply">true if Apply must be called, false otherwise</param>
        /// <exception cref="">Any database exception if an error occurs.</exception>
        /// <returns>true if apply returns true, false otherwise</returns>
        public virtual bool Save(bool bApply)
        {
            bool bResult = true;
            if (bApply)
                bResult = Apply();
            if (bResult)
            {
                if (IsNew)
                {//Save
                    Insert();
                    IsNew = false;
                }
                else
                {//update
                    Update();
                }
                NeedToBeSaved = false;
                IsBold = IsItalic = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Insert the current data
        /// </summary>
        protected abstract void Insert();

        /// <summary>
        /// Update the current data
        /// </summary>
        protected abstract void Update();

        /// <summary>
        /// Delete the current data
        /// </summary>
        protected abstract void Delete();

        /// <summary>
        /// Cancel modifications
        /// </summary>
        public abstract void Cancel();

    }
}
