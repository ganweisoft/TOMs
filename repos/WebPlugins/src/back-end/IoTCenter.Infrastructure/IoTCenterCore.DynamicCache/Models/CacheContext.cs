using System;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterCore.DynamicCache
{
    public class CacheContext
    {
        private HashSet<string> _contexts;
        private HashSet<string> _tags;

        public CacheContext(string cacheId)
        {
            CacheId = cacheId;
        }

        public CacheContext WithExpiryOn(DateTimeOffset expiry)
        {
            ExpiresOn = expiry;
            return this;
        }

        public CacheContext WithExpiryAfter(TimeSpan duration)
        {
            ExpiresAfter = duration;
            return this;
        }

        public CacheContext WithExpirySliding(TimeSpan window)
        {
            ExpiresSliding = window;
            return this;
        }

        public CacheContext AddContext(params string[] contexts)
        {
            if (_contexts == null)
            {
                _contexts = new HashSet<string>();
            }

            foreach (var context in contexts)
            {
                _contexts.Add(context);
            }

            return this;
        }

        public CacheContext RemoveContext(string context)
        {
            if (_contexts != null)
            {
                _contexts.Remove(context);
            }

            return this;
        }

        public CacheContext AddTag(params string[] tags)
        {
            if (_tags == null)
            {
                _tags = new HashSet<string>();
            }

            foreach (var tag in tags)
            {
                _tags.Add(tag);
            }

            return this;
        }

        public CacheContext RemoveTag(string tag)
        {
            if (_tags != null)
            {
                _tags.Remove(tag);
            }

            return this;
        }

        public string CacheId { get; }
        public ICollection<string> Contexts => (ICollection<string>)_contexts ?? Array.Empty<string>();
        public IEnumerable<string> Tags => _tags ?? Enumerable.Empty<string>();
        public DateTimeOffset? ExpiresOn { get; private set; }
        public TimeSpan? ExpiresAfter { get; private set; }
        public TimeSpan? ExpiresSliding { get; private set; }
    }
}
