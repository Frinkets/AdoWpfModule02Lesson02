using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace AdoWpfModule02Lesson02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _connString = "";
        public MainWindow()
        {
            InitializeComponent();

            //CommandBehaviorComboBox

            Array aComm = Enum.GetValues(typeof(CommandBehavior));
            for (int i = 0; i < aComm.Length; i++)
            {
                CommandBehaviorComboBox.Items.Add(new ComboBoxItem() { Content = aComm.GetValue(i).ToString() });
            }

            _connString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"]
                    .ConnectionString;
        }

        private void ErButton_Click(object sender, RoutedEventArgs e)
        {

            SqlParameter param =
                new SqlParameter("@intTabID", SqlDbType.Int)
                { Value = NameTextBox.Text };

            using (SqlConnection con =
                new SqlConnection(_connString))
            {
                con.StateChange += cn_ChangeStatus;

                con.Open();
                SqlCommand cmd =
                    new SqlCommand();
                cmd.Connection = con;
                //cmd.CommandText = "Select * from AccessTab where intTabID = @intTabID";
                //cmd.Parameters.Add(param);
                string query = "";
                query += "select * from newEquipment; ";

                #region 
                query += "Select * from OrderTab";

                //if (!string.IsNullOrEmpty(IdTextBox.Text)
                //    || !string.IsNullOrEmpty(NameTextBox.Text))
                //    query = query + " WHERE 1=1";

                //if (!string.IsNullOrEmpty(IdTextBox.Text))
                //    query = query + " AND intTabID = " + IdTextBox.Text;

                //if (!string.IsNullOrEmpty(NameTextBox.Text))
                //    query = query + " OR strTabName = '" + NameTextBox.Text + "'";

                cmd.CommandText = query;
                #endregion

                //2 запрос
                var rdr = cmd.ExecuteReader(CommandBehavior.Default);
                List<AccessTab> accessTabs = new List<AccessTab>();
                List<Equipment> equipments = new List<Equipment>();
                int select = 0;
                do
                {


                    if (select == 0)
                    {
                        while (rdr.Read())
                        {
                            Equipment equipment = new Equipment
                            {
                                intGarageRoom = rdr[0].ToString(),
                                strManufYear = rdr[1].ToString()
                            };
                            equipments.Add(equipment);
                        }
                    }
                    else
                    {
                        while (rdr.Read())
                        {
                            AccessTab accessTab =
                                new AccessTab
                                {
                                    intTabID = Int32.Parse(rdr[0].ToString()),
                                    strTabName = rdr[1].ToString()
                                };

                            accessTabs.Add(accessTab);
                        }
                    }
                    select++;
                } while (rdr.NextResult());


                NewEquipmentListView.ItemsSource = equipments;
                AccessTabListView.ItemsSource = accessTabs;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con =
                new SqlConnection(_connString);
            con.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText =
                "Select count(*) from AccessTab";
            string res = cmd.ExecuteScalar().ToString();
            MessageER.Content = res;

            con.Close();
        }

        private void cn_ChangeStatus(object sender,
            StateChangeEventArgs e)
        {
            MyStatusBar.Content = e.CurrentState;
        }

        private void cn_MessageInfo(object sender,
            SqlInfoMessageEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(_connString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "UpDate [Order] set strTabName";
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MessageER.Content += "UpDate";
                }
            }
        }
    }

    public class AccessTab
    {
        public int intTabID { get; set; }
        public string strTabName { get; set; }
    }
    public class Equipment
    {
        public string intGarageRoom { get; set; }
        public string strManufYear { get; set; }
    }
}
