using System;
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace AutoMagazineUpdatePatch
{
    class FuncoesUtilitarias
    {
        string sSis3264 = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
        string sInstallPath32 = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\SCE\SCEExec\AutoMagazine", "NetworkDir", null);
        string sInstallPath64 = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\SCE\SCEExec\AutoMagazine", "NetworkDir", null);
        string sWindows = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", null);
        string sServicePack = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion", null);
        string sProcessador = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
        string computerName = Environment.MachineName;
        string OSVersion = Environment.OSVersion.VersionString;
        string ddd = Environment.ProcessorCount.ToString();
        static string pastaNetwork;
        static IniFile _ini41 = new IniFile(pastaNetwork + "AutoMagazine.VSce");
        static string VerAtiva = _ini41.ReadValue("SCE", "ActiveProjectVersion");
        static IniFile _iniCurrent = new IniFile(pastaNetwork + VerAtiva + "\\Support\\Automagazine.isce");
        LogAMG Log = new LogAMG();

        /// <summary>Adiciona permissões (Todos + FullControl) na pasta passada por parâmetro</summary>
        /// <param name="fullPath">string fullPath</param>
        /// <returns>Boolean</returns>
        private bool GrantAccess(string fullPath)
        {
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(fullPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                Log.Log("Adicionando permissões a pasta Network " + dSecurity.ToString());
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, AccessControlType.Allow));
                //dInfo.SetAccessControl(dSecurity); demora muito se feito pela rede
                Log.Log("Permissões adicionadas em : " + fullPath);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Log("Erro ao atribuir permissões a pasta Network em: " + fullPath);
                return false;
            }
        }

        /// <summary>
        /// Checa se o sistema é x86 ou 64 bits, também é definido o caminho da pasta Network.
        /// </summary>
        /// <returns>boolean</returns>
        public bool Is64bits()
        {

            try
            {
                if (IntPtr.Size * 8 == 32)
                {
                    if (sInstallPath32.Substring(sInstallPath32.Length - 1) != @"\")
                    {
                        sInstallPath32 += @"\";                        
                    }
                    pastaNetwork = sInstallPath32;
                    return false;
                }
                else
                {
                    if (sInstallPath64.Substring(sInstallPath64.Length - 1) != @"\")
                    {
                        sInstallPath64 += @"\";                        
                    }
                    pastaNetwork = sInstallPath64;
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetFrameworkVersion()
        {
            string aux = "Version";
            string sFrameworkVersion_20 = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727", aux, null);
            string sFrameworkVersion_30 = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0", aux, null);
            string sFrameworkVersion_35 = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5", aux, null);
            string sFrameworkVersion_40 = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", aux, null);

            if (sFrameworkVersion_20 != null && sFrameworkVersion_30 != null && sFrameworkVersion_35 != null)
            {
                return sFrameworkVersion_35.Substring(0,3);
            }

            return sFrameworkVersion_20.Substring(0,3);
        }

        /// <summary>Devolve a letra da Build caso exista </summary>
        public string ProjectVersionBuild
        {
            get { return _iniCurrent.ReadValue("SCE", "ProjectVersionBuild"); }
        }

        /// <summary>Devolve a versão do AutoMagazine, exemplo: 4.4.20</summary>
        public string ProjectVersion
        {
            get { return _iniCurrent.ReadValue("SCE", "ProjectVersion"); }
        }

        /// <summary>Devolve a pasta da versão ativa, exemplo: 4.0, 4.1, 4.3, etc... </summary>
        public string VersaoAtiva
        {
           get { return _ini41.ReadValue("SCE", "ActiveProjectVersion"); }
        }
        
        /// <summary>Devolve o nome do produto, exemplo: Windows 7 ultimate edition</summary>
        public string Versao_do_Windows
        {
            get { return sWindows; }
        }

        /// <summary>Devolve o nome do ServicePack</summary>
        public string Windows_ServicePack
        {
            get { return sServicePack; }
        }

        /// <summary>Devolve o nome do processador e a quantidade de Nucleos</summary>
        public string Nome_do_Processador
        {
            get { return sProcessador + " " + ddd + " Nucleos"; }
        }

        /// <summary>Devolve o nome do computador</summary>
        public string Nome_do_Computador
        {
            get { return computerName; }
        }

        /// <summary>Devolve a pasta NetworkDir caso o AutoMagazine esteja instalado</summary>
        /// <param>parametros do networkdir1</param>
        /// <param>parametros do networkdir2</param>
        public string NetworkDir
        {
            get
            {
                if (Is64bits())
                {
                    return pastaNetwork;
                }
                else
                {
                    return pastaNetwork;
                }
            }
        }

    }
}
