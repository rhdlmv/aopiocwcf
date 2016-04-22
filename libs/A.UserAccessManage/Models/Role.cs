namespace A.UserAccessManage.Model
{
    using A.DB;
    using System;
    using System.Runtime.Serialization;

    [DataContract, DataTable("role")]
    public class Role : DbObject
    {
        public Role()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = "";
            this.ApplicationCode = "";
            this.AuthorityLevel = 0;
            this.Invalid = false;
            this.SystemDefault = false;
            this.CreateDatetime = DateTime.Now;
        }

        [DataField(DataFieldName="application_code", Length=0x20), DataMember(Name="ApplicationCode")]
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

        [DataField(DataFieldName="authority_level", NumericPercision=10), DataMember(Name="AuthorityLevel")]
        public virtual int AuthorityLevel
        {
            get
            {
                return base.GetValue<int>("AuthorityLevel");
            }
            set
            {
                int num = base.GetValue<int>("AuthorityLevel");
                base.SetValue<int>("AuthorityLevel", value);
                if (num != value)
                {
                    base.NotifyPropertyChanged("AuthorityLevel");
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

        [DataField(DataFieldName="invalid"), DataMember(Name="Invalid")]
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

        [DataMember(Name="Name"), DataField(DataFieldName="name", Length=50)]
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

        [DataMember(Name="Serial"), DataField(DataFieldName="serial", AllowDBNull=true, Length=50)]
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

        [DataField(DataFieldName="system_default"), DataMember(Name="SystemDefault")]
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

