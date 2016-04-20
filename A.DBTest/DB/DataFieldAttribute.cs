namespace EgoalTech.DB
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property)]
    public class DataFieldAttribute : Attribute
    {
        private bool _allowDBNull = false;
        private string _dataFieldName = "";
        private object _defaultValue = null;
        private Type _fieldType;
        private int _length = -1;
        private int _numericPercision = -1;
        private int _numericScale = -1;
        private string _objectFieldName = "";
        private bool _primaryKey = false;
        private bool _readOnly = false;

        public bool AllowDBNull
        {
            get => 
                this._allowDBNull
            set
            {
                this._allowDBNull = value;
            }
        }

        public bool AutoIncrement { get; set; }

        public string DataFieldName
        {
            get => 
                this._dataFieldName
            set
            {
                this._dataFieldName = value;
            }
        }

        public object DefaultValue
        {
            get => 
                this._defaultValue
            set
            {
                this._defaultValue = value;
            }
        }

        public Type FieldType
        {
            get => 
                this._fieldType
            set
            {
                this._fieldType = value;
            }
        }

        public string JoinDatabaseName { get; set; }

        public string JoinOnFieldName { get; set; }

        public string JoinTableName { get; set; }

        public int Length
        {
            get => 
                this._length
            set
            {
                this._length = value;
            }
        }

        public int NumericPercision
        {
            get => 
                this._numericPercision
            set
            {
                this._numericPercision = value;
            }
        }

        public int NumericScale
        {
            get => 
                this._numericScale
            set
            {
                this._numericScale = value;
            }
        }

        public string ObjectFieldName
        {
            get => 
                this._objectFieldName
            set
            {
                this._objectFieldName = value;
            }
        }

        public bool OuterJoin { get; set; }

        public bool PrimaryKey
        {
            get => 
                this._primaryKey
            set
            {
                this._primaryKey = value;
            }
        }

        public bool ReadOnly
        {
            get => 
                this._readOnly
            set
            {
                this._readOnly = value;
            }
        }

        public bool RightJoin { get; set; }
    }
}

