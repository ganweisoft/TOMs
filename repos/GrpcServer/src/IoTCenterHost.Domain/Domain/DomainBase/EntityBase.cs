//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenterHost.AppServices.Domain.DomainBase
{
    public abstract class EntityBase : IEntity
    {
        #region 私有字段
        private object key;
        #endregion

        #region 构造方法
        protected EntityBase()
            : this(null)
        {
        }

        protected EntityBase(object key)
        {
            this.key = key;
            if (this.key == null)
            {
                this.key = EntityBase.NewKey();
            }
        }
        #endregion

        #region 属性
        [NotMapped]
        public object Key
        {
            get
            {
                return this.key;
            }
            internal set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Key", "The Key property cannot be set to null.");
                }
                this.key = value;
            }
        }
        #endregion

        #region 方法 
        public static object NewKey()
        {
            return Guid.NewGuid();
        }
        #endregion

        #region
        public override bool Equals(object entity)
        {
            return entity != null
                && entity is EntityBase
                && this == (EntityBase)entity;
        }

        public static bool operator ==(EntityBase base1,
            EntityBase base2)
        {
            if ((object)base1 == null && (object)base2 == null)
            {
                return true;
            }

            if ((object)base1 == null || (object)base2 == null)
            {
                return false;
            }

            if (!base1.Key.Equals(base2.Key))
            {
                return false;
            }

            return true;
        }

        public static bool operator !=(EntityBase base1,
            EntityBase base2)
        {
            return (!(base1 == base2));
        }

        public override int GetHashCode()
        {
            if (this.key != null)
            {
                return this.key.GetHashCode();
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}