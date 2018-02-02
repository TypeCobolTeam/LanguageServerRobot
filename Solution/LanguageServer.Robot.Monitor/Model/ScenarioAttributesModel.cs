﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Model;

namespace LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// The Scenario Data Model.
    /// </summary>
    public class ScenarioAttributesModel : DataModel<Script>
    {
        /// <summary>
        /// Data Constructor
        /// </summary>
        /// <param name="data">The source data</param>
        public ScenarioAttributesModel(Script data) : base(data)
        {
            System.Diagnostics.Debug.Assert(data != null);
            this.DataOrigin = (Script)data.Clone();
        }

        public override string Category
        {
            get { return Properties.Resources.ScenarioCategory; }
        }

        public override bool IsModified
        {
            get
            {
                if (DataOrigin == null)
                    return false;
                return DataOrigin.uri != Data.uri || DataOrigin.name != Data.name ||
                       DataOrigin.description != Data.description;
            }
        }

        /// <summary>
        /// The File path
        /// </summary>
        public string FilePath
        {
            get
            {
                return new Uri(Data.uri).LocalPath;
            }
        }

        /// <summary>
        /// Scenario's name.
        /// </summary>
        public override String Name
        {
            get
            {
                return this.Data.name ?? "";
            }
            set
            {
                if (value != this.Data.name)
                {
                    this.Data.name = value;
                    OnPropertyChanged(NAME_PROPERTY);
                }
            }
        }

        /// <summary>
        /// The Description of this Data
        /// </summary>
        public override String Description
        {
            get
            {
                return this.Data.description ?? "";
            }
            set
            {
                this.Data.description = value;
            }
        }

        public override bool Apply()
        {
            //throw new NotImplementedException();
            return true;
        }

        protected override void Insert()
        {
            //throw new NotImplementedException();
        }

        protected override void Update()
        {
            //throw new NotImplementedException();
        }

        protected override void Delete()
        {
            //throw new NotImplementedException();
        }

        public override void Cancel()
        {
            //throw new NotImplementedException();
        }
    }
}
