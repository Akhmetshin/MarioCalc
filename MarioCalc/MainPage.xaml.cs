using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace MarioCalc
{
    public class HANDMADE
    {
        private int ind;
        string taskStr;
        string answerStr;
        public string level;
        public char operation;
        public float[] data;
        public byte[] dARGB;
        public byte[] lARGB;
        public byte[] tARGB;
        public byte[] aARGB;
        public SolidColorBrush dBrush;
        public SolidColorBrush lBrush;
        public SolidColorBrush tBrush;
        public SolidColorBrush aBrush;

        CultureInfo culture; // <-по документации
        string specifier = "F";

        public HANDMADE()
        {
            culture = CultureInfo.CreateSpecificCulture("ru_RU");
            dARGB = new byte[] { 255, 0, 0, 0 };
            lARGB = new byte[] { 255, 0, 0, 255 };
            tARGB = new byte[] { 255, 255, 0, 0 };
            aARGB = new byte[] { 255, 0, 255, 0 };
        }

        public void ResetInd()
        {
            ind = 0;
        }
        public void dataTuning()
        {
            if (data.Length % 2 == 1)
            {
                Array.Resize(ref data, data.Length + 1);
                data[data.Length - 1] = 1; // чтобы цифра не пропадала)
            }
        }
        public (int n, string taskStr, string answerStr) GetNextTask()
        {
            int i = ind;
            ind++;
            ind++; // берём сразу две цифры из массива
            float c = 0;
            if (ind <= data.Length)
            {
                taskStr = data[i].ToString() + operation + data[i + 1].ToString() + "=";
                switch (operation)
                {
                    case '+':
                        c = data[i] + data[i + 1];
                        break;
                    case '-':
                        c = data[i] - data[i + 1];
                        break;
                    case '*':
                        c = data[i] * data[i + 1];
                        break;
                    case '/':
                        c = data[i] / data[i + 1];
                        break;
                }
                answerStr = c.ToString(specifier, culture); // можно поменять количество знаков после запятой в настройках Windows
                answerStr = answerStr.TrimEnd('0');
                answerStr = answerStr.TrimEnd('.');
            }
            else
            {
                i = -1;
                ind = 0;
            }
            return (i, taskStr, answerStr);
        }
    }

    // Первоначально данные предполагались случайными.
    // Потом стало понятно, что удобнее управлять данными из внешнего файла и иметь предопределённые задания.
    // Появился файл Task.json и class HANDMADE.
    public class RNDM
    {
        public RNDM()
        {
            dARGB = new byte[] { 255, 0, 0, 0 };
            lARGB = new byte[] { 255, 0, 0, 255 };
            tARGB = new byte[] { 255, 255, 0, 0 };
            aARGB = new byte[] { 255, 0, 255, 0 };
        }

        public string level;
        public char operation;
        public int min;
        public int max;
        public int count;
        public byte[] dARGB;
        public byte[] lARGB;
        public byte[] tARGB;
        public byte[] aARGB;
    }

    public class Task
    {
        public int iLevel;
        public string App { get; set; }
        public string Ver { get; set; }
        public string PlayNo { get; set; }
        public string PlayYes { get; set; }
        public string PlayTada { get; set; }
        public string PlayTadadadaaam { get; set; }
        public DateTime Date { get; set; }

        public HANDMADE[] HandMadeTask;
        public RNDM[] RndmTask;
        public HANDMADE[] tasks;

        public string Initial()
        {
            string r = Tuning();
            if (r.Length > 0) return r;

            tasks = new HANDMADE[HandMadeTask.Length + RndmTask.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new HANDMADE();
            }
            HandMadeTask.CopyTo(tasks, 0);

            Random random = new Random();

            for (int j = HandMadeTask.Length, i = 0; i < RndmTask.Length; j++, i++)
            {
                tasks[j].level = RndmTask[i].level;
                tasks[j].operation = RndmTask[i].operation;
                tasks[j].data = new float[RndmTask[i].count];
                for (int k = 0; k < RndmTask[i].count; k++)
                {
                    tasks[j].data[k] = random.Next(RndmTask[i].min, RndmTask[i].max);
                }
                for (int k = 0; k < 4; k++)
                {
                    tasks[j].dARGB[k] = RndmTask[i].dARGB[k];
                    tasks[j].lARGB[k] = RndmTask[i].lARGB[k];
                    tasks[j].tARGB[k] = RndmTask[i].tARGB[k];
                    tasks[j].aARGB[k] = RndmTask[i].aARGB[k];
                }
            }
            for (int i = 0; i < tasks.Length; i++)
            {
                Windows.UI.Color colorD = Windows.UI.Color.FromArgb(tasks[i].dARGB[0], tasks[i].dARGB[1], tasks[i].dARGB[2], tasks[i].dARGB[3]);
                tasks[i].dBrush = new SolidColorBrush(colorD);
                Windows.UI.Color colorL = Windows.UI.Color.FromArgb(tasks[i].lARGB[0], tasks[i].lARGB[1], tasks[i].lARGB[2], tasks[i].lARGB[3]);
                tasks[i].lBrush = new SolidColorBrush(colorL);
                Windows.UI.Color colorT = Windows.UI.Color.FromArgb(tasks[i].tARGB[0], tasks[i].tARGB[1], tasks[i].tARGB[2], tasks[i].tARGB[3]);
                tasks[i].tBrush = new SolidColorBrush(colorT);
                Windows.UI.Color colorA = Windows.UI.Color.FromArgb(tasks[i].aARGB[0], tasks[i].aARGB[1], tasks[i].aARGB[2], tasks[i].aARGB[3]);
                tasks[i].aBrush = new SolidColorBrush(colorA);
            }

            return r;
        }
        public string Tuning()
        {
            string r = "";
            try
            {
                for (int i = 0; i < HandMadeTask.Length; i++)
                {
                    HandMadeTask[i].dataTuning();
                }
                for (int i = 0; i < RndmTask.Length; i++)
                {
                    if (RndmTask[i].count % 2 == 1) RndmTask[i].count++; // если нечётное, а надо чётное.
                }
            }
            catch (Exception ex)
            {
                //messageStr.Text = ex.Message;
                // надо научиться выбрасывать свои исключения из своего класса
                r = ex.Message;
            }
            return r;
        }
        public (int n, string taskStr, string answerStr, string buff) GetNextTask()
        {
            int i = -2;
            string taskStr = "";
            string answerStr = "";

            if (iLevel < tasks.Length)
            {
                (i, taskStr, answerStr) = tasks[iLevel].GetNextTask();
                if (i == -1) // коряво
                {
                    iLevel++;
                    if (iLevel < tasks.Length)
                    {
                        (i, taskStr, answerStr) = tasks[iLevel].GetNextTask();
                    }
                }
            }
            StringBuilder sb = new StringBuilder(answerStr);
            for (int j = 0; j < answerStr.Length; j++) sb[j] = '*';
            string buff = sb.ToString();

            return (i, taskStr, answerStr, buff);
        }
    }

    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFile taskFile;

        Task task;
        int n;
        string taskStr;
        string buff;
        string answerStr;
        int indAnsw;
        string levelStr;

        MediaPlayer PlayNo;
        MediaPlayer PlayYes;
        MediaPlayer PlayTada;
        MediaPlayer PlayTadadadaaam;

        bool flagNext = false;
        bool flagTada = false;
        bool flagEnd = false;

        SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        SolidColorBrush darkRedBrush = new SolidColorBrush(Windows.UI.Colors.DarkRed);
        SolidColorBrush blackBrush = new SolidColorBrush(Windows.UI.Colors.Black);

        bool flagS = true;

        [Obsolete]
        public MainPage()
        {
            this.InitializeComponent();

            LoadTask();

            nextButton.Focus(FocusState.Programmatic);
        }

        private async void CopyToLocal()
        {
            try
            {
                StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await installFolder.GetFileAsync("Task.json");
                StorageFile pngFile = await installFolder.GetFileAsync("2022-11-15.png");

                var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Task.json");
                if (item == null)
                {
                    await sampleFile.CopyAsync(storageFolder);
                    await pngFile.CopyAsync(storageFolder);
                }
                
                taskFile = sampleFile;

                item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Media");
                if (item == null)
                {
                    StorageFolder media = await storageFolder.CreateFolderAsync("Media", CreationCollisionOption.FailIfExists);
                    StorageFolder source = await installFolder.GetFolderAsync("Media");
                    foreach (var file in await source.GetFilesAsync())
                    {
                        await file.CopyAsync(media, file.Name, NameCollisionOption.ReplaceExisting);
                    }
                }
            }
            catch (Exception ex)
            {
                nextButton.IsEnabled = false;
                messageStr.Text = ex.Message;
            }
        }

        [Obsolete]
        private async void LoadTask()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            //StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            // Мне кажется, что удобнее и правильнее работать из Current.LocalSettings.
            // Но как туда скопировать файлы при установке программы я не нашёл.
            // Поэтому так. Копирую при первом запуске.
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings != null)
            {
                string localValue = localSettings.Values["izFirst"] as string;
                if (localValue == null)
                {
                    storageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    CopyToLocal();
                    localSettings.Values["izFirst"] = "no";
                }
            }

            try
            {
                if (taskFile == null)
                {
                    taskFile = await storageFolder.GetFileAsync("Task.json");
                }
                task = JsonConvert.DeserializeObject<Task>(File.ReadAllText(taskFile.Path));

                verStr.Text = task.Ver;

                string r = task.Initial();
                if (r.Length > 0) messageStr.Text = r;

                PlayNo = new MediaPlayer();
                PlayYes = new MediaPlayer();
                PlayTada = new MediaPlayer();
                PlayTadadadaaam = new MediaPlayer();

                StorageFile sf = await storageFolder.GetFileAsync(task.PlayNo);
                PlayNo.SetFileSource(sf);

                sf = await storageFolder.GetFileAsync(task.PlayYes);
                PlayYes.SetFileSource(sf);

                sf = await storageFolder.GetFileAsync(task.PlayTada);
                PlayTada.SetFileSource(sf);

                sf = await storageFolder.GetFileAsync(task.PlayTadadadaaam);
                PlayTadadadaaam.SetFileSource(sf);

                if (localSettings != null)
                {
                    string localValue = localSettings.Values["iLevel"] as string;
                    if (localValue != null)
                    {
                        task.iLevel = int.Parse(localValue);
                        if (task.iLevel >= task.tasks.Length) task.iLevel = task.tasks.Length - 1;
                    }
                    string fl = localSettings.Values["flagS"] as string;
                    if (fl != null)
                    {
                        flagS = bool.Parse(fl);
                    }
                    else
                    {
                        flagS = true;
                    }
                    if (flagS)
                    {
                        PlayNo.Volume = 1;
                        PlayYes.Volume = 1;
                        PlayTada.Volume = 1;
                        sButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
                    }
                    else
                    {
                        PlayNo.Volume = 0;
                        PlayYes.Volume = 0;
                        PlayTada.Volume = 0;
                        sButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 100, 0, 0));
                    }

                }
                else
                {
                    task.iLevel = 0;
                }

                (n, taskStr, answerStr, buff) = task.GetNextTask();
                taskSTR.Foreground = task.tasks[task.iLevel].tBrush;
                taskSTR.Text = taskStr;
                answerSTR.Foreground = task.tasks[task.iLevel].aBrush;
                answerSTR.Text = buff;

                levelStr = task.tasks[task.iLevel].level;
                level.Foreground = task.tasks[task.iLevel].lBrush;
                desk.Background = task.tasks[task.iLevel].dBrush;
                level.Text = levelStr;
                levelExt.Text = "(" + (task.tasks[task.iLevel].data.Length / 2).ToString() + "\\" + (n / 2 + 1).ToString() + ")";
            }
            catch (Exception ex)
            {
                nextButton.IsEnabled = false;
                messageStr.Text = ex.Message;
            }
        }

        private void NextTask()
        {
            flagNext = false;
            if (flagEnd) { return; }

            (n, taskStr, answerStr, buff) = task.GetNextTask();

            if (n == 0)
            {
                if (flagTada) PlayTada.Play();
            }

            if (task.iLevel < task.tasks.Length)
            {
                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["iLevel"] = task.iLevel.ToString();

                desk.Background = task.tasks[task.iLevel].dBrush;

                level.Foreground = task.tasks[task.iLevel].lBrush;
                level.Text = task.tasks[task.iLevel].level;
                levelExt.Text = "(" + (task.tasks[task.iLevel].data.Length / 2).ToString() + "\\" + (n / 2 + 1).ToString() + ")";

                taskSTR.Foreground = task.tasks[task.iLevel].tBrush;
                taskSTR.Text = taskStr;

                indAnsw = 0;
                answerSTR.Foreground = task.tasks[task.iLevel].aBrush;
                answerSTR.Text = buff;
                nextButton.BorderBrush = darkRedBrush;
                nextButton.Foreground = blackBrush;
            }
            else
            {
                level.Text = "!!";
                levelExt.Text = "";

                taskSTR.Text = "";
                answerSTR.Text = "";

                //taskSTR.Text = "учи дифуры";
                //var uri = new System.Uri("ms-appdata:///local/2022-11-15.png");
                //var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
                //Image.Source = file;
                Image.Opacity = 100; // не придумал ничего лучше. да просто не умею(.
                verStr.Text = "www.mariocalc.ru";
                flagEnd = true;
                //PlayTada.Play();
                PlayTadadadaaam.Play();
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (flagNext) NextTask();
            else
            {
                PlayNo.Play();
            }
        }

        private void Grid_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
        {
            if (buff == null) return;

            if (args.Character == 27) return;

            if (indAnsw < buff.Length)
            {
                StringBuilder sbAnsw = new StringBuilder(answerStr);
                StringBuilder sb = new StringBuilder(buff);
                if (args.Character == sbAnsw[indAnsw])
                {
                    sb[indAnsw] = sbAnsw[indAnsw];
                    if (indAnsw < buff.Length) indAnsw++;
                    flagTada = true;
                    PlayYes.Play();
                }
                else
                {
                    sb[indAnsw] = '*';
                    PlayNo.Play();
                }

                buff = sb.ToString();
            }
            else
            {
            }

            if (indAnsw == buff.Length)
            {
                flagNext = true;
                nextButton.BorderBrush = redBrush;
                nextButton.Foreground = redBrush;
            }
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (buff == null) return;

            if (e.Key == VirtualKey.Enter)
            {
                return; // ?
            }
            if (e.Key == VirtualKey.Escape) return;
            if (e.Key == VirtualKey.Control) return;

            if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
            {
                if (e.Key == VirtualKey.Left)
                {
                    if (task.iLevel > 0)
                    {
                        if (task.iLevel < task.tasks.Length) task.tasks[task.iLevel].ResetInd(); // в исходное состояние ind и уменьшить iLevel
                        task.iLevel--;
                        flagNext = true;
                        flagTada = false;
                        flagEnd = false;
                        Image.Opacity = 0;
                        NextTask();
                    }
                    return;
                }
                if (e.Key == VirtualKey.Right)
                {
                    if (task.iLevel < task.tasks.Length - 1)
                    {
                        task.tasks[task.iLevel].ResetInd();
                        task.iLevel++;
                        flagNext = true;
                        flagTada = false;
                        flagEnd = false;
                        Image.Opacity = 0;
                        NextTask();
                    }
                    return;
                }
            }
            if (indAnsw < buff.Length)
            {
                StringBuilder sb = new StringBuilder(buff);
                sb[indAnsw] = '-';
                answerSTR.Text = sb.ToString();
            }
        }

        private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (buff == null) return;

            if (flagEnd)
            {
                if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (e.Key == VirtualKey.R)
                    {
                        task.iLevel = 0;
                        flagNext = true;
                        flagTada = true;
                        NextTask();
                    }
                }
                return;
            }
            if (e.Key == VirtualKey.Escape) return;

            answerSTR.Text = buff;
        }

        private void sButton_Click(object sender, RoutedEventArgs e)
        {
            if (buff == null) return;

            flagS = !flagS;
            if (flagS)
            {
                PlayNo.Volume = 1;
                PlayYes.Volume = 1;
                PlayTada.Volume = 1;
                sButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
            }
            else
            {
                PlayNo.Volume = 0;
                PlayYes.Volume = 0;
                PlayTada.Volume = 0;
                sButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 100, 0, 0));
            }

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["flagS"] = flagS.ToString();

            nextButton.Focus(FocusState.Programmatic);
        }

        private void Grid_Unloaded(object sender, RoutedEventArgs e) // <- ???
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["flagS"] = flagS.ToString();
        }

        private void rButton_Click(object sender, RoutedEventArgs e)
        {
            nextButton.Focus(FocusState.Programmatic);

            task.iLevel = 0;
            task.tasks[task.iLevel].ResetInd();
            flagNext = true;
            flagTada = false;
            flagEnd = false;
            Image.Opacity = 0;
            NextTask();
        }
    }
}
