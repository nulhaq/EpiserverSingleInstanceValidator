using System;

namespace Foundation.Cms.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleInstancesAttribute : Attribute
    {
        public enum InstanceScope
        {
            Site,
            SameContentTree,
        }
        public InstanceScope Scope { get; set; }
    }
}
