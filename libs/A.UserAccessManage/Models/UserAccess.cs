namespace A.UserAccessManage.Model
{
    using A.DB;
    using System;
    using System.Runtime.Serialization;

    [DataContract, DataTable("user_access"), QueryTable("user_access_query")]
    public class UserAccess : DbObject
    {
        public UserAccess()
        {
            this.Id = Guid.NewGuid().ToString();
            this.OperationKey = "";
            this.ApplicationCode = "";
            this.UserId = "";
            this.FirstAccess = DateTime.Now;
            this.LastAccess = null;
            this.PreviousAccess = null;
            this.AccessCount = null;
            this.Active = false;
            this.Username = "";
        }

        [DataField(DataFieldName="access_count", AllowDBNull=true, NumericPercision=10), DataMember(Name="AccessCount")]
        public virtual int? AccessCount
        {
            get
            {
                return base.GetValue<int?>("AccessCount");
            }
            set
            {
                int? nullable = base.GetValue<int?>("AccessCount");
                base.SetValue<int?>("AccessCount", value);
                if (nullable != value)
                {
                    base.NotifyPropertyChanged("AccessCount");
                }
            }
        }

        [DataField(DataFieldName="active"), DataMember(Name="Active")]
        public virtual bool Active
        {
            get
            {
                return base.GetValue<bool>("Active");
            }
            set
            {
                bool flag = base.GetValue<bool>("Active");
                base.SetValue<bool>("Active", value);
                if (flag != value)
                {
                    base.NotifyPropertyChanged("Active");
                }
            }
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

        [DataField(DataFieldName="client", AllowDBNull=true, Length=50), DataMember(Name="Client")]
        public virtual string Client
        {
            get
            {
                return base.GetValue<string>("Client");
            }
            set
            {
                string str = base.GetValue<string>("Client");
                base.SetValue<string>("Client", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("Client");
                }
            }
        }

        [DataMember(Name="ClientId"), DataField(DataFieldName="client_id", AllowDBNull=true, Length=0x20)]
        public virtual string ClientId
        {
            get
            {
                return base.GetValue<string>("ClientId");
            }
            set
            {
                string str = base.GetValue<string>("ClientId");
                base.SetValue<string>("ClientId", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("ClientId");
                }
            }
        }

        [DataMember(Name="Extension"), DataField(DataFieldName="extension", AllowDBNull=true)]
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

        [DataField(DataFieldName="first_access", NumericPercision=0x17, NumericScale=3), DataMember(Name="FirstAccess")]
        public virtual DateTime FirstAccess
        {
            get
            {
                return base.GetValue<DateTime>("FirstAccess");
            }
            set
            {
                DateTime time = base.GetValue<DateTime>("FirstAccess");
                base.SetValue<DateTime>("FirstAccess", value);
                if (time != value)
                {
                    base.NotifyPropertyChanged("FirstAccess");
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

        [DataMember(Name="LastAccess"), DataField(DataFieldName="last_access", AllowDBNull=true, NumericPercision=0x17, NumericScale=3)]
        public virtual DateTime? LastAccess
        {
            get
            {
                return base.GetValue<DateTime?>("LastAccess");
            }
            set
            {
                DateTime? nullable = base.GetValue<DateTime?>("LastAccess");
                base.SetValue<DateTime?>("LastAccess", value);
                if (nullable != value)
                {
                    base.NotifyPropertyChanged("LastAccess");
                }
            }
        }

        [DataMember(Name="OperationKey"), DataField(DataFieldName="operation_key", Length=50)]
        public virtual string OperationKey
        {
            get
            {
                return base.GetValue<string>("OperationKey");
            }
            set
            {
                string str = base.GetValue<string>("OperationKey");
                base.SetValue<string>("OperationKey", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("OperationKey");
                }
            }
        }

        [DataField(DataFieldName="previous_access", AllowDBNull=true, NumericPercision=0x17, NumericScale=3), DataMember(Name="PreviousAccess")]
        public virtual DateTime? PreviousAccess
        {
            get
            {
                return base.GetValue<DateTime?>("PreviousAccess");
            }
            set
            {
                DateTime? nullable = base.GetValue<DateTime?>("PreviousAccess");
                base.SetValue<DateTime?>("PreviousAccess", value);
                if (nullable != value)
                {
                    base.NotifyPropertyChanged("PreviousAccess");
                }
            }
        }

        [DataMember(Name="UserId"), DataField(DataFieldName="user_id", Length=0x24)]
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

