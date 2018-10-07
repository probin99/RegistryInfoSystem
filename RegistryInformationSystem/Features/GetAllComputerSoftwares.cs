using RegistryInformationSystem.Context;
using RegistryInformationSystem.Models.Software;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace RegistryInformationSystem.Features
{
    public class GetAllComputerSoftwares
    {
        //WMI Query Registry for software details
        public int ReadRegistryForSoftwareWMI()
        {
            ComputerSoftware computerSoftwares = new ComputerSoftware();
            int rtn = -1;
            try
            {
                using (var db = new DatabaseContext())
                {
                    const int REG_SZ = 1;
                    const int REG_EXPAND_SZ = 2;
                    const int REG_BINARY = 3;
                    const int REG_DWORD = 4;
                    const int REG_MULTI_SZ = 7;
                    const int REG_QWORD = 11;
                    var queryDBSoftware = db.ComputerSoftwares.Select(x => x.Name);
                    List<int> softwareBits = new List<int> { 32, 64 };
                    foreach (int bit in softwareBits)
                    {
                        ManagementNamedValueCollection mContext = new ManagementNamedValueCollection
                        {
                            { "__ProviderArchitecture", bit },
                            { "__RequiredArchitecture", true }
                        };

                        ConnectionOptions connectionOptions = new ConnectionOptions
                        {
                            Impersonation = ImpersonationLevel.Impersonate,
                            Context = mContext
                        };

                        ManagementScope scope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\CIMV2", connectionOptions);
                        scope.Connect();

                        string softwareRegLoc = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";

                        ManagementClass registry = new ManagementClass(scope, new ManagementPath("StdRegProv"), null);
                        ManagementBaseObject inParams = registry.GetMethodParameters("EnumKey");
                        inParams["hDefKey"] = 0x80000002;//HKEY_LOCAL_MACHINE
                        inParams["sSubKeyName"] = softwareRegLoc;

                        // Read Registry Key Names 
                        ManagementBaseObject outParams = registry.InvokeMethod("EnumKey", inParams, null);
                        string[] softwareGuids = outParams["sNames"] as string[];

                        foreach (string subKeyName in softwareGuids)
                        {
                            inParams = registry.GetMethodParameters("EnumValues");
                            inParams["hDefKey"] = 0x80000002;//HKEY_LOCAL_MACHINE
                            inParams["sSubKeyName"] = softwareRegLoc + @"\" + subKeyName;

                            // Read Registry Value 
                            outParams = registry.InvokeMethod("EnumValues", inParams, null);
                            if (outParams.Properties["sNames"].Value != null
                                && outParams.Properties["Types"].Value != null)
                            {
                                //For Names
                                string[] softwareValueNames = (outParams.Properties["sNames"].Value) as string[];
                                int[] softwareValueTypes = (outParams.Properties["Types"].Value) as int[];                                

                                //For Types
                                for (int i = 0; i < softwareValueTypes.Count(); i++)
                                {
                                    int valueTypes = softwareValueTypes[i];
                                    string valueNames = softwareValueNames[i];
                                    string executionMethod;
                                    switch (valueTypes)
                                    {
                                        case REG_SZ:
                                            {
                                                executionMethod = "GetStringValue";
                                                //inParams = registry.GetMethodParameters("GetStringValue");
                                                break;
                                            }
                                        case REG_EXPAND_SZ:
                                            {
                                                executionMethod = "GetExpandedStringValue";
                                                break;
                                            }
                                        case REG_BINARY:
                                            {
                                                executionMethod = "GetBinaryValue";
                                                break;
                                            }
                                        case REG_DWORD:
                                            {
                                                executionMethod = "GetDWORDValue";
                                                break;
                                            }
                                        case REG_MULTI_SZ:
                                            {
                                                executionMethod = "GetMultiStringValue";
                                                break;
                                            }
                                        case REG_QWORD:
                                            {
                                                executionMethod = "GetQWORDValue";
                                                break;
                                            }
                                        default:
                                            {
                                                executionMethod = "Close";
                                                break;
                                            }
                                    }
                                    if (executionMethod != "Close")
                                    {
                                        inParams = registry.GetMethodParameters(executionMethod);
                                        inParams["hDefKey"] = 0x80000002;//HKEY_LOCAL_MACHINE
                                        inParams["sSubKeyName"] = softwareRegLoc + @"\" + subKeyName;
                                        inParams["sValueName"] = valueNames;
                                        outParams = registry.InvokeMethod(executionMethod, inParams, null);
                                        
                                        int softwareSystemComponent;                                            
                                        //Get values
                                        switch (valueNames)
                                        {
                                            case "DisplayName":
                                                {
                                                    computerSoftwares.Name = outParams.Properties["sValue"].Value.ToString();
                                                    break;
                                                }
                                            case "InstallLocation":
                                                {
                                                    computerSoftwares.InstalledLocation = outParams.Properties["sValue"].Value.ToString();
                                                    break;
                                                }
                                            case "DisplayVersion":
                                                {
                                                    computerSoftwares.Version = outParams.Properties["sValue"].Value.ToString();
                                                    break;
                                                }
                                            case "Publisher":
                                                {
                                                    computerSoftwares.Publisher = outParams.Properties["sValue"].Value.ToString();
                                                    break;
                                                }
                                            case "InstallDate":
                                                {
                                                    DateTime dt;
                                                    if (outParams.Properties["sValue"].Value.ToString() == null)
                                                    {
                                                        computerSoftwares.InstalledOn = Convert.ToDateTime("19900903");
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            if (DateTime.TryParseExact(outParams.Properties["sValue"].Value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                                            {
                                                                computerSoftwares.InstalledOn = dt;
                                                            }
                                                            else if (DateTime.TryParseExact(outParams.Properties["sValue"].Value.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                                            {
                                                                computerSoftwares.InstalledOn = dt;
                                                            }
                                                            else if (DateTime.TryParseExact(outParams.Properties["sValue"].Value.ToString(), "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                                            {
                                                                computerSoftwares.InstalledOn = dt;
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            throw ex;
                                                        }
                                                    }
                                                    break;
                                                }
                                            case "EstimatedSize":
                                                {
                                                    computerSoftwares.Size = Convert.ToInt32(outParams.Properties["uValue"].Value);
                                                    break;
                                                }
                                            case "SystemComponent":
                                                {                                                    
                                                    softwareSystemComponent = Convert.ToInt32(outParams.Properties["uValue"].Value);
                                                    if (softwareSystemComponent.Equals(""))
                                                    {
                                                        softwareSystemComponent = 1;
                                                    }
                                                    computerSoftwares.SystemComponent = softwareSystemComponent;
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                    }
                                }
                                
                                //Filter unwanted softwares
                                if (softwareValueNames.Contains("WindowsInstaller")
                                    && !softwareValueNames.Contains("ParentDisplayName")
                                    && !softwareValueNames.Contains("ParentDisplayName")
                                    && softwareValueNames.Contains("UninstallString"))
                                {
                                    var currentRegistryScan = computerSoftwares.Name.ToString();                                 
                                    if (!string.IsNullOrEmpty(currentRegistryScan)
                                        && char.IsUpper(currentRegistryScan[0])
                                        && !currentRegistryScan.Contains("-")
                                        && !currentRegistryScan.ToLower().Contains("pack")
                                        && !currentRegistryScan.ToLower().Contains("kb")
                                        && !currentRegistryScan.ToLower().Contains("update")
                                        && !queryDBSoftware.Contains(currentRegistryScan)
                                        && computerSoftwares.SystemComponent == 1)
                                    {
                                        computerSoftwares.Status = "Active";
                                        db.ComputerSoftwares.Add(computerSoftwares);
                                        db.SaveChanges();
                                    }
                                }                                
                            }
                        }
                    }
                    rtn = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rtn;
        }        
    }
}