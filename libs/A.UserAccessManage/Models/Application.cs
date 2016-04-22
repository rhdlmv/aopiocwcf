namespace A.UserAccessManage.Model
{
    using A.DB;
    using System;
    using System.Runtime.Serialization;

    [DataContract, DataTable("application")]
    public class Application : DbObject
    {
        public Application()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Code = "";
            this.Name = "";
            this.Activate = false;
            this.CreateDatetime = DateTime.Now;
        }

        [DataField(DataFieldName="activate"), DataMember(Name="Activate")]
        public virtual bool Activate
        {
            get
            {
                return base.GetValue<bool>("Activate");
            }
            set
            {
                bool flag = base.GetValue<bool>("Activate");
                base.SetValue<bool>("Activate", value);
                if (flag != value)
                {
                    base.NotifyPropertyChanged("Activate");
                }
            }
        }

        [DataField(DataFieldName="code", Length=0x20), DataMember(Name="Code")]
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

        [DataMember(Name="Name"), DataField(DataFieldName="name", Length=0x80)]
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
    }
}

