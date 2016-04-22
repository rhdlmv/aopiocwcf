namespace A.UserAccessManage.Model
{
    using A.DB;
    using System;
    using System.Runtime.Serialization;

    [DataContract, DataTable("user_role"), QueryTable("user_role_query")]
    public class UserRole : DbObject
    {
        public UserRole()
        {
            this.Id = Guid.NewGuid().ToString();
            this.UserId = "";
            this.RoleId = "";
            this.Sequence = 0;
            this.CreateDatetime = DateTime.Now;
            this.Username = "";
            this.RoleName = "";
            this.ApplicationCode = "";
            this.Status = "";
            this.FirstLoginTime = null;
            this.LastLoginTime = null;
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

        [DataField(DataFieldName="create_user_id", AllowDBNull=true, Length=0x24), DataMember(Name="CreateUserId")]
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

        [DataMember(Name="FirstLoginTime"), DataField(DataFieldName="first_login_time", AllowDBNull=true, ReadOnly=true, NumericPercision=0x17, NumericScale=3)]
        public virtual DateTime? FirstLoginTime
        {
            get
            {
                return base.GetValue<DateTime?>("FirstLoginTime");
            }
            set
            {
                DateTime? nullable = base.GetValue<DateTime?>("FirstLoginTime");
                base.SetValue<DateTime?>("FirstLoginTime", value);
                if (nullable != value)
                {
                    base.NotifyPropertyChanged("FirstLoginTime");
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

        [DataField(DataFieldName="last_login_time", AllowDBNull=true, ReadOnly=true, NumericPercision=0x17, NumericScale=3), DataMember(Name="LastLoginTime")]
        public virtual DateTime? LastLoginTime
        {
            get
            {
                return base.GetValue<DateTime?>("LastLoginTime");
            }
            set
            {
                DateTime? nullable = base.GetValue<DateTime?>("LastLoginTime");
                base.SetValue<DateTime?>("LastLoginTime", value);
                if (nullable != value)
                {
                    base.NotifyPropertyChanged("LastLoginTime");
                }
            }
        }

        [DataMember(Name="OrganizationName"), DataField(DataFieldName="organization_name", AllowDBNull=true, ReadOnly=true, Length=50)]
        public virtual string OrganizationName
        {
            get
            {
                return base.GetValue<string>("OrganizationName");
            }
            set
            {
                string str = base.GetValue<string>("OrganizationName");
                base.SetValue<string>("OrganizationName", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("OrganizationName");
                }
            }
        }

        [DataMember(Name="RealName"), DataField(DataFieldName="real_name", AllowDBNull=true, ReadOnly=true, Length=30)]
        public virtual string RealName
        {
            get
            {
                return base.GetValue<string>("RealName");
            }
            set
            {
                string str = base.GetValue<string>("RealName");
                base.SetValue<string>("RealName", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("RealName");
                }
            }
        }

        [DataField(DataFieldName="role_id", Length=0x24), DataMember(Name="RoleId")]
        public virtual string RoleId
        {
            get
            {
                return base.GetValue<string>("RoleId");
            }
            set
            {
                string str = base.GetValue<string>("RoleId");
                base.SetValue<string>("RoleId", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("RoleId");
                }
            }
        }

        [DataField(DataFieldName="role_name", ReadOnly=true, Length=50), DataMember(Name="RoleName")]
        public virtual string RoleName
        {
            get
            {
                return base.GetValue<string>("RoleName");
            }
            set
            {
                string str = base.GetValue<string>("RoleName");
                base.SetValue<string>("RoleName", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("RoleName");
                }
            }
        }

        [DataMember(Name="Sequence"), DataField(DataFieldName="sequence", NumericPercision=10)]
        public virtual int Sequence
        {
            get
            {
                return base.GetValue<int>("Sequence");
            }
            set
            {
                int num = base.GetValue<int>("Sequence");
                base.SetValue<int>("Sequence", value);
                if (num != value)
                {
                    base.NotifyPropertyChanged("Sequence");
                }
            }
        }

        [DataField(DataFieldName="status", ReadOnly=true, Length=0x20), DataMember(Name="Status")]
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

        [DataField(DataFieldName="user_id", Length=0x24), DataMember(Name="UserId")]
        public virtual string UserId
        {
            get
            {
                return base.GetValue<string>("UserId");
            }
            set
            {
                string str = base.GetValue<string>("UserId");
                base.SetValue<string>("UserId", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("UserId");
                }
            }
        }

        [DataField(DataFieldName="username", ReadOnly=true, Length=50), DataMember(Name="Username")]
        public virtual string Username
        {
            get
            {
                return base.GetValue<string>("Username");
            }
            set
            {
                string str = base.GetValue<string>("Username");
                base.SetValue<string>("Username", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Username");
                }
            }
        }
    }
}

