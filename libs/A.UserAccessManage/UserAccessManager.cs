using A.Commons.Db;
using A.Commons.Exception;
using A.Commons.Utils;
using A.DB;
using A.UserAccessManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.InteropServices;

namespace A.UserAccessManage
{
    public class UserAccessManager : MarshalByRefObject
    {
        private A.UserAccessManage.AccountManager _am;
        private int _timeout;
        private string connectionString;

        protected UserAccessManager()
        {
            this._timeout = 600;
        }

        public UserAccessManager(DbObjectOperator op) : this()
        {
            this._am = new A.UserAccessManage.AccountManager(op);
            this.ObjectOperator = op;
        }

        public UserAccessManager(string connectionString) : this()
        {
            this.connectionString = connectionString;
            DbObjectOperator op = new DbObjectOperator(this.connectionString);
            this._am = new A.UserAccessManage.AccountManager(op);
            this.ObjectOperator = op;
        }

        public virtual bool CheckAccess(string opKey)
        {
            UserAccess item = this.FindUserAccessItem(opKey);
            if (item == null)
            {
                return false;
            }
            if (!item.Active)
            {
                return false;
            }
            if (!this.CheckAlive(item))
            {
                item.Active = false;
                this.ObjectOperator.Update(item);
                return false;
            }
            User userById = this.AccountManager.GetUserById(item.UserId, false);
            return ((userById != null) && "ACTIVE".Equals(userById.Status));
        }

        private bool CheckAlive(UserAccess item)
        {
            long num = DateTime.Now.Ticks - item.LastAccess.Value.Ticks;
            num /= 0x989680L;
            return (num < this._timeout);
        }

        private UserAccess FindLastUserAccessItem(string user_id)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("user_id = '{0}'", user_id);
            OrderBy[] orderBys = new OrderBy[1];
            OrderBy by = new OrderBy
            {
                Column = "last_access",
                Desc = true
            };
            orderBys[0] = by;
            List<UserAccess> list = ((DbObjectOperator)this.ObjectOperator).QueryObjects<UserAccess>(builder.ToString(), 0x7fffffff, 0, orderBys);
            if ((list != null) && (list.Count > 0))
            {
                return list[0];
            }
            return null;
        }

