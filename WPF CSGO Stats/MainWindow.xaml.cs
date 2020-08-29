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
        public int n = 0;

        float goodkd = 1.2f;
        float okaykd = 1.0f;
        float badkd = 0.0f;

        float gooddpr = 0.5f;
        float okaydpr = 0.75f;
        float baddpr = 1.0f;

        float goodapr = 0.1f;
        float okayapr = 0.05f;
        float badapr = 0.0f;

        float goodkpr = 1.2f;
        float okaykpr = 1.0f;
        float badkpr = 0.0f;

        int[] kills = new int[1];
        int[] assists = new int[1];
        int[] deaths = new int[1];
        int[] mvps = new int[1];
        double[] kd = new double[1];
        int[] rounds = new int[1];
        double[] dprs = new double[1];
        double[] kprs = new double[1];
        double[] aprs = new double[1];

        string killstring = "kill";
        string assiststring = "assist";
        string deathstring = "death";
        string mvpstring = "mvp";
        string kdstring = "kd";
        string roundstring = "round";
        string dprstring = "dpr";
        string kprstring = "kpr";
        string aprstring = "apr";

        string killfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\kills\";
        string assistfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\assists\";
        string deathfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\deaths\";
        string mvpfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\mvps";
        string kdfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\kds\";
        string nfile = AppDomain.CurrentDomain.BaseDirectory + @"stats\n";
        string roundfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\rounds\";
        string dprfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\dprs\";
        string kprfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\kprs\";
        string aprfolder = AppDomain.CurrentDomain.BaseDirectory + @"stats\aprs\";

        public MainWindow()
        {
            InitializeComponent();

            //check if it is first time launch, and if not then read previous session data otherwise create new session
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
            //check if directories in place if not create directories
            if (!Directory.Exists(killfolder))
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
            if (!Directory.Exists(roundfolder))
            {
                Directory.CreateDirectory(roundfolder);
            }
            if (!Directory.Exists(dprfolder))
            {
                Directory.CreateDirectory(dprfolder);
            }
            if (!Directory.Exists(kprfolder))
            {
                Directory.CreateDirectory(kprfolder);
            }
            if (!Directory.Exists(aprfolder))
            {
                Directory.CreateDirectory(aprfolder);
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
            Array.Resize<int>(ref rounds, n + 1);
            Array.Resize<double>(ref dprs, n + 1);
            Array.Resize<double>(ref kprs, n + 1);
            Array.Resize<double>(ref aprs, n + 1);

            //check if it is the first time launching otherwise read prevois data
            if (n > 0)
            {

                kills = ArrayReader(n + 1, killstring);
                assists = ArrayReader(n + 1, assiststring);
                deaths = ArrayReader(n + 1, deathstring);
                mvps = ArrayReader(n + 1, mvpstring);
                kd = ArrayReaderDouble(n + 1, kdstring);
                rounds = ArrayReader(n + 1, roundstring);
                dprs = ArrayReaderDouble(n + 1, dprstring);
                kprs = ArrayReaderDouble(n + 1, kprstring);
                aprs = ArrayReaderDouble(n + 1, aprstring);
            }

            //getting inputs user
            kills[n] = int.Parse(this.TextFieldKills.Text);
            assists[n] = int.Parse(this.TextFieldAssist.Text);
            deaths[n] = int.Parse(this.TextFieldDeaths.Text);
            mvps[n] = int.Parse(this.TextFieldMVPs.Text);
            rounds[n] = int.Parse(this.TextFieldRounds.Text);

            //check if death count is greater than 0 if not then kd equals to kill count
            if (deaths[n] > 0)
            {
                kd[n] = Convert.ToDouble(kills[n] / deaths[n]);
            }
            else if (deaths[n] == 0)
            {
                kd[n] = Convert.ToDouble(kills[n]);
            }

            //calculating dpr, kpr and apr
            dprs[n] = Convert.ToDouble(deaths[n]) / Convert.ToDouble(rounds[n]);
            kprs[n] = Convert.ToDouble(kills[n]) / Convert.ToDouble(rounds[n]);
            aprs[n] = Convert.ToDouble(assists[n]) / Convert.ToDouble(rounds[n]);

            //saving kill stats in a new file
            FileStream fPramaterKills = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"stats\kills\kill_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wParamaterKills = new StreamWriter(fPramaterKills);
            wParamaterKills.BaseStream.Seek(0, SeekOrigin.End);
            wParamaterKills.Write(kills[n]);
            wParamaterKills.Flush();
            wParamaterKills.Close();

            //saving assist stats
            FileStream fParamaterAssists = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"stats\assists\assist_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wParamaterAssists = new StreamWriter(fParamaterAssists);
            wParamaterAssists.BaseStream.Seek(0, SeekOrigin.End);
            wParamaterAssists.Write(assists[n]);
            wParamaterAssists.Flush();
            wParamaterAssists.Close();

            //saving death stats
            FileStream fparameterdeaths = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"stats\deaths\death_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wparameterdeaths = new StreamWriter(fparameterdeaths);
            wparameterdeaths.BaseStream.Seek(0, SeekOrigin.End);
            wparameterdeaths.Write(deaths[n]);
            wparameterdeaths.Flush();
            wparameterdeaths.Close();

            //saving mvp stats
            FileStream fparametermvps = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"stats\mvps\mvp_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wparametermvps = new StreamWriter(fparametermvps);
            wparametermvps.BaseStream.Seek(0, SeekOrigin.End);
            wparametermvps.Write(mvps[n]);
            wparametermvps.Flush();
            wparametermvps.Close();

            //saving kd stats
            FileStream fparameterkd = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"stats\kds\kd_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wparameterkd = new StreamWriter(fparameterkd);
            wparameterkd.BaseStream.Seek(0, SeekOrigin.End);
            wparameterkd.Write(kd[n]);
            wparameterkd.Flush();
            wparameterkd.Close();

            //saving round stats
            FileStream fparameterround = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"stats\rounds\round_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wround = new StreamWriter(fparameterround);
            wround.BaseStream.Seek(0, SeekOrigin.End);
            wround.Write(rounds[n]);
            wround.Flush();
            wround.Close();

            //saving dpr stats
            FileStream fdpr = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"stats\dprs\dpr_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wdpr = new StreamWriter(fdpr);
            wdpr.BaseStream.Seek(0, SeekOrigin.End);
            wdpr.Write(dprs[n]);
            wdpr.Flush();
            wdpr.Close();

            //saving kpr stats
            FileStream fkpr = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"stats\kprs\kpr_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wkpr = new StreamWriter(fkpr);
            wkpr.BaseStream.Seek(0, SeekOrigin.End);
            wkpr.Write(kprs[n]);
            wkpr.Flush();
            wkpr.Close();

            //saving apr stats
            FileStream fapr = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"stats\aprs\apr_" + n, FileMode.Create, FileAccess.Write);
            StreamWriter wapr = new StreamWriter(fapr);
            wapr.BaseStream.Seek(0, SeekOrigin.End);
            wapr.Write(aprs[n]);
            wapr.Flush();
            wapr.Close();

            //increasing the index number so next time session will not overwrite this session data
            n++;

            //saving n integer
            FileStream fparametern = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"stats\n", FileMode.Create, FileAccess.Write);
            StreamWriter wparametern = new StreamWriter(fparametern);
            wparametern.BaseStream.Seek(0, SeekOrigin.End);
            wparametern.Write(n);
            wparametern.Flush();
            wparametern.Close();


        }

        //loads the database and shows the stats
        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {

            //cheacking if it is first launch
            if (n == 0)
            {
                //show messagebox if it is first time launch
                MessageBoxResult errorLoad = MessageBox.Show("You don't have any data yet!", "Error");
            }
            else
            {
                //resizing the arrays so new elements could be stored
                Array.Resize<int>(ref kills, n + 1);
                Array.Resize<int>(ref assists, n + 1);
                Array.Resize<int>(ref deaths, n + 1);
                Array.Resize<int>(ref mvps, n + 1);
                Array.Resize<double>(ref kd, n + 1);
                Array.Resize<int>(ref rounds, n + 1);
                Array.Resize<double>(ref dprs, n + 1);
                Array.Resize<double>(ref kprs, n + 1);
                Array.Resize<double>(ref aprs, n + 1);

                //check if it is the first time launching otherwise read prevois data
                if (n > 0)
                {

                    kills = ArrayReader(n + 1, killstring);
                    assists = ArrayReader(n + 1, assiststring);
                    deaths = ArrayReader(n + 1, deathstring);
                    mvps = ArrayReader(n + 1, mvpstring);
                    kd = ArrayReaderDouble(n + 1, kdstring);
                    rounds = ArrayReader(n + 1, roundstring);
                    dprs = ArrayReaderDouble(n + 1, dprstring);
                    kprs = ArrayReaderDouble(n + 1, kprstring);
                    aprs = ArrayReaderDouble(n + 1, aprstring);
                }

                //calculating total stats
                int killtotal = ArraySum(n, kills);
                int assisttotal = ArraySum(n, assists);
                int deathtotal = ArraySum(n, deaths);
                int mvptotal = ArraySum(n, mvps);
                int roundtotal = ArraySum(n, rounds);
                double kdaverage = ArraySumDouble(n, kd) / Convert.ToDouble(n);
                double dpraverage = ArraySumDouble(n, dprs) / Convert.ToDouble(n);
                double kpraverage = ArraySumDouble(n, kprs) / Convert.ToDouble(n);
                double apraverage = ArraySumDouble(n, aprs) / Convert.ToDouble(n);

                //showing the stats
                this.TextKillCount.Text = Convert.ToString(killtotal);
                this.TextAssistCount.Text = Convert.ToString(assisttotal);
                this.TextDeathCount.Text = Convert.ToString(deathtotal);
                this.TextMVPCount.Text = Convert.ToString(mvptotal);
                this.TextKD.Text = Convert.ToString(kdaverage);
                this.TextRoundCount.Text = Convert.ToString(roundtotal);
                this.TextDPR.Text = Convert.ToString(dpraverage);
                this.TextKPR.Text = Convert.ToString(kpraverage);
                this.TextAPR.Text = Convert.ToString(apraverage);

                //calculating the coords and color for kd line
                //checking it is good
                if (kdaverage >= goodkd)
                {

                    //setting colors
                    this.kdline.Stroke = new SolidColorBrush(Colors.Green);
                    this.linekdgood.Stroke = new SolidColorBrush(Colors.Green);
                    this.linekdgoodbl.Stroke = new SolidColorBrush(Colors.Green);
                    this.linekdokay.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekdokaybl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekdpoor.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekdpoorbl.Stroke = new SolidColorBrush(Colors.Gray);

                    //setting position
                    if (kdaverage >= 1.5f)
                    {
                        this.kdline.X1 = 100;


                    }
                    if (kdaverage >= 1.2f && kdaverage < 1.3f)
                    {
                        this.kdline.X1 = 67;

                    }
                    if (kdaverage >= 1.3f && kdaverage < 1.4f)
                    {
                        this.kdline.X1 = 78;
                    }
                    if (kdaverage >= 1.4f && kdaverage < 1.5f)
                    {
                        this.kdline.X1 = 89;
                    }
                    this.kdline.X2 = this.kdline.X1;
                }

                //checking if kd okay
                if (kdaverage >= okaykd && kdaverage < goodkd)
                {

                    //setting colors
                    this.kdline.Stroke = new SolidColorBrush(Colors.Orange);
                    this.linekdgood.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekdgoodbl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekdokay.Stroke = new SolidColorBrush(Colors.Orange);
                    this.linekdokaybl.Stroke = new SolidColorBrush(Colors.Orange);
                    this.linekdpoor.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekdpoorbl.Stroke = new SolidColorBrush(Colors.Gray);

                    //setting position
                    if (kdaverage <= 1.2f && kdaverage > 1.15f)
                    {
                        this.kdline.X1 = 66;
                    }
                    if (kdaverage <= 1.15f && kdaverage > 1.1f)
                    {
                        this.kdline.X1 = 54;
                    }
                    if (kdaverage <= 1.1f && kdaverage > 1.05f)
                    {
                        this.kdline.X1 = 42;
                    }
                    if (kdaverage <= 1.05f && okaykd >= kdaverage)
                    {
                        this.kdline.X1 = 34;
                    }
                    this.kdline.X2 = this.kdline.X1;
                }

                //checking if kd is poor
                if (kdaverage >= badkd && kdaverage < okaykd)
                {

                    //setting colors
                    this.kdline.Stroke = new SolidColorBrush(Colors.Red);
                    this.linekdgood.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekdgoodbl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekdokay.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekdokaybl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekdpoor.Stroke = new SolidColorBrush(Colors.Red);
                    this.linekdpoorbl.Stroke = new SolidColorBrush(Colors.Red);

                    //setting poition
                    if (kdaverage <= okaykd && kdaverage > 0.75f)
                    {
                        this.kdline.X1 = 33;
                    }
                    if (kdaverage <= 0.75f && kdaverage > 0.5f)
                    {
                        this.kdline.X1 = 21;
                    }
                    if (kdaverage <= 0.5 && kdaverage > 0.25f)
                    {
                        this.kdline.X1 = 9;
                    }
                    if (kdaverage <= 0.25f && kdaverage >= badkd)
                    {
                        this.kdline.X1 = 0;
                    }
                    this.kdline.X2 = this.kdline.X1;
                }

                //dpr line set up
                //checking dpr is good
                if (dpraverage <= gooddpr)
                {

                    //setting colors
                    this.DPRLine.Stroke = new SolidColorBrush(Colors.Green);
                    this.linedprgood.Stroke = new SolidColorBrush(Colors.Green);
                    this.linedprgoodbl.Stroke = new SolidColorBrush(Colors.Green);
                    this.linedprokay.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprokaybl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprpoor.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprpoorbl.Stroke = new SolidColorBrush(Colors.Gray);
                    
                    //setting position
                    if (dpraverage >= 0.0f && dpraverage < 0.2f)
                    {
                        this.DPRLine.X1 = 100;
                    }
                    if (dpraverage >= 0.2f && dpraverage < 0.4f)
                    {
                        this.DPRLine.X1 = 78;
                    }
                    if (dpraverage >= 0.4f && dpraverage <= 0.5f)
                    {
                        this.DPRLine.X1 = 67;
                    }
                    this.DPRLine.X2 = this.DPRLine.X1;
                }

                //checking if dpr is okay
                if (dpraverage <= okaydpr && dpraverage > gooddpr)
                {
                    
                    //setting colors
                    this.DPRLine.Stroke = new SolidColorBrush(Colors.Orange);
                    this.linedprgood.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprgoodbl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprokay.Stroke = new SolidColorBrush(Colors.Orange);
                    this.linedprokaybl.Stroke = new SolidColorBrush(Colors.Orange);
                    this.linedprpoor.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprpoorbl.Stroke = new SolidColorBrush(Colors.Gray);

                    //setting position
                    if (dpraverage <= okaydpr && dpraverage > 0.68f)
                    {
                        this.DPRLine.X1 = 34;
                    }
                    if (dpraverage <= 0.67f && dpraverage > 0.6f)
                    {
                        this.DPRLine.X1 = 50;
                    }
                    if (dpraverage <= 0.59f && dpraverage > gooddpr)
                    {
                        this.DPRLine.X1 = 66;
                    }
                    this.DPRLine.X2 = this.DPRLine.X1;
                }

                //checking if dpr is poor
                if (dpraverage <= baddpr && dpraverage > okaydpr)
                {

                    //setting colors
                    this.DPRLine.Stroke = new SolidColorBrush(Colors.Red);
                    this.linedprgood.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprgoodbl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprokay.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprokaybl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprpoor.Stroke = new SolidColorBrush(Colors.Red);
                    this.linedprpoorbl.Stroke = new SolidColorBrush(Colors.Red);

                    //setting position
                    if (dpraverage <= 1.0f && dpraverage > 0.92f)
                    {
                        this.DPRLine.X1 = 0;
                    }
                    if (dpraverage < 0.92f && dpraverage > 0.8f)
                    {
                        this.DPRLine.X1 = 15;
                    }
                    if (dpraverage < 0.8f && dpraverage >= baddpr)
                    {
                        this.DPRLine.X1 = 33;
                    }
                    this.DPRLine.X2 = this.DPRLine.X1;
                }

                //kpr stats
                //checking if kpr good
                if (kpraverage>=goodkpr)
                {
                    //setting colors
                    this.KPRLine.Stroke = new SolidColorBrush(Colors.Green);
                    this.linekprgood.Stroke = new SolidColorBrush(Colors.Green);
                    this.linekprgoodbl.Stroke = new SolidColorBrush(Colors.Green);
                    this.linekprokay.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekprokaybl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekprpoor.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekprpoorbl.Stroke = new SolidColorBrush(Colors.Gray);
                    
                    //setting position
                    if (kpraverage >= 1.4f && kpraverage < 1.5f)
                    {
                        this.KPRLine.X1 = 100;
                    }
                    if (kpraverage <= 1.4f && kpraverage > 1.3)
                    {
                        this.KPRLine.X1 = 80;
                    }
                    if (kpraverage >= goodkpr && kpraverage < 1.3)
                    {
                        this.KPRLine.X1 = 68;
                    }
                    this.KPRLine.X2 = this.KPRLine.X1;
                }

                //checking if kpr okay
                if (kpraverage >= okaykpr && kpraverage < goodkpr)
                {
                    //setting colors
                    this.KPRLine.Stroke = new SolidColorBrush(Colors.Orange);
                    this.linekprgood.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekprgoodbl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekprokay.Stroke = new SolidColorBrush(Colors.Orange);
                    this.linekprokaybl.Stroke = new SolidColorBrush(Colors.Orange);
                    this.linekprpoor.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekprpoorbl.Stroke = new SolidColorBrush(Colors.Gray);

                    //setting position
                    if (kpraverage > badkpr && kpraverage <= 1.05f)
                    {
                        this.KPRLine.X1 = 34;
                    }
                    if (kpraverage > 1.06f && kpraverage < 1.1f)
                    {
                        this.KPRLine.X1 = 44;
                    }
                    if (kpraverage > 1.11f && kpraverage < 1.15f)
                    {
                        this.KPRLine.X1 = 55;
                    }
                    if(kpraverage>1.16f&& kpraverage < goodkpr)
                    {
                        this.KPRLine.X1 = 66;
                    }
                    this.KPRLine.X2 = this.KPRLine.X1;
                }

                //checking if kpr is poor
                if (kpraverage >= badkpr && kpraverage < okaykpr)
                {
                    //setting colors
                    this.KPRLine.Stroke = new SolidColorBrush(Colors.Red);
                    this.linekprgood.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekprgoodbl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekprokay.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekprokaybl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linekprpoor.Stroke = new SolidColorBrush(Colors.Red);
                    this.linekprpoorbl.Stroke = new SolidColorBrush(Colors.Red);
                    
                    //setting position
                    if (kpraverage >= badkpr && kpraverage < 0.25f)
                    {
                        this.KPRLine.X1 = 0;
                    }
                    if (kpraverage >= 0.25f && kpraverage < 0.5f)
                    {
                        this.KPRLine.X1 = 11;
                    }
                    if(kpraverage>=0.5f&& kpraverage < 0.75f)
                    {
                        this.KPRLine.X1 = 22;
                    }
                    if (kpraverage >= 0.75f && kpraverage < okaykpr)
                    {
                        this.KPRLine.X1 = 33;
                    }
                    this.KPRLine.X2 = this.KPRLine.X1;
                }

                //apr stats
                //checking if apr good
                if (apraverage >= goodapr)
                {
                    //setting colors
                    this.APRLine.Stroke = new SolidColorBrush(Colors.Green);
                    this.lineaprgood.Stroke = new SolidColorBrush(Colors.Green);
                    this.lineaprgoodbl.Stroke = new SolidColorBrush(Colors.Green);
                    this.lineaprokay.Stroke = new SolidColorBrush(Colors.Gray);
                    this.lineaprokaybl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.lineaprpoor.Stroke = new SolidColorBrush(Colors.Gray);
                    this.lineaprpoorbl.Stroke = new SolidColorBrush(Colors.Gray);
                    
                    //setting position
                    if (apraverage >= 1.4f && apraverage < 1.5f)
                    {
                        this.APRLine.X1 = 100;
                    }
                    if (apraverage <= 1.4f && apraverage > 1.3)
                    {
                        this.APRLine.X1 = 80;
                    }
                    if (apraverage >= goodapr && apraverage < 1.3)
                    {
                        this.APRLine.X1 = 68;
                    }
                    this.APRLine.X2 = this.APRLine.X1;
                }

                //checking if apr is okay
                if (apraverage >= okayapr && apraverage < goodapr)
                {
                    //setting colors
                    this.APRLine.Stroke = new SolidColorBrush(Colors.Orange);
                    this.lineaprgood.Stroke = new SolidColorBrush(Colors.Gray);
                    this.lineaprgoodbl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.lineaprokay.Stroke = new SolidColorBrush(Colors.Orange);
                    this.lineaprokaybl.Stroke = new SolidColorBrush(Colors.Orange);
                    this.lineaprpoor.Stroke = new SolidColorBrush(Colors.Gray);
                    this.lineaprpoorbl.Stroke = new SolidColorBrush(Colors.Gray);

                    //setting position
                    if (apraverage > badapr && apraverage <= 1.05f)
                    {
                        this.APRLine.X1 = 34;
                    }
                    if (apraverage > 1.06f && apraverage < 1.1f)
                    {
                        this.APRLine.X1 = 44;
                    }
                    if (apraverage > 1.11f && apraverage < 1.15f)
                    {
                        this.APRLine.X1 = 55;
                    }
                    if (apraverage > 1.16f && apraverage < goodapr)
                    {
                        this.APRLine.X1 = 66;
                    }
                    this.APRLine.X2 = this.APRLine.X1;
                }

                //checking if apr is poor
                if (apraverage >= badapr && apraverage < okayapr)
                {
                    //setting colors
                    this.APRLine.Stroke = new SolidColorBrush(Colors.Red);
                    this.linedprgood.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprgoodbl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprokay.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprokaybl.Stroke = new SolidColorBrush(Colors.Gray);
                    this.linedprpoor.Stroke = new SolidColorBrush(Colors.Red);
                    this.linedprpoorbl.Stroke = new SolidColorBrush(Colors.Red);

                    //setting position
                    if (apraverage >= badapr && apraverage < 0.25f)
                    {
                        this.APRLine.X1 = 0;
                    }
                    if (apraverage >= 0.25f && apraverage < 0.5f)
                    {
                        this.APRLine.X1 = 11;
                    }
                    if (apraverage >= 0.5f && apraverage < 0.75f)
                    {
                        this.APRLine.X1 = 22;
                    }
                    if (apraverage >= 0.75f && apraverage < okayapr)
                    {
                        this.APRLine.X1 = 33;
                    }
                    this.APRLine.X2 = this.APRLine.X1;
                }
            }
        }

        //opens up "about" dialog
        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Simple Counter-Strike: Global Offensive statistics app\n\n\n2020\ncreated by fracta7", "About");
        }

        //method for calculating sum of array elements
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

        public double ArraySumDouble(int a, double[] b)
        {
            int temp = 0;
            double total = 0;
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
                    b[temp] = int.Parse(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"stats\" + c + @"s\" + c + @"_" + temp));
                    temp++;
                }
                return b;
            }

            else
            {
                while (temp < a - 1)
                {
                    b[temp] = int.Parse(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"stats\" + c + @"s\" + c + @"_" + temp));
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
                    b[temp] = double.Parse(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"stats\" + c + @"s\" + c + @"_" + temp));
                    temp++;
                }
                return b;
            }

            else
            {
                while (temp < a - 1)
                {
                    b[temp] = double.Parse(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"stats\" + c + @"s\" + c + @"_" + temp));
                    temp++;
                }
                return b;
            }

        }

        //checker to allow only numeric input
        private void TextFieldKills_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            //checking if it is a number
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        //close button handler
        private void closeApp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        //minimize button handler
        private void minimizeApp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //for draging the window from top bar
        private void dragMe(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
