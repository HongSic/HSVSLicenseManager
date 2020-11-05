using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using HSVSLicenseManager.Properties;
using Microsoft.Win32;

namespace HSVSLicenseManager
{
    //ProtectedData 클래스: https://docs.microsoft.com/ko-kr/dotnet/api/system.security.cryptography.protecteddata?view=dotnet-plat-ext-3.1

    //Trial period reset of Visual Studio Community Edition 8: https://dimitri.janczak.net/2019/07/13/trial-period-reset-of-visual-studio-community-edition/

    public partial class frmMain : Form
    {
        #region Utils
        private static string GetStringGlobalResource(string Key) { try { return Resources.ResourceManager.GetString(Key, Resources.Culture); } catch { return null; } }
        private static string[] SeprateComma(string str) { if (str != null) return str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); else return null; }
        private static bool CheckArray<T>(T[] Array) { return Array != null && Array.Length > 0; }
        #endregion

        Dictionary<string, string[]> VSKind = new Dictionary<string, string[]>();
        RegistryKey regkey_root;
        public frmMain()
        {
            InitializeComponent();


            regkey_root = Registry.ClassesRoot.OpenSubKey("Licenses");
            chkVSInstalled_CheckedChanged(null, null);
        }
        private void chkVSInstalled_CheckedChanged(object sender, EventArgs e)
        {
            cbVSVersion.Enabled = false;
            cbVSVersion.Items.Clear();

            if (chkVSInstalled.Checked)
            {
                foreach (var regkey_str in SeprateComma(GetStringGlobalResource("VS_REG_KEY")))
                {
                    var regkey = regkey_root.OpenSubKey(regkey_str);
                    if (regkey != null)
                    {
                        var vsver = GetStringGlobalResource("VS_REG_KEY_" + regkey_str);
                        cbVSVersion.Items.Add(new VSVersionItem(vsver, regkey_str, GetStringGlobalResource(vsver + "_Name"), regkey));
                    }
                }

                if (cbVSVersion.Items.Count > 0)
                {
                    cbVSVersion.Enabled = true;
                    cbVSVersion.SelectedIndex = cbVSVersion.Items.Count - 1;
                    return;
                }

                cbVSVersion.Items.Add("(No Visual Studio)");
                cbVSVersion.SelectedIndex = 0;
            }
            else
            {
                /*
                var vers = SeprateComma(GetStringGlobalResource("VSVersion"));
                if (CheckArray(vers))
                {
                    foreach (var ver in vers)
                    {
                        var vskinds = SeprateComma(GetStringGlobalResource(ver));
                        if (CheckArray(vskinds))
                        {
                            VSKind.Add(ver, vskinds);
                            cbVSVersion.Items.Add(new VSVersionItem(ver, GetStringGlobalResource(ver + "_Name")));
                        }
                    }
                }
                */
            }
        }

        private void cbVSVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbVSKind.Enabled = false;
            cbVSKind.Items.Clear();

            var veritem_root = cbVSVersion.Items[cbVSVersion.SelectedIndex] as VSVersionItem;
            if(veritem_root != null)
            {
                foreach (var vskind_str in SeprateComma(GetStringGlobalResource("VS_REG_SUB_" + veritem_root.Version)))
                {
                    var vskind_reg = veritem_root.Root.OpenSubKey(vskind_str);
                    if (vskind_reg != null)
                    {
                        string vskind_name = GetStringGlobalResource(string.Format("VS_REG_SUB_{0}_{1}", veritem_root.Version, vskind_str));
                        cbVSKind.Items.Add(new VSKindItem(veritem_root, vskind_str, vskind_name, vskind_reg));
                    }
                }

                if (cbVSKind.Items.Count > 0)
                {
                    cbVSKind.Enabled = true;
                    cbVSKind.SelectedIndex = cbVSKind.Items.Count - 1;
                    return;
                }
            }


            cbVSKind.Items.Add("(No VS Kind)");
            cbVSKind.SelectedIndex = 0;
        }

        private void cbVSKind_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item_root = cbVSKind.Items[cbVSKind.SelectedIndex] as VSKindItem;
            if (item_root != null)
            {
                using (var reg_item = item_root.Root)
                {
                    var data_enc = reg_item.GetValue(null) as byte[];
                    var data = ProtectedData.Unprotect(data_enc, null, DataProtectionScope.CurrentUser);
                    File.WriteAllBytes("D:\\key.bin", data);
                }  
            }
        }
    }
}
