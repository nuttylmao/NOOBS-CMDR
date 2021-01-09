using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using NOOBS_CMDR.Extensions;
using NOOBS_CMDR.Commands;
using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Security.Principal;

namespace NOOBS_CMDR
{
    public partial class NOOBS_CMDR_Homepage : Window, IDropTarget, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Properties

        public string WindowTitle
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return "NOOBS CMDR v" + version;
            }
        }

        #endregion Properties

        #region Variables

        private OBSWebsocket _obs;
        public OBSWebsocket obs
        {
            get { return _obs; }
            set
            {
                if (value != _obs)
                {
                    _obs = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<CommandType> CommandTypes = new ObservableCollection<CommandType>();
        private ObservableCollection<Command> Commands = new ObservableCollection<Command>();
        private ObservableCollection<Command> CommandsCopyStack = new ObservableCollection<Command>();

        #endregion Variables

        #region Constructors

        public NOOBS_CMDR_Homepage()
        {
            InitializeComponent();
            this.DataContext = this;

            // Check that all OBSCommand files exist and that environment PATH variable is set up
            if (!OBSCommandExists())
            {

                // OBSCommand not found, so need to install it but this needs to be done in admin mode
                if (!IsAdministrator())
                {
                    MessageBox.Show(@"Please restart in admin mode to install OBSCommand.", "OBSCommand Not Installed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    System.Environment.Exit(1);
                }
                else
                {
                    // Ask user to select directory
                    const string message ="Before using NOOBS CMDR, you must install OBSCommand. Choose a directory to install OBSCommand.";
                    const string caption = "NOOBS CMDR Install";
                    var result = MessageBox.Show(message, caption,
                                                 MessageBoxButton.OKCancel,
                                                 MessageBoxImage.Question);

                    if (result == MessageBoxResult.OK)
                    {
                        // Check that all OBSCommand files exist in NOOBS CMDR folder
                        string sourcePath = $"{System.AppDomain.CurrentDomain.BaseDirectory}OBSCommand";

                        // If they don't exist, we can't install OBSCommand
                        if (!File.Exists($"{sourcePath}\\OBSCommand.exe") || !File.Exists($"{sourcePath}\\Newtonsoft.Json.dll") || !File.Exists($"{sourcePath}\\obs-websocket-dotnet.dll") || !File.Exists($"{sourcePath}\\websocket-sharp.dll"))
                        {
                            MessageBox.Show(@"Could not find OBSCommand Files. Please re-download NOOBS CMDR.", "OBSCommand Files Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
                            System.Environment.Exit(1);
                        }

                        // Ask user to select directory
                        System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
                        
                        // Default to the Program Files directory
                        dlg.SelectedPath = Environment.Is64BitOperatingSystem ? Environment.GetEnvironmentVariable("ProgramFiles(x86)") : Environment.GetEnvironmentVariable("ProgramFiles");

                        // User selects OK
                        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            string selectedPath = $"{dlg.SelectedPath}\\OBSCommand";

                            // Create OBSCommand folder automatically + copy all OBSCommand files
                            Directory.CreateDirectory($"{dlg.SelectedPath}\\OBSCommand");
                            File.Copy($"{sourcePath}\\OBSCommand.exe", $"{selectedPath}\\OBSCommand.exe", true);
                            File.Copy($"{sourcePath}\\Newtonsoft.Json.dll", $"{selectedPath}\\Newtonsoft.Json.dll", true);
                            File.Copy($"{sourcePath}\\obs-websocket-dotnet.dll", $"{selectedPath}\\obs-websocket-dotnet.dll", true);
                            File.Copy($"{sourcePath}\\websocket-sharp.dll", $"{selectedPath}\\websocket-sharp.dll", true);

                            // Check the environment PATH variable
                            string enviromentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                            
                            // If the selected directory does not exist in the PATH variable, we need to add it
                            if (!enviromentPath.Contains(selectedPath))
                            {
                                Environment.SetEnvironmentVariable("PATH", $"{enviromentPath};{selectedPath}", EnvironmentVariableTarget.Machine);
                            }

                            MessageBox.Show(@"NOOBS CMDR is now ready to use.", "OBSCommand Successfully Installed", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            System.Environment.Exit(1);
                        }
                    }
                    else
                    {
                        System.Environment.Exit(1);
                    }
                }
            }

            ServerText.Text = ConfigurationManager.AppSettings.Get("Server");
            PortText.Text = ConfigurationManager.AppSettings.Get("Port");
            PasswordText.Password = ConfigurationManager.AppSettings.Get("Password");

            obs = new OBSWebsocket();
            obs.Connected += Obs_Connected;
            obs.Disconnected += Obs_Disconnected;

            Connect();

            CommandTypeList.ItemsSource = CommandTypes;
            CommandListBox.ItemsSource = Commands;

            CollectionView view = CollectionViewSource.GetDefaultView(CommandTypeList.ItemsSource) as CollectionView;
            view.Filter = CommandTypeListFilter;

            CreateActionTypes();
        }

        #region Filters

        private void Searchbar_SearchTextChanged(object sender, EventArgs e)
        {
            if (CommandTypeList.ItemsSource != null)
            {
                CollectionViewSource.GetDefaultView(CommandTypeList.ItemsSource).Refresh();
            }
        }

        public bool CommandTypeListFilter(object commandType)
        {
            if (string.IsNullOrEmpty(CommandTypeListSearch.Text))
            {
                return true;
            }
            else
            {
                return (
                    (commandType as CommandType).commandTypeDesc.ToString().IndexOf(CommandTypeListSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        #endregion Filters

        #endregion Constructors

        #region Functions

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public bool OBSCommandExists()
        {
            var enviromentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

            var paths = enviromentPath.Split(';');
            var exePath = paths.Select(x => Path.Combine(x, "OBSCommand.exe"))
                               .Where(x => File.Exists(x))
                               .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(exePath))
            {
                return false;
            }

            string path = exePath.Replace(@"\OBSCommand.exe", "");

            return
                File.Exists($"{path}\\OBSCommand.exe")
                && File.Exists($"{path}\\Newtonsoft.Json.dll")
                && File.Exists($"{path}\\obs-websocket-dotnet.dll")
                && File.Exists($"{path}\\websocket-sharp.dll");
        }

        private void CreateActionTypes()
        {
            CommandTypes.Add(new CommandType("Stream", CommandType.Type.Stream));
            CommandTypes.Add(new CommandType("Record", CommandType.Type.Record));
            CommandTypes.Add(new CommandType("Profile", CommandType.Type.Profile));
            CommandTypes.Add(new CommandType("Scene", CommandType.Type.Scene));
            CommandTypes.Add(new CommandType("Source", CommandType.Type.Source));
            CommandTypes.Add(new CommandType("Filter", CommandType.Type.Filter));
            CommandTypes.Add(new CommandType("Audio", CommandType.Type.Audio));
            CommandTypes.Add(new CommandType("Transition", CommandType.Type.Transition));
            CommandTypes.Add(new CommandType("Studio Mode", CommandType.Type.StudioMode));
            CommandTypes.Add(new CommandType("Screenshot", CommandType.Type.Screenshot));
            CommandTypes.Add(new CommandType("Replay Buffer", CommandType.Type.ReplayBuffer));
            CommandTypes.Add(new CommandType("Projector", CommandType.Type.Projector));
            CommandTypes.Add(new CommandType("Delay", CommandType.Type.Delay));
            CommandTypes.Add(new CommandType("Custom", CommandType.Type.Custom));
        }

        #endregion Functions

        #region Page Actions

        private void CommandList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Deselect when click outside of items
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ListBoxItem))
                CommandListBox.UnselectAll();
        }

        private void CommandTypeList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;

            if (item == null)
                return;

            if (item.Content.GetType() != typeof(CommandType))
                return;

            CommandType commandType = (CommandType)item.Content;

            switch (commandType.commandTypeID)
            {
                case CommandType.Type.Stream:
                    Commands.Add(new StreamCommand(obs));
                    break;
                case CommandType.Type.Record:
                    Commands.Add(new RecordingCommand(obs));
                    break;
                case CommandType.Type.Profile:
                    Commands.Add(new ProfileCommand(obs));
                    break;
                case CommandType.Type.Scene:
                    Commands.Add(new SceneCommand(obs));
                    break;
                case CommandType.Type.Source:
                    Commands.Add(new SourceCommand(obs));
                    break;
                case CommandType.Type.Filter:
                    Commands.Add(new FilterCommand(obs));
                    break;
                case CommandType.Type.Audio:
                    Commands.Add(new AudioCommand(obs));
                    break;
                case CommandType.Type.Transition:
                    Commands.Add(new TransitionCommand(obs));
                    break;
                case CommandType.Type.StudioMode:
                    Commands.Add(new StudioModeCommand(obs));
                    break;
                case CommandType.Type.Screenshot:
                    Commands.Add(new ScreenshotCommand(obs));
                    break;
                case CommandType.Type.ReplayBuffer:
                    Commands.Add(new ReplayBufferCommand(obs));
                    break;
                case CommandType.Type.Projector:
                    Commands.Add(new ProjectorCommand(obs));
                    break;
                case CommandType.Type.Delay:
                    Commands.Add(new DelayCommand(obs));
                    break;
                default:
                    Commands.Add(new CustomCommand(obs));
                    break;
            }

            // Scroll to end of list
            CommandListBox.ScrollIntoView(CommandListBox.Items[CommandListBox.Items.Count - 1]);
        }

        private void CommandListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteCommands(CommandListBox.SelectedItems.OfType<Command>().ToList());
            }
        }

        #endregion Page Actions

        #region Connect

        private void Connect()
        {
            if (!obs.IsConnected)
            {
                string server = ServerText.Text;
                string port = PortText.Text;
                string password = PasswordText.Password;

                try
                {
                    obs.Connect(@"ws://" + server + ":" + port, password);
                }
                catch (AuthFailureException)
                {
                    MessageBox.Show("Authentication failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                catch (ErrorResponseException ex)
                {
                    MessageBox.Show("Connect failed : " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connect failed : " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            else
            {
                obs.Disconnect();
            }
        }

        private void Obs_Connected(object sender, EventArgs e)
        {
            OnPropertyChanged("obs");

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["Server"].Value = ServerText.Text;
            configuration.AppSettings.Settings["Port"].Value = PortText.Text;
            configuration.AppSettings.Settings["Password"].Value = PasswordText.Password;
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }

        private void Obs_Disconnected(object sender, EventArgs e)
        {
            OnPropertyChanged("obs");
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        #endregion

        #region Test Command

        private void TestCommands(List<Command> commandList)
        {
            if (!OBSCommandExists())
            {
                MessageBox.Show(@"Please restart in admin mode to install OBSCommand.", "OBSCommand Not Installed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            string strCmdText;
            strCmdText = string.Format(@"/server={0}:{1} /password=""{2}""", ServerText.Text, PortText.Text, PasswordText.Password);

            foreach (Command command in commandList)
            {
                Console.WriteLine(command.ToString());
                strCmdText += " " + command.ToString().ReplaceTimestamp();
            }

            Console.WriteLine(strCmdText);

            var enviromentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

            var paths = enviromentPath.Split(';');
            var exePath = paths.Select(x => Path.Combine(x, "OBSCommand.exe"))
                               .Where(x => File.Exists(x))
                               .FirstOrDefault();

            Process process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = exePath,
                    Arguments = strCmdText
                }
            };
            process.Start();
        }

        private void TestCommandBtn_Click(object sender, RoutedEventArgs e)
        {
            TestCommands(Commands.ToList());
        }

        private void Test_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TestCommands(CommandListBox.SelectedItems.OfType<Command>().ToList());
        }

        #endregion Test Command

        #region Copy Commands

        private void CopyCommands(List<Command> commandList)
        {
            CommandsCopyStack.Clear();

            List<Command> reverseList = commandList;
            reverseList.Reverse();

            foreach (var command in reverseList)
                CommandsCopyStack.Add(command.Clone());
        }

        private void Copy_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            CopyCommands(CommandListBox.SelectedItems.OfType<Command>().ToList());
        }

        #endregion Copy Commands

        #region Paste Commands

        private void PasteCommands(int row)
        {
            foreach (var command in CommandsCopyStack)
                Commands.Insert(row, command.Clone());
        }

        private void Paste_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int row = 0;

            if (CommandListBox.SelectedItems.Count <= 0)
            {
                row = CommandListBox.Items.Count;
            }
            else if (CommandListBox.SelectedItems.Count == 1)
            {
                row = CommandListBox.SelectedIndex + 1;
            }
            else if (CommandListBox.SelectedItems.Count > 1)
            {
                foreach (var item in CommandListBox.SelectedItems)
                {
                    row = CommandListBox.Items.IndexOf(item) > row ? CommandListBox.Items.IndexOf(item) : row;
                }

                row = row + 1;
            }

            PasteCommands(row);
        }

        #endregion Paste Commands

        #region Delete Commands

        private void DeleteCommands(List<Command> commandList)
        {
            foreach (var command in commandList)
                Commands.Remove(command);
        }

        private void Delete_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteCommands(CommandListBox.SelectedItems.OfType<Command>().ToList());
        }

        #endregion Delete Commands

        #region Duplicate Commands

        private void DuplicateCommands(List<Command> commandList)
        {
            int row = 0;

            if (CommandListBox.SelectedItems.Count <= 0)
            {
                row = CommandListBox.Items.Count;
            }
            else if (CommandListBox.SelectedItems.Count == 1)
            {
                row = CommandListBox.SelectedIndex + 1;
            }
            else if (CommandListBox.SelectedItems.Count > 1)
            {
                foreach (var item in CommandListBox.SelectedItems)
                {
                    row = CommandListBox.Items.IndexOf(item) > row ? CommandListBox.Items.IndexOf(item) : row;
                }

                row = row + 1;
            }

            List<Command> reverseList = commandList;
            reverseList.Reverse();

            foreach (var command in reverseList)
                Commands.Insert(row, command.Clone());
        }

        private void Duplicate_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DuplicateCommands(CommandListBox.SelectedItems.OfType<Command>().ToList());
        }

        #endregion Duplicate Commands

        #region Export to Script

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Batch Script (*.bat)|*.bat|VBS Script (*.vbs)|*.vbs";
            if (dlg.ShowDialog() == true)
            {
                string path = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf("\\") + 1);
                string filename = Path.GetFileNameWithoutExtension(dlg.FileName);
                File.WriteAllText(string.Format(@"{0}\\{1}.bat", path, filename), CommandsToBatch(Commands.ToList()));
                File.WriteAllText(string.Format(@"{0}\\{1}.vbs", path, filename), CommandsToVBS(Commands.ToList()));
            }
        }

        private string CommandsToBatch(List<Command> commandList)
        {
            string strCmdText = string.Format(@"::::: GENERATED WITH NOOBS CMDR v{0} :::::", typeof(NOOBS_CMDR_Homepage).Assembly.GetName().Version);
            strCmdText += Environment.NewLine;
            strCmdText += Environment.NewLine;

            // Need to set up date variable for screenshots
            if (commandList.OfType<ScreenshotCommand>().Where(x => x.createNewFile).ToList().Count > 0)
            {
                strCmdText += @"FOR /f ""tokens=2 delims=="" %%G in ('wmic os get localdatetime /value') do set datetime=%%G";
                strCmdText += Environment.NewLine;
                strCmdText += @"SET hh=%time:~0,2%";
                strCmdText += Environment.NewLine;
                strCmdText += @"SET hh=%hh: =0%";
                strCmdText += Environment.NewLine;
                strCmdText += @"SET datetime=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2% %hh%-%time:~3,2%-%time:~6,2%";
                strCmdText += Environment.NewLine;
                strCmdText += Environment.NewLine;
            }
            
            strCmdText += string.Format(@"OBSCommand.exe /server={0}:{1} /password=""{2}""", ServerText.Text, PortText.Text, PasswordText.Password);

            foreach (Command command in commandList)
            {
                strCmdText += " " + command.ToString().ReplaceTimestamp(StringExtensions.TimestampType.Batch);
            }

            return strCmdText;
        }

        private string CommandsToVBS(List<Command> commandList)
        {
            string strCmdText = string.Format(@"''''' GENERATED WITH NOOBS CMDR v{0} '''''", typeof(NOOBS_CMDR_Homepage).Assembly.GetName().Version);
            strCmdText += Environment.NewLine;
            strCmdText += Environment.NewLine;

            // Need to set up date variable for screenshots
            if (commandList.OfType<ScreenshotCommand>().Where(x => x.createNewFile).ToList().Count > 0)
            {
                strCmdText += @"Dim g_oSB : Set g_oSB = CreateObject(""System.Text.StringBuilder"")";
                strCmdText += Environment.NewLine;
                strCmdText += @"Function sprintf(sFmt, aData)";
                strCmdText += Environment.NewLine;
                strCmdText += @"   g_oSB.AppendFormat_4 sFmt, (aData)";
                strCmdText += Environment.NewLine;
                strCmdText += @"   sprintf = g_oSB.ToString()";
                strCmdText += Environment.NewLine;
                strCmdText += @"   g_oSB.Length = 0";
                strCmdText += Environment.NewLine;
                strCmdText += @"End Function";
                strCmdText += Environment.NewLine;
                strCmdText += @"Dim datetime : datetime = now()";
                strCmdText += Environment.NewLine;
                strCmdText += Environment.NewLine;
            }

            strCmdText += @"Set WshShell = CreateObject(""WScript.Shell"")";
            strCmdText += Environment.NewLine;
            strCmdText += string.Format(@"WshShell.Run ""OBSCommand.exe /server={0}:{1} /password=""""{2}""""", ServerText.Text, PortText.Text, PasswordText.Password);

            foreach (Command command in commandList)
            {
                strCmdText += " " + command.ToString().Replace(@"""", @"""""").ReplaceTimestamp(StringExtensions.TimestampType.VBS);
            }

            strCmdText += @""", 0";

            return strCmdText;
        }

        #endregion Export to Script

        #region Import Script

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Batch Script (*.bat)|*.bat|VBS Script (*.vbs)|*.vbs";
            if (dlg.ShowDialog() == true)
            {
                if (dlg.FileName.ToLower().EndsWith(".bat") || dlg.FileName.ToLower().EndsWith(".vbs"))
                {
                    string contents;

                    //Read the contents of the file into a stream
                    var fileStream = dlg.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        contents = reader.ReadToEnd();
                    }

                    if (dlg.FileName.ToLower().EndsWith(".bat"))
                    {
                        ImportFromBatch(contents);
                    }
                    else if (dlg.FileName.ToLower().EndsWith(".vbs"))
                    {
                        ImportFromVBS(contents);
                    }
                }
                else
                {
                    // Do nothing
                }
            }
        }

        private void ImportFromBatch(string contents)
        {
            Commands.Clear();

            string[] lines = contents.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            foreach (string line in lines)
            {
                if (line.StartsWith("OBSCommand.exe"))
                {

                    string[] contentArray = Regex.Split(line, "[ ](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    foreach (string cmdText in contentArray)
                    {
                        if (cmdText.StartsWith("OBSCommand.exe"))
                        {
                            // Do nothing
                        }
                        else if (cmdText.StartsWith("/server"))
                        {
                            // Do nothing
                        }
                        else if (cmdText.StartsWith("/password"))
                        {
                            // Do nothing
                        }
                        else if (cmdText == "/startstream")
                        {
                            Commands.Add(new StreamCommand(obs)
                            {
                                streamingStatus = StreamCommand.Status.Start
                            });
                        }
                        else if (cmdText == "/stopstream")
                        {
                            Commands.Add(new StreamCommand(obs)
                            {
                                streamingStatus = StreamCommand.Status.Stop
                            });
                        }
                        else if (cmdText == "/command=StartStopStreaming")
                        {
                            Commands.Add(new StreamCommand(obs)
                            {
                                streamingStatus = StreamCommand.Status.Toggle
                            });
                        }
                        else if (cmdText == "/startrecording")
                        {
                            Commands.Add(new RecordingCommand(obs)
                            {
                                recordingStatus = RecordingCommand.Status.Start
                            });
                        }
                        else if (cmdText == "/stoprecording")
                        {
                            Commands.Add(new RecordingCommand(obs)
                            {
                                recordingStatus = RecordingCommand.Status.Stop
                            });
                        }
                        else if (cmdText == "/command=StartStopRecording")
                        {
                            Commands.Add(new RecordingCommand(obs)
                            {
                                recordingStatus = RecordingCommand.Status.Toggle
                            });
                        }
                        else if (cmdText == "/command=PauseRecording")
                        {
                            Commands.Add(new RecordingCommand(obs)
                            {
                                recordingStatus = RecordingCommand.Status.Pause
                            });
                        }
                        else if (cmdText == "/command=ResumeRecording")
                        {
                            Commands.Add(new RecordingCommand(obs)
                            {
                                recordingStatus = RecordingCommand.Status.Resume
                            });
                        }
                        else if (cmdText.StartsWith("/profile="))
                        {
                            Commands.Add(new ProfileCommand(obs)
                            {
                                profileName = cmdText.RemoveBeforeChar('=').RemoveQuotes()
                            });
                        }
                        else if (cmdText.StartsWith("/scene="))
                        {
                            Commands.Add(new SceneCommand(obs)
                            {
                                sceneName = cmdText.RemoveBeforeChar('=').RemoveQuotes()
                            });
                        }
                        else if (cmdText.StartsWith("/showsource=") || cmdText.StartsWith("/hidesource=") || cmdText.StartsWith("/togglesource="))
                        {
                            string[] sourceArray = Regex.Split(cmdText.RemoveBeforeChar('='), "[/](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                            SourceCommand.State sourceState;

                            if (cmdText.StartsWith("/showsource="))
                            {
                                sourceState = SourceCommand.State.Show;
                            }
                            else if (cmdText.StartsWith("/hidesource="))
                            {
                                sourceState = SourceCommand.State.Hide;
                            }
                            else
                            {
                                sourceState = SourceCommand.State.Toggle;
                            }


                            if (sourceArray.Count() == 1)
                            {
                                Commands.Add(new SourceCommand(obs)
                                {
                                    sourceState = sourceState,
                                    sourceName = sourceArray[0].RemoveQuotes()
                                });
                            }
                            else if (sourceArray.Count() == 2)
                            {
                                Commands.Add(new SourceCommand(obs)
                                {
                                    sourceState = sourceState,
                                    sceneName = sourceArray[0].RemoveQuotes(),
                                    sourceName = sourceArray[1].RemoveQuotes()
                                });
                            }
                        }
                        else if (cmdText.StartsWith("/command=SetSourceFilterVisibility,"))
                        {
                            string parString = cmdText.RemoveBeforeChar(',');
                            string[] pars = Regex.Split(parString, "[,](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                            string sourceName = "";
                            string filterName = "";
                            FilterCommand.State filterState = FilterCommand.State.Hide;

                            foreach (string par in pars)
                            {
                                if (par.StartsWith("sourceName"))
                                {
                                    sourceName = par.RemoveBeforeChar('=').RemoveQuotes();
                                }
                                else if (par.StartsWith("filterName"))
                                {
                                    filterName = par.RemoveBeforeChar('=').RemoveQuotes();
                                }
                                else if (par.StartsWith("filterEnabled"))
                                {
                                    filterState = par.RemoveBeforeChar('=').RemoveQuotes() == "True" ? FilterCommand.State.Show : FilterCommand.State.Hide;
                                }
                            }

                            Commands.Add(new FilterCommand(obs)
                            {
                                sourceName = sourceName,
                                filterName = filterName,
                                filterState = filterState
                            });
                        }
                        else if (cmdText.StartsWith("/mute="))
                        {
                            Commands.Add(new AudioCommand(obs)
                            {
                                audioState = AudioCommand.State.mute,
                                sourceName = cmdText.RemoveBeforeChar('=').RemoveQuotes()
                            });
                        }
                        else if (cmdText.StartsWith("/unmute="))
                        {
                            Commands.Add(new AudioCommand(obs)
                            {
                                audioState = AudioCommand.State.unmute,
                                sourceName = cmdText.RemoveBeforeChar('=').RemoveQuotes()
                            });
                        }
                        else if (cmdText.StartsWith("/toggleaudio="))
                        {
                            Commands.Add(new AudioCommand(obs)
                            {
                                audioState = AudioCommand.State.toggle,
                                sourceName = cmdText.RemoveBeforeChar('=').RemoveQuotes()
                            });
                        }
                        else if (cmdText.StartsWith("/setvolume="))
                        {
                            string parString = cmdText.RemoveBeforeChar('=');
                            string[] pars = Regex.Split(parString, "[,](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                            string sourceName = pars[0].RemoveQuotes();
                            int volume = int.Parse(pars[1]);
                            int delay = int.Parse(pars[2]);

                            Commands.Add(new AudioCommand(obs)
                            {
                                audioState = AudioCommand.State.setVolume,
                                sourceName = sourceName,
                                volume = volume,
                                delay = delay
                            });
                        }
                        else if (cmdText.StartsWith("/command=SetAudioMonitorType,"))
                        {
                            string parString = cmdText.RemoveBeforeChar(',');
                            string[] pars = Regex.Split(parString, "[,](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                            string sourceName = "";
                            AudioCommand.MonitoringType monitorType = AudioCommand.MonitoringType.none;

                            foreach (string par in pars)
                            {
                                if (par.StartsWith("sourceName"))
                                {
                                    sourceName = par.RemoveBeforeChar('=').RemoveQuotes();
                                }
                                else if (par.StartsWith("monitorType"))
                                {
                                    if (par.RemoveBeforeChar('=') == "none")
                                    {
                                        monitorType = AudioCommand.MonitoringType.none;
                                    }
                                    else if (par.RemoveBeforeChar('=') == "monitorOnly")
                                    {
                                        monitorType = AudioCommand.MonitoringType.monitorOnly;
                                    }
                                    else if (par.RemoveBeforeChar('=') == "monitorAndOutput")
                                    {
                                        monitorType = AudioCommand.MonitoringType.monitorAndOutput;
                                    }
                                }
                            }

                            Commands.Add(new AudioCommand(obs)
                            {
                                audioState = AudioCommand.State.setMonitoringType,
                                sourceName = sourceName,
                                monitoringType = monitorType
                            });
                        }
                        else if (cmdText.StartsWith("/command=SetCurrentTransition,"))
                        {
                            string parString = cmdText.RemoveBeforeChar(',');
                            string[] pars = Regex.Split(parString, "[,](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                            string transitionName = pars[0].RemoveBeforeChar('=').RemoveQuotes();

                            Commands.Add(new TransitionCommand(obs)
                            {
                                transitionState = TransitionCommand.State.SetCurrentTransition,
                                transitionName = transitionName
                            });
                        }
                        else if (cmdText.StartsWith("/command=SetTransitionDuration,"))
                        {
                            string parString = cmdText.RemoveBeforeChar(',');
                            string[] pars = Regex.Split(parString, "[,](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                            int duration = int.Parse(pars[0].RemoveBeforeChar('='));

                            Commands.Add(new TransitionCommand(obs)
                            {
                                transitionState = TransitionCommand.State.SetTransitionDuration,
                                duration = duration
                            });
                        }
                        else if (cmdText.StartsWith("/command=SetSceneTransitionOverride,"))
                        {
                            string parString = cmdText.RemoveBeforeChar(',');
                            string[] pars = Regex.Split(parString, "[,](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                            string sceneName = "";
                            string transitionName = "";
                            int transitionDuration = 100;

                            foreach (string par in pars)
                            {
                                if (par.StartsWith("sceneName"))
                                {
                                    sceneName = par.RemoveBeforeChar('=').RemoveQuotes();
                                }
                                else
                                if (par.StartsWith("transitionName"))
                                {
                                    transitionName = par.RemoveBeforeChar('=').RemoveQuotes();
                                }
                                else if (par.StartsWith("transitionDuration"))
                                {
                                    transitionDuration = int.Parse(par.RemoveBeforeChar('='));
                                }
                            }

                            Commands.Add(new TransitionCommand(obs)
                            {
                                transitionState = TransitionCommand.State.SetSceneTransitionOverride,
                                sceneName = sceneName,
                                transitionName = transitionName,
                                duration = transitionDuration
                            });
                        }
                        else if (cmdText.StartsWith("/command=RemoveSceneTransitionOverride,"))
                        {
                            string parString = cmdText.RemoveBeforeChar(',');
                            string[] pars = Regex.Split(parString, "[,](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                            string sceneName = pars[0].RemoveBeforeChar('=').RemoveQuotes();

                            Commands.Add(new TransitionCommand(obs)
                            {
                                transitionState = TransitionCommand.State.RemoveTransitionOverride,
                                sceneName = sceneName
                            });
                        }
                        else if (cmdText == "/command=EnableStudioMode")
                        {
                            Commands.Add(new StudioModeCommand(obs)
                            {
                                studioModeState = StudioModeCommand.State.Enable
                            });
                        }
                        else if (cmdText == "/command=DisableStudioMode")
                        {
                            Commands.Add(new StudioModeCommand(obs)
                            {
                                studioModeState = StudioModeCommand.State.Disable
                            });
                        }
                        else if (cmdText == "/command=ToggleStudioMode")
                        {
                            Commands.Add(new StudioModeCommand(obs)
                            {
                                studioModeState = StudioModeCommand.State.Toggle
                            });
                        }
                        else if (cmdText.StartsWith("/command=TakeSourceScreenshot,"))
                        {
                            string parString = cmdText.RemoveBeforeChar(',');
                            string[] pars = Regex.Split(parString, "[,](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                            string sourceName = "";
                            string saveToFilePath = "";

                            foreach (string par in pars)
                            {
                                if (par.StartsWith("sourceName"))
                                {
                                    sourceName = par.RemoveBeforeChar('=').RemoveQuotes();
                                }
                                else if (par.StartsWith("saveToFilePath"))
                                {
                                    saveToFilePath = par.RemoveBeforeChar('=').RemoveQuotes();
                                }
                            }

                            bool includesDate = saveToFilePath.IncludesDate();

                            saveToFilePath = saveToFilePath.RemoveDate();

                            if (obs.GetSceneList().Scenes.ConvertAll(x => x.Name).Contains(sourceName))
                            {
                                Commands.Add(new ScreenshotCommand(obs)
                                {
                                    screenshotType = ScreenshotCommand.Type.Scene,
                                    sceneName = sourceName,
                                    saveToFilePath = saveToFilePath,
                                    createNewFile = includesDate
                                });
                            }
                            else
                            {
                                Commands.Add(new ScreenshotCommand(obs)
                                {
                                    screenshotType = ScreenshotCommand.Type.Source,
                                    sourceName = sourceName,
                                    saveToFilePath = saveToFilePath,
                                    createNewFile = includesDate
                                });
                            }
                        }
                        else if (cmdText == "/command=StartReplayBuffer")
                        {
                            Commands.Add(new ReplayBufferCommand(obs)
                            {
                                replayBufferState = ReplayBufferCommand.State.Start
                            });
                        }
                        else if (cmdText == "/command=StopReplayBuffer")
                        {
                            Commands.Add(new ReplayBufferCommand(obs)
                            {
                                replayBufferState = ReplayBufferCommand.State.Stop
                            });
                        }
                        else if (cmdText == "/command=StartStopReplayBuffer")
                        {
                            Commands.Add(new ReplayBufferCommand(obs)
                            {
                                replayBufferState = ReplayBufferCommand.State.Toggle
                            });
                        }
                        else if (cmdText == "/command=SaveReplayBuffer")
                        {
                            Commands.Add(new ReplayBufferCommand(obs)
                            {
                                replayBufferState = ReplayBufferCommand.State.Save
                            });
                        }
                        else if (cmdText.StartsWith("/command=OpenProjector,"))
                        {
                            string parString = cmdText.RemoveBeforeChar(',');
                            string[] pars = Regex.Split(parString, "[,](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                            ProjectorCommand.Type projectorType = ProjectorCommand.Type.Preview;
                            int? monitor = null;
                            string name = "";

                            foreach (string par in pars)
                            {
                                if (par.StartsWith("type"))
                                {
                                    if (par.RemoveBeforeChar('=') == "Preview")
                                    {
                                        projectorType = ProjectorCommand.Type.Preview;
                                    }
                                    else if (par.RemoveBeforeChar('=') == "StudioProgram")
                                    {
                                        projectorType = ProjectorCommand.Type.StudioProgram;
                                    }
                                    else if (par.RemoveBeforeChar('=') == "Scene")
                                    {
                                        projectorType = ProjectorCommand.Type.Scene;
                                    }
                                    else if (par.RemoveBeforeChar('=') == "Source")
                                    {
                                        projectorType = ProjectorCommand.Type.Source;
                                    }
                                    else if (par.RemoveBeforeChar('=') == "MultiView")
                                    {
                                        projectorType = ProjectorCommand.Type.MultiView;
                                    }
                                }
                                else if (par.StartsWith("monitor"))
                                {
                                    monitor = int.Parse(par.RemoveBeforeChar('='));
                                }
                                else if (par.StartsWith("name"))
                                {
                                    name = par.RemoveBeforeChar('=').RemoveQuotes();
                                }
                            }
                            if (obs.GetSceneList().Scenes.ConvertAll(x => x.Name).Contains(name))
                            {
                                Commands.Add(new ProjectorCommand(obs)
                                {
                                    projectorType = projectorType,
                                    monitor = monitor,
                                    isFullscreen = monitor != null ? true : false,
                                    sceneName = name
                                });
                            }
                            else
                            {
                                Commands.Add(new ProjectorCommand(obs)
                                {
                                    projectorType = projectorType,
                                    monitor = monitor,
                                    isFullscreen = monitor != null ? true : false,
                                    sourceName = name
                                });
                            }
                        }
                        else if (cmdText.StartsWith("/delay="))
                        {
                            Commands.Add(new DelayCommand(obs)
                            {
                                delay = (int)(decimal.Parse(cmdText.RemoveBeforeChar('=')) * 1000)
                            });
                        }
                        else
                        {
                            Commands.Add(new CustomCommand(obs)
                            {
                                command = cmdText
                            });
                        }
                    }
                }
            }
        }

        private void ImportFromVBS(string contents)
        {
            string[] lines = contents.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            foreach (string line in lines)
            {
                if (line.StartsWith("WshShell.Run ") && line.EndsWith(", 0"))
                {
                    string cmd = line.RemoveDate();
                    cmd = cmd.Remove(cmd.Length - 4);
                    cmd = cmd.Substring(14);

                    cmd = cmd.Replace(@"""""", @"""");

                    ImportFromBatch(cmd);
                }
            }
        }

        #endregion Import Script

        #region IDropTarget

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (((ObservableCollection<Command>)dropInfo.TargetCollection).Contains(dropInfo.Data))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            int oldIndex = ((ObservableCollection<Command>)dropInfo.TargetCollection).IndexOf((Command)dropInfo.Data);
            int newIndex = dropInfo.InsertIndex <= oldIndex ? dropInfo.InsertIndex : dropInfo.InsertIndex - 1;

            ((ObservableCollection<Command>)dropInfo.TargetCollection).RemoveAt(oldIndex);
            ((ObservableCollection<Command>)dropInfo.TargetCollection).Insert(newIndex, (Command)dropInfo.Data);
        }

        #endregion IDropTarget
    }
}
