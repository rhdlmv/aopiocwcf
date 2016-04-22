namespace A.UserAccessManage.Model
{
    using A.DB;
    using System;
    using System.Runtime.Serialization;

    [DataContract, QueryTable("operation_query"), DataTable("operation")]
    public class Operation : DbObject
    {
        public Operation()
        {
            this.Id = Guid.NewGuid().ToString();
            this.OperationTypeId = "";
            this.Code = "";
            this.Name = "";
            this.CreateDatetime = DateTime.Now;
            this.OperationTypeName = "";
            this.ApplicationCode = "";
        }

        [DataMember(Name="ApplicationCode"), DataField(DataFieldName="application_code", ReadOnly=true, Length=0x20)]
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

        [DataMember(Name="Code"), DataField(DataFieldName="code", Length=50)]
        public virtual string Code
        {
            get
            {
                return base.GetValue<string>("Code");
            }
            set
            {
                string str = base.GetValue<string>("Code");
                base.SetValue<string>("Code", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Code");
                }
            }
        }

        [DataField(DataFieldName="create_datetime", ReadOnly=true, NumericPercision=0x17, NumericScale=3), DataMember(Name="CreateDatetime")]
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

        [DataField(DataFieldName="operation_type_id", Length=0x24), DataMember(Name="OperationTypeId")]
        public virtual string OperationTypeId
        {
            get
            {
                return base.GetValue<string>("OperationTypeId");
            }
            set
            {
                string str = base.GetValue<string>("OperationTypeId");
                base.SetValue<string>("OperationTypeId", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("OperationTypeId");
                }
            }
        }

        [DataField(DataFieldName="operation_type_name", ReadOnly=true, Length=50), DataMember(Name="OperationTypeName")]
        public virtual string OperationTypeName
        {
            get
            {
                return base.GetValue<string>("OperationTypeName");
            }
            set
            {
                string str = base.GetValue<string>("OperationTypeName");
                base.SetValue<string>("OperationTypeName", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("OperationTypeName");
                }
            }
        }
    }
}

