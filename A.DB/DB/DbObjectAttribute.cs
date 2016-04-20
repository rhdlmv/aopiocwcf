namespace EgoalTech.DB
{
    using System;

    public class DbObjectAttribute : Attribute
    {
        private string _dataFieldName = "";
        private string _fieldName = "";
        private bool _primaryKey = false;
        private bool _readOnly = false;

        public string DataFieldName
        {
            get
            {
                return this._dataFieldName;
            }
            set
            {
                this._dataFieldName = value;
            }
        }

        public string FieldName
        {
            get
            {
                return this._fieldName;
            }
            set
            {
                this._fieldName = value;
            }
        }

        public bool PrimaryKey
        {
            get
            {
                return this._primaryKey;
            }
            set
            {
                this._primaryKey = value;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return this._readOnly;
            }
            set
            {
                this._readOnly = value;
            }
        }
    }
}

