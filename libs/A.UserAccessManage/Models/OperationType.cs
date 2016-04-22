namespace A.UserAccessManage.Model
{
    using A.DB;
    using System;
    using System.Runtime.Serialization;

    [DataTable("operation_type"), DataContract]
    public class OperationType : DbObject
    {
        public OperationType()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = "";
            this.ApplicationCode = "";
            this.CreateDatetime = DateTime.Now;
        }

        [DataMember(Name="ApplicationCode"), DataField(DataFieldName="application_code", Length=0x20)]
        public virtual string ApplicationCode
        {
            get
            {
                return base.GetValue<string>("ApplicationCode");
            }
            set
            {
                string str = base.GetValue<string>("ApplicationCode");
                base.SetValue<string>("ApplicationCode", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("ApplicationCode");
                }
            }
        }

        [DataMember(Name="CreateDatetime"), DataField(DataFieldName="create_datetime", ReadOnly=true, NumericPercision=0x17, NumericScale=3)]
        public virtual DateTime CreateDatetime
        {
            get
            {
                return base.GetValue<DateTime>("CreateDatetime");
            }
            set
            {
                DateTime time = base.GetValue<DateTime>("CreateDatetime");
                base.SetValue<DateTime>("CreateDatetime", value);
                if (time != value)
                {
                    base.NotifyPropertyChanged("CreateDatetime");
                }
            }
        }

        [DataMember(Name="CreateUserId"), DataField(DataFieldName="create_user_id", AllowDBNull=true, Length=0x24)]
        public virtual string CreateUserId
        {
            get
            {
                return base.GetValue<string>("CreateUserId");
            }
            set
            {
                string str = base.GetValue<string>("CreateUserId");
                base.SetValue<string>("CreateUserId", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("CreateUserId");
                }
            }
        }

        [DataMember(Name="Id"), DataField(DataFieldName="id", PrimaryKey=true, Length=0x24)]
        public virtual string Id
        {
            get
            {
                return base.GetValue<string>("Id");
            }
            set
            {
                string str = base.GetValue<string>("Id");
                base.SetValue<string>("Id", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Id");
                }
            }
        }

        [DataField(DataFieldName="name", Length=50), DataMember(Name="Name")]
        public virtual string Name
        {
            get
            {
                return base.GetValue<string>("Name");
            }
            set
            {
                string str = base.GetValue<string>("Name");
                base.SetValue<string>("Name", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Name");
                }
            }
        }

        [DataMember(Name="ParentId"), DataField(DataFieldName="parent_id", AllowDBNull=true, Length=0x24)]
        public virtual string ParentId
        {
            get
            {
                return base.GetValue<string>("ParentId");
            }
            set
            {
                string str = base.GetValue<string>("ParentId");
                base.SetValue<string>("ParentId", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("ParentId");
                }
            }
        }
    }
}

