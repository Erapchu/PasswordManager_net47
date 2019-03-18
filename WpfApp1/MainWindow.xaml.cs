using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.ComponentModel;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    // comment
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("De-DE");
            InitializeComponent();
            Authorization();
        }

        public List<int> SearchIndexes { get; set; }
        public int CurrentN { get; set; }
        public bool IsUpdateIndex { get; set; }
        public bool IsSaved { get; set; }
        static public string CorrectPassword { get; set; }
        public string MyDocuments { get; set; }

        RoutedEventHandler firstAuth;

        private void Authorization()
        {
            MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            AuthDialog.IsOpen = true;

            if (EncryptDecrypt.Decrypt(MyDocuments + @"\passdata.dat", 4) == null)
            {
                AuthText.Text = "Type your new password";
                OKButtonAuth.Click -= OKButtonAuth_Click;

                firstAuth = delegate(object sender, RoutedEventArgs e)
                {
                    try
                    {
                        if (PasswordAccount.Password == string.Empty)
                            throw new Exception("Your password is empty");
                        CorrectPassword = PasswordAccount.Password;
                        using (BinaryWriter bw = new BinaryWriter(File.Open(MyDocuments + @"\passdata.dat", FileMode.Create)))
                        {
                            bw.Write(EncryptDecrypt.EncryptPass(CorrectPassword, 4));
                        }
                        OKButtonAuth.Command = DialogHost.CloseDialogCommand;
                        CancelButtonAuth.Click -= CancelButtonAuth_Click;
                    }
                    catch (Exception E)
                    {
                        HintAssist.SetHint(PasswordAccount, E.Message);
                        OKButtonAuth.Command = null;
                    }
                };
                OKButtonAuth.Click += firstAuth;
            }
            else
            {
                MemoryStream stream = new MemoryStream(EncryptDecrypt.Decrypt(MyDocuments + @"\passdata.dat", 4));
                using (BinaryReader br = new BinaryReader(stream))
                {
                    CorrectPassword = br.ReadString();
                }
            }
        }

        private void EditMode_ON_OFF()
        {
            //Переключение в режим редактирования/добавления информации
            SearchTextBox.IsEnabled = !SearchTextBox.IsEnabled;
            AccountsList.IsEnabled = !AccountsList.IsEnabled;

            NameTextBox.IsReadOnly = !NameTextBox.IsReadOnly;
            LoginTextBox.IsReadOnly = !LoginTextBox.IsReadOnly;
            PasswordTextBox.IsReadOnly = !PasswordTextBox.IsReadOnly;
            OtherTextBox.IsReadOnly = !OtherTextBox.IsReadOnly;

            OKButton.IsEnabled = !OKButton.IsEnabled;
            CancelButton.IsEnabled = !CancelButton.IsEnabled;
            PlusButton.IsEnabled = !PlusButton.IsEnabled;
            NameTextBox.Focus();

            if (AccountsList.SelectedIndex == -1)
            {
                NameTextBox.IsEnabled = !NameTextBox.IsEnabled;
                LoginTextBox.IsEnabled = !LoginTextBox.IsEnabled;
                PasswordTextBox.IsEnabled = !PasswordTextBox.IsEnabled;
                OtherTextBox.IsEnabled = !OtherTextBox.IsEnabled;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //Добавление записи
            AccountsList.SelectedIndex = -1;    //Снимаем выделение с listBox1, это вызовет событие listBox1_SelectedIndexChanged, которое почистит textBox'ы
            EditMode_ON_OFF();              //Переключились в режим редактирования
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            //Изменение текущей записи
            if (AccountsList.SelectedIndex != -1)
            {
                EditMode_ON_OFF();          //Переключились в режим редактирования
            }
            else
            {
                AttentionW win = new AttentionW();
                win.InformationText.Text = "No item selected from the list.";
                win.ShowDialog();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //Удаление текущей записи
            if (AccountsList.SelectedIndex != -1)
            {
                YesOrNot win = new YesOrNot();
                win.ShowDialog();
                if (win.DialogResult == true)
                {
                    int a;
                    if (AccountsList.SelectedIndex + 1 != AccountsList.Items.Count)
                        a = AccountsList.SelectedIndex;
                    else a = AccountsList.SelectedIndex - 1;
                    AccountsList.Items.Remove(AccountsList.SelectedItem);
                    AccountsList.SelectedIndex = a;
                    IsSaved = false;
                    IsUpdateIndex = false;
                }
            }
            else
            {
                AttentionW win = new AttentionW();
                win.InformationText.Text = "No item selected from the list.";
                win.ShowDialog();
            }
        }

        private void UpdateIndexes()
        {
            //Обновление индексов для поиска

            SearchIndexes = new List<int>();
            CurrentN = 0;
            if (SearchTextBox.Text != string.Empty)
            {
                int currentIndex = 0;
                foreach (object item in AccountsList.Items)
                {
                    if (item.ToString().ToLower().Contains(SearchTextBox.Text.ToLower()))
                        SearchIndexes.Add(currentIndex);
                    currentIndex++;
                }
                if (SearchIndexes.Count != 0)
                {
                    AccountsList.SelectedIndex = SearchIndexes[0];
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                    NextButton.IsEnabled = true;
                    PrevButton.IsEnabled = true;
                    label6.Text = string.Format("{0}/{1}", 1, SearchIndexes.Count);
                }
                else
                {
                    AccountsList.SelectedIndex = -1;
                    NextButton.IsEnabled = false;
                    PrevButton.IsEnabled = false;
                    label6.Text = string.Format("{0}/{1}", 0, SearchIndexes.Count);
                }
            }
            else
            {
                AccountsList.SelectedIndex = -1;
                NextButton.IsEnabled = false;
                PrevButton.IsEnabled = false;
                label6.Text = "0/0";
            }
            IsUpdateIndex = true;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Индексация и настройка видимости кнопки для очистки
            UpdateIndexes();
            if (SearchTextBox.Text != string.Empty) Clearbutton.Visibility = Visibility.Visible;
            else Clearbutton.Visibility = Visibility.Hidden;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //Сохранение данных в файл
            if (!IsSaved)
            {
                MemoryStream stream = new MemoryStream();
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(CorrectPassword);
                    foreach (DataAccount var in AccountsList.Items)
                    {
                        bw.Write(var.Name);
                        bw.Write(var.Login);
                        bw.Write(var.Password);
                        bw.Write(var.OtherInf);
                    }
                    IsSaved = true;
                }
                byte[] TextToFile = EncryptDecrypt.Encrypt(stream, 4);
                using (BinaryWriter bw = new BinaryWriter(File.Open(MyDocuments + @"\passdata.dat", FileMode.Create)))
                {
                    bw.Write(TextToFile);
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            SearchNext();
        }

        private void SearchNext()
        {
            //Переход к следующему элементу
            if (!IsUpdateIndex)
            {
                UpdateIndexes();
            }
            if (SearchIndexes.Count != 0)
            {
                CurrentN++;
                if (CurrentN < SearchIndexes.Count)
                {
                    AccountsList.SelectedIndex = SearchIndexes[CurrentN];
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                }
                else
                {
                    CurrentN = 0;
                    AccountsList.SelectedIndex = SearchIndexes[CurrentN];
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                }
                SearchTextBox.Focus();
                label6.Text = string.Format("{0}/{1}", CurrentN + 1, SearchIndexes.Count);
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            //Переход к предыдущему элементу
            if (!IsUpdateIndex)
            {
                UpdateIndexes();
            }
            if (SearchIndexes.Count != 0)
            {
                CurrentN--;
                if (CurrentN < 0)
                {
                    CurrentN = SearchIndexes.Count - 1;
                    AccountsList.SelectedIndex = SearchIndexes[CurrentN];
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                }
                else
                {
                    AccountsList.SelectedIndex = SearchIndexes[CurrentN];
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                }
                SearchTextBox.Focus();
                label6.Text = string.Format("{0}/{1}", CurrentN + 1, SearchIndexes.Count);
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //При нажатии кнопок в поисковом поле, если Enter, ищем следующий
            if (e.Key == Key.Enter)
            {
                if (SearchIndexes.Count != 0) SearchNext();
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //Применение изменений
            try
            {
                if (NameTextBox.Text == string.Empty || LoginTextBox.Text == string.Empty || PasswordTextBox.Text == string.Empty)
                {
                    throw new Exception("Enter information about Name, Login and Password");
                }
                EditMode_ON_OFF();
                DataAccount account = new DataAccount();
                account.Name = NameTextBox.Text;
                account.Login = LoginTextBox.Text;
                account.Password = PasswordTextBox.Text;
                account.OtherInf = OtherTextBox.Text;
                if (AccountsList.SelectedIndex == -1)
                {
                    AccountsList.Items.Add(account);    //Если добавляем
                    Sorting();
                    FindCurrent(account);
                }
                else
                {
                    AccountsList.Items.Remove(AccountsList.SelectedItem);   //Если изменяем
                    AccountsList.Items.Add(account);
                    Sorting();
                    //Ищем элемент, который добавился и является отсортированным (т.е. выделение при добавлении и сортировке в listBox1 снимается)
                    FindCurrent(account);
                }
                IsSaved = false;
                IsUpdateIndex = false;
            }
            catch (Exception E)
            {
                AttentionW win = new AttentionW();
                win.InformationText.Text = E.Message;
                win.ShowDialog();
            }
        }

        private void FindCurrent(object account)
        {
            foreach (object var in AccountsList.Items)
            {
                if (var == account)
                {
                    AccountsList.SelectedItem = var;
                    AccountsList.ScrollIntoView(AccountsList.SelectedItem);
                    return;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //Откат изменений
            EditMode_ON_OFF();
            SearchTextBox.Focus();
            if (AccountsList.Items.Count != 0 && AccountsList.SelectedIndex != -1)
            {
                DataAccount account = AccountsList.SelectedItem as DataAccount;
                NameTextBox.Text = account.Name;
                LoginTextBox.Text = account.Login;
                PasswordTextBox.Text = account.Password;
                OtherTextBox.Text = account.OtherInf;
            }
            if (AccountsList.Items.Count != 0 && AccountsList.SelectedIndex == -1)
            {
                NameTextBox.Text = string.Empty;
                LoginTextBox.Text = string.Empty;
                PasswordTextBox.Text = string.Empty;
                OtherTextBox.Text = string.Empty;
            }
        }

        private void Clearbutton_Click(object sender, RoutedEventArgs e)
        {
            //Очистка поискового поля, вызовет событие SearchTextBox_TextChanged
            SearchTextBox.Text = string.Empty;
            SearchTextBox.Focus();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            AuthDialog.IsOpen = true;
            AuthText.Text = WpfApp1.Properties.Resources.ChangePasswordContent;
            PasswordAccount.Password = "";
            HintAssist.SetHint(PasswordAccount, Properties.Resources.NewPassword);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Возникает при закрытии формы и спрашивает, сохранить ли
            if (!IsSaved)
            {
                YesOrNot win = new YesOrNot();
                win.textBlock.Text = "Save?";
                win.ShowDialog();
                if (win.DialogResult == true)
                {
                    MemoryStream stream = new MemoryStream();
                    using (BinaryWriter bw = new BinaryWriter(stream))
                    {
                        bw.Write(CorrectPassword);
                        foreach (DataAccount var in AccountsList.Items)
                        {
                            bw.Write(var.Name);
                            bw.Write(var.Login);
                            bw.Write(var.Password);
                            bw.Write(var.OtherInf);
                        }
                        IsSaved = true;
                    }
                    byte[] TextToFile = EncryptDecrypt.Encrypt(stream, 4);
                    using (BinaryWriter bw = new BinaryWriter(File.Open(MyDocuments + @"\passdata.dat", FileMode.Create)))
                    {
                        bw.Write(TextToFile);
                    }
                }
            }
        }

        private void AccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Просмотр информации о записи
            if (AccountsList.SelectedIndex != -1)
            {
                NameTextBox.IsEnabled = true;
                LoginTextBox.IsEnabled = true;
                PasswordTextBox.IsEnabled = true;
                OtherTextBox.IsEnabled = true;

                DataAccount account = AccountsList.SelectedItem as DataAccount;
                NameTextBox.Text = account.Name;
                LoginTextBox.Text = account.Login;
                PasswordTextBox.Text = account.Password;
                OtherTextBox.Text = account.OtherInf;
            }
            else
            {
                NameTextBox.IsEnabled = false;
                LoginTextBox.IsEnabled = false;
                PasswordTextBox.IsEnabled = false;
                OtherTextBox.IsEnabled = false;

                NameTextBox.Text = "";
                LoginTextBox.Text = "";
                PasswordTextBox.Text = "";
                OtherTextBox.Text = "";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Считывание данных из файла, инициализация переменных
            SearchIndexes = new List<int>();
            IsUpdateIndex = true;
            IsSaved = true;
            CurrentN = 0;

            if (Properties.Settings.Default.DesignTheme == "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml")
                toggleButton.IsChecked = true;
            if (Properties.Settings.Default.DesignTheme != string.Empty)
                Application.Current.Resources.MergedDictionaries[0].Source = new Uri(Properties.Settings.Default.DesignTheme);
            if (Properties.Settings.Default.PrimaryColor != string.Empty)
                Application.Current.Resources.MergedDictionaries[2].Source = new Uri(Properties.Settings.Default.PrimaryColor);
            if (Properties.Settings.Default.AccentColor != string.Empty)
                Application.Current.Resources.MergedDictionaries[3].Source = new Uri(Properties.Settings.Default.AccentColor);
            try
            {
                if(EncryptDecrypt.Decrypt(MyDocuments + @"\passdata.dat", 4) != null)
                {
                    MemoryStream stream = new MemoryStream(EncryptDecrypt.Decrypt(MyDocuments + @"\passdata.dat", 4));
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        br.ReadString();
                        DataAccount data;
                        while (br.PeekChar() != -1)
                        {
                            data = new DataAccount();
                            data.Name = br.ReadString();
                            data.Login = br.ReadString();
                            data.Password = br.ReadString();
                            data.OtherInf = br.ReadString();
                            AccountsList.Items.Add(data);
                        }
                    }
                }
            }
            catch (Exception E)
            {
                AttentionW attentionW = new AttentionW();
                attentionW.InformationText.Text = E.Message;
                attentionW.ShowDialog();
            }
            Sorting();
        }

        private void Sorting()
        {
            List<DataAccount> AllItems = new List<DataAccount>();
            foreach (DataAccount da in AccountsList.Items)
            {
                AllItems.Add(da);
            }
            AccountsList.Items.Clear();
            AllItems = AllItems.OrderBy(a => a.Name).ToList();
            foreach(DataAccount da in AllItems)
            {
                AccountsList.Items.Add(da);
            }
        }
        
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            /*!!!!!!!!!!!!!!!!!new PaletteHelper().SetLightDark(true);
             public ICommand ToggleBaseCommand { get; } = new AnotherCommandImplementation(o => ApplyBase((bool)o));

        private static void ApplyBase(bool isDark)
        {
            new PaletteHelper().SetLightDark(isDark);
        } ToggleBaseCommand -> Command = {Binding ToggleBaseCommand}*/

           //comment
           //develop comment
           // comment
           //develop comment

            if ((bool)toggleButton.IsChecked) Application.Current.Resources.MergedDictionaries[0].Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml", UriKind.RelativeOrAbsolute);
            else Application.Current.Resources.MergedDictionaries[0].Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml", UriKind.RelativeOrAbsolute);
            
            Properties.Settings.Default.DesignTheme = Application.Current.Resources.MergedDictionaries[0].Source.OriginalString;
            Properties.Settings.Default.Save();
        }

        string AccentColorString = "pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.{0}.xaml";
        string PrimaryColorString = "pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.{0}.xaml";

        int AccentIndex = 1;
        int PrimaryIndex = 18;

        private void AccentButton_Click(object sender, RoutedEventArgs e)
        {
            AccentIndex++;
            if (AccentIndex > 15) AccentIndex = 0;
            switch (AccentIndex)
            {
                case 0:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Amber"));
                        break;
                    }
                case 1:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Blue"));                   
                        break;
                    }
                case 2:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Cyan"));
                        break;
                    }
                case 3:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "DeepOrange"));
                        break;
                    }
                case 4:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "DeepPurple"));
                        break;
                    }
                case 5:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Green"));
                        break;
                    }
                case 6:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Indigo"));
                        break;
                    }
                case 7:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "LightBlue"));
                        break;
                    }
                case 8:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "LightGreen"));
                        break;
                    }
                case 9:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Lime"));
                        break;
                    }
                case 10:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Orange"));
                        break;
                    }
                case 11:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Pink"));
                        break;
                    }
                case 12:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Purple"));
                        break;
                    }
                case 13:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Red"));
                        break;
                    }
                case 14:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Teal"));
                        break;
                    }
                case 15:
                    {
                        Application.Current.Resources.MergedDictionaries[3].Source = new Uri(String.Format(AccentColorString, "Yellow"));
                        break;
                    }
            }

            Properties.Settings.Default.AccentColor = Application.Current.Resources.MergedDictionaries[3].Source.OriginalString;
            Properties.Settings.Default.Save();
        }
        private void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            PrimaryIndex++;
            if (PrimaryIndex > 15) PrimaryIndex = 0;
            switch (PrimaryIndex)
            {
                case 0:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Blue"));
                        break;
                    }
                case 1:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Amber"));
                        break;
                    }
                case 2:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "BlueGrey"));
                        break;
                    }
                case 3:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Brown"));
                        break;
                    }
                case 4:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Cyan"));
                        break;
                    }
                case 5:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "DeepOrange"));
                        break;
                    }
                case 6:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "DeepPurple"));
                        break;
                    }
                case 7:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Green"));
                        break;
                    }
                case 8:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Grey"));
                        break;
                    }
                case 9:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Indigo"));
                        break;
                    }
                case 10:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "LightBlue"));
                        break;
                    }
                case 11:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "LightGreen"));
                        break;
                    }
                case 12:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Lime"));
                        break;
                    }
                case 13:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Orange"));
                        break;
                    }
                case 14:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Pink"));
                        break;
                    }
                case 15:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Purple"));
                        break;
                    }
                case 16:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Red"));
                        break;
                    }
                case 17:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Teal"));
                        break;
                    }
                case 18:
                    {
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri(String.Format(PrimaryColorString, "Yellow"));
                        break;
                    }
            }
            Properties.Settings.Default.PrimaryColor = Application.Current.Resources.MergedDictionaries[2].Source.OriginalString;
            Properties.Settings.Default.Save();
        }

        private void OKButtonAuth_Click(object sender, RoutedEventArgs e)
        {
            if (CorrectPassword != PasswordAccount.Password)
            {
                Application.Current.Resources.MergedDictionaries[2].Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Red.xaml", UriKind.RelativeOrAbsolute);
                OKButtonAuth.Command = null;
                HintAssist.SetHint(PasswordAccount, Properties.Resources.IncorrectPassword);
            }
            else
            {
                if(Properties.Settings.Default.PrimaryColor != string.Empty)
                    Application.Current.Resources.MergedDictionaries[2].Source = new Uri(Properties.Settings.Default.PrimaryColor);
                OKButtonAuth.Command = DialogHost.CloseDialogCommand;
                HintAssist.SetHint(PasswordAccount, Properties.Resources.CorrectPassword);

                if (firstAuth != null) OKButtonAuth.Click -= firstAuth;
                OKButtonAuth.Click -= OKButtonAuth_Click;
                CancelButtonAuth.Click -= CancelButtonAuth_Click;
                OKButtonAuth.Click += delegate
                {
                    try
                    {
                        if (PasswordAccount.Password == string.Empty)
                            throw new Exception(Properties.Resources.EmptyPassword);
                        CorrectPassword = PasswordAccount.Password;
                        IsSaved = false;
                        OKButtonAuth.Command = DialogHost.CloseDialogCommand;

                    }
                    catch (Exception E)
                    {
                        HintAssist.SetHint(PasswordAccount, E.Message);
                        OKButtonAuth.Command = null;
                    }
                };
            }
        }

        private void CancelButtonAuth_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
