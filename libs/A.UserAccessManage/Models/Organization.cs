namespace A.UserAccessManage.Model
{
    using A.DB;
    using System;
    using System.Runtime.Serialization;

    [DataContract, QueryTable("organization_query"), DataTable("organization")]
    public class Organization : DbObject
    {
        public Organization()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Domain = "";
            this.SystemDefault = false;
            this.Status = "";
            this.Invalid = false;
            this.CreateDatetime = DateTime.Now;
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

        [DataField(DataFieldName="domain", Length=50), DataMember(Name="Domain")]
        public virtual string Domain
        {
            get
            {
                return base.GetValue<string>("Domain");
            }
            set
            {
                string str = base.GetValue<string>("Domain");
                base.SetValue<string>("Domain", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Domain");
                }
            }
        }

        [DataField(DataFieldName="extension", AllowDBNull=true, Length=0x800), DataMember(Name="Extension")]
        public virtual string Extension
        {
            get
            {
                return base.GetValue<string>("Extension");
            }
            set
            {
                string str = base.GetValue<string>("Extension");
                base.SetValue<string>("Extension", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Extension");
                }
            }
        }

        [DataField(DataFieldName="id", PrimaryKey=true, Length=0x24), DataMember(Name="Id")]
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

        [DataMember(Name="Invalid"), DataField(DataFieldName="invalid")]
        public virtual bool Invalid
        {
            get
            {
                return base.GetValue<bool>("Invalid");
            }
            set
            {
                bool flag = base.GetValue<bool>("Invalid");
                base.SetValue<bool>("Invalid", value);
                if (flag != value)
                {
                    base.NotifyPropertyChanged("Invalid");
                }
            }
        }

        [DataMember(Name="Name"), DataField(DataFieldName="name", AllowDBNull=true, Length=50)]
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

        [DataMember(Name="ParentOrganizationName"), DataField(DataFieldName="parent_organization_name", AllowDBNull=true, ReadOnly=true, Length=50)]
        public virtual string ParentOrganizationName
        {
            get
            {
                return base.GetValue<string>("ParentOrganizationName");
            }
            set
            {
                string str = base.GetValue<string>("ParentOrganizationName");
                base.SetValue<string>("ParentOrganizationName", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("ParentOrganizationName");
                }
            }
        }

        [DataMember(Name="RootId"), DataField(DataFieldName="root_id", AllowDBNull=true, Length=0x24)]
        public virtual string RootId
        {
            get
            {
                return base.GetValue<string>("RootId");
            }
            set
            {
                string str = base.GetValue<string>("RootId");
                base.SetValue<string>("RootId", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("RootId");
                }
            }
        }

        [DataField(DataFieldName="root_organization_name", AllowDBNull=true, ReadOnly=true, Length=50), DataMember(Name="RootOrganizationName")]
        public virtual string RootOrganizationName
        {
            get
            {
                return base.GetValue<string>("RootOrganizationName");
            }
            set
            {
                string str = base.GetValue<string>("RootOrganizationName");
                base.SetValue<string>("RootOrganizationName", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("RootOrganizationName");
                }
            }
        }

        [DataField(DataFieldName="serial", AllowDBNull=true, Length=50), DataMember(Name="Serial")]
        public virtual string Serial
        {
            get
            {
                return base.GetValue<string>("Serial");
            }
            set
            {
                string str = base.GetValue<string>("Serial");
                base.SetValue<string>("Serial", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Serial");
                }
            }
        }

        [DataMember(Name="Status"), DataField(DataFieldName="status", Length=0x20)]
        public virtual string Status
        {
            get
            {
                return base.GetValue<string>("Status");
            }
            set
            {
                string str = base.GetValue<string>("Status");
                base.SetValue<string>("Status", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Status");
                }
            }
        }

        [DataMember(Name="SystemDefault"), DataField(DataFieldName="system_default")]
        public virtual bool SystemDefault
        {
            get
            {
                return base.GetValue<bool>("SystemDefault");
            }
            set
            {
                bool flag = base.GetValue<bool>("SystemDefault");
                base.SetValue<bool>("SystemDefault", value);
                if (flag != value)
                {
                    base.NotifyPropertyChanged("SystemDefault");
                }
            }
        }
    }
}

