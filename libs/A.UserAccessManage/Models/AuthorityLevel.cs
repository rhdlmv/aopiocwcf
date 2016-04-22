namespace A.UserAccessManage.Model
{
    using A.DB;
    using System;
    using System.Runtime.Serialization;

    [DataTable("authority_level"), DataContract]
    public class AuthorityLevel : DbObject
    {
        public AuthorityLevel()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = "";
            this.Level = 0;
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

        [DataField(DataFieldName="level", NumericPercision=10), DataMember(Name="Level")]
        public virtual int Level
        {
            get
            {
                return base.GetValue<int>("Level");
            }
            set
            {
                int num = base.GetValue<int>("Level");
                base.SetValue<int>("Level", value);
                if (num != value)
                {
                    base.NotifyPropertyChanged("Level");
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
    }
}

