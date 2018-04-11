using System;
namespace Capibara.Attributes
{
    [AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = true)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public DisplayNameAttribute(string displayName)
        {
            this.DisplayName = displayName;
        }
    }
}
