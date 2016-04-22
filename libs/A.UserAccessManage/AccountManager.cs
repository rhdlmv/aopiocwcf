using A.Commons.Db;
using A.Commons.Exception;
using A.Commons.Utils;
using A.DB;
using A.UserAccessManage.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;

namespace A.UserAccessManage
{
    public class AccountManager
    {
        private IPasswordEncryptor _passwordEncryptor;
        private DbObjectOperator op;

        public AccountManager(DbObjectOperator op)
        {
            this.op = op;
        }

        public UserRole AssignUserRole(string userId, string roleId)
        {
            if (this.GetUserById(userId, false) == null)
            {
                throw new ObjectNotFoundException<User>("UE01-002", "User Not Found");
            }
            if (this.GetRole(roleId, false) == null)
            {
                throw new ObjectNotFoundException<Role>("UE01-004", "Role Not Found");
            }
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("user_id = '{0}' and role_id = '{1}'", userId, roleId);
            UserRole role2 = this.op.Retrieve<UserRole>(builder.ToString());
            if (role2 == null)
            {
                role2 = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    Sequence = 0
                };
                this.op.Insert(role2);
            }
            return role2;
        }

        public bool Authenticate(User user, string transactionCode, string credential, string extension)
        {
            IAuthenticator authenticator = this.GetAuthenticator(extension);
            if (authenticator == null)
            {
                return false;
            }
            User des = new User();
            A.Commons.Utils.CloneUtils.CloneObject(user, des, new string[0]);
            if (this.PasswordEncryptor.CanDecrypt)
            {
                des.Password = this.PasswordEncryptor.Decrypt(des, des.Password);
            }
            return authenticator.Authenticate(des, transactionCode, credential, extension);
        }

        public void ChagnePassword(User user, string newPassword)
        {
            User userById = this.GetUserById(user.Id, false);
            if (userById == null)
            {
                throw new ObjectNotFoundException<User>("UE01-002", "User Not Found");
            }
            userById.Password = this.PasswordEncryptor.Encrypt(userById, newPassword);
            this.op.Update(userById);
        }

        private bool CheckOperationDependent(string operationCode)
        {
            bool flag = false;
            string condition = "operation_code = '" + operationCode + "'";
            if (this.op.QueryCount<RolePolicy>(condition) > 0)
            {
                flag = true;
            }
            return flag;
        }

        private bool CheckOperationTypeDependent(string operationTypeId)
        {
            bool flag = false;
            string condition = "operation_type_id = '" + operationTypeId + "'";
            if (this.op.QueryCount<RolePolicy>(condition) > 0)
            {
                flag = true;
            }
            return flag;
        }

        public bool CheckPassword(User user, string passwordHashCode)
        {
            string password;
            if (this.PasswordEncryptor.CanDecrypt)
            {
                password = this.PasswordEncryptor.Decrypt(user, user.Password);
            }
            else
            {
                password = user.Password;
            }
            return (EncryptUtils.GenSHA1Code(password) == passwordHashCode);
        }

        private bool CheckRoleDependent(string roleId)
        {
            bool flag = false;
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("role_id = '{0}'", roleId);
            if (this.op.QueryCount<UserRole>(builder.ToString()) > 0)
            {
                flag = true;
            }
            return flag;
        }

