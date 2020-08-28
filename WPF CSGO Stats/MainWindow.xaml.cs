using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace WPF_CSGO_Stats
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 



    public partial class MainWindow : Window
    {

        //declaring variables
        int n = 0;
        int[] kills = new int[1];
        int[] assists = new int[1];
        int[] deaths = new int[1];
        int[] mvps = new int[1];
        double[] kd = new double[1];
        string killstring = "kill";
        string assiststring = "assist";
        string deathstring = "death";
        string mvpstring = "mvp";
        string kdstring = "kd";
        string killfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\kills\";
        string assistfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\assists\";
        string deathfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\assists\";
        string mvpfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\mvps";
        string kdfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\kds\";
        string nfile = AppDomain.CurrentDomain.BaseDirectory + @"stats\n";

        public MainWindow()
        {
            InitializeComponent();

            //check if it is first time launch, and if not read previous session data otherwise create new session
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"stats\"))
            {
                if (File.Exists(nfile))
                {
                    n = int.Parse(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"stats\n"));
                }
                else
                {
                    FileStream fparameternn = new FileStream(nfile, FileMode.Create, FileAccess.Write);
                    StreamWriter nfilew = new StreamWriter(fparameternn);
                    nfilew.BaseStream.Seek(0, SeekOrigin.End);
                    nfilew.Write(n);
                    nfilew.Flush();
                    nfilew.Close();
                }
            }
            //check if directories in place
            if(!Directory.Exists(killfolder))
            {
                Directory.CreateDirectory(killfolder);
            }
            if (!Directory.Exists(assistfolder))
            {
                Directory.CreateDirectory(assistfolder);
            }
            if (!Directory.Exists(deathfolder))
            {
                Directory.CreateDirectory(deathfolder);
            }
            if (!Directory.Exists(kdfolder))
            {
                Directory.CreateDirectory(kdfolder);
            }
            if (!Directory.Exists(mvpfolder))
            {
                Directory.CreateDirectory(mvpfolder);
            }
        }

        private void ButtonNewSave_Click(object sender, RoutedEventArgs e)
        {

            //resizing the arrays so new elements could be stored
            Array.Resize<int>(ref kills, n + 1);
            Array.Resize<int>(ref assists, n + 1);
            Array.Resize<int>(ref deaths, n + 1);
            Array.Resize<int>(ref mvps, n + 1);
            Array.Resize<double>(ref kd, n + 1);

            //check if it is the first time launching otherwise read prevois data
            if (n > 0)
            {

                kills = ArrayReader(n + 1, killstring);
                assists = ArrayReader(n + 1, assiststring);
                deaths = ArrayReader(n + 1, deathstring);
                mvps = ArrayReader(n + 1, mvpstring);
                kd = ArrayReaderDouble(n + 1, kdstring);
            }

            //getting inputs user
            kills[n] = int.Parse(this.TextFieldKills.Text);
            assists[n] = int.Parse(this.TextFieldAssist.Text);
            deaths[n] = int.Parse(this.TextFieldDeaths.Text);
            mvps[n] = int.Parse(this.TextFieldMVPs.Text);

            //check if death count is greater than 0 if not then kd equals to kill count
            if (deaths[n] > 0)
            {
                kd[n] = Convert.ToDouble(kills[n] / deaths[n]);
            }
            else if (deaths[n] == 0)
            {
                kd[n] = Convert.ToDouble(kills[n]);
            }

            //saving kill stats in a new file
            FileStream fPramaterKills = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\stats\kills\kill_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wParamaterKills = new StreamWriter(fPramaterKills);
            wParamaterKills.BaseStream.Seek(0, SeekOrigin.End);
            wParamaterKills.Write(kills[n]);
            wParamaterKills.Flush();
            wParamaterKills.Close();

            //saving assist stats
            FileStream fParamaterAssists = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\stats\assists\assist_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wParamaterAssists = new StreamWriter(fParamaterAssists);
            wParamaterAssists.BaseStream.Seek(0, SeekOrigin.End);
            wParamaterAssists.Write(assists[n]);
            wParamaterAssists.Flush();
            wParamaterAssists.Close();

            //saving death stats
            FileStream fparameterdeaths = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\stats\deaths\death_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wparameterdeaths = new StreamWriter(fparameterdeaths);
            wparameterdeaths.BaseStream.Seek(0, SeekOrigin.End);
            wparameterdeaths.Write(deaths[n]);
            wparameterdeaths.Flush();
            wparameterdeaths.Close();

            //saving mvp stats
            FileStream fparametermvps = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\stats\mvps\mvp_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wparametermvps = new StreamWriter(fparametermvps);
            wparametermvps.BaseStream.Seek(0, SeekOrigin.End);
            wparametermvps.Write(mvps[n]);
            wparametermvps.Flush();
            wparametermvps.Close();

            //saving kd stats
            FileStream fparameterkd = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\stats\kds\kd_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wparameterkd = new StreamWriter(fparameterkd);
            wparameterkd.BaseStream.Seek(0, SeekOrigin.End);
            wparameterkd.Write(kd[n]);
            wparameterkd.Flush();
            wparameterkd.Close();

            //increasing the index number so next time session will not overwrite this session data
            n++;

            //saving n integer
            FileStream fparametern = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\stats\n", FileMode.Create, FileAccess.Write);
            StreamWriter wparametern = new StreamWriter(fparametern);
            wparametern.BaseStream.Seek(0, SeekOrigin.End);
            wparametern.Write(n);
            wparametern.Flush();
            wparametern.Close();


        }

        //loads the database and shows the stats
        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            //resizing the arrays so new elements could be stored
            Array.Resize<int>(ref kills, n + 1);
            Array.Resize<int>(ref assists, n + 1);
            Array.Resize<int>(ref deaths, n + 1);
            Array.Resize<int>(ref mvps, n + 1);
            Array.Resize<double>(ref kd, n + 1);

            //check if it is the first time launching otherwise read prevois data
            if (n > 0)
            {

                kills = ArrayReader(n + 1, killstring);
                assists = ArrayReader(n + 1, assiststring);
                deaths = ArrayReader(n + 1, deathstring);
                mvps = ArrayReader(n + 1, mvpstring);
                kd = ArrayReaderDouble(n + 1, kdstring);
            }

            int killtotal = ArraySum(n, kills);
            int assisttotal = ArraySum(n, assists);
            int deathtotal = ArraySum(n, deaths);
            int mvptotal = ArraySum(n, mvps);
            double kdaverage;

            //checking if total deaths is zero or not otherwise average kd ratio will be total kill amount 
            if (deathtotal == 0)
            {
                kdaverage = killtotal;
            }
            else
            {
                kdaverage = Convert.ToDouble(killtotal) / Convert.ToDouble(deathtotal);
            }

            //showing the stats
            this.TextKillCount.Text = Convert.ToString(killtotal);
            this.TextAssistCount.Text = Convert.ToString(assisttotal);
            this.TextDeathCount.Text = Convert.ToString(deathtotal);
            this.TextMVPCount.Text = Convert.ToString(mvptotal);
            this.TextKD.Text = Convert.ToString(kdaverage);
        }

        //opens up "about" dialog
        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Simple Counter-Strike: Global Offensive statistics app\n\n\n2020\ncreated by fracta7", "About");
        }

        //method for calculating sum of array
        public int ArraySum(int a, int[] b)
        {
            int temp = 0;
            int total = 0;
            while (temp < a)
            {
                total = total + b[temp];
                temp++;
            }
            return total;
        }

        //reading previous session data from "database"
        public int[] ArrayReader(int a, string c)
        {
            int temp = 0;
            int[] b = new int[a];
            if (a == 0)
            {
                while (temp < a)
                {
                    b[temp] = int.Parse(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\stats\" + c + @"s\" + c + @"_" + temp));
                    temp++;
                }
                return b;
            }

            else
            {
                while (temp < a - 1)
                {
                    b[temp] = int.Parse(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\stats\" + c + @"s\" + c + @"_" + temp));
                    temp++;
                }
                return b;
            }

        }

        //the same method but with array of doubles
        public double[] ArrayReaderDouble(int a, string c)
        {
            int temp = 0;
            double[] b = new double[a];
            if (a == 0)
            {
                while (temp < a)
                {
                    b[temp] = int.Parse(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\stats\" + c + @"s\" + c + @"_" + temp));
                    temp++;
                }
                return b;
            }

            else
            {
                while (temp < a - 1)
                {
                    b[temp] = int.Parse(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\stats\" + c + @"s\" + c + @"_" + temp));
                    temp++;
                }
                return b;
            }

        }

        
        private void TextFieldKills_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            //checking if it is a number
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }
    }
}
