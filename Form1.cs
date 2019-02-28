using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using Ionic.Zip;
using System.Threading;
using System.Security.AccessControl;

namespace AutoMagazineUpdatePatch
{
    /* Carlos Rodrigues Batista 01-05-2015
     * Prezados eu sei que ninguem tem tempo para fazer este patch seguindo as boas práticas mas aqui vai umas dicas.
     * 
     * Para saber se um sistema é 64 ou 32bits, basta utilizar o "IntPtr.Size"
     * if(IntPtr.Size * 8 == 32) "32bits"
     * if(IntPtr.Size * 8 == 64) "64bits"
     * 
     * não precisa usar um metodo "is3264()", a palavra "is" já indica que o método é um boolean, mas vocês fizeram um "void"
     * ao fazer uma aplicação que vai ler do registro do windows deve-se pedir elevação ao sistema operacional.
     * 
     * façam isso :
     * 
     * public bool isAdmin()
     * {
     *      WindowsIdentity login = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(login);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
     * }
    

    */

    public partial class Form1 : Form
    {
        private GifImage gifImage = null;
        private string filePath = @"loading-gear.gif"; //por enquanto estou utilizando uma imagem externa para fazer a animação junto com a progressBar
        static Random rnd = new Random();
        Color randomcolor = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));       
        string sUpdatedVersion = "4.4.21b"; //Versão da build, vou mudar a versão da build manualmente por causa do tempo.
        LogAMG Log = new LogAMG();
        
        public Form1()
        {
            InitializeComponent();
            FuncoesUtilitarias ofunc = new FuncoesUtilitarias(); // ************* NÃO FAZ NADA -  NÃO MEXER *****************
            button2.Visible = false; //COLOCAR VISIBLE FALSE
            button1.Visible = true;
            gifImage = new GifImage(filePath);
            gifImage.ReverseAtEnd = false; //Não reverter a imagem quando chegar ao final
            pictureBox2.Image = gifImage.GetFrame(0);

            label8.Text = "v"+ ofunc.GetFrameworkVersion();
            label9.Text = sUpdatedVersion;
            
            
            
            progressBar1.BringToFront();            
            progressBar1.Visible = false;
            pictureBox2.Visible = false;
            this.Text = "Atualizador AutoMagazine "+sUpdatedVersion;

            Log.Log("-----------------------------------------");
            
            //Se eu não tratar a existencia das dependências o instalador dá crash, fiz uma simples checagem para saber se existe os arquivos.
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "loading-gear.gif") || !File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Ionic.Zip.dll"))
            {
                string msg1 = "Não foram encontrados os seguintes arquivos:\n";
                msg1 += "loading-gear.gif\n";
                msg1 += "Ionic.Zip.dll\n";
                msg1 += "operação cancelada.\n";

                MessageBox.Show(msg1,"Gerenciador do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
                
            }
            else
            {
                Application.DoEvents();
                is3264();
            }
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        //LogAMG log = new LogAMG(" ------ Log_Inicio ------");
        // PUBLICAS
        
        string sSis3264 = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
        string sInstallPath32 = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\SCE\SCEExec\AutoMagazine", "NetworkDir", null);
        string sInstallPath64 = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\SCE\SCEExec\AutoMagazine", "NetworkDir", null);
        string sWindows = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", null);
        string sFrameworkVersion = Environment.Version.ToString(); //(string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion", null);
        string sProcessador = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", null);
        string computerName = Environment.MachineName;
        string OSVersion = Environment.OSVersion.VersionString;
        string ddd = Environment.ProcessorCount.ToString();
        long fff = Environment.WorkingSet / 1024;
        
        string sVersion = "";
        string sUpdateFile = "AMUpdate.zip";
               
        byte[] oUpdateFile = Properties.Resources.AMUpdate;

        
        // Oficiais
        
        private void is3264()
        {            
            //string[] linhas = File.ReadAllLines(sInstallPath32 + "4.1\\Support\\Automagazine.isce");
            try
            {
                if ((IntPtr.Size * 8) == 32)
                {
                    textBox1.Text = sInstallPath32;
                    //obtendo o ultimo caracter do caminho NetworkDir e checando para saber se é "\"
                    if (sInstallPath32 != null)
                    {
                        if (textBox1.Text.Substring(textBox1.Text.Length - 1) != @"\")
                            textBox1.Text += @"\";
                    }

                    label5.Text = sWindows.ToString() + " (32bits)";
                    
                    IniFile _ini41 = new IniFile(textBox1.Text + "AutoMagazine.VSce");
                    string ActiveProjectVersion = _ini41.ReadValue("SCE", "ActiveProjectVersion");
                    //Aqui é onde faço a leitura do arquivo "Automagazine.isce", escrita ou leitura
                    IniFile _iniCurrent = new IniFile(textBox1.Text + ActiveProjectVersion + "\\Support\\Automagazine.isce");
                    string ProjectVersion = _iniCurrent.ReadValue("SCE", "ProjectVersion");
                    string ProjectVersionBuild = _iniCurrent.ReadValue("SCE", "ProjectVersionBuild");

                    Log.Log("Versão instalada " + ProjectVersion + ProjectVersionBuild);
                    
                    /*///////////////////// Fiz essa parte para liberar um patch compativel com todas as versões
                    ////////////////////// para adicionar no script do AMG referencia para as Dlls da EPSON (sem OPOS)
                    string PDV = _iniCurrent.ReadValue("PDV2004.Files", "");
                    int count = 100;
                    string PDV111 = _iniCurrent.ReadValue("PDV2004.Files", (count).ToString());

                    if (PDV == "")
                    {
                        while (_iniCurrent.ReadValue("PDV2004.Files", count.ToString()) != "")
                        {
                            count += 1;
                        }

                        if (_iniCurrent.ReadValue("PDV2004.Files", (count).ToString()) == "" && _iniCurrent.ReadValue("PDV2004.Files", (count - 1).ToString()) != "ISCENFiscalEpsonDrv.dll")
                        {
                            _iniCurrent.WriteValue("PDV2004.Files", "", null);// Apagando o sinal de igual(=) que fica antes ou depois de cada chave
                            
                            // escrever aqui dentro as novas dlls que o pdv irá carregar
                            _iniCurrent.WriteValue("PDV2004.Files", (count).ToString(), "InterfaceEpsonNF.dll");
                            count += 1;
                            _iniCurrent.WriteValue("PDV2004.Files", (count).ToString(), "ISCENFiscalEpsonDrv.dll");
                            count += 1;
                            Thread.Sleep(2000);
                            Application.DoEvents();
                            _iniCurrent.WriteValue("PDV2004.Files", "", "");//encerra a adição de dlls e coloca o maldito (=) na linha abaixo

                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "SourceDir", "\\ImpressoraNaoFiscais");
                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "DestDir", "<SysDir>\\");
                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "FileType", "File");
                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "ValidationType", "FileVersion");
                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "SCEVersion", "1.5.0.0");

                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "", "");//encerra a adição de dlls e coloca o maldito (=) na linha abaixo

                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "SourceDir", "\\ImpressoraNaoFiscais");
                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "DestDir", "<SysDir>\\");
                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "FileType", "Component");
                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "ValidationType", "FileVersion");
                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "SCEVersion", "1.0.0.23");

                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "", "");//encerra a adição de dlls e coloca o maldito (=) na linha abaixo
                        }
                        
                    }

                    *////////////////////// FIM

                    label10.Text = ProjectVersion + " " + ProjectVersionBuild;

                    if (sInstallPath32 == null)
                    {
                        MessageBox.Show("Não há AutoMagazine instalado no computador", "Gerenciador de erros", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        button1.Enabled = false;
                        button1.ForeColor = Color.Red;
                        textBox1.ForeColor = Color.Red;
                        textBox1.Text = "A instalação do AutoMagazine não foi detectada.";
                        Log.Log("A instalação do AutoMagazine não foi detectada. " + ProjectVersion + ProjectVersionBuild);
                    }

                    /* implementar a leitura da versão anterior atravez do arquivo "*.ISCE"
                    foreach (string line in linhas)
                    {
                        //if(line.Equals("ProjectVersion"))
                            
                    }
                     */
                    

                }
                else if ((IntPtr.Size * 8) == 64)
                {

                    textBox1.Text = sInstallPath64;
                    //obtendo o ultimo caracter do caminho NetworkDir e checando para saber se é "\"
                    if (sInstallPath64 != null)
                    {
                        if (textBox1.Text.Substring(textBox1.Text.Length - 1) != @"\")
                            textBox1.Text += @"\";
                    }

                    label5.Text = sWindows.ToString() + " (64bits)";

                    IniFile _ini41 = new IniFile(textBox1.Text + "AutoMagazine.VSce");
                    string ActiveProjectVersion = _ini41.ReadValue("SCE", "ActiveProjectVersion");
                    IniFile _iniCurrent = new IniFile(textBox1.Text + ActiveProjectVersion + "\\Support\\Automagazine.isce");
                    string ProjectVersion = _iniCurrent.ReadValue("SCE", "ProjectVersion");
                    string ProjectVersionBuild = _iniCurrent.ReadValue("SCE", "ProjectVersionBuild");

                    /*///////////////////// Fiz essa parte para liberar um patch compativel com todas as versões
                    ////////////////////// para adicionar no script do AMG referencia para as Dlls da EPSON (sem OPOS)
                    string PDV = _iniCurrent.ReadValue("PDV2004.Files", "");
                    int count = 100;
                    string PDV111 = _iniCurrent.ReadValue("PDV2004.Files", (count).ToString());

                    if (PDV == "")
                    {
                        while (_iniCurrent.ReadValue("PDV2004.Files", count.ToString()) != "")
                        {
                            count += 1;
                        }

                        if (_iniCurrent.ReadValue("PDV2004.Files", (count).ToString()) == "" && _iniCurrent.ReadValue("PDV2004.Files", (count - 1).ToString()) != "ISCENFiscalEpsonDrv.dll")
                        {
                            _iniCurrent.WriteValue("PDV2004.Files", "", null);// Apagando o sinal de igual(=) que fica antes ou depois de cada chave

                            // escrever aqui dentro as novas dlls que o pdv irá carregar
                            _iniCurrent.WriteValue("PDV2004.Files", (count).ToString(), "InterfaceEpsonNF.dll");
                            count += 1;
                            _iniCurrent.WriteValue("PDV2004.Files", (count).ToString(), "ISCENFiscalEpsonDrv.dll");
                            count += 1;
                            Thread.Sleep(2000);
                            Application.DoEvents();
                            _iniCurrent.WriteValue("PDV2004.Files", "", "");//encerra a adição de dlls e coloca o maldito (=) na linha abaixo

                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "SourceDir", "\\ImpressoraNaoFiscais");
                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "DestDir", "<SysDir>\\");
                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "FileType", "File");
                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "ValidationType", "FileVersion");
                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "SCEVersion", "1.5.0.0");

                            _iniCurrent.WriteValue("PDV2004.InterfaceEpsonNF.dll", "", "");//encerra a adição de dlls e coloca o maldito (=) na linha abaixo

                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "SourceDir", "\\ImpressoraNaoFiscais");
                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "DestDir", "<SysDir>\\");
                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "FileType", "Component");
                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "ValidationType", "FileVersion");
                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "SCEVersion", "1.0.0.23");

                            _iniCurrent.WriteValue("PDV2004.ISCENFiscalEpsonDrv.dll", "", "");//encerra a adição de dlls e coloca o maldito (=) na linha abaixo
                        }

                    }

                    *////////////////////// FIM
                    Log.Log("Versão instalada " + ProjectVersion + ProjectVersionBuild);

                    label10.Text = ProjectVersion + " " + ProjectVersionBuild;

                    if (sInstallPath64 == null)
                    {
                        MessageBox.Show("Não há AutoMagazine instalado no computador ", "Gerenciador de erros", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        button1.Enabled = false;
                        button1.ForeColor = Color.Red;
                        textBox1.ForeColor = Color.Red;
                        textBox1.Text = "A instalação do AutoMagazine não foi detectada.";
                        Log.Log("instalação não detectada " + ProjectVersion + ProjectVersionBuild);
                    }
                    //log.LogWrite("Sistema 64bits detectado!");
                    
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.LogWrite("Erro na detecçãooooo do sistema operacional --> IntPtr.Size = " + IntPtr.Size.ToString());

            }
            
        }

        private bool GrantAccess(string fullPath)
        {
            try
            {
                label1.Text = "Adicionando permissões a pasta Network";
                DirectoryInfo dInfo = new DirectoryInfo(fullPath);
                //DirectorySecurity dSecurity = dInfo.GetAccessControl();
                Log.Log("Adicionando permissões a pasta Network (comentei no codigo porque estava demorando muito se a pasta network estiver em \"ProgramFiles\")");
                //dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, AccessControlType.Allow)); //esta demorando muito a adicionar permissões na pasa Network
                //dInfo.SetAccessControl(dSecurity); demora muito se feito pela rede
                //Log.Log("Permissões adicionadas em : " + fullPath);
                label1.Text = "Permissões adicionadas com sucesso";
                return true;                
            }
            catch(Exception ex)
            {
                MessageBox.Show("" + ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Log("Erro ao acessar a pasta Network em: " + fullPath);
                return false;                
            }
        }
        
        private void ExtractZip(string zipFileLocation, string destination)
        {
            try
            {
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(zipFileLocation))
                {

                    zip.ExtractProgress += new EventHandler<ExtractProgressEventArgs>(zip_ExtractProgress);
                    zip.ExtractAll(destination, ExtractExistingFileAction.OverwriteSilently);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        string aux;
        void zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            try
            {
                if (e.TotalBytesToTransfer > 0)
                {
                    progressBar1.Value = Convert.ToInt32(100 * e.BytesTransferred / e.TotalBytesToTransfer);
                    if (progressBar1.Value == 25 || progressBar1.Value == 50 || progressBar1.Value == 75 || progressBar1.Value == 99) //Se o resultado da divisão por 3 do "progressBar1.Value" for igual a 0, ele atualiza o frame da imagem.
                    {
                        pictureBox2.Image = gifImage.GetNextFrame();
                        pictureBox2.Update();
                    }
                    //log.LogWrite("Extraindo Arquivo --> ExtractProgressEventArgs = " + label1.Text);
                    //Está escrevendo varias linhas iguais,   VER DEPOIS
                    label1.ForeColor = System.Drawing.Color.White;
                    label1.Font = new Font(label1.Font.FontFamily, 8);
                    label1.Text = e.CurrentEntry.ToString();
                    if (aux != e.CurrentEntry.FileName)
                    {
                        Log.Log("Extraindo arquivo: " + e.CurrentEntry.FileName);
                        aux = e.CurrentEntry.FileName;
                    }

                    Application.DoEvents();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //log..LogWrite("Erro Extraindo Arquivo --> ExtractProgressEventArgs = " + e.CurrentEntry.ToString());
            }

            finally
            {
                if (e.EntriesExtracted != 0 && e.EntriesTotal != 0 && e.EntriesExtracted == e.EntriesTotal)
                {
                    label1.Text = sUpdatedVersion + " Instalada com sucesso!";
                    //pictureBox2.Visible = false;
                    pictureBox2.Update();
                    Log.Log(label1.Text);
                }
            }
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = false;
            progressBar1.Visible = true;
            pictureBox2.Visible = true;
            progressBar1.Value = 1;
            try
            {

                if (GrantAccess(textBox1.Text))
                {
                    IniFile _ini41 = new IniFile(textBox1.Text + "AutoMagazine.VSce");
                    string ActiveProjectVersion = _ini41.ReadValue("SCE","ActiveProjectVersion");
                    Application.DoEvents();
                    DirectoryInfo dir = new DirectoryInfo(textBox1.Text);
                    //dir.MoveTo(textBox1.Text + "installBackup");
                    string[] files = Directory.GetFiles(textBox1.Text, "*.tmp");
                    foreach (FileInfo file in dir.GetFiles("*.tmp", SearchOption.AllDirectories))
                    {
                        file.Delete();
                        Log.Log("Arquivo temporario " + file + " removido");
                    }

                    try
                    {
                        pictureBox2.Image = gifImage.GetNextFrame();
                        label1.ForeColor = System.Drawing.Color.White;
                        label1.Font = new Font(label1.Font.FontFamily, 12);
                        pictureBox2.Image = gifImage.GetNextFrame();
                        label1.Text = "Copiando o pacote da versão " + sUpdatedVersion;
                        pictureBox2.Image = gifImage.GetNextFrame();
                        progressBar1.Value = 10;
                        Application.DoEvents();
                        Thread.Sleep(2000);
                        pictureBox2.Image = gifImage.GetNextFrame();
                        progressBar1.Value = 30;
                        pictureBox2.Image = gifImage.GetNextFrame();
                        Update();
                        File.WriteAllBytes(textBox1.Text + ActiveProjectVersion + "\\" + sUpdateFile, oUpdateFile);
                        progressBar1.Value = 70;
                        pictureBox2.Image = gifImage.GetNextFrame();
                        if (oUpdateFile.LongLength == File.ReadAllBytes(textBox1.Text + ActiveProjectVersion + "\\" + sUpdateFile).LongLength)
                        {
                            label1.ForeColor = System.Drawing.Color.White;
                            label1.Font = new Font(label1.Font.FontFamily, 12);
                            label1.Text = "AMGUpdate.zip copiado com sucesso! ";
                            progressBar1.Value = 100;
                            pictureBox2.Image = gifImage.GetNextFrame();
                        }
                        else
                        {
                            label1.ForeColor = System.Drawing.Color.Red;
                            label1.Font = new Font(label1.Font.FontFamily, 12);
                            label1.Text = "AMGUpdate.zip parece estar corrompido";
                            progressBar1.Value = 0;
                            MessageBox.Show("Operação cancelada", "Gerenciador de erro");
                            Application.Exit();
                            this.Close();
                        }

                        Application.DoEvents();
                        Thread.Sleep(2000);
                        
                    }
                    catch
                    {
                        
                    }
                    
                    ExtractZip(textBox1.Text + ActiveProjectVersion +"\\"+ sUpdateFile, textBox1.Text + ActiveProjectVersion);
                    Log.Log("Inicio da Atualização da SCEScriptEngine em: " + textBox1.Text + ActiveProjectVersion + "\\Support\\SCEScriptEngine.Dll");
                    try
                    {
                            File.SetAttributes(textBox1.Text + "Config\\SCEScriptEngine.Dll", FileAttributes.Normal);
                            File.SetAttributes(textBox1.Text + ActiveProjectVersion + "\\Support\\SCEScriptEngine.Dll", FileAttributes.Normal);
                            File.Copy(textBox1.Text + ActiveProjectVersion + "\\Support\\SCEScriptEngine.Dll", textBox1.Text + "Config\\SCEScriptEngine.Dll", true);
                            Log.Log("SCEScriptEngine Atualizada com sucesso em: " + textBox1.Text + "Config\\SCEScriptEngine.Dll");                            
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("O instalador não conseguiu copiar a SCEScriptEngine.dll, o arquivo deve estar marcado como somente leitura" + ex.Message, "");
                        Log.Log("ERRO - SCEScriptEngine " + textBox1.Text + ActiveProjectVersion + "\\Support\\SCEScriptEngine.Dll");
                        Log.Log("ERRO - SCEScriptEngine " + textBox1.Text + "Config\\SCEScriptEngine.Dll");
                    }

                    IniFile Ini = new IniFile(textBox1.Text + "Config\\Network.cfg");
                    string AlternativeUrl = Ini.ReadValue("SCE", "AlternativeUrl");
                    string AlternativeDir = Ini.ReadValue("SCE", "AlternativeDir");

                    try
                    {
                        if (File.Exists(Ini.path))
                        {

                            if (AlternativeDir == "")
                            {
                                Ini.WriteValue("SCE", "AlternativeDir", "Clientes");
                            }

                            if (AlternativeUrl == "")
                            {
                                Ini.WriteValue("SCE", "AlternativeUrl", "");
                            }
                            //File.Copy(textBox1.Text + "4.1\\Support\\Network.cfg", textBox1.Text + "Config\\Network.cfg", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("" + ex, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("A pasta Network não possui privilégios de controle total (Configure permissões para a pasta)" , "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                //log.LogWrite("Arquivo SCEScriptEngine.dll --> Copiado");
                label1.ForeColor = System.Drawing.Color.White;
                label1.Font = new Font(label1.Font.FontFamily, 12);                
                progressBar1.Visible = false;
                button1.Visible = false;
                button2.Visible = true;
                this.Update();
                //MessageBox.Show("AutoMagazine " + sUpdatedVersion + " Atualizado com sucesso!", "Gerenciador do sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //log.LogWrite("------ Fim da Atualização ------");
            }

            catch (Exception ex)
            {
                MessageBox.Show("" + ex, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            progressBar1.Value = 0;
            pictureBox2.Visible = false;
            button1.Visible = false;
            button2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        

    }
}

