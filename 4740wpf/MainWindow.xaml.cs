using System;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;
using System.Windows;

namespace _4740wpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void searchButton_Click(object sender, EventArgs e)
        {
            string targetUser = usernameTextBox.Text;
            string currentDate = DateTime.Now.ToString("MM/dd/yyyy");

            string query = $@"
            <QueryList>
              <Query Id='0' Path='ForwardedEvents'>
                <Select Path='ForwardedEvents'>
                  *[System[(EventID=4740) and TimeCreated[timediff(@SystemTime) &lt;= 86400000]]]
                  and
                  *[EventData[Data[@Name='TargetUserName']='{targetUser}']]
                </Select>
              </Query>
            </QueryList>
            ";

            EventLogQuery eventLogQuery = new EventLogQuery("ForwardedEvents", PathType.LogName, query);
            EventLogReader eventLogReader = new EventLogReader(eventLogQuery);

            string result = "";
            for (EventRecord eventInstance = await Task.Run(() => eventLogReader.ReadEvent()); eventInstance != null; eventInstance = await Task.Run(() => eventLogReader.ReadEvent()))
            {
                result += "Событие 4740 найдено:" + Environment.NewLine;
                result += "Дата и время: " + eventInstance.TimeCreated + Environment.NewLine;
                result += "Имя пользователя: " + eventInstance.Properties[0].Value + Environment.NewLine;
                result += "Компьютер: " + eventInstance.Properties[1].Value + Environment.NewLine;
                result += "Сообщение: " + eventInstance.FormatDescription() + Environment.NewLine;
                result += "------------------------" + Environment.NewLine;
            }

            resultTextBox.Text = result;
        }
    }
}
