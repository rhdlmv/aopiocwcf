namespace A.UserAccessManage.Model
{
    using A.DB;
    using System;
    using System.Runtime.Serialization;

    [DataTable("organization_available_application"), DataContract]
    public class OrganizationAvailableApplication : DbObject
    {
        public OrganizationAvailableApplication()
        {
            this.Id = Guid.NewGuid().ToString();
            this.OrganizationId = "";
            this.ApplicationCode = "";
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

        [DataField(DataFieldName="organization_id", Length=0x24), DataMember(Name="OrganizationId")]
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
    }
}

