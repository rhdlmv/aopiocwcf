using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;

namespace EgoalTech.DB
{
    [DataContract]
    public abstract class DbObject : IStorageObject, INotifyPropertyChanged
    {
        private Dictionary<string, object> _values = new Dictionary<string, object>();
        private List<string> notifies = new List<string>();
        private DbObjectState previousState;
        private DbObjectState state = new DbObjectState();
        private bool suspendNotify;

        public event PropertyChangedEventHandler PropertyChanged;

        public DbObject()
        {
            this.state.UpdateInitialValues(this);
            this.PropertyChanged += new PropertyChangedEventHandler(this.OnPropertyChanged);
        }

        public virtual T Clone<T>() where T : DbObject, new()
        {
            T local = DbObjectTools.Clone<T>(this);
            CloneUtils.CloneObject<DbObjectState>(this.State, local.State, new Expression<Func<DbObjectState, object>>[0]);
            CloneUtils.CloneObject<DbObjectState>(this.previousState, local.previousState, new Expression<Func<DbObjectState, object>>[0]);
            return local;
        }

        public virtual void Clone(DbObject srcObj, bool notifyChanged = true)
        {
            if (!notifyChanged)
            {
                this.SuspendNotify();
            }
            CloneUtils.CloneObject<DbObject>(srcObj, this, new Expression<Func<DbObject, object>>[0]);
            CloneUtils.CloneObject<DbObjectState>(this.State, this.State, new Expression<Func<DbObjectState, object>>[0]);
            CloneUtils.CloneObject<DbObjectState>(this.previousState, this.previousState, new Expression<Func<DbObjectState, object>>[0]);
            if (!notifyChanged)
            {
                this.ResumeNotify();
            }
        }

        public virtual bool Compare(DbObject obj)
        {
            return DbObjectTools.Compare(this, obj);
        }

        public object GetValue(string propertyName)
        {
            object obj2 = null;
            this.Values.TryGetValue(propertyName, out obj2);
            return obj2;
        }

        public T GetValue<T>(string propertyName)
        {
            if (this.Values.ContainsKey(propertyName))
            {
                return (T)this.GetValue(propertyName);
            }
            return default(T);
        }

        protected void NotifyPropertyChanged(string name)
        {
            if (!this.suspendNotify)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
            else
            {
                this.notifies.Add(name);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object obj2 = this.GetValue(e.PropertyName);
            this.State.PropertyChanged(e.PropertyName, obj2);
        }

        public virtual void OnRead(object sender, DbEventArgs e)
        {
            this.State.ObjectRead = true;
            this.State.ModifiedValues.Clear();
            this.State.UpdateInitialValues(this);
        }

        private void OnTranscationCommit(object sender, EventArgs e)
        {
            DbObjectOperator @operator = sender as DbObjectOperator;
            @operator.TranscationCommit -= new EventHandler(this.OnTranscationCommit);
            this.previousState = null;
        }

        private void OnTranscationRollback(object sender, EventArgs e)
        {
            DbObjectOperator @operator = sender as DbObjectOperator;
            @operator.TranscationRollback -= new EventHandler(this.OnTranscationRollback);
            if (this.previousState != null)
            {
                CloneUtils.CloneObject<DbObjectState>(this.previousState, this.State, new Expression<Func<DbObjectState, object>>[0]);
            }
        }

        public virtual void OnWrote(object sender, DbEventArgs e)
        {
            this.State.ObjectWrote = true;
            this.State.ModifiedValues.Clear();
            this.State.UpdateInitialValues(this);
            this.previousState = DbObjectTools.Clone<DbObjectState>(this.State);
            e.Operator.TranscationRollback -= new EventHandler(this.OnTranscationRollback);
            e.Operator.TranscationRollback += new EventHandler(this.OnTranscationRollback);
            e.Operator.TranscationCommit -= new EventHandler(this.OnTranscationCommit);
            e.Operator.TranscationCommit += new EventHandler(this.OnTranscationCommit);
        }

        public virtual void ReflushSuspendNotifies()
        {
            if (!this.suspendNotify)
            {
                int count = this.notifies.Count;
                for (int i = 0; i < count; i++)
                {
                    this.NotifyPropertyChanged(this.notifies[i]);
                }
                this.notifies.Clear();
            }
        }

        public void ResumeNotify()
        {
            this.suspendNotify = false;
        }

        public void SetValue(string propertyName, object value)
        {
            if (this.Values.ContainsKey(propertyName))
            {
                this.Values[propertyName] = value;
            }
            else
            {
                this.Values.Add(propertyName, value);
            }
        }

        public void SetValue<T>(string propertyName, T value)
        {
            this.SetValue(propertyName, value);
        }

        public void SuspendNotify()
        {
            this.suspendNotify = true;
        }

        public bool IsModified
        {
            get
            {
                return this.State.IsModified;
            }
        }

        public bool IsNewObject
        {
            get
            {
                return this.State.IsNewObject;
            }
        }

        public Dictionary<string, ValueInfo> ModifiedValues
        {
            get
            {
                return this.State.ModifiedValues;
            }
        }

        public bool ObjectRead
        {
            get
            {
                return this.State.ObjectRead;
            }
        }

        public bool ObjectWrote
        {
            get
            {
                return this.State.ObjectWrote;
            }
        }

        private DbObjectState State
        {
            get
            {
                if (this.state == null)
                {
                    this.state = new DbObjectState();
                }
                return this.state;
            }
        }

        private Dictionary<string, object> Values
        {
            get
            {
                if (this._values == null)
                {
                    this._values = new Dictionary<string, object>();
                }
                return this._values;
            }
            set
            {
                this._values = value;
            }
        }
    }
}

