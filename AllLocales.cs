using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace Logix5000
{
    class AllLocales
    {
        public class CultureInfoComparer : IComparer<CultureInfo>
        {
            public int Compare(CultureInfo x, CultureInfo y)
            {
                if (x == null)
                    if (y == null)
                        return 0;
                    else
                        return -1;
                else if (y == null)
                    return 1;

                int diff = StringComparer.InvariantCultureIgnoreCase.Compare(x.Parent.Name, y.Parent.Name);
                if (diff == 0)
                    diff = StringComparer.InvariantCultureIgnoreCase.Compare(x.Name, y.Name);
                if (diff == 0)
                    diff = x.LCID - y.LCID;
                return diff;
            }
        }

        static void Main(string[] args)
        {
            using (StreamWriter sw = new StreamWriter("AllLocales.txt", false, Encoding.UTF8))
            {
                sw.WriteLine("#{0}\t{1}\t{2}\t{3}\t{4}\t{5}", "Name", "Parent", "LCID", "EnglishName", "NativeName", "CultureTypes");
                WriteLocales(sw, CultureInfo.GetCultures(CultureTypes.FrameworkCultures));

                sw.WriteLine();
                sw.WriteLine("#These locales depend on the user's environment (i.e. what languages are installed)");
                WriteLocales(sw, CultureInfo.GetCultures(CultureTypes.WindowsOnlyCultures));

                sw.Close();
            }
        }

        private static void WriteLocales(StreamWriter sw, CultureInfo[] cultures)
        {
            List<CultureInfo> frameworkCultures = new List<CultureInfo>();
            foreach (CultureInfo ci in cultures)
            {
                frameworkCultures.Add(ci);
                CultureInfo parent = ci.Parent;
                while (parent != CultureInfo.InvariantCulture)
                {
                    frameworkCultures.Add(parent);
                    parent = parent.Parent;
                }
            }
            
            frameworkCultures.Sort(new CultureInfoComparer());
            CultureInfo prev = null;
            foreach (CultureInfo ci in frameworkCultures)
            {
                if (prev != null && prev.Name == ci.Name)
                    continue;
                prev = ci;

                sw.WriteLine("{0}\t{1}\t0x{2:X4}\t\"{3}\"\t\"{4}\"\t\"{5}\"",
                    ci.Name.ToLowerInvariant(),
                    ci.Parent.Name.ToLowerInvariant(),
                    ci.LCID,
                    ci.EnglishName,
                    ci.NativeName,
                    ci.CultureTypes & (CultureTypes.FrameworkCultures | 
                                        CultureTypes.WindowsOnlyCultures | 
                                        CultureTypes.NeutralCultures | 
                                        CultureTypes.SpecificCultures));
            }
        }
    }
}