        private RolePolicy FindRolePolicy(List<RolePolicy> policys, string operationCode)
        {
            List<RolePolicy> list = (from item in policys
                                     where item.OperationCode == operationCode
                                     select item).ToList<RolePolicy>();
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public virtual UserAccess FindUserAccessItem(string opKey)
        {
            return this.ObjectOperator.Retrieve<UserAccess>("operation_key", opKey);
        }

        public virtual Organization GetOrganization(User user)
        {
            return this.ObjectOperator.Retrieve<Organization>("id", user.OrganizationId);
        }

        public virtual Organization GetOrganization(string opKey)
        {
            User user = this.GetUser(opKey, true);
            if (user == null)
            {
                return null;
            }
            return this.ObjectOperator.Retrieve<Organization>("id", user.OrganizationId);
        }

        public virtual User GetUser(string opKey, bool filterPassword = true)
        {
            UserAccess access = this.FindUserAccessItem(opKey);
            if (access == null)
            {
                return null;
            }
            User user = new User();
            this.ObjectOperator.Retrieve(user, "id", access.UserId);
            user.Id = access.UserId;
            if (user == null)
            {
                return null;
            }
            User des = new User();
            if (filterPassword)
            {
                A.Commons.Utils.CloneUtils.CloneObject(user, des, new string[] { "Password" });
                return des;
            }
            A.Commons.Utils.CloneUtils.CloneObject(user, des, new string[0]);
            return des;
        }

        public virtual List<RolePolicy> GetUserRolePolicys(string userId, string applicationCode)
        {
            int num;
            int num3;
            if (this.ObjectOperator.Retrieve<User>("id", userId) == null)
            {
                throw new ObjectNotFoundException<User>("UE01-002", "User Not Found");
            }
            List<UserRole> userRoles = this._am.GetUserRoles(userId);
            int count = userRoles.Count;
            List<RolePolicy>[] listArray = new List<RolePolicy>[count];
            for (num = 0; num < count; num++)
            {
                listArray[num] = this._am.GetRolePolicys(userRoles[num].RoleId);
            }
            List<Operation> operations = this._am.GetOperations(applicationCode);
            int num4 = operations.Count;
            RolePolicy[] source = new RolePolicy[num4];
            for (num = 0; num < count; num++)
            {
                num3 = 0;
                while (num3 < num4)
                {
                    if ((source[num3] == null) || (source[num3].State == 0))
                    {
                        RolePolicy policy = this.FindRolePolicy(listArray[num], operations[num3].Code);
                        if (policy != null)
                        {
                            source[num3] = policy;
                        }
                    }
                    num3++;
                }
            }
            List<RolePolicy> list3 = source.ToList<RolePolicy>();
            for (num3 = 0; num3 < list3.Count; num3++)
            {
                if (list3[num3] == null)
                {
                    list3.Remove(list3[num3]);
                    num3--;
                }
            }
            return list3;
        }

        public virtual bool KeepAlive(string opKey)
        {
            UserAccess item = this.FindUserAccessItem(opKey);
            if (item == null)
            {
                return false;
            }
            if (!item.Active)
            {
                return false;
            }
            if (!this.CheckAlive(item))
            {
                item.Active = false;
                this.ObjectOperator.Update(item);
                return false;
            }
            item.LastAccess = new DateTime?(DateTime.Now);
            item.AccessCount += 1;
            this.ObjectOperator.Update(item);
            return true;
        }

        public virtual string Login(string domain, string applicationCode, string client, string username, string verify_code, string transaction_code, string clientId, string extension, int failCountLock = 0)
        {
            UserAccess access2 = null;
            string operationKey = null;
            User user = this.VerifyUser(domain, username, verify_code, transaction_code, failCountLock, extension);
            if (user == null)
            {
                throw new PasswordIncorrectException("Username or password is incorrect!");
            }
            access2 = this.FindLastUserAccessItem(user.Id);
            UserAccess access = new UserAccess
            {
                UserId = user.Id,
                Client = client,
                ApplicationCode = applicationCode,
                OperationKey = Guid.NewGuid().ToString(),
                Active = true,
                LastAccess = new DateTime?(DateTime.Now),
                FirstAccess = DateTime.Now,
                AccessCount = 0,
                ClientId = clientId,
                Extension = extension
            };
            operationKey = access.OperationKey;
            if (access2 != null)
            {
                access.PreviousAccess = access2.LastAccess;
            }
            this.ObjectOperator.Insert(access);
            if ((this.EnableUniqueAccess && (access2 != null)) && access2.Active)
            {
                access2.Active = false;
                this.ObjectOperator.Update(access2);
            }
            user.AccessCount++;
            DateTime now = DateTime.Now;
            if (user.AccessCount == 1)
            {
                user.FirstLoginTime = new DateTime?(now);
            }
            user.LastLoginTime = new DateTime?(now);
            this.ObjectOperator.Update(user);
            return operationKey;
        }

        public virtual bool Logout(string opKey)
        {
            UserAccess access = this.FindUserAccessItem(opKey);
            if (access == null)
            {
                return false;
            }
            if (!access.Active)
            {
                return false;
            }
            access.Active = false;
            this.ObjectOperator.Update(access);
            return true;
        }

        public virtual User VerifyUser(string domain, string username, string verify_code, string transaction_code, int failCountLock, string extension)
        {
            User userByUsername = this._am.GetUserByUsername(domain, username);
            if (userByUsername == null)
            {
                return null;
            }
            if (!"ACTIVE".Equals(userByUsername.Status))
            {
                throw new DeactiveUserException("User is not active!");
            }
            string parentId = this._am.GetOrganization(userByUsername.OrganizationId, true).ParentId;
            while (parentId != null)
            {
                Organization organization = this._am.GetOrganization(parentId, true);
                parentId = organization.ParentId;
                if (!"ACTIVE".Equals(organization.Status))
                {
                    throw new DeactiveUserException("Organization is not active!");
                }
            }
            bool flag = this._am.Authenticate(userByUsername, transaction_code, verify_code, extension);
            if (!flag)
            {
                int loginFailCount = userByUsername.LoginFailCount;
                if ((failCountLock > 0) && ((loginFailCount + 1) == failCountLock))
                {
                    userByUsername.Status = "FREEZED";
                }
                userByUsername.LoginFailCount = loginFailCount + 1;
            }
            else
            {
                userByUsername.LoginFailCount = 0;
            }
            this._am.UpdateUser(userByUsername);
            if (!flag)
            {
                return null;
            }
            return userByUsername;
        }

        public A.UserAccessManage.AccountManager AccountManager
        {
            get
            {
                return this._am;
            }
        }

        public bool EnableUniqueAccess { get; set; }

        public IObjectOperator ObjectOperator { get; private set; }

        public int Timeout
        {
            get
            {
                return this._timeout;
            }
            set
            {
                this._timeout = value;
            }
        }
    }
}

