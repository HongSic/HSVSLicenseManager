using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSVSLicenseManager
{
    public class VSVersionItem : IDisposable
    {
        public VSVersionItem(string Version, string Key, string Name, RegistryKey Root) { this.Version = Version; this.Key = Key; this.Name = Name; this.Root = Root; }
        public string Version { get; }
        public string Key { get; }
        public string Name { get; }

        public RegistryKey Root { get; }

        public void Dispose() {  Root.Close(); }

        public override string ToString() { return Name; }

        ~VSVersionItem() { Dispose(); }
    }

    public class VSKindItem : IDisposable
    {
        public VSKindItem(VSVersionItem Parent, string Key, string Name, RegistryKey Root) { this.Parent = Parent; this.Key = Key; this.Name = Name; this.Root = Root; }
        public VSVersionItem Parent { get; }
        public string Key { get; }
        public string Name { get; }

        public RegistryKey Root { get; }

        public void Dispose() { Root.Close(); }

        public override string ToString() { return Name; }

        ~VSKindItem() { Dispose(); }
    }

    public class VSLicenseRegData
    {
        public VSLicenseRegData()
        {

        }
        public string Version { get; set; }
        public string Kind { get; set; }

        public string RegKey { get; set; }
        public string RegValue { get; set; }
    }
}
