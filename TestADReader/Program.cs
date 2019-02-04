using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;


namespace TestADReader
{
    class Program
    {
        static void DirectorySearch(string AFilter)
        {
            for (int i = 0; i < 10; i++)
            {
                uint count = 0;
                string queryString = AFilter;            
                DirectoryEntry entry = new DirectoryEntry();
                DirectorySearcher search = new DirectorySearcher(entry, queryString);
                search.PageSize = 1000;
                foreach (SearchResult result in search.FindAll())
                {
                    DirectoryEntry user = result.GetDirectoryEntry();
                    if (user != null)
                    {
                        Console.WriteLine("GUID : " + user.Guid.ToString());
                        Console.WriteLine("NativeGuid : " + user.NativeGuid.ToString());
                        Console.WriteLine("Path : " + user.Path.ToString());
                        Console.WriteLine("----------------------------------------------");
                        user.RefreshCache(new string[] { "canonicalName" });
                        string canonicalName = user.Properties["canonicalName"].Value.ToString();
                        count++;
                    }
                }
            }

        }
        static void PrincipialSearch(string AFilter)
        {
            string lSearchString = AFilter;
            using (var context = new PrincipalContext(ContextType.Domain, "xxxx.xxxx.xxxx"))
            {
                using (var searcher = new PrincipalSearcher(new GroupPrincipal(context)))
                {
                    //PrincipalSearcher QueryFilter
                    foreach (var result in searcher.FindAll())
                    {
                        DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                        if (de.Properties["distinguishedName"].Value.ToString().Contains(lSearchString) || lSearchString!="")
                        {                            
                            Console.WriteLine("GUID : " + de.Guid.ToString());
                            Console.WriteLine("NativeGuid : " + de.NativeGuid.ToString());
                            Console.WriteLine("Path : " + de.Path.ToString());
                            
                            foreach (string PropertyName in de.Properties.PropertyNames)
                            {
                                if (!(de.Properties[PropertyName].Value is System.Byte[]))
                                {
                                    Console.WriteLine(PropertyName + ": " + de.Properties[PropertyName].Value.ToString());
                                }
                                else
                                {
                                    if (PropertyName == "objectSid")
                                    {
                                        Console.WriteLine(PropertyName + ": " + de.Properties[PropertyName].Value.ToString());
                                        byte[] lArray = (byte[])de.Properties[PropertyName].Value;
                                        var sid = new SecurityIdentifier(lArray, 0);
                                        sid.ToString();
                                        Console.WriteLine(PropertyName + ": " + sid.ToString());
                                    }
                                }
                            }
                            Console.WriteLine("------------------------------------------------------");
                        }
                        
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            PrincipialSearch("");
            PrincipialSearch("OU=xxxxx,");
            DirectorySearch("(&(objectClass=user)(objectClass=person)(!objectClass=computer))");
            DirectorySearch("(&(objectClass=user)(objectClass=person)(objectClass=computer))");
            Console.ReadLine();
        }
    }
}
