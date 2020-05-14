using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace Firebase
{
    public partial class Form1 : Form
    {
        DataTable dt = new DataTable();
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "RlvtuZFXvYlLWSwTu5sJwhcsU3K1UQsoJNxlIkGt",
            BasePath = "https://awesome-ae82a.firebaseio.com/"
        };
        IFirebaseClient client;
        public Form1()
        {
            InitializeComponent();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);
            if(client == null)
            {
                MessageBox.Show("No Database Found");
            }
            if (client != null)
            {
                MessageBox.Show("Connection is established");
            }
            dt.Columns.Add("ID");
            dt.Columns.Add("NAME");
            dt.Columns.Add("ADDRESS");
            dt.Columns.Add("AGE");
            dataGridView1.DataSource = dt;
        }

        public async void button1_Click(object sender, EventArgs e)
        {
            FirebaseResponse resp = await client.GetTaskAsync("Counter/node");
            Counter_class get = resp.ResultAs<Counter_class>();
            MessageBox.Show(get.cnt);

            var data = new Data
            {
                ID = (Convert.ToInt32(get.cnt) + 1).ToString(),/* textBox1.Text,*/
                NAME = textBox2.Text,
                ADDRESS = textBox3.Text,
                AGE = textBox4.Text,
            };
            SetResponse response = await client.SetTaskAsync("Information/" + data.ID, data);
            Data result = response.ResultAs<Data>();
            MessageBox.Show("Data Inserted");
            var obj = new Counter_class
            {
                cnt = data.ID
            };

            SetResponse response1 = await client.SetTaskAsync("Counter/node",obj);
            
        }

        public void button6_Click(object sender, EventArgs e)
        {
            export();
        }
        public async void export()
        {
            dt.Rows.Clear();
            int i = 0;
            FirebaseResponse resp1 = await client.GetTaskAsync("Counter/node");
            Counter_class obj1 = resp1.ResultAs<Counter_class>();
            int cnt = Convert.ToInt32(obj1.cnt);

            while(true)
            {
                if(i==cnt)
                {
                    break;
                }
                i++;
                try
                {
                    var response = await client.GetTaskAsync("Information");
                    /*Data obj2 = resp2.ResultAs<Data>();
                    DataRow row = dt.NewRow();*/
                    var result = response.ResultAs<Dictionary<string, Data>>();
                    foreach (var item in result)
                    {
                        var value = item.Value;
                        DataRow row = dt.NewRow();
                        row["ID"] = value.ID;
                        row["NAME"] = value.NAME;
                        row["ADDRESS"] = value.ADDRESS;
                        row["AGE"] = value.AGE;
                        dt.Rows.Add(row);
                    }
                }
                catch
                {

                }
            }
            MessageBox.Show("DONE!");

        }

        public async void button4_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.DeleteTaskAsync("Information/"+textBox1.Text);
            MessageBox.Show("Deleted Record of ID : " + textBox1.Text);
        }

        public async void button5_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.DeleteTaskAsync("Information");
            MessageBox.Show("All Elements Deleted / Information node has been deleted");
            

        }

        public async void button3_Click(object sender, EventArgs e)
        {
            var data = new Data
            {
                ID = textBox1.Text,
                NAME = textBox2.Text,
                ADDRESS = textBox3.Text,
                AGE = textBox4.Text
            };
            FirebaseResponse response = await client.UpdateTaskAsync("Information/" + textBox1.Text,data);
            Data result = response.ResultAs<Data>();
            MessageBox.Show("Data Updated At ID : " + result.ID);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.GetTaskAsync("Information/"+textBox1.Text);
            Data obj = response.ResultAs<Data>();
            textBox1.Text = obj.ID;
            textBox2.Text = obj.NAME;
            textBox3.Text = obj.ADDRESS;
            textBox4.Text = obj.AGE;
            MessageBox.Show("Data Retrieved Successfully");

        }
    }
}