        private List<T> CloneItems<T>(IList list) where T : IStorageObject, new()
        {
            List<T> list2 = new List<T>();
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                T local2 = default(T);
                T des = (local2 == null) ? Activator.CreateInstance<T>() : default(T);
                A.Commons.Utils.CloneUtils.CloneObject((T)list[i], des, new string[0]);
                list2.Add(des);
            }
            return list2;
        }

        public AuthorityLevel CreateAuthorityLevel(string name, int level)
        {
            if (this.op.Retrieve<AuthorityLevel>("level", level) != null)
            {
                throw new ObjectAlreadyExistException<AuthorityLevel>("UE02-010", "AuthorityLevel already exist!");
            }
            AuthorityLevel level2 = new AuthorityLevel
            {
                Name = name,
                Level = level
            };
            this.op.Insert(level2);
            return level2;
        }

        public Operation CreateOperation(string name, string operation_code, string operation_type_guid)
        {
            if (this.GetOperationType(operation_type_guid) == null)
            {
                throw new ObjectNotFoundException<OperationType>("UE01-006", "Operation type not found!");
            }
            if (this.op.Retrieve<Operation>(string.Format("code = '{0}'", operation_code)) != null)
            {
                throw new ObjectAlreadyExistException<Operation>("UE02-009", "Operation already exist");
            }
            Operation operation = new Operation
            {
                Name = name,
                Code = operation_code,
                OperationTypeId = operation_type_guid
            };
            this.op.Insert(operation);
            return operation;
        }

        public OperationType CreateOperationType(string name, string parentId, string applicationCode)
        {
            if (this.op.Retrieve<OperationType>(string.Format("name = '{0}' and application_code = '{1}'", name, applicationCode)) != null)
            {
                throw new ObjectAlreadyExistException<OperationType>("UE02-008", "Operation type name is exist");
            }
            OperationType type = new OperationType
            {
                Name = name,
                ParentId = parentId,
                ApplicationCode = applicationCode
            };
            this.op.Insert(type);
            return type;
        }

        public Organization CreateOrganization(string name, string domain, string serial, string parentId, bool systemDefault, string status)
        {
            Organization organization = this.op.Retrieve<Organization>("serial", serial);
            if ((organization != null) && !organization.Invalid)
            {
                throw new ObjectAlreadyExistException<Organization>("UE02-004", "org exist");
            }
            if (string.IsNullOrWhiteSpace(domain))
            {
                if (string.IsNullOrWhiteSpace(parentId))
                {
                    throw new ArgumentIsNullOrEmptyException("Domain cannot be empty");
                }
            }
            else
            {
                organization = this.op.Retrieve<Organization>("domain", domain);
                if ((organization != null) && !organization.Invalid)
                {
                    throw new ObjectAlreadyExistException<Organization>("UE02-005", "Domain exist");
                }
            }
            Organization organization2 = null;
            if (!string.IsNullOrEmpty(parentId))
            {
                organization2 = this.GetOrganization(parentId, true);
            }
            Organization organization3 = new Organization
            {
                Domain = domain,
                Serial = serial,
                Name = name,
                SystemDefault = systemDefault
            };
            if (organization2 != null)
            {
                organization3.ParentId = organization2.Id;
                if (!string.IsNullOrEmpty(organization2.RootId))
                {
                    organization3.RootId = organization2.RootId;
                }
                else
                {
                    organization3.RootId = organization2.Id;
                }
            }
            if (string.IsNullOrWhiteSpace(status))
            {
                organization3.Status = "ACTIVE";
            }
            else
            {
                this.ValidateStatus(status, typeof(OrganizationStatus));
                organization3.Status = status;
            }
            this.op.Insert(organization3);
            return this.GetOrganization(organization3.Id, false);
        }

        public OrganizationAvailableApplication CreateOrganizationAvailableApplication(string organizationId, string applicationCode)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("organization_id = '{0}' and application_code = '{1}'", organizationId, applicationCode);
            if (this.op.Retrieve<OrganizationAvailableApplication>(builder.ToString()) != null)
            {
                throw new ObjectAlreadyExistException<OrganizationAvailableApplication>("UE02-007", "Organization access policy already exists");
            }
            OrganizationAvailableApplication application = new OrganizationAvailableApplication
            {
                OrganizationId = organizationId,
                ApplicationCode = applicationCode
            };
            this.op.Insert(application);
            return application;
        }

        public Role CreateRole(Role role)
        {
            if (StringUtils.IsNullOrEmptyOrWhiteSpace(role.Name))
            {
                throw new ArgumentIsNullOrEmptyException("Role name is required.");
            }
            if (StringUtils.IsNullOrEmptyOrWhiteSpace(role.ApplicationCode))
            {
                throw new ArgumentIsNullOrEmptyException("Application code is required.");
            }
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("invalid = 0 and name = '{0}' and application_code = '{1}'", role.Name, role.ApplicationCode);
            if (this.op.Retrieve<Role>(builder.ToString()) != null)
            {
                throw new ObjectAlreadyExistException<Role>("UE02-006", "Role already exists");
            }
            Role des = new Role();
            A.Commons.Utils.CloneUtils.CloneObjectIgnoreId(role, des);
            this.op.Insert(des);
            return des;
        }

        public RolePolicy CreateRolePolicy(string roleId, string operationCode, int state)
        {
            if ((state != 0) && (state != 1))
            {
                throw new InvalidPolicyStateException("State must be 1 or 0.");
            }
            if (this.op.Retrieve<Role>("id", roleId) == null)
            {
                throw new ObjectNotFoundException<Role>("UE01-004", "Role Not Found");
            }
            if (this.op.Retrieve<Operation>("code", operationCode) == null)
            {
                throw new ObjectNotFoundException<Operation>("UE01-005", "Operation not found");
            }
            RolePolicy policy = new RolePolicy
            {
                RoleId = roleId,
                OperationCode = operationCode,
                State = state
            };
            this.op.Insert(policy);
            return policy;
        }

        public User CreateUser(User user)
        {
            User user2 = new User();
            if (this.FindExistingObject(this.op, user2, "username", user.Username, user.OrganizationId))
            {
                throw new ObjectAlreadyExistException<User>("UE02-002", "Username already exists");
            }
            A.Commons.Utils.CloneUtils.CloneObject(user, user2, new string[] { "Id", "Password" });
            if (StringUtils.IsNullOrEmptyOrWhiteSpace(user.Serial))
            {
                user2.Serial = "--";
            }
            else
            {
                SqlStringBuilder builder = new SqlStringBuilder();
                builder.AppendFormat("serial = '{0}' and organization_id = '{1}'", user.Serial, user.OrganizationId);
                if (this.op.Retrieve<User>(builder.ToString()) != null)
                {
                    throw new ObjectAlreadyExistException<User>("UE02-011", "User with this serial already exists");
                }
            }
            user2.Password = this.PasswordEncryptor.Encrypt(user2, user.Password);
            if (string.IsNullOrWhiteSpace(user.Status))
            {
                user2.Status = "ACTIVE";
            }
            else
            {
                this.ValidateStatus(user.Status, typeof(UserStatus));
            }
            this.op.Insert(user2);
            return this.GetUserById(user2.Id, false);
        }

        private bool FindExistingObject(DbObjectOperator op, IStorageObject obj, string name, object value, string organization_guid)
        {
            string str;
            if ((value is string) || (value is DateTime))
            {
                str = string.Format("{0} = '{1}'", name, value);
            }
            else
            {
                str = string.Format("{0} = {1}", name, value);
            }
            str = str + " and " + string.Format("organization_id = '{0}'", organization_guid);
            return op.Retrieve(obj, str);
        }

        public Application GetApplicationByCode(string applicationCode)
        {
            return this.op.Retrieve<Application>("code", applicationCode);
        }

        public List<Application> GetApplications()
        {
            OrderBy[] orderBys = new OrderBy[1];
            OrderBy by = new OrderBy
            {
                Column = "name"
            };
            orderBys[0] = by;
            return this.op.QueryObjects<Application>("", 0x7fffffff, 0, orderBys);
        }

        private IAuthenticator GetAuthenticator(string extension)
        {
            string authenticationMethod = ExtensionUtils.GetAuthenticationMethod(extension);
            if (!string.IsNullOrWhiteSpace(authenticationMethod))
            {
                string str2 = ConfigurationManager.AppSettings[authenticationMethod];
                if (!string.IsNullOrWhiteSpace(str2))
                {
                    return (Activator.CreateInstance(Type.GetType(str2)) as IAuthenticator);
                }
            }
            return new UsernamePasswordAuthenticator();
        }

        public List<AuthorityLevel> GetAuthorityLevels()
        {
            OrderBy[] orderBys = new OrderBy[1];
            OrderBy by = new OrderBy
            {
                Column = "level"
            };
            orderBys[0] = by;
            return this.op.QueryObjects<AuthorityLevel>("", 0x7fffffff, 0, orderBys);
        }

        public List<Organization> GetChildOrganizations(string parentOrganizationId)
        {
            this.GetOrganization(parentOrganizationId, true);
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat(" parent_id = '{0}'", parentOrganizationId);
            OrderBy[] orderBys = new OrderBy[1];
            OrderBy by = new OrderBy
            {
                Column = "name"
            };
            orderBys[0] = by;
            return this.op.QueryObjects<Organization>(builder.ToString(), 0x7fffffff, 0, orderBys);
        }

        private List<T1> GetItems<T1>(string condition) where T1 : IStorageObject, new()
        {
            return this.op.QueryObjects<T1>(condition, 0x7fffffff, 0, null);
        }

        public List<Operation> GetOperations(string application_code)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("application_code = '{0}'", application_code);
            OrderBy[] orderBys = new OrderBy[1];
            OrderBy by = new OrderBy
            {
                Column = "code"
            };
            orderBys[0] = by;
            return this.op.QueryObjects<Operation>(builder.ToString(), 0x7fffffff, 0, orderBys);
        }

        public List<Operation> GetOperationsByOperationType(string operationTypeId)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("operation_type_id = '{0}'", operationTypeId);
            OrderBy[] orderBys = new OrderBy[1];
            OrderBy by = new OrderBy
            {
                Column = "code"
            };
            orderBys[0] = by;
            return this.op.QueryObjects<Operation>(builder.ToString(), 0x7fffffff, 0, orderBys);
        }

        public OperationType GetOperationType(string operationTypeId)
        {
            return this.op.Retrieve<OperationType>("id", operationTypeId);
        }

        public List<OperationType> GetOperationTypes(string applicationCode)
        {
            string condition = string.Format("application_code = '{0}'", applicationCode);
            OrderBy[] orderBys = new OrderBy[1];
            OrderBy by = new OrderBy
            {
                Column = "id"
            };
            orderBys[0] = by;
            return this.op.QueryObjects<OperationType>(condition, 0x7fffffff, 0, orderBys);
        }

        public Organization GetOrganization(string orgId, bool checkExist = false)
        {
            SqlStringBuilder builder = new SqlStringBuilder("invalid = 0 ");
            builder.AppendFormat(" and id = '{0}'", orgId);
            Organization organization = this.op.Retrieve<Organization>(builder.ToString());
            if ((organization == null) && checkExist)
            {
                throw new ObjectNotFoundException<Organization>("UE01-001", "Organization Not Found");
            }
            return organization;
        }

        public List<OrganizationAvailableApplication> GetOrganizationApplications(string organizationId, bool includeDeactiveRecord)
        {
            if (this.GetOrganization(organizationId, false) == null)
            {
                throw new ObjectNotFoundException<Organization>("UE01-001", "Organization Not Found");
            }
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("organization_id = '{0}'", organizationId);
            OrderBy[] orderBys = new OrderBy[1];
            OrderBy by = new OrderBy
            {
                Column = "application_code"
            };
            orderBys[0] = by;
            return this.op.QueryObjects<OrganizationAvailableApplication>(builder.ToString(), 0x7fffffff, 0, orderBys);
        }

        public Organization GetOrganizationByDomain(string domain)
        {
            if (domain == null)
            {
                return null;
            }
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat(" domain = '{0}'", domain);
            return this.op.Retrieve<Organization>(builder.ToString());
        }

        public User GetOrganizationUserByUsername(string organizationId, string username)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("organization_id = '{0}'", organizationId);
            builder.AppendFormat(" and username = '{0}'", username);
            return this.op.Retrieve<User>(builder.ToString());
        }

        public Role GetRole(string roleId, bool checkExist = false)
        {
            SqlStringBuilder builder = new SqlStringBuilder("invalid = 0");
            builder.AppendFormat(" and id = '{0}'", roleId);
            Role role = this.op.Retrieve<Role>(builder.ToString());
            if ((role == null) && checkExist)
            {
                throw new ObjectNotFoundException<Role>("UE01-004", "Role not exist");
            }
            return role;
        }

        public List<RolePolicy> GetRolePolicys(string roleId)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("role_id = '{0}'", roleId);
            return this.GetItems<RolePolicy>(builder.ToString());
        }

        public List<Role> GetRoles(string applicationCode)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("application_code = '{0}' and invalid = 0", applicationCode);
            OrderBy[] orderBys = new OrderBy[1];
            OrderBy by = new OrderBy
            {
                Column = "name"
            };
            orderBys[0] = by;
            return this.op.QueryObjects<Role>(builder.ToString(), 0x7fffffff, 0, orderBys);
        }

        public List<UserRole> GetRoleUsers(string roleId)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("role_id = '{0}'", roleId);
            return this.GetItems<UserRole>(builder.ToString());
        }

        public List<Organization> GetSubOrganizations(string parentOrganizationId)
        {
            this.GetOrganization(parentOrganizationId, true);
            List<Organization> list = new List<Organization>();
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat(" parent_id = '{0}'", parentOrganizationId);
            OrderBy[] orderBys = new OrderBy[1];
            OrderBy by = new OrderBy
            {
                Column = "name"
            };
            orderBys[0] = by;
            List<Organization> collection = this.op.QueryObjects<Organization>(builder.ToString(), 0x7fffffff, 0, orderBys);
            if (collection != null)
            {
                list.AddRange(collection);
                foreach (Organization organization in collection)
                {
                    List<Organization> subOrganizations = this.GetSubOrganizations(organization.Id);
                    if (subOrganizations != null)
                    {
                        list.AddRange(subOrganizations);
                    }
                }
            }
            return list;
        }

        public User GetUserById(string user_id, bool filterPassword = false)
        {
            if (user_id == null)
            {
                return null;
            }
            User user = this.op.Retrieve<User>("id", user_id);
            if ((user != null) && filterPassword)
            {
                user.Password = null;
            }
            return user;
        }

        public User GetUserByUsername(string domain, string username)
        {
            Organization organizationByDomain = null;
            organizationByDomain = this.GetOrganizationByDomain(domain);
            if (organizationByDomain == null)
            {
                return null;
            }
            User organizationUserByUsername = this.GetOrganizationUserByUsername(organizationByDomain.Id, username);
            if (organizationUserByUsername == null)
            {
                SqlStringBuilder builder = new SqlStringBuilder();
                builder.AppendFormat("root_organization_id = '{0}'", organizationByDomain.Id);
                builder.AppendFormat(" and username = '{0}'", username);
                organizationUserByUsername = this.op.Retrieve<User>(builder.ToString());
            }
            return organizationUserByUsername;
        }

        public List<UserRole> GetUserRoles(string userId)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("user_id = '{0}'", userId);
            return this.GetItems<UserRole>(builder.ToString());
        }

        public List<UserRole> GetUserRoles(string userId, string applicationCode)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("user_id = '{0}' and application_code = '{1}'", userId, applicationCode);
            return this.GetItems<UserRole>(builder.ToString());
        }

        public void RemoveOperation(string operation_id)
        {
            Operation operation = this.op.Retrieve<Operation>("id", operation_id);
            if (operation == null)
            {
                throw new ObjectNotFoundException<Operation>("UE01-005", "Operation not found");
            }
            if (this.CheckOperationDependent(operation.Code))
            {
                throw new ObjectDependencyException<Operation>("UE05-007", "Some Object Depend On The Operarion, Can Not Remove");
            }
            this.op.Delete(operation);
        }

        public void RemoveOperationType(string operationTypeId)
        {
            OperationType type = this.op.Retrieve<OperationType>("id", operationTypeId);
            if (this.CheckOperationTypeDependent(operationTypeId))
            {
                throw new ObjectDependencyException<OperationType>("UE05-006", "Some Object Depend On The Operation type, Can Not Remove");
            }
            this.op.Delete(type);
        }

        public void RemoveOrganization(string organizationId)
        {
            Organization organization = this.GetOrganization(organizationId, true);
            if (organization.SystemDefault)
            {
                throw new RemoveSystemDefaultRecordException<User>("UE03-002", "Can not remove system default organization");
            }
            List<Organization> childOrganizations = this.GetChildOrganizations(organizationId);
            if ((childOrganizations != null) && (childOrganizations.Count > 0))
            {
                throw new RemoveAssociatedRecordException();
            }
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("organization_id = '{0}'", organizationId);
            if (this.op.QueryCount<User>(builder.ToString()) > 0)
            {
                throw new RemoveAssociatedRecordException();
            }
            organization.Invalid = true;
            this.op.Update(organization);
        }

        public void RemoveOrganizationAvailableApplication(string id)
        {
            OrganizationAvailableApplication application = this.op.Retrieve<OrganizationAvailableApplication>("id", id);
            if (application != null)
            {
                this.op.Delete(application);
            }
        }

        public void RemoveRole(string roleId)
        {
            Role role = this.op.Retrieve<Role>("id", roleId);
            if (role == null)
            {
                throw new ObjectNotFoundException<Role>("UE01-004", "Role not exist");
            }
            if (role.SystemDefault)
            {
                throw new RemoveSystemDefaultRecordException<Role>("UE03-003", "System Default Role Can Not Remove");
            }
            if (this.CheckRoleDependent(roleId))
            {
                throw new ObjectDependencyException<Role>("UE05-005", "Some Object Depend On The Role, Can Not Remove");
            }
            role.Invalid = true;
            this.op.Update(role);
        }

        public void RemoveRolePolicy(string rolePolicyId)
        {
            RolePolicy policy = this.op.Retrieve<RolePolicy>("id", rolePolicyId);
            if (policy != null)
            {
                this.op.Delete(policy);
            }
        }

        public void RemoveUser(string user_id)
        {
            User user = this.op.Retrieve<User>("id", user_id);
            if (user == null)
            {
                throw new ObjectNotFoundException<User>("UE01-002", "User Not Found");
            }
            if (user.SystemDefault)
            {
                throw new RemoveSystemDefaultRecordException<User>("UE03-001", "Can not remove system default user");
            }
            user.Invalid = true;
            this.op.Update(user);
        }

        public void RevokeUserRole(string userRoleId)
        {
            UserRole role = this.op.Retrieve<UserRole>("id", userRoleId);
            if (role != null)
            {
                this.op.Delete(role);
            }
        }

        private void SetItems<T>(string condition, IList list) where T : IStorageObject, new()
        {
            int num;
            List<T> list2 = this.CloneItems<T>(list);
            List<T> list3 = this.op.QueryObjects<T>(condition, 0x7fffffff, 0, null);
            int count = list3.Count;
            for (num = 0; num < count; num++)
            {
                this.op.Delete(list3[num]);
            }
            count = list2.Count;
            for (num = 0; num < count; num++)
            {
                this.op.Insert(list2[num]);
            }
        }

        public void SetOrganizationStatus(string organizationId, string status)
        {
            this.ValidateStatus(status, typeof(OrganizationStatus));
            Organization organization = this.GetOrganization(organizationId, true);
            organization.Status = status;
            this.op.Update(organization);
        }

        public void SetRolePolicys(string roleId, List<RolePolicy> policys)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("role_id = '{0}'", roleId);
            int count = policys.Count;
            for (int i = 0; i < count; i++)
            {
                policys[i].RoleId = roleId;
                if (StringUtils.IsNullOrEmptyOrWhiteSpace(policys[i].Id))
                {
                    policys[i].Id = Guid.NewGuid().ToString();
                }
            }
            this.SetItems<RolePolicy>(builder.ToString(), policys);
        }

        public void SetRoleUsers(string roleId, List<UserRole> users)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("role_id = '{0}'", roleId);
            int count = users.Count;
            for (int i = 0; i < count; i++)
            {
                users[i].RoleId = roleId;
                if (StringUtils.IsNullOrEmptyOrWhiteSpace(users[i].Id))
                {
                    users[i].Id = Guid.NewGuid().ToString();
                }
            }
            this.SetItems<UserRole>(builder.ToString(), users);
        }

        public void SetUserRoles(string userId, List<UserRole> roles)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder.AppendFormat("user_id = '{0}'", userId);
            int count = roles.Count;
            for (int i = 0; i < count; i++)
            {
                roles[i].UserId = userId;
                if (StringUtils.IsNullOrEmptyOrWhiteSpace(roles[i].Id))
                {
                    roles[i].Id = Guid.NewGuid().ToString();
                }
            }
            this.SetItems<UserRole>(builder.ToString(), roles);
        }

        public void SetUserStatus(string userId, string status)
        {
            this.ValidateStatus(status, typeof(UserStatus));
            User userById = this.GetUserById(userId, false);
            if (userById == null)
            {
                throw new ObjectNotFoundException<User>("UE01-002", "User Not Found");
            }
            userById.Status = status;
            if ("ACTIVE".Equals(status))
            {
                userById.LoginFailCount = 0;
            }
            this.op.Update(userById);
        }

        public void UpdateOperation(Operation operation)
        {
            Operation operation2 = new Operation();
            if (!this.op.Retrieve(operation2, "id", operation.Id))
            {
                throw new ObjectNotFoundException<Operation>("UE01-005", "Operation not found");
            }
            operation2.Name = operation.Name;
            operation2.Code = operation.Code;
            this.op.Update(operation2);
        }

        public void UpdateOperationType(OperationType operationType)
        {
            OperationType des = this.GetOperationType(operationType.Id);
            if (des == null)
            {
                throw new ObjectNotFoundException<OperationType>("UE01-006", "Operation type not found!");
            }
            A.Commons.Utils.CloneUtils.CloneObjectIgnoreId(operationType, des);
            this.op.Update(des);
        }

        public void UpdateOrganization(Organization org)
        {
            Organization organization = this.GetOrganization(org.Id, true);
            organization.Name = org.Name;
            organization.Domain = org.Domain;
            organization.Serial = org.Serial;
            this.op.Update(organization);
        }

        public void UpdateRole(Role role)
        {
            Role des = this.op.Retrieve<Role>("id", role.Id);
            string id = des.Id;
            if (!des.Name.Equals(role.Name))
            {
                SqlStringBuilder builder = new SqlStringBuilder();
                builder.AppendFormat("invalid = 0 and name = '{0}' and application_code = '{1}'", role.Name, role.ApplicationCode);
                if (this.op.Retrieve<Role>(builder.ToString()) != null)
                {
                    throw new ObjectAlreadyExistException<Role>("UE02-006", "Role Name already exists");
                }
            }
            A.Commons.Utils.CloneUtils.CloneObjectIgnoreId(role, des);
            this.op.Update(des);
        }

        public void UpdateUser(User user)
        {
            User userById = this.GetUserById(user.Id, false);
            if (userById == null)
            {
                throw new ObjectNotFoundException<User>("UE01-002", "User Not Found");
            }
            A.Commons.Utils.CloneUtils.CloneObject(user, userById, new string[] { "Id", "Username", "Password", "Invalid", "Status" });
            this.op.Update(userById);
        }

        private void ValidateStatus(string status, Type type)
        {
            bool flag = false;
            foreach (FieldInfo info in type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static))
            {
                string str = info.GetValue(null) as string;
                if (status.Equals(str))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                throw new InvalidStatusException("Unknown status: " + status);
            }
        }

        public IObjectOperator ObjectOperator
        {
            get
            {
                return this.op;
            }
        }

        public IPasswordEncryptor PasswordEncryptor
        {
            get
            {
                if (this._passwordEncryptor == null)
                {
                    return new PlainTextPasswordEncryptor();
                }
                return this._passwordEncryptor;
            }
            set
            {
                this._passwordEncryptor = value;
            }
        }
    }
}

