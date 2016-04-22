namespace A.UserAccessManage.Model
{
    using A.DB;
    using System;
    using System.Runtime.Serialization;

    [DataTable("user"), QueryTable("user_query"), DataContract]
    public class User : DbObject
    {
        public User()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Username = "";
            this.PasswordNeverExpire = false;
            DateTime? nullable = null;
            this.PasswordExpireDate = nullable;
            this.PasswordResetOnLogin = false;
            this.LoginFailCount = 0;
            this.SystemDefault = false;
            this.Invalid = false;
            this.CreateDatetime = DateTime.Now;
            this.Status = "";
            this.AccessCount = 0;
            nullable = null;
            this.FirstLoginTime = nullable;
            this.LastLoginTime = null;
            this.SystemDefaultOrg = false;
        }

        [DataMember(Name="AccessCount"), DataField(DataFieldName="access_count", NumericPercision=10)]
        public virtual int AccessCount
        {
            get
            {
                return base.GetValue<int>("AccessCount");
            }
            set
            {
                int num = base.GetValue<int>("AccessCount");
                base.SetValue<int>("AccessCount", value);
                if (num != value)
                {
                    base.NotifyPropertyChanged("AccessCount");
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

        [DataMember(Name="Extension"), DataField(DataFieldName="extension", AllowDBNull=true, Length=0x800)]
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

        [DataMember(Name="FirstLoginTime"), DataField(DataFieldName="first_login_time", AllowDBNull=true, NumericPercision=0x17, NumericScale=3)]
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

        [DataField(DataFieldName="last_login_time", AllowDBNull=true, NumericPercision=0x17, NumericScale=3), DataMember(Name="LastLoginTime")]
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

        [DataField(DataFieldName="login_fail_count", NumericPercision=10), DataMember(Name="LoginFailCount")]
        public virtual int LoginFailCount
        {
            get
            {
                return base.GetValue<int>("LoginFailCount");
            }
            set
            {
                int num = base.GetValue<int>("LoginFailCount");
                base.SetValue<int>("LoginFailCount", value);
                if (num != value)
                {
                    base.NotifyPropertyChanged("LoginFailCount");
                }
            }
        }

        [DataField(DataFieldName="organization_id", AllowDBNull=true, Length=0x24), DataMember(Name="OrganizationId")]
        public virtual string OrganizationId
        {
            get
            {
                return base.GetValue<string>("OrganizationId");
            }
            set
            {
                string str = base.GetValue<string>("OrganizationId");
                base.SetValue<string>("OrganizationId", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("OrganizationId");
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

        [DataMember(Name="Password"), DataField(DataFieldName="password", AllowDBNull=true, Length=50)]
        public virtual string Password
        {
            get
            {
                return base.GetValue<string>("Password");
            }
            set
            {
                string str = base.GetValue<string>("Password");
                base.SetValue<string>("Password", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Password");
                }
            }
        }

        [DataField(DataFieldName="password_expire_date", AllowDBNull=true, NumericPercision=0x17, NumericScale=3), DataMember(Name="PasswordExpireDate")]
        public virtual DateTime? PasswordExpireDate
        {
            get
            {
                return base.GetValue<DateTime?>("PasswordExpireDate");
            }
            set
            {
                DateTime? nullable = base.GetValue<DateTime?>("PasswordExpireDate");
                base.SetValue<DateTime?>("PasswordExpireDate", value);
                if (nullable != value)
                {
                    base.NotifyPropertyChanged("PasswordExpireDate");
                }
            }
        }

        [DataMember(Name="PasswordNeverExpire"), DataField(DataFieldName="password_never_expire")]
        public virtual bool PasswordNeverExpire
        {
            get
            {
                return base.GetValue<bool>("PasswordNeverExpire");
            }
            set
            {
                bool flag = base.GetValue<bool>("PasswordNeverExpire");
                base.SetValue<bool>("PasswordNeverExpire", value);
                if (flag != value)
                {
                    base.NotifyPropertyChanged("PasswordNeverExpire");
                }
            }
        }

        [DataMember(Name="PasswordResetOnLogin"), DataField(DataFieldName="password_reset_on_login")]
        public virtual bool PasswordResetOnLogin
        {
            get
            {
                return base.GetValue<bool>("PasswordResetOnLogin");
            }
            set
            {
                bool flag = base.GetValue<bool>("PasswordResetOnLogin");
                base.SetValue<bool>("PasswordResetOnLogin", value);
                if (flag != value)
                {
                    base.NotifyPropertyChanged("PasswordResetOnLogin");
                }
            }
        }

        [DataMember(Name="RealName"), DataField(DataFieldName="real_name", AllowDBNull=true, Length=30)]
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

        [DataField(DataFieldName="remark", AllowDBNull=true, Length=200), DataMember(Name="Remark")]
        public virtual string Remark
        {
            get
            {
                return base.GetValue<string>("Remark");
            }
            set
            {
                string str = base.GetValue<string>("Remark");
                base.SetValue<string>("Remark", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Remark");
                }
            }
        }

        [DataField(DataFieldName="root_organization_id", AllowDBNull=true, ReadOnly=true, Length=0x24), DataMember(Name="RootOrganizationId")]
        public virtual string RootOrganizationId
        {
            get
            {
                return base.GetValue<string>("RootOrganizationId");
            }
            set
            {
                string str = base.GetValue<string>("RootOrganizationId");
                base.SetValue<string>("RootOrganizationId", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("RootOrganizationId");
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

        [DataMember(Name="SystemDefaultOrg"), DataField(DataFieldName="system_default_org", ReadOnly=true)]
        public virtual bool SystemDefaultOrg
        {
            get
            {
                return base.GetValue<bool>("SystemDefaultOrg");
            }
            set
            {
                bool flag = base.GetValue<bool>("SystemDefaultOrg");
                base.SetValue<bool>("SystemDefaultOrg", value);
                if (flag != value)
                {
                    base.NotifyPropertyChanged("SystemDefaultOrg");
                }
            }
        }

        [DataField(DataFieldName="username", Length=50), DataMember(Name="Username")]
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

