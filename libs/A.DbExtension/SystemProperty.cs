﻿using A.DB;
using System;

namespace A.DBExtension
{
    [DataTable("system_property")]
    internal class SystemProperty : DbObject
    {
        private string id;
        private string name;
        private string value;

        public SystemProperty()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = "";
            this.Value = "";
        }

        [DataField(DataFieldName = "id", PrimaryKey = true, Length = 0x24)]
        public virtual string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    base.NotifyPropertyChanged("Id");
                }
            }
        }

        [DataField(DataFieldName = "name", Length = 0x80)]
        public virtual string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    base.NotifyPropertyChanged("Name");
                }
            }
        }

        [DataField(DataFieldName = "value")]
        public virtual string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    base.NotifyPropertyChanged("Value");
                }
            }
        }
    }
}

