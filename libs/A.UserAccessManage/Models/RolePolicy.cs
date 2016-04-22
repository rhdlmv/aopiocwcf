using A.DB;
using System;
using System.Runtime.Serialization;

namespace A.UserAccessManage.Model
{
    [DataContract, DataTable("role_policy"), QueryTable("role_policy_query")]
    public class RolePolicy : DbObject
    {
        public RolePolicy()
        {
            this.Id = Guid.NewGuid().ToString();
            this.RoleId = "";
            this.OperationCode = "";
            this.State = 0;
            this.CreateDatetime = DateTime.Now;
            this.OperationName = "";
        }

        [DataField(DataFieldName = "create_datetime", ReadOnly = true, NumericPercision = 0x17, NumericScale = 3), DataMember(Name = "CreateDatetime")]
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

        [DataField(DataFieldName = "create_user_id", AllowDBNull = true, Length = 0x24), DataMember(Name = "CreateUserId")]
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

        [DataField(DataFieldName = "id", PrimaryKey = true, Length = 0x24), DataMember(Name = "Id")]
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

        [DataMember(Name = "OperationCode"), DataField(DataFieldName = "operation_code", Length = 50)]
        public virtual string OperationCode
        {
            get
            {
                return base.GetValue<string>("OperationCode");
            }
            set
            {
                string str = base.GetValue<string>("OperationCode");
                base.SetValue<string>("OperationCode", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("OperationCode");
                }
            }
        }

        [DataMember(Name = "OperationName"), DataField(DataFieldName = "operation_name", ReadOnly = true, Length = 50)]
        public virtual string OperationName
        {
            get
            {
                return base.GetValue<string>("OperationName");
            }
            set
            {
                string str = base.GetValue<string>("OperationName");
                base.SetValue<string>("OperationName", value);
                if (str != value)
                {
                    base.NotifyPropertyChanged("OperationName");
                }
            }
        }

        [DataMember(Name = "RoleId"), DataField(DataFieldName = "role_id", Length = 0x24)]
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

        [DataMember(Name = "State"), DataField(DataFieldName = "state", NumericPercision = 10)]
        public virtual int State
        {
            get
            {
                return base.GetValue<int>("State");
            }
            set
            {
                int num = base.GetValue<int>("State");
                base.SetValue<int>("State", value);
                if (num != value)
                {
                    base.NotifyPropertyChanged("State");
                }
            }
        }
    }
}

